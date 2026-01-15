using MixedReality.Toolkit;
using MixedReality.Toolkit.Input;
using MixedReality.Toolkit.SpatialManipulation;
using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using static Microsoft.MixedReality.GraphicsTools.ClippingPrimitive;
using static MixedReality.Toolkit.Input.XRRayInteractorExtensions;

namespace Metacore
{
    [RequireComponent(typeof(TrackedPoseDriverLookup))]
    public class MetacoreHandTracker : MonoBehaviour
    {
        private TrackedPoseDriverLookup _trackedPoseDriverLookup;
        public TrackedPoseDriverLookup TrackedPoseDriverLookup
        {
            get
            {
                if (_trackedPoseDriverLookup == null)
                {
                    _trackedPoseDriverLookup = GetComponent<TrackedPoseDriverLookup>();
                }

                return _trackedPoseDriverLookup;
            }
        }

        private MRTKRayInteractor _leftFarRayInteractor;
        public MRTKRayInteractor LeftFarRayInteractor
        {
            get
            {
                if (TrackedPoseDriverLookup == null)
                    return null;

                if (_leftFarRayInteractor == null)
                {
                    _leftFarRayInteractor = TrackedPoseDriverLookup.LeftHandTrackedPoseDriver.GetComponentInChildren<MRTKRayInteractor>();
                }
                return _leftFarRayInteractor;
            }
        }

        private MRTKRayInteractor _rightFarRayInteractor;
        public MRTKRayInteractor RightFarRayInteractor
        {
            get
            {
                if (TrackedPoseDriverLookup == null)
                    return null;
                if (_rightFarRayInteractor == null)
                {
                    _rightFarRayInteractor = TrackedPoseDriverLookup.RightHandTrackedPoseDriver.GetComponentInChildren<MRTKRayInteractor>();
                }
                return _rightFarRayInteractor;
            }
        }

        public MRTKRayInteractor CurrentFarRayInteractor
        {
            get
            {
                if (_currentTrackedHandedness == Handedness.Left)
                {
                    return LeftFarRayInteractor;
                }
                else if (_currentTrackedHandedness == Handedness.Right)
                {
                    return RightFarRayInteractor;
                }

                return null;
            }
        }

        private PinchInputReader _leftPinchInputReader;
        public PinchInputReader LeftPinchInputReader
        {
            get
            {
                if (_leftPinchInputReader == null)
                {
                    _leftPinchInputReader = Util.FindChild<PinchInputReader>(TrackedPoseDriverLookup.LeftHandTrackedPoseDriver.gameObject, "SelectInputReader");
                }

                return _leftPinchInputReader;
            }
        }

        private PinchInputReader _rightPinchInputReader;
        public PinchInputReader RightPinchInputReader
        {
            get
            {
                if (_rightPinchInputReader == null)
                {
                    _rightPinchInputReader = Util.FindChild<PinchInputReader>(TrackedPoseDriverLookup.RightHandTrackedPoseDriver.gameObject, "SelectInputReader");
                }

                return _rightPinchInputReader;
            }
        }

        private Handedness _trackedHandedness = Handedness.Both;
        public Handedness TrackedHandedness
        {
            get => _trackedHandedness;
            set
            {
                if (_trackedHandedness != value && IsValidHandedness(value))
                {
                    _trackedHandedness = value;
                }
            }
        }

        // Stores currently attached hand if valid (only possible values Left, Right, or None)
        private Handedness _currentTrackedHandedness = Handedness.None;
        public Handedness CurrentTrackedHandedness => _currentTrackedHandedness;

        // Stores controller side to favor if TrackedHandedness is set to both left and right.
        private Handedness _preferredTrackedHandedness = Handedness.Right;
        public Handedness PreferredTrackedHandedness
        {
            get => _preferredTrackedHandedness;
            set
            {
                if ((value == Handedness.Left || value == Handedness.Right)
                    && _preferredTrackedHandedness != value)
                {
                    _preferredTrackedHandedness = value;
                }
            }
        }

        private TargetHitDetails _farRayHitDetails = new TargetHitDetails();
        public TargetHitDetails FarRayHitDetails => _farRayHitDetails;

        private void OnEnable()
        {
            LeftFarRayInteractor?.selectEntered.AddListener(LocateTargetHitPointLeft);
            RightFarRayInteractor?.selectEntered?.AddListener(LocateTargetHitPointRight);
        }

        private void OnDisable()
        {
            LeftFarRayInteractor?.selectEntered.RemoveListener(LocateTargetHitPointLeft);
            RightFarRayInteractor?.selectEntered.RemoveListener(LocateTargetHitPointRight);
        }

        private void Update()
        {
            UpdateTrackedHandedness();
        }

        private static readonly ProfilerMarker UpdateTrackedHandednessPerfMaker =
            new ProfilerMarker("[Metacore] MetacoreHandTracker.UpdateTrackedHandedness");

        private void UpdateTrackedHandedness()
        {
            using (UpdateTrackedHandednessPerfMaker.Auto())
            {
                _currentTrackedHandedness = Handedness.None;
                if (XRSubsystemHelpers.HandsAggregator != null)
                {
                    _currentTrackedHandedness = TrackedHandedness;
                    if ((_currentTrackedHandedness & Handedness.Both) == Handedness.Both)
                    {
                        if (IsHandTracked(PreferredTrackedHandedness))
                        {
                            _currentTrackedHandedness = PreferredTrackedHandedness;
                        }
                        else if (IsHandTracked(PreferredTrackedHandedness.GetOppositeHandedness()))
                        {
                            _currentTrackedHandedness = PreferredTrackedHandedness.GetOppositeHandedness();
                        }
                        else
                        {
                            _currentTrackedHandedness = Handedness.None;
                        }
                    }
                }
            }
        }

        private bool IsValidHandedness(Handedness hand)
        {
            return hand.IsMatch(Handedness.Both) || hand == Handedness.None;
        }

        private bool IsHandTracked(Handedness hand)
        {
            XRNode? node = hand.ToXRNode();
            if (!node.HasValue) { return false; }
            return XRSubsystemHelpers.HandsAggregator != null &&
                   XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.Palm, node.Value, out HandJointPose pose);
        }

        private void LocateTargetHitPointLeft(SelectEnterEventArgs args)
        {
            if (CurrentTrackedHandedness != Handedness.Left)
                return;

            LeftFarRayInteractor.TryLocateTargetHitPoint(args.interactableObject, out _farRayHitDetails);
        }

        private void LocateTargetHitPointRight(SelectEnterEventArgs args)
        {
            if (CurrentTrackedHandedness != Handedness.Right)
                return;

            RightFarRayInteractor.TryLocateTargetHitPoint(args.interactableObject, out _farRayHitDetails);
        }
    }
}
