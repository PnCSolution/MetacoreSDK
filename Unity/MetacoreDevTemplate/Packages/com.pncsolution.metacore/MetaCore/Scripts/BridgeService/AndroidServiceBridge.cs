using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Pnc.Model;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*************************************************************
* PNC XR SDK API 및 이벤트(메세지) 전송규격
1.SDK에서의 메세지 전송에는 json 을 사용한다.
2.메세지데이터(json) 의 serialize/deserialize 에는 newtonsoft-json 을 사용한다.
3.newtonsoft-json 은 unity(il2cpp) 에서 사용이 가능한 라이브러리를 사용해야 한다.
  3.1 "https://github.com/jilleJr/Newtonsoft.Json-for-Unity/wiki/Install-official-via-UPM" 참조

4.다음과 같은 방식으로 json 라이브러리를 설치해야 한다.
  4.1 패키지 폴더(수정"~/packages" )의 manifest.json 파일을 텍스트 편집기로 연다
  4.2 "dependencies" 항목에 패키지명을 입력하고 저장한다.
    {
      "dependencies": {
        "com.unity.nuget.newtonsoft-json": "3.0.2",
        // ...
      }
}
*************************************************************/
namespace Pnc
{
    public class AndroidServiceBridge : MonoBehaviour
    {
        private const Boolean DEBUG = false;
        public class AidlServiceContext
        {
            private string _serviceType; //실행중인 서비스타입(이름)
            private string _serviceState = EVT_SERVICE_DESTROY; //실행중인 서비스 상태, //서비스연결상태 "serviceInit/serviceStart/serviceStop/serviceTerm";
            private string _uniqueId = "FFFFFFFF"; //서비스 제어용 uniqueId, FFFFFFFF: 서비스 없음

            public AidlServiceContext(string serviceType)
            {
                _serviceType = serviceType;
            }

            public string GetServiceType()
            {
                return _serviceType;
            }

            public void SetServiceType(string serviceType)
            {
                _serviceType = serviceType;
            }

            public string GetServiceState()
            {
                return _serviceState;
            }

            public void SetServiceState(string serviceState)
            {
                _serviceState = serviceState;
            }

            public string GetServiceUniqueId()
            {
                return _uniqueId;
            }

            public void SetServiceUniqueId(string uniqueId)
            {
                _uniqueId = uniqueId;
            }
        }

        [System.Serializable]
        public class StrEvent : UnityEvent<string>
        {
        }

        //선택할 수 있는 안드로이드 서비스의 종류
        public const string SERVICE_TYPE_STT_VVK = "pnc.server.service.stt.vvk"; //STT_VVK 서비스
        public const string SERVICE_TYPE_CAMERAPREVIEW = "pnc.server.service.camerapreview";
        public const string SERVICE_TYPE_CALIBRATION = "pnc.server.service.gesturecalibration";

        //안드로이드 서비스로부터 수신되는 이벤트 종류
        private const string EVT_SERVICE_INIT = "serviceInit";       //서비스가 초기화되었음
        private const string EVT_SERVICE_DESTROY = "serviceDestroy";       //서비스가 종료되었음
        private const string EVT_SERVICE_RESUME = "serviceResume";     //서비스가 시작되었음
        private const string EVT_SERVICE_STOP = "serviceStop";       //서비스가 중지되었음
        private const string EVT_STT_RESULT = "sttResult";           //음성인식 결과 수신됨

        enum ServiceLifeCycle
        {
            Init,
            Resume,
            Stop,
            Destroy
        }

        //ipc object for AIDL
        public AndroidJavaObject _aidlPlugin;
        private static AndroidServiceBridge _instance = null; //singleton object

        [HideInInspector]
        public StrEvent eventStt = new StrEvent();

        [HideInInspector]
        public StrEvent eventCameraPreview = new StrEvent();

        private ConcurrentDictionary<string, AidlServiceContext> _aidlContexts; //AIDL서비스 리스트


        /*###############################################
        #                                               #
        #      for singleton pattern                    #
        #                                               #
        ################################################*/
        public static AndroidServiceBridge Instance()
        {
            if (_instance == null)
            {
                throw new PncException("[AndroidServiceBridge] could not find the AndroidServiceBridge object.");
            }
            return _instance;
        }

        /*###############################################
        #                                               #
        #      for unity life cycle                     #
        #                                               #
        ################################################*/
        //override life cycle
        protected void Awake()
        {
            Debug.Log($":::::     AndroidServiceBridge Awake     :::::");
            if (_instance == null)
            {
                _instance = this;
                //AIDL 로딩한 후 서비스API 호출할 것
                LoadAidl();
                _aidlContexts = new ConcurrentDictionary<string, AidlServiceContext>();
                //앱이 종료되기 전까지 singleton 객체 유지하도록 
                //DontDestroyOnLoad(this.gameObject);
                // AddAidlService(SERVICE_TYPE_STT_VVK);
            }
            else
            {
                Destroy(this.gameObject);
                return;
            }
        }

        protected void OnDestroy()
        {
            Debug.Log($":::::     AndroidServiceBridge OnDestroy     :::::");
            setAidlServices(ServiceLifeCycle.Destroy);
        }

        //override life cycle
        protected void OnApplicationPause(bool pause)
        {
            Debug.Log($":::::     AndroidServiceBridge OnApplicationPause paused={pause}     :::::");
            if (!pause)
            {
                //서비스 resume
                setAidlServices(ServiceLifeCycle.Resume);
            }
            else
            {
                //서비스 stop
                setAidlServices(ServiceLifeCycle.Stop);
            }
        }

        //override life cycle
        protected void OnApplicationQuit()
        {
            Debug.Log(":::::     AndroidServiceBridge OnApplicationQuit     :::::");
            setAidlServices(ServiceLifeCycle.Destroy);
        }

        /*###############################################
        #                                               #
        #      internal functions                       #
        #                                               #
        ################################################*/
        /************************************************
        *   android 서비스의 상태 저장                   *
        *************************************************/
        private void SetServiceState(string serviceType, string serviceState)
        {
            if (_aidlContexts.ContainsKey(serviceType))
                _aidlContexts[serviceType].SetServiceState(serviceState);
        }

        /************************************************
        *   android 서비스의 상태 조회                   *
        *************************************************/
        public string GetServiceState(string serviceType)
        {
            if (_aidlContexts.ContainsKey(serviceType))
                return _aidlContexts[serviceType].GetServiceState();

            return null;
        }

        /************************************************
       * aidl 서비스의 uniqueId 저장                   *
       *************************************************/
        private void setAidlServiceUniqueId(string serviceType, string uniqueId)
        {
            if (_aidlContexts.ContainsKey(serviceType))
            {
                AidlServiceContext context = _aidlContexts[serviceType];
                context.SetServiceUniqueId(uniqueId);
                if (DEBUG) Debug.Log(":::::     Update AidlService Handle at dictionries     :::::");
            }
            else
            {
                Debug.LogError(":::::     [ERROR] Update AidlService Handle, Context not found     :::::");
            }
        }

        /************************************************
        * aidl 서비스의 uniqueId 조회                   *
        *************************************************/
        public string getAidlServiceUniqueId(string serviceType)
        {
            if (_aidlContexts.ContainsKey(serviceType))
            {
                AidlServiceContext context = _aidlContexts[serviceType];
                if (DEBUG) Debug.Log($":::::     QueryAidlServiceUniqueId:: Service Found at Dictionaries, serviceTpye={serviceType}, uniqueId={context.GetServiceUniqueId()}     :::::");
                return context.GetServiceUniqueId();
            }
            Debug.LogError(":::::     [ERROR]  QueryAidlServiceUniqueId:: Context not found     :::::");
            return null;
        }

        /************************************************
        * aidl 서비스 조회                               *
        *************************************************/
        public AidlServiceContext getAidlServiceContextByServiceType(string serviceType)
        {
            if (_aidlContexts.ContainsKey(serviceType))
            {
                return _aidlContexts[serviceType];
            }
            Debug.LogError(":::::     [ERROR]  QueryAidlServiceByServiceType:: Context not found     :::::");
            return null;
        }

        public AidlServiceContext getAidlServiceContextByUniqueId(string uniqueId)
        {
            foreach (KeyValuePair<string, AidlServiceContext> pair in _aidlContexts)
            {
                string serviceType = pair.Key;
                AidlServiceContext context = pair.Value;
                if (uniqueId!.Equals(context.GetServiceUniqueId())) return context;
            }
            Debug.LogError(":::::     [ERROR]  QueryAidlServiceByUniqueId:: Context not found     :::::");
            return null;
        }

        /************************************************
        *   load AIDL module                            *
        *************************************************/

        private void LoadAidl()
        {
            if (_aidlPlugin == null)
            {
                _aidlPlugin = new AndroidJavaObject("kr.co.pncsolution.aidlmodule.AidlLoaderPlugin");
            }
        }

        /************************************************
        *           AddAidlService:                     *
        *************************************************/
        public void AddAidlService(string serviceType)
        {
            if (DEBUG) Debug.Log($":::::     AndroidServiceBridge::Add AidlService serviceType={serviceType}     :::::");
            if (_aidlContexts != null)
            {
                if (!_aidlContexts.ContainsKey(serviceType))
                {
                    if (DEBUG) Debug.Log($":::::     Add new AidlService serviceType={serviceType}     :::::");
                    //서비스가 등록되어 있지 않으면 추가
                    AidlServiceContext context = new AidlServiceContext(serviceType);
                    _aidlContexts.TryAdd(serviceType, context);
                    ServiceInit(context);
                }
                else
                {
                    if (DEBUG) Debug.Log($":::::     Exist AidlService serviceType={serviceType}     :::::");
                    return;
                }
            }
            else
            {
                Debug.LogError(":::::     [ERROR] invalid dictionary for AidlContext     :::::");
            }
        }

        /// <summary>
        /// set all registered AidlService's LifeCycle (Init, Resume, Stop, Destroy)
        /// </summary>
        /// <param name="serviceLifeCycle"></param>
        private void setAidlServices(ServiceLifeCycle serviceLifeCycle)
        {
            foreach (KeyValuePair<string, AidlServiceContext> pair in _aidlContexts)
            {
                string serviceType = pair.Key;
                AidlServiceContext context = pair.Value;
                setAidlService(context, serviceLifeCycle);
            }
        }

        /// <summary>
        /// set 'context' AidlService's LifeCycle
        /// </summary>
        /// <param name="context"></param>
        /// <param name="serviceLifeCycle"></param>
        private void setAidlService(AidlServiceContext context, ServiceLifeCycle serviceLifeCycle)
        {
            if (context == null)
            {
                Debug.LogError($":::::     AndroidServiceBridge::EnableAidlService, [ERROR] Invalid service context!     :::::");
                return;
            }
            //string serviceType = context.GetServiceType();
            switch (serviceLifeCycle)
            {
                case ServiceLifeCycle.Init:
                    ServiceInit(context);
                    break;
                case ServiceLifeCycle.Resume:
                    ServiceResume(context);
                    break;
                case ServiceLifeCycle.Stop:
                    ServiceStop(context);
                    break;
                case ServiceLifeCycle.Destroy:
                    ServiceStop(context);
                    ServiceDestroy(context);
                    break;
            }
        }
       
        /*###############################################
        #                                               #
        #      calls to Android service functions       #
        #                                               #
        ################################################*/
        private void ServiceInit(AidlServiceContext context)
        {
            StartCoroutine(_serviceInit(context));
        }

        IEnumerator _serviceInit(AidlServiceContext context)
        {
            string serviceType = context.GetServiceType();
            Debug.Log($":::::     AndroidServiceBridge::ServiceInit, serviceType={serviceType}     :::::");

            Pnc.Model.PncApi pncApi = new Pnc.Model.PncApi();
            pncApi.name = "init";
            pncApi.serviceType = serviceType;
            string jsonParam = Pnc.Model.PncJson<Pnc.Model.PncApi>.serialize(pncApi);
            string uniqueId = CallAndroidNative(pncApi.name, jsonParam);
            if (DEBUG) Debug.Log($":::::     AndroidServiceBridge::ServiceInit Done, serviceType={serviceType} is created. uniqueId={uniqueId}      :::::");
            //native init함수가 성공한 이후에 uniqueId 가 반환되고, 반환된 uniqueId 값을 업데이트 해야한다..
            setAidlServiceUniqueId(serviceType, uniqueId);
            yield return null;
        }

        private void ServiceResume(AidlServiceContext context)
        {
            StartCoroutine(_serviceResume(context));
        }

        IEnumerator _serviceResume(AidlServiceContext context)
        {
            string serviceType = context.GetServiceType();
            string uniqueId = context.GetServiceUniqueId();

            if (uniqueId == null)
            {
                Debug.LogError($":::::     AndroidServiceBridge::ServiceStart, uniqueId={uniqueId} has not been found. This serivce will be omitted.     :::::");
            }
            Debug.Log($":::::     AndroidServiceBridge::ServiceStart, uniqueId={uniqueId}, serviceType={serviceType}. Try to start...     :::::");

            Pnc.Model.PncApi pncApi = new Pnc.Model.PncApi();
            pncApi.name = "resume";
            pncApi.serviceType = serviceType;
            pncApi.uniqueId = uniqueId;

            string jsonParam = Pnc.Model.PncJson<Pnc.Model.PncApi>.serialize(pncApi);
            string result = CallAndroidNative(pncApi.name, jsonParam);

            Debug.Log($":::::     AndroidServiceBridge::ServiceStart, uniqueId={pncApi.uniqueId}, serviceType={serviceType}. Done     :::::");
            yield return null;

        }

        private void ServiceStop(AidlServiceContext context)
        {
            _serviceStop(context);
        }

        void _serviceStop(AidlServiceContext context)
        {
            string serviceType = context.GetServiceType();
            string uniqueId = context.GetServiceUniqueId();

            if (uniqueId == null)
            {
                Debug.LogError($":::::     AndroidServiceBridge::ServiceStop, uniqueId={uniqueId} has not been found. This serivce will be omitted.     :::::");
            }

            Pnc.Model.PncApi pncApi = new Pnc.Model.PncApi();
            pncApi.name = "stop";
            pncApi.serviceType = serviceType;
            pncApi.uniqueId = uniqueId;
            Debug.Log($":::::     AndroidServiceBridge::ServiceStop, uniqueId={pncApi.uniqueId}, serviceType={serviceType}     :::::");
            string jsonParam = Pnc.Model.PncJson<Pnc.Model.PncApi>.serialize(pncApi);
            string result = CallAndroidNative(pncApi.name, jsonParam);

            //yield return null;
        }

        private void ServiceDestroy(AidlServiceContext context)
        {
            _serviceDestroy(context);
        }

        void _serviceDestroy(AidlServiceContext context)
        {
            //            string uniqueId = QueryAidlServiceUniqueId(serviceType);
            string serviceType = context.GetServiceType();
            string uniqueId = context.GetServiceUniqueId();

            if (uniqueId == null)
            {
                Debug.LogError($":::::     AndroidServiceBridge::ServiceTerm, uniqueId={uniqueId} has not been found. This serivce will be omitted.     :::::");
            }

            Pnc.Model.PncApi pncApi = new Pnc.Model.PncApi();
            pncApi.name = "destroy";
            pncApi.serviceType = serviceType;
            pncApi.uniqueId = uniqueId;
            Debug.Log($":::::     AndroidServiceBridge::ServiceTerm, uniqueId={pncApi.uniqueId}, serviceType={serviceType}     :::::");

            string jsonParam = Pnc.Model.PncJson<Pnc.Model.PncApi>.serialize(pncApi);
            string result = CallAndroidNative(pncApi.name, jsonParam);
            //yield return null;
        }

        public void ServiceInvoke(String _serviceType, String _json)
        {
            if (_aidlContexts.ContainsKey(_serviceType))
            {
                _serviceInvoke(getAidlServiceContextByServiceType(_serviceType), _json);
            }
        }

        void _serviceInvoke(AidlServiceContext context, String _json)
        {
            string serviceType = context.GetServiceType();
            string uniqueId = context.GetServiceUniqueId();

            if (uniqueId == null)
            {
                Debug.LogError($":::::     AndroidServiceBridge::ServiceInvoke, uniqueId={uniqueId} has not been found. This serivce will be omitted.     :::::");
            }

            Pnc.Model.PncApi pncApi = new Pnc.Model.PncApi();
            pncApi.name = "invoke";
            pncApi.serviceType = serviceType;
            pncApi.uniqueId = uniqueId;
            if (_json != null)
            {
                pncApi.tempJson = _json;
            }
            Debug.Log($":::::     AndroidServiceBridge::ServiceInvoke, uniqueId={pncApi.uniqueId}, serviceType={serviceType}     :::::");

            string jsonParam = Pnc.Model.PncJson<Pnc.Model.PncApi>.serialize(pncApi);
            string result = CallAndroidNative(pncApi.name, jsonParam);
            //yield return null;
        }

        /*###############################################
        #                                               #
        #      calls to Android native functions        #
        #                                               #
        ################################################*/
        private String CallAndroidNative(string functionName, string jsonParam)
        {
            string result = "FFFFFFFF"; //uniqueid
            if (_aidlPlugin != null)
            {
#if UNITY_ANDROID
                if (jsonParam != null)
                {
                    result = _aidlPlugin.Call<string>(functionName, jsonParam);
                }
                else
                {
                    result = _aidlPlugin.Call<string>(functionName);
                }
                if (DEBUG) Debug.Log($"'{functionName}' result code={result}");
#endif
            }
            return result;
        }

        /*###############################################
        #                                               #
        #   the events have been updated from android   #
        #                                               #
        ################################################*/
        void OnEventArrived(string jsonParam)
        {
            dispatchEventHandler(jsonParam);
        }

        void OnCameraArrived(string not)
        {
            eventCameraPreview.Invoke(not);
        }

        void dispatchEventHandler(string jsonParam)
        {
            PncEvent evt = PncJson<PncEvent>.deserialize(jsonParam);
            string type = evt.getEventType();
            Debug.Log($":::::     AndroidServiceBridge::dispatchEventHandler: param={jsonParam}     :::::");
            //simon 230531 broadcasting(unknown) 또는 나에게 온 이벤트가 아닌 경우 return한다.
            string uniqueId = evt.getUniqueuId();
            if (uniqueId != "unknown" && type != EVT_SERVICE_INIT)
            {
                foreach (KeyValuePair<string, AidlServiceContext> pair in _aidlContexts)
                {
                    AidlServiceContext context = pair.Value;
                    if (!uniqueId.Equals(context.GetServiceUniqueId()))
                    {
                        Debug.Log($":::::     AndroidServiceBridge::OnEventArrived: unique ID not found! return!! uniqueId:{uniqueId}     :::::");
                        return;
                    }
                }
            }

            switch (type)
            {
                case EVT_SERVICE_INIT: //서비스 초기화됨
                    OnMsgServiceInit(evt);
                    break;
                case EVT_SERVICE_DESTROY: //서비스 종료됨
                    OnMsgServiceDestroy(evt);
                    break;
                case EVT_SERVICE_RESUME: //서비스 시작됨
                    OnMsgServiceResume(evt);
                    break;
                case EVT_SERVICE_STOP: //서비스 멈춤
                    OnMsgServiceStop(evt);
                    break;
                case EVT_STT_RESULT: //음성인식 결과 수신됨
                    eventStt.Invoke(jsonParam);
                    break;
                default:
                    if (DEBUG) Debug.LogWarning($"Unknown event={type}");
                    break;
            }
        }

        /************************************************
        * received 'ServiceInit' message from android   *
        *************************************************/
        void OnMsgServiceInit(PncEvent evt)
        {
            //서비스가 바인드되고, ServiceConnected 후에 "ServiceInit" 이 전달된다.
            //따라서, ServiceInit 수신된 후에 ServiceResume/ServiceStop/ServiceDestroy 등을 호출해야 한다.
            SetServiceState(evt.getOwner(), EVT_SERVICE_INIT);
            AidlServiceContext context = getAidlServiceContextByUniqueId(evt.getUniqueuId());
            if (context != null) ServiceResume(context);
        }

        /************************************************
        * received 'ServiceTerm' message from android   *
        *************************************************/
        void OnMsgServiceDestroy(PncEvent evt)
        {
            SetServiceState(evt.getOwner(), EVT_SERVICE_DESTROY);
        }

        /************************************************
        * received 'ServiceStart' message from android  *
        *************************************************/
        void OnMsgServiceResume(PncEvent evt)
        {
            SetServiceState(evt.getOwner(), EVT_SERVICE_RESUME);

        }

        /************************************************
        * received 'ServiceStop' message from android   *
        *************************************************/
        void OnMsgServiceStop(PncEvent evt)
        {
            SetServiceState(evt.getOwner(), EVT_SERVICE_STOP);
        }
        
        public void stopVideo()
        {
            ServiceInvoke(SERVICE_TYPE_CAMERAPREVIEW, "stopVideo");
        }

        public void capturePicture()
        {
            ServiceInvoke(SERVICE_TYPE_CAMERAPREVIEW, "capturePicture");
        }

        public void startVideo()
        {
            ServiceInvoke(SERVICE_TYPE_CAMERAPREVIEW, "startVideo");
        }
    } //AndroidService
} //namespace pnc
