using BorwinAnalyse.BaseClass;
using BorwinAnalyse.ImportBom;
using BrowLib.Static;
using ComponentFactory.Krypton.Toolkit;
using NPOI.SS.Formula.Functions;
using SqlHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BorwinAnalyse.Forms
{
    public partial class FormImPortBom : KryptonForm
    {
        public FormImPortBom()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        public FormImPortBom(string bomPath, string xyData, string ProductCode, string side) : this()
        {
            txtBomPath.Text = bomPath;
            txtXYPath.Text = xyData;
            productCode = ProductCode;
            this.side = side;
            AnalyFile();
        }

        private string productCode = "";
        private string side = "";

        private Dictionary<int, string> ErrorLog = new Dictionary<int, string>();

        /// <summary>
        /// 获取BOM数据
        /// </summary>
        public List<P_Search_Eng_Bom_Result> p_Search_Engs = new List<P_Search_Eng_Bom_Result>();

        /// <summary>
        /// 获取XY数据
        /// </summary>
        public List<P_Search_Eng_XYData_Result> eng_XYData_Results = new List<P_Search_Eng_XYData_Result>();

        private void btnBomTitleSetting_Click(object sender, EventArgs e)
        {
            FormBomTitle form = new FormBomTitle();
            form.ShowDialog();
        }

        private void btnXYTitleSetting_Click(object sender, EventArgs e)
        {
            FormXYTitle form = new FormXYTitle();
            form.ShowDialog();

        }

        private async void btnAnalyFile_Click(object sender, EventArgs e)
        {
            btnAnalyFile.Enabled = false;
            AnalyFile();
            btnAnalyFile.Enabled = true;
        }

        private async void AnalyFile()
        {
            dgv_BOM.DataSource = null;
            dgvXYData.DataSource = null;
            ErrorLog.Clear();
            try
            {
                await Task.Run(() =>
                {
                    AnalyBomFile();
                    AnalyXYFile();
                });
            }
            catch (Exception)
            {

            }
            BindData();
            UpdateLanguage();
        }
        private void UpdateLanguage()
        {
            BrowApp.Language.Language.Instance.UpdateLanguage(this, null);
        }
        private void AnalyBomFile()
        {
            p_Search_Engs.Clear();
            DateTime time = DateTime.Now;
            if (File.Exists(txtBomPath.Text))
            {
                //开始加载Bom文件
                DataTable dt = NOPIHelperEX.ExcelToDataTable(txtBomPath.Text, true);
                if (dt == null)
                    return;
                Dictionary<string, int> colDic = new Dictionary<string, int>();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    string colName = dt.Columns[i].ColumnName;
                    if (AnaylseDataManager.Instance.Bom.MaterialCodes.Contains(colName))
                    {
                        if (colDic.ContainsKey("物料编码") == false)
                            colDic.Add("物料编码", i);
                    }
                    else if (AnaylseDataManager.Instance.Bom.MaterialDescriptions.Contains(colName))
                    {
                        if (colDic.ContainsKey("物料描述") == false)
                            colDic.Add("物料描述", i);
                    }
                    else if (AnaylseDataManager.Instance.Bom.QTYs.Contains(colName))
                    {
                        if (colDic.ContainsKey("用量") == false)
                            colDic.Add("用量", i);
                    }
                    else if (AnaylseDataManager.Instance.Bom.Positions.Contains(colName))
                    {
                        if (colDic.ContainsKey("元件位置") == false)
                            colDic.Add("元件位置", i);
                    }
                    else if (AnaylseDataManager.Instance.Bom.QTYs.Contains(colName))
                    {
                        if (colDic.ContainsKey("替代料") == false)
                            colDic.Add("替代料", i);
                    }
                }

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i].ItemArray[0] == null || dt.Rows[i].ItemArray[0].ToString() == "")
                    {
                        continue;
                    }
                    P_Search_Eng_Bom_Result _Bom_Result = new P_Search_Eng_Bom_Result();
                    _Bom_Result.序号 = i + 1;
                    _Bom_Result.产品编码 = productCode;
                    if (colDic.ContainsKey("物料编码"))
                    {
                        _Bom_Result.物料编码 = dt.Rows[i][colDic["物料编码"]].ToString();
                    }
                    else if (dt.Rows[i].ItemArray.Length > 0)
                    {
                        _Bom_Result.物料编码 = dt.Rows[i][0].ToString();
                    }
                    if (colDic.ContainsKey("物料描述"))
                    {
                        _Bom_Result.物料描述 = dt.Rows[i][colDic["物料描述"]].ToString();
                    }
                    else if (dt.Rows[i].ItemArray.Length > 1)
                    {
                        _Bom_Result.物料描述 = dt.Rows[i][1].ToString();
                    }
                    if (colDic.ContainsKey("用量"))
                    {
                        if (int.TryParse(dt.Rows[i][colDic["用量"]].ToString(), out int qty))
                            _Bom_Result.用量 = qty;
                    }
                    else if (dt.Rows[i].ItemArray.Length > 2)
                    {
                        if (int.TryParse(dt.Rows[i][2].ToString(), out int qty))
                            _Bom_Result.用量 = qty;
                    }
                    if (colDic.ContainsKey("元件位置"))
                    {
                        _Bom_Result.元件位置 = dt.Rows[i][colDic["元件位置"]].ToString();
                    }
                    else if (dt.Rows[i].ItemArray.Length > 3)
                    {
                        _Bom_Result.元件位置 = dt.Rows[i][3].ToString();
                    }
                    if (colDic.ContainsKey("替代料"))
                    {
                        _Bom_Result.替代料 = dt.Rows[i][colDic["替代料"]].ToString();
                    }
                    else
                    {
                        _Bom_Result.替代料 = "";
                    }

                    AnalyRow(_Bom_Result);

                    _Bom_Result.修改者 = "Test";
                    _Bom_Result.创建者 = "Test";
                    _Bom_Result.创建时间 = time;
                    _Bom_Result.修改时间 = time;
                    p_Search_Engs.Add(_Bom_Result);
                }
            }
            else
            {

            }

        }

        private bool AnalyRow(P_Search_Eng_Bom_Result _Bom_Result)
        {
            AnalyseResult analyseResult = CommonAnalyse.Instance.AnalyseMethod_copy(_Bom_Result.物料描述);
            switch (analyseResult.Type)
            {
                case "电阻":
                    _Bom_Result.元件类型 = "R";
                    break;
                case "电容":
                    _Bom_Result.元件类型 = "C";
                    break;
                default:
                    _Bom_Result.元件类型 = "O";
                    break;
            }
            if (analyseResult.Result)
            {
                _Bom_Result.元件尺寸 = analyseResult.Size;
                _Bom_Result.标准值 = decimal.Parse(analyseResult.Value);
                _Bom_Result.单位 = analyseResult.Unit;
                if (analyseResult.Grade.Contains("%"))
                {
                    string grade = analyseResult.Grade.Replace("%", "");
                    _Bom_Result.上限公差 = decimal.Parse(grade);
                    _Bom_Result.下限公差 = decimal.Parse(grade);
                    decimal val = (decimal)_Bom_Result.上限公差 / (decimal)100.00;
                    _Bom_Result.最大值 = _Bom_Result.标准值 + _Bom_Result.标准值 * (val);
                    _Bom_Result.最小值 = _Bom_Result.标准值 - _Bom_Result.标准值 * (val);
                    _Bom_Result.公差类别 = "%";
                }
                else
                {
                    _Bom_Result.上限公差 = decimal.Parse(analyseResult.Grade);
                    _Bom_Result.下限公差 = decimal.Parse(analyseResult.Grade);
                    _Bom_Result.最大值 = _Bom_Result.标准值 + _Bom_Result.上限公差;
                    _Bom_Result.最小值 = _Bom_Result.标准值 - _Bom_Result.下限公差;
                    _Bom_Result.公差类别 = "±";
                }

            }
            else
            {
                _Bom_Result.元件尺寸 = analyseResult.Size;
                if (analyseResult.Unit != null)
                    _Bom_Result.单位 = analyseResult.Unit;
                if (decimal.TryParse(analyseResult.Value, out decimal value))
                {
                    _Bom_Result.标准值 = decimal.Parse(analyseResult.Value);
                    if (analyseResult.Grade.Contains("%"))
                    {
                        string grade = analyseResult.Grade.Replace("%", "");
                        _Bom_Result.上限公差 = decimal.Parse(grade);
                        _Bom_Result.下限公差 = decimal.Parse(grade);
                        decimal val = (decimal)_Bom_Result.上限公差 / (decimal)100.00;
                        _Bom_Result.最大值 = _Bom_Result.标准值 + _Bom_Result.标准值 * (val);
                        _Bom_Result.最小值 = _Bom_Result.标准值 - _Bom_Result.标准值 * (val);
                        _Bom_Result.公差类别 = "%";
                    }
                    else if (decimal.TryParse(analyseResult.Grade, out decimal g))
                    {
                        _Bom_Result.上限公差 = decimal.Parse(analyseResult.Grade);
                        _Bom_Result.下限公差 = decimal.Parse(analyseResult.Grade);
                        _Bom_Result.最大值 = _Bom_Result.标准值 + _Bom_Result.上限公差;
                        _Bom_Result.最小值 = _Bom_Result.标准值 - _Bom_Result.下限公差;
                        _Bom_Result.公差类别 = "±";
                    }
                }
                else
                {
                    if (analyseResult.Grade.Contains("%"))
                    {
                        string grade = analyseResult.Grade.Replace("%", "");
                        _Bom_Result.上限公差 = decimal.Parse(grade);
                        _Bom_Result.下限公差 = decimal.Parse(grade);
                        decimal val = (decimal)_Bom_Result.上限公差 / (decimal)100.00;
                        //_Bom_Result.最大值 = _Bom_Result.标准值 + _Bom_Result.标准值 * (val);
                        //_Bom_Result.最小值 = _Bom_Result.标准值 - _Bom_Result.标准值 * (val);
                        _Bom_Result.公差类别 = "%";
                    }
                    else if (decimal.TryParse(analyseResult.Grade, out decimal g))
                    {
                        _Bom_Result.上限公差 = decimal.Parse(analyseResult.Grade);
                        _Bom_Result.下限公差 = decimal.Parse(analyseResult.Grade);
                        //_Bom_Result.最大值 = _Bom_Result.标准值 + _Bom_Result.上限公差;
                        //_Bom_Result.最小值 = _Bom_Result.标准值 - _Bom_Result.下限公差;
                        _Bom_Result.公差类别 = "±";
                    }
                }
                if (_Bom_Result.元件类型 != "O")
                {
                    ErrorLog.Add(_Bom_Result.序号, analyseResult.DefaultFormat() + "," + analyseResult.ErrorMsg);
                }
            }
            return analyseResult.Result;
        }

        private void AnalyXYFile()
        {
            eng_XYData_Results.Clear();
            if (File.Exists(txtXYPath.Text))
            {
                //开始加载XY文件
                DataTable dt = NOPIHelperEX.ExcelToDataTable(txtXYPath.Text, true);
                if (dt == null)
                    return;
                Dictionary<string, int> colDic = new Dictionary<string, int>();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    string colName = dt.Columns[i].ColumnName;
                    if (AnaylseDataManager.Instance.XYData.Positions.Contains(colName))
                    {
                        if (colDic.ContainsKey("元件位置") == false)
                            colDic.Add("元件位置", i);
                    }
                    else if (AnaylseDataManager.Instance.XYData.XPos.Contains(colName))
                    {
                        if (colDic.ContainsKey("X坐标") == false)
                            colDic.Add("X坐标", i);
                    }
                    else if (AnaylseDataManager.Instance.XYData.YPos.Contains(colName))
                    {
                        if (colDic.ContainsKey("Y坐标") == false)
                            colDic.Add("Y坐标", i);
                    }
                    else if (AnaylseDataManager.Instance.XYData.Angles.Contains(colName))
                    {
                        if (colDic.ContainsKey("角度") == false)
                            colDic.Add("角度", i);
                    }
                    else if (AnaylseDataManager.Instance.XYData.Sides.Contains(colName))
                    {
                        if (colDic.ContainsKey("板面") == false)
                            colDic.Add("板面", i);
                    }
                }

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    P_Search_Eng_XYData_Result _XY_Result = new P_Search_Eng_XYData_Result();
                    _XY_Result.序号 = i + 1;
                    _XY_Result.产品编码 = productCode;
                    _XY_Result.拼板 = 1;
                    if (colDic.ContainsKey("元件位置"))
                    {
                        _XY_Result.元件位置 = dt.Rows[i][colDic["元件位置"]].ToString();
                    }
                    else if (dt.Rows[i].ItemArray.Length > 0)
                    {
                        _XY_Result.元件位置 = dt.Rows[i][0].ToString();
                    }
                    if (colDic.ContainsKey("X坐标"))
                    {
                        int value = colDic["X坐标"];
                        if (decimal.TryParse(dt.Rows[i][value].ToString(), out decimal x))
                        {
                            _XY_Result.X坐标 = x;
                        }
                        else
                            _XY_Result.X坐标 = 0;
                    }
                    else if (dt.Rows[i].ItemArray.Length > 1)
                    {
                        _XY_Result.X坐标 = 0;
                    }
                    if (colDic.ContainsKey("Y坐标"))
                    {
                        if (decimal.TryParse(dt.Rows[i][colDic["Y坐标"]].ToString(), out decimal qty))
                            _XY_Result.Y坐标 = qty;
                    }
                    else if (dt.Rows[i].ItemArray.Length > 2)
                    {
                        _XY_Result.Y坐标 = 0;
                    }
                    if (colDic.ContainsKey("角度"))
                    {

                        if (decimal.TryParse(dt.Rows[i][colDic["角度"]].ToString(), out decimal angle))
                            _XY_Result.角度 = angle;
                    }
                    else if (dt.Rows[i].ItemArray.Length > 3)
                    {
                        _XY_Result.角度 = 0;
                    }
                    //if (colDic.ContainsKey("板面"))
                    //{
                    //    _XY_Result.板面 = dt.Rows[i][colDic["板面"]].ToString();
                    //}
                    //else if (dt.Rows[i].ItemArray.Length > 3)
                    //{
                    //    _XY_Result.板面 = "Top";
                    //}
                    _XY_Result.板面 = side;
                    foreach (var item in p_Search_Engs)
                    {
                        string _pos = item.元件位置.Replace(" ", ",");
                        List<string> res = _pos.Split(',').ToList();
                        if (res != null)
                            if (res.Where(x => x == _XY_Result.元件位置).ToList().Count > 0)
                            {
                                _XY_Result.物料描述 = item.物料描述;
                                _XY_Result.物料编码 = item.物料编码;
                                _XY_Result.元件类型 = item.元件类型;
                                _XY_Result.元件尺寸 = item.元件尺寸;
                                _XY_Result.单位 = item.单位;
                                break;
                            }

                    }

                    eng_XYData_Results.Add(_XY_Result);
                    _XY_Result.拼板 = 1;
                    if (_XY_Result.元件位置.ToUpper().Contains("MARK"))
                        _XY_Result.是否贴装 = "NO";
                    else
                        _XY_Result.是否贴装 = "YES";
                }

            }
            else
            {

            }
        }
        private void BindData()
        {
            try
            {
                BindBomData();
                BindXYData();
                RefreshLog();
            }
            catch (Exception)
            {

            }
        }

        private void BindBomData()
        {
            dgv_BOM.DataSource = p_Search_Engs;
            for (int i = 0; i < dgv_BOM.RowCount; i++)
            {
                if (ErrorLog.ContainsKey(i + 1))
                {
                    dgv_BOM.Rows[i].Cells[0].Style.BackColor = Color.Red;
                }
            }
        }

        private void BindXYData()
        {
            dgvXYData.DataSource = eng_XYData_Results;

            for (int i = 0; i < dgvXYData.RowCount; i++)
            {
                if (dgvXYData.Rows[i].Cells[5].Value == null || string.IsNullOrEmpty(dgvXYData.Rows[i].Cells[5].Value.ToString()))
                {
                    dgvXYData.Rows[i].Cells[5].Style.BackColor = Color.Red;
                }
            }
        }

        private void RefreshLog()
        {
            kryptonRichTextBox1.Text = "";
            foreach (var item in ErrorLog)
            {
                kryptonRichTextBox1.Text += "Row=" + item.Key + "Result=" + item.Value + "\r\n";
            }

        }

        private void btnOpenFileBom_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            dlg.Filter = "xlsx|*.xlsx;*.xls;*.XLS";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtBomPath.Text = dlg.FileName;
            }
        }

        private void btnOpenFileXY_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            dlg.Filter = "xlsx|*.xlsx;*.xls;*.XLS";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtXYPath.Text = dlg.FileName;
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnDeleteBom_Click(object sender, EventArgs e)
        {
            int index = -1;
            if (dgv_BOM.SelectedRows.Count > 0)
            {
                index = dgv_BOM.SelectedRows[0].Index;

            }
            if (index >= 0)
            {
                dgv_BOM.DataSource = null;
                p_Search_Engs.RemoveAt(index);
                dgv_BOM.DataSource = p_Search_Engs;
                dgv_BOM.Refresh();
            }
        }

        private void dgv_BOM_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            for (int i = 0; i < dgv_BOM.Rows.Count; i++)
            {
                dgv_BOM.Rows[i].Selected = false;
            }
            if (e.RowIndex >= 0)
            {
                dgv_BOM.Rows[e.RowIndex].Selected = true;
            }
        }

        private void btnDeleteXY_Click(object sender, EventArgs e)
        {
            int index = -1;
            if (dgvXYData.SelectedRows.Count > 0)
            {
                index = dgvXYData.SelectedRows[0].Index;

            }
            if (index >= 0)
            {
                dgvXYData.DataSource = null;
                eng_XYData_Results.RemoveAt(index);
                dgvXYData.DataSource = eng_XYData_Results;
                dgvXYData.Refresh();
            }
        }

        private void dgvXYData_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            for (int i = 0; i < dgvXYData.Rows.Count; i++)
            {
                dgvXYData.Rows[i].Selected = false;
            }
            if (e.RowIndex >= 0)
            {
                dgvXYData.Rows[e.RowIndex].Selected = true;
            }
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            int index = -1;
            if (dgv_BOM.SelectedRows.Count > 0)
            {
                index = dgv_BOM.SelectedRows[0].Index;

            }
            if (index >= 0)
            {
                string materialCode = p_Search_Engs[index].物料编码;
                string materialName = p_Search_Engs[index].物料描述;
                FormMaterialMessage frm = new FormMaterialMessage(materialCode, materialName);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    p_Search_Engs[index].物料描述 = frm.MaterialName;
                    if (AnalyRow(p_Search_Engs[index]))
                    {
                        dgv_BOM.Rows[index].Cells[0].Style.BackColor = Color.White;
                    }
                    dgv_BOM.Refresh();
                }

            }
        }

        private void btnAnayRow_Click(object sender, EventArgs e)
        {
            if (dgv_BOM.SelectedRows.Count > 0)
            {
                int index = dgv_BOM.SelectedRows[0].Index;
                if (AnalyRow(p_Search_Engs[index]))
                {
                    dgv_BOM.Rows[index].Cells[0].Style.BackColor = Color.White;
                }
                dgv_BOM.Refresh();
            }
        }
    }
}
