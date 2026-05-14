using BrowApp;
using BrowApp.Language;
using BrowApp.MessageTip;
using CKVisionAppNet;
using ComponentFactory.Krypton.Toolkit;
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
    public partial class CalibFrom : KryptonForm
    {
        private IntPtr ImageView = IntPtr.Zero;
        private double MoveX;
        private double MoveY;
        private double dMoveX;
        private double dMoveY;


        CKVisionAppApi.ViewDrawCallBack drawCallBack;// 视图显示自定义绘制图形回调
        CKVisionAppApi.ViewMessageCallBack msgCallBack;//视图消息回调

        public bool RunFlag { get; set; }
        public double Dx { get; set; }
        public double Dy { get; set; }
        public string Code { get; set; }
        public double Angle { get; set; }

        public double offsetX { get; set; }
        public double offsetY { get; set; }

        double TRANSFORM_X(double x, double y, double cosa, double sina, double cx, double cy)
        {
            return (((x) - (cx)) * (cosa) + ((y) - (cy)) * (sina) + (cx));
        }

        double TRANSFORM_Y(double x, double y, double cosa, double sina, double cx, double cy)
        {
            return (((y) - (cy)) * (cosa) - ((x) - (cx)) * (sina) + (cy));
        }
        public PointF RotPoint(PointF Center, float dist, float Angle)
        {
            PointF newPt = new PointF();
            PointF rotPt = new PointF(Center.X + dist, Center.Y);
            double arc = Angle * Math.PI / 180;
            double cosa = Math.Cos(arc);
            double sina = Math.Sin(arc);

            newPt.X = (float)TRANSFORM_X((double)rotPt.X, (double)rotPt.Y, cosa, sina, (double)Center.X, (double)Center.Y);
            newPt.Y = (float)TRANSFORM_Y((double)rotPt.X, (double)rotPt.Y, cosa, sina, (double)Center.X, (double)Center.Y);
            return newPt;
        }


        public void ViewDrawCallBack(IntPtr hDC, int x, int y, double scale, IntPtr pUserParam)
        {
            using (Graphics tGrap = Graphics.FromHdc(hDC))
            {

                int lineLength = 100;//箭头延伸长度 

                tGrap.SmoothingMode = SmoothingMode.AntiAlias;
                tGrap.InterpolationMode = InterpolationMode.HighQualityBicubic;
                tGrap.CompositingQuality = CompositingQuality.HighQuality;

                PointF temCenter = new PointF((float)((Global.ImageWidth / 2) * scale), (float)((Global.ImageHight / 2) * scale));// 中心点

                float lineAngle = (float)this.Angle;///箭头旋转角度

                Pen tempPen = new Pen(Color.Red, 5);
                AdjustableArrowCap aac = new AdjustableArrowCap(3, 4); /* 定义可调箭头 */
                tempPen.CustomEndCap = aac;

                PointF endPt = RotPoint(temCenter, lineLength, lineAngle);///箭头延伸长度 计算延伸与旋转之后的点

                tGrap.DrawLine(tempPen, temCenter, endPt);

                tempPen.Dispose();

                Brush brush = new SolidBrush(Color.Red);
                Font ft = new Font("System", 15, FontStyle.Bold);
                tGrap.DrawString(this.Angle.ToString() + "°", ft, brush, (float)(endPt.X + 10), (float)(endPt.Y + 20));

                brush.Dispose();
                ft.Dispose();


                //绘制鼠标跟随十字
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
        public void ViewMsgCallBack(uint msg, int wParam,int lParam, IntPtr pUserParam)
        {
            switch (msg)
            {
                case (int)CKVisionAppNet.AppVIEW_WM.MOUSEMOVE: //鼠标移动消息
                    {
                        double dx = (double)(lParam & 0xffff);
                        double dy = (double)(((lParam >> 16) & 0xffff));

                        CKVisionAppApi.ViewToImage(ImageView, ref dx, ref dy); // 视图窗口坐标转换到图像上的坐标
                        View_txt.Text = String.Format("Image X:{0:F1}", dx, dy) + String.Format(",Y {0:F1}", dy); ;
                    }
                    break;
                case (int)CKVisionAppNet.AppVIEW_WM.LBUTTONDOWN:   //鼠标左键点击消息
                    {
                        double dx = (double)(lParam & 0xffff);
                        double dy = (double)(((lParam >> 16) & 0xffff));
                        this.MoveX = (int)dx;
                        this.MoveY = (int)dy;

                        double iwidth = Global.ImageWidth, iHight = Global.ImageHight;

                        //Global.VisionApp.GetImageWh(ImageView, out iwidth, out iHight);

                        double IcentX = iwidth / 2;
                        double IcentY = iHight / 2;
                        CKVisionAppApi.ImageToView(ImageView, ref IcentX, ref IcentY);// 视图窗口坐标转换到图像上的坐标

                        this.dMoveX = dx - IcentX;
                        this.dMoveY = dy - IcentY;

                        CKVisionAppApi.ViewToImage(ImageView, ref dx, ref dy);// 视图窗口坐标转换到图像上的坐标
                        VisionApp.CalibType calib = new VisionApp.CalibType();
                        calib = Global.VisionApp.GetCalib(VisionGlobal.UpCailb_Obj);

                        double Cen_X, Cen_Y, X, Y;
                        Global.VisionApp.PixToPos(calib, iwidth / 2, iHight / 2, out Cen_X, out Cen_Y);
                        Global.VisionApp.PixToPos(calib, dx, dy, out X, out Y);

                        this.Dx = (X - Cen_X);
                        this.Dy = (Y - Cen_Y);

                        Dx_lab.Values.ExtraText = (X - Cen_X).ToString("F3");
                        DY_lab.Values.ExtraText = (Y - Cen_Y).ToString("F3");
                    }
                    break;
                case (int)CKVisionAppNet.AppVIEW_WM.RBUTTONDOWN:
                    {
                        ;///  BrowApp.APP.Tip.ShowTip(0, "警告".tr(), "ViewMsgCallBack RBUTTONDOWN");

                    }
                    break;
                case (int)CKVisionAppNet.AppVIEW_WM.MOUSEWHEEL:
                    {
                        /// BrowApp.APP.Tip.ShowTip(0, "警告".tr(), "ViewMsgCallBack MOUSEWHEEL");
                    }
                    break;
            }
        }
        void Refimae(object sender, EventArgs e)
        {
            try
            {
                switch (sender)
                {
                    case "Task5":
                        Global.VisionApp.RedrawView(ImageView);
                        CKVisionAppApi.SetViewAllMode(ImageView, false);
                        break;
                }
            }
            catch { }
        }

        public CalibFrom()
        {
            InitializeComponent();
            // 窗口加载时强制置顶
            this.Load += (sender, e) =>
            {
                ForceTopMost();
                Win32Api.SetForegroundWindow(this.Handle); // 确保获得焦点
            };


            this.Header.MouseDown += TopBar_MouseDown;
            this.Header.MouseMove += TopBar_MouseMove;
            Global.VisionApp.ProcEndEvent += new EventHandler(Refimae);
            
        }
        //强制置顶到所有窗口之上
        private void ForceTopMost()
        {
            // 
            Win32Api.SetWindowPos(
                this.Handle,
                Win32Api.HWND_TOPMOST,
                0, 0, 0, 0,
                Win32Api.SWP_NOMOVE | Win32Api.SWP_NOSIZE | Win32Api.SWP_SHOWWINDOW
            );
        }

        // 可选：当窗口失去焦点时重新置顶
        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            ForceTopMost();
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

        private void CalibFrom_Load(object sender, EventArgs e)
        {
            ImageView = Global.VisionApp.CreateView(this.View_pic.Handle, this.View_pic.Width, this.View_pic.Height, 1);
            Global.VisionApp.SetView(ImageView, "Task5", "十字线", "ImageView");
            //视图自定义绘图回调
            drawCallBack = new CKVisionAppApi.ViewDrawCallBack(ViewDrawCallBack);
            CKVisionAppApi.SetDrawCallback(ImageView, drawCallBack, this.Handle);

            msgCallBack = new CKVisionAppApi.ViewMessageCallBack(ViewMsgCallBack);
            CKVisionAppApi.SetMsgCallback(ImageView, msgCallBack, this.Handle);
            Global.VisionApp.ExecuteProc("Task5", 0);
            this.Ange_num.Value = decimal.Parse(this.Angle.ToString());
            this.Code_txt.Text = this.Code.ToString();
            BrowApp.Language.Language.Instance.UpdateLanguage(this, null);
        }

        private void Rectify_Btn_Click(object sender, EventArgs e)
        {
            Global.Light.SetRgbLight(Global.Systemdata.P_LED.LED_R, Global.Systemdata.P_LED.LED_G, Global.Systemdata.P_LED.LED_B);
            double X = Global.X轴.GetPrfPos();
            double Y = Global.Y轴.GetPrfPos();
            new HandFlow().SafeMoveXyz(0, X-this.Dx, Y-this.Dy, Global.CamHeight);
            Global.VisionApp.ExecuteProc("Task5", 0);

            this.MoveX = this.MoveX - this.dMoveX;
            this.MoveY = this.MoveY - this.dMoveY;
        }

        private void Rectify_Btn2_Click(object sender, EventArgs e)
        {
            Global.Light.SetRgbLight(Global.Systemdata.P_LED.LED_R, Global.Systemdata.P_LED.LED_G, Global.Systemdata.P_LED.LED_B);
            double X = Global.X轴.GetPrfPos();
            double Y = Global.Y轴.GetPrfPos();
            new HandFlow().SafeMoveXyz(0, X - offsetX, Y - offsetY, Global.CamHeight);
            Global.VisionApp.ExecuteProc("Task5", 0);
        }

        private void Ok_Btn_Click(object sender, EventArgs e)
        {
            Global.VisionApp.ProcEndEvent -= new EventHandler(Refimae);
            if (kryptonCheckBox1.Checked)
            {
                string FBCCode, XYCode, BoardSide, ProductCode, ProductName;
                this.Angle = Convert.ToDouble(this.Ange_num.Value);
                Global.ReadFAIConfig(out FBCCode, out XYCode, out BoardSide, out ProductCode, out ProductName);
                //if (new JD.Fai.Data.FlyingProbe().SetElementAngle(FBCCode, this.Code, Convert.ToDouble(this.Ange_num.Value), this.kryptonCheckBox2.Checked))
                //{
                //    Global.BomData = new JD.Fai.Data.FlyingProbe().GetDataTable(FBCCode);
                //    Global.TcpClass.Send("M:RefBOM_OK");
                //    CKVisionAppApi.SetViewTool(ImageView, IntPtr.Zero);
                //    CKVisionAppApi.FreeView(ImageView);
                //    ImageView=IntPtr.Zero;
                //    Global.GlobBomRefEvent?.Invoke(this, new EventArgs());
                //    this.RunFlag = true;
                //    this.Dispose();
                //    this.Close();
                //}
                //else
                {
                  FloatingTip.ShowError("数据写入失败".tr());
                }
            }
            else
            {
                CKVisionAppApi.SetViewTool(ImageView, IntPtr.Zero);
                CKVisionAppApi.FreeView(ImageView);
                ImageView=IntPtr.Zero;
                this.Dispose();
                this.RunFlag = true;
                this.Close();
            }
        }

        private void Exit_btn_Click(object sender, EventArgs e)
        {
            Global.VisionApp.ProcEndEvent -= new EventHandler(Refimae);
            this.RunFlag = false;
            CKVisionAppApi.SetViewTool(ImageView, IntPtr.Zero);
            CKVisionAppApi.FreeView(ImageView);
            ImageView=IntPtr.Zero;
            this.Dispose();
            this.Close();
        }

        private void SetBtn_Click(object sender, EventArgs e)
        {
            Global.VisionApp.SetToolData("Task5", "十字线", 102, 0, SetSize.Value.ToString(), 1);
            Global.VisionApp.ExecuteProc("Task5", 0);
        }
    }
}
