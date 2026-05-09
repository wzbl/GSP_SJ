using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSP.Light
{
    public class HG_Light : LBase
    {
        LightController.Comm LComm = new LightController.Comm();
        private LightController.Command cmd = LightController.Command.Null;

        public string DeviceName {  get; set; }
        public bool IsOpen {  get; set; }
        public  bool LightIni(string PortName)
        {
            try
            {
                if (!LComm.IsOpen)
                {
                    LComm.PortName = PortName;
                    LComm.Baudrate = LightController.Baudrate.Baud_19200;
                    LComm.Databits = LightController.Databits.Eight;
                    LComm.Parity = System.IO.Ports.Parity.None;
                    LComm.Stopbits = System.IO.Ports.StopBits.One;
                    LComm.ReadTimeout = 300;//ms
                    LComm.WriteTimeout = 300;
                    LComm.Open();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public  bool OpenLight(int Channel)
        {
            LightController.ChannelNum channel = (LightController.ChannelNum)Channel;
            int result = LComm.OpenLight(channel);//打开光源
            cmd = LightController.Command.OpenLight;
            if (result == 0) { return true; }
            else
                return false;
        }

        public  bool CloseLight(int Channel)
        {
            LightController.ChannelNum channel = (LightController.ChannelNum)Channel;
            int result = LComm.CloseLight(channel);//关闭光源
            cmd = LightController.Command.CloseLight;
            if (result == 0) { return true; }
            else
                return false;
        }

        public  bool SetIntensity(int Channel, int Light)
        {
            LightController.ChannelNum channel = (LightController.ChannelNum)Channel;
            int result = LComm.SetIntensity(Light, channel);//设置光源亮度
            cmd = LightController.Command.CloseLight;
            if (result == 0) { return true; }
            else
                return false;
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
            LightController.ChannelNum channel = (LightController.ChannelNum)Channel;
            return LComm.GetIntensity(channel);//获取亮度
        }
    }
}
