using BrowApp;
using BrowApp.IO;
using BrowApp.Language;
using BrowApp.MessageTip;
using BrowLib;
using BrowLib.Static;
using CKVisionAppNet;
using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace GSP.UI
{
    public partial class UI_Program : UserControl
    {
        private FlowControl Flow = new FlowControl();
        private Algorithm algorithm = new Algorithm();
        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        private int tdelay = 0;

        /// <summary>
        /// 拼图显示视图
        /// </summary>
        private IntPtr m_imageView;
        private double MoveX;
        private double MoveY;
        private double dMoveX;
        private double dMoveY;
        private double Dx, Dy;

        CKVisionAppApi.ViewDrawCallBack drawCallBack;// 视图显示自定义绘制图形回调

        CKVisionAppApi.ViewMessageCallBack msgCallBack;// 视图消息回调

        public void ViewDrawCallBack(IntPtr hDC, int x, int y, double scale, IntPtr pUserParam)
        {
            if (Is_follow.Checked)
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
        }
        public void ViewMsgCallBack(uint msg, int wParam, int lParam, IntPtr pUserParam)
        {
            switch (msg)
            {
                case (int)AppVIEW_WM.MOUSEMOVE: //鼠标移动消息
                    {
                        double dx = (double)(lParam & 0xffff);
                        double dy = (double)(((lParam >> 16) & 0xffff));
                        CKVisionAppApi.ViewToImage(m_imageView, ref dx, ref dy); // 视图窗口坐标转换到图像上的坐标

                        //uiSymbolLabel3.Text = String.Format("图像 X:{0:F1}", dx, dy) + String.Format(",Y {0:F1}", dy); ;
                    }
                    break;
                case (int)AppVIEW_WM.LBUTTONDOWN:   //鼠标左键点击消息
                    {
                        tdelay = 0;
                        double dx = (double)(lParam & 0xffff);
                        double dy = (double)(((lParam >> 16) & 0xffff));

                        this.MoveX = (int)dx;
                        this.MoveY = (int)dy;

                        double iwidth = 0, iHight = 0, IcentX = 0, IcentY = 0;

                        Global.VisionApp.GetImageWh(m_imageView, out iwidth, out iHight);

                        IcentX = iwidth / 2;
                        IcentY = iHight / 2;

                        CKVisionAppApi.ImageToView(m_imageView, ref IcentX, ref IcentY);// 图像上的坐标转换到视图窗口坐标

                        this.dMoveX = dx - IcentX;
                        this.dMoveY = dy - IcentY;

                        CKVisionAppApi.ViewToImage(m_imageView, ref dx, ref dy);// 视图窗口坐标转换到图像上的坐标
                        VisionApp.CalibType Calib = new VisionApp.CalibType();
                        Calib = Global.VisionApp.GetCalib(VisionGlobal.UpCailb_Obj);
                        double Cen_X = 0, Cen_Y = 0, X, Y, width, hight;

                        algorithm.GetStartCenter_Pix(Global.Parm.PcbLong, Global.Parm.PcbHight, Global.Systemdata.CameRaview.Row,
                        Global.Systemdata.CameRaview.Col, iwidth, iHight, out width, out hight);
                        switch (Global.Model)
                        {
                            case 0:
                                Global.VisionApp.PixToPos(Calib, (iwidth - width / 2), (hight / 2), out Cen_X, out Cen_Y);
                                break;
                            case 1:
                                Global.VisionApp.PixToPos(Calib, (iwidth - width / 2), (iHight - hight / 2), out Cen_X, out Cen_Y);
                                break;
                            case 2:
                            case 3:
                                Global.VisionApp.PixToPos(Calib, (width / 2), (hight / 2), out Cen_X, out Cen_Y);//860
                                break;
                        }
                        Global.VisionApp.PixToPos(Calib, dx, dy, out X, out Y);

                        this.Dx = (X - Cen_X) * Global.Ratio;
                        this.Dy = (Y - Cen_Y) * Global.Ratio;

                        if (!Global.SystemRun && Is_follow.Checked)
                        {
                            new HandFlow().SafeMoveXyz(0, Global.Systemdata.PatlePos.Xpos - this.Dx,
                             Global.Systemdata.PatlePos.Ypos - this.Dy, Global.CamHeight);
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
        public UI_Program()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            Global.RefFormHandler += new EventHandler(LoadBom_Click);
            Global.VisionApp.ProcEndEvent += new EventHandler(IsRun);
            Global.GlobBomRefEvent += new EventHandler(RefBomdata);
        }
        void IsRun(object sender, EventArgs e)
        {
            try
            {
              Global.VisionApp.RedrawView(m_imageView);
            }
            catch { }
        }
        ///  避免窗体控件改变大小界面瞬间的叠影
        /// </summary>
        /// <summary>        
        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        CreateParams cp = base.CreateParams;
        //        cp.ExStyle |= 0x02000000; // Turn on WS_EX_COMPOSITED
        //        return cp;
        //    }
        //}
        private void UI_Program_Load(object sender, EventArgs e)
        {
            ObjtoFrom();
            m_imageView = Global.VisionApp.CreateView(this.P_Viewpic.Handle, this.P_Viewpic.Width, this.P_Viewpic.Height, 5);
            Global.VisionApp.SetView( m_imageView,  "Task2", "图像合并" , "m_imageView");
            Global.VisionApp.SetViewTool();
            //视图自定义绘图回调
            drawCallBack = new CKVisionAppApi.ViewDrawCallBack(ViewDrawCallBack);
            CKVisionAppApi.SetDrawCallback(m_imageView, drawCallBack, this.Handle);

            msgCallBack = new CKVisionAppApi.ViewMessageCallBack(ViewMsgCallBack);
            CKVisionAppApi.SetMsgCallback(m_imageView, msgCallBack, this.Handle);

            BrowApp.Language.Language.Instance.UpdateLanguage(this, null);

            timer.Tick += Timer_Tick;
            timer.Interval = 200;
        }
        private void ObjtoFrom()
        {
            try
            {
                this.length_num.Value = decimal.Parse(Global.Parm.PcbLong.ToString());
                this.Width_num.Value = decimal.Parse(Global.Parm.PcbHight.ToString());
                this.Hoffset_num.Value = decimal.Parse(Global.Parm.Hoffset.ToString());

                this.PbXoffset_num.Value = decimal.Parse(Global.Parm.PbXoffset.ToString());
                this.PbYoffset_num.Value = decimal.Parse(Global.Parm.PbYoffset.ToString());
                this.OffsetX_num.Value = decimal.Parse(Global.Parm.Offset_X.ToString());
                this.OffsetY_num.Value = decimal.Parse(Global.Parm.Offset_Y.ToString());


                this.Vrow_num.Value = decimal.Parse(Global.Systemdata.CameRaview.Row.ToString());
                this.Vcolumn_num.Value = decimal.Parse(Global.Systemdata.CameRaview.Col.ToString());

                this.Vrow_num1.Value = decimal.Parse(Global.Systemdata.sCameRaview.Row.ToString());
                this.Vcolumn_num1.Value = decimal.Parse(Global.Systemdata.sCameRaview.Col.ToString());

                this.Cutrow_num.Value = decimal.Parse(Global.Systemdata.Cutsize.Row.ToString());
                this.Cutcolumn_num.Value = decimal.Parse(Global.Systemdata.Cutsize.Col.ToString());

                this.Mask1Xpos.Value = decimal.Parse(Global.Parm.Mak1Pos.Xpos.ToString());
                this.Mask1Ypos.Value = decimal.Parse(Global.Parm.Mak1Pos.Ypos.ToString());


                this.Mask2Xpos.Value = decimal.Parse(Global.Parm.Mak2Pos.Xpos.ToString());
                this.Mask2Ypos.Value = decimal.Parse(Global.Parm.Mak2Pos.Ypos.ToString());



                this.BMask1_Xnum.Value = decimal.Parse(Global.Parm.BMak1Pos.Xpos.ToString());
                this.BMask1_Ynum.Value = decimal.Parse(Global.Parm.BMak1Pos.Ypos.ToString());

                this.BMask2_Xnum.Value = decimal.Parse(Global.Parm.BMak2Pos.Xpos.ToString());
                this.BMask2_Ynum.Value = decimal.Parse(Global.Parm.BMak2Pos.Ypos.ToString());


                this.PatelXpos.Value = decimal.Parse(Global.Systemdata.PatlePos.Xpos.ToString());
                this.PatelYpos.Value = decimal.Parse(Global.Systemdata.PatlePos.Ypos.ToString());
            }
            catch { }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (tdelay >= 100)
            {
                if (Is_follow.Checked)
                    Is_follow.Checked = false;
                else
                    Is_follow.Checked = true;
                timer.Stop();
                tdelay = 0;
                FloatingTip.ShowWarning("长时间无操作取消跟随".tr());
            }
            tdelay++;
        }
        #region 拼图设置
        private void PTeachPos_Click(object sender, EventArgs e)
        {
            PatelXpos.Value = decimal.Parse(Global.X轴.GetPrfPos().ToString());
            PatelYpos.Value = decimal.Parse(Global.Y轴.GetPrfPos().ToString());
        }

        private void Ck_debug_CheckedChanged(object sender, EventArgs e)
        {
            if (Ck_debug.Checked)
                Global.VisionApp.SetToolData("Task2", "定义变量", 2110, 0, "true", 0);
            else
                Global.VisionApp.SetToolData("Task2", "定义变量", 2110, 0, "false", 0);
        }

        private void P_Viewpic_Resize(object sender, EventArgs e)
        {
            Global.VisionApp.MoveView(m_imageView, this.P_Viewpic.Width, this.P_Viewpic.Height);
        }
        private void GoTo_btn_Click(object sender, EventArgs e)
        {
            if (!Global.SystemInitialOk)
            {
                APP.Tip.ShowTip(0, "警告", "设备未归零,请先归零再执行操作!!!".tr(), "确定".tr());
                return;
            }
            if (Flow.IsManualRun()) { return; }
            WaitFrm waitFrm = new WaitFrm(GoTo_btn);
            waitFrm.Enabled = true;
            Flow.ExecuteManual(() =>
            {
                new HandFlow().SafeMoveXyz(0, (double)PatelXpos.Value, (double)PatelYpos.Value, Global.CamHeight);

                Invoke(new Action(() =>
                {
                    waitFrm.Enabled = false;
                }));
            });
        }

        private void MergerStart_btn_Click(object sender, EventArgs e)
        {
            double[] X, Y;
            double Rows, Clos;
            bool rtn;
            Rows = Convert.ToDouble(row_Num.Value);
            Clos = Convert.ToDouble(column_num.Value);

            Global.Parm.PcbLong = Clos * Global.Systemdata.CameRaview.Row;
            Global.Parm.PcbHight = Rows * Global.Systemdata.CameRaview.Col;
            Global.StopFlag = false;

            if (ck_light2.Checked)
            {
               Global.Light.SetRgbLight(Global.Systemdata.P_LED2.LED_R, Global.Systemdata.P_LED2.LED_G, Global.Systemdata.P_LED2.LED_B);
            }
            else
            {
               Global.Light.SetRgbLight(Global.Systemdata.P_LED.LED_R, Global.Systemdata.P_LED.LED_G, Global.Systemdata.P_LED.LED_B);
            }

            Global.VisionApp.StopRunProc("Task5");
            Global.VisionApp.SetView(m_imageView, "Task2", "图像合并", "m_imageView");
            Global.VisionApp.SetToolData("Task2", "定义变量", 2107, 0, Global.Systemdata.PateFile, 3);
            Global.VisionApp.SetToolData("Task2", "定义变量", 2100, 0, "0", 1);
            Global.VisionApp.SetToolData("Task2", "定义变量", 2101, 0, Rows.ToString(), 1);
            Global.VisionApp.SetToolData("Task2", "定义变量", 2102, 0, Clos.ToString(), 1);
            Global.VisionApp.SetToolData("Task2", "定义变量", 2105, 0, Global.Systemdata.Cutsize.Row.ToString(), 1);
            Global.VisionApp.SetToolData("Task2", "定义变量", 2106, 0, Global.Systemdata.Cutsize.Col.ToString(), 1);
            string[] st = algorithm.Photolocation2(Global.Model, Global.Systemdata.PatlePos.Xpos, Global.Systemdata.PatlePos.Ypos,
            Global.Systemdata.CameRaview.Row, Global.Systemdata.CameRaview.Col, out X, out Y, Rows, Clos);

            WaitFrm waitFrm = new WaitFrm(MergerStart_btn);
            waitFrm.Enabled = true;
            Thread Start = new Thread(() =>
            {
                for (int i = 1; i < X.Length; i++)
                {
                    if (Global.StopFlag) { break; }
                    BrowLib.Controller.MotionAPI.LinXyMoveA(800, 8000, X[i], Y[i], 1);
                    do
                    {
                        rtn = BrowLib.Controller.MotionAPI.LinXYRuningA();
                        if (Global.StopFlag) { break; }
                        Thread.Sleep(2);
                    }
                    while (!rtn);
                    Global.VisionApp.ExecuteProc("Task2", 0);
                    do
                    {
                        if (Global.StopFlag) { break; }
                        Thread.Sleep(2);
                    }
                    while (!Global.VisionApp.EndProc["Task2"]);
                    if (!Global.VisionApp.RunState("Task2", "数据判断")) { FloatingTip.ShowError("拼图失败？？？".tr()); break; }
                }
                this.Invoke(new Action(() =>
                {
                    waitFrm.Enabled = false;

                }));
            });
            Start.Start();
        }

        private void MergerStart_btn2_Click(object sender, EventArgs e)
        {
            double[] X, Y;
            double Rows, Clos;
            bool rtn;
            Global.StopFlag = false;
            switch (Global.Model)
            {
                case 0:
                    Global.Parm.PcbLong = Convert.ToDouble(length_num.Value);
                    Global.Parm.PcbHight = Convert.ToDouble(Width_num.Value);
                    break;
                case 1:
                case 2:
                case 3:
                    Global.Parm.PcbLong = Convert.ToDouble(Width_num.Value);
                    Global.Parm.PcbHight = Convert.ToDouble(length_num.Value);
                    break;
            }
            
            if (ck_light2.Checked)
            {
              Global.Light.SetRgbLight(Global.Systemdata.P_LED2.LED_R, Global.Systemdata.P_LED2.LED_G, Global.Systemdata.P_LED2.LED_B);
            }
            else
            {
               Global.Light.SetRgbLight(Global.Systemdata.P_LED.LED_R, Global.Systemdata.P_LED.LED_G, Global.Systemdata.P_LED.LED_B);
            }
            Global.VisionApp.StopRunProc("Task5");
            string[] st = algorithm.Photolocation(Global.Model, Global.Systemdata.PatlePos.Xpos, Global.Systemdata.PatlePos.Ypos,
            Global.Parm.PcbLong, Global.Parm.PcbHight, Global.Systemdata.CameRaview.Row,
            Global.Systemdata.CameRaview.Col, out X, out Y, out Rows, out Clos);
            Global.VisionApp.SetToolData("Task2", "定义变量", 2107, 0, Global.Systemdata.PateFile, 3);
            Global.VisionApp.SetToolData("Task2", "定义变量", 2100, 0, "0", 1);
            Global.VisionApp.SetToolData("Task2", "定义变量", 2101, 0, Rows.ToString(), 1);
            Global.VisionApp.SetToolData("Task2", "定义变量", 2102, 0, Clos.ToString(), 1);
            Global.VisionApp.SetToolData("Task2", "定义变量", 2105, 0, Global.Systemdata.Cutsize.Row.ToString(), 1);
            Global.VisionApp.SetToolData("Task2", "定义变量", 2106, 0, Global.Systemdata.Cutsize.Col.ToString(), 1);
            WaitFrm waitFrm = new WaitFrm(MergerStart_btn2);
            waitFrm.Enabled = true;
            Thread Start = new Thread(() =>
            {
                for (int i = 1; i < X.Length; i++)
                {
                    if (Global.StopFlag) { break; }
                    BrowLib.Controller.MotionAPI.LinXyMoveA(800, 8000, X[i], Y[i], 1);
                    Thread.Sleep(10);
                    do
                    {
                        rtn = BrowLib.Controller.MotionAPI.LinXYRuningA();
                        if (Global.StopFlag) { break; }
                        Thread.Sleep(50);
                    }
                    while (!rtn);
                    bool re= Global.VisionApp.ExecuteProc("Task2", 0);
                    Thread.Sleep(10);
                    do
                    {
                        if (Global.StopFlag) { break; }
                        Thread.Sleep(2);
                    }
                    while (!Global.VisionApp.EndProc["Task2"]);
                    if (!Global.VisionApp.RunState("Task2", "数据判断")) { FloatingTip.ShowError("拼图失败".tr()); break; }
                }
                this.Invoke(new Action(() =>
                {
                    waitFrm.Enabled = false;

                }));
            });
            Start.Start();
        }
        private void SetWidth_btn_Click(object sender, EventArgs e)
        {
            double with = Convert.ToDouble(Width_num.Value);

            Global.调宽.PMove(50, 2000, with - 52, 1);
        }
        private void P_Save_Click(object sender, EventArgs e)
        {
            if (KryptonMessageBox.Show("确定要保存参数吗？".tr()) != DialogResult.OK) { return; }
            try
            {
                Global.Systemdata.PatlePos.Xpos = Convert.ToDouble(PatelXpos.Value);
                Global.Systemdata.PatlePos.Ypos = Convert.ToDouble(PatelYpos.Value);

                Global.Systemdata.CameRaview.Row = Convert.ToDouble(Vrow_num.Value);
                Global.Systemdata.CameRaview.Col = Convert.ToDouble(Vcolumn_num.Value);

                Global.Systemdata.sCameRaview.Row = Convert.ToDouble(Vrow_num1.Value);
                Global.Systemdata.sCameRaview.Col = Convert.ToDouble(Vcolumn_num1.Value);


                Global.Systemdata.Cutsize.Row = Convert.ToInt32(Cutrow_num.Value);
                Global.Systemdata.Cutsize.Col = Convert.ToInt32(Cutcolumn_num.Value);

                Global.Systemdata.Save();
                FloatingTip.ShowOk("参数保存成功".tr());
            }
            catch (Exception ex)
            {
              FloatingTip.ShowError("参数保存失败".tr() + ex.ToString());
            }
        }
        private void P_Save2_Click(object sender, EventArgs e)
        {
            if (KryptonMessageBox.Show("确定要保存参数吗？".tr()) != DialogResult.OK) { return; }
            try
            {
                Global.Systemdata.PatlePos.Xpos = Convert.ToDouble(PatelXpos.Value);
                Global.Systemdata.PatlePos.Ypos = Convert.ToDouble(PatelYpos.Value);

                Global.Systemdata.CameRaview.Row = Convert.ToDouble(Vrow_num.Value);
                Global.Systemdata.CameRaview.Col = Convert.ToDouble(Vcolumn_num.Value);

                Global.Systemdata.sCameRaview.Row = Convert.ToDouble(Vrow_num1.Value);
                Global.Systemdata.sCameRaview.Col = Convert.ToDouble(Vcolumn_num1.Value);

                Global.Systemdata.Cutsize.Row = Convert.ToInt32(Cutrow_num.Value);
                Global.Systemdata.Cutsize.Col = Convert.ToInt32(Cutcolumn_num.Value);

                Global.Systemdata.Save();

                FloatingTip.ShowOk("参数保存成功".tr());
            }
            catch (Exception ex)
            {
                FloatingTip.ShowError("参数保存失败".tr() + ex.ToString());
            }
        }
        #endregion

        #region BOM导入
        private void bomSave_Click(object sender, EventArgs e)
        {
            if (KryptonMessageBox.Show("确定要保存参数吗？".tr()) != DialogResult.OK) { return; }
            try
            {
                Global.Parm.BMak1Pos.Xpos = Convert.ToDouble(BMask1_Xnum.Value);
                Global.Parm.BMak1Pos.Ypos = Convert.ToDouble(BMask1_Ynum.Value);
                Global.Parm.BMak2Pos.Xpos = Convert.ToDouble(BMask2_Xnum.Value);
                Global.Parm.BMak2Pos.Ypos = Convert.ToDouble(BMask2_Ynum.Value);


                VisionGlobal.bMak1_X = Global.Parm.BMak1Pos.Xpos;
                VisionGlobal.bMak1_Y = Global.Parm.BMak1Pos.Ypos;

                VisionGlobal.bMak2_X = Global.Parm.BMak2Pos.Xpos;
                VisionGlobal.bMak2_Y = Global.Parm.BMak2Pos.Ypos;


                Global.Parm.SetCode = Global.RecipePatn;
                Global.Parm.Save();

                FloatingTip.ShowOk("参数保存成功".tr());
            }
            catch (Exception ex)
            {
                FloatingTip.ShowError("参数保存失败".tr() + ex.ToString());
            }
        }
        void RefBomdata(object sender, EventArgs e)
        {
            try
            {
                if (Global.BomData != null)
                {
                    this.BomDgv.DataSource = null;
                    this.BomDgv.DataSource = Global.BomData;
                    this.BomDgv.Update();
                }
            }
            catch { }
        }

        public void  refbom()
        {
            string FBCCode, XYCode, BoardSide, ProductCode, ProductName;
            double X, Y;
            Global.ReadFAIConfig(out FBCCode, out XYCode, out BoardSide, out ProductCode, out ProductName);
            //XYCodeTxt.Text = FBCCode;
            Global.FBCCode = FBCCode;
            Global.RecipePatn = XYCode + BoardSide;
            Global.Parm.SetCode = Global.RecipePatn;
            Global.VisionApp.SetCodePath = Global.RecipePatn;
            bool Btn1 = Global.Parm.Read(ref Global.Parm);
            bool Btn2 = Global.VisionApp.ReadObj("Mark.proc", "Task3");

            Global.VisionApp.RefView("Mark1_imageView");
            Global.VisionApp.RefView("Mark2_imageView");

            Global.VisionApp.SetToolData("Task2", "定义变量", 2111, 0, Global.Ratio.ToString(), 1);
            Global.VisionApp.SetToolData("Task2", "定义变量", 2112, 0, Global.ARatio.ToString(), 1);
            //SysPara.Hoffset = SysPara.ParmSystem.ParmData.Hoffset;
            if (Btn1 && Btn2) { Global.IsRecipe = true; }
            else { Global.IsRecipe = false; }

            VisionGlobal.bMak1_X = Global.Parm.BMak1Pos.Xpos;
            VisionGlobal.bMak1_Y = Global.Parm.BMak1Pos.Ypos;

            VisionGlobal.bMak2_X = Global.Parm.BMak2Pos.Xpos;
            VisionGlobal.bMak2_Y = Global.Parm.BMak2Pos.Ypos;

            //ObjtoFrom();
            try
            {
                //Global.BomData = new JD.Fai.Data.FlyingProbe().GetDataTable(FBCCode);
                algorithm.CalculateTransferXY(Global.BomData, out X, out Y);
                VisionGlobal.TranslationX = X;
                VisionGlobal.TranslationY = Y;
                //this.BomDgv.DataSource = Global.BomData;
                FloatingTip.ShowOk("导入BOM坐标成功".tr());
            }
            catch
            {
                FloatingTip.ShowError("导入BOM坐标失败".tr());
            }
        }
        private void LoadBom_Click(object sender, EventArgs e)
        {
            string FBCCode, XYCode, BoardSide, ProductCode, ProductName;
            double X, Y;
            Global.ReadFAIConfig(out FBCCode, out XYCode, out BoardSide, out ProductCode, out ProductName);
            XYCodeTxt.Text = FBCCode;
            Global.FBCCode = FBCCode;
            Global.RecipePatn = XYCode + BoardSide;
            Global.Parm.SetCode = Global.RecipePatn;
            Global.VisionApp.SetCodePath = Global.RecipePatn;
            bool Btn1 = Global.Parm.Read(ref Global.Parm);
            bool Btn2 = Global.VisionApp.ReadObj("Mark.proc", "Task3");

            Global.VisionApp.RefView("Mark1_imageView");
            Global.VisionApp.RefView("Mark2_imageView");

            Global.VisionApp.SetToolData("Task2", "定义变量", 2111, 0, Global.Ratio.ToString(), 1);
            Global.VisionApp.SetToolData("Task2", "定义变量", 2112, 0, Global.ARatio.ToString(), 1);
            //SysPara.Hoffset = SysPara.ParmSystem.ParmData.Hoffset;
            if (Btn1 && Btn2) { Global.IsRecipe = true; }
            else { Global.IsRecipe = false; }

            VisionGlobal.bMak1_X = Global.Parm.BMak1Pos.Xpos;
            VisionGlobal.bMak1_Y = Global.Parm.BMak1Pos.Ypos;

            VisionGlobal.bMak2_X = Global.Parm.BMak2Pos.Xpos;
            VisionGlobal.bMak2_Y = Global.Parm.BMak2Pos.Ypos;

            ObjtoFrom();

            try
            {
                //Global.BomData = new JD.Fai.Data.FlyingProbe().GetDataTable(XYCodeTxt.Text);
                algorithm.CalculateTransferXY(Global.BomData, out X, out Y);
                VisionGlobal.TranslationX = X;
                VisionGlobal.TranslationY = Y;
                this.BomDgv.DataSource = Global.BomData;
                FloatingTip.ShowOk("导入BOM坐标成功".tr());
            }
            catch
            {
                FloatingTip.ShowError("导入BOM坐标失败".tr());
            }
        }

        private void LoadFlie_Click(object sender, EventArgs e)
        {
            double X, Y;
            try
            {
                string FileName = null;

                if (FileEx.OpenDialog(ref FileName, "csv文件|*.csv", "csv"))
                {
                    Global.BomData = new BrowLib.FileClass.DataCsv().OpenCsv(FileName);
                    algorithm.CalculateTransferXY(Global.BomData, out X, out Y);
                    VisionGlobal.TranslationX = X;
                    VisionGlobal.TranslationY = Y;
                    this.BomDgv.DataSource = Global.BomData;
                    BomDgv.Update();
                    Global.Parm.SetCode = Global.RecipePatn;
                    bool Btn1 = Global.Parm.Read(ref Global.Parm);

                    VisionGlobal.bMak1_X = Global.Parm.BMak1Pos.Xpos;
                    VisionGlobal.bMak1_Y = Global.Parm.BMak1Pos.Ypos;

                    VisionGlobal.bMak2_X = Global.Parm.BMak2Pos.Xpos;
                    VisionGlobal.bMak2_Y = Global.Parm.BMak2Pos.Ypos;
                    ObjtoFrom();
                    Global.VisionApp.SetCodePath = Global.RecipePatn;
                    Global.VisionApp.ReadObj("Mark.proc", "Task3");
                    FloatingTip.Show("导入BOM坐标表成功".tr());
                }
                else
                {
                    FloatingTip.ShowError("导入BOM坐标表失败".tr());
                }
            }
            catch (Exception ex)
            {
                FloatingTip.ShowError(ex.ToString(), 5000);
            }
        }

        private void GoTo_btn2_Click(object sender, EventArgs e)
        {

        }

        private void Find_btn_Click(object sender, EventArgs e)
        {

            try
            {
                for (int i = 0; i < BomDgv.Rows.Count; i++)
                {
                    if (Global.BomData.Rows[i]["位置号"].ToString().Trim() == Code_txt.Text)
                    {
                        this.BomDgv.Rows[i].Selected = true;
                        this.BomDgv.FirstDisplayedScrollingRowIndex = i;
                        this.BomDgv.BindingContext[this.BomDgv.DataSource].Position = i;
                        return;
                    }
                }
            }
            catch
            {
                FloatingTip.ShowWarning("位置号不存在".tr());
            }
        }
        #endregion

        #region Mark设置
        private void M_Save_Click(object sender, EventArgs e)
        {
            Global.VisionApp.SetCodePath = Global.RecipePatn;
            if (Global.VisionApp.Saveobj("Mark.proc", "Task3"))
            {
                FloatingTip.ShowOk("配方:".tr() + Global.RecipePatn + "Mark保存成功".tr());
            }
            else
            {
                FloatingTip.ShowError("配方:".tr() + Global.RecipePatn + "Mark保存失败".tr());
            }
        }

        private void M_Save2_Click(object sender, EventArgs e)
        {
            if (KryptonMessageBox.Show("确定要保存参数吗？".tr()) != DialogResult.OK) { return; }
            try
            {
                Global.Parm.Mak1Pos.Xpos = Convert.ToDouble(Mask1Xpos.Value);
                Global.Parm.Mak1Pos.Ypos = Convert.ToDouble(Mask1Ypos.Value);

                Global.Parm.Mak2Pos.Xpos = Convert.ToDouble(Mask2Xpos.Value);
                Global.Parm.Mak2Pos.Ypos = Convert.ToDouble(Mask2Ypos.Value);

                Global.Parm.PbXoffset = Convert.ToDouble(PbXoffset_num.Value);
                Global.Parm.PbYoffset = Convert.ToDouble(PbYoffset_num.Value);

                Global.Parm.Offset_X = Convert.ToDouble(OffsetX_num.Value);
                Global.Parm.Offset_Y = Convert.ToDouble(OffsetY_num.Value);

                VisionGlobal.bMak1_X = Global.Parm.Mak1Pos.Xpos;
                VisionGlobal.bMak1_Y = Global.Parm.Mak1Pos.Ypos;

                VisionGlobal.bMak2_X = Global.Parm.Mak2Pos.Xpos;
                VisionGlobal.bMak2_Y = Global.Parm.Mak2Pos.Ypos;

                Global.Parm.SetCode = Global.RecipePatn;

                Global.Parm.Save();

                FloatingTip.ShowOk("参数保存成功".tr());
            }
            catch (Exception ex)
            {
                FloatingTip.ShowError("参数保存失败".tr() + ex.ToString());
            }
        }

        private void MTeachPos_Click(object sender, EventArgs e)
        {
            Mask1Xpos.Value = decimal.Parse(Global.X轴.GetPrfPos().ToString());
            Mask1Ypos.Value = decimal.Parse(Global.Y轴.GetPrfPos().ToString());
        }

        private void MGoTo_btn_Click(object sender, EventArgs e)
        {
            if (!Global.SystemInitialOk)
            {
                APP.Tip.ShowTip(0, "警告", "设备未归零,请先归零再执行操作".tr(), "确定".tr());
                return;
            }
            if (Flow.IsManualRun()) { return; }
            WaitFrm waitFrm = new WaitFrm(MGoTo_btn);
            waitFrm.Enabled = true;
            Flow.ExecuteManual(() =>
            {
                new HandFlow().SafeMoveXyz(0, (double)Mask1Xpos.Value, (double)Mask1Ypos.Value, Global.CamHeight);
                Global.VisionApp.SetToolData("Task3", "定义变量", 2108, 0, "1", 1);

                Invoke(new Action(() =>
                {
                    waitFrm.Enabled = false;
                }));
            });
        }

        private void MTeachPos2_Click(object sender, EventArgs e)
        {
            Mask1Xpos.Value = decimal.Parse(Global.X轴.GetPrfPos().ToString());
            Mask1Ypos.Value = decimal.Parse(Global.Y轴.GetPrfPos().ToString());
        }

        private void MView_pic_Resize(object sender, EventArgs e)
        {
            Global.VisionApp.ShowQuick(this.MView_pic.Handle, "Task3", MView_pic.Width, MView_pic.Height);
        }
        private void MGoTo_btn2_Click(object sender, EventArgs e)
        {
            if (!Global.SystemInitialOk)
            {
                APP.Tip.ShowTip(0, "警告", "设备未归零,请先归零再执行操作".tr(), "确定".tr());
                return;
            }
            if (Flow.IsManualRun()) { return; }
            WaitFrm waitFrm = new WaitFrm(MGoTo_btn2);
            waitFrm.Enabled = true;
            Flow.ExecuteManual(() =>
            {
                new HandFlow().SafeMoveXyz(0, (double)Mask2Xpos.Value, (double)Mask2Ypos.Value, Global.CamHeight);
                Global.VisionApp.SetToolData("Task3", "定义变量", 2108, 0, "2", 1);
                Invoke(new Action(() =>
                {
                    waitFrm.Enabled = false;
                }));
            });
        }


        #endregion

        #region 视觉编辑

        private string CurrentName = "Task4";

        private void CamSecond_btn_Click(object sender, EventArgs e)
        {
           
            Global.VisionApp.ShowQuick(this.View_Pic.Handle, "Task4", this.View_Pic.Width, this.View_Pic.Height);
            CurrentName = "Task4";
        }

        private void View_Pic_Resize(object sender, EventArgs e)
        {
            Global.VisionApp.ShowQuick(this.View_Pic.Handle, CurrentName, this.View_Pic.Width, this.View_Pic.Height);
        }

        private void CSave_btn_Click(object sender, EventArgs e)
        {
            if (!Global.VisionApp.SaveProject(@"./VisionProject/VisionPro.proj"))
            {
                FloatingTip.ShowError("项目文件保存失败");
            }
            else
            {
                FloatingTip.ShowOk("项目文件保存成功".tr());
            }
        }
        private void Blanksticker_btn_Click(object sender, EventArgs e)
        {
            Global.VisionApp.ShowQuick(this.View_Pic.Handle, "Task6", this.View_Pic.Width, this.View_Pic.Height);
            CurrentName = "Task6";
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
            Spd = Convert.ToDouble(Slow_tb.Value);
            switch (KeyName)
                    {
                        case "DZ_N":       /*********【顶升轴-方向】***********/
                            if (Jop_Rio.Checked)
                            {
                                Global.顶升轴.JOP(0, Spd);
                            }
                            else
                            {
                              Global.顶升轴.PMove(Spd, 1000, -1 * Convert.ToDouble(Motdis_num.Value), 0);
                            }
                            break;
                        case "DZ_P":              /*********【顶升轴+方向】***********/
                            if (Jop_Rio.Checked)
                            {
                                Global.顶升轴.JOP(1, Spd);
                            }
                            else
                            {
                                Global.顶升轴.PMove(Spd, 1000, Convert.ToDouble(Motdis_num.Value), 0);
                            }
                            break;
                    }
              
        }
        private void JMouseUp(object sender, MouseEventArgs e)
        {
            if (Jop_Rio.Checked)
            {
                Global.顶升轴.AxisStop();
            }
        }
        #endregion



    }
}
