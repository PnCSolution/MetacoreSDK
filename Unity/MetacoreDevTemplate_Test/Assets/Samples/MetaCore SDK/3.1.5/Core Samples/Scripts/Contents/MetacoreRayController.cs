using Pnc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metacore
{
    public class MetacoreRayController : MonoBehaviour
    {
        [SerializeField] private List<MetacoreRayInteractor> _metacoreRayInteractors;

        private void Start()
        {
#if !UNITY_EDITOR
            SetRaySettings();
#endif
        }

        private void SetRaySettings()
        {
            var (magnetic, rayDir) = LoadRaySettings();

            foreach (var interactor in _metacoreRayInteractors)
            {
                interactor.sphereCastRadius = magnetic;
                interactor.aimPoseXRotationOffset = rayDir;
            }
        }

        private (float, float) LoadRaySettings()
        {
            List<float> handRayOption = AndroidServiceBridge.Instance().CallHandOption();

            if (handRayOption == null || handRayOption.Count != 2)
            {
                return (0.030f, 10.05f); // (magnetic, rayDir)
            }
            else
            {
                return (handRayOption[0], handRayOption[1]);
            }
        }
    }
}