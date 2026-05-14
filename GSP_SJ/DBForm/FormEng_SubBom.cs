using BrowApp.Language;
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
    public partial class FormEng_SubBom : KryptonForm
    {
        public FormEng_SubBom()
        {
            InitializeComponent();

            this.Load += FormEng_SubBom_Load;
        }

        public string ProductCode = "";
        public string MaterialCode = "";
        public string MaterialDescription = "";

        /// <summary>
        /// 替代料
        /// </summary>
        /// <param name="productCode">产品</param>
        /// <param name="materialCode"></param>
        /// <param name="materialDescription"></param>
        public FormEng_SubBom(string productCode, string materialCode, string materialDescription) : this()
        {
            this.ProductCode = productCode;
            this.MaterialCode = materialCode;
            this.MaterialDescription = materialDescription;
        }

        private void FormEng_SubBom_Load(object sender, EventArgs e)
        {
            search();
            dgvProgram.CellClick += dgvProgram_CellClick;
            UpdateLanguage();
        }
        private void UpdateLanguage()
        {
            BrowApp.Language.Language.Instance.UpdateLanguage(this, null);
        }
        private void dgvProgram_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                dgvProgram.Rows[e.RowIndex].Selected = true;
            }
        }

        private void search()
        {
            dgvProgram.Rows.Clear();
            List<Eng_BomSub> sus = SQLDataControl.SearchEng_BomSub(ProductCode, MaterialCode);
            for (int i = 0; i < sus.Count; i++)
            {
                dgvProgram.Rows.Add
                    (
                    sus[i].ProductCode,
                    sus[i].MaterialCode,
                    sus[i].SubMatCode,
                    sus[i].SubMatName,
                    sus[i].Creator,
                    sus[i].CreationDate
                    );
                dgvProgram.Rows[dgvProgram.Rows.Count - 1].Cells[2].ReadOnly = true;
            }
            dgvProgram.Refresh();
        }


        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dgvProgram.Rows.Count; i++) 
            {
                if (dgvProgram.Rows[i].Cells[2].Value == null)
                {
                     BrowApp.APP.Tip.ShowTip(1, "警告".tr(), "请填写替代料编码".tr(), "确定".tr());
                    return;
                }

                if (dgvProgram.Rows[i].Cells[3].Value == null)
                {
                     BrowApp.APP.Tip.ShowTip(1, "警告".tr(), "请填写替代料描述".tr(), "确定".tr());
                    return;
                }
                string subCode = dgvProgram.Rows[i].Cells[2].Value.ToString();
                string SubName = dgvProgram.Rows[i].Cells[3].Value.ToString();
                string creator = dgvProgram.Rows[i].Cells[4].Value.ToString();

                if (string.IsNullOrEmpty(subCode))
                {
                     BrowApp.APP.Tip.ShowTip(1, "警告".tr(), "请填写替代料编码".tr(), "确定".tr());
                    return;
                }

                if (string.IsNullOrEmpty(SubName))
                {
                     BrowApp.APP.Tip.ShowTip(1, "警告".tr(), "请填写替代料描述".tr(), "确定".tr());
                    return;
                }

                SQLDataControl.Eng_BomSubAdd(ProductCode, MaterialCode, subCode, SubName, creator, 0, out string msg);
                SQLDataControl.UpdatateEng_BomSub(ProductCode, MaterialCode, subCode, SubName);
            }
            search();
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if(dgvProgram.SelectedRows.Count > 0)
            {
                string subCode = dgvProgram.SelectedRows[0].Cells[2].Value.ToString();
                SQLDataControl.Eng_BomSubAdd(ProductCode, MaterialCode, subCode,"","",1,out string msg);
                search();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            dgvProgram.Rows.Add
                (
                ProductCode,
                MaterialCode,
                "",
                MaterialDescription,
                "test",
                ""
                );
            dgvProgram.Rows[dgvProgram.Rows.Count - 1].Cells[2].ReadOnly = false;
        }
    }
}
