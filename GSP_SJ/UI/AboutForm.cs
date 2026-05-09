using BrowApp.Language;
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
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
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
        private void AboutForm_Load(object sender, EventArgs e)
        {
            Vision_lab.Values.ExtraText = Global.Vosion;
            this.Mode_lab.Values.ExtraText = "机型:".tr() + Global.sModel + ":" + "激光型号".tr() + ":" + Global.sLaserType;
            BrowApp.Language.Language.Instance.UpdateLanguage(this, null);
        }

        private void Close_btn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
