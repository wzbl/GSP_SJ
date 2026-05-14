
using BrowApp;
using BrowApp.Language;
using BrowApp.MessageTip;
using BrowLib;
using CKVisionAppNet;
using ComponentFactory.Krypton.Toolkit;
using GSP.SystemData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace GSP.UI
{
    public partial class UI_CalibForm : UserControl
    {
        private int Selectidex = 0, Selectidex2 = 0, Selectidex3 = 0;

        private double OffsetX, OffsetY;

        private string SelectTask = "Task1";

        FlowControl flow=new FlowControl();
        public UI_CalibForm()
        {
            InitializeComponent();
            Global.Laser.Serial.ComRefEvent += new EventHandler(RefLaser);
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
        void RefLaser(object sender, EventArgs e)
        {
            try
            {
                this.BeginInvoke(new Action(() =>
                {
                    LaserValue_Lab.Text = Global.Laser.LaserValue(Global.LaserType).ToString("F3") + "mm";
                }));
            }
            catch { }
        }
        private void UI_CalibForm_Load(object sender, EventArgs e)
        {
           DatatableIni(TPosDgv, Global.CalibData.TPosTable);
           this.OffsetDgv.DataSource = new BindingSource(Global.CalibData.Offsets,null);//绑定数据源
           this.SizeDgv.DataSource = new BindingSource(Global.CalibData.Sizes, null);//绑定数据源
            ObjToForm();
           BrowApp.Language.Language.Instance.UpdateLanguage(this, null);
           if(Global.Is_DownCam)
            {
                kryptonGroupBox8.Visible = true;
                kryptonGroupBox11.Visible = true;

            }
            else
            {
                kryptonGroupBox8.Visible = false;
                kryptonGroupBox11.Visible = false;
            }
        }
        private void ObjToForm()
        {   
            //相机标定
            Xpos_unm.Value = decimal.Parse(Global.CalibData.CalibParm.Xpos.ToString());
            Ypos_unm.Value= decimal.Parse(Global.CalibData.CalibParm.Ypos.ToString());
            Zpos_unm.Value=decimal.Parse(Global.CalibData.CalibParm.Zpos.ToString());
            offset_num.Value= decimal.Parse(Global.CalibData.CalibParm.Offect.ToString());
            
            //相机VS偏差
            CamPos_X.Value = decimal.Parse(Global.CalibData.CamPos.Xpos.ToString());
            CamPos_Y.Value = decimal.Parse(Global.CalibData.CamPos.Ypos.ToString());
            laserPos_X.Value = decimal.Parse(Global.CalibData.LaserPos.Xpos.ToString());
            laserPos_Y.Value= decimal.Parse(Global.CalibData.LaserPos.Ypos.ToString());
            Reference_value.Value= decimal.Parse(Global.CalibData.LaserValue.ToString());
            dx_Value.Value= decimal.Parse(Global.CalibData.Laser_X.ToString());
            dy_Value.Value= decimal.Parse(Global.CalibData.Laser_Y.ToString());
            allowOffset_Value.Value=decimal.Parse(Global.CalibData.Allowablevalue.ToString());

            PinPos_X.Value= decimal.Parse(Global.CalibData.PinPos.Xpos.ToString());
            PinPos_Y.Value = decimal.Parse(Global.CalibData.PinPos.Ypos.ToString());
            PinPos_Z.Value = decimal.Parse(Global.CalibData.PinPos.Zpos.ToString());

            CenterX.Value = decimal.Parse(Global.CalibData.CenterData.CenterX.ToString());
            CenterY.Value = decimal.Parse(Global.CalibData.CenterData.CenterY.ToString());
           
            CenterDx.Value = decimal.Parse(Global.CalibData.CenterData.CenterdX.ToString());
            CenterDy.Value = decimal.Parse(Global.CalibData.CenterData.CenterdY.ToString());
        }
        public void DatatableIni(DataGridView DataGridView, DataTable dDtaTable)
        {
            try
            {
                if (dDtaTable.Columns.Count != DataGridView.Columns.Count)
                {
                    dDtaTable.Columns.Clear();
                    for (int i = 0; i < DataGridView.Columns.Count; i++)
                    {
                        dDtaTable.Columns.Add(DataGridView.Columns[i].HeaderText);
                    }
                }
                DataGridView.Columns.Clear();
                DataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                DataGridView.DataSource = null;
                DataGridView.DataSource = dDtaTable;
                DataGridView.Update();
                for (int i = 0; i < DataGridView.Columns.Count; i++)
                {
                    DataGridView.Columns[i].Resizable = DataGridViewTriState.False;
                    DataGridView.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                }
                DataGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DataGridView.AllowUserToResizeColumns = false;
                DataGridView.AllowUserToResizeRows = false;
            }
            catch { }
        }
        /// <summary>
        /// 9点标定
        /// </summary>
        /// <param name="StartXpos"></param>
        /// <param name="StartYpos"></param>
        /// <param name="offset"></param>
        /// <param name="PixX"></param>
        /// <param name="PixY"></param>
        /// <param name="PosX"></param>
        /// <param name="PosY"></param>
        private void PointCalib(double StartXpos, double StartYpos, double offset, out double[] PixX, out double[] PixY, out double[] PosX, out double[] PosY)
        {

            String CalibPath = Directory.GetCurrentDirectory().ToString() + "\\Systemfile\\Calib.tpf";//设置标定路径
            PixX = new double[9]; PixY = new double[9]; PosX = new double[9]; PosY = new double[9];
            double[] MoveX = new double[9] { StartXpos, StartXpos + offset, StartXpos + offset, StartXpos, StartXpos - offset, StartXpos - offset, StartXpos - offset, StartXpos, StartXpos + offset };
            double[] MoveY = new double[9] { StartYpos, StartYpos, StartYpos + offset, StartYpos + offset, StartYpos + offset, StartYpos, StartYpos - offset, StartYpos - offset, StartYpos - offset };
            bool rtn;
            Global.VisionApp.SetToolData("Task1", "标定数据", 2100, 0, "0", 1);
            Global.VisionApp.SetToolData("Task1", "标定数据", 2103, 0, CalibPath.ToString(), 3);
            for (int i = 0; i < 9; i++)
            {
                BrowLib.Controller.MotionAPI.LinXyMoveA(500, 5000, MoveX[i], MoveY[i], 1);
                do
                {
                    if (!Global.X轴.GetSevOn() || !Global.Y轴.GetSevOn()) { BrowApp.APP.Tip.ShowTip(1, "警告".tr(), "轴未上使能？".tr()); return; }
                    rtn = BrowLib.Controller.MotionAPI.LinXYRuningA();
                    Thread.Sleep(2);
                }
                while (!rtn);
                Global.VisionApp.SetToolData("Task1", "标定数据", 2101, 0, MoveX[i].ToString(), 2);
                Global.VisionApp.SetToolData("Task1", "标定数据", 2102, 0, MoveY[i].ToString(), 2);
                Global.VisionApp.ExecuteProc("Task1", 0);
                do
                {
                    Thread.Sleep(2);
                }
                while (!Global.VisionApp.IsEndProc);
                if (!Global.VisionApp.RunState("Task1", "执行结果")) { BrowApp.APP.Tip.ShowTip(1, "警告".tr(), "标定失败!!!".tr()); return; };
                PosX[i] = MoveX[i]; PosY[i] = MoveY[i];
                Thread.Sleep(500);
            }
             BrowApp.APP.Tip.ShowTip(0, "提示".tr(), "标定成功!!!".tr(), "确定".tr());

        }
        /// <summary>
        /// 9点旋转标定
        /// </summary>
        private void RotatePointCalib(double StartXpos, double StartYpos, double offset,double offsetAngle, out double[] PixX, out double[] PixY, out double[] PosX, out double[] PosY)
        {
            int Step=0;
            bool Runflag = true;
            int Rnum = 0;
            String CalibPath = Directory.GetCurrentDirectory().ToString() + "\\Systemfile\\DownCalib.tpf";//设置标定路径
            PixX = new double[9]; PixY = new double[9]; PosX = new double[9]; PosY = new double[9];
            double[] MoveX = new double[9] { StartXpos, StartXpos + offset, StartXpos + offset, StartXpos, StartXpos - offset, StartXpos - offset, StartXpos - offset, StartXpos, StartXpos + offset };
            double[] MoveY = new double[9] { StartYpos, StartYpos, StartYpos + offset, StartYpos + offset, StartYpos + offset, StartYpos, StartYpos - offset, StartYpos - offset, StartYpos - offset };
            Global.VisionApp.SetToolData("Task7", "标定变量", 2110, 0, "1", 1);
            Global.VisionApp.SetToolData("Task7", "标定变量", 2100, 0, "0", 1);
            Global.VisionApp.SetToolData("Task7", "标定变量", 2103, 0, CalibPath.ToString(), 3);
            int i = 0;
            while(Runflag)
            {
                //电机错误检查
                if (!Global.X轴.GetSevOn() || !Global.Y轴.GetSevOn()|| !Global.R轴.GetSevOn()) { FloatingTip.ShowError("轴未上使能".tr()); return; }
                if (Global.X轴.ALM() || Global.Y轴.ALM() || Global.R轴.ALM()) { FloatingTip.ShowError("驱动报警".tr()); return; }
                switch (Step)
                {
                    case 0:
                        BrowLib.Controller.MotionAPI.LinXyMoveA(500, 5000, MoveX[i], MoveY[i], 1);
                        Step++;
                        break;
                    case 1:
                        if (BrowLib.Controller.MotionAPI.LinXYRuningA())
                        {
                            Thread.Sleep(20);
                            Global.VisionApp.SetToolData("Task7", "标定变量", 2101, 0, MoveX[i].ToString(), 2);
                            Global.VisionApp.SetToolData("Task7", "标定变量", 2102, 0, MoveY[i].ToString(), 2);
                            Global.VisionApp.ExecuteProc("Task7", 0);
                            i++;
                            Step++;
                        }
                        break;
                    case 2:
                        if(Global.VisionApp.EndProc["Task7"])
                        {
                            if (!Global.VisionApp.RunState("Task7", "执行结果"))
                            {
                                FloatingTip.ShowError("标定失败!!!");
                                return;
                            }
                            else
                            {
                               if(i<9)
                                {
                                    Step=0;
                                    PosX[i] = MoveX[i]; PosY[i] = MoveY[i];
                                }
                                else
                                {
                                    FloatingTip.ShowOk("9点标定完成".tr());
                                    Step++;
                                }
                            }
                        }
                        break;
                    case 3: //旋转标定
                        BrowLib.Controller.MotionAPI.LinXyMoveA(500, 5000, StartXpos, StartYpos, 1);
                        Global.VisionApp.SetToolData("Task7", "标定变量", 2110, 0, "2", 1);
                        Global.VisionApp.SetToolData("Task7", "标定变量", 2100, 0, "0", 1);
                        Thread.Sleep(20);
                        Step++;
                        break;
                    case 4:
                        if (BrowLib.Controller.MotionAPI.LinXYRuningA())
                        {
                            Thread.Sleep(20);
                            //找角度
                            //Global.VisionApp.ExecuteProc("Task7", 0);
                            Step++;
                        }
                        break;
                    case 5:
                        double[] Rlist = new double[19] {-offsetAngle,-80,-70,-60,-50,-40,-30,-20,-10,0,10,20,30,40,50,60,70,80,offsetAngle };
                        Global.R轴.PMove(20, 1000, Rlist[Rnum], 1);
                        Step++;
                        break;
                    case 6:
                        if(Global.R轴.Runing())
                        {
                            Rnum++;
                            //找角度
                            Thread.Sleep(20);
                            Global.VisionApp.ExecuteProc("Task7", 0);
                            if(Rnum<19)
                            {
                                Step = 5;
                            }
                            else
                            {
                                Step++;
                            }
                        }
                        break;
                    case 7://计算旋转中心
                        double CenX = Global.VisionApp.GetDblValue("Task7", "标定变量", 2106, 0);
                        double CenY = Global.VisionApp.GetDblValue("Task7", "标定变量", 2107, 0);

                        double CenDX = Global.VisionApp.GetDblValue("Task7", "标定变量", 2108, 0);
                        double CenDY = Global.VisionApp.GetDblValue("Task7", "标定变量", 2109, 0);
                        this.Invoke(new Action(() =>
                        {
                            CenterX.Value = decimal.Parse(CenX.ToString());
                            CenterY.Value = decimal.Parse(CenY.ToString());

                            CenterDx.Value = decimal.Parse(CenDX.ToString());
                            CenterDy.Value = decimal.Parse(CenDY.ToString());
                        }));
                        Step++;
                        break;
                    case 8:
                        FloatingTip.ShowOk("旋转中心标定完成".tr());
                        Runflag = false;
                        Step = 0;
                        break;
                }
                Thread.Sleep(200);
            }
        }

        #region 相机标定
        private void TeachPos_Click(object sender, EventArgs e)
        {
            Xpos_unm.Value = decimal.Parse(Global.X轴.GetPrfPos().ToString());
            Ypos_unm.Value = decimal.Parse(Global.Y轴.GetPrfPos().ToString());
            Zpos_unm.Value = decimal.Parse(Global.Z轴.GetPrfPos().ToString());
        }

        private void GoToBtn_Click(object sender, EventArgs e)
        {
            if (!Global.SystemInitialOk)
            {
                APP.Tip.ShowTip(0, "警告".tr(), "设备未归零,请先归零再执行操作!!!", "确定".tr());
                return;
            }
            if (flow.IsManualRun()) { return; }
            WaitFrm wait=new WaitFrm(GoToBtn,"定位中,请稍后");
            wait.Enabled = true;
            flow.ExecuteManual(() =>
            {
                new HandFlow().SafeMoveXyz(0, (double)Xpos_unm.Value, (double)Ypos_unm.Value, (double)Zpos_unm.Value);

                Invoke(new Action(() =>
                {
                    wait.Enabled = false;
                }));
            });
        }

        private void TeachPos1_Click(object sender, EventArgs e)
        {
            try
            {
                PinPos_X.Value = decimal.Parse(Global.X轴.GetPrfPos().ToString());
                PinPos_Y.Value = decimal.Parse(Global.Y轴.GetPrfPos().ToString());
                PinPos_Z.Value = decimal.Parse(Global.Z轴.GetPrfPos().ToString());
            }
            catch { }

        }
        private void GoToBtn1_Click(object sender, EventArgs e)
        {
            if (!Global.SystemInitialOk)
            {
                APP.Tip.ShowTip(0, "警告".tr(), "设备未归零,请先归零再执行操作!!!".tr(), "确定".tr());
                return;
            }
            if (flow.IsManualRun()) { return; }
            WaitFrm waitFrm = new WaitFrm(GoToBtn1);
            waitFrm.Enabled = true;
            flow.ExecuteManual(() =>
            {
                new HandFlow().SafeMoveXyz(0, (double)PinPos_X.Value, (double)PinPos_Y.Value, (double)PinPos_Z.Value);

                Invoke(new Action(() =>
                {
                    waitFrm.Enabled = false;
                }));
            });
        }
        private void Save_Btn_Click(object sender, EventArgs e)
        {
            if ( BrowApp.APP.Tip.ShowTip(0, "提示".tr(), "确定要保存参数吗？".tr(), "确定".tr(),"取消".tr()) != 1) { return; }
            try
            {
                Global.CalibData.CalibParm.Xpos = Convert.ToDouble(Xpos_unm.Value);
                Global.CalibData.CalibParm.Ypos = Convert.ToDouble(Ypos_unm.Value);
                Global.CalibData.CalibParm.Zpos = Convert.ToDouble(Zpos_unm.Value);
                Global.CalibData.CalibParm.Offect = Convert.ToDouble(offset_num.Value);
                Global.CamHeight = Convert.ToDouble(Zpos_unm.Value);

                Global.CalibData.PinPos.Xpos = Convert.ToDouble(PinPos_X.Value);
                Global.CalibData.PinPos.Ypos = Convert.ToDouble(PinPos_Y.Value);
                Global.CalibData.PinPos.Zpos = Convert.ToDouble(PinPos_Z.Value);

                Global.CalibData.CenterData.CenterX = Convert.ToDouble(CenterX.Value);
                Global.CalibData.CenterData.CenterY = Convert.ToDouble(CenterY.Value);

                Global.CalibData.CenterData.CenterdX = Convert.ToDouble(CenterDx.Value);
                Global.CalibData.CenterData.CenterdY = Convert.ToDouble(CenterDy.Value);


                Global.CalibData.Save();
                FloatingTip.ShowOk("参数保存成功!".tr());
            }
            catch
            {
              FloatingTip.ShowError("参数保存失败!".tr());
            }
        }
        private void View_pic_Resize(object sender, EventArgs e)
        {
            Global.VisionApp.ShowQuick(this.View_pic.Handle, SelectTask, this.View_pic.Width, this.View_pic.Height);
        }

        private void UpCam_ck_Click(object sender, EventArgs e)
        {
            this.SelectTask = "Task1";
            View_pic_Resize(this,null);
        }
        private void DownCam_ck_Click(object sender, EventArgs e)
        {
            this.SelectTask = "Task7";
            View_pic_Resize(this, null);
        }
        /// <summary>
        /// 中心标定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CenterCalibration_btn_Click(object sender, EventArgs e)
        {
            DownCam_ck.Checked = true;
            DownCam_ck_Click(this, null);
            double StartX = Convert.ToDouble(PinPos_X.Value);
            double StartY = Convert.ToDouble(PinPos_Y.Value);
            double Offset = Convert.ToDouble(offset_num.Value);
            double dxAngle = Convert.ToDouble(offset_Angle.Value);
            double[] PixX; double[] PixY; double[] PosX; double[] PosY;
            if (flow.IsManualRun()) return;
            WaitFrm waitFrm = new WaitFrm(Calibration_Btn, "标定运行中,请稍后...".tr());
            waitFrm.Enabled = true;
            flow.ExecuteManual(() =>
            {
                RotatePointCalib(StartX, StartY, Offset, dxAngle, out PixX, out PixY, out PosX, out PosY);
                Invoke(new Action(() =>
                {
                    waitFrm.Enabled = false;
                    
                }));
            });
        }
        #endregion


        #region 相机VS测试针

        private void UpdataOffsetDgv(int Selectidex)
        {
            CType_txt.Text = OffsetDgv.Rows[Selectidex].Cells[0].Value.ToString();
            Angle_Num.Value = decimal.Parse(OffsetDgv.Rows[Selectidex].Cells[1].Value.ToString());
            Offset_Xnum.Value = decimal.Parse(OffsetDgv.Rows[Selectidex].Cells[2].Value.ToString());
            Offset_Ynum.Value = decimal.Parse(OffsetDgv.Rows[Selectidex].Cells[3].Value.ToString());
        }
        /// <summary>
        /// 选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OffsetDgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < this.OffsetDgv.RowCount)
            {
               
                try
                {
                    this.Selectidex = e.RowIndex;
                    CType_txt.Text = OffsetDgv.Rows[Selectidex].Cells[0].Value.ToString();
                    Angle_Num.Value = decimal.Parse(OffsetDgv.Rows[Selectidex].Cells[1].Value.ToString());
                    Offset_Xnum.Value = decimal.Parse(OffsetDgv.Rows[Selectidex].Cells[2].Value.ToString());
                    Offset_Ynum.Value = decimal.Parse(OffsetDgv.Rows[Selectidex].Cells[3].Value.ToString());
                }
                catch { }
            }
                
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_btn1_Click(object sender, EventArgs e)
        {
            Global.CalibData.Offsets.Add(new CalibData.OffsetType
            {
                Name = CType_txt.Text,
                Angle = Angle_Num.Value.ToString(),
                OffsetX = Offset_Xnum.Value.ToString(),
                OffsetY = Offset_Ynum.Value.ToString(),
                
            });
            ((BindingSource)OffsetDgv.DataSource).ResetBindings(false); // 重置绑定而不清除当前项
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delete_Btn1_Click(object sender, EventArgs e)
        {
            this.OffsetDgv.Rows.RemoveAt(this.Selectidex);
            if (this.Selectidex != 0)
                this.Selectidex = Selectidex - 1;
        }
       //应用
        private void Apply_btn_Click(object sender, EventArgs e)
        {
            Offset_Xnum.Value = decimal.Parse(OffsetX.ToString("F3"));
            Offset_Ynum.Value = decimal.Parse(OffsetY.ToString("F3"));
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Edit_Btn1_Click(object sender, EventArgs e)
        {
            try
            {
               
                int RowIndex= this.OffsetDgv.FirstDisplayedScrollingRowIndex;
                this.OffsetDgv.DataSource = new BindingSource(Global.CalibData.Offsets, null);//绑定数据源
                Global.CalibData.Offsets[Selectidex].Name = CType_txt.Text;
                Global.CalibData.Offsets[Selectidex].Angle = Angle_Num.Value.ToString();
                Global.CalibData.Offsets[Selectidex].OffsetX = Offset_Xnum.Value.ToString();
                Global.CalibData.Offsets[Selectidex].OffsetY = Offset_Ynum.Value.ToString();
                this.OffsetDgv.Rows[Selectidex].Selected = true;
                this.OffsetDgv.FirstDisplayedScrollingRowIndex=RowIndex;
            }
            catch { }
            this.OffsetDgv.Refresh();
        }
        private void Restore_btn_Click(object sender, EventArgs e)
        {
           Global.CalibData.Read(ref Global.CalibData);
           this.OffsetDgv.DataSource = new BindingSource(Global.CalibData.Offsets, null);//绑定数据源
        }
        private void Save_Btn1_Click(object sender, EventArgs e)
        {
            try
            {
                Global.CalibData.Save();
                FloatingTip.ShowOk("参数保存成功!".tr());
            }
            catch { FloatingTip.ShowOk("参数保存失败!".tr()); }
           
        }
        private void Add_btn2_Click(object sender, EventArgs e)
        {
            try
            {
                Global.CalibData.Sizes.Add(new CalibData.SizeType
                {
                    Name = CType_txt1.Text,
                    Size = SizeNum.Value.ToString(),
                    Hight = ZHightnum.Value.ToString(),
                    Length = UpDownL.Value.ToString(),
                    width = UpDownW.Value.ToString(),
                    Clipsize = PinchNum.Value.ToString(),
                });
                ((BindingSource)SizeDgv.DataSource).ResetBindings(false); // 重置绑定而不清除当前项
            }
            catch (Exception Error)
            { FloatingTip.ShowError(Error.Message); }
        }
        private void Delete_Btn12_Click(object sender, EventArgs e)
        {
            if (BrowApp.APP.Tip.ShowTip(0, "提示".tr(), "确定要删除参数吗？","确定".tr(),"取消".tr())!=1) { return; }
            try
            {
                this.SizeDgv.Rows.RemoveAt(this.Selectidex2);
                if (this.Selectidex2 != 0)
                    this.Selectidex2 = Selectidex2 - 1;
            }
            catch (Exception Error)
            { FloatingTip.ShowError(Error.Message); }
        }

        private void Edit_Btn2_Click(object sender, EventArgs e)
        {   try
            {
                int RowIndex = this.SizeDgv.FirstDisplayedScrollingRowIndex;
                this.SizeDgv.DataSource = new BindingSource(Global.CalibData.Sizes, null);//绑定数据源
                SizeDgv.Rows[Selectidex2].Cells[0].Value = CType_txt1.Text;
                SizeDgv.Rows[Selectidex2].Cells[1].Value = SizeNum.Value.ToString();
                SizeDgv.Rows[Selectidex2].Cells[2].Value = ZHightnum.Value.ToString();
                SizeDgv.Rows[Selectidex2].Cells[3].Value = UpDownL.Value.ToString();
                SizeDgv.Rows[Selectidex2].Cells[4].Value = UpDownW.Value.ToString();
                SizeDgv.Rows[Selectidex2].Cells[5].Value = PinchNum.Value.ToString();
                this.SizeDgv.Rows[Selectidex2].Selected = true;
                this.SizeDgv.FirstDisplayedScrollingRowIndex=RowIndex;
            }
            catch (Exception Error)
            { FloatingTip.ShowError(Error.Message); }
        }
        private void Automatic(string TYPE, double refL, double refG, out List<Global.Spectype> SpectypesOut)
        {
            double L = 0, K = 0, G = 0;
            SpectypesOut = new List<Global.Spectype>();
            foreach (Global.Spectype Type in Global.Specificatio)
            {
                if (Type.name.Contains(TYPE))
                {
                    L = Type.L;
                    K = Type.K;
                    G = Type.G;
                    break;
                }
            }
            for (int i = 0; i < Global.Specificatio.Count; i++)
            {
                double al = refL + (L - Global.Specificatio[i].L);
                double aG = refG + (G - Global.Specificatio[i].G);
                SpectypesOut.Add(new Global.Spectype
                {
                    name = Global.Specificatio[i].name,
                    L = al,
                    K = Global.Specificatio[i].K,
                    G = aG
                });
            }
        }
        private void Autocompute_btn_Click(object sender, EventArgs e)
        {
            if (BrowApp.APP.Tip.ShowTip(0, "提示".tr(), "确定替换自动计算数据？","确定".tr(), "取消".tr()) != 1) { return; }
            List<Global.Spectype> Spectypes = new List<Global.Spectype>();
            Automatic(kryptonTextBox2.Text, Convert.ToDouble(SizeNum.Value), Convert.ToDouble(ZHightnum.Value), out Spectypes);
            this.SizeDgv.Rows.Clear();
            for (int i = 0; i < Spectypes.Count; i++)
            {
                this.SizeDgv.Rows.Add(Spectypes[i].name + "R", Spectypes[i].L.ToString(), Spectypes[i].G.ToString(), (Global.Specificatio[i].L + 1).ToString(), (Global.Specificatio[i].K + 1).ToString());
                this.SizeDgv.Rows.Add(Spectypes[i].name + "C", Spectypes[i].L.ToString(), Spectypes[i].G.ToString(), (Global.Specificatio[i].L + 1).ToString(), (Global.Specificatio[i].K + 1).ToString());
            }
        }
        int selectedColumn;
        private void SizeDgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //点击列头选中整列
            if (e.RowIndex == -1)
            {
                SizeDgv.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;
                SizeDgv.Columns[e.ColumnIndex].Selected = true;//让此列立刻被选中
                selectedColumn = e.ColumnIndex;//记录被选中的列
            }
            else
                SizeDgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;//单选某个单元格
            SizeDgv.BeginEdit(false);

            if (e.RowIndex >= 0 && e.RowIndex < this.SizeDgv.RowCount)
            {
                try
                {
                    this.Selectidex2 = e.RowIndex;
                    CType_txt1.Text = SizeDgv.Rows[Selectidex2].Cells[0].Value.ToString();
                    SizeNum.Value = decimal.Parse(SizeDgv.Rows[Selectidex2].Cells[1].Value.ToString());
                    ZHightnum.Value = decimal.Parse(SizeDgv.Rows[Selectidex2].Cells[2].Value.ToString());
                    UpDownL.Value = decimal.Parse(SizeDgv.Rows[Selectidex2].Cells[3].Value.ToString());
                    UpDownW.Value = decimal.Parse(SizeDgv.Rows[Selectidex2].Cells[4].Value.ToString());
                    PinchNum.Value = decimal.Parse(SizeDgv.Rows[Selectidex2].Cells[5].Value.ToString());
                }
                catch (Exception error)
                {
                   FloatingTip.ShowError(error.ToString());
                }
            }
        }

        private void MoveItemUp<T>(List<T> list, int index)
        {
            if (index > 0) // 确保不是第一项
            {
                T item = list[index];
                list.RemoveAt(index);
                list.Insert(index - 1, item);
            }
        }
        private void MoveItemDown<T>(List<T> list, int index)
        {
            if (index < list.Count - 1) // 确保不是最后一项
            {
                T item = list[index];
                list.RemoveAt(index);
                list.Insert(index + 1, item);
            }
        }
        /// <summary>
        /// 上移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void up_btn_Click(object sender, EventArgs e)
        {
            MoveItemUp<CalibData.SizeType>(Global.CalibData.Sizes, this.Selectidex2);
            this.Selectidex2--;
            SizeDgv.Rows[Selectidex2].Selected = true;
            this.SizeDgv.Refresh();
        }
        /// <summary>
        /// 下移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void down_btn_Click(object sender, EventArgs e)
        {

            MoveItemDown<CalibData.SizeType>(Global.CalibData.Sizes, this.Selectidex2);
            this.Selectidex2++;
            SizeDgv.Rows[Selectidex2].Selected = true;
            this.SizeDgv.Refresh();
        }
        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Batch_btn_Click(object sender, EventArgs e)
        {
            if (selectedColumn == 1 || selectedColumn == 2)
            {
                foreach (var item in Global.CalibData.Sizes)
                {
                    if (Is_increment.Checked)
                    {
                        if (selectedColumn == 1)
                            item.Size = (Convert.ToDouble(item.Size)+Convert.ToDouble(value_num.Value)).ToString();
                        if (selectedColumn == 2)
                            item.Hight =(Convert.ToDouble(item.Hight) +Convert.ToDouble(value_num.Value)).ToString();
                    }
                    else
                    {
                        if (selectedColumn == 1)
                            item.Size = value_num.Value.ToString();
                        if (selectedColumn == 2)
                            item.Hight = value_num.Value.ToString();
                    }
                }
            }
            this.SizeDgv.Refresh();
        }
        /// <summary>
        /// 还原
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Restore_btn2_Click(object sender, EventArgs e)
        {
            Global.CalibData.Read(ref Global.CalibData);
            this.SizeDgv.DataSource = new BindingSource(Global.CalibData.Sizes, null);//绑定数据源
        }
        private void SizeGet_btn_Click(object sender, EventArgs e)
        {
            try
            {
                SizeDgv.Rows[Selectidex2].Cells[1].Value = Global.左夹爪轴.GetPrfPos().ToString();
            }
            catch (Exception Error)
            { FloatingTip.ShowError(Error.Message); }
        }

        private void HightGet_btn_Click(object sender, EventArgs e)
        {
            try
            {
                SizeDgv.Rows[Selectidex2].Cells[2].Value = Global.Z轴.GetPrfPos().ToString();
            }
            catch (Exception Error)
            { FloatingTip.ShowError(Error.Message); }
           
        }
        #endregion


        #region 激光校正
        private void TeachPos2_Click(object sender, EventArgs e)
        {
            CamPos_X.Value = decimal.Parse(Global.X轴.GetPrfPos().ToString());
            CamPos_Y.Value = decimal.Parse(Global.Y轴.GetPrfPos().ToString());
        }

        private void TeachPos3_Click(object sender, EventArgs e)
        {
            laserPos_X.Value = decimal.Parse(Global.X轴.GetPrfPos().ToString());
            laserPos_Y.Value = decimal.Parse(Global.Y轴.GetPrfPos().ToString());
        }

        private void GoToBtn2_Click(object sender, EventArgs e)
        {
            if (!Global.SystemInitialOk)
            {
                APP.Tip.ShowTip(0, "警告".tr(), "设备未归零,请先归零再执行操作!!!", "确定".tr());
                return;
            }
            if (flow.IsManualRun()) { return; }
            WaitFrm waitFrm = new WaitFrm(GoToBtn2,"定位运行中,请稍后".tr());
            waitFrm.Enabled = true;
            flow.ExecuteManual(() =>
            {
                new HandFlow().SafeMoveXyz(1, (double)CamPos_X.Value, (double)CamPos_Y.Value, Global.CamHeight);

                Invoke(new Action(() =>
                {
                    waitFrm.Enabled =false;
                }));
            });
        }

        private void GoToBtn3_Click(object sender, EventArgs e)
        {
            if (!Global.SystemInitialOk)
            {
                APP.Tip.ShowTip(0, "警告".tr(), "设备未归零,请先归零再执行操作!!!", "确定".tr());
                return;
            }
            if (flow.IsManualRun()) { return; }
            WaitFrm waitFrm = new WaitFrm(GoToBtn3, "定位运行中,请稍后".tr());
            waitFrm.Enabled = true;
            flow.ExecuteManual(() =>
            {
                new HandFlow().SafeMoveXyz(1, (double)laserPos_X.Value, (double)laserPos_Y.Value, Global.CamHeight);

                Invoke(new Action(() =>
                {
                    waitFrm.Enabled = false;
                }));
            });

        }

        private void LaserRead_btn_Click(object sender, EventArgs e)
        {
            Global.Laser.Send(Global.LaserType);
        }

        private void SetReference_btn_Click(object sender, EventArgs e)
        {
            Reference_value.Value = decimal.Parse(LaserValue_Lab.Text.Replace("mm", ""));
        }

        private void heightcorrection_btn_Click(object sender, EventArgs e)
        {
            double Nvalue = Convert.ToDouble(LaserValue_Lab.Text.Replace("mm", ""));
            double jvalue = Convert.ToDouble(Reference_value.Value.ToString());
            double Dix = jvalue - Nvalue;
            if (Math.Abs(Dix) > 1)
            {
                FloatingTip.ShowWarning("偏差超出范围!".tr());
               
                return;
            }
            else
            {
                offset_higth.Value = decimal.Parse(Dix.ToString("F3"));
            }
        }
        private void Apply_btn2_Click(object sender, EventArgs e)
        {
            Global.Hoffset = Convert.ToDouble(offset_higth.Value);
        }

        private void ComputeOffset_btn_Click(object sender, EventArgs e)
        {
            dx_Value.Value = decimal.Parse(((double)CamPos_X.Value - (double)laserPos_X.Value).ToString());
            dy_Value.Value = decimal.Parse(((double)CamPos_Y.Value - (double)laserPos_Y.Value).ToString());
        }

        private void Save_Btn2_Click(object sender, EventArgs e)
        {
            try
            {
                Global.CalibData.CamPos.Xpos = Convert.ToDouble(CamPos_X.Value);
                Global.CalibData.CamPos.Ypos = Convert.ToDouble(CamPos_Y.Value);

                Global.CalibData.LaserPos.Xpos = Convert.ToDouble(laserPos_X.Value);
                Global.CalibData.LaserPos.Ypos = Convert.ToDouble(laserPos_Y.Value);

                Global.CalibData.LaserValue = Convert.ToDouble(Reference_value.Value);
                Global.CalibData.Laser_X = Convert.ToDouble(dx_Value.Value);
                Global.CalibData.Laser_Y = Convert.ToDouble(dy_Value.Value);
                Global.CalibData.Allowablevalue= Convert.ToDouble(allowOffset_Value.Value);

                Global.CalibData.Save();
                FloatingTip.ShowOk("参数保存成功!".tr());
            }
            catch { FloatingTip.ShowOk("参数保存失败!".tr()); }
        }
        #endregion

        #region 校正测试
        //删除
        private void Deletebtn_Click(object sender, EventArgs e)
        {
            try
            {
                Global.CalibData.TPosTable.Rows.RemoveAt(this.Selectidex3);
                this.Selectidex3 = Selectidex3 - 1;
            }
            catch { }
        }
        //修改
        private void Editbtn_Click(object sender, EventArgs e)
        {
            try
            {
                TPosDgv.Rows[Selectidex3].Cells[0].Value = Type_txt.Text;
                TPosDgv.Rows[Selectidex3].Cells[1].Value = XNum.Value.ToString();
                TPosDgv.Rows[Selectidex3].Cells[2].Value = YNum.Value.ToString();
                TPosDgv.Rows[Selectidex3].Cells[3].Value = RNum.Value.ToString();
                TPosDgv.Rows[Selectidex3].Cells[4].Value = CamHNum.Value.ToString();
                TPosDgv.Rows[Selectidex3].Cells[5].Value = ZNum.Value.ToString();
                TPosDgv.Rows[Selectidex3].Cells[6].Value = XYSpdNum.Value.ToString();
                TPosDgv.Rows[Selectidex3].Cells[7].Value = ZSpdNum.Value.ToString();
            }
            catch { }
        }

        private void AddBtn_Click(object sender, EventArgs e)
        {
            Global.CalibData.TPosTable.Rows.Add(Type_txt.Text, "0", "0", "0",
            "0", "0", "100", "20");
            Selectidex3 = this.TPosDgv.Rows.Count - 1;
            this.TPosDgv.Rows[Selectidex3].Selected = true;
            this.TPosDgv.BindingContext[this.TPosDgv.DataSource].Position = Selectidex3;
            //PosDgv_SelectIndexChange(sender, new int());
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            BrowLib.FileClass.DataCsv Csv = new BrowLib.FileClass.DataCsv();
            if ( BrowApp.APP.Tip.ShowTip(0, "提示".tr(), "确定要保存参数吗？".tr(), "确定".tr(), "取消".tr()) != 1) { return; }
            {
                try
                {
                    Global.CalibData.TPosTable = Csv.dvtodt(this.TPosDgv);
                    DatatableIni(this.TPosDgv, Global.CalibData.TPosTable);
                    this.TPosDgv.Rows[Selectidex3].Selected = true;
                    this.TPosDgv.BindingContext[this.TPosDgv.DataSource].Position = Selectidex3;
                    Global.CalibData.Save();
                    BrowApp.MessageTip.FloatingTip.ShowOk("参数保存成功!!!".tr());
                }
                catch 
                {
                    BrowApp.MessageTip.FloatingTip.ShowOk("参数保存失败!!!".tr());
                }
            }
        }
        /// <summary>
        /// 相机位置学习
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CamHteachBtn_Click(object sender, EventArgs e)
        {
            try
            {
                TPosDgv.Rows[Selectidex3].Cells[1].Value = decimal.Parse(Global.X轴.GetPrfPos().ToString());
                TPosDgv.Rows[Selectidex3].Cells[2].Value = decimal.Parse(Global.Y轴.GetPrfPos().ToString());
                TPosDgv.Rows[Selectidex3].Cells[4].Value = decimal.Parse(Global.Z轴.GetPrfPos().ToString());
            }
            catch { }
        }

        private void PinteachBtn_Click(object sender, EventArgs e)
        {
            try
            {
                TPosDgv.Rows[Selectidex3].Cells[5].Value = decimal.Parse(Global.Z轴.GetPrfPos().ToString());
            }
            catch { }
        }
        /// <summary>
        /// 相机标定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Calibration_Btn_Click(object sender, EventArgs e)
        {
            UpCam_ck.Checked = true;
            UpCam_ck_Click(this, EventArgs.Empty);
            double StartX = Convert.ToDouble(Xpos_unm.Value);
            double StartY = Convert.ToDouble(Ypos_unm.Value);
            double Offset = Convert.ToDouble(offset_num.Value);
            double[] PixX; double[] PixY; double[] PosX; double[] PosY;
            if (flow.IsManualRun()) return;
            WaitFrm waitFrm = new WaitFrm(Calibration_Btn,"标定运行中,请稍后...".tr());
            waitFrm.Enabled = true;
            flow.ExecuteManual(() =>
            {
                PointCalib(StartX, StartY, Offset, out PixX, out PixY, out PosX, out PosY);
                Invoke(new Action(() =>
                {
                    waitFrm.Enabled = false;
                    Pix_PosNum.Value = decimal.Parse(Global.Pix.ToString());
                }));
            });
        }
        private void FindIndex(string Angle)
        {
            try
            {
                for (int i = 0; i < OffsetDgv.Rows.Count; i++)
                {
                    if (OffsetDgv.Rows[i].Cells[1].Value.ToString().Trim() == Angle)
                    {
                        this.OffsetDgv.Rows[i].Selected = true;
                        this.OffsetDgv.FirstDisplayedScrollingRowIndex = i;
                        this.OffsetDgv.BindingContext[this.OffsetDgv.DataSource].Position = i;
                        this.Selectidex = i;
                        // 选中目标行
                        this.OffsetDgv.Rows[Selectidex].Selected = true;
                        UpdataOffsetDgv(this.Selectidex);
                        return;
                    }
                }
                FloatingTip.ShowWarning("不存在<" + Angle + ">偏差数据");
            }
            catch
            {
                FloatingTip.ShowWarning("不存在<" + Angle + ">偏差数据");
            }
        }

        private void offset_Angle_ValueChanged(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 默认计算角度
        /// </summary>
        private double[] defAnglelist = new double[] { 45.0, 90.0, -45.0 };
        private void Auto_btn_Click(object sender, EventArgs e)
        {
            double xOffset;
            double yOffset;
            Global.GetAngleOffset("0", out xOffset, out yOffset);
            double CenterdX;
            double CenterdY;
            new Algorithm().GetOffsetXY(Global.CalibData.PinPos.Xpos, Global.CalibData.PinPos.Ypos, 0.0, Global.CalibData.CenterData.CenterX, Global.CalibData.CenterData.CenterY, out CenterdX, out CenterdY);
            bool flag = xOffset != 0.0 && yOffset != 0.0;
            if (flag)
            {
                for (int i = 0; i < this.defAnglelist.Length; i ++)
                {
                    double NCenterdX;
                    double NCenterdY;
                    new Algorithm().GetOffsetXY(Global.CalibData.PinPos.Xpos, Global.CalibData.PinPos.Ypos, this.defAnglelist[i], Global.CalibData.CenterData.CenterX, Global.CalibData.CenterData.CenterY, out NCenterdX, out NCenterdY);
                    CalibData.OffsetType Re = Global.CalibData.Offsets.FirstOrDefault((CalibData.OffsetType H) => H.Angle == this.defAnglelist[i].ToString());
                    if (Re != null)
                    {
                        bool flag3 =  BrowApp.APP.Tip.ShowTip(0, "提示".tr(), "已经存在数据，确定要覆盖吗？".tr(), "确定".tr(), "取消".tr()) != 1;
                        if (flag3)
                        {
                            break;
                        }
                        Re.Name = this.CType_txt.Text;
                        Re.Angle = this.defAnglelist[i].ToString();
                        Re.OffsetX = (xOffset + (NCenterdX - CenterdX)).ToString("F3");
                        Re.OffsetY = (yOffset + (NCenterdY - CenterdY)).ToString("F3");
                        ((BindingSource)this.OffsetDgv.DataSource).ResetBindings(false);
                    }
                    else
                    {
                        Global.CalibData.Offsets.Add(new CalibData.OffsetType
                        {
                            Name = this.CType_txt.Text,
                            Angle = this.defAnglelist[i].ToString(),
                            OffsetX = (xOffset + (NCenterdX - CenterdX)).ToString("F3"),
                            OffsetY = (yOffset + (NCenterdY - CenterdY)).ToString("F3")
                        });
                        ((BindingSource)this.OffsetDgv.DataSource).ResetBindings(false);
                    }
                }
            }
        }
        private void Customization_btn_Click(object sender, EventArgs e)
        {
            double xOffset;
            double yOffset;
            Global.GetAngleOffset("0", out xOffset, out yOffset);
            double CenterdX;
            double CenterdY;
            new Algorithm().GetOffsetXY(Global.CalibData.PinPos.Xpos, Global.CalibData.PinPos.Ypos, 0.0, Global.CalibData.CenterData.CenterX, Global.CalibData.CenterData.CenterY, out CenterdX, out CenterdY);
            bool flag = xOffset != 0.0 && yOffset != 0.0;
            if (flag)
            {
                    double NCenterdX;
                    double NCenterdY;
                    new Algorithm().GetOffsetXY(Global.CalibData.PinPos.Xpos, Global.CalibData.PinPos.Ypos,Convert.ToDouble(Customizedangle.Value), Global.CalibData.CenterData.CenterX, Global.CalibData.CenterData.CenterY, out NCenterdX, out NCenterdY);
                    CalibData.OffsetType Re = Global.CalibData.Offsets.FirstOrDefault((CalibData.OffsetType H) => H.Angle == Customizedangle.Value.ToString());
                    if (Re != null)
                    {
                        bool flag3 = BrowApp.APP.Tip.ShowTip(0, "提示".tr(), "已经存在数据，确定要覆盖吗？".tr(), "确定".tr(), "取消".tr()) != 1;
                        if (flag3)
                        {
                           return;
                        }
                        Re.Name = this.CType_txt.Text;
                        Re.Angle = Customizedangle.Value.ToString();
                        Re.OffsetX = (xOffset + (NCenterdX - CenterdX)).ToString("F3");
                        Re.OffsetY = (yOffset + (NCenterdY - CenterdY)).ToString("F3");
                    ((BindingSource)this.OffsetDgv.DataSource).ResetBindings(false);
                }
                    else
                    {
                        Global.CalibData.Offsets.Add(new CalibData.OffsetType
                        {
                            Name = this.CType_txt.Text,
                            Angle = Customizedangle.Value.ToString(),
                            OffsetX = (xOffset + (NCenterdX - CenterdX)).ToString("F3"),
                            OffsetY = (yOffset + (NCenterdY - CenterdY)).ToString("F3")
                        });
                        ((BindingSource)this.OffsetDgv.DataSource).ResetBindings(false);
                    }
                }
            
        }

        private void kryptonDataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            SizeDgv_CellClick(sender, e);
        }

        private void CalculateBtn_Click(object sender, EventArgs e)
        {
            OffsetX = Convert.ToDouble(TPosDgv.Rows[Selectidex3].Cells[1].Value) - Global.X轴.GetPrfPos();
            OffsetY = Convert.ToDouble(TPosDgv.Rows[Selectidex3].Cells[2].Value) - Global.Y轴.GetPrfPos();
            string Offset = string.Format("X偏差:{0}    Y偏差:{1}", OffsetX.ToString("F3"), OffsetY.ToString("F3"));
            XYoffset_lab.Text = Offset;
            Offset_lab.Text= Offset;
            this.TabControl.SelectedIndex = 1;
            FindIndex(TPosDgv.Rows[Selectidex3].Cells[3].Value.ToString());
        }

        private void RuntoBtn_Click(object sender, EventArgs e)
        {
            if (!Global.SystemInitialOk)
            {
                APP.Tip.ShowTip(1, "警告".tr(), "设备未归零,请先归零再执行操作!!!".tr());
                return;
            }
            if (flow.IsManualRun()) { return; }

            double offsetX = 0, offsetY = 0;
           
            double X = Convert.ToDouble(TPosDgv.Rows[Selectidex3].Cells[1].Value);
            double Y = Convert.ToDouble(TPosDgv.Rows[Selectidex3].Cells[2].Value);
            double Z = Convert.ToDouble(TPosDgv.Rows[Selectidex3].Cells[4].Value);
            double R = Convert.ToDouble(TPosDgv.Rows[Selectidex3].Cells[3].Value);
            double Zhight = Convert.ToDouble(TPosDgv.Rows[Selectidex3].Cells[5].Value);

            double dis = Convert.ToDouble(ZHcDisnum.Value);
            double SLowspd = Convert.ToDouble(ZHcspdNum.Value);
            Global.GetOffset(R,out offsetX,out offsetY, Global.Is_DownCam);
            
            APP.Log.I_Log("旋转偏移dX:" + offsetX.ToString() + "旋转偏移dY:" + offsetY.ToString());
            if (IsYN.Checked)
            {
                X = X - offsetX;
                Y = Y - offsetY;
                Z = Zhight;
            }
            else
            {
                dis = 0;
            }
            double XYSpd = Convert.ToDouble(TPosDgv.Rows[Selectidex3].Cells[6].Value);
            double Zspd = Convert.ToDouble(TPosDgv.Rows[Selectidex3].Cells[7].Value);
            if (XYSpd == 0 || Zspd == 0) { APP.Tip.ShowTip(0, "警告".tr(), "速度不能为0？", "确定".tr()); return; }
            WaitFrm waitFrm = new WaitFrm(RuntoBtn);
            waitFrm.Enabled = true;
            flow.ExecuteManual(() =>
            {
                new HandFlow().SafeMoveXYZR(XYSpd, Zspd, 0, X, Y, Z, R, dis, SLowspd);

                Invoke(new Action(() =>
                {
                    waitFrm.Enabled = false;
                }));
            });
        }
        /// <summary>
        /// 选中刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TPosDgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                this.Selectidex3 =e.RowIndex;
                Type_txt.Text = TPosDgv.Rows[Selectidex3].Cells[0].Value.ToString();
                XNum.Value = decimal.Parse(TPosDgv.Rows[Selectidex3].Cells[1].Value.ToString());
                YNum.Value = decimal.Parse(TPosDgv.Rows[Selectidex3].Cells[2].Value.ToString());
                CamHNum.Value = decimal.Parse(TPosDgv.Rows[Selectidex3].Cells[4].Value.ToString());
                ZNum.Value = decimal.Parse(TPosDgv.Rows[Selectidex3].Cells[5].Value.ToString());
                RNum.Value = decimal.Parse(TPosDgv.Rows[Selectidex3].Cells[3].Value.ToString());
                XYSpdNum.Value = decimal.Parse(TPosDgv.Rows[Selectidex3].Cells[6].Value.ToString());
                ZSpdNum.Value = decimal.Parse(TPosDgv.Rows[Selectidex3].Cells[7].Value.ToString());

            }
            catch { }
        }
        #endregion
    }
}
