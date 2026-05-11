using ComponentFactory.Krypton.Toolkit;
using SqlHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSP_SJ
{
    public partial class FormSize : KryptonForm
    {
        public FormSize()
        {
            InitializeComponent();
            this.Load += FormCompensationParam_Load;
        }

        private void FormCompensationParam_Load(object sender, EventArgs e)
        {
            Search();
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
                dgvProgram.Rows[e.RowIndex].Selected = true;
        }

        List<Man_ComponentSize> man_ComponentSizes = new List<Man_ComponentSize>();

        private void Search()
        {
            dgvProgram.Rows.Clear();
            man_ComponentSizes = SQLDataControl.GetMan_ComponentSize();
            foreach (var item in man_ComponentSizes)
            {
                bool isOCR = item.IsOCR == true ? true : false;
                dgvProgram.Rows.
                    Add(item.SizeId,
                         item.SizeCode,
                         item.PixelWidth,
                          item.PixelHeight,
                          item.PhysicalSizeLength,
                          item.PhysicalSizeWidth,
                          isOCR,
                          item.Remark
                     );
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            int row = man_ComponentSizes[man_ComponentSizes.Count - 1].SizeId + 1;
            dgvProgram.Rows.Add(row, "", "", "", "", "", false, "");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < dgvProgram.Rows.Count; i++)
                {
                    int SizeId = int.Parse(dgvProgram.Rows[i].Cells[0].Value.ToString());
                    string SizeCode = dgvProgram.Rows[i].Cells[1].Value == null ? "" : dgvProgram.Rows[i].Cells[1].Value.ToString();
                    string PixelWidth = dgvProgram.Rows[i].Cells[2].Value == null ? "" : dgvProgram.Rows[i].Cells[2].Value.ToString();
                    string PixelHeight = dgvProgram.Rows[i].Cells[3].Value == null ? "" : dgvProgram.Rows[i].Cells[3].Value.ToString();
                    string PhysicalSizeLength = dgvProgram.Rows[i].Cells[4].Value == null ? "" : dgvProgram.Rows[i].Cells[4].Value.ToString();
                    string PhysicalSizeWidth = dgvProgram.Rows[i].Cells[5].Value == null ? "" : dgvProgram.Rows[i].Cells[5].Value.ToString();
                    bool IsOCR = bool.Parse(dgvProgram.Rows[i].Cells[6].Value.ToString());
                    string Remark = dgvProgram.Rows[i].Cells[7].Value == null ? "" : dgvProgram.Rows[i].Cells[7].Value.ToString();
                    int pw = 0;
                    int ph = 0;
                    decimal phyL = 0;
                    decimal phyW = 0;
                    if (string.IsNullOrEmpty(SizeCode))
                    {
                        MessageBox.Show("请填写尺寸");
                        return;
                    }

                    else if (!int.TryParse(PixelWidth, out pw))
                    {
                        MessageBox.Show("请填写截图宽度");
                        return;
                    }

                    else if (!int.TryParse(PixelHeight, out ph))
                    {
                        MessageBox.Show("请填写截图高度");
                        return;
                    }
                    else if (!decimal.TryParse(PhysicalSizeLength, out phyL))
                    {
                        MessageBox.Show("请填写物理长度");
                        return;
                    }

                    else if (!decimal.TryParse(PhysicalSizeWidth, out phyW))
                    {
                        MessageBox.Show("请填写物理宽度");
                        return;
                    }

                    SQLDataControl.AddMan_ComponentSize(SizeId, SizeCode, pw, ph, phyL, phyW, IsOCR, Remark);
                }
                Search();
                MessageBox.Show("保存成功");
            }
            catch (Exception)
            {

            }

        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            Search();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvProgram.SelectedRows.Count > 0)
            {
                try
                {
                    if (dgvProgram.SelectedRows[0].Index < man_ComponentSizes.Count)
                        SQLDataControl.DeleteMan_ComponentSize(man_ComponentSizes[dgvProgram.SelectedRows[0].Index].SizeId);

                    dgvProgram.Rows.RemoveAt(dgvProgram.SelectedRows[0].Index);
                }
                catch (Exception ex)
                {

                }
                
          
            }
        }
    }
}
