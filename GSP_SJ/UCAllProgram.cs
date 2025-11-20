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
    public partial class UCAllProgram : UserControl
    {
        public UCAllProgram()
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
            this.Load+=UCAllProgram_Load;
            dgvProgram.CellDoubleClick += DgvProgram_CellDoubleClick;
            dgvProgram.CellClick += DgvProgram_CellClick;
        }

        private void DgvProgram_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
               
                dgvProgram.Rows[e.RowIndex].Selected = true;
            }
        }

        private void DgvProgram_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string productCode = dgvProgram.Rows[e.RowIndex].Cells[0].Value.ToString();
            FormProductItem form = new FormProductItem(eng_Programs.Where(x=>x.产品编号==productCode).First());
            form.Show();
        }

        private void UCAllProgram_Load(object sender, EventArgs e)
        {
            Search();
        }
        List<View_Eng_Program> eng_Programs;
        private void Search()
        {
            eng_Programs =  SQLDataControl.GetAllProgramm();
            dgvProgram.DataSource = eng_Programs;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            FormProductItem form = new FormProductItem();
            form.Show();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            Search();
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (dgvProgram.SelectedRows.Count > 0) 
            {
                string productCode = dgvProgram.SelectedRows[0].Cells[0].Value.ToString();
                SQLDataControl.DeleteProgram(productCode);
                Search();

            } 

        }
    }
}
