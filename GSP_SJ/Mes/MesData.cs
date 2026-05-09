using BrowLib.FileClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSP.Mes
{
   public class MesData
   {
       private DataJson dataJson = new DataJson();
        /// <summary>
        /// 状态上传数据
        /// </summary>
        public class StatusData
        {
           public string sourceFlag { get; set; }
            public string software { get; set; }
            public string station { get; set; }
            public string equipmentNo { get; set; }
            public string user { get; set; }
            public string currTime { get; set; }
            public string equipmentStatusID { get; set; }
            public string reasonNo { get; set; }
            public string description { get; set; }
        }

        public class AlarmData
        {
            public string sourceFlag { get; set; }
            public string software { get; set; }
            public string station { get; set; }
            public string equipmentNo { get; set; }
            public string currTime { get; set; }
            public string user { get; set; }
            public string alertCode { get; set; }
            public string alertReset { get; set; }
            public string alertDescription { get; set; }
            public string alertLevel { get; set; }
            public string alertID { get; set; }
            public string alertLocation { get; set; }
        }


        public class ParmType
        {
            public string param { get; set; }
            public string value { get; set; }
            public string unitNo { get; set; }
            public string time { get; set; }
        }
        public class PrmData
        {
            public string sourceFlag { get; set; }
            public string software { get; set; }
            public string station { get; set; }
            public string equipmentNo { get; set; }
            public string currTime { get; set; }
            public string user { get; set; }

            public List<ParmType> paramList { get; set; } = new List<ParmType>();

        }

        public class  mResult
        {
            public string data { get; set; }
            public string message { get; set; }
            public string ngCode { get; set; }
            public string result { get; set; }
        }


        public string UpDataAPI_S(string user, string equipmentStatusID,out string updata)
        {

            StatusData statusData = new StatusData
            {
                sourceFlag = Global.mesConfig.sourceFlag,
                software = Global.mesConfig.software,
                station = Global.mesConfig.station,
                equipmentNo = Global.mesConfig.equipmentNo,
                user = user,
                equipmentStatusID = equipmentStatusID,
                currTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                reasonNo = "",
                description = ""
            };
            string JsonString = ConvertJson.JsonFormat(ConvertJson.ToJsonString(statusData));
            updata = JsonString;
           return new HttpHelper().PostHttpResponse(Global.mesConfig.Url_1,JsonString,3000,"null",null);
        }

        public string UpDataAPI_Alm(string user, string alertCode, string alertID,string Description,string TokenID, out string updata)
        {

            AlarmData statusData = new AlarmData
            {
                sourceFlag = Global.mesConfig.sourceFlag,
                software = Global.mesConfig.software,
                station = Global.mesConfig.station,
                equipmentNo = Global.mesConfig.equipmentNo,
                currTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                user = user,
                alertCode = alertCode,
                alertReset = "1",
                alertDescription = Description,
                alertLevel = "1",
                alertID = alertID,
                alertLocation = ""
            };
            string JsonString = ConvertJson.JsonFormat(ConvertJson.ToJsonString(statusData));
            updata = JsonString;
            return new HttpHelper().PostHttpResponse(Global.mesConfig.Url_2, JsonString, 3000, TokenID, null);
        }

        public string UpDataAPI_Pam(string user,List<ParmType> ParamList,out string updata)
        {

            PrmData statusData = new PrmData
            {
                sourceFlag = Global.mesConfig.sourceFlag,
                software = Global.mesConfig.software,
                station = Global.mesConfig.station,
                equipmentNo = Global.mesConfig.equipmentNo,
                currTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                user = user,
                paramList = ParamList

            };
            string JsonString = ConvertJson.JsonFormat(ConvertJson.ToJsonString(statusData,""));
            updata = JsonString;
            return new HttpHelper().PostHttpResponse(Global.mesConfig.Url_3, JsonString, 3000, "null", null);
        }
    }
}
