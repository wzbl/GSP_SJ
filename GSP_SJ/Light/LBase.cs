using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSP.Light
{
    public  interface  LBase
    {
        string DeviceName { get; set; }

        bool IsOpen { get; set; }
        bool LightIni(string PortName);

        bool OpenLight(int Channel);

        bool CloseLight(int Channel);
        bool SetIntensity(int Channel, int Light);

        int GetIntensity(int Channel);

        bool SetRgbLight(int R, int G, int B);
    }
}
