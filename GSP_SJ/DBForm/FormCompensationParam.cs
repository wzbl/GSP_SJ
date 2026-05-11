using ComponentFactory.Krypton.Toolkit;
using SqlHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
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
            this.Shown += FormCompensationParam_Shown;
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        private void FormCompensationParam_Shown(object sender, EventArgs e)
        {
            if (comOptions.Items.Count > 0)
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
            bindComboxData("LcrType", 1);
            bindComboxData("Frequency", 7);
            bindComboxData("Voltage", 8);
            bindComboxData("RangeType", 9);
            bindComboxData("Range", 10);
            bindComboxData("Speed", 12);
            bindComboxData("Resistance", 13);
            dgvProgram.CellClick += DgvProgram_CellClick;

            UpdateLanguage();
        }
        private void UpdateLanguage()
        {
            BrowApp.Language.Language.Instance.UpdateLanguage(this, null);
        }
        private void DgvProgram_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                dgvProgram.Rows[e.RowIndex].Selected = true;
            }
        }

        private void bindComboxData(string LcrType, int colIndex)
        {
            List<Eng_Meter> _Meters = SQLDataControl.GetEng_MeterBySort(LcrType);
            List<string> types = new List<string>();
            for (int i = 0; i < _Meters.Count; i++)
            {
                types.Add(_Meters[i].ShowValue);
            }
            DBEventAction.DgvColBindData(dgvProgram, colIndex, types);
        }
        List<Eng_MeterOptionItem> items = new List<Eng_MeterOptionItem>();
        private void Search(string meterName = "")
        {
            dgvProgram.Rows.Clear();
            items = SQLDataControl.GetMeterOptionItem(meterName);
            for (int i = 0; i < items.Count; i++)
            {
                dgvProgram.Rows.Add
                    (
                    items[i].Row,
                   SQLDataControl.GetEng_MeterBySaveValue("LcrType", items[i].LcrType),
                   items[i].MinValue,
                   "",                 //最小值单位，列3
                   items[i].MaxValue,
                   "",                //最大值单位，列5
                   "",                  //测试类型，列6
                   SQLDataControl.GetEng_MeterBySaveValue("Frequency", items[i].Frequency),
                   SQLDataControl.GetEng_MeterBySaveValue("Voltage", items[i].Voltage),
                   SQLDataControl.GetEng_MeterBySaveValue("RangeType", items[i].RangeType),
                   items[i].Range,
                   items[i].HoldTimes,
                   SQLDataControl.GetEng_MeterBySaveValue("Speed", items[i].Speed),
                   SQLDataControl.GetEng_MeterBySaveValue("Resistance", items[i].Resistance),
                    items[i].ReadCount,
                    items[i].FRValue,
                    items[i].MinValidValue,
                    "",                  //下限单位，列17
                    items[i].MaxValidValue,
                    "",                   //上限单位，列19
                    items[i].Remarks
                    );
                string type = items[i].LcrType;
                BindData(type, i);
            }
        }

        private void BindData(string type, int row)
        {
            string unit = "RUnit";
            string funcType = "RFunctionType";
            switch (type)
            {
                case "R":
                case "电阻":
                    //电阻
                    bindCellComboxData("RUnit", row, 3);
                    bindCellComboxData("RUnit", row, 5);
                    bindCellComboxData("RUnit", row, 17);
                    bindCellComboxData("RUnit", row, 19);
                    bindCellComboxData("RFunctionType", row, 6);
                    unit = "RUnit";
                    funcType = "RFunctionType";
                    break;

                case "C":
                case "电容":
                    //电容

                    bindCellComboxData("CUnit", row, 3);
                    bindCellComboxData("CUnit", row, 5);
                    bindCellComboxData("CUnit", row, 17);
                    bindCellComboxData("CUnit", row, 19);
                    bindCellComboxData("CFunctionType", row, 6);
                    unit = "CUnit";
                    funcType = "CFunctionType";
                    break;

                case "L":
                case "电感":
                    //电感
                    bindCellComboxData("LUnit", row, 3);
                    bindCellComboxData("LUnit", row, 5);
                    bindCellComboxData("LUnit", row, 17);
                    bindCellComboxData("LUnit", row, 19);
                    bindCellComboxData("LFunctionType", row, 6);
                    unit = "LUnit";
                    funcType = "LFunctionType";
                    break;

                case "B":
                case "DCR":
                    //DCR
                    bindCellComboxData("DCRUnit", row, 3);
                    bindCellComboxData("DCRUnit", row, 5);
                    bindCellComboxData("DCRUnit", row, 17);
                    bindCellComboxData("DCRUnit", row, 19);
                    bindCellComboxData("DCRFunctionType", row, 6);
                    unit = "DCRUnit";
                    funcType = "DCRFunctionType";
                    break;

                case "D":
                case "LED":
                    //LED
                    bindCellComboxData("LEDUnit", row, 3);
                    bindCellComboxData("LEDUnit", row, 5);
                    bindCellComboxData("LEDUnit", row, 17);
                    bindCellComboxData("LEDUnit", row, 19);
                    bindCellComboxData("LEDFunctionType", row, 6);
                    funcType = "LEDFunctionType";
                    unit = "LEDUnit";
                    break;
            }
            if (row < items.Count)
            {
                dgvProgram.Rows[row].Cells[3].Value = SQLDataControl.GetEng_MeterBySaveValue(unit, items[row].MinValueUnit);
                dgvProgram.Rows[row].Cells[5].Value = SQLDataControl.GetEng_MeterBySaveValue(unit, items[row].MaxValueUnit);
                dgvProgram.Rows[row].Cells[6].Value = SQLDataControl.GetEng_MeterBySaveValue(funcType, items[row].FunctionType);
                dgvProgram.Rows[row].Cells[17].Value = SQLDataControl.GetEng_MeterBySaveValue(unit, items[row].MinValidValueUnit);
                dgvProgram.Rows[row].Cells[19].Value = SQLDataControl.GetEng_MeterBySaveValue(unit, items[row].MaxValidValueUnit);
            }
        }

        public void GetUnitAndFuncType(string type, out string unit, out string funcType)
        {
            unit = "RUnit";
            funcType = "RFunctionType";
            switch (type)
            {
                case "R":
                case "电阻":

                    unit = "RUnit";
                    funcType = "RFunctionType";
                    break;

                case "C":
                case "电容":
                    //电容
                    unit = "CUnit";
                    funcType = "CFunctionType";
                    break;

                case "L":
                case "电感":
                    //电感

                    unit = "LUnit";
                    funcType = "LFunctionType";
                    break;

                case "B":
                case "DCR":
                    //DCR

                    unit = "DCRUnit";
                    funcType = "DCRFunctionType";
                    break;

                case "D":
                case "LED":
                    //LED

                    funcType = "LEDFunctionType";
                    unit = "LEDUnit";
                    break;
            }
        }

        private void bindCellComboxData(string LcrType, int rowIndex, int colIndex)
        {
            List<Eng_Meter> _Meters = SQLDataControl.GetEng_MeterBySort(LcrType);
            List<string> types = new List<string>();
            for (int i = 0; i < _Meters.Count; i++)
            {
                types.Add(_Meters[i].ShowValue);
            }
            DBEventAction.DgvColBindData(dgvProgram, rowIndex, colIndex, types);
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
                SQLDataControl.CopyMeterOption(formMeter.NewName, comOptions.Text);
                comOptions.Items.Clear();
                List<Eng_MeterOption> _Customers = SQLDataControl.GetMeterOption();
                for (int i = 0; i < _Customers.Count; i++)
                {
                    comOptions.Items.Add(_Customers[i].OptionName);
                    if (i == _Customers.Count - 1)
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
                    if (formMeter.NewName == _Customers[i].OptionName)
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
            try
            {
                int value = int.Parse(dgvProgram.Rows[dgvProgram.Rows.Count - 1].Cells[0].Value.ToString());
                dgvProgram.Rows.
                    Add(
                    value + 1,
                     "",
                    dgvProgram.Rows[dgvProgram.Rows.Count - 1].Cells[2].Value,
                     "",
                    dgvProgram.Rows[dgvProgram.Rows.Count - 1].Cells[4].Value,
                    "",
                    "",
                    dgvProgram.Rows[dgvProgram.Rows.Count - 1].Cells[7].Value,
                    dgvProgram.Rows[dgvProgram.Rows.Count - 1].Cells[8].Value,
                    dgvProgram.Rows[dgvProgram.Rows.Count - 1].Cells[9].Value,
                    dgvProgram.Rows[dgvProgram.Rows.Count - 1].Cells[10].Value,
                    dgvProgram.Rows[dgvProgram.Rows.Count - 1].Cells[11].Value,
                    dgvProgram.Rows[dgvProgram.Rows.Count - 1].Cells[12].Value,
                    dgvProgram.Rows[dgvProgram.Rows.Count - 1].Cells[13].Value,
                    dgvProgram.Rows[dgvProgram.Rows.Count - 1].Cells[14].Value,
                    dgvProgram.Rows[dgvProgram.Rows.Count - 1].Cells[15].Value,
                    dgvProgram.Rows[dgvProgram.Rows.Count - 1].Cells[16].Value,
                    "",
                    dgvProgram.Rows[dgvProgram.Rows.Count - 1].Cells[18].Value,
                    "",
                    ""
                    );
            }
            catch (Exception)
            {

            }

        }

        /// <summary>
        /// 删除行
        /// </summary>
        private void btnDeleteRow_Click(object sender, EventArgs e)
        {

            if (dgvProgram.SelectedRows.Count > 0)
            {
                string row = dgvProgram.SelectedRows[0].Cells[0].Value.ToString();

                if (comOptions.SelectedIndex == 0 && items.Where(x => x.Row == int.Parse(row)).ToList().Count > 0)
                {
                    MessageBox.Show("默认参数不可修改");
                    return;
                }
                else
                {
                    SQLDataControl.DeleteEng_MeterOptionItem(comOptions.Text, int.Parse(row));
                }

                dgvProgram.Rows.RemoveAt(dgvProgram.SelectedRows[0].Index);
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < dgvProgram.Rows.Count; i++)
                {
                    string optionName = comOptions.Text;
                    int row = -1;
                    if (int.TryParse(DBEventAction.GetDGVCellValue(dgvProgram, i, 0), out int r))
                    {
                        row = r;
                    }
                    else
                    {
                        MessageBox.Show("行号错误");
                        return;
                    }
                    string LcrType = SQLDataControl.GetEng_MeterByShowValue("LcrType", DBEventAction.GetDGVCellValue(dgvProgram, i, 1));
                    GetUnitAndFuncType(LcrType, out string unit, out string funcType);
                    Nullable<decimal> MinValue;
                    if (decimal.TryParse(DBEventAction.GetDGVCellValue(dgvProgram, i, 2), out decimal minV))
                    {
                        MinValue = minV;
                    }
                    else
                    {
                        MessageBox.Show("最小值错误");
                        return;
                    }
                    string MinValueUnit = SQLDataControl.GetEng_MeterByShowValue(unit, DBEventAction.GetDGVCellValue(dgvProgram, i, 3));
                    Nullable<decimal> MaxValue;
                    if (decimal.TryParse(DBEventAction.GetDGVCellValue(dgvProgram, i, 4), out decimal maxV))
                    {
                        MaxValue = maxV;
                    }
                    else
                    {
                        MessageBox.Show("最大值错误");
                        return;
                    }
                    string MaxValueUnit = SQLDataControl.GetEng_MeterByShowValue(unit, DBEventAction.GetDGVCellValue(dgvProgram, i, 5));
                    string FunctionType = SQLDataControl.GetEng_MeterByShowValue(funcType, DBEventAction.GetDGVCellValue(dgvProgram, i, 6));
                    string Frequency = SQLDataControl.GetEng_MeterByShowValue("Frequency", DBEventAction.GetDGVCellValue(dgvProgram, i, 7));
                    string Voltage = SQLDataControl.GetEng_MeterByShowValue("Voltage", DBEventAction.GetDGVCellValue(dgvProgram, i, 8));
                    string RangeType = SQLDataControl.GetEng_MeterByShowValue("RangeType", DBEventAction.GetDGVCellValue(dgvProgram, i, 9));
                    string Range = DBEventAction.GetDGVCellValue(dgvProgram, i, 10);
                    Nullable<int> HoldTimes;
                    if (int.TryParse(DBEventAction.GetDGVCellValue(dgvProgram, i, 11), out int timers))
                    {
                        HoldTimes = timers;
                    }
                    else
                    {
                        MessageBox.Show("失败次数错误");
                        return;
                    }
                    string Speed = SQLDataControl.GetEng_MeterByShowValue("Speed", DBEventAction.GetDGVCellValue(dgvProgram, i, 12));
                    string Resistance = SQLDataControl.GetEng_MeterByShowValue("Resistance", DBEventAction.GetDGVCellValue(dgvProgram, i, 13));
                    Nullable<int> ReadCount;
                    if (int.TryParse(DBEventAction.GetDGVCellValue(dgvProgram, i, 14), out int count))
                    {
                        ReadCount = count;
                    }
                    else
                    {
                        MessageBox.Show("取值次数错误");
                        return;
                    }
                    Nullable<decimal> FRValue;
                    if (decimal.TryParse(DBEventAction.GetDGVCellValue(dgvProgram, i, 15), out decimal frv))
                    {
                        FRValue = frv;
                    }
                    else
                    {
                        MessageBox.Show("波动区间错误");
                        return;
                    }
                    Nullable<decimal> MinValidValue;
                    if (decimal.TryParse(DBEventAction.GetDGVCellValue(dgvProgram, i, 16), out decimal minValidValue))
                    {
                        MinValidValue = minValidValue;
                    }
                    else
                    {
                        MessageBox.Show("有效值下限错误");
                        return;
                    }
                    string MinValidValueUnit = SQLDataControl.GetEng_MeterByShowValue(unit, DBEventAction.GetDGVCellValue(dgvProgram, i, 17));
                    Nullable<decimal> MaxValidValue;
                    if (decimal.TryParse(DBEventAction.GetDGVCellValue(dgvProgram, i, 18), out decimal maxValidValue))
                    {
                        MaxValidValue = maxValidValue;
                    }
                    else
                    {
                        MessageBox.Show("有效值上限错误");
                        return;
                    }
                    string MaxValidValueUnit = SQLDataControl.GetEng_MeterByShowValue(unit, DBEventAction.GetDGVCellValue(dgvProgram, i, 19));
                    string Remarks = DBEventAction.GetDGVCellValue(dgvProgram, i, 20);

                    SQLDataControl.AddEng_MeterOptionItem(optionName, row, LcrType, MinValue, MinValueUnit, MaxValue, MaxValueUnit, FunctionType, Frequency, Voltage, RangeType, Range, HoldTimes, Speed, Resistance, ReadCount, FRValue, MinValidValue, MinValidValueUnit, MaxValidValue, MaxValidValueUnit, Remarks);
                }

                MessageBox.Show("保存成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
          
        }

        private void dgvProgram_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 1)
            {
                //切换元件类型

                string type = dgvProgram.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                BindData(type, e.RowIndex);
            }
        }
    }
}
