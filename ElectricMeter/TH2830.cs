using BrowApp;
using BrowApp.Language;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ElectricMeter
{
    /// <summary>
    /// 同惠TH2830电表
    /// RS232通讯
    /// </summary>
    public class TH2830 : IElectricMeter
    {
        private SerialPort SerialPort { get; set; }

        private double ReadData = 0;
        private ReadStatus readStatus = ReadStatus.None;

        public void Close()
        {
            try
            {
                SerialPort.Close();
            }
            catch (Exception ex)
            {
                LogHelper.Log.I_Log(ex.Message);
            }

        }

        public void Connect()
        {
            try
            {
                SerialPort = new SerialPort
                {
                    PortName = "COM4",
                    BaudRate = 115200,
                    DataBits = 8,
                    Parity = Parity.None,
                    StopBits = StopBits.One
                };
                OpenSerialPort();
            }
            catch (Exception ex)
            {
                LogHelper.Log.I_Log(ex.Message);
            }

        }

        public void OpenSerialPort()
        {
            try
            {
                if (SerialPort.IsOpen) SerialPort.Close();
                Thread.Sleep(100);
                SerialPort.Open();
                if (SerialPort.IsOpen)
                {
                    //打开串口成功
                    SerialPort.DataReceived += SerialPort_DataReceived;
                    ReadRequest();
                    LogHelper.Log.I_Log("电表连接成功");
                }
                else
                {
                    //打开串口失败
                    LogHelper.Log.I_Log("电表连接失败");
                }
            }
            catch (Exception ex)
            {
                //打开串口失败
                LogHelper.Log.I_Log("电表连接异常" + "：" + ex.Message);
            }
        }

        /// <summary>
        /// 解析串口返回数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(100);
            try
            {
                byte[] recvData = new byte[SerialPort.BytesToRead];
                SerialPort.Read(recvData, 0, recvData.Length);
                string Recv = Encoding.Default.GetString(recvData);
                LogHelper.Log.I_Log("接收到电表数据".tr() + ":" + Recv.Replace("\r\n", ""));
                string[] datels = Recv.Trim().Split(',');
                double.TryParse(datels[0], out ReadData);
            }
            catch (Exception ex)
            {
                ReadData = 0;
                LogHelper.Log.I_Log(ex.Message);
            }
            readStatus = ReadStatus.Finished;
        }

        public ReadStatus GetReadStatus()
        {
            return readStatus;
        }

        public bool IsConnected()
        {
            return SerialPort.IsOpen;
        }

        public void ReadRequest()
        {
            readStatus = ReadStatus.Waiting;
            if (SerialPort.IsOpen)
            {
                string cmd = "FETC?";
                SerialPort.DiscardInBuffer();
                SerialPort.Write(cmd + "\r\n");
                LogHelper.Log.I_Log("测值指令：".tr() + cmd);
            }
            else
            {
                LogHelper.Log.I_Log("电表未连接".tr());
            }
        }

        public void ResetReadStatus()
        {
            readStatus = ReadStatus.None;
        }

        public void SetFrequency(string frequency)
        {
            ///发频率
            string sendstr = "FREQ " + frequency + "HZ";
            LogHelper.Log.I_Log("设定测定频率: ".tr() + sendstr);
            SerialPort.Write(sendstr + "\r\n");
        }

        public void SetType(TestType type, string frequency)
        {
            switch (type)
            {
                case TestType.None:
                    break;
                case TestType.R:
                    RESTypeCommand(frequency);
                    break;
                case TestType.C:
                    CAPTypeCommand(frequency);
                    break;
                case TestType.H:
                    break;
            }
        }

        public double Value()
        {
            return ReadData;
        }

        /// <summary>
        /// 电阻类型指令
        /// </summary>
        private void RESTypeCommand(string frequency)
        {
            string sendstrV = "VOLT 2";
            LogHelper.Log.I_Log("设定电阻类型：".tr() + sendstrV);
            SerialPort.Write(sendstrV + "\r\n");
            SetFrequency(frequency);
            string type = "FUNC:IMP RX";
            LogHelper.Log.I_Log(type);
            SerialPort.Write(type + "\r\n");
        }

        /// <summary>
        /// 电容类型指令
        /// </summary>
        private void CAPTypeCommand(string frequency)
        {
            bool WriteVOLT = true;
            //根据电压值发
            string sendstrV = WriteVOLT ? "VOLT 1" : "VOLT 2";   //5mv-2v
            LogHelper.Log.I_Log("设定电容类型：".tr() + sendstrV);
            SerialPort.Write(sendstrV + "\r\n");
            SetFrequency(frequency);
            string type = "FUNC:IMP CPQ";
            LogHelper.Log.I_Log(type);
            SerialPort.Write(type + "\r\n");
        }
    }
}
