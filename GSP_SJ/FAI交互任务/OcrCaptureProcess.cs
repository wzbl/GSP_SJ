using BrowApp;
using BrowApp.Language;
using BrowLib;
using GSP;
using System;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// OCR自动采集流程管理器
/// </summary>
public class OcrCaptureProcess
{
    #region 常量定义

    private const double POSITION_TOLERANCE = 0.01;
    private const int LOOP_SLEEP_MS = 5;
    private const int MOTION_DELAY_MS = 10;
    private const string SHARED_MEMORY_NAME = "SharedImage";

    #endregion

    #region 状态枚举

    /// <summary>
    /// OCR采集流程步骤枚举
    /// </summary>
    public enum OcrStep : int
    {
        /// <summary>
        /// Z轴回安全位
        /// </summary>
        MoveZToSafeHeight = 0,

        /// <summary>
        /// 等待Z轴到位
        /// </summary>
        WaitZSafeHeight = 1,

        /// <summary>
        /// 请求OCR指令
        /// </summary>
        RequestOcrCommand = 2,

        /// <summary>
        /// 处理OCR指令
        /// </summary>
        ProcessOcrCommand = 3,

        /// <summary>
        /// 获取组件位置信息
        /// </summary>
        GetComponentInfo = 4,

        /// <summary>
        /// 移动到目标位置
        /// </summary>
        MoveToTargetPosition = 5,

        /// <summary>
        /// 等待移动完成
        /// </summary>
        WaitMovementComplete = 6,

        /// <summary>
        /// 移动到拍照高度
        /// </summary>
        MoveToCaptureHeight = 7,

        /// <summary>
        /// 等待拍照高度到位
        /// </summary>
        WaitCaptureHeight = 8,

        /// <summary>
        /// 计算拼图参数
        /// </summary>
        CalculatePatternParameters = 9,

        /// <summary>
        /// 移动到拼图位置
        /// </summary>
        MoveToPatternPosition = 10,

        /// <summary>
        /// 等待并执行拍照
        /// </summary>
        WaitAndExecuteCapture = 11,

        /// <summary>
        /// 处理拍照结果
        /// </summary>
        ProcessCaptureResult = 12,

        /// <summary>
        /// 拼图完成检查
        /// </summary>
        CheckPatternComplete = 13,

        /// <summary>
        /// 等待OCR确认
        /// </summary>
        WaitOcrConfirmation = 14,

        /// <summary>
        /// 错误恢复
        /// </summary>
        ErrorRecovery = 99
    }

    /// <summary>
    /// OCR指令类型
    /// </summary>
    public enum OcrCommandType
    {
        Finish,
        Graspimg
    }

    #endregion

    #region 属性

    /// <summary>
    /// 当前步骤
    /// </summary>
    public OcrStep CurrentStep { get; private set; }

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
    private OcrContext _context;

    /// <summary>
    /// 步骤计时器
    /// </summary>
    private Stopwatch _stepTimer;

    /// <summary>
    /// 取消令牌
    /// </summary>
    private CancellationTokenSource _cancellationTokenSource;

    #endregion

    #region 构造函数

    public OcrCaptureProcess()
    {
        _context = new OcrContext();
        _stepTimer = new Stopwatch();
        _cancellationTokenSource = new CancellationTokenSource();
        CurrentStep = OcrStep.MoveZToSafeHeight;
    }

    #endregion

    #region 公共方法

    /// <summary>
    /// 启动OCR采集流程
    /// </summary>
    public void Start()
    {
        if (IsRunning) return;

        IsRunning = true;
        CurrentStep = OcrStep.MoveZToSafeHeight;
        _context.Reset();
        Global.StopFlag = false;
        Global.SystemRun = true;

        Task.Run(() => RunProcessAsync(_cancellationTokenSource.Token));
    }

    /// <summary>
    /// 停止OCR采集流程
    /// </summary>
    public void Stop()
    {
        IsRunning = false;
        _cancellationTokenSource.Cancel();
        SafetyStopAllMotions();

        Global.MachineState = GEnumEx.MachineState.MachineStop;
        Global.SystemRun = false;
        Global.StopFlag = true;

        LogInfo("OCR采集流程已停止");
    }

    /// <summary>
    /// 暂停/恢复流程
    /// </summary>
    public void TogglePause()
    {
        IsPaused = !IsPaused;
        LogInfo($"OCR采集流程已{(IsPaused ? "暂停" : "恢复")}");
    }

    /// <summary>
    /// 重置流程
    /// </summary>
    public void Reset()
    {
        Stop();
        _cancellationTokenSource = new CancellationTokenSource();
        CurrentStep = OcrStep.MoveZToSafeHeight;
        _context.Reset();
        LogInfo("OCR采集流程已重置");
    }

    #endregion

    #region 主要流程

    /// <summary>
    /// 异步执行主流程
    /// </summary>
    private async Task RunProcessAsync(CancellationToken cancellationToken)
    {
        LogInfo("开始OCR自动采集流程");

        while (IsRunning && !cancellationToken.IsCancellationRequested && !Global.StopFlag)
        {
            try
            {
                // 检查是否可以执行当前步骤
                if (CanExecuteStep(CurrentStep))
                {
                    // 执行当前步骤
                    OcrStep nextStep = await ExecuteStepAsync(CurrentStep, cancellationToken);

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
                LogInfo("OCR采集流程被取消");
                break;
            }
            catch (Exception ex)
            {
                // 处理异常
                HandleProcessError(ex);
                CurrentStep = OcrStep.ErrorRecovery;
            }
        }

        IsRunning = false;
        LogInfo("OCR采集流程结束");
    }

    /// <summary>
    /// 检查是否可以执行当前步骤
    /// </summary>
    private bool CanExecuteStep(OcrStep step)
    {
        return !IsPaused;
    }

    /// <summary>
    /// 执行步骤
    /// </summary>
    private async Task<OcrStep> ExecuteStepAsync(OcrStep step, CancellationToken cancellationToken)
    {
        _stepTimer.Restart();

        try
        {
            switch (step)
            {
                case OcrStep.MoveZToSafeHeight:
                    return ExecuteMoveZToSafeHeight();

                case OcrStep.WaitZSafeHeight:
                    return await ExecuteWaitZSafeHeightAsync();

                case OcrStep.RequestOcrCommand:
                    return ExecuteRequestOcrCommand();

                case OcrStep.ProcessOcrCommand:
                    return await ExecuteProcessOcrCommandAsync();

                case OcrStep.GetComponentInfo:
                    return ExecuteGetComponentInfo();

                case OcrStep.MoveToTargetPosition:
                    return ExecuteMoveToTargetPosition();

                case OcrStep.WaitMovementComplete:
                    return await ExecuteWaitMovementCompleteAsync();

                case OcrStep.MoveToCaptureHeight:
                    return ExecuteMoveToCaptureHeight();

                case OcrStep.WaitCaptureHeight:
                    return await ExecuteWaitCaptureHeightAsync();

                case OcrStep.CalculatePatternParameters:
                    return ExecuteCalculatePatternParameters();

                case OcrStep.MoveToPatternPosition:
                    return ExecuteMoveToPatternPosition();

                case OcrStep.WaitAndExecuteCapture:
                    return await ExecuteWaitAndExecuteCaptureAsync();

                case OcrStep.ProcessCaptureResult:
                    return await ExecuteProcessCaptureResultAsync();

                case OcrStep.CheckPatternComplete:
                    return await ExecuteCheckPatternCompleteAsync();

                case OcrStep.WaitOcrConfirmation:
                    return await ExecuteWaitOcrConfirmationAsync();

                case OcrStep.ErrorRecovery:
                    return await ExecuteErrorRecoveryAsync();

                default:
                    return (OcrStep)((int)step + 1);
            }
        }
        catch (Exception ex)
        {
            LogError($"执行步骤 {step} 时发生错误:", ex);
            return OcrStep.ErrorRecovery;
        }
    }

    #endregion

    #region 步骤实现

    /// <summary>
    /// Z轴回安全位
    /// </summary>
    private OcrStep ExecuteMoveZToSafeHeight()
    {
        Global.Z轴.PMove(
            Global.RunZVel,
            Global.RunZAcc,
            2, // 安全高度
            1
        );
        LogInfo("Z轴回安全位");
        return OcrStep.WaitZSafeHeight;
    }

    /// <summary>
    /// 等待Z轴到位
    /// </summary>
    private async Task<OcrStep> ExecuteWaitZSafeHeightAsync()
    {
        if (await CheckAxisPositionAsync("Z轴", 2, POSITION_TOLERANCE))
        {
            await Task.Delay(MOTION_DELAY_MS);
            return OcrStep.RequestOcrCommand;
        }
        return OcrStep.WaitZSafeHeight;
    }

    /// <summary>
    /// 请求OCR指令
    /// </summary>
    private OcrStep ExecuteRequestOcrCommand()
    {
        Global.TcpClass.TCP_AStaart = false;
        Global.TcpClass.Send("A:OCRGET\r\n");
        LogInfo("发送OCR指令请求");
        return OcrStep.ProcessOcrCommand;
    }

    /// <summary>
    /// 处理OCR指令
    /// </summary>
    private async Task<OcrStep> ExecuteProcessOcrCommandAsync()
    {
        if (Global.TcpClass.TCP_AStaart)
        {
            string response = Global.TcpClass.TCP_Result.Trim();
            string[] parts = response.Split(':');

            LogInfo($"收到OCR指令: {response}");

            if (parts[0] == "A")
            {
                if (parts[1] == "Finish")
                {
                    return await HandleFinishCommandAsync();
                }
                else if (parts.Length > 2 && parts[2] == "Graspimg")
                {
                    return await HandleGraspimgCommandAsync(parts);
                }
            }
        }

        return OcrStep.ProcessOcrCommand;
    }

    /// <summary>
    /// 处理完成指令
    /// </summary>
    private async Task<OcrStep> HandleFinishCommandAsync()
    {
        Global.TcpClass.Send("A:Finish");
        LogInfo("OCR采集完成");
        CompleteProcess();
        return OcrStep.ErrorRecovery;
    }

    /// <summary>
    /// 处理抓图指令
    /// </summary>
    private async Task<OcrStep> HandleGraspimgCommandAsync(string[] parts)
    {
        // 提取组件信息
        _context.ComponentCode = parts[1];

        if (parts.Length >= 9)
        {
            _context.Width = Convert.ToDouble(parts[3]);
            _context.Height = Convert.ToDouble(parts[4]);
            _context.CaptureHeight = Convert.ToDouble(parts[5]);

            int r = Convert.ToInt16(parts[6]);
            int g = Convert.ToInt16(parts[7]);
            int b = Convert.ToInt16(parts[8]);

            // 设置光源
            if (r != -1 || g != -1 || b != -1)
            {
                Global.Light.SetRgbLight(r, g, b);
                LogInfo($"设置自定义光源 R:{r}, G:{g}, B:{b}");
            }
            else
            {
                Global.Light.SetRgbLight(
                    Global.Systemdata.P_LED.LED_R,
                    Global.Systemdata.P_LED.LED_G,
                    Global.Systemdata.P_LED.LED_B
                );
                LogInfo($"设置默认光源 R:{Global.Systemdata.P_LED.LED_R}, G:{Global.Systemdata.P_LED.LED_G}, B:{Global.Systemdata.P_LED.LED_B}");
            }

            LogInfo($"接收抓图指令 - 组件: {_context.ComponentCode}, 宽: {_context.Width}, 高: {_context.Height}, 拍照高度: {_context.CaptureHeight}");
            return OcrStep.GetComponentInfo;
        }

        LogError($"抓图指令格式错误: {string.Join(":", parts)}");
        return OcrStep.ErrorRecovery;
    }

    /// <summary>
    /// 获取组件位置信息
    /// </summary>
    private OcrStep ExecuteGetComponentInfo()
    {
        DataRow[] dataRows = Global.BomData.Select($"位置号='{_context.ComponentCode}'");

        if (dataRows.Length == 0)
        {
            ShowError($"未找到组件: {_context.ComponentCode}");
            StopProcess($"组件{_context.ComponentCode}不存在");
            return OcrStep.ErrorRecovery;
        }

        DataRow row = dataRows[0];

        // 获取基本数据
        _context.OriginalX = Convert.ToDouble(row["原始X坐标"]);
        _context.OriginalY = Convert.ToDouble(row["原始Y坐标"]);
        _context.Rotation = Convert.ToDouble(row["原始方向"]);
        _context.OffsetX = Convert.ToDouble(row["X坐标调整"]);
        _context.OffsetY = Convert.ToDouble(row["Y坐标调整"]);

        // 坐标变换
        _context.TransformedX = _context.OriginalX + VisionGlobal.TranslationX + Global.Parm.PbXoffset;
        _context.TransformedY = _context.OriginalY + VisionGlobal.TranslationY + Global.Parm.PbYoffset;

        // 如果没有Mark定位，使用原始Mark位置
        if (Global.Is_NoMark)
        {
            VisionGlobal.Mak1_X = Global.Parm.Mak1Pos.Xpos;
            VisionGlobal.Mak1_Y = Global.Parm.Mak1Pos.Ypos;
            VisionGlobal.Mak2_X = Global.Parm.Mak2Pos.Xpos;
            VisionGlobal.Mak2_Y = Global.Parm.Mak2Pos.Ypos;
        }

        // Mark计算
        int errorCode = new Algorithm().MakAlgorithm(
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
            return OcrStep.ErrorRecovery;
        }

        // 整体补偿和偏移调整
        _context.TargetX = targetX + Global.Parm.Offset_X - _context.OffsetX;
        _context.TargetY = targetY + Global.Parm.Offset_Y - _context.OffsetY;

        LogInfo($"组件位置计算完成 - 目标位置: X={_context.TargetX:F3}, Y={_context.TargetY:F3}");
        return OcrStep.MoveToTargetPosition;
    }

    /// <summary>
    /// 移动到目标位置
    /// </summary>
    private OcrStep ExecuteMoveToTargetPosition()
    {
        BrowLib.Controller.MotionAPI.LinXyMoveA(
            Global.RunXYVel,
            Global.RunXYAcc,
            _context.TargetX,
            _context.TargetY,
            1,
            2
        );
        LogInfo($"移动到目标位置: X={_context.TargetX:F3}, Y={_context.TargetY:F3}");
        return OcrStep.WaitMovementComplete;
    }

    /// <summary>
    /// 等待移动完成
    /// </summary>
    private async Task<OcrStep> ExecuteWaitMovementCompleteAsync()
    {
        if (await CheckLinearMotionCompleteAsync())
        {
            await Task.Delay(MOTION_DELAY_MS);
            return OcrStep.MoveToCaptureHeight;
        }
        return OcrStep.WaitMovementComplete;
    }

    /// <summary>
    /// 移动到拍照高度
    /// </summary>
    private OcrStep ExecuteMoveToCaptureHeight()
    {
        double captureHeight = Global.CamHeight + _context.CaptureHeight;
        Global.Z轴.PMove(
            Global.RunZVel,
            Global.RunZAcc,
            captureHeight,
            1
        );
        LogInfo($"移动到拍照高度: {captureHeight}");
        return OcrStep.WaitCaptureHeight;
    }

    /// <summary>
    /// 等待拍照高度到位
    /// </summary>
    private async Task<OcrStep> ExecuteWaitCaptureHeightAsync()
    {
        double captureHeight = Global.CamHeight + _context.CaptureHeight;

        if (await CheckAxisPositionAsync("Z轴", captureHeight, POSITION_TOLERANCE))
        {
            await Task.Delay(MOTION_DELAY_MS);
            return OcrStep.CalculatePatternParameters;
        }
        return OcrStep.WaitCaptureHeight;
    }

    /// <summary>
    /// 计算拼图参数
    /// </summary>
    private OcrStep ExecuteCalculatePatternParameters()
    {
        new Algorithm().GetpatePoint2(
            _context.TargetX,
            _context.TargetY,
            Global.Systemdata.sCameRaview.Row,
            Global.Systemdata.sCameRaview.Col,
            _context.Width,
            _context.Height,
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
        Global.VisionApp.SetToolData("Task6", "定义变量", 2110, 0, "false", 0);
        Global.VisionApp.SetToolData("Task6", "定义变量", 2107, 0, Global.Systemdata.PateFile, 3);
        Global.VisionApp.SetToolData("Task6", "定义变量", 2100, 0, "0", 1);
        Global.VisionApp.SetToolData("Task6", "定义变量", 2101, 0, rows.ToString(), 1);
        Global.VisionApp.SetToolData("Task6", "定义变量", 2102, 0, columns.ToString(), 1);
        Global.VisionApp.SetToolData("Task6", "定义变量", 2105, 0, Global.Systemdata.Cutsize.Row.ToString(), 1);
        Global.VisionApp.SetToolData("Task6", "定义变量", 2106, 0, Global.Systemdata.Cutsize.Col.ToString(), 1);

        LogInfo($"拼图参数计算完成 - 行数: {rows}, 列数: {columns}, 总位置数: {xPositions?.Length ?? 0}");
        return OcrStep.MoveToPatternPosition;
    }

    /// <summary>
    /// 移动到拼图位置
    /// </summary>
    private OcrStep ExecuteMoveToPatternPosition()
    {
        if (_context.CurrentIndex < _context.XPositions.Length)
        {
            double targetX = _context.XPositions[_context.CurrentIndex];
            double targetY = _context.YPositions[_context.CurrentIndex];

            BrowLib.Controller.MotionAPI.LinXyMoveA(800, 8000, targetX, targetY, 1);
            LogInfo($"移动到拼图位置 {_context.CurrentIndex}: ({targetX:F3}, {targetY:F3})");
            return OcrStep.WaitAndExecuteCapture;
        }
        else
        {
            LogInfo("所有拼图位置已完成，检查拼图结果");
            return OcrStep.CheckPatternComplete;
        }
    }

    /// <summary>
    /// 等待并执行拍照
    /// </summary>
    private async Task<OcrStep> ExecuteWaitAndExecuteCaptureAsync()
    {
        if (await CheckLinearMotionCompleteAsync())
        {
            // 移动到下一个位置
            _context.CurrentIndex++;

            // 执行拍照
            Global.VisionApp.ExecuteProc("Task6", 0);
            LogInfo($"执行拍照，位置索引: {_context.CurrentIndex - 1}");

            return OcrStep.ProcessCaptureResult;
        }
        return OcrStep.WaitAndExecuteCapture;
    }

    /// <summary>
    /// 处理拍照结果
    /// </summary>
    private async Task<OcrStep> ExecuteProcessCaptureResultAsync()
    {
        if (Global.VisionApp.EndProc["Task6"])
        {
            if (Global.VisionApp.RunState("Task6", "数据判断"))
            {
                // 继续下一个位置
                return OcrStep.MoveToPatternPosition;
            }
            else
            {
                ShowError("拼图失败");
                Global.TcpClass.Send("A:GraspimgNG");
                StopProcess("拼图失败");
                return OcrStep.ErrorRecovery;
            }
        }
        return OcrStep.ProcessCaptureResult;
    }

    /// <summary>
    /// 拼图完成检查
    /// </summary>
    private async Task<OcrStep> ExecuteCheckPatternCompleteAsync()
    {
        if (Global.VisionApp.GetBoolValue("Task6", "定义变量", 2109, 0))
        {
            LogInfo("拼图完成，准备写入共享内存");
            return await WriteImageToSharedMemoryAsync();
        }
        else
        {
            // 检查是否已经完成所有位置
            if (_context.CurrentIndex >= _context.XPositions.Length)
            {
                LogInfo("所有位置已完成但拼图未完成，发送抓图完成信号");
                return SendGraspimgCompleteSignal();
            }
            return OcrStep.CheckPatternComplete;
        }
    }

    /// <summary>
    /// 写入图像到共享内存
    /// </summary>
    private async Task<OcrStep> WriteImageToSharedMemoryAsync()
    {
        try
        {
            int tFormat = 0;
            int tWidth = 0;
            int tHeight = 0;

            // 获取图像数据
            IntPtr ptr = Global.VisionApp.GetImageBits("Task6", "图像合并", ref tFormat, ref tWidth, ref tHeight);

            // 写入共享内存
            bool result = MemoryHelper.Write_SharedMemory(ptr, tFormat, tWidth, tHeight, SHARED_MEMORY_NAME);

            LogInfo($"写入共享内存 {SHARED_MEMORY_NAME} 结果: {result}, 图像尺寸: {tWidth}x{tHeight}, 格式: {tFormat}");

            // 发送抓图完成信号
            return SendGraspimgCompleteSignal();
        }
        catch (Exception ex)
        {
            LogError($"写入共享内存失败: {ex.Message}");
            Global.TcpClass.Send("A:GraspimgNG");
            StopProcess("写入共享内存失败");
            return OcrStep.ErrorRecovery;
        }
    }

    /// <summary>
    /// 发送抓图完成信号
    /// </summary>
    private OcrStep SendGraspimgCompleteSignal()
    {
        Global.TcpClass.TCP_AStaart = false;
        Global.TcpClass.Send("A:GraspimgOK");
        LogInfo("发送抓图完成信号");
        return OcrStep.WaitOcrConfirmation;
    }

    /// <summary>
    /// 等待OCR确认
    /// </summary>
    private async Task<OcrStep> ExecuteWaitOcrConfirmationAsync()
    {
        if (Global.TcpClass.TCP_AStaart)
        {
            string response = Global.TcpClass.TCP_Result.Trim();
            string[] parts = response.Split(':');

            LogInfo($"收到OCR确认: {response}");

            if (parts[0] == "A" && parts[1] == "OCR_OK")
            {
                // 重置索引，继续下一个组件
                _context.CurrentIndex = 1;
                return OcrStep.RequestOcrCommand;
            }
        }
        return OcrStep.WaitOcrConfirmation;
    }

    /// <summary>
    /// 错误恢复
    /// </summary>
    private async Task<OcrStep> ExecuteErrorRecoveryAsync()
    {
        LogInfo("执行错误恢复流程");

        // 安全停止所有运动
        SafetyStopAllMotions();

        await Task.Delay(1000);

        // 询问用户是否继续
        int result = APP.Tip.ShowTip(1, "错误恢复".tr(), "OCR采集流程遇到错误，是否重新开始？".tr(), "是".tr(), "否".tr());

        if (result == 1)
        {
            LogInfo("用户选择重新开始");
            Reset();
            return OcrStep.MoveZToSafeHeight;
        }
        else
        {
            LogInfo("用户选择停止流程");
            Stop();
            return OcrStep.ErrorRecovery;
        }
    }

    #endregion

    #region 工具方法

    /// <summary>
    /// 完成流程
    /// </summary>
    private void CompleteProcess()
    {
        Global.SystemRun = false;
        Global.MachineState = GEnumEx.MachineState.MachineStop;
        Global.StopFlag = true;
        IsRunning = false;
        LogInfo("OCR采集流程正常完成");
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
        LogInfo($"OCR采集流程停止: {reason}");
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
            Global.Z轴.AxisStop();
            BrowLib.Controller.CardAPI.StopAxis();
        }
        catch (Exception ex)
        {
            LogError($"安全停止时出错: {ex.Message}");
        }
    }

    #endregion

    #region 硬件交互封装

    /// <summary>
    /// 检查线性运动是否完成
    /// </summary>
    private async Task<bool> CheckLinearMotionCompleteAsync()
    {
        return await Task.Run(() => !BrowLib.Controller.MotionAPI.LinXYRuningA());
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
        APP.Log.I_Log($"[OcrCapture] {message}");
    }

    /// <summary>
    /// 记录错误日志
    /// </summary>
    private void LogError(string message, Exception exception=null)
    {
        APP.Log.E_Log($"[OcrCapture] {message}" ,exception);
    }

    /// <summary>
    /// 记录调试日志
    /// </summary>
    private void LogDebug(string message)
    {
      APP.Log.I_Log($"[OcrCapture-Debug] {message}");
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
    /// OCR采集流程上下文数据
    /// </summary>
    private class OcrContext
    {
        /// <summary>
        /// 组件代码
        /// </summary>
        public string ComponentCode { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// 高度
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// 拍照高度
        /// </summary>
        public double CaptureHeight { get; set; }

        /// <summary>
        /// 原始X坐标
        /// </summary>
        public double OriginalX { get; set; }

        /// <summary>
        /// 原始Y坐标
        /// </summary>
        public double OriginalY { get; set; }

        /// <summary>
        /// 旋转角度
        /// </summary>
        public double Rotation { get; set; }

        /// <summary>
        /// X坐标调整
        /// </summary>
        public double OffsetX { get; set; }

        /// <summary>
        /// Y坐标调整
        /// </summary>
        public double OffsetY { get; set; }

        /// <summary>
        /// 变换后的X坐标
        /// </summary>
        public double TransformedX { get; set; }

        /// <summary>
        /// 变换后的Y坐标
        /// </summary>
        public double TransformedY { get; set; }

        /// <summary>
        /// 目标X坐标
        /// </summary>
        public double TargetX { get; set; }

        /// <summary>
        /// 目标Y坐标
        /// </summary>
        public double TargetY { get; set; }

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
            ComponentCode = string.Empty;
            Width = Height = CaptureHeight = 0;
            OriginalX = OriginalY = Rotation = 0;
            OffsetX = OffsetY = 0;
            TransformedX = TransformedY = 0;
            TargetX = TargetY = 0;
            XPositions = YPositions = null;
            Rows = Columns = 0;
            CurrentIndex = 1;
        }
    }

    #endregion
}