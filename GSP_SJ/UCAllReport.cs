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
    public partial class UCAllReport : UserControl
    {
        public UCAllReport()
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
            this.Load += UCAllReport_Load;
        }

        private void UCAllReport_Load(object sender, EventArgs e)
        {
            Search();
        }
        List<Man_Report> reports;
        private void Search()
        {
            reports = SQLDataControl.GetAllReport();
            dgvProgram.DataSource = reports;
        }
    }
}
