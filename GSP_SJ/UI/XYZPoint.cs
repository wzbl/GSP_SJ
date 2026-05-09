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

namespace GSP.UI
{
    public partial class XYZPoint : Form
    {
        private float angleX, angleY;
        private Point customOrigin = Point.Empty;
        private const int AxisLength = 100;

        public XYZPoint()
        {
            InitializeComponent();
            DoubleBuffered = true;
            SetupTrackBars();
            this.MouseClick += MainForm_MouseClick;
            this.Resize += (s, e) => Invalidate();
        }


        private void SetupTrackBars()
        {
            var trackBarX = new TrackBar { Minimum = 0, Maximum = 360, Dock = DockStyle.Bottom };
            var trackBarY = new TrackBar { Minimum = 0, Maximum = 360, Dock = DockStyle.Bottom };

            trackBarX.ValueChanged += (s, e) => { angleX = trackBarX.Value * (float)Math.PI / 180; Invalidate(); };
            trackBarY.ValueChanged += (s, e) => { angleY = trackBarY.Value * (float)Math.PI / 180; Invalidate(); };

            Controls.Add(trackBarX);
            Controls.Add(trackBarY);
        }

        private void MainForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                customOrigin = e.Location;
                Invalidate();
            }
        }
        protected override void OnPrint(PaintEventArgs e)
        {
            base.OnPrint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var origin = customOrigin.IsEmpty
                ? new Point(ClientSize.Width / 2, ClientSize.Height / 2)
                : customOrigin;

            DrawAxis(g, origin, Color.Red, new Point3D(AxisLength, 0, 0), "X");
            DrawAxis(g, origin, Color.Green, new Point3D(0, AxisLength, 0), "Y");
            DrawAxis(g, origin, Color.Blue, new Point3D(0, 0, AxisLength), "Z");
        }
     

        private void DrawAxis(Graphics g, Point origin, Color color, Point3D endPoint, string label)
        {
            var rotated = RotatePoint(endPoint);
            var screenEnd = Project(rotated, origin);

            using (var pen = new Pen(color, 2))
            {
                g.DrawLine(pen, origin, screenEnd);
                DrawArrowHead(g, pen, origin, screenEnd);
                DrawLabel(g, color, screenEnd, label);
            }
        }

        private Point3D RotatePoint(Point3D point)
        {
            // 绕Y轴旋转
            float x = point.X * (float)Math.Cos(angleY) + point.Z * (float)Math.Sin(angleY);
            float z = -point.X * (float)Math.Sin(angleY) + point.Z * (float)Math.Cos(angleY);

            // 绕X轴旋转
            float y = point.Y * (float)Math.Cos(angleX) - z * (float)Math.Sin(angleX);
            z = point.Y * (float)Math.Sin(angleX) + z * (float)Math.Cos(angleX);

            return new Point3D(x, y, z);
        }

        private Point Project(Point3D point, Point origin)
        {
            return new Point(
                origin.X + (int)point.X,
                origin.Y - (int)point.Y  // 反转Y轴方向
            );
        }

        private void DrawArrowHead(Graphics g, Pen pen, Point start, Point end)
        {
            const int arrowSize = 10;
            var direction = new PointF(end.X - start.X, end.Y - start.Y);
            var length = Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);
            if (length == 0) return;

            var unit = new PointF(direction.X / (float)length, direction.Y / (float)length);

            var arrowPoints = new[]
            {
                new PointF(
                    end.X - unit.X * arrowSize + unit.Y * arrowSize,
                    end.Y - unit.Y * arrowSize - unit.X * arrowSize),
                new PointF(
                    end.X - unit.X * arrowSize - unit.Y * arrowSize,
                    end.Y - unit.Y * arrowSize + unit.X * arrowSize)
            };

            g.DrawLines(pen, new[] { end, arrowPoints[0], end, arrowPoints[1] });
        }

        private void DrawLabel(Graphics g, Color color, Point position, string text)
        {
            using (var font = new Font("Arial", 10))
            using (var brush = new SolidBrush(color))
            {
                g.DrawString(text, font, brush, position.X + 5, position.Y + 5);
            }
        }

        struct Point3D
        {
            public float X, Y, Z;

            public Point3D(float x, float y, float z)
            {
                X = x;
                Y = y;
                Z = z;
            }
        }


    }


   
}
