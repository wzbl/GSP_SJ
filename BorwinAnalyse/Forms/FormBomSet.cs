using BorwinAnalyse.UCControls;
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
    public partial class FormBomSet :Form
    {
        public FormBomSet()
        {
            InitializeComponent();
            this.Load+=FormBomSet_Load; 
        }

        private void FormBomSet_Load(object sender, EventArgs e)
        {
            this.Controls.Add(new UCAnalyseSet());
        }
    }
}
