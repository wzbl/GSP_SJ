using BrowApp.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GSP.Flow
{
    #region 流程管理器
    /// <summary>
    /// 自动运行流程管理器
    /// </summary>
    public class AutoRunManager
    {
        private readonly Dictionary<int, BaseStep> _steps = new Dictionary<int, BaseStep>();
        private readonly StepContext _context = new StepContext();
        //private readonly IHardwareController _hardware;
        //private readonly IVisionController _vision;

        public AutoRunManager()
        {
            //// 初始化控制器
            //_hardware = new BrowHardwareController();
            //_vision = new VisionController();

            // 注册所有步骤
            RegisterSteps();
        }

        /// <summary>
        /// 注册步骤
        /// </summary>
        private void RegisterSteps()
        {
            //RegisterStep(new Step0_InitialCheck(_hardware, _vision));
            //RegisterStep(new Step1_EntrySensorCheck(_hardware, _vision));
            //RegisterStep(new Step2_StartBoardIn(_hardware, _vision));
            //RegisterStep(new Step3_BoardInPositionCheck(_hardware, _vision));
            //RegisterStep(new Step4_AdjustWidth(_hardware, _vision));
            //RegisterStep(new Step5_AdjustWidthCheck(_hardware, _vision));
            //RegisterStep(new Step6_LiftCylinderUp(_hardware, _vision));
            //RegisterStep(new Step7_LiftPositionCheck(_hardware, _vision));
            //RegisterStep(new Step8_SetLightAndZPosition(_hardware, _vision));
            //RegisterStep(new Step9_ZAxisCheckAndRReset(_hardware, _vision));
            //RegisterStep(new Step10_RAxisResetCheck(_hardware, _vision));
            //RegisterStep(new Step11_MarkPositionMove(_hardware, _vision));
            //RegisterStep(new Step12_MarkPositionCheck(_hardware, _vision));
            //RegisterStep(new Step13_ZAxisToMarkHeight(_hardware, _vision));
            //RegisterStep(new Step14_ZAxisMarkHeightCheck(_hardware, _vision));
            //RegisterStep(new Step15_MarkVisionCapture(_hardware, _vision));
            //RegisterStep(new Step16_MarkVisionResult(_hardware, _vision));
            //RegisterStep(new Step17_MarkRectifyCalculate(_hardware, _vision));
            //RegisterStep(new Step18_RunModeSelect(_hardware, _vision));

            // 注册其他步骤（根据需要补充）
            // RegisterStep(new Step19_TcpSend(_hardware, _vision));
            // RegisterStep(new Step20_TcpReceive(_hardware, _vision));
        }

        /// <summary>
        /// 注册单个步骤
        /// </summary>
        private void RegisterStep(BaseStep step)
        {
            if (_steps.ContainsKey(step.StepId))
            {
                throw new ArgumentException($"步骤{step.StepId}已注册");
            }
            _steps[step.StepId] = step;
        }
        /// <summary>
        /// 启动自动运行
        /// </summary>
        public void Start()
        {
            _context.Log("===== 自动检测流程启动 =====");
            _context.CurrentStep = 0;

            while (!_context.StopFlag)
            {
                try
                {
                    //// 检查暂停（忽略特殊步骤）
                    //if (_context.PauseFlag && !GetCurrentStep().IgnorePause)? false)
                    //{
                    //    Thread.Sleep(10);
                    //    continue;
                    //}

                    // 执行当前步骤
                    var nextStep = ExecuteCurrentStep();

                    // 处理执行结果
                    if (nextStep == -1)
                    {
                        _context.Log("流程终止");
                        break;
                    }

                    if (nextStep != _context.CurrentStep)
                    {
                        _context.Log($"步骤切换：{_context.CurrentStep} -> {nextStep}");
                        _context.CurrentStep = nextStep;
                    }

                    Thread.Sleep(5);
                }
                catch (Exception ex)
                {
                    _context.Log($"步骤{_context.CurrentStep}执行异常：{ex.Message}");
                    _context.ShowWarning("错误", $"步骤{_context.CurrentStep}执行失败：{ex.Message}", "确定".tr());
                    Global.StopFlag = true;
                }
            }

            _context.Log("===== 自动检测流程结束 =====");
            Reset();
        }

        /// <summary>
        /// 执行当前步骤
        /// </summary>
        private int ExecuteCurrentStep()
        {
            if (!_steps.TryGetValue(_context.CurrentStep, out var step))
            {
                _context.Log($"未找到步骤{_context.CurrentStep}的实现");
                Global.StopFlag = true;
                return -1;
            }

            return step.Execute(_context);
        }

        /// <summary>
        /// 获取当前步骤
        /// </summary>
        private BaseStep GetCurrentStep()
        {
            _steps.TryGetValue(_context.CurrentStep, out var step);
            return step;
        }

        /// <summary>
        /// 重置流程
        /// </summary>
        public void Reset()
        {
            _context.CurrentStep = 0;
            _context.MakNum = 0;
            _context.Mode = 0;
            _context.Dx = 0;
            _context.Dy = 0;
            Global.StopFlag = false;
            Global.PauseFlag = false;
        }

        /// <summary>
        /// 调试：跳转到指定步骤
        /// </summary>
        /// <param name="stepId">步骤ID</param>
        public void GotoStep(int stepId)
        {
            if (_steps.ContainsKey(stepId))
            {
                _context.Log($"调试跳转：{_context.CurrentStep} -> {stepId}");
                _context.CurrentStep = stepId;
            }
            else
            {
                _context.Log($"调试跳转失败：步骤{stepId}不存在");
            }
        }
    }
    #endregion

}
