using MixedReality.Toolkit.UX;
using Qualcomm.Snapdragon.Spaces;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Metacore
{
    public class MetacorePlaneDetectionSampleController : MetacoreSampleController
    {
        public PressableButton EnableConvexHullToggle;
        private ARPlaneManager _arPlaneManager;
        private SpacesARPlaneManagerConfig _arPlaneManagerConfig;

        protected override void Awake()
        {
            base.Awake();
            _arPlaneManager = FindFirstObjectByType<ARPlaneManager>();
            _arPlaneManagerConfig = FindFirstObjectByType<SpacesARPlaneManagerConfig>();
        }

        public override void Start()
        {
            base.Start();
            if (_arPlaneManagerConfig != null)
            {
                EnableConvexHullToggle.ForceSetToggled(_arPlaneManagerConfig.ConvexHullEnabled);
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            if (_arPlaneManager != null)
            {
                EnableConvexHullToggle.OnClicked.AddListener(() => OnToggleConvexHull(EnableConvexHullToggle.IsToggled));
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
            if (_arPlaneManager != null)
            {
                EnableConvexHullToggle.OnClicked.RemoveListener(() => OnToggleConvexHull(EnableConvexHullToggle.IsToggled));
            }
        }

        public void OnToggleConvexHull(bool inValue)
        {
            if (_arPlaneManagerConfig != null)
            {
                _arPlaneManagerConfig.ConvexHullEnabled = inValue;
            }
        }

        protected override bool CheckSubststem()
        {
            return _arPlaneManager.subsystem?.running ?? false;
        }
    }
}
