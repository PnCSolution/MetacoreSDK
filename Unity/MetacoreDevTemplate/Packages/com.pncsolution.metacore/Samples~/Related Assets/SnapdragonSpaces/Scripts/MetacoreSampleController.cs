using MixedReality.Toolkit;
using MixedReality.Toolkit.Input;
using MixedReality.Toolkit.Input.Simulation;
using Qualcomm.Snapdragon.Spaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.OpenXR;

namespace Metacore
{
    public class MetacoreSampleController : MonoBehaviour
    {
        public delegate void HandTriggered();

        public bool RunEditorForce = false;
        public bool RunSubsystemChecks = true;
        public List<GameObject> ContentOnPassed;
        public List<GameObject> ContentOnFailed;

        protected bool _isPassthoroughOn { get; private set; }
        protected bool SubsystemChecksPassed;

        protected BaseRuntimeFeature _baseRuntimeFeature { get; private set; }
        protected TrackedPoseDriverLookup _trackedPoseDriverLookup;

        protected virtual void Awake()
        {
            _trackedPoseDriverLookup = ComponentCache<TrackedPoseDriverLookup>.FindFirstActiveInstance();
        }

        public virtual void Start()
        {
            foreach (var content in ContentOnPassed)
            {
                content.SetActive(SubsystemChecksPassed);
            }

            foreach (var content in ContentOnFailed)
            {
                content.SetActive(!SubsystemChecksPassed);
            }

            if (!FeatureUseCheckUtility.IsFeatureUseable(_baseRuntimeFeature))
            {
                return;
            }

            if (!_baseRuntimeFeature.IsPassthroughSupported())
            {
                return;
            }

            _isPassthoroughOn = _baseRuntimeFeature.GetPassthroughEnabled();
            _baseRuntimeFeature.SetPassthroughEnabled(_isPassthoroughOn);
        }

        public virtual void OnEnable()
        {
            _baseRuntimeFeature = OpenXRSettings.Instance.GetFeature<BaseRuntimeFeature>();
            SubsystemChecksPassed = (FeatureUseCheckUtility.IsFeatureEnabled(_baseRuntimeFeature) && GetSubsystemCheck()) || RunEditorForce;
        }

        public virtual void OnDisable()
        {

        }

        protected bool GetSubsystemCheck()
        {
            return !RunSubsystemChecks || CheckSubststem();
        }

        protected virtual bool CheckSubststem()
        {
            return false;
        }
    }
}

