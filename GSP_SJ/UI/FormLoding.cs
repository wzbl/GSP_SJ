using ComponentFactory.Krypton.Toolkit;
using GSP_SJ.DBForm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSP_SJ.UI
{
    public partial class FormLoding : KryptonForm
    {
        public FormLoding()
        {
            InitializeComponent();
            this.Load += FormLoding_Load;
        }

        public bool CloseFlg = false;
        private void FormLoding_Load(object sender, EventArgs e)
        {
            TopMost = true;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (CloseFlg)
            {
                this.Close();
            }
        }
    }
}
