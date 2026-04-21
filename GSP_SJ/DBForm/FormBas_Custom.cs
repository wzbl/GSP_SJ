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
    public partial class FormBas_Custom : KryptonForm
    {
        public FormBas_Custom()
        {
            InitializeComponent();
            this.Load += FormBas_Custom_Load;
            dgvProgram.CellClick += DgvProgram_CellClick;
        }

        private void DgvProgram_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                dgvProgram.Rows[e.RowIndex].Selected = true;
            }
        }

        private void FormBas_Custom_Load(object sender, EventArgs e)
        {
            search();
        }

        private void search()
        {
            dgvProgram.Rows.Clear();
            List<Bas_Customer> _Customers = SQLDataControl.GetBas_Custom();
            for (int i = 0; i < _Customers.Count; i++)
            {
                dgvProgram.Rows.Add(_Customers[i].CustomerCode, _Customers[i].CustomerName);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            List<Bas_Customer> _Customers = SQLDataControl.GetBas_Custom();
            for (int i = 0; i < dgvProgram.Rows.Count; i++)
            {
                int row = i + 1;
                if (dgvProgram.Rows[i].Cells[0] == null)
                {
                    MessageBox.Show("请填写客户代码");
                    return;
                }

                if (dgvProgram.Rows[i].Cells[1] == null)
                {
                    MessageBox.Show("请填写客户名称");
                    return;
                }
                string customerCode = dgvProgram.Rows[i].Cells[0].ToString();
                string customerName = dgvProgram.Rows[i].Cells[0].ToString();

                if (string.IsNullOrEmpty(customerCode))
                {
                    MessageBox.Show("请填写客户代码");
                    return;
                }

                if (string.IsNullOrEmpty(customerName))
                {
                    MessageBox.Show("请填写客户名称");
                    return;
                }
                if (_Customers.Where(x => x.Row == row).ToList().Count == 0)
                    SQLDataControl.AddBas_Custom(row, customerCode, customerName);
                else
                    SQLDataControl.UpdateBas_Custom(row, customerCode, customerName);
            }
            search();
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (dgvProgram.SelectedRows.Count > 0)
            {
                int row = dgvProgram.SelectedRows[0].Index;
                SQLDataControl.DeleteBas_Custom(row + 1);
                search();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            dgvProgram.Rows.Add("", "");
        }
    }
}
