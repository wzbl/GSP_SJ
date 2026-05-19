using BrowApp;
using BrowApp.Language;
using BrowApp.MessageTip;
using ComponentFactory.Krypton.Toolkit;
using ElectricMeter;
using Emgu.Util.TypeEnum;
using GSP;
using GSP.UI;
using GSP_SJ.Form_Chart;
using GSP_SJ.ModelClass;
using HalconDotNet;
using Microsoft.CSharp.RuntimeBinder;
using SqlHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
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
        public Bitmap image;
        /// <summary>
        /// 拼图
        /// </summary>
        public Bitmap image2;
        public FormAction(ComponentCanvas canva)
        {
            InitializeComponent();
            try
            {

            }
            catch (Exception ex)
            {

            }

        }
        private string reportCode = "";
        private string productCode = "";
        List<string> Position = new List<string>();
        public List<Man_ComponentSize> sizes;
        public FormAction(string reportCode, string productCode)
        {
            InitializeComponent();
            this.reportCode = reportCode;
            this.productCode = productCode;
            try
            {
                comMaterialCode.Items.Clear();
                comLcrStandValue.Items.Clear();
                pictureBoxZoom1 = new UCZoom(Type_Window.Position);
                this.panel2.Controls.Add(pictureBoxZoom1);
                pictureBoxZoom2 = new UCZoom(Type_Window.Puzzle);
                this.panel3.Controls.Add(pictureBoxZoom2);
                pictureBoxZoom3 = new UCZoom(Type_Window.OCR);
                kryptonPanel5.Controls.Add(pictureBoxZoom3);
                pictureBoxZoom2.ContextMenuStrip = contextMenuStrip1;
                comBomdgvShowModel.SelectedIndex = 0;
                comBomdgvShowType.SelectedIndex = 0;
                this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
                DBEventAction.SelectComponent += SelectComponent;
                DBEventAction.ChangeImg += ChangeImg;
                dgvBom.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvBom_CellClick);
                dgvdgvSub.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvdgvSub_CellClick);
                txtReportCode.Text = reportCode;
                txtProductCode.Text = productCode;
                comSortType.SelectedIndex = 4;
                comSamplingSource.SelectedIndex = 1;
                comSamplingType.SelectedIndex = 0;
                numSamplingNum.Value = 1;
                comMaterialCode.Enabled = false;
                comLcrStandValue.Enabled = false;
                com复测.SelectedIndex = 2;
                DBEventAction.RefreshManReport += RefreshManReport;
                this.位置2ToolStripMenuItem.Enabled = false;
                this.位置3ToolStripMenuItem.Enabled = false;
                this.位置4ToolStripMenuItem.Enabled = false;
            }
            catch (Exception ex)
            {

            }
            this.Shown += OnShown;
            this.FormClosing += FormClosin;
        }
        private void FormClosin(object sender, EventArgs e)
        {
            IsManualLcr = false;
        }
        private void OnShown(object sender, EventArgs e)
        {
            UpdateLanguage();
        }

        private void RefreshManReport()
        {
            RefreshDataGrid();
            image = PublicFunction.ByteToBitmap(DBEventAction._Reports.PositionImage);
            if (image != null)
                pictureBoxZoom1.SetImage(image);
            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.Rows[0].Selected = true;
                string pos = dataGridView1.Rows[0].Cells[0].Value.ToString();
                pictureBoxZoom1.SetSelectCompont(pos);
                pictureBoxZoom2.SetSelectCompont(pos);
                pictureBoxZoom3.SetSelectCompont(pos);
                RefreshMaterial(pos);
            }
        }

        public  void Init()
        {
            if (image2 != null)
            {
                pictureBoxZoom2.SetImage(image2);
                pictureBoxZoom3.SetImage((Image)image2.Clone());
            }
            if (image != null)
                pictureBoxZoom1.SetImage(image);
            pictureBoxZoom1.Refresh();
            pictureBoxZoom2.Refresh();
            pictureBoxZoom3.Refresh();
            pictureBoxZoom2.Refresh_IsFurnace();
            foreach (var item in DBEventAction.man_ReportItems)
            {
                comMaterialCode.Items.Add(item.MaterialCode);
                comLcrStandValue.Items.Add(item.LcrStandardValue);
                Position.Add(item.Position);
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

                dataGridView1.Rows.Add(
                   item.Position,
                   item.ResultType,
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
                refresCellStatus(item);
            }
            dataGridView1.Refresh();
            RefreshBom();
            RefreshMessage();
            txtProductName.Text = DBEventAction._Reports.ProductName;
            txtBoardSide.Text = DBEventAction._Reports.BoardSide;
            txtBoardQty.Text = DBEventAction._Reports.BoardQty.ToString();
      
            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.Rows[0].Selected = true;
                string pos = dataGridView1.Rows[0].Cells[0].Value.ToString();
                pictureBoxZoom1.SetSelectCompont(pos);
                pictureBoxZoom2.SetSelectCompont(pos);
                pictureBoxZoom3.SetSelectCompont(pos);
                RefreshMaterial(pos);
            }
            comSortType.SelectedIndexChanged += comSortType_SelectedIndexChanged;

            if (DBEventAction._Reports.IsFurnace == null || (bool)DBEventAction._Reports.IsFurnace)
                this.位置1ToolStripMenuItem.Enabled = false;
            else
                this.位置1ToolStripMenuItem.Enabled = true;
        }
        private void UpdateLanguage()
        {
            BrowApp.Language.Language.Instance.UpdateLanguage(this, null);
        }

        private void ChangeImg(Type_Window window)
        {
            switch (window)
            {
                case Type_Window.Position:
                    this.image = new Bitmap(pictureBoxZoom1.Image);
                    break;
                case Type_Window.Puzzle:
                    this.image2 = new Bitmap(pictureBoxZoom2.Image);
                    if (image2 != null)
                        pictureBoxZoom3.SetImage((Image)image2.Clone());
                    SQLDataControl.UpdateMan_Report(reportCode, PublicFunction.BitmapToByte(image2));
                    //跟新图像到数据库
                    break;
                case Type_Window.OCR:

                    break;
            }
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

                    RefreshMaterial(pos);
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
                RefreshMaterial(pos);
            }
        }

        private void RefreshDataGrid()
        {
            DBEventAction.man_ReportItems = SQLDataControl.Search_Man_ReportItem(reportCode);
            dataGridView1.Rows.Clear();
            //item.CheckType
            foreach (var item in DBEventAction.man_ReportItems)
            {
                dataGridView1.Rows.Add(
                    item.Position,
                    item.ResultType,
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

                refresCellStatus(item);

            }
            RefreshBom();
        }

        private void refresCellStatus(Man_ReportItem item)
        {
            if (item.ResultType == "manual")
            {
                dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[1].Style.BackColor = Color.Green;

                if (item.CheckResult == "PASS")
                {
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[2].Style.BackColor = Color.Green;
                }
            }
            else if (item.ResultType == "auto")
            {
                dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[1].Style.BackColor = Color.Lime;
                if (item.CheckResult == "PASS")
                {
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[2].Style.BackColor = Color.Lime;
                }
            }
            dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[1].Style.ForeColor = Color.White;
            if (item.CheckResult == "FAIL")
            {
                dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[2].Style.BackColor = Color.Red;
                dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[13].Style.ForeColor = Color.Red;
            }

            dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[6].Style.ForeColor = Color.Blue;
        }


        private void 位置1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //位置Mark1

            string pos = "";
            if (pictureBoxZoom1.GetSelectComponent(out Image img, out Component component))
            {
                pos = component.Designator;
            }
            FormLocation_Point formLocation = new FormLocation_Point(Position, pos);
            if (formLocation.ShowDialog() == DialogResult.OK)
            {
                //dataGridView1.ClearSelection();
                //dataGridView1.Rows[formLocation.Index].Selected = true;
                //dataGridView1.FirstDisplayedScrollingRowIndex = formLocation.Index;
                Location_Point.handPositionX[0] = double.Parse(DBEventAction.man_ReportItems[formLocation.Index].X.ToString());
                Location_Point.handPositionY[0] = double.Parse(DBEventAction.man_ReportItems[formLocation.Index].Y.ToString());
                Location_Point.eyePixelPositionX[0] = double.Parse(pictureBoxZoom2.PixPoint().X.ToString());
                Location_Point.eyePixelPositionY[0] = double.Parse(pictureBoxZoom2.PixPoint().Y.ToString());

                pictureBoxZoom1.SetSelectCompont(formLocation.Position);
                MotionCommand.C_Mark(formLocation.Position, 1, 1, Location_Point.handPositionX[0], Location_Point.handPositionY[0], (int)Location_Point.eyePixelPositionX[0], (int)Location_Point.eyePixelPositionY[0], pictureBoxZoom2.Image.Width, pictureBoxZoom2.Image.Height, (double)numpcbLong.Value, (double)numpcbHeight.Value);

                this.位置1ToolStripMenuItem.Enabled = false;
                this.位置2ToolStripMenuItem.Enabled = true;
                this.位置3ToolStripMenuItem.Enabled = false;
                this.位置4ToolStripMenuItem.Enabled = false;
            }

        }

        private void 位置2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //位置Mark2
            string pos = "";
            if (pictureBoxZoom1.GetSelectComponent(out Image img, out Component component))
            {
                pos = component.Designator;
            }
            FormLocation_Point formLocation = new FormLocation_Point(Position, pos);
            if (formLocation.ShowDialog() == DialogResult.OK)
            {
                //dataGridView1.ClearSelection();
                //dataGridView1.Rows[formLocation.Index].Selected = true;
                //dataGridView1.FirstDisplayedScrollingRowIndex = formLocation.Index;
                Location_Point.handPositionX[1] = double.Parse(DBEventAction.man_ReportItems[formLocation.Index].X.ToString());
                Location_Point.handPositionY[1] = double.Parse(DBEventAction.man_ReportItems[formLocation.Index].Y.ToString());
                Location_Point.eyePixelPositionX[1] = double.Parse(pictureBoxZoom2.PixPoint().X.ToString());
                Location_Point.eyePixelPositionY[1] = double.Parse(pictureBoxZoom2.PixPoint().Y.ToString());
                pictureBoxZoom1.SetSelectCompont(formLocation.Position);
                MotionCommand.C_Mark(formLocation.Position, 2, 1, Location_Point.handPositionX[1], Location_Point.handPositionY[1], (int)Location_Point.eyePixelPositionX[1], (int)Location_Point.eyePixelPositionY[1], pictureBoxZoom2.Image.Width, pictureBoxZoom2.Image.Height, (double)numpcbLong.Value, (double)numpcbHeight.Value);

                this.位置1ToolStripMenuItem.Enabled = false;
                this.位置2ToolStripMenuItem.Enabled = false;
                this.位置3ToolStripMenuItem.Enabled = true;
                this.位置4ToolStripMenuItem.Enabled = false;
            }
        }

        private void 位置3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string pos = "";
            if (pictureBoxZoom1.GetSelectComponent(out Image img, out Component component))
            {
                pos = component.Designator;
            }
            FormLocation_Point formLocation = new FormLocation_Point(Position, pos);
            if (formLocation.ShowDialog() == DialogResult.OK)
            {
                //dataGridView1.ClearSelection();
                //dataGridView1.Rows[formLocation.Index].Selected = true;
                //dataGridView1.FirstDisplayedScrollingRowIndex = formLocation.Index;
                Location_Point.handPositionX[2] = double.Parse(DBEventAction.man_ReportItems[formLocation.Index].X.ToString());
                Location_Point.handPositionY[2] = double.Parse(DBEventAction.man_ReportItems[formLocation.Index].Y.ToString());
                Location_Point.eyePixelPositionX[2] = double.Parse(pictureBoxZoom2.PixPoint().X.ToString());
                Location_Point.eyePixelPositionY[2] = double.Parse(pictureBoxZoom2.PixPoint().Y.ToString());
                pictureBoxZoom1.SetSelectCompont(formLocation.Position);
                this.位置1ToolStripMenuItem.Enabled = false;
                this.位置2ToolStripMenuItem.Enabled = false;
                this.位置3ToolStripMenuItem.Enabled = false;
                this.位置4ToolStripMenuItem.Enabled = true;
            }
        }

        private void 位置4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string pos = "";
            if (pictureBoxZoom1.GetSelectComponent(out Image img, out Component component))
            {
                pos = component.Designator;
            }
            FormLocation_Point formLocation = new FormLocation_Point(Position, pos);
            if (formLocation.ShowDialog() == DialogResult.OK)
            {
                //dataGridView1.ClearSelection();
                //dataGridView1.Rows[formLocation.Index].Selected = true;
                //dataGridView1.FirstDisplayedScrollingRowIndex = formLocation.Index;
                Location_Point.handPositionX[3] = double.Parse(DBEventAction.man_ReportItems[formLocation.Index].X.ToString());
                Location_Point.handPositionY[3] = double.Parse(DBEventAction.man_ReportItems[formLocation.Index].Y.ToString());
                Location_Point.eyePixelPositionX[3] = double.Parse(pictureBoxZoom2.PixPoint().X.ToString());
                Location_Point.eyePixelPositionY[3] = double.Parse(pictureBoxZoom2.PixPoint().Y.ToString());
                pictureBoxZoom1.SetSelectCompont(formLocation.Position);
                SQLDataControl.UpdatateManReport_IsFurnace(DBEventAction._Reports.ReportCode, true);
                foreach (var item in DBEventAction.man_ReportItems)
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
                        //SQLDataControl.UpdateXYData_LXY(productCode, item.Position, decimal.Parse(pixelPositionX.D.ToString()), decimal.Parse(pixelPositionY.D.ToString()));
                        SQLDataControl.UpdateMan_ReportItem_LXY(item.ReportCode, item.Position, decimal.Parse(pixelPositionX.D.ToString()), decimal.Parse(pixelPositionY.D.ToString()));

                    }
                }
                pictureBoxZoom2.Refresh_IsFurnace();
                RefreshDataGrid();
                this.位置1ToolStripMenuItem.Enabled = false;
                this.位置2ToolStripMenuItem.Enabled = false;
                this.位置3ToolStripMenuItem.Enabled = false;
                this.位置4ToolStripMenuItem.Enabled = false;
            }
        }
        #endregion

        #region 信息统计

        private void RefreshMaterial(string pos)
        {
            int i = -1;
            for (int j = 0; j < DBEventAction.man_ReportItems.Count; j++)
            {
                if (DBEventAction.man_ReportItems[j].Position == pos)
                {
                    i = j;
                    break;
                }
            }
            if (i < 0)
                return;
            txtMaterialCode.Text = DBEventAction.man_ReportItems[i].MaterialCode;
            txtMaterialName.Text = DBEventAction.man_ReportItems[i].MaterialName;
            txtPosition.Text = DBEventAction.man_ReportItems[i].Position;
            txtBoardId.Text = DBEventAction.man_ReportItems[i].BoardId.ToString();
            txtLcrType.Text = DBEventAction.man_ReportItems[i].LcrType;
            txtLcrUnitCode.Text = DBEventAction.man_ReportItems[i].LcrUnitCode;
            txtLcrStandardValue.Text = DBEventAction.man_ReportItems[i].LcrStandardValue.ToString();
            txtSize.Text = DBEventAction.man_ReportItems[i].Size;
            txtLcrMaxValue.Text = DBEventAction.man_ReportItems[i].LcrMaxValue.ToString();
            txtLcrMinValue.Text = DBEventAction.man_ReportItems[i].LcrMinValue.ToString();
            txtLcrCheckValue.Text = DBEventAction.man_ReportItems[i].LcrCheckValue.ToString();
            txtIsDefined.Text = DBEventAction.man_ReportItems[i].IsDefined.ToString();
            txtIsSMD.Text = DBEventAction.man_ReportItems[i].IsSMD.ToString();
            txtStationCode.Text = DBEventAction.man_ReportItems[i].StationCode;
        }
        public void RefreshMessage()
        {
            txtTotalQty.Text = DBEventAction._Reports.TotalQty.ToString();
            txtPassQty.Text = DBEventAction._Reports.PassQty.ToString();
            txtFailQty.Text = DBEventAction._Reports.FailQty.ToString();
            txtMissQty.Text = DBEventAction._Reports.MissQty.ToString();
            txtNoSmdQty.Text = DBEventAction._Reports.NoSmdQty.ToString();
            txtAutomationQty.Text = DBEventAction._Reports.AutomationQty.ToString();
            txtRQty.Text = DBEventAction._Reports.RQty.ToString();
            txtCQty.Text = DBEventAction._Reports.CQty.ToString();
            txtLQty.Text = DBEventAction._Reports.LQty.ToString();
            txtDQty.Text = DBEventAction._Reports.DQty.ToString();
            txtBQty.Text = DBEventAction._Reports.BQty.ToString();
            txtOQty.Text = DBEventAction._Reports.OQty.ToString();
            txtTQty.Text = DBEventAction._Reports.TQty.ToString();
            txtIQty.Text = DBEventAction._Reports.IQty.ToString();
        }
        #endregion

        #region 常用功能

        #region 排序
        private void comSortType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comSortType.SelectedIndex)
            {
                case 0:
                    //最优路径
                    break;
                case 1:
                    //站位表
                    break;
                case 2:
                    //BOM表
                    SortByBOM();
                    break;
                case 3:
                    //物料类别
                    SortByLcrType();
                    break;
                case 4:
                    //就近原则
                    break;
                case 5:
                    //优先0度
                    break;
                case 6:
                    //优先90度
                    break;
            }
        }


        private void SortByBOM()
        {

            DBEventAction.man_ReportItems = DBEventAction.man_ReportItems.OrderBy(p => p.BomSequence).ToList();
            RefreshDataGridByChoise();
        }

        private void SortByLcrType()
        {
            DBEventAction.man_ReportItems = DBEventAction.man_ReportItems.OrderBy(p => p.LcrType).ToList();
            RefreshDataGridByChoise();
        }
        #endregion

        private void chkSampling_CheckedChanged(object sender, EventArgs e)
        {
            comSamplingSource.Enabled = chkSampling.Checked;
            comSamplingType.Enabled = chkSampling.Checked;
            numSamplingNum.Enabled = chkSampling.Checked;
        }

        #region 元件筛选
        private void radType_CheckedChanged(object sender, EventArgs e)
        {
            radTypeR.Enabled = radType.Checked;
            radTypeC.Enabled = radType.Checked;
            radTypeL.Enabled = radType.Checked;
            radTypeD.Enabled = radType.Checked;
            radTypeT.Enabled = radType.Checked;
            radTypeI.Enabled = radType.Checked;
            radTypeO.Enabled = radType.Checked;
            comMaterialCode.Enabled = radMaterialCode.Checked;
            comLcrStandValue.Enabled = radLcrStandValue.Checked;
            RefreshDataGridByChoise();
        }
        private void chk01005_CheckedChanged(object sender, EventArgs e)
        {
            RefreshDataGridByChoise();
        }

        private void comMaterialCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshDataGridByChoise();
        }

        private void comLcrStandValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshDataGridByChoise();
        }

        private void RefreshDataGridByChoise()
        {
            dataGridView1.Rows.Clear();

            List<string> sizes = new List<string>();
            if (chk01005.Checked)
                sizes.Add("01005");
            if (chk0402.Checked)
                sizes.Add("0402");
            if (chk0201.Checked)
                sizes.Add("0201");
            if (chk0603.Checked)
                sizes.Add("0603");
            if (chk0805.Checked)
                sizes.Add("0805");
            if (chk1206.Checked)
                sizes.Add("1206");
            if (chk1210.Checked)
                sizes.Add("1210");
            if (chk1812.Checked)
                sizes.Add("1812");
            if (chk2010.Checked)
                sizes.Add("2010");
            if (chk2512.Checked)
                sizes.Add("2512");
            List<Man_ReportItem> _ReportItems = DBEventAction.man_ReportItems.Where(x => sizes.Contains(x.Size)).ToList();

            if (radType.Checked)
            {
                List<string> lcrtypes = new List<string>();
                if (radTypeR.Checked)
                    lcrtypes.Add("R");
                if (radTypeC.Checked)
                    lcrtypes.Add("C");
                if (radTypeL.Checked)
                    lcrtypes.Add("L");
                if (radTypeD.Checked)
                    lcrtypes.Add("D");
                if (radTypeO.Checked)
                    lcrtypes.Add("O");
                if (radTypeT.Checked)
                    lcrtypes.Add("T");
                if (radTypeI.Checked)
                    lcrtypes.Add("I");
                _ReportItems = _ReportItems.Where(x => lcrtypes.Contains(x.LcrType)).ToList();
            }
            else if (radMaterialCode.Checked)
            {
                if (!string.IsNullOrEmpty(comMaterialCode.Text))
                {
                    _ReportItems = _ReportItems.Where(x => x.MaterialCode == comMaterialCode.Text).ToList();
                }

            }
            else if (radLcrStandValue.Checked)
            {
                if (decimal.TryParse(comLcrStandValue.Text, out decimal v))
                {
                    _ReportItems = _ReportItems.Where(x => x.LcrStandardValue == v).ToList();
                }
            }

            if (radCheckStatus_人工.Checked)
            {
                _ReportItems = _ReportItems.Where(x => x.ResultType == "manual").ToList();
            }
            else if (radCheckStatus_已测.Checked)
            {
                _ReportItems = _ReportItems.Where(x => x.CheckResult == "PASS" || x.CheckResult == "FAIL").ToList();
            }
            else if (radCheckStatus_PASS.Checked)
            {
                _ReportItems = _ReportItems.Where(x => x.CheckResult == "PASS").ToList();
            }
            else if (radCheckStatus_FAIL.Checked)
            {
                _ReportItems = _ReportItems.Where(x => x.CheckResult == "FAIL").ToList();
            }
            else if (radCheckStatus_UNKNOWN.Checked)
            {
                _ReportItems = _ReportItems.Where(x => x.CheckResult == "UNKNOWN").ToList();
            }
            else if (radCheckStatus_未测.Checked)
            {
                _ReportItems = _ReportItems.Where(x => x.CheckResult == "FAIL" || string.IsNullOrEmpty(x.CheckResult)).ToList();
            }

            foreach (var item in _ReportItems)
            {
                dataGridView1.Rows.Add(
                    item.Position,
                    item.ResultType,
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
                refresCellStatus(item);
            }
        }
        #endregion

        /// <summary>
        /// 人工判断
        /// </summary>

        private void btnManualPASS_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int selectedRow = dataGridView1.SelectedRows[0].Index;
                string position = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                pictureBoxZoom2.GetSelectComponent(out Image image, out Component component);
                SQLDataControl.UpdateMan_ReportItem_FailCause(reportCode, position, "manual", "PASS", "", DBEventAction.User.UserName, PublicFunction.BitmapToByte(new Bitmap(image)), PublicFunction.BitmapToByte(new Bitmap(image)), "");
                DBEventAction.man_ReportItems = SQLDataControl.Search_Man_ReportItem(reportCode);
                RefreshDataGridByChoise();
                selectedRow++;
                if (dataGridView1.Rows.Count > selectedRow)
                {
                    dataGridView1.Rows[selectedRow].Selected = true;
                    dataGridView1.FirstDisplayedScrollingRowIndex = selectedRow - 1;
                }
            }
        }

        private void btnManualFAIL_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int selectedRow = dataGridView1.SelectedRows[0].Index;
                string FileCause = (sender as KryptonButton).Text;
                string position = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                pictureBoxZoom2.GetSelectComponent(out Image image, out Component component);
                SQLDataControl.UpdateMan_ReportItem_FailCause(reportCode, position, "manual", "FAIL", FileCause, DBEventAction.User.UserName, PublicFunction.BitmapToByte(new Bitmap(image)), PublicFunction.BitmapToByte(new Bitmap(image)), "");
                DBEventAction.man_ReportItems = SQLDataControl.Search_Man_ReportItem(reportCode);
                RefreshDataGridByChoise();
                selectedRow++;
                if (dataGridView1.Rows.Count > selectedRow)
                {
                    dataGridView1.Rows[selectedRow].Selected = true;
                    dataGridView1.FirstDisplayedScrollingRowIndex = selectedRow - 1;
                }
            }
        }

        #endregion

        #region 外观检测

        private void RefreshBom()
        {
            dgvBom.Rows.Clear();
            var Bomdgv = DBEventAction.man_ReportItems.GroupBy(X => X.MaterialCode).Select(g => new
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
                            if (item.LcrType != "R")
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
                var obj = DBEventAction.man_ReportItems.Where(x => x.MaterialCode == materialCode).ToList();
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
            DBEventAction.IsShowCompoment = IsShowCompoment.Checked;
            pictureBoxZoom3.Refresh();
        }

        private void IsShowCompomentPos_CheckedChanged(object sender, EventArgs e)
        {
            DBEventAction.IsShowComponentPos = IsShowCompomentPos.Checked;
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

        private void btnOne_click_Detection_Click(object sender, EventArgs e)
        {
            FormResCheck formResCheck = new FormResCheck(reportCode, productCode, image2, pictureBoxZoom3.Components);
            formResCheck.ShowDialog();
        }

        private void btnAddmodel_Click(object sender, EventArgs e)
        {
            if (pictureBoxZoom3.GetSelectComponent(out Image img, out Component clickedComponent))
            {
                Man_ReportItem item = DBEventAction.man_ReportItems.Where(x => x.Position == clickedComponent.Designator).First();

                AddModel(item, clickedComponent, img);

                FormModelItem formModelItem = new FormModelItem(item.MaterialCode, item.MaterialName, clickedComponent.ProductCode, img, item.LcrType);
                formModelItem.ShowDialog();
            }

        }

        private void AddModel(Man_ReportItem item, Component clickedComponent, Image img)
        {
            Eng_ModelItem eng_ModelItem = new Eng_ModelItem();
            int w = img.Width / 5;
            int h = img.Height / 5;
            Rectangle rectangle = new Rectangle(
            w,
            h,
            img.Width - (w * 2),
            img.Height - (h * 2));

            eng_ModelItem.ProductCode = productCode;
            eng_ModelItem.MaterialCode = item.MaterialCode;
            eng_ModelItem.Id = SQLDataControl.GetEng_ModelItem(productCode, item.MaterialCode).ToList().Count + 1;
            eng_ModelItem.mPicture = PublicFunction.CompressImage(img, "", 100, img.Width, img.Height);
            eng_ModelItem.mPW = img.Width;
            eng_ModelItem.mPH = img.Height;
            eng_ModelItem.mZoomRatio = 1;
            eng_ModelItem.mDPI = 300;
            eng_ModelItem.Groups = 1;
            eng_ModelItem.IsMain = false;
            eng_ModelItem.Polarity = false;
            eng_ModelItem.IsManual = false;
            eng_ModelItem.PLeft = rectangle.X;
            eng_ModelItem.PTop = rectangle.Y;
            eng_ModelItem.Pw = rectangle.Width;
            eng_ModelItem.Ph = rectangle.Height;
            eng_ModelItem.NA = 0;
            eng_ModelItem.NB = 0;
            eng_ModelItem.XA = 100;
            eng_ModelItem.XB = 0;
            eng_ModelItem.P1 = 20;
            eng_ModelItem.P2 = 10;
            eng_ModelItem.LCy = 85;
            eng_ModelItem.HCy = 100;
            eng_ModelItem.Remarks = "";
            eng_ModelItem.Creator = DBEventAction.User.UserName;
            eng_ModelItem.CreationDate = DateTime.Now;
            SQLDataControl.UpdateEng_ModelItem(eng_ModelItem);
        }




        #endregion

        #region 自动检测
        private void chk复测_CheckedChanged(object sender, EventArgs e)
        {
            com复测.Enabled = chk复测.Checked;
        }
        #endregion

        #region MotionControl

        private void btnStart_Click(object sender, EventArgs e)
        {
            MotionCommand.Start();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            MotionCommand.Pause();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            MotionCommand.Stop();
        }
        private void btnAutoModel_Click(object sender, EventArgs e)
        {
            if (BrowLib.Controller.OutPort["自动切换手动"].State())
                BrowLib.Controller.OutPort["自动切换手动"].Off();
            else
            {
                BrowLib.Controller.OutPort["自动切换手动"].On();
                BrowLib.Controller.OutPort["四线切换两线"].On();
            }

        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            MotionCommand.Home();
        }

        private void btnReSetAlarm_Click(object sender, EventArgs e)
        {
            APP.Alarm.Clear();//报警清除
        }

        private void btnGoStopPos_Click(object sender, EventArgs e)
        {
            MotionCommand.GoToStopPos();
        }

        private void btnFlow_In_Click(object sender, EventArgs e)
        {
            MotionCommand.In_Click();
        }

        private void btnFlow_OUT_Click(object sender, EventArgs e)
        {
            MotionCommand.Out_Click();
        }

        private void btn2线测试_Click(object sender, EventArgs e)
        {
            BrowLib.Controller.OutPort["四线切换两线"].On();
        }

        private void btn4线测试_Click(object sender, EventArgs e)
        {
            BrowLib.Controller.OutPort["四线切换两线"].Off();
        }

        private void btn顶升_Click(object sender, EventArgs e)
        {
            if (BrowLib.Controller.OutPort["顶升气缸_OUT"].State())
            {
                BrowLib.Controller.OutPort["顶升气缸_OUT"].Off();
            }
            else
            {
                BrowLib.Controller.OutPort["顶升气缸_OUT"].On();
            }
        }


        #endregion

        bool IsManualLcr = false;

        private void btnSetLcrValue_Click(object sender, EventArgs e)
        {
            if (ElectricMeterManager.IsConnected() == false)
            {
                FloatingTip.ShowError("请先连接电表".tr());
                return;
            }
            if (IsManualLcr)
            {
                return;
            }
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int index = dataGridView1.SelectedRows[0].Index;
                string lcrType = dataGridView1.Rows[index].Cells[9].Value.ToString();
                if (dataGridView1.Rows[index].Cells[3].Value == null || dataGridView1.Rows[index].Cells[5].Value == null || dataGridView1.Rows[index].Cells[8].Value == null)
                    return;
                if (double.TryParse(dataGridView1.Rows[index].Cells[3].Value.ToString(), out double value))
                {

                }
                if (dataGridView1.Rows[index].Cells[5].Value == null)
                    return;
                string unit = dataGridView1.Rows[index].Cells[4].Value.ToString();
                if (double.TryParse(dataGridView1.Rows[index].Cells[5].Value.ToString(), out double min))
                {

                }
                if (double.TryParse(dataGridView1.Rows[index].Cells[8].Value.ToString(), out double max))
                {

                }
                int step = 0;
                IsManualLcr = true;
                Thread thread = new Thread(() =>
                {
                    while (IsManualLcr)
                    {
                        switch (step)
                        {
                            case 0:
                                ElectricMeterManager.SetLCRParameter(lcrType, value, unit, max, min);
                                step++;
                                break;
                            case 1:
                                ElectricMeterManager.ReadRequest();
                                step++;
                                break;
                            case 2:
                                if (ElectricMeterManager.GetReadStatus() == ReadStatus.Finished)
                                {
                                    bool res = ElectricMeterManager.GetLCRResult(out double val);
                                    this.Invoke(new Action(() =>
                                    {
                                        dataGridView1.Rows[index].Cells[6].Value = val;
                                        if (res)
                                        {
                                            dataGridView1.Rows[index].Cells[6].Style.ForeColor = Color.Lime;
                                        }
                                        else
                                        {
                                            dataGridView1.Rows[index].Cells[6].Style.ForeColor = Color.Red;
                                        }
                                    }));
                                    step = 1;
                                }
                                break;
                        }
                        Thread.Sleep(50);
                    }
                });
                thread.Start();
            }
        }

        private void btnStopManualLcr_Click(object sender, EventArgs e)
        {
            IsManualLcr = false;
        }
    }

}
