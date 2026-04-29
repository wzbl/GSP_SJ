using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace GSP_SJ
{
    public partial class Form2 : Form
    {

        private ComponentCanvas canvas;
        private float radioW = 1f;
        private float radioH = 1f;
        Image img;
        public Form2()
        {
            InitializeComponent();
    
            this.Shown += Form2_Shown;
            canvas = new ComponentCanvas()
            {
                Dock = DockStyle.Fill
            };

            canvas.ComponentDoubleClick += Canvas_ComponentDoubleClick;
            this.panel1.Controls.Add(canvas);
        }

        private void Canvas_ComponentDoubleClick(object sender, ComponentCanvas.ComponentEventArgs e)
        {
            Component comp = e.Component;
            MessageBox.Show(
                $"元件: {comp.Designator}\n" +
                $"位置: X={comp.Position.X:F2}, Y={comp.Position.Y:F2}\n" +
                $"尺寸: {comp.Size.Width:F2} x {comp.Size.Height:F2}\n" +
                $"角度: {comp.Angle:F1}°\n" +
                $"字符: {comp.Text}",
                "元件信息",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void Form2_Shown(object sender, EventArgs e)
        {
            //radioW = (img.Width * 1.0f / Width);
            //radioH = img.Height * 1.0f / Height;
            //pictureBox1.Image = img;
            //pictureBox1.MouseDoubleClick += PictureBox1_MouseDoubleClick;


            SqlHelper.SQL.ConnectSqlSever("127.0.0.1", "FAI_New", "FAILogin", "123456");
            DataTable dataTable = SqlHelper.SQL.Excute("select * from Eng_XYData where ProductCode ='1212'");
            dataGridView1.DataSource = dataTable;
            List<Component> components = new List<Component>();
            foreach (DataRow row in dataTable.Rows)
            {
                try
                {
                    if (row.ItemArray.Length > 10)
                    {
                        Size size = new Size(86, 55);
                        Color color = Color.HotPink;
                        if (row.ItemArray[6].ToString().Contains("01005"))
                        {
                            size = new Size(16, 9);
                            color = Color.DimGray;
                        }
                        else if (row.ItemArray[6].ToString().Contains("0201"))
                        {
                            size = new Size(25, 13);
                            components[0].Color = Color.Red;
                            color = Color.DarkCyan;
                        }
                        else if (row.ItemArray[6].ToString().Contains("0402"))
                        {
                            size = new Size(43, 23);
                            color = Color.Blue;
                        }
                        else if (row.ItemArray[6].ToString().Contains("0603"))
                        {
                            size = new Size(68, 37);
                            color = Color.Azure;
                        }
                        else if (row.ItemArray[6].ToString().Contains("0805"))
                        {
                            size = new Size(86, 55);
                            color = Color.Cyan;
                        }
                        else if (row.ItemArray[6].ToString().Contains("1206"))
                        {
                            size = new Size(133, 68);
                            color = Color.Gray;
                        }
                        else if (row.ItemArray[1].ToString().ToUpper().Contains("MARK"))
                        {
                            size = new Size(68, 37);
                            color = Color.DarkGreen;
                        }
                        string _x = row.ItemArray[7].ToString();
                        string _y = row.ItemArray[8].ToString();

                        if (float.TryParse(_x.Trim().ToString(), out float x) && float.TryParse(_y.Trim().ToString(), out float y))
                        {
                            float angle = 0;
                            if (float.TryParse(row.ItemArray[10].ToString(), out
                                angle))
                            {

                            }
                            Component component = new Component(row.ItemArray[1].ToString(), new PointF(x * Component.radio, (y) * Component.radio), new SizeF(size.Width * Component.radio / 25.0f, size.Height * Component.radio / 25.0f), angle, "null", color);
                            components.Add(component);
                        }
                    }
                }
                catch (Exception)
                {



                }
            }
            canvas.Components = components;
            canvas.FitToView();


        }

        private void Form2_Load(object sender, EventArgs e)
        {
            radioW = (img.Width * 1.0f / Width);
            radioH = img.Height * 1.0f / Height;
            //pictureBox1.Image = img;
            //pictureBox1.MouseDoubleClick += PictureBox1_MouseDoubleClick;
        }

        private void PictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            PointF point = new PointF(e.X / radioW, e.Y / radioH);
            PointF worldPos = canvas.ScreenToWorld(new Point((int)point.X, (int)point.Y));
            //worldPos = ScreenToWorld(new Point((int)worldPos.X, (int)worldPos.Y));
            // 查找点击的元件 - 使用改进的检测方法
            Component clickedComponent = canvas.FindComponentAt(worldPos);
            if (clickedComponent != null)
            {
                // 触发双击事件
                canvas.OnComponentDoubleClick(clickedComponent);

                // 可选：高亮显示被点击的元件
                //canvas. HighlightComponent(clickedComponent);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > 0)
            {
                string pos = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                canvas.SetSelectPos(pos);
            }
        }
    }
}
