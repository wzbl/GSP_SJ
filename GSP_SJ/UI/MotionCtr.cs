using BrowApp;
using BrowApp.IO;
using BrowApp.Language;
using BrowApp.MessageTip;
using BrowLib;
using BrowLib.Motion;
using BrowLib.Static;
using BrowLib.Units;
using CKVisionAppNet;
using ComponentFactory.Krypton.Toolkit;
using GSP_SJ.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.util;
using System.Windows.Forms;
using System.Xml.Linq;
using static CKVisionAppNet.CKVisionAppApi;

namespace GSP.UI
{
    public partial class MotionCtr:KryptonForm
    {
        private KeyboardHook keyboard;
        private FlowControl hFlow=new FlowControl();
        private List<string> Motors = new List<string>();
        List<List<bool>> motorStatusTemp;
        private System.Windows.Forms.Timer timer;
        private IntPtr ImageView = IntPtr.Zero;
        CKVisionAppApi.ViewDrawCallBack drawCallBack;// 视图显示自定义绘制图形回调
        CKVisionAppApi.ViewMessageCallBack msgCallBack;// 视图消息回调
        private double MoveX;
        private double MoveY;
        private double dMoveX;
        private double dMoveY;
        private double Dx;
        private double Dy;
        private double Now_X, Now_Y;

        private string AxisName;//轴名称
        private string SelectAxisName;//选中轴名称

        private WaitFrm busy;
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

        private string FormatStr(double Pix_x,double Pix_Y,double dx,double dy)
        {
          return  String.Format("View: X {0:F1} Y {1:F1}   DX:{2:F3} DY:{3:F3}", Pix_x, Pix_Y, dx, dy);
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

                        CKVisionAppApi.ViewToImage(ImageView, ref dx, ref dy); // 视图窗口坐标转换到图像上的坐标
                        MotionHeader.Values.Description = FormatStr(dx, dy, this.Dx, this.Dy);
                    }
                    break;
                case (int)AppVIEW_WM.LBUTTONDOWN:   //鼠标左键点击消息
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

                        VisionApp.CalibType Calib = new VisionApp.CalibType();
                        Calib = Global.VisionApp.GetCalib(VisionGlobal.UpCailb_Obj);

                        double Cen_X, Cen_Y, X, Y;
                        Global.VisionApp.PixToPos(Calib, iwidth / 2, iHight / 2, out Cen_X, out Cen_Y);
                        Global.VisionApp.PixToPos(Calib, dx, dy, out X, out Y);

                        this.Dx = (X - Cen_X);
                        this.Dy = (Y - Cen_Y);


                        MotionHeader.Values.Description=FormatStr(dx, dy, this.Dx, this.Dy);

                        if (IsFollow_ck.Checked)
                        {
                            //Global.VisionApp.DispImageView(ImageView, "Task5", "十字线");
                            Global.Light.SetRgbLight(Global.Systemdata.P_LED.LED_R, Global.Systemdata.P_LED.LED_G, Global.Systemdata.P_LED.LED_B);
                            new HandFlow().SafeMoveXyz(0, Global.X轴.GetPrfPos() - this.Dx,
                                Global.Y轴.GetPrfPos() - this.Dy, Global.CamHeight);
                            if (!Ck_Current.Checked) { Global.VisionApp.ExecuteProc("Task5", 0); }
                            this.MoveX = this.MoveX - this.dMoveX;
                            this.MoveY = this.MoveY - this.dMoveY;
                        }
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
        public MotionCtr()
        {
            InitializeComponent();

            //设置窗体的双缓冲
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer
            | ControlStyles.ResizeRedraw
            | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            //利用反射设置DataGridView的双缓冲
            Type dgvType = this.dgvMotorStatus.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
            BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(this.dgvMotorStatus, true, null);

            this.MotionHeader.MouseDown += TopBar_MouseDown;
            this.MotionHeader.MouseMove += TopBar_MouseMove;

            keyboard = new KeyboardHook();
            keyboard.Start();
            keyboard.KeyDown += new KeyEventHandler(JKeyDown);
            keyboard.KeyUp += new KeyEventHandler(JKeyUp);

            DgvMotIni();
            IoCtrIni();
            timer = new System.Windows.Forms.Timer();

            timer.Tick += new System.EventHandler(this.Timer_Tick);

            timer.Interval = 100;
            timer.Start();
        }
        private void DgvMotIni()
        {
            try
            {
                foreach (BrowLib.Motion.AxisParm.CAxisParm axisParm in AxisParm.AParms)
                {
                    Motors.Add(axisParm.AxisName);
                }
                motorStatusTemp = new List<List<bool>>();
                for (int i = 0; i < Motors.Count; i++)
                {
                    int index = this.dgvMotorStatus.Rows.Add(Motors[i], "0.000", "0.000",
                     Resources.icon24_灰色,
                     Resources.icon24_绿色,
                    Resources.icon24_灰色,
                    Resources.icon24_灰色,
                    Resources.icon24_灰色);
                    this.dgvMotorStatus.Rows[index].Height = this.dgvMotorStatus.Height / (Motors.Count + 1);
                    if (i < 8)
                    {
                        this.dgvMotorStatus.Columns[i].Resizable = DataGridViewTriState.False;
                        this.dgvMotorStatus.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                    }
                    motorStatusTemp.Add(new List<bool> { false, false, false, false, false });
                }
            }
            catch (Exception ex) { }
        }
        private void IoCtrIni()
        {
            try
            {
                List<string> innames = new List<string>();

                List<string> outnames = new List<string>();

                foreach (BrowLib.GPIO.IOParm.CIOType IO in BrowLib.GPIO.IOParm.IOParms)
                {
                    if (IO.IOType.IndexOf("IN") > -1)
                    {
                        innames.Add(IO.IoName);
                    }
                    else
                    {
                        outnames.Add(IO.IoName);
                    }
                }
                ioControl1.InPartName = innames.ToArray();
                ioControl1.OutPartName = outnames.ToArray();
            }
            catch (Exception ex) { }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            #region 轴状态刷新
            if (dgvMotorStatus.RowCount < 0) { return; }
            try
            {
                for (int i = 0; i < dgvMotorStatus.RowCount; i++)
                {
                    AxisName = dgvMotorStatus.Rows[i].Cells[0].Value.ToString();

                    this.dgvMotorStatus.Rows[i].Cells[1].Value = BrowLib.Controller.Motion[AxisName].GetEncPos().ToString("F3") + "mm";
                    this.dgvMotorStatus.Rows[i].Cells[2].Value = BrowLib.Controller.Motion[AxisName].GetPrfPos().ToString("F3") + "mm";

                    if (motorStatusTemp[i][0] != BrowLib.Controller.Motion[AxisName].ALM())
                    {
                        motorStatusTemp[i][0] = BrowLib.Controller.Motion[AxisName].ALM();
                        this.dgvMotorStatus.Rows[i].Cells[3].Value = BrowLib.Controller.Motion[AxisName].ALM() ?
                        Resources.icon24_灰色 : Resources.icon24_红灯;
                    }
                    if (motorStatusTemp[i][1] != BrowLib.Controller.Motion[AxisName].GetSevOn())
                    {
                        motorStatusTemp[i][1] = BrowLib.Controller.Motion[AxisName].GetSevOn();
                        this.dgvMotorStatus.Rows[i].Cells[4].Value = BrowLib.Controller.Motion[AxisName].GetSevOn() ?
                        Resources.icon24_绿色 : Resources.icon24_灰色;
                    }
                    if (motorStatusTemp[i][2] != BrowLib.Controller.Motion[AxisName].ORG())
                    {
                        motorStatusTemp[i][2] = BrowLib.Controller.Motion[AxisName].ORG();
                        this.dgvMotorStatus.Rows[i].Cells[5].Value = BrowLib.Controller.Motion[AxisName].ORG() ?
                            Resources.icon24_灰色 : Resources.icon24_绿色;
                    }
                    if (motorStatusTemp[i][3] != BrowLib.Controller.Motion[AxisName].EL())
                    {
                        motorStatusTemp[i][3] = BrowLib.Controller.Motion[AxisName].EL();
                        this.dgvMotorStatus.Rows[i].Cells[6].Value = BrowLib.Controller.Motion[AxisName].EL() ?
                       Resources.icon24_灰色 : Resources.icon24_绿色;
                    }
                    if (motorStatusTemp[i][4] != BrowLib.Controller.Motion[AxisName].PL())
                    {
                        motorStatusTemp[i][4] = BrowLib.Controller.Motion[AxisName].PL();
                        this.dgvMotorStatus.Rows[i].Cells[7].Value = BrowLib.Controller.Motion[AxisName].PL() ?
                        Resources.icon24_灰色 : Resources.icon24_绿色;
                    }
                }
            }
            catch { }
            #endregion

            #region IO刷新
            ioControl1.Refresh();
            #endregion
        }
        /// <summary>
        /// 激活窗口
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <param name="nCmdShow">0:关闭窗口 1:正常大小显示窗口  2:最小化窗口 3:最大化窗口</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);
        ~MotionCtr()
        {
            keyboard.KeyDown -= new KeyEventHandler(JKeyDown);
            keyboard.KeyUp -= new KeyEventHandler(JKeyUp);
            Global.VisionApp.ProcEndEvent -= new EventHandler(RefImage);
            CKVisionAppApi.SetViewTool(ImageView, IntPtr.Zero);
            CKVisionAppApi.FreeView(ImageView);
            ImageView=IntPtr.Zero;
            keyboard.Stop();
        }
        private void MotionCtr_Load(object sender, EventArgs e)
        {
            Global.Light.SetRgbLight(Global.Systemdata.P_LED.LED_R, Global.Systemdata.P_LED.LED_G, Global.Systemdata.P_LED.LED_B);
            ImageView = Global.VisionApp.CreateView(this.M_View_pic.Handle, this.M_View_pic.Width, this.M_View_pic.Height, 1);
            Global.VisionApp.SetView(ImageView, "Task5", "十字线", "ImageView");

            //视图自定义绘图回调
            drawCallBack = new ViewDrawCallBack(ViewDrawCallBack);
            CKVisionAppApi.SetDrawCallback(ImageView, drawCallBack, this.Handle);

            msgCallBack = new ViewMessageCallBack(ViewMsgCallBack);
            CKVisionAppApi.SetMsgCallback(ImageView, msgCallBack, this.Handle);
            Global.VisionApp.ExecuteProc("Task5", 0);
            Global.VisionApp.ProcEndEvent += new EventHandler(RefImage);
            //初始化语言
            BrowApp.Language.Language.Instance.UpdateLanguage(this, null);
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

        void RefImage(object sender, EventArgs e)
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
        private void close_btn_Click(object sender, EventArgs e)
        {
            try
            {
                Controller.CardAPI.StopAxis();
                Global.VisionApp.ProcEndEvent -= new EventHandler(RefImage);
                if (ImageView != IntPtr.Zero)
                {
                    CKVisionAppApi.SetViewTool(ImageView, IntPtr.Zero);
                    CKVisionAppApi.FreeView(ImageView);
                    ImageView = IntPtr.Zero;
                }
                keyboard.Stop();
                
                this.Dispose();
                this.Close();
            }
            catch { }
        }
        public void FomActivated()
        {
            SystemEx.ShowWindow(this.Handle, 1);
        }
        private bool IsSelectAis()
        {
            if (SelectAxisName == null)
            {
                KryptonMessageBox.Show("请选中轴在执行操作".tr(), "提示".tr(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
        private void SwEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsSelectAis()) { return; }
            if (SwEnable.Checked)
            {
              BrowLib.Controller.Motion[SelectAxisName].SetServon();
            }
            else
            {
                BrowLib.Controller.Motion[SelectAxisName].SetServoff();

            }
        }

        private void dgvMotorStatus_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < this.dgvMotorStatus.RowCount)
            {
                SelectAxisName = dgvMotorStatus.Rows[e.RowIndex].Cells[0].Value.ToString();
               
                var re = AxisParm.AParms.Where(n => n.AxisName == SelectAxisName).ToList();
                if(re.Any()&& re!=null)
                this.Offset_Num.Value =decimal.Parse(re.First().Homeoffset.ToString());
            }
        }

        private void Stop_btn_Click(object sender, EventArgs e)
        {
            GLB.HomeStop = true;
            if (!IsSelectAis()) { return; }
            Controller.Motion[SelectAxisName].AxisStop();
        }

        private void Home_Btn_Click(object sender, EventArgs e)
        {
            if (!IsSelectAis()) { return; }
            GLB.HomeStop = false;
            if (busy == null || !busy.IsHandleCreated)
            {
              busy = new WaitFrm(Home_Btn);
            }
           
            busy.Enabled = true;
            Controller.SetLimit(false, SelectAxisName);
            Controller.Motion[SelectAxisName].Home(() =>
            {
                Invoke(new Action(() =>
                {
                    GLB.HomeStop = true;
                    busy.Enabled = false;
                }));
                Controller.SetLimit(true, SelectAxisName);
            }, 100000);
        }
        private void JMouseDown(object sender, MouseEventArgs e)
        {
            if (!Global.SystemInitialOk)
            {
                APP.Tip.ShowTip(0, "警告", "设备未归零,请先归零再执行操作".tr(), "确定");
                return;
            }
            if (Global.SystemRun)
            {
                FloatingTip.ShowWarning("设备自动运行不允许操作".tr());
                return;
            }
            string KeyName = ((System.Windows.Forms.Control)sender).Name;
            double Spd;
            if (fast_Rio.Checked) { Spd = Convert.ToDouble(FastSpd_num.Value); }
            else
                Spd = Convert.ToDouble(Slow_tb.Value);
            switch (Global.Model)
            {
                case 0:  //离线
                    switch (KeyName)
                    {
                        case "MX_N":       /*********【X轴-方向】***********/
                            dgvMotorStatus.FirstDisplayedScrollingRowIndex = 0;
                            if (Jop_Rio.Checked)
                            {
                                Global.X轴.JOP(0, Spd);
                            }
                            else
                            {
                                Global.X轴.PMove(Spd, 3000, -1 * Convert.ToDouble(Motdis_num.Value), 0);
                            }
                            break;
                        case "MX_P":              /*********【X轴+方向】***********/
                            dgvMotorStatus.FirstDisplayedScrollingRowIndex = 0;
                            if (Jop_Rio.Checked)
                            {
                                Global.X轴.JOP(1, Spd);
                            }
                            else
                            {
                                Global.X轴.PMove(Spd, 3000, Convert.ToDouble(Motdis_num.Value), 0);
                            }
                            break;
                        case "MY_N":       /*********【Y轴负方向】***********/
                            dgvMotorStatus.FirstDisplayedScrollingRowIndex = 1;
                            if (Jop_Rio.Checked)
                            {
                                Global.Y轴.JOP(0, Spd);
                            }
                            else
                            {
                                Global.Y轴.PMove(Spd, 3000, -1 * Convert.ToDouble(Motdis_num.Value), 0);
                            }
                            break;

                        case "MY_P":       /*********【Y轴正方向】***********/
                            dgvMotorStatus.FirstDisplayedScrollingRowIndex = 1;
                            if (Jop_Rio.Checked)
                            {
                                Global.Y轴.JOP(1, Spd);
                            }
                            else
                            {
                                Global.Y轴.PMove(Spd, 3000, Convert.ToDouble(Motdis_num.Value), 0);
                            }
                            break;

                        case "MZ_N":              /*********【Z轴负方向】***********/
                            dgvMotorStatus.FirstDisplayedScrollingRowIndex = 2;
                            if (Jop_Rio.Checked)
                            {
                                Global.Z轴.JOP(0, Spd);
                            }
                            else
                            {
                                Global.Z轴.PMove(Spd, 3000,-1*Convert.ToDouble(Motdis_num.Value), 0);
                            }
                            break;
                        case "MZ_P":       /*********【Z轴正方向】***********/
                            dgvMotorStatus.FirstDisplayedScrollingRowIndex = 2;
                            if (Jop_Rio.Checked)
                            {
                                Global.Z轴.JOP(1, Spd);
                            }
                            else
                            {
                                Global.Z轴.PMove(Spd, 3000, Convert.ToDouble(Motdis_num.Value), 0);
                            }
                            break;

                        case "MR_CW":       /*********【R+方向】***********/
                            dgvMotorStatus.FirstDisplayedScrollingRowIndex = 3;
                            if (Jop_Rio.Checked)
                            {
                                Global.R轴.JOP(1, Spd);
                            }
                            else
                            {
                              Global.R轴.PMove(Spd, 3000, Convert.ToDouble(Motdis_num.Value), 0);
                            }
                            break;
                        case "MR_CCW":       /*********【R-方向】***********/
                            dgvMotorStatus.FirstDisplayedScrollingRowIndex = 3;
                            if (Jop_Rio.Checked)
                            {
                                Global.R轴.JOP(0, Spd);
                            }
                            else
                            {
                                Global.R轴.PMove(Spd, 3000, -1*Convert.ToDouble(Motdis_num.Value), 0);
                            }
                            break;

                        case "LClaw_P":    /*********【左夹爪+方向】***********/
                            dgvMotorStatus.FirstDisplayedScrollingRowIndex = 4;
                            if (Jop_Rio.Checked)
                            {
                                Global.左夹爪轴.JOP(1, Spd);
                            }
                            else
                            {
                                Global.左夹爪轴.PMove(Spd, 500, Convert.ToDouble(Motdis_num.Value), 0);
                            }
                            break;
                        case "LClaw_N":    /*********【左夹爪-方向】***********/
                            dgvMotorStatus.FirstDisplayedScrollingRowIndex = 4;
                            if (Jop_Rio.Checked)
                            {
                                Global.左夹爪轴.JOP(0, Spd);
                            }
                            else
                            {
                                Global.左夹爪轴.PMove(Spd, 500, -1*Convert.ToDouble(Motdis_num.Value), 0);
                            }
                            break;

                        case "RClaw_P":    /*********【右夹爪+方向】***********/
                            dgvMotorStatus.FirstDisplayedScrollingRowIndex = 4;
                            if (Jop_Rio.Checked)
                            {
                                Global.右夹爪轴.JOP(1, Spd);
                            }
                            else
                            {
                                Global.右夹爪轴.PMove(Spd, 500, Convert.ToDouble(Motdis_num.Value), 0);
                            }
                            break;
                        case "RClaw_N":    /*********【右夹爪-方向】***********/
                            dgvMotorStatus.FirstDisplayedScrollingRowIndex = 4;
                            if (Jop_Rio.Checked)
                            {
                                Global.右夹爪轴.JOP(0, Spd);
                            }
                            else
                            {
                                Global.右夹爪轴.PMove(Spd, 500, -1 * Convert.ToDouble(Motdis_num.Value), 0);
                            }
                            break;

                        case "Width_P":    /*********【调宽+方向】***********/
                            dgvMotorStatus.FirstDisplayedScrollingRowIndex = 6;
                            if (Jop_Rio.Checked)
                            {
                                Global.调宽.JOP(1, Spd);
                            }
                            else
                            {
                                Global.调宽.PMove(Spd, 500, Convert.ToDouble(Motdis_num.Value), 0);
                            }
                            break;
                        case "Width_N":    /*********【调宽-方向】***********/
                            dgvMotorStatus.FirstDisplayedScrollingRowIndex = 6;
                            if (Jop_Rio.Checked)
                            {
                                Global.调宽.JOP(0, Spd);
                            }
                            else
                            {
                                Global.调宽.PMove(Spd, 500, -1 * Convert.ToDouble(Motdis_num.Value), 0);
                            }
                            break;
                    }
                    break;
                case 1://在线机
                case 2://860
                case 3://860P
                    switch (KeyName)
                    {
                        case "MX_N":       /*********【X轴-方向】***********/
                            dgvMotorStatus.FirstDisplayedScrollingRowIndex = 0;
                            if (Jop_Rio.Checked)
                            {
                                Global.X轴.JOP(0, Spd);
                            }
                            else
                            {
                                Global.X轴.PMove(Spd, 3000, -1 * Convert.ToDouble(Motdis_num.Value), 0);
                            }
                            break;
                        case "MX_P":              /*********【X轴+方向】***********/
                            dgvMotorStatus.FirstDisplayedScrollingRowIndex = 0;
                            if (Jop_Rio.Checked)
                            {
                                Global.X轴.JOP(1, Spd);
                            }
                            else
                            {
                                Global.X轴.PMove(Spd, 3000, Convert.ToDouble(Motdis_num.Value), 0);
                            }
                            break;
                        case "MY_N":       /*********【Y轴负方向】***********/
                            dgvMotorStatus.FirstDisplayedScrollingRowIndex = 1;
                            if (Jop_Rio.Checked)
                            {
                                Global.Y轴.JOP(1, Spd);
                            }
                            else
                            {
                                Global.Y轴.PMove(Spd, 3000, Convert.ToDouble(Motdis_num.Value), 0);
                            }
                            break;

                        case "MY_P":       /*********【Y轴正方向】***********/
                            dgvMotorStatus.FirstDisplayedScrollingRowIndex = 1;
                            if (Jop_Rio.Checked)
                            {
                                Global.Y轴.JOP(0, Spd);
                            }
                            else
                            {
                                Global.Y轴.PMove(Spd, 3000, -1*Convert.ToDouble(Motdis_num.Value), 0);
                            }
                            break;

                        case "MZ_N":              /*********【Z轴负方向】***********/
                            dgvMotorStatus.FirstDisplayedScrollingRowIndex = 2;
                            if (Jop_Rio.Checked)
                            {
                                Global.Z轴.JOP(0, Spd);
                            }
                            else
                            {
                                Global.Z轴.PMove(Spd, 3000, -1 * Convert.ToDouble(Motdis_num.Value), 0);
                            }
                            break;
                        case "MZ_P":       /*********【Z轴正方向】***********/
                            dgvMotorStatus.FirstDisplayedScrollingRowIndex = 2;
                            if (Jop_Rio.Checked)
                            {
                                Global.Z轴.JOP(1, Spd);
                            }
                            else
                            {
                                Global.Z轴.PMove(Spd, 3000, Convert.ToDouble(Motdis_num.Value), 0);
                            }
                            break;

                        case "MR_CW":       /*********【R+方向】***********/
                            dgvMotorStatus.FirstDisplayedScrollingRowIndex = 3;
                            if (Jop_Rio.Checked)
                            {
                                Global.R轴.JOP(1, Spd);
                            }
                            else
                            {
                                Global.R轴.PMove(Spd, 3000, Convert.ToDouble(Motdis_num.Value), 0);
                            }
                            break;
                        case "MR_CCW":       /*********【R-方向】***********/
                            dgvMotorStatus.FirstDisplayedScrollingRowIndex = 3;
                            if (Jop_Rio.Checked)
                            {
                                Global.R轴.JOP(0, Spd);
                            }
                            else
                            {
                                Global.R轴.PMove(Spd, 3000, -1 * Convert.ToDouble(Motdis_num.Value), 0);
                            }
                            break;

                        case "LClaw_P":    /*********【左夹爪+方向】***********/
                            dgvMotorStatus.FirstDisplayedScrollingRowIndex = 4;
                            if (Jop_Rio.Checked)
                            {
                                Global.左夹爪轴.JOP(1, Spd);
                            }
                            else
                            {
                                Global.左夹爪轴.PMove(Spd, 500, Convert.ToDouble(Motdis_num.Value), 0);
                            }
                            break;
                        case "LClaw_N":    /*********【左夹爪-方向】***********/
                            dgvMotorStatus.FirstDisplayedScrollingRowIndex = 4;
                            if (Jop_Rio.Checked)
                            {
                                Global.左夹爪轴.JOP(0, Spd);
                            }
                            else
                            {
                                Global.左夹爪轴.PMove(Spd, 500, -1 * Convert.ToDouble(Motdis_num.Value), 0);
                            }
                            break;

                        case "RClaw_P":    /*********【右夹爪+方向】***********/
                            dgvMotorStatus.FirstDisplayedScrollingRowIndex = 4;
                            if (Jop_Rio.Checked)
                            {
                                Global.右夹爪轴.JOP(1, Spd);
                            }
                            else
                            {
                                Global.右夹爪轴.PMove(Spd, 500, Convert.ToDouble(Motdis_num.Value), 0);
                            }
                            break;
                        case "RClaw_N":    /*********【右夹爪-方向】***********/
                            dgvMotorStatus.FirstDisplayedScrollingRowIndex = 4;
                            if (Jop_Rio.Checked)
                            {
                                Global.右夹爪轴.JOP(0, Spd);
                            }
                            else
                            {
                                Global.右夹爪轴.PMove(Spd, 500, -1 * Convert.ToDouble(Motdis_num.Value), 0);
                            }
                            break;

                        case "Width_P":    /*********【调宽+方向】***********/
                            dgvMotorStatus.FirstDisplayedScrollingRowIndex = 6;
                            if (Jop_Rio.Checked)
                            {
                                Global.调宽.JOP(1, Spd);
                            }
                            else
                            {
                                Global.调宽.PMove(Spd, 500, Convert.ToDouble(Motdis_num.Value), 0);
                            }
                            break;
                        case "Width_N":    /*********【调宽-方向】***********/
                            dgvMotorStatus.FirstDisplayedScrollingRowIndex = 6;
                            if (Jop_Rio.Checked)
                            {
                                Global.调宽.JOP(0, Spd);
                            }
                            else
                            {
                                Global.调宽.PMove(Spd, 500, -1 * Convert.ToDouble(Motdis_num.Value), 0);
                            }
                            break;
                    }
                    break;
            }
        }
        private void JMouseUp(object sender, MouseEventArgs e)
        {
            if (Jop_Rio.Checked)
            {
                Global.X轴.AxisStop();
                Global.Y轴.AxisStop();
                Global.Z轴.AxisStop();
                Global.R轴.AxisStop();
                Global.左夹爪轴.AxisStop();
                Global.右夹爪轴.AxisStop();
                Global.调宽.AxisStop();
            }
        }
        private void JKeyDown(object sender, KeyEventArgs e)
        {
            if (!Global.SystemInitialOk) { return; }

            if (Global.SystemRun) { return; }
            double KSpd,SlowSpd;
            KSpd = Convert.ToDouble(Fast_tb.Value);
            SlowSpd = Convert.ToDouble(Slow_tb.Value);
            switch (Global.Model)
            {
                case 0:
                    switch (e.KeyData.ToString())
                    {
                        case "Up, Shift"://Y快速
                            Global.Y轴.JOP(1, KSpd);
                            break;
                        case "Up, Control"://Y慢速
                            Global.Y轴.JOP(1, SlowSpd);
                            break;
                        case "Down, Shift"://Y快速
                            Global.Y轴.JOP(0, KSpd);
                            break;
                        case "Down, Control"://Y慢速
                            Global.Y轴.JOP(0, SlowSpd);
                            break;
                        case "Left, Shift"://X快速
                            Global.X轴.JOP(1, KSpd);
                            break;
                        case "Left, Control"://X慢速
                            Global.X轴.JOP(1, SlowSpd);
                            break;
                        case "Right, Shift"://X快速
                            Global.X轴.JOP(0, KSpd);
                            break;
                        case "Right, Control"://X慢速
                            Global.X轴.JOP(0, SlowSpd);
                            break;
                    }
                    break;
                case 1:
                case 2:
                case 3:
                    switch (e.KeyData.ToString())
                    {
                        case "Up, Shift"://Y快速
                            Global.Y轴.JOP(0, KSpd);
                            break;
                        case "Up, Control"://Y慢速
                            Global.Y轴.JOP(0, SlowSpd);
                            break;
                        case "Down, Shift"://Y快速
                            Global.Y轴.JOP(1, KSpd);
                            break;
                        case "Down, Control"://Y慢速
                            BrowLib.Controller  .Motion["Y轴"].JOP(1, SlowSpd);
                            break;
                        case "Left, Shift"://X快速
                            Global.X轴.JOP(1, KSpd);
                            break;
                        case "Left, Control"://X慢速
                            Global.X轴.JOP(1, SlowSpd);
                            break;
                        case "Right, Shift"://X快速
                            Global.X轴.JOP(0, KSpd);
                            break;
                        case "Right, Control"://X慢速
                            Global.X轴.JOP(0, SlowSpd);
                            break;
                    }
                    break;
            }
        }
        private void JKeyUp(object sender, KeyEventArgs e)
        {
            Global.X轴.AxisStop();
            Global.Y轴.AxisStop();
        }

        private void Fast_tb_ValueChanged(object sender, EventArgs e)
        {
            FastSpd_num.Value =decimal.Parse( Fast_tb.Value.ToString());
        }

        private void Slow_tb_ValueChanged(object sender, EventArgs e)
        {
            SlowSpd_num.Value = decimal.Parse(Slow_tb.Value.ToString());
        }

        private void Set_offset_Click(object sender, EventArgs e)
        {

            try
            {
                for (int i = 0; i < AxisParm.AParms.Count; i++)
                {
                    if (AxisParm.AParms[i].AxisName == SelectAxisName)
                    {
                        AxisParm.AParms[i].Homeoffset = int.Parse(this.Offset_Num.Value.ToString());
                    }
                }
                Controller.AxisParm.Write();
                FloatingTip.ShowOk(SelectAxisName + "Axis" + "设置成功".tr());
            }
            catch
            {
                FloatingTip.ShowOk(SelectAxisName + "Axis" + "设置失败".tr());
            }
        }

        private void Ck_Current_CheckedChanged(object sender, EventArgs e)
        {
            if(Global.VisionApp.IsRunProc("Task5"))
            {
              Global.VisionApp.StopRunProc("Task5");
            }
            else
            {
                Global.VisionApp.RunProc("Task5");
            }
            //if (Ck_Current.Checked) {  }
            //else
            //    Global.VisionApp.StopRunProc("Task5");
        }

        private void Search_Btn_Click(object sender, EventArgs e)
        {
            Global.Light.SetRgbLight(Global.Systemdata.S_LED.LED_R, Global.Systemdata.S_LED.LED_G, Global.Systemdata.S_LED.LED_B);
            Global.VisionApp.SetToolData("Task4", "定义变量", 2101, 0, Row_txt.Text, 2);//Roi长
            Global.VisionApp.SetToolData("Task4", "定义变量", 2102, 0, Col_txt.Text, 2);//Roi宽
            Global.VisionApp.SetToolData("Task4", "定义变量", 2110, 0, angle_txt.Text, 2);//Roi角度
            Global.VisionApp.SetToolData("Task4", "定义变量", 2111, 0, Mode_txt.Text, 3);//模板名称
            Global.VisionApp.ExecuteProc("Task4", 0);
            WaitFrm wait=new WaitFrm(Search_Btn);
            wait.Enabled = true;
            Task task = Task.Factory.StartNew(() =>
            {
                do { Thread.Sleep(2); }
                while (!Global.VisionApp.EndProc["Task4"]);
                this.Invoke(new Action(() =>
                {
                    if (Global.VisionApp.RunState("Task4", "执行结果"))
                    {
                        this.Dx = Global.VisionApp.GetDblValue("Task4", "定义变量", 2108, 0);
                        this.Dy = Global.VisionApp.GetDblValue("Task4", "定义变量", 2109, 0);
                        Dx_lab.Text = this.Dx.ToString("F3");
                        DY_lab.Text = this.Dy.ToString("F3");
                    }
                    Global.VisionApp.SetView(ImageView, "Task4", "多轮廓匹配2", "ImageView");
                    wait.Enabled = false;
                }));
            });
        }

        private void Rectify_Btn_Click(object sender, EventArgs e)
        {
            Global.VisionApp.SetView(ImageView, "Task5", "十字线", "ImageView");
            Global.Light.SetRgbLight(Global.Systemdata.P_LED.LED_R, Global.Systemdata.P_LED.LED_G, Global.Systemdata.P_LED.LED_B);

            double X = Global.X轴.GetPrfPos();
            double Y = Global.Y轴.GetPrfPos();
            new HandFlow().SafeMoveXyz(0, X - this.Dx, Y - this.Dy, Global.CamHeight);
            Global.VisionApp.ExecuteProc("Task5", 0);
            this.MoveX = this.MoveX - this.dMoveX;
            this.MoveY = this.MoveY - this.dMoveY;
        }
        private void GoToBtn_Click(object sender, EventArgs e)
        {
            WaitFrm wait = new WaitFrm(GoToBtn);
            wait.Enabled = true;
            double X = Global.X轴.GetPrfPos();
            double Y = Global.Y轴.GetPrfPos();

            Now_X = X; Now_Y = Y;
            double offsetX = 0, offsetY = 0, ZHight = 0, Size = 0;
            Global.GetOffset(Convert.ToDouble(angle_txt.Text), out offsetX, out offsetY, Global.Is_DownCam);
            Size = Global.GetSize(Mode_txt.Text);
            ZHight = Global.GetHight(Mode_txt.Text);
            X = X - offsetX;
            Y = Y - offsetY;
            double R = Convert.ToDouble(angle_txt.Text);
            hFlow.ExecuteManual(() =>
            {
                new HandFlow().SafeMoveXYZR(200, 30, 0, X, Y, ZHight, R, Size, 2, Global.Systemdata.buf_Zspeed);
                Invoke(new Action(() =>
                {
                    wait.Enabled = false;
                }));
            });
        }

        private void refresh_btn_Click(object sender, EventArgs e)
        {
            Row_txt.Text = Global.GetL(Mode_txt.Text).ToString();
            Col_txt.Text = Global.GetW(Mode_txt.Text).ToString();
        }

        private void CamHight_btn_Click(object sender, EventArgs e)
        {
            if (!Global.SystemInitialOk)
            {
                APP.Tip.ShowTip(0, "警告", "设备未归零,请先归零再执行操作".tr(), "确定");
                return;
            }
            if (hFlow.IsManualRun()) { return; }
            WaitFrm wait = new WaitFrm(CamHight_btn);
            wait.Enabled = true;
            hFlow.ExecuteManual(() =>
            {
                Global.Z轴.PMove(20, 2000, Global.CamHeight, 1);
                do
                {
                    if (Global.Z轴.ALM()) { break; }
                    if (Global.Z轴.Runing()) { break; }
                    Thread.Sleep(5);
                } while (true);
                Invoke(new Action(() =>
                {
                    wait.Enabled = false;
                }));
            });
        }

        private void SetBtn_Click(object sender, EventArgs e)
        {
            Global.VisionApp.SetToolData("Task5", "十字线", 102, 0, SetSize.Value.ToString(), 1);
            Global.VisionApp.ExecuteProc("Task5", 0);
        }

        private void CcdHight_Btn_Click(object sender, EventArgs e)
        {
            WaitFrm wait = new WaitFrm(CcdHight_Btn);
            wait.Enabled = true;
            hFlow.ExecuteManual(() =>
            {
                new HandFlow().SafeMoveXYZR(200, 20, 0, Now_X, Now_Y, Global.CamHeight, 0);
                Invoke(new Action(() =>
                {
                    wait.Enabled = false;
                }));
            });
        }
    }
}
