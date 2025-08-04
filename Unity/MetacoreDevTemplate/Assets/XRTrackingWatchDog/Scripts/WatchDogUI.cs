using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class WatchDogUI : MonoBehaviour
{

    #region field
    [HideInInspector]
    public bool isYes = true;
    public Image YesBtn;
    public Image NoBtn;
    public Sprite[] BtnSP;

    #endregion


    #region unitymono
  
    #endregion

    #region method
    public void ToggledBtn()
    {
        isYes = !isYes;
        if (isYes)
        {
            YesBtn.sprite = BtnSP[1];
            NoBtn.sprite = BtnSP[0];
        }
        else if(!isYes)
        {
            YesBtn.sprite = BtnSP[0];
            NoBtn.sprite = BtnSP[1];
        }
    }
    #endregion
}
