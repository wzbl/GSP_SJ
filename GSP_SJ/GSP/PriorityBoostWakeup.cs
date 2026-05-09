using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GSP
{
    public class PriorityBoostWakeup
    {
        // 1. 最高优先级唤醒（立即生效）
        public static void WakeupImmediate()
        {
            var process = Process.GetCurrentProcess();

            // 保存原始优先级
            var originalPriority = process.PriorityClass;

            try
            {
                // 提升到实时优先级（最高）
                process.PriorityClass = ProcessPriorityClass.RealTime;

                // 强制CPU调度
                Thread.Yield();

                // 立即发送关键消息
                SendCriticalMessages();

                // 短暂保持高优先级
                Thread.Sleep(10);

                // 恢复正常优先级
                process.PriorityClass = originalPriority;
            }
            catch
            {
                try { process.PriorityClass = originalPriority; } catch { }
            }
        }

        // 2. 线程优先级提升（更精确）
        public static void BoostThreadPriority()
        {
            // 获取并提升所有线程优先级
            var process = Process.GetCurrentProcess();

            foreach (ProcessThread thread in process.Threads)
            {
                try
                {
                    if (thread.ThreadState == System.Diagnostics.ThreadState.Wait &&
                        thread.WaitReason == ThreadWaitReason.Suspended)
                    {
                        // 恢复挂起的线程
                        ResumeThread(thread.Id);
                    }

                    // 提升优先级
                    thread.PriorityLevel = ThreadPriorityLevel.TimeCritical;
                }
                catch { }
            }
        }

        [DllImport("kernel32.dll")]
        private static extern uint ResumeThread(int threadId);

        private static void SendCriticalMessages()
        {
            IntPtr mainHandle = Process.GetCurrentProcess().MainWindowHandle;
            if (mainHandle != IntPtr.Zero)
            {
                // 发送最高优先级的系统消息
                const uint WM_QUIT = 0x0012;
                const uint WM_DESTROY = 0x0002;
                const uint WM_CLOSE = 0x0010;
                const uint WM_NULL = 0x0000;

                // 立即发送，不排队
                for (int i = 0; i < 10; i++)
                {
                    SendMessage(mainHandle, WM_NULL, IntPtr.Zero, IntPtr.Zero);
                }

                // 发送激活消息
                SendMessage(mainHandle, 0x0006, (IntPtr)1, IntPtr.Zero); // WM_ACTIVATE
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg,
            IntPtr wParam, IntPtr lParam);
    }
}
