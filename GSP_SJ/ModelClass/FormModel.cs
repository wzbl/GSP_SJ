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
    public partial class FormModel : Form
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
    }
}
