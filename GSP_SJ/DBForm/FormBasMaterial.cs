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
    public partial class FormBasMaterial : Form
    {
        public FormBasMaterial()
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
            dgvProgram.DataSource = SQLDataControl.View_Bas_Material();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            Search();
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (dgvProgram.SelectedRows.Count > 0)
            {
                string customerCode = "";
                string materialCode = dgvProgram.SelectedRows[0].Cells[0].Value.ToString();
                string materialName = "";
                string lcrType = "";
                decimal lcrStandardValue = 0;
                string lcrUnitCode = "";
                decimal lcrMaxValue = 0;
                decimal lcrMinValue = 0;
                string size = "";
                decimal compensateValue = 0;
                byte[] picture = null;
                string remarks = "";
                string creator = "";
                DateTime creationDate = DateTime.Now;
                string modifier = "";
                DateTime modificationDate = DateTime.Now;
                string screenPrinting = "";
                decimal maxTolerance = 0;
                decimal minTolerance = 0;
                string toleranceType = "";
                string componentPackaging = "";
                string msg = null;
                SQLDataControl.UpdateBas_Material(1, customerCode, materialCode, materialName, lcrType, lcrStandardValue, lcrUnitCode, lcrMaxValue, lcrMinValue, size, compensateValue, picture, remarks, creator, creationDate, modifier, modificationDate, screenPrinting, maxTolerance, minTolerance, toleranceType, componentPackaging, out msg);
                Search();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {

        }

        private void dgvProgram_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                dgvProgram.Rows[e.RowIndex].Selected = true;
            }

        }
    }
}
