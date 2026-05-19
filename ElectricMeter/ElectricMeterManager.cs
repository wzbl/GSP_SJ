using BrowApp;
using BrowApp.Language;
using SqlHelper;
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

        static double standValue = 0;

        static double maxValue = 0;

        static double minValue = 0;

        static TestType type;

        static string unit = "";

        /// <summary>
        /// 电桥参数
        /// </summary>
        public static List<Eng_MeterOptionItem> _MeterOptionItems = new List<Eng_MeterOptionItem>();

        public static void SetElectricMeterType(ElectricMeterType meterType)
        {
            switch (meterType)
            {
                case ElectricMeterType.TH2830:
                    electric = new TH2830();
                    electric.Connect();
                    break;
            }
        }

        public static bool IsConnected()
        {
            if (electric == null)
                return false;
            return electric.IsConnected();
        }


        public static void Close()
        {
            if (IsConnected())
                electric.Close();
        }

        public static void SetCommand(TestType testType, string frequency)
        {
            try
            {
                electric.SetType(testType, frequency);
            }
            catch (Exception ex)
            {

            }

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
            double val = electric.Value();
            GetUnitReadData(ref val);
            return val;
        }

        /// <summary>
        /// 设置LCR参数
        /// </summary>
        /// <param name="LcrType"></param>
        /// <param name="MinValue"></param>
        /// <param name="MinValueUnit"></param>
        /// <param name="MaxValue"></param>
        /// <param name="MaxValueUnit"></param>
        public static void SetLCRParameter(string LcrType, double value, string Unit, double MaxValue, double MinValue)
        {
            unit = Unit;
            standValue = value;
            maxValue = MaxValue;
            minValue = MinValue;
            LogHelper.Log.I_Log("设置测值信息:" + LcrType + "-" + value + Unit);
            switch (LcrType)
            {
                case "R":
                    type = TestType.R;
                    break;
                case "C":
                    type = TestType.C;
                    break;
                case "H":
                    type = TestType.H;
                    break;
            }
            SetCommand(type, GetFrequency());
        }

        public static bool GetLCRResult(out double value)
        {
            value = 0;
            value = GetValue();
            if (value < minValue || value > maxValue)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取频率
        /// </summary>
        public static string GetFrequency()
        {
            string frequency = "1000";
            try
            {
                List<Eng_MeterOptionItem> eng_Meters = _MeterOptionItems.Where(x => x.LcrType == type.ToString()).ToList();
                if (eng_Meters != null && eng_Meters.Count > 0)
                {
                    decimal minValue = 0;
                    decimal maxValue = 0;
                    string minUnit = "";
                    string maxUnit = "";
                    double value = GetUnitValue(standValue, unit);
                    foreach (var item in eng_Meters)
                    {
                        if (item.MinValue != null)
                        {
                            minValue = (decimal)item.MinValue;
                        }
                        else
                        {
                            continue;
                        }

                        if (item.MaxValue != null)
                        {
                            maxValue = (decimal)item.MaxValue;
                        }
                        else
                        {
                            continue;
                        }

                        if (item.MinValueUnit != null)
                        {
                            minUnit = item.MinValueUnit;
                        }
                        else
                        {
                            continue;
                        }

                        if (item.MaxValueUnit != null)
                        {
                            maxUnit = item.MaxValueUnit;
                        }
                        else
                        {
                            continue;
                        }

                        double min = GetUnitValue(double.Parse(minValue.ToString()), minUnit);
                        double max = GetUnitValue(double.Parse(maxValue.ToString()), maxUnit);
                        if (value >= min && value <= max)
                        {
                            frequency = item.Frequency;
                            if (frequency != null)
                                return frequency;
                        }

                    }
                }
            }
            catch (Exception)
            {

            }
            return frequency;
        }

        private static double GetUnitValue(double value, string unit)
        {
            double sta = value;
            switch (unit)
            {
                case "mΩ":
                    sta = sta / 1000;
                    break;

                case "Ω":

                    break;

                case "KΩ":
                    sta = sta * 1000;
                    break;

                case "MΩ":
                    sta = sta * 1000 * 1000;
                    break;

                case "PF":
                    sta = sta / (1000000000000);
                    break;

                case "NF":
                    sta = sta / (1000 * 1000 * 1000);
                    break;

                case "UF":
                    sta = sta / (1000 * 1000);
                    break;

                case "MF":
                    sta = sta / (1000);
                    break;

                case "F":
                    break;
            }
            return sta;
        }

        /// <summary>
        /// 电表读值单位化
        /// </summary>
        private static void GetUnitReadData(ref double data)
        {
            switch (unit)
            {
                case "mΩ":
                    data = data * 1000;
                    break;

                case "Ω":
                    break;

                case "KΩ":
                    data = data / 1000;
                    break;

                case "MΩ":
                    data = data / 1000 / 1000;
                    break;

                case "PF":
                    data = data * 1000000000000;
                    break;

                case "NF":
                    data = data * 1000000000;
                    break;

                case "UF":
                    data = data * 1000000;
                    break;

                case "MF":
                    data = data * 1000;
                    break;

                case "F":
                    break;

                default:
                    break;
            }

        }

    }



}
