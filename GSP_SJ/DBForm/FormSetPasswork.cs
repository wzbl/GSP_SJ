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
                    MessageBox.Show("两次密码不一致");
                }
            }
            else
            {
                if (SQLDataControl.ChangePassword(userCode, txtOld.Text, txtNew.Text, out string msg))
                {
                    MessageBox.Show("密码更新完成");
                    this.Close();
                }
                else
                {
                    MessageBox.Show(msg);
                }
            }
        }
    }
}
