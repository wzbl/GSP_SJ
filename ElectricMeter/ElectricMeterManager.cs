using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricMeter
{
    public class ElectricMeterManager
    {

        static IElectricMeter electric;

        public static void SetElectricMeterType(ElectricMeterType meterType)
        {

        }

        public static bool IsConnected()
        {
            if (electric == null)
                return false;
            return electric.IsConnected();
        }

        public static void Connect()
        {
            electric.Connect();
        }

        public static void Close()
        {
            electric.Close();
        }

        public static void SetType(TestType testType)
        {
            electric.SetType(testType);
        }
        
        public static void Set(string frequency)
        {
            electric.SetFrequency(frequency);
        }
        
        public static void ReadRequest()
        {
            electric.ReadRequest();
        }

        public static void ResetReadStatus()
        {
            electric.ResetReadStatus();
        }

        public static ReadStatus GetReadStatus()
        {
            return electric.GetReadStatus();
        }

        public static double GetValue()
        {
            return electric.Value();
        }

    }



}
