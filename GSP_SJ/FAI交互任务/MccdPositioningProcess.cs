using BrowApp;
using BrowApp.Language;
using BrowLib;
using GSP;
using GSP.UI;
using System;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// MCCD定位流程管理器
/// </summary>
public class MccdPositioningProcess
{
    #region 常量定义

    private const double POSITION_TOLERANCE = 0.02;
    private const int LOOP_SLEEP_MS = 3;
    private const int MOTION_DELAY_MS = 50;
    private const int TEST_DELAY_MS = 200;
    private const double SAFE_HEIGHT_SPEED_FACTOR = 0.8;

    #endregion

    #region 状态枚举

    /// <summary>
    /// MCCD定位步骤枚举
    /// </summary>
    public enum MccdStep : int
    {
        /// <summary>
        /// 线数切换
        /// </summary>
        SwitchLineMode = 0,

        /// <summary>
        /// Z轴移动到安全高度
        /// </summary>
        MoveZToSafeHeight = 1,

        /// <summary>
        /// 等待Z轴到位
        /// </summary>
        WaitZSafeHeight = 2,

        /// <summary>
        /// 获取组件信息
        /// </summary>
        GetComponentInfo = 3,

        /// <summary>
        /// 移动到Mark补偿位置
        /// </summary>
        MoveToMarkCompensatedPosition = 4,

        /// <summary>
        /// 等待移动完成
        /// </summary>
        WaitMovementComplete = 5,

        /// <summary>
        /// Z轴移动到CCD高度
        /// </summary>
        MoveZToCcdHeight = 6,

        /// <summary>
        /// 等待CCD高度到位
        /// </summary>
        WaitCcdHeight = 7,

        /// <summary>
        /// 校准对话框
        /// </summary>
        CalibrationDialog = 8,

        /// <summary>
        /// 计算最终位置并移动
        /// </summary>
        CalculateAndMoveToFinalPosition = 9,

        /// <summary>
        /// 等待所有轴到位
        /// </summary>
        WaitAllAxesReady = 10,

        /// <summary>
        /// Z轴快速下针
        /// </summary>
        ZAxisFastApproach = 11,

        /// <summary>
        /// Z轴精确定位
        /// </summary>
        ZAxisPrecisePositioning = 12,

        /// <summary>
        /// 夹紧或测试
        /// </summary>
        ClampOrTest = 13,

        /// <summary>
        /// 等待夹紧完成
        /// </summary>
        WaitClampComplete = 14,

        /// <summary>
        /// 等待测试确认
        /// </summary>
        WaitTestConfirmation = 15,

        /// <summary>
        /// 等待轴归位
        /// </summary>
        WaitAxesReturn = 16,

        /// <summary>
        /// Z轴回安全高度
        /// </summary>
        ReturnZToSafeHeight = 17,

        /// <summary>
        /// 等待安全高度到位
        /// </summary>
        WaitSafeHeight = 18,

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
    public MccdStep CurrentStep { get; private set; }

    /// <summary>
    /// 是否正在运行
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// 组件代码
    /// </summary>
    public string ComponentCode { get; private set; }

    /// <summary>
    /// 线数模式
    /// </summary>
    public string LineMode { get; private set; }

    /// <summary>
    /// 上下文数据
    /// </summary>
    private MccdContext _context;

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

    public MccdPositioningProcess()
    {
        _context = new MccdContext();
        _stepTimer = new Stopwatch();
        _cancellationTokenSource = new CancellationTokenSource();
        CurrentStep = MccdStep.SwitchLineMode;
    }

    #endregion

    #region 公共方法

    /// <summary>
    /// 启动MCCD定位流程
    /// </summary>
    /// <param name="componentCode">组件代码</param>
    /// <param name="lineMode">线数模式</param>
    public void Start(string componentCode, string lineMode)
    {
        if (IsRunning) return;

        IsRunning = true;
        ComponentCode = componentCode;
        LineMode = lineMode;
        CurrentStep = MccdStep.SwitchLineMode;
        _context.Reset();
        Global.StopFlag = false;

        Task.Run(() => RunProcessAsync(_cancellationTokenSource.Token));
    }

    /// <summary>
    /// 停止MCCD定位流程
    /// </summary>
    public void Stop()
    {
        IsRunning = false;
        _cancellationTokenSource.Cancel();
        SafetyStopAllMotions();

        Global.StopFlag = true;
        LogInfo("MCCD定位流程已停止");
    }

    /// <summary>
    /// 重置流程
    /// </summary>
    public void Reset()
    {
        Stop();
        _cancellationTokenSource = new CancellationTokenSource();
        CurrentStep = MccdStep.SwitchLineMode;
        _context.Reset();
        LogInfo("MCCD定位流程已重置");
    }

    #endregion

    #region 主要流程

    /// <summary>
    /// 异步执行主流程
    /// </summary>
    private async Task RunProcessAsync(CancellationToken cancellationToken)
    {
        LogInfo($"开始MCCD定位流程，组件: {ComponentCode}, 线数模式: {LineMode}");

        while (IsRunning && !cancellationToken.IsCancellationRequested && !Global.StopFlag)
        {
            try
            {
                // 执行当前步骤
                MccdStep nextStep = await ExecuteStepAsync(CurrentStep, cancellationToken);

                // 更新步骤
                if (nextStep != CurrentStep)
                {
                    LogInfo($"步骤切换: {CurrentStep} -> {nextStep}");
                    CurrentStep = nextStep;
                }

                // 短暂休眠以减少CPU占用
                await Task.Delay(LOOP_SLEEP_MS, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // 正常取消
                LogInfo("MCCD定位流程被取消");
                break;
            }
            catch (Exception ex)
            {
                // 处理异常
                HandleProcessError(ex);
                CurrentStep = MccdStep.ErrorRecovery;
            }
        }

        IsRunning = false;
        LogInfo("MCCD定位流程结束");
    }

    /// <summary>
    /// 执行步骤
    /// </summary>
    private async Task<MccdStep> ExecuteStepAsync(MccdStep step, CancellationToken cancellationToken)
    {
        _stepTimer.Restart();

        try
        {
            switch (step)
            {
                case MccdStep.SwitchLineMode:
                    return ExecuteSwitchLineMode();

                case MccdStep.MoveZToSafeHeight:
                    return ExecuteMoveZToSafeHeight();

                case MccdStep.WaitZSafeHeight:
                    return await ExecuteWaitZSafeHeightAsync();

                case MccdStep.GetComponentInfo:
                    return ExecuteGetComponentInfo();

                case MccdStep.MoveToMarkCompensatedPosition:
                    return ExecuteMoveToMarkCompensatedPosition();

                case MccdStep.WaitMovementComplete:
                    return await ExecuteWaitMovementCompleteAsync();

                case MccdStep.MoveZToCcdHeight:
                    return ExecuteMoveZToCcdHeight();

                case MccdStep.WaitCcdHeight:
                    return await ExecuteWaitCcdHeightAsync();

                case MccdStep.CalibrationDialog:
                    return ExecuteCalibrationDialog();

                case MccdStep.CalculateAndMoveToFinalPosition:
                    return ExecuteCalculateAndMoveToFinalPosition();

                case MccdStep.WaitAllAxesReady:
                    return await ExecuteWaitAllAxesReadyAsync();

                case MccdStep.ZAxisFastApproach:
                    return ExecuteZAxisFastApproach();

                case MccdStep.ZAxisPrecisePositioning:
                    return await ExecuteZAxisPrecisePositioningAsync();

                case MccdStep.ClampOrTest:
                    return ExecuteClampOrTest();

                case MccdStep.WaitClampComplete:
                    return await ExecuteWaitClampCompleteAsync();

                case MccdStep.WaitTestConfirmation:
                    return await ExecuteWaitTestConfirmationAsync();

                case MccdStep.WaitAxesReturn:
                    return await ExecuteWaitAxesReturnAsync();

                case MccdStep.ReturnZToSafeHeight:
                    return ExecuteReturnZToSafeHeight();

                case MccdStep.WaitSafeHeight:
                    return await ExecuteWaitSafeHeightAsync();

                case MccdStep.ErrorRecovery:
                    return await ExecuteErrorRecoveryAsync();

                default:
                    return (MccdStep)((int)step + 1);
            }
        }
        catch (Exception ex)
        {
            LogError($"执行步骤 {step} 时发生错误: {ex.Message}", ex);
            return MccdStep.ErrorRecovery;
        }
    }

    #endregion

    #region 步骤实现

    /// <summary>
    /// 线数切换
    /// </summary>
    private MccdStep ExecuteSwitchLineMode()
    {
        switch (LineMode)
        {
            case "2":
                BrowLib.Controller.OutPort["四线切换两线"].On();
                LogInfo("切换为两线模式");
                break;
            case "4":
                BrowLib.Controller.OutPort["四线切换两线"].Off();
                LogInfo("切换为四线模式");
                break;
            default:
                LogWarning($"未知的线数模式: {LineMode}");
                break;
        }

        return MccdStep.MoveZToSafeHeight;
    }

    /// <summary>
    /// Z轴移动到安全高度
    /// </summary>
    private MccdStep ExecuteMoveZToSafeHeight()
    {
        Global.Z轴.PMove(
            Global.RunZVel * SAFE_HEIGHT_SPEED_FACTOR,
            Global.RunZAcc * SAFE_HEIGHT_SPEED_FACTOR,
            Global.Systemdata.SafeHigh,
            1
        );
        LogInfo($"Z轴移动到安全高度: {Global.Systemdata.SafeHigh}");
        return MccdStep.WaitZSafeHeight;
    }

    /// <summary>
    /// 等待Z轴到位
    /// </summary>
    private async Task<MccdStep> ExecuteWaitZSafeHeightAsync()
    {
        if (await CheckAxisPositionAsync("Z轴", Global.Systemdata.SafeHigh, POSITION_TOLERANCE))
        {
            return MccdStep.GetComponentInfo;
        }
        return MccdStep.WaitZSafeHeight;
    }

    /// <summary>
    /// 获取组件信息
    /// </summary>
    private MccdStep ExecuteGetComponentInfo()
    {
        DataRow[] dataRows = Global.BomData.Select($"位置号='{ComponentCode}'");

        if (dataRows.Length == 0)
        {
            ShowError($"未找到组件: {ComponentCode}");
            StopProcess($"组件{ComponentCode}不存在");
            return MccdStep.ErrorRecovery;
        }

        DataRow row = dataRows[0];

        // 获取基本数据
        _context.OriginalX = Convert.ToDouble(row["原始X坐标"]);
        _context.OriginalY = Convert.ToDouble(row["原始Y坐标"]);
        _context.OriginalRotation = Convert.ToDouble(row["原始方向"]);
        _context.OffsetX = Convert.ToDouble(row["X坐标调整"]);
        _context.OffsetY = Convert.ToDouble(row["Y坐标调整"]);
        _context.ComponentType = row["尺寸"].ToString();

        // 坐标变换
        _context.TransformedX = _context.OriginalX + VisionGlobal.TranslationX + Global.Parm.PbXoffset;
        _context.TransformedY = _context.OriginalY + VisionGlobal.TranslationY + Global.Parm.PbYoffset;

        // 获取组件参数
        _context.LeftPosition = Global.GetSize(_context.ComponentType);
        _context.TestHeight = Global.GetHight(_context.ComponentType);
        _context.ClampSize = Global.JQSize(_context.ComponentType);

        // 如果没有Mark定位，使用原始Mark位置
        if (Global.Is_NoMark)
        {
            VisionGlobal.Mak1_X = Global.Parm.Mak1Pos.Xpos;
            VisionGlobal.Mak1_Y = Global.Parm.Mak1Pos.Ypos;
            VisionGlobal.Mak2_X = Global.Parm.Mak2Pos.Xpos;
            VisionGlobal.Mak2_Y = Global.Parm.Mak2Pos.Ypos;
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
            out double markX,
            out double markY
        );

        if (errorCode != 0)
        {
            ShowError("Mark绑定数据出错");
            StopProcess("Mark绑定数据出错");
            return MccdStep.ErrorRecovery;
        }

        // 整体补偿
        _context.MarkCompensatedX = markX + Global.Parm.Offset_X;
        _context.MarkCompensatedY = markY + Global.Parm.Offset_Y;

        LogInfo($"组件信息获取完成 - 原始: ({_context.OriginalX:F3}, {_context.OriginalY:F3}), " +
               $"Mark补偿: ({_context.MarkCompensatedX:F3}, {_context.MarkCompensatedY:F3}), " +
               $"开口大小: {_context.LeftPosition}, 测试高度: {_context.TestHeight}");

        return MccdStep.MoveToMarkCompensatedPosition;
    }

    /// <summary>
    /// 移动到Mark补偿位置
    /// </summary>
    private MccdStep ExecuteMoveToMarkCompensatedPosition()
    {
        BrowLib.Controller.MotionAPI.LinXyMoveA(
            Global.RunXYVel,
            Global.RunXYAcc,
            _context.MarkCompensatedX,
            _context.MarkCompensatedY,
            1,
            2
        );
        LogInfo($"移动到Mark补偿位置: ({_context.MarkCompensatedX:F3}, {_context.MarkCompensatedY:F3})");
        return MccdStep.WaitMovementComplete;
    }

    /// <summary>
    /// 等待移动完成
    /// </summary>
    private async Task<MccdStep> ExecuteWaitMovementCompleteAsync()
    {
        if (await CheckLinearMotionCompleteAsync())
        {
            _context.CcdHeight = Global.Systemdata.Ccdhight;
            LogInfo($"设置CCD高度: {_context.CcdHeight}");
            return MccdStep.MoveZToCcdHeight;
        }
        return MccdStep.WaitMovementComplete;
    }

    /// <summary>
    /// Z轴移动到CCD高度
    /// </summary>
    private MccdStep ExecuteMoveZToCcdHeight()
    {
        Global.Z轴.PMove(20, 1000, _context.CcdHeight, 1);
        LogInfo($"Z轴移动到CCD高度: {_context.CcdHeight}");
        return MccdStep.WaitCcdHeight;
    }

    /// <summary>
    /// 等待CCD高度到位
    /// </summary>
    private async Task<MccdStep> ExecuteWaitCcdHeightAsync()
    {
        if (await CheckAxisPositionAsync("Z轴", _context.CcdHeight, POSITION_TOLERANCE))
        {
            await Task.Delay(MOTION_DELAY_MS);
            return MccdStep.CalibrationDialog;
        }
        return MccdStep.WaitCcdHeight;
    }

    /// <summary>
    /// 校准对话框
    /// </summary>
    private MccdStep ExecuteCalibrationDialog()
    {
        try
        {
            CalibFrom calibrationForm = new CalibFrom
            {
                Code = ComponentCode,
                Angle = _context.OriginalRotation,
                offsetX = _context.OffsetX,
                offsetY = _context.OffsetY
            };

            calibrationForm.ShowDialog();

            _context.CalibrationOffsetX = calibrationForm.Dx;
            _context.CalibrationOffsetY = calibrationForm.Dy;
            _context.CalibratedRotation = new Algorithm().GetAngle(calibrationForm.Angle);

            if (calibrationForm.RunFlag)
            {
                LogInfo($"校准完成 - Dx: {_context.CalibrationOffsetX:F3}, Dy: {_context.CalibrationOffsetY:F3}, " +
                       $"角度: {_context.CalibratedRotation:F2}°");
                return MccdStep.CalculateAndMoveToFinalPosition;
            }
            else
            {
                Global.TcpClass.Send("M:CCDSTOP");
                LogInfo("用户取消校准");
                StopProcess("用户取消校准");
                return MccdStep.ErrorRecovery;
            }
        }
        catch (Exception ex)
        {
            LogError($"校准对话框错误: {ex.Message}", ex);
            return MccdStep.ErrorRecovery;
        }
    }

    /// <summary>
    /// 计算最终位置并移动
    /// </summary>
    private MccdStep ExecuteCalculateAndMoveToFinalPosition()
    {
        // 获取相机偏移
        Global.GetOffset(_context.CalibratedRotation, out double cameraOffsetX, out double cameraOffsetY, Global.Is_DownCam);

        LogInfo($"角度: {_context.CalibratedRotation:F2}°, 旋转偏移: ({cameraOffsetX:F3}, {cameraOffsetY:F3})");

        // 计算最终偏移
        double finalOffsetX = cameraOffsetX + _context.CalibrationOffsetX;
        double finalOffsetY = cameraOffsetY + _context.CalibrationOffsetY;

        // 计算最终位置
        double finalX = _context.MarkCompensatedX + finalOffsetX;
        double finalY = _context.MarkCompensatedY + finalOffsetY;

        LogInfo($"最终目标位置: ({finalX:F3}, {finalY:F3})");

        // 坐标补偿
        var compensatedPos = Global.coordinateCompensator.GetCompensatedPosition(finalX, finalY);

        LogInfo($"补偿后位置: ({compensatedPos.Item1:F3}, {compensatedPos.Item2:F3})");

        // 并行移动所有轴
        MoveAllAxesToFinalPosition(
            compensatedPos.Item1,
            compensatedPos.Item2,
            _context.CalibratedRotation,
            _context.LeftPosition
        );

        return MccdStep.WaitAllAxesReady;
    }

    /// <summary>
    /// 等待所有轴到位
    /// </summary>
    private async Task<MccdStep> ExecuteWaitAllAxesReadyAsync()
    {
        if (await CheckAllAxesReadyAsync())
        {
            _context.TestHeight += Global.Hoffset;
            LogInfo($"下针高度补偿: {Global.Hoffset}, 最终测试高度: {_context.TestHeight}");
            return MccdStep.ZAxisFastApproach;
        }
        return MccdStep.WaitAllAxesReady;
    }

    /// <summary>
    /// Z轴快速下针
    /// </summary>
    private MccdStep ExecuteZAxisFastApproach()
    {
        double fastApproachHeight = _context.TestHeight - 2;
        Global.Z轴.PMove(300, 3000, fastApproachHeight, 1);
        LogInfo($"Z轴快速下针到: {fastApproachHeight}");
        return MccdStep.ZAxisPrecisePositioning;
    }

    /// <summary>
    /// Z轴精确定位
    /// </summary>
    private async Task<MccdStep> ExecuteZAxisPrecisePositioningAsync()
    {
        double fastApproachHeight = _context.TestHeight - 2;

        if (await CheckAxisPositionAsync("Z轴", fastApproachHeight, POSITION_TOLERANCE))
        {
            Global.Z轴.PMove(
                Global.Systemdata.buf_Zspeed,
                500,
                _context.TestHeight,
                1
            );
            LogInfo($"Z轴精确定位到: {_context.TestHeight}");
            return MccdStep.ClampOrTest;
        }
        return MccdStep.ZAxisPrecisePositioning;
    }

    /// <summary>
    /// 夹紧或测试
    /// </summary>
    private MccdStep ExecuteClampOrTest()
    {
        if (IsZAxisAtPosition(_context.TestHeight))
        {
            if (_context.ClampSize > 0 && _context.ClampSize < 1)
            {
                LogInfo($"执行夹紧动作，尺寸: {_context.ClampSize}");
                ClampJaws(_context.ClampSize);
                return MccdStep.WaitClampComplete;
            }
            else
            {
                Thread.Sleep(TEST_DELAY_MS);
                SendTestCompleteSignal();
                return MccdStep.WaitTestConfirmation;
            }
        }
        return MccdStep.ClampOrTest;
    }

    /// <summary>
    /// 等待夹紧完成
    /// </summary>
    private async Task<MccdStep> ExecuteWaitClampCompleteAsync()
    {
        if (await CheckAxisRunningAsync("左夹爪轴") && await CheckAxisRunningAsync("右夹爪轴"))
        {
            Thread.Sleep(TEST_DELAY_MS);
            SendTestCompleteSignal();
            return MccdStep.WaitTestConfirmation;
        }
        return MccdStep.WaitClampComplete;
    }

    /// <summary>
    /// 等待测试确认
    /// </summary>
    private async Task<MccdStep> ExecuteWaitTestConfirmationAsync()
    {
        if (Global.TcpClass.TCP_MStaart)
        {
            string response = Global.TcpClass.TCP_Result.Trim();
            string[] parts = response.Split(':');

            LogInfo($"收到测试确认: {response}");

            if (parts[0] == "M" && parts[1] == "TestOk")
            {
                // 等待测试延迟
                await Task.Delay(Global.Systemdata.TestDlay);

                // 如果夹爪需要张开
                if (_context.ClampSize > 0 && _context.ClampSize < 1)
                {
                    LogInfo($"执行张开动作，尺寸: {_context.ClampSize}");
                    OpenJaws(_context.ClampSize);
                }

                return MccdStep.WaitAxesReturn;
            }
        }
        return MccdStep.WaitTestConfirmation;
    }

    /// <summary>
    /// 等待轴归位
    /// </summary>
    private async Task<MccdStep> ExecuteWaitAxesReturnAsync()
    {
        if (await CheckLinearMotionCompleteAsync())
        {
            if (await CheckAxisRunningAsync("R轴") &&
                await CheckAxisRunningAsync("左夹爪轴") &&
                await CheckAxisRunningAsync("右夹爪轴"))
            {
                return MccdStep.ReturnZToSafeHeight;
            }
        }
        return MccdStep.WaitAxesReturn;
    }

    /// <summary>
    /// Z轴回安全高度
    /// </summary>
    private MccdStep ExecuteReturnZToSafeHeight()
    {
        Global.Z轴.PMove(100, 1000, Global.Systemdata.SafeHigh, 1);
        LogInfo($"Z轴回安全高度: {Global.Systemdata.SafeHigh}");
        return MccdStep.WaitSafeHeight;
    }

    /// <summary>
    /// 等待安全高度到位
    /// </summary>
    private async Task<MccdStep> ExecuteWaitSafeHeightAsync()
    {
        if (await CheckAxisPositionAsync("Z轴", Global.Systemdata.SafeHigh, POSITION_TOLERANCE))
        {
            LogInfo("MCCD定位流程完成");
            CompleteProcess();
            return MccdStep.ErrorRecovery;
        }
        return MccdStep.WaitSafeHeight;
    }

    /// <summary>
    /// 错误恢复
    /// </summary>
    private async Task<MccdStep> ExecuteErrorRecoveryAsync()
    {
        LogInfo("执行错误恢复流程");

        // 安全停止所有运动
        SafetyStopAllMotions();

        await Task.Delay(1000);

        // 询问用户是否继续
        int result = APP.Tip.ShowTip(1, "错误恢复".tr(), "MCCD定位流程遇到错误，是否重新开始？".tr(), "是".tr(), "否".tr());

        if (result == 1)
        {
            LogInfo("用户选择重新开始");
            Reset();
            return MccdStep.SwitchLineMode;
        }
        else
        {
            LogInfo("用户选择停止流程");
            Stop();
            return MccdStep.ErrorRecovery;
        }
    }

    #endregion

    #region 工具方法

    /// <summary>
    /// 并行移动所有轴到最终位置
    /// </summary>
    private void MoveAllAxesToFinalPosition(double x, double y, double rotation, double leftPosition)
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
        Global.左夹爪轴.PMove(10, 500, leftPosition, 1);
        Global.右夹爪轴.PMove(10, 500, leftPosition, 1);

        LogInfo($"移动所有轴 - 位置: ({x:F3}, {y:F3}), 角度: {rotation:F2}°, 开口: {leftPosition}");
    }

    /// <summary>
    /// 夹紧夹爪
    /// </summary>
    private void ClampJaws(double clampSize)
    {
        Global.左夹爪轴.PMove(10, 500, clampSize, 0);
        Global.右夹爪轴.PMove(10, 500, clampSize, 0);
    }

    /// <summary>
    /// 张开夹爪
    /// </summary>
    private void OpenJaws(double clampSize)
    {
        Global.左夹爪轴.PMove(10, 500, -clampSize, 0);
        Global.右夹爪轴.PMove(10, 500, -clampSize, 0);
    }

    /// <summary>
    /// 发送测试完成信号
    /// </summary>
    private void SendTestCompleteSignal()
    {
        Global.TcpClass.TCP_MStaart = false;
        Global.TcpClass.Send($"M:GoToOk:{_context.CalibrationOffsetX}:{_context.CalibrationOffsetY}\r\n");
        LogInfo($"发送测试完成信号，偏移: ({_context.CalibrationOffsetX:F3}, {_context.CalibrationOffsetY:F3})");
    }

    /// <summary>
    /// 完成流程
    /// </summary>
    private void CompleteProcess()
    {
        Global.StopFlag = true;
        IsRunning = false;
        LogInfo("MCCD定位流程正常完成");
    }

    /// <summary>
    /// 停止流程
    /// </summary>
    private void StopProcess(string reason)
    {
        Global.StopFlag = true;
        Global.TcpClass.Send("M:Stop");
        IsRunning = false;
        LogInfo($"MCCD定位流程停止: {reason}");
    }

    /// <summary>
    /// 处理流程错误
    /// </summary>
    private void HandleProcessError(Exception ex)
    {
        LogError($"流程执行错误 - 步骤 {CurrentStep}: {ex.Message}\n{ex.StackTrace}", ex);
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
            Global.R轴.AxisStop();
            Global.左夹爪轴.AxisStop();
            Global.右夹爪轴.AxisStop();
            BrowLib.Controller.CardAPI.StopAxis();
        }
        catch (Exception ex)
        {
            LogError($"安全停止时出错: {ex.Message}",ex);
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

    /// <summary>
    /// 检查轴是否在运行
    /// </summary>
    private async Task<bool> CheckAxisRunningAsync(string axisName)
    {
        return await Task.Run(() => !BrowLib.Controller.Motion[axisName].Runing());
    }

    /// <summary>
    /// 检查所有轴是否就绪
    /// </summary>
    private async Task<bool> CheckAllAxesReadyAsync()
    {
        if (!await CheckLinearMotionCompleteAsync())
            return false;

        if (!await CheckAxisPositionAsync("R轴", _context.CalibratedRotation, POSITION_TOLERANCE))
            return false;

        if (!await CheckAxisPositionAsync("左夹爪轴", _context.LeftPosition, POSITION_TOLERANCE))
            return false;

        if (!await CheckAxisPositionAsync("右夹爪轴", _context.LeftPosition, POSITION_TOLERANCE))
            return false;

        return true;
    }

    /// <summary>
    /// 检查Z轴是否在指定位置
    /// </summary>
    private bool IsZAxisAtPosition(double targetHeight)
    {
        double currentPos = Global.Z轴.GetPrfPos();
        return Math.Abs(currentPos - targetHeight) < POSITION_TOLERANCE;
    }

    #endregion

    #region 日志和提示

    /// <summary>
    /// 记录信息日志
    /// </summary>
    private void LogInfo(string message)
    {
        APP.Log.I_Log($"[MccdPositioning] {message}");
    }

    /// <summary>
    /// 记录错误日志
    /// </summary>
    private void LogError(string message, Exception exception)
    {
        APP.Log.E_Log($"[MccdPositioning] {message}", exception);
    }

    /// <summary>
    /// 记录警告日志
    /// </summary>
    private void LogWarning(string message)
    {
        APP.Log.W_Log($"[MccdPositioning] {message}");
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
    /// MCCD定位流程上下文数据
    /// </summary>
    private class MccdContext
    {
        /// <summary>
        /// 原始X坐标
        /// </summary>
        public double OriginalX { get; set; }

        /// <summary>
        /// 原始Y坐标
        /// </summary>
        public double OriginalY { get; set; }

        /// <summary>
        /// 原始旋转角度
        /// </summary>
        public double OriginalRotation { get; set; }

        /// <summary>
        /// X坐标调整
        /// </summary>
        public double OffsetX { get; set; }

        /// <summary>
        /// Y坐标调整
        /// </summary>
        public double OffsetY { get; set; }

        /// <summary>
        /// 组件类型
        /// </summary>
        public string ComponentType { get; set; }

        /// <summary>
        /// 变换后的X坐标
        /// </summary>
        public double TransformedX { get; set; }

        /// <summary>
        /// 变换后的Y坐标
        /// </summary>
        public double TransformedY { get; set; }

        /// <summary>
        /// 左侧位置
        /// </summary>
        public double LeftPosition { get; set; }

        /// <summary>
        /// 测试高度
        /// </summary>
        public double TestHeight { get; set; }

        /// <summary>
        /// 夹紧尺寸
        /// </summary>
        public double ClampSize { get; set; }

        /// <summary>
        /// Mark补偿后的X坐标
        /// </summary>
        public double MarkCompensatedX { get; set; }

        /// <summary>
        /// Mark补偿后的Y坐标
        /// </summary>
        public double MarkCompensatedY { get; set; }

        /// <summary>
        /// CCD高度
        /// </summary>
        public double CcdHeight { get; set; }

        /// <summary>
        /// 校准偏移X
        /// </summary>
        public double CalibrationOffsetX { get; set; }

        /// <summary>
        /// 校准偏移Y
        /// </summary>
        public double CalibrationOffsetY { get; set; }

        /// <summary>
        /// 校准后的旋转角度
        /// </summary>
        public double CalibratedRotation { get; set; }

        /// <summary>
        /// 重置上下文
        /// </summary>
        public void Reset()
        {
            OriginalX = OriginalY = OriginalRotation = 0;
            OffsetX = OffsetY = 0;
            ComponentType = string.Empty;
            TransformedX = TransformedY = 0;
            LeftPosition = TestHeight = ClampSize = 0;
            MarkCompensatedX = MarkCompensatedY = 0;
            CcdHeight = 0;
            CalibrationOffsetX = CalibrationOffsetY = 0;
            CalibratedRotation = 0;
        }
    }

    #endregion
}