using BorwinAnalyse.Forms;
using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Toolkit;
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
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            //kryptonPage1.Controls.Add(new UCAllProgram());
            //kryptonPage2.Controls.Add(new UCAllReport());
            Global.ProguceItem+=(x) =>
            {
                if (x != null)
                {
                    kryptonNavigator1.Pages.Add(new KryptonPage(x.产品编号));
                    UCProgram uCProgram = new UCProgram(x);
                    kryptonNavigator1.Pages[kryptonNavigator1.Pages.Count - 1].Controls.Add(uCProgram);
                    kryptonNavigator1.SelectedPage=kryptonNavigator1.Pages[kryptonNavigator1.Pages.Count - 1];
                }
            };
        }

        private void 分析规则ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormBomSet formBomSet = new FormBomSet();
            formBomSet.ShowDialog();
        }

        private void 资料库ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FormBasMaterial formBasMaterial = new FormBasMaterial();
            formBasMaterial.ShowDialog();
        }

        private void 尺寸管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSize formSize = new FormSize();
            formSize.ShowDialog();
        }

        private void 电桥配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormCompensationSet formCompensationSet = new FormCompensationSet();
            formCompensationSet.ShowDialog();
        }

        private void 模板库ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormModel formModel = new FormModel();
            formModel.ShowDialog();
        }

        private void 系统参数ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 电桥参数ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormCompensationParam formCompensationParam = new FormCompensationParam();
            formCompensationParam.ShowDialog();
        }

        private void btnAddProduce_Click(object sender, EventArgs e)
        {
            //FormProductItem form = new FormProductItem();
            //form.ShowDialog();
            string text = "程序";
            kryptonNavigator1.Pages.Add(new KryptonPage(text));
            kryptonNavigator1.Pages[kryptonNavigator1.Pages.Count - 1].Controls.Add(new UCProgram());
            kryptonNavigator1.SelectedPage = kryptonNavigator1.Pages[kryptonNavigator1.Pages.Count - 1];
        }

        private void btnAddReport_Click(object sender, EventArgs e)
        {

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
                kryptonNavigator1.Pages.Add(new KryptonPage(text));
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
                kryptonNavigator1.Pages.Add(new KryptonPage(text));
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
    }
}
