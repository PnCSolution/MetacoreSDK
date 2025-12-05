using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using PNC;

public class SttCallback: MonoBehaviour
{
    string TAG = "[class]CallbackTest :: ";

    //[SerializeField]
    //private PNCPopUpSliderManager pncPopUpSliderManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void print_debug_0(){
        Debug.Log("debug log 0...");
    }

    public void print_debug_1(){
        Debug.Log("debug log 1...");
    }


    public void ParseVolumeMax(){
        Debug.Log(TAG+"[method] ParseVolumeMax...");

        //int tmpVolumeValue = (int)pncPopUpSliderManager.volumeSliderInfo.mrtkSlider.Value;

        //PNCAndroidServiceBridge.Instance.SetVolumeOnAndroid(PNCAndroidServiceBridge.Instance.GetMaxVolume());
        //pncPopUpSliderManager.volumeSliderInfo.mrtkSlider.Value = 15;

    }


    public void ParseVolumeMin(){
        Debug.Log(TAG+"[method] ParseVolumeMin...");

        //int tmpVolumeValue = (int)pncPopUpSliderManager.volumeSliderInfo.mrtkSlider.Value;
        
        //PNCAndroidServiceBridge.Instance.SetVolumeOnAndroid(0);
        //pncPopUpSliderManager.volumeSliderInfo.mrtkSlider.Value = 0;

    }


    public void ParseVolumeUp()
    {
        //int tmpVolumeValue = PNCAndroidServiceBridge.Instance.GetCurrentVolumeOnAndroid();

        Debug.Log(TAG+"[method] ParseVolumeUp...");

        //int tmpVolumeValue = (int)pncPopUpSliderManager.volumeSliderInfo.mrtkSlider.Value;

        //PNCAndroidServiceBridge.Instance.SetVolumeOnAndroid(tmpVolumeValue + 1);
        //pncPopUpSliderManager.volumeSliderInfo.mrtkSlider.Value = tmpVolumeValue + 1;

        //PNCSoundEffectManager.Instance.PlaySpeechEffectSound();
    }

    public void ParseVolumeDown() 
    {
        Debug.Log(TAG+"[method] ParseVolumeDown...");
        //int tmpVolumeValue = PNCAndroidServiceBridge.Instance.GetCurrentVolumeOnAndroid();
        //int tmpVolumeValue = (int)pncPopUpSliderManager.volumeSliderInfo.mrtkSlider.Value;

        //PNCAndroidServiceBridge.Instance.SetVolumeOnAndroid(tmpVolumeValue  - 1);
        //pncPopUpSliderManager.volumeSliderInfo.mrtkSlider.Value = tmpVolumeValue - 1;

        //PNCSoundEffectManager.Instance.PlaySpeechEffectSound();
    }


    public void ParseBrightnessMax(){
        Debug.Log(TAG+"[method] ParseBrightnessMax...");

        //int tmpBrightnessValue = (int)pncPopUpSliderManager.brightnessSliderInfo.mrtkSlider.Value;

        //pncPopUpSliderManager.brightnessSliderInfo.mrtkSlider.Value = 255;
        //PNCAndroidServiceBridge.Instance.SetBrightnessOnAndroid(255);

    }


    public void ParseBrightnessMin(){
        Debug.Log(TAG+"[method] ParseBrightnessMin...");

        //int tmpBrightnessValue = (int)pncPopUpSliderManager.brightnessSliderInfo.mrtkSlider.Value;

        //pncPopUpSliderManager.brightnessSliderInfo.mrtkSlider.Value = 2;
        //PNCAndroidServiceBridge.Instance.SetBrightnessOnAndroid(2);

    }


    public void ParseBrightnessUp()
    {
        Debug.Log(TAG+"[method] ParseBrightnessUp...");
        //int tmpBrightnessValue = PNCAndroidServiceBridge.Instance.GetCurrentBrightnessOnAndroid();
        //int tmpBrightnessValue = (int)pncPopUpSliderManager.brightnessSliderInfo.mrtkSlider.Value;

        //if(tmpBrightnessValue > 150)
        //{
        //    pncPopUpSliderManager.brightnessSliderInfo.mrtkSlider.Value = 255;
        //}
        //else if(tmpBrightnessValue <= 150 && tmpBrightnessValue > 86)
        //{
        //    pncPopUpSliderManager.brightnessSliderInfo.mrtkSlider.Value = 255;
        //}
        //else if (tmpBrightnessValue <= 86 && tmpBrightnessValue > 54)
        //{
        //    pncPopUpSliderManager.brightnessSliderInfo.mrtkSlider.Value = 150;
        //}
        //else if (tmpBrightnessValue <= 54 && tmpBrightnessValue > 33)
        //{
        //    pncPopUpSliderManager.brightnessSliderInfo.mrtkSlider.Value = 86;
        //}
        //else if (tmpBrightnessValue <= 33 && tmpBrightnessValue > 22)
        //{
        //    pncPopUpSliderManager.brightnessSliderInfo.mrtkSlider.Value = 54;
        //}
        //else if (tmpBrightnessValue <= 22 && tmpBrightnessValue > 15)
        //{
        //    pncPopUpSliderManager.brightnessSliderInfo.mrtkSlider.Value = 33;
        //}
        //else if (tmpBrightnessValue <= 15 && tmpBrightnessValue > 10)
        //{
        //    pncPopUpSliderManager.brightnessSliderInfo.mrtkSlider.Value = 22;
        //}
        //else if (tmpBrightnessValue <= 10 && tmpBrightnessValue > 8)
        //{
        //    pncPopUpSliderManager.brightnessSliderInfo.mrtkSlider.Value = 15;
        //}
        //else if (tmpBrightnessValue <= 8 && tmpBrightnessValue > 5)
        //{
        //    pncPopUpSliderManager.brightnessSliderInfo.mrtkSlider.Value = 10;
        //}
        //else if (tmpBrightnessValue <= 5 && tmpBrightnessValue > 2)
        //{
        //    pncPopUpSliderManager.brightnessSliderInfo.mrtkSlider.Value = 8;
        //}
        //else if (tmpBrightnessValue <= 2 && tmpBrightnessValue > 0)
        //{
        //    pncPopUpSliderManager.brightnessSliderInfo.mrtkSlider.Value = 5;
        //}
        ////PNCAndroidServiceBridge.Instance.SetBrightnessOnAndroid(tmpBrightnessValue + 25);

        //PNCSoundEffectManager.Instance.PlaySpeechEffectSound();
    }

    public void ParseBrightnessDown()
    {
        
        Debug.Log(TAG+"[method] ParseBrightnessDown...");
        //int tmpBrightnessValue = PNCAndroidServiceBridge.Instance.GetCurrentBrightnessOnAndroid();
        //int tmpBrightnessValue = (int)pncPopUpSliderManager.brightnessSliderInfo.mrtkSlider.Value;
        
        //if(tmpBrightnessValue > 150)
        //{
        //    pncPopUpSliderManager.brightnessSliderInfo.mrtkSlider.Value = 86;
        //}
        //else if(tmpBrightnessValue <= 150 && tmpBrightnessValue > 86)
        //{
        //    pncPopUpSliderManager.brightnessSliderInfo.mrtkSlider.Value = 54;
        //}
        //else if (tmpBrightnessValue <= 86 && tmpBrightnessValue > 54)
        //{
        //    pncPopUpSliderManager.brightnessSliderInfo.mrtkSlider.Value = 33;
        //}
        //else if (tmpBrightnessValue <= 54 && tmpBrightnessValue > 33)
        //{
        //    pncPopUpSliderManager.brightnessSliderInfo.mrtkSlider.Value = 22;
        //}
        //else if (tmpBrightnessValue <= 33 && tmpBrightnessValue > 22)
        //{
        //    pncPopUpSliderManager.brightnessSliderInfo.mrtkSlider.Value = 15;
        //}
        //else if (tmpBrightnessValue <= 22 && tmpBrightnessValue > 15)
        //{
        //    pncPopUpSliderManager.brightnessSliderInfo.mrtkSlider.Value = 10;
        //}
        //else if (tmpBrightnessValue <= 15 && tmpBrightnessValue > 10)
        //{
        //    pncPopUpSliderManager.brightnessSliderInfo.mrtkSlider.Value = 8;
        //}
        //else if (tmpBrightnessValue <= 10 && tmpBrightnessValue > 8)
        //{
        //    pncPopUpSliderManager.brightnessSliderInfo.mrtkSlider.Value = 5;
        //}
        //else if (tmpBrightnessValue <= 8 && tmpBrightnessValue > 5)
        //{
        //    pncPopUpSliderManager.brightnessSliderInfo.mrtkSlider.Value = 2;
        //}
        //else if (tmpBrightnessValue <= 5 && tmpBrightnessValue > 2)
        //{
        //    pncPopUpSliderManager.brightnessSliderInfo.mrtkSlider.Value = 0;
        //}
        //else if (tmpBrightnessValue <= 2 && tmpBrightnessValue > 0)
        //{
        //    pncPopUpSliderManager.brightnessSliderInfo.mrtkSlider.Value = 0;
        //}

        //PNCSoundEffectManager.Instance.PlaySpeechEffectSound();
    }

}
