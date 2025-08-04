using UnityEngine;
using System;
using kr.co.pncsolution.Stt;

public class ReceiveTextResultCallback: AndroidJavaProxy{
    string TAG = "ReceiveTextResultCallback :: ";

    SttComponent _stt_component;

    public ReceiveTextResultCallback(SttComponent input): base(
        "kr.co.pncsolution.vivokalibrary.vivoka.callbacklisteners.SendResultText"
    ){
        _stt_component = input;
    }

    public void onSend(string input_string_data){
        Debug.Log(TAG+"input string = " + input_string_data);

        try{
            _stt_component.receive_text_result_from_vivoka_android_lib(input_string_data);
        } catch(Exception e){

        }
    }
}
