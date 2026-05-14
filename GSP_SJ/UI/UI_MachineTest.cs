using BrowApp;
using BrowApp.Language;
using BrowApp.MessageTip;
using BrowLib;
using CKVisionAppNet;
using ComponentFactory.Krypton.Toolkit;
using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace GSP.UI
{
    public partial class UI_MachineTest : UserControl
    {
        private int Selectidex;

        private int Selectidex1;

        private FlowControl Flow = new FlowControl();

        private IntPtr ImageView = IntPtr.Zero;
        private IntPtr ImageView1 = IntPtr.Zero;

        public UI_MachineTest()
        {
            InitializeComponent();
        }

       ~UI_MachineTest()
        {
            Global.VisionApp.ProcEndEvent -= new EventHandler(RefImage);
            Global.VisionApp.ViewDispose(ImageView);
        }
        public void CpkDgvIni(int Count)
        {
            this.CpkDgv.Rows.Clear();
            {
                for (int i = 0; i < Count; i++)
                {
                    this.CpkDgv.Rows.Add(this.CpkDgv.Rows.Count, 0, 0, 0, 0);
                }
            }
        }

        public void CompensationDgvIni(int Count)
        {
            if (this.CompensationDgv.Rows.Count >= Count) return;
            this.CompensationDgv.Rows.Clear();

            for (int i = 0; i < Count; i++)
            {
                this.CompensationDgv.Rows.Add(this.CpkDgv.Rows.Count, 0, 0, 0, 0);
            }

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

        private void CompensatorPointsToDgv()
        {
            this.CompensationDgv.Rows.Clear();
            try
            {
                foreach (var item in Global.coordinateCompensator.Points)
                {
                    this.CompensationDgv.Rows.Add(this.CompensationDgv.Rows.Count, item.X, item.Y, item.OffsetX, item.OffsetY);
                }
            }
            catch { }
           
        }

        private void Save_Btn_Click(object sender, EventArgs e)
        {
            BrowLib.FileClass.DataCsv Csv = new BrowLib.FileClass.DataCsv();
            if ( BrowApp.APP.Tip.ShowTip(0, "提示".tr(), "确定要保存参数吗？".tr(),  "确定".tr(), "取消".tr()) != 1) { return; }
            {
                try
                {
                    Global.Systemdata.AgingTable = Csv.dvtodt(this.TPosDgv);
                    DatatableIni(this.TPosDgv, Global.Systemdata.AgingTable);
                    this.TPosDgv.Rows[Selectidex].Selected = true;
                    this.TPosDgv.BindingContext[this.TPosDgv.DataSource].Position = Selectidex;
                    Global.CalibData.Save();
                    BrowApp.MessageTip.FloatingTip.ShowOk("参数保存成功".tr());
                }
                catch
                {
                    BrowApp.MessageTip.FloatingTip.ShowOk("参数保存失败".tr());
                }
            }
        }

        private void UI_MachineTest_Load(object sender, EventArgs e)
        {
            DatatableIni(TPosDgv, Global.Systemdata.AgingTable);

            CompensatorPointsToDgv();

            ImageView = Global.VisionApp.CreateView(this.View_Pic.Handle, this.View_Pic.Width, this.View_Pic.Height, 1);
            Global.VisionApp.SetView(ImageView, "Task8", "轮廓匹配", "View_Pic");
            ImageView1 = Global.VisionApp.CreateView(this.View_Pic1.Handle, this.View_Pic1.Width, this.View_Pic1.Height, 1);
            Global.VisionApp.SetView(ImageView1, "Task8", "轮廓匹配", "View_Pic1");
            Global.VisionApp.ProcEndEvent += new EventHandler(RefImage);
        }
        void RefImage(object sender, EventArgs e)
        {
            try
            {
                switch (sender)
                {
                    case "Task8":
                        Global.VisionApp.RedrawView(ImageView);
                        Global.VisionApp.RedrawView(ImageView1);
                        break;
                }
            }
            catch { }
        }
        private void Add_btn1_Click(object sender, EventArgs e)
        {
            try
            {
                string X = Global.X轴.GetPrfPos().ToString();
                string Y = Global.Y轴.GetPrfPos().ToString();
                string Z = Global.Z轴.GetPrfPos().ToString();
                string R = Global.R轴.GetPrfPos().ToString();

                Global.Systemdata.AgingTable.Rows.Add(this.TPosDgv.Rows.Count.ToString(), X, Y, Z, R);

                Selectidex = this.TPosDgv.Rows.Count - 1;
                this.TPosDgv.Rows[Selectidex].Selected = true;
                this.TPosDgv.BindingContext[this.TPosDgv.DataSource].Position = Selectidex;
            }
            catch { }
        }

        private void Delete_Btn1_Click(object sender, EventArgs e)
        {
            try
            {
                
                Global.Systemdata.AgingTable.Rows.RemoveAt(this.Selectidex);
                this.Selectidex = Selectidex - 1;
            }
            catch { }
        }

        private void Edit_Btn1_Click(object sender, EventArgs e)
        {
            try
            {
                TPosDgv.Rows[Selectidex].Cells[1].Value = X_num.Value.ToString();
                TPosDgv.Rows[Selectidex].Cells[2].Value = Y_num.Value.ToString();
                TPosDgv.Rows[Selectidex].Cells[3].Value = Z_num.Value.ToString();
                TPosDgv.Rows[Selectidex].Cells[4].Value = R_num.Value.ToString();

            }
            catch { }
        }


        private void StopRun_btn_Click(object sender, EventArgs e)
        {
            APP.Log.I_Log("停止老化测试");
            Global.StopFlag = true;
            Global.MachineState = GEnumEx.MachineState.MachineStop;
            Global.SystemRun = false;
        }

        private void Start_btn_Click(object sender, EventArgs e)
        {
            if (!Global.SystemInitialOk)
            {
                APP.Tip.ShowTip(1, "警告".tr(), "设备未归零,请先归零再执行操作".tr(), "确定".tr());
                return;
            }
            if (Flow.IsManualRun()) { return; }
            WaitFrm waitFrm = new WaitFrm(Start_btn);
            waitFrm.Enabled=true;
            Global.StopFlag = false;
            Global.SystemRun = true;
            Global.MachineState = GEnumEx.MachineState.MachineRuning;
            APP.Log.I_Log("开始老化测试");
            int num = Convert.ToInt32(Test_num.Value);
            Flow.ExecuteManual(() =>
            {
                Flow.Agingtest(num, (j) =>
                {
                    Invoke(new Action(() =>
                    {
                        TestNum_lab.Text = j.ToString();
                    }));
                });
                Invoke(new Action(() =>
                {
                    waitFrm.Enabled = false;
                }));
            });
        }
        private void TPosDgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < this.TPosDgv.RowCount)
            {

                try
                {
                    this.Selectidex = e.RowIndex;
                    X_num.Value = decimal.Parse(TPosDgv.Rows[Selectidex].Cells[1].Value.ToString());
                    Y_num.Value = decimal.Parse(TPosDgv.Rows[Selectidex].Cells[2].Value.ToString()); ;
                    Z_num.Value = decimal.Parse(TPosDgv.Rows[Selectidex].Cells[3].Value.ToString());
                    R_num.Value = decimal.Parse(TPosDgv.Rows[Selectidex].Cells[4].Value.ToString());
                }
                catch { }
            }
        }

        private void Edit_Mode_Click(object sender, EventArgs e)
        {
            CreateMode createMode = new CreateMode();
            createMode.TaskName = "Task8";
            createMode.ShowDialog();
        }

        private void Start_Text_Click(object sender, EventArgs e)
        {
            if (!Global.SystemInitialOk)
            {
                APP.Tip.ShowTip(1, "警告".tr(), "设备未归零,请先归零再执行操作".tr(), "确定".tr());
                return;
            }
            if (Flow.IsManualRun()) { return; }
            WaitFrm waitFrm = new WaitFrm(Start_Text);
            waitFrm.Enabled = true;
            Global.StopFlag = false;
            Global.SystemRun = true;
            Global.MachineState = GEnumEx.MachineState.MachineRuning;
            APP.Log.I_Log("开始重复精度测试");
            int Axisid = 1;
            if (X_ck.Checked)
            {
                Axisid = 1;
            }
            else if (Y_ck.Checked)
            {
                Axisid = 2;
            }
            else if (Z_ck.Checked)
            {
                Axisid = 3;
            }
            else if (R_ck.Checked)
            {
                Axisid = 4;
            }
            int num = Convert.ToInt32(Conut_num.Value);
            CpkDgvIni(num);
            double StartPos = Convert.ToDouble(StartPos_Num.Value);
            double Spd = Convert.ToDouble(Spd_num.Value);
            double Acc = Convert.ToDouble(Acc_num.Value);
            double offset= Convert.ToDouble(Offset_num.Value);
            int daley= Convert.ToInt32(Dlay_num.Value);
            Flow.ExecuteManual(() =>
            {
                Flow.RepeatTest(num, StartPos, Spd, Acc, offset, daley ,Axisid,(j,Pos) =>
                {
                    Invoke(new Action(() =>
                    {
                        Num_lab.Text= j.ToString();
                        this.CpkDgv.Rows[j - 1].Cells[Axisid].Value = Pos.ToString("F3");
                    }));
                });
                Invoke(new Action(() =>
                {
                    waitFrm.Enabled = false;
                }));
            });
        }

        private void learn_Btn_Click(object sender, EventArgs e)
        {
            if (X_ck.Checked)
            {
                StartPos_Num.Value = decimal.Parse(Global.X轴.GetPrfPos().ToString());
            }
            else if (Y_ck.Checked)
            {
                StartPos_Num.Value = decimal.Parse(Global.Y轴.GetPrfPos().ToString());
            }
            else if (Z_ck.Checked)
            {
                StartPos_Num.Value = decimal.Parse(Global.Z轴.GetPrfPos().ToString());
            }
            else if (R_ck.Checked)
            {
                StartPos_Num.Value = decimal.Parse(Global.R轴.GetPrfPos().ToString());
            }
        }

        private void Export_btn_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();

            for (int i = 0; i < CpkDgv.Columns.Count; i++)
            {
                Type type = CpkDgv.Columns[i].HeaderText.GetType();
                dt.Columns.Add(CpkDgv.Columns[i].HeaderText, type);
            }

            for (int i = 0; i < CpkDgv.Rows.Count; i++)
            {
                string[] strings = new string[CpkDgv.Columns.Count];
                for (int j = 0; j < CpkDgv.Columns.Count; j++)
                {
                    strings[j] = CpkDgv.Rows[i].Cells[j].FormattedValue.ToString();
                }
                dt.Rows.Add(strings);
            }

            NOPIHelper.ExportDataToExcel(dt);
        }

        private void ComputeBtn_Click(object sender, EventArgs e)
        {

        }

        private void AddPoint_Click(object sender, EventArgs e)
        {
            string X = Global.X轴.GetPrfPos().ToString();
            string Y = Global.Y轴.GetPrfPos().ToString();
            Point_X.Value=decimal.Parse(X);
            Point_Y.Value = decimal.Parse(Y);
            this.CompensationDgv.Rows.Add(this.CompensationDgv.Rows.Count,
            X, Y, Point_DX.Value.ToString(), Point_DY.Value.ToString());

        }

      
        private void DelPoint_Click(object sender, EventArgs e)
        {
            try
            {
              this.CompensationDgv.Rows.RemoveAt(this.Selectidex1);
              this.Selectidex1 = Selectidex1 - 1;
            }
            catch { }
        }
        private void EditPoint_Click(object sender, EventArgs e)
        {
            try
            {
                CompensationDgv.Rows[Selectidex1].Cells[1].Value = Point_X.Value.ToString();
                CompensationDgv.Rows[Selectidex1].Cells[2].Value = Point_Y.Value.ToString();
                CompensationDgv.Rows[Selectidex1].Cells[3].Value = Point_DX.Value.ToString();
                CompensationDgv.Rows[Selectidex1].Cells[4].Value = Point_DY.Value.ToString();

            }
            catch { }
        }

        private void Save_Compensationlist_Click(object sender, EventArgs e)
        {
            try
            {
                Global.coordinateCompensator.Clear();
                for (int i = 0; i < this.CompensationDgv.Rows.Count; i++)
                {
                    double X = Convert.ToDouble(this.CompensationDgv.Rows[i].Cells[1].Value);
                    double Y = Convert.ToDouble(this.CompensationDgv.Rows[i].Cells[2].Value);


                    double dX = Convert.ToDouble(this.CompensationDgv.Rows[i].Cells[3].Value);
                    double dY = Convert.ToDouble(this.CompensationDgv.Rows[i].Cells[4].Value);
                   
                    Global.coordinateCompensator.AddPoint(X, Y, dX, dY);
                    Global.coordinateCompensator.SaveToCsv(Global.GlabPath + "/Compensator.Csv");
                    BrowApp.MessageTip.FloatingTip.ShowOk("保存成功".tr());
                }
            }
            catch
            {
                BrowApp.MessageTip.FloatingTip.ShowOk("保存失败".tr());
            }
        }

        private void Verification_Btn_Click(object sender, EventArgs e)
        {
            double X = Convert.ToDouble(this.Xpos.Value);
            double Y = Convert.ToDouble(this.Ypos.Value);
            Tuple<double,double> tuple= Global.coordinateCompensator.GetCompensatedPosition(X, Y);
            NXpos.Value = decimal.Parse(tuple.Item1.ToString());
            NYpos.Value = decimal.Parse(tuple.Item2.ToString());

            if (!Global.SystemInitialOk)
            {
                APP.Tip.ShowTip(0, "警告".tr(), "设备未归零,请先归零再执行操作!!!", "确定".tr());
                return;
            }
            if (Flow.IsManualRun()) { return; }
            WaitFrm wait = new WaitFrm(Verification_Btn, "定位中,请稍后");
            wait.Enabled = true;
            Flow.ExecuteManual(() =>
            {
                new HandFlow().SafeMoveXyz(0, (double)NXpos.Value, (double)NYpos.Value, 1);

                Invoke(new Action(() =>
                {
                    wait.Enabled = false;
                }));
            });
        }

        private void CompensationDgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < this.CompensationDgv.RowCount)
            {

                try
                {
                    this.Selectidex1 = e.RowIndex;
                    Point_X.Value = decimal.Parse(CompensationDgv.Rows[Selectidex1].Cells[1].Value.ToString());
                    Point_Y.Value = decimal.Parse(CompensationDgv.Rows[Selectidex1].Cells[2].Value.ToString()); ;
                    Point_DX.Value = decimal.Parse(CompensationDgv.Rows[Selectidex1].Cells[3].Value.ToString());
                    Point_DY.Value = decimal.Parse(CompensationDgv.Rows[Selectidex1].Cells[4].Value.ToString());
                }
                catch { }
            }
        }

        private void Teach__Click(object sender, EventArgs e)
        {
            string X = Global.X轴.GetPrfPos().ToString();
            string Y = Global.Y轴.GetPrfPos().ToString();
            if (X_rad.Checked)
            {
                StartPos_.Value = decimal.Parse(X);
            }
            else if (Y_rad.Checked)
            {
                StartPos_.Value = decimal.Parse(Y);
            }
            

        }

        private void Goto__Click(object sender, EventArgs e)
        {

        }

        private void EMode_Click(object sender, EventArgs e)
        {
            CreateMode createMode = new CreateMode();
            createMode.TaskName = "Task8";
            createMode.ShowDialog();
        }

        private void Calibration_Run_Click(object sender, EventArgs e)
        {
            if (!Global.SystemInitialOk)
            {
                APP.Tip.ShowTip(1, "警告".tr(), "设备未归零,请先归零再执行操作".tr(), "确定".tr());
                return;
            }
            if (Flow.IsManualRun()) { return; }
            WaitFrm waitFrm = new WaitFrm(Calibration_Run);
            waitFrm.Enabled = true;
            Global.StopFlag = false;
            Global.SystemRun = true;
            Global.MachineState = GEnumEx.MachineState.MachineRuning;
            APP.Log.I_Log("开始标定扫描");
            int Axisid = 1;
            if (X_rad.Checked)
            {
                Axisid = 1;
            }
            else if (Y_rad.Checked)
            {
                Axisid = 2;
            }
            int num = Convert.ToInt32(Number_.Value);
            CompensationDgvIni(num);
            double StartPos = Convert.ToDouble(StartPos_.Value);
            double Spd = Convert.ToDouble(Spd_.Value);
            double Acc = Convert.ToDouble(Acc_.Value);
            double offset = Convert.ToDouble(Interval_.Value);
            int daley = Convert.ToInt32(delay_.Value);
            Flow.ExecuteManual(() =>
            {
                Flow.AutoCalibrationFlow(num, StartPos, Spd, Acc, offset, daley, Axisid, (j, Pos) =>
                {
                    Invoke(new Action(() =>
                    {
                        Numlab_.Text = j.ToString();
                        if (X_rad.Checked)
                        {
                            this.CompensationDgv.Rows[j - 1].Cells[1].Value = StartPos+ offset * j;
                            //this.CompensationDgv.Rows[j - 1].Cells[2].Value = 0;
                            this.CompensationDgv.Rows[j - 1].Cells[3].Value = Pos.ToString("F3");
                            //this.CompensationDgv.Rows[j - 1].Cells[4].Value = 0;
                        }
                        else if (Y_rad.Checked)
                        {
                            //this.CompensationDgv.Rows[j - 1].Cells[1].Value = 0;
                            this.CompensationDgv.Rows[j - 1].Cells[2].Value = StartPos+ offset * j;
                            //this.CompensationDgv.Rows[j - 1].Cells[3].Value =0;
                            this.CompensationDgv.Rows[j - 1].Cells[4].Value = Pos.ToString("F3");
                        }
                       
                    }));
                });
                Invoke(new Action(() =>
                {
                    waitFrm.Enabled = false;
                }));
            });
        }
    }
}
