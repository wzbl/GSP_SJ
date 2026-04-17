using SqlHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
                            color = Color.BlueViolet;
                        }
                        else if (row.ItemArray[6].ToString().Contains("0201"))
                        {
                            size = new Size(25, 13);
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
                            color = Color.Aquamarine;
                        }
                        else if (row.ItemArray[6].ToString().Contains("0805"))
                        {
                            size = new Size(86, 55);
                            color = Color.Cyan;
                        }
                        else if (row.ItemArray[6].ToString().Contains("1206"))
                        {
                            size = new Size(133, 68);
                            color = Color.Brown;
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
                            Component component = new Component(row.ItemArray[1].ToString(), new PointF(x * Component.radio, (-y) * Component.radio), new SizeF(size.Width * Component.radio / 25.0f, size.Height * Component.radio / 25.0f), angle, "null", color);
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

            double width = Width;
            double height = Height;
        }

        private void InitializeCanvas()
        {
            canvas = new ComponentCanvas()
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
                }
                canvas.FitToView();
            };
            ToolStripButton toolStrip4 = new ToolStripButton("水平Y镜像");//原点Y轴对称
            toolStrip4.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {
                    canvas.Components[i].Position = new PointF(canvas.Components[i].Position.X, -canvas.Components[i].Position.Y);
                }
                canvas.FitToView();
            };

            ToolStripButton toolStrip5 = new ToolStripButton("逆时针90");
            toolStrip5.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {
                    canvas.RotateCounterClockwise90(canvas.Components[i].Position.X, canvas.Components[i].Position.Y, out double x1, out double y1);
                    canvas.Components[i].Position = new PointF((float)x1, (float)y1);
                }
                canvas.FitToView();
            };
            ToolStripButton toolStrip6 = new ToolStripButton("顺时针90");
            toolStrip6.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {
                    canvas.RotateClockwise90(canvas.Components[i].Position.X, canvas.Components[i].Position.Y, out double x1, out double y1);
                    canvas.Components[i].Position = new PointF((float)x1, (float)y1);
                }
                canvas.FitToView();
            };

            ToolStripButton toolStrip7 = new ToolStripButton("旋转180");
            toolStrip7.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {
                    canvas.Components[i].Position = new PointF(-canvas.Components[i].Position.X, -canvas.Components[i].Position.Y);
                }
                canvas.FitToView();
            };

            ToolStripButton toolStrip8 = new ToolStripButton("元件水平X镜像");
            toolStrip8.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {
                    if (Math.Abs(Math.Sin(canvas.Components[i].Angle)) != 1)
                    {
                        canvas.Components[i].Angle = 180 - canvas.Components[i].Angle;
                    }
                }
                canvas.FitToView();
            };
            ToolStripButton toolStrip9 = new ToolStripButton("元件水平Y镜像");
            toolStrip9.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {
                    if (Math.Abs(Math.Sin(canvas.Components[i].Angle)) != 0)
                    {
                        canvas.Components[i].Angle = 360 - canvas.Components[i].Angle;
                        //if (Math.Abs(Math.Sin(canvas.Components[i].Angle)) == 1)
                        //    canvas.Components[i].Angle = canvas.Components[i].Angle - 180;
                        //else
                        //{
                        //    canvas.Components[i].Angle = canvas.Components[i].Angle - 180;
                        //}
                    }
                }
                canvas.FitToView();
            };

            ToolStripButton toolStrip10 = new ToolStripButton("元件逆时针90");
            toolStrip10.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {
                    canvas.Components[i].Angle = canvas.Components[i].Angle - 90;
                }
                canvas.FitToView();
            };
            ToolStripButton toolStrip11 = new ToolStripButton("元件顺时针90");
            toolStrip11.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {
                    canvas.Components[i].Angle = 90 + canvas.Components[i].Angle;
                }
                canvas.FitToView();
            };

            ToolStripButton toolStrip12 = new ToolStripButton("元件旋转180");
            toolStrip12.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {
                    canvas.Components[i].Angle = 180 + canvas.Components[i].Angle;
                }
                canvas.FitToView();
            };


            ToolStripButton toolStrip13 = new ToolStripButton("图像界面");
            toolStrip13.Click += (s, e) =>
            {
                Form3 form3 = new Form3(canvas);
                form3.ShowDialog();
            };

            ToolStripButton toolStrip14 = new ToolStripButton("保存数据");
            toolStrip14.Click += (s, e) =>
            {
                //for (int i = 0; i < canvas.Components.Count; i++)
                //{
                //    string cmd = "update Eng_XYData set X='" + canvas.Components[i].Position.X + "' ,Y= '" + canvas.Components[i].Position.Y + "' , Angle='" + canvas.Components[i].Angle + "' where ProductCode ='1212' and Position='" + canvas.Components[i].Designator + "'";
                //    SqlHelper.SQL.ExecuteQuery(cmd);
                //}

                //if (canvas.Image != null)
                {
                    if (File.Exists("C:\\Users\\14802\\Desktop\\1.bmp"))
                        File.Delete("C:\\Users\\14802\\Desktop\\1.bmp");
                    canvas.SaveToFile("C:\\Users\\14802\\Desktop\\1.bmp");
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
             toolStrip12,
             toolStrip13,
             toolStrip14
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (canvas.Image != null)
            {
                if (File.Exists("C:\\Users\\14802\\Desktop\\1.png"))
                    File.Delete("C:\\Users\\14802\\Desktop\\1.png");
                canvas.Image.Save("C:\\Users\\14802\\Desktop\\1.png");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3(canvas);
            form3.Show();
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

    public partial class ComponentCanvas : PictureBox
    {
        private List<Component> components = new List<Component>();
        private float scale = 1f;
        private PointF panOffset = PointF.Empty;
        private Point lastMousePos;
        private Point movePoint;
        private bool isPanning = false;
        private PointF mouseWorldPos = PointF.Empty;

        // 坐标轴设置
        private bool showAxes = true;
        private Color axesColor = Color.Red;
        private float gridSize = 50.0f;
        //150,270
        // 拖动模式
        private PanMode currentPanMode = PanMode.None;

        public int OrgWidth = 15000;
        public int orgHight = 8000;

        public string ProductCode = "";

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
                        this.Cursor = Cursors.SizeAll;
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
            movePoint = e.Location;
            if (isPanning && currentPanMode != PanMode.None)
            {
                // 计算鼠标移动距离
                int deltaX = e.X - lastMousePos.X;
                int deltaY = e.Y - lastMousePos.Y;
                // 更新平移偏移量
                panOffset.X += deltaX;
                panOffset.Y += deltaY;
                lastMousePos = e.Location;
            }
            Invalidate();
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
            scale = Math.Max(0.5f, Math.Min(25.0f, scale * zoomFactor));

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
        public Component FindComponentAt(PointF worldPos)
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

        /// <summary>
        /// 设置选中的坐标点
        /// </summary>
        /// <param name="name"></param>
        public void SetSelectPos(string name)
        {
            // 反向遍历列表（后绘制的元件在前）
            for (int i = components.Count - 1; i >= 0; i--)
            {
                if (components[i].Designator == name)
                {
                    highlightedComponent = components[i];
                    //视图移动到显示到选择的坐标点
                    scale = 10f;
                    Invalidate();
                    break;
                }
            }

        }

        // 高亮被点击的元件
        private Component highlightedComponent = null;
        private void HighlightComponent(Component component)
        {
            highlightedComponent = component;
            Invalidate();

            //// 2秒后取消高亮
            //Timer highlightTimer = new Timer();
            //highlightTimer.Interval = 2000;
            //highlightTimer.Tick += (s, e) =>
            //{
            //    highlightedComponent = null;
            //    Invalidate();
            //    highlightTimer.Stop();
            //    highlightTimer.Dispose();
            //};
            //highlightTimer.Start();
        }

        //计算世界坐标
        public PointF ScreenToWorld(Point screenPoint)
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
        public void OnComponentDoubleClick(Component component)
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

        private DrawingCommandRecorder _recorder = new DrawingCommandRecorder();
        private Rectangle _drawingBounds = new Rectangle(0, 0, 10000, 10000); // 大尺寸
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            _recorder.Clear();

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // 绘制背景和坐标轴
            DrawBackground(g);
            if (showAxes) DrawAxes(g);

            // 绘制所有元件
            foreach (var component in components)
            {
                DrawComponent(g, component);
            }

            // 绘制状态信息
            DrawStatus(g);
            //Bitmap bitmap = new Bitmap((int)(Width * scale), (int)(Height * scale));
            //using (Graphics g = Graphics.FromImage(bitmap))
            //{
            //    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            //    // 绘制背景和坐标轴
            //    DrawBackground(g);
            //    if (showAxes) DrawAxes(g);
            //    // 绘制所有元件
            //    foreach (var component in components)
            //    {
            //        DrawComponent(g, component);
            //    }
            //    // 绘制状态信息
            //    DrawStatus(g);
            //}

            //Image = bitmap;


        }

        #region 保存图片

        /// <summary>
        /// X坐标平移值
        /// </summary>
        public int XSaveOffset = 0;
        /// <summary>
        /// Y坐标平移值
        /// </summary>
        
        public int YSaveOffset = 0;

        public Bitmap fullImage = null;

        // 预览全图
        public void ShowFullImagePreview()
        {
            fullImage = null;
            using (fullImage = SaveToBitmap())
            {
                Form previewForm = new Form
                {
                    Text = "全图预览",
                    Size = new Size(Width, Height),
                    StartPosition = FormStartPosition.CenterParent
                };

                PictureBox pictureBox = new PictureBox
                {
                    Image = fullImage,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Dock = DockStyle.Fill
                };

                Button saveButton = new Button
                {
                    Text = "保存图片",
                    Dock = DockStyle.Bottom,
                    Height = 30
                };
                saveButton.Click += (s, e) =>
                {
                    using (SaveFileDialog saveDialog = new SaveFileDialog())
                    {
                        saveDialog.Filter = "BMP图片|*.bmp|JPEG图片|*.jpg|PNG图片|*.png";
                        saveDialog.FileName = $"{ProductCode}";

                        if (saveDialog.ShowDialog() == DialogResult.OK)
                        {
                            System.Drawing.Imaging.ImageFormat format = System.Drawing.Imaging.ImageFormat.Bmp;
                            switch (Path.GetExtension(saveDialog.FileName).ToLower())
                            {
                                case ".jpg":
                                case ".jpeg":
                                    format = System.Drawing.Imaging.ImageFormat.Jpeg;
                                    break;
                                case ".bmp":
                                    format = System.Drawing.Imaging.ImageFormat.Bmp;
                                    break;
                            }

                            fullImage.Save(saveDialog.FileName, format);
                            //保存一份到数据库
                            byte[] bytes = PublicFunction.GetByte(saveDialog.FileName);
                            SQLDataControl.UpdateProgramOptionPicture(ProductCode, "", bytes);
                            MessageBox.Show("全图保存成功！", "保存成功");
                        }
                    }
                };
    
                previewForm.Controls.Add(pictureBox);
                previewForm.Controls.Add(saveButton);
                previewForm.ShowDialog();
            }
        }

        // 计算所有元件的边界
        private RectangleF CalculateAllComponentsBounds()
        {
            if (components == null || components.Count == 0)
                return new RectangleF(0, 0, 100, 100);

            var bounds = components[0].GetBounds();

            foreach (var comp in components)
            {
                bounds = RectangleF.Union(bounds, comp.GetBounds());
            }

            return bounds;
        }

        private void SaveDrawAxes(Graphics g, float scaw)
        {
            using (Pen axesPen = new Pen(axesColor, 5.0f)) // 固定线宽
            using (Pen gridPen = new Pen(Color.FromArgb(30, axesColor), 5.0f)) // 固定线宽
            {
                // 获取当前可见的世界坐标范围
                RectangleF visibleWorld = new RectangleF(0, 0, OrgWidth, orgHight);
                float gridSize = 50 * scaw;
                int rX = 1;
                int ry = 1;

                if (components[0].Position.X > 0 && components[0].Position.Y > 0)
                {
                    //第四向限
                    PointF xStart = new PointF(0, SaveOffset);
                    PointF xEnd = new PointF(visibleWorld.Right, SaveOffset);
                    PointF yStart = new PointF(SaveOffset, 0);
                    PointF yEnd = new PointF(SaveOffset, visibleWorld.Bottom);
                    g.DrawLine(axesPen, xStart, xEnd); // X轴
                    g.DrawLine(axesPen, yStart, yEnd); // Y轴
                    DrawAxisArrows(g, xEnd, yEnd);


                    // 水平网格线
                    for (float y = (float)Math.Floor(visibleWorld.Top / gridSize) * gridSize;
                         y <= visibleWorld.Bottom; y += gridSize)
                    {
                        PointF start = new PointF(visibleWorld.Left, y + SaveOffset);
                        PointF end = new PointF(visibleWorld.Right, y + SaveOffset);
                        g.DrawLine(gridPen, start, end);

                        // 绘制Y轴刻度标签
                        if (scale > 0.3f) // 只在足够放大时显示标签
                        {
                            string label = (-y / scaw).ToString("F0");
                            SizeF labelSize = g.MeasureString(label, SystemFonts.SmallCaptionFont);
                            PointF labelPos = new PointF(SaveOffset, y + SaveOffset);
                            g.DrawString(label, new Font("Arial", Math.Max(100.0f, scaw)), Brushes.Black,
                                       labelPos.X - labelSize.Width + 25, labelPos.Y - labelSize.Height / 2);
                        }
                    }

                    // 垂直网格线
                    for (float x = (float)Math.Floor(visibleWorld.Left / gridSize) * gridSize;
                         x <= visibleWorld.Right; x += gridSize)
                    {
                        PointF start = new PointF(x + SaveOffset, visibleWorld.Top);
                        PointF end = new PointF(x + SaveOffset, visibleWorld.Bottom);
                        g.DrawLine(gridPen, start, end);
                        // 绘制X轴刻度标签
                        if (scale > 0.3f)
                        {
                            string label = (x / scaw).ToString("F0");
                            SizeF labelSize = g.MeasureString(label, SystemFonts.DefaultFont);
                            PointF labelPos = new PointF(x + SaveOffset, SaveOffset);
                            g.DrawString(label, new Font("Arial", Math.Max(100.0f, scaw)), Brushes.Black,
                                       labelPos.X - labelSize.Width / 2, labelPos.Y + 2);
                            //g.DrawString(label, SystemFonts.SmallCaptionFont, Brushes.Black,
                            //       labelPos.X - labelSize.Width / 2, 0);
                        }
                    }
                }
                else if (components[0].Position.X < 0 && components[0].Position.Y > 0)
                {
                    //第三向限
                    PointF xStart = new PointF(0, SaveOffset);
                    PointF xEnd = new PointF(visibleWorld.Right, SaveOffset);
                    PointF yStart = new PointF(OrgWidth - SaveOffset, 0);
                    PointF yEnd = new PointF(OrgWidth - SaveOffset, visibleWorld.Bottom);
                    g.DrawLine(axesPen, xStart, xEnd); // X轴
                    g.DrawLine(axesPen, yStart, yEnd); // Y轴
                    DrawAxisArrows(g, xEnd, yEnd);
                    rX = -1;


                    // 水平网格线
                    for (float y = (float)Math.Floor(visibleWorld.Top / gridSize) * gridSize;
                         y <= visibleWorld.Bottom; y += gridSize)
                    {
                        PointF start = new PointF(visibleWorld.Left, y + SaveOffset);
                        PointF end = new PointF(visibleWorld.Right, y + SaveOffset);
                        g.DrawLine(gridPen, start, end);

                        // 绘制Y轴刻度标签
                        if (scale > 0.3f) // 只在足够放大时显示标签
                        {
                            string label = (-y / scaw).ToString("F0");
                            SizeF labelSize = g.MeasureString(label, SystemFonts.SmallCaptionFont);
                            PointF labelPos = new PointF(OrgWidth - SaveOffset, y + SaveOffset);
                            g.DrawString(label, new Font("Arial", Math.Max(100.0f, scaw)), Brushes.Black,
                                       labelPos.X - labelSize.Width + 25, labelPos.Y - labelSize.Height / 2);
                        }
                    }

                    // 垂直网格线
                    for (float x = (float)OrgWidth; x >= 0; x -= gridSize)
                    {
                        PointF start = new PointF(x - SaveOffset, visibleWorld.Top);
                        PointF end = new PointF(x - SaveOffset, visibleWorld.Bottom);
                        g.DrawLine(gridPen, start, end);
                        // 绘制X轴刻度标签
                        if (scale > 0.3f)
                        {
                            string label = (-(OrgWidth - x) / scaw).ToString("F0");
                            SizeF labelSize = g.MeasureString(label, SystemFonts.DefaultFont);
                            PointF labelPos = new PointF(x - SaveOffset, SaveOffset);
                            g.DrawString(label, new Font("Arial", Math.Max(100.0f, scaw)), Brushes.Black,
                                       labelPos.X - labelSize.Width / 2, labelPos.Y + 2);
                        }
                    }

                }
                else if (components[0].Position.X < 0 && components[0].Position.Y < 0)
                {
                    //第二向限
                    PointF xStart = new PointF(0, orgHight - SaveOffset);
                    PointF xEnd = new PointF(visibleWorld.Right, orgHight - SaveOffset);
                    PointF yStart = new PointF(OrgWidth - SaveOffset, 0);
                    PointF yEnd = new PointF(OrgWidth - SaveOffset, visibleWorld.Bottom);
                    g.DrawLine(axesPen, xStart, xEnd); // X轴
                    g.DrawLine(axesPen, yStart, yEnd); // Y轴
                    DrawAxisArrows(g, xEnd, yEnd);
                    rX = -1;
                    ry = -1;

                    // 水平网格线
                    for (float y = (float)orgHight; y >= 0; y -= gridSize)
                    {
                        PointF start = new PointF(visibleWorld.Left, y - SaveOffset);
                        PointF end = new PointF(visibleWorld.Right, y - SaveOffset);
                        g.DrawLine(gridPen, start, end);

                        // 绘制Y轴刻度标签
                        if (scale > 0.3f) // 只在足够放大时显示标签
                        {
                            string label = ((orgHight - y) / scaw).ToString("F0");
                            SizeF labelSize = g.MeasureString(label, SystemFonts.SmallCaptionFont);
                            PointF labelPos = new PointF(OrgWidth - SaveOffset, y - SaveOffset);
                            g.DrawString(label, new Font("Arial", Math.Max(100.0f, scaw)), Brushes.Black,
                                       labelPos.X - labelSize.Width + 25, labelPos.Y - labelSize.Height / 2);
                        }
                    }

                    // 垂直网格线
                    for (float x = (float)OrgWidth; x >= 0; x -= gridSize)
                    {
                        PointF start = new PointF(x - SaveOffset, visibleWorld.Top);
                        PointF end = new PointF(x - SaveOffset, visibleWorld.Bottom);
                        g.DrawLine(gridPen, start, end);
                        // 绘制X轴刻度标签
                        if (scale > 0.3f)
                        {
                            string label = (-(OrgWidth - x) / scaw).ToString("F0");
                            SizeF labelSize = g.MeasureString(label, SystemFonts.DefaultFont);
                            PointF labelPos = new PointF(x - SaveOffset, orgHight - SaveOffset);
                            g.DrawString(label, new Font("Arial", Math.Max(100.0f, scaw)), Brushes.Black,
                                       labelPos.X - labelSize.Width / 2, labelPos.Y + 2);
                        }
                    }

                }
                else
                {
                    //第一向限
                    PointF xStart = new PointF(0, orgHight - SaveOffset);
                    PointF xEnd = new PointF(visibleWorld.Right, orgHight - SaveOffset);
                    PointF yStart = new PointF(SaveOffset, 0);
                    PointF yEnd = new PointF(SaveOffset, visibleWorld.Bottom);
                    g.DrawLine(axesPen, xStart, xEnd); // X轴
                    g.DrawLine(axesPen, yStart, yEnd); // Y轴
                    DrawAxisArrows(g, xEnd, yEnd);
                    ry = -1;


                    // 水平网格线
                    for (float y = (float)orgHight; y >= 0; y -= gridSize)
                    {
                        PointF start = new PointF(visibleWorld.Left, y - SaveOffset);
                        PointF end = new PointF(visibleWorld.Right, y - SaveOffset);
                        g.DrawLine(gridPen, start, end);

                        // 绘制Y轴刻度标签
                        if (scale > 0.3f) // 只在足够放大时显示标签
                        {
                            string label = ((orgHight - y) / scaw).ToString("F0");
                            SizeF labelSize = g.MeasureString(label, SystemFonts.SmallCaptionFont);
                            PointF labelPos = new PointF(SaveOffset, y - SaveOffset);
                            g.DrawString(label, new Font("Arial", Math.Max(100.0f, scaw)), Brushes.Black,
                                       labelPos.X - labelSize.Width + 25, labelPos.Y - labelSize.Height / 2);
                        }
                    }

                    // 垂直网格线
                    for (float x = (float)Math.Floor(visibleWorld.Left / gridSize) * gridSize;
                         x <= visibleWorld.Right; x += gridSize)
                    {
                        PointF start = new PointF(x + SaveOffset, visibleWorld.Top);
                        PointF end = new PointF(x + SaveOffset, visibleWorld.Bottom);
                        g.DrawLine(gridPen, start, end);
                        // 绘制X轴刻度标签
                        if (scale > 0.3f)
                        {
                            string label = (x / scaw).ToString("F0");
                            SizeF labelSize = g.MeasureString(label, SystemFonts.DefaultFont);
                            PointF labelPos = new PointF(x + SaveOffset, orgHight - SaveOffset);
                            g.DrawString(label, new Font("Arial", Math.Max(100.0f, scaw)), Brushes.Black,
                                       labelPos.X - labelSize.Width / 2, labelPos.Y + 2);
                        }
                    }
                }




            }
        }

        /// <summary>
        /// 整体偏移100个像素
        /// </summary>
        public int SaveOffset = 500;
        private void SaveDrawComponent(Graphics g, Component component, float scaw)
        {
            // 保存当前图形状态
            GraphicsState state = g.Save();
            try
            {
                // 计算元件在屏幕上的位置和尺寸
                PointF screenPos = new PointF(component.Position.X * scaw, component.Position.Y * scaw);
                float screenWidth = component.Size.Width * scaw;
                float screenHeight = component.Size.Height * scaw;
                //// 设置变换：先平移到元件中心，然后旋转

                g.TranslateTransform(screenPos.X + XSaveOffset, screenPos.Y + YSaveOffset);
                g.RotateTransform(component.Angle);


                //记录屏幕坐标
                PointF screenPos1 = new PointF(screenPos.X + XSaveOffset, screenPos.Y + YSaveOffset);
                //string cmd = "update Eng_XYData set RX='" + screenPos1.X + "' ,RY= '" + screenPos1.Y + "' where ProductCode = " + ProductCode + " and Position='" + component.Designator + "'";
                //SqlHelper.SQL.ExecuteQuery(cmd);
                //跟新屏幕坐标到数据库
                SQLDataControl.UpdateXYData_RXY(ProductCode, component.Designator, (decimal)screenPos1.X, (decimal)screenPos1.Y);
                // 绘制元件矩形
                using (Pen pen = new Pen(component.Color, 1.0f))
                using (Brush fillBrush = new SolidBrush(Color.FromArgb(30, component.Color)))
                {
                    RectangleF rect = new RectangleF(
                        -screenWidth / 2,
                        -screenHeight / 2,
                        screenWidth,
                        screenHeight
                    );

                    g.FillRectangle(fillBrush, rect);
                    g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
                    _recorder.RecordRectangle(rect, component.Color, 3.0f);
                }


                // 绘制位号文本
                using (Font font = new Font("Arial", Math.Max(3.0f, scaw)))
                using (Brush textBrush = new SolidBrush(component.Color))
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

        public float SaveScale = 1f;
        private Bitmap SaveToBitmap()
        {
            // 创建与控件相同大小的位图
            Bitmap bitmap = new Bitmap(OrgWidth, orgHight);
            RectangleF rectangleF = CalculateAllComponentsBounds();
            float scaw = OrgWidth * 1.0f / (rectangleF.Width + 30);
            float scah = orgHight * 1.0f / (rectangleF.Height + 40);

            if (components[0].Position.X > 0 && components[0].Position.Y > 0)
            {
                //第四向限
                XSaveOffset = 0 + SaveOffset;
                YSaveOffset = 0 + SaveOffset;
            }
            else if (components[0].Position.X < 0 && components[0].Position.Y > 0)
            {
                //第三向限
                XSaveOffset = OrgWidth - SaveOffset;
                YSaveOffset = 0 + SaveOffset;
            }
            else if (components[0].Position.X < 0 && components[0].Position.Y < 0)
            {
                //第二向限
                XSaveOffset = OrgWidth - SaveOffset;
                YSaveOffset = orgHight - SaveOffset;
            }
            else
            {
                //第一向限
                XSaveOffset = SaveOffset;
                YSaveOffset = orgHight - SaveOffset;
            }

            if (scaw < scah)
                SaveScale = scaw;
            else
                SaveScale = scah;

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.Clear(Color.White);

                // 保存当前图形状态
                GraphicsState state = g.Save();

                try
                {
                    // 绘制坐标轴和网格
                    SaveDrawAxes(g, SaveScale);

                    // 绘制所有元件
                    foreach (var component in components)
                    {
                        SaveDrawComponent(g, component, SaveScale);
                    }

                    g.DrawString(ProductCode, new Font("Arial", 200), Brushes.Black, OrgWidth/2, 100);
                }
                finally
                {
                    // 恢复图形状态
                    g.Restore(state);
                }
            }

            return bitmap;
        }

        // 保存为文件
        public void SaveToFile(string filePath, System.Drawing.Imaging.ImageFormat format = null)
        {
            ShowFullImagePreview();
            //if (format == null)
            //    format = System.Drawing.Imaging.ImageFormat.Bmp;

            //using (Bitmap bitmap = SaveToBitmap())
            //{
            //    bitmap.Save(filePath, format);
            //}

        }
        #endregion




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
                    _recorder.RecordRectangle(rect, component.Color, 3.0f);
                }

                if (isHighlighted)
                {

                    using (Pen pen = new Pen(Color.Red, 3.0f))
                    using (Brush fillBrush = new SolidBrush(Color.FromArgb(60, Color.Red)))
                    {
                        RectangleF rect = new RectangleF(
                             -screenWidth / 2,
                        -screenHeight / 2,
                        screenWidth,
                        screenHeight
                        );
                        g.DrawLine(pen, rect.X + rect.Width / 2, rect.Y + rect.Height, rect.X + rect.Width / 2, rect.Y + rect.Height + 100);
                        g.DrawLine(pen, rect.X + rect.Width / 2, rect.Y, rect.X + rect.Width / 2, rect.Y - 100);

                        g.DrawLine(pen, rect.X, rect.Y + rect.Height / 2, rect.X - 100, rect.Y + rect.Height / 2);
                        g.DrawLine(pen, rect.X + rect.Width, rect.Y + rect.Height / 2, rect.X + rect.Width + 100, rect.Y + rect.Height / 2);


                    }
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

        private void DrawAxes(Graphics g, bool saveImg = false)
        {
            using (Pen axesPen = new Pen(axesColor, 1.0f)) // 固定线宽
            using (Pen gridPen = new Pen(Color.FromArgb(50, axesColor), 1.0f)) // 固定线宽
            {
                // 获取当前可见的世界坐标范围
                RectangleF visibleWorld = GetVisibleWorldRect(saveImg);


                // 绘制坐标轴
                PointF xStart = WorldToScreen(new PointF(visibleWorld.Left, 0));
                PointF xEnd = WorldToScreen(new PointF(visibleWorld.Right, 0));
                PointF yStart = WorldToScreen(new PointF(0, visibleWorld.Top));
                PointF yEnd = WorldToScreen(new PointF(0, visibleWorld.Bottom));

                g.DrawLine(axesPen, xStart, xEnd); // X轴
                g.DrawLine(axesPen, yStart, yEnd); // Y轴


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
                        string label = (-y).ToString("F0");
                        SizeF labelSize = g.MeasureString(label, SystemFonts.SmallCaptionFont);
                        PointF labelPos = WorldToScreen(new PointF(visibleWorld.Left + 10, y));
                        g.DrawString(label, SystemFonts.SmallCaptionFont, Brushes.Black,
                                   labelPos.X - labelSize.Width + 25, labelPos.Y - labelSize.Height / 2);

                        //g.DrawString(label, SystemFonts.SmallCaptionFont, Brushes.Black,
                        //           0, labelPos.Y - labelSize.Height / 2);
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
                        //g.DrawString(label, SystemFonts.SmallCaptionFont, Brushes.Black,
                        //       labelPos.X - labelSize.Width / 2, 0);
                    }
                }

                // 绘制坐标轴箭头
                DrawAxisArrows(g, xEnd, yEnd);
            }
        }

        private RectangleF GetVisibleWorldRect(bool saveImg = false)
        {
            // 计算当前可见的世界坐标范围
            PointF topLeft = ScreenToWorld(new Point(0, 0));
            //topLeft= ScreenToWorld(new Point((int)topLeft.X, (int)topLeft.Y));
            PointF bottomRight = ScreenToWorld(new Point(Width, Height));
            //bottomRight = ScreenToWorld(new Point((int)bottomRight.X, (int)bottomRight.Y));


            if (saveImg)
            {
                int width = (int)((bottomRight.X - topLeft.X) * scale);
                int height = (int)((bottomRight.Y - topLeft.Y) * scale);

                return new RectangleF(
              topLeft.X, topLeft.Y,
             width,
             height
               );
            }
            else
            {
                return new RectangleF(
              topLeft.X, topLeft.Y,
              bottomRight.X - topLeft.X,
              bottomRight.Y - topLeft.Y
          );
            }



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
            string status = $"缩放: {scale:F2}x | 坐标: X={mouseWorldPos.X:F2}, Y={-mouseWorldPos.Y:F2}|屏幕坐标: X={movePoint.X:F2}, Y={movePoint.Y:F2}|产品编码:{ProductCode}";
            using (Brush brush = new SolidBrush(Color.Black))
            using (Font font = new Font("Arial", 9))
            {
                g.DrawString(status, font, brush, 10, Height - 25);
            }

        }


        // 顺时针旋转90度
        public void RotateClockwise90(double x, double y, out double X1, out double Y1)
        {
            X1 = y;
            Y1 = -x;
        }

        // 逆时针旋转90度
        public void RotateCounterClockwise90(double x, double y, out double X1, out double Y1)
        {
            X1 = -y;
            Y1 = x;
        }

    }


    public class GraphicsRecorder
    {

        public static bool SaveImg = false;
        // 记录 Graphics 绘图操作为图元文件
        public static byte[] RecordGraphicsToMetafile(Graphics graphics, Rectangle bounds, Action<Graphics> drawAction)
        {
            SaveImg = false;
            using (var memoryStream = new MemoryStream())
            {
                // 创建图元文件
                using (var metafile = new Metafile(memoryStream, graphics.GetHdc()))
                using (var recordedGraphics = Graphics.FromImage(metafile))
                {
                    // 执行绘图操作
                    drawAction(recordedGraphics);
                }

                graphics.ReleaseHdc();
                return memoryStream.ToArray();
            }
        }

        // 从图元文件二进制数据重新播放
        public static void PlayMetafileFromBinary(byte[] metafileData, Graphics targetGraphics)
        {
            using (var memoryStream = new MemoryStream(metafileData))
            using (var metafile = new Metafile(memoryStream))
            {
                targetGraphics.DrawImage(metafile, Point.Empty);
            }
        }
    }


    [Serializable]
    public abstract class DrawingCommand
    {
        public abstract void Execute(Graphics g);
    }

    [Serializable]
    public class DrawLineCommand : DrawingCommand
    {
        public PointF Start { get; set; }
        public PointF End { get; set; }
        public Color Color { get; set; }
        public float Width { get; set; }

        public override void Execute(Graphics g)
        {
            using (var pen = new Pen(Color, Width))
                g.DrawLine(pen, Start, End);
        }
    }

    [Serializable]
    public class DrawRectangleCommand : DrawingCommand
    {
        public RectangleF Rect { get; set; }
        public Color Color { get; set; }
        public float Width { get; set; }

        public override void Execute(Graphics g)
        {
            Pen pen = new Pen(Color);
            using (Brush fillBrush = new SolidBrush(Color.FromArgb(30, Color)))
            {
                g.FillRectangle(fillBrush, Rect);
                g.DrawRectangle(pen, Rect.X, Rect.Y, Rect.Width, Rect.Height);
            }
        }
    }

    [Serializable]
    public class DrawStringCommand : DrawingCommand
    {
        public string Text { get; set; }
        public PointF Location { get; set; }
        public string FontName { get; set; }
        public float FontSize { get; set; }
        public Color Color { get; set; }

        public override void Execute(Graphics g)
        {
            using (var font = new Font(FontName, FontSize))
            using (var brush = new SolidBrush(Color))
                g.DrawString(Text, font, brush, Location);
        }
    }

    public class DrawingCommandRecorder
    {
        private List<DrawingCommand> _commands = new List<DrawingCommand>();

        public void Clear()
        {
            _commands.Clear();
        }

        public void RecordLine(PointF start, PointF end, Color color, float width = 1)
        {
            _commands.Add(new DrawLineCommand
            {
                Start = start,
                End = end,
                Color = color,
                Width = width
            });
        }

        public void RecordRectangle(RectangleF rect, Color color, float width = 1)
        {
            _commands.Add(new DrawRectangleCommand
            {
                Rect = rect,
                Color = color,
                Width = width
            });
        }

        public void RecordString(string text, PointF location, string fontName, float fontSize, Color color)
        {
            _commands.Add(new DrawStringCommand
            {
                Text = text,
                Location = location,
                FontName = fontName,
                FontSize = fontSize,
                Color = color
            });
        }

        // 序列化为二进制数据
        public byte[] SerializeToBinary()
        {
            using (var memoryStream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, _commands);
                return memoryStream.ToArray();
            }
        }

        // 从二进制数据反序列化
        public static DrawingCommandRecorder DeserializeFromBinary(byte[] data)
        {
            using (var memoryStream = new MemoryStream(data))
            {
                var formatter = new BinaryFormatter();
                var commands = (List<DrawingCommand>)formatter.Deserialize(memoryStream);

                var recorder = new DrawingCommandRecorder();
                // 这里需要反射设置私有字段，或者提供公共方法添加命令
                return recorder;
            }
        }

        // 重放所有绘图命令
        public void Replay(Graphics graphics)
        {
            foreach (var command in _commands)
            {
                command.Execute(graphics);
            }
        }
    }


}
