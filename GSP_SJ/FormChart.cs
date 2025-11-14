using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSP_SJ
{
    public partial class FormChart : Form
    {
        private ComponentCanvas canvas;

        public FormChart()
        {
            InitializeComponent();
            InitializeCanvas();
            //CreateSampleData();

            this.Shown += FormChart_Shown;
        }

        private void FormChart_Shown(object sender, EventArgs e)
        {
            SqlHelper.SQL.ConnectSqlSever("127.0.0.1", "FAI_New", "FAILogin", "123456");
            DataTable dataTable = SqlHelper.SQL.Excute("select * from Eng_XYData where ProductCode ='1212'");
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

        private void InitializeCanvas()
        {
            canvas = new ComponentCanvas
            {
                Dock = DockStyle.Fill
            };

            canvas.ComponentDoubleClick += Canvas_ComponentDoubleClick;
            this.Controls.Add(canvas);

            // 添加工具栏
            AddToolbar();
        }

        private void AddToolbar()
        {
            ToolStrip toolStrip = new ToolStrip();

            //    // 缩放控制
            ToolStripLabel scaleLabel = new ToolStripLabel("缩放:");
            ToolStripTextBox scaleBox = new ToolStripTextBox();
            scaleBox.Text = "100%";
            scaleBox.Width = 60;
            scaleBox.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    if (scaleBox.Text.EndsWith("%"))
                    {
                        string percentStr = scaleBox.Text.TrimEnd('%');
                        if (float.TryParse(percentStr, out float percent))
                        {
                            canvas.Scale = percent / 100.0f;
                        }
                    }
                }
            };
            ToolStripButton toolStrip1 = new ToolStripButton("重置视图");
            toolStrip1.Click += (s, e) => canvas.ResetView();
            ToolStripButton toolStrip2 = new ToolStripButton("适合视图");
            toolStrip2.Click += (s, e) => canvas.FitToView();
            ToolStripButton toolStrip3 = new ToolStripButton("水平X镜像");//原点X轴对称
            toolStrip3.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {
                    canvas.Components[i].Position = new PointF(-canvas.Components[i].Position.X, canvas.Components[i].Position.Y);
                    canvas.FitToView();
                }
            };
            ToolStripButton toolStrip4 = new ToolStripButton("水平Y镜像");//原点Y轴对称
            toolStrip4.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {
                    canvas.Components[i].Position = new PointF(canvas.Components[i].Position.X, -canvas.Components[i].Position.Y);
                    canvas.FitToView();
                }
            };

            ToolStripButton toolStrip5 = new ToolStripButton("逆时针90");
            toolStrip5.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {
                    
                }
            };
            ToolStripButton toolStrip6 = new ToolStripButton("顺时针90");
            toolStrip6.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {
                    
                }
            };

            ToolStripButton toolStrip7 = new ToolStripButton("旋转180");
            toolStrip7.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {

                }
            };

            ToolStripButton toolStrip8 = new ToolStripButton("元件水平X镜像");
            toolStrip8.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {

                }
            };
            ToolStripButton toolStrip9 = new ToolStripButton("元件水平Y镜像");
            toolStrip9.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {

                }
            };

            ToolStripButton toolStrip10 = new ToolStripButton("元件逆时针90");
            toolStrip10.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {

                }
            };
            ToolStripButton toolStrip11 = new ToolStripButton("元件顺时针90");
            toolStrip11.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {

                }
            };

            ToolStripButton toolStrip12 = new ToolStripButton("元件旋转180");
            toolStrip12.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {

                }
            };

            toolStrip.Items.AddRange(new ToolStripItem[]
            {
             toolStrip1,
             toolStrip2 ,
             toolStrip3,
             toolStrip4,
             toolStrip5,
             toolStrip6,
             toolStrip7,
             toolStrip8,
             toolStrip9,
             toolStrip10,
             toolStrip11,
             toolStrip12
            });

            this.Controls.Add(toolStrip);
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
    }

    public class Component
    {
        public string Designator { get; set; }
        public PointF Position { get; set; }
        public SizeF Size { get; set; }
        public float Angle { get; set; }
        public string Text { get; set; }
        public Color Color { get; set; } = Color.Black;

        public static float radio = 2f;
        public Component(string designator, PointF position, SizeF size, float angle, string text, Color color)
        {
            Designator = designator;
            Position = position;
            Size = size;
            Angle = angle;
            Text = text;
            Color = color;
        }

        // 获取元件的边界矩形（考虑旋转）
        public RectangleF GetBounds()
        {
            float halfWidth = Size.Width / 2;
            float halfHeight = Size.Height / 2;
            return new RectangleF(
                Position.X - halfWidth,
                Position.Y - halfHeight,
                Size.Width,
                Size.Height
            );
        }

        // 修复：检查点是否在元件内（考虑旋转）
        public bool Contains(PointF point)
        {
            // 将点转换到元件局部坐标系
            PointF localPoint = WorldToLocal(point);

            // 在局部坐标系中检查是否在矩形内
            float halfWidth = Size.Width / 2;
            float halfHeight = Size.Height / 2;

            return Math.Abs(localPoint.X) <= halfWidth && Math.Abs(localPoint.Y) <= halfHeight;
        }

        // 将世界坐标转换到元件局部坐标（考虑旋转）
        private PointF WorldToLocal(PointF worldPoint)
        {
            // 平移到元件中心
            float dx = worldPoint.X - Position.X;
            float dy = worldPoint.Y - Position.Y;

            // 如果没有旋转，直接返回
            if (Angle == 0) return new PointF(dx, dy);

            // 反向旋转点
            float angleRad = -Angle * (float)Math.PI / 180f;
            float cos = (float)Math.Cos(angleRad);
            float sin = (float)Math.Sin(angleRad);

            return new PointF(
                dx * cos - dy * sin,
                dx * sin + dy * cos
            );
        }

        // 添加调试方法：获取旋转后的四个角点
        public PointF[] GetRotatedCorners()
        {
            float halfWidth = Size.Width / 2;
            float halfHeight = Size.Height / 2;

            PointF[] corners = new PointF[]
            {
            new PointF(-halfWidth, -halfHeight), // 左上
            new PointF(halfWidth, -halfHeight),  // 右上
            new PointF(halfWidth, halfHeight),   // 右下
            new PointF(-halfWidth, halfHeight)   // 左下
            };

            // 如果有旋转，应用旋转
            if (Angle != 0)
            {
                float angleRad = Angle * (float)Math.PI / 180f;
                float cos = (float)Math.Cos(angleRad);
                float sin = (float)Math.Sin(angleRad);

                for (int i = 0; i < corners.Length; i++)
                {
                    float x = corners[i].X;
                    float y = corners[i].Y;
                    corners[i] = new PointF(
                        x * cos - y * sin + Position.X,
                        x * sin + y * cos + Position.Y
                    );
                }
            }
            else
            {
                // 没有旋转，只需平移
                for (int i = 0; i < corners.Length; i++)
                {
                    corners[i] = new PointF(
                        corners[i].X + Position.X,
                        corners[i].Y + Position.Y
                    );
                }
            }

            return corners;
        }
    }

    public partial class ComponentCanvas : UserControl
    {
        private List<Component> components = new List<Component>();
        private float scale = 1f;
        private PointF panOffset = PointF.Empty;
        private Point lastMousePos;
        private bool isPanning = false;
        private PointF mouseWorldPos = PointF.Empty;

        // 坐标轴设置
        private bool showAxes = true;
        private Color axesColor = Color.Gray;
        private float gridSize = 50.0f;
        //150,270
        // 拖动模式
        private PanMode currentPanMode = PanMode.None;

        private enum PanMode
        {
            None,
            LeftButton,
            MiddleButton,
            RightButton
        }

        public ComponentCanvas()
        {

            this.DoubleBuffered = true;
            this.BackColor = Color.White;

            // 鼠标事件
            this.MouseDown += ComponentCanvas_MouseDown;
            this.MouseMove += ComponentCanvas_MouseMove;
            this.MouseUp += ComponentCanvas_MouseUp;
            this.MouseWheel += ComponentCanvas_MouseWheel;
            this.MouseDoubleClick += ComponentCanvas_MouseDoubleClick;
            this.Resize += (s, e) => Invalidate();
        }

        // 鼠标按下事件 - 支持多种拖动方式
        private void ComponentCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            // 设置拖动模式
            if (e.Button == MouseButtons.Left)
            {
                currentPanMode = PanMode.LeftButton;
            }
            else if (e.Button == MouseButtons.Middle)
            {
                currentPanMode = PanMode.MiddleButton;
            }
            else if (e.Button == MouseButtons.Right)
            {
                currentPanMode = PanMode.RightButton;
            }

            if (currentPanMode != PanMode.None)
            {
                isPanning = true;
                lastMousePos = e.Location;

                // 根据拖动模式设置不同的光标
                switch (currentPanMode)
                {
                    case PanMode.LeftButton:
                        this.Cursor = Cursors.Hand;
                        break;
                    case PanMode.MiddleButton:
                        this.Cursor = Cursors.Hand;
                        break;
                    case PanMode.RightButton:
                        this.Cursor = Cursors.SizeAll;
                        break;
                }
            }
        }

        // 鼠标移动事件
        private void ComponentCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            // 更新鼠标世界坐标（用于状态显示）
            mouseWorldPos = ScreenToWorld(e.Location);

            if (isPanning && currentPanMode != PanMode.None)
            {
                // 计算鼠标移动距离
                int deltaX = e.X - lastMousePos.X;
                int deltaY = e.Y - lastMousePos.Y;

                // 更新平移偏移量
                panOffset.X += deltaX;
                panOffset.Y += deltaY;
                Debug.WriteLine($"move: ({e.X}, {e.Y}) -> panOffset: ({panOffset.X:F2}, {panOffset.Y:F2})");
                lastMousePos = e.Location;
                Invalidate();
            }
        }

        // 鼠标释放事件
        private void ComponentCanvas_MouseUp(object sender, MouseEventArgs e)
        {
            // 检查释放的按钮是否与当前拖动模式匹配
            if ((e.Button == MouseButtons.Left && currentPanMode == PanMode.LeftButton) ||
                (e.Button == MouseButtons.Middle && currentPanMode == PanMode.MiddleButton) ||
                (e.Button == MouseButtons.Right && currentPanMode == PanMode.RightButton))
            {
                isPanning = false;
                currentPanMode = PanMode.None;
                this.Cursor = Cursors.Default;
            }
        }

        // 鼠标滚轮事件 - 缩放
        private void ComponentCanvas_MouseWheel(object sender, MouseEventArgs e)
        {
            float zoomFactor = e.Delta > 0 ? 1.2f : 0.8f;
            float oldScale = scale;

            // 限制缩放范围
            scale = Math.Max(0.05f, Math.Min(25.0f, scale * zoomFactor));

            if (scale != oldScale)
            {
                // 获取鼠标位置的世界坐标
                PointF mouseWorld = ScreenToWorld(e.Location);
                // 调整偏移量以实现以鼠标为中心的缩放
                panOffset.X = e.Location.X - mouseWorld.X * scale;
                panOffset.Y = e.Location.Y - mouseWorld.Y * scale;
                Invalidate();
            }
        }

        // 鼠标双击事件
        private void ComponentCanvas_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                PointF worldPos = ScreenToWorld(e.Location);
                //worldPos = ScreenToWorld(new Point((int)worldPos.X, (int)worldPos.Y));
                // 查找点击的元件 - 使用改进的检测方法
                Component clickedComponent = FindComponentAt(worldPos);
                if (clickedComponent != null)
                {
                    // 触发双击事件
                    OnComponentDoubleClick(clickedComponent);

                    // 可选：高亮显示被点击的元件
                    HighlightComponent(clickedComponent);
                }
                //else
                //{
                //    // 如果没有点击到元件，显示点击位置的坐标
                //    MessageBox.Show($"坐标位置: X={worldPos.X:F2}, Y={worldPos.Y:F2}",
                //                  "坐标信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //}
            }
        }

        // 改进的元件查找方法
        private Component FindComponentAt(PointF worldPos)
        {
            // 反向遍历列表（后绘制的元件在前）
            for (int i = components.Count - 1; i >= 0; i--)
            {
                var component = components[i];

                // 首先检查边界矩形（快速排除）
                var bounds = component.GetBounds();
                if (!bounds.Contains(worldPos))
                    continue;

                // 然后进行精确的包含检测（考虑旋转）
                if (component.Contains(worldPos))
                    return component;
            }

            return null;
        }

        // 高亮被点击的元件
        private Component highlightedComponent = null;
        private void HighlightComponent(Component component)
        {
            highlightedComponent = component;
            Invalidate();

            // 2秒后取消高亮
            Timer highlightTimer = new Timer();
            highlightTimer.Interval = 2000;
            highlightTimer.Tick += (s, e) =>
            {
                highlightedComponent = null;
                Invalidate();
                highlightTimer.Stop();
                highlightTimer.Dispose();
            };
            highlightTimer.Start();
        }

        //计算世界坐标
        private PointF ScreenToWorld(Point screenPoint)
        {
            return new PointF(
                (screenPoint.X - panOffset.X) / scale,
                (screenPoint.Y - panOffset.Y) / scale
            );
        }

        //画到屏幕
        private PointF WorldToScreen(PointF worldPoint)
        {
            return new PointF(
                worldPoint.X * scale + panOffset.X,
                worldPoint.Y * scale + panOffset.Y
            );
        }

        public List<Component> Components
        {
            get => components;
            set
            {
                components = value;
                Invalidate();
            }
        }

        public float Scale
        {
            get => scale;
            set
            {
                scale = Math.Max(0.1f, Math.Min(5.0f, value));
                Invalidate();
            }
        }

        public bool ShowAxes
        {
            get => showAxes;
            set
            {
                showAxes = value;
                Invalidate();
            }
        }

        // 重置视图
        public void ResetView()
        {
            scale = 1.0f;
            panOffset = PointF.Empty;
            Invalidate();
        }

        // 居中显示所有元件
        public void FitToView()
        {
            if (components == null || components.Count == 0)
            {
                // 如果没有元件，重置到默认视图
                ResetView();
                return;
            }

            // 计算所有元件的边界
            var bounds = components[0].GetBounds();
            foreach (var comp in components)
            {
                bounds = RectangleF.Union(bounds, comp.GetBounds());
            }

            // 添加边距（元件尺寸的20%）
            float marginX = bounds.Width * 0.2f;
            float marginY = bounds.Height * 0.2f;
            bounds.Inflate(marginX, marginY);

            // 如果边界太小，设置最小尺寸
            if (bounds.Width < 50)
            {
                float centerX = bounds.X + bounds.Width / 2;
                bounds.X = centerX - 25;
                bounds.Width = 50;
            }
            if (bounds.Height < 50)
            {
                float centerY = bounds.Y + bounds.Height / 2;
                bounds.Y = centerY - 25;
                bounds.Height = 50;
            }

            // 计算缩放比例
            float scaleX = (Width - 40) / bounds.Width;
            float scaleY = (Height - 40) / bounds.Height;
            scale = Math.Min(scaleX, scaleY);

            // 确保缩放比例在合理范围内
            scale = Math.Max(0.1f, Math.Min(10.0f, scale));

            // 计算平移偏移量，使边界矩形居中
            panOffset = new PointF(
                (Width - bounds.Width * scale) / 2 - bounds.X * scale,
                (Height - bounds.Height * scale) / 2 - bounds.Y * scale
            );

            Invalidate();
        }

        //// 居中显示所有元件
        //public void FitToView()
        //{
        //    if (components.Count == 0) return;

        //    var bounds = components[0].GetBounds();
        //    foreach (var comp in components)
        //    {
        //        bounds = RectangleF.Union(bounds, comp.GetBounds());
        //    }

        //    float scaleX = (Width - 40) / bounds.Width;
        //    float scaleY = (Height - 40) / bounds.Height;
        //    scale = Math.Min(scaleX, scaleY) * 0.9f;

        //    panOffset = new PointF(
        //        (Width - bounds.Width * scale) / 2 - bounds.X * scale,
        //        (Height - bounds.Height * scale) / 2 - bounds.Y * scale
        //    );

        //    Invalidate();
        //}


        // 添加键盘快捷键支持
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Add:
                case Keys.Oemplus:
                    Scale *= 1.2f;
                    return true;

                case Keys.Subtract:
                case Keys.OemMinus:
                    Scale *= 0.8f;
                    return true;

                case Keys.Home:
                    FitToView();
                    return true;

                case Keys.Escape:
                    ResetView();
                    return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        // 元件双击事件
        public event EventHandler<ComponentEventArgs> ComponentDoubleClick;
        protected virtual void OnComponentDoubleClick(Component component)
        {
            ComponentDoubleClick?.Invoke(this, new ComponentEventArgs(component));
        }

        public class ComponentEventArgs : EventArgs
        {
            public Component Component { get; }

            public ComponentEventArgs(Component component)
            {
                Component = component;
            }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // 绘制背景和坐标轴
            DrawBackground(g);
            if (showAxes) DrawAxes(g);
            // 设置全局变换
            //g.TranslateTransform(panOffset.X, panOffset.Y);
            //g.ScaleTransform(scale, scale);
            // 绘制所有元件
            foreach (var component in components)
            {
                DrawComponent(g, component);
            }

            // 绘制状态信息
            DrawStatus(g);
        }

        private void DrawBackground(Graphics g)
        {
            g.Clear(Color.White);
        }

        //private void DrawAxes(Graphics g)
        //{
        //    using (Pen axesPen = new Pen(axesColor, 1.0f)) // 固定线宽
        //    using (Pen gridPen = new Pen(Color.FromArgb(50, axesColor), 1.0f)) // 固定线宽
        //    {
        //        // 获取当前可见的世界坐标范围
        //        RectangleF visibleWorld = GetVisibleWorldRect();
        //        // 水平网格线
        //        for (float y = (float)Math.Floor(visibleWorld.Top / gridSize) * gridSize;
        //             y <= visibleWorld.Bottom; y += gridSize)
        //        {
        //            PointF start = WorldToScreen(new PointF(visibleWorld.Left, y));
        //            PointF end = WorldToScreen(new PointF(visibleWorld.Right, y));
        //            g.DrawLine(gridPen, start, end);

        //            // 绘制Y轴刻度标签
        //            if (scale > 0.3f) // 只在足够放大时显示标签
        //            {
        //                string label = (y).ToString("F0");
        //                SizeF labelSize = g.MeasureString(label, SystemFonts.SmallCaptionFont);
        //                PointF labelPos = WorldToScreen(new PointF(visibleWorld.Left, y));
        //                g.DrawString(label, SystemFonts.SmallCaptionFont, Brushes.Black,
        //                           labelPos.X - labelSize.Width - 2, labelPos.Y - labelSize.Height / 2);
        //            }
        //        }

        //        // 垂直网格线
        //        for (float x = (float)Math.Floor(visibleWorld.Left / gridSize) * gridSize;
        //             x <= visibleWorld.Right; x += gridSize)
        //        {
        //            PointF start = WorldToScreen(new PointF(x, visibleWorld.Top));
        //            PointF end = WorldToScreen(new PointF(x, visibleWorld.Bottom));
        //            g.DrawLine(gridPen, start, end);

        //            // 绘制X轴刻度标签
        //            if (scale > 0.3f)
        //            {
        //                string label = (x).ToString("F0");
        //                SizeF labelSize = g.MeasureString(label, SystemFonts.SmallCaptionFont);
        //                PointF labelPos = WorldToScreen(new PointF(x, visibleWorld.Top));
        //                g.DrawString(label, SystemFonts.SmallCaptionFont, Brushes.Black,
        //                           labelPos.X - labelSize.Width / 2, labelPos.Y + 2);
        //            }
        //        }

        //        // 绘制坐标轴
        //        PointF xStart = WorldToScreen(new PointF(visibleWorld.Left, 0));
        //        PointF xEnd = WorldToScreen(new PointF(visibleWorld.Right, 0));
        //        PointF yStart = WorldToScreen(new PointF(0, visibleWorld.Top));
        //        PointF yEnd = WorldToScreen(new PointF(0, visibleWorld.Bottom));

        //        g.DrawLine(axesPen, xStart, xEnd); // X轴
        //        g.DrawLine(axesPen, yStart, yEnd); // Y轴

        //        // 绘制坐标轴箭头
        //        DrawAxisArrows(g, xEnd, yEnd);
        //    }
        //}

        //private RectangleF GetVisibleWorldRect()
        //{
        //    // 计算当前可见的世界坐标范围
        //    PointF topLeft = ScreenToWorld(new Point(0, 0));
        //    PointF bottomRight = ScreenToWorld(new Point(Width, Height));

        //    return new RectangleF(
        //        topLeft.X, topLeft.Y,
        //        bottomRight.X - topLeft.X,
        //        bottomRight.Y - topLeft.Y
        //    );
        //}

        private void DrawAxisArrows(Graphics g, PointF xEnd, PointF yEnd)
        {
            // 绘制X轴箭头
            float arrowSize = 8.0f;
            PointF[] xArrow = new PointF[]
            {
              xEnd,
              new PointF(xEnd.X - arrowSize, xEnd.Y - arrowSize / 2),
              new PointF(xEnd.X - arrowSize, xEnd.Y + arrowSize / 2)
            };
            g.FillPolygon(Brushes.Black, xArrow);

            // 绘制Y轴箭头
            PointF[] yArrow = new PointF[]
            {
             yEnd,
             new PointF(yEnd.X - arrowSize / 2, yEnd.Y + arrowSize),
             new PointF(yEnd.X + arrowSize / 2, yEnd.Y + arrowSize)
            };
            g.FillPolygon(Brushes.Black, yArrow);

            // 绘制坐标轴标签
            g.DrawString("X", SystemFonts.SmallCaptionFont, Brushes.Black, xEnd.X - 10, xEnd.Y + 5);
            g.DrawString("Y", SystemFonts.SmallCaptionFont, Brushes.Black, yEnd.X + 5, yEnd.Y - 10);
        }

        private void DrawComponent(Graphics g, Component component)
        {
            // 保存当前图形状态
            GraphicsState state = g.Save();
            try
            {
                // 计算元件在屏幕上的位置和尺寸
                PointF screenPos = WorldToScreen(component.Position);
                float screenWidth = component.Size.Width * scale;
                float screenHeight = component.Size.Height * scale;
                // 设置变换：先平移到元件中心，然后旋转
                g.TranslateTransform(screenPos.X, screenPos.Y);
                g.RotateTransform(component.Angle);
                // 判断是否是高亮元件
                bool isHighlighted = component == highlightedComponent;
                // 绘制元件矩形
                using (Pen pen = new Pen(isHighlighted ? Color.Red : component.Color,
                                       isHighlighted ? 3.0f : 1.0f))
                using (Brush fillBrush = new SolidBrush(Color.FromArgb(
                    isHighlighted ? 60 : 30,
                    isHighlighted ? Color.Red : component.Color)))
                {
                    RectangleF rect = new RectangleF(
                        -screenWidth / 2,
                        -screenHeight / 2,
                        screenWidth,
                        screenHeight
                    );

                    g.FillRectangle(fillBrush, rect);
                    g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
                }

                // 绘制位号文本
                using (Font font = new Font("Arial", Math.Max(3.0f, scale)))
                using (Brush textBrush = new SolidBrush(isHighlighted ? Color.Red : component.Color))
                {
                    StringFormat format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };

                    g.DrawString(component.Designator, font, textBrush, 0, 0, format);
                }

                // 如果是调试模式，绘制旋转后的边界
                if (isDebugMode)
                {
                    DrawDebugInfo(g, component, screenWidth, screenHeight);
                }
            }
            finally
            {
                // 恢复图形状态
                g.Restore(state);
            }
        }

        private void DrawAxes(Graphics g)
        {
            using (Pen axesPen = new Pen(axesColor, 1.0f)) // 固定线宽
            using (Pen gridPen = new Pen(Color.FromArgb(50, axesColor), 1.0f)) // 固定线宽
            {
                // 获取当前可见的世界坐标范围
                RectangleF visibleWorld = GetVisibleWorldRect();



                // 水平网格线
                for (float y = (float)Math.Floor(visibleWorld.Top / gridSize) * gridSize;
                     y <= visibleWorld.Bottom; y += gridSize)
                {
                    PointF start = WorldToScreen(new PointF(visibleWorld.Left, y));
                    PointF end = WorldToScreen(new PointF(visibleWorld.Right, y));
                    g.DrawLine(gridPen, start, end);

                    // 绘制Y轴刻度标签
                    if (scale > 0.3f) // 只在足够放大时显示标签
                    {
                        string label = (y).ToString("F0");
                        SizeF labelSize = g.MeasureString(label, SystemFonts.SmallCaptionFont);
                        PointF labelPos = WorldToScreen(new PointF(visibleWorld.Left, y));
                        g.DrawString(label, SystemFonts.SmallCaptionFont, Brushes.Black,
                                   labelPos.X - labelSize.Width - 2, labelPos.Y - labelSize.Height / 2);
                    }
                }

                // 垂直网格线
                for (float x = (float)Math.Floor(visibleWorld.Left / gridSize) * gridSize;
                     x <= visibleWorld.Right; x += gridSize)
                {
                    PointF start = WorldToScreen(new PointF(x, visibleWorld.Top));
                    PointF end = WorldToScreen(new PointF(x, visibleWorld.Bottom));
                    g.DrawLine(gridPen, start, end);

                    // 绘制X轴刻度标签
                    if (scale > 0.3f)
                    {
                        string label = (x).ToString("F0");
                        SizeF labelSize = g.MeasureString(label, SystemFonts.SmallCaptionFont);
                        PointF labelPos = WorldToScreen(new PointF(x, visibleWorld.Top));
                        g.DrawString(label, SystemFonts.SmallCaptionFont, Brushes.Black,
                                   labelPos.X - labelSize.Width / 2, labelPos.Y + 2);
                    }
                }

                // 绘制坐标轴
                PointF xStart = WorldToScreen(new PointF(visibleWorld.Left, 0));
                PointF xEnd = WorldToScreen(new PointF(visibleWorld.Right, 0));
                PointF yStart = WorldToScreen(new PointF(0, visibleWorld.Top));
                PointF yEnd = WorldToScreen(new PointF(0, visibleWorld.Bottom));

                g.DrawLine(axesPen, xStart, xEnd); // X轴
                g.DrawLine(axesPen, yStart, yEnd); // Y轴

                // 绘制坐标轴箭头
                DrawAxisArrows(g, xEnd, yEnd);
            }
        }

        private RectangleF GetVisibleWorldRect()
        {
            // 计算当前可见的世界坐标范围
            PointF topLeft = ScreenToWorld(new Point(0, 0));
            //topLeft= ScreenToWorld(new Point((int)topLeft.X, (int)topLeft.Y));
            PointF bottomRight = ScreenToWorld(new Point(Width, Height));
            //bottomRight = ScreenToWorld(new Point((int)bottomRight.X, (int)bottomRight.Y));
            return new RectangleF(
                topLeft.X, topLeft.Y,
                bottomRight.X - topLeft.X,
                bottomRight.Y - topLeft.Y
            );
        }

        // 调试信息绘制
        private bool isDebugMode = false; // 可以通过工具栏切换
        private void DrawDebugInfo(Graphics g, Component component, float width, float height)
        {
            // 绘制局部坐标系
            using (Pen axisPen = new Pen(Color.Blue, 1.0f))
            {
                g.DrawLine(axisPen, -width / 2, 0, width / 2, 0); // X轴
                g.DrawLine(axisPen, 0, -height / 2, 0, height / 2); // Y轴
            }

            // 绘制边界点
            using (Brush pointBrush = new SolidBrush(Color.Green))
            {
                float pointSize = Math.Max(2.0f, 3.0f / scale);
                g.FillEllipse(pointBrush, -width / 2 - pointSize / 2, -height / 2 - pointSize / 2, pointSize, pointSize);
                g.FillEllipse(pointBrush, width / 2 - pointSize / 2, -height / 2 - pointSize / 2, pointSize, pointSize);
                g.FillEllipse(pointBrush, -width / 2 - pointSize / 2, height / 2 - pointSize / 2, pointSize, pointSize);
                g.FillEllipse(pointBrush, width / 2 - pointSize / 2, height / 2 - pointSize / 2, pointSize, pointSize);
            }
        }

        private void DrawComponentMarks(Graphics g, float width, float height, Color color)
        {
            // 绘制引脚标记或其他元件特征
            using (Pen markPen = new Pen(color, 1.0f))
            {
                // 在元件四角绘制小圆点
                float markSize = Math.Max(2.0f, 3.0f / scale);

                // 左上角
                g.FillEllipse(Brushes.Red, -width / 2 - markSize / 2, -height / 2 - markSize / 2, markSize, markSize);
                // 右上角  
                g.FillEllipse(Brushes.Red, width / 2 - markSize / 2, -height / 2 - markSize / 2, markSize, markSize);
                // 左下角
                g.FillEllipse(Brushes.Red, -width / 2 - markSize / 2, height / 2 - markSize / 2, markSize, markSize);
                // 右下角
                g.FillEllipse(Brushes.Red, width / 2 - markSize / 2, height / 2 - markSize / 2, markSize, markSize);
            }
        }

        private void DrawStatus(Graphics g)
        {
            string status = $"缩放: {scale:F2}x | 坐标: X={mouseWorldPos.X / Component.radio:F2}, Y={mouseWorldPos.Y / Component.radio:F2}";
            using (Brush brush = new SolidBrush(Color.Black))
            using (Font font = new Font("Arial", 9))
            {
                g.DrawString(status, font, brush, 10, Height - 25);
            }
        }
    }
}
