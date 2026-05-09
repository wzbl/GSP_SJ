using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSP.Light
{
    public partial class LightControl : UserControl
    {
        public LightControl()
        {
            InitializeComponent();
        }
        public void LightCtrIni()
        {
            R_TrackBar.Value = Global.Light.GetIntensity(1);
            this.R_num.Value = decimal.Parse(R_TrackBar.Value.ToString());
            G_TrackBar.Value = Global.Light.GetIntensity(2);
            this.G_num.Value = decimal.Parse(G_TrackBar.Value.ToString());
            B_TrackBar.Value = Global.Light.GetIntensity(3);
            this.B_num.Value = decimal.Parse(B_TrackBar.Value.ToString());
        }
       
        private void tableLayoutPanel1_MouseHover(object sender, EventArgs e)
        {
            this.LightCtrIni();
        }
        private void R_TrackBar_ValueChanged(object sender, EventArgs e)
        {
            Global.Light.SetIntensity(1, R_TrackBar.Value);
            this.R_num.Value = decimal.Parse(R_TrackBar.Value.ToString());
        }

        private void G_TrackBar_ValueChanged(object sender, EventArgs e)
        {
            Global.Light.SetIntensity(2, G_TrackBar.Value);
            this.G_num.Value = decimal.Parse(G_TrackBar.Value.ToString());
        }

        private void B_TrackBar_ValueChanged(object sender, EventArgs e)
        {
            Global.Light.SetIntensity(3, B_TrackBar.Value);
            this.B_num.Value = decimal.Parse(B_TrackBar.Value.ToString());
        }

        private void R_num_ValueChanged(object sender, EventArgs e)
        {
            int rVaue = Convert.ToInt32(R_num.Value);
            Global.Light.SetIntensity(1, rVaue);
            R_TrackBar.Value = rVaue;
        }

        private void G_num_ValueChanged(object sender, EventArgs e)
        {
            int rVaue = Convert.ToInt32(G_num.Value);
            Global.Light.SetIntensity(2, rVaue);
            G_TrackBar.Value = rVaue;
        }

        private void B_num_ValueChanged(object sender, EventArgs e)
        {
            int rVaue = Convert.ToInt32(B_num.Value);
            Global.Light.SetIntensity(3, rVaue);
            B_TrackBar.Value = rVaue;
        }

       
    }
}
