using BorwinAnalyse.BaseClass;
using BorwinAnalyse.ImportBom;
using BrowApp.Language;
using GSP;
using GSP_SJ.DBForm;
using GSP_SJ.ModelClass;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GSP_SJ
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            #region 异常捕获事件
            //处理未捕获的异常
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            //处理UI线程异常
            Application.ThreadException += new global::System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            //处理非UI线程异常
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(Application_UnhandledException);
            #endregion

            bool createNew;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (global::System.Threading.Mutex mutex = new global::System.Threading.Mutex(true, Application.ProductName, out createNew))
            {
                if (createNew)
                {
                    MiddleLayer.InitialProject();
                   
                    FormLogin formLogin = new FormLogin();
                    if (formLogin.ShowDialog() == DialogResult.OK)
                        new StartForm().ShowDialog();
                    Application.Run(new FormMain());
                }
                else
                {
                     BrowApp.APP.Tip.ShowTip(1, "警告".tr(), "程序已经在运行中,请勿重复打开".tr(), "确定".tr());
                    global::System.Threading.Thread.Sleep(1000);
                    global::System.Environment.Exit(Environment.ExitCode);
                }
            }
        }
        static void Application_ThreadException(object sender, global::System.Threading.ThreadExceptionEventArgs e)
        {
            Application_Error(e.Exception);
        }
        static void Application_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Application_Error((Exception)e.ExceptionObject);
        }
        public static void Application_Error(Exception error)
        {
            BrowApp.APP.Log.E_Log(error.Message, error);
        }
    }
}
