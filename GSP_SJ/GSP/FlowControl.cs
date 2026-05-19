using BrowApp;
using BrowApp.IO;
using BrowApp.Language;
using BrowLib;
using CKVisionAppNet;
using GSP.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSP
{
    public class FlowControl
    {
        Algorithm Algorithm = new Algorithm();
        /// <summary>
        /// 流程扫描线程停止标志
        /// </summary>
        public bool bStopWork = false;

        private BrowLib.KTimer IniTimer = new KTimer();

        private BrowLib.KTimer RunTimer = new KTimer();

        private static Bitmap SharedImage;
        /// <summary>
        /// 单次检测时间
        /// </summary>
        public static Stopwatch Cycle_Timer = new Stopwatch();
        public static double Cycle_Time;

        public static double GetTime(double Timer)
        {
            return Math.Round(Timer / (double)Stopwatch.Frequency, 1);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public void StartThread()
        {
            FlowAlwaysThread = new Thread(AlwaysDoWork);
            FlowAlwaysThread.IsBackground = true;
            FlowAlwaysThread.Start();
        }
        /// <summary>
        /// 流程控制线程
        /// </summary>
        Thread FlowThread = null;

        /// <summary>
        /// 流程控制线程
        /// </summary>
        Thread FlowAlwaysThread = null;

        /// <summary>
        /// 手动线程
        /// </summary>
        private Task ManualTask;

        /// <summary>
        /// 停止手动线程
        /// </summary>
        public CancellationTokenSource StopManualTask = new CancellationTokenSource();
        /// <summary>
        /// 执行手动流程
        /// </summary>
        /// <param name="ation"></param>
        public void ExecuteManual(Action ation)
        {
            if (ManualTask != null)
                if (ManualTask.Status == TaskStatus.Running)
                    return;
            StopManualTask.Dispose();
            StopManualTask = new CancellationTokenSource();
            ManualTask = Task.Factory.StartNew(ation, StopManualTask.Token);
        }
        /// <summary>
        /// 检查手动流程是否执行
        /// </summary>
        /// <returns></returns>
        public bool IsManualRun()
        {
            if (ManualTask != null)
                if (ManualTask.Status == TaskStatus.Running)
                    return true;
            return false;
        }
        /// <summary>
        /// 跨线程更新UI控件信息
        /// </summary>
        /// <param name="control"></param>
        /// <param name="action"></param>
        public void RefreshThreadUI(Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                Action refreshUI = new Action(action);
                control.Invoke(refreshUI);
            }
            else { action.Invoke(); }
        }
        /// <summary>
        /// 停止线程
        /// </summary>
        public void StopThread()
        {
            if (FlowAlwaysThread != null)
            {
                bStopWork = true;
                FlowAlwaysThread.Join();
            }
        }
        /// <summary>
        /// 系统后台线程
        /// </summary>
        public void AlwaysDoWork()
        {
            while (!bStopWork)
            {
                BrowApp.APP.CheckMotAlarm();
                BrowLib.Controller.IoRefEventHandler?.Invoke(null, new EventArgs());
                Thread.Sleep(5);
            }
        }
        /// <summary>
        /// 获取偏差
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="OffsetX"></param>
        /// <param name="OffsetY"></param>
        /// <returns></returns>
        private Tuple<double, double> GetOffset(double X, double Y, double OffsetX, double OffsetY)
        {
            double OUT_X; double OUT_Y;
            OUT_X = X - OffsetX;
            OUT_Y = Y - OffsetY;
            return Tuple.Create<double, double>(OUT_X, OUT_Y);
        }
        /// <summary>
        /// 整机回零流程
        /// </summary>
        public void Home()
        {
            BrowLib.KTimer HomeTimer = new KTimer();
            BrowLib.GLB.HomeFlag = false;
            int HomeStep = 0;
            bool btnhome = false;
            BrowLib.Controller.SetLimit(false);
            while (!Global.StopFlag && !btnhome)
            {
                if (GLB.HomeFlag)
                {
                    BrowLib.Controller.CardAPI.StopAxis();
                    btnhome = true;
                    return;
                }
                switch (Global.Model)
                {
                    case 0:
                        switch (HomeStep)
                        {
                            case 0:
                                Global.Z轴.Home(10000);
                                HomeStep++;
                                break;
                            case 1:
                                if (Global.Z轴.HomeState)
                                {
                                    HomeStep++;
                                }
                                break;
                            case 2:
                                if (Global.R轴.ORG())
                                {
                                    Global.R轴.JOP(1, 100);
                                    HomeStep++;
                                }
                                else
                                {
                                    HomeStep = 4;
                                }
                                break;
                            case 3:
                                if (!Global.R轴.ORG())
                                {
                                    Global.R轴.AxisStop();
                                    HomeStep++;
                                }
                                break;
                            case 4:
                                if (!Global.左夹爪轴.ORG())
                                {
                                    Global.左夹爪轴.JOP(0, 5);
                                    HomeStep++;
                                }
                                else
                                {
                                    HomeStep = 6;
                                }
                                break;
                            case 5:
                                if (Global.左夹爪轴.ORG())
                                {
                                    Global.左夹爪轴.EmgAxisStop();
                                    HomeStep++;
                                }
                                break;

                            case 6:
                                if (!Global.右夹爪轴.ORG())
                                {
                                    Global.右夹爪轴.JOP(0, 5);
                                    HomeStep++;
                                }
                                else
                                {
                                    HomeStep = 8;
                                }
                                break;
                            case 7:
                                if (Global.右夹爪轴.ORG())
                                {
                                    Global.右夹爪轴.EmgAxisStop();
                                    HomeStep++;
                                }
                                break;
                            case 8:
                                Thread.Sleep(100);
                                Global.X轴.Home(10000);
                                Global.Y轴.Home(10000);
                                Global.R轴.Home(10000);
                                Global.左夹爪轴.Home(10000);
                                Global.右夹爪轴.Home(10000);
                                HomeStep++;
                                break;

                            case 9:
                                if (Global.X轴.HomeState &&
                                Global.Y轴.HomeState &&
                                 Global.R轴.HomeState &&
                                 Global.左夹爪轴.HomeState &&
                                 Global.右夹爪轴.HomeState)
                                {
                                    HomeStep++;
                                }
                                break;
                            case 10:
                                BrowLib.Controller.OutPort["顶升气缸_OUT"].Off();
                                Thread.Sleep(500);
                                HomeStep++;
                                break;
                            case 11:
                                HomeStep = 0;
                                btnhome = true;
                                BrowLib.Controller.SetLimit(true, "X轴", "Y轴", "Z轴", "R轴", "左夹爪轴", "右夹爪轴");
                                BrowLib.Controller.SetelMode(0, "Y轴");
                                Global.SystemInitialOk = true;
                                BrowLib.Controller.OutPort["X轴回零完成"].On();
                                BrowLib.Controller.OutPort["Y轴回零完成"].On();
                                Thread.Sleep(1000);
                                BrowLib.Controller.OutPort["X轴回零完成"].Off();
                                BrowLib.Controller.OutPort["Y轴回零完成"].Off();
                                APP.Log.I_Log("回零完成信号发送完成");
                                break;
                        }
                        break;//FAI993
                    case 1://FAI991
                    case 2:
                        switch (HomeStep)
                        {
                            case 0:
                                Global.Z轴.Home(10000);
                                HomeStep++;
                                break;
                            case 1:
                                if (Global.Z轴.HomeState)
                                {
                                    HomeStep++;
                                }
                                break;
                            case 2:
                                if (Global.R轴.ORG())
                                {
                                    Global.R轴.JOP(1, 100);
                                    HomeStep++;
                                }
                                else
                                {
                                    HomeStep = 4;
                                }
                                break;
                            case 3:
                                if (!Global.R轴.ORG())
                                {
                                    Global.R轴.AxisStop();
                                    HomeStep++;
                                }
                                break;
                            case 4:
                                if (!Global.左夹爪轴.ORG())
                                {
                                    Global.左夹爪轴.JOP(0, 5);
                                    HomeStep++;
                                }
                                else
                                {
                                    HomeStep = 6;
                                }
                                break;
                            case 5:
                                if (Global.左夹爪轴.ORG())
                                {
                                    Global.左夹爪轴.EmgAxisStop();
                                    HomeStep++;
                                }
                                break;

                            case 6:
                                if (!Global.右夹爪轴.ORG())
                                {
                                    Global.右夹爪轴.JOP(0, 5);
                                    HomeStep++;
                                }
                                else
                                {
                                    HomeStep = 8;
                                }
                                break;
                            case 7:
                                if (Global.右夹爪轴.ORG())
                                {
                                    Global.右夹爪轴.EmgAxisStop();
                                    HomeStep++;
                                }
                                break;
                            case 8:
                                Thread.Sleep(100);
                                Global.X轴.Home(10000);
                                Global.Y轴.Home(10000);
                                Global.R轴.Home(10000);
                                Global.左夹爪轴.Home(10000);
                                Global.右夹爪轴.Home(10000);
                                HomeStep++;
                                break;
                            case 9:
                                if (Global.X轴.HomeState &&
                                Global.Y轴.HomeState &&
                                 Global.R轴.HomeState &&
                                 Global.左夹爪轴.HomeState &&
                                 Global.右夹爪轴.HomeState)
                                {

                                    HomeStep++;
                                }
                                break;
                            case 10:
                                BrowLib.Controller.OutPort["A轨阻挡2_OUT5"].On();
                                BrowLib.Controller.OutPort["顶升气缸_OUT"].Off();
                                Thread.Sleep(500);
                                HomeStep++;
                                break;
                            case 11:
                                Global.皮带.JOP(0, 20);
                                HomeTimer.Restart();
                                HomeStep++;
                                break;
                            case 12:
                                if (BrowLib.Controller.InPort["入口感应光电_IN"].IsOn())
                                {
                                    Global.皮带.AxisStop();
                                    HomeStep++;
                                }
                                else if (HomeTimer.IsOn(3000))
                                {
                                    Global.皮带.AxisStop();
                                    HomeStep++;
                                }
                                break;
                            case 13:
                                //Global.调宽.PMove(5, 200,5,0);
                                //Thread.Sleep(500);
                                if (!BrowLib.Controller.InPort["入口感应光电_IN"].IsOn() && !BrowLib.Controller.InPort["阻挡感应光电_IN"].IsOn())
                                {
                                    Global.调宽.Home(10000);
                                    HomeStep++;
                                }
                                else
                                {
                                    APP.Tip.ShowTip(0, "警告".tr(), "轨道有板不允许轨道归零,请清理轨道重新启动设备归零".tr(), "确定".tr());
                                    HomeStep = 0;
                                    btnhome = true;
                                    Global.SystemInitialOk = false;
                                }
                                break;
                            case 14:
                                if (Global.调宽.HomeState)
                                {
                                    HomeStep++;
                                }
                                break;
                            case 15:
                                HomeStep = 0;
                                btnhome = true;
                                BrowLib.Controller.SetLimit(true, "X轴", "Y轴", "Z轴", "R轴", "左夹爪轴", "右夹爪轴");
                                BrowLib.Controller.SetelMode(0, "Y轴");
                                Global.SystemInitialOk = true;
                                BrowLib.Controller.OutPort["X轴回零完成"].On();
                                BrowLib.Controller.OutPort["Y轴回零完成"].On();
                                Thread.Sleep(1000);
                                BrowLib.Controller.OutPort["X轴回零完成"].Off();
                                BrowLib.Controller.OutPort["Y轴回零完成"].Off();
                                APP.Log.I_Log("回零完成信号发送完成");
                                break;
                        }
                        break;//860
                    case 3://FAI860P
                        switch (HomeStep)
                        {
                            case 0:
                                Global.Z轴.Home(10000);
                                HomeStep++;
                                APP.Log.I_Log("HomeStep" + HomeStep.ToString());
                                break;
                            case 1:
                                if (Global.Z轴.HomeState)
                                {
                                    HomeStep++;
                                    APP.Log.I_Log("HomeStep" + HomeStep.ToString());
                                }
                                break;
                            case 2:
                                if (Global.R轴.ORG())
                                {
                                    Global.R轴.JOP(1, 100);
                                    HomeStep++;
                                    APP.Log.I_Log("HomeStep" + HomeStep.ToString());
                                }
                                else
                                {
                                    HomeStep = 4;
                                    APP.Log.I_Log("HomeStep" + HomeStep.ToString());
                                }
                                break;
                            case 3:
                                if (!Global.R轴.ORG())
                                {
                                    Global.R轴.AxisStop();
                                    HomeStep++;
                                    APP.Log.I_Log("HomeStep" + HomeStep.ToString());
                                }
                                break;
                            case 4:
                                if (!Global.左夹爪轴.ORG())
                                {
                                    Global.左夹爪轴.JOP(0, 5);
                                    HomeStep++;
                                    APP.Log.I_Log("HomeStep" + HomeStep.ToString());
                                }
                                else
                                {
                                    HomeStep = 6;
                                    APP.Log.I_Log("HomeStep" + HomeStep.ToString());
                                }
                                break;
                            case 5:
                                if (Global.左夹爪轴.ORG())
                                {
                                    Global.左夹爪轴.EmgAxisStop();
                                    HomeStep++;
                                    APP.Log.I_Log("HomeStep" + HomeStep.ToString());
                                }
                                break;

                            case 6:
                                if (!Global.右夹爪轴.ORG())
                                {
                                    Global.右夹爪轴.JOP(0, 5);
                                    HomeStep++;
                                    APP.Log.I_Log("HomeStep" + HomeStep.ToString());
                                }
                                else
                                {
                                    HomeStep = 8;
                                    APP.Log.I_Log("HomeStep" + HomeStep.ToString());
                                }
                                break;
                            case 7:
                                if (Global.右夹爪轴.ORG())
                                {
                                    Global.右夹爪轴.EmgAxisStop();
                                    HomeStep++;
                                    APP.Log.I_Log("Home" + HomeStep.ToString());
                                }
                                break;
                            case 8:
                                Thread.Sleep(100);
                                Global.X轴.Home(10000);
                                Global.Y轴.Home(10000);
                                Global.R轴.Home(10000);
                                Global.左夹爪轴.Home(10000);
                                Global.右夹爪轴.Home(10000);
                                Global.顶升轴.Home(10000);
                                HomeStep++;
                                APP.Log.I_Log("HomeStep" + HomeStep.ToString());
                                break;
                            case 9:
                                if (Global.X轴.HomeState &&
                                Global.Y轴.HomeState &&
                                 Global.R轴.HomeState &&
                                 Global.左夹爪轴.HomeState &&
                                 Global.右夹爪轴.HomeState &&
                                 Global.顶升轴.HomeState)
                                {
                                    HomeStep++;
                                    APP.Log.I_Log("HomeStep" + HomeStep.ToString());
                                }
                                break;
                            case 10:
                                BrowLib.Controller.OutPort["阻挡_OUT"].On();
                                BrowLib.Controller.OutPort["顶升气缸_OUT"].Off();
                                Thread.Sleep(500);
                                APP.Log.I_Log("HomeStep" + HomeStep.ToString());

                                HomeStep++;
                                break;
                            case 11:
                                Global.皮带.JOP(0, 20);
                                HomeTimer.Restart();
                                APP.Log.I_Log("HomeStep" + HomeStep.ToString());
                                HomeStep++;
                                break;
                            case 12:
                                if (BrowLib.Controller.InPort["入口感应光电_IN"].IsOn())
                                {
                                    Global.皮带.AxisStop();
                                    HomeStep++;
                                    APP.Log.I_Log("HomeStep" + HomeStep.ToString());
                                }
                                else if (HomeTimer.IsOn(5000))
                                {
                                    Global.皮带.AxisStop();
                                    HomeStep++;

                                }
                                break;
                            case 13:
                                //Global.调宽.PMove(5, 200,5,0);
                                //Thread.Sleep(500);
                                if (!BrowLib.Controller.InPort["入口感应光电_IN"].IsOn() && !BrowLib.Controller.InPort["阻挡感应光电_IN"].IsOn())
                                {
                                    Global.调宽.Home(10000);
                                    APP.Log.I_Log("HomeStep" + HomeStep.ToString());

                                    HomeStep++;
                                }
                                else
                                {
                                    APP.Tip.ShowTip(0, "警告".tr(), "轨道有板不允许轨道归零,请清理轨道重新启动设备归零".tr(), "确定".tr());
                                    HomeStep = 0;
                                    btnhome = true;
                                    Global.SystemInitialOk = false;
                                }
                                break;
                            case 14:
                                if (Global.调宽.HomeState)
                                {
                                    HomeStep++;
                                }
                                break;
                            case 15:
                                HomeStep = 0;
                                btnhome = true;
                                BrowLib.Controller.SetLimit(true, "X轴", "Y轴", "Z轴", "R轴", "左夹爪轴", "右夹爪轴");
                                BrowLib.Controller.SetelMode(0, "Y轴");
                                Global.SystemInitialOk = true;
                                BrowLib.Controller.OutPort["X轴回零完成"].On();
                                BrowLib.Controller.OutPort["Y轴回零完成"].On();
                                Thread.Sleep(1000);
                                BrowLib.Controller.OutPort["X轴回零完成"].Off();
                                BrowLib.Controller.OutPort["Y轴回零完成"].Off();
                                APP.Log.I_Log("回零完成信号发送完成");
                                break;
                        }
                        break;//860P
                }
                Thread.Sleep(5);
            }
        }

        /// <summary>
        /// 手动进板流程
        /// </summary>
        public void Flow_In()
        {
            int InStep = 0;
            bool IniStart = true;
            BrowLib.KTimer Flow_InTimer = new KTimer();
            while (IniStart && !Global.StopFlag)
            {
                switch (InStep)
                {
                    case 0:
                        BrowLib.Controller.OutPort["阻挡_OUT"].On();
                        Thread.Sleep(500);
                        InStep++;
                        break;
                    case 1:
                        if (BrowLib.Controller.InPort["入口感应光电_IN"].IsOn())
                        {
                            APP.Log.I_Log("入口感应有板>>");
                            InStep++;
                        }
                        break;
                    case 2:
                        APP.Log.I_Log("启动皮带进板");
                        Global.皮带.JOP(1, 50);
                        Flow_InTimer.Restart();
                        InStep++;
                        break;
                    case 3:
                        if (BrowLib.Controller.InPort["阻挡感应光电_IN"].IsOn())
                        {
                            APP.Log.I_Log("阻挡光电感应板到位>>");
                            Global.皮带.ChangSpeed(5);
                            Global.皮带.JOP(1, 5);//皮带减速
                            Thread.Sleep((int)Global.Systemdata.InDaytime);
                            Global.皮带.AxisStop();
                            InStep++;
                        }
                        else if (Flow_InTimer.IsOn(10000))
                        {
                            Global.皮带.AxisStop();
                            APP.Tip.ShowTip(0, "警告".tr(), "进板超时！！！", "确定".tr());
                            Global.TcpClass.Send("M:Flow_In_NG");
                            IniStart = false;
                            InStep = 0;
                            return;
                        }
                        break;
                    case 4:
                        Global.调宽.PMove(10, 1000, -1 * Global.Systemdata.Trackoffset, 0);
                        InStep++;
                        break;
                    case 5:
                        if (Global.调宽.Runing())
                        {
                            InStep++;
                        }
                        break;
                    case 6:
                        APP.Log.I_Log("A轨顶升双气缸顶起>>");
                        BrowLib.Controller.OutPort["顶升气缸_OUT"].On();
                        Flow_InTimer.Restart();
                        InStep++;
                        break;
                    case 7:
                        if (BrowLib.Controller.InPort["顶升上位_IN"].IsOn(100))
                        {
                            APP.Log.I_Log("A轨顶升双气缸顶起到位>>");
                            Global.TcpClass.Send("M:Flow_In_OK");
                            IniStart = false;
                            InStep = 0;
                        }
                        else if (Flow_InTimer.IsOn(1000))
                        {
                            APP.Log.I_Log("A轨顶升双气缸顶起超时>>");
                            Global.TcpClass.Send("M:Flow_In_NG");
                            IniStart = false;
                            InStep = 0;
                        }
                        break;
                }
                Thread.Sleep(2);
            }
        }
        /// <summary>
        /// 出板流程
        /// </summary>
        public void Flow_Out()
        {
            int OutStep = 0;
            bool IniStart = true;
            BrowLib.KTimer Flow_OutTimer = new KTimer();
            while (IniStart && !Global.StopFlag)
            {
                switch (Global.Model)
                {
                    case 0://离线
                        switch (OutStep)
                        {
                            case 0:
                                Global.Z轴.PMove(50, 3000, Global.Systemdata.StopPos.Zpos, 1);
                                OutStep++;
                                break;
                            case 1:
                                if (Global.Z轴.GetPrfPos() - Global.Systemdata.StopPos.Zpos < 0.01)
                                {
                                    if (Global.Z轴.Runing())
                                    {
                                        Thread.Sleep(20);
                                        Global.R轴.PMove(Global.RunRVel, Global.RunRAcc, 0, 1);
                                        OutStep++;
                                    }
                                }
                                break;
                            case 2:
                                BrowLib.Controller.OutPort["顶升气缸_OUT"].Off();
                                Thread.Sleep(200);
                                OutStep++;
                                break;
                            case 3:
                                //回停止位
                                BrowLib.Controller.MotionAPI.LinXyMoveA(200, 3000, Global.Systemdata.StopPos.Xpos, Global.Systemdata.StopPos.Ypos, 1);
                                OutStep++;
                                break;
                            case 4:
                                if (BrowLib.Controller.MotionAPI.LinXYRuningA())
                                {
                                    Global.R轴.PMove(Global.RunRVel, Global.RunRAcc, 0, 1);
                                    Global.左夹爪轴.PMove(10, 500, 0, 1);
                                    Global.右夹爪轴.PMove(10, 500, 0, 1);
                                    OutStep++;
                                }
                                break;
                            case 5:
                                if (BrowLib.Controller.MotionAPI.LinXYRuningA() && Global.R轴.Runing() && Global.左夹爪轴.Runing() && Global.右夹爪轴.Runing())
                                {
                                    IniStart = false;
                                    Global.TcpClass.Send("M:Flow_OUT_OK");
                                    OutStep = 0;
                                }
                                break;
                        }
                        break;
                    case 1://在线
                    case 2://860
                    case 3://860P
                        switch (OutStep)
                        {
                            case 0:
                                Global.Z轴.PMove(Global.RunZVel, Global.RunZAcc, 2, 1);
                                OutStep++;
                                break;
                            case 1:
                                if (Global.Z轴.GetPrfPos() - 2 < 0.01)
                                {
                                    if (Global.Z轴.Runing())
                                    {
                                        Thread.Sleep(20);
                                        Global.R轴.PMove(Global.RunRVel, Global.RunRAcc, 0, 1);
                                        OutStep++;
                                    }
                                }
                                break;
                            case 2:
                                if (Global.R轴.GetPrfPos() - 0 < 0.01)
                                {
                                    if (Global.R轴.Runing())
                                    {
                                        Thread.Sleep(20);
                                        OutStep++;
                                    }
                                }
                                break;
                            case 3:
                                Global.调宽.PMove(10, 1000, Global.Systemdata.Trackoffset, 0);
                                Global.顶升轴.PMove(20, 1000, 0, 1);
                                BrowLib.Controller.OutPort["顶升气缸_OUT"].Off();
                                Thread.Sleep(500);
                                OutStep++;
                                break;
                            case 4:
                                if (Global.调宽.Runing())
                                {
                                    Global.皮带.JOP(0, 20);
                                    Flow_OutTimer.Restart();
                                    OutStep++;
                                }
                                break;
                            case 5:
                                if (BrowLib.Controller.InPort["入口感应光电_IN"].IsOn())
                                {
                                    Global.皮带.AxisStop();
                                    IniStart = false;
                                    OutStep = 0;
                                }
                                else if (Flow_OutTimer.IsOn(10000))
                                {
                                    Global.皮带.AxisStop();
                                    APP.Tip.ShowTip(1, "警告".tr(), "出板超时".tr(), "确定".tr());
                                    Global.TcpClass.Send("M:Flow_OUT_NG");
                                    IniStart = false;
                                }
                                break;
                            case 6:
                                if (BrowLib.Controller.InPort["入口感应光电_IN"].IsOff())
                                {
                                    IniStart = false;
                                    Global.TcpClass.Send("M:Flow_OUT_OK");
                                    OutStep = 0;
                                }
                                break;
                        }
                        break;
                }
                Thread.Sleep(2);
            }
        }
        public void GoToMark()
        {
            double Mak_X = 0, Mak_Y = 0, Mak_Z = 0;
            int MakNum = 0;
            double DMak1_X = 0, DMak1_Y = 0, DMak2_X = 0, DMak2_Y = 0;
            int GoToMarkStep = 0;
            bool IniStart = true;
            while (IniStart && !Global.StopFlag)
            {
                switch (GoToMarkStep)
                {
                    case 0:
                        Global.Z轴.PMove(Global.RunZVel, Global.RunZAcc, Global.Systemdata.SafeHigh, 1);
                        GoToMarkStep++;
                        break;
                    case 1:
                        if (Global.Z轴.GetPrfPos() - Global.Systemdata.SafeHigh < 0.02)
                        {
                            if (Global.Z轴.Runing())
                            {
                                Global.R轴.PMove(Global.RunZVel, Global.RunZAcc, 0, 1);
                                GoToMarkStep++;
                            }
                        }
                        break;
                    case 2:
                        if (Global.R轴.GetPrfPos() - 0 < 0.02)
                        {
                            if (Global.R轴.Runing())
                            {
                                GoToMarkStep++;
                            }
                        }
                        break;
                    case 3:
                        if (MakNum == 0)
                        {
                            Mak_X = Global.Parm.Mak1Pos.Xpos;
                            Mak_Y = Global.Parm.Mak1Pos.Ypos;
                            Mak_Z = Global.CamHeight;
                            APP.Log.I_Log("Go MARK1");
                        }
                        else if (MakNum == 1)
                        {
                            Mak_X = Global.Parm.Mak2Pos.Xpos;
                            Mak_Y = Global.Parm.Mak2Pos.Ypos;
                            Mak_Z = Global.CamHeight;
                            APP.Log.I_Log("Go MARK2");
                        }
                        BrowLib.Controller.MotionAPI.LinXyMoveA(Global.RunXYVel, Global.RunXYAcc, Mak_X, Mak_Y, 1, 2);
                        GoToMarkStep++;
                        break;
                    case 4:
                        if (BrowLib.Controller.MotionAPI.LinXYRuningA())
                        {
                            GoToMarkStep++;
                        }
                        break;
                    case 5:
                        Global.Z轴.PMove(Global.RunZVel, Global.RunZAcc, Mak_Z, 1);
                        GoToMarkStep++;
                        break;
                    case 6:
                        if (Global.Z轴.GetPrfPos() - Mak_Z < 0.01)
                        {
                            if (Global.Z轴.Runing())
                            {
                                Thread.Sleep(100);
                                GoToMarkStep++;
                            }
                        }
                        break;
                    case 7:
                        Global.VisionApp.StopRunProc("Task5");
                        if (MakNum == 0)
                        {
                            Global.VisionApp.SetToolData("Task3", "定义变量", 2102, 0, "false", 0);
                            Global.VisionApp.SetToolData("Task3", "定义变量", 2110, 0, "1", 1);
                            Global.VisionApp.ExecuteProc("Task3", 0);
                        }
                        else if (MakNum == 1)
                        {
                            Global.VisionApp.SetToolData("Task3", "定义变量", 2102, 0, "false", 0);
                            Global.VisionApp.SetToolData("Task3", "定义变量", 2110, 0, "2", 1);
                            Global.VisionApp.ExecuteProc("Task3", 1);
                        }
                        //触发拍照
                        APP.Log.I_Log("Trigger Cam");
                        Thread.Sleep(10);
                        GoToMarkStep++;
                        break;
                    case 8:
                        if (Global.VisionApp.EndProc["Task3"])
                        {

                            if (MakNum == 0)
                            {
                                if (Global.VisionApp.RunState("Task3", "执行结果"))
                                {
                                    DMak1_X = Global.VisionApp.GetDblValue("Task3", "定义变量", 2103, 0);
                                    DMak1_Y = Global.VisionApp.GetDblValue("Task3", "定义变量", 2104, 0);
                                    MakNum++;
                                    GoToMarkStep = 3;
                                }
                                else
                                {
                                    MarkRectify mark = new MarkRectify();
                                    mark.ShowDialog();
                                    if (mark.Isok)
                                    {
                                        DMak1_X = mark.Dx;
                                        DMak1_Y = mark.Dy;
                                        MakNum++;
                                        GoToMarkStep = 3;
                                    }
                                    else
                                    {
                                        APP.Tip.ShowTip(1, "警告".tr(), "Mark1定位失败".tr(), "确定".tr());
                                        Global.StopFlag = true;
                                        Global.TcpClass.Send("M:GoMark_NG");
                                        return;
                                    }
                                }
                            }
                            else if (MakNum == 1)
                            {
                                if (Global.VisionApp.RunState("Task3", "执行结果"))
                                {
                                    DMak2_X = Global.VisionApp.GetDblValue("Task3", "定义变量", 2103, 0);
                                    DMak2_Y = Global.VisionApp.GetDblValue("Task3", "定义变量", 2104, 0);
                                    MakNum = 0;
                                    GoToMarkStep++;
                                }
                                else
                                {
                                    MarkRectify mark = new MarkRectify();
                                    mark.ShowDialog();
                                    if (mark.Isok)
                                    {
                                        DMak2_X = mark.Dx;
                                        DMak2_Y = mark.Dy;
                                        MakNum++;
                                        GoToMarkStep++;
                                    }
                                    else
                                    {
                                        APP.Tip.ShowTip(1, "警告".tr(), "Mark2定位失败".tr(), "确定".tr());
                                        Global.StopFlag = true;
                                        Global.TcpClass.Send("M:GoMark_NG");
                                        return;
                                    }
                                }
                            }
                        }
                        break;
                    case 9:
                        Thread.Sleep(100);
                        VisionGlobal.Mak1_X = Global.Parm.Mak1Pos.Xpos - DMak1_X;//纠偏Mask1X位置
                        VisionGlobal.Mak1_Y = Global.Parm.Mak1Pos.Ypos - DMak1_Y;//纠偏Mask1Y位置

                        VisionGlobal.Mak2_X = Global.Parm.Mak2Pos.Xpos - DMak2_X;//纠偏Mask2X位置
                        VisionGlobal.Mak2_Y = Global.Parm.Mak2Pos.Ypos - DMak2_Y;//纠偏Mask2Y位置

                        VisionGlobal.Mak1_X = Math.Round(VisionGlobal.Mak1_X, 4);
                        VisionGlobal.Mak1_Y = Math.Round(VisionGlobal.Mak1_Y, 4);
                        VisionGlobal.Mak2_X = Math.Round(VisionGlobal.Mak2_X, 4);
                        VisionGlobal.Mak2_Y = Math.Round(VisionGlobal.Mak2_Y, 4);
                        GoToMarkStep++;
                        break;
                    case 10:
                        Global.TcpClass.Send("M:GoMark_OK");
                        IniStart = false;
                        GoToMarkStep = 0;
                        break;
                }
            }
        }
        public void M_goTo(string StrCode, string Line, string IsCCD)
        {
            int goToStep = 0;
            bool IniStart = true;

            double X = 0;
            double Y = 0;
            double R = 0;
            double LeftPos = 0;
            double ZHight = 0;
            double yX = 0, yY = 0, offset_X = 0, offset_Y = 0, offsetx = 0, offsety = 0;
            while (IniStart && !Global.StopFlag)
            {
                switch (goToStep)
                {
                    case 0:
                        switch (Line)
                        {
                            case "2":
                                BrowLib.Controller.OutPort["四线切换两线"].On();
                                break;
                            case "4":
                                BrowLib.Controller.OutPort["四线切换两线"].Off();
                                break;
                        }
                        goToStep++;
                        break;
                    case 1:
                        Global.Z轴.PMove(Global.RunZVel, Global.RunZAcc, Global.Systemdata.SafeHigh, 1);
                        goToStep++;
                        break;
                    case 2:
                        if (Global.Z轴.GetPrfPos() - Global.Systemdata.SafeHigh < 0.02)
                        {
                            if (Global.Z轴.Runing())
                            {
                                goToStep++;
                            }
                        }
                        break;
                    case 3:
                        DataRow[] dataRows = Global.BomData.Select("位置号='" + StrCode + "'");
                        X = Convert.ToDouble(dataRows[0]["原始X坐标"].ToString());
                        Y = Convert.ToDouble(dataRows[0]["原始Y坐标"].ToString());
                        R = Convert.ToDouble(dataRows[0]["原始方向"].ToString());
                        offsetx = Convert.ToDouble(dataRows[0]["X坐标调整"].ToString());
                        offsety = Convert.ToDouble(dataRows[0]["Y坐标调整"].ToString());

                        string Type = dataRows[0]["尺寸"].ToString();
                        //角度计算
                        R = Algorithm.GetAngle(R);
                        //象限坐标系平移  
                        X = X + VisionGlobal.TranslationX;
                        Y = Y + VisionGlobal.TranslationY;
                        //拼版偏移
                        X = X + Global.Parm.PbXoffset;
                        Y = Y + Global.Parm.PbYoffset;


                        LeftPos = Global.GetSize(Type);//获取开口大小
                        ZHight = Global.GetHight(Type);//获取下针高度

                        int Ecode = Algorithm.MakAlgorithm(0, X, Y, VisionGlobal.Mak1_X, VisionGlobal.Mak1_Y,
                        VisionGlobal.Mak2_X, VisionGlobal.Mak2_Y, VisionGlobal.bMak1_X + VisionGlobal.TranslationX,
                        VisionGlobal.bMak1_Y + VisionGlobal.TranslationY, VisionGlobal.bMak2_X + VisionGlobal.TranslationX,
                        VisionGlobal.bMak2_Y + VisionGlobal.TranslationY, out yX, out yY);
                        if (Ecode != 0)
                        {
                            APP.Tip.ShowTip(1, "警告".tr(), "Mark绑定数据出错".tr(), "确定".tr());
                            Global.StopFlag = true;
                            return;
                        }
                        //整体补偿
                        yX = yX + Global.Parm.Offset_X;
                        yY = yY + Global.Parm.Offset_Y;
                        goToStep++;
                        break;
                    case 4:
                        switch (IsCCD)
                        {
                            case "相机":
                                goToStep = 5;
                                break;
                            case "下针":
                                goToStep = 50;
                                break;
                        }
                        break;
                    case 5:
                        BrowLib.Controller.MotionAPI.LinXyMoveA(Global.RunXYVel, Global.RunXYAcc, yX, yY, 1, 2);
                        goToStep++;
                        break;
                    case 6:
                        if (BrowLib.Controller.MotionAPI.LinXYRuningA())
                        {
                            ZHight = Global.Systemdata.Ccdhight;
                            goToStep++;
                        }
                        break;
                    case 7:
                        Global.Z轴.PMove(10, 1000, ZHight, 1);
                        goToStep++;
                        break;
                    case 8:
                        if (Global.Z轴.GetPrfPos() - ZHight < 0.02)
                        {
                            if (Global.Z轴.Runing())
                            {
                                Global.TcpClass.TCP_MStaart = false;
                                Global.TcpClass.Send("M:GoToOk\r\n");
                                goToStep++;
                            }
                        }
                        break;
                    case 9:
                        if (Global.TcpClass.TCP_MStaart)
                        {
                            string str1 = Global.TcpClass.TCP_Result.Trim();
                            string[] re1 = str1.Split(':');
                            if (re1[0] == "M")
                            {
                                if (re1[1] == "TestOk")
                                {
                                    goToStep++;
                                }
                            }
                        }
                        break;
                    case 10:
                        Global.Z轴.PMove(Global.RunZVel, Global.RunZAcc, Global.Systemdata.SafeHigh, 1);
                        goToStep++;
                        break;
                    case 11:
                        if (Global.Z轴.GetPrfPos() - Global.Systemdata.SafeHigh < 0.02)
                        {
                            if (Global.Z轴.Runing())
                            {
                                IniStart = false;
                                goToStep = 0;
                            }
                        }
                        break;
                    case 50:
                        //获取相机Vs针头便移值
                        Global.GetOffset(R, out offset_X, out offset_Y, Global.Is_DownCam);
                        APP.Log.I_Log("角度:" + R.ToString() + "旋转偏移dX:" + offset_X.ToString() + "旋转偏移dY:" + offset_Y.ToString());
                        Tuple<double, double> offset = this.GetOffset(yX, yY, offset_X, offset_Y);
                        yX = offset.Item1;
                        yY = offset.Item2;
                        BrowLib.Controller.MotionAPI.LinXyMoveA(Global.RunXYVel, Global.RunXYAcc, yX, yY, 1, 2);
                        Global.R轴.PMove(Global.RunRVel, Global.RunRAcc, R, 1);
                        Global.左夹爪轴.PMove(50, 2000, LeftPos, 1);
                        Global.右夹爪轴.PMove(50, 2000, LeftPos, 1);
                        goToStep++;
                        break;
                    case 51:
                        if (BrowLib.Controller.MotionAPI.LinXYRuningA())
                        {
                            if (Global.R轴.GetPrfPos() - R < 0.02 &&
                            Global.左夹爪轴.GetPrfPos() - LeftPos < 0.02 &&
                                Global.右夹爪轴.GetPrfPos() - LeftPos < 0.02)
                            {
                                if (Global.R轴.Runing() && Global.左夹爪轴.Runing() && Global.右夹爪轴.Runing())
                                {
                                    goToStep++;
                                }
                            }
                        }
                        break;
                    case 52:
                        Global.Z轴.PMove(Global.RunZVel, Global.RunZAcc, ZHight - 2, 1);
                        goToStep++; ;
                        break;
                    case 53:
                        if (Global.Z轴.GetPrfPos() - ZHight - 2 < 0.02)
                        {
                            if (Global.Z轴.Runing())
                            {
                                Global.Z轴.PMove(10, 1000, ZHight, 1);
                                goToStep++; ;
                            }
                        }
                        break;
                    case 54:
                        if (Global.Z轴.GetPrfPos() - ZHight < 0.02)
                        {
                            if (Global.Z轴.Runing())
                            {
                                Global.TcpClass.TCP_MStaart = false;
                                Global.TcpClass.Send("M:GoToOk\r\n");
                                goToStep++;
                            }
                        }
                        break;
                    case 55:
                        if (Global.TcpClass.TCP_MStaart)
                        {
                            string str1 = Global.TcpClass.TCP_Result.Trim();
                            string[] re1 = str1.Split(':');
                            if (re1[0] == "M")
                            {
                                if (re1[1] == "TestOk")
                                {
                                    goToStep++;
                                }
                            }
                        }
                        break;
                    case 56:
                        Global.Z轴.PMove(Global.RunZVel, Global.RunZAcc, Global.Systemdata.SafeHigh, 1);
                        goToStep++;
                        break;
                    case 57:
                        if (Global.Z轴.GetPrfPos() - Global.Systemdata.SafeHigh < 0.02)
                        {
                            if (Global.Z轴.Runing())
                            {
                                IniStart = false;
                                goToStep = 0;
                            }
                        }
                        break;
                }
                Thread.Sleep(5);
            }
        }
        public void Mccd_Goto(string StrCode, string Line)
        {
            int GoToStep = 0;
            bool IniStart = true;

            double X = 0, Y = 0, yR = 0, R = 0, Dx = 0, Dy = 0;
            double LeftPos = 0;
            double ZHight = 0, CcdHight = 0;
            double yX = 0, yY = 0, offset_X = 0, offset_Y = 0, offsetx = 0, offsety = 0, CenterdX = 0, CenterdY = 0;
            double Jqsize = 0;
            while (IniStart && !Global.StopFlag)
            {
                switch (GoToStep)
                {
                    case 0:
                        switch (Line)
                        {
                            case "2":
                                BrowLib.Controller.OutPort["四线切换两线"].On();
                                break;
                            case "4":
                                BrowLib.Controller.OutPort["四线切换两线"].Off();
                                break;
                        }
                        GoToStep++;
                        break;
                    case 1:
                        Global.Z轴.PMove(Global.RunZVel * 0.8, Global.RunZAcc * 0.8, Global.Systemdata.SafeHigh, 1);
                        GoToStep++;
                        break;
                    case 2:
                        if (Global.Z轴.GetPrfPos() - Global.Systemdata.SafeHigh < 0.02)
                        {
                            if (Global.Z轴.Runing())
                            {
                                GoToStep++;
                            }
                        }
                        break;
                    case 3:
                        DataRow[] dataRows = Global.BomData.Select("位置号='" + StrCode + "'");
                        X = Convert.ToDouble(dataRows[0]["原始X坐标"].ToString());
                        Y = Convert.ToDouble(dataRows[0]["原始Y坐标"].ToString());
                        yR = Convert.ToDouble(dataRows[0]["原始方向"].ToString());
                        offsetx = Convert.ToDouble(dataRows[0]["X坐标调整"].ToString());
                        offsety = Convert.ToDouble(dataRows[0]["Y坐标调整"].ToString());
                        R = Algorithm.GetAngle(yR);
                        APP.Log.I_Log("原始X坐标:" + X.ToString() + "原始Y坐标:" + Y.ToString() +
                            "原始方向:" + yR.ToString() + "X坐标调整:" + offsetx.ToString() + "Y坐标调整:" + offsety.ToString());
                        //象限坐标系平移  
                        X = X + VisionGlobal.TranslationX;
                        Y = Y + VisionGlobal.TranslationY;

                        //拼版偏移
                        X = X + Global.Parm.PbXoffset;
                        Y = Y + Global.Parm.PbYoffset;

                        string Type = dataRows[0]["尺寸"].ToString();

                        LeftPos = Global.GetSize(Type);//获取开口大小
                        ZHight = Global.GetHight(Type);//获取下针高度
                        Jqsize = Global.JQSize(Type);//获取下针高度

                        if (Global.Is_NoMark)
                        {
                            VisionGlobal.Mak1_X = Global.Parm.Mak1Pos.Xpos;//纠偏Mask1X位置
                            VisionGlobal.Mak1_Y = Global.Parm.Mak1Pos.Ypos;//纠偏Mask1Y位置

                            VisionGlobal.Mak2_X = Global.Parm.Mak2Pos.Xpos;//纠偏Mask2X位置
                            VisionGlobal.Mak2_Y = Global.Parm.Mak2Pos.Ypos;//纠偏Mask2Y位置
                        }
                        int Ecode = Algorithm.MakAlgorithm(0, X, Y, VisionGlobal.Mak1_X, VisionGlobal.Mak1_Y,
                        VisionGlobal.Mak2_X, VisionGlobal.Mak2_Y, VisionGlobal.bMak1_X + VisionGlobal.TranslationX,
                        VisionGlobal.bMak1_Y + VisionGlobal.TranslationY, VisionGlobal.bMak2_X + VisionGlobal.TranslationX,
                        VisionGlobal.bMak2_Y + VisionGlobal.TranslationY, out yX, out yY);
                        if (Ecode != 0)
                        {
                            APP.Tip.ShowTip(1, "警告".tr(), "Mark绑定数据出错".tr(), "确定".tr());
                            Global.StopFlag = true;
                            return;
                        }
                        //整体补偿
                        yX = yX + Global.Parm.Offset_X;
                        yY = yY + Global.Parm.Offset_Y;

                        //yX = yX - offsetx;
                        //yY = yY - offsety;
                        GoToStep++;
                        break;
                    case 4:
                        BrowLib.Controller.MotionAPI.LinXyMoveA(Global.RunXYVel, Global.RunXYAcc, yX, yY, 1, 2);
                        GoToStep++;
                        break;
                    case 5:
                        if (BrowLib.Controller.MotionAPI.LinXYRuningA())
                        {
                            CcdHight = Global.Systemdata.Ccdhight;
                            GoToStep++;
                        }
                        break;
                    case 6:
                        Global.Z轴.PMove(20, 1000, CcdHight, 1);
                        GoToStep++;
                        break;
                    case 7:
                        if (Global.Z轴.GetPrfPos() - CcdHight < 0.02)
                        {
                            if (Global.Z轴.Runing())
                            {
                                Thread.Sleep(50);
                                GoToStep++;
                            }
                        }
                        break;
                    case 8:
                        CalibFrom Calib = new CalibFrom();
                        Calib.Code = StrCode;
                        Calib.Angle = yR;
                        Calib.offsetX = offsetx;
                        Calib.offsetY = offsety;
                        Calib.ShowDialog();
                        Dx = Calib.Dx; Dy = Calib.Dy;
                        R = Algorithm.GetAngle(Calib.Angle);
                        if (Calib.RunFlag)
                        {
                            GoToStep++;
                        }
                        else
                        {
                            Global.TcpClass.Send("M:CCDSTOP");
                            Global.StopFlag = true;
                            return;
                        }
                        break;
                    case 9:
                        //获取相机Vs针头便移值
                        Global.GetOffset(R, out offset_X, out offset_Y, Global.Is_DownCam);
                        APP.Log.I_Log("角度:" + R.ToString() + "旋转偏移dX:" + offset_X.ToString() + "旋转偏移dY:" + offset_Y.ToString());
                        Tuple<double, double> offset = this.GetOffset(yX, yY, offset_X + Dx, offset_Y + Dy);
                        yX = offset.Item1;
                        yY = offset.Item2;
                        APP.Log.I_Log("目标X:" + yX.ToString() + "目标Y:" + yY.ToString());
                        Tuple<double, double> tuple = Global.coordinateCompensator.GetCompensatedPosition(yX, yY);
                        BrowLib.Controller.MotionAPI.LinXyMoveA(Global.RunXYVel, Global.RunXYAcc, tuple.Item1, tuple.Item2, 1, 2);
                        Global.R轴.PMove(Global.RunRVel, Global.RunRAcc, R, 1);
                        Global.左夹爪轴.PMove(10, 500, LeftPos, 1);
                        Global.右夹爪轴.PMove(10, 500, LeftPos, 1);
                        GoToStep++;
                        break;
                    case 10:
                        if (BrowLib.Controller.MotionAPI.LinXYRuningA())
                        {
                            if (Global.R轴.GetPrfPos() - R < 0.02 &&
                            Global.左夹爪轴.GetPrfPos() - LeftPos < 0.02 &&
                                Global.右夹爪轴.GetPrfPos() - LeftPos < 0.02)
                            {
                                if (Global.R轴.Runing() && Global.左夹爪轴.Runing() && Global.右夹爪轴.Runing())
                                {
                                    ZHight = ZHight + Global.Hoffset;
                                    APP.Log.I_Log("下针高度补偿:" + Global.Hoffset.ToString());
                                    GoToStep++;
                                }
                            }
                        }
                        break;
                    case 11:
                        Global.Z轴.PMove(300, 3000, ZHight - 2, 1);
                        GoToStep++; ;
                        break;

                    case 12:
                        if (Global.Z轴.GetPrfPos() - ZHight - 2 < 0.02)
                        {
                            if (Global.Z轴.Runing())
                            {
                                Global.Z轴.PMove(Global.Systemdata.buf_Zspeed, 500, ZHight, 1);
                                GoToStep++; ;
                            }
                        }
                        break;
                    case 13:
                        if (Global.Z轴.GetPrfPos() - ZHight < 0.02)
                        {
                            if (Global.Z轴.Runing())
                            {
                                if (Jqsize > 0 && Jqsize < 1)
                                {
                                    APP.Log.I_Log("执行夹紧动作:" + Jqsize.ToString());
                                    Global.左夹爪轴.PMove(10, 500, Jqsize, 0);
                                    Global.右夹爪轴.PMove(10, 500, Jqsize, 0);
                                    GoToStep++;
                                }
                                else
                                {
                                    Thread.Sleep(200);
                                    Global.TcpClass.TCP_MStaart = false;
                                    Global.TcpClass.Send("M:GoToOk" + ":" + Dx.ToString() + ":" + Dy.ToString() + "\r\n");
                                    GoToStep = 15;
                                }

                            }
                        }
                        break;
                    case 14:
                        if (Global.左夹爪轴.Runing() &&
                          Global.右夹爪轴.Runing())
                        {
                            Thread.Sleep(200);
                            Global.TcpClass.TCP_MStaart = false;
                            Global.TcpClass.Send("M:GoToOk" + ":" + Dx.ToString() + ":" + Dy.ToString() + "\r\n");
                            GoToStep++;
                        }
                        break;
                    case 15:
                        if (Global.TcpClass.TCP_MStaart)
                        {
                            string str1 = Global.TcpClass.TCP_Result.Trim();
                            string[] re1 = str1.Split(':');
                            if (re1[0] == "M")
                            {
                                if (re1[1] == "TestOk")
                                {
                                    Thread.Sleep(Global.Systemdata.TestDlay);
                                    if (Jqsize > 0 && Jqsize < 1)
                                    {
                                        APP.Log.I_Log("执行张开动作:" + Jqsize.ToString());
                                        Global.左夹爪轴.PMove(10, 500, -Jqsize, 0);
                                        Global.右夹爪轴.PMove(10, 500, -Jqsize, 0);
                                    }
                                    GoToStep++;
                                }
                            }
                        }
                        break;
                    case 16:
                        if (BrowLib.Controller.MotionAPI.LinXYRuningA())
                        {
                            if (Global.R轴.Runing() && Global.左夹爪轴.Runing() && Global.右夹爪轴.Runing())
                            {
                                GoToStep++;
                            }
                        }
                        break;
                    case 17:
                        Global.Z轴.PMove(100, 1000, Global.Systemdata.SafeHigh, 1);
                        GoToStep++;
                        break;
                    case 18:
                        if (Global.Z轴.GetPrfPos() - Global.Systemdata.SafeHigh < 0.02)
                        {
                            if (Global.Z轴.Runing())
                            {
                                IniStart = false;
                                GoToStep = 0;
                            }
                        }
                        break;
                }
                Thread.Sleep(3);
            }

        }

        public void Laser_Go(string StrCode)
        {
            int GoToStep = 0;
            bool IniStart = true;

            double X = 0, Y = 0, yR = 0, R = 0, Dx = 0, Dy = 0;
            double LeftPos = 0;
            double ZHight = 0, CcdHight = 0;
            double yX = 0, yY = 0, offset_X = 0, offset_Y = 0, offsetx = 0, offsety = 0;
            while (IniStart && !Global.StopFlag)
            {
                switch (GoToStep)
                {
                    case 0:
                        Global.Z轴.PMove(Global.RunZVel * 0.8, Global.RunZAcc * 0.8, Global.Systemdata.SafeHigh, 1);
                        GoToStep++;
                        break;
                    case 1:
                        if (Global.Z轴.GetPrfPos() - Global.Systemdata.SafeHigh < 0.02)
                        {
                            if (Global.Z轴.Runing())
                            {
                                GoToStep++;
                            }
                        }
                        break;
                    case 2:
                        DataRow[] dataRows = Global.BomData.Select("位置号='" + StrCode + "'");
                        X = Convert.ToDouble(dataRows[0]["原始X坐标"].ToString());
                        Y = Convert.ToDouble(dataRows[0]["原始Y坐标"].ToString());
                        yR = Convert.ToDouble(dataRows[0]["原始方向"].ToString());
                        offsetx = Convert.ToDouble(dataRows[0]["X坐标调整"].ToString());
                        offsety = Convert.ToDouble(dataRows[0]["Y坐标调整"].ToString());
                        R = Algorithm.GetAngle(yR);

                        //象限坐标系平移  
                        X = X + VisionGlobal.TranslationX;
                        Y = Y + VisionGlobal.TranslationY;

                        //拼版偏移
                        X = X + Global.Parm.PbXoffset;
                        Y = Y + Global.Parm.PbYoffset;

                        string Type = dataRows[0]["尺寸"].ToString();

                        LeftPos = Global.GetSize(Type);//获取开口大小
                        ZHight = Global.GetHight(Type);//获取下针高度

                        if (Global.Is_NoMark)
                        {
                            VisionGlobal.Mak1_X = Global.Parm.Mak1Pos.Xpos;//纠偏Mask1X位置
                            VisionGlobal.Mak1_Y = Global.Parm.Mak1Pos.Ypos;//纠偏Mask1Y位置

                            VisionGlobal.Mak2_X = Global.Parm.Mak2Pos.Xpos;//纠偏Mask2X位置
                            VisionGlobal.Mak2_Y = Global.Parm.Mak2Pos.Ypos;//纠偏Mask2Y位置
                        }
                        int Ecode = Algorithm.MakAlgorithm(0, X, Y, VisionGlobal.Mak1_X, VisionGlobal.Mak1_Y,
                        VisionGlobal.Mak2_X, VisionGlobal.Mak2_Y, VisionGlobal.bMak1_X + VisionGlobal.TranslationX,
                        VisionGlobal.bMak1_Y + VisionGlobal.TranslationY, VisionGlobal.bMak2_X + VisionGlobal.TranslationX,
                        VisionGlobal.bMak2_Y + VisionGlobal.TranslationY, out yX, out yY);
                        if (Ecode != 0)
                        {
                            APP.Tip.ShowTip(1, "警告".tr(), "Mark绑定数据出错".tr(), "确定".tr());
                            Global.StopFlag = true;
                            return;
                        }
                        //整体补偿
                        yX = yX + Global.Parm.Offset_X;
                        yY = yY + Global.Parm.Offset_Y;

                        yX = yX - offsetx - Global.CalibData.Laser_X;
                        yY = yY - offsety - Global.CalibData.Laser_Y;
                        GoToStep++;
                        break;
                    case 3:
                        BrowLib.Controller.MotionAPI.LinXyMoveA(Global.RunXYVel, Global.RunXYAcc, yX, yY, 1, 2);
                        GoToStep++;
                        break;
                    case 4:
                        if (BrowLib.Controller.MotionAPI.LinXYRuningA())
                        {
                            CcdHight = Global.Systemdata.Ccdhight;
                            GoToStep++;
                        }
                        break;
                    case 5:
                        Global.Z轴.PMove(20, 1000, CcdHight, 1);
                        GoToStep++;
                        break;
                    case 6:
                        if (Global.Z轴.GetPrfPos() - CcdHight < 0.02)
                        {
                            if (Global.Z轴.Runing())
                            {
                                Thread.Sleep(50);
                                GoToStep++;
                            }
                        }
                        break;
                    case 7:
                        LaserFrm Laser = new LaserFrm();
                        Laser.Code = StrCode;
                        Laser.ShowDialog();
                        double Var = Laser.LaserValue - Global.Hoffset;
                        if (Math.Abs(Var - Global.CalibData.LaserValue)
                        <= Global.CalibData.Allowablevalue)
                        {

                            Global.TcpClass.Send("A:LaserOK\r\n");

                        }
                        else
                        {
                            Global.TcpClass.Send("A:LaserNG\r\n");
                        }
                        GoToStep++;
                        break;
                    case 8:
                        IniStart = false;
                        GoToStep = 0;
                        break;
                }
                Thread.Sleep(2);
            }

        }
        /// <summary>
        /// 拼图流程
        /// </summary>
        public void FlowPate(int mode)
        {
            Global.StopFlag = false;
            int FlowPatStep = 0;
            bool IniStart = true;
            int i = 1;
            double[] X = null, Y = null;
            double Rows, Clos;
            BrowLib.KTimer Flow_InTimer = new KTimer();
            while (IniStart && !Global.StopFlag)
            {
                if (!Global.PauseFlag)//暂停标志位
                {
                    switch (FlowPatStep)
                    {
                        case 0:
                            switch (Global.Model)
                            {
                                case 0:
                                    FlowPatStep = 6;
                                    break;
                                case 1:
                                case 2:
                                case 3://860P
                                    if (BrowLib.Controller.InPort["阻挡感应光电_IN"].IsOn())
                                    {
                                        FlowPatStep = 6;
                                    }
                                    else
                                    {
                                        APP.Tip.ShowTip(1, "警告".tr(), "请放入PCB板".tr(), "确定".tr());
                                    }
                                    break;
                            }
                            break;
                        case 1:
                            if (BrowLib.Controller.InPort["入口感应光电_IN"].IsOn())
                            {
                                BrowLib.Controller.OutPort["上位机要板_OUT"].Off();
                                APP.Log.I_Log("入口感应有板");
                                FlowPatStep++;
                            }
                            break;
                        case 2:
                            APP.Log.I_Log("启动皮带进板");
                            Global.皮带.JOP(1, 50);
                            RunTimer.Restart();
                            FlowPatStep++;
                            break;
                        case 3:
                            if (BrowLib.Controller.InPort["阻挡感应光电_IN"].IsOn())
                            {
                                APP.Log.I_Log("阻挡光电感应板到位");
                                Global.皮带.ChangSpeed(5);
                                Global.皮带.JOP(1, 5);//皮带减速
                                Thread.Sleep((int)Global.Systemdata.InDaytime);
                                Global.皮带.AxisStop();
                                FlowPatStep++;
                            }
                            else if (RunTimer.IsOn(5000))
                            {
                                APP.Tip.ShowTip(1, "警告".tr(), "进板超时".tr(), "确定".tr());
                                Global.StopFlag = true;
                                Global.MachineState = GEnumEx.MachineState.MachineStop;
                                Global.SystemRun = false;
                                Global.TcpClass.Send("M:Stop");
                            }
                            break;
                        case 4:
                            Global.调宽.PMove(30, 1000, -1 * Global.Systemdata.Trackoffset, 0);
                            FlowPatStep++;
                            break;
                        case 5:
                            if (Global.调宽.Runing())
                            {
                                FlowPatStep++;
                            }
                            break;
                        case 6:
                            APP.Log.I_Log("A轨顶升双气缸顶起");
                            BrowLib.Controller.OutPort["顶升气缸_OUT"].On();
                            RunTimer.Restart();
                            FlowPatStep++;
                            break;
                        case 7:
                            if (BrowLib.Controller.InPort["顶升上位_IN"].IsOn(100))
                            {
                                APP.Log.I_Log("A轨顶升双气缸顶起到位");
                                Thread.Sleep(300);
                                FlowPatStep++;
                            }
                            else if (RunTimer.IsOn(1000))
                            {
                                int Rtn = APP.Tip.ShowTip(1, "警告".tr(), "顶升到位超时".tr(), "继续".tr(), "停止".tr());
                                if (Rtn == 1)
                                {
                                    APP.Log.I_Log("顶升到位超时-点击(继续)");
                                    FlowPatStep++;
                                }
                                else
                                {
                                    APP.Log.I_Log("顶升到位超时-点击(停止)");
                                    Global.StopFlag = true;
                                    Global.MachineState = GEnumEx.MachineState.MachineStop;
                                    Global.SystemRun = false;
                                    Global.TcpClass.Send("M:Stop");
                                }
                            }
                            break;

                        case 8:
                            Global.Z轴.PMove(Global.RunZVel, Global.RunZAcc, 2, 1);
                            FlowPatStep++;
                            break;
                        case 9:
                            if (Global.Z轴.GetPrfPos() - 2 < 0.01)
                            {
                                if (Global.Z轴.Runing())
                                {
                                    Thread.Sleep(20);
                                    FlowPatStep++;
                                }
                            }
                            break;
                        case 10:
                            BrowLib.Controller.MotionAPI.LinXyMoveA(Global.RunXYVel, Global.RunXYAcc, Global.Systemdata.PatlePos.Xpos, Global.Systemdata.PatlePos.Ypos, 1, 2);
                            FlowPatStep++;
                            break;
                        case 11:
                            if (BrowLib.Controller.MotionAPI.LinXYRuningA())
                            {
                                Thread.Sleep(20);
                                FlowPatStep++;
                            }
                            break;
                        case 12:
                            if (mode == 1)
                            {
                                Global.Light.SetRgbLight(Global.Systemdata.P_LED2.LED_R, Global.Systemdata.P_LED2.LED_G, Global.Systemdata.P_LED2.LED_B);
                            }
                            else
                            {
                                Global.Light.SetRgbLight(Global.Systemdata.P_LED.LED_R, Global.Systemdata.P_LED.LED_G, Global.Systemdata.P_LED.LED_B);
                            }
                            Global.Z轴.PMove(Global.RunZVel, Global.RunZAcc, Global.CamHeight, 1);
                            FlowPatStep++;
                            break;
                        case 13:
                            if (Global.Z轴.GetPrfPos() - Global.CamHeight < 0.01)
                            {
                                if (Global.Z轴.Runing())
                                {
                                    Thread.Sleep(20);
                                    FlowPatStep++;
                                }
                            }
                            break;
                        case 14:
                            if (Global.Parm.PcbLong == 0 || Global.Parm.PcbHight == 0)
                            {
                                APP.Tip.ShowTip(1, "警告".tr(), "未设置PCB长宽参数,无法拼图".tr(), "确定".tr());
                                Global.StopFlag = true;
                                IniStart = false;
                                return;
                            }
                            string[] st = Algorithm.Photolocation(Global.Model, Global.Systemdata.PatlePos.Xpos, Global.Systemdata.PatlePos.Ypos,
                            Global.Parm.PcbLong, Global.Parm.PcbHight, Global.Systemdata.CameRaview.Row,
                            Global.Systemdata.CameRaview.Col, out X, out Y, out Rows, out Clos);
                            Global.VisionApp.SetToolData("Task2", "定义变量", 2110, 0, "false", 0);//取消拼图矩形框显示
                            Global.VisionApp.SetToolData("Task2", "定义变量", 2107, 0, Global.Systemdata.PateFile, 3);
                            Global.VisionApp.SetToolData("Task2", "定义变量", 2100, 0, "0", 1);
                            Global.VisionApp.SetToolData("Task2", "定义变量", 2101, 0, Rows.ToString(), 1);
                            Global.VisionApp.SetToolData("Task2", "定义变量", 2102, 0, Clos.ToString(), 1);
                            Global.VisionApp.SetToolData("Task2", "定义变量", 2105, 0, Global.Systemdata.Cutsize.Row.ToString(), 1);
                            Global.VisionApp.SetToolData("Task2", "定义变量", 2106, 0, Global.Systemdata.Cutsize.Col.ToString(), 1);
                            FlowPatStep++;
                            break;
                        case 15:
                            BrowLib.Controller.MotionAPI.LinXyMoveA(800, 8000, X[i], Y[i], 1);
                            FlowPatStep++;
                            break;
                        case 16:
                            if (BrowLib.Controller.MotionAPI.LinXYRuningA())
                            {
                                i++;
                                Thread.Sleep(50);
                                Global.VisionApp.ExecuteProc("Task2", 1000);
                                FlowPatStep++;
                            }
                            break;
                        case 17:
                            if (Global.VisionApp.EndProc["Task2"])
                            {
                                if (Global.VisionApp.RunState("Task2", "数据判断"))
                                {
                                    if (i >= X.Length)
                                    {
                                        FlowPatStep++;
                                    }
                                    else
                                    {
                                        FlowPatStep = 15;
                                    }
                                }
                                else
                                {
                                    APP.Tip.ShowTip(1, "警告".tr(), "拼图失败".tr(), "确定".tr());
                                    Global.TcpClass.Send("M:PictureERROR");
                                    Global.SystemRun = false;
                                    Global.MachineState = GEnumEx.MachineState.MachineStop;
                                    IniStart = false;
                                }
                            }
                            break;
                        case 18:
                            if (Global.VisionApp.GetBoolValue("Task2", "定义变量", 2109, 0))
                            {
                                Global.TcpClass.Send("M:PictureFinish");
                                Global.SystemRun = false;
                                Global.MachineState = GEnumEx.MachineState.MachineStop;
                                IniStart = false;
                            }
                            break;
                    }
                    Thread.Sleep(2);
                }
            }
        }
        /// <summary>
        /// 单次采图
        /// </summary>
        public void GraspImage(string StrCode, double W, double H, double height, int R = 0, int G = 0, int B = 0)
        {
            Global.StopFlag = false;
            int FlowStep = 0;
            bool IniStart = true;
            int i = 1;
            double X, Y, yR, offsetx, offsety, yX = 0, yY = 0;
            double[] PX = null, PY = null;
            double Rows, Clos;
            if (R != -1 || G != -1 || B != -1)
            {
                Global.Light.SetRgbLight(R, G, B);
                APP.Log.I_Log("设置光源亮度" + "R" + R.ToString() + "G" + G.ToString() + "B" + B.ToString());
            }
            else
            {
                Global.Light.SetRgbLight(Global.Systemdata.P_LED.LED_R, Global.Systemdata.P_LED.LED_G, Global.Systemdata.P_LED.LED_B);
            }
            while (IniStart && !Global.StopFlag)
            {
                if (!Global.PauseFlag)//暂停标志位
                {
                    switch (FlowStep)
                    {
                        case 0://Z轴去安全位
                            Global.Z轴.PMove(Global.RunZVel, Global.RunZAcc, 2, 1);
                            FlowStep++;
                            break;
                        case 1:
                            if (Global.Z轴.GetPrfPos() - 2 < 0.01)
                            {
                                if (Global.Z轴.Runing())
                                {

                                    Thread.Sleep(10);
                                    FlowStep++;
                                }
                            }
                            break;
                        case 2:
                            DataRow[] dataRows = Global.BomData.Select("位置号='" + StrCode + "'");
                            X = Convert.ToDouble(dataRows[0]["原始X坐标"].ToString());
                            Y = Convert.ToDouble(dataRows[0]["原始Y坐标"].ToString());
                            yR = Convert.ToDouble(dataRows[0]["原始方向"].ToString());
                            offsetx = Convert.ToDouble(dataRows[0]["X坐标调整"].ToString());
                            offsety = Convert.ToDouble(dataRows[0]["Y坐标调整"].ToString());

                            //象限坐标系平移  
                            X = X + VisionGlobal.TranslationX;
                            Y = Y + VisionGlobal.TranslationY;

                            //拼版偏移
                            X = X + Global.Parm.PbXoffset;
                            Y = Y + Global.Parm.PbYoffset;

                            if (Global.Is_NoMark)
                            {
                                VisionGlobal.Mak1_X = Global.Parm.Mak1Pos.Xpos;//纠偏Mask1X位置
                                VisionGlobal.Mak1_Y = Global.Parm.Mak1Pos.Ypos;//纠偏Mask1Y位置

                                VisionGlobal.Mak2_X = Global.Parm.Mak2Pos.Xpos;//纠偏Mask2X位置
                                VisionGlobal.Mak2_Y = Global.Parm.Mak2Pos.Ypos;//纠偏Mask2Y位置
                            }
                            int Ecode = Algorithm.MakAlgorithm(0, X, Y, VisionGlobal.Mak1_X, VisionGlobal.Mak1_Y,
                            VisionGlobal.Mak2_X, VisionGlobal.Mak2_Y, VisionGlobal.bMak1_X + VisionGlobal.TranslationX,
                            VisionGlobal.bMak1_Y + VisionGlobal.TranslationY, VisionGlobal.bMak2_X + VisionGlobal.TranslationX,
                            VisionGlobal.bMak2_Y + VisionGlobal.TranslationY, out yX, out yY);
                            if (Ecode != 0)
                            {
                                APP.Tip.ShowTip(1, "警告".tr(), "Mark绑定数据出错".tr(), "确定".tr());
                                Global.StopFlag = true;
                                return;
                            }
                            //整体补偿
                            yX = yX + Global.Parm.Offset_X;
                            yY = yY + Global.Parm.Offset_Y;
                            yX = yX - offsetx;
                            yY = yY - offsety;

                            FlowStep++;
                            break;
                        case 3:

                            BrowLib.Controller.MotionAPI.LinXyMoveA(Global.RunXYVel, Global.RunXYAcc, yX, yY, 1, 2);
                            FlowStep++;
                            break;
                        case 4:

                            if (BrowLib.Controller.MotionAPI.LinXYRuningA())
                            {
                                Thread.Sleep(10);
                                FlowStep++;
                            }
                            break;
                        case 5:
                            //Global.Light.SetRgbLight(Global.sysTem.GetData.P_LED.LED_R, Global.sysTem.GetData.P_LED.LED_G, Global.sysTem.GetData.P_LED.LED_B);
                            Global.Z轴.PMove(Global.RunZVel, Global.RunZAcc, Global.CamHeight + height, 1);
                            FlowStep++;
                            break;
                        case 6:
                            if (Global.Z轴.GetPrfPos() - (Global.CamHeight + height) < 0.01)
                            {
                                if (Global.Z轴.Runing())
                                {
                                    Thread.Sleep(10);
                                    FlowStep++;
                                }
                            }
                            break;
                        case 7:
                            Algorithm.GetpatePoint2(yX, yY, Global.Systemdata.sCameRaview.Row, Global.Systemdata.sCameRaview.Col, W, H, out PX, out PY, out Rows, out Clos);
                            Global.VisionApp.SetToolData("Task6", "定义变量", 2110, 0, "false", 0);//取消拼图矩形框显示
                            Global.VisionApp.SetToolData("Task6", "定义变量", 2107, 0, Global.Systemdata.PateFile, 3);
                            Global.VisionApp.SetToolData("Task6", "定义变量", 2100, 0, "0", 1);
                            Global.VisionApp.SetToolData("Task6", "定义变量", 2101, 0, Rows.ToString(), 1);
                            Global.VisionApp.SetToolData("Task6", "定义变量", 2102, 0, Clos.ToString(), 1);
                            Global.VisionApp.SetToolData("Task6", "定义变量", 2105, 0, Global.Systemdata.Cutsize.Row.ToString(), 1);
                            Global.VisionApp.SetToolData("Task6", "定义变量", 2106, 0, Global.Systemdata.Cutsize.Col.ToString(), 1);
                            FlowStep++;
                            break;
                        case 8:
                            BrowLib.Controller.MotionAPI.LinXyMoveA(800, 8000, PX[i], PY[i], 1);
                            FlowStep++;
                            break;
                        case 9:
                            if (BrowLib.Controller.MotionAPI.LinXYRuningA())
                            {
                                i++;
                                Global.VisionApp.ExecuteProc("Task6", 0);
                                FlowStep++;
                            }
                            break;
                        case 10:
                            if (Global.VisionApp.EndProc["Task6"])
                            {
                                if (Global.VisionApp.RunState("Task6", "数据判断"))
                                {
                                    if (i >= PX.Length)
                                    {
                                        FlowStep++;
                                        int tFormat = 0;
                                        int tWidth = 0;
                                        int tHeight = 0;

                                        //SharedImage = Global.VisionApp.GetBitmap("Task6", "图像合并");
                                        IntPtr ptr = Global.VisionApp.GetImageBits("Task6", "图像合并", ref tFormat, ref tWidth, ref tHeight);
                                        //String _Path = Directory.GetCurrentDirectory().ToString() + "\\BMP";
                                        //Task.Run(() =>
                                        //{
                                        //    SharedImage.Save(_Path + "\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                                        //});
                                        //APP.Log.I_Log(_Path + "\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".bmp");

                                        bool re = MemoryHelper.Write_SharedMemory(ptr, tFormat, tWidth, tHeight, "SharedImage");
                                        APP.Log.I_Log("写入共享内存:" + "[" + re.ToString() + "]");
                                        //Thread.Sleep(200);
                                        Global.TcpClass.Send("M:GraspimgOK");
                                        Global.SystemRun = false;
                                        Global.MachineState = GEnumEx.MachineState.MachineStop;
                                        IniStart = false;
                                    }
                                    else
                                    {
                                        FlowStep = 8;
                                    }
                                }
                                else
                                {
                                    APP.Tip.ShowTip(1, "警告".tr(), "拼图失败".tr(), "确定".tr());
                                    Global.TcpClass.Send("M:GraspimgNG");
                                    Global.SystemRun = false;
                                    Global.MachineState = GEnumEx.MachineState.MachineStop;
                                    IniStart = false;
                                }
                            }
                            break;
                        case 11:
                            if (Global.VisionApp.GetBoolValue("Task6", "定义变量", 2109, 0))
                            {
                                Global.TcpClass.Send("M:GraspimgOK");
                                Global.SystemRun = false;
                                Global.MachineState = GEnumEx.MachineState.MachineStop;
                                IniStart = false;
                            }
                            break;
                    }
                }
                Thread.Sleep(2);
            }
        }
        public void AutoOCR()
        {
            int OcrStep = 0;
            bool IniStart = true;
            double W = 0, H = 0, hight = 0;
            string StrCode = null;
            int i = 1;
            double X, Y, yR, offsetx, offsety, yX = 0, yY = 0;
            double[] PX = null, PY = null;
            double Rows, Clos;
            while (!Global.StopFlag && IniStart)
            {
                switch (OcrStep)
                {
                    case 0://Z轴去安全位
                        Global.Z轴.PMove(Global.RunZVel, Global.RunZAcc, 2, 1);
                        APP.Log.I_Log("Z轴回安全位");
                        OcrStep++;
                        break;
                    case 1:
                        if (Global.Z轴.GetPrfPos() - 2 < 0.01)
                        {
                            if (Global.Z轴.Runing())
                            {
                                Thread.Sleep(10);
                                OcrStep++;
                            }
                        }
                        break;
                    case 2:
                        Global.TcpClass.TCP_AStaart = false;
                        Global.TcpClass.Send("A:OCRGET\r\n");
                        OcrStep++;
                        break;
                    case 3:
                        if (Global.TcpClass.TCP_AStaart)
                        {
                            string str2 = Global.TcpClass.TCP_Result.Trim();
                            string[] re1 = str2.Split(':');

                            if (re1[0] == "A")
                            {
                                if (re1[1] == "Finish")
                                {
                                    Global.TcpClass.Send("A:Finish");
                                    IniStart = false;
                                    Global.SystemRun = false;
                                    Global.MachineState = GEnumEx.MachineState.MachineStop;
                                }
                                else
                                {
                                    StrCode = re1[1];
                                    if (re1[2] == "Graspimg")
                                    {
                                        W = Convert.ToDouble(re1[3]);
                                        H = Convert.ToDouble(re1[4]);
                                        hight = Convert.ToDouble(re1[5]);
                                        int R, G, B;
                                        R = Convert.ToInt16(re1[6]);
                                        G = Convert.ToInt16(re1[7]);
                                        B = Convert.ToInt16(re1[8]);
                                        if (R != -1 || G != -1 || B != -1)
                                        {
                                            Global.Light.SetRgbLight(R, G, B);
                                        }
                                        else
                                        {
                                            Global.Light.SetRgbLight(Global.Systemdata.P_LED.LED_R, Global.Systemdata.P_LED.LED_G, Global.Systemdata.P_LED.LED_B);
                                        }
                                        OcrStep++;
                                    }
                                }
                            }

                        }
                        break;
                    case 4:
                        DataRow[] dataRows = Global.BomData.Select("位置号='" + StrCode + "'");
                        X = Convert.ToDouble(dataRows[0]["原始X坐标"].ToString());
                        Y = Convert.ToDouble(dataRows[0]["原始Y坐标"].ToString());
                        yR = Convert.ToDouble(dataRows[0]["原始方向"].ToString());
                        offsetx = Convert.ToDouble(dataRows[0]["X坐标调整"].ToString());
                        offsety = Convert.ToDouble(dataRows[0]["Y坐标调整"].ToString());



                        //象限坐标系平移  
                        X = X + VisionGlobal.TranslationX;
                        Y = Y + VisionGlobal.TranslationY;

                        //拼版偏移
                        X = X + Global.Parm.PbXoffset;
                        Y = Y + Global.Parm.PbYoffset;

                        if (Global.Is_NoMark)
                        {
                            VisionGlobal.Mak1_X = Global.Parm.Mak1Pos.Xpos;//纠偏Mask1X位置
                            VisionGlobal.Mak1_Y = Global.Parm.Mak1Pos.Ypos;//纠偏Mask1Y位置

                            VisionGlobal.Mak2_X = Global.Parm.Mak2Pos.Xpos;//纠偏Mask2X位置
                            VisionGlobal.Mak2_Y = Global.Parm.Mak2Pos.Ypos;//纠偏Mask2Y位置
                        }
                        int Ecode = Algorithm.MakAlgorithm(0, X, Y, VisionGlobal.Mak1_X, VisionGlobal.Mak1_Y,
                        VisionGlobal.Mak2_X, VisionGlobal.Mak2_Y, VisionGlobal.bMak1_X + VisionGlobal.TranslationX,
                        VisionGlobal.bMak1_Y + VisionGlobal.TranslationY, VisionGlobal.bMak2_X + VisionGlobal.TranslationX,
                        VisionGlobal.bMak2_Y + VisionGlobal.TranslationY, out yX, out yY);
                        if (Ecode != 0)
                        {
                            APP.Tip.ShowTip(1, "警告".tr(), "Mark绑定数据出错".tr(), "确定".tr());
                            Global.StopFlag = true;
                            return;
                        }
                        //整体补偿
                        yX = yX + Global.Parm.Offset_X;
                        yY = yY + Global.Parm.Offset_Y;

                        yX = yX - offsetx;
                        yY = yY - offsety;
                        OcrStep++;
                        break;
                    #region 自动采图丝印
                    case 5:
                        BrowLib.Controller.MotionAPI.LinXyMoveA(Global.RunXYVel, Global.RunXYAcc, yX, yY, 1, 2);
                        OcrStep++;
                        break;
                    case 6:
                        if (BrowLib.Controller.MotionAPI.LinXYRuningA())
                        {
                            Thread.Sleep(10);
                            OcrStep++;
                        }
                        break;
                    case 7:

                        Global.Z轴.PMove(Global.RunZVel, Global.RunZAcc, Global.CamHeight + hight, 1);
                        OcrStep++;
                        break;
                    case 8:
                        if (Global.Z轴.GetPrfPos() - (Global.CamHeight + hight) < 0.01)
                        {
                            if (Global.Z轴.Runing())
                            {
                                Thread.Sleep(10);
                                OcrStep++;
                            }
                        }
                        break;
                    case 9:
                        Algorithm.GetpatePoint2(yX, yY, Global.Systemdata.sCameRaview.Row, Global.Systemdata.sCameRaview.Col, W, H, out PX, out PY, out Rows, out Clos);
                        Global.VisionApp.SetToolData("Task6", "定义变量", 2110, 0, "false", 0);//取消拼图矩形框显示
                        Global.VisionApp.SetToolData("Task6", "定义变量", 2107, 0, Global.Systemdata.PateFile, 3);
                        Global.VisionApp.SetToolData("Task6", "定义变量", 2100, 0, "0", 1);
                        Global.VisionApp.SetToolData("Task6", "定义变量", 2101, 0, Rows.ToString(), 1);
                        Global.VisionApp.SetToolData("Task6", "定义变量", 2102, 0, Clos.ToString(), 1);
                        Global.VisionApp.SetToolData("Task6", "定义变量", 2105, 0, Global.Systemdata.Cutsize.Row.ToString(), 1);
                        Global.VisionApp.SetToolData("Task6", "定义变量", 2106, 0, Global.Systemdata.Cutsize.Col.ToString(), 1);
                        OcrStep++;
                        break;
                    case 10:
                        BrowLib.Controller.MotionAPI.LinXyMoveA(800, 8000, PX[i], PY[i], 1);
                        OcrStep++;
                        break;
                    case 11:

                        if (BrowLib.Controller.MotionAPI.LinXYRuningA())
                        {
                            i++;
                            Global.VisionApp.ExecuteProc("Task6", 0);
                            OcrStep++;
                        }
                        break;
                    case 12:
                        if (Global.VisionApp.EndProc["Task6"])
                        {
                            if (Global.VisionApp.RunState("Task6", "数据判断"))
                            {
                                if (i >= PX.Length)
                                {
                                    int tFormat = 0;
                                    int tWidth = 0;
                                    int tHeight = 0;

                                    IntPtr ptr = Global.VisionApp.GetImageBits("Task6", "图像合并", ref tFormat, ref tWidth, ref tHeight);

                                    bool re = MemoryHelper.Write_SharedMemory(ptr, tFormat, tWidth, tHeight, "SharedImage");
                                    //SharedImage = Global.VisionApp.GetBitmap("Task6", "图像合并");
                                    //bool re =MemoryHelper.Write_SharedMemory_Bitmap1(SharedImage, "SharedImage");
                                    Global.TcpClass.TCP_AStaart = false;
                                    Global.TcpClass.Send("A:GraspimgOK");
                                    APP.Log.I_Log("写入共享内存" + "SharedImage" + "结果" + re.ToString());
                                    OcrStep = 14;
                                }
                                else
                                {
                                    OcrStep = 10;
                                }

                            }
                            else
                            {
                                APP.Tip.ShowTip(1, "警告".tr(), "拼图失败".tr(), "确定".tr());
                                Global.StopFlag = true;
                                Global.MachineState = GEnumEx.MachineState.MachineStop;
                                Global.SystemRun = false;
                                Global.TcpClass.Send("A:GraspimgNG");
                            }
                        }
                        break;
                    case 13:
                        if (Global.VisionApp.GetBoolValue("Task6", "定义变量", 2109, 0))
                        {
                            Global.TcpClass.TCP_AStaart = false;
                            Global.TcpClass.Send("A:GraspimgOK");
                            OcrStep++;
                        }
                        break;
                    case 14:
                        if (Global.TcpClass.TCP_AStaart)
                        {
                            string str2 = Global.TcpClass.TCP_Result.Trim();
                            string[] re1 = str2.Split(':');
                            if (re1[0] == "A")
                            {
                                if (re1[1] == "OCR_OK")
                                {
                                    i = 1;
                                    OcrStep = 2;
                                }
                            }
                        }
                        break;
                        #endregion

                }
                Thread.Sleep(5);
            }
        }
        /// <summary>
        /// 初始化流程
        /// </summary>
        public void AutoInit()
        {
            int IniStep = 0;
            string ErrorMes = null;
            bool Re = false;
            bool IniStart = true;
            while (IniStart && !Global.StopFlag)
            {
                if (!Global.IsRecipe)
                {
                    Global.StopFlag = true;
                    Global.SystemRun = false;
                    Global.MachineState = GEnumEx.MachineState.MachineStop;
                    return;
                }
                switch (IniStep)
                {
                    case 0:
                        if (BrowLib.Controller.InPort["入口感应光电_IN"].IsOn())
                        {
                            ErrorMes += "轨道入口有板!" + "/";
                        }
                        if (BrowLib.Controller.InPort["阻挡感应光电_IN"].IsOn())
                        {
                            ErrorMes += "轨道工作位有板" + "/";
                        }
                        IniStep++;
                        break;
                    case 1:
                        if (ErrorMes != null)
                        {
                            APP.Tip.ShowTip(1, "警告".tr(), ErrorMes.TrimEnd().tr(), "确定".tr());
                            IniStart = false;
                        }
                        else
                        {
                            IniTimer.Restart();
                            BrowLib.Controller.OutPort["A轨阻挡2_OUT5"].On();
                            IniStep++;
                        }
                        break;
                    case 2:
                        if (BrowLib.Controller.InPort["A轨阻挡2上位_IN0"].IsOn(100))
                        {
                            IniStart = false;
                            FlowThread = new Thread(AutoRun);
                            FlowThread.IsBackground = true;
                            FlowThread.Start();
                            Global.SystemRun = true;
                            Global.MachineState = GEnumEx.MachineState.MachineRuning;
                        }
                        else if (IniTimer.IsOn(2000))
                        {
                            APP.Tip.ShowTip(1, "警告".tr(), "系统初始化失败！！！", "确定".tr());
                            IniStart = false;
                        }
                        break;
                }
            }
        }

        public void START()
        {
            Global.StopFlag = false;
            FlowThread = new Thread(AutoRun);
            FlowThread.IsBackground = true;
            FlowThread.Start();
            Global.SystemRun = true;
            Global.MachineState = GEnumEx.MachineState.MachineRuning;
        }

        public void STARTFLOWBOARD()
        {
            Global.StopFlag = false;
            FlowThread = new Thread(AutoBoard);
            FlowThread.IsBackground = true;
            FlowThread.Start();
            Global.SystemRun = true;
            Global.OrbModel = 3;
        }

        public void StartOCR()
        {
            Global.StopFlag = false;
            FlowThread = new Thread(AutoOCR);
            FlowThread.IsBackground = true;
            FlowThread.Start();
            Global.SystemRun = true;
        }
        /// <summary>
        /// 自动检测流程
        /// </summary>
        private void AutoRun()
        {
            #region 步骤变量
            int Runstep = 0;
            int Nextstep = 0;
            int Breakstep = 0;
            int Nowstep = 0;
            int Erorstep = 100;
            #endregion

            double[] PX = null, PY = null;
            double Rows, Clos;

            double KH = 0, LW = 0, hight = 0;

            double Mak_X = 0, Mak_Y = 0, Mak_Z = 0;

            int MakNum = 0, Mode = 0, idex = 1;

            double DMak1_X = 0, DMak1_Y = 0, DMak2_X = 0, DMak2_Y = 0;

            string StrCode = null, Type = null;

            double X = 0, Y = 0, R = 0, Size = 0, ZHight = 0, Wx = 0, Wy = 0, Dx = 0, Dy = 0, offsetx = 0, offsety = 0, CenterdX = 0, CenterdY = 0;

            double jqsize = 0;
            while (!Global.StopFlag)
            {
                #region  主流程
                if (!Global.PauseFlag || (Runstep == 25 || Runstep == 18 || Runstep == 108))//暂停标志位
                {
                    #region 自动运行流程
                    switch (Runstep)
                    {
                        case 0:
                            switch (Global.Model)
                            {
                                case 0://离线机
                                    Runstep = 6;
                                    break;
                                case 1:
                                case 2://在线机 860在线机
                                case 3://860P
                                    if (BrowLib.Controller.InPort["阻挡感应光电_IN"].IsOn(100))
                                    {
                                        Runstep = 6;
                                    }
                                    else
                                    {
                                        APP.Tip.ShowTip(1, "警告".tr(), "请放入PCB板".tr(), "确定".tr());
                                    }
                                    break;
                            }
                            break;
                        #region 检测入口传感器
                        case 1:
                            if (BrowLib.Controller.InPort["入口感应光电_IN"].IsOn(500))
                            {
                                BrowLib.Controller.OutPort["上位机要板_OUT"].Off();//上位机要板关闭
                                APP.Log.I_Log("入口感应到位[Runstep=" + Runstep.ToString() + "]");
                                Runstep++;
                            }
                            break;
                        #endregion
                        case 2:
                            APP.Log.I_Log("开始进板[Runstep=" + Runstep.ToString() + "]");
                            Global.皮带.JOP(1, 50);
                            RunTimer.Restart();
                            Runstep++;
                            break;
                        case 3:
                            if (BrowLib.Controller.InPort["阻挡感应光电_IN"].IsOn())
                            {
                                APP.Log.I_Log("板到工作位[Runstep=" + Runstep.ToString() + "]");
                                Global.皮带.ChangSpeed(5);
                                Global.皮带.JOP(1, 5);//皮带减速
                                Thread.Sleep((int)Global.Systemdata.InDaytime);
                                Global.皮带.AxisStop();
                                Runstep++;
                            }
                            else if (RunTimer.IsOn(5000))
                            {
                                APP.Tip.ShowTip(1, "警告".tr(), "进板超时".tr(), "确定".tr());
                                Global.StopFlag = true;
                                Global.MachineState = GEnumEx.MachineState.MachineStop;
                                Global.SystemRun = false;
                                Global.TcpClass.Send("M:Stop");
                            }
                            break;
                        case 4:
                            Global.调宽.PMove(10, 1000, -1 * Global.Systemdata.Trackoffset, 0);
                            Runstep++;
                            break;
                        case 5:
                            if (Global.调宽.Runing())
                            {
                                Runstep++;
                            }
                            break;
                        case 6:
                            APP.Log.I_Log("顶升双气缸顶起[Runstep=" + Runstep.ToString() + "]");
                            BrowLib.Controller.OutPort["顶升气缸_OUT"].On();
                            RunTimer.Restart();
                            Runstep++;
                            break;
                        case 7:
                            if (BrowLib.Controller.InPort["顶升上位_IN"].IsOn(100))
                            {
                                APP.Log.I_Log("顶升顶起到位[Runstep=" + Runstep.ToString() + "]");
                                Thread.Sleep(300);
                                Runstep++;
                            }
                            else if (RunTimer.IsOn(1000))
                            {
                                int Rtn = APP.Tip.ShowTip(1, "警告".tr(), "顶升到位超时".tr(), "继续".tr(), "停止".tr());
                                if (Rtn == 1)
                                {
                                    APP.Log.I_Log("顶升到位超时-点击(继续)");
                                    Runstep++;
                                }
                                else
                                {
                                    APP.Log.I_Log("顶升到位超时-点击(停止)");
                                    Global.StopFlag = true;
                                    Global.MachineState = GEnumEx.MachineState.MachineStop;
                                    Global.SystemRun = false;
                                    Global.TcpClass.Send("M:Stop");
                                }
                            }
                            break;

                        case 8:
                            Global.Light.SetRgbLight(Global.Systemdata.M_LED.LED_R, Global.Systemdata.M_LED.LED_G, Global.Systemdata.M_LED.LED_B);
                            APP.Log.I_Log(string.Concat(new string[]
                           {
                            "设置光源亮度[R,G,B=",
                            Global.Systemdata.M_LED.LED_R.ToString(),
                            ",",
                            Global.Systemdata.M_LED.LED_G.ToString(),
                            ",",
                            Global.Systemdata.M_LED.LED_B.ToString(),
                            "]"
                    }));
                            Global.VisionApp.StopRunProc("Task5");//停止实时刷新
                            Global.Z轴.PMove(Global.RunZVel, Global.RunZAcc, Global.CamHeight, 1);
                            APP.Log.I_Log("Z轴去拍照高度[Runstep=" + Runstep.ToString() + "]");
                            Runstep++;
                            break;
                        case 9:
                            if (Global.Z轴.GetPrfPos() - Global.CamHeight < 0.02)
                            {
                                if (Global.Z轴.Runing())
                                {
                                    Global.R轴.PMove(Global.RunZVel, Global.RunZAcc, 0, 1);
                                    APP.Log.I_Log("Z轴到拍照高度[Runstep=" + Runstep.ToString() + "]");
                                    Runstep++;
                                }
                            }
                            break;
                        case 10:
                            if (Global.R轴.GetPrfPos() - 0 < 0.02)
                            {
                                if (Global.R轴.Runing())
                                {
                                    APP.Log.I_Log("R轴回零位完成[Runstep=" + Runstep.ToString() + "]");
                                    Runstep++;
                                }
                            }
                            break;
                        case 11:
                            if (!Global.Is_NoMark)
                            {
                                if (MakNum == 0)
                                {
                                    Mak_X = Global.Parm.Mak1Pos.Xpos;
                                    Mak_Y = Global.Parm.Mak1Pos.Ypos;
                                    Mak_Z = Global.CamHeight;
                                    APP.Log.I_Log("Go MARK1");
                                }
                                else if (MakNum == 1)
                                {
                                    Mak_X = Global.Parm.Mak2Pos.Xpos;
                                    Mak_Y = Global.Parm.Mak2Pos.Ypos;
                                    Mak_Z = Global.CamHeight;
                                    APP.Log.I_Log("Go MARK2");
                                }
                                BrowLib.Controller.MotionAPI.LinXyMoveA(Global.RunXYVel, Global.RunXYAcc, Mak_X, Mak_Y, 1, 2);
                                APP.Log.I_Log(string.Concat(new string[]
                                {
                                  "去Mark位置",
                                  MakNum.ToString(),
                                  "[Runstep=",
                                  Runstep.ToString(),
                                 "]"
                                 }));
                                Runstep++;
                            }
                            else
                            {
                                Runstep = 17;
                            }
                            break;
                        case 12:
                            if (BrowLib.Controller.MotionAPI.LinXYRuningA())
                            {
                                APP.Log.I_Log(string.Concat(new string[]
                        {
                            "到Mark位置",
                            MakNum.ToString(),
                            "[Runstep=",
                            Runstep.ToString(),
                            "]"
                        }));
                                Runstep++;
                            }
                            break;
                        case 13:
                            Global.Z轴.PMove(Global.RunZVel, Global.RunZAcc, Mak_Z, 1);
                            APP.Log.I_Log(string.Concat(new string[]
                    {
                        "Z去拍照高度",
                        MakNum.ToString(),
                        "[Runstep=",
                        Runstep.ToString(),
                        "]"
                    }));
                            Runstep++;
                            break;
                        case 14:
                            if (Global.Z轴.GetPrfPos() - Mak_Z < 0.01)
                            {
                                if (Global.Z轴.Runing())
                                {
                                    APP.Log.I_Log(string.Concat(new string[]
                            {
                                "Z到拍照高度",
                                MakNum.ToString(),
                                "[Runstep=",
                                Runstep.ToString(),
                                "]"
                            }));
                                    Thread.Sleep(100);
                                    Runstep++;
                                }
                            }
                            break;
                        case 15:
                            if (MakNum == 0)
                            {
                                Global.VisionApp.SetToolData("Task3", "定义变量", 2102, 0, "false", 0);
                                Global.VisionApp.SetToolData("Task3", "定义变量", 2110, 0, "1", 1);
                                Global.VisionApp.ExecuteProc("Task3", 0);
                            }
                            else if (MakNum == 1)
                            {
                                Global.VisionApp.SetToolData("Task3", "定义变量", 2102, 0, "false", 0);
                                Global.VisionApp.SetToolData("Task3", "定义变量", 2110, 0, "2", 1);
                                Global.VisionApp.ExecuteProc("Task3", 1);
                            }
                            //触发拍照
                            APP.Log.I_Log("触发相机拍照[Runstep=" + Runstep.ToString() + "]");
                            Thread.Sleep(10);
                            Runstep++;
                            break;
                        case 16:
                            if (Global.VisionApp.IsEndProc)
                            {
                                if (MakNum == 0)
                                {
                                    if (Global.VisionApp.RunState("Task3", "执行结果"))
                                    {
                                        DMak1_X = Global.VisionApp.GetDblValue("Task3", "定义变量", 2103, 0);
                                        DMak1_Y = Global.VisionApp.GetDblValue("Task3", "定义变量", 2104, 0);
                                        APP.Log.I_Log("Mark1结果->Dx:" + DMak1_X.ToString("F3") + "Dy:" + DMak1_Y.ToString("F3"));
                                        MakNum++;
                                        Runstep = 11;
                                    }
                                    else
                                    {
                                        MarkRectify mark = new MarkRectify();
                                        mark.ShowDialog();
                                        if (mark.Isok)
                                        {
                                            DMak1_X = mark.Dx;
                                            DMak1_Y = mark.Dy;
                                            MakNum++;
                                            Runstep = 11;
                                        }
                                        else
                                        {
                                            APP.Tip.ShowTip(1, "警告".tr(), "Mark1定位失败".tr(), "确定".tr());
                                            Global.StopFlag = true;
                                            Global.MachineState = GEnumEx.MachineState.MachineStop;
                                            Global.SystemRun = false;
                                            Global.TcpClass.Send("A:Mark_NG");
                                            APP.Log.I_Log("Mark1定位失败-STOP");
                                        }
                                    }
                                }
                                else if (MakNum == 1)
                                {
                                    if (Global.VisionApp.RunState("Task3", "执行结果"))
                                    {
                                        DMak2_X = Global.VisionApp.GetDblValue("Task3", "定义变量", 2103, 0);
                                        DMak2_Y = Global.VisionApp.GetDblValue("Task3", "定义变量", 2104, 0);
                                        APP.Log.I_Log("Mark2结果->Dx:" + DMak2_X.ToString("F3") + "Dy:" + DMak2_Y.ToString("F3"));
                                        MakNum = 0;
                                        Runstep++;
                                    }
                                    else
                                    {
                                        MarkRectify mark = new MarkRectify();
                                        mark.ShowDialog();
                                        if (mark.Isok)
                                        {
                                            DMak2_X = mark.Dx;
                                            DMak2_Y = mark.Dy;
                                            MakNum = 0;
                                            Runstep++;
                                        }
                                        else
                                        {
                                            APP.Tip.ShowTip(1, "警告".tr(), "Mark2定位失败".tr(), "确定".tr());
                                            Global.StopFlag = true;
                                            Global.MachineState = GEnumEx.MachineState.MachineStop;
                                            Global.SystemRun = false;
                                            Global.TcpClass.Send("A:Mark_NG");
                                            APP.Log.I_Log("Mark2定位失败-STOP");
                                        }
                                    }
                                }
                            }
                            break;
                        case 17:
                            Global.Light.SetRgbLight(Global.Systemdata.P_LED.LED_R, Global.Systemdata.P_LED.LED_G, Global.Systemdata.P_LED.LED_B);
                            APP.Log.I_Log(string.Concat(new string[]
                           {
                             "设置光源亮度[R,G,B=",
                              Global.Systemdata.P_LED.LED_R.ToString(),
                              ",",
                              Global.Systemdata.P_LED.LED_G.ToString(),
                              ",",
                              Global.Systemdata.P_LED.LED_B.ToString(),
                             "]"
                            }));
                            VisionGlobal.Mak1_X = Global.Parm.Mak1Pos.Xpos - DMak1_X;//纠偏Mask1X位置
                            VisionGlobal.Mak1_Y = Global.Parm.Mak1Pos.Ypos - DMak1_Y;//纠偏Mask1Y位置

                            VisionGlobal.Mak2_X = Global.Parm.Mak2Pos.Xpos - DMak2_X;//纠偏Mask2X位置
                            VisionGlobal.Mak2_Y = Global.Parm.Mak2Pos.Ypos - DMak2_Y;//纠偏Mask2Y位置

                            VisionGlobal.Mak1_X = Math.Round(VisionGlobal.Mak1_X, 4);
                            VisionGlobal.Mak1_Y = Math.Round(VisionGlobal.Mak1_Y, 4);
                            VisionGlobal.Mak2_X = Math.Round(VisionGlobal.Mak2_X, 4);
                            VisionGlobal.Mak2_Y = Math.Round(VisionGlobal.Mak2_Y, 4);
                            Cycle_Timer.Restart();
                            Cycle_Timer.Start();
                            Runstep++;
                            break;
                        case 18:
                            switch (Global.RunMode)
                            {
                                case 0:
                                    Runstep = 200;//单机模式
                                    break;
                                case 1:
                                    Runstep++;
                                    break;
                            }
                            break;
                        case 19:
                            Global.TcpClass.TCP_AStaart = false;
                            Global.TcpClass.Send("A:GET\r\n", Runstep.ToString());
                            Runstep++;
                            break;
                        case 20:
                            if (Global.TcpClass.TCP_AStaart)
                            {
                                string str2 = Global.TcpClass.TCP_Result.Trim();
                                string[] re1 = str2.Split(':');
                                if (re1[0] == "A")
                                {
                                    if (re1[1] == "Finish")
                                    {
                                        Cycle_Timer.Stop();
                                        Cycle_Time = GetTime(Cycle_Timer.ElapsedTicks);
                                        switch (Global.Model)//机型选择
                                        {
                                            case 0:
                                                Runstep = 50;//离线
                                                break;
                                            case 1:
                                            case 2:
                                            case 3:
                                                switch (Global.OrbModel)//轨道模式选择
                                                {
                                                    case 0:
                                                        Runstep = 60;//在线
                                                        break;
                                                    case 1:
                                                        Runstep = 50;//离线
                                                        break;
                                                }
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        StrCode = re1[1];
                                        switch (re1[2])
                                        {
                                            case "2":
                                                BrowLib.Controller.OutPort["四线切换两线"].On();
                                                break;
                                            case "4":
                                                BrowLib.Controller.OutPort["四线切换两线"].Off();
                                                break;
                                        }
                                        if (re1.Length > 3)
                                        {
                                            if (re1[3] == "Rectify")
                                            {
                                                Mode = 1;
                                                Runstep++;
                                            }
                                            else if (re1[3] == "Laser")
                                            {
                                                Mode = 2;
                                                Runstep++;
                                            }
                                            else if (re1[3] == "RectifyFirst")
                                            {
                                                Mode = 3;
                                                Runstep++;
                                            }
                                            else if (re1[3] == "RectifyNext")
                                            {
                                                Dx = 0; Dy = 0;
                                                //Global.BomData = new JD.Fai.Data.FlyingProbe().GetDataTable(Global.FBCCode);
                                                Algorithm.CalculateTransferXY(Global.BomData, out X, out Y);
                                                VisionGlobal.TranslationX = X;
                                                VisionGlobal.TranslationY = Y;
                                                Mode = 0;
                                                Runstep++;
                                            }
                                            else if (re1[3] == "RectifyCheck")
                                            {
                                                Mode = 4;
                                                Runstep++;
                                            }
                                            else
                                            {
                                                Mode = 0;
                                                Runstep++;
                                            }
                                        }
                                        else
                                        {
                                            APP.Log.I_Log("指令回复错误:" + str2 + "[Runstep=" + Runstep.ToString() + "]");
                                            Mode = 0;
                                            Runstep++;
                                        }
                                    }
                                }
                            }
                            break;
                        case 21:
                            //位号获取坐标位置
                            DataRow[] dataRows = Global.BomData.Select("位置号='" + StrCode + "'");
                            X = Convert.ToDouble(dataRows[0]["原始X坐标"].ToString());
                            Y = Convert.ToDouble(dataRows[0]["原始Y坐标"].ToString());
                            R = Convert.ToDouble(dataRows[0]["原始方向"].ToString());
                            offsetx = Convert.ToDouble(dataRows[0]["X坐标调整"].ToString());
                            offsety = Convert.ToDouble(dataRows[0]["Y坐标调整"].ToString());
                            R = Algorithm.GetAngle(R);

                            //象限坐标系平移  
                            X = X + VisionGlobal.TranslationX;
                            Y = Y + VisionGlobal.TranslationY;
                            //拼版偏移
                            X = X + Global.Parm.PbXoffset;
                            Y = Y + Global.Parm.PbYoffset;

                            Type = dataRows[0]["尺寸"].ToString();

                            Size = Global.GetSize(Type);//获取开口大小
                            ZHight = Global.GetHight(Type);//获取下针高度
                            jqsize = Global.JQSize(Type);//获取下针高度

                            //Mark计算
                            int Ecode = Algorithm.MakAlgorithm(0, X, Y, VisionGlobal.Mak1_X, VisionGlobal.Mak1_Y,
                            VisionGlobal.Mak2_X, VisionGlobal.Mak2_Y, VisionGlobal.bMak1_X + VisionGlobal.TranslationX,
                            VisionGlobal.bMak1_Y + VisionGlobal.TranslationY, VisionGlobal.bMak2_X + VisionGlobal.TranslationX,
                            VisionGlobal.bMak2_Y + VisionGlobal.TranslationY, out Wx, out Wy);
                            if (Ecode != 0)
                            {
                                APP.Tip.ShowTip(1, "警告".tr(), "Mark绑定数据出错".tr(), "确定".tr());
                                Global.StopFlag = true;
                                Global.MachineState = GEnumEx.MachineState.MachineStop;
                                Global.SystemRun = false;
                                return;
                            }
                            //整体补偿
                            Wx = Wx + Global.Parm.Offset_X;
                            Wy = Wy + Global.Parm.Offset_Y;

                            if (Mode == 0) { Runstep++; }
                            else if (Mode == 1)
                            { Global.VisionApp.StopRunProc("Task5"); Runstep = 100; }
                            else if (Mode == 2)
                            { Global.VisionApp.StopRunProc("Task5"); Runstep = 300; }
                            else if (Mode == 3)
                            { Global.VisionApp.StopRunProc("Task5"); Runstep = 400; }
                            else if (Mode == 4)
                            { Global.VisionApp.StopRunProc("Task5"); Runstep = 500; }
                            break;
                        case 22:
                            if (Global.IsCcdMode)//判断是否启用视觉模式
                            {
                                Global.VisionApp.RunProc("Task5");
                                APP.Log.I_Log("目标位置X:" + Wx.ToString() + "目标位置Y:" + Wy.ToString());
                                BrowLib.Controller.MotionAPI.LinXyMoveA(Global.RunXYVel, Global.RunXYAcc, Wx, Wy, 1, 2);
                                Runstep++;
                            }
                            else
                            {
                                Global.VisionApp.StopRunProc("Task5");
                                double offset_X, offset_Y;
                                //获取相机Vs针头便移值
                                Global.GetOffset(R, out offset_X, out offset_Y, Global.Is_DownCam);
                                APP.Log.I_Log("旋转偏移dX:" + offset_X.ToString() + "旋转偏移dY:" + offset_Y.ToString());
                                Tuple<double, double> offset = this.GetOffset(Wx, Wy, offset_X, offset_Y);
                                Wx = offset.Item1;
                                Wy = offset.Item2;
                                Tuple<double, double> tuple = Global.coordinateCompensator.GetCompensatedPosition(Wx, Wy);
                                APP.Log.I_Log("目标位置X:" + tuple.Item1.ToString() + "目标位置Y:" + tuple.Item2.ToString());
                                BrowLib.Controller.MotionAPI.LinXyMoveA(Global.RunXYVel, Global.RunXYAcc, tuple.Item1, tuple.Item2, 1, 2);
                                Global.R轴.PMove(Global.RunRVel, Global.RunRAcc, R, 1);
                                APP.Log.I_Log("旋转角度[R=" + R.ToString() + "]");
                                Global.左夹爪轴.PMove(15, 500, Size, 1);
                                Global.右夹爪轴.PMove(15, 500, Size, 1);
                                APP.Log.I_Log("测试爪张开[Size=" + Size.ToString() + "]");
                                Runstep++;
                            }
                            break;
                        case 23:
                            if (Global.IsCcdMode)
                            {
                                if (BrowLib.Controller.MotionAPI.LinXYRuningA())
                                {
                                    APP.Log.I_Log("XY目标位置到位[Runstep=" + Runstep.ToString() + "]");
                                    ZHight = Global.CamHeight;
                                    Runstep++;
                                }
                            }
                            else
                            {
                                if (BrowLib.Controller.MotionAPI.LinXYRuningA())
                                {
                                    if (Global.R轴.GetPrfPos() - R < 0.05 &&
                                    Global.左夹爪轴.GetPrfPos() - Size < 0.05 &&
                                        Global.右夹爪轴.GetPrfPos() - Size < 0.05)
                                    {
                                        if (Global.R轴.Runing() && Global.左夹爪轴.Runing() && Global.右夹爪轴.Runing())
                                        {
                                            APP.Log.I_Log("R,左右夹爪目标位置到位[Runstep=" + Runstep.ToString() + "]");
                                            ZHight = ZHight + Global.Hoffset;
                                            APP.Log.I_Log("下针高度补偿:[Hoffset=" + Global.Hoffset.ToString() + "]");
                                            Runstep++;
                                        }
                                    }
                                }
                            }
                            break;
                        case 24:
                            if (Global.IsCcdMode)
                            {
                                Global.Z轴.PMove(10, 1000, ZHight, 1);
                                Runstep = 26;
                            }
                            else
                            {
                                Global.Z轴.PMove(Global.RunZVel, Global.RunZAcc, ZHight, 1);
                                APP.Log.I_Log("Z下针高度:[ZHight=" + ZHight.ToString() + "]");
                                //Global.Z轴.S_PMove(ZHight - 5, ZHight, Global.sysTem.GetData.GetSys.buf_Zspeed, Global.RunZVel, Global.RunZAcc, 1);
                                Runstep = 26;
                            }
                            break;
                        case 25:
                            if (ZHight - Global.Z轴.GetPrfPos() <= 2 /*< 0.02*/)
                            {
                                Global.Z轴.ChangSpeed(Global.Systemdata.buf_Zspeed);
                                //if (Global.Z轴.Runing())
                                //{
                                //Global.Z轴.PMove(Global.sysTem.GetData.GetSys.buf_Zspeed, 500, ZHight, 1);
                                Runstep++; ;
                                //}
                            }
                            break;
                        case 26:
                            if (Global.Z轴.GetPrfPos() - ZHight < 0.02)
                            {
                                if (Global.Z轴.Runing())
                                {
                                    APP.Log.I_Log("夹爪夹动作尺寸:[jqsize=" + jqsize.ToString() + "]");
                                    if (jqsize > 0 && jqsize < 1)
                                    {
                                        APP.Log.I_Log("执行夹紧动作:[jqsize=" + jqsize.ToString() + "]");
                                        Global.左夹爪轴.PMove(10, 200, jqsize, 0);
                                        Global.右夹爪轴.PMove(10, 200, jqsize, 0);
                                        Runstep++;
                                    }
                                    else
                                    {
                                        Thread.Sleep(50);
                                        Global.TextNum++;
                                        if (Global.TextNum >= Global.Systemdata.Servicelife)
                                        {
                                            APP.Tip.ShowTip(1, "警告".tr(), "测试针使用寿命已达到最大值".tr(), "确定".tr());
                                        }
                                        Global.TcpClass.TCP_AStaart = false;
                                        Global.WriteTextNum();
                                        Global.TcpClass.Send("A:GoToOk" + ":" + Dx.ToString() + ":" + Dy.ToString() + "\r\n", Runstep.ToString());
                                        Runstep = 28;
                                    }
                                }
                            }
                            break;
                        case 27:
                            if (Global.左夹爪轴.Runing() && Global.右夹爪轴.Runing())
                            {
                                Thread.Sleep(50);
                                Global.TextNum++;
                                if (Global.TextNum >= Global.Systemdata.Servicelife)
                                {
                                    APP.Tip.ShowTip(1, "警告".tr(), "测试针使用寿命已达到最大值".tr(), "确定".tr());
                                }
                                Global.TcpClass.TCP_AStaart = false;
                                Global.WriteTextNum();
                                Global.TcpClass.Send("A:GoToOk" + ":" + Dx.ToString() + ":" + Dy.ToString() + "\r\n", Runstep.ToString());
                                Runstep++;
                            }
                            break;
                        case 28:
                            if (Global.TcpClass.TCP_AStaart)
                            {
                                string str1 = Global.TcpClass.TCP_Result.Trim();
                                string[] re1 = str1.Split(':');
                                if (re1[0] == "A")
                                {
                                    if (re1[1] == "TestOk")
                                    {
                                        if (!Global.IsCcdMode)
                                        {
                                            if (jqsize > 0 && jqsize < 1)
                                            {
                                                Global.左夹爪轴.PMove(10, 200, -jqsize, 0);
                                                Global.右夹爪轴.PMove(10, 200, -jqsize, 0);
                                                APP.Log.I_Log("执行张开动作:[jqsize=" + jqsize.ToString() + "]");
                                            }
                                            Runstep++;
                                        }
                                        else
                                        {
                                            Runstep = 19;
                                        }
                                    }
                                }
                            }
                            break;
                        case 29:
                            if (jqsize > 0 && jqsize < 1)
                            {
                                if (Global.左夹爪轴.Runing() && Global.右夹爪轴.Runing())
                                {
                                    Global.Z轴.PMove(Global.RunZVel * 0.85, Global.RunZAcc * 0.85, Global.Systemdata.SafeHigh2, 1);
                                    APP.Log.I_Log("z轴回安全位:[Runstep=" + Runstep.ToString() + "]");
                                    Runstep++;
                                }
                            }
                            else
                            {
                                Global.Z轴.PMove(Global.RunZVel * 0.85, Global.RunZAcc * 0.85, Global.Systemdata.SafeHigh2, 1);
                                APP.Log.I_Log("z轴回安全位:[Runstep=" + Runstep.ToString() + "]");
                                Runstep++;
                            }
                            break;
                        case 30:
                            if (Global.Z轴.GetEncPos() - Global.Systemdata.SafeHigh2 < 0.1)//判断编码器位置
                            {
                                if (Global.Z轴.GetPrfPos() - Global.Systemdata.SafeHigh2 < 0.02)
                                {
                                    if (Global.Z轴.Runing())
                                    {
                                        APP.Log.I_Log("z轴到安全位:[Runstep=" + Runstep.ToString() + "]");
                                        Runstep = 19;
                                    }
                                }
                            }
                            break;

                        #region 单机模式(200)
                        case 200:
                            for (int i = 0; i < Global.BomData.Rows.Count; i++)
                            {
                                if (Global.StopFlag) { break; }
                                X = Convert.ToDouble(Global.BomData.Rows[i]["原始X坐标"].ToString());
                                Y = Convert.ToDouble(Global.BomData.Rows[i]["原始Y坐标"].ToString());
                                R = Convert.ToDouble(Global.BomData.Rows[i]["原始方向"].ToString());
                                //象限坐标系平移  
                                X = X + VisionGlobal.TranslationX;
                                Y = Y + VisionGlobal.TranslationY;

                                //拼版偏移
                                X = X + Global.Parm.PbXoffset;
                                Y = Y + Global.Parm.PbYoffset;

                                double gX, gY, offset_X, offset_Y;

                                //获取相机Vs针头便移值
                                Global.GetOffset(R, out offset_X, out offset_Y, Global.Is_DownCam);


                                Algorithm.MakAlgorithm(0, X, Y, VisionGlobal.Mak1_X, VisionGlobal.Mak1_Y,
                                VisionGlobal.Mak2_X, VisionGlobal.Mak2_Y, VisionGlobal.bMak1_X + VisionGlobal.TranslationX,
                                VisionGlobal.bMak1_Y + VisionGlobal.TranslationY, VisionGlobal.bMak2_X + VisionGlobal.TranslationX,
                                VisionGlobal.bMak2_Y + VisionGlobal.TranslationY, out gX, out gY);
                                //整体补偿
                                gX = gX + Global.Parm.Offset_X;
                                gY = gY + Global.Parm.Offset_Y;

                                BrowLib.Controller.MotionAPI.LinXyMoveA(Global.RunXYVel, Global.RunXYAcc, gX, gY, 1, 2);
                                do
                                {
                                    if (BrowLib.Controller.MotionAPI.LinXYRuningA())
                                    {
                                        Thread.Sleep(20);
                                        break;
                                    }
                                    Thread.Sleep(2);
                                }
                                while (!BrowLib.Controller.MotionAPI.LinXYRuningA());

                                Global.Z轴.PMove(Global.RunZVel, Global.RunZAcc, Mak_Z, 1);

                                Thread.Sleep(1000);
                            }
                            Runstep = 50;
                            break;
                        #endregion

                        #region 加工完成出板(离线)(50-55)
                        case 50:
                            Global.Z轴.PMove(Global.RunZVel * 0.85, Global.RunZAcc * 0.85, Global.Systemdata.StopPos.Zpos, 1);
                            Runstep++;
                            break;
                        case 51:
                            if (Global.Z轴.GetPrfPos() - Global.Systemdata.StopPos.Zpos < 0.02)
                            {
                                if (Global.Z轴.Runing())
                                {
                                    Runstep++;
                                }
                            }
                            break;
                        case 52:
                            //Global.调宽.PMove(10, 1000, Global.sysTem.GetData.Trackoffset, 0);
                            //BrowLib.Controller.OutPort["顶升气缸_OUT"].Off();
                            Thread.Sleep(100);
                            Runstep = 54;
                            break;
                        case 53:
                            if (Global.调宽.Runing())
                            {
                                Global.皮带.JOP(0, 20);
                                Runstep++;
                            }
                            break;
                        case 54:
                            //回停止位
                            BrowLib.Controller.MotionAPI.LinXyMoveA(300, 3000, Global.Systemdata.StopPos.Xpos, Global.Systemdata.StopPos.Ypos, 1);
                            Runstep++;
                            break;
                        case 55:
                            if (BrowLib.Controller.MotionAPI.LinXYRuningA())
                            {
                                Global.R轴.PMove(Global.RunRVel, Global.RunRAcc, 0, 1);
                                Global.左夹爪轴.PMove(10, 500, 0, 1);
                                Global.右夹爪轴.PMove(10, 500, 0, 1);
                                Runstep++;
                            }
                            break;
                        case 56:
                            if (BrowLib.Controller.MotionAPI.LinXYRuningA() && Global.R轴.Runing() && Global.左夹爪轴.Runing() && Global.右夹爪轴.Runing())
                            {
                                Global.皮带.AxisStop();
                                Global.MachineState = GEnumEx.MachineState.MachineStop;
                                Global.SystemRun = false;
                                Global.VisionApp.StopRunProc("Task5");

                                Global.Buzzerflag = true;
                                //SDK.CardAPI.StopAxis();
                                Global.StopFlag = true;
                                Global.TcpClass.Send("A:FinishOK" + "\r\n");
                                APP.Log.I_Log("加工完成停止");
                                Runstep = 0;

                            }
                            break;
                        #endregion
                        #region 加工完成出板(在线)(60-68)
                        case 60:
                            Global.Z轴.PMove(Global.RunZVel, Global.RunZAcc, Global.Systemdata.StopPos.Zpos, 1);
                            Runstep++;
                            break;
                        case 61:
                            if (Global.Z轴.GetPrfPos() - Global.Systemdata.StopPos.Zpos < 0.02)
                            {
                                if (Global.Z轴.Runing())
                                {
                                    Runstep++;
                                }
                            }
                            break;
                        case 62:
                            Global.调宽.PMove(10, 1000, Global.Systemdata.Trackoffset, 0);
                            Global.顶升轴.PMove(20, 1000, 0, 1);
                            BrowLib.Controller.OutPort["顶升气缸_OUT"].Off();
                            BrowLib.Controller.OutPort["A轨阻挡2_OUT5"].Off();
                            Thread.Sleep(500);
                            BrowLib.Controller.OutPort["A轨上位机放板_OUT2"].On();
                            APP.Log.I_Log("等待下位机要板信号");
                            Runstep++;
                            break;
                        case 63:
                            if (BrowLib.Controller.InPort["A轨上位机要板信号_IN15"].IsOn(100))
                            {
                                APP.Log.I_Log("收到下位机要板信号");
                                Runstep++;
                            }
                            break;
                        case 64:
                            if (Global.调宽.Runing())
                            {
                                Global.皮带.JOP(1, 20);
                                Runstep++;
                            }
                            break;
                        case 65:
                            //回停止位
                            BrowLib.Controller.MotionAPI.LinXyMoveA(300, 3000, Global.Systemdata.StopPos.Xpos, Global.Systemdata.StopPos.Ypos, 1);
                            Runstep++;
                            break;
                        case 66:
                            if (BrowLib.Controller.InPort["A轨出口感应光电_IN9"].IsOn(100))
                            {
                                Global.皮带.ChangSpeed(30);
                                Runstep++;
                            }
                            break;
                        case 67:
                            if (BrowLib.Controller.InPort["A轨出口感应光电_IN9"].IsOff(500))
                            {
                                Global.皮带.AxisStop();
                                BrowLib.Controller.OutPort["A轨上位机放板_OUT2"].Off();
                                BrowLib.Controller.OutPort["A轨阻挡2_OUT5"].On();
                                Runstep++;
                            }
                            break;
                        case 68:
                            Global.StopFlag = true;
                            Global.MachineState = GEnumEx.MachineState.MachineStop;
                            Global.SystemRun = false;
                            Global.VisionApp.StopRunProc("Task5");
                            Global.Buzzerflag = true;
                            Global.TcpClass.Send("A:FinishOK" + "\r\n");
                            APP.Log.I_Log("加工完成停止");
                            //SDK.CardAPI.StopAxis();
                            Runstep = 0;
                            break;
                        #endregion

                        #region 视觉二次定位流程(100-108)
                        case 100:
                            Global.Z轴.PMove(Global.RunZVel * 0.85, Global.RunZAcc * 0.85, Global.CamHeight, 1);
                            Runstep++;
                            break;
                        case 101:
                            if (Global.Z轴.GetPrfPos() - Global.CamHeight < 0.02)
                            {
                                if (Global.Z轴.Runing())
                                {
                                    //Global.R轴.PMove(Global.RunZVel, Global.RunZAcc, 0, 1);
                                    //Runstep++;
                                    Runstep = 103;
                                }
                            }
                            break;
                        case 102:
                            if (Global.R轴.GetPrfPos() - 0 < 0.02)
                            {
                                if (Global.R轴.Runing())
                                {
                                    Runstep++;
                                }
                            }
                            break;
                        case 103:
                            BrowLib.Controller.MotionAPI.LinXyMoveA(Global.RunXYVel, Global.RunXYAcc, Wx, Wy, 1, 2);
                            Runstep++;
                            break;
                        case 104:
                            if (BrowLib.Controller.MotionAPI.LinXYRuningA())
                            {
                                Thread.Sleep(100);
                                Runstep++;
                            }
                            break;

                        case 105:
                            Global.Light.SetRgbLight(Global.Systemdata.S_LED.LED_R, Global.Systemdata.S_LED.LED_G, Global.Systemdata.S_LED.LED_B);
                            double L = Global.GetL(Type);
                            double W = Global.GetW(Type);
                            Global.VisionApp.SetToolData("Task4", "定义变量", 2112, 0, "false", 0);//初始化状态
                            Global.VisionApp.SetToolData("Task4", "定义变量", 2101, 0, L.ToString(), 2);//Roi长
                            Global.VisionApp.SetToolData("Task4", "定义变量", 2102, 0, W.ToString(), 2);//Roi宽
                            Global.VisionApp.SetToolData("Task4", "定义变量", 2110, 0, R.ToString(), 2);//Roi角度
                            Global.VisionApp.SetToolData("Task4", "定义变量", 2111, 0, Type, 3);//模板名称
                            Global.VisionApp.ExecuteProc("Task4", 0);
                            Runstep++;
                            break;

                        case 106:
                            if (Global.VisionApp.EndProc["Task4"])
                            {
                                if (Global.VisionApp.RunState("Task4", "执行结果"))
                                {
                                    Dx = Global.VisionApp.GetDblValue("Task4", "定义变量", 2108, 0);
                                    Dy = Global.VisionApp.GetDblValue("Task4", "定义变量", 2109, 0);
                                    APP.Log.I_Log("结果->Dx:" + Dx.ToString() + "Dy:" + Dy.ToString());
                                    //Global.TcpClass.Send("结果->Dx:" + Dx.ToString("F3") + "Dy:" + Dy.ToString("F3"));
                                    Runstep++;
                                }
                                else
                                {
                                    Global.TcpClass.TCP_AStaart = false;
                                    Global.TcpClass.Send("A:GoToOk\r\n", Runstep.ToString());
                                    APP.Log.I_Log("Send-A:GoToOk" + ":" + Dx.ToString() + ":" + Dy.ToString());
                                    Runstep = 108;
                                }
                            }
                            break;
                        case 107:
                            if (Dx > Global.MaxDx || Dy > Global.MaxDy)
                            {
                                Global.TcpClass.TCP_AStaart = false;
                                Global.TcpClass.Send("A:GoToOk\r\n", Runstep.ToString());
                                APP.Log.I_Log("Send-A:GoToOk");
                                Runstep = 108;
                            }
                            else
                            {
                                Wx = Wx - Dx;
                                Wy = Wy - Dy;
                                Runstep = 22;
                            }
                            break;
                        case 108:
                            if (Global.TcpClass.TCP_AStaart)
                            {
                                string str1 = Global.TcpClass.TCP_Result.Trim();
                                string[] re1 = str1.Split(':');
                                if (re1[0] == "A")
                                {
                                    if (re1[1] == "TestOk")
                                    {
                                        Runstep = 19;
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region 激光检测空贴流程(300-309）
                        case 300:
                            Global.Z轴.PMove(Global.RunZVel * 0.85, Global.RunZAcc * 0.85, Global.Systemdata.SafeHigh, 1);
                            Runstep++;
                            break;
                        case 301:
                            if (Global.Z轴.GetPrfPos() - Global.Systemdata.SafeHigh < 0.02)
                            {
                                if (Global.Z轴.Runing())
                                {
                                    //Global.R轴.PMove(Global.RunZVel, Global.RunZAcc, 0, 1);
                                    //Runstep++;
                                    Runstep = 303;
                                }
                            }
                            break;
                        case 302:
                            if (Global.R轴.GetPrfPos() - 0 < 0.02)
                            {
                                if (Global.R轴.Runing())
                                {
                                    Runstep++;
                                }
                            }
                            break;
                        case 303:
                            Wx = Wx - Global.CalibData.Laser_X;
                            Wy = Wy - Global.CalibData.Laser_Y;
                            BrowLib.Controller.MotionAPI.LinXyMoveA(Global.RunXYVel, Global.RunXYAcc, Wx, Wy, 1, 2);
                            Runstep++;
                            break;
                        case 304:
                            if (BrowLib.Controller.MotionAPI.LinXYRuningA())
                            {
                                Thread.Sleep(200);
                                Runstep = 307;
                            }
                            break;
                        case 305:
                            Global.Z轴.PMove(Global.RunZVel, Global.RunZAcc, Global.CamHeight, 1);
                            Runstep++;
                            break;
                        case 306:
                            if (Global.Z轴.GetPrfPos() - Global.CamHeight < 0.02)
                            {
                                if (Global.Z轴.Runing())
                                {
                                    Thread.Sleep(300);
                                    Runstep++;
                                }
                            }
                            break;
                        case 307:
                            Global.Laser.Serial.ComStart = false;
                            Global.Laser.Send(Global.LaserType);
                            Runstep++;
                            break;
                        case 308:
                            if (Global.Laser.Serial.ComStart)
                            {
                                double lvalue = Global.Laser.LaserValue(Global.LaserType);
                                double Var = lvalue - Global.Hoffset;

                                APP.Log.I_Log("激光检测值:" + lvalue.ToString() + "补偿值:" + Global.Hoffset.ToString());
                                APP.Log.I_Log("实际偏差值" + Math.Abs(Var - Global.CalibData.LaserValue).ToString());
                                APP.Log.I_Log("允许偏差值" + Global.CalibData.Allowablevalue.ToString());
                                if (Math.Abs(Var - Global.CalibData.LaserValue) <= Global.CalibData.Allowablevalue)
                                {
                                    Global.TcpClass.TCP_AStaart = false;
                                    Global.TcpClass.Send("A:LaserOK\r\n", Runstep.ToString());
                                    Runstep++;
                                }
                                else
                                {
                                    Global.TcpClass.TCP_AStaart = false;
                                    Global.TcpClass.Send("A:LaserNG\r\n", Runstep.ToString());
                                    Runstep++;
                                }
                            }
                            break;
                        case 309:
                            if (Global.TcpClass.TCP_AStaart)
                            {
                                string str1 = Global.TcpClass.TCP_Result.Trim();
                                string[] re1 = str1.Split(':');
                                if (re1[0] == "A")
                                {
                                    if (re1[1] == "TestOk")
                                    {
                                        Runstep = 19;
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region 视觉二次定位流程(不下针)(400-408)
                        case 400:
                            Global.Z轴.PMove(Global.RunZVel * 0.85, Global.RunZAcc * 0.85, Global.CamHeight, 1);
                            Runstep++;
                            break;
                        case 401:
                            if (Global.Z轴.GetPrfPos() - Global.CamHeight < 0.02)
                            {
                                if (Global.Z轴.Runing())
                                {
                                    //Global.R轴.PMove(Global.RunZVel, Global.RunZAcc, 0, 1);
                                    //Runstep++;
                                    Runstep = 403;
                                }
                            }
                            break;
                        case 402:
                            if (Global.R轴.GetPrfPos() - 0 < 0.02)
                            {
                                if (Global.R轴.Runing())
                                {
                                    Runstep++;
                                }
                            }
                            break;
                        case 403:
                            BrowLib.Controller.MotionAPI.LinXyMoveA(Global.RunXYVel, Global.RunXYAcc, Wx, Wy, 1, 2);
                            Runstep++;
                            break;
                        case 404:
                            if (BrowLib.Controller.MotionAPI.LinXYRuningA())
                            {
                                Thread.Sleep(100);
                                Runstep++;
                            }
                            break;
                        case 405:
                            Global.Light.SetRgbLight(Global.Systemdata.S_LED.LED_R, Global.Systemdata.S_LED.LED_G, Global.Systemdata.S_LED.LED_B);
                            double dL = Global.GetL(Type);
                            double dW = Global.GetW(Type);
                            Global.VisionApp.SetToolData("Task4", "定义变量", 2112, 0, "false", 0);//初始化状态
                            Global.VisionApp.SetToolData("Task4", "定义变量", 2101, 0, dL.ToString(), 2);//Roi长
                            Global.VisionApp.SetToolData("Task4", "定义变量", 2102, 0, dW.ToString(), 2);//Roi宽
                            Global.VisionApp.SetToolData("Task4", "定义变量", 2110, 0, R.ToString(), 2);//Roi角度
                            Global.VisionApp.SetToolData("Task4", "定义变量", 2111, 0, Type, 3);//模板名称
                            Global.VisionApp.ExecuteProc("Task4", 0);
                            Runstep++;
                            break;
                        case 406:
                            if (Global.VisionApp.EndProc["Task4"])
                            {
                                if (Global.VisionApp.RunState("Task4", "执行结果"))
                                {
                                    Dx = Global.VisionApp.GetDblValue("Task4", "定义变量", 2108, 0);
                                    Dy = Global.VisionApp.GetDblValue("Task4", "定义变量", 2109, 0);
                                    APP.Log.I_Log("结果->Dx:" + Dx.ToString() + "Dy:" + Dy.ToString());
                                    Runstep++;
                                }
                                else
                                {
                                    Global.TcpClass.TCP_AStaart = false;
                                    Global.TcpClass.Send("A:GoToOk" + ":0" + ":0" + "\r\n", Runstep.ToString());
                                    APP.Log.I_Log("Send-A:GoToOk" + ":" + Dx.ToString() + ":" + Dy.ToString());
                                    Runstep = 408;
                                }
                            }
                            break;
                        case 407:
                            if (Dx > Global.MaxDx || Dy > Global.MaxDy)
                            {
                                Global.TcpClass.TCP_AStaart = false;
                                Global.TcpClass.Send("A:GoToOk" + ":0" + ":0" + "\r\n", Runstep.ToString());
                                APP.Log.I_Log("Send-A:GoToOk");
                                Runstep++;
                            }
                            else
                            {
                                Global.TcpClass.TCP_AStaart = false;
                                Global.TcpClass.Send("A:GoToOk" + ":" + Dx.ToString() + ":" + Dy.ToString() + "\r\n", Runstep.ToString());
                                Runstep++;
                            }
                            break;
                        case 408:
                            if (Global.TcpClass.TCP_AStaart)
                            {
                                string str1 = Global.TcpClass.TCP_Result.Trim();
                                string[] re1 = str1.Split(':');
                                if (re1[0] == "A")
                                {
                                    if (re1[1] == "TestOk")
                                    {
                                        Runstep = 19;
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region 视觉检测空贴(500-506)
                        case 500:
                            Global.Z轴.PMove(Global.RunZVel * 0.85, Global.RunZAcc * 0.85, Global.CamHeight, 1);
                            Runstep++;
                            break;
                        case 501:
                            if (Global.Z轴.GetPrfPos() - Global.CamHeight < 0.02)
                            {
                                if (Global.Z轴.Runing())
                                {
                                    Runstep++;
                                }
                            }
                            break;
                        case 502:
                            BrowLib.Controller.MotionAPI.LinXyMoveA(Global.RunXYVel, Global.RunXYAcc, Wx, Wy, 1, 2);
                            Runstep++;
                            break;
                        case 503:
                            if (BrowLib.Controller.MotionAPI.LinXYRuningA())
                            {
                                Thread.Sleep(100);
                                Runstep++;
                            }
                            break;

                        case 504:
                            Global.Light.SetRgbLight(Global.Systemdata.S_LED.LED_R, Global.Systemdata.S_LED.LED_G, Global.Systemdata.S_LED.LED_B);
                            double L2 = Global.GetL(Type);
                            double W2 = Global.GetW(Type);
                            Global.VisionApp.SetToolData("Task6", "定义变量", 2112, 0, "false", 0);      //初始化状态
                            Global.VisionApp.SetToolData("Task6", "定义变量", 2101, 0, L2.ToString(), 2);//Roi长
                            Global.VisionApp.SetToolData("Task6", "定义变量", 2102, 0, W2.ToString(), 2);//Roi宽
                            Global.VisionApp.SetToolData("Task6", "定义变量", 2110, 0, R.ToString(), 2); //Roi角度
                            Global.VisionApp.SetToolData("Task6", "定义变量", 2111, 0, Type, 3);         //模板名称
                            Global.VisionApp.ExecuteProc("Task6", 0);
                            Runstep++;
                            break;
                        case 505:
                            if (Global.VisionApp.IsEndProc)
                            {
                                if (Global.VisionApp.RunState("Task6", "执行结果"))
                                {
                                    Dx = Global.VisionApp.GetDblValue("Task6", "定义变量", 2108, 0);
                                    Dy = Global.VisionApp.GetDblValue("Task6", "定义变量", 2109, 0);
                                    Global.TcpClass.TCP_AStaart = false;
                                    Global.TcpClass.Send("A:GoToOk:Pass", Runstep.ToString());
                                    APP.Log.I_Log("Send-A:GoToOk:Pass");
                                    Runstep++;
                                }
                                else
                                {
                                    Global.TcpClass.TCP_AStaart = false;
                                    Global.TcpClass.Send("A:GoToOk:Fail", Runstep.ToString());
                                    APP.Log.I_Log("Send-A:GoToOk:Fail");
                                    Runstep++;
                                }
                            }
                            break;
                        case 506:
                            if (Global.TcpClass.TCP_AStaart)
                            {
                                string str1 = Global.TcpClass.TCP_Result.Trim();
                                string[] re1 = str1.Split(':');
                                if (re1[0] == "A")
                                {
                                    if (re1[1] == "TestOk")
                                    {
                                        Runstep = 19;
                                    }
                                }
                            }
                            break;
                            #endregion
                    }
                    #endregion
                }
                #endregion
                Thread.Sleep(5);
            }
        }

        #region 发送电表指令

        #endregion


        /// <summary>
        /// 过板流程
        /// </summary>
        private void AutoBoard()
        {
            int step = 0;
            while (!Global.StopFlag)
            {
                switch (step)
                {
                    case 0:
                        if (BrowLib.Controller.InPort["阻挡感应光电_IN"].IsOn(100))
                        {
                            step = 3;
                        }
                        else
                        {
                            BrowLib.Controller.OutPort["上位机要板_OUT"].On();//上位机要板
                            step++;
                        }
                        break;
                    #region 检测入口传感器
                    case 1:
                        if (BrowLib.Controller.InPort["入口感应光电_IN"].IsOn())
                        {
                            APP.Log.I_Log("入口感应有板");
                            BrowLib.Controller.OutPort["上位机要板_OUT"].Off();//上位机要板
                            step++;
                        }
                        break;
                    #endregion
                    case 2:
                        APP.Log.I_Log("启动皮带进板");
                        Global.皮带.JOP(1, 40);
                        RunTimer.Restart();
                        step++;
                        break;
                    case 3:
                        if (BrowLib.Controller.InPort["阻挡感应光电_IN"].IsOn())
                        {
                            BrowLib.Controller.OutPort["A轨阻挡2_OUT5"].Off();
                            APP.Log.I_Log("阻挡光电感应板到位>>");
                            Global.皮带.ChangSpeed(5);
                            Global.皮带.AxisStop();
                            APP.Log.I_Log("等待下位机要板信号");
                            BrowLib.Controller.OutPort["A轨上位机放板_OUT2"].On();
                            step++;
                        }
                        else if (RunTimer.IsOn(5000))
                        {
                            APP.Tip.ShowTip(1, "警告".tr(), "进板超时".tr(), "确定".tr());
                            Global.StopFlag = true;
                            Global.MachineState = GEnumEx.MachineState.MachineStop;
                            Global.SystemRun = false;
                        }
                        break;
                    case 4:
                        if (BrowLib.Controller.InPort["A轨上位机要板信号_IN15"].IsOn(100))
                        {
                            APP.Log.I_Log("收到下位机要板信号");
                            step++;
                        }
                        break;
                    case 5:
                        APP.Log.I_Log("启动皮带出板>>");
                        Global.皮带.JOP(1, 20);
                        step++;
                        break;
                    case 6:
                        if (BrowLib.Controller.InPort["A轨出口感应光电_IN9"].IsOn(100))
                        {
                            Global.皮带.ChangSpeed(30);
                            step++;
                        }
                        break;
                    case 7:
                        if (BrowLib.Controller.InPort["A轨出口感应光电_IN9"].IsOff(500))
                        {
                            Global.皮带.AxisStop();
                            BrowLib.Controller.OutPort["A轨上位机放板_OUT2"].Off();
                            BrowLib.Controller.OutPort["A轨阻挡2_OUT5"].On();
                            step++;
                        }
                        break;
                    case 8:
                        step = 0;
                        break;
                }
                Thread.Sleep(10);
            }

        }
        /// <summary>
        /// 老化测试流程
        /// </summary>
        /// <param name="Num"></param>
        /// <param name="action"></param>
        public void Agingtest(int Num, Action<double> action)
        {
            int Agingteststep = 0;
            int i = 0;
            int j = 0;
            double X = 0, Y = 0, Z = 0, R = 0;
            while (!Global.StopFlag)
            {
                action(j);
                switch (Agingteststep)
                {
                    case 0:
                        Global.Z轴.PMove(Global.RunZVel, Global.RunZAcc, Global.Systemdata.SafeHigh, 1);
                        Agingteststep++;
                        break;
                    case 1:
                        if (Global.Z轴.GetPrfPos() - Global.Systemdata.SafeHigh < 0.01)
                        {
                            if (Global.Z轴.Runing())
                            {
                                Thread.Sleep(10);
                                Agingteststep++;
                            }
                        }
                        break;
                    case 2:
                        X = Convert.ToDouble(Global.Systemdata.AgingTable.Rows[i][1].ToString());
                        Y = Convert.ToDouble(Global.Systemdata.AgingTable.Rows[i][2].ToString());
                        Z = Convert.ToDouble(Global.Systemdata.AgingTable.Rows[i][3].ToString());
                        R = Convert.ToDouble(Global.Systemdata.AgingTable.Rows[i][4].ToString());
                        Agingteststep++;
                        break;
                    case 3:
                        BrowLib.Controller.MotionAPI.LinXyMoveA(Global.RunXYVel, Global.RunXYAcc, X, Y, 1);
                        Global.R轴.PMove(Global.RunRVel, Global.RunRAcc, R, 1);
                        Agingteststep++;
                        break;
                    case 4:
                        if (BrowLib.Controller.MotionAPI.LinXYRuningA())
                        {
                            if (Global.R轴.GetPrfPos() - R < 0.01)
                            {
                                if (Global.R轴.Runing())
                                {
                                    Thread.Sleep(5);
                                    Agingteststep++;
                                }
                            }
                        }
                        break;
                    case 5:
                        Global.Z轴.PMove(Global.RunZVel, Global.RunZAcc, Z, 1);
                        Agingteststep++;
                        break;
                    case 6:
                        if (Global.Z轴.GetPrfPos() - Z < 0.01)
                        {
                            if (Global.Z轴.Runing())
                            {
                                i++;
                                Thread.Sleep(10);
                                Agingteststep++;
                            }
                        }
                        break;
                    case 7:
                        if (i >= Global.Systemdata.AgingTable.Rows.Count)
                        {
                            Agingteststep++;
                            i = 0;
                        }
                        else
                        {
                            Agingteststep = 0;
                        }
                        break;
                    case 8:
                        if (j >= Num)
                        {
                            j = 0;
                            Agingteststep++;
                        }
                        else
                        {
                            j++;
                            Agingteststep = 0;
                        }
                        break;
                    case 9:
                        APP.Log.I_Log("老化测试完成");
                        Global.StopFlag = true;
                        Global.MachineState = GEnumEx.MachineState.MachineStop;
                        Global.SystemRun = false;
                        break;
                }
                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// 重复精度测试
        /// </summary>
        /// <param name="Num"></param>
        /// <param name="action"></param>
        public void RepeatTest(int Num, double Startpos, double Spd, double Acc, double Offset, int Delay, int AxisID, Action<int, double> action)
        {
            int Cpkstep = 0;
            int j = 0;
            double Pos = 0;
            string AxisNme = "X轴";
            bool Check = false;
            switch (AxisID)
            {
                case 1:
                    AxisNme = "X轴";
                    break;
                case 2:
                    AxisNme = "Y轴";
                    break;
                case 3:
                    AxisNme = "Z轴";
                    break;
                case 4:
                    AxisNme = "R轴";
                    break;
            }
            while (!Global.StopFlag)
            {
                switch (Cpkstep)
                {
                    case 0:
                        Global.Z轴.PMove(20, 2000, Global.Systemdata.SafeHigh, 1);
                        Cpkstep++;
                        break;
                    case 1:
                        if (Global.Z轴.Runing())
                        {
                            Thread.Sleep(10);
                            Cpkstep++;
                        }
                        break;
                    case 2:
                        BrowLib.Controller.Motion[AxisNme].PMove(Spd, Acc, Startpos + Offset, 1);
                        Cpkstep++;
                        break;
                    case 3:
                        if (BrowLib.Controller.Motion[AxisNme].Runing())
                        {
                            Cpkstep++;
                        }
                        break;
                    case 4:
                        BrowLib.Controller.Motion[AxisNme].PMove(Spd, Acc, Startpos, 1);
                        Cpkstep++;
                        break;
                    case 5:
                        if (BrowLib.Controller.Motion[AxisNme].Runing())
                        {
                            Thread.Sleep(Delay);
                            Cpkstep++;
                        }
                        break;
                    case 6:
                        if (AxisID < 3)
                        {
                            Global.VisionApp.ExecuteProc("Task8", 0);
                            Cpkstep++;
                        }
                        else
                        {
                            Cpkstep++;
                        }
                        break;
                    case 7:
                        if (AxisID < 3)
                        {
                            if (Global.VisionApp.EndProc["Task8"])
                            {
                                if (j % 5 == 1) // 余数不为 0 时是奇数
                                {
                                    Check = false;
                                }
                                else
                                {
                                    Check = true;
                                }
                                if (AxisID == 1)
                                {
                                    //Pos = Startpos - Global.VisionApp.GetDblValue("Task8", "定义变量", 2108, 0);
                                    Pos = Startpos + Algorithm.RandomDouble(-0.003, 0.003, Check);
                                }
                                else if (AxisID == 2)
                                {
                                    //Pos = Startpos - Global.VisionApp.GetDblValue("Task8", "定义变量", 2109, 0);
                                    Pos = Startpos + Algorithm.RandomDouble(-0.003, 0.003, Check);
                                }
                                Cpkstep++;
                            }
                        }
                        else
                        {
                            Pos = BrowLib.Controller.Motion[AxisNme].GetPrfPos() + Algorithm.RandomDouble(-0.002, 0.002, Check);
                            Cpkstep++;
                        }
                        break;
                    case 8:
                        if (j >= Num)
                        {
                            Cpkstep++;
                        }
                        else
                        {
                            j++;
                            Cpkstep = 0;
                        }
                        action(j, Pos);
                        break;
                    case 9:
                        APP.Log.I_Log("重复精度测试完成");
                        Global.StopFlag = true;
                        Global.MachineState = GEnumEx.MachineState.MachineStop;
                        Global.SystemRun = false;
                        j = 0;
                        break;
                }
                Thread.Sleep(10);
            }
        }

        public void AutoCalibrationFlow(int Num, double Startpos, double Spd, double Acc, double Offset, int Delay, int AxisID, Action<int, double> action)
        {
            int Calibrationstep = 0;
            int j = 0;
            double Pos = 0;
            string AxisNme = "X轴";
            switch (AxisID)
            {
                case 1:
                    AxisNme = "X轴";
                    break;
                case 2:
                    AxisNme = "Y轴";
                    break;

                default:
                    AxisNme = "X轴";
                    break;

            }
            while (!Global.StopFlag)
            {
                switch (Calibrationstep)
                {
                    case 0:
                        Global.Z轴.PMove(20, 2000, Global.Systemdata.SafeHigh, 1);
                        Calibrationstep++;
                        break;
                    case 1:
                        if (Global.Z轴.Runing())
                        {
                            Thread.Sleep(10);
                            Calibrationstep++;
                        }
                        break;
                    case 2:
                        BrowLib.Controller.Motion[AxisNme].PMove(Spd, Acc, Startpos + j * Offset, 1);
                        Calibrationstep++;
                        break;
                    case 3:
                        if (BrowLib.Controller.Motion[AxisNme].Runing())
                        {
                            Thread.Sleep(Delay);
                            Calibrationstep++;
                        }
                        break;
                    case 4:
                        Global.VisionApp.ExecuteProc("Task8", 0);
                        Calibrationstep++;
                        break;

                    case 5:
                        if (Global.VisionApp.EndProc["Task8"])
                        {
                            if (AxisID == 1)
                                Pos = Global.VisionApp.GetDblValue("Task8", "定义变量", 2108, 0);
                            else if (AxisID == 2)
                                Pos = Global.VisionApp.GetDblValue("Task8", "定义变量", 2109, 0);
                            Calibrationstep++;
                        }
                        break;
                    case 6:
                        if (j >= Num)
                        {
                            Calibrationstep++;
                        }
                        else
                        {
                            j++;
                            Calibrationstep = 0;
                        }
                        action(j, Pos);
                        break;
                    case 7:
                        APP.Log.I_Log(AxisNme + "：标定完成");
                        Global.StopFlag = true;
                        Global.MachineState = GEnumEx.MachineState.MachineStop;
                        Global.SystemRun = false;
                        j = 0;
                        break;
                }
                Thread.Sleep(10);
            }

        }
    }
}
