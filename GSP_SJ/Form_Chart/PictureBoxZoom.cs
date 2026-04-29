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


    public enum Type_Window
    {
        None = 0,
        Puzzle,
        Screen
    };
}
