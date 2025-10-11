using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSP_SJ
{
    public partial class FormTest : Form
    {
        public FormTest()
        {
            InitializeComponent();
        }
        FAI_NewEntities db = new FAI_NewEntities();
        private void FormTest_Load(object sender, EventArgs e)
        {
            kryptonDataGridView1.CellDoubleClick += CellDoubleClick;
            //ObjectParameter XYCode = new ObjectParameter("XYCode", "");
            //int ret = db.P_Eng_GetXYCode(XYCode);
        }

        private void CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var query = db.Eng_PubModelItem.OrderByDescending(x => x.CreationDate).ToList();
            string materialcode = kryptonDataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            kryptonDataGridView7.DataSource = db.Eng_PubModel.Where(x => x.MaterialCode == materialcode).ToList();
        }

        public void SearchDB()
        {
            var query = db.Eng_PubModelItem.OrderByDescending(x => x.CreationDate).ToList();

            kryptonDataGridView1.DataSource = query;
        }

        public void UpdateDB()
        {
            db.SaveChanges();
            kryptonDataGridView1.Update();
            SearchDB();
        }

        public void DeleteDB()
        {
            var query = db.Eng_PubModelItem.ToList();
            db.Eng_PubModelItem.Remove(query[0]);
            db.SaveChanges();
            SearchDB();
        }

        public void InsertDB()
        {
            var query = db.Eng_PubModelItem.ToList();
            Eng_PubModel eng_PubModel = new Eng_PubModel();
            Eng_PubModelItem item = new Eng_PubModelItem();
            item.MaterialCode = Guid.NewGuid().ToString();
            eng_PubModel.MaterialCode = item.MaterialCode;
            //item.ID = query[0].ID;
            item.mPicture = query[0].mPicture;
            item.mPW = query[0].mPW;
            item.mPH = query[0].mPH;
            item.mZoomRatio = query[0].mZoomRatio;
            item.mDPI = query[0].mDPI;
            item.PLeft = query[0].PLeft;
            item.PTop = query[0].PTop;
            item.Pw = query[0].Pw;
            item.Ph = query[0].Ph;
            item.Picture = query[0].Picture;
            item.PLeft2 = query[0].PLeft2;
            item.PTop2 = query[0].PTop2;
            item.Pw2 = query[0].Pw2;
            item.Ph2 = query[0].Ph2;
            item.Picture2 = query[0].Picture2;
            item.PLeft3 = query[0].PLeft3;
            item.PTop3 = query[0].PTop3;
            item.Pw3 = query[0].Pw3;
            item.Ph3 = query[0].Ph3;
            item.Picture3 = query[0].Picture3;
            item.PLeft4 = query[0].PLeft4;
            item.PTop4 = query[0].PTop4;
            item.Pw4 = query[0].Pw4;
            item.Ph4 = query[0].Ph4;
            item.CreationDate = DateTime.Now;
            item.ModificationDate = DateTime.Now;
            eng_PubModel.CreationDate = item.CreationDate;
            db.Eng_PubModelItem.Add(item);
            db.Eng_PubModel.Add(eng_PubModel);
            kryptonDataGridView7.DataSource = item.Eng_PubModel;
            db.SaveChanges();
            SearchDB();
        }

        private void 查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SearchDB();
        }

        private void 跟新ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateDB();
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteDB();
        }

        private void 插入ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertDB();
        }

        private void kryptonDataGridView1_Click(object sender, EventArgs e)
        {

        }
    }
}
