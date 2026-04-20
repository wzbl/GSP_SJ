using BrowApp.Language;
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

namespace BorwinAnalyse.ImportBom
{
    public partial class FormBomTitle : KryptonForm
    {
        public FormBomTitle()
        {
            InitializeComponent();
            this.Load += FormLoad;
        }

        private void FormLoad(object sender, EventArgs e)
        {
            Title_MaterialCodes.Load(AnaylseDataManager.Instance.Bom.MaterialCodes,"物料编号".tr());
            Title_MaterialDescriptions.Load(AnaylseDataManager.Instance.Bom.MaterialDescriptions,"物料描述".tr());
            Title_QTYs.Load(AnaylseDataManager.Instance.Bom.QTYs,"用量".tr());
            Title_Positions.Load(AnaylseDataManager.Instance.Bom.Positions,"元件位置".tr());
            Title_ReplaceMaterials.Load(AnaylseDataManager.Instance.Bom.ReplaceMaterials,"替代料".tr());
        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            AnaylseDataManager.Instance.Bom.MaterialCodes= Title_MaterialCodes.GetData();
            AnaylseDataManager.Instance.Bom.MaterialDescriptions = Title_MaterialDescriptions.GetData();
            AnaylseDataManager.Instance.Bom.QTYs = Title_QTYs.GetData();
            AnaylseDataManager.Instance.Bom.Positions = Title_Positions.GetData();
            AnaylseDataManager.Instance.Bom.ReplaceMaterials = Title_ReplaceMaterials.GetData();
            AnaylseDataManager.Instance.Save();
        }
    }
}
