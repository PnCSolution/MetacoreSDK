using MixedReality.Toolkit;
using MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metacore
{
    public class MetacoreRayInteractor : MRTKRayInteractor
    {
        [Range(-40f, 40f)] public float aimPoseXRotationOffset = 15f;

        protected override void Update()
        {
            base.Update();

            if (AimPoseSource != null && AimPoseSource.TryGetPose(out Pose aimPose))
            {
                Quaternion aimCustom = aimPose.rotation;
                aimCustom.eulerAngles = new Vector3(aimCustom.eulerAngles.x + aimPoseXRotationOffset, aimCustom.eulerAngles.y, aimCustom.eulerAngles.z);
                transform.SetPositionAndRotation(aimPose.position, aimCustom);
            }
        }
    }
}