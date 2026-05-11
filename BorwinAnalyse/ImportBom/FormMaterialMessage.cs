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
    public partial class FormMaterialMessage : KryptonForm
    {
        public string MaterialCode { get; internal set; }
        public string MaterialName { get; internal set; }

        public FormMaterialMessage(string materialCode, string materialName)
        {
            InitializeComponent();
            txtMaterialCode.Text = materialCode;
            txtMaterialName.Text = materialName;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            MaterialName = txtMaterialName.Text;
            DialogResult = DialogResult.OK;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
