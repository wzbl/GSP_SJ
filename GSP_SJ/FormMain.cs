using BorwinAnalyse.Forms;
using BrowApp;
using BrowApp.Alarm;
using BrowApp.CustomControl;
using BrowApp.Language;
using BrowApp.MessageTip;
using BrowLib;
using CKVisionAppNet;
using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Toolkit;
using GSP;
using GSP.Mes;
using GSP.UI;
using GSP_SJ.DBForm;
using GSP_SJ.ModelClass;
using GSP_SJ.Properties;
using SqlHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GSP.Mes.MesData;
using static System.Net.Mime.MediaTypeNames;

namespace GSP_SJ
{
    public partial class FormMain : KryptonForm
    {
        
        public FormMain()
        {
            InitializeComponent();
            this.Shown += FormMain_Shown;
            IniButtonMenuItems();
            RefAuthority("");
            Global.VisionApp.VisionAppIni();
            Global.VisionApp.CreationProject();
            Global.TcpClass.TcpMessageReceived += RefReceive;
            Global.UserHandler += new EventHandler(RefUser);
            // 3. 创建委托实例
            AlarmHelper.AddEvent += UpAlam;

            timer = new System.Windows.Forms.Timer();
            timer.Interval = 20;
            timer.Tick += timer1_Tick;
            //timer.Start();
            this.Load+= FormMain_Load;
            this.FormClosing += BrowForm_FormClosing;
        }

        /// <summary>
        /// 关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (KryptonMessageBox.Show("确定要关闭软件吗？".tr(), "提示".tr(), MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                Global.VisionApp.VisionAppClose();
                BrowLib.Controller.CardAPI.Servoff();
                this.Dispose();
                System.Environment.Exit(Environment.ExitCode);
            }
            else
            {
                e.Cancel = true;
            }
        }


        private void FormMain_Load(object sender, EventArgs e)
        {
           
            State_mes.TextLine1 = "系统未初始化？";
            State_mes.StateNormal.TextColor = Color.DarkOrange;
       
            Flow.StartThread();
            BrowApp.Language.Language.Instance.UpdateLanguage(this, null);
            Global.GlobRefEvent?.Invoke();
            BrowLib.Controller.SetLimit(false);
            Global.VisionApp.SetLanguage("eng");
            Task.Run(() => {
                bool RE = Global.VisionApp.LoadProject(@"./VisionProject/VisionPro.proj");
                APP.Log.I_Log("加载视觉方案{" + RE.ToString() + "}");
            });
            bool result = Global.TcpClass.TCPini("127.0.0.1", 8000, "Tcp");
            APP.Log.I_Log("FAI通讯连接{" + result.ToString() + "}");
        }

        System.Windows.Forms.Timer timer;
        private int Btimes;
        #region 定时器
        private KTimer KTimer = new KTimer();
        private KTimer MesTimer = new KTimer();
        private KTimer BuzzerTimer = new KTimer();
        private KTimer FlickerTimer = new KTimer();
        private KTimer Mes_Timer = new KTimer();
        FlowControl Flow = new FlowControl();
        Algorithm Algorithm = new Algorithm();
        private MarkForm markFrm;
        private int States = -1;//停机
        #endregion

        #region 系统Timer事件
        private void timer1_Tick(object sender, EventArgs e)
        {
            Flicker();//状态闪烁
            UpdataPram();
            Upstates();

            #region 轴位置刷新
            Xpos_lab.TextLine2 = Global.X轴.GetPrfPos().ToString("F2") + "mm";
            Ypos_lab.TextLine2 = Global.Y轴.GetPrfPos().ToString("F2") + "mm";
            Zpos_lab.TextLine2 = Global.Z轴.GetPrfPos().ToString("F2") + "mm";
            Rpos_lab.TextLine2 = Global.R轴.GetPrfPos().ToString("F2") + "mm";
            #endregion

            #region 用户权限管理
            //if (Global.Authority == null)
            //{
            //    User_lab.TextLine1 = "未登录".tr(); ;
            //}
            //else
            //{
            //    User_lab.TextLine1 = Global.UserName;
            //}
            #endregion

            #region 刷新系统使用资源
            if (KTimer.IsOn(3000))
            {
                //CPU_Lab.TextLine1 = (CpuCounter.NextValue() / 10).ToString("0.0") + "%";
                //RAM_lab.TextLine1 = (ramCounter.NextValue() / 1024 / 1024).ToString("0.0") + "MB";
                KTimer.Restart();
            }
            #endregion

            #region 报警状态刷新
            //if(BrowLib.Controller.InPort["气压监控_IN"].IsOff(50)&& Global.Is_Airpressure)
            //{
            //    APP.Alarm.Show("Styem", "9002", "气压过低", "I", 500, "请检查总气压阀气压是否异常");
            //}
            if (BrowLib.Controller.InPort["急停_IN"].IsOff(50))
            {
                APP.Alarm.Show("9001");
                Global.StopFlag = true;
                Global.PauseFlag = false;
                Global.SystemRun = false;
                Global.OrbModel = 1;
                Global.MachineState = GEnumEx.MachineState.MachineStop;
                BrowLib.Controller.CardAPI.StopAxis();
                //Global.VisionApp.StopRunProc("Task5");

            }
            if (!Global.IsRunState)
            {
                if ((BrowLib.Controller.InPort["安全门_IN"].IsOff(100)) && Global.Systemdata.IsSafeDoor)
                {
                    APP.Alarm.Show("S01011");
                }
            }
            if (APP.Alarm.AlarmFlag)//报警状态
            {
                Global.AlarmFlag = true;
                Global.PauseFlag = true;

      
                RefSystemState(0);
                if (Global.MachineState == GEnumEx.MachineState.MachineRuning)
                {
                    Global.TcpClass.Send("M:Error");
                    Global.MachineState = GEnumEx.MachineState.MachineError;
                }
                BrowLib.Controller.OutPort["蜂鸣器_OUT"].On();
                BrowLib.Controller.OutPort["三色灯红色_OUT"].On();
                BrowLib.Controller.OutPort["三色灯黄色_OUT"].Off();
                BrowLib.Controller.OutPort["三色灯绿色_OUT"].Off();
                States = 1;//报警
            }
            else
            {
                if (Global.OrbModel != 3)
                {
                    Global.AlarmFlag = false;
                    if (Global.MachineState == GEnumEx.MachineState.MachineError)
                    {
                        if (Global.PauseFlag)
                        {
                            Global.MachineState = GEnumEx.MachineState.MachinePause;
                        }
                    }
              
                    switch (Global.MachineState)
                    {
                        case GEnumEx.MachineState.MachineRuning:
                            if (Global.IsRunState)
                            {
                                if ((BrowLib.Controller.InPort["安全门_IN"].IsOff(50)) && Global.Systemdata.IsSafeDoor)
                                {
                                    APP.Alarm.Show("S01011");
                                }
                            }
                            if (G_Cbtn.Checked != true)
                            {
                                Global.strMachineState = "设备运行中";
                                RefSystemState(1);
                                BrowLib.Controller.OutPort["蜂鸣器_OUT"].Off();
                                BrowLib.Controller.OutPort["三色灯红色_OUT"].Off();
                                BrowLib.Controller.OutPort["三色灯黄色_OUT"].Off();
                                BrowLib.Controller.OutPort["三色灯绿色_OUT"].On();
                            }
                            break;
                        case GEnumEx.MachineState.MachinePause:
                            if (Y_Cbtn.Checked != true)
                            {
                                Global.strMachineState = "设备暂停中";
                                RefSystemState(3);
                                BrowLib.Controller.OutPort["蜂鸣器_OUT"].Off();
                                BrowLib.Controller.OutPort["三色灯红色_OUT"].Off();
                                BrowLib.Controller.OutPort["三色灯黄色_OUT"].On();
                                BrowLib.Controller.OutPort["三色灯绿色_OUT"].Off();

                                Global.TcpClass.Send("M:Pause");
                            }
                            break;
                        case GEnumEx.MachineState.MachineInitialize:
                            if (Y_Cbtn.Checked != true)
                            {
                                RefSystemState(2);
                                Global.strMachineState = "设备初始化中";

                                BrowLib.Controller.OutPort["蜂鸣器_OUT"].Off();
                                BrowLib.Controller.OutPort["三色灯红色_OUT"].Off();
                                BrowLib.Controller.OutPort["三色灯黄色_OUT"].On();
                                BrowLib.Controller.OutPort["三色灯绿色_OUT"].On();

                            }
                            break;
                        case GEnumEx.MachineState.MachineIdle:
                            if (G_Cbtn.Checked != true)
                            {
                                RefSystemState(4);
                                Global.strMachineState = "设备待机中";
                                BrowLib.Controller.OutPort["蜂鸣器_OUT"].Off();
                                BrowLib.Controller.OutPort["三色灯红色_OUT"].Off();
                                BrowLib.Controller.OutPort["三色灯黄色_OUT"].Off();
                                BrowLib.Controller.OutPort["三色灯绿色_OUT"].On();
                                States = 2;//待机
                            }
                            break;
                        case GEnumEx.MachineState.MachineStop:
                            if (Y_Cbtn.Checked != true)
                            {
                                Global.strMachineState = "设备停止中>>>";
                                RefSystemState(5);
                                BrowLib.Controller.OutPort["蜂鸣器_OUT"].Off();
                                BrowLib.Controller.OutPort["三色灯红色_OUT"].Off();
                                BrowLib.Controller.OutPort["三色灯黄色_OUT"].On();
                                BrowLib.Controller.OutPort["三色灯绿色_OUT"].Off();
                                States = 0;//停机
                            }
                            break;
                    }
                }
                else
                {
                    Global.strMachineState = "过板模式运行中";
                    RefSystemState(6);
                    BrowLib.Controller.OutPort["蜂鸣器_OUT"].Off();
                    BrowLib.Controller.OutPort["三色灯红色_OUT"].Off();
                    BrowLib.Controller.OutPort["三色灯黄色_OUT"].Off();
                    BrowLib.Controller.OutPort["三色灯绿色_OUT"].On();
                }
            }
            #endregion

            #region  蜂鸣器
            if (Global.Buzzerflag)
            {
                Btimes++;
                BrowLib.Controller.OutPort["蜂鸣器_OUT"].On();
                if (Btimes >= Global.BuzzeDely / 50)
                {
                    BrowLib.Controller.OutPort["蜂鸣器_OUT"].Off();
                    Global.Buzzerflag = false;
                    Btimes = 0;
                }
            }
            #endregion
        }
        #endregion

        private int _States = 0;//停机
        private void Upstates()
        {

            if (_States != States)
            {
                _States = States;

                string StatusID = "STOP";

                switch (_States)
                {
                    case 0:
                        StatusID = "STOP";
                        break;
                    case 1:
                        StatusID = "FAULT";
                        break;

                    case 2:
                        StatusID = "IDLE";
                        break;

                    case 3:
                        StatusID = "RUN";
                        break;
                }

                if (Global.mesConfig.Ulr1_ck)
                {
                    Task.Run(() =>
                    {

                        try
                        {
                            BrowLib.FileClass.RwIni iniFile = new BrowLib.FileClass.RwIni(Global.Systemdata.CfgFile, "", Encoding.Default);
                            string UserID = iniFile.Read("AutoFAI", "UserID", "AutoFAI", "UTF-8");
                            string Updata;
                            string Result = new MesData().UpDataAPI_S(UserID, StatusID, out Updata);
                            APP.Log.M_Log("Mes上传" + Updata);
                            APP.Log.M_Log("Mes回传" + Result);
                        }
                        catch { }
                    });
                }
            }
        }
        private void UpdataPram()
        {
            if (Mes_Timer.IsOn(60 * 1000 * 5) && Global.mesConfig.Ulr3_ck)
            {
                //上传参数
                try
                {
                    string UserID = "";
                    List<ParmType> parms = new List<ParmType>();
                    parms.Add(new ParmType
                    {
                        param = "高度补偿",
                        value = Global.Parm.Hoffset.ToString(),
                        unitNo = "",
                        time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    });
                    parms.Add(new ParmType
                    {
                        param = "板长",
                        value = Global.Parm.PcbLong.ToString(),
                        unitNo = "",
                        time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    });
                    parms.Add(new ParmType
                    {
                        param = "板宽",
                        value = Global.Parm.PcbHight.ToString(),
                        unitNo = "",
                        time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    });
                    string Updata;
                    string Result = new MesData().UpDataAPI_Pam(UserID, parms, out Updata);
                    APP.Log.M_Log("", UserID, "Mes上传:" + Updata);
                    APP.Log.M_Log("", UserID, "Mes回传:" + Result);
                }
                catch { }

                Mes_Timer.Restart();
            }
        }

        /// <summary>
        /// 状态闪烁
        /// </summary>
        private void Flicker()
        {
            if (FlickerTimer.IsOn(500))
            {
                if (R_Cbtn.Checked) { R_Cbtn.Enabled = !R_Cbtn.Enabled; }
                if (G_Cbtn.Checked) { G_Cbtn.Enabled = !G_Cbtn.Enabled; }
                if (Y_Cbtn.Checked) { Y_Cbtn.Enabled = !Y_Cbtn.Enabled; }
                FlickerTimer.Restart();
            }
        }
        void UpAlam(object sends, EventArgs e)
        {
            string Message = ((BrowApp.Alarm.AlarmHelper.AlarmType)sends).E_Message;
            string E_Code = ((BrowApp.Alarm.AlarmHelper.AlarmType)sends).E_Code;
            string alertID = Global.mesConfig.equipmentNo + DateTime.Now.ToString("yyyyMMddhhmmss");
            if (Global.mesConfig.Ulr2_ck)
            {
                Task.Run(() => {
                    try
                    {
                        BrowLib.FileClass.RwIni iniFile = new BrowLib.FileClass.RwIni(Global.Systemdata.CfgFile, "", Encoding.Default);
                        string UserID = iniFile.Read("AutoFAI", "UserID", "AutoFAI", "UTF-8");
                        string TokenID = iniFile.Read("AutoFAI", "TokenID", "AutoFAI", "UTF-8");
                        string Updata;
                        string Result = new MesData().UpDataAPI_Alm(UserID, E_Code, alertID, Message, TokenID, out Updata);
                        APP.Log.M_Log("Mes上传" + Updata);
                        APP.Log.M_Log("Mes回传" + Result);
                    }
                    catch { }
                });
            }
        }
        void RefReceive(object sends, string message)
        {
            new FlowControl().RefreshThreadUI(this, () =>
            {
                string[] GetMes = message.Split(':');
                if (GetMes[0] == "M")
                {
                    switch (GetMes[1])
                    {
                        case "ClearAlarm":
                            APP.Alarm.Clear();//报警清除
                            break;
                        case "刷新BOM":
                            Global.RefFormHandler?.Invoke(this, new EventArgs());
                            Global.Authority = Global.ReadAuthority();
                            Global.TcpClass.Send("M:刷新BOM_OK");
                            break;
                        case "刷新配方":
                            RefRecipe(Convert.ToInt32(GetMes[2]));
                            Global.TcpClass.Send("M:刷新配方_OK");
                            Global.Hoffset = Convert.ToDouble(GetMes[3]);
                            break;
                        case "模式切换_OFF":
                            BrowLib.Controller.OutPort["自动切换手动"].Off();
                            Global.TcpClass.Send("M:模式切换_OFF_OK");
                            break;
                        case "模式切换_ON":
                            BrowLib.Controller.OutPort["自动切换手动"].On();
                            BrowLib.Controller.OutPort["四线切换两线"].On();
                            Global.TcpClass.Send("M:模式切换_ON_OK");
                            break;
                        case "用户登出":
                            Global.Password = null;
                            Global.Authority = null;
                            Global.UserName = null;
                            Global.Hoffset = 0;
                            break;
                        case "Flow_In":
                            In_Click();
                            break;
                        case "Flow_OUT":
                            Out_Click();
                            break;
                        case "四线测量":
                            BrowLib.Controller.OutPort["四线切换两线"].Off();
                            Global.TcpClass.Send("M:四线测量_OK");
                            break;
                        case "二线测量":
                            BrowLib.Controller.OutPort["四线切换两线"].On();
                            Global.TcpClass.Send("M:二线测量_OK");
                            break;
                        case "归零":
                            this.HomeBtn_Click(this, new EventArgs());
                            break;
                        case "开路清零":
                            double size = Global.GetSize("0201");//获取开口大小
                            Flow.ExecuteManual(() =>
                            {
                                new HandFlow().SafeMoveXYZR(200, 10, 0, Global.CalibData.ZeroPos.Xpos, Global.CalibData.ZeroPos.Ypos, Global.CalibData.ZeroPos.Zpos, 0, size);
                                Global.TcpClass.Send("M:开路清零就绪");
                            });
                            break;
                        case "短路清零":
                            double size2 = Global.GetSize("0201");//获取开口大小
                            Flow.ExecuteManual(() =>
                            {
                                new HandFlow().SafeMoveXYZR(200, 10, 0, Global.CalibData.ZeroPos2.Xpos, Global.CalibData.ZeroPos2.Ypos, Global.CalibData.ZeroPos2.Zpos, 0, size2);
                                Global.TcpClass.Send("M:短路清零就绪");
                            });
                            break;
                        case "短路清零完成":
                            new HandFlow().SafeMoveXyz(Global.Systemdata.StopPos.Zpos, Global.Systemdata.StopPos.Xpos, Global.Systemdata.StopPos.Ypos, Global.Systemdata.StopPos.Zpos);
                            break;
                        case "Picture"://拼图启动
                            Global.PauseFlag = false;
                            this.TopMost = true;
                            switch (Global.Model)
                            {
                                case 0:
                                    Global.Parm.PcbLong = Convert.ToDouble(GetMes[2]);
                                    Global.Parm.PcbHight = Convert.ToDouble(GetMes[3]);
                                    break;
                                case 1:
                                case 2:
                                case 3:
                                    Global.Parm.PcbLong = Convert.ToDouble(GetMes[3]);
                                    Global.Parm.PcbHight = Convert.ToDouble(GetMes[2]);
                                    break;
                            }
                            int mode = Convert.ToInt16(GetMes[4]);
                            PateSart(mode);
                            this.TopMost = false;
                            break;
                        case "Start":
                            Global.StopFlag = true;
                            Thread.Sleep(50);
                            if (GetMes[2] == "CCD")
                            {
                                Global.IsCcdMode = true;
                            }
                            else
                            {
                                Global.IsCcdMode = false;
                            }
                            Global.PauseFlag = false;
                            Start();
                            this.WindowState = FormWindowState.Minimized;
                            this.TopMost = false;
                            break;
                        case "Continue":
                            Global.PauseFlag = false;
                            Global.MachineState = GEnumEx.MachineState.MachineRuning;
                            break;
                        case "Stop":
                            Stop();
                            break;
                        case "SetWidth":
                            SetWidth(Convert.ToDouble(GetMes[2]));
                            break;
                        case "Mark":
                            Global.StopFlag = false;
                            if (Flow.IsManualRun()) { return; }
                            Flow.ExecuteManual(() =>
                            {
                                Flow.GoToMark();
                            });
                            break;
                        case "GoTo":
                            Global.StopFlag = false;
                            if (Flow.IsManualRun()) { return; }
                            Flow.ExecuteManual(() =>
                            {
                                Flow.M_goTo(GetMes[2], GetMes[3], GetMes[4]);
                            });
                            break;
                        case "CCD_GoTo":
                            if (Global.BomData.Rows.Count <= 0)
                            {
                                APP.Tip.ShowTip(0, "警告".tr(), "未导入数据".tr(), "确定".tr());
                                return;
                            }
                            if (!Global.IsRecipe)
                            {
                                APP.Tip.ShowTip(0, "警告".tr(), "无配方数据".tr(), "确定".tr());
                                return;
                            }
                            if (Global.AlarmFlag)
                            {
                                APP.Tip.ShowTip(0, "警告", "请先清除报警再执行操作".tr(), "确定".tr());
                                return;
                            }
                            Global.StopFlag = false;
                            if (Flow.IsManualRun()) { return; }
                            Flow.ExecuteManual(() =>
                            {
                                Flow.Mccd_Goto(GetMes[2], GetMes[3]);
                            });
                            break;
                        case "C_Mark":
                            switch (Global.Model)
                            {
                                case 0:
                                    Global.Parm.PcbLong = Convert.ToDouble(GetMes[11]);
                                    Global.Parm.PcbHight = Convert.ToDouble(GetMes[12]);
                                    break;
                                case 1:
                                case 2:
                                case 3:
                                    Global.Parm.PcbLong = Convert.ToDouble(GetMes[12]);
                                    Global.Parm.PcbHight = Convert.ToDouble(GetMes[11]);
                                    break;
                            }
                            Cmark(Convert.ToInt32(GetMes[7]), Convert.ToInt32(GetMes[8]), Convert.ToInt32(GetMes[4]), Convert.ToInt32(GetMes[5]));
                            if (markFrm == null || markFrm.IsDisposed)
                            {
                                markFrm = new MarkForm();
                                markFrm.RefProc();
                                markFrm.Show();
                                markFrm.Activate();

                            }
                            else
                            {
                                markFrm.RefProc();
                                markFrm.Activate();
                                markFrm.FomActivated();
                            }
                            markFrm.Mode = Convert.ToInt32(GetMes[2]);
                            markFrm.Code = GetMes[6];
                            switch (GetMes[3])
                            {
                                case "Mark1":
                                    markFrm.Bmark1_X = Convert.ToDouble(GetMes[9]);
                                    markFrm.Bmark1_Y = Convert.ToDouble(GetMes[10]);
                                    markFrm.Num = 1;
                                    break;
                                case "Mark2":
                                    markFrm.Bmark2_X = Convert.ToDouble(GetMes[9]);
                                    markFrm.Bmark2_Y = Convert.ToDouble(GetMes[10]);
                                    markFrm.Num = 2;
                                    break;
                            }
                            markFrm.RefMarkfrm();
                            break;
                        case "GoStopPos":
                            new HandFlow().SafeMoveXYZR(200, 20, 0, Global.Systemdata.StopPos.Xpos, Global.Systemdata.StopPos.Ypos, Global.Systemdata.StopPos.Zpos, 0);
                            break;
                        case "GoLaser":
                            if (Global.BomData.Rows.Count <= 0)
                            {
                                APP.Tip.ShowTip(0, "警告", "未导入数据".tr(), "确定".tr());
                                return;
                            }
                            if (!Global.IsRecipe)
                            {
                                APP.Tip.ShowTip(0, "警告".tr(), "无配方数据".tr(), "确定".tr());
                                return;
                            }
                            Global.StopFlag = false;
                            if (Flow.IsManualRun()) { return; }
                            Flow.ExecuteManual(() =>
                            {
                                Flow.Laser_Go(GetMes[2]);
                            });
                            break;
                        case "Up":
                            BrowLib.Controller.OutPort["顶升气缸_OUT"].On();
                            Global.TcpClass.Send("Up_OK");
                            break;
                        case "Down":
                            BrowLib.Controller.OutPort["顶升气缸_OUT"].Off();
                            Global.TcpClass.Send("Down_OK");
                            break;
                        case "Check":
                            if (BrowLib.Controller.OutPort["顶升气缸_OUT"].State())
                            {
                                Global.TcpClass.Send("S_Up");
                            }
                            else
                            {
                                Global.TcpClass.Send("S_Down");
                            }
                            break;
                        case "Graspimg":
                            if (Global.BomData.Rows.Count <= 0)
                            {
                                APP.Tip.ShowTip(0, "警告".tr(), "未导入数据".tr(), "确定");
                                return;
                            }
                            if (!Global.IsRecipe)
                            {
                                APP.Tip.ShowTip(0, "警告".tr(), "无配方数据".tr(), "确定".tr());
                                return;
                            }
                            Global.StopFlag = false;
                            if (Flow.IsManualRun()) { return; }
                            Flow.ExecuteManual(() =>
                            {
                                Flow.GraspImage(GetMes[2], Convert.ToDouble(GetMes[3]), Convert.ToDouble(GetMes[4]), Convert.ToDouble(GetMes[5]), Convert.ToInt32(GetMes[6]), Convert.ToInt32(GetMes[7]), Convert.ToInt32(GetMes[8]));
                            });
                            break;
                        case "AutoGraspimg":
                            if (Global.BomData.Rows.Count <= 0)
                            {
                                APP.Tip.ShowTip(0, "警告".tr(), "未导入数据".tr(), "确定".tr());
                                return;
                            }
                            if (!Global.IsRecipe)
                            {
                                APP.Tip.ShowTip(0, "警告".tr(), "无配方数据".tr(), "确定".tr());
                                return;
                            }
                            Flow.StartOCR();
                            break;

                        case "MesError":
                            APP.Alarm.Show("MES", "M0001", GetMes[2], "I", 10);
                            break;

                    }
                }
            });
        }
        private void RefRecipe(int mode)
        {
            string FBCCode, XYCode, BoardSide, ProductCode, ProductName;
            double X, Y;
            Global.ReadFAIConfig(out FBCCode, out XYCode, out BoardSide, out ProductCode, out ProductName);
            Global.RecipePatn = XYCode + BoardSide;
            Global.Parm.SetCode = Global.RecipePatn;
            Global.VisionApp.SetCodePath = Global.RecipePatn;
            if (mode == 0)
            {
                Global.Is_NoMark = true;
                bool Btn1 = Global.Parm.Read(ref Global.Parm);
                if (Btn1) { Global.IsRecipe = true; }
                else
                    Global.IsRecipe = false;
            }
            else if (mode == 1)
            {
                Global.Is_NoMark = false;
                bool Btn1 = Global.Parm.Read(ref Global.Parm);
                bool Btn2 = Global.VisionApp.ReadObj("Mark.proc", "Task3");
                if (Btn1 && Btn2) { Global.IsRecipe = true; }
                else
                    Global.IsRecipe = false;
            }

            GSP.VisionGlobal.bMak1_X = Global.Parm.BMak1Pos.Xpos;
            GSP.VisionGlobal.bMak1_Y = Global.Parm.BMak1Pos.Ypos;

            GSP.VisionGlobal.bMak2_X = Global.Parm.BMak2Pos.Xpos;
            GSP.VisionGlobal.bMak2_Y = Global.Parm.BMak2Pos.Ypos;

        }

        private void RefSystemState(int Type)
        {
            switch (Type)//报警中
            {
                case 0:
                    State_mes.TextLine1 = "系统报警中";
                    State_mes.StateNormal.TextColor = Color.Red;
                    R_Cbtn.Checked = true;
                    G_Cbtn.Checked = false;
                    Y_Cbtn.Checked = false;
                    break;
                case 1://运行中
                    State_mes.TextLine1 = "系统自动运行";
                    State_mes.StateNormal.TextColor = Color.Green;

                    R_Cbtn.Checked = false;
                    G_Cbtn.Checked = true;
                    Y_Cbtn.Checked = false;
                    break;
                case 2://初始化中
                    State_mes.TextLine1 = "系统初始化中";
                    State_mes.StateNormal.TextColor = Color.DarkOrange;

                    R_Cbtn.Checked = false;
                    G_Cbtn.Checked = false;
                    Y_Cbtn.Checked = true;
                    break;
                case 3://系统暂停中
                    State_mes.TextLine1 = "系统暂停中";
                    State_mes.StateNormal.TextColor = Color.DarkOrange;


                    R_Cbtn.Checked = false;
                    G_Cbtn.Checked = false;
                    Y_Cbtn.Checked = true;
                    break;
                case 4://系统待机中
                    State_mes.TextLine1 = "系统待机中";
                    State_mes.StateNormal.TextColor = Color.DarkOrange;


                    R_Cbtn.Checked = false;
                    G_Cbtn.Checked = false;
                    Y_Cbtn.Checked = true;
                    break;
                case 5://系统停止中
                    State_mes.TextLine1 = "系统停止中";
                    State_mes.StateNormal.TextColor = Color.DarkOrange;



                    R_Cbtn.Checked = false;
                    G_Cbtn.Checked = false;
                    Y_Cbtn.Checked = true;
                    break;
                case 6://过板模式运行中
                    State_mes.TextLine1 = "过板模式运行中";
                    State_mes.StateNormal.TextColor = Color.Green;




                    R_Cbtn.Checked = false;
                    G_Cbtn.Checked = true;
                    Y_Cbtn.Checked = false;
                    break;
            }

        }

        void RefUser(object sends, EventArgs e)
        {
            if (sends != null)
            {
                RefAuthority(sends.ToString());
            }
            else
            {
                RefAuthority("");
            }

        }

        /// <summary>
        ///启动拼图
        /// </summary>
        /// <param name="mode"></param>
        private void PateSart(int mode)
        {
            if (Global.PauseFlag == true)
            {
                if (!Global.AlarmFlag)
                {
                    Global.PauseFlag = false;
                    RefSystemState(1);
                    Global.MachineState = GEnumEx.MachineState.MachineRuning;
                    APP.Log.I_Log("暂停恢复启动");
                }
                else
                {
                    APP.Tip.ShowTip(1, "警告".tr(), "请先解除报警/再启动设备".tr(), "确定".tr());
                    Global.TcpClass.Send("M:PictureNG");
                    return;
                }
            }
            else
            {
                if (Global.SystemRun)
                {
                    return;
                }
                if (!Global.SystemInitialOk)
                {
                    APP.Tip.ShowTip(0, "警告".tr(), "自动运行前请先复位".tr(), "确定".tr());
                    Global.TcpClass.Send("M:PictureNG");
                    return;
                }
                if (Global.AlarmFlag)
                {
                    APP.Tip.ShowTip(0, "警告", "请先解除报警 / 再启动设备".tr(), "确定".tr());
                    Global.TcpClass.Send("M:PictureNG");
                    return;
                }
                else
                {
                    Global.StopFlag = false;
                    Global.SystemRun = true;
                    Global.MachineState = GEnumEx.MachineState.MachineRuning;
                    Global.TcpClass.Send("M:PictureStart");
                    if (Flow.IsManualRun()) { return; }
                    Flow.ExecuteManual(() =>
                    {
                        Flow.FlowPate(mode);
                    });
                }
            }
        }
        private void Start()
        {
            if (Global.PauseFlag == true)
            {
                if (!Global.AlarmFlag)
                {
                    Global.PauseFlag = false;
                    RefSystemState(1);
                    Global.MachineState = GEnumEx.MachineState.MachineRuning;
                    APP.Log.I_Log("暂停恢复启动");
                }
                else
                {
                    APP.Tip.ShowTip(0, "警告".tr(), "请先解除报警/再启动设备".tr(), "确定".tr());
                    Global.TcpClass.Send("M:StartNG");
                    return;
                }
            }
            else
            {
                if (Global.SystemRun)
                {
                    return;
                }
                if (!Global.SystemInitialOk)
                {
                    APP.Tip.ShowTip(0, "警告", "自动运行前请先复位".tr(), "确定");
                    Global.TcpClass.Send("M:StartNG");
                    return;
                }
                if (Global.AlarmFlag)
                {
                    APP.Tip.ShowTip(0, "警告".tr(), "请先解除报警 / 再启动设备".tr(), "确定".tr());
                    Global.TcpClass.Send("M:StartNG");
                    return;
                }
                if (Global.BomData.Rows.Count <= 0)
                {
                    APP.Tip.ShowTip(0, "警告".tr(), "未导入数据".tr(), "确定".tr());
                    Global.TcpClass.Send("M:StartNG");
                    return;
                }
                if (!Global.IsRecipe)
                {
                    APP.Tip.ShowTip(0, "警告", "无配方数据".tr(), "确定".tr());
                    Global.TcpClass.Send("M:RecipeNull");
                    return;
                }
                else
                {
                    
                    Flow.START();
                    Global.TcpClass.Send("M:StartOK");
                    States = 3;//运行
                }
            }
        }
        private void Puse()
        {
            if (Global.MachineState == GEnumEx.MachineState.MachineRuning)
            {
                Global.PauseFlag = true;
                Global.MachineState = GEnumEx.MachineState.MachinePause;
                Global.TcpClass.Send("M:Pause");
            }
        }
        private void Stop()
        {
            Global.StopFlag = true;
            Global.PauseFlag = false;
            Global.SystemRun = false;
            Global.OrbModel = 1;
            Global.MachineState = GEnumEx.MachineState.MachineStop;
            BrowLib.Controller.CardAPI.StopAxis();
            BrowLib.Controller.CardAPI.ClearSts();
            Global.VisionApp.StopRunProc("Task5");
            Global.TcpClass.Send("M:Stop");
        }

        private void SetWidth(double Width)
        {

            if (Width > Global.Systemdata.GdWight)
            {
                APP.Tip.ShowTip(0, "警告".tr(), "无法设置轨道宽度".tr(), "确定".tr());
                Global.TcpClass.Send("M:SetWidth_NG");
                return;
            }

            Flow.ExecuteManual(() =>
            {
                int re = new HandFlow().SetWidth(20, Global.Systemdata.GdWight - Width);
                if (re == 0)
                {
                    Global.TcpClass.Send("M:SetWidth_OK");

                }
                else
                {
                    Global.TcpClass.Send("M:SetWidth_NG");
                }
            });
        }

        private void Cmark(int Mpix_X, int Mpix_Y, int iwidth, int iHight)
        {
            VisionApp.CalibType Calib = new VisionApp.CalibType();
            Calib = Global.VisionApp.GetCalib(GSP.VisionGlobal.UpCailb_Obj);
            double Cen_X = 0, Cen_Y = 0, X, Y, width, hight;
            Algorithm.GetStartCenter_Pix(Global.Parm.PcbLong, Global.Parm.PcbHight, Global.Systemdata.CameRaview.Row,
            Global.Systemdata.CameRaview.Col, iwidth, iHight, out width, out hight);
            switch (Global.Model)
            {
                case 0://993
                    Global.VisionApp.PixToPos(Calib, (iwidth - width / 2), (hight / 2), out Cen_X, out Cen_Y);
                    break;
                case 1://991
                    Global.VisionApp.PixToPos(Calib, (iwidth - width / 2), (iHight - hight / 2), out Cen_X, out Cen_Y);
                    break;
                case 2: //860
                case 3://860P
                    Global.VisionApp.PixToPos(Calib, (width / 2), (hight / 2), out Cen_X, out Cen_Y);
                    break;
            }
            Global.VisionApp.PixToPos(Calib, Mpix_X, Mpix_Y, out X, out Y);
            double Dx = (X - Cen_X) * Global.Ratio * Global.ARatio;
            double Dy = (Y - Cen_Y) * Global.Ratio * Global.ARatio;
            new HandFlow().SafeMoveXyz(0, Global.Systemdata.PatlePos.Xpos - Dx,
           Global.Systemdata.PatlePos.Ypos - Dy, Global.CamHeight);
        }

        private void In_Click()
        {
            if (!Global.SystemInitialOk)
            {
                APP.Tip.ShowTip(0, "警告".tr(), "设备未归零,请先归零再执行操作".tr(), "确定".tr());
                Global.TcpClass.Send("M:Flow_In_NG");
                return;
            }
            Global.StopFlag = false;
            if (Flow.IsManualRun()) { return; }
            Flow.ExecuteManual(() =>
            {
                Flow.Flow_In();
            });
        }
        private void Out_Click()
        {
            if (!Global.SystemInitialOk)
            {
                APP.Tip.ShowTip(0, "警告".tr(), "设备未归零,请先归零再执行操作".tr(), "确定");
                Global.TcpClass.Send("M:Flow_OUT_NG");
                return;
            }
            Global.StopFlag = false;
            if (Flow.IsManualRun()) { return; }
            Flow.ExecuteManual(() =>
            {
                Flow.Flow_Out();
            });
        }
        private void FormMain_Shown(object sender, EventArgs e)
        {
            DBEventAction.ProguceItem += (x) =>
            {
                if (x != null)
                {

                    string text = x.产品编号;

                    if (kryptonNavigator1.Pages.Where(x1 => x1.Text == text).Count() == 0)
                    {
                        kryptonNavigator1.Pages.Add(GetKryptonPage(text));
                        UCProgram uCProgram = new UCProgram(x);
                        kryptonNavigator1.Pages[kryptonNavigator1.Pages.Count - 1].Controls.Add(uCProgram);
                        kryptonNavigator1.SelectedPage = kryptonNavigator1.Pages[kryptonNavigator1.Pages.Count - 1];
                    }
                    else
                    {
                        //所有程序
                        for (int i = 0; i < kryptonNavigator1.Pages.Count; i++)
                        {
                            if (kryptonNavigator1.Pages[i].Text == text)
                            {
                                kryptonNavigator1.SelectedPage = kryptonNavigator1.Pages[i];
                                break;
                            }
                        }
                    }

                }
            };

            DBEventAction.ReportItem += (x) =>
            {
                if (x != null)
                {
                    string text = x.ReportCode;
                    if (kryptonNavigator1.Pages.Where(x1 => x1.Text == text).Count() == 0)
                    {
                        kryptonNavigator1.Pages.Add(GetKryptonPage(text));
                        UCReport uCProgram = new UCReport(x);
                        kryptonNavigator1.Pages[kryptonNavigator1.Pages.Count - 1].Controls.Add(uCProgram);
                        kryptonNavigator1.SelectedPage = kryptonNavigator1.Pages[kryptonNavigator1.Pages.Count - 1];
                    }
                    else
                    {
                        //所有程序
                        for (int i = 0; i < kryptonNavigator1.Pages.Count; i++)
                        {
                            if (kryptonNavigator1.Pages[i].Text == text)
                            {
                                kryptonNavigator1.SelectedPage = kryptonNavigator1.Pages[i];
                                for (int j = 0; j < kryptonNavigator1.Pages[i].Controls.Count; j++)
                                {
                                    if (kryptonNavigator1.Pages[i].Controls[j] is UCReport)
                                    {
                                        (kryptonNavigator1.Pages[i].Controls[j] as UCReport).RefreshData(x);
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            };
           
        }


        #region 菜单项
        ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem MenuItem1;
        ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem MenuItem2;
        ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem MenuItem3;
        ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem MenuItem4;
        ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem MenuItem5;
        ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem MenuItem6;
        ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem MenuItem7;
        ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem MenuItem8;
        ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem MenuItem9;
        ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem MenuItem10;
        ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem MenuItem11;
        ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem MenuItem12;
        ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem MenuItem13;
        private void IniButtonMenuItems()
        {
            MenuItem1 = new KryptonContextMenuItem();
            MenuItem2 = new KryptonContextMenuItem();
            MenuItem3 = new KryptonContextMenuItem();
            MenuItem4 = new KryptonContextMenuItem();
            MenuItem5 = new KryptonContextMenuItem();
            MenuItem6 = new KryptonContextMenuItem();
            MenuItem7 = new KryptonContextMenuItem();
            MenuItem8 = new KryptonContextMenuItem();
            MenuItem9 = new KryptonContextMenuItem();
            MenuItem10 = new KryptonContextMenuItem();
            MenuItem11 = new KryptonContextMenuItem();
            MenuItem12 = new KryptonContextMenuItem();
            MenuItem13 = new KryptonContextMenuItem();

            MenuItem1.Image = Resources._35;
            MenuItem1.Text = "视觉管理".tr();
            this.MenuItem1.Click += new System.EventHandler(this.MenuItem1_Click);

            MenuItem2.Image = Resources.icons8_language_48;
            MenuItem2.Text = "语言管理".tr();
            this.MenuItem2.Click += new System.EventHandler(this.MenuItem2_Click);

            MenuItem3.Image = Resources.icons8_编程逻辑控制器_24;
            MenuItem3.Text = "系统调试".tr();
            this.MenuItem3.Click += new System.EventHandler(this.MenuItem3_Click);

            MenuItem4.Image = Resources._51;
            MenuItem4.Text = "系统自检".tr();
            this.MenuItem4.Click += new System.EventHandler(this.MenuItem4_Click);

            MenuItem5.Image = Resources._70;
            MenuItem5.Text = "关于".tr();
            this.MenuItem5.Click += new System.EventHandler(this.MenuItem5_Click);

            MenuItem6.Image = Resources._9;
            MenuItem6.Text = "更新日志".tr();
            this.MenuItem6.Click += new System.EventHandler(this.MenuItem6_Click);

            MenuItem7.Image = Resources.icons8_编程逻辑控制器_24;
            MenuItem7.Text = "用户管理".tr();
            this.MenuItem7.Click += new System.EventHandler(this.MenuItem7_Click);

            MenuItem8.Image = Resources._51;
            MenuItem8.Text = "站位表".tr();
            this.MenuItem8.Click += new System.EventHandler(this.MenuItem8_Click);

            MenuItem9.Image = Resources._70;
            MenuItem9.Text = "资料库".tr();
            this.MenuItem9.Click += new System.EventHandler(this.MenuItem9_Click);

            MenuItem10.Image = Resources._9;
            MenuItem10.Text = "尺寸管理".tr();
            this.MenuItem10.Click += new System.EventHandler(this.MenuItem10_Click);

            MenuItem11.Image = Resources.icons8_编程逻辑控制器_24;
            MenuItem11.Text = "电桥配置".tr();
            this.MenuItem11.Click += new System.EventHandler(this.MenuItem11_Click);

            MenuItem12.Image = Resources._51;
            MenuItem12.Text = "电桥参数".tr();
            this.MenuItem12.Click += new System.EventHandler(this.MenuItem12_Click);

            MenuItem13.Image = Resources._70;
            MenuItem13.Text = "模板库".tr();
            this.MenuItem13.Click += new System.EventHandler(this.MenuItem13_Click);


        }

        private void MenuItem1_Click(object sender, EventArgs e)
        {
            SwitchFrm(MenuItem1.Text, MiddleLayer.UI_Vision);
        }
        private void MenuItem2_Click(object sender, EventArgs e)
        {
            SwitchFrm(MenuItem2.Text, MiddleLayer.UI_Language);
        }
        private void MenuItem3_Click(object sender, EventArgs e)
        {
            BrowApp.Motion motion = new BrowApp.Motion();
            motion.ShowDialog();
        }
        private void MenuItem4_Click(object sender, EventArgs e)
        {
            SwitchFrm(MenuItem4.Text, MiddleLayer.UI_MachineTest);
        }
        private void MenuItem5_Click(object sender, EventArgs e)
        {
            new AboutForm().ShowDialog();
        }
        private void MenuItem6_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("notepad.exe", @".\ReleaseNote.txt");
        }
        private void MenuItem7_Click(object sender, EventArgs e)
        {
            //用户管理
            FormUserManager user = new FormUserManager();
            user.ShowDialog();
        }
        private void MenuItem8_Click(object sender, EventArgs e)
        {
            //站位表
        }
        private void MenuItem9_Click(object sender, EventArgs e)
        {
            //资料库
            FormBasMaterial formBasMaterial = new FormBasMaterial();
            formBasMaterial.ShowDialog();
        }
        private void MenuItem10_Click(object sender, EventArgs e)
        {
            //尺寸管理
            FormSize formSize = new FormSize();
            formSize.ShowDialog();
        }
        private void MenuItem11_Click(object sender, EventArgs e)
        {
            //电桥配置
            FormCompensationSet formCompensationSet = new FormCompensationSet();
            formCompensationSet.ShowDialog();
        }
        private void MenuItem12_Click(object sender, EventArgs e)
        {
            //电桥参数
            FormCompensationParam formCompensationParam = new FormCompensationParam();
            formCompensationParam.ShowDialog();
        }
        private void MenuItem13_Click(object sender, EventArgs e)
        {
            //模板库
            FormModel formModel = new FormModel();
            formModel.ShowDialog();
        }
        #endregion

        public KryptonPage GetKryptonPage(string text)
        {
            KryptonPage page = new KryptonPage(text);
            ButtonSpecAny button = new ButtonSpecAny();
            //button.Checked = ButtonCheckState.Unchecked;
            button.Style = PaletteButtonStyle.ButtonSpec;
            button.Type = PaletteButtonSpecStyle.Close;
            button.Click += new System.EventHandler(this.buttonSpecAny1_Click);
            button.Tag = text;
            button.UniqueName = "A7A03A5C9B674000F6B605AA05BC4336";
            page.ButtonSpecs.AddRange(new ButtonSpecAny[] {
            button});
            return page;
        }

        private void NewRibbonRecentDoc(string name, int tag, Bitmap bitmap)
        {
            KryptonContextMenuItem kryptonRibbonRecentDoc = new KryptonContextMenuItem();
            kryptonRibbonRecentDoc.Text = name;
            kryptonRibbonRecentDoc.Tag = tag.ToString();
            kryptonRibbonRecentDoc.Image = bitmap;
            this.MainRib.RibbonAppButton.AppButtonMenuItems.Add(kryptonRibbonRecentDoc);
            kryptonRibbonRecentDoc.Click += KryptonRibbonRecentDoc_Click;
        }

        private void KryptonRibbonRecentDoc_Click(object sender, EventArgs e)
        {
            string tag = ((KryptonContextMenuItem)sender).Tag.ToString();
            switch (tag)
            {
                case "0":
                    //用户管理
                    FormUserManager user = new FormUserManager();
                    user.ShowDialog();
                    break;
                case "1":
                    //站位表
                    break;
                case "2":
                    //资料库
                    FormBasMaterial formBasMaterial = new FormBasMaterial();
                    formBasMaterial.ShowDialog();
                    break;
                case "3":
                    //尺寸管理
                    FormSize formSize = new FormSize();
                    formSize.ShowDialog();
                    break;
                case "4":
                    //电桥配置
                    FormCompensationSet formCompensationSet = new FormCompensationSet();
                    formCompensationSet.ShowDialog();
                    break;
                case "5":
                    //电桥参数
                    FormCompensationParam formCompensationParam = new FormCompensationParam();
                    formCompensationParam.ShowDialog();
                    break;
                case "6":
                    //模板库
                    FormModel formModel = new FormModel();
                    formModel.ShowDialog();
                    break;
                case "7":
                    //语言

                    break;
                default:
                    break;
            }
            this.MainRib.RibbonAppButton.AppButtonImage = ((KryptonContextMenuItem)sender).Image;
        }

        private void btnAddProduce_Click(object sender, EventArgs e)
        {
            //FormProductItem form = new FormProductItem();
            //form.ShowDialog();
            string text = "程序";
            kryptonNavigator1.Pages.Add(GetKryptonPage(text));
            kryptonNavigator1.Pages[kryptonNavigator1.Pages.Count - 1].Controls.Add(new UCProgram());
            kryptonNavigator1.SelectedPage = kryptonNavigator1.Pages[kryptonNavigator1.Pages.Count - 1];
        }

        private void btnAddReport_Click(object sender, EventArgs e)
        {
            string text = "报告";
            kryptonNavigator1.Pages.Add(GetKryptonPage(text));
            kryptonNavigator1.Pages[kryptonNavigator1.Pages.Count - 1].Controls.Add(new UCReport(true));
            kryptonNavigator1.SelectedPage = kryptonNavigator1.Pages[kryptonNavigator1.Pages.Count - 1];
        }

        /// <summary>
        /// 查看所有产品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAllProduce_Click(object sender, EventArgs e)
        {
            string text = btnAllProduce.TextLine1;

            if (kryptonNavigator1.Pages.Where(x => x.Text == text).Count() == 0)
            {
                kryptonNavigator1.Pages.Add(GetKryptonPage(text));
                kryptonNavigator1.Pages[kryptonNavigator1.Pages.Count - 1].Controls.Add(new UCAllProgram());
                kryptonNavigator1.SelectedPage = kryptonNavigator1.Pages[kryptonNavigator1.Pages.Count - 1];
            }
            else
            {
                //所有程序
                for (int i = 0; i < kryptonNavigator1.Pages.Count; i++)
                {
                    if (kryptonNavigator1.Pages[i].Text == text)
                    {
                        kryptonNavigator1.SelectedPage = kryptonNavigator1.Pages[i];
                        break;
                    }
                }
            }



        }

        /// <summary>
        /// 查看所有报告
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAllReport_Click(object sender, EventArgs e)
        {
            //所有报告
            string text = btnAllReport.TextLine1;
            if (kryptonNavigator1.Pages.Where(x => x.Text == text).Count() == 0)
            {
                kryptonNavigator1.Pages.Add(GetKryptonPage(text));
                kryptonNavigator1.Pages[kryptonNavigator1.Pages.Count - 1].Controls.Add(new UCAllReport());
                kryptonNavigator1.SelectedPage = kryptonNavigator1.Pages[kryptonNavigator1.Pages.Count - 1];
            }
            else
            {
                //所有程序
                for (int i = 0; i < kryptonNavigator1.Pages.Count; i++)
                {
                    if (kryptonNavigator1.Pages[i].Text == text)
                    {
                        kryptonNavigator1.SelectedPage = kryptonNavigator1.Pages[i];
                        break;
                    }
                }
            }
        }

        private void buttonSpecAny1_Click(object sender, EventArgs e)
        {
            ButtonSpecAny pa = (sender as ButtonSpecAny);
            for (int i = 0; i < kryptonNavigator1.Pages.Count; i++)
            {
                if (kryptonNavigator1.Pages[i].Text == pa.Tag.ToString())
                {
                    kryptonNavigator1.Pages.RemoveAt(i);
                    break;
                }
            }
        }

        private void UserLoad_btn_Click(object sender, EventArgs e)
        {
            BrowApp.User.LoginFrom user = new BrowApp.User.LoginFrom();
            user.ShowDialog();
            Global.Password = user.Password;
            Global.Authority = user.Authority;
            Global.UserName = user.UserName;
            Global.UserHandler?.Invoke(user.Authority, new EventArgs());
            APP.Log.I_Log("用户登录-用户名:" + Global.UserName + "-权限:" + Global.Authority);
            //初始化系统菜单
            if(!string.IsNullOrEmpty(user.Authority))
             groupMotion.Visible = true;
            else
             groupMotion.Visible = false;
        }

        private void logout_btn_Click(object sender, EventArgs e)
        {
            Global.Password = null;
            Global.Authority = null;
            Global.UserName = null;
            Global.UserHandler?.Invoke("", new EventArgs());
            APP.Log.I_Log("用户登出");
            groupMotion.Visible = false;
        }

        /// <summary>
        /// 用户权限管理
        /// </summary>
        /// <param name="Authority"></param>
        private void RefAuthority(string Authority)
        {
            this.MainRib.RibbonAppButton.AppButtonMenuItems.Clear();
            switch (Authority)
            {
                case "Administrator"://管理员
                    this.MainRib.RibbonAppButton.AppButtonMenuItems
                        .AddRange(new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItemBase[] {
                            MenuItem7, MenuItem8, MenuItem9, MenuItem10, MenuItem11, MenuItem12, MenuItem13,
                this.MenuItem1,MenuItem2,MenuItem3,MenuItem4,MenuItem5,MenuItem6});
                    CalibBtn.Enabled = true;
                    ProgramBtn.Enabled = true;
                    Debug_btn.Enabled = true;
                    System_btn.Enabled = true;
                    Mes_btn.Enabled = true;
                    break;
                case "Engineer"://工程师
                    this.MainRib.RibbonAppButton.AppButtonMenuItems.
                        AddRange(new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItemBase[] {
                                MenuItem7, MenuItem8, MenuItem9, MenuItem10, MenuItem11, MenuItem12, MenuItem13,
                MenuItem2,MenuItem3,MenuItem4,MenuItem5,MenuItem6});
                    CalibBtn.Enabled = true;
                    ProgramBtn.Enabled = true;
                    Debug_btn.Enabled = true;
                    System_btn.Enabled = true;
                    Mes_btn.Enabled = true;
                    break;
                case "Technician"://技术员
                    this.MainRib.RibbonAppButton.AppButtonMenuItems.AddRange(
                        new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItemBase[] {
                                MenuItem7, MenuItem8, MenuItem9, MenuItem10, MenuItem11, MenuItem12, MenuItem13,
               MenuItem5,MenuItem6});
                    CalibBtn.Enabled = false;
                    ProgramBtn.Enabled = true;
                    Debug_btn.Enabled = true;
                    System_btn.Enabled = true;
                    Mes_btn.Enabled = true;
                    break;
                case "Operator"://技术员
                    this.MainRib.RibbonAppButton.AppButtonMenuItems.
                        AddRange(new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItemBase[] {
                                MenuItem7, MenuItem8, MenuItem9, MenuItem10, MenuItem11, MenuItem12, MenuItem13,
               MenuItem5,MenuItem6});
                    CalibBtn.Enabled = false;
                    ProgramBtn.Enabled = false;
                    Debug_btn.Enabled = false;
                    System_btn.Enabled = false;
                    Mes_btn.Enabled = false;
                    break;
                default:
                    this.MainRib.RibbonAppButton.AppButtonMenuItems.
                        AddRange(new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItemBase[] {
                                MenuItem7, MenuItem8, MenuItem9, MenuItem10, MenuItem11, MenuItem12, MenuItem13,
               MenuItem5,MenuItem6});
                    CalibBtn.Enabled = false;
                    ProgramBtn.Enabled = false;
                    Debug_btn.Enabled = false;
                    System_btn.Enabled = false;
                    Mes_btn.Enabled = false;
                    break;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {

        }

        private void btnResetAlarm_Click(object sender, EventArgs e)
        {

        }

        private void CalibBtn_Click(object sender, EventArgs e)
        {
            SwitchFrm(CalibBtn.TextLine1, MiddleLayer.UI_Calib);
        }

        /// <summary>
        /// 切换窗体
        /// </summary>
        /// <param name="control"></param>
        private void SwitchFrm(string text, Control control)
        {
            if (kryptonNavigator1.Pages.Where(x => x.Text == text).Count() == 0)
            {
                kryptonNavigator1.Pages.Add(GetKryptonPage(text));
                kryptonNavigator1.Pages[kryptonNavigator1.Pages.Count - 1].Controls.Add(control);
                kryptonNavigator1.SelectedPage = kryptonNavigator1.Pages[kryptonNavigator1.Pages.Count - 1];
            }
            else
            {
                //所有程序
                for (int i = 0; i < kryptonNavigator1.Pages.Count; i++)
                {
                    if (kryptonNavigator1.Pages[i].Text == text)
                    {
                        kryptonNavigator1.SelectedPage = kryptonNavigator1.Pages[i];
                        break;
                    }
                }
            }
        }

        private void ProgramBtn_Click(object sender, EventArgs e)
        {
            SwitchFrm(ProgramBtn.TextLine1, MiddleLayer.UI_Program);
        }
        private MotionCtr motionCtr;
        private void Debug_btn_Click(object sender, EventArgs e)
        {
            if (Global.SystemRun) { FloatingTip.ShowWarning("请暂停设备再执行操作".tr()); return; }
            if (motionCtr == null || motionCtr.IsDisposed)
            {
                motionCtr = new MotionCtr();
                motionCtr.Show();
                motionCtr.Activate();
            }
            else
            {
                motionCtr.Activate();
                motionCtr.FomActivated();
            }
        }

        private void System_btn_Click(object sender, EventArgs e)
        {
            SwitchFrm(System_btn.TextLine1, MiddleLayer.UI_System);
        }

        private void Mes_btn_Click(object sender, EventArgs e)
        {
            SwitchFrm(System_btn.TextLine1, MiddleLayer.UI_MESControl);
        }

     

        private void HomeBtn_Click(object sender, EventArgs e)
        {
            if (Global.SystemRun) { return; }
            Global.MachineState = GEnumEx.MachineState.MachineInitialize;
            Global.SystemInitialOk = false;
            Global.StopFlag = false;
            HomeStart home = new HomeStart();
            home.ShowDialog();
            if (home.IsHomeflg)
            {
                Global.MachineState = GEnumEx.MachineState.MachineStop;
                Global.TcpClass.Send("M:归零_OK");
            }
            else
            {
                Global.MachineState = GEnumEx.MachineState.MachineError;
                RefSystemState(2);
                Global.TcpClass.Send("M:归零_NG");
            }
        }
    }
}
