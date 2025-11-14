using MyTestWcfServiceLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace GSP_SJ
{
    public partial class Form1 : Form
    {
        private List<Component2> components = new List<Component2>();
        private float scale = 1.0f;
        private PointF panOffset = new PointF(0, 0);
        private bool isPanning = false;
        private Point lastMousePos;

        public Form1()
        {
            InitializeComponent();
            InitializeComponents();
            this.DoubleBuffered = true; // 启用双缓冲防止闪烁
        }

        private void InitializeComponents()
        {
            // 添加一些示例元件
            //components.Add(new Component2("R1", 100, 50));
            //components.Add(new Component2("C1", -150, -80));
            //components.Add(new Component2("U1", -200, 120));
            //components.Add(new Component2("L1", 180, -100));
            //components.Add(new Component2("D1", 50, -120));
            //components.Add(new Component2("Q1", -50, 150));

            SqlHelper.SQL.ConnectSqlSever("127.0.0.1", "FAI_New", "FAILogin", "123456");
            DataTable dataTable = SqlHelper.SQL.Excute("select * from Eng_XYData where ProductCode ='1212'");
       
            foreach (DataRow row in dataTable.Rows)
            {
                try
                {
                    if (row.ItemArray.Length > 10)
                    {
                        Size size = new Size(86, 55);
                        Color color = Color.HotPink;
                        float fontradio = 1.0f;
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
                            fontradio = 1.2f;
                        }
                        else if (row.ItemArray[6].ToString().Contains("0402"))
                        {
                            size = new Size(43, 23);
                            color = Color.Blue;
                            fontradio = 1.4f;
                        }
                        else if (row.ItemArray[6].ToString().Contains("0603"))
                        {
                            size = new Size(68, 37);
                            color = Color.Azure;
                            fontradio = 1.6f;
                        }
                        else if (row.ItemArray[6].ToString().Contains("0805"))
                        {
                            size = new Size(86, 55);
                            color = Color.Cyan;
                            fontradio = 1.8f;
                        }
                        else if (row.ItemArray[6].ToString().Contains("1206"))
                        {
                            size = new Size(133, 68);
                            color = Color.Gray;
                            fontradio = 2f;
                        }
                        else if (row.ItemArray[1].ToString().ToUpper().Contains("MARK"))
                        {
                            size = new Size(200, 100);
                            color = Color.DarkGreen;
                            fontradio = 2f;
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
                            Component2 component = new Component2(row.ItemArray[1].ToString(), x , y);
                            components.Add(component);
                        }
                    }
                }
                catch (Exception)
                {



                }
            }

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawCoordinateSystem(e.Graphics);
            DrawComponents(e.Graphics);
        }

        private void DrawCoordinateSystem(Graphics g)
        {
            int centerX = this.ClientSize.Width / 2;
            int centerY = this.ClientSize.Height / 2;

            // 应用缩放和平移变换
            g.TranslateTransform(centerX + panOffset.X, centerY + panOffset.Y);
            g.ScaleTransform(scale, scale);

            // 设置抗锯齿
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // 绘制坐标轴
            Pen axisPen = new Pen(Color.Black, 2);
            Pen gridPen = new Pen(Color.LightGray, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };

            // 绘制网格
            for (int x = -270; x <= 270; x += 30)
            {
                g.DrawLine(gridPen, x, -150, x, 150);
            }
            for (int y = -150; y <= 150; y += 30)
            {
                g.DrawLine(gridPen, -270, y, 270, y);
            }

            // 绘制X轴和Y轴
            g.DrawLine(axisPen, -270, 0, 270, 0); // X轴
            g.DrawLine(axisPen, 0, -150, 0, 150); // Y轴

            // 绘制箭头
            DrawArrow(g, axisPen, new PointF(270, 0), new PointF(260, 5), new PointF(260, -5)); // X轴箭头
            DrawArrow(g, axisPen, new PointF(0, -150), new PointF(-5, -140), new PointF(5, -140)); // Y轴箭头

            // 绘制刻度
            Font tickFont = new Font("Arial", 8);
            Brush textBrush = Brushes.Black;

            for (int x = -270; x <= 270; x += 30)
            {
                if (x != 0)
                {
                    g.DrawLine(Pens.Black, x, -3, x, 3);
                    g.DrawString(x.ToString(), tickFont, textBrush, x - 10, 10);
                }
            }

            for (int y = -150; y <= 150; y += 30)
            {
                if (y != 0)
                {
                    g.DrawLine(Pens.Black, -3, y, 3, y);
                    g.DrawString(y.ToString(), tickFont, textBrush, 10, y - 5);
                }
            }

            // 绘制坐标轴标签
            Font labelFont = new Font("Arial", 10, FontStyle.Bold);
            g.DrawString("X", labelFont, textBrush, 250, 15);
            g.DrawString("Y", labelFont, textBrush, -15, -140);

            // 绘制范围指示
            g.DrawString($"X: [-270, 270]", tickFont, textBrush, -100, 130);
            g.DrawString($"Y: [-150, 150]", tickFont, textBrush, -100, 145);

            // 清理资源
            axisPen.Dispose();
            gridPen.Dispose();
            tickFont.Dispose();
            labelFont.Dispose();
        }

        private void DrawArrow(Graphics g, Pen pen, PointF tip, PointF leftWing, PointF rightWing)
        {
            g.DrawLine(pen, tip, leftWing);
            g.DrawLine(pen, tip, rightWing);
        }

        private void DrawComponents(Graphics g)
        {
            foreach (var component in components)
            {
                component.Draw(g, scale);
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            float oldScale = scale;
            if (e.Delta > 0)
            {
                scale *= 1.2f; // 放大
            }
            else
            {
                scale /= 1.2f; // 缩小
            }

            // 限制缩放范围
            scale = Math.Max(0.1f, Math.Min(5.0f, scale));

            // 更新缩放显示
            UpdateScaleDisplay();

            this.Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                isPanning = true;
                lastMousePos = e.Location;
                this.Cursor = Cursors.SizeAll;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (isPanning)
            {
                panOffset.X += e.X - lastMousePos.X;
                panOffset.Y += e.Y - lastMousePos.Y;
                lastMousePos = e.Location;
                this.Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Left)
            {
                isPanning = false;
                this.Cursor = Cursors.Default;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Invalidate();
        }

        private void UpdateScaleDisplay()
        {
            this.Text = $"元件位号图 - 缩放: {scale:P0}";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateScaleDisplay();
        }

        // 添加新元件的方法
        public void AddComponent(string name, float x, float y)
        {
            if (x >= -270 && x <= 270 && y >= -150 && y <= 150)
            {
                components.Add(new Component2(name, x, y));
                this.Invalidate();
            }
        }

        // 清空所有元件
        public void ClearComponents()
        {
            components.Clear();
            this.Invalidate();
        }
    }


    public class Component2
    {
        public string Name { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public Color Color { get; set; }

        public Component2(string name, float x, float y)
        {
            Name = name;
            X = x;
            Y = y;
            // 为不同元件类型分配不同颜色
            Color = GetColorByName(name);
        }

        private Color GetColorByName(string name)
        {
            if (name.StartsWith("R")) return Color.Red;        // 电阻
            if (name.StartsWith("C")) return Color.Blue;       // 电容
            if (name.StartsWith("L")) return Color.Green;      // 电感
            if (name.StartsWith("U")) return Color.Purple;     // 集成电路
            if (name.StartsWith("D")) return Color.Orange;     // 二极管
            if (name.StartsWith("Q")) return Color.Brown;      // 晶体管
            return Color.Black;                                // 默认
        }

        public void Draw(Graphics g, float scale)
        {
            // 元件大小随缩放调整
            float size = Math.Max(4, 8 / scale);
            float fontSize = Math.Max(6, 8 / scale);

            // 绘制元件点
            using (Brush brush = new SolidBrush(Color))
            using (Pen pen = new Pen(Color.DarkGray, 1))
            {
                // 绘制元件位置点
                g.FillEllipse(brush, X - size / 2, Y - size / 2, size, size);
                g.DrawEllipse(pen, X - size / 2, Y - size / 2, size, size);

                // 绘制元件名称
                using (Font font = new Font("Arial", fontSize))
                using (Brush textBrush = new SolidBrush(Color.Black))
                {
                    SizeF textSize = g.MeasureString(Name, font);
                    g.DrawString(Name, font, textBrush,
                        X - textSize.Width / 2,
                        Y + size + 2);
                }

                // 绘制坐标信息
                string coordText = $"({X}, {Y})";
                using (Font coordFont = new Font("Arial", fontSize - 1))
                using (Brush coordBrush = new SolidBrush(Color.Gray))
                {
                    SizeF coordSize = g.MeasureString(coordText, coordFont);
                    g.DrawString(coordText, coordFont, coordBrush,
                        X - coordSize.Width / 2,
                        Y - size - coordSize.Height - 2);
                }
            }
        }
    }
}
