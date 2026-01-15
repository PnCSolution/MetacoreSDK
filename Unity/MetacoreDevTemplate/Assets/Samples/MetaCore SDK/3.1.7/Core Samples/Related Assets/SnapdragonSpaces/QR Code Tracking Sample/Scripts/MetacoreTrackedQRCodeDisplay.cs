using Qualcomm.Snapdragon.Spaces;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Metacore
{
    public class MetacoreTrackedQRCodeDisplay : MonoBehaviour
    {
        public SpacesARMarker ARMarker;
        public TMP_Text TrackableIdText;
        public TMP_Text DataStringText;

        private void Update()
        {
            TrackableIdText.text = ARMarker.trackableId.ToString();
            DataStringText.text = ARMarker.Data;
        }
    }
}

