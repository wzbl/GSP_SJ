using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace GSP_SJ.ModelClass
{
    public class ROI
    {
        public Rectangle Bounds { get; set; }
        public string Name { get; set; }
        public Color Color { get; set; } = Color.Red;
        public bool IsSelected { get; set; }

        public int Left
        {
            get
            {
                return Bounds.Left;
            }
        }

        public int Right
        {
            get
            {
                return Bounds.Right;
            }
        }

        public int Top
        {
            get
            {
                return Bounds.Top;
            }
        }

        public int Bottom
        {
            get
            {
                return Bounds.Bottom;
            }
        }

        // 控制点大小
        private const int HandleSize = 8;

        public ROI(Rectangle bounds, string name = "")
        {
            Bounds = bounds;
            Name = name;
        }

        // 绘制ROI
        public virtual void Draw(Graphics g)
        {
            using (Pen pen = new Pen(Color, IsSelected ? 2f : 1f))
            using (Brush fillBrush = new SolidBrush(Color.FromArgb(50, Color)))
            {
                // 填充矩形
                g.FillRectangle(fillBrush, Bounds);
                // 绘制边框
                g.DrawRectangle(pen, Bounds);

                // 如果被选中，绘制控制点
                if (IsSelected)
                {
                    DrawHandles(g);
                }
            }

            // 绘制名称
            if (!string.IsNullOrEmpty(Name))
            {
                using (Brush textBrush = new SolidBrush(Color))
                using (Font font = new Font("Arial", 8))
                {
                    g.DrawString(Name, font, textBrush, Bounds.Location);
                }
            }
        }

        // 绘制控制点
        protected virtual void DrawHandles(Graphics g)
        {
            using (Brush handleBrush = new SolidBrush(Color.White))
            using (Pen handlePen = new Pen(Color.Black))
            {
                // 四个角
                Rectangle[] handles = new Rectangle[]
                {
                new Rectangle(Bounds.Left - HandleSize/2, Bounds.Top - HandleSize/2, HandleSize, HandleSize), // 左上
                new Rectangle(Bounds.Right - HandleSize/2, Bounds.Top - HandleSize/2, HandleSize, HandleSize), // 右上
                new Rectangle(Bounds.Left - HandleSize/2, Bounds.Bottom - HandleSize/2, HandleSize, HandleSize), // 左下
                new Rectangle(Bounds.Right - HandleSize/2, Bounds.Bottom - HandleSize/2, HandleSize, HandleSize)  // 右下
                };

                foreach (var handle in handles)
                {
                    g.FillRectangle(handleBrush, handle);
                    g.DrawRectangle(handlePen, handle);
                }
            }
        }

        // 检查点是否在ROI内
        public virtual bool Contains(Point point)
        {
            return Bounds.Contains(point);
        }

        // 获取控制点类型
        public virtual HandleType GetHandleAtPoint(Point point)
        {
            if (!IsSelected) return HandleType.None;

            Rectangle[] handles = new Rectangle[]
            {
            new Rectangle(Bounds.Left - HandleSize/2, Bounds.Top - HandleSize/2, HandleSize, HandleSize),
            new Rectangle(Bounds.Right - HandleSize/2, Bounds.Top - HandleSize/2, HandleSize, HandleSize),
            new Rectangle(Bounds.Left - HandleSize/2, Bounds.Bottom - HandleSize/2, HandleSize, HandleSize),
            new Rectangle(Bounds.Right - HandleSize/2, Bounds.Bottom - HandleSize/2, HandleSize, HandleSize)
            };

            for (int i = 0; i < handles.Length; i++)
            {
                if (handles[i].Contains(point))
                    return (HandleType)(i + 1);
            }

            return Bounds.Contains(point) ? HandleType.Move : HandleType.None;
        }
    }

    public class RectangularROI : DirectionalROI
    {
        public RectangularROI(Rectangle bounds, string name = "") : base(bounds, name)
        {
        }
    }

    public class DirectionalROIPictureBox : PictureBox
    {
        private List<DirectionalROI> rois = new List<DirectionalROI>();
        private DirectionalROI selectedROI;
        private DirectionalROI hoveredROI;
        private Point lastMousePosition;
        private HandleType currentHandle = HandleType.None;
        private bool isDrawing = false;
        private Point drawingStart;
        private Rectangle drawingRect;
        private ROIType drawingType = ROIType.Rectangle;

        private ROIMode mode = ROIMode.Select;
        private Rectangle imageRect;
        private Size imageSize;

        public DirectionalROIPictureBox()
        {
            this.DoubleBuffered = true;
            this.SizeMode = PictureBoxSizeMode.Zoom;
            this.MouseDown += OnMouseDown;
            this.MouseMove += OnMouseMove;
            this.MouseUp += OnMouseUp;
            this.Paint += OnPaint;
            this.Resize += OnResize;
            this.KeyDown += OnKeyDown;
        }

        // 公共属性
        public List<DirectionalROI> ROIs => rois;
        public DirectionalROI SelectedROI => selectedROI;
        public ROIMode Mode
        {
            get => mode;
            set
            {
                mode = value;
                if (mode != ROIMode.Select && selectedROI != null)
                {
                    selectedROI.IsSelected = false;
                    selectedROI = null;
                }
                Invalidate();
            }
        }

        public ROIType DrawingType
        {
            get => drawingType;
            set
            {
                drawingType = value;
                if (mode == ROIMode.Draw)
                {
                    Invalidate();
                }
            }
        }

        // 重写Image属性
        public new Image Image
        {
            get => base.Image;
            set
            {
                base.Image = value;
                if (value != null)
                {
                    imageSize = value.Size;
                    CalculateImageRect();
                }
            }
        }

        private void CalculateImageRect()
        {
            if (Image == null) return;

            float ratioX = (float)ClientSize.Width / imageSize.Width;
            float ratioY = (float)ClientSize.Height / imageSize.Height;
            float ratio = Math.Min(ratioX, ratioY);

            int displayWidth = (int)(imageSize.Width * ratio);
            int displayHeight = (int)(imageSize.Height * ratio);

            int x = (ClientSize.Width - displayWidth) / 2;
            int y = (ClientSize.Height - displayHeight) / 2;

            imageRect = new Rectangle(x, y, displayWidth, displayHeight);
        }

        private Point ScreenToImage(Point screenPoint)
        {
            if (Image == null || imageRect.IsEmpty) return screenPoint;

            float scaleX = (float)imageSize.Width / imageRect.Width;
            float scaleY = (float)imageSize.Height / imageRect.Height;

            int imageX = (int)((screenPoint.X - imageRect.X) * scaleX);
            int imageY = (int)((screenPoint.Y - imageRect.Y) * scaleY);

            imageX = Math.Max(0, Math.Min(imageX, imageSize.Width - 1));
            imageY = Math.Max(0, Math.Min(imageY, imageSize.Height - 1));

            return new Point(imageX, imageY);
        }

        private Point ImageToScreen(Point imagePoint)
        {
            if (Image == null || imageRect.IsEmpty) return imagePoint;

            float scaleX = (float)imageRect.Width / imageSize.Width;
            float scaleY = (float)imageRect.Height / imageSize.Height;

            int screenX = (int)(imagePoint.X * scaleX + imageRect.X);
            int screenY = (int)(imagePoint.Y * scaleY + imageRect.Y);

            return new Point(screenX, screenY);
        }

        private Rectangle ImageToScreen(Rectangle imageRect)
        {
            Point screenLoc = ImageToScreen(imageRect.Location);
            Point screenBR = ImageToScreen(new Point(imageRect.Right, imageRect.Bottom));

            return new Rectangle(screenLoc.X, screenLoc.Y, screenBR.X - screenLoc.X, screenBR.Y - screenLoc.Y);
        }

        private void OnResize(object sender, EventArgs e)
        {
            CalculateImageRect();
            Invalidate();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (selectedROI == null) return;

            float angleStep = e.Control ? 1f : 5f; // Ctrl键按下时微调

            switch (e.KeyCode)
            {
                case Keys.Left:
                    selectedROI.Angle -= angleStep;
                    Invalidate();
                    break;
                case Keys.Right:
                    selectedROI.Angle += angleStep;
                    Invalidate();
                    break;
                case Keys.R:
                    selectedROI.Angle = 0; // 重置角度
                    Invalidate();
                    break;
            }
        }

        // 添加ROI
        public void AddROI(DirectionalROI roi)
        {
            roi.Name = rois.Count.ToString();
            rois.Add(roi);
            Invalidate();
        }

        // 删除选中的ROI
        public void DeleteSelectedROI()
        {
            if (selectedROI != null)
            {
                rois.Remove(selectedROI);
                selectedROI = null;
                Invalidate();
            }
        }

        // 清除所有ROI
        public void ClearROIs()
        {
            rois.Clear();
            selectedROI = null;
            Invalidate();
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || Image == null) return;

            lastMousePosition = e.Location;
            Point imagePoint = ScreenToImage(e.Location);

            if (!imageRect.Contains(e.Location)) return;

            if (mode == ROIMode.Draw)
            {
                isDrawing = true;
                drawingStart = imagePoint;
                drawingRect = new Rectangle(imagePoint, Size.Empty);
            }
            else if (mode == ROIMode.Select)
            {
                if (selectedROI != null)
                {
                    // 转换到图像坐标进行命中测试
                    currentHandle = selectedROI.GetHandleAtPoint(imagePoint);
                }

                if (currentHandle == HandleType.None)
                {
                    selectedROI = null;
                    foreach (var roi in rois.AsEnumerable().Reverse())
                    {
                        if (roi.Contains(imagePoint))
                        {
                            selectedROI = roi;
                            break;
                        }
                    }

                    if (selectedROI != null)
                    {
                        foreach (var roi in rois)
                        {
                            roi.IsSelected = roi == selectedROI;
                        }
                        currentHandle = HandleType.Move;
                    }
                    else
                    {
                        foreach (var roi in rois)
                        {
                            roi.IsSelected = false;
                        }
                    }
                }
            }

            Invalidate();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (Image == null) return;

            Point imagePoint = ScreenToImage(e.Location);
            Point lastImagePos = ScreenToImage(lastMousePosition);

            if (isDrawing)
            {
                int x = Math.Min(drawingStart.X, imagePoint.X);
                int y = Math.Min(drawingStart.Y, imagePoint.Y);
                int width = Math.Abs(imagePoint.X - drawingStart.X);
                int height = Math.Abs(imagePoint.Y - drawingStart.Y);

                drawingRect = new Rectangle(x, y, width, height);
                Invalidate();
            }
            else if (e.Button == MouseButtons.Left && selectedROI != null && currentHandle != HandleType.None)
            {
                Point delta = new Point(imagePoint.X - lastImagePos.X, imagePoint.Y - lastImagePos.Y);
                selectedROI.ApplyTransform(currentHandle, delta);
                Invalidate();
            }
            else
            {
                var prevHovered = hoveredROI;
                hoveredROI = null;

                foreach (var roi in rois.AsEnumerable().Reverse())
                {
                    if (roi.Contains(imagePoint))
                    {
                        hoveredROI = roi;
                        break;
                    }
                }

                if (prevHovered != hoveredROI)
                {
                    this.Cursor = (hoveredROI != null || currentHandle != HandleType.None)
                        ? Cursors.Hand : Cursors.Default;
                    Invalidate();
                }
            }

            lastMousePosition = e.Location;


        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || Image == null) return;

            if (isDrawing && drawingRect.Width > 5 && drawingRect.Height > 5)
            {
                DirectionalROI newROI = CreateROIByType(drawingRect, $"ROI {rois.Count + 1}");
                rois.Add(newROI);

                if (selectedROI != null)
                    selectedROI.IsSelected = false;
                selectedROI = newROI;
                selectedROI.IsSelected = true;

                isDrawing = false;
                Invalidate();
            }

            currentHandle = HandleType.None;
            if (selectedROI != null)
            {
                selectedROI.ChangeROI?.Invoke(this.Image, selectedROI);
            }
        }

        private DirectionalROI CreateROIByType(Rectangle bounds, string name)
        {
            switch (drawingType)
            {
                case ROIType.Rectangle:
                    return new RectangularROI(bounds, name);
                case ROIType.Ellipse:
                //return new EllipticalROI(bounds, name);
                case ROIType.Arrow:
                //return new ArrowROI(bounds, name);
                default:
                    return new RectangularROI(bounds, name);
            }
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(this.BackColor);

            if (this.Image != null && !imageRect.IsEmpty)
            {
                e.Graphics.DrawImage(this.Image, imageRect);
                e.Graphics.DrawRectangle(Pens.Gray, imageRect);
            }

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // 绘制所有ROI
            foreach (var roi in rois)
            {
                // 保存变换状态
                GraphicsState state = e.Graphics.Save();

                // 应用显示缩放
                e.Graphics.TranslateTransform(imageRect.X, imageRect.Y);
                float scaleX = (float)imageRect.Width / imageSize.Width;
                float scaleY = (float)imageRect.Height / imageSize.Height;
                e.Graphics.ScaleTransform(scaleX, scaleY);

                // 绘制ROI
                roi.Draw(e.Graphics);

                // 恢复变换状态
                e.Graphics.Restore(state);
            }

            // 绘制临时ROI
            if (isDrawing)
            {
                Rectangle screenRect = ImageToScreen(drawingRect);
                using (Pen pen = new Pen(Color.Blue, 1f))
                using (Brush brush = new SolidBrush(Color.FromArgb(50, Color.Blue)))
                {
                    e.Graphics.FillRectangle(brush, screenRect);
                    e.Graphics.DrawRectangle(pen, screenRect);
                }
            }
        }
    }

    // 扩展的枚举
    public enum ROIType
    {
        Rectangle,
        Ellipse,
        Arrow
    }

    public enum ROIMode
    {
        Select,
        Draw
    }

    public class ROIPictureBox : PictureBox
    {
        private List<ROI> rois = new List<ROI>();
        private ROI selectedROI;
        private ROI hoveredROI;
        private Point lastMousePosition;
        private HandleType currentHandle = HandleType.None;
        private bool isDrawing = false;
        private Point drawingStart;
        private Rectangle drawingRect;

        // ROI操作模式
        private ROIMode mode = ROIMode.Select;

        public ROIPictureBox()
        {
            this.DoubleBuffered = true;
            this.MouseDown += OnMouseDown;
            this.MouseMove += OnMouseMove;
            this.MouseUp += OnMouseUp;
            this.Paint += OnPaint;
        }

        // 公共属性
        public List<ROI> ROIs => rois;
        public ROI SelectedROI => selectedROI;
        public ROIMode Mode
        {
            get => mode;
            set
            {
                mode = value;
                if (mode != ROIMode.Select && selectedROI != null)
                {
                    selectedROI.IsSelected = false;
                    selectedROI = null;
                }
            }
        }

        // 添加ROI
        public void AddROI(ROI roi)
        {
            rois.Add(roi);
            Invalidate();
        }

        // 删除选中的ROI
        public void DeleteSelectedROI()
        {
            if (selectedROI != null)
            {
                rois.Remove(selectedROI);
                selectedROI = null;
                Invalidate();
            }
        }

        // 清除所有ROI
        public void ClearROIs()
        {
            rois.Clear();
            selectedROI = null;
            Invalidate();
        }

        // 获取所有ROI
        public IEnumerable<ROI> GetROIs()
        {
            return rois.AsReadOnly();
        }

        // 鼠标按下事件
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            lastMousePosition = e.Location;

            if (mode == ROIMode.Draw)
            {
                // 开始绘制新ROI
                isDrawing = true;
                drawingStart = e.Location;
                drawingRect = new Rectangle(e.Location, Size.Empty);
            }
            else if (mode == ROIMode.Select)
            {
                // 选择模式
                if (selectedROI != null)
                {
                    currentHandle = selectedROI.GetHandleAtPoint(e.Location);
                }

                if (currentHandle == HandleType.None)
                {
                    // 尝试选择新的ROI
                    selectedROI = rois.LastOrDefault(roi => roi.Contains(e.Location));
                    if (selectedROI != null)
                    {
                        // 取消之前选中的ROI
                        foreach (var roi in rois)
                        {
                            roi.IsSelected = roi == selectedROI;
                        }
                        currentHandle = HandleType.Move;
                    }
                    else
                    {
                        // 点击空白处，取消所有选择
                        foreach (var roi in rois)
                        {
                            roi.IsSelected = false;
                        }
                        selectedROI = null;
                    }
                }
            }

            Invalidate();
        }

        // 鼠标移动事件
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                // 绘制新ROI
                int x = Math.Min(drawingStart.X, e.X);
                int y = Math.Min(drawingStart.Y, e.Y);
                int width = Math.Abs(e.X - drawingStart.X);
                int height = Math.Abs(e.Y - drawingStart.Y);

                drawingRect = new Rectangle(x, y, width, height);
                Invalidate();
            }
            else if (e.Button == MouseButtons.Left && selectedROI != null && currentHandle != HandleType.None)
            {
                // 拖动或调整ROI
                int deltaX = e.X - lastMousePosition.X;
                int deltaY = e.Y - lastMousePosition.Y;

                Rectangle newBounds = selectedROI.Bounds;

                switch (currentHandle)
                {
                    case HandleType.Move:
                        newBounds.X += deltaX;
                        newBounds.Y += deltaY;
                        break;
                    case HandleType.TopLeft:
                        newBounds.X += deltaX;
                        newBounds.Y += deltaY;
                        newBounds.Width -= deltaX;
                        newBounds.Height -= deltaY;
                        break;
                    case HandleType.TopRight:
                        newBounds.Y += deltaY;
                        newBounds.Width += deltaX;
                        newBounds.Height -= deltaY;
                        break;
                    case HandleType.BottomLeft:
                        newBounds.X += deltaX;
                        newBounds.Width -= deltaX;
                        newBounds.Height += deltaY;
                        break;
                    case HandleType.BottomRight:
                        newBounds.Width += deltaX;
                        newBounds.Height += deltaY;
                        break;
                }

                // 确保尺寸不为负
                if (newBounds.Width < 0)
                {
                    newBounds.X += newBounds.Width;
                    newBounds.Width = -newBounds.Width;
                }
                if (newBounds.Height < 0)
                {
                    newBounds.Y += newBounds.Height;
                    newBounds.Height = -newBounds.Height;
                }

                // 限制在PictureBox范围内
                newBounds.X = Math.Max(0, Math.Min(newBounds.X, this.Width - newBounds.Width));
                newBounds.Y = Math.Max(0, Math.Min(newBounds.Y, this.Height - newBounds.Height));
                newBounds.Width = Math.Min(newBounds.Width, this.Width - newBounds.X);
                newBounds.Height = Math.Min(newBounds.Height, this.Height - newBounds.Y);

                selectedROI.Bounds = newBounds;
                Invalidate();
            }
            else
            {
                // 更新悬停状态
                var prevHovered = hoveredROI;
                hoveredROI = rois.LastOrDefault(roi => roi.Contains(e.Location));

                if (prevHovered != hoveredROI)
                {
                    this.Cursor = hoveredROI != null || (selectedROI?.GetHandleAtPoint(e.Location) != HandleType.None)
                        ? Cursors.Hand : Cursors.Default;
                    Invalidate();
                }
            }

            lastMousePosition = e.Location;
        }

        // 鼠标释放事件
        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            if (isDrawing)
            {
                // 完成绘制
                if (drawingRect.Width > 5 && drawingRect.Height > 5)
                {
                    var newROI = new ROI(drawingRect, $"ROI {rois.Count + 1}");
                    rois.Add(newROI);

                    // 选中新创建的ROI
                    if (selectedROI != null)
                        selectedROI.IsSelected = false;
                    selectedROI = newROI;
                    selectedROI.IsSelected = true;
                }

                isDrawing = false;
                Invalidate();
            }

            currentHandle = HandleType.None;
        }

        // 绘制事件
        private void OnPaint(object sender, PaintEventArgs e)
        {
            // 先绘制基础图像
            if (this.Image != null)
            {
                e.Graphics.DrawImage(this.Image, this.ClientRectangle);
            }

            // 设置高质量绘制
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // 绘制所有ROI
            foreach (var roi in rois)
            {
                roi.Draw(e.Graphics);
            }

            // 如果正在绘制，显示临时矩形
            if (isDrawing)
            {
                using (Pen pen = new Pen(Color.Blue, 1f))
                using (Brush brush = new SolidBrush(Color.FromArgb(50, Color.Blue)))
                {
                    e.Graphics.FillRectangle(brush, drawingRect);
                    e.Graphics.DrawRectangle(pen, drawingRect);
                }
            }
        }
    }
}
