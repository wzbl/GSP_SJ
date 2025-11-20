using BorwinAnalyse.BaseClass;
using BorwinAnalyse.UCControls;
using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using NPOI.OpenXmlFormats.Spreadsheet;
using BrowApp.Language;

namespace BorwinAnalyse.Forms
{
    public partial class AnalyseMainForm : Form
    {
        public AnalyseMainForm()
        {
            InitializeComponent();
            this.components = new System.ComponentModel.Container();
            f = this;
            
        }

        //string SoftVersion = "V1.001";                                       //初版
        //string SoftUpdateTime = "2025/01/13";
        //string SoftVersion = "V1.002";                                       //输出文档
        //string SoftUpdateTime = "2025/02/27";
        //string SoftVersion = "V1.003";                                       //替代料号
        //string SoftUpdateTime = "2025/03/10";
        string SoftVersion = "V1.004";                                         //快速查询（含料带信息）
        string SoftUpdateTime = "2025/03/13";

        UCBOM uCBOM;
        UCSearchBom uCSearch;
        UCAnalyseSet uCAnalyseSet ;

        public static AnalyseMainForm f;

        public void ShowModelData()
        {
            if (uCSearch == null)
            {
                uCSearch = new UCSearchBom();
                kryptonSplitContainer2.Panel2.Controls.Add(uCSearch);
            }
        }

        private void AnalyseMainForm_Load(object sender, EventArgs e)
        {
            if (this.components == null)
            {
                this.components = new System.ComponentModel.Container();
            }
            this.Text += " " + SoftVersion + " " + SoftUpdateTime;
        }


        private void KryptonButton_Click(object sender, EventArgs e)
        {
            switch (((KryptonButton)sender).Tag)
            {
                case "BOM":
                    if (uCBOM == null)
                    {
                        uCBOM = new UCBOM();
                        kryptonSplitContainer2.Panel2.Controls.Add(uCBOM);
                    }
                    uCBOM.BringToFront();
                    break;
                case "查询":
                    if (uCSearch==null)
                    {
                        uCSearch = new UCSearchBom();
                        kryptonSplitContainer2.Panel2.Controls.Add(uCSearch);
                    }
                    //uCSearch.ucSearchBom1.ComModelUpdata();
                    uCSearch.BringToFront();
                    break;
                case "设置":
                    if (uCAnalyseSet == null)
                    {
                        uCAnalyseSet = new UCAnalyseSet();
                        kryptonSplitContainer2.Panel2.Controls.Add(uCAnalyseSet);
                    }
                    uCAnalyseSet.BringToFront();
                    break;
                default:
                    break;
            }
        }


        private void AnalyseMainForm_Shown(object sender, EventArgs e)
        {
            if (uCBOM == null)
            {
                uCBOM = new UCBOM();
                kryptonSplitContainer2.Panel2.Controls.Add(uCBOM);
            }
            uCBOM.BringToFront();
        }

     

        private void kryptonButton5_Click(object sender, EventArgs e)
        {
           
        }
    }


}
