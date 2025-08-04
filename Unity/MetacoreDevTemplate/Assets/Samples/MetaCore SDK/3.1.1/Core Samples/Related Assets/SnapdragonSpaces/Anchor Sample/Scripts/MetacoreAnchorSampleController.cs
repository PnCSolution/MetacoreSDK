using MixedReality.Toolkit.Input;
using MixedReality.Toolkit.UX;
using Newtonsoft.Json.Bson;
using QCHT.Interactions.Core;
using Qualcomm.Snapdragon.Spaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Metacore
{
    public class MetacoreAnchorSampleController : MetacoreSampleController
    {
        public ARAnchorManager AnchorManager;
        public GameObject GizmoTransparent;
        public GameObject GizmoSurface;
        public GameObject GizmoTrackedAnchor;
        public GameObject GizmoUntrackedAnchor;
        public GameObject GizmoSavedAddition;
        public GameObject GizmoNotSavedAddition;
        public PressableButton SaveNewAnchorsToggle;
        public PressableButton UseSurfacePlacementToggle;
        public TMP_Text NumberOfAnchorsStoredText;

        private readonly List<GameObject> _anchorGizmos = new List<GameObject>();
        private readonly List<GameObject> _sessionGizmos = new List<GameObject>();
        private SpacesAnchorStore _anchorStore;
        private bool _placeAnchorAtRaycastHit;
        private bool _canPlaceAnchorGizmos = true;
        private GameObject _indicatorGizmo;
        private GameObject _transparentGizmo;
        private GameObject _surfaceGizmo;
        private MetacoreHandTracker _handTracker;
        private bool _saveAnchorsToStore => SaveNewAnchorsToggle.IsToggled;
        private bool _useSurfacePlacement => UseSurfacePlacementToggle.IsToggled;

        private readonly string _anchorGizmoName = "AnchorGizmo";
        private readonly string _additionName = "Addition;";

        protected override void Awake()
        {
            base.Awake();
            _anchorStore = FindFirstObjectByType<SpacesAnchorStore>();
            _handTracker = _trackedPoseDriverLookup.GetOrAddComponent<MetacoreHandTracker>();
        }

        public override void Start()
        {
            base.Start();
            if (!SubsystemChecksPassed)
            {
                return;
            }

            _indicatorGizmo = new GameObject("IndicatorGizmo");
            _transparentGizmo = Instantiate(GizmoTransparent, _indicatorGizmo.transform.position, Quaternion.identity, _indicatorGizmo.transform);
            _surfaceGizmo = Instantiate(GizmoSurface, _indicatorGizmo.transform.position, Quaternion.identity, _indicatorGizmo.transform);
            _surfaceGizmo.SetActive(false);
            NumberOfAnchorsStoredText.text = _anchorStore.GetSavedAnchorNames().Length.ToString();
        }

        public override void OnEnable()
        {
            base.OnEnable();
            if (!SubsystemChecksPassed)
            {
                return;
            }

            AnchorManager.anchorsChanged += OnAnchorsChanged;

            if (_handTracker.RightPinchInputReader.SelectAction.action != null)
            {
                _handTracker.RightPinchInputReader.SelectAction.action.performed += OnTriggerAction;
                _handTracker.RightPinchInputReader.SelectAction.action.Enable();
            }

            if (_handTracker.LeftPinchInputReader.SelectAction.action != null)
            {
                _handTracker.LeftPinchInputReader.SelectAction.action.performed += OnTriggerAction;
                _handTracker.LeftPinchInputReader.SelectAction.action.Enable();
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
            if (!SubsystemChecksPassed)
            {
                return;
            }

            AnchorManager.anchorsChanged -= OnAnchorsChanged;

            if (_handTracker.RightPinchInputReader != null && _handTracker.RightPinchInputReader.SelectAction.action != null)
            {
                _handTracker.RightPinchInputReader.SelectAction.action.performed -= OnTriggerAction;
            }

            if (_handTracker.RightPinchInputReader != null && _handTracker.LeftPinchInputReader.SelectAction.action != null)
            {
                _handTracker.LeftPinchInputReader.SelectAction.action.performed -= OnTriggerAction;
            }
        }

        private void Update()
        {
            if (!SubsystemChecksPassed)
            {
                return;
            }

            if (_handTracker.CurrentFarRayInteractor == null)
            {
                _indicatorGizmo.SetActive(false);
            }
            else
            {
                Vector3 targetPosition = _handTracker.CurrentFarRayInteractor.transform.position + _handTracker.CurrentFarRayInteractor.transform.forward;

                _indicatorGizmo.SetActive(true);

                if (_useSurfacePlacement)
                {
                    if (_handTracker.CurrentFarRayInteractor.TryGetHitPosition(_handTracker.FarRayHitDetails, out targetPosition))
                    {
                        _surfaceGizmo.SetActive(true);
                        _transparentGizmo.SetActive(false);
                    }
                    else
                    {
                        _surfaceGizmo.SetActive(false);
                        _transparentGizmo.SetActive(true);
                    }
                }

                _indicatorGizmo.transform.position = targetPosition;
            }
        }

        public void InstantiateGizmos()
        {
            var targetPosition = _indicatorGizmo.transform.position;
            var sessionGizmo = _placeAnchorAtRaycastHit ? Instantiate(GizmoSurface, targetPosition, Quaternion.identity) : Instantiate(GizmoTransparent, targetPosition, Quaternion.identity);
            _sessionGizmos.Add(sessionGizmo);
            var anchorGizmo = new GameObject
            {
                transform =
                {
                    position = targetPosition,
                    rotation = Quaternion.identity
                }
            };
            var anchor = anchorGizmo.AddComponent<ARAnchor>();
            var gizmo = Instantiate(GizmoUntrackedAnchor, anchor.transform);
            gizmo.name = _anchorGizmoName;
            if (_saveAnchorsToStore)
            {
                var addition = Instantiate(GizmoNotSavedAddition, anchor.transform);
                addition.name = _additionName;
                _anchorStore.SaveAnchorWithResult(anchor, result =>
                {
                    NumberOfAnchorsStoredText.text = _anchorStore.GetSavedAnchorNames().Length.ToString();
                    UpdateGizmoSavedAddition(anchor);
                });
            }
        }

        public void LoadAllSavedAnchors()
        {
            _anchorStore.LoadAllSavedAnchors(success =>
            {
                Debug.Log("Load Anchor Success: " + success);
            });
        }

        public void ClearAnchorStore()
        {
            _anchorStore.ClearStore();
            NumberOfAnchorsStoredText.text = _anchorStore.GetSavedAnchorNames().Length.ToString();
        }

        public void DestroyGizmos()
        {
            foreach (var anchorGizmo in _anchorGizmos.ToList())
            {
                Destroy(anchorGizmo);
            }

            foreach (var gizmo in _sessionGizmos.ToList())
            {
                Destroy(gizmo);
            }

            _sessionGizmos.Clear();
        }
        public void OnPointerEnterEvent()
        {
            _canPlaceAnchorGizmos = false;
        }

        public void OnPointerExitEvent()
        {
            _canPlaceAnchorGizmos = true;
        }

        private void OnTriggerAction(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (!_canPlaceAnchorGizmos)
            {
                return;
            }

            InstantiateGizmos();
        }

#if AR_FOUNDATION_6_0_OR_NEWER
        private void OnAnchorsChanged(ARTrackablesChangedEventArgs<ARAnchor> args)
#else
        private void OnAnchorsChanged(ARAnchorsChangedEventArgs args)
#endif
        {
            foreach (var anchor in args.added)
            {
                _anchorGizmos.Add(anchor.gameObject);
            }

            foreach (var anchor in args.updated)
            {
                foreach (Transform child in anchor.transform)
                {
                    var go = child.gameObject;
                    if (go.name == _anchorGizmoName)
                    {
                        go.SetActive(false);
                        Destroy(go);
                    }
                }

                var newGizmo = Instantiate(anchor.trackingState == TrackingState.None ? GizmoUntrackedAnchor : GizmoUntrackedAnchor, anchor.transform);
                newGizmo.name = _anchorGizmoName;

                UpdateGizmoSavedAddition(anchor);
            }

            foreach (var anchor in args.removed)
            {
#if AR_FOUNDATION_6_0_OR_NEWER
                _anchorGizmos.Remove(anchor.Value.gameObject);
#else
                _anchorGizmos.Remove(anchor.gameObject);
#endif
            }
        }

        private void UpdateGizmoSavedAddition(ARAnchor anchor)
        {
            if (_anchorStore.GetSavedAnchorNameFromARAnchor(anchor) != string.Empty)
            {
                foreach (Transform child in anchor.transform)
                {
                    var go = child.gameObject;
                    if (go.name == _additionName)
                    {
                        go.SetActive(false);
                        Destroy(go);
                    }

                    var newAddition = Instantiate(GizmoSavedAddition, anchor.transform);
                    newAddition.name = _additionName;
                }
            }
        }

        protected override bool CheckSubststem()
        {
#if !UNITY_EDITOR
            if (!_baseRuntimeFeature.CheckServicesCameraPermissions())
            {
                Debug.LogWarning("The OpenXR runtime has no camera permissions!");
                return false;
            }
#endif
            return (AnchorManager.subsystem?.running ?? false);
        }
    }
}

