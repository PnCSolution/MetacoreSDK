using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeKeyEvent : MonoBehaviour
{

    #region field
    XRTrackingWatchdog watchdog;
    #endregion



    #region unitymono
    void Start()
    {
        watchdog = FindObjectOfType<XRTrackingWatchdog>();
#if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            using (AndroidJavaClass plugin = new AndroidJavaClass("com.pnc.keyevent.keyEventSender"))
            {
                plugin.CallStatic("registerVolumeCallback", currentActivity);
            }
        }
#endif
    }
    #endregion


    #region method
    public void OnVolumeKeyPressed(string arg)
    {
        switch (arg)
        {
            case "Up":
                watchdog?.PressedUp();
                break;
            case "Down":
                watchdog?.PressedDown();
                break;
        }
    }
    #endregion
}
