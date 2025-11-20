using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSP_SJ
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            this.Shown += FormMain_Shown;
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            kryptonPage1.Controls.Add(new UCAllProgram());
            kryptonPage2.Controls.Add(new UCAllReport());
        }
    }
}
