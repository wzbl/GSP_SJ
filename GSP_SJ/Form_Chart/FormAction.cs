using ComponentFactory.Krypton.Toolkit;
using Emgu.Util.TypeEnum;
using GSP_SJ.Form_Chart;
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
        private string  productCode = "";
        private List<Man_ReportItem> man_ReportItems = new List<Man_ReportItem>();
        public FormAction(string reportCode,string productCode)
        {
            InitializeComponent();
            this.reportCode = reportCode;
            this.productCode = productCode;
            try
            {
                RefreshDataGrid();
                pictureBoxZoom1 = new UCZoom(Type_Window.Screen);
                Bitmap image = PublicFunction.ByteToBitmap(SQLDataControl.GetProgramOptionPicture(productCode));
                pictureBoxZoom1.SetImage(image);
                this.panel2.Controls.Add(pictureBoxZoom1);
              
                foreach (var item in man_ReportItems)
                {
                    Size size = new Size(100, 100);
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
                        pictureBoxZoom1.AddComponent(component);
                    }
                }

                pictureBoxZoom2 = new UCZoom(Type_Window.Screen);
                Bitmap image2 = new Bitmap("D:\\img\\1.jpg");
                pictureBoxZoom2.SetImage(image2);
                this.panel3.Controls.Add(pictureBoxZoom2);
                foreach (var item in man_ReportItems)
                {
                    Size size = new Size(100, 100);
                    Color color = Color.HotPink;
                    string _x = item.LX.ToString();
                    string _y = item.LY.ToString();
                    if (float.TryParse(_x.Trim().ToString(), out float x) && float.TryParse(_y.Trim().ToString(), out float y))
                    {
                        float angle = 0;
                        if (float.TryParse(item.Angle.ToString(), out
                            angle))
                        {

                        }
                        Component component = new Component(item.Position, new PointF(x, y), size, angle, "null", color);
                        pictureBoxZoom2.AddComponent(component);
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
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            Global.SelectComponent += SelectComponent;
        }

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
            man_ReportItems = SQLDataControl.Search_Man_ReportItem(reportCode);
            dataGridView1.Rows.Clear();
            foreach (var item in man_ReportItems)
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
        }

        private void 位置1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> Position = new List<string>();
            for (int i = 0; i < man_ReportItems.Count; i++)
            {
                Position.Add(man_ReportItems[i].Position);
            }
            FormLocation_Point formLocation = new FormLocation_Point(Position);
            if (formLocation.ShowDialog() == DialogResult.OK)
            {
                dataGridView1.ClearSelection();
                dataGridView1.Rows[formLocation.Index].Selected = true;
                dataGridView1.FirstDisplayedScrollingRowIndex = formLocation.Index;
                Location_Point.handPositionX[0] = double.Parse( man_ReportItems[formLocation.Index].X.ToString());
                Location_Point.handPositionY[0] = double.Parse(man_ReportItems[formLocation.Index].Y.ToString());
                Location_Point.eyePixelPositionX[0] = double.Parse(pictureBoxZoom2.PixPoint().X.ToString());
                Location_Point.eyePixelPositionY[0] = double.Parse(pictureBoxZoom2.PixPoint().Y.ToString());

                pictureBoxZoom1.SetSelectCompont(formLocation.Position);
            }

        }

        private void 位置2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> Position = new List<string>();
            for (int i = 0; i < man_ReportItems.Count; i++)
            {
                Position.Add(man_ReportItems[i].Position);
            }
            FormLocation_Point formLocation = new FormLocation_Point(Position);
            if (formLocation.ShowDialog() == DialogResult.OK)
            {
                dataGridView1.ClearSelection();
                dataGridView1.Rows[formLocation.Index].Selected = true;
                dataGridView1.FirstDisplayedScrollingRowIndex = formLocation.Index;
                Location_Point.handPositionX[1] = double.Parse(man_ReportItems[formLocation.Index].X.ToString());
                Location_Point.handPositionY[1] = double.Parse(man_ReportItems[formLocation.Index].Y.ToString());
                Location_Point.eyePixelPositionX[1] = double.Parse(pictureBoxZoom2.PixPoint().X.ToString());
                Location_Point.eyePixelPositionY[1] = double.Parse(pictureBoxZoom2.PixPoint().Y.ToString());
                pictureBoxZoom1.SetSelectCompont(formLocation.Position);
            }
        }

        private void 位置3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> Position = new List<string>();
            for (int i = 0; i < man_ReportItems.Count; i++)
            {
                Position.Add(man_ReportItems[i].Position);
            }
            FormLocation_Point formLocation = new FormLocation_Point(Position);
            if (formLocation.ShowDialog() == DialogResult.OK)
            {
                dataGridView1.ClearSelection();
                dataGridView1.Rows[formLocation.Index].Selected = true;
                dataGridView1.FirstDisplayedScrollingRowIndex = formLocation.Index;
                Location_Point.handPositionX[2] = double.Parse(man_ReportItems[formLocation.Index].X.ToString());
                Location_Point.handPositionY[2] = double.Parse(man_ReportItems[formLocation.Index].Y.ToString());
                Location_Point.eyePixelPositionX[2] = double.Parse(pictureBoxZoom2.PixPoint().X.ToString());
                Location_Point.eyePixelPositionY[2] = double.Parse(pictureBoxZoom2.PixPoint().Y.ToString());
                pictureBoxZoom1.SetSelectCompont(formLocation.Position);
            }
        }

        private void 位置4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> Position = new List<string>();
            for (int i = 0; i < man_ReportItems.Count; i++)
            {
                Position.Add(man_ReportItems[i].Position);
            }
            FormLocation_Point formLocation = new FormLocation_Point(Position);
            if (formLocation.ShowDialog() == DialogResult.OK)
            {
                dataGridView1.ClearSelection();
                dataGridView1.Rows[formLocation.Index].Selected = true;
                dataGridView1.FirstDisplayedScrollingRowIndex = formLocation.Index;
                Location_Point.handPositionX[3] = double.Parse(man_ReportItems[formLocation.Index].X.ToString());
                Location_Point.handPositionY[3] = double.Parse(man_ReportItems[formLocation.Index].Y.ToString());
                Location_Point.eyePixelPositionX[3] = double.Parse(pictureBoxZoom2.PixPoint().X.ToString());
                Location_Point.eyePixelPositionY[3] = double.Parse(pictureBoxZoom2.PixPoint().Y.ToString());
                pictureBoxZoom1.SetSelectCompont(formLocation.Position);

                foreach (var item in man_ReportItems)
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
                    }
                }
                RefreshDataGrid();
            }
        }
    }

}
