using Qualcomm.Snapdragon.Spaces;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Metacore
{
    public class MetacoreQrCodeTrackingSampleController : MetacoreSampleController
    {
        public SpacesQrCodeManager ArQrCodeManager;
        public TMP_Text MarkerWidthText;
        public TMP_Text MarkerHeightText;

        public TMP_Text MinQrCodeVersionText;
        public TMP_Text MaxQrCodeVersionText;

        public override void OnEnable()
        {
            base.OnEnable();
            UpdateQrCodeManagerUI();
        }

        private void UpdateQrCodeManagerUI()
        {
            MarkerWidthText.text = ArQrCodeManager.markerSize.x.ToString();
            MarkerHeightText.text = ArQrCodeManager.markerSize.y.ToString();
            MinQrCodeVersionText.text = ArQrCodeManager.minQrVersion.ToString();
            MaxQrCodeVersionText.text = ArQrCodeManager.maxQrVersion.ToString();
        }

        protected override bool CheckSubststem()
        {
            return ArQrCodeManager.subsystem?.running ?? false;
        }
    }
}