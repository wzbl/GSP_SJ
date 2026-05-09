using BrowApp;
using CKVisionAppNet;
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
    public partial class UI_Prodution : UserControl
    {

        private IntPtr Mark1_imageView; //Mark1显示视图
        private IntPtr Mark2_imageView; //Mark2显示视图
        private IntPtr Normal_imageView;//匹配视图
        private IntPtr GimageView; //实时显示
        private Timer timer;
        public UI_Prodution()
        {
            InitializeComponent();

            Global.RefFormHandler += new EventHandler(R_Click);
            Global.VisionApp.ProcEndEvent += new EventHandler(Refimae);
            timer=new Timer();
            timer.Interval = 100;
            timer.Tick += Timer_Tick;
            timer.Start();
    }

        private void Timer_Tick(object sender, EventArgs e)
        {
            kryptonLabel6.Values.ExtraText = Global.TextNum.ToString();
        }

        private void Prodution_Load(object sender, EventArgs e)
        {
            Mark1_imageView = Global.VisionApp.CreateView(this.Mark1pic.Handle, this.Mark1pic.Width, this.Mark1pic.Height, 1);
            Mark2_imageView = Global.VisionApp.CreateView(this.Mark2pic.Handle, this.Mark2pic.Width, this.Mark2pic.Height, 2);
            GimageView = Global.VisionApp.CreateView(this.Mpic.Handle, this.Mpic.Width, this.Mpic.Height, 3);
            Normal_imageView = Global.VisionApp.CreateView(this.Mpic.Handle, this.Mpic.Width - 250, this.Mpic.Height - 250, 4);

            Global.VisionApp.SetView(Mark1_imageView,"Task3", "MARK1" , "Mark1_imageView");
            Global.VisionApp.SetView(Mark2_imageView,  "Task3", "MARK2" , "Mark2_imageView");
            Global.VisionApp.SetView(GimageView, "Task5", "采集图像", "GimageView");
            Global.VisionApp.SetView(Normal_imageView, "Task4", "采集图像", "Normal_imageView");

            BrowApp.Language.Language.Instance.UpdateLanguage(this, null);
        }
        private void Mark1pic_Resize(object sender, EventArgs e)
        {
            Global.VisionApp.MoveView(Mark1_imageView, this.Mark1pic.Width, this.Mark1pic.Height);
        }
        private void Mark2pic_Resize(object sender, EventArgs e)
        {
            Global.VisionApp.MoveView(Mark2_imageView, this.Mark2pic.Width, this.Mark2pic.Height);
        }
        private void Mpic_Resize(object sender, EventArgs e)
        {
            Global.VisionApp.MoveView(GimageView, this.Mpic.Width, this.Mpic.Height);
        }

        private void ContinuouBtn_Click(object sender, EventArgs e)
        {
            if (Global.CamGlab)
            {
                Global.CamGlab = false;
                Global.VisionApp.StopRunProc("Task5");
            }
            else
            {
                Global.CamGlab = true;
                Global.VisionApp.RunProc("Task5");
            }
        }
        void Refimae(object sender, EventArgs e)
        {
            try
            {
                switch (sender)
                {
                    case "Task4":
                        Global.VisionApp.CenterZoom(Normal_imageView, 0.6);
                        Global.VisionApp.RedrawView(Normal_imageView);
                        break;
                    case "Task5":
                        Global.VisionApp.RedrawView(GimageView);
                        break;
                    case "Task3":
                        Global.VisionApp.RedrawView(Mark1_imageView);
                        Global.VisionApp.RedrawView(Mark2_imageView);
                        break;
                }

            }
            catch { }
        }
        private void R_Click(object sender, EventArgs e)
        {
            string FBCCode;
            string XYCode;
            string BoardSide;
            string ProductCode;
            string ProductName;
            Global.ReadFAIConfig(out FBCCode, out XYCode, out BoardSide, out ProductCode, out ProductName);

            FBCCode_Lab.Values.ExtraText = FBCCode;
            XYCode_lab.Values.ExtraText = XYCode;
            BoardSide_lab.Values.ExtraText = BoardSide;
            ProductCode_lab.Values.ExtraText = ProductCode;
            ProductName_lab.Values.ExtraText = ProductName;

        }
    }
}
