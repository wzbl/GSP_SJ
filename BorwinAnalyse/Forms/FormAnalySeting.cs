using BorwinAnalyse.UCControls;
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

namespace BorwinAnalyse.Forms
{
    public partial class FormAnalySeting : KryptonForm
    {
        public FormAnalySeting()
        {
            InitializeComponent();

        }

        public FormAnalySeting(string ruleName):this()
        {
            this.Controls.Add(new UCAnalyseSet(ruleName));
        }
    }
}
