using ComponentFactory.Krypton.Toolkit;
using SqlHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSP_SJ.ModelClass
{
    public partial class FormModel : KryptonForm
    {
        public FormModel()
        {
            InitializeComponent();
            this.Load += FormModelManager_Load;
        }

        private void FormModelManager_Load(object sender, EventArgs e)
        {
            RefreshData();
            dataGridView1.CellClick += dataGridView1_CellClick;
            dataGridView1.CellDoubleClick += dataGridView1_CellDoubleClick;
            txtSearch.TextChanged += txtSearch_TextChanged;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                string materialCode = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                if (materialCode.ToUpper().StartsWith(txtSearch.Text.ToUpper()))
                {
                    dataGridView1.Rows[i].Cells[0].Style.ForeColor = Color.Lime;
                }
                else
                {
                    dataGridView1.Rows[i].Cells[0].Style.ForeColor = Color.Black;
                }
            }
        }

        private void RefreshData()
        {
            dataGridView1.DataSource = SQLDataControl.GetAllEng_PubModel();
        }

        /// <summary>
        /// 获取模板图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string materialCode = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                List<Eng_PubModelItem> eng_PubModelItems = SQLDataControl.GETEng_PubModelItem(materialCode);
                if (eng_PubModelItems != null && eng_PubModelItems.Count > 0)
                {
                    byte[] img = eng_PubModelItems[0].mPicture;
                    MemoryStream memoryStream = new MemoryStream(img);
                    pictureBox1.Image = Image.FromStream(memoryStream);
                }
                dataGridView1.Rows[e.RowIndex].Selected = true;
            }
        }

        /// <summary>
        /// 获取模板数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string materialCode = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                string descpcrition = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                FormModelItem modelItem = new FormModelItem(materialCode, descpcrition);
                modelItem.ShowDialog();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count>0)
            {
                string materialCode = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                SQLDataControl.DeleteEng_PubModel(materialCode);
                RefreshData();
            }
        }

        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("确定要删除所有模板吗？", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                //SQLDataControl.DeleteAllEng_PubModel();
            }
            RefreshData();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshData();
        }
    }
}
