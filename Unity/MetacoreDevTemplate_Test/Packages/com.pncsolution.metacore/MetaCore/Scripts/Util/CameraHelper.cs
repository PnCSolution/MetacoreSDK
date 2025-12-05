using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

public static class CameraHelper
{
    // 이거 꼭 필요한건가요. unsafe를 꼭 써야 하는거면 unsafe 코드를 모듈로 옮기고, 결과값만 받게 하는게 조을거 같아요
    public static unsafe Texture2D UpdateCameraTexture(Texture2D _cameraTexture, XRCpuImage image)
    {
        var format = TextureFormat.RGBA32;


        var downsamplingFactor = Mathf.CeilToInt(image.width / 1632);//프레임 개선을 위해 3264의 반인 1632로 나누어줍니다.
        var outputDimensions = image.dimensions / downsamplingFactor;

        if (_cameraTexture == null || _cameraTexture.width != outputDimensions.x || _cameraTexture.height != outputDimensions.y)
        {
            _cameraTexture = new Texture2D(outputDimensions.x, outputDimensions.y, format, false);
        }

        var rawTextureData = _cameraTexture.GetRawTextureData<byte>();
        var rawTexturePtr = new IntPtr(rawTextureData.GetUnsafePtr());


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


        _cameraTexture.Apply();
        return _cameraTexture;
    }
}
