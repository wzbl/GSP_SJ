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
            foreach (var item in position)
            {
                comPosition.Items.Add(item);
            }
            this.position = position;
            comPosition.Text = pos;
            UpdateLanguage();
        }

        private void UpdateLanguage()
        {
            BrowApp.Language.Language.Instance.UpdateLanguage(this, null);
        }

        private List<string> position;
        public string Position = "";
        public int Index = -1;
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(comPosition.Text))
            {
                MessageBox.Show("请选择位号");
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
