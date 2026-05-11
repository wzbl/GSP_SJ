using ComponentFactory.Krypton.Toolkit;
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

namespace GSP_SJ
{
    public partial class FormProductItem : KryptonForm
    {
        public FormProductItem()
        {
            InitializeComponent();
            UCProgram uCProgram = new UCProgram();
            this.Controls.Add(uCProgram);
            UpdateLanguage();
        }
        private void UpdateLanguage()
        {
            BrowApp.Language.Language.Instance.UpdateLanguage(this, null);
        }

        public FormProductItem(View_Eng_Program view_Eng_Program)
        {
            InitializeComponent();
            UCProgram uCProgram = new UCProgram(view_Eng_Program);
            this.Controls.Add(uCProgram);
        }
    }
}
