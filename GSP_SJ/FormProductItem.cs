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
    public partial class FormProductItem : Form
    {
        public FormProductItem()
        {
            InitializeComponent();
            UCProgram uCProgram = new UCProgram();
            this.Controls.Add(uCProgram);
        }

        public FormProductItem(View_Eng_Program view_Eng_Program)
        {
            InitializeComponent();
            UCProgram uCProgram = new UCProgram(view_Eng_Program);
            this.Controls.Add(uCProgram);
        }
    }
}
