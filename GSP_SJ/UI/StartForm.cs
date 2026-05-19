using BorwinAnalyse.BaseClass;
using BorwinAnalyse.ImportBom;
using ComponentFactory.Krypton.Toolkit;
using ElectricMeter;
using GSP_SJ.ModelClass;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace GSP
{
    public partial class StartForm : KryptonForm
    {
        private BrowLib.KTimer timer;//计时器
        private const int LOAD_ITEM_COUNT = 8;
        private int m_loadProgress;
        private bool Rtn = false;
        Thread ThreadIni;
        public StartForm()
        {
            InitializeComponent();
            this.IniHeader.MouseDown += TopBar_MouseDown;
            this.IniHeader.MouseMove += TopBar_MouseMove;
            this.Load += StartForm_Load;

            timer = new BrowLib.KTimer();
            IniHeader.ButtonSpecs[0].Visible = false;
            IniHeader.ButtonSpecs[1].Visible = true;
            ThreadIni = new Thread(IniWork);
            ThreadIni.IsBackground = true;
            ThreadIni.Start();
        }
        private void StartForm_Load(object sender, EventArgs e)
        {
            IniHeader.Values.Description = "Vosion:" + Global.Vosion;
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
        /// 双缓冲，解决界面加载、放大、缩小的卡顿问题
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }
        //运行初始化消息类型
        private enum InitInfoReportMode
        {
            Progress = 0,
            Complete = 1,
        }

        private void Close_btn_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
            DialogResult = DialogResult.No;
        }
        //运行初始化消息委托
        private delegate void InitInfoReportHandler(InitInfoReportMode mode, int loadProgress, bool result, string text, string time);
        //运行初始化消息处理
        private void InitInfoReportedProc(InitInfoReportMode mode, int loadProgress, bool result, string text, string time)
        {
            switch (mode)
            {
                case InitInfoReportMode.Progress:
                    SysInit_Listbox.Items.Add(new KryptonListItem()
                    {
                        ShortText = text,
                        LongText = "(" + time + "ms)",
                        Image = result ? GSP_SJ.Properties.Resources.Ok_3_24px : GSP_SJ.Properties.Resources.Warn_24px
                    });
                    lblPercent.Text = string.Format("{0}%", (int)((double)(loadProgress + 1) / (double)LOAD_ITEM_COUNT * 100));
                    break;
                case InitInfoReportMode.Complete:
                    //MSetting.Mode = m_loadSimul ? MachineMode.Simul : MachineMode.Normal;

                    //if (m_loadErrCount <= 0 && !m_loadSimul)
                    //{
                    DialogResult = DialogResult.OK;
                    this.Close();
                    //}
                    IniHeader.ButtonSpecs[0].Visible = true;
                    IniHeader.ButtonSpecs[1].Visible = true;
                    break;
                default:
                    break;
            }
        }
        private void InitInfoReported(InitInfoReportMode mode, int loadProgress, bool result, string text, string time)
        {
            if (this.InvokeRequired)
            {
                BeginInvoke(new InitInfoReportHandler(InitInfoReportedProc), new object[] { mode, loadProgress, result, text, time });
            }
            else
            {
                InitInfoReportedProc(mode, loadProgress, result, text, time);
            }
        }
        private void IniWork()
        {
            while (m_loadProgress < LOAD_ITEM_COUNT)
            {
                switch (m_loadProgress)
                {
                    case 0:
                        timer.Restart();
                        Rtn = BrowApp.APP.Alarm.InitAlarmConfiguration(null);
                        BrowApp.Language.Language.Instance.Load();
                        m_loadProgress++;
                        InitInfoReported(InitInfoReportMode.Progress, m_loadProgress, Rtn, "初始化报警配置文件", timer.GetTime().ToString());
                        break;
                    case 1:
                        timer.Restart();
                        Rtn = BrowLib.Controller.IOIni();
                        InitInfoReported(InitInfoReportMode.Progress, m_loadProgress, Rtn, "初始化IO配置文件", timer.GetTime().ToString());
                        m_loadProgress++;
                        break;
                    case 2:
                        timer.Restart();
                        Global.CalibData.Read(ref Global.CalibData);
                        Global.CamHeight = Global.CalibData.CalibParm.Zpos;
                        Global.Systemdata.Read(ref Global.Systemdata);
                        Global.mesConfig.Read(ref Global.mesConfig);
                        Global.Authority = Global.ReadAuthority();
                        Global.sLaserType = Global.GetLaserType();
                        Global.sModel = Global.GetModel();
                        Global.BuzzeDely = Global.Systemdata.BuzzeDely;
                        Global.GetSystem();
                        Global.coordinateCompensator.LoadFromCsv(Global.GlabPath + "/Compensator.Csv");
                        Global.GetSpecifications();
                        Global.ReadTextNum();
                        InitInfoReported(InitInfoReportMode.Progress, m_loadProgress, Rtn, "加载系统配置参数", timer.GetTime().ToString());
                        m_loadProgress++;
                        break;
                    case 3:
                        timer.Restart();
                        this.Rtn = Global.INICard();
                        InitInfoReported(InitInfoReportMode.Progress, m_loadProgress, Rtn, "初始化运动控制卡", timer.GetTime().ToString());
                        m_loadProgress++;
                        break;
                    case 4:
                        timer.Restart();
                        Rtn = Global.TcpClass.TCPini("127.0.0.1", 8000, "Tcp");
                        InitInfoReported(InitInfoReportMode.Progress, m_loadProgress, Rtn, "初始化TCP连接", timer.GetTime().ToString());
                        m_loadProgress++;
                        break;
                    case 5:
                        timer.Restart();
                        Rtn = Global.Light.LightIni(Global.Systemdata.LightCom);
                        InitInfoReported(InitInfoReportMode.Progress, m_loadProgress, Rtn, "初始化光源控制器", timer.GetTime().ToString());
                        m_loadProgress++;
                        break;
                    case 6:
                        timer.Restart();
                        Rtn = Global.Laser.LaserIni(Global.Systemdata.LaserCom, Global.LaserType);
                        InitInfoReported(InitInfoReportMode.Progress, m_loadProgress, Rtn, "初始化激光位移传感器", timer.GetTime().ToString());
                        Thread.Sleep(1000);
                        m_loadProgress++;
                        break;
                    case 7:
                        CommonAnalyse.Instance.Load();
                        AnaylseDataManager.Instance.Load();
                        DeepOCRHelper.Init();
                        Browocrlib.OCRHelper.Init();
                   
                        InitInfoReported(InitInfoReportMode.Complete, -1, true, null, null);
                        break;
                }
                Thread.Sleep(10);
            }
        }
    }
}
