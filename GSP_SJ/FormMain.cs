using BorwinAnalyse.Forms;
using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Toolkit;
using GSP_SJ.DBForm;
using GSP_SJ.ModelClass;
using SqlHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace GSP_SJ
{
    public partial class FormMain : KryptonForm
    {
        public FormMain()
        {
            InitializeComponent();
            this.Shown += FormMain_Shown;
            NewRibbonRecentDoc("用户管理", 0, Properties.Resources.create1_24px);
            NewRibbonRecentDoc("站位表", 1, Properties.Resources.create1_24px);
            NewRibbonRecentDoc("资料库", 2, Properties.Resources.create1_24px);
            NewRibbonRecentDoc("尺寸管理", 3, Properties.Resources.create1_24px);
            NewRibbonRecentDoc("电桥配置", 4, Properties.Resources.create1_24px);
            NewRibbonRecentDoc("电桥参数", 5, Properties.Resources.create1_24px);
            NewRibbonRecentDoc("模板库", 6, Properties.Resources.create1_24px);
            NewRibbonRecentDoc("语言", 7, Properties.Resources.create1_24px);
        }
        private void FormMain_Shown(object sender, EventArgs e)
        {
            Global.ProguceItem+=(x) =>
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

            Global.ReportItem += (x) =>
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
            lbUser.TextLine1 = Global.User.UserName;
        }

        public KryptonPage GetKryptonPage(string text)
        {
            KryptonPage page = new KryptonPage(text);
            ButtonSpecAny  button=new ButtonSpecAny();
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
            this.kryptonRibbon1.RibbonAppButton.AppButtonMenuItems.Add(kryptonRibbonRecentDoc);
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
            this.kryptonRibbon1.RibbonAppButton.AppButtonImage = ((KryptonContextMenuItem)sender).Image;
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
    }
}
