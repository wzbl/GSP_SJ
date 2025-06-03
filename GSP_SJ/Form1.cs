using MyTestWcfServiceLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSP_SJ
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        ServiceReference1.MyFirstServiceClient client = new ServiceReference1.MyFirstServiceClient();

        private void btnOpenServices_Click(object sender, EventArgs e)
        {
           string s= client.GetData(1);
        }

        private void btnCloseServices_Click(object sender, EventArgs e)
        {
            CompositeType composite= client.GetDataUsingDataContract(new MyTestWcfServiceLibrary.CompositeType());
        }
    }
}
