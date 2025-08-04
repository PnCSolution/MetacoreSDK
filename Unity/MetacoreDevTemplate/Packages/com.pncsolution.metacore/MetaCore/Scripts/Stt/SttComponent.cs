using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Serialization;
using System.Diagnostics;
using System.Threading;
using util;
using System.Runtime.CompilerServices;

namespace kr.co.pncsolution.Stt {
    [Serializable]
    // [DefaultExecutionOrder(int.MinValue)]
    // [RequireComponent(typeof(CallbackTest))]
    [RequireComponent(typeof(UnityMainThreadDispatcher))]
    public class SttComponent : MonoBehaviour
    {
        private string TAG = "[class]SttComponent :: ";

        // public string result_string;
        // public GameObject result_gameObject;

        // public CommandString _command;

        private string _separator = "::";


        // public Button[] string_arr;
        // public UnityEvent temp;

        // public TestButtonClass[] _buttons_list;
        public GameObjectClass[] _gameobjects_list;

        // public UnityEvent _callback_list;

        AndroidJavaObject _unity_activity;
        AndroidJavaObject _android_context;
        AndroidJavaObject _library_instance;

        ReceiveTextResultCallback _receive_text_result_callback;


        void Awake(){
            UnityEngine.Debug.Log(TAG+"[method]Awake...[START]");
            AndroidJavaClass unity_player = new AndroidJavaClass(
                "com.unity3d.player.UnityPlayer"
            );
            _unity_activity = unity_player.GetStatic<AndroidJavaObject>("currentActivity");
            _android_context = _unity_activity.Call<AndroidJavaObject>("getApplicationContext");

            _library_instance = new AndroidJavaObject(
                "kr.co.pncsolution.vivokalibrary.vivoka.VivokaLibraryMain"
            );


            _library_instance.Call("set_context", _android_context);
            _library_instance.Call("set_activity", _unity_activity);

            _receive_text_result_callback = new ReceiveTextResultCallback(this);
            _library_instance.Call("set_result_text_callback", _receive_text_result_callback);


            // todo
            _library_instance.Call("set_dynamic_command_array", generate_command_array_to_string());

            // AndroidServiceBridge.Instance().eventStt.AddListener(android_service_bridge_invoke_result);
            UnityEngine.Debug.Log(TAG+"[method]Awake...[END]");
        }

        void Start(){
            initStt();
        }

        private void OnDestroy() {
            stopStt();
        }


        public void initStt(){
            _library_instance.Call("initStt");
        }

        public void stopStt(){
            _library_instance.Call("stopStt");
        }

        private string generate_command_array_to_string(){
            string return_string = "";

            foreach(GameObjectClass one in _gameobjects_list){
                return_string += one.target + _separator;
            }

            return return_string;
        }


        bool _previous_status = false;
        private void OnApplicationFocus(bool input_status) {
            UnityEngine.Debug.Log(TAG+"[method]OnApplicationFocus...[START]");
            UnityEngine.Debug.Log(TAG+"[Boolean]OnApplicationPause bool = "+input_status);

            if(_previous_status!=input_status){
                if(input_status==false){
                    _library_instance.Call("stopStt");
                } else {
                    _library_instance.Call("initStt");
                }
                _previous_status = input_status;
            }
            UnityEngine.Debug.Log(TAG+"[method]OnApplicationFocus...[END]");
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void receive_text_result_from_vivoka_android_lib(string input){
            UnityEngine.Debug.Log(TAG+"result = "+input);
            android_service_bridge_invoke_result(input);
        }


        public void android_service_bridge_invoke_result(string jsonParam){

            UnityEngine.Debug.Log(TAG+"[method]start foreach for game objects list...");

            // int result_string_num = -1;

            // result_string_num = return_parsing_result(jsonParam);

            foreach(GameObjectClass one in _gameobjects_list){
                if(one.target==jsonParam){
                    one.CommandCallback.Invoke();
                }
            }

        }


        // private int return_parsing_result(string input_string){
        //     for(int i=0; i<command_string_array.Length; i++){
        //         if(command_string_array[i]==input_string){
        //             return i;
        //         }
        //     }

        //     return -1;
        // }


        public void test_invoke_response_func(Button input){
            UnityEngine.Debug.Log("**** hello, world~! ****");
            input.onClick.Invoke();
        }

        public void test_invoke_response_func_2(){
            UnityEngine.Debug.Log("**** volume up~! ****");
        }

        // static public string[] test_string_array = {
        //     "볼륨 업",
        //     "볼륨 다운",
        //     "메뉴 확인",
        //     "메뉴 취소",
        //     "화면 초기화"
        // };


        // public enum CommandString{
        //     [InspectorName("볼륨 업")]
        //     Command0,
        //     [InspectorName("볼륨 다운")]
        //     Command1,
        //     [InspectorName("메뉴 확인")]
        //     Command2,
        //     [InspectorName("메뉴 취소")]
        //     Command3,
        //     [InspectorName("화면 초기화")]
        //     Command4
        // }

    }


    // [Serializable]
    // public class TestButtonClass{
    //     public Button target_button;

    //     // public SttComponent.CommandString command_popup;
    //     public UnityEvent command_callback;

    // }


    [Serializable]
    public class GameObjectClass{
        // public SttComponent.CommandString command_popup;
        public string target;
        // public AndroidServiceBridge.StrEvent commandCallback = AndroidServiceBridge.Instance().eventStt;
        public UnityEvent CommandCallback;
    }

}

