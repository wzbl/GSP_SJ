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

namespace GSP_SJ
{
    public partial class FormCompensationSet : Form
    {
        public FormCompensationSet()
        {
            InitializeComponent();
            this.Load += FormCompensationParam_Load;
        }

        private void FormCompensationParam_Load(object sender, EventArgs e)
        {
            Search();
        }

        private void Search()
        {
            dgvProgram.DataSource = SQLDataControl.View_Bas_CompensationValue();
        }
    }
}
