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
    public partial class FormAddRule : KryptonForm
    {
        public FormAddRule()
        {
            InitializeComponent();
            UpdateLanguage();
        }
        private void UpdateLanguage()
        {
            BrowApp.Language.Language.Instance.UpdateLanguage(this, null);
        }

        public FormAddRule(string rule) : this()
        {
            txtRule.Text = rule;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(txtRule.Text))
            {
                MessageBox.Show("请输入规则名称");
                return;
            }

            if (AnaylseDataManager.Instance.Rules.Contains(txtRule.Text))
            {
                MessageBox.Show("规则名称已存在");
                return;
            }

            AnaylseDataManager.Instance.Rules.Add(txtRule.Text);
            AnaylseDataManager.Instance.Save();
            DialogResult = DialogResult.OK;
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
