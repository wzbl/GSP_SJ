using BrowApp.Language;
using ComponentFactory.Krypton.Toolkit;
using Emgu.CV.Flann;
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
    public partial class FormLocation_Point : KryptonForm
    {
        public FormLocation_Point(List<string> position,string pos="")
        {
            InitializeComponent();
            this.position = position;
            comPosition.DataSource = position;
            comPosition.Text = pos;
        }

       

        private List<string> position;
        public string Position = "";
        public int Index = -1;
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(comPosition.Text))
            {
                 BrowApp.APP.Tip.ShowTip(1, "警告".tr(), "请选择位号".tr(), "确定".tr());
            }
            Position = comPosition.Text;
            Index = comPosition.SelectedIndex;
            for (int i = 0; i < position.Count; i++)
            {
                if (position[i]== Position)
                {
                    Index = i;
                    break;
                }
            }
            DialogResult = DialogResult.OK;
        }

        private void txtChoise_TextChanged(object sender, EventArgs e)
        {
            //comPosition.Items.Clear();
            //if (string.IsNullOrEmpty(txtChoise.Text))
            //{
            //    foreach (var item in position)
            //    {
            //        comPosition.Items.Add(item);
            //    }
            //}
            //else
            //{
            //    foreach (var item in position)
            //    {
            //        if (item.Contains(txtChoise.Text.ToUpper()))
            //        {
            //            comPosition.Items.Add(item);
            //        }
            //    }
            //}
        }

        private void btnCa_Click(object sender, EventArgs e)
        {

        }
    }
}
