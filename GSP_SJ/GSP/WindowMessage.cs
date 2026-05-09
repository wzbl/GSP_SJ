using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSP
{
    public enum SendMessage : int
    {
        加载配方 = 0X001,
        模式切换_ON = 0X002,
        模式切换_OFF = 0X003,
        Flow_In = 0X004,
        Flow_OUT = 0X005,
        用户登出 = 0X006,
        四线测量 = 0X007,
        二线测量 = 0X008,
        开路清零 = 0X009,
        开路清零就绪 = 0X010,
        开路清零完成 = 0X011,
        短路清零 = 0X012,
        短路清零就绪 = 0X013,
        短路清零完成 = 0X014,
        归零 = 0X015,
        设置宽度 = 0X016,
        TCP连接 = 0X018
    }
    /// <summary>
    /// 传递WindowMessage
    /// </summary>
    public class WindowMessage
    {
        public const int UserMessage = 0x0400;//用户自定义消息的开始数值
        public const int UserMessage2 = 0x004A;//用户自定义消息的开始数值(拼图扫描仪时使用和全自动时使用)

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern int FindWindow(string lpClassName, string lpWindowName);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(
        int hWnd, // handle to destination window
        int Msg, // message
        int wParam, // first message parameter
        ref COPYDATASTRUCT lParam // second message parameter
        );

        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }

        //异步消息发送API
        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        public static extern int PostMessage(
            IntPtr hWnd,        // 信息发往的窗口的句柄
            int Msg,            // 消息ID
            int wParam,         // 参数1
            int lParam            // 参数2
        );

        /// <summary>
        /// 运行指定可执行文件
        /// </summary>
        /// <param name="programPathName">可执行文件的路径(含可执行文件名称和后缀)</param>
        /// <param name="arguments">可执行文件的命令行参数</param>
        /// <returns></returns>
        public bool runProgram(string programPathName, string arguments)
        {
            try
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process();

                p.StartInfo.FileName = @programPathName;
                p.StartInfo.WorkingDirectory = @programPathName;
                if (arguments.Trim().Length > 0)
                    p.StartInfo.Arguments = " " + arguments;
                p.Start();
                //Thread.Sleep(5000);//原本等待十秒
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("发生错误.\r\n" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 运行指定可执行文件
        /// </summary>
        /// <param name="programPath">可执行文件的路径(不含可执行文件名称和后缀)</param>
        /// <param name="programName">可执行文件名称(含后缀)</param>
        /// <param name="arguments">可执行文件的命令行参数</param>
        /// <returns></returns>
        public bool runProgram(string programPath, string programName, string arguments)
        {
            try
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process();

                p.StartInfo.FileName = @programPath + programName;
                p.StartInfo.WorkingDirectory = @programPath + programName;
                if (arguments.Trim().Length > 0)
                    p.StartInfo.Arguments = " " + arguments;
                p.Start();
                //Thread.Sleep(5000);//原本等待十秒
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("发生错误.\r\n" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 查找窗口句柄并向窗体发送Window消息
        /// </summary>
        /// <param name="windowName">窗体名</param>
        /// <param name="message">发送的消息</param>
        /// <param name="isRunProgram">是否提示</param>
        /// <returns></returns>
        public bool findAndSendWindowMessage(string windowName, string message, bool isTip)
        {
            try
            {
                int WINDOW_HANDLER = FindWindow(null, @windowName); //检测窗体是否开启
                if (WINDOW_HANDLER == 0)
                {
                    if (isTip) MessageBox.Show("请检查[" + windowName + "]窗口是否正常打开.", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                byte[] sarr = System.Text.Encoding.Default.GetBytes(message);
                int len = sarr.Length;
                COPYDATASTRUCT cds;
                cds.dwData = (IntPtr)100; //这里可以传入一些自定义的数据，但只能是4字节整数 
                cds.lpData = message; //消息字符串
                cds.cbData = len + 1; //长度(按字节计算)
                SendMessage(WINDOW_HANDLER, UserMessage2, 0, ref cds); //发送
            }
            catch (Exception ex)
            {
                MessageBox.Show("发送Window消息失败. \r\n" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        public bool findAndSendWindowMessage_Process(string processName, string arguments, bool question)
        {
            System.Diagnostics.Process[] processList = System.Diagnostics.Process.GetProcesses();  //系统中所有进程
            foreach (System.Diagnostics.Process process in processList)
            {
                if (process.ProcessName.ToUpper() == processName)
                {
                    if (question)
                    {
                        if (MessageBox.Show("程序已启动,是否强制结束并重新开启？", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            process.Kill(); //结束进程
                            return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 向窗体发送Window消息
        /// </summary>
        /// <param name="WINDOW_HANDLER">窗口句柄</param>
        /// <param name="message">发送的消息</param>
        /// <returns></returns>
        public bool sendWindowMessage(int WINDOW_HANDLER, string message)
        {
            try
            {
                byte[] sarr = System.Text.Encoding.Default.GetBytes(message);
                int len = sarr.Length;
                COPYDATASTRUCT cds;
                cds.dwData = (IntPtr)100; //这里可以传入一些自定义的数据，但只能是4字节整数 
                cds.lpData = message; //消息字符串
                cds.cbData = len + 1; //长度(按字节计算)
                SendMessage(WINDOW_HANDLER, UserMessage2, 0, ref cds); //发送
                int ii = 0;
                ii++;
            }
            catch (Exception ex)
            {
                MessageBox.Show("发送Window消息失败. \r\n" + ex.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 向窗体发送Window消息
        /// </summary>
        /// <param name="WINDOW_HANDLER">窗口句柄</param>
        /// <param name="message">发送的消息</param>
        /// <returns></returns>
        public bool postWindowMessage(int WINDOW_HANDLER, int message, int userMessage)
        {
            try
            {
                PostMessage((IntPtr)WINDOW_HANDLER, userMessage, 0, message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("发送Window消息失败. \r\n" + ex.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 查找窗口句柄并向窗体发送Window消息（用户自定义消息数值）
        /// </summary>
        /// <param name="windowName">窗体名</param>
        /// <param name="message">发送的消息</param>
        /// <param name="userMessage">用户自定义消息的数值</param>
        /// <param name="isTip">是否提示</param>
        /// <returns></returns>
        public bool findAndPostWindowMessage(string windowName, int message, int userMessage, bool isTip)
        {
            try
            {
                int WINDOW_HANDLER = FindWindow(null, @windowName); //检测窗体是否开启
                if (WINDOW_HANDLER == 0)
                {
                    if (isTip) MessageBox.Show("请检查[" + windowName + "]窗口是否正常打开.", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                PostMessage((IntPtr)WINDOW_HANDLER, userMessage, 0, message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("发送Window消息失败. \r\n" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

    }
}
