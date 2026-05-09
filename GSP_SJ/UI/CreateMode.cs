using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSP.UI
{
    public partial class CreateMode : Form
    {
        public CreateMode()
        {
            InitializeComponent();
            this.Load += CreateMode_Load;
            this.Header.MouseDown += TopBar_MouseDown;
            this.Header.MouseMove += TopBar_MouseMove;
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
        public string TaskName { get; set; }
        private void CreateMode_Load(object sender, EventArgs e)
        {
            Global.VisionApp.ShowQuick(this.Proc_pic.Handle, TaskName, this.Proc_pic.Width, this.Proc_pic.Height);
            Global.VisionApp.ExecuteProc(TaskName, 0);
            BrowApp.Language.Language.Instance.UpdateLanguage(this, null);
        }

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        private void Rectify_Btn_Click(object sender, EventArgs e)
        {
           double dx =Global.VisionApp.GetDblValue("Task8", "定义变量", 2108, 0);
           double dy = Global.VisionApp.GetDblValue("Task8", "定义变量", 2109,0);
           Global.Light.SetRgbLight(Global.Systemdata.P_LED.LED_R, Global.Systemdata.P_LED.LED_G, Global.Systemdata.P_LED.LED_B);
           double X = Global.X轴.GetPrfPos();
           double Y = Global.Y轴.GetPrfPos();
           new HandFlow().SafeMoveXyz(0, X - dx, Y - dy, Global.CamHeight);
           Global.VisionApp.ExecuteProc("Task8", 0);
        }
    }
}
