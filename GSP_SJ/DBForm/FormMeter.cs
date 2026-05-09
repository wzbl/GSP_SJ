using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace GSP_SJ
{
    public partial class FormMeter : KryptonForm
    {
        public FormMeter()
        {
            InitializeComponent();
        }

        public string NewName = "";

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show("请输入名称");
                return;
            }
            NewName = txtName.Text;
            DialogResult = DialogResult.OK;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
