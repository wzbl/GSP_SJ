using BrowApp;
using BrowApp.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace GSP.UI
{
    public partial class HomeStart : Form
    {
        FlowControl flow = new FlowControl();
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        public HomeStart()
        {
            InitializeComponent();

            this.Header.MouseDown += TopBar_MouseDown;
            this.Header.MouseMove += TopBar_MouseMove;

            timer.Interval = 100;
            timer.Tick += timer1_Tick;
            timer.Start();
        }
        #region 点击panel控件移动窗口
        private Point downPoint;
        private void TopBar_MouseDown(object sender, MouseEventArgs e)
        {
            downPoint = new Point(e.X, e.Y);
        }
        private void TopBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Location = new Point(this.Location.X + e.X - downPoint.X,
                    this.Location.Y + e.Y - downPoint.Y);
            }
        }
        #endregion
        public bool IsHomeflg { get; set; }

        private void HomeState_Load(object sender, EventArgs e)
        {
            if (flow.IsManualRun()) return;
            flow.ExecuteManual(flow.Home);
            BrowApp.Language.Language.Instance.UpdateLanguage(this, null);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!flow.IsManualRun())
            {
                timer.Stop();

                if (Global.SystemInitialOk)
                {
                    APP.Log.I_Log("设备回零完成");
                    IsHomeflg = true;
                }
                else
                {
                    APP.Log.I_Log("设备回零未完成? 请检查问题在执行回零操作!!!");
                    IsHomeflg = false;
                }
                this.Close();
                this.Dispose();
            }
        }
        private void Stop_btn_Click(object sender, EventArgs e)
        {
            Global.StopFlag = true;
            BrowLib.Controller.CardAPI.StopAxis();
            Thread.Sleep(100);
            timer.Stop();
            APP.Log.I_Log("设备回零未完成? 请检查问题在执行回零操作");
            IsHomeflg = false;
            this.Close();
            this.Dispose();
        }
    }
}
