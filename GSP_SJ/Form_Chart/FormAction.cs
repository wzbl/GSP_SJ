using ComponentFactory.Krypton.Toolkit;
using Emgu.Util.TypeEnum;
using GSP_SJ.Form_Chart;
using GSP_SJ.ModelClass;
using HalconDotNet;
using SqlHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSP_SJ
{
    public partial class FormAction : KryptonForm
    {

        UCZoom pictureBoxZoom1;
        UCZoom pictureBoxZoom2;
        UCZoom pictureBoxZoom3;
        /// <summary>
        /// 位号图
        /// </summary>
        Bitmap image;
        /// <summary>
        /// 拼图
        /// </summary>
        Bitmap image2;
        public FormAction(ComponentCanvas canva)
        {
            InitializeComponent();
            try
            {

            }
            catch (Exception ex)
            {

            }
            this.Load += Form3_Load;
        }
        private string reportCode = "";
        private string productCode = "";
        public FormAction(string reportCode, string productCode)
        {
            InitializeComponent();
            this.reportCode = reportCode;
            this.productCode = productCode;
            try
            {
                RefreshDataGrid();
                pictureBoxZoom1 = new UCZoom(Type_Window.Screen);
                image = PublicFunction.ByteToBitmap(SQLDataControl.GetProgramOptionPicture(productCode));
                pictureBoxZoom1.SetImage(image);
                this.panel2.Controls.Add(pictureBoxZoom1);

                List<Man_ComponentSize> sizes = SQLDataControl.GetMan_ComponentSize();
                pictureBoxZoom2 = new UCZoom(Type_Window.Screen);
                image2 = new Bitmap("D:\\img\\1.jpg");
                pictureBoxZoom2.SetImage(image2);

                pictureBoxZoom3 = new UCZoom(Type_Window.OCR);
                kryptonPanel5.Controls.Add(pictureBoxZoom3);
                pictureBoxZoom3.SetImage((Image)image2.Clone());
                this.panel3.Controls.Add(pictureBoxZoom2);

                foreach (var item in Global.man_ReportItems)
                {
                    bool isHaveModel = SQLDataControl.GetEng_ModelItem(productCode, item.MaterialCode).Count > 0;
                    List<Man_ComponentSize> ComponentSize = sizes.Where(X => X.SizeCode == item.Size).ToList();

                    Size size = new Size(100, 100);

                    if (ComponentSize.Count > 0)
                    {
                        size = new Size((int)ComponentSize.First().PixelWidth, (int)ComponentSize.First().PixelHeight);
                    }

                    Color color = Color.HotPink;
                    string _x = item.RX.ToString();
                    string _y = item.RY.ToString();
                    if (float.TryParse(_x.Trim().ToString(), out float x) && float.TryParse(_y.Trim().ToString(), out float y))
                    {
                        float angle = 0;
                        if (float.TryParse(item.Angle.ToString(), out
                            angle))
                        {

                        }

                        Component component = new Component(item.Position, new PointF(x, y), size, angle, "null", color);
                        component.ProductCode = productCode;
                        component.MaterialCode = item.MaterialCode;
                        component.MaterialName = item.MaterialName;
                        component.IsHaveModel = isHaveModel;
                        pictureBoxZoom1.AddComponent(component);
                    }

                    _x = item.LX.ToString();
                    _y = item.LY.ToString();
                    if (float.TryParse(_x.Trim().ToString(), out x) && float.TryParse(_y.Trim().ToString(), out y))
                    {
                        float angle = 0;
                        if (float.TryParse(item.Angle.ToString(), out
                            angle))
                        {

                        }
                        Component component = new Component(item.Position, new PointF(x, y), size, angle, "null", color);
                        component.ProductCode = productCode;
                        component.MaterialCode = item.MaterialCode;
                        component.MaterialName = item.MaterialName;
                        component.IsHaveModel = isHaveModel;
                        pictureBoxZoom2.AddComponent(component);
                        pictureBoxZoom3.AddComponent(component);
                    }
                }

                pictureBoxZoom2.ContextMenuStrip = contextMenuStrip1;

            }
            catch (Exception ex)
            {

            }
            this.Load += Form3_Load;
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            comBomdgvShowModel.SelectedIndex = 0;
            comBomdgvShowType.SelectedIndex = 0;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            Global.SelectComponent += SelectComponent;

            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.Rows[0].Selected = true;
                string pos = dataGridView1.Rows[0].Cells[0].Value.ToString();
                pictureBoxZoom1.SetSelectCompont(pos);
                pictureBoxZoom2.SetSelectCompont(pos);
                pictureBoxZoom3.SetSelectCompont(pos);
            }
            dgvBom.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvBom_CellClick);
            dgvdgvSub.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvdgvSub_CellClick);

        }

        #region 电性检测
        private void SelectComponent(string pos)
        {
            pictureBoxZoom2.SetSelectCompont(pos);
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                string p = dataGridView1.Rows[i].Cells[0].Value.ToString();
                dataGridView1.FirstDisplayedScrollingRowIndex = i;
                if (p == pos)
                {
                    dataGridView1.ClearSelection();
                    dataGridView1.Rows[i].Selected = true;
                    break;
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                dataGridView1.ClearSelection();
                dataGridView1.Rows[e.RowIndex].Selected = true;
                string pos = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                pictureBoxZoom1.SetSelectCompont(pos);
                pictureBoxZoom2.SetSelectCompont(pos);

            }
        }

        private void RefreshDataGrid()
        {
            Global.man_ReportItems = SQLDataControl.Search_Man_ReportItem(reportCode);
            dataGridView1.Rows.Clear();
            foreach (var item in Global.man_ReportItems)
            {
                dataGridView1.Rows.Add(
                    item.Position,
                    item.CheckType,
                    item.CheckResult,
                    item.LcrStandardValue,
                    item.LcrUnitCode,
                    item.LcrMinValue,
                    item.LcrCheckValue,
                    item.LcrUnitCode,
                    item.LcrMaxValue,
                    item.LcrType,
                    item.Size,
                    item.MaterialCode,
                    item.MaterialName,
                    item.FailCause,
                    item.Angle,
                    item.IsSMD,
                    item.Remarks,
                    item.Creator,
                    item.CreationDate,
                    item.IsDefined,
                    item.BomSequence,
                    item.StandardCode,
                    item.CheckOcrStr,
                    "",
                    item.CheckLine,
                    item.MaxTolerance,
                    item.MinTolerance,
                    item.ToleranceType
                    );
            }
            RefreshBom();
        }

        private void 位置1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> Position = new List<string>();
            for (int i = 0; i < Global.man_ReportItems.Count; i++)
            {
                Position.Add(Global.man_ReportItems[i].Position);
            }
            FormLocation_Point formLocation = new FormLocation_Point(Position);
            if (formLocation.ShowDialog() == DialogResult.OK)
            {
                dataGridView1.ClearSelection();
                dataGridView1.Rows[formLocation.Index].Selected = true;
                dataGridView1.FirstDisplayedScrollingRowIndex = formLocation.Index;
                Location_Point.handPositionX[0] = double.Parse(Global.man_ReportItems[formLocation.Index].X.ToString());
                Location_Point.handPositionY[0] = double.Parse(Global.man_ReportItems[formLocation.Index].Y.ToString());
                Location_Point.eyePixelPositionX[0] = double.Parse(pictureBoxZoom2.PixPoint().X.ToString());
                Location_Point.eyePixelPositionY[0] = double.Parse(pictureBoxZoom2.PixPoint().Y.ToString());

                pictureBoxZoom1.SetSelectCompont(formLocation.Position);
            }

        }

        private void 位置2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> Position = new List<string>();
            for (int i = 0; i < Global.man_ReportItems.Count; i++)
            {
                Position.Add(Global.man_ReportItems[i].Position);
            }
            FormLocation_Point formLocation = new FormLocation_Point(Position);
            if (formLocation.ShowDialog() == DialogResult.OK)
            {
                dataGridView1.ClearSelection();
                dataGridView1.Rows[formLocation.Index].Selected = true;
                dataGridView1.FirstDisplayedScrollingRowIndex = formLocation.Index;
                Location_Point.handPositionX[1] = double.Parse(Global.man_ReportItems[formLocation.Index].X.ToString());
                Location_Point.handPositionY[1] = double.Parse(Global.man_ReportItems[formLocation.Index].Y.ToString());
                Location_Point.eyePixelPositionX[1] = double.Parse(pictureBoxZoom2.PixPoint().X.ToString());
                Location_Point.eyePixelPositionY[1] = double.Parse(pictureBoxZoom2.PixPoint().Y.ToString());
                pictureBoxZoom1.SetSelectCompont(formLocation.Position);
            }
        }

        private void 位置3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> Position = new List<string>();
            for (int i = 0; i < Global.man_ReportItems.Count; i++)
            {
                Position.Add(Global.man_ReportItems[i].Position);
            }
            FormLocation_Point formLocation = new FormLocation_Point(Position);
            if (formLocation.ShowDialog() == DialogResult.OK)
            {
                dataGridView1.ClearSelection();
                dataGridView1.Rows[formLocation.Index].Selected = true;
                dataGridView1.FirstDisplayedScrollingRowIndex = formLocation.Index;
                Location_Point.handPositionX[2] = double.Parse(Global.man_ReportItems[formLocation.Index].X.ToString());
                Location_Point.handPositionY[2] = double.Parse(Global.man_ReportItems[formLocation.Index].Y.ToString());
                Location_Point.eyePixelPositionX[2] = double.Parse(pictureBoxZoom2.PixPoint().X.ToString());
                Location_Point.eyePixelPositionY[2] = double.Parse(pictureBoxZoom2.PixPoint().Y.ToString());
                pictureBoxZoom1.SetSelectCompont(formLocation.Position);
            }
        }

        private void 位置4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> Position = new List<string>();
            for (int i = 0; i < Global.man_ReportItems.Count; i++)
            {
                Position.Add(Global.man_ReportItems[i].Position);
            }
            FormLocation_Point formLocation = new FormLocation_Point(Position);
            if (formLocation.ShowDialog() == DialogResult.OK)
            {
                dataGridView1.ClearSelection();
                dataGridView1.Rows[formLocation.Index].Selected = true;
                dataGridView1.FirstDisplayedScrollingRowIndex = formLocation.Index;
                Location_Point.handPositionX[3] = double.Parse(Global.man_ReportItems[formLocation.Index].X.ToString());
                Location_Point.handPositionY[3] = double.Parse(Global.man_ReportItems[formLocation.Index].Y.ToString());
                Location_Point.eyePixelPositionX[3] = double.Parse(pictureBoxZoom2.PixPoint().X.ToString());
                Location_Point.eyePixelPositionY[3] = double.Parse(pictureBoxZoom2.PixPoint().Y.ToString());
                pictureBoxZoom1.SetSelectCompont(formLocation.Position);

                foreach (var item in Global.man_ReportItems)
                {
                    Size size = new Size(100, 100);
                    Color color = Color.HotPink;
                    string _x = item.X.ToString();
                    string _y = item.Y.ToString();
                    if (float.TryParse(_x.Trim().ToString(), out float x) && float.TryParse(_y.Trim().ToString(), out float y))
                    {
                        float angle = 0;
                        if (float.TryParse(item.Angle.ToString(), out
                            angle))
                        {

                        }
                        Location_Point.WorldToImage(x, y, out HTuple pixelPositionX, out HTuple pixelPositionY);
                        try
                        {
                            pictureBoxZoom2.UpdateComponent(item.Position, new PointF(float.Parse(pixelPositionX.D.ToString()), float.Parse(pixelPositionY.D.ToString())));
                        }
                        catch (Exception)
                        {

                        }
                        SQLDataControl.UpdateXYData_LXY(productCode, item.Position, decimal.Parse(pixelPositionX.D.ToString()), decimal.Parse(pixelPositionY.D.ToString()));
                        SQLDataControl.UpdateMan_ReportItem_LXY(item.ReportCode, item.Position, decimal.Parse(pixelPositionX.D.ToString()), decimal.Parse(pixelPositionY.D.ToString()));
                    }
                }
                RefreshDataGrid();
            }
        }
        #endregion

        #region 外观检测

        private void RefreshBom()
        {
            dgvBom.Rows.Clear();
            var Bomdgv = Global.man_ReportItems.GroupBy(X => X.MaterialCode).Select(g => new
            {
                MaterialCode = g.Key,  // 分组字段
                LcrType = g.First().LcrType,       // 字段A
                num = g.Count(),
                MaterialName = g.First().MaterialName       // 字段B
            }).ToList();

            foreach (var item in Bomdgv)
            {
                if (!string.IsNullOrEmpty(item.MaterialCode))
                {
                    bool isModel = SQLDataControl.GetEng_ModelItem(productCode, item.MaterialCode).Count() > 0;
                    switch (comBomdgvShowModel.SelectedIndex)
                    {
                        case 0:

                            break;
                        case 1:
                            if (!isModel)
                            {
                                continue;
                            }
                            break;
                        case 2:
                            if (isModel)
                            {
                                continue;
                            }
                            break;
                    }

                    switch (comBomdgvShowType.SelectedIndex)
                    {
                        case 0:

                            break;
                        case 1:
                            if (item.LcrType!="R")
                            {
                                continue;
                            }
                            break;
                        case 2:
                            if (item.LcrType != "C")
                            {
                                continue;
                            }
                            break;
                        case 3:
                            if (item.LcrType != "L")
                            {
                                continue;
                            }
                            break;
                        case 4:
                            if (item.LcrType != "B")
                            {
                                continue;
                            }
                            break;
                        case 5:
                            if (item.LcrType != "D")
                            {
                                continue;
                            }
                            break;
                        case 6:
                            if (item.LcrType != "I")
                            {
                                continue;
                            }
                            break;
                        case 7:
                            if (item.LcrType != "T")
                            {
                                continue;
                            }
                            break;
                        case 8:
                            if (item.LcrType != "O")
                            {
                                continue;
                            }
                            break;
                    }

                    dgvBom.Rows.Add(item.MaterialCode, item.LcrType, item.num, item.MaterialName, isModel);
                    if (isModel)
                    {
                        dgvBom.Rows[dgvBom.Rows.Count - 1].Cells[4].Style.BackColor = Color.Green;
                        dgvBom.Rows[dgvBom.Rows.Count - 1].Cells[4].Style.ForeColor = Color.White;
                    }
                    else
                    {
                        dgvBom.Rows[dgvBom.Rows.Count - 1].Cells[4].Style.BackColor = Color.White;
                        dgvBom.Rows[dgvBom.Rows.Count - 1].Cells[4].Style.ForeColor = Color.Red;
                    }
                }

            }
        }

        private void dgvdgvSub_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                dgvdgvSub.ClearSelection();
                dgvdgvSub.Rows[e.RowIndex].Selected = true;
                string pos = dgvdgvSub.Rows[e.RowIndex].Cells[2].Value.ToString();
                pictureBoxZoom3.SetSelectCompont(pos);
            }
        }

        private void dgvBom_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                dgvBom.ClearSelection();
                dgvBom.Rows[e.RowIndex].Selected = true;
                string materialCode = dgvBom.Rows[e.RowIndex].Cells[0].Value.ToString();
                var obj = Global.man_ReportItems.Where(x => x.MaterialCode == materialCode).ToList();
                dgvdgvSub.Rows.Clear();
                if (obj.Count > 0)
                {
                    pictureBoxZoom3.SetSelectCompont(obj.First().Position);
                    foreach (var item in obj)
                    {

                        dgvdgvSub.Rows.Add
                            (
                              item.BoardId,
                              item.MaterialCode,
                              item.Position,
                              item.LX,
                              item.LY,
                              item.Angle,
                              item.Size,
                              item.CheckOcrStr
                            );
                    }
                }

            }
        }
        private void IsShowCompoment_CheckedChanged(object sender, EventArgs e)
        {
            Global.IsShowCompoment = IsShowCompoment.Checked;
            pictureBoxZoom3.Refresh();
        }

        private void IsShowCompomentPos_CheckedChanged(object sender, EventArgs e)
        {
            Global.IsShowComponentPos = IsShowCompomentPos.Checked;
            pictureBoxZoom3.Refresh();
        }

        private void comBomdgvShowModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshBom();
        }

        private void comBomdgvShowType_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshBom();
        }

        private void btnSizeManager_Click(object sender, EventArgs e)
        {
            FormSize formSize = new FormSize();
            formSize.ShowDialog();
        }
        #endregion

        private void btnOne_click_Detection_Click(object sender, EventArgs e)
        {
            FormResCheck formResCheck = new FormResCheck(reportCode, productCode, image2,pictureBoxZoom3.Components);
            formResCheck.ShowDialog();
        }
    }

}
