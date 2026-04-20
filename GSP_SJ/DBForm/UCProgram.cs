using BorwinAnalyse.BaseClass;
using BorwinAnalyse.Forms;
using BorwinAnalyse.ImportBom;
using BorwinAnalyse.UCControls;
using ComponentFactory.Krypton.Ribbon;
using GSP_SJ.ModelClass;
using SqlHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace GSP_SJ
{
    public partial class UCProgram : UserControl
    {
        private string ProductCode = "";
        UCXYDataChart uCXYDataChart;
        public UCProgram()
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
            this.Load += UCProgram_Load;

            comBomRule.Items.Clear();

            for (int i = 0; i < AnaylseDataManager.Instance.Rules.Count; i++)
            {
                comBomRule.Items.Add(AnaylseDataManager.Instance.Rules[i]);
            }
            if (AnaylseDataManager.Instance.Rules.Count > 0)
                comBomRule.Text = AnaylseDataManager.Instance.Rules[0];
        }
        public UCProgram(View_Eng_Program view_Eng_Program) : this()
        {
            txtProductCode.Enabled = false;
            this.ProductCode = view_Eng_Program.产品编号;
            txtProductCode.Text = view_Eng_Program.产品编号;
            txtProductName.Text = view_Eng_Program.产品名称;
            txtCustom.Text = view_Eng_Program.客户;
            switch (view_Eng_Program.板面)
            {
                case "S":
                    ComSide.SelectedIndex = 0;
                    break;
                case "D1":
                    ComSide.SelectedIndex = 1;
                    break;
                case "D2":
                    ComSide.SelectedIndex = 2;
                    break;
                default:
                    ComSide.SelectedIndex = 0;
                    break;
            }

            string ruleName = view_Eng_Program.分析规则;
            if (AnaylseDataManager.Instance.Rules.Contains(ruleName))
            {
                comBomRule.Text = ruleName;
            }
        }


        List<P_Search_Eng_Bom_Result> p_Search_Engs = new List<P_Search_Eng_Bom_Result>();
        List<P_Search_Eng_XYData_Result> eng_XYData_Results = new List<P_Search_Eng_XYData_Result>();

        private void UCProgram_Load(object sender, EventArgs e)
        {
            uCXYDataChart = new UCXYDataChart();
            this.kryptonPage3.Controls.Add(uCXYDataChart);
            RefreshBomData();
            RefreshXYData();
            RefreshModelData();
        }

        private void RefreshModelData()
        {

        }

        private void RefreshBomData()
        {
            p_Search_Engs = SQLDataControl.SearchBom(txtProductCode.Text);
            //if (p_Search_Engs.Count > 0)
            {
                DataGridView_BOM.DataSource = p_Search_Engs;
            }
        }

        private void RefreshXYData()
        {
            eng_XYData_Results = SQLDataControl.SearchXYData(txtProductCode.Text);
            //if (eng_XYData_Results.Count > 0)
            {
                dgvXYData.DataSource = eng_XYData_Results;
                uCXYDataChart.canvas.ProductCode = txtProductCode.Text;
                uCXYDataChart.RefreshData(eng_XYData_Results);
            }
        }

        private void btnExportXYData_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtProductCode.Text))
            {
                MessageBox.Show("请输入产品编号");
                return;
            }
            if (string.IsNullOrEmpty(ComSide.Text))
            {
                MessageBox.Show("请选择板面");
                return;
            }
            if (MessageBox.Show("当前数据将被清除,是否导入XY数据?", "提示", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                //取消
                return;
            }
            else
            {
                DataGridView_BOM.DataSource = null;
                dgvXYData.DataSource = null;
                try
                {
                    //删除数据,导入
                    SQLDataControl.DeleteBom(txtProductCode.Text);
                    SQLDataControl.DeleteXYData(txtProductCode.Text);
                }
                catch (Exception)
                {

                }


            }
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = true;
            dlg.Filter = "xlsx|*.xlsx;*.xls;*.XLS";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string[] path = dlg.FileNames;
                string bomFile = "";
                string xyFile = "";
                for (int i = 0; i < path.Length; i++)
                {
                    string fileName = System.IO.Path.GetFileName(path[i]).Split('.')[0];
                    if (fileName.EndsWith("BOM") || fileName.EndsWith("bom"))
                    {
                        bomFile = path[i];
                    }
                    else if (fileName.EndsWith("XY") || fileName.EndsWith("xy"))
                    {
                        xyFile = path[i];
                    }
                }
                FormImPortBom formImPortBom = new FormImPortBom(bomFile, xyFile, txtProductCode.Text, ComSide.Text);
                if (formImPortBom.ShowDialog() == DialogResult.OK)
                {
                    p_Search_Engs = formImPortBom.p_Search_Engs;
                    eng_XYData_Results = formImPortBom.eng_XYData_Results;
                    DataGridView_BOM.DataSource = p_Search_Engs;
                    dgvXYData.DataSource = eng_XYData_Results;
                    uCXYDataChart.canvas.ProductCode = txtProductCode.Text;
                    uCXYDataChart.RefreshData(formImPortBom.eng_XYData_Results);
                }
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            btnSave.Enabled = false;
            await Task.Run(() =>
            {
                save();
            });
            uCXYDataChart.RefreshData(eng_XYData_Results);
            uCXYDataChart.Save();
            eng_XYData_Results = SQLDataControl.SearchXYData(txtProductCode.Text);
            dgvXYData.DataSource = eng_XYData_Results;
            btnSave.Enabled = true;
        }

        private async void save()
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    string side = "S";
                    switch (ComSide.SelectedIndex)
                    {
                        case 0:
                            side = "S";
                            break;
                        case 1:
                            side = "D1";
                            break;
                        case 2:
                            side = "D2";
                            break;
                    }

                    string rule = comBomRule.Text;
                    if (rule == "")
                    {
                        rule = "Default";
                    }

                    SQLDataControl.AddProgram(txtProductCode.Text,
              txtProductName.Text, txtCustom.Text, side, "test", rule);

                    for (int i = 0; i < DataGridView_BOM.Rows.Count; i++)
                    {
                        int qty = 0;
                        if (DataGridView_BOM.Rows[i].Cells[4].Value != null)
                            if (int.TryParse(DataGridView_BOM.Rows[i].Cells[4].Value.ToString(), out qty))
                            {

                            }

                        SQLDataControl.InsertBom(
                            txtProductCode.Text,                                                              //产品编码
                          DataGridView_BOM.Rows[i].Cells[5].Value == null ? "" : DataGridView_BOM.Rows[i].Cells[5].Value.ToString(),                             //position
                          DataGridView_BOM.Rows[i].Cells[2].Value == null ? "" : DataGridView_BOM.Rows[i].Cells[2].Value.ToString(),                //物料编码
                          DataGridView_BOM.Rows[i].Cells[3].Value == null ? "" : DataGridView_BOM.Rows[i].Cells[3].Value.ToString(),                       //物料描述
                               qty,                      //用量
                               (i + 1),                                                             //序号
                          DataGridView_BOM.Rows[i].Cells[6].Value == null ? "" : DataGridView_BOM.Rows[i].Cells[6].Value.ToString(),                     //类型
                          GetValue(DataGridView_BOM.Rows[i].Cells[7].Value),//标准值
                          DataGridView_BOM.Rows[i].Cells[8].Value == null ? "" : DataGridView_BOM.Rows[i].Cells[8].Value.ToString(),//单位
                           GetValue(DataGridView_BOM.Rows[i].Cells[9].Value),//最大值
                          GetValue(DataGridView_BOM.Rows[i].Cells[10].Value),//最小值
                         DataGridView_BOM.Rows[i].Cells[11].Value == null ? "" : DataGridView_BOM.Rows[i].Cells[11].Value.ToString(),//尺寸
                          DataGridView_BOM.Rows[i].Cells[12].Value == null ? "" : DataGridView_BOM.Rows[i].Cells[12].Value.ToString(),//替代料
                          DataGridView_BOM.Rows[i].Cells[13].Value == null ? "" : DataGridView_BOM.Rows[i].Cells[13].Value.ToString(),//创建者
                           GetValue(DataGridView_BOM.Rows[i].Cells[17].Value),//上限公差
                           GetValue(DataGridView_BOM.Rows[i].Cells[18].Value),//下限公差
                           DataGridView_BOM.Rows[i].Cells[19].Value == null ? "" : DataGridView_BOM.Rows[i].Cells[19].Value.ToString()//公差类别

                        );
                    }
                    for (int i = 0; i < dgvXYData.Rows.Count; i++)
                    {
                        SQLDataControl.InsertXYData(
                            txtProductCode.Text,                                                                       //产品编码
                           dgvXYData.Rows[i].Cells[1].Value == null ? "" : dgvXYData.Rows[i].Cells[1].Value.ToString(),                                              //position
                           int.Parse(GetValue(dgvXYData.Rows[i].Cells[2].Value).ToString()),                //boardId
                           dgvXYData.Rows[i].Cells[3].Value == null ? "" : dgvXYData.Rows[i].Cells[3].Value.ToString(),  //boardSide
                           dgvXYData.Rows[i].Cells[4].Value == null ? "" : dgvXYData.Rows[i].Cells[4].Value.ToString(),//isSMD
                           dgvXYData.Rows[i].Cells[5].Value == null ? "" : dgvXYData.Rows[i].Cells[5].Value.ToString(),//materialCode
                           dgvXYData.Rows[i].Cells[6].Value == null ? "" : dgvXYData.Rows[i].Cells[6].Value.ToString(),//materialName
                         GetValue(dgvXYData.Rows[i].Cells[7].Value),// x
                           GetValue(dgvXYData.Rows[i].Cells[8].Value),//y
                           dgvXYData.Rows[i].Cells[9].Value == null ? "" : dgvXYData.Rows[i].Cells[9].Value.ToString(),       //unit
                           GetValue(dgvXYData.Rows[i].Cells[10].Value),       //angle
                           GetValue(dgvXYData.Rows[i].Cells[11].Value),
                           GetValue(dgvXYData.Rows[i].Cells[12].Value),
                           dgvXYData.Rows[i].Cells[13].Value == null ? "" : dgvXYData.Rows[i].Cells[13].Value.ToString(),            //remarks
                           dgvXYData.Rows[i].Cells[14].Value == null ? "" : dgvXYData.Rows[i].Cells[14].Value.ToString(),
                           dgvXYData.Rows[i].Cells[16].Value == null ? "" : dgvXYData.Rows[i].Cells[16].Value.ToString(),
                           "",
                           GetValue(dgvXYData.Rows[i].Cells[10].Value),   //standardRotation
                           dgvXYData.Rows[i].Cells[18].Value == null ? "" : dgvXYData.Rows[i].Cells[18].Value.ToString(),
                           dgvXYData.Rows[i].Cells[19].Value == null ? "" : dgvXYData.Rows[i].Cells[19].Value.ToString(),
                           dgvXYData.Rows[i].Cells[20].Value == null ? "" : dgvXYData.Rows[i].Cells[20].Value.ToString(),
                           GetValue(dgvXYData.Rows[i].Cells[21].Value),//ox
                           GetValue(dgvXYData.Rows[i].Cells[22].Value),//oy
                           GetValue(dgvXYData.Rows[i].Cells[23].Value),//oangle
                                       (i + 1),
                                       0,
                                       0,
                                       "",
                                       null,
                                       0,
                                       0,
                                       "",
                                       0,
                                       0,
                                       0,
                                       0,
                                       "",
                                       0,
                                       0,   //层
                                       0,
                                       0

                            );
                    }


                }));
            }
            catch (Exception ex)
            {

            }
        }

        public decimal GetValue(object value)
        {
            try
            {
                decimal val = 0;
                if (value != null)
                {
                    if (decimal.TryParse(value.ToString(), out val))
                    {

                    }
                    else
                    {
                        val = 0;
                    }
                }
                return val;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        private void btnSetRule_Click(object sender, EventArgs e)
        {
            FormAnalySeting formAnalySeting = new FormAnalySeting(comBomRule.Text);
            formAnalySeting.ShowDialog();

            comBomRule.Items.Clear();
            for (int i = 0; i < AnaylseDataManager.Instance.Rules.Count; i++)
            {
                comBomRule.Items.Add(AnaylseDataManager.Instance.Rules[i]);
            }

        }

        private async void btnAnalyse_Click(object sender, EventArgs e)
        {
            btnAnalyse.Enabled = false;
            ErrorLog.Clear();
            kryptonRichTextBox1.Text = "";
            await Task.Run(() =>
            {
                try
                {
                    AnalyBomFile();
                }
                catch (Exception ex)
                {

                }
            });
            for (int i = 0; i < DataGridView_BOM.RowCount; i++)
            {
                if (ErrorLog.ContainsKey(i + 1))
                {
                    DataGridView_BOM.Rows[i].Cells[0].Style.BackColor = Color.Red;
                    if (ErrorLog.TryGetValue(i + 1, out string msg))
                    {
                        if (msg.Contains("尺寸"))
                        {
                            DataGridView_BOM.Rows[i].Cells[11].Style.BackColor = Color.White;
                        }
                        if (msg.Contains("单位"))
                        {
                            DataGridView_BOM.Rows[i].Cells[8].Style.BackColor = Color.White;
                        }
                        if (msg.Contains("值"))
                        {
                            DataGridView_BOM.Rows[i].Cells[7].Style.BackColor = Color.White;
                        }
                    }

                }
                else
                {
                    DataGridView_BOM.Rows[i].Cells[7].Style.BackColor = Color.White;
                    DataGridView_BOM.Rows[i].Cells[8].Style.BackColor = Color.White;
                    DataGridView_BOM.Rows[i].Cells[11].Style.BackColor = Color.White;
                }
            }
            DataGridView_BOM.Refresh();
            dgvXYData.Refresh();
            uCXYDataChart.RefreshData(eng_XYData_Results);
            RefreshLog();
            btnAnalyse.Enabled = true;
        }
        private void RefreshLog()
        {
            kryptonRichTextBox1.Text = "解析完成" + "\r\n";
            foreach (var item in ErrorLog)
            {
                kryptonRichTextBox1.Text += "Row=" + item.Key + "Result=" + item.Value + "\r\n";
            }

        }
        private Dictionary<int, string> ErrorLog = new Dictionary<int, string>();
        private void AnalyBomFile()
        {
            for (int i = 0; i < p_Search_Engs.Count; i++)
            {
                AnalyseResult analyseResult = CommonAnalyse.Instance.AnalyseMethod_copy(p_Search_Engs[i].物料描述);
                switch (analyseResult.Type)
                {
                    case "电阻":
                        p_Search_Engs[i].元件类型 = "R";
                        break;
                    case "电容":
                        p_Search_Engs[i].元件类型 = "C";
                        break;
                    default:
                        p_Search_Engs[i].元件类型 = "O";
                        break;
                }
                if (analyseResult.Result)
                {
                    p_Search_Engs[i].元件尺寸 = analyseResult.Size;
                    p_Search_Engs[i].标准值 = decimal.Parse(analyseResult.Value);
                    p_Search_Engs[i].单位 = analyseResult.Unit;
                    if (analyseResult.Grade.Contains("%"))
                    {
                        string grade = analyseResult.Grade.Replace("%", "");
                        p_Search_Engs[i].上限公差 = decimal.Parse(grade);
                        p_Search_Engs[i].下限公差 = decimal.Parse(grade);
                        decimal val = (decimal)p_Search_Engs[i].上限公差 / (decimal)100.00;
                        p_Search_Engs[i].最大值 = p_Search_Engs[i].标准值 + p_Search_Engs[i].标准值 * (val);
                        p_Search_Engs[i].最小值 = p_Search_Engs[i].标准值 - p_Search_Engs[i].标准值 * (val);
                        p_Search_Engs[i].公差类别 = "%";
                    }
                    else
                    {
                        p_Search_Engs[i].上限公差 = decimal.Parse(analyseResult.Grade);
                        p_Search_Engs[i].下限公差 = decimal.Parse(analyseResult.Grade);
                        p_Search_Engs[i].最大值 = p_Search_Engs[i].标准值 + p_Search_Engs[i].上限公差;
                        p_Search_Engs[i].最小值 = p_Search_Engs[i].标准值 - p_Search_Engs[i].下限公差;
                        p_Search_Engs[i].公差类别 = "±";
                    }

                }
                else
                {
                    p_Search_Engs[i].元件尺寸 = analyseResult.Size;
                    if (analyseResult.Unit != null)
                        p_Search_Engs[i].单位 = analyseResult.Unit;
                    if (decimal.TryParse(analyseResult.Value, out decimal value))
                    {
                        p_Search_Engs[i].标准值 = decimal.Parse(analyseResult.Value);
                        if (analyseResult.Grade.Contains("%"))
                        {
                            string grade = analyseResult.Grade.Replace("%", "");
                            p_Search_Engs[i].上限公差 = decimal.Parse(grade);
                            p_Search_Engs[i].下限公差 = decimal.Parse(grade);
                            decimal val = (decimal)p_Search_Engs[i].上限公差 / (decimal)100.00;
                            p_Search_Engs[i].最大值 = p_Search_Engs[i].标准值 + p_Search_Engs[i].标准值 * (val);
                            p_Search_Engs[i].最小值 = p_Search_Engs[i].标准值 - p_Search_Engs[i].标准值 * (val);
                            p_Search_Engs[i].公差类别 = "%";
                        }
                        else if (decimal.TryParse(analyseResult.Grade, out decimal g))
                        {
                            p_Search_Engs[i].上限公差 = decimal.Parse(analyseResult.Grade);
                            p_Search_Engs[i].下限公差 = decimal.Parse(analyseResult.Grade);
                            p_Search_Engs[i].最大值 = p_Search_Engs[i].标准值 + p_Search_Engs[i].上限公差;
                            p_Search_Engs[i].最小值 = p_Search_Engs[i].标准值 - p_Search_Engs[i].下限公差;
                            p_Search_Engs[i].公差类别 = "±";
                        }
                    }
                    else
                    {
                        if (analyseResult.Grade.Contains("%"))
                        {
                            string grade = analyseResult.Grade.Replace("%", "");
                            p_Search_Engs[i].上限公差 = decimal.Parse(grade);
                            p_Search_Engs[i].下限公差 = decimal.Parse(grade);
                            decimal val = (decimal)p_Search_Engs[i].上限公差 / (decimal)100.00;
                            //p_Search_Engs[i].最大值 = p_Search_Engs[i].标准值 + p_Search_Engs[i].标准值 * (val);
                            //p_Search_Engs[i].最小值 = p_Search_Engs[i].标准值 - p_Search_Engs[i].标准值 * (val);
                            p_Search_Engs[i].公差类别 = "%";
                        }
                        else if (decimal.TryParse(analyseResult.Grade, out decimal g))
                        {
                            p_Search_Engs[i].上限公差 = decimal.Parse(analyseResult.Grade);
                            p_Search_Engs[i].下限公差 = decimal.Parse(analyseResult.Grade);
                            //p_Search_Engs[i].最大值 = p_Search_Engs[i].标准值 + p_Search_Engs[i].上限公差;
                            //p_Search_Engs[i].最小值 = p_Search_Engs[i].标准值 - p_Search_Engs[i].下限公差;
                            p_Search_Engs[i].公差类别 = "±";
                        }
                    }
                    if (p_Search_Engs[i].元件类型 != "O")
                    {
                        ErrorLog.Add(p_Search_Engs[i].序号, analyseResult.DefaultFormat() + "," + analyseResult.ErrorMsg);
                    }
                }
                try
                {
                    List<P_Search_Eng_XYData_Result> _XY_Result = eng_XYData_Results.Where(x => x.物料编码 == p_Search_Engs[i].物料编码).ToList();
                    if (_XY_Result != null)
                    {
                        foreach (var item in _XY_Result)
                        {
                            item.元件类型 = p_Search_Engs[i].元件类型;
                            item.元件尺寸 = p_Search_Engs[i].元件尺寸;
                            item.单位 = p_Search_Engs[i].单位;
                        }
                    }
                }
                catch (Exception)
                {

                }

            }

        }

        private void btnAddReport_Click(object sender, EventArgs e)
        {

        }



        private void btnExportBom_Click(object sender, EventArgs e)
        {
            try
            {
                NOPIHelperEX.ExportDataToExcel(NOPIHelperEX.ToDataTable(DataGridView_BOM));
            }
            catch (Exception ex)
            {

            }

        }



        private void btnExportXYData_Click_1(object sender, EventArgs e)
        {
            try
            {
                NOPIHelperEX.ExportDataToExcel(NOPIHelperEX.ToDataTable(dgvXYData));
            }
            catch (Exception)
            {

            }

        }

        private void DataGridView_BOM_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                DataGridView_BOM.Rows[e.RowIndex].Selected = true;
        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            if (DataGridView_BOM.SelectedRows.Count == 0)
                return;
            string customerCode = "";
            string materialCode = p_Search_Engs[DataGridView_BOM.SelectedRows[0].Index].物料编码;
            string materialName = p_Search_Engs[DataGridView_BOM.SelectedRows[0].Index].物料描述; ;
            string lcrType = p_Search_Engs[DataGridView_BOM.SelectedRows[0].Index].元件类型; 
            decimal lcrStandardValue = (decimal)p_Search_Engs[DataGridView_BOM.SelectedRows[0].Index].标准值; 
            string lcrUnitCode = p_Search_Engs[DataGridView_BOM.SelectedRows[0].Index].单位;
            decimal lcrMaxValue = (decimal)p_Search_Engs[DataGridView_BOM.SelectedRows[0].Index].最大值; 
            decimal lcrMinValue = (decimal)p_Search_Engs[DataGridView_BOM.SelectedRows[0].Index].最小值; 
            string size = p_Search_Engs[DataGridView_BOM.SelectedRows[0].Index].元件尺寸; ;
            decimal compensateValue = 0;
            byte[] picture = null;
            string remarks = "";
            string creator = "";
            DateTime creationDate = DateTime.Now;
            string modifier = "";
            DateTime modificationDate= DateTime.Now;
            string screenPrinting = "";
            decimal maxTolerance = (decimal)p_Search_Engs[DataGridView_BOM.SelectedRows[0].Index].上限公差;
            decimal minTolerance = (decimal)p_Search_Engs[DataGridView_BOM.SelectedRows[0].Index].下限公差;
            string toleranceType = p_Search_Engs[DataGridView_BOM.SelectedRows[0].Index].公差类别;
            string componentPackaging = "";
            string msg = null;
            SQLDataControl.UpdateBas_Material(0, customerCode, materialCode, materialName, lcrType, lcrStandardValue, lcrUnitCode, lcrMaxValue, lcrMinValue, size, compensateValue, picture, remarks, creator, creationDate, modifier, modificationDate, screenPrinting, maxTolerance, minTolerance, toleranceType, componentPackaging, out msg);
            if (string.IsNullOrEmpty(msg))
            {
                MessageBox.Show("添加完成");
                return;
            }

            if(MessageBox.Show(msg+",是否替换?", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                SQLDataControl.UpdateBas_Material(2, customerCode, materialCode, materialName, lcrType, lcrStandardValue, lcrUnitCode, lcrMaxValue, lcrMinValue, size, compensateValue, picture, remarks, creator, creationDate, modifier, modificationDate, screenPrinting, maxTolerance, minTolerance, toleranceType, componentPackaging, out msg);
            }
        }
    }
}
