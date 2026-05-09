using BrowApp;
using BrowApp.Language;
using BrowApp.MessageTip;
using GSP_SJ.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSP.UI
{
    public partial class LaserFrm : Form
    {

        public bool RunFlag { get; set; }
        public string Code { get; set; }
        public double LaserValue { get; set; }


        public LaserFrm()
        {
            InitializeComponent();
            Global.Laser.Serial.ComRefEvent += new EventHandler(RefLaser);
        }
        void RefLaser(object sender, EventArgs e)
        {
            try
            {
                this.BeginInvoke(new Action(() =>
                {
                    LaserValue = Global.Laser.LaserValue(Global.LaserType);
                    double Var = LaserValue - Global.Hoffset;
                    Laser_lab.Text = Global.Laser.LaserValue(Global.LaserType).ToString()+"mm";
                    double Dx = Var - Global.CalibData.LaserValue;
                    Offset_txt.Text = Dx.ToString("F3");
                    APP.Log.I_Log("实际偏差值" + Math.Abs(Dx).ToString());
                    APP.Log.I_Log("允许偏差值" + Global.CalibData.Allowablevalue.ToString());
                    if (Math.Abs(Dx) <= Global.CalibData.Allowablevalue)
                    {
                        Result_lab.Text = "OK";
                        Result_lab.StateNormal.ShortText.Color1 = Color.Green;
                        Result_lab.Values.Image = Resources._155;
                    }
                    else
                    {
                        Result_lab.Text = "NG";
                        Result_lab.Values.Image = Resources._114;
                        Result_lab.StateNormal.ShortText.Color1 = Color.Red;
                    }
                }));
            }
            catch { }

        }

       

        private void Apply_btn_Click(object sender, EventArgs e)
        {
            double dix = Convert.ToDouble(Offset_txt.Text);
            if (Math.Abs(dix) > 1)
            {
                FloatingTip.ShowWarning("偏差超出范围".tr());
                return;
            }
            else
            {
                Global.Hoffset = dix;
            }
        }

        private void Ok_Btn_Click(object sender, EventArgs e)
        {
            Global.Laser.Serial.ComRefEvent -= new EventHandler(RefLaser);
            this.Dispose();
            this.Close();
        }

        private void Exit_btn_Click(object sender, EventArgs e)
        {
            Global.Laser.Serial.ComRefEvent -= new EventHandler(RefLaser);
            this.Dispose();
            this.Close();
        }

        private void LaserFrm_Load(object sender, EventArgs e)
        {
            Code_txt.Text = this.Code;
            LaserValue_txt.Text = Global.CalibData.LaserValue.ToString();
            Allowablevalue_txt.Text = Global.CalibData.Allowablevalue.ToString();
            LaserValue_Lab.Text = Global.Hoffset.ToString()+"mm";
            Global.Laser.Send(Global.LaserType);
            BrowApp.Language.Language.Instance.UpdateLanguage(this, null);
        }
    }
}
