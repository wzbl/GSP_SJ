using BrowApp;
using BrowApp.Language;
using BrowLib;
using GSP;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// 拼图流程管理器
/// </summary>
public class PatternFlowProcess
{
    #region 常量定义

    private const double POSITION_TOLERANCE = 0.01;
    private const int LOOP_SLEEP_MS = 2;
    private const int CAMERA_DELAY_MS = 20;
    private const int LIFT_DELAY_MS = 300;
    private const int FEEDING_TIMEOUT_MS = 5000;
    private const int LIFT_TIMEOUT_MS = 1000;

    #endregion

    #region 状态枚举

    /// <summary>
    /// 拼图流程步骤枚举
    /// </summary>
    public enum PatternStep : int
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
        /// Z轴移动到安全高度
        /// </summary>
        MoveZToSafeHeight = 8,

        /// <summary>
        /// 等待Z轴到位
        /// </summary>
        WaitZSafeHeight = 9,

        /// <summary>
        /// 移动到拼图起始位置
        /// </summary>
        MoveToPatternStart = 10,

        /// <summary>
        /// 等待移动到起始位置
        /// </summary>
        WaitPatternStart = 11,

        /// <summary>
        /// 设置光源并移动到拍照高度
        /// </summary>
        SetupCameraAndLight = 12,

        /// <summary>
        /// 等待到达拍照高度
        /// </summary>
        WaitCameraHeight = 13,

        /// <summary>
        /// 计算拼图参数
        /// </summary>
        CalculatePatternParameters = 14,

        /// <summary>
        /// 移动到拼图位置
        /// </summary>
        MoveToPatternPosition = 15,

        /// <summary>
        /// 等待移动完成并拍照
        /// </summary>
        WaitAndCapture = 16,

        /// <summary>
        /// 处理拍照结果
        /// </summary>
        ProcessCaptureResult = 17,

        /// <summary>
        /// 拼图完成检查
        /// </summary>
        PatternCompletionCheck = 18,

        /// <summary>
        /// 错误恢复
        /// </summary>
        ErrorRecovery = 99
    }

    #endregion

    #region 属性

    /// <summary>
    /// 当前步骤
    /// </summary>
    public PatternStep CurrentStep { get; private set; }

    /// <summary>
    /// 是否正在运行
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// 是否暂停
    /// </summary>
    public bool IsPaused { get; set; }

    /// <summary>
    /// 拼图模式
    /// </summary>
    public int PatternMode { get; private set; }

    /// <summary>
    /// 上下文数据
    /// </summary>
    private PatternContext _context;

    /// <summary>
    /// 步骤计时器
    /// </summary>
    private Stopwatch _stepTimer;

    /// <summary>
    /// 运行计时器
    /// </summary>
    private KTimer _runTimer;

    /// <summary>
    /// 取消令牌
    /// </summary>
    private CancellationTokenSource _cancellationTokenSource;

    #endregion

    #region 构造函数

    public PatternFlowProcess()
    {
        _context = new PatternContext();
        _stepTimer = new Stopwatch();
        _runTimer = new KTimer();
        _cancellationTokenSource = new CancellationTokenSource();
        CurrentStep = PatternStep.CheckEntrance;
    }

    #endregion

    #region 公共方法

    /// <summary>
    /// 启动拼图流程
    /// </summary>
    /// <param name="mode">拼图模式</param>
    public void Start(int mode = 1)
    {
        if (IsRunning) return;

        IsRunning = true;
        PatternMode = mode;
        CurrentStep = PatternStep.CheckEntrance;
        _context.Reset();
        Global.StopFlag = false;
        Global.SystemRun = true;

        Task.Run(() => RunProcessAsync(_cancellationTokenSource.Token));
    }

    /// <summary>
    /// 停止拼图流程
    /// </summary>
    public void Stop()
    {
        IsRunning = false;
        _cancellationTokenSource.Cancel();
        SafetyStopAllMotions();

        Global.MachineState = GEnumEx.MachineState.MachineStop;
        Global.SystemRun = false;
        Global.StopFlag = true;

        LogInfo("拼图流程已停止");
    }

    /// <summary>
    /// 暂停/恢复流程
    /// </summary>
    public void TogglePause()
    {
        IsPaused = !IsPaused;
        LogInfo($"拼图流程已{(IsPaused ? "暂停" : "恢复")}");
    }

    /// <summary>
    /// 重置流程
    /// </summary>
    public void Reset()
    {
        Stop();
        _cancellationTokenSource = new CancellationTokenSource();
        CurrentStep = PatternStep.CheckEntrance;
        _context.Reset();
        LogInfo("拼图流程已重置");
    }

    #endregion

    #region 主要流程

    /// <summary>
    /// 异步执行主流程
    /// </summary>
    private async Task RunProcessAsync(CancellationToken cancellationToken)
    {
        LogInfo($"开始拼图流程，模式: {PatternMode}");

        while (IsRunning && !cancellationToken.IsCancellationRequested && !Global.StopFlag)
        {
            try
            {
                // 检查是否可以执行当前步骤
                if (CanExecuteStep(CurrentStep))
                {
                    // 执行当前步骤
                    PatternStep nextStep = await ExecuteStepAsync(CurrentStep, cancellationToken);

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
                LogInfo("拼图流程被取消");
                break;
            }
            catch (Exception ex)
            {
                // 处理异常
                HandleProcessError(ex);
                CurrentStep = PatternStep.ErrorRecovery;
            }
        }

        IsRunning = false;
        LogInfo("拼图流程结束");
    }

    /// <summary>
    /// 检查是否可以执行当前步骤
    /// </summary>
    private bool CanExecuteStep(PatternStep step)
    {
        // 检查暂停标志
        if (!Global.PauseFlag || IsCriticalStep(step))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 判断是否为关键步骤（即使暂停也要执行）
    /// </summary>
    private bool IsCriticalStep(PatternStep step)
    {
        // 根据需要定义关键步骤
        return step == PatternStep.ErrorRecovery;
    }

    /// <summary>
    /// 执行步骤
    /// </summary>
    private async Task<PatternStep> ExecuteStepAsync(PatternStep step, CancellationToken cancellationToken)
    {
        _stepTimer.Restart();

        try
        {
            switch (step)
            {
                case PatternStep.CheckEntrance:
                    return await ExecuteCheckEntranceAsync();

                case PatternStep.EntranceSensor:
                    return await ExecuteEntranceSensorAsync();

                case PatternStep.StartFeeding:
                    return ExecuteStartFeeding();

                case PatternStep.WaitWorkPosition:
                    return await ExecuteWaitWorkPositionAsync();

                case PatternStep.AdjustWidth:
                    return ExecuteAdjustWidth();

                case PatternStep.WaitWidthAdjust:
                    return await ExecuteWaitWidthAdjustAsync();

                case PatternStep.CylinderUp:
                    return ExecuteCylinderUp();

                case PatternStep.WaitCylinderUp:
                    return await ExecuteWaitCylinderUpAsync();

                case PatternStep.MoveZToSafeHeight:
                    return ExecuteMoveZToSafeHeight();

                case PatternStep.WaitZSafeHeight:
                    return await ExecuteWaitZSafeHeightAsync();

                case PatternStep.MoveToPatternStart:
                    return ExecuteMoveToPatternStart();

                case PatternStep.WaitPatternStart:
                    return await ExecuteWaitPatternStartAsync();

                case PatternStep.SetupCameraAndLight:
                    return ExecuteSetupCameraAndLight();

                case PatternStep.WaitCameraHeight:
                    return await ExecuteWaitCameraHeightAsync();

                case PatternStep.CalculatePatternParameters:
                    return ExecuteCalculatePatternParameters();

                case PatternStep.MoveToPatternPosition:
                    return ExecuteMoveToPatternPosition();

                case PatternStep.WaitAndCapture:
                    return await ExecuteWaitAndCaptureAsync();

                case PatternStep.ProcessCaptureResult:
                    return await ExecuteProcessCaptureResultAsync();

                case PatternStep.PatternCompletionCheck:
                    return await ExecutePatternCompletionCheckAsync();

                case PatternStep.ErrorRecovery:
                    return await ExecuteErrorRecoveryAsync();

                default:
                    return (PatternStep)((int)step + 1);
            }
        }
        catch (Exception ex)
        {
            LogError($"执行步骤 {step} 时发生错误: {ex.Message}");
            return PatternStep.ErrorRecovery;
        }
    }

    #endregion

    #region 步骤实现

    /// <summary>
    /// 入口检测判断
    /// </summary>
    private async Task<PatternStep> ExecuteCheckEntranceAsync()
    {
        switch (Global.Model)
        {
            case 0: // 离线机
                LogInfo("离线机模式，跳过入口检测");
                return PatternStep.CylinderUp;

            case 1:
            case 2:
            case 3: // 在线机
                if (await CheckSensorAsync("阻挡感应光电_IN"))
                {
                    LogInfo("在线机模式，板已到位");
                    return PatternStep.CylinderUp;
                }
                else
                {
                    ShowWarning("请放入PCB板");
                    return PatternStep.CheckEntrance;
                }

            default:
                LogError($"未知的机器模式: {Global.Model}");
                return PatternStep.ErrorRecovery;
        }
    }

    /// <summary>
    /// 入口感应确认
    /// </summary>
    private async Task<PatternStep> ExecuteEntranceSensorAsync()
    {
        if (await CheckSensorAsync("入口感应光电_IN"))
        {
            BrowLib.Controller.OutPort["上位机要板_OUT"].Off();
            LogInfo("入口感应有板");
            return PatternStep.StartFeeding;
        }
        return PatternStep.EntranceSensor;
    }

    /// <summary>
    /// 开始进板
    /// </summary>
    private PatternStep ExecuteStartFeeding()
    {
        LogInfo("启动皮带进板");
        Global.皮带.JOP(1, 50);
        _runTimer.Restart();
        return PatternStep.WaitWorkPosition;
    }

    /// <summary>
    /// 等待进板完成
    /// </summary>
    private async Task<PatternStep> ExecuteWaitWorkPositionAsync()
    {
        if (await CheckSensorAsync("阻挡感应光电_IN"))
        {
            LogInfo("阻挡光电感应板到位");
            Global.皮带.ChangSpeed(5);
            Global.皮带.JOP(1, 5);

            await Task.Delay((int)Global.Systemdata.InDaytime);
            Global.皮带.AxisStop();
            return PatternStep.AdjustWidth;
        }
        else if (_runTimer.IsOn(FEEDING_TIMEOUT_MS))
        {
            return await HandleTimeoutAsync("进板超时");
        }

        return PatternStep.WaitWorkPosition;
    }

    /// <summary>
    /// 调整轨道宽度
    /// </summary>
    private PatternStep ExecuteAdjustWidth()
    {
        double targetPos = -1 * Global.Systemdata.Trackoffset;
        Global.调宽.PMove(30, 1000, targetPos, 0);
        LogInfo($"调整轨道宽度到: {targetPos}");
        return PatternStep.WaitWidthAdjust;
    }

    /// <summary>
    /// 等待轨道宽度调整完成
    /// </summary>
    private async Task<PatternStep> ExecuteWaitWidthAdjustAsync()
    {
        if (!Global.调宽.Runing())
        {
            LogInfo("轨道宽度调整完成");
            return PatternStep.CylinderUp;
        }
        return PatternStep.WaitWidthAdjust;
    }

    /// <summary>
    /// 顶升气缸顶起
    /// </summary>
    private PatternStep ExecuteCylinderUp()
    {
        LogInfo("A轨顶升双气缸顶起");
        BrowLib.Controller.OutPort["顶升气缸_OUT"].On();
        _runTimer.Restart();
        return PatternStep.WaitCylinderUp;
    }

    /// <summary>
    /// 等待气缸到位
    /// </summary>
    private async Task<PatternStep> ExecuteWaitCylinderUpAsync()
    {
        if (await CheckSensorAsync("顶升上位_IN", 100))
        {
            LogInfo("A轨顶升双气缸顶起到位");
            await Task.Delay(LIFT_DELAY_MS);
            return PatternStep.MoveZToSafeHeight;
        }
        else if (_runTimer.IsOn(LIFT_TIMEOUT_MS))
        {
            return await HandleCylinderTimeoutAsync();
        }

        return PatternStep.WaitCylinderUp;
    }

    /// <summary>
    /// Z轴移动到安全高度
    /// </summary>
    private PatternStep ExecuteMoveZToSafeHeight()
    {
        Global.Z轴.PMove(
            Global.RunZVel,
            Global.RunZAcc,
            2, // 安全高度
            1
        );
        LogInfo("Z轴移动到安全高度");
        return PatternStep.WaitZSafeHeight;
    }

    /// <summary>
    /// 等待Z轴到位
    /// </summary>
    private async Task<PatternStep> ExecuteWaitZSafeHeightAsync()
    {
        if (await CheckAxisPositionAsync("Z轴", 2, POSITION_TOLERANCE))
        {
            await Task.Delay(CAMERA_DELAY_MS);
            return PatternStep.MoveToPatternStart;
        }
        return PatternStep.WaitZSafeHeight;
    }

    /// <summary>
    /// 移动到拼图起始位置
    /// </summary>
    private PatternStep ExecuteMoveToPatternStart()
    {
        BrowLib.Controller.MotionAPI.LinXyMoveA(
            Global.RunXYVel,
            Global.RunXYAcc,
            Global.Systemdata.PatlePos.Xpos,
            Global.Systemdata.PatlePos.Ypos,
            1,
            2
        );
        LogInfo($"移动到拼图起始位置: ({Global.Systemdata.PatlePos.Xpos}, {Global.Systemdata.PatlePos.Ypos})");
        return PatternStep.WaitPatternStart;
    }

    /// <summary>
    /// 等待移动到起始位置
    /// </summary>
    private async Task<PatternStep> ExecuteWaitPatternStartAsync()
    {
        if (await CheckLinearMotionCompleteAsync())
        {
            await Task.Delay(CAMERA_DELAY_MS);
            return PatternStep.SetupCameraAndLight;
        }
        return PatternStep.WaitPatternStart;
    }

    /// <summary>
    /// 设置光源并移动到拍照高度
    /// </summary>
    private PatternStep ExecuteSetupCameraAndLight()
    {
        // 设置光源
        if (PatternMode == 1)
        {
            Global.Light.SetRgbLight(
                Global.Systemdata.P_LED2.LED_R,
                Global.Systemdata.P_LED2.LED_G,
                Global.Systemdata.P_LED2.LED_B
            );
            LogInfo($"设置光源模式2 - R:{Global.Systemdata.P_LED2.LED_R}, G:{Global.Systemdata.P_LED2.LED_G}, B:{Global.Systemdata.P_LED2.LED_B}");
        }
        else
        {
            Global.Light.SetRgbLight(
                Global.Systemdata.P_LED.LED_R,
                Global.Systemdata.P_LED.LED_G,
                Global.Systemdata.P_LED.LED_B
            );
            LogInfo($"设置光源模式1 - R:{Global.Systemdata.P_LED.LED_R}, G:{Global.Systemdata.P_LED.LED_G}, B:{Global.Systemdata.P_LED.LED_B}");
        }

        // 移动到拍照高度
        Global.Z轴.PMove(
            Global.RunZVel,
            Global.RunZAcc,
            Global.CamHeight,
            1
        );
        LogInfo($"Z轴移动到拍照高度: {Global.CamHeight}");
        return PatternStep.WaitCameraHeight;
    }

    /// <summary>
    /// 等待到达拍照高度
    /// </summary>
    private async Task<PatternStep> ExecuteWaitCameraHeightAsync()
    {
        if (await CheckAxisPositionAsync("Z轴", Global.CamHeight, POSITION_TOLERANCE))
        {
            await Task.Delay(CAMERA_DELAY_MS);
            return PatternStep.CalculatePatternParameters;
        }
        return PatternStep.WaitCameraHeight;
    }

    /// <summary>
    /// 计算拼图参数
    /// </summary>
    private PatternStep ExecuteCalculatePatternParameters()
    {
        // 检查PCB参数
        if (Global.Parm.PcbLong == 0 || Global.Parm.PcbHight == 0)
        {
            ShowWarning("未设置PCB长宽参数,无法拼图");
            StopProcess("PCB参数未设置");
            return PatternStep.ErrorRecovery;
        }

        // 计算拼图位置
        string[] st =new Algorithm().Photolocation(
            Global.Model,
            Global.Systemdata.PatlePos.Xpos,
            Global.Systemdata.PatlePos.Ypos,
            Global.Parm.PcbLong,
            Global.Parm.PcbHight,
            Global.Systemdata.CameRaview.Row,
            Global.Systemdata.CameRaview.Col,
            out double[] xPositions,
            out double[] yPositions,
            out double rows,
            out double columns
        );

        // 保存到上下文
        _context.XPositions = xPositions;
        _context.YPositions = yPositions;
        _context.Rows = rows;
        _context.Columns = columns;
        _context.CurrentIndex = 1; // 从索引1开始

        // 设置视觉参数
        Global.VisionApp.SetToolData("Task2", "定义变量", 2110, 0, "false", 0);
        Global.VisionApp.SetToolData("Task2", "定义变量", 2107, 0, Global.Systemdata.PateFile, 3);
        Global.VisionApp.SetToolData("Task2", "定义变量", 2100, 0, "0", 1);
        Global.VisionApp.SetToolData("Task2", "定义变量", 2101, 0, rows.ToString(), 1);
        Global.VisionApp.SetToolData("Task2", "定义变量", 2102, 0, columns.ToString(), 1);
        Global.VisionApp.SetToolData("Task2", "定义变量", 2105, 0, Global.Systemdata.Cutsize.Row.ToString(), 1);
        Global.VisionApp.SetToolData("Task2", "定义变量", 2106, 0, Global.Systemdata.Cutsize.Col.ToString(), 1);

        LogInfo($"拼图参数计算完成 - 行数: {rows}, 列数: {columns}, 总位置数: {xPositions?.Length ?? 0}");
        return PatternStep.MoveToPatternPosition;
    }

    /// <summary>
    /// 移动到拼图位置
    /// </summary>
    private PatternStep ExecuteMoveToPatternPosition()
    {
        if (_context.CurrentIndex < _context.XPositions.Length)
        {
            double targetX = _context.XPositions[_context.CurrentIndex];
            double targetY = _context.YPositions[_context.CurrentIndex];

            BrowLib.Controller.MotionAPI.LinXyMoveA(800, 8000, targetX, targetY, 1);
            LogInfo($"移动到拼图位置 {_context.CurrentIndex}: ({targetX}, {targetY})");
            return PatternStep.WaitAndCapture;
        }
        else
        {
            LogInfo("所有拼图位置已完成");
            return PatternStep.PatternCompletionCheck;
        }
    }

    /// <summary>
    /// 等待移动完成并拍照
    /// </summary>
    private async Task<PatternStep> ExecuteWaitAndCaptureAsync()
    {
        if (await CheckLinearMotionCompleteAsync())
        {
            // 移动到下一个位置
            _context.CurrentIndex++;

            // 执行拍照
            Global.VisionApp.ExecuteProc("Task2", 1000);
            LogInfo($"执行拍照，位置索引: {_context.CurrentIndex - 1}");

            return PatternStep.ProcessCaptureResult;
        }
        return PatternStep.WaitAndCapture;
    }

    /// <summary>
    /// 处理拍照结果
    /// </summary>
    private async Task<PatternStep> ExecuteProcessCaptureResultAsync()
    {
        if (Global.VisionApp.EndProc["Task2"])
        {
            if (Global.VisionApp.RunState("Task2", "数据判断"))
            {
                // 继续下一个位置
                return PatternStep.MoveToPatternPosition;
            }
            else
            {
                ShowWarning("拼图失败");
                Global.TcpClass.Send("M:PictureERROR");
                StopProcess("拼图失败");
                return PatternStep.ErrorRecovery;
            }
        }
        return PatternStep.ProcessCaptureResult;
    }

    /// <summary>
    /// 拼图完成检查
    /// </summary>
    private async Task<PatternStep> ExecutePatternCompletionCheckAsync()
    {
        if (Global.VisionApp.GetBoolValue("Task2", "定义变量", 2109, 0))
        {
            Global.TcpClass.Send("M:PictureFinish");
            LogInfo("拼图完成");
            CompleteProcess();
            return PatternStep.ErrorRecovery;
        }
        return PatternStep.PatternCompletionCheck;
    }

    /// <summary>
    /// 错误恢复
    /// </summary>
    private async Task<PatternStep> ExecuteErrorRecoveryAsync()
    {
        LogInfo("执行错误恢复流程");

        // 安全停止所有运动
        SafetyStopAllMotions();

        await Task.Delay(1000);

        // 询问用户是否继续
        int result = APP.Tip.ShowTip(1, "错误恢复".tr(), "拼图流程遇到错误，是否重新开始？".tr(), "是".tr(), "否".tr());

        if (result == 1)
        {
            LogInfo("用户选择重新开始");
            Reset();
            return PatternStep.CheckEntrance;
        }
        else
        {
            LogInfo("用户选择停止流程");
            Stop();
            return PatternStep.ErrorRecovery;
        }
    }

    #endregion

    #region 工具方法

    /// <summary>
    /// 处理气缸超时
    /// </summary>
    private async Task<PatternStep> HandleCylinderTimeoutAsync()
    {
        LogError($"气缸顶升超时！已等待 {_runTimer.GetTime()}ms");

        int result = APP.Tip.ShowTip(1, "警告".tr(), "顶升到位超时".tr(), "继续".tr(), "停止".tr());

        if (result == 1) // 继续
        {
            LogInfo("顶升到位超时 - 用户选择继续");
            return PatternStep.MoveZToSafeHeight;
        }
        else // 停止
        {
            LogInfo("顶升到位超时 - 用户选择停止");
            StopProcess("顶升到位超时");
            return PatternStep.ErrorRecovery;
        }
    }

    /// <summary>
    /// 处理超时
    /// </summary>
    private async Task<PatternStep> HandleTimeoutAsync(string message)
    {
        ShowWarning(message);
        LogError($"{message} - 超时");

        int result = APP.Tip.ShowTip(1, "警告".tr(), message.tr(), "继续".tr(), "停止".tr());

        if (result == 1) // 继续
        {
            LogInfo($"{message} - 用户选择继续");
            return (PatternStep)((int)CurrentStep + 1);
        }
        else // 停止
        {
            LogInfo($"{message} - 用户选择停止");
            StopProcess(message);
            return PatternStep.ErrorRecovery;
        }
    }

    /// <summary>
    /// 完成流程
    /// </summary>
    private void CompleteProcess()
    {
        Global.SystemRun = false;
        Global.MachineState = GEnumEx.MachineState.MachineStop;
        Global.StopFlag = true;
        IsRunning = false;
        LogInfo("拼图流程正常完成");
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
        IsRunning = false;
        LogInfo($"拼图流程停止: {reason}");
    }

    /// <summary>
    /// 处理流程错误
    /// </summary>
    private void HandleProcessError(Exception ex)
    {
        LogError($"流程执行错误 - 步骤 {CurrentStep}: {ex.Message}\n{ex.StackTrace}");
        ShowError($"步骤 {CurrentStep} 执行异常");
        SafetyStopAllMotions();
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

    #endregion

    #region 硬件交互封装

    /// <summary>
    /// 检查传感器状态
    /// </summary>
    private async Task<bool> CheckSensorAsync(string sensorName, int delayMs = 0)
    {
        try
        {
            if (delayMs > 0)
            {
                await Task.Delay(delayMs);
            }

            bool state = BrowLib.Controller.InPort[sensorName].IsOn();
            LogDebug($"传感器 {sensorName} 状态: {state}");
            return state;
        }
        catch (Exception ex)
        {
            LogError($"检查传感器 {sensorName} 失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 检查线性运动是否完成
    /// </summary>
    private async Task<bool> CheckLinearMotionCompleteAsync()
    {
        return await Task.Run(() => BrowLib.Controller.MotionAPI.LinXYRuningA());
    }

    /// <summary>
    /// 检查轴是否到达目标位置
    /// </summary>
    private async Task<bool> CheckAxisPositionAsync(string axisName, double targetPos, double tolerance)
    {
        return await Task.Run(() =>
        {
            double currentPos = BrowLib.Controller.Motion[axisName].GetPrfPos();
            return Math.Abs(currentPos - targetPos) < tolerance && !BrowLib.Controller.Motion[axisName].Runing();
        });
    }

    #endregion

    #region 日志和提示

    /// <summary>
    /// 记录信息日志
    /// </summary>
    private void LogInfo(string message)
    {
        APP.Log.I_Log($"[PatternFlow] {message}");
    }

    /// <summary>
    /// 记录错误日志
    /// </summary>
    private void LogError(string message, Exception exception=null)
    {
        APP.Log.E_Log($"[PatternFlow] {message}", exception);
    }

    /// <summary>
    /// 记录调试日志
    /// </summary>
    private void LogDebug(string message)
    {
        
      APP.Log.I_Log($"[PatternFlow-Debug] {message}");
       
    }

    /// <summary>
    /// 显示警告
    /// </summary>
    private void ShowWarning(string message)
    {
        APP.Tip.ShowTip(1, "警告".tr(), message.tr(), "确定".tr());
    }

    /// <summary>
    /// 显示错误
    /// </summary>
    private void ShowError(string message)
    {
        APP.Tip.ShowTip(1, "错误".tr(), message.tr(), "确定".tr());
    }

    #endregion

    #region 上下文类

    /// <summary>
    /// 拼图流程上下文数据
    /// </summary>
    private class PatternContext
    {
        /// <summary>
        /// X轴位置数组
        /// </summary>
        public double[] XPositions { get; set; }

        /// <summary>
        /// Y轴位置数组
        /// </summary>
        public double[] YPositions { get; set; }

        /// <summary>
        /// 行数
        /// </summary>
        public double Rows { get; set; }

        /// <summary>
        /// 列数
        /// </summary>
        public double Columns { get; set; }

        /// <summary>
        /// 当前索引
        /// </summary>
        public int CurrentIndex { get; set; }

        /// <summary>
        /// 重置上下文
        /// </summary>
        public void Reset()
        {
            XPositions = null;
            YPositions = null;
            Rows = 0;
            Columns = 0;
            CurrentIndex = 1;
        }
    }

    #endregion
}