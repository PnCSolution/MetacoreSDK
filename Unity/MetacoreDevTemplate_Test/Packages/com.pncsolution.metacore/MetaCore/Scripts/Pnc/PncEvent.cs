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
    public class PncEvent
    {
      public string type { get; set; }  // 이벤트 종류: HandsResult/Slam/...
      public string owner { get; set; } // 이벤트가 발생한 서비스, gesture/slam/...
      public string uniqueId { get; set; }     // 서비스 고유ID
      public long timestamp {get; set;} // 이벤트 발생시각
      public string error {get; set;} //에러코드

/*
      public Pnc.Model.Hands.HandsResult handsResult;
      public Pnc.Model.Slam.SlamResult slamResult;
*/

      public string getEventType() {
        return type;

      }

      public void setEventType(string type) {
        this.type = type;        
      }

      public string getOwner() {
        return owner;

      }

      public void setOwner(string owner) {
        this.owner = owner;        
      }
      public string getUniqueuId()
      {
        return uniqueId;

      }

      public void setUniqueuId(string uniqueId)
      {
        this.uniqueId = uniqueId;
      }


      public long getTimestamp() {
        return timestamp;

      }

      public void setTimestamp(long timestamp) {
        this.timestamp = timestamp;        
      }

      public string getError() {
          return this.error;
      }

      public void setError(string error) {
          this.error = error;
      }

/*
      public Pnc.Model.Hands.HandsResult getHandsResult() {
        return handsResult;
      }

      public void setHandsResult(Pnc.Model.Hands.HandsResult handsResult) {
        this.handsResult = handsResult;
      }

      public Pnc.Model.Slam.SlamResult getSlamResult() {
        return slamResult;        
      }

      public void setSlamResult(Pnc.Model.Slam.SlamResult slamResult) {
        this.slamResult = slamResult;
      }
*/      

/*
      public static PncEvent deserialize(string jsonString)
      {
        PncEvent model = JsonConvert.DeserializeObject<PncEvent>(jsonString);
        return model;
      }

      public static string serialize(PncEvent jsonObject)
      {
        return JsonConvert.SerializeObject(jsonObject);
      }
*/
      public string info()
      {
        string buff = string.Empty;

        buff = $"[PncEvent type={type}";
        return buff;
      }

    }
  } //model
} //pnc
