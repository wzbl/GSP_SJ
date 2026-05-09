using CKVisionAppNet;
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
    public partial class MarkRectify : Form
    {
        private IntPtr M_View = IntPtr.Zero;

        private double MoveX;
        private double MoveY;

        private double dMoveX;
        private double dMoveY;

        CKVisionAppApi.ViewDrawCallBack drawCallBack;// 视图显示自定义绘制图形回调

        CKVisionAppApi.ViewMessageCallBack msgCallBack;// 视图消息回调

        public double Dx { get; set; }
        public double Dy { get; set; }

        public bool Isok { get; set; }
        public string Code { get; set; }

        private string FormatStr(double Pix_x, double Pix_Y, double dx, double dy)
        {
            return String.Format("View: X {0:F1} Y {1:F1}   DX:{2:F3} DY:{3:F3}", Pix_x, Pix_Y, dx, dy);
        }
        public void ViewDrawCallBack(IntPtr hDC, int x, int y, double scale, IntPtr pUserParam)
        {
            using (Graphics tGrap = Graphics.FromHdc(hDC))
            {
                Pen Pen = new Pen(Color.Blue, 2);

                PointF Mst1 = new PointF((float)(MoveX - 20 + x), (float)(MoveY + y));
                PointF Med1 = new PointF((float)(MoveX + 20 + x), (float)(MoveY + y));

                PointF Mst2 = new PointF((float)(MoveX + x), (float)(MoveY - 20 + y));
                PointF Med2 = new PointF((float)(MoveX + x), (float)(MoveY + 20 + y));

                tGrap.DrawLine(Pen, Mst1, Med1);
                tGrap.DrawLine(Pen, Mst2, Med2);

                Pen.Dispose();
                tGrap.Dispose();
            }
        }
        /// <summary>
        /// 视图消息回调函数
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="pUserParam"></param>
        public void ViewMsgCallBack(uint msg, int wParam, int lParam, IntPtr pUserParam)
        {
            switch (msg)
            {
                case (int)AppVIEW_WM.MOUSEMOVE: //鼠标移动消息
                    {
                        double dx = (double)(lParam & 0xffff);
                        double dy = (double)(((lParam >> 16) & 0xffff));

                        CKVisionAppApi.ViewToImage(M_View, ref dx, ref dy); // 视图窗口坐标转换到图像上的坐标
                        Header.Values.Description = FormatStr(dx, dy, this.Dx, this.Dy);
                    }
                    break;
                case (int)AppVIEW_WM.LBUTTONDOWN:   //鼠标左键点击消息
                    {
                        double dx = (double)(lParam &0xffff);
                        double dy = (double)(((lParam >>16) & 0xffff));
                        this.MoveX = (int)dx;
                        this.MoveY = (int)dy;

                        double iwidth = Global.ImageWidth, iHight = Global.ImageHight;
                        //SysPara.VisionApp.GetImageWh(M_View, out iwidth, out iHight);

                        double IcentX = iwidth / 2;
                        double IcentY = iHight / 2;

                        CKVisionAppApi.ImageToView(M_View, ref IcentX, ref IcentY);// 视图窗口坐标转换到图像上的坐标

                        this.dMoveX = dx - IcentX;
                        this.dMoveY = dy - IcentY;

                        CKVisionAppApi.ViewToImage(M_View, ref dx, ref dy);// 视图窗口坐标转换到图像上的坐标

                        VisionApp.CalibType Calib = new VisionApp.CalibType();
                        Calib = Global.VisionApp.GetCalib(VisionGlobal.UpCailb_Obj);

                        double Cen_X, Cen_Y, X, Y;
                        Global.VisionApp.PixToPos(Calib, iwidth / 2, iHight / 2, out Cen_X, out Cen_Y);
                        Global.VisionApp.PixToPos(Calib, dx, dy, out X, out Y);

                        this.Dx = (X - Cen_X);
                        this.Dy = (Y - Cen_Y);

                        Header.Values.Description = FormatStr(dx, dy, this.Dx, this.Dy);


                    }
                    break;
                case (int)AppVIEW_WM.RBUTTONDOWN:
                    {
                        ;/// MessageBox.Show("ViewMsgCallBack RBUTTONDOWN");

                    }
                    break;
                case (int)AppVIEW_WM.MOUSEWHEEL:
                    {
                        ///  MessageBox.Show("ViewMsgCallBack MOUSEWHEEL");

                    }
                    break;
            }
        }
        public MarkRectify()
        {
            InitializeComponent();
            this.Header.MouseDown += TopBar_MouseDown;
            this.Header.MouseMove += TopBar_MouseMove;
        }
        #region 点击panel控件移动窗口
        private Point downPoint;
        private void TopBar_MouseDown(object sender, MouseEventArgs e)
        {
            downPoint = new Point(e.X, e.Y);
        }
        private void TopBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Location = new Point(this.Location.X + e.X - downPoint.X,
                    this.Location.Y + e.Y - downPoint.Y);
            }
        }
        #endregion
        private void MarkRectify_Load(object sender, EventArgs e)
        {
            M_View = Global.VisionApp.CreateView(this.View_pic.Handle, this.View_pic.Width, this.View_pic.Height, 1);
            Global.VisionApp.SetView(M_View, "Task5", "十字线", "M_View");
            //视图自定义绘图回调
            drawCallBack = new CKVisionAppApi.ViewDrawCallBack(ViewDrawCallBack);
            CKVisionAppApi.SetDrawCallback(M_View, drawCallBack, this.Handle);

            msgCallBack = new CKVisionAppApi.ViewMessageCallBack(ViewMsgCallBack);
            CKVisionAppApi.SetMsgCallback(M_View, msgCallBack, this.Handle);
            Global.VisionApp.ExecuteProc("Task5", 0);
            Global.VisionApp.ProcEndEvent += new EventHandler(RefImage);
            
            //初始化语言
            BrowApp.Language.Language.Instance.UpdateLanguage(this, null);
        }
        void RefImage(object sender, EventArgs e)
        {
            try
            {
                switch (sender)
                {
                    case "Task5":
                        Global.VisionApp.RedrawView(M_View);
                        CKVisionAppApi.SetViewAllMode(M_View, false);
                        break;
                }
            }
            catch { }

        }

        private void OK_btn_Click(object sender, EventArgs e)
        {
            Global.VisionApp.ProcEndEvent -= new EventHandler(RefImage);
            CKVisionAppApi.SetViewTool(M_View, IntPtr.Zero);
            CKVisionAppApi.FreeView(M_View);
            M_View=IntPtr.Zero;
            this.Isok = true;
            this.Close();
        }

        private void Exit_btn_Click(object sender, EventArgs e)
        {
            Global.VisionApp.ProcEndEvent -= new EventHandler(RefImage);
            this.Isok = false;
            CKVisionAppApi.SetViewTool(M_View, IntPtr.Zero);
            CKVisionAppApi.FreeView(M_View);
            M_View = IntPtr.Zero;
            this.Close();
        }
    }
}
