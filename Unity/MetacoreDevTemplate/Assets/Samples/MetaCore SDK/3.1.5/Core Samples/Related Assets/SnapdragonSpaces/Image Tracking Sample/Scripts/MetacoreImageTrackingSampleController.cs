using MixedReality.Toolkit.UX;
using Qualcomm.Snapdragon.Spaces;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Metacore
{
    public class MetacoreImageTrackingSampleController : MetacoreSampleController
    {
        [Serializable]
        public struct TrackableInfo
        {
            public TMP_Text TrackingStatusText;
            public TMP_Text[] PositionTexts;
        }

        public ARTrackedImageManager ARTrackedImageManager;
        public SpacesReferenceImageConfigurator SpacesReferenceImageConfigurator;
        public PressableButton DynamicModeToggle;
        public PressableButton StaticModeToggle;
        public PressableButton AdaptiveModeToggle;
        public TrackableInfo[] TrackableInfos;
        private readonly string _referenceImageName = "Spaces Town";
        private readonly Dictionary<TrackableId, TrackableInfo> _trackedImages = new Dictionary<TrackableId, TrackableInfo>();

        public override void OnEnable()
        {
            base.OnEnable();

#if AR_FOUNDATION_6_0_OR_NEWER
            ARTrackedImageManager.trackablesChanged.AddListener(OnTrackedImagesChanged);
#else
            ARTrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
#endif

            if (SpacesReferenceImageConfigurator.HasReferenceImageTrackingMode(_referenceImageName))
            {
                switch (SpacesReferenceImageConfigurator.GetTrackingModeForReferenceImage(_referenceImageName))
                {
                    case SpacesImageTrackingMode.STATIC:
                        StaticModeToggle.ForceSetToggled(true);
                        break;
                    case SpacesImageTrackingMode.DYNAMIC:
                        DynamicModeToggle.ForceSetToggled(true);
                        break;
                    case SpacesImageTrackingMode.ADAPTIVE:
                        AdaptiveModeToggle.ForceSetToggled(true);
                        break;
                    case SpacesImageTrackingMode.INVALID:
                        Debug.LogWarning($"Invalid tracking mode for reference image: {_referenceImageName}");
                        break;
                }
            }
            else
            {
                Debug.LogWarning($"Could not find reference image: {_referenceImageName} ");
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
#if AR_FOUNDATION_6_0_OR_NEWER
            ARTrackedImageManager.trackablesChanged.RemoveListener(OnTrackedImagesChanged);
#else
            ARTrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
#endif

            foreach (var trackedImage in _trackedImages)
            {
                SpacesReferenceImageConfigurator.StopTrackingImageInstance(_referenceImageName, trackedImage.Key);
            }
        }

#if AR_FOUNDATION_6_0_OR_NEWER
        private void OnTrackedImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
#else
        private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
#endif
        {
            foreach (var trackedImage in args.added)
            {
                if (trackedImage.referenceImage.name == _referenceImageName)
                {
                    _trackedImages.Add(trackedImage.trackableId, TrackableInfos[0]);
                    UpdateTrackedText(trackedImage, TrackableInfos[0]);
                }
            }

            foreach (var trackedImage in args.updated)
            {
                if (_trackedImages.TryGetValue(trackedImage.trackableId, out TrackableInfo info))
                {
                    UpdateTrackedText(trackedImage, info);
                }
            }

            foreach (var trackedImage in args.removed)
            {
#if AR_FOUNDATION_6_0_OR_NEWER
                if (_trackedImages.TryGetValue(trackedImage.Key, out TrackableInfo info))
#else
                if (_trackedImages.TryGetValue(trackedImage.trackableId, out TrackableInfo info))
#endif
                {
                    info.TrackingStatusText.text = "None";
                    info.PositionTexts[0].text = "0.00";
                    info.PositionTexts[1].text = "0.00";
                    info.PositionTexts[2].text = "0.00";

#if AR_FOUNDATION_6_0_OR_NEWER
                    _trackedImages.Remove(trackedImage.Key);
#else
                    _trackedImages.Remove(trackedImage.trackableId);
#endif
                }
            }
        }

        private void UpdateTrackedText(ARTrackedImage trackedImage, TrackableInfo info)
        {
            Vector3 position = trackedImage.transform.position;
            info.TrackingStatusText.text = trackedImage.trackingState.ToString();
            info.PositionTexts[0].text = position.x.ToString("#0.00");
            info.PositionTexts[1].text = position.y.ToString("#0.00");
            info.PositionTexts[2].text = position.z.ToString("#0.00");
        }

        protected override bool CheckSubststem()
        {
            return ARTrackedImageManager.subsystem?.running ?? false;
        }
    }
}