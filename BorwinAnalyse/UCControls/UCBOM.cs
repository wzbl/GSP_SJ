using BorwinAnalyse.BaseClass;
using BorwinAnalyse.Forms;
using BorwinAnalyse.Model;
using BrowApp;
using BrowApp.Language;
using BrowApp.MessageTip;
using ComponentFactory.Krypton.Docking;
using ComponentFactory.Krypton.Toolkit;
using SqlHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace BorwinAnalyse.UCControls
{
    public partial class UCBOM : UserControl
    {
        CancellationTokenSource tokenSource;
        bool isStart = false;
        private Dictionary<string, int> DictColIndex = new Dictionary<string, int>();

        public int CodeColumn { get; set; } = 0;
        public int DescrColumn { get; set; } = 1;
        public int ReplaceCodeColumn { get; set; } = 0;

        int DGVReplaceCol = 0;

        Dictionary<string, List<string>> keyValuePairsReplaceKey = new Dictionary<string, List<string>>();

        public string ProductCode = "";

        public UCBOM()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            this.Load += UCBOM_Load;
            this.components = new System.ComponentModel.Container();
            this.DataGridView_BOM.ColumnDisplayIndexChanged += DataGridView_BOM_ColumnDisplayIndexChanged; ;
        }

        public UCBOM(string productCode) :
            this()
        {
            this.ProductCode = productCode;
            txtName.Text = productCode;
            List<P_Search_Eng_Bom_Result> p_Search_Engs = SQLDataControl.SearchBom(ProductCode);
            if (p_Search_Engs.Count > 0)
            {
                DataGridView_BOM.DataSource = p_Search_Engs;
            }
        }

        private void DataGridView_BOM_ColumnDisplayIndexChanged(object sender, DataGridViewColumnEventArgs e)
        {
            string ColName = e.Column.HeaderText.ToString();
            int Colndex = e.Column.DisplayIndex;
            if (!DictColIndex.ContainsKey(ColName))
            {
                DictColIndex.Add(ColName, Colndex);
            }
            else
            {
                DictColIndex.Remove(ColName);
                DictColIndex.Add(ColName, Colndex);
            }
        }

        ToolTip toolTip = new ToolTip();
        private void showTips(Control control, string msg)
        {
            toolTip.SetToolTip(control, msg);
        }

        private void UCBOM_Load(object sender, EventArgs e)
        {
            if (AnalyseDt == null)
                InitUI();

            txtCodeCol.Text = CodeColumn.ToString();
            txtDescriptionCol.Text = DescrColumn.ToString();

            DataGridView_BOM.RowTemplate.Height = 40;

            UpdataLanguage();

            MicroKeyManager.Instance.AddEventToControl(this);



        }

        public void UpdataLanguage()
        {

        }
        DataTable AnalyseDt;
        List<string> codes = new List<string>();
        private void InitUI()
        {
            kryptonDockableNavigator1.SelectedIndex = 0;
            txtName.Text = BomManager.Instance.CurrentBomName;
            AnalyseDt = new DataTable();
            AnalyseDt.Columns.Add("序号".tr(), typeof(int));
            AnalyseDt.Columns.Add("barCode");
            AnalyseDt.Columns.Add("description");
            AnalyseDt.Columns.Add("result");
            AnalyseDt.Columns.Add("type");
            AnalyseDt.Columns.Add("size");
            AnalyseDt.Columns.Add("value");
            AnalyseDt.Columns.Add("unit");
            AnalyseDt.Columns.Add("grade");
            AnalyseDt.Columns.Add("最大值");
            AnalyseDt.Columns.Add("最小值");
            AnalyseDt.Columns.Add("位置");
            AnalyseDt.Columns.Add("用量");
            AnalyseDt.Columns.Add("tapeType");
            AnalyseDt.Columns.Add("tapeWidth");
            AnalyseDt.Columns.Add("pitch");
            AnalyseDt.Columns.Add("judgeOCV");
            AnalyseDt.Columns.Add("replaceCode");
            //InitDataGrid();

        }

        /// <summary>
        /// 初始化网格
        /// (id ，barCode , description ,status ,type ,size ,value ,unit ,grade ,exp1 ,exp2 ,exp3 ,exp4 ,exp5 ,createTime)
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void InitDataGrid()
        {
            DataGridView_Result.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
             new DataGridViewTextBoxColumn()
            {
                FillWeight = 45.07613F,
                HeaderText = "序号".tr(),
                MinimumWidth = 6,
                Name = "Column1",
                ReadOnly = true,
                //SortMode = DataGridViewColumnSortMode.NotSortable
            },
            new DataGridViewTextBoxColumn()
            {
                FillWeight = 45.07613F,
                HeaderText = "条码".tr(),
                MinimumWidth = 6,
                Name = "Column1",
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable
            },
            new DataGridViewTextBoxColumn()
            {
                FillWeight = 45.07613F,
                HeaderText = "物料描述".tr(),
                MinimumWidth = 6,
                Name = "Column2",
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable
            },
            new DataGridViewTextBoxColumn()
            {
                FillWeight = 45.07613F,
                HeaderText = "结果".tr(),
                MinimumWidth = 6,
                Name = "Column3",
                ReadOnly = true,
                //SortMode = DataGridViewColumnSortMode.NotSortable
            },
            new DataGridViewTextBoxColumn()
            {
                FillWeight = 45.07613F,
                HeaderText = "类型".tr(),
                MinimumWidth = 6,
                Name = "Column4",
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable
            },
            new DataGridViewTextBoxColumn()
            {
                FillWeight = 45.07613F,
                HeaderText = "尺寸".tr(),
                MinimumWidth = 6,
                Name = "Column5",
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable
            },
            new DataGridViewTextBoxColumn()
            {
                FillWeight = 45.07613F,
                HeaderText = "值".tr(),
                MinimumWidth = 6,
                Name = "Column6",
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable
            },
                  new DataGridViewTextBoxColumn()
            {
                FillWeight = 45.07613F,
                HeaderText = "单位".tr(),
                MinimumWidth = 6,
                Name = "Column7",
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable
            },
            new DataGridViewTextBoxColumn()
            {
                FillWeight = 45.07613F,
                HeaderText = "等级".tr(),
                MinimumWidth = 6,
                Name = "Column8",
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable
            },new DataGridViewTextBoxColumn()
            {
                FillWeight = 45.07613F,
                //HeaderText = "exp1",
                HeaderText = "tapeType",
                MinimumWidth = 6,
                Name = "Column7",
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable
            },
            new DataGridViewTextBoxColumn()
            {
                FillWeight = 45.07613F,
                //HeaderText = "exp2",
                HeaderText = "tapeWidth",
                MinimumWidth = 6,
                Name = "Column7",
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable
            },
            new DataGridViewTextBoxColumn()
            {
                FillWeight = 45.07613F,
                //HeaderText = "exp3",
                HeaderText = "pitch",
                MinimumWidth = 6,
                Name = "Column7",
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable
            },
            new DataGridViewTextBoxColumn()
            {
                FillWeight = 45.07613F,
                //HeaderText = "exp4",
                HeaderText = "judgeOCV",
                MinimumWidth = 6,
                Name = "Column7",
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable
            },
            new DataGridViewTextBoxColumn()
            {
                FillWeight = 45.07613F,
                HeaderText = "replaceCode",
                MinimumWidth = 6,
                Name = "Column7",
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable
            }
            });
            DataGridView_Result.RowTemplate.Height = 40;
            DGVReplaceCol = 13;
        }

        DataTable dt;
        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "xlsx|*.xlsx;*.xls;*.XLS";
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtImportPath.Text = openFileDialog.FileName;
                if (string.IsNullOrEmpty(txtImportPath.Text)) return;
                string[] splits = txtImportPath.Text.Split('\\');
                int len = splits.Length;
                string file = splits[len - 1].Substring(0, splits[len - 1].IndexOf("."));
                txtName.Text = file;
            }
            else return;

            if (!File.Exists(txtImportPath.Text)) { return; }
            dt = new DataTable();
            DataGridView_BOM.DataSource = dt;
            DataGridView_BOM.Refresh();
            dt = NOPIHelperEX.ExcelToDataTable(txtImportPath.Text, CommonAnalyse.Instance.IsTitleRow);
            DataGridView_BOM.DataSource = dt;
            DataGridView_BOM.Refresh();
            lbResult.Text = "数据总行数".tr() + ":" + DataGridView_BOM.RowCount;

            for (int i = 0; i < DataGridView_BOM.Columns.Count; i++)
            {
                //DataGridView_BOM.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            this.DictColIndex?.Clear();
            if (DataGridView_BOM.DataSource != null)
            {
                if (dt != null)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        this.DictColIndex.Add(dt.Columns[i].Caption, i);
                    }
                }
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (isStart)
            {
                btnStart.Values.Image = Properties.Resources.运行;

                StopAnalyse();
                SetColor();
            }
            else
            {
                if (AnalyseDt == null)
                {
                    return;
                }
                tokenSource = new CancellationTokenSource();
                kryptonDockableNavigator1.SelectedIndex = 1;
                btnStart.Values.Image = Properties.Resources.停止;
                StartAnalyse();
                Thread.Sleep(100);

                //MessageBox.Show("解析完成".tr());
            }
        }

        public async void StartAnalyse()
        {
            isStart = true;
            if (chkAnalyseSelectRow.Checked)
            {
                AnalyseSelect();
            }
            else
            {
                AllCount = DataGridView_BOM.Rows.Count - 1;
                OKCount = 0;
                NGCount = 0;
                HistoryCount = 0;
                DicRepeatCode.Clear();
                ResetResult();
                analyseResults.Clear();
                AnalyseDt.Rows.Clear();
                codes.Clear();
                //Stopwatch stopwatch = new Stopwatch();
                //stopwatch.Start();
                DataGridView_Result.DataSource = null;
                await AnalyseAll();
                //stopwatch.Stop();
                //MessageBox.Show("解析完成用时"+stopwatch.ElapsedMilliseconds);

                Thread.Sleep(10);
                DataGridView_Result.DataSource = null;
                DataGridView_Result.DataSource = AnalyseDt;
                SetColor();
                DataGridView_Result.Refresh();
                for (int i = 0; i < DataGridView_Result.Columns.Count; i++)
                {
                    DataGridView_Result.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                }
                DataGridView_Result.Columns["序号"].SortMode = DataGridViewColumnSortMode.Automatic;
                DataGridView_Result.Columns["result"].SortMode = DataGridViewColumnSortMode.Automatic;
                UpdataResult();
            }
            tokenSource?.Cancel();
            btnStart.Values.Image = Properties.Resources.icons8_开始_80;
            isStart = false;
        }

        /// <summary>
        /// 分析选中行
        /// </summary>
        public void AnalyseSelect()
        {
            int dstIndex = 0;

            DataGridView_Result.Refresh();

            for (int i = 0; i < DataGridView_BOM.SelectedRows.Count; i++)
            {
                int index = DataGridView_BOM.SelectedRows[i].Index;
                string code = DataGridView_BOM.SelectedRows[i].Cells[CodeColumn].Value.ToString();
                AnalyseResult analyseResult = new AnalyseResult();
                try
                {
                    analyseResult = CommonAnalyse.Instance.AnalyseMethod_copy(DataGridView_BOM.SelectedRows[i].Cells[DescrColumn].Value.ToString());
                }
                catch (Exception ex)
                {
                    BrowApp.APP.Tip.ShowTip(0, "警告".tr(), ex.Message, "确定".tr());
                }
                //AnalyseDt.Rows[index].ItemArray[3] = analyseResult.Result;
                //AnalyseDt.Rows[index].ItemArray[4] = analyseResult.Type;
                //AnalyseDt.Rows[index].ItemArray[5] = analyseResult.Size;
                //AnalyseDt.Rows[index].ItemArray[6] = analyseResult.Value;
                //AnalyseDt.Rows[index].ItemArray[7] = analyseResult.Unit;
                //AnalyseDt.Rows[index].ItemArray[8] = analyseResult.Grade;
                //AnalyseDt.Rows[index].ItemArray[9] = analyseResult.DefaultFormat();
                //AnalyseDt.Rows[index].ItemArray[10] = analyseResult.Width;
                //AnalyseDt.Rows[index].ItemArray[11] = analyseResult.Space;

                if (DataGridView_Result.Rows.Count < 0)
                {
                    dstIndex = 0;
                    AnalyseDt.Rows[dstIndex].ItemArray[0] = dstIndex;
                    AnalyseDt.Rows[dstIndex].ItemArray[1] = DataGridView_BOM.SelectedRows[i].Cells[CodeColumn].Value;
                    AnalyseDt.Rows[dstIndex].ItemArray[2] = DataGridView_BOM.SelectedRows[i].Cells[DescrColumn].Value;
                    AnalyseDt.Rows[dstIndex].ItemArray[3] = analyseResult.Result;
                    AnalyseDt.Rows[dstIndex].ItemArray[4] = analyseResult.Type;
                    AnalyseDt.Rows[dstIndex].ItemArray[5] = analyseResult.Size;
                    AnalyseDt.Rows[dstIndex].ItemArray[6] = analyseResult.Value;
                    AnalyseDt.Rows[dstIndex].ItemArray[7] = analyseResult.Unit;
                    AnalyseDt.Rows[dstIndex].ItemArray[8] = analyseResult.Grade;
                    AnalyseDt.Rows[dstIndex].ItemArray[9] = String.Empty;
                    AnalyseDt.Rows[dstIndex].ItemArray[10] = analyseResult.Width;
                    AnalyseDt.Rows[dstIndex].ItemArray[11] = analyseResult.Space;
                    AnalyseDt.Rows[dstIndex].ItemArray[12] = String.Empty;
                    //AnalyseDt.Rows[dstIndex].ItemArray[13] = analyseResult.DefaultFormat();
                    AnalyseDt.Rows[dstIndex].ItemArray[13] = analyseResult.ReplaceCode;
                }
                else if (index > DataGridView_Result.Rows.Count)
                {
                    bool ret = false;
                    for (int k = 0; k < DataGridView_Result.Rows.Count; k++)
                    {
                        if (DataGridView_Result.Rows[k].Cells[1].Value == DataGridView_BOM.SelectedRows[i].Cells[CodeColumn].Value)
                        {
                            dstIndex = k;
                            AnalyseDt.Rows[dstIndex].ItemArray[2] = DataGridView_BOM.SelectedRows[i].Cells[DescrColumn].Value;
                            AnalyseDt.Rows[dstIndex].ItemArray[3] = analyseResult.Result;
                            AnalyseDt.Rows[dstIndex].ItemArray[4] = analyseResult.Type;
                            AnalyseDt.Rows[dstIndex].ItemArray[5] = analyseResult.Size;
                            AnalyseDt.Rows[dstIndex].ItemArray[6] = analyseResult.Value;
                            AnalyseDt.Rows[dstIndex].ItemArray[7] = analyseResult.Unit;
                            AnalyseDt.Rows[dstIndex].ItemArray[8] = analyseResult.Grade;
                            AnalyseDt.Rows[dstIndex].ItemArray[9] = String.Empty;
                            AnalyseDt.Rows[dstIndex].ItemArray[10] = analyseResult.Width;
                            AnalyseDt.Rows[dstIndex].ItemArray[11] = analyseResult.Space;
                            AnalyseDt.Rows[dstIndex].ItemArray[12] = String.Empty;
                            //AnalyseDt.Rows[dstIndex].ItemArray[13] = analyseResult.DefaultFormat();
                            AnalyseDt.Rows[dstIndex].ItemArray[13] = analyseResult.ReplaceCode;
                            ret = true;
                            break;
                        }
                    }
                    if (!ret)
                    {
                        AnalyseDt.Rows.Add(
                            (DataGridView_Result.Rows.Count > 0) ? (DataGridView_Result.Rows.Count - 1) : 0,
                            DataGridView_BOM.SelectedRows[CodeColumn].Cells[0].Value,
                            DataGridView_BOM.SelectedRows[DescrColumn].Cells[1].Value,
                            analyseResult.Result,
                            analyseResult.Type,
                            analyseResult.Size,
                            analyseResult.Value,
                            analyseResult.Unit,
                            analyseResult.Grade,
                            String.Empty,
                            analyseResult.Width,
                            analyseResult.Space,
                            String.Empty,
                            //analyseResult.DefaultFormat()
                            analyseResult.ReplaceCode
                            );
                        dstIndex = AnalyseDt.Rows.Count - 1;
                    }
                }
                else if (DataGridView_Result.Rows.Count == 0 ||
                    (index < DataGridView_Result.Rows.Count && DataGridView_Result.Rows[index].Cells[1].Value != DataGridView_BOM.SelectedRows[i].Cells[CodeColumn].Value))
                {
                    AnalyseDt.Rows.Add(
                        (DataGridView_Result.Rows.Count > 0) ? (DataGridView_Result.Rows.Count - 1) : 0,
                        DataGridView_BOM.SelectedRows[0].Cells[CodeColumn].Value,
                        DataGridView_BOM.SelectedRows[0].Cells[DescrColumn].Value,
                        analyseResult.Result,
                        analyseResult.Type,
                        analyseResult.Size,
                        analyseResult.Value,
                        analyseResult.Unit,
                        analyseResult.Grade,
                        String.Empty,
                        analyseResult.Width,
                        analyseResult.Space,
                        String.Empty,
                        //analyseResult.DefaultFormat()
                        analyseResult.ReplaceCode
                        );
                    dstIndex = AnalyseDt.Rows.Count - 1;
                }
                else if (index < DataGridView_Result.Rows.Count && DataGridView_Result.Rows[index].Cells[1].Value == DataGridView_BOM.SelectedRows[i].Cells[CodeColumn].Value)
                {
                    dstIndex = index;
                    AnalyseDt.Rows[dstIndex].ItemArray[DescrColumn] = DataGridView_BOM.SelectedRows[i].Cells[1].Value;
                    AnalyseDt.Rows[dstIndex].ItemArray[3] = analyseResult.Result;
                    AnalyseDt.Rows[dstIndex].ItemArray[4] = analyseResult.Type;
                    AnalyseDt.Rows[dstIndex].ItemArray[5] = analyseResult.Size;
                    AnalyseDt.Rows[dstIndex].ItemArray[6] = analyseResult.Value;
                    AnalyseDt.Rows[dstIndex].ItemArray[7] = analyseResult.Unit;
                    AnalyseDt.Rows[dstIndex].ItemArray[8] = analyseResult.Grade;
                    AnalyseDt.Rows[dstIndex].ItemArray[9] = String.Empty;
                    AnalyseDt.Rows[dstIndex].ItemArray[10] = analyseResult.Width;
                    AnalyseDt.Rows[dstIndex].ItemArray[11] = analyseResult.Space;
                    AnalyseDt.Rows[dstIndex].ItemArray[12] = String.Empty; ;
                    //AnalyseDt.Rows[dstIndex].ItemArray[13] = analyseResult.DefaultFormat();
                    AnalyseDt.Rows[dstIndex].ItemArray[13] = analyseResult.ReplaceCode;
                }

                DataGridView_Result.DataSource = AnalyseDt;
                DataGridView_Result.Refresh();
                for (int k = 0; k < DataGridView_Result.Columns.Count; k++)
                {
                    DataGridView_Result.Columns[k].SortMode = DataGridViewColumnSortMode.NotSortable;
                }
                DataGridView_Result.Columns["序号"].SortMode = DataGridViewColumnSortMode.Automatic;
                DataGridView_Result.Columns["result"].SortMode = DataGridViewColumnSortMode.Automatic;
                Thread.Sleep(100);

                DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
                if (DataGridView_Result.Rows[dstIndex].Cells[3].FormattedValue.ToString() == "False")
                {
                    dataGridViewCellStyle.ForeColor = Color.Red;
                }
                else
                {
                    dataGridViewCellStyle.ForeColor = Color.Green;
                }

                DataGridView_Result.ClearSelection();
                DataGridView_Result.Rows[dstIndex].Cells[3].Style = dataGridViewCellStyle;
                DataGridView_Result.Rows[dstIndex].Selected = true;
                DataGridView_Result.FirstDisplayedScrollingRowIndex = dstIndex;
            }

        }

        /// <summary>
        /// 分析所有行
        /// </summary>
        public async Task AnalyseAll()
        {
            await Task.Run(() =>
            {
                GetReplaceCodeKey();
                bool isInBom = false;
                bool isRepeat = false;
                bool isNoDataRow = false;
                for (int i = 0; i < DataGridView_BOM.RowCount; i++)
                {
                    if (tokenSource.IsCancellationRequested) break;
                    if (IsOKRow(i, out isInBom, out isRepeat, out isNoDataRow))
                    {
                        string columnPartNo = DictColIndex.Where(u => u.Value == CodeColumn).Single().Key;
                        string columnSpec = DictColIndex.Where(u => u.Value == DescrColumn).Single().Key;
                        string columnReplace = DictColIndex.Where(u => u.Value == ReplaceCodeColumn).Single().Key;
                        string columnwSpec = DictColIndex.Where(u => u.Value == 2).Single().Key;

                        string barCode = DataGridView_BOM.Rows[i].Cells[columnPartNo].Value.ToString().Trim();
                        string description = DataGridView_BOM.Rows[i].Cells[columnSpec].Value.ToString();
                        description = description.Replace("'", "");
                        string spec = DataGridView_BOM.Rows[i].Cells[columnwSpec].Value.ToString();
                        string replaceCode = DataGridView_BOM.Rows[i].Cells[columnReplace].Value.ToString();
                        AnalyseMethod(barCode, description, spec, replaceCode);
                    }
                    if (isInBom)
                    {
                        HistoryCount++;
                    }
                    if (isNoDataRow)
                    {
                        NoDataCount++;
                    }
                    if (isRepeat)
                    {
                        string columnPartNo = DictColIndex.Where(u => u.Value == CodeColumn).Single().Key;
                        string barCode = DataGridView_BOM.Rows[i].Cells[columnPartNo].Value.ToString();
                        if (!DicRepeatCode.ContainsKey(barCode))
                        {
                            DicRepeatCode.Add(barCode, 1);
                        }
                        else
                        {
                            DicRepeatCode[barCode] += 1;
                        }
                    }
                }
                FloatingTip.ShowOk("解析完成".tr());
            }, tokenSource.Token);
        }

        private void GetReplaceCodeKey()
        {
            keyValuePairsReplaceKey.Clear();
            bool isInBom = false;
            bool isRepeat = false;
            bool isNoDataRow = false;
            for (int i = 0; i < DataGridView_BOM.RowCount; i++)
            {
                if (IsOKRow(i, out isInBom, out isRepeat, out isNoDataRow))
                {
                    List<string> strs = new List<string>();
                    if (CommonAnalyse.Instance.IsHadReplaceCode)
                    {
                        string key = DataGridView_BOM.Rows[i].Cells[ReplaceCodeColumn].Value.ToString();
                        string columnPartNo = DictColIndex.Where(u => u.Value == CodeColumn).Single().Key;
                        string barCode = DataGridView_BOM.Rows[i].Cells[columnPartNo].Value.ToString();
                        if (!keyValuePairsReplaceKey.ContainsKey(key))
                        {
                            strs.Add(barCode);
                            keyValuePairsReplaceKey.Add(key, strs);
                        }

                        if (keyValuePairsReplaceKey.ContainsKey(key))
                        {
                            if (!keyValuePairsReplaceKey[key].Contains(barCode))
                            {
                                keyValuePairsReplaceKey[key].Add(barCode);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 判断解析行是否可以解析
        /// </summary>
        /// <returns></returns>
        private bool IsOKRow(int i, out bool IsInBom, out bool IsRepeat, out bool IsNoDataRow)
        {
            IsInBom = false;
            IsRepeat = false;
            IsNoDataRow = false;
            if (DataGridView_BOM.Rows[i].IsNewRow || DataGridView_BOM.Rows[i].Cells[CodeColumn].Value == null)
            {
                return false;
            }
            if (string.IsNullOrEmpty(DataGridView_BOM.Rows[i].Cells[CodeColumn].Value.ToString()))
            {
                IsNoDataRow = true;
                return false;
            }
            if (DataGridView_BOM.Rows[i].Cells[DescrColumn].Value == null)
            {
                IsNoDataRow = true;
                UpdataAnalyseData(new AnalyseResult { BarCode = DataGridView_BOM.Rows[i].Cells[CodeColumn].Value.ToString() });
                return false;
            }

            string barCode = DataGridView_BOM.Rows[i].Cells[CodeColumn].Value.ToString();
            if (BomManager.Instance.AllBomData.Where(x => x.barCode == barCode).ToList().Count > 0)
            {
                //如果bom中有这个条码就不解析了
                IsInBom = true;
                return false;
            }

            if (codes.Contains(barCode))
            {
                //如果条码已经解析了，不再解析
                IsRepeat = true;
                return false;
            }
            return true;
        }

        public void AnalyseMethod(string barcode, string description, string spec, string replaceCodeRule = "")
        {
            BomDataModel bomData = BomManager.Instance.SearchByBarCode(barcode);
            AnalyseResult analyseResult = new AnalyseResult();
            if (bomData != null)
            {
                analyseResult.BarCode = bomData.barCode;
                analyseResult.Description = bomData.description;
                analyseResult.Value = bomData.value;
                analyseResult.Type = bomData.type;
                analyseResult.Unit = bomData.unit;
                analyseResult.Size = bomData.size;
                analyseResult.Grade = bomData.grade;
            }
            else
            {
                try
                {
                    analyseResult = CommonAnalyse.Instance.AnalyseMethod_copy(description);
                }
                catch (Exception ex)
                {
                    APP.Tip.ShowTip(0, "警告", ex.Message);
                }
                analyseResult.BarCode = barcode;
                analyseResult.Description = description;
                //CommonAnalyse.Instance.AnalyWidth(spec, ref analyseResult);
            }

            //替代料号
            if (CommonAnalyse.Instance.IsHadReplaceCode && !String.IsNullOrEmpty(replaceCodeRule))
            {
                if (CommonAnalyse.Instance.IsHadReplaceCodeCol)
                {
                    if (!String.IsNullOrEmpty(analyseResult.ReplaceCode))
                    {
                        if (!analyseResult.ReplaceCode.Contains(replaceCodeRule))
                        {
                            analyseResult.ReplaceCode = analyseResult.ReplaceCode + "," + replaceCodeRule;
                        }
                    }
                    else
                    {
                        analyseResult.ReplaceCode = replaceCodeRule;
                    }
                }

                if (CommonAnalyse.Instance.IsSameLocationRule)
                {
                    try
                    {
                        if (keyValuePairsReplaceKey != null)
                        {
                            List<string> listStr = new List<string>(keyValuePairsReplaceKey[replaceCodeRule].ToArray());
                            listStr.Remove(barcode);
                            for (int i = 0; i < listStr.Count; i++)
                            {
                                if (i == 0)
                                {
                                    analyseResult.ReplaceCode = listStr[0];
                                }
                                else
                                {
                                    analyseResult.ReplaceCode += "," + listStr[i];
                                }

                            }
                        }
                    }
                    catch
                    {

                    }

                }
            }
            if (analyseResult.Type == "电阻" || analyseResult.Type == "电容")
            {
                analyseResult.Quarity = replaceCodeRule.Split(',').ToList().Count;
                analyseResult.Position = replaceCodeRule;
            }
           

            UpdataAnalyseData(analyseResult);
        }

        public void StopAnalyse()
        {
            isStart = false;
            tokenSource?.Cancel();
        }

        int AllCount = 0;
        int RepeatCount = 0;
        int OKCount = 0;
        int NGCount = 0;
        int HistoryCount = 0;
        int NoDataCount = 0;
        Dictionary<string, int> DicRepeatCode = new Dictionary<string, int>();
        Queue<AnalyseResult> analyseResults = new Queue<AnalyseResult>();

        /// <summary>
        /// 更新解析数据
        /// </summary>
        /// <param name="analyseResult"></param>
        private void UpdataAnalyseData(AnalyseResult analyseResult)
        {

            string grade = analyseResult.Grade.Replace("%", "");
            decimal val = 0;

            if (decimal.TryParse(grade, out decimal gradeDecimal) && decimal.TryParse(analyseResult.Value, out  val))
            {

                analyseResult.MinValue = val - (val * gradeDecimal) / 100;
                analyseResult.MaxValue = val + (val * gradeDecimal) / 100;
            }


            SQLDataControl.InsertEngBom(
                ProductCode, 
                analyseResult.Position, 
                analyseResult.BarCode,
                analyseResult.Description,
                analyseResult.Quarity,
                 AnalyseDt.Rows.Count,
                 analyseResult.Type,
                 val,
                 analyseResult.Unit,
                 analyseResult.MaxValue,
                 analyseResult.MinValue,
                 analyseResult.Size,
                "Test",
                 gradeDecimal,
                 gradeDecimal,
                 "%"
                );

            AnalyseDt.Rows.Add
                (
                           AnalyseDt.Rows.Count,
                           analyseResult.BarCode,
                           analyseResult.Description,
                           analyseResult.Result,
                           analyseResult.Type,
                           analyseResult.Size,
                           analyseResult.Value,
                           analyseResult.Unit,
                           analyseResult.Grade,
                           analyseResult.MinValue,
                           analyseResult.MaxValue,
                           analyseResult.Position,
                           analyseResult.Quarity,
                           analyseResult.Width,
                           analyseResult.Space,
                           String.Empty,
                           analyseResult.ReplaceCode
                );
            if (analyseResult.Result)
            {
                OKCount++;
            }
            else
            {
                NGCount++;
            }
            //AllCount++;
            codes.Add(analyseResult.BarCode);
            UpdataResult();
        }

        private void UpdataResult()
        {
            this.Invoke(new Action(() =>
            {
                //lbResult.Text = "总数".tr() + ":" + AllCount + "成功".tr() + ":" + OKCount + "失败".tr() + ":" + NGCount;
                lblTotalRows.Text = AllCount.ToString();
                lblAnalyseSuccessRows.Text = OKCount.ToString();
                if (AllCount > 0)
                {
                    lblAnalyseSuccessRows.Text += " " + String.Format("({0}%)", (((double)OKCount / AllCount) * 100).ToString("0"));
                }
                lblAnalyseFailRows.Text = NGCount.ToString();
                lblHistoryRows.Text = HistoryCount.ToString();
                int cnt = 0;
                if (DicRepeatCode.Keys.Count > 0)
                {
                    foreach (int value in DicRepeatCode.Values)
                    {
                        cnt += value;
                    }
                    lblRepeatRows.Text = cnt.ToString();// + DicRepeatCode.Keys.ToList()[0];
                }
                lblNoDataRows.Text = NoDataCount.ToString();
                //if (AllCount % 5000 == 0)
                //{
                //    DataGridView_Result.DataSource = null;
                //    DataGridView_Result.DataSource = AnalyseDt;
                //    DataGridView_Result.Refresh();
                //    for (int i = 0; i < DataGridView_Result.Columns.Count; i++)
                //    {
                //        DataGridView_Result.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                //    }
                //}

            }));
        }

        private void ResetResult()
        {
            this.Invoke(new Action(() =>
            {
                //lbResult.Text = "总数".tr() + ":" + AllCount + "成功".tr() + ":" + OKCount + "失败".tr() + ":" + NGCount;
                lblTotalRows.Text = String.Empty;
                lblAnalyseSuccessRows.Text = String.Empty;
                lblAnalyseFailRows.Text = String.Empty;
                lblHistoryRows.Text = String.Empty;
            }));
        }

        public void SetColor()
        {
            for (int i = 0; i < DataGridView_Result.Rows.Count; i++)
            {
                DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
                if (DataGridView_Result.Rows[i].Cells[3].FormattedValue.ToString() == "False")
                {
                    dataGridViewCellStyle.ForeColor = Color.Red;
                }
                else
                {
                    dataGridViewCellStyle.ForeColor = Color.Green;
                }
                DataGridView_Result.Rows[i].Cells["result"].Style = dataGridViewCellStyle;
            }
        }

        /// <summary>
        /// 保存BOM
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show("产品名称不能为空".tr());
                return;
            }
            Save();
        }

        private void Save()
        {
            //BomManager.Instance.SaveInAllBomData(txtName.Text, DataGridView_Result);
            //if (MessageBox.Show("保存成功，是否作为当前BOM？".tr(), "提示".tr(), MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            //{
            //    BomManager.Instance.SaveInCurrentBomData(txtName.Text, DataGridView_Result);
            //    MessageBox.Show("已经设置为当前BOM".tr(), "提示".tr(), MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
            for (int i = 0; i < DataGridView_Result.RowCount; i++)
            {

              
            }



        }

        /// <summary>
        /// 合并按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMerge_Click(object sender, EventArgs e)
        {
            Merge();
        }

        /// <summary>
        /// 合并符
        /// </summary>
        private string MergeChar = "";
        /// <summary>
        /// 合并
        /// </summary>
        private void Merge()
        {
            if (dt == null)
            {
                return;
            }
            if (!dt.Columns.Contains(txtColumn1.Text.Trim()) || !dt.Columns.Contains(txtColumn2.Text.Trim()))
            {
                return;
            }

            if (CommonAnalyse.Instance.IsSeparator)
            {
                List<Separator> splitCharList = CommonAnalyse.Instance.Separators.Where(u => u.Enable == true).ToList();
                if (splitCharList.Count > 0)
                {
                    MergeChar = splitCharList[0].Acsii;
                }
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];
                row.BeginEdit();
                row[txtColumn1.Text.Trim()] = row[txtColumn1.Text.Trim()].ToString() + MergeChar + row[txtColumn2.Text.Trim()].ToString();
                row.EndEdit();
                dt.AcceptChanges();
            }
            dt.Columns.Remove(dt.Columns[txtColumn2.Text.Trim()]);

            this.DictColIndex?.Clear();

            if (dt != null)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    this.DictColIndex.Add(dt.Columns[i].Caption, i);
                }
            }
        }

        private void btnShowModelData_Click(object sender, EventArgs e)
        {
            ShowModelData();
        }

        private void ShowModelData()
        {
            AnalyseMainForm.f.ShowModelData();
        }

        private void btnDeleteSelectRow_Click(object sender, EventArgs e)
        {
            if (dt == null)
            {
                return;
            }
            int count = DataGridView_BOM.SelectedRows.Count;
            for (int i = 0; i < count;)
            {
                dt.Rows.RemoveAt(DataGridView_BOM.SelectedRows[i].Index);
                count = DataGridView_BOM.SelectedRows.Count;
            }

        }

        private void btnDeleteSelectColumn_Click(object sender, EventArgs e)
        {
            if (dt == null)
            {
                return;
            }
            if (DataGridView_BOM.SelectedCells.Count > 0)
                dt.Columns.RemoveAt(DataGridView_BOM.SelectedCells[col].ColumnIndex);

        }
        int col = 0;
        private void DataGridView_BOM_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }
            int index = e.RowIndex;
            col = e.ColumnIndex;
            //DataGridView_BOM.Columns[e.ColumnIndex].Selected = true;
            DataGridView_BOM.Rows[index].Selected = true;
        }

        private void DataGridView_Result_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }
            int index = e.RowIndex;

            DataGridView_Result.Rows[index].Selected = true;
        }

        private void btnShowModelData_MouseHover(object sender, EventArgs e)
        {
            showTips(btnShowModelData, "搜索".tr());
        }

        private void btnImport_MouseHover(object sender, EventArgs e)
        {
            showTips(btnImport, "导入文件".tr());
        }

        private void btnStart_MouseHover(object sender, EventArgs e)
        {
            showTips(btnStart, "开始解析".tr());
        }

        private void btnSave_MouseHover(object sender, EventArgs e)
        {
            showTips(btnSave, "保存".tr());
        }

        private void BtnGetCodeCol_Click(object sender, EventArgs e)
        {
            if (DataGridView_BOM.CurrentCell.ColumnIndex >= 0)
            {
                CodeColumn = DataGridView_BOM.CurrentCell.ColumnIndex;
                txtCodeCol.Text = DictColIndex.Where(u => u.Value == CodeColumn).Single().Key;
            }
            else
            {
                CodeColumn = 0;
                txtCodeCol.Text = DictColIndex.Where(u => u.Value == CodeColumn).Single().Key;
            }
        }

        private void BtnGetDescriptionCol_Click(object sender, EventArgs e)
        {
            if (DataGridView_BOM.CurrentCell.ColumnIndex >= 0)
            {
                DescrColumn = DataGridView_BOM.CurrentCell.ColumnIndex;
                txtDescriptionCol.Text = DictColIndex.Where(u => u.Value == DescrColumn).Single().Key;
            }
            else
            {
                DescrColumn = 1;
                txtDescriptionCol.Text = DictColIndex.Where(u => u.Value == DescrColumn).Single().Key;
            }
        }

        private void BtnTestAnaly_Click(object sender, EventArgs e)
        {
            if (DataGridViewAnalyTest.Rows.Count <= 0)
            {
                return;
            }

            for (int i = 0; i < DataGridViewAnalyTest.Rows.Count; i++)
            {
                if (DataGridViewAnalyTest.Rows[i].Cells[0].Value == null)
                {
                    continue;
                }
                string description = DataGridViewAnalyTest.Rows[i].Cells[0].Value.ToString();
                for (int k = 1; k < DataGridViewAnalyTest.Columns.Count; k++)
                {
                    DataGridViewAnalyTest.Rows[i].Cells[k].Value = null;
                    DataGridViewAnalyTest.Rows[i].Cells[k].Value = null;
                    DataGridViewAnalyTest.Rows[i].Cells[k].Value = null;
                    DataGridViewAnalyTest.Rows[i].Cells[k].Value = null;
                    DataGridViewAnalyTest.Rows[i].Cells[k].Value = null;
                    DataGridViewAnalyTest.Rows[i].Cells[k].Value = null;
                }
                if (!String.IsNullOrEmpty(description))
                {
                    AnalyseResult analyseResult = new AnalyseResult();
                    try
                    {
                        analyseResult = CommonAnalyse.Instance.AnalyseMethod_copy(description);
                    }
                    catch (Exception ex)
                    {
                        APP.Tip.ShowTip(0, "警告", ex.Message);
                    }
                    if (analyseResult.Result)
                    {
                        DataGridViewAnalyTest.Rows[i].Cells[1].Value = analyseResult.Result;
                        DataGridViewAnalyTest.Rows[i].Cells[2].Value = analyseResult.Type;
                        DataGridViewAnalyTest.Rows[i].Cells[3].Value = analyseResult.Size;
                        DataGridViewAnalyTest.Rows[i].Cells[4].Value = analyseResult.Value;
                        DataGridViewAnalyTest.Rows[i].Cells[5].Value = analyseResult.Unit;
                        DataGridViewAnalyTest.Rows[i].Cells[6].Value = analyseResult.Grade;
                    }
                    else
                    {
                        DataGridViewAnalyTest.Rows[i].Cells[1].Value = analyseResult.Result;
                    }
                    DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
                    if (DataGridViewAnalyTest.Rows[i].Cells[1].FormattedValue.ToString() == "False")
                    {
                        dataGridViewCellStyle.ForeColor = Color.Red;
                    }
                    else
                    {
                        dataGridViewCellStyle.ForeColor = Color.Green;
                    }
                    DataGridViewAnalyTest.Rows[i].Cells[1].Style = dataGridViewCellStyle;
                }
            }
            MessageBox.Show("解析完成".tr(), "解析测试".tr(), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnClearTestAnaly_Click(object sender, EventArgs e)
        {
            DataGridViewAnalyTest.Rows.Clear();
        }

        private void btnExportFile_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();

            for (int i = 0; i < DataGridView_Result.Columns.Count; i++)
            {
                Type type = DataGridView_Result.Columns[i].HeaderText.GetType();
                dt.Columns.Add(DataGridView_Result.Columns[i].HeaderText, type);
            }

            for (int i = 0; i < DataGridView_Result.Rows.Count; i++)
            {
                string[] strings = new string[DataGridView_Result.Columns.Count];
                for (int j = 0; j < DataGridView_Result.Columns.Count; j++)
                {
                    strings[j] = DataGridView_Result.Rows[i].Cells[j].FormattedValue.ToString();
                }
                dt.Rows.Add(strings);
            }

            NOPIHelper.ExportDataToExcel(dt);
        }

        private void BtnReplaceCodeCol_Click(object sender, EventArgs e)
        {
            if (DataGridView_BOM.CurrentCell.ColumnIndex >= 0)
            {
                ReplaceCodeColumn = DataGridView_BOM.CurrentCell.ColumnIndex;
                txtReplaceCodeCol.Text = DictColIndex.Where(u => u.Value == ReplaceCodeColumn).Single().Key;
            }
            else
            {
                ReplaceCodeColumn = 2;
                txtReplaceCodeCol.Text = DictColIndex.Where(u => u.Value == ReplaceCodeColumn).Single().Key;
            }
        }


        public void GetReplaceCodeMethod()
        {
            if (CommonAnalyse.Instance.IsHadReplaceCode)
            {
                if (CommonAnalyse.Instance.IsSameLocationRule)
                {

                }
            }
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            AnalyseMainForm analyseMainForm = new AnalyseMainForm(); analyseMainForm.ShowDialog();
        }
    }
}
