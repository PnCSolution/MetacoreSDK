using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Metacore
{
    public class MetacoreHitTestingSampleController : MetacoreSampleController
    {
        public GameObject HitIndicator;
        public GameObject NoHitIndicator;
        private Vector3 _desiredPosition;
        private Quaternion _desiredRotation;
        private GameObject _activeIndicator;
        private ARMeshManager _arMeshManager;
        private MetacoreHandTracker _handTracker;

        protected override void Awake()
        {
            base.Awake();
            _arMeshManager = FindFirstObjectByType<ARMeshManager>();
            _handTracker = _trackedPoseDriverLookup.GetOrAddComponent<MetacoreHandTracker>();
        }

        public override void Start()
        {
            base.Start();
            if (!SubsystemChecksPassed)
            {
                return;
            }

            _activeIndicator = NoHitIndicator;
            _activeIndicator.SetActive(true);
        }

        private void Update()
        {
            if (!SubsystemChecksPassed)
            {
                return;
            }

            CastRay();
            _activeIndicator.transform.position = _desiredPosition;
            _activeIndicator.transform.rotation = _desiredRotation;
        }

        public void CastRay()
        {
            if (_handTracker.CurrentFarRayInteractor == null)
            {
                HitIndicator.SetActive(false);
                NoHitIndicator.SetActive(false);
            }
            else
            {
                if (_handTracker.CurrentFarRayInteractor.TryGetHitInfo(_handTracker.FarRayHitDetails, out _desiredPosition, out _desiredRotation))
                {
                    HitIndicator.SetActive(true);
                    NoHitIndicator.SetActive(false);

                    _activeIndicator = HitIndicator;
                }
                else
                {
                    HitIndicator.SetActive(false);
                    NoHitIndicator.SetActive(true);

                    _activeIndicator = NoHitIndicator;
                }
            }
        }

        protected override bool CheckSubststem()
        {
            return _arMeshManager.subsystem?.running ?? false;
        }
    }
}

