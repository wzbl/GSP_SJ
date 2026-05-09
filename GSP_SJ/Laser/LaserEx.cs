using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSP.Laser
{
    public class LaserEx
    {
        public BrowLib.Communication.SerialEx Serial { get; set; }
        private byte[] S_Data = new byte[] { 0x25, 0x30, 0x31, 0x23, 0x52, 0x4D, 0x44, 0x35, 0x43, 0x0D };//松下
        private byte[] S_Data1 = new byte[] { 0x02, 0x43, 0xB0, 0x01, 0x03, 0xF2 };//深视
        private byte[] S_Data2 = new byte[] { 0x01, 0x04, 0x00, 0x00, 0x00, 0x02, 0x71, 0xCB };//松下模拟量
        private byte[] S_Data3 = new byte[] { 0xFA, 0xFB, 0x0A, 0x00, 0x01, 0x00, 0x01, 0x01, 0x00, 0x00, 0x00, 0x00, 0xAD, 0xFA, 0xFC, 0xFD };
        public LaserEx()
        {
            Serial = new BrowLib.Communication.SerialEx();
        }
        public bool LaserIni(string PortName, int LaserType)
        {
            bool Rtn=false;
            switch (LaserType)
            {
                case 0://松下
                    Serial.IsHex = false;//不启用16进制接收
                    Rtn=Serial.PortIni(PortName, 115200, 8, 0, 1, "");
                    break;
                case 1://深视 SD22-35-485-VA
                    Serial.IsHex = true;//启用16进制接收
                    Rtn = Serial.PortIni(PortName, 115200, 8, 0, 1, "");
                    break;
                case 2://深视 SD33-35-485-VA
                    Serial.IsHex = true;//启用16进制接收
                    Rtn = Serial.PortIni(PortName, 115200, 8, 0, 1, "");
                    break;
                case 3://松下模拟量HG-C1050
                    Serial.IsHex = true;//启用16进制接收
                    Rtn = Serial.PortIni(PortName, 115200, 8, 0, 1, "");
                    break;
                case 4://PH0SKEY   PDL-050-485
                    Serial.IsHex = true;//启用16进制接收
                    Rtn=Serial.PortIni(PortName, 115200, 8, 0, 1, "");
                    break;
            }
            return Rtn;
        }
        public void Send(byte[] senddata)
        {
            Serial.SendData(senddata);
        }
        public void Send(string senddata)
        {
            Serial.SendData(senddata);
        }
        public void Send(int LaserType)
        {
            switch (LaserType)
            {
                case 0:
                    Serial.SendData(S_Data);
                    break;
                case 1:
                    Serial.SendData(S_Data1);
                    break;
                case 2:
                    Serial.SendData(S_Data1);
                    break;
                case 3:
                    Serial.SendData(S_Data2);
                    break;
                case 4:
                    Serial.SendData(S_Data3);
                    break;
            }
        }
        public double LaserValue(int LaserType)
        {
            double value = 0;
            string hex;
            try
            {
                switch (LaserType)
                {
                    case 0:
                        value = Convert.ToDouble(Serial.Result.Substring(7, 8)) / 10000;
                        break;
                    case 1:
                        hex = Serial.Result.Replace(" ", "");
                        value = (double)Convert.ToInt16(hex.Substring(4, 4), 16) / 100;
                        break;
                    case 2:
                        hex = Serial.Result.Replace(" ", "");
                        value = (double)Convert.ToInt16(hex.Substring(4, 4), 16) / 1000;
                        break;
                    case 3:
                        hex = Serial.Result.Replace(" ", "");
                        string VA = hex.Substring(6, 4);
                        double vvalue = (double)Convert.ToInt16(hex.Substring(6, 4), 16);
                        value = AIToBvalue(0, vvalue, 0, 4095, -15, 15);
                        break;
                    case 4:
                        hex = Serial.Result.Replace(" ", "");
                        string hi1 = hex.Substring(16, 2);
                        string ho1 = hex.Substring(18, 2);
                        string hi2 = hex.Substring(20, 2);
                        string ho2 = hex.Substring(22, 2);
                        string strvalue = ho2 + hi2 + ho1 + hi1;
                        value = 50 - (double)Convert.ToInt32(strvalue, 16) / 10000;
                        break;
                }
                return value;
            }
            catch { return 0; }
        }
        public double AIToBvalue(int type, double invalue, double aimin, double aimax, double bmin, double bmax)
        {
            double value = 0;
            switch (type)
            {
                case 0:
                    value = (invalue - aimin) / (aimax - aimin) * (bmax - bmin) + bmin;
                    break;
                case 1:
                    value = (invalue - bmin) / (bmax - bmin) * (aimax - aimin) + aimin;
                    break;
            }
            return value;
        }
    }
}
