using BrowApp.Language;
using ComponentFactory.Krypton.Toolkit;
using SqlHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSP_SJ.DBForm
{
    public partial class FormSetPasswork : KryptonForm
    {
        public FormSetPasswork()
        {
            InitializeComponent();
            UpdateLanguage();
        }
        private void UpdateLanguage()
        {
            BrowApp.Language.Language.Instance.UpdateLanguage(this, null);
        }
        int type = 0;
        public string password = "";
        public string userCode = "";
        public FormSetPasswork(string userCode, int i = 0) : this()
        {
            if (i != 0)
            {
                txtOld.Enabled = false;
                //添加用户

            }
            else
            {
                //修改密码
            }
            type = i;
            this.userCode = userCode;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (type != 0)
            {
                if (txtNew.Text == txtCheck.Text)
                {
                    DialogResult = DialogResult.OK;
                    password = txtNew.Text;
                }
                else
                {
                    BrowApp.APP.Tip.ShowTip(1, "警告".tr(), "两次密码不一致".tr(), "确定".tr());
                }
            }
            else
            {
                if (SQLDataControl.ChangePassword(userCode, txtOld.Text, txtNew.Text, out string msg))
                {
                    BrowApp.APP.Tip.ShowTip(0, "提示".tr(), "密码更新完成".tr(), "确定".tr());
                    this.Close();
                }
                else
                {
                    BrowApp.APP.Tip.ShowTip(1, "警告".tr(), msg, "确定".tr());
                }
            }
        }
    }
}
