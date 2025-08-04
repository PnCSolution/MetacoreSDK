using UnityEngine;
using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace Pnc
{
    namespace Model
    {
        [System.Serializable]
        public class PncApi
        {
            public string name { get; set; }     // API name, 호출할 API 명
            public string serviceType { get; set; }     // API parameter, 서비스종류: gesture/slam/...
            public string uniqueId { get; set; }     // API parameter, 서비스 고유ID
            public string tempJson { get; set; }    //invoke 등 데이터를 추가적으로 보내야 할 경우 josn 으로 패킹해서 전달.

            //public static PncApi deserialize(string jsonString)
            //{
            //    PncApi model = JsonConvert.DeserializeObject<PncApi>(jsonString);
            //    return model;
            //}

            //public static string serialize(PncApi jsonObject)
            //{
            //    return JsonConvert.SerializeObject(jsonObject);
            //}

            public string info()
            {
                string buff = string.Empty;

                buff = $"[PncApi name={name}, serviceType={serviceType}, uniqueId={uniqueId} ";
                //buff = $"[PncApi name={name}, serviceType={serviceType}, uniqueId={uniqueId}, cameraId={cameraId}, cameraRotation={cameraRotation}, sttCommand={sttCommand} ";
                return buff;
            }

        }
    } //model
} //pnc
