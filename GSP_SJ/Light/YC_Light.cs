using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSP.Light
{
    public class YC_Light : LBase
    {
        public BrowLib.Communication.SerialEx Serial { get; set; } = new BrowLib.Communication.SerialEx();
        public string DeviceName { get; set; }
        public bool IsOpen { get; set; }
        public  bool LightIni(string PortName)
        {
            bool Rtn = false;
            try
            {
                Serial.IsHex = false;//不启用16进制接收
                Rtn=Serial.PortIni(PortName, 19200, 8, 0, 1, "");
                return Rtn;
            }
            catch
            {
                return Rtn;
            }
        }

        public  bool OpenLight(int Channel)
        {
            try
            {
                Serial.SendData("SSH#");
                return true;
            }
            catch { return false; }
        }

        public  bool CloseLight(int Channel)
        {
            try
            {

                Serial.SendData("SSL#");
                return true;
            }
            catch { return false; }
        }

        public  bool SetIntensity(int Channel, int Light)
        {
            try
            {
                string cmd = null;
                switch (Channel)
                {
                    case 1:
                        cmd = string.Format("SA0{0}#", Light.ToString("D3"));
                        break;
                    case 2:
                        cmd = string.Format("SB0{0}#", Light.ToString("D3"));
                        break;
                    case 3:
                        cmd = string.Format("SC0{0}#", Light.ToString("D3"));
                        break;
                    case 4:
                        cmd = string.Format("SD0{0}#", Light.ToString("D3"));
                        break;
                    default:
                        cmd = string.Format("SA0{0}#", Light.ToString("D3"));
                        break;
                }
                Serial.SendData(cmd);
                return true;
            }
            catch { return false; }
        }

        public  bool SetRgbLight(int R, int G, int B)
        {
            bool re1 = SetIntensity(1, R);
            bool re2 = SetIntensity(2, G);
            bool re3 = SetIntensity(3, B);
            if (re1 && re2 && re3) { return true; }
            else
                return false;
        }

        public  int GetIntensity(int Channel)
        {
            try
            {
                string cmd = null;
                switch (Channel)
                {
                    case 1:
                        cmd = "SA#";
                        break;
                    case 2:
                        cmd = "SB#";
                        break;
                    case 3:
                        cmd = "SC#";
                        break;
                    case 4:
                        cmd = "SD#";
                        break;
                    default:
                        cmd = "SA#";
                        break;
                }
                Serial.SendData(cmd);
                System.Threading.Thread.Sleep(50);
                string data = Serial.Result;
                if (data != null)
                {
                    if (data.Length > 2)
                    {
                        data = Serial.Result.Substring(2);
                    }
                    else
                    {
                        data = data; // 原字符串长度小于2，不做操作
                    }
                }
                return Convert.ToInt16(data);
            }
            catch { return 0; }
        }
    }
}
