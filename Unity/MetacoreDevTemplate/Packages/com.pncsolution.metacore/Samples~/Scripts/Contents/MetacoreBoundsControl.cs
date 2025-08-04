using MixedReality.Toolkit.SpatialManipulation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metacore
{
    public class MetacoreBoundsControl : BoundsControl
    {
        [SerializeField] bool _fixedBounds;
        [SerializeField] Vector3 _fixedLocalScale = Vector3.one;
        [SerializeField] Vector3 _fixedLocalPosition = Vector3.zero;

        private bool _hasSearchedBounds = false;
        private GameObject _boxInstance;

        protected override void Update()
        {
            base.Update();

            if (_fixedBounds && TryFindBoundingBox())
            {
                _boxInstance.transform.localScale = _fixedLocalScale;
                _boxInstance.transform.localPosition = _fixedLocalPosition;
            }
        }

        private bool TryFindBoundingBox()
        {
            if (_hasSearchedBounds == true)
                return true;

            _boxInstance = Util.FindChild(gameObject, "BoundingBoxWithHandles(Clone)", true);
            if (_boxInstance != null)
            {
                _hasSearchedBounds = true;
                return true;
            }

            return false;
        }
    }
}
