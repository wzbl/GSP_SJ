using GSP_SJ.ModelClass;
using HalconDotNet;
using SqlHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSP_SJ.Form_Chart
{
    public class ZoomablePictureBox : PictureBox
    {
        private Image originalImage;
        private float zoom = 1.0f;
        private Point dragStart;
        private Point imagePosition = Point.Empty;
        private bool isDragging = false;
        private const float ZOOM_FACTOR = 1.5f;
        private const float MIN_ZOOM = 0.1f;
        private const float MAX_ZOOM = 1.5f;

        // ====================== 新增 Ctrl 十字架功能 ======================
        private bool isCtrlPressed = false;
        private PointF currentMousePos = PointF.Empty;
        private PointF crossImagePos = PointF.Empty;


        private Type_Window type_Window = Type_Window.None;

        public List<Component> components = new List<Component>();

        private ROIModel rOIModel = ROIModel.Select;
        private Point RoiStart;

        public List<Rectangle> ROIs = new List<Rectangle>();

        private Rectangle _drawingRoi;          // 正在画的ROI
        private Rectangle _selectedRoi;         // 当前选中的ROI
        private bool _isDraggingRoi;            // 是否正在拖动ROI
        private Point _roiDragOffset;           // 拖动偏移
        private Rectangle clickedComponentRectangle = new Rectangle();

        private HImage hImage = null;

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
            this.MouseDoubleClick += ZoomablePictureBox_MouseDoubleClick;
        }

        public ZoomablePictureBox(Type_Window type_Window)
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);

            this.SizeMode = PictureBoxSizeMode.AutoSize;
            this.MouseWheel += ZoomablePictureBox_MouseWheel;
            this.MouseDown += ZoomablePictureBox_MouseDown;
            this.MouseMove += ZoomablePictureBox_MouseMove;
            this.MouseUp += ZoomablePictureBox_MouseUp;
            this.MouseDoubleClick += ZoomablePictureBox_MouseDoubleClick;
            this.type_Window = type_Window;

            // 监听键盘按键
            //this.KeyDown += ZoomablePictureBox_KeyDown;
            //this.KeyUp += ZoomablePictureBox_KeyUp;
            this.MouseMove += (s, e) => currentMousePos = e.Location;
            IsSelectCompont = false;
            clickedComponent = null;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Control && !isCtrlPressed)
            {
                isCtrlPressed = true;
                this.Cursor = Cursors.Cross;
                Invalidate();
            }
            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (!e.Control && isCtrlPressed)
            {
                isCtrlPressed = false;
                this.Cursor = Cursors.Default;
                Invalidate();
            }
            base.OnKeyUp(e);
        }

        // ====================== Ctrl 按键监听 ======================
        private void ZoomablePictureBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && !isCtrlPressed)
            {
                isCtrlPressed = true;
                this.Cursor = Cursors.Cross;
                Invalidate();
            }
        }

        private void ZoomablePictureBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (!e.Control && isCtrlPressed)
            {
                isCtrlPressed = false;
                this.Cursor = Cursors.Default;
                Invalidate();
            }
        }


        private void ZoomablePictureBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && originalImage != null)
            {
                IsSelectCompont = false;
                clickedComponent = null;
                if (IsPointInImage(e.Location))
                {
                    PointF worldPos = ScreenToImagePoint(e.Location);
                    // 查找点击的元件 - 使用改进的检测方法
                    clickedComponent = FindComponentAt(worldPos);
                    if (clickedComponent != null)
                    {
                        IsSelectCompont = true;
                    }

                }
                Invalidate();

                if (IsSelectCompont)
                {
                    if (type_Window == Type_Window.Screen)
                    {
                        Global.SelectComponent?.Invoke(clickedComponent.Designator);

                    }
                    else if (type_Window == Type_Window.OCR)
                    {
                        if(clickedComponent.IsHaveModel)
                        {

                        }
                        else if(ROIs.Count > 0)
                        {
                            CountComponentsInROI(ROIs[0], out List<Component> comps);
                            if (!comps.Contains(clickedComponent))
                            {
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                        RoiImg();
                        if (hImage != null)
                        {
                            //hImage.WriteImage("bmp",0, "C:\\Users\\14802\\Desktop\\pos.bmp");
                            Man_ReportItem item = Global.man_ReportItems.Where(x => x.Position == clickedComponent.Designator).First();
                            if (item != null)
                            {
                                FormModelItem formModelItem = new FormModelItem(item.MaterialCode, item.MaterialName, clickedComponent.ProductCode, HalconImageConverter.HImageToBitmapRGB(hImage), item.LcrType);
                                formModelItem.ShowDialog();
                                bool isHaveModel = SQLDataControl.GetEng_ModelItem(clickedComponent.ProductCode, item.MaterialCode).ToList().Count > 0;
                                components.Where(x => x.MaterialCode == item.MaterialCode).ToList().ForEach(x => x.IsHaveModel = isHaveModel);

                            }
                        }
                    }
                }
            }
        }


        public bool GetSelectComponent(out Image image,out Component component)
        {
            image = null;
            component = null;
            if (!IsSelectCompont)
            {
                return false;
            }
            RoiImg();
            component = clickedComponent;
            if(hImage != null)
                image = HalconImageConverter.HImageToBitmapRGB(hImage);
            return true;
        }
        
        private void RoiImg()
        {
            hImage = null;
            HObject hObject, ho_ImageReduced, ho_findCircleRoi = null;
            HOperatorSet.GenEmptyObj(out hObject);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            try
            {
                Point point = new Point((int)clickedComponent.Position.X, (int)clickedComponent.Position.Y);
                Rectangle rOI = new Rectangle((int)(point.X - clickedComponent.Size.Width / 2), (int)(point.Y - clickedComponent.Size.Height / 2), (int)clickedComponent.Size.Width, (int)clickedComponent.Size.Height);

                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    //HOperatorSet.GenRectangle2(out ho_findCircleRoi, rOI.X, rOI.Y, clickedComponent.Angle,
                    //                                                        rOI.Height/2, rOI.Width/2);
                    HOperatorSet.GenRectangle1(out ho_findCircleRoi, rOI.Top - 50, rOI.Left - 50,
                                                                          rOI.Bottom + 50, rOI.Right + 50);
                    HOperatorSet.ReduceDomain(HalconImageConverter.BitmapToHImage2(new Bitmap(originalImage)), ho_findCircleRoi, out hObject);
                    HOperatorSet.ChangeDomain(hObject, ho_findCircleRoi, out ho_ImageReduced);
                    HOperatorSet.CropDomain(ho_ImageReduced, out ho_ImageReduced);
                    ho_ImageReduced = PublicFunction.RotateImage(ho_ImageReduced, clickedComponent.Angle);
                    hImage = new HImage(ho_ImageReduced);
                    HOperatorSet.GetImageSize(hImage, out HTuple width, out HTuple height);
                    if (width > 60 && height > 60)
                    {
                        HOperatorSet.GenRectangle1(out ho_findCircleRoi, 40, 40,
                                                                      height - 40, width - 40);
                        HOperatorSet.ChangeDomain(hImage, ho_findCircleRoi, out ho_ImageReduced);
                        HOperatorSet.CropDomain(ho_ImageReduced, out ho_ImageReduced);
                        hImage = new HImage(ho_ImageReduced);
                    }

                    //hImage.WriteImage("bmp", 0, "C:\\Users\\14802\\Desktop\\" + clickedComponent.Designator + ".bmp");
                }
            }
            catch (Exception)
            {

            }

            ho_findCircleRoi.Dispose();
            hObject.Dispose();
            ho_ImageReduced.Dispose();
        }

       
        /// <summary>
        /// 获取鼠标点击的像素点
        /// </summary>
        /// <returns></returns>
        public PointF PixPoint()
        {
            return ScreenToImagePoint(new Point((int)currentMousePos.X, (int)currentMousePos.Y));
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
                    using (Pen pen2 = new Pen(Color.Blue, 3.0f))
                    using (Brush fillBrush = new SolidBrush(Color.FromArgb(60, Color.Red)))
                    {
                        pe.Graphics.TranslateTransform(point.X, point.Y);
                        pe.Graphics.RotateTransform(-clickedComponent.Angle);
                        clickedComponentRectangle = new Rectangle(
                              -(int)screenWidth / 2,
                                 -(int)screenHeight / 2,
                              (int)screenWidth,
                              (int)screenHeight);
                        pe.Graphics.DrawRectangle(pen2, clickedComponentRectangle);
                        pe.Graphics.ResetTransform();
                        RectangleF rect = new RectangleF(
                             point.X - 20,
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
                // ====================== 绘制 Ctrl 十字架 ======================
                if (isCtrlPressed)
                {
                    DrawCross(pe.Graphics);
                }
                else
                    DrawStatus(pe.Graphics);
                //Point p = ScreenToImagePoint(new Point((int)movePoint.X, (int)movePoint.Y));
                //pe.Graphics.DrawString("X:"+p.X+",Y:"+p.Y, new Font("Arial", 20), Brushes.Red, 150, Height - 200);


                // ====================== 绘制所有 ROI ======================
                DrawAllROIs(pe.Graphics);

                // ====================== 绘制正在拖拽的 ROI ======================
                if (rOIModel == ROIModel.RectangleROI && !_drawingRoi.IsEmpty)
                {
                    using (Pen pen = new Pen(Color.Red, 2))
                    {
                        pen.DashStyle = DashStyle.Dash;
                        pe.Graphics.DrawRectangle(pen, _drawingRoi);
                    }
                }

                foreach (var component in components)
                {
                    if (component.IsHaveModel && Global.IsShowCompoment)
                    {
                        Point p = ImageToScreenPoint(new Point((int)component.Position.X, (int)component.Position.Y));
                        float screenWidth = component.Size.Width * zoom;
                        float screenHeight = component.Size.Height * zoom;
                        pe.Graphics.TranslateTransform(p.X, p.Y);
                        pe.Graphics.RotateTransform(-component.Angle);
                        using (Pen pen2 = new Pen(Color.Yellow, 3.0f))
                        {
                            pe.Graphics.DrawRectangle(pen2, new Rectangle(
                              -(int)screenWidth / 2,
                                 -(int)screenHeight / 2,
                              (int)screenWidth,
                              (int)screenHeight));
                        }
                        pe.Graphics.ResetTransform();
                        if (Global.IsShowComponentPos)
                        {
                            pe.Graphics.DrawString(component.Designator, new Font("Arial", 10), Brushes.Yellow, p.X - 20, p.Y);
                        }
                    }

                }
            }
            else
            {
                if (isCtrlPressed)
                {
                    DrawCross(pe.Graphics);
                }
                base.OnPaint(pe);
            }
        }

        // 绘制所有 ROI（选中为绿色，普通为红色，显示内部元件数量）
        private void DrawAllROIs(Graphics g)
        {
            foreach (var roi in ROIs)
            {
                bool isSelected = roi == _selectedRoi;
                Color color = isSelected ? Color.Lime : Color.Red;
                int count = CountComponentsInROI(roi, out List<Component> comps);
                using (Pen pen = new Pen(color, isSelected ? 3 : 2))
                using (Brush textBrush = new SolidBrush(Color.White))
                using (Brush backBrush = new SolidBrush(Color.FromArgb(180, color)))
                using (Font font = new Font("Arial", 9))
                {
                    g.DrawRectangle(pen, roi);
                    string text = $"元件：{count}";
                    SizeF size = g.MeasureString(text, font);
                    g.FillRectangle(backBrush, roi.X, roi.Y - 20, size.Width, size.Height);
                    g.DrawString(text, font, textBrush, roi.X, roi.Y - 20);
                }
            }
        }

        // 图像坐标矩形 → 屏幕坐标矩形
        public Rectangle ImageToScreenRect(RectangleF imgRect)
        {
            if (originalImage == null) return Rectangle.Empty;
            Rectangle destRect = GetDestinationRectangle();
            float scaleX = destRect.Width / (float)originalImage.Width;
            float scaleY = destRect.Height / (float)originalImage.Height;

            return new Rectangle(
                (int)(destRect.X + imgRect.X * scaleX),
                (int)(destRect.Y + imgRect.Y * scaleY),
                (int)(imgRect.Width * scaleX),
                (int)(imgRect.Height * scaleY));
        }

        // 屏幕坐标矩形 → 图像坐标矩形
        public RectangleF ScreenToImageRect(Rectangle screenRect)
        {
            if (originalImage == null) return RectangleF.Empty;
            Rectangle destRect = GetDestinationRectangle();
            if (destRect.Width == 0 || destRect.Height == 0) return RectangleF.Empty;

            float scaleX = (float)originalImage.Width / destRect.Width;
            float scaleY = (float)originalImage.Height / destRect.Height;
            return new RectangleF(
                (screenRect.X - destRect.X) * scaleX,
                (screenRect.Y - destRect.Y) * scaleY,
                screenRect.Width * scaleX,
                screenRect.Height * scaleY);
        }


        // 统计 ROI 内元件数量
        public int CountComponentsInROI(Rectangle screenRoi, out List<Component> comps)
        {
            int count = 0;
            comps = new List<Component>();
            foreach (var c in components)
            {
                Point screenPt = ImageToScreenPoint(new Point((int)c.Position.X, (int)c.Position.Y));
                if (screenRoi.Contains(screenPt))
                {
                    count++;
                    comps.Add(c);
                }
            }
            return count;
        }

        // ====================== 绘制十字架核心 ======================
        private void DrawCross(Graphics g)
        {
            PointF screenPos;

            // 鼠标不在图片上 → 显示在图片中心
            if (!IsPointInImage(Point.Round(currentMousePos)))
            {
                crossImagePos = new PointF(originalImage.Width / 2f, originalImage.Height / 2f);
                screenPos = ImageToScreenPoint(new Point((int)crossImagePos.X, (int)crossImagePos.Y));
            }
            else
            {
                crossImagePos = ScreenToImagePoint(Point.Round(currentMousePos));
                screenPos = currentMousePos;
            }

            // 绘制十字线
            using (Pen pen = new Pen(Color.Blue, 2.0f))
            {
                pen.DashStyle = DashStyle.Dash;
                g.DrawLine(pen, (float)screenPos.X - Width, (float)screenPos.Y, (float)screenPos.X + Width, (float)screenPos.Y);
                g.DrawLine(pen, (float)screenPos.X, (float)screenPos.Y - Height, (float)screenPos.X, (float)screenPos.Y + Height);
            }

            // 绘制坐标标签
            string text = $"({crossImagePos.X:F0},{crossImagePos.Y:F0})";
            using (Font font = new Font("Arial", 10))
            using (Brush brush = new SolidBrush(Color.White))
            using (Brush back = new SolidBrush(Color.FromArgb(180, Color.Blue)))
            {
                SizeF size = g.MeasureString(text, font);
                g.FillRectangle(back, screenPos.X + 10, screenPos.Y + 10, size.Width, size.Height);
                g.DrawString(text, font, brush, screenPos.X + 10, screenPos.Y + 10);
            }
        }

        private void DrawStatus(Graphics g)
        {
            string status = $"缩放: {zoom:F2}x | 坐标: X={mouseWorldPos.X:F2}, Y={-mouseWorldPos.Y:F2}|屏幕坐标: X={movePoint.X:F2}, Y={movePoint.Y:F2}";
            using (Brush brush = new SolidBrush(Color.Black))
            using (Font font = new Font("Arial", 100))
            {
                g.DrawString(status, font, brush, 10, Height - 100);
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
            if (originalImage == null && isCtrlPressed) return;

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
            imagePosition.X += (int)((imagePosAfterZoom.X - imagePosBeforeZoom.X) * zoom);
            imagePosition.Y += (int)((imagePosAfterZoom.Y - imagePosBeforeZoom.Y) * zoom);
            ROIs.Clear();
            Invalidate();
        }

        bool IsSelectCompont = false;

        /// <summary>
        /// 选中元件
        /// </summary>
        public Component clickedComponent = null;
        private void ZoomablePictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && originalImage != null && rOIModel == ROIModel.RectangleROI)
            {
                isDragging = true;
                //检查是否点击了已有 ROI → 开始拖动
                foreach (var roi in ROIs)
                {
                    if (roi.Contains(e.Location))
                    {
                        _selectedRoi = roi;
                        _isDraggingRoi = true;
                        _roiDragOffset = new Point(e.X - roi.X, e.Y - roi.Y);
                        this.Cursor = Cursors.Hand;
                        return;
                    }
                }
                ROIs.Clear();
                // 开始画新 ROI
                RoiStart = e.Location;
                _drawingRoi = Rectangle.Empty;
                _selectedRoi = Rectangle.Empty;
                this.Cursor = Cursors.Cross;
            }
            else if (e.Button == MouseButtons.Left && originalImage != null && !isCtrlPressed)
            {
                isDragging = true;
                dragStart = e.Location;
                this.Cursor = Cursors.Hand;
            }
            else if (e.Button == MouseButtons.Right && originalImage != null && !isCtrlPressed)
            {
                isCtrlPressed = true;
                this.Cursor = Cursors.Cross;
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
            movePoint = e.Location;
            mouseWorldPos = new PointF(0, 0);
            if (isDragging && originalImage != null && rOIModel == ROIModel.RectangleROI)
            {
                // 绘制矩形模式
                if (_isDraggingRoi && !_selectedRoi.IsEmpty)
                {
                    // 拖动已选中的 ROI
                    int x = e.X - _roiDragOffset.X;
                    int y = e.Y - _roiDragOffset.Y;
                    ROIs.Remove(_selectedRoi);
                    _selectedRoi = new Rectangle(x, y, _selectedRoi.Width, _selectedRoi.Height);
                    ROIs.Add(_selectedRoi);
                    Invalidate();
                    return;
                }

                // 绘制中 ROI
                if (!RoiStart.IsEmpty)
                {
                    int x = Math.Min(e.X, RoiStart.X);
                    int y = Math.Min(e.Y, RoiStart.Y);
                    int w = Math.Abs(e.X - RoiStart.X);
                    int h = Math.Abs(e.Y - RoiStart.Y);
                    _drawingRoi = new Rectangle(x, y, w, h);
                    Invalidate();
                }
            }
            else if (isDragging && originalImage != null && rOIModel == ROIModel.Select)
            {
                // 拖动模式
                // 计算拖动距离并更新图像位置
                int deltaX = e.X - dragStart.X;
                int deltaY = e.Y - dragStart.Y;

                imagePosition.X += deltaX;
                imagePosition.Y += deltaY;

                for (int i = 0; i < ROIs.Count; i++)
                {
                    ROIs[i] = new Rectangle(ROIs[i].X + deltaX, ROIs[i].Y + deltaY, ROIs[i].Width, ROIs[i].Height);
                }

                dragStart = e.Location;
                Invalidate();
            }
            if (isCtrlPressed)
                Invalidate();
        }

        private void ZoomablePictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
                this.Cursor = Cursors.Default;
            }
            else if (e.Button == MouseButtons.Right)
            {
                isCtrlPressed = false;
                this.Cursor = Cursors.Default;
                Invalidate();
            }

            if (rOIModel == ROIModel.RectangleROI && !_drawingRoi.IsEmpty && _drawingRoi.Width > 5 && _drawingRoi.Height > 5)
            {
                //将ROI添加到列表中
                ROIs.Add(_drawingRoi);
                CountComponentsInROI(_drawingRoi, out List<Component> comps);
            }
            _drawingRoi = Rectangle.Empty;
            Invalidate();
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

        public void SetROIModel(ROIModel model)
        {
            this.rOIModel = model;
        }
    }

    public enum ROIModel
    {
        Select,
        RectangleROI
    }
}
