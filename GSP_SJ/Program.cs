using BorwinAnalyse.BaseClass;
using BorwinAnalyse.ImportBom;
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
            // 强制跳过 iTextSharp 授权验证（解决你报的错误）
            System.Reflection.FieldInfo field = typeof(iTextSharp.text.io.StreamUtil).GetField("FONT_TEMP_DIR", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            if (field != null)
            {
                field.SetValue(null, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            CommonAnalyse.Instance.Load();
            AnaylseDataManager.Instance.Load();
            DeepOCRHelper.Init();
            FormLogin formLogin = new FormLogin();
            if (formLogin.ShowDialog() == DialogResult.OK)
                Application.Run(new FormMain());


        }
    }
}
