using Pnc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Pnc.AndroidServiceBridge;


[RequireComponent(typeof(AndroidServiceBridge))]
public class ProcessCameraView : MonoBehaviour
{
    public GameObject go_canvas_Image;
    Texture2D text;
    AndroidServiceBridge asb;

    void Start()
    {
        text = new Texture2D(320, 240, TextureFormat.RGBA32, false);
        asb = this.GetComponent<AndroidServiceBridge>();
        asb.AddAidlService(SERVICE_TYPE_CAMERAPREVIEW);
        asb.eventCameraPreview.AddListener(OnMsgCameraPreviewUpdated);
    }

    void OnMsgCameraPreviewUpdated(string jsonParam)
    {
        if (go_canvas_Image == null)
        {
            return;
        }
        sbyte[] result = asb._aidlPlugin.Call<sbyte[]>("getCameraDatabyte");
        text.SetPixelData(result, 0);
        text.Apply();
        if (text != null)
        {
            go_canvas_Image.GetComponent<RawImage>().texture = text;
        }
    }
}
