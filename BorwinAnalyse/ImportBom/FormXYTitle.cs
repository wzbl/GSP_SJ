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
    public partial class FormXYTitle : KryptonForm
    {
        public FormXYTitle()
        {
            InitializeComponent();
            this.Load+=FormXYTitle_Load;    
        }

        private void FormXYTitle_Load(object sender, EventArgs e)
        {
            Title_Positions.Load(AnaylseDataManager.Instance.XYData.Positions,"元件位置".tr());
            Title_XPos.Load(AnaylseDataManager.Instance.XYData.XPos,"X坐标".tr());
            Title_YPos.Load(AnaylseDataManager.Instance.XYData.YPos,"Y坐标".tr());
            Title_Angles.Load(AnaylseDataManager.Instance.XYData.Angles,"角度".tr());
            Title_Sides.Load(AnaylseDataManager.Instance.XYData.Sides,"板面".tr());
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            AnaylseDataManager.Instance.XYData.Positions = Title_Positions.GetData();
            AnaylseDataManager.Instance.XYData.XPos = Title_XPos.GetData();
            AnaylseDataManager.Instance.XYData.YPos = Title_YPos.GetData();
            AnaylseDataManager.Instance.XYData.Angles = Title_Angles.GetData();
            AnaylseDataManager.Instance.XYData.Sides = Title_Sides.GetData();

            AnaylseDataManager.Instance.Save();

        }
    }
}
