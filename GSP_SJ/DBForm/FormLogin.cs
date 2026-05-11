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
    public partial class FormLogin : KryptonForm
    {
        public FormLogin()
        {
            InitializeComponent();
            UpdateLanguage();
        }
        private void UpdateLanguage()
        {
            BrowApp.Language.Language.Instance.UpdateLanguage(this, null);
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(SQLDataControl.Login(txtcode.Text,txtpassword.Text,out string msg))
            {
                DBEventAction.User = SQLDataControl.GetUser().Where(x => x.UserCode == txtcode.Text).First();
             
                DialogResult = DialogResult.OK;
            }
            else
            {
                DBEventAction.User = null;
                MessageBox.Show(msg);
            }
               
        }
    }
}
