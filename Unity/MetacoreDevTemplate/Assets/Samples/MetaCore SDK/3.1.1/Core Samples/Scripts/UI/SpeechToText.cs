using MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechToText : MonoBehaviour
{
    [SerializeField] ToggleCollection ToggleCollection;

    public void SetToggled(int idx)
    {
        ToggleCollection.SetSelection(idx, true);
    }
}
