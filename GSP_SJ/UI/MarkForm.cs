using BrowApp;
using BrowApp.Language;
using BrowApp.MessageTip;
using BrowLib.Static;
using CKVisionAppNet;
using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSP.UI
{
    public partial class MarkForm : Form
    {
        private IntPtr M_View = IntPtr.Zero;

        CKVisionAppApi.ViewDrawCallBack  drawCallBack;// 视图显示自定义绘制图形回调

        CKVisionAppApi.ViewMessageCallBack msgCallBack;// 视图消息回调

        private double MoveX;
        private double MoveY;

        private double dMoveX;
        private double dMoveY;

        public bool IsVisible { get; set; }

        public int Num { get; set; } = 1;
        public double Dx { get; set; }
        public double Dy { get; set; }

        public int Mode { get; set; }

        public string Code
        {
            set
            {
              Code_txt.Text = value;
            }
        }
        public double Bmark1_X
        {
            set
            {
                BMPos_X1.Value = decimal.Parse(value.ToString());
               
            }
        }
        public double Bmark1_Y
        {
            set
            {
                BMPos_Y1.Value = decimal.Parse(value.ToString());
                
            }
        }
        public double Bmark2_X
        {
            set
            {
                BMPos_X2.Value = decimal.Parse(value.ToString());
                
            }
        }
        public double Bmark2_Y
        {
            set
            {
                BMPos_Y2.Value = decimal.Parse(value.ToString());

            }
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
        public void ViewMsgCallBack(uint msg, int wParam, int lParam, IntPtr pUserParam)
        {
            switch (msg)
            {
                case (int)AppVIEW_WM.MOUSEMOVE: //鼠标移动消息
                    {
                        double dx = (double)(lParam & 0xffff);
                        double dy = (double)(((lParam>> 16) & 0xffff));

                        CKVisionAppApi.ViewToImage(M_View, ref dx, ref dy); // 视图窗口坐标转换到图像上的坐标
                        //uiSymbolLabel3.Text = String.Format("图像 X:{0:F1}", dx, dy) + String.Format(",Y {0:F1}", dy); ;
                    }
                    break;
                case (int)AppVIEW_WM.LBUTTONDOWN:   //鼠标左键点击消息
                    {
                        double dx = (double)(lParam& 0xffff);
                        double dy = (double)(((lParam>> 16) & 0xffff));
                        this.MoveX = (int)dx;
                        this.MoveY = (int)dy;

                        double iwidth = Global.ImageWidth, iHight = Global.ImageHight;
                        //SysPara.VisionApp.GetImageWh(M_ImageView, out iwidth, out iHight);

                        double IcentX = iwidth / 2;
                        double IcentY = iHight / 2;

                        CKVisionAppApi.ImageToView(M_View, ref IcentX, ref IcentY);//视图窗口坐标转换到图像上的坐标

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

                        Global.Light.SetRgbLight(Global.Systemdata.M_LED.LED_R, Global.Systemdata.M_LED.LED_G, Global.Systemdata.M_LED.LED_B);
                        new HandFlow().SafeMoveXyz(0, Global.X轴.GetPrfPos() - this.Dx,
                        Global.Y轴.GetPrfPos() - this.Dy, Global.CamHeight);
                        Global.VisionApp.ExecuteProc("Task5", 0);
                        this.MoveX = this.MoveX - this.dMoveX;
                        this.MoveY = this.MoveY - this.dMoveY;

                    }
                    break;
                case (int)AppVIEW_WM.RBUTTONDOWN:
                    {
                        ;///  BrowApp.APP.Tip.ShowTip(0, "警告".tr(), "ViewMsgCallBack RBUTTONDOWN");

                    }
                    break;
                case (int)AppVIEW_WM.MOUSEWHEEL:
                    {
                        ///   BrowApp.APP.Tip.ShowTip(0, "警告".tr(), "ViewMsgCallBack MOUSEWHEEL");

                    }
                    break;
            }
        }

        public void RefProc()
        {
            Global.VisionApp.ExecuteProc("Task5", 0);
        }
        public MarkForm()
        {
            InitializeComponent();

            this.MarkHeader.MouseDown += TopBar_MouseDown;
            this.MarkHeader.MouseMove += TopBar_MouseMove;
            Global.VisionApp.ProcEndEvent += new EventHandler(RefImage);
            Global.VisionApp.ToolClick += VisionApp_ToolClick;
        }

        private void VisionApp_ToolClick(object sender, EventArgs e)
        {
            if (sender.ToString() == "MARK1")
                Global.VisionApp.SetToolData("Task3", "定义变量", 2110, 0, "1", 1);
            else
                Global.VisionApp.SetToolData("Task3", "定义变量", 2110, 0, "2", 1);

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

        /// <summary>
        /// 激活窗口
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <param name="nCmdShow">0:关闭窗口 1:正常大小显示窗口  2:最小化窗口 3:最大化窗口</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);
        public void FomActivated()
        {
            SystemEx.ShowWindow(this.Handle, 1);
        }
        private void MarkForm_Load(object sender, EventArgs e)
        {
            M_View = Global.VisionApp.CreateView(this.Hand_page.Handle, this.Hand_page.Width, this.Hand_page.Height, 5);
            Global.VisionApp.SetView(M_View, "Task5", "十字线", "M_View");

            //视图自定义绘图回调
            drawCallBack = new CKVisionAppApi.ViewDrawCallBack(ViewDrawCallBack);
            CKVisionAppApi.SetDrawCallback(M_View, drawCallBack, this.Handle);
            msgCallBack = new CKVisionAppApi.ViewMessageCallBack(ViewMsgCallBack);
            CKVisionAppApi.SetMsgCallback(M_View, msgCallBack, this.Handle);
            Global.VisionApp.ShowQuick(this.Mark_page.Handle, "Task3", Mark_page.Width, Mark_page.Height);
            Global.VisionApp.ExecuteProc("Task5", 0);
            ObjtoFrm();
            Tab_Ctr.SelectedIndex = 0;
            BrowApp.Language.Language.Instance.UpdateLanguage(this, null);
        }
        private void ObjtoFrm()
        {
            this.MPos_X1.Value = decimal.Parse(Global.Parm.Mak1Pos.Xpos.ToString());
            this.MPos_Y1.Value = decimal.Parse(Global.Parm.Mak1Pos.Ypos.ToString());

            this.MPos_X2.Value = decimal.Parse(Global.Parm.Mak2Pos.Xpos.ToString());
            this.MPos_Y2.Value = decimal.Parse(Global.Parm.Mak2Pos.Ypos.ToString());

            this.BMPos_X1.Value = decimal.Parse(Global.Parm.BMak1Pos.Xpos.ToString());
            this.BMPos_Y1.Value = decimal.Parse(Global.Parm.BMak1Pos.Ypos.ToString());

            this.BMPos_X2.Value = decimal.Parse(Global.Parm.BMak2Pos.Xpos.ToString());
            this.BMPos_Y2.Value = decimal.Parse(Global.Parm.BMak2Pos.Ypos.ToString());
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
        public void RefMarkfrm()
        {
            if (Num > 1)
            {
                Next_btn.Visible = false;
                Save_btn.Visible = true;

                TeachPos2.Visible = true;
                TeachPos1.Visible = false;
                Global.VisionApp.SetToolData("Task3", "定义变量", 2110, 0, "2", 1);
            }
            else
            {
                Global.VisionApp.SetToolData("Task3", "定义变量", 2110, 0, "1", 1);
                Next_btn.Visible = true;
                Save_btn.Visible = false;

                TeachPos2.Visible = false;
                TeachPos1.Visible = true;
            }
            if (this.Mode == 0)
            {
                TabControl.SelectedIndex = 0;
                IsHand_Rio.Checked = true;
            }
            else if (this.Mode == 1)
            {
                IsMark_Rio.Checked = true;
                TabControl.SelectedIndex = 1;
            }
            //TabControl.Visible = false;
        }

        private void TeachPos1_Click(object sender, EventArgs e)
        {
            MPos_X1.Value = decimal.Parse(Global.X轴.GetPrfPos().ToString("F3"));
            MPos_Y1.Value = decimal.Parse(Global.Y轴.GetPrfPos().ToString("F3"));
        }

        private void TeachPos2_Click(object sender, EventArgs e)
        {
            MPos_X2.Value = decimal.Parse(Global.X轴.GetPrfPos().ToString("F3"));
            MPos_Y2.Value = decimal.Parse(Global.Y轴.GetPrfPos().ToString("F3"));
        }

        private void Next_btn_Click(object sender, EventArgs e)
        {
            Global.TcpClass.Send("M:Mark1_OK");
            this.Visible = false;
        }

        private void Save_btn_Click(object sender, EventArgs e)
        {
            Global.VisionApp.ProcEndEvent -= new EventHandler(RefImage);
            //Global.IsMove = false;
            if (BrowApp.APP.Tip.ShowTip(0, "提示".tr(), "确定要保存参数吗？".tr(),"确定".tr(),"取消".tr())!=1) { return; }
            try
            {
                Global.Parm.Mak1Pos.Xpos = Convert.ToDouble(MPos_X1.Value);
                Global.Parm.Mak1Pos.Ypos = Convert.ToDouble(MPos_Y1.Value);
                Global.Parm.Mak2Pos.Xpos = Convert.ToDouble(MPos_X2.Value);
                Global.Parm.Mak2Pos.Ypos = Convert.ToDouble(MPos_Y2.Value);

                Global.Parm.BMak1Pos.Xpos = Convert.ToDouble(BMPos_X1.Value);
                Global.Parm.BMak1Pos.Ypos = Convert.ToDouble(BMPos_Y1.Value);
                Global.Parm.BMak2Pos.Xpos = Convert.ToDouble(BMPos_X2.Value);
                Global.Parm.BMak2Pos.Ypos = Convert.ToDouble(BMPos_Y2.Value);
                Global.Parm.SetCode = Global.RecipePatn;
                Global.Parm.Save();
                FloatingTip.ShowOk("参数保存成功".tr());
                if(IsMark_Rio.Checked)
                {
                    Global.VisionApp.SetCodePath = Global.RecipePatn;
                    if (Global.VisionApp.Saveobj("Mark.proc", "Task3"))
                    {
                        FloatingTip.ShowOk("配方:".tr() + Global.RecipePatn + "Mark保存成功".tr());
                        Num = 1;
                        CKVisionAppApi.SetViewTool(M_View, IntPtr.Zero);
                        CKVisionAppApi.FreeView(M_View);
                        M_View=IntPtr.Zero;
                        Global.TcpClass.Send("M:Mark2_OK");
                        this.Dispose();
                        this.Close();
                    }
                    else
                    {
                        FloatingTip.ShowError("配方:" + Global.RecipePatn + "Mark保存失败".tr());
                    }
                }
                else
                {
                    Num = 1;
                    CKVisionAppApi.SetViewTool(M_View, IntPtr.Zero);
                    CKVisionAppApi.FreeView(M_View);
                    M_View = IntPtr.Zero;
                    Global.TcpClass.Send("M:Mark2_OK");
                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                FloatingTip.ShowError("参数保存失败".tr() + ex.ToString());
            }
        }

        private void Close_btn_Click(object sender, EventArgs e)
        {
            Global.VisionApp.ProcEndEvent -= new EventHandler(RefImage);
            Num = 1;
            //Global.IsMove = false;
            CKVisionAppApi.SetViewTool(M_View, IntPtr.Zero);
            CKVisionAppApi.FreeView(M_View);
            M_View = IntPtr.Zero;
            this.Dispose();
            this.Close();
        }

        private void IsHand_Rio_CheckedChanged(object sender, EventArgs e)
        {
            TabControl.SelectedIndex= 0;
        }

        private void IsMark_Rio_CheckedChanged(object sender, EventArgs e)
        {
            TabControl.SelectedIndex = 1;
        }

        private void JMouseDown(object sender, MouseEventArgs e)
        {
            if (!Global.SystemInitialOk)
            {
                APP.Tip.ShowTip(0, "警告".tr(), "设备未归零,请先归零再执行操作".tr(), "确定".tr());
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
                            if (Jop_Rio.Checked)
                            {
                                Global.Y轴.JOP(1, Spd);
                            }
                            else
                            {
                                Global.Y轴.PMove(Spd, 3000, Convert.ToDouble(Motdis_num.Value), 0);
                            }
                            break;
                    }
                    break;
                case 1:
                case 2: //在线机
                case 3: //860P
                    switch (KeyName)
                    {
                        case "MX_N":       /*********【X轴-方向】***********/
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
                            if (Jop_Rio.Checked)
                            {
                                Global.Y轴.JOP(0, Spd);
                            }
                            else
                            {
                                Global.Y轴.PMove(Spd, 3000, -1*Convert.ToDouble(Motdis_num.Value), 0);
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
            }
        }
    }
}
