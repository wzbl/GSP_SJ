using BrowApp;
using BrowLib;
using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GSP.Flow
{
    public enum ProcessState
    {
        Idle,
        Running,
        Paused,
        Stopped,
        Error
    }

    public enum StepState
    {
        Ready,
        Executing,
        Completed,
        Failed,
        Timeout
    }

    /// <summary>
    /// 步骤执行上下文
    /// 用于在各个步骤间共享数据，替代原方法中的局部变量
    /// </summary>
    public class StepContext
    {
        // 步骤控制
        public int CurrentStep { get; set; }
        public bool StopFlag => Global.StopFlag;
        public bool PauseFlag => Global.PauseFlag;
        public CancellationTokenSource Cts { get; } = new CancellationTokenSource(); // 异步取消令牌

        // 运动控制相关
        public double X { get; set; }=0;
        public double Y { get; set; } = 0;
        public double R { get; set; }
        public double X_offset { get; set; }
        public double Y_offset { get; set; }
        public double ZHight { get; set; }
        public double Wx { get; set; } = 0;
        public double Wy { get; set; } = 0;
        public double Dx { get; set; }
        public double Dy { get; set; }

        // Mark点相关
        public double DMak1_X { get; set; }
        public double DMak1_Y { get; set; }
        public double DMak2_X { get; set; }
        public double DMak2_Y { get; set; }
        public int MakNum { get; set; }

        // 模式控制
        public int Mode { get; set; }
        public string StrCode { get; set; }
        public string Type { get; set; }

        // 计时器
        public KTimer GlobalTimer { get; } = new KTimer();
        public Stopwatch CycleTimer { get; } = new Stopwatch();
        
        public double OffsetX { get; internal set; }
        public double OffsetY { get; internal set; }
        public double Size { get; internal set; }
        public double JqSize { get; internal set; }
        public double CycleTime { get; internal set; }

        // 日志记录
        public void Log(string message)
        {
            APP.Log.I_Log($"[Step{CurrentStep}] {message}");
        }

        // 提示框封装
        public int ShowWarning(string title, string message, params string[] buttons)
        {
            return APP.Tip.ShowTip(1, title, message, buttons);
        }
        // 获取时间
        public double GetTime(long ticks) => ticks / (double)Stopwatch.Frequency;
        // 停止流程封装
        public void StopProcess(string reason)
        {
            Log($"流程停止: {reason}");
            Global.StopFlag = true;
            Global.MachineState = GEnumEx.MachineState.MachineStop;
            Global.SystemRun = false;
            Global.TcpClass.Send("M:Stop");
        }
        /// <summary>
        /// 异步等待暂停恢复（核心：不阻塞线程）
        /// </summary>
        public async Task WaitForResumeAsync(CancellationToken token)
        {
            while (PauseFlag && !token.IsCancellationRequested)
            {
                await Task.Delay(100, token); // 100ms检查一次，平衡CPU和实时性
            }
        }
    }

    /// <summary>
    /// 步骤基类
    /// 所有具体步骤都继承此类，实现统一的执行接口
    /// </summary>
    public abstract class BaseStep
    {
        /// <summary>
        /// 步骤编号
        /// </summary>
        public abstract int StepId { get; }
        /// <summary>
        /// 步骤名称
        /// </summary>
        public virtual string StepName { get; } 
        /// <summary>
        /// 是否忽略暂停（对应原代码中的特殊步骤）
        /// </summary>
        public virtual bool IgnorePause => false;
        /// <summary>
        /// 执行上下文（子类可直接访问）
        /// </summary>
        protected StepContext context;
        /// <summary>
        /// 步骤是否完成
        /// </summary>
        public virtual bool IsCompleted { get; }
        /// <summary>是否超时</summary>
        bool IsTimeout { get; }

        /// <summary>超时时间 ms</summary>
        int Timeout { get; }

        /// <summary>下一步</summary>
        BaseStep NextStep { get; }
        /// <summary>
        /// 异步撤销/回滚步骤
        /// </summary>
        public virtual void Undo() { }

        protected StepState CurrentStepState = StepState.Ready;
        /// <summary>
        /// 异步执行步骤（核心方法）
        /// </summary>
        /// <param name="context">执行上下文</param>
        /// <returns>下一个步骤ID（-1表示停止）</returns>
        public int Tick(StepContext context)
        {
            this.context = context;
            try
            {
                return Execute(context);
            }
            catch (OperationCanceledException)
            {
                context.Log($"步骤{StepId}执行被取消");
                return -1;
            }
            catch (Exception ex)
            {
                context.Log($"步骤{StepId}执行异常: {ex.Message}\n{ex.StackTrace}");
                return -1;
            }
        }
        /// <summary>
        /// 具体步骤逻辑（子类实现）
        /// </summary>
        public abstract int Execute(StepContext context);
        /// <summary>
        /// 单次条件检查
        /// </summary>
        protected bool CheckDone(Func<bool> condition)
        {
            if (CurrentStepState == StepState.Ready)
            {
                context.Log($"[{StepId}] {StepName} 开始");
                context.GlobalTimer.Restart();
                CurrentStepState = StepState.Executing;
            }
            return condition();
        }
        /// <summary>
        /// 检查轴是否到达目标位置
        /// </summary>
        protected bool CheckAxisPosition(string axisName, double targetPos, double tolerance)
        {
            double currentPos = BrowLib.Controller.Motion[axisName].GetPrfPos();
            if (targetPos < 0) return BrowLib.Controller.Motion[axisName].Runing();
            return Math.Abs(currentPos - targetPos) < tolerance && BrowLib.Controller.Motion[axisName].Runing();
        }
        /// <summary>
        /// 超时判断
        /// </summary>
        protected bool CheckTimeout(int timeoutMs)
        {
            if(context.GlobalTimer.IsOn(timeoutMs))
            {
                context.Log($"[{StepId}] {StepName} 超时({timeoutMs}ms)");
                return true;
            }
            return false;
        }
        /// <summary>
        /// 检查是否需要停止执行
        /// </summary>
        protected bool CheckStop(StepContext context)
        {
            if (context.StopFlag)
            {
                context.Log("检测到停止标志，终止步骤执行");
                return true;
            }
            return false;
        }
        /// <summary>重置步骤</summary>
        public virtual void Reset(){ }
    }

}
