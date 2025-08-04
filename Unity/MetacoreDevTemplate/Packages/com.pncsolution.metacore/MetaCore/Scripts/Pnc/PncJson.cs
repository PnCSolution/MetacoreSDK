/***************************************************
* json serializer/deserializer                     *
*                                                  *
* Date: 2022/05/11                                 *
* Author: hglee@pncsolution.co.kr                  *
****************************************************/
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Pnc
{
  namespace Model
  {
    /*
    * Json serialzer/deserializer
    */
    public class PncJson<T>
    {
      public static T deserialize(string jsonString)
      {
        T jsonObject = JsonConvert.DeserializeObject<T>(jsonString);
        if(jsonObject==null) {
          throw new PncException("[exception occurred] what=[json deserialize]");
        }
        return jsonObject;
      }

      public static string serialize(T jsonObject)
      {
        string jsonString = JsonConvert.SerializeObject(jsonObject);
        if(jsonString==null) {
          throw new PncException("[exception occurred] what=[json serialize]");
        }
        return jsonString;
      }
    } //JsonConverter
  } //Model
} //PncRtc
