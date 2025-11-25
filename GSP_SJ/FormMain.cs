using BorwinAnalyse.Forms;
using GSP_SJ.ModelClass;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSP_SJ
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            this.Shown += FormMain_Shown;
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            kryptonPage1.Controls.Add(new UCAllProgram());
            kryptonPage2.Controls.Add(new UCAllReport());
            DeepOCRHelper.Init();
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
    }
}
