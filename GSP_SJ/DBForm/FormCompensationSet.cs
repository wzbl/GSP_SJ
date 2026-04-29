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

namespace GSP_SJ
{
    public partial class FormCompensationSet : KryptonForm
    {
        public FormCompensationSet()
        {
            InitializeComponent();
            this.Load += FormCompensationParam_Load;
        }

        private void FormCompensationParam_Load(object sender, EventArgs e)
        {

            Search();
            dgvProgram.CellClick += dgvProgram_CellClick;
        }

        private void dgvProgram_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                dgvProgram.Rows[e.RowIndex].Selected = true;
        }

        private void Search()
        {
            dgvProgram.Rows.Clear();
            List<Bas_CompensationValue> _CompensationValues = SQLDataControl.SearchBas_CompensationValue();
            foreach (Bas_CompensationValue _compensationValue in _CompensationValues)
            {
                dgvProgram.Rows.Add(
                    _compensationValue.Sort,
                    _compensationValue.LcrMinValue,
                    _compensationValue.LcrMaxValue,
                    _compensationValue.LcrUnitCode,
                    _compensationValue.LcrPrecision.ToString(),
                    _compensationValue.LcrCompensationMaxValue,
                    _compensationValue.LcrCompensationMinValue,
                    _compensationValue.IsEnabled,
                    _compensationValue.Creator,
                    _compensationValue.CreationDate,
                    _compensationValue.Modifier,
                    _compensationValue.ModificationDate
                    );
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            int row = int.Parse(dgvProgram.Rows[dgvProgram.RowCount - 1].Cells[0].Value.ToString());
            dgvProgram.Rows.Add(row + 1, "0", "0", "UF", "0.0000", "0", "0", true);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if(dgvProgram.SelectedRows.Count > 0)
            {
                int row = int.Parse(dgvProgram.Rows[dgvProgram.SelectedRows[0].Index].Cells[0].Value.ToString());
                if (dgvProgram.SelectedRows[0].Index < SQLDataControl.SearchBas_CompensationValue().Count)
                    SQLDataControl.DeleteBas_CompensationValue(row);

                dgvProgram.Rows.RemoveAt(dgvProgram.SelectedRows[0].Index);
                Search();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < dgvProgram.Rows.Count; i++)
                {
                    Bas_CompensationValue compensationValue = new Bas_CompensationValue()
                    {
                        Sort = int.Parse(dgvProgram.Rows[i].Cells[0].Value.ToString()),
                        LcrMinValue = decimal.Parse(dgvProgram.Rows[i].Cells[1].Value.ToString()),
                        LcrMaxValue = decimal.Parse(dgvProgram.Rows[i].Cells[2].Value.ToString()),
                        LcrUnitCode = dgvProgram.Rows[i].Cells[3].Value.ToString(),
                        LcrPrecision = decimal.Parse(dgvProgram.Rows[i].Cells[4].Value.ToString()),
                        LcrCompensationMaxValue = decimal.Parse(dgvProgram.Rows[i].Cells[5].Value.ToString()),
                        LcrCompensationMinValue = decimal.Parse(dgvProgram.Rows[i].Cells[6].Value.ToString()),
                        IsEnabled = bool.Parse(dgvProgram.Rows[i].Cells[7].Value.ToString()),
                        Creator = Global.User.UserName,
                        CreationDate = DateTime.Now,
                        Modifier = Global.User.UserName,
                        ModificationDate = DateTime.Now
                    };
                    SQLDataControl.AddBas_CompensationValue(compensationValue);
                }

                Search();
            }
            catch (Exception ex)
            {

            }
           
        }
    }
}
