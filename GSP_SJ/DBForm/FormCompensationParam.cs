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
    public partial class FormCompensationParam : Form
    {
        public FormCompensationParam()
        {
            InitializeComponent();

            this.Load += FormCompensationParam_Load;
            this.Shown+= FormCompensationParam_Shown;
        }

        private void FormCompensationParam_Shown(object sender, EventArgs e)
        {
            if(comOptions.Items.Count>0)
            comOptions.Text = comOptions.Items[0].ToString();
        }

        private void FormCompensationParam_Load(object sender, EventArgs e)
        {
            List<Eng_MeterOption> _Customers = SQLDataControl.GetMeterOption();
            for (int i = 0; i < _Customers.Count; i++)
            {
                comOptions.Items.Add(_Customers[i].OptionName);
                if (i == 0)
                {
                    //comOptions.Text = _Customers[i].OptionName;
                    //Search(comOptions.Text);
                }
            }
        }

        private void Search(string meterName = "")
        {
            //dgvProgram.Rows.Clear();
            dgvProgram.DataSource = null;
            List <Eng_MeterOptionItem> items = SQLDataControl.GetMeterOptionItem(meterName);
            dgvProgram.DataSource = items;
        }

        private void comOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            Search(comOptions.Text);
        }

        /// <summary>
        /// 复制表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            FormMeter formMeter = new FormMeter();
            if (formMeter.ShowDialog() == DialogResult.OK)
            {
                SQLDataControl.CopyMeterOption(formMeter.NewName,comOptions.Text);
                comOptions.Items.Clear();
                List<Eng_MeterOption> _Customers = SQLDataControl.GetMeterOption();
                for (int i = 0; i < _Customers.Count; i++)
                {
                    comOptions.Items.Add(_Customers[i].OptionName);
                    if (i == _Customers.Count-1)
                    {
                        comOptions.Text = _Customers[i].OptionName;
                        Search(comOptions.Text);
                    }
                }
            }

        }
        /// <summary>
        /// 重命名表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void kryptonButton2_Click(object sender, EventArgs e)
        {
            if (comOptions.SelectedIndex == 0)
            {
                MessageBox.Show("默认参数不可修改");
                return;
            }
            FormMeter formMeter = new FormMeter();
            if (formMeter.ShowDialog() == DialogResult.OK)
            {
                SQLDataControl.RenameMeterOption(formMeter.NewName, comOptions.Text);

                List<Eng_MeterOption> _Customers = SQLDataControl.GetMeterOption();
                comOptions.Items.Clear();
                for (int i = 0; i < _Customers.Count; i++)
                {
                    comOptions.Items.Add(_Customers[i].OptionName);
                    if (formMeter.NewName== _Customers[i].OptionName)
                    {
                        comOptions.Text = _Customers[i].OptionName;
                        //Search(comOptions.Text);
                    }
                }
            }
        }

        /// <summary>
        /// 删除表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void kryptonButton3_Click(object sender, EventArgs e)
        {
            if (comOptions.SelectedIndex == 0)
            {
                MessageBox.Show("默认参数不可修改");
                return;
            }
            SQLDataControl.DeleteMeterOption(comOptions.Text);
            comOptions.Items.Clear();
            List<Eng_MeterOption> _Customers = SQLDataControl.GetMeterOption();
            for (int i = 0; i < _Customers.Count; i++)
            {
                comOptions.Items.Add(_Customers[i].OptionName);
                if (i == 0)
                {
                    comOptions.Text = _Customers[i].OptionName;
                    //Search(comOptions.Text);
                }
            }
        }

        
        /// <summary>
        /// 新增行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddRow_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 删除行
        /// </summary>
        private void btnDeleteRow_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 保存
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {

        }
    }
}
