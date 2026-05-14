using BrowApp;
using BrowApp.Language;
using BrowApp.MessageTip;
using GSP.SystemData;
using GSP_SJ.Properties;
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
    public partial class UI_SystemData : UserControl
    {
        FlowControl Flow=new FlowControl();
        public UI_SystemData()
        {
            InitializeComponent();
            Global.Laser.Serial.ComRefEvent += new EventHandler(RefLaser);
            Global.GlobRefEvent += ObjFom;
        }
        private void FomObj()
        {

            Global.Systemdata.TestDlay = Convert.ToInt32(TestDlay.Value);
            Global.Systemdata.BuzzeDely = Convert.ToInt32(BuzzeDely.Value);
            Global.BuzzeDely = Global.Systemdata.BuzzeDely;

            Global.Systemdata.XYVelocity = Global.XYVelocity;
            Global.Systemdata.RVelocity = Global.RVelocity;
            Global.Systemdata.ZVelocity = Global.ZVelocity;

            Global.Systemdata.OrbModel = Global.OrbModel;

            Global.Systemdata.Ccdhight = Convert.ToDouble(Camheight.Value);

            Global.Systemdata.SafeHigh = Convert.ToDouble(SafHight.Value);

            Global.Systemdata.SafeHigh2 = Convert.ToDouble(UpHight.Value);

            Global.Systemdata.RunMode = Global.RunMode;

            Global.Systemdata.IsCcdMode = Global.IsCcdMode;

            Global.Systemdata.IsNoMark = Global.Is_NoMark;

            Global.Systemdata.PateFile = PatlePatn_txt.Text;

            Global.Systemdata.CfgFile = Configpath_txt.Text;
            //停止位置
            Global.Systemdata.StopPos.Xpos = Convert.ToDouble(this.StopXpos.Value);
            Global.Systemdata.StopPos.Ypos = Convert.ToDouble(this.StopYpos.Value);
            Global.Systemdata.StopPos.Zpos = Convert.ToDouble(this.StopZpos.Value);

            Global.Systemdata.Trackoffset = Convert.ToDouble(this.Trackoffset.Value);

            Global.Systemdata.InDaytime = Convert.ToDouble(this.Entrydelay.Value);
            Global.Systemdata.buf_Zspeed = Convert.ToDouble(this.Zbufferspd.Value);

            Global.Systemdata.M_LED.LED_R = Convert.ToInt16(this.MR_Num.Value);
            Global.Systemdata.M_LED.LED_G = Convert.ToInt16(this.MG_Num.Value);
            Global.Systemdata.M_LED.LED_B = Convert.ToInt16(this.MB_Num.Value);

            Global.Systemdata.P_LED.LED_R = Convert.ToInt16(this.PR_Num.Value);
            Global.Systemdata.P_LED.LED_G = Convert.ToInt16(this.PG_Num.Value);
            Global.Systemdata.P_LED.LED_B = Convert.ToInt16(this.PB_Num.Value);

            Global.Systemdata.P_LED2.LED_R = Convert.ToInt16(this.PR_Num2.Value);
            Global.Systemdata.P_LED2.LED_G = Convert.ToInt16(this.PG_Num2.Value);
            Global.Systemdata.P_LED2.LED_B = Convert.ToInt16(this.PB_Num2.Value);

            Global.Systemdata.S_LED.LED_R = Convert.ToInt16(this.SR_Num.Value);
            Global.Systemdata.S_LED.LED_G = Convert.ToInt16(this.SG_Num.Value);
            Global.Systemdata.S_LED.LED_B = Convert.ToInt16(this.SB_Num.Value);

            Global.Systemdata.Servicelife = Convert.ToInt32(this.Servicelife.Value);

            Global.Systemdata.LaserCom = LaserCom_cob.Text;
            Global.Systemdata.LightCom = LightCom_cob.Text;

            Global.Systemdata.GdWight = Convert.ToDouble(this.GdWight_Num.Value);
        }

        private void ObjFom()
        {
            XY_TrackBar.Value = (int)(Global.Systemdata.XYVelocity * 100);
            Global.XYVelocity = Global.Systemdata.XYVelocity;

            R_TrackBar.Value = (int)(Global.Systemdata.RVelocity * 100);
            Global.RVelocity = Global.Systemdata.XYVelocity;

            Z_TrackBar.Value = (int)(Global.Systemdata.ZVelocity * 100);
            Global.ZVelocity = Global.Systemdata.XYVelocity;

            Global.OrbModel = Global.Systemdata.OrbModel;
            switch (Global.OrbModel)
            {
                case 0:
                    OnLine_rido.Checked = true;
                    break;
                case 1:
                    Offline_rido.Checked = true;
                    break;
                case 2:
                    direct_rido.Checked = true;
                    break;
            }

            Global.RrfSpd();

            Camheight.Value = decimal.Parse(Global.Systemdata.Ccdhight.ToString());

            SafHight.Value = decimal.Parse(Global.Systemdata.SafeHigh.ToString());

            UpHight.Value = decimal.Parse(Global.Systemdata.SafeHigh2.ToString());

            TestDlay.Value = decimal.Parse(Global.Systemdata.TestDlay.ToString());

            BuzzeDely.Value = decimal.Parse(Global.Systemdata.BuzzeDely.ToString());



            if (Global.Systemdata.RunMode == 0) { Global.RunMode = 0; Mode1_Rdio.Checked = true; }
            else { Global.RunMode = 1; Mode2_Rdio.Checked = true; }

            if (Global.Systemdata.IsCcdMode) { Global.IsCcdMode = true; ck_VisionMot.Checked = true; }
            else { Global.IsCcdMode = false; ck_VisionMot.Checked = false; }

            if (Global.Systemdata.IsNoMark) { Global.Is_NoMark = true; ck_mark.Checked = true; }
            else { Global.Is_NoMark = false; ck_mark.Checked = false; }

            Configpath_txt.Text = Global.Systemdata.CfgFile;
            PatlePatn_txt.Text = Global.Systemdata.PateFile;

            this.StopXpos.Value = decimal.Parse(Global.Systemdata.StopPos.Xpos.ToString());
            this.StopYpos.Value = decimal.Parse(Global.Systemdata.StopPos.Ypos.ToString());
            this.StopZpos.Value = decimal.Parse(Global.Systemdata.StopPos.Zpos.ToString());
            this.Trackoffset.Value = decimal.Parse(Global.Systemdata.Trackoffset.ToString());
            this.Entrydelay.Value = decimal.Parse(Global.Systemdata.InDaytime.ToString());

            this.Zbufferspd.Value = decimal.Parse(Global.Systemdata.buf_Zspeed.ToString());

            this.ck_safetydoor.Checked = Global.Systemdata.IsSafeDoor;


            this.MR_Num.Value = decimal.Parse(Global.Systemdata.M_LED.LED_R.ToString());
            this.MG_Num.Value = decimal.Parse(Global.Systemdata.M_LED.LED_G.ToString());
            this.MB_Num.Value = decimal.Parse(Global.Systemdata.M_LED.LED_B.ToString());


            this.PR_Num.Value = decimal.Parse(Global.Systemdata.P_LED.LED_R.ToString());
            this.PG_Num.Value = decimal.Parse(Global.Systemdata.P_LED.LED_G.ToString());
            this.PB_Num.Value = decimal.Parse(Global.Systemdata.P_LED.LED_B.ToString());



            this.PR_Num2.Value = decimal.Parse(Global.Systemdata.P_LED2.LED_R.ToString());
            this.PG_Num2.Value = decimal.Parse(Global.Systemdata.P_LED2.LED_G.ToString());
            this.PB_Num2.Value = decimal.Parse(Global.Systemdata.P_LED2.LED_B.ToString());


            this.SR_Num.Value = decimal.Parse(Global.Systemdata.S_LED.LED_R.ToString());
            this.SG_Num.Value = decimal.Parse(Global.Systemdata.S_LED.LED_G.ToString());
            this.SB_Num.Value = decimal.Parse(Global.Systemdata.S_LED.LED_B.ToString());
            this.Servicelife.Value = decimal.Parse(Global.Systemdata.Servicelife.ToString());


            this.LaserCom_cob.Text = Global.Systemdata.LaserCom;
            this.LightCom_cob.Text = Global.Systemdata.LightCom;

            
            this.GdWight_Num.Value= decimal.Parse(Global.Systemdata.GdWight.ToString());
        }

        private void UI_SystemData_Load(object sender, EventArgs e)
        {
            ObjFom();
            BrowApp.Language.Language.Instance.UpdateLanguage(this, null);
        }

        private void tPosBtn_Click(object sender, EventArgs e)
        {
            this.StopXpos.Value = decimal.Parse(Global.X轴.GetPrfPos().ToString());
            this.StopYpos.Value = decimal.Parse(Global.Y轴.GetPrfPos().ToString());
            this.StopZpos.Value = decimal.Parse(Global.Z轴.GetPrfPos().ToString());
        }

        private void GoTo_btn_Click(object sender, EventArgs e)
        {
            if (!Global.SystemInitialOk)
            {
                APP.Tip.ShowTip(0, "警告".tr(), "设备未归零,请先归零再执行操作!!!".tr(), "确定".tr());
                return;
            }
            if (Flow.IsManualRun()) { return; }
            WaitFrm waitFrm = new WaitFrm(GoTo_btn);
            waitFrm.Enabled = true;
            Flow.ExecuteManual(() =>
            {
                new HandFlow().SafeMoveXyz(0, (double)StopXpos.Value, (double)StopYpos.Value, (double)StopZpos.Value);

                Invoke(new Action(() =>
                {
                    waitFrm.Enabled = false;
                }));
            });
        }
        private void Save_btn_Click(object sender, EventArgs e)
        {
            try
            {
                FomObj();
                Global.Systemdata.Save();
                FloatingTip.ShowOk("保存系统参数成功".tr());
            }
            catch
            {
                FloatingTip.ShowError("保存系统参数失败".tr());
            }
        }
        private void XY_TrackBar_ValueChanged(object sender, EventArgs e)
        {
            Global.XYVelocity = (double)XY_TrackBar.Value / 100;
            XY_lab.Text = XY_TrackBar.Value.ToString();
            Global.RrfSpd();
        }

        private void R_TrackBar_ValueChanged(object sender, EventArgs e)
        {
            Global.RVelocity = (double)R_TrackBar.Value / 100;
            R_lab.Text = R_TrackBar.Value.ToString();
            Global.RrfSpd();
        }

        private void Z_TrackBar_ValueChanged(object sender, EventArgs e)
        {
            Global.ZVelocity = (double)Z_TrackBar.Value / 100;
            Z_lab.Text = Z_TrackBar.Value.ToString();
            Global.RrfSpd();
        }
        private void ck_mark_CheckedChanged(object sender, EventArgs e)
        {
            if (ck_mark.Checked)
            {
                Global.Is_NoMark = true;
            }
            else
            {
                Global.Is_NoMark = false;
            }
        }
        private void ck_safetydoor_CheckedChanged(object sender, EventArgs e)
        {
            if (ck_safetydoor.Checked)
                Global.Systemdata.IsSafeDoor = true;
            else
                Global.Systemdata.IsSafeDoor = false;
        }
        private void ck_buzzer_CheckedChanged(object sender, EventArgs e)
        {
            if(ck_buzzer.Checked)
            {
                
            }
        }
        private void Mode1_Rdio_CheckedChanged(object sender, EventArgs e)
        {
            Global.RunMode = 0;
        }

        private void Mode2_Rdio_CheckedChanged(object sender, EventArgs e)
        {
            Global.RunMode = 1;
        }

        private void ck_VisionMot_CheckedChanged(object sender, EventArgs e)
        {
            if (ck_VisionMot.Checked)
            {
                Global.IsCcdMode = true;
            }
            else
            {
                Global.IsCcdMode = false;
            }
        }

        private void OnLine_rido_CheckedChanged(object sender, EventArgs e)
        {
            Global.OrbModel = 0;
        }

        private void Offline_rido_CheckedChanged(object sender, EventArgs e)
        {
            Global.OrbModel = 1;
        }

        private void direct_rido_CheckedChanged(object sender, EventArgs e)
        {
            Global.OrbModel =2;
        }

        private void TrackBar_ValueChanged(object sender, EventArgs e)
        {
            Global.Light.SetIntensity(Convert.ToInt16(Channel_num.Value), TrackBar.Value);
            this.Value_num.Value = decimal.Parse(TrackBar.Value.ToString());
        }

        private void Save_Btn2_Click(object sender, EventArgs e)
        {
            try
            {
                FomObj();
                Global.Systemdata.Save();
                FloatingTip.ShowOk("保存系统参数成功".tr());
            }
            catch
            {
                FloatingTip.ShowError("保存系统参数失败".tr());
            }
        }

        private void Connect_btn_Click(object sender, EventArgs e)
        {
           if(Global.Light.LightIni(LightCom_cob.Text))
            {
                Connect_btn.Values.Image = Resources._155;
            }
            else
            {
                Connect_btn.Values.Image = Resources._114;
            }
        }

        private void Connect2_btn_Click(object sender, EventArgs e)
        {
            if (Global.Laser.LaserIni(LaserCom_cob.Text, Global.LaserType))
            {
                Connect2_btn.Values.Image = Resources._155;
            }
            else
            {
                Connect2_btn.Values.Image = Resources._114;
            }
        }
        void RefLaser(object sender, EventArgs e)
        {
            try
            {
                this.BeginInvoke(new Action(() =>
                {
                    Laser_lab.Text = Global.Laser.LaserValue(Global.LaserType).ToString("F3") + "mm";
                }));
            }
            catch { }

        }

        private void SetLight1_btn_Click(object sender, EventArgs e)
        {
            try
            {
                PR_Num.Value = decimal.Parse(Global.Light.GetIntensity(1).ToString());
                PG_Num.Value = decimal.Parse(Global.Light.GetIntensity(2).ToString());
                PB_Num.Value = decimal.Parse(Global.Light.GetIntensity(3).ToString());
            }
            catch { FloatingTip.ShowError("亮度设置失败"); }
        }

        private void SetLight2_btn_Click(object sender, EventArgs e)
        {
            try
            {
                PR_Num2.Value = decimal.Parse(Global.Light.GetIntensity(1).ToString());
                PG_Num2.Value = decimal.Parse(Global.Light.GetIntensity(2).ToString());
                PB_Num2.Value = decimal.Parse(Global.Light.GetIntensity(3).ToString());
            }
            catch { FloatingTip.ShowError("亮度设置失败"); }
        }

        private void SetLight3_btn_Click(object sender, EventArgs e)
        {
            try
            {
                SR_Num.Value = decimal.Parse(Global.Light.GetIntensity(1).ToString());
                SG_Num.Value = decimal.Parse(Global.Light.GetIntensity(2).ToString());
                SB_Num.Value = decimal.Parse(Global.Light.GetIntensity(3).ToString());
            }
            catch { FloatingTip.ShowError("亮度设置失败"); }
        }

        private void SetLight4_btn_Click(object sender, EventArgs e)
        {
            try
            {
                MR_Num.Value = decimal.Parse(Global.Light.GetIntensity(1).ToString());
                MG_Num.Value = decimal.Parse(Global.Light.GetIntensity(2).ToString());
                MB_Num.Value = decimal.Parse(Global.Light.GetIntensity(3).ToString());
            }
            catch { FloatingTip.ShowError("亮度设置失败"); }
        }

        private void Read_btn_Click(object sender, EventArgs e)
        {
            Global.Laser.Send(Global.LaserType);
        }
    }
}
