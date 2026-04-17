using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

namespace GSP_SJ
{
    public partial class PictureBoxZoom : PictureBox
    {
        public PictureBoxZoom()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.Resize += (s, e) => Reset();
            SizeMode = PictureBoxSizeMode.AutoSize;

        }


        private float _zoom = 1.0f;
        private Point _panPosition = Point.Empty;
        private Point _lastMousePosition;
        private bool _isPanning;



        public float Zoom
        {
            get => _zoom;
            set
            {
                _zoom = Math.Max(0.1f, Math.Min(value, 10f));
                Invalidate();
            }
        }



        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (Image == null) return;

            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // 计算缩放后的图像尺寸
            int scaledWidth = (int)(Image.Width * _zoom);
            int scaledHeight = (int)(Image.Height * _zoom);

            // 计算绘制位置（居中显示）
            Rectangle destRect = new Rectangle(
                _panPosition.X + (this.Width - scaledWidth) / 2,
                _panPosition.Y + (this.Height - scaledHeight) / 2,
                scaledWidth,
                scaledHeight);

            // 绘制图像
            e.Graphics.DrawImage(Image, destRect, new Rectangle(0, 0, Image.Width, Image.Height), GraphicsUnit.Pixel);


        }

        // 其他原有方法保持不变 (OnMouseWheel, OnMouseDown, OnMouseUp, OnMouseMove)
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            // 获取鼠标相对于控件的坐标
            Point mousePos = e.Location;

            // 获取鼠标相对于图像的位置（在缩放前）
            Point imagePosBeforeZoom = ScreenToImage(mousePos);

            // 调整缩放级别
            float zoomFactor = e.Delta > 0 ? 1.2f : 1 / 1.2f;
            Zoom *= zoomFactor;

            // 获取鼠标相对于图像的位置（在缩放后）
            Point imagePosAfterZoom = ScreenToImage(mousePos);

            // 调整平移位置以保持鼠标下的点不变
            _panPosition.X += (int)((imagePosAfterZoom.X - imagePosBeforeZoom.X) * _zoom);
            _panPosition.Y += (int)((imagePosAfterZoom.Y - imagePosBeforeZoom.Y) * _zoom);

            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                _isPanning = true;
                _lastMousePosition = e.Location;
                this.Cursor = Cursors.Hand;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button == MouseButtons.Left)
            {
                _isPanning = false;
                this.Cursor = Cursors.Default;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_isPanning)
            {
                Point delta = new Point(e.X - _lastMousePosition.X, e.Y - _lastMousePosition.Y);
                _panPosition.X += delta.X;
                _panPosition.Y += delta.Y;
                _lastMousePosition = e.Location;
                Invalidate();
            }
        }
        public void Reset()
        {
            if (Image == null) return;

            // 计算适合控件大小的缩放比例
            float zoomX = (float)this.Width / Image.Width;
            float zoomY = (float)this.Height / Image.Height;

            _zoom = Math.Min(zoomX, zoomY);
            _panPosition = Point.Empty;

            Invalidate();
        }

        // 图像坐标转换为屏幕坐标
        private Rectangle ImageToScreen(Rectangle imageRect)
        {
            int scaledWidth = (int)(Image.Width * _zoom);
            int scaledHeight = (int)(Image.Height * _zoom);

            // 计算图像在屏幕上的位置
            int imageLeft = _panPosition.X + (this.Width - scaledWidth) / 2;
            int imageTop = _panPosition.Y + (this.Height - scaledHeight) / 2;

            // 转换ROI坐标
            return new Rectangle(
                imageLeft + (int)(imageRect.X * _zoom),
                imageTop + (int)(imageRect.Y * _zoom),
                (int)(imageRect.Width * _zoom),
                (int)(imageRect.Height * _zoom));
        }

        // 屏幕坐标转换为图像坐标
        private Point ScreenToImage(Point screenPoint)
        {
            int scaledWidth = (int)(Image.Width * _zoom);
            int scaledHeight = (int)(Image.Height * _zoom);

            int imageX = screenPoint.X - (this.Width - scaledWidth) / 2 - _panPosition.X;
            int imageY = screenPoint.Y - (this.Height - scaledHeight) / 2 - _panPosition.Y;

            return new Point((int)(imageX / _zoom), (int)(imageY / _zoom));
        }


    }



    public class ZoomablePictureBox : PictureBox
    {
        private Image originalImage;
        private float zoom = 1.0f;
        private Point dragStart;
        private Point imagePosition = Point.Empty;
        private bool isDragging = false;
        private const float ZOOM_FACTOR = 1.2f;
        private const float MIN_ZOOM = 0.1f;
        private const float MAX_ZOOM = 1.2f;


        public List<Component> components =new List<Component>();

        public float Zoom
        {
            get { return zoom; }
            set
            {
                zoom = Math.Max(MIN_ZOOM, Math.Min(MAX_ZOOM, value));
                Invalidate();
            }
        }

        public new Image Image
        {
            get { return originalImage; }
            set
            {
                originalImage = value;
                zoom = 1.0f;
                imagePosition = Point.Empty;
                base.Image = value;
                Invalidate();
            }
        }

        public ZoomablePictureBox()
        {
            this.SizeMode = PictureBoxSizeMode.AutoSize;
            this.MouseWheel += ZoomablePictureBox_MouseWheel;
            this.MouseDown += ZoomablePictureBox_MouseDown;
            this.MouseMove += ZoomablePictureBox_MouseMove;
            this.MouseUp += ZoomablePictureBox_MouseUp;
        }

        PointF movePoint;
        PointF mouseWorldPos;

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (originalImage != null && zoom != 1.0f)
            {
                // 自定义绘制缩放后的图像
                var destRect = GetDestinationRectangle();
                pe.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                pe.Graphics.DrawImage(originalImage, destRect);

                // 绘制选中元件
                if (IsSelectCompont)
                {
                    float screenWidth = clickedComponent.Size.Width * zoom;
                    float screenHeight = clickedComponent.Size.Height * zoom;

                    Point point = ImageToScreenPoint(new Point((int)clickedComponent.Position.X, (int)clickedComponent.Position.Y));
                    using (Pen pen = new Pen(Color.Red, 3.0f))
                    using (Brush fillBrush = new SolidBrush(Color.FromArgb(60, Color.Red)))
                    {
                        //pe.Graphics.DrawRectangle(pen,new Rectangle(point.X-20, point.Y-20, 40, 40));
                        RectangleF rect = new RectangleF(
                             point.X-20,
                         point.Y - 20,
                        40,
                        40
                        );
                        pe.Graphics.DrawLine(pen, rect.X + rect.Width / 2, rect.Y + rect.Height, rect.X + rect.Width / 2, rect.Y + rect.Height + 100);
                        pe.Graphics.DrawLine(pen, rect.X + rect.Width / 2, rect.Y, rect.X + rect.Width / 2, rect.Y - 100);

                        pe.Graphics.DrawLine(pen, rect.X, rect.Y + rect.Height / 2, rect.X - 100, rect.Y + rect.Height / 2);
                        pe.Graphics.DrawLine(pen, rect.X + rect.Width, rect.Y + rect.Height / 2, rect.X + rect.Width + 100, rect.Y + rect.Height / 2);


                    }
                }

                

                DrawStatus(pe.Graphics);
            }
            else
            {
                base.OnPaint(pe);
            }
        }

        private void DrawStatus(Graphics g)
        {
            string status = $"缩放: {zoom:F2}x | 坐标: X={mouseWorldPos.X:F2}, Y={-mouseWorldPos.Y:F2}|屏幕坐标: X={movePoint.X:F2}, Y={movePoint.Y:F2}";
            using (Brush brush = new SolidBrush(Color.Black))
            using (Font font = new Font("Arial", 9))
            {
                g.DrawString(status, font, brush, 10, Height - 25);
            }

        }

        public void setSelectCompont(string name, int width, int height)
        {
            List<Component> coms = components.Where(x => x.Designator == name).ToList().ToList();
            if (coms.Count > 0)
            {
                IsSelectCompont = true;
                clickedComponent = coms[0];
                Zoom = 0.9f;
                // 计算指定点在当前缩放和位置下的屏幕坐标
                Point currentScreenPoint = ImageToScreenPoint(new Point((int)coms[0].Position.X, (int)coms[0].Position.Y));

                // 计算视图中心点
                Point viewCenter = new Point(width / 2, height / 2);

                // 计算需要调整的偏移量
                int deltaX = viewCenter.X - currentScreenPoint.X;
                int deltaY = viewCenter.Y - currentScreenPoint.Y;

                // 更新图像位置
                imagePosition.X += deltaX;
                imagePosition.Y += deltaY;

                Invalidate();
            }
        }

        /// <summary>
        /// 将图像中的指定点移动到视图中心
        /// </summary>
        /// <param name="imagePoint">原始图像中的坐标点</param>
        public void CenterToImagePoint(Point imagePoint)
        {
            if (originalImage == null) return;

            // 计算指定点在当前缩放和位置下的屏幕坐标
            Point currentScreenPoint = ImageToScreenPoint(imagePoint);

            // 计算视图中心点
            Point viewCenter = new Point(Width / 2, Height / 2);

            // 计算需要调整的偏移量
            int deltaX = viewCenter.X - currentScreenPoint.X;
            int deltaY = viewCenter.Y - currentScreenPoint.Y;

            // 更新图像位置
            imagePosition.X += deltaX;
            imagePosition.Y += deltaY;

            Invalidate();
        }

        /// <summary>
        /// 将屏幕坐标点移动到视图中心
        /// </summary>
        /// <param name="screenPoint">屏幕坐标点</param>
        public void CenterToScreenPoint(Point screenPoint)
        {
            if (originalImage == null) return;

            // 将屏幕坐标转换为图像坐标
            Point imagePoint = ScreenToImagePoint(screenPoint);

            // 如果点在图像范围内，则居中显示
            if (IsPointInImage(screenPoint))
            {
                CenterToImagePoint(imagePoint);
            }
        }

        /// <summary>
        /// 将指定点居中并缩放到指定级别
        /// </summary>
        /// <param name="imagePoint">原始图像中的坐标点</param>
        /// <param name="targetZoom">目标缩放级别</param>
        public void CenterAndZoomToPoint(Point imagePoint, float targetZoom)
        {
            if (originalImage == null) return;

            // 先设置缩放级别
            Zoom = targetZoom;

            // 然后将指定点居中
            CenterToImagePoint(imagePoint);
        }

        /// <summary>
        /// 将指定矩形区域居中并自动计算合适的缩放级别
        /// </summary>
        /// <param name="imageRect">原始图像中的矩形区域</param>
        public void CenterAndZoomToRectangle(Rectangle imageRect)
        {
            if (originalImage == null) return;

            // 计算适合矩形区域的缩放级别
            float zoomX = (float)Width / imageRect.Width;
            float zoomY = (float)Height / imageRect.Height;
            float targetZoom = Math.Min(zoomX, zoomY) * 0.9f; // 留一点边距

            // 限制缩放范围
            targetZoom = Math.Max(MIN_ZOOM, Math.Min(MAX_ZOOM, targetZoom));

            // 计算矩形的中心点
            Point centerPoint = new Point(
                imageRect.X + imageRect.Width / 2,
                imageRect.Y + imageRect.Height / 2
            );

            // 居中并缩放
            CenterAndZoomToPoint(centerPoint, targetZoom);
        }


        /// <summary>
        /// 将屏幕坐标转换为原始图像坐标
        /// </summary>
        /// <param name="screenPoint">屏幕坐标（相对于 PictureBox）</param>
        /// <returns>原始图像中的坐标</returns>
        public Point ScreenToImagePoint(Point screenPoint)
        {
            if (originalImage == null) return Point.Empty;

            Rectangle destRect = GetDestinationRectangle();
            if (destRect.Width == 0 || destRect.Height == 0) return Point.Empty;

            // 将屏幕坐标转换为图像坐标
            float scaleX = (float)originalImage.Width / destRect.Width;
            float scaleY = (float)originalImage.Height / destRect.Height;

            int imageX = (int)((screenPoint.X - destRect.X) * scaleX);
            int imageY = (int)((screenPoint.Y - destRect.Y) * scaleY);

            return new Point(imageX, imageY);
        }

        /// <summary>
        /// 将原始图像坐标转换为屏幕坐标
        /// </summary>
        /// <param name="imagePoint">原始图像坐标</param>
        /// <returns>屏幕坐标（相对于 PictureBox）</returns>
        public Point ImageToScreenPoint(Point imagePoint)
        {
            if (originalImage == null) return Point.Empty;

            Rectangle destRect = GetDestinationRectangle();
            if (destRect.Width == 0 || destRect.Height == 0) return Point.Empty;

            // 将图像坐标转换为屏幕坐标
            float scaleX = destRect.Width / (float)originalImage.Width;
            float scaleY = destRect.Height / (float)originalImage.Height;

            int screenX = destRect.X + (int)(imagePoint.X * scaleX);
            int screenY = destRect.Y + (int)(imagePoint.Y * scaleY);

            return new Point(screenX, screenY);
        }

        /// <summary>
        /// 检查屏幕坐标是否在图像范围内
        /// </summary>
        public bool IsPointInImage(Point screenPoint)
        {
            if (originalImage == null) return false;

            Point imagePoint = ScreenToImagePoint(screenPoint);
            return imagePoint.X >= 0 && imagePoint.X < originalImage.Width &&
                   imagePoint.Y >= 0 && imagePoint.Y < originalImage.Height;
        }

        /// <summary>
        /// 获取当前可见的图像区域在原始图像中的矩形
        /// </summary>
        public Rectangle GetVisibleImageRegion()
        {
            if (originalImage == null) return Rectangle.Empty;

            Point topLeft = ScreenToImagePoint(new Point(0, 0));
            Point bottomRight = ScreenToImagePoint(new Point(Width, Height));

            int x = Math.Max(0, Math.Min(topLeft.X, bottomRight.X));
            int y = Math.Max(0, Math.Min(topLeft.Y, bottomRight.Y));
            int width = Math.Min(originalImage.Width - x, Math.Abs(bottomRight.X - topLeft.X));
            int height = Math.Min(originalImage.Height - y, Math.Abs(bottomRight.Y - topLeft.Y));

            return new Rectangle(x, y, width, height);
        }

        private Rectangle GetDestinationRectangle()
        {
            if (originalImage == null) return Rectangle.Empty;

            int scaledWidth = (int)(originalImage.Width * zoom);
            int scaledHeight = (int)(originalImage.Height * zoom);

            int x = (Width - scaledWidth) / 2 + imagePosition.X;
            int y = (Height - scaledHeight) / 2 + imagePosition.Y;

            return new Rectangle(x, y, scaledWidth, scaledHeight);
        }
 

        private void ZoomablePictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if (originalImage == null) return;

            // 保存缩放前的鼠标位置（相对于图像）
            Point mousePos = e.Location;
            Point imagePosBeforeZoom = GetImagePointFromScreenPoint(mousePos);

            // 计算缩放
            if (e.Delta > 0)
                Zoom *= ZOOM_FACTOR;
            else
                Zoom /= ZOOM_FACTOR;

            // 计算缩放后的鼠标位置，并调整图像位置以保持缩放中心
            Point imagePosAfterZoom = GetImagePointFromScreenPoint(mousePos);
            imagePosition.X += (imagePosAfterZoom.X - imagePosBeforeZoom.X);
            imagePosition.Y += (imagePosAfterZoom.Y - imagePosBeforeZoom.Y);

            Invalidate();
        }
        bool IsSelectCompont = false;
        Component clickedComponent = null;
        private void ZoomablePictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && originalImage != null)
            {
                isDragging = true;
                dragStart = e.Location;
                this.Cursor = Cursors.Hand;
                //IsSelectCompont = false;
                //if (IsPointInImage(e.Location))
                //{
                //    PointF worldPos = ScreenToImagePoint(e.Location);
                //    // 查找点击的元件 - 使用改进的检测方法
                //    clickedComponent = FindComponentAt(worldPos);
                //    if (clickedComponent != null)
                //    {
                //        IsSelectCompont = true;
                //        //MessageBox.Show($"点击的元件：{clickedComponent.Designator}");
                //    }
                //}
              
            }
            Invalidate();
        }

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

  
        private void ZoomablePictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            movePoint =e.Location;
            mouseWorldPos=new PointF(0,0);
            if (isDragging && originalImage != null)
            {
                // 计算拖动距离并更新图像位置
                int deltaX = e.X - dragStart.X;
                int deltaY = e.Y - dragStart.Y;

                imagePosition.X += deltaX;
                imagePosition.Y += deltaY;

                dragStart = e.Location;
                Invalidate();
            }
        }

        private void ZoomablePictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
                this.Cursor = Cursors.Default;
            }
        }

        private Point GetImagePointFromScreenPoint(Point screenPoint)
        {
            if (originalImage == null) return Point.Empty;

            Rectangle destRect = GetDestinationRectangle();
            if (destRect.Width == 0 || destRect.Height == 0) return Point.Empty;

            // 将屏幕坐标转换为图像坐标
            float scaleX = (float)originalImage.Width / destRect.Width;
            float scaleY = (float)originalImage.Height / destRect.Height;

            int imageX = (int)((screenPoint.X - destRect.X) * scaleX);
            int imageY = (int)((screenPoint.Y - destRect.Y) * scaleY);

            return new Point(imageX, imageY);
        }

        // 重置视图
        public void ResetView()
        {
            zoom = 1.0f;
            imagePosition = Point.Empty;
            Invalidate();
        }

        // 缩放到适合大小
        public void ZoomToFit()
        {
            if (originalImage == null) return;

            float zoomX = (float)this.Width / originalImage.Width;
            float zoomY = (float)this.Height / originalImage.Height;
            zoom = Math.Min(zoomX, zoomY);
            imagePosition = Point.Empty;
            Invalidate();
        }
    }
}
