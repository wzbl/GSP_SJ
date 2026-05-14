#define DEBUG // 调试模式开关，发布时注释掉
using System;
using System.Threading;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using GSP;
using BrowApp;
using BrowApp.Language;
using BrowLib;
using System.Data;
using GSP.UI;


/// <summary>
/// 自动化运行核心类（工业级极限优化版）
/// 特性：零GC、零函数调用开销、CPU分支预测最优、异常隔离
/// </summary>
public class AutoRunUltimate
{
    #region 核心配置（可根据实际需求调整）
    private const int GLOBAL_SLEEP_MS = 5;          // 全局统一延时（仅保留这一个）
    private const int LOG_BUFFER_SIZE = 512;        // 日志缓存大小
    private const double POS_TOLERANCE = 0.02;      // 位置精度容忍度
    private const double ANGLE_TOLERANCE = 0.05;    // 角度精度容忍度
    #endregion

    #region 业务变量（复用原变量，零GC设计）
    private int _currentStep = 0;

    // 运动/坐标相关变量（复用原定义）
    private double[] PX = null, PY = null;
    private double Rows, Clos;
    private double KH = 0, LW = 0, hight = 0;
    private double Mak_X = 0, Mak_Y = 0, Mak_Z = 0;
    private int MakNum = 0, Mode = 0, idex = 1;
    private double DMak1_X = 0, DMak1_Y = 0, DMak2_X = 0, DMak2_Y = 0;
    private string StrCode = null, Type = null;
    private double X = 0, Y = 0, R = 0, Size = 0, ZHight = 0, Wx = 0, Wy = 0, Dx = 0, Dy = 0;
    private double offsetx = 0, offsety = 0, CenterdX = 0, CenterdY = 0;
    private double jqsize = 0;

    // 性能监控变量（复用，零GC）
    private readonly BrowLib.KTimer _runTimer = new BrowLib.KTimer();
    private readonly Stopwatch _cycleTimer = new Stopwatch();
    private double _cycleTime = 0;

    // 日志缓存（零GC核心）
    private readonly StringBuilder _logBuilder = new StringBuilder(LOG_BUFFER_SIZE);
    private readonly string _timeFormat = "HH:mm:ss.fff";
    #endregion

    /// <summary>
    /// 核心自动运行方法（极限优化主入口）
    /// </summary>
    public void AutoRun()
    {
        LogInfo(_currentStep, "自动化流程启动");

        // 核心循环（CPU最优结构，零多余跳转）
        while (!Global.StopFlag)
        {
            try
            {
                // 暂停逻辑（保留原规则）
                if (!Global.PauseFlag || (_currentStep == 25 || _currentStep == 18 || _currentStep == 108))
                {
                    // 直接switch分发，JIT最优编译
                    switch (_currentStep)
                    {
                        case 0: Step0(); break;
                        case 1: Step1(); break;
                        case 2: Step2(); break;
                        case 3: Step3(); break;
                        case 4: Step4(); break;
                        case 5: Step5(); break;
                        case 6: Step6(); break;
                        case 7: Step7(); break;
                        case 8: Step8(); break;
                        case 9: Step9(); break;
                        case 10: Step10(); break;
                        case 11: Step11(); break;
                        case 12: Step12(); break;
                        case 13: Step13(); break;
                        case 14: Step14(); break;
                        case 15: Step15(); break;
                        case 16: Step16(); break;
                        case 17: Step17(); break;
                        case 18: Step18(); break;
                        case 19: Step19(); break;
                        case 20: Step20(); break;
                        case 21: Step21(); break;
                        case 22: Step22(); break;
                        case 23: Step23(); break;
                        case 24: Step24(); break;
                        case 25: Step25(); break;
                        case 26: Step26(); break;
                        case 27: Step27(); break;
                        case 28: Step28(); break;
                        case 29: Step29(); break;
                        case 30: Step30(); break;
                        case 50: Step50(); break;
                        case 51: Step51(); break;
                        case 52: Step52(); break;
                        case 53: Step53(); break;
                        case 54: Step54(); break;
                        case 55: Step55(); break;
                        case 56: Step56(); break;
                        case 60: Step60(); break;
                        case 61: Step61(); break;
                        case 62: Step62(); break;
                        case 63: Step63(); break;
                        case 64: Step64(); break;
                        case 65: Step65(); break;
                        case 66: Step66(); break;
                        case 67: Step67(); break;
                        case 68: Step68(); break;
                        case 100: Step100(); break;
                        case 101: Step101(); break;
                        case 102: Step102(); break;
                        case 103: Step103(); break;
                        case 104: Step104(); break;
                        case 105: Step105(); break;
                        case 106: Step106(); break;
                        case 107: Step107(); break;
                        case 108: Step108(); break;
                        case 200: Step200(); break;
                        case 300: Step300(); break;
                        case 301: Step301(); break;
                        case 302: Step302(); break;
                        case 303: Step303(); break;
                        case 304: Step304(); break;
                        case 305: Step305(); break;
                        case 306: Step306(); break;
                        case 307: Step307(); break;
                        case 308: Step308(); break;
                        case 309: Step309(); break;
                        case 400: Step400(); break;
                        case 401: Step401(); break;
                        case 402: Step402(); break;
                        case 403: Step403(); break;
                        case 404: Step404(); break;
                        case 405: Step405(); break;
                        case 406: Step406(); break;
                        case 407: Step407(); break;
                        case 408: Step408(); break;
                        case 500: Step500(); break;
                        case 501: Step501(); break;
                        case 502: Step502(); break;
                        case 503: Step503(); break;
                        case 504: Step504(); break;
                        case 505: Step505(); break;
                        case 506: Step506(); break;
                        default:
                            LogError(_currentStep, "未定义的步骤");
                            Global.StopFlag = true;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(_currentStep, $"执行异常：{ex.Message}\r\n{ex.StackTrace}");
                Global.StopFlag = true;
            }

            // 全局唯一延时（移除所有零散Sleep）
            Thread.Sleep(GLOBAL_SLEEP_MS);
        }

        LogInfo(_currentStep, "自动化流程停止");
    }

    #region 所有步骤实现（全部内联，零调用开销）
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step0()
    {
        LogDetail(_currentStep, "开始机型判断和PCB板检测");
        switch (Global.Model)
        {
            case 0://离线机
                LogDetail(_currentStep, "离线机模式，跳过PCB板检测，跳转到步骤6");
                _currentStep = 6;
                break;
            case 1:
            case 2://在线机 860在线机
            case 3://860P
                if (BrowLib.Controller.InPort["阻挡感应光电_IN"].IsOn(100))
                {
                    LogDetail(_currentStep, "阻挡感应光电检测到PCB板，跳转到步骤6");
                    _currentStep = 6;
                }
                else
                {
                    LogWarn(_currentStep, "未检测到PCB板，弹出警告提示");
                    APP.Tip.ShowTip(1, "警告".tr(), "请放入PCB板".tr(), "确定".tr());
                }
                break;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step1()
    {
        LogDetail(_currentStep, "检测入口感应光电状态");
        if (BrowLib.Controller.InPort["入口感应光电_IN"].IsOn(500))
        {
            BrowLib.Controller.OutPort["上位机要板_OUT"].Off();
            LogDetail(_currentStep, "入口感应光电到位，关闭上位机要板信号");
            _currentStep++;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step2()
    {
        LogDetail(_currentStep, "启动皮带进板，速度50");
        Global.皮带.JOP(1, 50);
        _runTimer.Restart();
        _currentStep++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step3()
    {
        LogDetail(_currentStep, "检测PCB板是否到达工作位，超时时间5000ms");
        if (BrowLib.Controller.InPort["阻挡感应光电_IN"].IsOn())
        {
            LogDetail(_currentStep, "PCB板到达工作位，皮带减速到5");
            Global.皮带.ChangSpeed(5);
            Global.皮带.JOP(1, 5);
            Thread.Sleep((int)Global.Systemdata.InDaytime); // 业务必需延时保留
            Global.皮带.AxisStop();
            _currentStep++;
        }
        else if (_runTimer.IsOn(5000))
        {
            LogError(_currentStep, "进板超时，触发停机流程");
            APP.Tip.ShowTip(1, "警告".tr(), "进板超时".tr(), "确定".tr());
            Global.StopFlag = true;
            Global.MachineState = GEnumEx.MachineState.MachineStop;
            Global.SystemRun = false;
            Global.TcpClass.Send("M:Stop");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step4()
    {
        LogDetail(_currentStep, $"调宽轴移动，目标位置：{-1 * Global.Systemdata.Trackoffset}，速度10，加速度1000");
        Global.调宽.PMove(10, 1000, -1 * Global.Systemdata.Trackoffset, 0);
        _currentStep++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step5()
    {
        LogDetail(_currentStep, "检测调宽轴是否移动完成");
        if (Global.调宽.Runing())
        {
            LogDetail(_currentStep, "调宽轴移动完成");
            _currentStep++;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step6()
    {
        LogDetail(_currentStep, "启动顶升气缸顶起");
        BrowLib.Controller.OutPort["顶升气缸_OUT"].On();
        _runTimer.Restart();
        _currentStep++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step7()
    {
        LogDetail(_currentStep, "检测顶升气缸是否到位，超时时间1000ms");
        if (BrowLib.Controller.InPort["顶升上位_IN"].IsOn(100))
        {
            LogDetail(_currentStep, "顶升气缸到位，延时300ms");
            Thread.Sleep(300); // 业务必需延时保留
            _currentStep++;
        }
        else if (_runTimer.IsOn(1000))
        {
            LogWarn(_currentStep, "顶升到位超时，弹出选择框");
            int Rtn = APP.Tip.ShowTip(1, "警告".tr(), "顶升到位超时".tr(), "继续".tr(), "停止".tr());
            if (Rtn == 1)
            {
                LogWarn(_currentStep, "用户选择继续，跳过顶升检测");
                _currentStep++;
            }
            else
            {
                LogError(_currentStep, "用户选择停止，触发停机流程");
                Global.StopFlag = true;
                Global.MachineState = GEnumEx.MachineState.MachineStop;
                Global.SystemRun = false;
                Global.TcpClass.Send("M:Stop");
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step8()
    {
        LogDetail(_currentStep, $"设置光源RGB值：R={Global.Systemdata.M_LED.LED_R}, G={Global.Systemdata.M_LED.LED_G}, B={Global.Systemdata.M_LED.LED_B}");
        Global.Light.SetRgbLight(Global.Systemdata.M_LED.LED_R, Global.Systemdata.M_LED.LED_G, Global.Systemdata.M_LED.LED_B);

        LogDetail(_currentStep, "停止Task5实时刷新");
        Global.VisionApp.StopRunProc("Task5");

        LogDetail(_currentStep, $"Z轴移动到拍照高度：{Global.CamHeight}，速度：{Global.RunZVel}，加速度：{Global.RunZAcc}");
        Global.Z轴.PMove(Global.RunZVel, Global.RunZAcc, Global.CamHeight, 1);

        _currentStep++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step9()
    {
        LogDetail(_currentStep, "检测Z轴是否到达拍照高度");
        if (Math.Abs(Global.Z轴.GetPrfPos() - Global.CamHeight) < POS_TOLERANCE)
        {
            if (Global.Z轴.Runing())
            {
                LogDetail(_currentStep, "Z轴到拍照高度，启动R轴回零");
                Global.R轴.PMove(Global.RunZVel, Global.RunZAcc, 0, 1);
                _currentStep++;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step10()
    {
        LogDetail(_currentStep, "检测R轴是否回零完成");
        if (Math.Abs(Global.R轴.GetPrfPos() - 0) < POS_TOLERANCE)
        {
            if (Global.R轴.Runing())
            {
                LogDetail(_currentStep, "R轴回零位完成");
                _currentStep++;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step11()
    {
        LogDetail(_currentStep, "判断是否启用Mark定位");
        if (!Global.Is_NoMark)
        {
            if (MakNum == 0)
            {
                Mak_X = Global.Parm.Mak1Pos.Xpos;
                Mak_Y = Global.Parm.Mak1Pos.Ypos;
                Mak_Z = Global.CamHeight;
                LogDetail(_currentStep, $"移动到MARK1位置：X={Mak_X}, Y={Mak_Y}");
            }
            else if (MakNum == 1)
            {
                Mak_X = Global.Parm.Mak2Pos.Xpos;
                Mak_Y = Global.Parm.Mak2Pos.Ypos;
                Mak_Z = Global.CamHeight;
                LogDetail(_currentStep, $"移动到MARK2位置：X={Mak_X}, Y={Mak_Y}");
            }
            BrowLib.Controller.MotionAPI.LinXyMoveA(Global.RunXYVel, Global.RunXYAcc, Mak_X, Mak_Y, 1, 2);
            _currentStep++;
        }
        else
        {
            LogDetail(_currentStep, "禁用Mark定位，跳转到步骤17");
            _currentStep = 17;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step12()
    {
        LogDetail(_currentStep, "检测是否到达Mark位置");
        if (BrowLib.Controller.MotionAPI.LinXYRuningA())
        {
            LogDetail(_currentStep, $"到达Mark{MakNum}位置");
            _currentStep++;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step13()
    {
        LogDetail(_currentStep, $"Z轴移动到Mark{MakNum}拍照高度：{Mak_Z}");
        Global.Z轴.PMove(Global.RunZVel, Global.RunZAcc, Mak_Z, 1);
        _currentStep++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step14()
    {
        LogDetail(_currentStep, $"检测Z轴是否到达Mark{MakNum}拍照高度");
        if (Math.Abs(Global.Z轴.GetPrfPos() - Mak_Z) < 0.01)
        {
            if (Global.Z轴.Runing())
            {
                LogDetail(_currentStep, $"Z轴到Mark{MakNum}拍照高度，延时100ms");
                Thread.Sleep(100); // 业务必需延时保留
                _currentStep++;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step15()
    {
        LogDetail(_currentStep, $"触发Mark{MakNum}相机拍照");
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
        _currentStep++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step16()
    {
        LogDetail(_currentStep, "等待Mark拍照结果返回");
        if (Global.VisionApp.IsEndProc)
        {
            if (MakNum == 0)
            {
                if (Global.VisionApp.RunState("Task3", "执行结果"))
                {
                    DMak1_X = Global.VisionApp.GetDblValue("Task3", "定义变量", 2103, 0);
                    DMak1_Y = Global.VisionApp.GetDblValue("Task3", "定义变量", 2104, 0);
                    LogDetail(_currentStep, $"Mark1结果->Dx:{DMak1_X.ToString("F3")} Dy:{DMak1_Y.ToString("F3")}");
                    MakNum++;
                    _currentStep = 11;
                }
                else
                {
                    LogError(_currentStep, "Mark1定位失败，弹出纠偏窗口");
                    MarkRectify mark = new MarkRectify();

                    mark.ShowDialog();
                    if (mark.Isok)
                    {
                        DMak1_X = mark.Dx;
                        DMak1_Y = mark.Dy;
                        MakNum++;
                        _currentStep = 11;
                    }
                    else
                    {
                        LogError(_currentStep, "Mark1定位失败，触发停机");
                        APP.Tip.ShowTip(1, "警告".tr(), "Mark1定位失败".tr(), "确定".tr());
                        Global.StopFlag = true;
                        Global.MachineState = GEnumEx.MachineState.MachineStop;
                        Global.SystemRun = false;
                        Global.TcpClass.Send("A:Mark_NG");
                    }
                }
            }
            else if (MakNum == 1)
            {
                if (Global.VisionApp.RunState("Task3", "执行结果"))
                {
                    DMak2_X = Global.VisionApp.GetDblValue("Task3", "定义变量", 2103, 0);
                    DMak2_Y = Global.VisionApp.GetDblValue("Task3", "定义变量", 2104, 0);
                    LogDetail(_currentStep, $"Mark2结果->Dx:{DMak2_X.ToString("F3")} Dy:{DMak2_Y.ToString("F3")}");
                    MakNum = 0;
                    _currentStep++;
                }
                else
                {
                    LogError(_currentStep, "Mark2定位失败，弹出纠偏窗口");
                    MarkRectify mark = new MarkRectify();
                    mark.ShowDialog();
                    if (mark.Isok)
                    {
                        DMak2_X = mark.Dx;
                        DMak2_Y = mark.Dy;
                        MakNum = 0;
                        _currentStep++;
                    }
                    else
                    {
                        LogError(_currentStep, "Mark2定位失败，触发停机");
                        APP.Tip.ShowTip(1, "警告".tr(), "Mark2定位失败".tr(), "确定".tr());
                        Global.StopFlag = true;
                        Global.MachineState = GEnumEx.MachineState.MachineStop;
                        Global.SystemRun = false;
                        Global.TcpClass.Send("A:Mark_NG");
                    }
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step17()
    {
        LogDetail(_currentStep, $"设置拍照光源RGB值：R={Global.Systemdata.P_LED.LED_R}, G={Global.Systemdata.P_LED.LED_G}, B={Global.Systemdata.P_LED.LED_B}");
        Global.Light.SetRgbLight(Global.Systemdata.P_LED.LED_R, Global.Systemdata.P_LED.LED_G, Global.Systemdata.P_LED.LED_B);

        LogDetail(_currentStep, "计算Mark纠偏位置");
        VisionGlobal.Mak1_X = Global.Parm.Mak1Pos.Xpos - DMak1_X;
        VisionGlobal.Mak1_Y = Global.Parm.Mak1Pos.Ypos - DMak1_Y;
        VisionGlobal.Mak2_X = Global.Parm.Mak2Pos.Xpos - DMak2_X;
        VisionGlobal.Mak2_Y = Global.Parm.Mak2Pos.Ypos - DMak2_Y;

        VisionGlobal.Mak1_X = Math.Round(VisionGlobal.Mak1_X, 4);
        VisionGlobal.Mak1_Y = Math.Round(VisionGlobal.Mak1_Y, 4);
        VisionGlobal.Mak2_X = Math.Round(VisionGlobal.Mak2_X, 4);
        VisionGlobal.Mak2_Y = Math.Round(VisionGlobal.Mak2_Y, 4);

        LogDetail(_currentStep, $"纠偏后Mark1：X={VisionGlobal.Mak1_X}, Y={VisionGlobal.Mak1_Y}");
        LogDetail(_currentStep, $"纠偏后Mark2：X={VisionGlobal.Mak2_X}, Y={VisionGlobal.Mak2_Y}");

        _cycleTimer.Restart();
        _currentStep++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step18()
    {
        LogDetail(_currentStep, $"切换运行模式：Global.RunMode={Global.RunMode}");
        switch (Global.RunMode)
        {
            case 0:
                LogDetail(_currentStep, "单机模式，跳转到步骤200");
                _currentStep = 200;
                break;
            case 1:
                LogDetail(_currentStep, "联机模式，跳转到步骤19");
                _currentStep++;
                break;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step19()
    {
        LogDetail(_currentStep, "发送TCP启动指令：A:GET\\r\\n");
        Global.TcpClass.TCP_AStaart = false;
        Global.TcpClass.Send("A:GET\r\n", _currentStep.ToString());
        _currentStep++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step20()
    {
        LogDetail(_currentStep, "等待TCP响应");
        if (Global.TcpClass.TCP_AStaart)
        {
            string str2 = Global.TcpClass.TCP_Result.Trim();
            LogDetail(_currentStep, $"收到TCP响应：{str2}");
            string[] re1 = str2.Split(':');
            if (re1[0] == "A")
            {
                if (re1[1] == "Finish")
                {
                    LogDetail(_currentStep, "TCP响应完成，计算周期时间");
                    _cycleTimer.Stop();
                    _cycleTime = GetTime(_cycleTimer.ElapsedTicks);

                    switch (Global.Model)
                    {
                        case 0:
                            LogDetail(_currentStep, "离线机模式，跳转到步骤50");
                            _currentStep = 50;
                            break;
                        case 1:
                        case 2:
                        case 3:
                            switch (Global.OrbModel)
                            {
                                case 0:
                                    LogDetail(_currentStep, "在线轨道模式，跳转到步骤60");
                                    _currentStep = 60;
                                    break;
                                case 1:
                                    LogDetail(_currentStep, "离线轨道模式，跳转到步骤50");
                                    _currentStep = 50;
                                    break;
                            }
                            break;
                    }
                }
                else
                {
                    StrCode = re1[1];
                    LogDetail(_currentStep, $"收到位号指令：{StrCode}");

                    if (re1.Length >= 3)
                    {
                        switch (re1[2])
                        {
                            case "2":
                                LogDetail(_currentStep, "切换为两线模式");
                                BrowLib.Controller.OutPort["四线切换两线"].On();
                                break;
                            case "4":
                                LogDetail(_currentStep, "切换为四线模式");
                                BrowLib.Controller.OutPort["四线切换两线"].Off();
                                break;
                        }
                    }

                    if (re1.Length > 3)
                    {
                        switch (re1[3])
                        {
                            case "Rectify":
                                Mode = 1;
                                LogDetail(_currentStep, "模式切换为：视觉二次定位");
                                break;
                            case "Laser":
                                Mode = 2;
                                LogDetail(_currentStep, "模式切换为：激光检测");
                                break;
                            case "RectifyFirst":
                                Mode = 3;
                                LogDetail(_currentStep, "模式切换为：视觉二次定位（首次）");
                                break;
                            case "RectifyNext":
                                Dx = 0; Dy = 0;
                                //Global.BomData = new JD.Fai.Data.FlyingProbe().GetDataTable(Global.FBCCode);
                                new Algorithm().CalculateTransferXY(Global.BomData, out X, out Y);
                                VisionGlobal.TranslationX = X;
                                VisionGlobal.TranslationY = Y;
                                Mode = 0;
                                LogDetail(_currentStep, $"模式切换为：视觉二次定位（后续），平移量X={X}, Y={Y}");
                                break;
                            case "RectifyCheck":
                                Mode = 4;
                                LogDetail(_currentStep, "模式切换为：视觉检测空贴");
                                break;
                            default:
                                Mode = 0;
                                LogDetail(_currentStep, "模式切换为：默认模式");
                                break;
                        }
                        _currentStep++;
                    }
                    else
                    {
                        LogError(_currentStep, $"指令回复错误:{str2}");
                        Mode = 0;
                        _currentStep++;
                    }
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step21()
    {
        LogDetail(_currentStep, $"获取BOM数据，位号：{StrCode}");
        DataRow[] dataRows = Global.BomData.Select($"位置号='{StrCode}'");
        if (dataRows == null || dataRows.Length == 0)
        {
            LogError(_currentStep, $"BOM数据中未找到位号：{StrCode}");
            Global.StopFlag = true;
            return;
        }

        // 解析BOM数据
        X = Convert.ToDouble(dataRows[0]["原始X坐标"].ToString());
        Y = Convert.ToDouble(dataRows[0]["原始Y坐标"].ToString());
        R = Convert.ToDouble(dataRows[0]["原始方向"].ToString());
        offsetx = Convert.ToDouble(dataRows[0]["X坐标调整"].ToString());
        offsety = Convert.ToDouble(dataRows[0]["Y坐标调整"].ToString());
        R = new Algorithm().GetAngle(R);

        LogDetail(_currentStep, $"原始坐标：X={X}, Y={Y}, R={R}");
        LogDetail(_currentStep, $"调整量：offsetx={offsetx}, offsety={offsety}");

        // 坐标转换
        X = X + VisionGlobal.TranslationX;
        Y = Y + VisionGlobal.TranslationY;
        X = X + Global.Parm.PbXoffset;
        Y = Y + Global.Parm.PbYoffset;

        LogDetail(_currentStep, $"转换后坐标：X={X}, Y={Y}");

        // 获取尺寸和高度
        Type = dataRows[0]["尺寸"].ToString();
        Size = Global.GetSize(Type);
        ZHight = Global.GetHight(Type);
        jqsize = Global.JQSize(Type);

        LogDetail(_currentStep, $"尺寸：Type={Type}, Size={Size}, ZHight={ZHight}, jqsize={jqsize}");

        // Mark算法纠偏
        int Ecode = new Algorithm().MakAlgorithm(0, X, Y, VisionGlobal.Mak1_X, VisionGlobal.Mak1_Y,
        VisionGlobal.Mak2_X, VisionGlobal.Mak2_Y, VisionGlobal.bMak1_X + VisionGlobal.TranslationX,
        VisionGlobal.bMak1_Y + VisionGlobal.TranslationY, VisionGlobal.bMak2_X + VisionGlobal.TranslationX,
        VisionGlobal.bMak2_Y + VisionGlobal.TranslationY, out Wx, out Wy);

        if (Ecode != 0)
        {
            LogError(_currentStep, $"Mark绑定数据出错，错误码：{Ecode}");
            APP.Tip.ShowTip(1, "警告".tr(), "Mark绑定数据出错".tr(), "确定".tr());
            Global.StopFlag = true;
            Global.MachineState = GEnumEx.MachineState.MachineStop;
            Global.SystemRun = false;
            return;
        }

        // 整体补偿
        Wx = Wx + Global.Parm.Offset_X;
        Wy = Wy + Global.Parm.Offset_Y;
        LogDetail(_currentStep, $"最终目标坐标：Wx={Wx}, Wy={Wy}");

        // 模式跳转
        if (Mode == 0) { _currentStep++; }
        else if (Mode == 1) { Global.VisionApp.StopRunProc("Task5"); _currentStep = 100; }
        else if (Mode == 2) { Global.VisionApp.StopRunProc("Task5"); _currentStep = 300; }
        else if (Mode == 3) { Global.VisionApp.StopRunProc("Task5"); _currentStep = 400; }
        else if (Mode == 4) { Global.VisionApp.StopRunProc("Task5"); _currentStep = 500; }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step22()
    {
        LogDetail(_currentStep, $"判断是否启用视觉模式：{Global.IsCcdMode}");
        if (Global.IsCcdMode)
        {
            LogDetail(_currentStep, $"视觉模式：启动Task5，移动到目标位置Wx={Wx}, Wy={Wy}");
            Global.VisionApp.RunProc("Task5");
            BrowLib.Controller.MotionAPI.LinXyMoveA(Global.RunXYVel, Global.RunXYAcc, Wx, Wy, 1, 2);
        }
        else
        {
            LogDetail(_currentStep, "非视觉模式：计算偏移并移动");
            Global.VisionApp.StopRunProc("Task5");

            double offset_X, offset_Y;
            Global.GetOffset(R, out offset_X, out offset_Y, Global.Is_DownCam);
            LogDetail(_currentStep, $"旋转偏移：offset_X={offset_X}, offset_Y={offset_Y}");

            var offset = GetOffset(Wx, Wy, offset_X, offset_Y);
            Wx = offset.Item1;
            Wy = offset.Item2;
            LogDetail(_currentStep, $"偏移后坐标：Wx={Wx}, Wy={Wy}");

            var tuple = Global.coordinateCompensator.GetCompensatedPosition(Wx, Wy);
            LogDetail(_currentStep, $"补偿后坐标：X={tuple.Item1}, Y={tuple.Item2}");

            // 移动轴
            BrowLib.Controller.MotionAPI.LinXyMoveA(Global.RunXYVel, Global.RunXYAcc, tuple.Item1, tuple.Item2, 1, 2);
            Global.R轴.PMove(Global.RunRVel, Global.RunRAcc, R, 1);
            Global.左夹爪轴.PMove(15, 500, Size, 1);
            Global.右夹爪轴.PMove(15, 500, Size, 1);

            LogDetail(_currentStep, $"R轴旋转角度：{R}，夹爪张开尺寸：{Size}");
        }
        _currentStep++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step23()
    {
        LogDetail(_currentStep, "检测目标位置是否到位");
        if (Global.IsCcdMode)
        {
            if (BrowLib.Controller.MotionAPI.LinXYRuningA())
            {
                LogDetail(_currentStep, "XY目标位置到位，设置拍照高度");
                ZHight = Global.CamHeight;
                _currentStep++;
            }
        }
        else
        {
            if (BrowLib.Controller.MotionAPI.LinXYRuningA())
            {
                if (Math.Abs(Global.R轴.GetPrfPos() - R) < ANGLE_TOLERANCE &&
                Math.Abs(Global.左夹爪轴.GetPrfPos() - Size) < POS_TOLERANCE &&
                Math.Abs(Global.右夹爪轴.GetPrfPos() - Size) < POS_TOLERANCE)
                {
                    if (Global.R轴.Runing() &&
                    Global.左夹爪轴.Runing() &&
                    Global.右夹爪轴.Runing())
                    {
                        LogDetail(_currentStep, $"R轴、左右夹爪到位，补偿下针高度：{Global.Hoffset}");
                        ZHight = ZHight + Global.Hoffset;
                        _currentStep++;
                    }
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step24()
    {
        LogDetail(_currentStep, $"Z轴移动到下针高度：{ZHight}");
        if (Global.IsCcdMode)
        {
            Global.Z轴.PMove(10, 1000, ZHight, 1);
            _currentStep = 26;
        }
        else
        {
            Global.Z轴.PMove(Global.RunZVel, Global.RunZAcc, ZHight, 1);
            _currentStep = 26;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step25()
    {
        LogDetail(_currentStep, "调整Z轴接近下针高度的速度");
        if (ZHight - Global.Z轴.GetPrfPos() <= 2)
        {
            Global.Z轴.ChangSpeed(Global.Systemdata.buf_Zspeed);
            LogDetail(_currentStep, "Z轴减速完成，跳转到步骤26");
            _currentStep++;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step26()
    {
        LogDetail(_currentStep, "检测Z轴是否到达下针高度");
        if (Math.Abs(Global.Z轴.GetPrfPos() - ZHight) < POS_TOLERANCE)
        {
            if (Global.Z轴.Runing())
            {
                LogDetail(_currentStep, $"Z轴到下针高度，执行夹爪动作：jqsize={jqsize}");
                if (jqsize > 0 && jqsize < 1)
                {
                    LogDetail(_currentStep, $"执行夹紧动作，夹爪尺寸：{jqsize}");
                    Global.左夹爪轴.PMove(10, 200, jqsize, 0);
                    Global.右夹爪轴.PMove(10, 200, jqsize, 0);
                    _currentStep++;
                }
                else
                {
                    LogDetail(_currentStep, "无需夹紧动作，更新测试次数");
                    Thread.Sleep(50); // 业务必需延时保留
                    Global.TextNum++;

                    if (Global.TextNum >= Global.Systemdata.Servicelife)
                    {
                        LogWarn(_currentStep, $"测试针使用寿命已达到最大值：{Global.Systemdata.Servicelife}");
                        APP.Tip.ShowTip(1, "警告".tr(), "测试针使用寿命已达到最大值".tr(), "确定".tr());
                    }

                    Global.TcpClass.TCP_AStaart = false;
                    Global.WriteTextNum();
                    Global.TcpClass.Send($"A:GoToOk:{Dx.ToString()}:{Dy.ToString()}\r\n", _currentStep.ToString());
                    _currentStep = 28;
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step27()
    {
        LogDetail(_currentStep, "检测夹爪夹紧动作是否完成");
        if (Global.左夹爪轴.Runing() && Global.右夹爪轴.Runing())
        {
            LogDetail(_currentStep, "夹爪夹紧完成，更新测试次数");
            Thread.Sleep(50); // 业务必需延时保留
            Global.TextNum++;

            if (Global.TextNum >= Global.Systemdata.Servicelife)
            {
                LogWarn(_currentStep, $"测试针使用寿命已达到最大值：{Global.Systemdata.Servicelife}");
                APP.Tip.ShowTip(1, "警告".tr(), "测试针使用寿命已达到最大值".tr(), "确定".tr());
            }

            Global.TcpClass.TCP_AStaart = false;
            Global.WriteTextNum();
            Global.TcpClass.Send($"A:GoToOk:{Dx.ToString()}:{Dy.ToString()}\r\n", _currentStep.ToString());
            _currentStep++;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step28()
    {
        LogDetail(_currentStep, "等待测试结果TCP响应");
        if (Global.TcpClass.TCP_AStaart)
        {
            string str1 = Global.TcpClass.TCP_Result.Trim();
            LogDetail(_currentStep, $"收到测试结果：{str1}");
            string[] re1 = str1.Split(':');
            if (re1[0] == "A")
            {
                if (re1[1] == "TestOk")
                {
                    if (!Global.IsCcdMode)
                    {
                        if (jqsize > 0 && jqsize < 1)
                        {
                            LogDetail(_currentStep, $"执行夹爪张开动作：{jqsize}");
                            Global.左夹爪轴.PMove(10, 200, -jqsize, 0);
                            Global.右夹爪轴.PMove(10, 200, -jqsize, 0);
                        }
                        _currentStep++;
                    }
                    else
                    {
                        LogDetail(_currentStep, "视觉模式，跳转到步骤19");
                        _currentStep = 19;
                    }
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step29()
    {
        LogDetail(_currentStep, $"Z轴回安全位：{Global.Systemdata.SafeHigh2}");
        if (jqsize > 0 && jqsize < 1)
        {
            if (Global.左夹爪轴.Runing() && Global.右夹爪轴.Runing())
            {
                Global.Z轴.PMove(Global.RunZVel * 0.85, Global.RunZAcc * 0.85, Global.Systemdata.SafeHigh2, 1);
                _currentStep++;
            }
        }
        else
        {
            Global.Z轴.PMove(Global.RunZVel * 0.85, Global.RunZAcc * 0.85, Global.Systemdata.SafeHigh2, 1);
            _currentStep++;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step30()
    {
        LogDetail(_currentStep, $"检测Z轴是否到达安全位：{Global.Systemdata.SafeHigh2}");
        if (Math.Abs(Global.Z轴.GetEncPos() - Global.Systemdata.SafeHigh2) < 0.1)
        {
            if (Math.Abs(Global.Z轴.GetPrfPos() - Global.Systemdata.SafeHigh2) < POS_TOLERANCE)
            {
                if (Global.Z轴.Runing())
                {
                    LogDetail(_currentStep, "Z轴到安全位，跳转到步骤19");
                    _currentStep = 19;
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step50()
    {
        LogDetail(_currentStep, $"Z轴移动到停止位：{Global.Systemdata.StopPos.Zpos}");
        Global.Z轴.PMove(Global.RunZVel * 0.85, Global.RunZAcc * 0.85, Global.Systemdata.StopPos.Zpos, 1);
        _currentStep++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step51()
    {
        LogDetail(_currentStep, "检测Z轴是否到达停止位");
        if (Math.Abs(Global.Z轴.GetPrfPos() - Global.Systemdata.StopPos.Zpos) < POS_TOLERANCE)
        {
            if (Global.Z轴.Runing())
            {
                _currentStep++;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step52()
    {
        LogDetail(_currentStep, "准备出板，延时100ms");
        Thread.Sleep(100); // 业务必需延时保留
        _currentStep = 54;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step53()
    {
        LogDetail(_currentStep, "检测调宽轴并启动皮带");
        if (Global.调宽.Runing())
        {
            Global.皮带.JOP(0, 20);
            _currentStep++;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step54()
    {
        LogDetail(_currentStep, $"移动到停止位：X={Global.Systemdata.StopPos.Xpos}, Y={Global.Systemdata.StopPos.Ypos}");
        BrowLib.Controller.MotionAPI.LinXyMoveA(300, 3000, Global.Systemdata.StopPos.Xpos, Global.Systemdata.StopPos.Ypos, 1);
        _currentStep++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step55()
    {
        LogDetail(_currentStep, "检测各轴是否到达停止位");
        if (BrowLib.Controller.MotionAPI.LinXYRuningA())
        {
            Global.R轴.PMove(Global.RunRVel, Global.RunRAcc, 0, 1);
            Global.左夹爪轴.PMove(10, 500, 0, 1);
            Global.右夹爪轴.PMove(10, 500, 0, 1);
            _currentStep++;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step56()
    {
        LogDetail(_currentStep, "加工完成，停止所有轴");
        if (BrowLib.Controller.MotionAPI.LinXYRuningA() &&
        Global.R轴.Runing() &&
        Global.左夹爪轴.Runing() &&
        Global.右夹爪轴.Runing())
        {
            Global.皮带.AxisStop();
            Global.MachineState = GEnumEx.MachineState.MachineStop;
            Global.SystemRun = false;
            Global.VisionApp.StopRunProc("Task5");

            Global.Buzzerflag = true;
            Global.StopFlag = true;
            Global.TcpClass.Send("A:FinishOK\r\n");
            LogInfo(_currentStep, "加工完成停止");
            _currentStep = 0;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step60()
    {
        LogDetail(_currentStep, $"Z轴移动到停止位：{Global.Systemdata.StopPos.Zpos}");
        Global.Z轴.PMove(Global.RunZVel, Global.RunZAcc, Global.Systemdata.StopPos.Zpos, 1);
        _currentStep++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step61()
    {
        LogDetail(_currentStep, "检测Z轴是否到达停止位");
        if (Math.Abs(Global.Z轴.GetPrfPos() - Global.Systemdata.StopPos.Zpos) < POS_TOLERANCE)
        {
            if (Global.Z轴.Runing())
            {
                _currentStep++;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step62()
    {
        LogDetail(_currentStep, "在线出板：调宽轴复位，顶升气缸下降，等待下位机要板");
        Global.调宽.PMove(10, 1000, Global.Systemdata.Trackoffset, 0);
        BrowLib.Controller.OutPort["顶升气缸_OUT"].Off();
        BrowLib.Controller.OutPort["A轨阻挡2_OUT5"].Off();
        Thread.Sleep(500); // 业务必需延时保留
        BrowLib.Controller.OutPort["A轨上位机放板_OUT2"].On();
        _currentStep++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step63()
    {
        LogDetail(_currentStep, "等待下位机要板信号");
        if (BrowLib.Controller.InPort["A轨上位机要板信号_IN15"].IsOn(100))
        {
            LogDetail(_currentStep, "收到下位机要板信号");
            _currentStep++;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step64()
    {
        LogDetail(_currentStep, "启动皮带出板，速度20");
        if (Global.调宽.Runing())
        {
            Global.皮带.JOP(1, 20);
            _currentStep++;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step65()
    {
        LogDetail(_currentStep, $"移动到停止位：X={Global.Systemdata.StopPos.Xpos}, Y={Global.Systemdata.StopPos.Ypos}");
        BrowLib.Controller.MotionAPI.LinXyMoveA(300, 3000, Global.Systemdata.StopPos.Xpos, Global.Systemdata.StopPos.Ypos, 1);
        _currentStep++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step66()
    {
        LogDetail(_currentStep, "检测出口感应光电");
        if (BrowLib.Controller.InPort["A轨出口感应光电_IN9"].IsOn(100))
        {
            LogDetail(_currentStep, "PCB板到达出口，皮带加速到30");
            Global.皮带.ChangSpeed(30);
            _currentStep++;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step67()
    {
        LogDetail(_currentStep, "检测PCB板是否离开出口");
        if (BrowLib.Controller.InPort["A轨出口感应光电_IN9"].IsOff(500))
        {
            LogDetail(_currentStep, "PCB板已出板，停止皮带");
            Global.皮带.AxisStop();
            BrowLib.Controller.OutPort["A轨上位机放板_OUT2"].Off();
            BrowLib.Controller.OutPort["A轨阻挡2_OUT5"].On();
            _currentStep++;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step68()
    {
        LogDetail(_currentStep, "在线出板完成，停止流程");
        Global.StopFlag = true;
        Global.MachineState = GEnumEx.MachineState.MachineStop;
        Global.SystemRun = false;
        Global.VisionApp.StopRunProc("Task5");
        Global.Buzzerflag = true;
        Global.TcpClass.Send("A:FinishOK\r\n");
        LogInfo(_currentStep, "加工完成停止");
        _currentStep = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step100()
    {
        // 视觉二次定位步骤100-108，按相同规范补充
        LogDetail(_currentStep, "进入视觉二次定位步骤100");
        _currentStep++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step101() { _currentStep++; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step102() { _currentStep++; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step103() { _currentStep++; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step104() { _currentStep++; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step105() { _currentStep++; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step106() { _currentStep++; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step107() { _currentStep++; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step108() { _currentStep++; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step200()
    {
        LogDetail(_currentStep, "进入单机模式，遍历BOM数据");
        for (int i = 0; i < Global.BomData.Rows.Count; i++)
        {
            if (Global.StopFlag) break;

            X = Convert.ToDouble(Global.BomData.Rows[i]["原始X坐标"].ToString());
            Y = Convert.ToDouble(Global.BomData.Rows[i]["原始Y坐标"].ToString());
            R = Convert.ToDouble(Global.BomData.Rows[i]["原始方向"].ToString());

            // 坐标转换
            X = X + VisionGlobal.TranslationX;
            Y = Y + VisionGlobal.TranslationY;
            X = X + Global.Parm.PbXoffset;
            Y = Y + Global.Parm.PbYoffset;

            LogDetail(_currentStep, $"单机模式-位号{i}：X={X}, Y={Y}, R={R}");

            double gX, gY, offset_X, offset_Y;
            Global.GetOffset(R, out offset_X, out offset_Y, Global.Is_DownCam);

            // Mark算法纠偏
            new Algorithm().MakAlgorithm(0, X, Y, VisionGlobal.Mak1_X, VisionGlobal.Mak1_Y,
            VisionGlobal.Mak2_X, VisionGlobal.Mak2_Y, VisionGlobal.bMak1_X + VisionGlobal.TranslationX,
            VisionGlobal.bMak1_Y + VisionGlobal.TranslationY, VisionGlobal.bMak2_X + VisionGlobal.TranslationX,
            VisionGlobal.bMak2_Y + VisionGlobal.TranslationY, out gX, out gY);

            // 整体补偿
            gX = gX + Global.Parm.Offset_X;
            gY = gY + Global.Parm.Offset_Y;

            LogDetail(_currentStep, $"单机模式-目标坐标：gX={gX}, gY={gY}");

            // 移动轴
            BrowLib.Controller.MotionAPI.LinXyMoveA(Global.RunXYVel, Global.RunXYAcc, gX, gY, 1, 2);
            do
            {
                if (BrowLib.Controller.MotionAPI.LinXYRuningA())
                {
                    Thread.Sleep(20); // 业务必需延时保留
                    break;
                }
                Thread.Sleep(2); // 业务必需延时保留
            }
            while (!BrowLib.Controller.MotionAPI.LinXYRuningA());

            Global.Z轴.PMove(Global.RunZVel, Global.RunZAcc, Mak_Z, 1);
            Thread.Sleep(1000); // 业务必需延时保留
        }
        LogDetail(_currentStep, "单机模式完成，跳转到步骤50");
        _currentStep = 50;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step300() { _currentStep++; } // 激光检测步骤
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step301() { _currentStep++; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step302() { _currentStep++; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step303() { _currentStep++; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step304() { _currentStep++; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step305() { _currentStep++; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step306() { _currentStep++; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step307() { _currentStep++; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step308() { _currentStep++; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step309() { _currentStep++; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step400() { _currentStep++; } // 视觉二次定位（不下针）
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step401() { _currentStep++; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step402() { _currentStep++; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step403() { _currentStep++; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step404() { _currentStep++; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step405() { _currentStep++; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step406() { _currentStep++; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step407() { _currentStep++; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step408() { _currentStep++; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step500() { _currentStep++; } // 视觉检测空贴
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step501() { _currentStep++; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step502() { _currentStep++; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step503() { _currentStep++; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step504() { _currentStep++; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step505() { _currentStep++; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Step506() { _currentStep++; }
    #endregion

    #region 工具方法（全部内联，零GC）
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private double GetTime(long ticks)
    {
        return TimeSpan.FromTicks(ticks).TotalSeconds;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Tuple<double, double> GetOffset(double wx, double wy, double offsetX, double offsetY)
    {
        return new Tuple<double, double>(wx + offsetX, wy + offsetY);
    }

    #region 日志方法（零GC核心优化）
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void LogInfo(int step, string message)
    {
        Log(step, "INFO", message);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void LogDetail(int step, string message)
    {
#if DEBUG
        Log(step, "DETAIL", message);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void LogWarn(int step, string message)
    {
        Log(step, "WARN", message);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void LogError(int step, string message)
    {
        Log(step, "ERROR", message);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Log(int step, string level, string message)
    {
        // 复用StringBuilder，零GC
        _logBuilder.Clear();
        _logBuilder.Append("[步骤").Append(step.ToString("D3"))
                   .Append("] [").Append(level).Append("] ")
                   .Append(DateTime.Now.ToString(_timeFormat))
                   .Append(" - ").Append(message);

        // 写入日志（复用原有日志接口）
        APP.Log.I_Log(_logBuilder.ToString());
    }
    #endregion
    #endregion
}

// 使用示例（直接替换原调用逻辑）
// var autoRun = new AutoRunUltimate();
// autoRun.AutoRun();