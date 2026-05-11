using BrowApp.Language;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BorwinAnalyse.ImportBom
{
    public partial class UCTilleConfig : UserControl
    {
        public UCTilleConfig()
        {
            InitializeComponent();
            dgvData.CellClick += new DataGridViewCellEventHandler(dgvData_CellClick);
        }

        public void Load(List<string> datas,string title)
        { 
            lbTitleName.Text = title;
            dgvData.Rows.Clear();
            for (int i = 0; i < datas.Count; i++)
            {
                dgvData.Rows.Add(datas[i]);
            }
            btnAdd.Text= btnAdd.Text.tr();
            btnDelete.Text = btnDelete.Text.tr();
        }

        public List<string> GetData()
        {
            List<string> datas = new List<string>();
            for (int i = 0; i < dgvData.Rows.Count; i++)
            {
                datas.Add(dgvData.Rows[i].Cells[0].Value.ToString());
            }
            return datas;
        }

        private void dgvData_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            for (int i = 0; i < dgvData.SelectedRows.Count; i++)
            {
                dgvData.SelectedRows[i].Selected = false;
            }

            if (e.RowIndex >= 0)
            {
                dgvData.Rows[e.RowIndex].Selected = true;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            dgvData.Rows.Add("");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
           if (dgvData.SelectedRows.Count > 0)
            {
                dgvData.Rows.RemoveAt(dgvData.SelectedRows[0].Index);
            }
        }
    }
}
