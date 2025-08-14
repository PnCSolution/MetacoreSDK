using MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MixedReality.Toolkit.Input.XRRayInteractorExtensions;

public static class MetacoreExtensions
{
    #region MRTKRayInteractor
    public static bool TryGetHitInfo(this MRTKRayInteractor rayInteractor, TargetHitDetails targetHitDetails, out Vector3 position, out Quaternion normal)
    {
        bool hasHit = false;
        Vector3 hitPosition = Vector3.zero;
        Vector3 hitNormal = Vector3.zero;

        if (rayInteractor.interactablesSelected.Count > 0)
        {
            hasHit = true;
            hitPosition = targetHitDetails.HitTargetTransform.TransformPoint(targetHitDetails.TargetLocalHitPoint);
            hitNormal = targetHitDetails.HitTargetTransform.TransformDirection(targetHitDetails.TargetLocalHitNormal);
        }
        else
        {
            hasHit = rayInteractor.TryGetHitInfo(out hitPosition, out hitNormal, out int _, out bool _);
        }

        position = hitPosition == Vector3.zero ? rayInteractor.transform.position + rayInteractor.transform.forward : hitPosition;
        normal = hitNormal == Vector3.zero ? Quaternion.identity : Quaternion.LookRotation(hitNormal);

        return hasHit;
    }

    public static bool TryGetHitPosition(this MRTKRayInteractor rayInteractor, TargetHitDetails targetHitDetails, out Vector3 position)
    {
        return TryGetHitInfo(rayInteractor, targetHitDetails, out position, out _);
    }

    public static bool TryGetHitNormal(this MRTKRayInteractor rayInteractor, TargetHitDetails targetHitDetails, out Quaternion normal)
    {
        return TryGetHitInfo(rayInteractor, targetHitDetails, out _, out normal);
    }
    #endregion
}
