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
    public partial class FormSize : Form
    {
        public FormSize()
        {
            InitializeComponent();
            this.Load += FormCompensationParam_Load;
        }

        private void FormCompensationParam_Load(object sender, EventArgs e)
        {
            Search();
        }
        List<View_ComponentSize> view_ComponentSizes = new List<View_ComponentSize>();
        private void Search()
        {
            view_ComponentSizes = SQLDataControl.View_ComponentSize();
            dgvProgram.DataSource = view_ComponentSizes;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            view_ComponentSizes.Add(new View_ComponentSize()
            {
                尺寸 = "",
                截图长度 = 0,
                截图宽度 = 0,
                物理长度 = 0,
                物理宽度 = 0,
                OCR = false,
                备注 = ""
            });
            dgvProgram.DataSource = null;
            dgvProgram.DataSource = view_ComponentSizes;
            dgvProgram.Refresh();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            Search();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {

        }
    }
}
