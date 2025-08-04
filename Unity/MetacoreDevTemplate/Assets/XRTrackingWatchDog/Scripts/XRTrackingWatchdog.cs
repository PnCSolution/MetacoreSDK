using MixedReality.Toolkit.Subsystems;
using MixedReality.Toolkit;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Hands;

public class XRTrackingWatchdog : MonoBehaviour
{

    #region field
    [Header("Settings")]
    public float freezeThresholdSeconds = 10f;

    [Header("Status")]
    public bool isHeadFrozen = false;
    private Transform headTransform;
    private Vector3 lastHeadPos;
    private Quaternion lastHeadRot;
    private float lastHeadUpdateTime;

    [Header("WatchDog UI")]
    public GameObject watchDogUI;
    GameObject UIObject = null;
    #endregion


    #region unitymono

    void Start()
    {
        // Get main camera transform
        headTransform = Camera.main?.transform;
        if (headTransform == null)
        {
            Debug.LogError("[Watchdog] Main Camera not found.");
            enabled = false;
            return;
        }

        lastHeadPos = headTransform.position;
        lastHeadRot = headTransform.rotation;
        lastHeadUpdateTime = Time.time;


    }

    void Update()
    {
        // === HEAD TRACKING CHECK (Every frame) ===
        if (!ApproximatelyEqual(headTransform.position, lastHeadPos) ||
            !ApproximatelyEqual(headTransform.rotation, lastHeadRot))
        {
            lastHeadPos = headTransform.position;
            lastHeadRot = headTransform.rotation;
            lastHeadUpdateTime = Time.time;

            if (isHeadFrozen)
            {
                Debug.Log("[Watchdog] Headset started moving again.");
                isHeadFrozen = false;
                OnOffUI(isHeadFrozen);
            }
        }
        else if (Time.time - lastHeadUpdateTime > freezeThresholdSeconds)
        {
            lastHeadUpdateTime = Time.time;
            Debug.LogWarning($"[Watchdog] Headset has been stationary for {freezeThresholdSeconds} seconds.");
            isHeadFrozen = true;
            OnOffUI(isHeadFrozen);
        }

      
    }

    #endregion


    #region method


    void OnOffUI(bool _isOn)
    {
        if (_isOn && UIObject == null)
        {
            UIObject = Instantiate(watchDogUI, Camera.main.transform);
            UIObject.transform.localPosition = new Vector3(0, 0, .5f); 
            UIObject.SetActive(true);
        }
        else if (!_isOn)
        {
            Destroy(UIObject);
            UIObject = null;
        }

    }

    public void PressedUp()
    {
        if (UIObject != null)
        {
            UIObject.GetComponent<WatchDogUI>().ToggledBtn();
        }
    }

    public void PressedDown()
    {
        if (UIObject != null)
        {
            bool isYes = UIObject.GetComponent<WatchDogUI>().isYes;

            if (isYes)
            {
                reboot();
            }
            else if(!isYes)
            {
                OnOffUI(false);
            }
        }
    }
    // === Vector3 comparison with 5 decimal places precision ===
    private bool ApproximatelyEqual(Vector3 a, Vector3 b)
    {
        return Mathf.RoundToInt(a.x * 100000) == Mathf.RoundToInt(b.x * 100000) &&
               Mathf.RoundToInt(a.y * 100000) == Mathf.RoundToInt(b.y * 100000) &&
               Mathf.RoundToInt(a.z * 100000) == Mathf.RoundToInt(b.z * 100000);
    }

    // === Quaternion comparison with 5 decimal places precision ===
    private bool ApproximatelyEqual(Quaternion a, Quaternion b)
    {
        return Mathf.RoundToInt(a.x * 100000) == Mathf.RoundToInt(b.x * 100000) &&
               Mathf.RoundToInt(a.y * 100000) == Mathf.RoundToInt(b.y * 100000) &&
               Mathf.RoundToInt(a.z * 100000) == Mathf.RoundToInt(b.z * 100000) &&
               Mathf.RoundToInt(a.w * 100000) == Mathf.RoundToInt(b.w * 100000);
    }

    public void reboot()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                    using (AndroidJavaObject powerManager = unityActivity.Call<AndroidJavaObject>("getSystemService", "power"))
                    {
                        powerManager.Call("reboot", null); 
                    }
                }
            }
            catch (AndroidJavaException e)
            {
                Debug.LogError($"Reboot failed: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning("RebootDevice can only be called on Android.");
        }
    }
    #endregion
}
