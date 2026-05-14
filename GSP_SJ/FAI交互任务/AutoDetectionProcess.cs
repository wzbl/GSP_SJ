using BrowApp;
using BrowApp.Language;
using BrowLib;
using GSP;
using GSP.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// 自动检测流程管理器
/// </summary>
public class AutoDetectionProcess
{
    #region 常量定义

    private const double POSITION_TOLERANCE = 0.02;
    private const int LOOP_SLEEP_MS = 2;
    private const int CAMERA_DELAY_MS = 10;
    private const int LIFT_DELAY_MS = 300;
    private const int FEEDING_TIMEOUT_MS = 5000;
    private const int LIFT_TIMEOUT_MS = 1000;
    private const double SAFETY_HEIGHT_DIFF =0.1;
    #endregion

    #region 状态枚举

    /// <summary>
    /// 主流程步骤枚举
    /// </summary>
    public enum ProcessStep : int
    {
        /// <summary>
        /// 入口检测
        /// </summary>
        CheckEntrance = 0,

        /// <summary>
        /// 入口感应确认
        /// </summary>
        EntranceSensor = 1,

        /// <summary>
        /// 开始进板
        /// </summary>
        StartFeeding = 2,

        /// <summary>
        /// 等待进板完成
        /// </summary>
        WaitWorkPosition = 3,

        /// <summary>
        /// 调整轨道宽度
        /// </summary>
        AdjustWidth = 4,

        /// <summary>
        /// 等待轨道宽度调整完成
        /// </summary>
        WaitWidthAdjust = 5,

        /// <summary>
        /// 顶升气缸顶起
        /// </summary>
        CylinderUp = 6,

        /// <summary>
        /// 等待气缸到位
        /// </summary>
        WaitCylinderUp = 7,

        /// <summary>
        /// 设置拍照参数
        /// </summary>
        PrepareCamera = 8,

        /// <summary>
        /// Z轴去拍照位
        /// </summary>
        MoveZToCameraHeight = 9,

        /// <summary>
        /// R轴去0位
        /// </summary>
        MoveRToZero = 10,

        /// <summary>
        /// 等待R轴到位
        /// </summary>
        WaitRToZero = 11,

        /// <summary>
        /// Mark拍照位
        /// </summary>
        GoToMarkPosition = 12,

        /// <summary>
        /// 等待到位完成
        /// </summary>
        WaitMarkPosition = 13,

        /// <summary>
        /// Z轴去拍照位
        /// </summary>
        MoveZForMark = 14,

        /// <summary>
        /// Z轴去拍照位完成
        /// </summary>
        WaitZForMark = 15,

        /// <summary>
        /// 触发相机拍照
        /// </summary>
        TriggerMarkCamera = 16,

        /// <summary>
        /// 等待拍照结果
        /// </summary>
        ProcessMarkResult = 17,

        /// <summary>
        /// Mark定位完成设置
        /// </summary>
        SetupAfterMark = 18,

        /// <summary>
        /// 选择运行模式
        /// </summary>
        SelectRunMode = 19,

        /// <summary>
        /// FAI请求指令
        /// </summary>
        RequestCommand = 20,

        /// <summary>
        /// 等待返回指令
        /// </summary>
        ProcessCommand = 21,

        /// <summary>
        /// 获取执行坐标位置
        /// </summary>
        GetComponentPosition = 22,

        /// <summary>
        /// 去目标位置
        /// </summary>
        MoveToTarget = 23,

        /// <summary>
        /// 等待目标位置完成
        /// </summary>
        WaitMotionComplete = 24,

        /// <summary>
        /// 去测试高度
        /// </summary>
        MoveZToTestHeight = 25,

        /// <summary>
        /// 执行二段速度
        /// </summary>
        SlowDownZAxis = 26,

        /// <summary>
        /// 到达测试位
        /// </summary>
        ClampOrTest = 27,

        /// <summary>
        /// 等待测试完成
        /// </summary>
        WaitClampComplete = 28,

        /// <summary>
        /// 确认测试开始
        /// </summary>
        ConfirmTestStart = 29,

        /// <summary>
        /// 运行到安全高度
        /// </summary>
        ReturnZToSafeHeight = 30,

        /// <summary>
        /// 运行到安全高度完成
        /// </summary>
        WaitZSafeHeight = 31,

        /// <summary>
        /// 单机模式
        /// </summary>
        SingleMode = 200,

        /// <summary>
        /// 离线完成
        /// </summary>
        OfflineCompletion = 50,

        /// <summary>
        /// 在线完成
        /// </summary>
        OnlineCompletion = 60,

        /// <summary>
        /// 视觉二次定位
        /// </summary>
        VisualAlignment = 100,

        /// <summary>
        /// 激光检测
        /// </summary>
        LaserDetection = 300,

        /// <summary>
        /// 视觉定位（不下针）
        /// </summary>
        VisualAlignmentNoNeedle = 400,

        /// <summary>
        /// 视觉检测空贴
        /// </summary>
        VisualDetectionEmpty = 500,

        /// <summary>
        /// 错误恢复
        /// </summary>
        ErrorRecovery = 999
    }

    /// <summary>
    /// 运行模式
    /// </summary>
    public enum RunMode
    {
        SingleMachine = 0,
        Online = 1
    }

    /// <summary>
    /// 处理模式
    /// </summary>
    public enum ProcessMode
    {
        Normal = 0,
        VisualAlignment = 1,
        LaserDetection = 2,
        VisualNoNeedle = 3,
        VisualEmpty = 4
    }

    #endregion

    #region 属性

    /// <summary>
    /// 当前步骤
    /// </summary>
    public ProcessStep CurrentStep { get; private set; }

    /// <summary>
    /// 是否正在运行
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// 是否暂停
    /// </summary>
    public bool IsPaused { get; set; }

    /// <summary>
    /// 上下文数据
    /// </summary>
    private ProcessContext _context;

    /// <summary>
    /// 步骤计时器
    /// </summary>
    private Stopwatch _stepTimer;

    /// <summary>
    /// 循环计时器
    /// </summary>
    private Stopwatch _cycleTimer;

    /// <summary>
    /// 取消令牌
    /// </summary>
    private CancellationTokenSource _cancellationTokenSource;

    #endregion

    #region 构造函数

    public AutoDetectionProcess()
    {
        _context = new ProcessContext();
        _stepTimer = new Stopwatch();
        _cycleTimer = new Stopwatch();
        _cancellationTokenSource = new CancellationTokenSource();
        CurrentStep = ProcessStep.CheckEntrance;
    }

    #endregion

    #region 公共方法
    /// <summary>
    /// 将时钟滴答数转换为时间字符串（分钟:秒.毫秒）
    /// </summary>
    /// <param name="ticks">时钟滴答数</param>
    /// <returns>格式化的时间字符串</returns>
    private double GetTime(long ticks)
    {
        // ticks是Stopwatch的ElapsedTicks，需要转换为秒
        return (double)ticks / Stopwatch.Frequency;
    }
    /// <summary>
    /// 启动自动检测流程
    /// </summary>
    public void Start()
    {
        if (IsRunning) return;

        IsRunning = true;
        CurrentStep = ProcessStep.CheckEntrance;
        _context.Reset();
        Global.StopFlag = false;
        Global.SystemRun = true;

        Task.Run(() => RunProcessAsync(_cancellationTokenSource.Token));
        Global.TcpClass.Send("M:StartOK");
    }

    /// <summary>
    /// 停止自动检测流程
    /// </summary>
    public void Stop()
    {
        IsRunning = false;
        _cancellationTokenSource.Cancel();
        SafetyStopAllMotions();

        Global.MachineState = GEnumEx.MachineState.MachineStop;
        Global.SystemRun = false;
        Global.StopFlag = true;

        LogInfo("流程已停止");
    }

    /// <summary>
    /// 暂停/恢复流程
    /// </summary>
    public void TogglePause()
    {
        IsPaused = !IsPaused;
        LogInfo($"流程已{(IsPaused ? "暂停" : "恢复")}");
    }

    /// <summary>
    /// 重置流程
    /// </summary>
    public void Reset()
    {
        Stop();
        _cancellationTokenSource = new CancellationTokenSource();
        CurrentStep = ProcessStep.CheckEntrance;
        _context.Reset();
        LogInfo("流程已重置");
    }

    #endregion

    #region 主要流程

    /// <summary>
    /// 异步执行主流程
    /// </summary>
    private async Task RunProcessAsync(CancellationToken cancellationToken)
    {
        LogInfo("开始自动检测流程");

        while (IsRunning && !cancellationToken.IsCancellationRequested && !Global.StopFlag)
        {
            try
            {
                // 检查是否可以执行当前步骤
                if (CanExecuteStep(CurrentStep))
                {
                    // 执行当前步骤
                    ProcessStep nextStep = await ExecuteStepAsync(CurrentStep, cancellationToken);

                    // 更新步骤
                    if (nextStep != CurrentStep)
                    {
                        LogInfo($"步骤切换: {CurrentStep} -> {nextStep}");
                        CurrentStep = nextStep;
                    }
                }

                // 短暂休眠以减少CPU占用
                await Task.Delay(LOOP_SLEEP_MS, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // 正常取消
                LogInfo("流程被取消");
                break;
            }
            catch (Exception ex)
            {
                // 处理异常
                HandleProcessError(ex);
                CurrentStep = ProcessStep.ErrorRecovery;
            }
        }

        IsRunning = false;
        LogInfo("自动检测流程结束");
    }

    /// <summary>
    /// 检查是否可以执行当前步骤
    /// </summary>
    private bool CanExecuteStep(ProcessStep step)
    {
        // 某些关键步骤即使暂停也要执行
        if (!IsPaused || IsCriticalStep(step))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 判断是否为关键步骤（即使暂停也要执行）
    /// </summary>
    private bool IsCriticalStep(ProcessStep step)
    {
        return step == ProcessStep.SelectRunMode ||
               step == ProcessStep.ConfirmTestStart ||
               step == (ProcessStep)18 ||
               step == (ProcessStep)108;
    }

    /// <summary>
    /// 执行步骤
    /// </summary>
    private async Task<ProcessStep> ExecuteStepAsync(ProcessStep step, CancellationToken cancellationToken)
    {

        try
        {
            switch (step)
            {
                // 轨道入口检测
                case ProcessStep.CheckEntrance:
                    return await ExecuteCheckEntranceAsync();

                case ProcessStep.EntranceSensor:
                    return await ExecuteEntranceSensorAsync();

                case ProcessStep.StartFeeding:
                    return ExecuteStartFeeding();

                case ProcessStep.WaitWorkPosition:
                    return await ExecuteWaitWorkPositionAsync();

                case ProcessStep.AdjustWidth:
                    return ExecuteAdjustWidth();

                case ProcessStep.WaitWidthAdjust:
                    return await ExecuteWaitWidthAdjustAsync();

                case ProcessStep.CylinderUp:
                    return ExecuteCylinderUp();

                case ProcessStep.WaitCylinderUp:
                    return await ExecuteWaitCylinderUpAsync();

                case ProcessStep.PrepareCamera:
                    return ExecutePrepareCamera();

                case ProcessStep.MoveZToCameraHeight:
                    return ExecuteMoveZToCameraHeight();

                case ProcessStep.MoveRToZero:
                    return ExecuteMoveRToZero();

                case ProcessStep.WaitRToZero:
                    return await ExecuteWaitRToZeroAsync();

                case ProcessStep.GoToMarkPosition:
                    return ExecuteGoToMarkPosition();

                case ProcessStep.WaitMarkPosition:
                    return await ExecuteWaitMarkPositionAsync();

                case ProcessStep.MoveZForMark:
                    return ExecuteMoveZForMark();

                case ProcessStep.WaitZForMark:
                    return await ExecuteWaitZForMarkAsync();

                case ProcessStep.TriggerMarkCamera:
                    return ExecuteTriggerMarkCamera();

                case ProcessStep.ProcessMarkResult:
                    return await ExecuteProcessMarkResultAsync();

                case ProcessStep.SetupAfterMark:
                    return ExecuteSetupAfterMark();

                case ProcessStep.SelectRunMode:
                    return ExecuteSelectRunMode();

                case ProcessStep.RequestCommand:
                    return ExecuteRequestCommand();

                case ProcessStep.ProcessCommand:
                    return await ExecuteProcessCommandAsync();

                case ProcessStep.GetComponentPosition:
                    return ExecuteGetComponentPosition();

                case ProcessStep.MoveToTarget:
                    return ExecuteMoveToTarget();

                case ProcessStep.WaitMotionComplete:
                    return await ExecuteWaitMotionCompleteAsync();

                case ProcessStep.MoveZToTestHeight:
                    return ExecuteMoveZToTestHeight();

                case ProcessStep.SlowDownZAxis:
                    return await ExecuteSlowDownZAxisAsync();

                case ProcessStep.ClampOrTest:
                    return ExecuteClampOrTest();

                case ProcessStep.WaitClampComplete:
                    return await ExecuteWaitClampCompleteAsync();

                case ProcessStep.ConfirmTestStart:
                    return await ExecuteConfirmTestStartAsync();

                case ProcessStep.ReturnZToSafeHeight:
                    return ExecuteReturnZToSafeHeight();

                case ProcessStep.WaitZSafeHeight:
                    return await ExecuteWaitZSafeHeightAsync();

                // 子流程
                case ProcessStep.SingleMode:
                    return await ExecuteSingleModeAsync();

                case ProcessStep.OfflineCompletion:
                    return await ExecuteOfflineCompletionAsync();

                case ProcessStep.OnlineCompletion:
                    return await ExecuteOnlineCompletionAsync();

                case ProcessStep.VisualAlignment:
                    return await ExecuteVisualAlignmentAsync();

                case ProcessStep.LaserDetection:
                    return await ExecuteLaserDetectionAsync();

                case ProcessStep.VisualAlignmentNoNeedle:
                    return await ExecuteVisualAlignmentNoNeedleAsync();

                case ProcessStep.VisualDetectionEmpty:
                    return await ExecuteVisualDetectionEmptyAsync();

                case ProcessStep.ErrorRecovery:
                    return await ExecuteErrorRecoveryAsync();

                default:
                    // 默认情况下步进到下一步
                    return (ProcessStep)((int)step + 1);
            }
        }
        catch (Exception ex)
        {
            LogError($"执行步骤 {step} 时发生错误: {ex.Message}");
            return ProcessStep.ErrorRecovery;
        }
    }

    #endregion

    #region 步骤实现 - 入口检测

    /// <summary>
    /// 入口检测判断
    /// </summary>
    private async Task<ProcessStep> ExecuteCheckEntranceAsync()
    {
        switch (Global.Model)
        {
            case 0: // 离线机
                LogInfo("离线机模式，跳过入口检测");
                return ProcessStep.CylinderUp;

            case 1:
            case 2:
            case 3: // 在线机
                if (await CheckSensorAsync("阻挡感应光电_IN", 100))
                {
                    LogInfo("在线机模式，板已到位");
                    return ProcessStep.CylinderUp;
                }
                else
                {
                    ShowWarning("请放入PCB板");
                    return ProcessStep.CheckEntrance; // 保持当前步骤
                }

            default:
                LogError($"未知的机器模式: {Global.Model}");
                return ProcessStep.ErrorRecovery;
        }
    }

    /// <summary>
    /// 入口感应确认
    /// </summary>
    private async Task<ProcessStep> ExecuteEntranceSensorAsync()
    {
        if (await CheckSensorAsync("入口感应光电_IN", 500))
        {
            BrowLib.Controller.OutPort["上位机要板_OUT"].Off();
            LogInfo("入口感应到位");
            return ProcessStep.StartFeeding;
        }
        return ProcessStep.EntranceSensor;
    }

    /// <summary>
    /// 开始进板
    /// </summary>
    private ProcessStep ExecuteStartFeeding()
    {
        LogInfo("开始进板");
        Global.皮带.JOP(1, 50);
        _stepTimer.Restart();
        return ProcessStep.WaitWorkPosition;
    }

    /// <summary>
    /// 等待进板完成
    /// </summary>
    private async Task<ProcessStep> ExecuteWaitWorkPositionAsync()
    {
        if (await CheckSensorAsync("阻挡感应光电_IN", 50))
        {
            LogInfo("板到工作位");
            Global.皮带.ChangSpeed(5);
            Global.皮带.JOP(1, 5);
            await Task.Delay((int)Global.Systemdata.InDaytime);
            Global.皮带.AxisStop();
            return ProcessStep.AdjustWidth;
        }
        else if (_stepTimer.ElapsedMilliseconds > FEEDING_TIMEOUT_MS)
        {
            return await HandleTimeoutAsync("进板超时");
        }

        return ProcessStep.WaitWorkPosition;
    }

    /// <summary>
    /// 调整轨道宽度
    /// </summary>
    private ProcessStep ExecuteAdjustWidth()
    {
        double targetPos = -1 * Global.Systemdata.Trackoffset;
        Global.调宽.PMove(10, 1000, targetPos, 0);
        LogInfo($"调整轨道宽度到: {targetPos}");
        return ProcessStep.WaitWidthAdjust;
    }

    /// <summary>
    /// 等待轨道宽度调整完成
    /// </summary>
    private async Task<ProcessStep> ExecuteWaitWidthAdjustAsync()
    {
        if (!Global.调宽.Runing())
        {
            LogInfo("轨道宽度调整完成");
            return ProcessStep.CylinderUp;
        }
        return ProcessStep.WaitWidthAdjust;
    }

    /// <summary>
    /// 顶升气缸顶起
    /// </summary>
    private ProcessStep ExecuteCylinderUp()
    {
        LogInfo("顶升双气缸顶起");
        BrowLib.Controller.OutPort["顶升气缸_OUT"].On();
        _stepTimer.Restart();
        return ProcessStep.WaitCylinderUp;
    }

    /// <summary>
    /// 等待气缸到位
    /// </summary>
    private async Task<ProcessStep> ExecuteWaitCylinderUpAsync()
    {
        if (await CheckSensorAsync("顶升上位_IN", 100))
        {
            LogInfo("顶升双气缸顶起到位");
            await Task.Delay(LIFT_DELAY_MS);
            return ProcessStep.PrepareCamera;
        }
        else if(_stepTimer.ElapsedMilliseconds > LIFT_TIMEOUT_MS)
        {
            return await HandleTimeoutAsync("顶升到位超时");
        }
        return ProcessStep.WaitCylinderUp;
    }

    #endregion

    #region 步骤实现 - 相机准备

    /// <summary>
    /// 设置拍照参数
    /// </summary>
    private ProcessStep ExecutePrepareCamera()
    {
        Global.Light.SetRgbLight(
            Global.Systemdata.M_LED.LED_R,
            Global.Systemdata.M_LED.LED_G,
            Global.Systemdata.M_LED.LED_B
        );

        LogInfo($"设置Mark光源亮度 R:{Global.Systemdata.M_LED.LED_R}, G:{Global.Systemdata.M_LED.LED_G}, B:{Global.Systemdata.M_LED.LED_B}");

        Global.VisionApp.StopRunProc("Task5"); // 停止实时刷新

        return ProcessStep.MoveZToCameraHeight;
    }

    /// <summary>
    /// Z轴去拍照位
    /// </summary>
    private ProcessStep ExecuteMoveZToCameraHeight()
    {
        Global.Z轴.PMove(
            Global.RunZVel,
            Global.RunZAcc,
            Global.CamHeight,
            1
        );

        LogInfo($"Z轴去拍照高度: {Global.CamHeight}");
        return ProcessStep.MoveRToZero;
    }

    /// <summary>
    /// R轴去0位
    /// </summary>
    private ProcessStep ExecuteMoveRToZero()
    {
        Global.R轴.PMove(
            Global.RunZVel,
            Global.RunZAcc,
            0,
            1
        );

        LogInfo("R轴回零位");
        return ProcessStep.WaitRToZero;
    }

    /// <summary>
    /// 等待R轴到位
    /// </summary>
    private async Task<ProcessStep> ExecuteWaitRToZeroAsync()
    {
        if (await CheckAxisPositionAsync("R轴", 0, POSITION_TOLERANCE))
        {
            if (await CheckAxisPositionAsync("Z轴", Global.CamHeight, POSITION_TOLERANCE))
            {
                LogInfo("Z轴和R轴到位完成");
                return ProcessStep.GoToMarkPosition;
            }
        }
        return ProcessStep.WaitRToZero;
    }

    #endregion

    #region 步骤实现 - Mark定位

    /// <summary>
    /// Mark拍照位
    /// </summary>
    private ProcessStep ExecuteGoToMarkPosition()
    {
        if (!Global.Is_NoMark)
        {
            double markX, markY;

            if (_context.MarkNum == 0)
            {
                markX = Global.Parm.Mak1Pos.Xpos;
                markY = Global.Parm.Mak1Pos.Ypos;
                LogInfo("前往 Mark1 位置");
            }
            else
            {
                markX = Global.Parm.Mak2Pos.Xpos;
                markY = Global.Parm.Mak2Pos.Ypos;
                LogInfo("前往 Mark2 位置");
            }

            _context.MarkTargetX = markX;
            _context.MarkTargetY = markY;
            _context.MarkTargetZ = Global.CamHeight;

            BrowLib.Controller.MotionAPI.LinXyMoveA(
                Global.RunXYVel,
                Global.RunXYAcc,
                markX,
                markY,
                1,
                2
            );

            return ProcessStep.WaitMarkPosition;
        }
        else
        {
            LogInfo("跳过Mark定位流程");
            return ProcessStep.SetupAfterMark;
        }
    }

    /// <summary>
    /// 等待到位完成
    /// </summary>
    private async Task<ProcessStep> ExecuteWaitMarkPositionAsync()
    {
        if (await CheckLinearMotionCompleteAsync())
        {
            LogInfo($"到达 Mark{_context.MarkNum + 1} 位置");
            return ProcessStep.MoveZForMark;
        }

        return ProcessStep.WaitMarkPosition;
    }

    /// <summary>
    /// Z轴去拍照位
    /// </summary>
    private ProcessStep ExecuteMoveZForMark()
    {
        Global.Z轴.PMove(
            Global.RunZVel,
            Global.RunZAcc,
            _context.MarkTargetZ,
            1
        );

        LogInfo($"Z轴前往Mark拍照高度: {_context.MarkTargetZ}");
        return ProcessStep.WaitZForMark;
    }

    /// <summary>
    /// Z轴去拍照位完成
    /// </summary>
    private async Task<ProcessStep> ExecuteWaitZForMarkAsync()
    {
        if (await CheckAxisPositionAsync("Z轴", _context.MarkTargetZ, POSITION_TOLERANCE))
        {
            LogInfo($"Z轴到达Mark拍照高度");
            await Task.Delay(100);
            return ProcessStep.TriggerMarkCamera;
        }

        return ProcessStep.WaitZForMark;
    }

    /// <summary>
    /// 触发相机拍照
    /// </summary>
    private ProcessStep ExecuteTriggerMarkCamera()
    {
        if (_context.MarkNum == 0)
        {
            Global.VisionApp.SetToolData("Task3", "定义变量", 2102, 0, "false", 0);
            Global.VisionApp.SetToolData("Task3", "定义变量", 2110, 0, "1", 1);
            Global.VisionApp.ExecuteProc("Task3", 0);
            LogInfo("触发Mark1相机拍照");
        }
        else
        {
            Global.VisionApp.SetToolData("Task3", "定义变量", 2102, 0, "false", 0);
            Global.VisionApp.SetToolData("Task3", "定义变量", 2110, 0, "2", 1);
            Global.VisionApp.ExecuteProc("Task3", 1);
            LogInfo("触发Mark2相机拍照");
        }

        Thread.Sleep(CAMERA_DELAY_MS);
        return ProcessStep.ProcessMarkResult;
    }

    /// <summary>
    /// 处理Mark结果
    /// </summary>
    private async Task<ProcessStep> ExecuteProcessMarkResultAsync()
    {
        if (!Global.VisionApp.IsEndProc)
        {
            return ProcessStep.ProcessMarkResult;
        }

        if (_context.MarkNum == 0)
        {
            return await ProcessMarkResultAsync(0);
        }
        else
        {
            return await ProcessMarkResultAsync(1);
        }
    }

    /// <summary>
    /// 处理单个Mark结果
    /// </summary>
    private async Task<ProcessStep> ProcessMarkResultAsync(int markIndex)
    {
        if (Global.VisionApp.RunState("Task3", "执行结果"))
        {
            double dx = Global.VisionApp.GetDblValue("Task3", "定义变量", 2103, 0);
            double dy = Global.VisionApp.GetDblValue("Task3", "定义变量", 2104, 0);

            if (markIndex == 0)
            {
                _context.Mark1X = Global.Parm.Mak1Pos.Xpos - dx;
                _context.Mark1Y = Global.Parm.Mak1Pos.Ypos - dy;
                LogInfo($"Mark1 定位结果: Dx={dx:F3}, Dy={dy:F3}");
                _context.MarkNum++;
                return ProcessStep.GoToMarkPosition;
            }
            else
            {
                _context.Mark2X = Global.Parm.Mak2Pos.Xpos - dx;
                _context.Mark2Y = Global.Parm.Mak2Pos.Ypos - dy;
                LogInfo($"Mark2 定位结果: Dx={dx:F3}, Dy={dy:F3}");
                _context.MarkNum = 0;
                return ProcessStep.SetupAfterMark;
            }
        }
        else
        {
            return await HandleMarkFailureAsync($"Mark{markIndex + 1}定位失败");
        }
    }

    /// <summary>
    /// Mark定位完成设置
    /// </summary>
    private ProcessStep ExecuteSetupAfterMark()
    {
        Global.Light.SetRgbLight(
            Global.Systemdata.P_LED.LED_R,
            Global.Systemdata.P_LED.LED_G,
            Global.Systemdata.P_LED.LED_B
        );

        LogInfo($"设置测试光源亮度 R:{Global.Systemdata.P_LED.LED_R}, G:{Global.Systemdata.P_LED.LED_G}, B:{Global.Systemdata.P_LED.LED_B}");

        // 保存Mark补偿位置
        VisionGlobal.Mak1_X = Math.Round(_context.Mark1X, 4);
        VisionGlobal.Mak1_Y = Math.Round(_context.Mark1Y, 4);
        VisionGlobal.Mak2_X = Math.Round(_context.Mark2X, 4);
        VisionGlobal.Mak2_Y = Math.Round(_context.Mark2Y, 4);

        LogInfo($"Mark1补偿位置: X={VisionGlobal.Mak1_X:F4}, Y={VisionGlobal.Mak1_Y:F4}");
        LogInfo($"Mark2补偿位置: X={VisionGlobal.Mak2_X:F4}, Y={VisionGlobal.Mak2_Y:F4}");

        _cycleTimer.Restart();
        _cycleTimer.Start();

        return ProcessStep.SelectRunMode;
    }

    #endregion

    #region 步骤实现 - 运行模式选择

    /// <summary>
    /// 选择运行模式
    /// </summary>
    private ProcessStep ExecuteSelectRunMode()
    {
        switch (Global.RunMode)
        {
            case 0: // 单机模式
                LogInfo("进入单机模式");
                return ProcessStep.SingleMode;

            case 1: // 在线模式
                LogInfo("进入在线模式");
                return ProcessStep.RequestCommand;

            default:
                LogError($"未知的运行模式: {Global.RunMode}");
                return ProcessStep.ErrorRecovery;
        }
    }

    /// <summary>
    /// FAI请求指令
    /// </summary>
    private ProcessStep ExecuteRequestCommand()
    {
        Global.TcpClass.TCP_AStaart = false;
        Global.TcpClass.Send("A:GET\r\n", CurrentStep.ToString());
        LogInfo("发送FAI指令请求");
        return ProcessStep.ProcessCommand;
    }

    /// <summary>
    /// 处理指令
    /// </summary>
    private async Task<ProcessStep> ExecuteProcessCommandAsync()
    {
        if (Global.TcpClass.TCP_AStaart)
        {
            string result = Global.TcpClass.TCP_Result.Trim();
            LogInfo($"收到FAI指令: {result}");

            string[] parts = result.Split(':');

            if (parts[0] == "A")
            {
                if (parts[1] == "Finish")
                {
                    _cycleTimer.Stop();
                    double cycleTime = GetTime(_cycleTimer.ElapsedTicks);
                    LogInfo($"循环时间: {cycleTime}");

                    // 根据机器类型选择完成流程
                    if (Global.Model == 0) // 离线机
                    {
                        return ProcessStep.OfflineCompletion;
                    }
                    else // 在线机
                    {
                        if (Global.OrbModel == 0) // 在线模式
                        {
                            return ProcessStep.OnlineCompletion;
                        }
                        else // 离线模式
                        {
                            return ProcessStep.OfflineCompletion;
                        }
                    }
                }
                else
                {
                    _context.ComponentCode = parts[1];

                    // 处理线数切换
                    if (parts.Length > 2)
                    {
                        if (parts[2] == "2")
                        {
                            BrowLib.Controller.OutPort["四线切换两线"].On();
                        }
                        else if (parts[2] == "4")
                        {
                            BrowLib.Controller.OutPort["四线切换两线"].Off();
                        }
                    }

                    // 处理处理模式
                    if (parts.Length > 3)
                    {
                        switch (parts[3])
                        {
                            case "Rectify":
                                _context.ProcessMode = ProcessMode.VisualAlignment;
                                break;
                            case "Laser":
                                _context.ProcessMode = ProcessMode.LaserDetection;
                                break;
                            case "RectifyFirst":
                                _context.ProcessMode = ProcessMode.VisualNoNeedle;
                                break;
                            case "RectifyNext":
                                // 重新获取BOM数据
                                //Global.BomData = new JD.Fai.Data.FlyingProbe().GetDataTable(Global.FBCCode);
                                new Algorithm().CalculateTransferXY(Global.BomData, out double transX, out double transY);
                                VisionGlobal.TranslationX = transX;
                                VisionGlobal.TranslationY = transY;
                                _context.OffsetX = 0;
                                _context.OffsetY = 0;
                                _context.ProcessMode = ProcessMode.Normal;
                                break;
                            case "RectifyCheck":
                                _context.ProcessMode = ProcessMode.VisualEmpty;
                                break;
                            default:
                                _context.ProcessMode = ProcessMode.Normal;
                                break;
                        }
                    }
                    else
                    {
                        LogWarning("指令回复格式错误");
                        _context.ProcessMode = ProcessMode.Normal;
                    }

                    return ProcessStep.GetComponentPosition;
                }
            }
        }

        return ProcessStep.ProcessCommand;
    }

    #endregion

    #region 步骤实现 - 组件测试

    /// <summary>
    /// 获取组件位置
    /// </summary>
    private ProcessStep ExecuteGetComponentPosition()
    {
        DataRow[] dataRows = Global.BomData.Select($"位置号='{_context.ComponentCode}'");

        if (dataRows.Length == 0)
        {
            LogError($"未找到位号: {_context.ComponentCode}");
            return ProcessStep.ErrorRecovery;
        }

        DataRow row = dataRows[0];
        // 获取基本数据
        _context.OriginalX = Convert.ToDouble(row["原始X坐标"]);
        _context.OriginalY = Convert.ToDouble(row["原始Y坐标"]);
        _context.Rotation =new  Algorithm().GetAngle(Convert.ToDouble(row["原始方向"]));
        _context.OffsetX = Convert.ToDouble(row["X坐标调整"]);
        _context.OffsetY = Convert.ToDouble(row["Y坐标调整"]);
        _context.ComponentType = row["尺寸"].ToString();
        _context.ComponentSize = Global.GetSize(_context.ComponentType);
        _context.TestHeight = Global.GetHight(_context.ComponentType);
        _context.ClampSize = Global.JQSize(_context.ComponentType);

        // 坐标变换
        _context.TransformedX = _context.OriginalX + VisionGlobal.TranslationX + Global.Parm.PbXoffset;
        _context.TransformedY = _context.OriginalY + VisionGlobal.TranslationY + Global.Parm.PbYoffset;

        if (Global.Is_NoMark)
        {
            VisionGlobal.Mak1_X = Global.Parm.Mak1Pos.Xpos;//纠偏Mask1X位置
            VisionGlobal.Mak1_Y = Global.Parm.Mak1Pos.Ypos;//纠偏Mask1Y位置

            VisionGlobal.Mak2_X = Global.Parm.Mak2Pos.Xpos;//纠偏Mask2X位置
            VisionGlobal.Mak2_Y = Global.Parm.Mak2Pos.Ypos;//纠偏Mask2Y位置
        }

        // Mark计算
        int errorCode =new Algorithm().MakAlgorithm(
            0,
            _context.TransformedX,
            _context.TransformedY,
            VisionGlobal.Mak1_X,
            VisionGlobal.Mak1_Y,
            VisionGlobal.Mak2_X,
            VisionGlobal.Mak2_Y,
            VisionGlobal.bMak1_X + VisionGlobal.TranslationX,
            VisionGlobal.bMak1_Y + VisionGlobal.TranslationY,
            VisionGlobal.bMak2_X + VisionGlobal.TranslationX,
            VisionGlobal.bMak2_Y + VisionGlobal.TranslationY,
            out double targetX,
            out double targetY
        );

        if (errorCode != 0)
        {
            ShowError("Mark绑定数据出错");
            StopProcess("Mark绑定数据出错");
            return ProcessStep.ErrorRecovery;
        }

        // 整体补偿
        _context.TargetX = targetX + Global.Parm.Offset_X;
        _context.TargetY = targetY + Global.Parm.Offset_Y;

        LogInfo($"组件位置 - 原始: ({_context.OriginalX:F3}, {_context.OriginalY:F3}), 变换后: ({_context.TransformedX:F3}, {_context.TransformedY:F3}), 最终: ({_context.TargetX:F3}, {_context.TargetY:F3})");

        return ProcessStep.MoveToTarget;
        // 根据模式选择下一步
        //return _context.ProcessMode switch
        //{
        //    ProcessMode.Normal => ProcessStep.MoveToTarget,
        //    ProcessMode.VisualAlignment => ProcessStep.VisualAlignment,
        //    ProcessMode.LaserDetection => ProcessStep.LaserDetection,
        //    ProcessMode.VisualNoNeedle => ProcessStep.VisualAlignmentNoNeedle,
        //    ProcessMode.VisualEmpty => ProcessStep.VisualDetectionEmpty,
        //    _ => ProcessStep.MoveToTarget
        //};
    }

    /// <summary>
    /// 移动到目标位置
    /// </summary>
    private ProcessStep ExecuteMoveToTarget()
    {
        if (Global.IsCcdMode)
        {
            // 视觉模式
            Global.VisionApp.RunProc("Task5");
            LogInfo($"视觉模式 - 目标位置: X={_context.TargetX:F3}, Y={_context.TargetY:F3}");

            BrowLib.Controller.MotionAPI.LinXyMoveA(
                Global.RunXYVel,
                Global.RunXYAcc,
                _context.TargetX,
                _context.TargetY,
                1,
                2
            );
        }
        else
        {
            // 非视觉模式
            Global.VisionApp.StopRunProc("Task5");

            // 获取相机偏移
            Global.GetOffset(_context.Rotation, out double cameraOffsetX, out double cameraOffsetY, Global.Is_DownCam);
            LogInfo($"相机偏移 - X: {cameraOffsetX:F3}, Y: {cameraOffsetY:F3}");

            // 计算偏移位置
            double offsetX = _context.TargetX - cameraOffsetX;
            double offsetY = _context.TargetY - cameraOffsetY;

            // 坐标补偿
            var compensatedPos = Global.coordinateCompensator.GetCompensatedPosition(offsetX, offsetY);

            LogInfo($"补偿后位置 - X: {compensatedPos.Item1:F3}, Y: {compensatedPos.Item2:F3}");

            // 并行移动所有轴
            MoveMultipleAxes(
                compensatedPos.Item1,
                compensatedPos.Item2,
                _context.Rotation,
                _context.ComponentSize
            );
        }

        return ProcessStep.WaitMotionComplete;
    }

    /// <summary>
    /// 等待运动完成
    /// </summary>
    private async Task<ProcessStep> ExecuteWaitMotionCompleteAsync()
    {
        if (Global.IsCcdMode)
        {
            if (await CheckLinearMotionCompleteAsync())
            {
                LogInfo("XY轴运动完成（视觉模式）");
                _context.TestHeight = Global.CamHeight;
                return ProcessStep.MoveZToTestHeight;
            }
        }
        else
        {
            if (await CheckAllAxesReadyAsync())
            {
                LogInfo("所有轴运动完成");
                _context.TestHeight += Global.Hoffset;
                LogInfo($"下针高度补偿: {Global.Hoffset}");
                return ProcessStep.MoveZToTestHeight;
            }
        }

        return ProcessStep.WaitMotionComplete;
    }

    /// <summary>
    /// 移动到测试高度
    /// </summary>
    private ProcessStep ExecuteMoveZToTestHeight()
    {
        if (Global.IsCcdMode)
        {
            Global.Z轴.PMove(10, 1000, _context.TestHeight, 1);
        }
        else
        {
            Global.Z轴.PMove(Global.RunZVel, Global.RunZAcc, _context.TestHeight, 1);
        }

        LogInfo($"Z轴移动到测试高度: {_context.TestHeight}");

        if (Global.IsCcdMode)
        {
            return ProcessStep.ClampOrTest;
        }
        else
        {
            return ProcessStep.SlowDownZAxis;
        }
    }

    /// <summary>
    /// Z轴减速
    /// </summary>
    private async Task<ProcessStep> ExecuteSlowDownZAxisAsync()
    {
        double currentPos = Global.Z轴.GetPrfPos();

        if (_context.TestHeight - currentPos <= 2)
        {
            Global.Z轴.ChangSpeed(Global.Systemdata.buf_Zspeed);
            return ProcessStep.ClampOrTest;
        }

        return ProcessStep.SlowDownZAxis;
    }

    /// <summary>
    /// 夹紧或测试
    /// </summary>
    private ProcessStep ExecuteClampOrTest()
    {
        if (IsZAxisAtPosition(_context.TestHeight))
        {
            LogInfo($"夹爪动作尺寸: {_context.ClampSize}");

            if (_context.ClampSize > 0 && _context.ClampSize < 1)
            {
                LogInfo($"执行夹紧动作，尺寸: {_context.ClampSize}");
                ClampJaws(_context.ClampSize);
                return ProcessStep.WaitClampComplete;
            }
            else
            {
                Thread.Sleep(50);
                CompleteTestAction();
                return ProcessStep.ConfirmTestStart;
            }
        }

        return ProcessStep.ClampOrTest;
    }

    /// <summary>
    /// 等待夹紧完成
    /// </summary>
    private async Task<ProcessStep> ExecuteWaitClampCompleteAsync()
    {
        if (await CheckAxisRunningAsync("左夹爪轴") && await CheckAxisRunningAsync("右夹爪轴"))
        {
            Thread.Sleep(50);
            CompleteTestAction();
            return ProcessStep.ConfirmTestStart;
        }

        return ProcessStep.WaitClampComplete;
    }

    /// <summary>
    /// 确认测试开始
    /// </summary>
    private async Task<ProcessStep> ExecuteConfirmTestStartAsync()
    {
        if (Global.TcpClass.TCP_AStaart)
        {
            string result = Global.TcpClass.TCP_Result.Trim();
            string[] parts = result.Split(':');

            if (parts[0] == "A" && parts[1] == "TestOk")
            {
                if (!Global.IsCcdMode)
                {
                    if (_context.ClampSize > 0 && _context.ClampSize < 1)
                    {
                        // 张开夹爪
                        Global.左夹爪轴.PMove(10, 200, -_context.ClampSize, 0);
                        Global.右夹爪轴.PMove(10, 200, -_context.ClampSize, 0);
                        LogInfo($"执行张开动作，尺寸: {_context.ClampSize}");
                    }

                    return ProcessStep.ReturnZToSafeHeight;
                }
                else
                {
                    return ProcessStep.RequestCommand;
                }
            }
        }

        return ProcessStep.ConfirmTestStart;
    }

    /// <summary>
    /// 返回到安全高度
    /// </summary>
    private ProcessStep ExecuteReturnZToSafeHeight()
    {
        double safeHeight = _context.ClampSize > 0 && _context.ClampSize < 1 ?
            Global.Systemdata.SafeHigh2 : Global.Systemdata.SafeHigh2;

        Global.Z轴.PMove(
            Global.RunZVel * 0.85,
            Global.RunZAcc * 0.85,
            safeHeight,
            1
        );

        LogInfo($"Z轴返回到安全高度: {safeHeight}");
        return ProcessStep.WaitZSafeHeight;
    }

    /// <summary>
    /// 等待到达安全高度
    /// </summary>
    private async Task<ProcessStep> ExecuteWaitZSafeHeightAsync()
    {
        double safeHeight = _context.ClampSize > 0 && _context.ClampSize < 1 ?
            Global.Systemdata.SafeHigh2 : Global.Systemdata.SafeHigh2;

        if (await CheckAxisPositionAsync("Z轴", safeHeight, POSITION_TOLERANCE))
        {
            LogInfo("Z轴到达安全高度");

            // 检查夹爪是否完成张开动作
            if (_context.ClampSize > 0 && _context.ClampSize < 1)
            {
                if (await CheckAxisRunningAsync("左夹爪轴") && await CheckAxisRunningAsync("右夹爪轴"))
                {
                    return ProcessStep.RequestCommand;
                }
            }
            else
            {
                return ProcessStep.RequestCommand;
            }
        }

        return ProcessStep.WaitZSafeHeight;
    }

    #endregion

    #region 步骤实现 - 子流程

    /// <summary>
    /// 单机模式
    /// </summary>
    private async Task<ProcessStep> ExecuteSingleModeAsync()
    {
        for (int i = 0; i < Global.BomData.Rows.Count; i++)
        {
            if (Global.StopFlag) break;

            DataRow row = Global.BomData.Rows[i];

            double x = Convert.ToDouble(row["原始X坐标"]);
            double y = Convert.ToDouble(row["原始Y坐标"]);
            double r =new Algorithm().GetAngle(Convert.ToDouble(row["原始方向"]));

            // 坐标变换
            x = x + VisionGlobal.TranslationX + Global.Parm.PbXoffset;
            y = y + VisionGlobal.TranslationY + Global.Parm.PbYoffset;

            // 获取相机偏移
            Global.GetOffset(r, out double offsetX, out double offsetY, Global.Is_DownCam);

            // Mark计算
           new Algorithm().MakAlgorithm(
                0, x, y,
                VisionGlobal.Mak1_X, VisionGlobal.Mak1_Y,
                VisionGlobal.Mak2_X, VisionGlobal.Mak2_Y,
                VisionGlobal.bMak1_X + VisionGlobal.TranslationX,
                VisionGlobal.bMak1_Y + VisionGlobal.TranslationY,
                VisionGlobal.bMak2_X + VisionGlobal.TranslationX,
                VisionGlobal.bMak2_Y + VisionGlobal.TranslationY,
                out double gX, out double gY
            );

            // 整体补偿
            gX = gX + Global.Parm.Offset_X;
            gY = gY + Global.Parm.Offset_Y;

            // 移动到位置
            BrowLib.Controller.MotionAPI.LinXyMoveA(
                Global.RunXYVel,
                Global.RunXYAcc,
                gX,
                gY,
                1,
                2
            );

            // 等待移动完成
            await WaitForLinearMotionCompleteAsync();

            // Z轴移动
            Global.Z轴.PMove(
                Global.RunZVel,
                Global.RunZAcc,
                Global.CamHeight,
                1
            );

            await Task.Delay(1000);
        }

        return ProcessStep.OfflineCompletion;
    }

    /// <summary>
    /// 离线完成流程
    /// </summary>
    private async Task<ProcessStep> ExecuteOfflineCompletionAsync()
    {
        // Z轴回到停止位置
        Global.Z轴.PMove(
            Global.RunZVel * 0.85,
            Global.RunZAcc * 0.85,
            Global.Systemdata.StopPos.Zpos,
            1
        );

        if (await CheckAxisPositionAsync("Z轴", Global.Systemdata.StopPos.Zpos, POSITION_TOLERANCE))
        {
            // 皮带运动
            Global.皮带.JOP(0, 20);

            // XY回到停止位置
            BrowLib.Controller.MotionAPI.LinXyMoveA(
                300,
                3000,
                Global.Systemdata.StopPos.Xpos,
                Global.Systemdata.StopPos.Ypos,
                1
            );

            // 其他轴回零
            Global.R轴.PMove(Global.RunRVel, Global.RunRAcc, 0, 1);
            Global.左夹爪轴.PMove(10, 500, 0, 1);
            Global.右夹爪轴.PMove(10, 500, 0, 1);

            if (await CheckAllAxesAtHomeAsync())
            {
                Global.皮带.AxisStop();
                CompleteProcess("离线加工完成");
                return ProcessStep.CheckEntrance;
            }
        }

        return ProcessStep.OfflineCompletion;
    }

    /// <summary>
    /// 在线完成流程
    /// </summary>
    private async Task<ProcessStep> ExecuteOnlineCompletionAsync()
    {
        // Z轴回到停止位置
        Global.Z轴.PMove(
            Global.RunZVel,
            Global.RunZAcc,
            Global.Systemdata.StopPos.Zpos,
            1
        );

        if (await CheckAxisPositionAsync("Z轴", Global.Systemdata.StopPos.Zpos, POSITION_TOLERANCE))
        {
            // 调整宽度并放下顶升气缸
            Global.调宽.PMove(10, 1000, Global.Systemdata.Trackoffset, 0);
            BrowLib.Controller.OutPort["顶升气缸_OUT"].Off();
            BrowLib.Controller.OutPort["A轨阻挡2_OUT5"].Off();

            await Task.Delay(500);

            // 等待下位机要板信号
            BrowLib.Controller.OutPort["A轨上位机放板_OUT2"].On();
            LogInfo("等待下位机要板信号");

            if (await CheckSensorAsync("A轨上位机要板信号_IN15", 100))
            {
                LogInfo("收到下位机要板信号");

                if (!Global.调宽.Runing())
                {
                    // 开始出板
                    Global.皮带.JOP(1, 20);

                    // XY回到停止位置
                    BrowLib.Controller.MotionAPI.LinXyMoveA(
                        300,
                        3000,
                        Global.Systemdata.StopPos.Xpos,
                        Global.Systemdata.StopPos.Ypos,
                        1
                    );

                    // 检测出板完成
                    if (await WaitBoardExitAsync())
                    {
                        CompleteProcess("在线加工完成");
                        return ProcessStep.CheckEntrance;
                    }
                }
            }
        }

        return ProcessStep.OnlineCompletion;
    }

    /// <summary>
    /// 视觉二次定位
    /// </summary>
    private async Task<ProcessStep> ExecuteVisualAlignmentAsync()
    {
        switch ((int)CurrentStep - 100)
        {
            case 0: // 100
                Global.Z轴.PMove(
                    Global.RunZVel * 0.85,
                    Global.RunZAcc * 0.85,
                    Global.CamHeight,
                    1
                );
                return ProcessStep.VisualAlignment + 1;

            case 1: // 101
                if (await CheckAxisPositionAsync("Z轴", Global.CamHeight, POSITION_TOLERANCE))
                {
                    // 直接到103步骤
                    return ProcessStep.VisualAlignment + 3;
                }
                return CurrentStep;

            case 3: // 103
                BrowLib.Controller.MotionAPI.LinXyMoveA(
                    Global.RunXYVel,
                    Global.RunXYAcc,
                    _context.TargetX,
                    _context.TargetY,
                    1,
                    2
                );
                return ProcessStep.VisualAlignment + 4;

            case 4: // 104
                if (await CheckLinearMotionCompleteAsync())
                {
                    await Task.Delay(100);
                    return ProcessStep.VisualAlignment + 5;
                }
                return CurrentStep;

            case 5: // 105
                Global.Light.SetRgbLight(
                    Global.Systemdata.S_LED.LED_R,
                    Global.Systemdata.S_LED.LED_G,
                    Global.Systemdata.S_LED.LED_B
                );

                double L = Global.GetL(_context.ComponentType);
                double W = Global.GetW(_context.ComponentType);

                Global.VisionApp.SetToolData("Task4", "定义变量", 2112, 0, "false", 0);
                Global.VisionApp.SetToolData("Task4", "定义变量", 2101, 0, L.ToString(), 2);
                Global.VisionApp.SetToolData("Task4", "定义变量", 2102, 0, W.ToString(), 2);
                Global.VisionApp.SetToolData("Task4", "定义变量", 2110, 0, _context.Rotation.ToString(), 2);
                Global.VisionApp.SetToolData("Task4", "定义变量", 2111, 0, _context.ComponentType, 3);
                Global.VisionApp.ExecuteProc("Task4", 0);

                return ProcessStep.VisualAlignment + 6;

            case 6: // 106
                if (Global.VisionApp.EndProc["Task4"])
                {
                    if (Global.VisionApp.RunState("Task4", "执行结果"))
                    {
                        _context.OffsetX = Global.VisionApp.GetDblValue("Task4", "定义变量", 2108, 0);
                        _context.OffsetY = Global.VisionApp.GetDblValue("Task4", "定义变量", 2109, 0);
                        LogInfo($"视觉定位结果: Dx={_context.OffsetX:F3}, Dy={_context.OffsetY:F3}");
                        return ProcessStep.VisualAlignment + 7;
                    }
                    else
                    {
                        SendTestResult("A:GoToOk", $"{_context.OffsetX}:{_context.OffsetY}");
                        return ProcessStep.VisualAlignment + 8;
                    }
                }
                return CurrentStep;

            case 7: // 107
                if (_context.OffsetX > Global.MaxDx || _context.OffsetY > Global.MaxDy)
                {
                    SendTestResult("A:GoToOk", "");
                    return ProcessStep.VisualAlignment + 8;
                }
                else
                {
                    _context.TargetX = _context.TargetX - _context.OffsetX;
                    _context.TargetY = _context.TargetY - _context.OffsetY;
                    return ProcessStep.MoveToTarget;
                }

            case 8: // 108
                if (Global.TcpClass.TCP_AStaart)
                {
                    string result = Global.TcpClass.TCP_Result.Trim();
                    string[] parts = result.Split(':');
                    if (parts[0] == "A" && parts[1] == "TestOk")
                    {
                        return ProcessStep.RequestCommand;
                    }
                }
                return CurrentStep;

            default:
                return ProcessStep.ErrorRecovery;
        }
    }

    /// <summary>
    /// 激光检测
    /// </summary>
    private async Task<ProcessStep> ExecuteLaserDetectionAsync()
    {
        int subStep = (int)CurrentStep - 300;

        switch (subStep)
        {
            case 0: // 300
                Global.Z轴.PMove(
                    Global.RunZVel * 0.85,
                    Global.RunZAcc * 0.85,
                    Global.Systemdata.SafeHigh,
                    1
                );
                return ProcessStep.LaserDetection + 1;

            case 1: // 301
                if (await CheckAxisPositionAsync("Z轴", Global.Systemdata.SafeHigh, POSITION_TOLERANCE))
                {
                    // 直接到303步骤
                    return ProcessStep.LaserDetection + 3;
                }
                return CurrentStep;

            case 3: // 303
                _context.TargetX = _context.TargetX - Global.CalibData.Laser_X;
                _context.TargetY = _context.TargetY - Global.CalibData.Laser_Y;

                BrowLib.Controller.MotionAPI.LinXyMoveA(
                    Global.RunXYVel,
                    Global.RunXYAcc,
                    _context.TargetX,
                    _context.TargetY,
                    1,
                    2
                );
                return ProcessStep.LaserDetection + 4;

            case 4: // 304
                if (await CheckLinearMotionCompleteAsync())
                {
                    await Task.Delay(200);
                    return ProcessStep.LaserDetection + 7;
                }
                return CurrentStep;

            case 7: // 307
                Global.Laser.Serial.ComStart = false;
                Global.Laser.Send(Global.LaserType);
                return ProcessStep.LaserDetection + 8;

            case 8: // 308
                if (Global.Laser.Serial.ComStart)
                {
                    double lvalue = Global.Laser.LaserValue(Global.LaserType);
                    double varValue = lvalue - Global.Hoffset;

                    LogInfo($"激光检测值: {lvalue:F3}, 补偿值: {Global.Hoffset:F3}");
                    LogInfo($"实际偏差值: {Math.Abs(varValue - Global.CalibData.LaserValue):F3}");
                    LogInfo($"允许偏差值: {Global.CalibData.Allowablevalue:F3}");

                    if (Math.Abs(varValue - Global.CalibData.LaserValue) <= Global.CalibData.Allowablevalue)
                    {
                        SendTestResult("A:LaserOK", "");
                    }
                    else
                    {
                        SendTestResult("A:LaserNG", "");
                    }

                    return ProcessStep.LaserDetection + 9;
                }
                return CurrentStep;

            case 9: // 309
                if (Global.TcpClass.TCP_AStaart)
                {
                    string result = Global.TcpClass.TCP_Result.Trim();
                    string[] parts = result.Split(':');
                    if (parts[0] == "A" && parts[1] == "TestOk")
                    {
                        return ProcessStep.RequestCommand;
                    }
                }
                return CurrentStep;

            default:
                return ProcessStep.ErrorRecovery;
        }
    }

    /// <summary>
    /// 视觉定位（不下针）
    /// </summary>
    private async Task<ProcessStep> ExecuteVisualAlignmentNoNeedleAsync()
    {
        int subStep = (int)CurrentStep - 400;

        switch (subStep)
        {
            case 0: // 400
                Global.Z轴.PMove(
                    Global.RunZVel * 0.85,
                    Global.RunZAcc * 0.85,
                    Global.CamHeight,
                    1
                );
                return ProcessStep.VisualAlignmentNoNeedle + 1;

            case 1: // 401
                if (await CheckAxisPositionAsync("Z轴", Global.CamHeight, POSITION_TOLERANCE))
                {
                    // 直接到403步骤
                    return ProcessStep.VisualAlignmentNoNeedle + 3;
                }
                return CurrentStep;

            case 3: // 403
                BrowLib.Controller.MotionAPI.LinXyMoveA(
                    Global.RunXYVel,
                    Global.RunXYAcc,
                    _context.TargetX,
                    _context.TargetY,
                    1,
                    2
                );
                return ProcessStep.VisualAlignmentNoNeedle + 4;

            case 4: // 404
                if (await CheckLinearMotionCompleteAsync())
                {
                    await Task.Delay(100);
                    return ProcessStep.VisualAlignmentNoNeedle + 5;
                }
                return CurrentStep;

            case 5: // 405
                Global.Light.SetRgbLight(
                    Global.Systemdata.S_LED.LED_R,
                    Global.Systemdata.S_LED.LED_G,
                    Global.Systemdata.S_LED.LED_B
                );

                double dL = Global.GetL(_context.ComponentType);
                double dW = Global.GetW(_context.ComponentType);

                Global.VisionApp.SetToolData("Task4", "定义变量", 2112, 0, "false", 0);
                Global.VisionApp.SetToolData("Task4", "定义变量", 2101, 0, dL.ToString(), 2);
                Global.VisionApp.SetToolData("Task4", "定义变量", 2102, 0, dW.ToString(), 2);
                Global.VisionApp.SetToolData("Task4", "定义变量", 2110, 0, _context.Rotation.ToString(), 2);
                Global.VisionApp.SetToolData("Task4", "定义变量", 2111, 0, _context.ComponentType, 3);
                Global.VisionApp.ExecuteProc("Task4", 0);

                return ProcessStep.VisualAlignmentNoNeedle + 6;

            case 6: // 406
                if (Global.VisionApp.EndProc["Task4"])
                {
                    if (Global.VisionApp.RunState("Task4", "执行结果"))
                    {
                        _context.OffsetX = Global.VisionApp.GetDblValue("Task4", "定义变量", 2108, 0);
                        _context.OffsetY = Global.VisionApp.GetDblValue("Task4", "定义变量", 2109, 0);
                        LogInfo($"视觉定位结果: Dx={_context.OffsetX:F3}, Dy={_context.OffsetY:F3}");
                        return ProcessStep.VisualAlignmentNoNeedle + 7;
                    }
                    else
                    {
                        SendTestResult("A:GoToOk", "0:0");
                        return ProcessStep.VisualAlignmentNoNeedle + 8;
                    }
                }
                return CurrentStep;

            case 7: // 407
                if (_context.OffsetX > Global.MaxDx || _context.OffsetY > Global.MaxDy)
                {
                    SendTestResult("A:GoToOk", "0:0");
                }
                else
                {
                    SendTestResult("A:GoToOk", $"{_context.OffsetX}:{_context.OffsetY}");
                }
                return ProcessStep.VisualAlignmentNoNeedle + 8;

            case 8: // 408
                if (Global.TcpClass.TCP_AStaart)
                {
                    string result = Global.TcpClass.TCP_Result.Trim();
                    string[] parts = result.Split(':');
                    if (parts[0] == "A" && parts[1] == "TestOk")
                    {
                        return ProcessStep.RequestCommand;
                    }
                }
                return CurrentStep;

            default:
                return ProcessStep.ErrorRecovery;
        }
    }

    /// <summary>
    /// 视觉检测空贴
    /// </summary>
    private async Task<ProcessStep> ExecuteVisualDetectionEmptyAsync()
    {
        int subStep = (int)CurrentStep - 500;

        switch (subStep)
        {
            case 0: // 500
                Global.Z轴.PMove(
                    Global.RunZVel * 0.85,
                    Global.RunZAcc * 0.85,
                    Global.CamHeight,
                    1
                );
                return ProcessStep.VisualDetectionEmpty + 1;

            case 1: // 501
                if (await CheckAxisPositionAsync("Z轴", Global.CamHeight, POSITION_TOLERANCE))
                {
                    return ProcessStep.VisualDetectionEmpty + 2;
                }
                return CurrentStep;

            case 2: // 502
                BrowLib.Controller.MotionAPI.LinXyMoveA(
                    Global.RunXYVel,
                    Global.RunXYAcc,
                    _context.TargetX,
                    _context.TargetY,
                    1,
                    2
                );
                return ProcessStep.VisualDetectionEmpty + 3;

            case 3: // 503
                if (await CheckLinearMotionCompleteAsync())
                {
                    await Task.Delay(100);
                    return ProcessStep.VisualDetectionEmpty + 4;
                }
                return CurrentStep;

            case 4: // 504
                Global.Light.SetRgbLight(
                    Global.Systemdata.S_LED.LED_R,
                    Global.Systemdata.S_LED.LED_G,
                    Global.Systemdata.S_LED.LED_B
                );

                double L2 = Global.GetL(_context.ComponentType);
                double W2 = Global.GetW(_context.ComponentType);

                Global.VisionApp.SetToolData("Task6", "定义变量", 2112, 0, "false", 0);
                Global.VisionApp.SetToolData("Task6", "定义变量", 2101, 0, L2.ToString(), 2);
                Global.VisionApp.SetToolData("Task6", "定义变量", 2102, 0, W2.ToString(), 2);
                Global.VisionApp.SetToolData("Task6", "定义变量", 2110, 0, _context.Rotation.ToString(), 2);
                Global.VisionApp.SetToolData("Task6", "定义变量", 2111, 0, _context.ComponentType, 3);
                Global.VisionApp.ExecuteProc("Task6", 0);

                return ProcessStep.VisualDetectionEmpty + 5;

            case 5: // 505
                if (Global.VisionApp.IsEndProc)
                {
                    if (Global.VisionApp.RunState("Task6", "执行结果"))
                    {
                        _context.OffsetX = Global.VisionApp.GetDblValue("Task6", "定义变量", 2108, 0);
                        _context.OffsetY = Global.VisionApp.GetDblValue("Task6", "定义变量", 2109, 0);
                        SendTestResult("A:GoToOk", "Pass");
                    }
                    else
                    {
                        SendTestResult("A:GoToOk", "Fail");
                    }
                    return ProcessStep.VisualDetectionEmpty + 6;
                }
                return CurrentStep;

            case 6: // 506
                if (Global.TcpClass.TCP_AStaart)
                {
                    string result = Global.TcpClass.TCP_Result.Trim();
                    string[] parts = result.Split(':');
                    if (parts[0] == "A" && parts[1] == "TestOk")
                    {
                        return ProcessStep.RequestCommand;
                    }
                }
                return CurrentStep;

            default:
                return ProcessStep.ErrorRecovery;
        }
    }

    /// <summary>
    /// 错误恢复
    /// </summary>
    private async Task<ProcessStep> ExecuteErrorRecoveryAsync()
    {
        LogInfo("执行错误恢复流程");

        // 安全停止所有运动
        SafetyStopAllMotions();

        // 尝试回到安全位置
        Global.Z轴.PMove(100, 1000, Global.Systemdata.SafeHigh, 1);
        BrowLib.Controller.MotionAPI.LinXyMoveA(100, 1000, Global.Systemdata.StopPos.Xpos, Global.Systemdata.StopPos.Ypos, 1);

        await Task.Delay(1000);

        // 询问用户是否继续
        int result = APP.Tip.ShowTip(1, "错误恢复".tr(), "检测到错误，是否重新开始？".tr(), "是".tr(), "否".tr());

        if (result == 1)
        {
            LogInfo("用户选择重新开始");
            return ProcessStep.CheckEntrance;
        }
        else
        {
            LogInfo("用户选择停止流程");
            Stop();
            return ProcessStep.ErrorRecovery;
        }
    }

    #endregion

    #region 工具方法

    /// <summary>
    /// 并行移动多个轴
    /// </summary>
    private void MoveMultipleAxes(double x, double y, double rotation, double size)
    {
        // XY轴移动
        BrowLib.Controller.MotionAPI.LinXyMoveA(
            Global.RunXYVel,
            Global.RunXYAcc,
            x,
            y,
            1,
            2
        );

        // R轴旋转
        Global.R轴.PMove(
            Global.RunRVel,
            Global.RunRAcc,
            rotation,
            1
        );

        // 夹爪张开
        Global.左夹爪轴.PMove(15, 500, size, 1);
        Global.右夹爪轴.PMove(15, 500, size, 1);

        LogInfo($"旋转角度: {rotation:F2}°, 夹爪张开尺寸: {size:F3}");
    }

    /// <summary>
    /// 检查所有轴是否就绪
    /// </summary>
    private async Task<bool> CheckAllAxesReadyAsync()
    {
        if (!await CheckLinearMotionCompleteAsync())
            return false;

        if (!await CheckAxisPositionAsync("R轴", _context.Rotation, 0.1))
            return false;

        if (!await CheckAxisPositionAsync("左夹爪轴", 0, 0.2))
            return false;

        if (!await CheckAxisPositionAsync("右夹爪轴", 0, 0.2))
            return false;

        return true;
    }

    /// <summary>
    /// 检查所有轴是否回到原点
    /// </summary>
    private async Task<bool> CheckAllAxesAtHomeAsync()
    {
        if (!await CheckLinearMotionCompleteAsync())
            return false;

        if (!await CheckAxisPositionAsync("R轴", 0, 0.05))
            return false;

        if (!await CheckAxisPositionAsync("左夹爪轴", 0, 0.05))
            return false;

        if (!await CheckAxisPositionAsync("右夹爪轴",0,0.05))
            return false;
        return true;
    }

    /// <summary>
    /// 安全停止所有运动
    /// </summary>
    private void SafetyStopAllMotions()
    {
        try
        {
            Global.皮带.AxisStop();
            Global.Z轴.AxisStop();
            Global.R轴.AxisStop();
            Global.左夹爪轴.AxisStop();
            Global.右夹爪轴.AxisStop();
            Global.调宽.AxisStop();

            // 关闭输出
            BrowLib.Controller.OutPort["顶升气缸_OUT"].Off();
            BrowLib.Controller.OutPort["上位机要板_OUT"].Off();
        }
        catch (Exception ex)
        {
            LogError($"安全停止时出错: {ex.Message}");
        }
    }

    /// <summary>
    /// 处理超时
    /// </summary>
    private async Task<ProcessStep> HandleTimeoutAsync(string message)
    {
        ShowWarning(message);
        LogError($"{message} - 超时");

        // 询问用户操作
        int result = APP.Tip.ShowTip(1, "警告".tr(), message.tr(), "继续".tr(), "停止".tr());

        if (result == 1) // 继续
        {
            LogInfo($"{message} - 用户选择继续");
            return (ProcessStep)((int)CurrentStep + 1);
        }
        else // 停止
        {
            LogInfo($"{message} - 用户选择停止");
            StopProcess(message);
            return ProcessStep.ErrorRecovery;
        }
    }

    /// <summary>
    /// 处理Mark定位失败
    /// </summary>
    private async Task<ProcessStep> HandleMarkFailureAsync(string error)
    {
        MarkRectify dialog = new MarkRectify();
        dialog.ShowDialog();

        if (dialog.Isok)
        {
            if (_context.MarkNum == 0)
            {
                _context.Mark1X = dialog.Dx;
                _context.Mark1Y = dialog.Dy;
                _context.MarkNum++;
                return ProcessStep.GoToMarkPosition;
            }
            else
            {
                _context.Mark2X = dialog.Dx;
                _context.Mark2Y = dialog.Dy;
                _context.MarkNum = 0;
                return ProcessStep.SetupAfterMark;
            }
        }
        else
        {
            ShowError(error);
            StopProcess($"{error} - 用户取消");
            return ProcessStep.ErrorRecovery;
        }
    }

    /// <summary>
    /// 完成测试动作
    /// </summary>
    private void CompleteTestAction()
    {
        Global.TextNum++;

        if (Global.TextNum >= Global.Systemdata.Servicelife)
        {
            ShowWarning("测试针使用寿命已达到最大值");
        }

        Global.TcpClass.TCP_AStaart = false;
        Global.WriteTextNum();

        Global.TcpClass.Send(
            $"A:GoToOk:{_context.OffsetX}:{_context.OffsetY}\r\n",
            "CompleteTestAction"
        );

        LogInfo($"发送测试完成指令，偏移: X={_context.OffsetX:F3}, Y={_context.OffsetY:F3}");
    }

    /// <summary>
    /// 发送测试结果
    /// </summary>
    private void SendTestResult(string command, string data)
    {
        Global.TcpClass.TCP_AStaart = false;
        Global.TcpClass.Send($"{command}:{data}\r\n", CurrentStep.ToString());
        LogInfo($"发送测试结果: {command}:{data}");
    }

    /// <summary>
    /// 完成整个流程
    /// </summary>
    private void CompleteProcess(string message)
    {
        Global.皮带.AxisStop();
        Global.MachineState = GEnumEx.MachineState.MachineStop;
        Global.SystemRun = false;
        Global.VisionApp.StopRunProc("Task5");
        Global.Buzzerflag = true;
        Global.StopFlag = true;

        Global.TcpClass.Send("A:FinishOK\r\n");
        LogInfo(message);
    }

    /// <summary>
    /// 停止流程
    /// </summary>
    private void StopProcess(string reason)
    {
        Global.StopFlag = true;
        Global.MachineState = GEnumEx.MachineState.MachineStop;
        Global.SystemRun = false;
        Global.TcpClass.Send("M:Stop");
        LogInfo($"流程停止: {reason}");
    }
    /// <summary>
    /// 处理流程错误
    /// </summary>
    private void HandleProcessError(Exception ex)
    {
        LogError($"流程执行错误 - 步骤 {CurrentStep}: {ex.Message}\n{ex.StackTrace}");
        ShowError($"步骤 {CurrentStep} 执行异常");
        // 紧急停止
        SafetyStopAllMotions();
    }

    #endregion

    #region 硬件交互封装

    /// <summary>
    /// 检查传感器状态
    /// </summary>
    private async Task<bool> CheckSensorAsync(string sensorName, int delayMs = 0)
    {
        return await Task.Run(() => BrowLib.Controller.InPort[sensorName].IsOn(delayMs));
    }

    /// <summary>
    /// 检查线性运动是否完成
    /// </summary>
    private async Task<bool> CheckLinearMotionCompleteAsync()
    {
        return await Task.Run(() => BrowLib.Controller.MotionAPI.LinXYRuningA());
    }

    /// <summary>
    /// 等待线性运动完成
    /// </summary>
    private async Task WaitForLinearMotionCompleteAsync()
    {
        while (!await CheckLinearMotionCompleteAsync())
        {
            await Task.Delay(10);
            if (Global.StopFlag) break;
        }
    }

    /// <summary>
    /// 检查轴是否到达目标位置
    /// </summary>
    private async Task<bool> CheckAxisPositionAsync(string axisName, double targetPos, double tolerance)
    {
        return await Task.Run(() =>
        {
            double currentPos = BrowLib.Controller.Motion[axisName].GetPrfPos();
            if (targetPos <= 0) return BrowLib.Controller.Motion[axisName].Runing();
            return Math.Abs(currentPos - targetPos) < tolerance && BrowLib.Controller.Motion[axisName].Runing();
        });
    }

    /// <summary>
    /// 检查轴是否在运行
    /// </summary>
    private async Task<bool> CheckAxisRunningAsync(string axisName)
    {
        return await Task.Run(() => BrowLib.Controller.Motion[axisName].Runing());
    }

    /// <summary>
    /// 检查Z轴是否在指定位置
    /// </summary>
    private bool IsZAxisAtPosition(double targetHeight)
    {
        double currentPos = Global.Z轴.GetPrfPos();
        return Math.Abs(currentPos - targetHeight) < POSITION_TOLERANCE;
    }

    /// <summary>
    /// 夹紧夹爪
    /// </summary>
    private void ClampJaws(double clampSize)
    {
        Global.左夹爪轴.PMove(10, 200, clampSize, 0);
        Global.右夹爪轴.PMove(10, 200, clampSize, 0);
    }

    /// <summary>
    /// 等待板子退出
    /// </summary>
    private async Task<bool> WaitBoardExitAsync()
    {
        // 等待出口感应
        if (!await CheckSensorAsync("A轨出口感应光电_IN9", 100))
            return false;

        // 加速皮带
        Global.皮带.ChangSpeed(30);

        // 等待板子完全离开
        await Task.Delay(500);

        if (await CheckSensorAsync("A轨出口感应光电_IN9", 500))
        {
            Global.皮带.AxisStop();
            BrowLib.Controller.OutPort["A轨上位机放板_OUT2"].Off();
            BrowLib.Controller.OutPort["A轨阻挡2_OUT5"].On();
            return true;
        }

        return false;
    }

    #endregion

    #region 日志和提示

    private void LogInfo(string message)
    {
        APP.Log.I_Log($"[AutoDetection] {message}");
    }

    private void LogError(string message, Exception error=null)
    {
        APP.Log.E_Log($"[AutoDetection] {message}", error);
    }

    private void LogWarning(string message)
    {
        APP.Log.W_Log($"[AutoDetection] {message}");
    }

    private void ShowWarning(string message)
    {
        APP.Tip.ShowTip(1, "警告".tr(), message.tr(), "确定".tr());
    }

    private void ShowError(string message)
    {
        APP.Tip.ShowTip(1, "错误", message.tr(), "确定".tr());
    }

    #endregion

    #region 上下文类

    /// <summary>
    /// 流程上下文数据
    /// </summary>
    private class ProcessContext
    {
        // Mark相关
        public int MarkNum { get; set; }
        public double Mark1X { get; set; }
        public double Mark1Y { get; set; }
        public double Mark2X { get; set; }
        public double Mark2Y { get; set; }
        public double MarkTargetX { get; set; }
        public double MarkTargetY { get; set; }
        public double MarkTargetZ { get; set; }

        // 组件相关
        public string ComponentCode { get; set; }
        public string ComponentType { get; set; }
        public double OriginalX { get; set; }
        public double OriginalY { get; set; }
        public double TransformedX { get; set; }
        public double TransformedY { get; set; }
        public double TargetX { get; set; }
        public double TargetY { get; set; }
        public double Rotation { get; set; }
        public double ComponentSize { get; set; }
        public double TestHeight { get; set; }
        public double ClampSize { get; set; }
        public double OffsetX { get; set; }
        public double OffsetY { get; set; }

        // 流程模式
        public ProcessMode ProcessMode { get; set; }

        /// <summary>
        /// 重置上下文
        /// </summary>
        public void Reset()
        {
            MarkNum = 0;
            Mark1X = Mark1Y = Mark2X = Mark2Y = 0;
            MarkTargetX = MarkTargetY = MarkTargetZ = 0;
            ComponentCode = string.Empty;
            ComponentType = string.Empty;
            OriginalX = OriginalY = TransformedX = TransformedY = 0;
            TargetX = TargetY = Rotation = ComponentSize = TestHeight = ClampSize = 0;
            OffsetX = OffsetY = 0;
            ProcessMode = ProcessMode.Normal;
        }

       
    }

    #endregion
}