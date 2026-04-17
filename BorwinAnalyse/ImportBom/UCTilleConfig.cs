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
    public partial class UCTilleConfig : UserControl
    {
        public UCTilleConfig()
        {
            InitializeComponent();
        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            listTitle.Items.Add("");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {

        }
    }
}
