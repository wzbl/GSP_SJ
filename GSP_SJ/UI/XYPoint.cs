using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSP.UI
{
    public partial class XYPoint : Form
    {

        // 原点位置（可动态设置）
        private int _originX = 400;  // 默认原点X位置（像素）
        private int _originY = 300;  // 默认原点Y位置（像素）

        // 坐标系参数
        private const int AxisLength = 750; // 轴延伸长度（像素）
        private const int TickInterval = 20; // 刻度间隔（像素）
        private const int LabelOffset = 10;  // 标签偏移量


        public XYPoint()
        {
            InitializeComponent();
            this.Text = "可指定原点的坐标系";
            this.Size = new Size(800, 600);
            this.DoubleBuffered = true;
            this.Paint += MainForm_Paint;
            this.Resize += (s, e) => this.Invalidate();


            // 添加按钮测试动态修改原点
            AddTestControls();

        }

        private void AddTestControls()
        {
            Button btnSetOrigin = new Button
            {
                Text = "设置原点为(50, 50)",
                Location = new Point(10, 10),
                AutoSize = true
            };
            btnSetOrigin.Click += (s, e) =>
            {
                _originX = 50;
                _originY = 50;
                this.Invalidate(); // 触发重绘
            };
            this.Controls.Add(btnSetOrigin);
        }

        // 主绘制方法
        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(Color.White);

            // 绘制坐标轴
            DrawAxes(g);
            DrawGrid(g);
            // 绘制刻度线及标签
            DrawTicksAndLabels(g);
        }

        // 绘制坐标轴
        private void DrawAxes(Graphics g)
        {
            using (Pen axisPen = new Pen(Color.Red, 5))
            {
                // X轴（从原点向左右延伸）
                g.DrawLine(axisPen,
                    _originX - AxisLength, _originY,
                    _originX + AxisLength, _originY);

                // Y轴（从原点向上下延伸）
                g.DrawLine(axisPen,
                    _originX, _originY - AxisLength,
                    _originX, _originY + AxisLength);
            }

            // 绘制箭头
            DrawArrow(g, _originX + AxisLength, _originY, 10, 0); // X轴箭头
            DrawArrow(g, _originX, _originY - AxisLength, 0, -10); // Y轴箭头
        }

        // 绘制箭头
        private void DrawArrow(Graphics g, int x, int y, int dx, int dy)
        {
            Point[] arrowPoints =
            {
            new Point(x, y),
            new Point(x - dx + dy, y - dy + dx),
            new Point(x - dx - dy, y - dy - dx)
        };
            g.FillPolygon(Brushes.Black, arrowPoints);
        }

        private void DrawGrid(Graphics g)
        {
            using (Pen gridPen = new Pen(Color.LightGray))
            {
                // 水平网格线（X轴方向）
                for (int y = _originY - AxisLength; y <= _originY + AxisLength; y += TickInterval)
                {
                    g.DrawLine(gridPen, _originX - AxisLength, y, _originX + AxisLength, y);
                }

                // 垂直网格线（Y轴方向）
                for (int x = _originX - AxisLength; x <= _originX + AxisLength; x += TickInterval)
                {
                    g.DrawLine(gridPen, x, _originY - AxisLength, x, _originY + AxisLength);
                }
            }
        }
        // 绘制刻度及标签
        private void DrawTicksAndLabels(Graphics g)
        {
            Font labelFont = new Font("Arial", 8);
            Brush labelBrush = Brushes.Black;

            // X轴刻度（左右延伸）
            for (int offset = -AxisLength; offset <= AxisLength; offset += TickInterval)
            {
                int x = _originX + offset;
                // 刻度线
                g.DrawLine(Pens.Gray, x, _originY - 5, x, _originY + 5);
                // 标签（排除原点）
                if (offset != 0)
                {
                    string label = (offset / TickInterval * (AxisLength / 20)).ToString();
                    g.DrawString(label, labelFont, labelBrush, x - 10, _originY + LabelOffset);
                }
            }

            // Y轴刻度（上下延伸）
            for (int offset = -AxisLength; offset <= AxisLength; offset += TickInterval)
            {
                int y = _originY + offset;
                // 刻度线
                g.DrawLine(Pens.Gray, _originX - 5, y, _originX + 5, y);
                // 标签（排除原点）
                if (offset != 0)
                {
                    string label = (-offset / TickInterval * (AxisLength / 20)).ToString();
                    g.DrawString(label, labelFont, labelBrush, _originX + LabelOffset, y - 8);
                }
            }

            // 原点标签
            g.DrawString("(0,0)", labelFont, labelBrush, _originX + 5, _originY + 5);
        }

        // 公共方法：设置原点位置
        public void SetOrigin(int x, int y)
        {
            _originX = x;
            _originY = y;
            this.Invalidate();
        }

    }

}
