using BorwinAnalyse.Forms;
using BrowApp;
using BrowApp.Alarm;
using BrowApp.CustomControl;
using BrowApp.Language;
using BrowApp.MessageTip;
using BrowLib;
using CKVisionAppNet;
using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Ribbon;
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
            Global.UserHandler += new EventHandler(RefUser);
            // 3. 创建委托实例
            AlarmHelper.AddEvent += UpAlam;

            timer = new System.Windows.Forms.Timer();
            timer.Interval = 20;
            timer.Tick += timer1_Tick;
            //timer.Start();
            this.Load += FormMain_Load;
            this.FormClosing += BrowForm_FormClosing;
        }

        /// <summary>
        /// 关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (BrowApp.APP.Tip.ShowTip(0, "提示".tr(), "确定要关闭软件吗？".tr(), "确定".tr(), "取消".tr()) == 1)
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

            State_mes.TextLine1 = "系统未初始化？".tr();
            State_mes.StateNormal.TextColor = Color.DarkOrange;

            BrowApp.Language.Language.Instance.UpdateLanguage(this, null);
            Global.GlobRefEvent?.Invoke();
            BrowLib.Controller.SetLimit(false);
            Global.VisionApp.SetLanguage("eng");
            Task.Run(() =>
            {
                bool RE = Global.VisionApp.LoadProject(@"./VisionProject/VisionPro.proj");
                APP.Log.I_Log("加载视觉方案{" + RE.ToString() + "}");
            });

        }

        System.Windows.Forms.Timer timer;
        private int Btimes;

        private MotionCtr motionCtr;

        #region 定时器
        private KTimer KTimer = new KTimer();
        private KTimer FlickerTimer = new KTimer();
        private KTimer Mes_Timer = new KTimer();
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
                    Global.MachineState = GEnumEx.MachineState.MachineError;
                }
                BrowLib.Controller.OutPort["蜂鸣器_OUT"].On();
                BrowLib.Controller.OutPort["三色灯红色_OUT"].On();
                BrowLib.Controller.OutPort["三色灯黄色_OUT"].Off();
                BrowLib.Controller.OutPort["三色灯绿色_OUT"].Off();
                Global.States = 1;//报警
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
                                Global.States = 2;//待机
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
                                Global.States = 0;//停机
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

        private void Upstates()
        {

            if (Global._States != Global.States)
            {
                Global._States = Global.States;

                string StatusID = "STOP";

                switch (Global._States)
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
        #endregion

        void UpAlam(object sends, EventArgs e)
        {
            try
            {
                string Message = ((BrowApp.Alarm.AlarmHelper.AlarmType)sends).E_Message;
                string E_Code = ((BrowApp.Alarm.AlarmHelper.AlarmType)sends).E_Code;
                string alertID = Global.mesConfig.equipmentNo + DateTime.Now.ToString("yyyyMMddhhmmss");
                if (Global.mesConfig.Ulr2_ck)
                {
                    Task.Run(() =>
                    {
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
            catch (Exception)
            {

            }

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
                    kryptonNavigator1.SelectedPage.ImageLarge = Properties.Resources._32;
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
                    kryptonNavigator1.SelectedPage.ImageLarge = Properties.Resources._66;
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
            SwitchFrm(MenuItem1.Text, MiddleLayer.UI_Vision, MenuItem1.Image);
        }
        private void MenuItem2_Click(object sender, EventArgs e)
        {
            SwitchFrm(MenuItem2.Text, MiddleLayer.UI_Language, MenuItem2.Image);
        }
        private void MenuItem3_Click(object sender, EventArgs e)
        {
            BrowApp.Motion motion = new BrowApp.Motion();
            motion.ShowDialog();
        }
        private void MenuItem4_Click(object sender, EventArgs e)
        {
            SwitchFrm(MenuItem4.Text, MiddleLayer.UI_MachineTest, MenuItem4.Image);
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

        private void btnAddProduce_Click(object sender, EventArgs e)
        {
            string text = "新程序".tr();
            kryptonNavigator1.Pages.Add(GetKryptonPage(text));
            kryptonNavigator1.Pages[kryptonNavigator1.Pages.Count - 1].Controls.Add(new UCProgram());
            kryptonNavigator1.SelectedPage = kryptonNavigator1.Pages[kryptonNavigator1.Pages.Count - 1];
            kryptonNavigator1.SelectedPage.ImageLarge = btnAddProduce.ImageLarge;
        }

        private void btnAddReport_Click(object sender, EventArgs e)
        {
            string text = "新报告".tr();
            kryptonNavigator1.Pages.Add(GetKryptonPage(text));
            kryptonNavigator1.Pages[kryptonNavigator1.Pages.Count - 1].Controls.Add(new UCReport(true));
            kryptonNavigator1.SelectedPage = kryptonNavigator1.Pages[kryptonNavigator1.Pages.Count - 1];
            kryptonNavigator1.SelectedPage.ImageLarge = btnAddReport.ImageLarge;
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

            kryptonNavigator1.SelectedPage.ImageLarge = btnAllProduce.ImageLarge;

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

            kryptonNavigator1.SelectedPage.ImageLarge = btnAllReport.ImageLarge;
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
            if (!string.IsNullOrEmpty(user.Authority))
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
            APP.Alarm.Clear();//报警清除
        }

        private void CalibBtn_Click(object sender, EventArgs e)
        {
            SwitchFrm(CalibBtn.TextLine1, MiddleLayer.UI_Calib, CalibBtn.ImageLarge);
        }

        /// <summary>
        /// 切换窗体
        /// </summary>
        /// <param name="control"></param>
        private void SwitchFrm(string text, Control control, System.Drawing.Image image)
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
            kryptonNavigator1.SelectedPage.ImageLarge = image;
        }

        private void ProgramBtn_Click(object sender, EventArgs e)
        {
            SwitchFrm(ProgramBtn.TextLine1, MiddleLayer.UI_Program, ProgramBtn.ImageLarge);
        }

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
            SwitchFrm(System_btn.TextLine1, MiddleLayer.UI_System, System_btn.ImageLarge);
        }

        private void Mes_btn_Click(object sender, EventArgs e)
        {
            SwitchFrm(System_btn.TextLine1, MiddleLayer.UI_MESControl, System_btn.ImageLarge);
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
            }
            else
            {
                Global.MachineState = GEnumEx.MachineState.MachineError;
                RefSystemState(2);
            }
        }
    }
}
