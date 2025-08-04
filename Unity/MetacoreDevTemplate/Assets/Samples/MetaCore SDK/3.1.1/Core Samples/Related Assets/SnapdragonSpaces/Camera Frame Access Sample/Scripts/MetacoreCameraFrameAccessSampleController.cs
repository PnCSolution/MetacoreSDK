using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Metacore
{
    public class MetacoreCameraFrameAccessSampleController : MetacoreSampleController
    {
        [Header("Camera Feed")]
        public RawImage CameraRawImage;
        public bool RenderUsingYUVPlanes;

        [Header("GUI Bindings")]
        public TMP_Text[] ResolutionTexts;
        public TMP_Text[] FocalLengthTexts;
        public TMP_Text[] PrincipalPointTexts;
        public TMP_Text ConfigNotFoundText;

        private NativeArray<XRCameraConfiguration> _cameraConfigs;
        private ARCameraManager _cameraManager;
        private Texture2D _cameraTexture;
        private float _defaultAspectRatio;
        private bool _configurationFound;
        private bool _feedPaused;
        private XRCameraIntrinsics _intrinsics;
        private XRCpuImage _lastCpuImage;
        private Vector2 _maxTextureSize;

        private byte[] _rgbBuffer;

        protected override void Awake()
        {
            base.Awake();
            _cameraManager = FindFirstObjectByType<ARCameraManager>();
        }

        public override void Start()
        {
            base.Start();

            if (!SubsystemChecksPassed)
            {
                return;
            }

            _configurationFound = FindSupportedConfiguration();
            if (!_configurationFound)
            {
                OnConfigurationNotFound();
                return;
            }

            _cameraManager.frameReceived += OnFrameReceived;

            _maxTextureSize = CameraRawImage.rectTransform.sizeDelta;
            _defaultAspectRatio = _maxTextureSize.x / _maxTextureSize.y;
        }

        private void OnFrameReceived(ARCameraFrameEventArgs args)
        {
            if (_feedPaused)
            {
                return;
            }

            if (!_cameraManager.TryAcquireLatestCpuImage(out _lastCpuImage))
            {
                return;
            }

            UpdateCameraTexture(_lastCpuImage, RenderUsingYUVPlanes);
            // Update intrinsics on every frame, as intrinsics can change over time
            UpdateCameraIntrinsics();
        }

        private void UpdateCameraIntrinsics()
        {
            if (!_cameraManager.TryGetIntrinsics(out _intrinsics))
            {
                Debug.Log("Failed to acquire camera intrinsics.");
                return;
            }

            ResolutionTexts[0].text = _intrinsics.resolution.x.ToString();
            ResolutionTexts[1].text = _intrinsics.resolution.y.ToString();
            FocalLengthTexts[0].text = _intrinsics.focalLength.x.ToString("#0.00");
            FocalLengthTexts[1].text = _intrinsics.focalLength.y.ToString("#0.00");
            PrincipalPointTexts[0].text = _intrinsics.principalPoint.x.ToString("#0.00");
            PrincipalPointTexts[1].text = _intrinsics.principalPoint.y.ToString("#0.00");
        }

        private unsafe void UpdateCameraTexture(XRCpuImage image, bool convertYuvManually)
        {
            var format = TextureFormat.RGBA32;

            // Down-sample to < 640px width for devices with large sensors
            // When converting the image manually, use native resolution
            var downsamplingFactor = Mathf.CeilToInt(image.width / 1632);
            var outputDimensions = convertYuvManually ? image.dimensions : image.dimensions / downsamplingFactor;

            if (_cameraTexture == null || _cameraTexture.width != outputDimensions.x || _cameraTexture.height != outputDimensions.y)
            {
                _cameraTexture = new Texture2D(outputDimensions.x, outputDimensions.y, format, false);
                ResizeCameraFeed(outputDimensions);
            }

            var rawTextureData = _cameraTexture.GetRawTextureData<byte>();
            var rawTexturePtr = new IntPtr(rawTextureData.GetUnsafePtr());

            if (convertYuvManually)
            {
                switch (image.planeCount)
                {
                    // YUYV format - 1 plane (YUYV)
                    case 1:
                        ConvertYuyvImageIntoBuffer(image, rawTexturePtr, format);
                        break;
                    // YUV format - 2 planes (Y and UV)
                    case 2:
                        ConvertYuvImageIntoBuffer(image, rawTexturePtr, format);
                        break;
                }
            }
            else
            {
                var conversionParams = new XRCpuImage.ConversionParams(image, format);
                try
                {
                    conversionParams.inputRect = new RectInt(0, 0, image.width, image.height);
                    conversionParams.outputDimensions = outputDimensions;
                    image.Convert(conversionParams, rawTexturePtr, rawTextureData.Length);
                }
                finally
                {
                    image.Dispose();
                }
            }

            _cameraTexture.Apply();
            CameraRawImage.texture = _cameraTexture;

        }

        private void ConvertYuyvImageIntoBuffer(XRCpuImage image, IntPtr targetBuffer, TextureFormat format)
        {
            var bufferSize = image.height * image.width * (format == TextureFormat.RGB24 ? 3 : 4);

            if (_rgbBuffer == null || _rgbBuffer.Length != bufferSize)
            {
                _rgbBuffer = new byte[bufferSize];
            }

            var yuyvPlane = image.GetPlane(0);

            for (int row = 0; row < image.height; row++)
            {
                for (int col = 0; col < image.width; col++)
                {
                    var y = yuyvPlane.data[row * yuyvPlane.rowStride + col * 2];

                    var offset = row * yuyvPlane.rowStride + (col / 2) * yuyvPlane.pixelStride;
                    sbyte u = (sbyte)(yuyvPlane.data[offset + 1] - 128);
                    sbyte v = (sbyte)(yuyvPlane.data[offset + 3] - 128);

                    // YUV NV12 to RGB conversion
                    // https://en.wikipedia.org/wiki/YUV#Y%E2%80%B2UV420sp_(NV21)_to_RGB_conversion_(Android)
                    var r = y + (1.370705f * v);
                    var g = y - (0.698001f * v) - (0.337633f * u);
                    var b = y + (1.732446f * u);

                    r = r > 255 ? 255 : r < 0 ? 0 : r;
                    g = g > 255 ? 255 : g < 0 ? 0 : g;
                    b = b > 255 ? 255 : b < 0 ? 0 : b;

                    int pixelIndex = ((image.height - row - 1) * image.width) + col;

                    switch (format)
                    {
                        case TextureFormat.RGB24:
                            _rgbBuffer[3 * pixelIndex] = (byte)r;
                            _rgbBuffer[(3 * pixelIndex) + 1] = (byte)g;
                            _rgbBuffer[(3 * pixelIndex) + 2] = (byte)b;
                            break;
                        case TextureFormat.RGBA32:
                            _rgbBuffer[4 * pixelIndex] = (byte)r;
                            _rgbBuffer[(4 * pixelIndex) + 1] = (byte)g;
                            _rgbBuffer[(4 * pixelIndex) + 2] = (byte)b;
                            _rgbBuffer[(4 * pixelIndex) + 3] = 255;
                            break;
                        case TextureFormat.BGRA32:
                            _rgbBuffer[4 * pixelIndex] = (byte)b;
                            _rgbBuffer[(4 * pixelIndex) + 1] = (byte)g;
                            _rgbBuffer[(4 * pixelIndex) + 2] = (byte)r;
                            _rgbBuffer[(4 * pixelIndex) + 3] = 255;
                            break;
                    }
                }
            }
            Marshal.Copy(_rgbBuffer, 0, targetBuffer, bufferSize);
        }

        private void ConvertYuvImageIntoBuffer(XRCpuImage image, IntPtr targetBuffer, TextureFormat format)
        {
            var bufferSize = image.height * image.width * (format == TextureFormat.RGB24 ? 3 : 4);

            if (_rgbBuffer == null || _rgbBuffer.Length != bufferSize)
            {
                _rgbBuffer = new byte[bufferSize];
            }

            var yPlane = image.GetPlane(0);
            var uvPlane = image.GetPlane(1);

            for (int row = 0; row < image.height; row++)
            {
                for (int col = 0; col < image.width; col++)
                {
                    var y = yPlane.data[row * yPlane.rowStride + col * yPlane.pixelStride];

                    var offset = (row / 2) * uvPlane.rowStride + (col / 2) * uvPlane.pixelStride;
                    sbyte u = (sbyte)(uvPlane.data[offset] - 128);
                    sbyte v = (sbyte)(uvPlane.data[offset + 1] - 128);

                    // YUV NV12 to RGB conversion
                    // https://en.wikipedia.org/wiki/YUV#Y%E2%80%B2UV420sp_(NV21)_to_RGB_conversion_(Android)
                    var r = y + (1.370705f * v);
                    var g = y - (0.698001f * v) - (0.337633f * u);
                    var b = y + (1.732446f * u);

                    r = r > 255 ? 255 : r < 0 ? 0 : r;
                    g = g > 255 ? 255 : g < 0 ? 0 : g;
                    b = b > 255 ? 255 : b < 0 ? 0 : b;

                    int pixelIndex = ((image.height - row - 1) * image.width) + col;

                    switch (format)
                    {
                        case TextureFormat.RGB24:
                            _rgbBuffer[3 * pixelIndex] = (byte)r;
                            _rgbBuffer[(3 * pixelIndex) + 1] = (byte)g;
                            _rgbBuffer[(3 * pixelIndex) + 2] = (byte)b;
                            break;
                        case TextureFormat.RGBA32:
                            _rgbBuffer[4 * pixelIndex] = (byte)r;
                            _rgbBuffer[(4 * pixelIndex) + 1] = (byte)g;
                            _rgbBuffer[(4 * pixelIndex) + 2] = (byte)b;
                            _rgbBuffer[(4 * pixelIndex) + 3] = 255;
                            break;
                        case TextureFormat.BGRA32:
                            _rgbBuffer[4 * pixelIndex] = (byte)b;
                            _rgbBuffer[(4 * pixelIndex) + 1] = (byte)g;
                            _rgbBuffer[(4 * pixelIndex) + 2] = (byte)r;
                            _rgbBuffer[(4 * pixelIndex) + 3] = 255;
                            break;
                    }
                }
            }
            Marshal.Copy(_rgbBuffer, 0, targetBuffer, bufferSize);
        }

        private void ResizeCameraFeed(Vector2Int outputDimensions)
        {
            var outputAspectRatio = outputDimensions.x / (float)outputDimensions.y;
            if (outputAspectRatio > _defaultAspectRatio)
            {
                CameraRawImage.rectTransform.sizeDelta = new Vector2(_maxTextureSize.x, _maxTextureSize.x / outputAspectRatio);
            }
            else
            {
                CameraRawImage.rectTransform.sizeDelta = new Vector2(_maxTextureSize.y * outputAspectRatio, _maxTextureSize.y);
            }
        }

        private void OnConfigurationNotFound()
        {
            foreach (var content in ContentOnPassed)
            {
                content.SetActive(false);
            }

            foreach (var content in ContentOnFailed)
            {
                content.SetActive(true);
            }

            ConfigNotFoundText.text = "Could not find supported frame configuration.";
        }

        private bool FindSupportedConfiguration()
        {
            _cameraConfigs = _cameraManager.GetConfigurations(Allocator.Persistent);
            return _cameraConfigs.Length > 0;
        }

        public void OnPausePress()
        {
            _feedPaused = true;
        }

        public void OnResumePress()
        {
            _feedPaused = false;
        }

        protected override bool CheckSubststem()
        {
            return _cameraManager.subsystem?.running ?? false;
        }
    }
}

