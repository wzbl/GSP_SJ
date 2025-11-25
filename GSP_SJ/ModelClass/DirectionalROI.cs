using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSP_SJ.ModelClass
{
    public class DirectionalROI
    {
        public Rectangle Bounds { get; set; }
        public string Name { get; set; }
        public Color Color { get; set; } = Color.Red;
        public bool IsSelected { get; set; }
        public float Angle { get; set; } // 旋转角度（度）
        public PointF RotationCenter { get; set; } // 旋转中心

        protected const int HandleSize = 8;//8
        protected const int RotationHandleDistance = 0;//30

        public Action<Image, DirectionalROI> ChangeROI;

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

        public DirectionalROI(Rectangle bounds, string name = "")
        {
            Bounds = bounds;
            Name = name;
            RotationCenter = new PointF(bounds.X + bounds.Width / 2f, bounds.Y + bounds.Height / 2f);
        }

        // 获取旋转后的边界点
        protected virtual PointF[] GetTransformedPoints()
        {
            PointF center = RotationCenter;
            PointF[] points = new PointF[]
            {
            new PointF(Bounds.Left, Bounds.Top),
            new PointF(Bounds.Right, Bounds.Top),
            new PointF(Bounds.Right, Bounds.Bottom),
            new PointF(Bounds.Left, Bounds.Bottom)
            };

            if (Angle != 0)
            {
                double angleRad = Angle * Math.PI / 180.0;
                float cos = (float)Math.Cos(angleRad);
                float sin = (float)Math.Sin(angleRad);

                for (int i = 0; i < points.Length; i++)
                {
                    // 平移至原点
                    float x = points[i].X - center.X;
                    float y = points[i].Y - center.Y;

                    // 旋转
                    float newX = x * cos - y * sin;
                    float newY = x * sin + y * cos;

                    // 平移回原位置
                    points[i] = new PointF(newX + center.X, newY + center.Y);
                }
            }

            return points;
        }

        // 绘制ROI
        public virtual void Draw(Graphics g)
        {
            PointF[] points = GetTransformedPoints();

            using (Pen pen = new Pen(Color, IsSelected ? 2f : 1f))
            using (Brush fillBrush = new SolidBrush(Color.FromArgb(50, Color)))
            {
                // 填充多边形
                g.FillPolygon(fillBrush, points);
                // 绘制边框
                g.DrawPolygon(pen, points);

                // 绘制方向箭头
                //DrawDirectionArrow(g, points);

                // 如果被选中，绘制控制点
                if (IsSelected)
                {
                    DrawHandles(g, points);
                }
            }

            // 绘制名称
            if (!string.IsNullOrEmpty(Name))
            {
                using (Brush textBrush = new SolidBrush(Color))
                using (Font font = new Font("Arial", 8))
                {
                    PointF textPoint = GetTextPosition(points);
                    g.DrawString(Name, font, textBrush, textPoint);
                }
            }
        }

        // 绘制方向箭头
        protected virtual void DrawDirectionArrow(Graphics g, PointF[] points)
        {
            PointF center = RotationCenter;
            PointF directionPoint = GetDirectionPoint(center);

            using (Pen arrowPen = new Pen(Color, 2f))
            {
                // 绘制方向线
                g.DrawLine(arrowPen, center, directionPoint);

                // 绘制箭头
                DrawArrowHead(g, arrowPen, center, directionPoint);
            }
        }

        // 获取方向点
        protected virtual PointF GetDirectionPoint(PointF center)
        {
            double angleRad = Angle * Math.PI / 180.0;
            float distance = Math.Min(Bounds.Width, Bounds.Height) * 0.4f;

            float x = center.X + distance * (float)Math.Cos(angleRad);
            float y = center.Y + distance * (float)Math.Sin(angleRad);

            return new PointF(x, y);
        }

        // 绘制箭头头部
        protected virtual void DrawArrowHead(Graphics g, Pen pen, PointF start, PointF end)
        {
            float arrowSize = 8f;
            double angle = Math.Atan2(end.Y - start.Y, end.X - start.X);

            PointF[] arrowPoints = new PointF[3];
            arrowPoints[0] = end;
            arrowPoints[1] = new PointF(
                end.X - arrowSize * (float)Math.Cos(angle - Math.PI / 6),
                end.Y - arrowSize * (float)Math.Sin(angle - Math.PI / 6)
            );
            arrowPoints[2] = new PointF(
                end.X - arrowSize * (float)Math.Cos(angle + Math.PI / 6),
                end.Y - arrowSize * (float)Math.Sin(angle + Math.PI / 6)
            );

            g.DrawLines(pen, arrowPoints);
            g.FillPolygon(new SolidBrush(pen.Color), arrowPoints);
        }

        // 绘制控制点
        protected virtual void DrawHandles(Graphics g, PointF[] points)
        {
            using (Brush handleBrush = new SolidBrush(Color.White))
            using (Pen handlePen = new Pen(Color.Black))
            {
                // 四个角控制点
                for (int i = 0; i < points.Length; i++)
                {
                    RectangleF handle = new RectangleF(
                        points[i].X - HandleSize / 2f,
                        points[i].Y - HandleSize / 2f,
                        HandleSize, HandleSize
                    );
                    g.FillRectangle(handleBrush, handle);
                    g.DrawRectangle(handlePen, handle.Left, handle.Top, handle.Width, handle.Height);
                }

                // 旋转控制点
                PointF rotationHandle = GetRotationHandlePoint();
                RectangleF rotationHandleRect = new RectangleF(
                    rotationHandle.X - HandleSize / 2f,
                    rotationHandle.Y - HandleSize / 2f,
                    HandleSize, HandleSize
                );
                g.FillRectangle(handleBrush, rotationHandleRect);
                g.DrawRectangle(handlePen, rotationHandleRect.Left, rotationHandleRect.Top,
                               rotationHandleRect.Width, rotationHandleRect.Height);

                // 旋转中心点
                RectangleF centerHandle = new RectangleF(
                    RotationCenter.X - HandleSize / 2f,
                    RotationCenter.Y - HandleSize / 2f,
                    HandleSize, HandleSize
                );
                g.FillRectangle(Brushes.Yellow, centerHandle);
                g.DrawRectangle(handlePen, centerHandle.Left, centerHandle.Top,
                               centerHandle.Width, centerHandle.Height);
            }
        }

        // 获取旋转控制点位置
        protected virtual PointF GetRotationHandlePoint()
        {
            double angleRad = Angle * Math.PI / 180.0;
            float distance = RotationHandleDistance;

            float x = RotationCenter.X + distance * (float)Math.Cos(angleRad);
            float y = RotationCenter.Y + distance * (float)Math.Sin(angleRad);

            return new PointF(x, y);
        }

        // 获取文本位置
        protected virtual PointF GetTextPosition(PointF[] points)
        {
            // 文本显示在左上角
            return points[0];
        }

        // 检查点是否在ROI内
        public virtual bool Contains(Point point)
        {
            PointF[] points = GetTransformedPoints();

            // 简单的矩形检测（实际应该使用多边形检测）
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddPolygon(points);
                return path.IsVisible(point);
            }
        }

        // 获取控制点类型
        public virtual HandleType GetHandleAtPoint(Point point)
        {
            if (!IsSelected) return HandleType.None;

            PointF[] points = GetTransformedPoints();

            // 检查角控制点
            for (int i = 0; i < points.Length; i++)
            {
                RectangleF handle = new RectangleF(
                    points[i].X - HandleSize / 2f,
                    points[i].Y - HandleSize / 2f,
                    HandleSize, HandleSize
                );
                if (handle.Contains(point))
                    return (HandleType)(i + 1); // TopLeft, TopRight, BottomRight, BottomLeft
            }

            // 检查旋转控制点
            PointF rotationHandle = GetRotationHandlePoint();
            RectangleF rotationHandleRect = new RectangleF(
                rotationHandle.X - HandleSize / 2f,
                rotationHandle.Y - HandleSize / 2f,
                HandleSize, HandleSize
            );
            if (rotationHandleRect.Contains(point))
                return HandleType.Rotate;

            // 检查旋转中心点
            RectangleF centerHandle = new RectangleF(
                RotationCenter.X - HandleSize / 2f,
                RotationCenter.Y - HandleSize / 2f,
                HandleSize, HandleSize
            );
            if (centerHandle.Contains(point))
                return HandleType.Center;

            // 检查是否在多边形内
            return Contains(point) ? HandleType.Move : HandleType.None;
        }

        // 应用变换
        public virtual void ApplyTransform(HandleType handleType, Point delta)
        {
            switch (handleType)
            {
                case HandleType.Move:
                    Bounds = new Rectangle(Bounds.X + delta.X, Bounds.Y + delta.Y, Bounds.Width, Bounds.Height);
                    RotationCenter = new PointF(RotationCenter.X + delta.X, RotationCenter.Y + delta.Y);
                    break;

                case HandleType.TopLeft:
                    Bounds = new Rectangle(Bounds.X + delta.X, Bounds.Y + delta.Y,
                                         Bounds.Width - delta.X, Bounds.Height - delta.Y);
                    break;

                case HandleType.TopRight:
                    Bounds = new Rectangle(Bounds.X, Bounds.Y + delta.Y,
                                         Bounds.Width + delta.X, Bounds.Height - delta.Y);
                    break;

                case HandleType.BottomRight:
                    Bounds = new Rectangle(Bounds.X, Bounds.Y,
                                         Bounds.Width + delta.X, Bounds.Height + delta.Y);
                    break;

                case HandleType.BottomLeft:
                    Bounds = new Rectangle(Bounds.X + delta.X, Bounds.Y,
                                         Bounds.Width - delta.X, Bounds.Height + delta.Y);
                    break;

                case HandleType.Rotate:
                    // 计算新的角度
                    //PointF vector = new PointF(delta.X, delta.Y);
                    //Angle += (float)(Math.Atan2(vector.Y, vector.X) * 180 / Math.PI);
                    //// 限制角度在 0-360 度
                    //Angle = Angle % 360;
                    //if (Angle < 0) Angle += 360;
                    break;

                case HandleType.Center:
                    RotationCenter = new PointF(RotationCenter.X + delta.X, RotationCenter.Y + delta.Y);
                    break;
            }

            // 确保尺寸有效
            if (Bounds.Width < 10) Bounds = new Rectangle(Bounds.X, Bounds.Y, 10, Bounds.Height);
            if (Bounds.Height < 10) Bounds = new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, 10);


        }
    }

    // 扩展的控制点类型枚举
    public enum HandleType
    {
        None = 0,
        TopLeft = 1,
        TopRight = 2,
        BottomRight = 3,
        BottomLeft = 4,
        Move = 5,
        Rotate = 6,
        Center = 7
    }
}
