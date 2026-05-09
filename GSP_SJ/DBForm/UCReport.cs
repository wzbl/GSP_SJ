using BorwinAnalyse.BaseClass;
using Emgu.CV.CvEnum;
using Emgu.CV.UI.GLView;
using SqlHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSP_SJ
{
    public partial class UCReport : UserControl
    {
        /// <summary>
        ///主界面新建报告
        /// </summary>
        public UCReport()
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
            InitUIBind();
        }

        private void InitUIBind()
        {
            foreach (var item in SQLDataControl.GetAllProgramm())
            {
                comProductCode.Items.Add(item.产品编号);
            }
            List<Eng_MeterOption> _Customers = SQLDataControl.GetMeterOption();
            for (int i = 0; i < _Customers.Count; i++)
            {
                comOptionCode.Items.Add(_Customers[i].OptionName);
            }

            comBoardSide.Items.Add("S");
            comBoardSide.Items.Add("T");
            comBoardSide.Items.Add("B");
            lbCheckResult.Text = "";
        }

        private string bigImgPath = "";

        /// <summary>
        /// 重报告中打开查看
        /// </summary>
        /// <param name="sender"></param>
        public UCReport(P_Man_Report_Search_Result _Report_Search) : this()
        {
            RefreshData(_Report_Search);
        }

        public void RefreshData(P_Man_Report_Search_Result _Report_Search)
        {
            string status = "";
            switch (_Report_Search.Status)
            {
                case "NEW":
                    status = "检测中";
                    break;
                case "CLOSE":
                    status = "已结束";
                    break;
                case "UNABLE":
                    status = "不可测";
                    break;
            }
            string _class = "";
            switch (_Report_Search.@class)
            {
                case "10":
                    _class = "白班";
                    break;
                default:
                    _class = "晚班";
                    break;
            }
            string checkType = "";
            switch (_Report_Search.CheckType)
            {
                case "10":
                    checkType = "首检";
                    break;
                case "20":
                    checkType = "巡检";
                    break;
                case "30":
                    checkType = "尾检";
                    break;
                case "40":
                    checkType = "Faist inspection";
                    break;
            }

            txtReportCode.Text = _Report_Search.ReportCode;
            comBoardSide.Text = _Report_Search.BoardSide; comBoardSide.Enabled = false;
            comProductCode.Text = _Report_Search.ProductCode; comProductCode.Enabled = false;
            txtProductName.Text = _Report_Search.ProductName; txtProductName.Enabled = false;
            comStatus.Text = status;
            txtCreator.Text = _Report_Search.Creator;
            txtCreationDate.Text = _Report_Search.CreationDate.ToString();
            txtBomVersion.Text = _Report_Search.BomVersion;
            txtBarcode.Text = _Report_Search.Barcode;
            comOptionCode.Text = SQLDataControl.GetMeterOptionCode(_Report_Search.OptionCode);
            txtPCBVersion.Text = _Report_Search.PCBVersion;
            comcheckType.Text = checkType;
            comPLCode.Text = _Report_Search.PLCode;
            comClass.Text = _class;
            txtBatchQty.Text = _Report_Search.BatchQty.ToString();
            txtWoCode.Text = _Report_Search.WoCode;
            txtLotNumber.Text = _Report_Search.LotNumber;
            numtxtBoardQty.Text = _Report_Search.BoardQty.ToString(); numtxtBoardQty.Enabled = false;
            txtAutomationQty.Text = _Report_Search.AutomationQty.ToString();
            txtManualQty.Text = _Report_Search.ManualQty.ToString();
            txtPassQty.Text = _Report_Search.PassQty.ToString();
            txtFailQty.Text = _Report_Search.FailQty.ToString();
            txtMissQty.Text = _Report_Search.MissQty.ToString();
            txtFinishedBy.Text = _Report_Search.FinishedBy;
            txtsForceFinish.Text = _Report_Search.IsForceFinish;
            FinishedDate.Text = _Report_Search.FinishedDate.ToString();
            lbCheckResult.Text = _Report_Search.CheckResult;
            txtModifier.Text = _Report_Search.Modifier;
            txtModificationDate.Text = _Report_Search.ModificationDate.ToString();
            txtCheckTime.Text = _Report_Search.CheckTime.ToString();
            txtAllTime.Text = (_Report_Search.CheckTime / 60.00).ToString();
            if (_Report_Search.CheckResult == "PASS")
            {
                lbCheckResult.StateCommon.ShortText.Color1 = Color.Green;
            }
            else
            {
                lbCheckResult.StateCommon.ShortText.Color1 = Color.Red;
            }
            search(txtReportCode.Text);
            bigImgPath = _Report_Search.PicturePath;
            comIsCheckNoSMD.Text = _Report_Search.IsCheckNoSMD;
            richRemarks.Text = _Report_Search.Remarks;
        }

        public bool IsAddReport = false;
        /// <summary>
        /// 新建报告
        /// </summary>
        public UCReport(bool isNew) : this()
        {
            IsAddReport = true;
            txtReportCode.Text = DateTime.Now.ToString("yyyyMMddHHmmss");
            comProductCode.SelectedIndexChanged += new EventHandler(comProductCode_SelectedIndexChanged);
            btnStart.Enabled = false;
        }

        private void comProductCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            //选择程序
            View_Eng_Program _Program = SQLDataControl.GetProgramm(comProductCode.Text);
            numtxtBoardQty.Value = 1;
            txtProductName.Text = _Program.产品名称;
            txtBomVersion.Text = _Program.版本;
            comBoardSide.SelectedIndex = 0;//_Program.板面;
        }

        private void search(string reportCode)
        {
            dgvXYData.Rows.Clear();
            List<Man_ReportItem> _ReportItems = SQLDataControl.Search_Man_ReportItem(reportCode);
            int Rcount = 0;
            int Ccount = 0;
            int Lcount = 0;
            int Ocount = 0;
            int DCRcount = 0;
            int SmdCount = 0;
            foreach (var item in _ReportItems)
            {
                string type = "";
                switch (item.LcrType)
                {
                    case "R":
                        //电阻
                        Rcount++;
                        type = "电阻";
                        break;
                    case "C":
                        //电容
                        Ccount++;
                        type = "电容";
                        break;
                    case "L":
                        //电感
                        Lcount++;
                        type = "电感";
                        break;
                    case "B":
                        //DCR
                        DCRcount++;
                        type = "DCR";
                        break;
                    case "D":
                        //LED
                        break;
                    default:
                        //其他
                        Ocount++;
                        type = "其他";
                        break;
                }
                dgvXYData.Rows.Add(
                    item.Position,  //位置
                    item.BoardId,  //板ID
                    item.IsSMD,     //贴装
                    item.CheckResult, //检查结果
                    type,
                    item.MaterialCode,
                    item.MaterialName,
                    item.LcrStandardValue,
                    item.LcrUnitCode,
                    item.LcrMinValue,
                    item.LcrCheckValue,
                    item.LcrMaxValue,
                    item.Size,
                    item.ResultType,
                    item.FailCause,
                    "",   //元件丝印
                    item.MaxTolerance,
                    item.MinTolerance,
                    item.ToleranceType,
                    item.BomSequence,
                    item.Frequency,
                    item.Remarks,
                    item.Creator,
                    item.CreationDate
                );

                if (item.IsSMD == "YES")
                {
                    SmdCount++;
                }
                if (item.CheckResult == "PASS")
                {
                    dgvXYData.Rows[dgvXYData.Rows.Count - 1].Cells[3].Style.ForeColor = Color.Lime;
                }
                else
                    dgvXYData.Rows[dgvXYData.Rows.Count - 1].Cells[3].Style.ForeColor = Color.Red;

                if (item.ResultType == "manual")
                {
                    dgvXYData.Rows[dgvXYData.Rows.Count - 1].Cells[13].Style.ForeColor = Color.Green;
                }else if (item.ResultType == "auto")
                {
                    dgvXYData.Rows[dgvXYData.Rows.Count - 1].Cells[13].Style.ForeColor = Color.Blue;
                }

            }
            txtSMDQty.Text = SmdCount.ToString();
            txtNoSMDQty.Text = (_ReportItems.Count - SmdCount).ToString();
            txt电阻QTY.Text = Rcount.ToString();
            txt电容QTY.Text = Ccount.ToString();
            txt电感QTY.Text = Lcount.ToString();
            txtOtherQty.Text = Ocount.ToString();
            txtDCRQTY.Text = DCRcount.ToString();

            txtTotalQty.Text = _ReportItems.Count.ToString();

        }

        private void btnSearchProgramm_Click(object sender, EventArgs e)
        {
            try
            {
                string productCode = comProductCode.Text;
                if (string.IsNullOrEmpty(productCode))
                    return;
                DBEventAction.ProguceItem?.Invoke(SQLDataControl.GetAllProgramm().Where(x => x.产品编号 == productCode).First());
            }
            catch (Exception)
            {

            }

        }

        private void btnSearchCompensationParam_Click(object sender, EventArgs e)
        {
            FormCompensationParam compensationParam = new FormCompensationParam();
            compensationParam.ShowDialog();
        }

        private void btnQA_Click(object sender, EventArgs e)
        {
            btnSave_Click(null, null);
            SQLDataControl.SetQAMan_Report(txtReportCode.Text, DBEventAction.User.UserName);
            DBEventAction.RefreshReport?.Invoke(txtReportCode.Text);
        }

        /// <summary>
        /// 导出坐标数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportDGV_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvXYData.Rows.Count == 0)
                {
                    return;
                }
                NOPIHelperEX.ExportDataToExcel(NOPIHelperEX.ToDataTable(dgvXYData), comProductCode.Text + txtReportCode.Text);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnCheckExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvXYData.Rows.Count == 0)
                {
                    return;
                }
                NOPIHelperEX.ExportDataToExcel(NOPIHelperEX.ToDataTable(dgvXYData), "检测记录" + txtReportCode.Text);
            }
            catch (Exception ex)
            {

            }
        }


        /// <summary>
        /// 导出PDF报告
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportPDF_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            //设置文件标题
            saveFileDialog.Title = "导出报告";
            //设置文件类型
            saveFileDialog.Filter = "PDF (*.pdf)|*.pdf";
            //设置默认文件类型显示顺序  
            saveFileDialog.FilterIndex = 1;
            //是否自动在文件名中添加扩展名
            saveFileDialog.AddExtension = true;
            //是否记忆上次打开的目录
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.FileName = comProductCode.Text + txtReportCode.Text;
            //按下确定选择的按钮  
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                List<PDFHeader> headerList = GetPDFHeaders();
                List<InspectionItem> inspectionItemList = GetInspectionItemList();
                string[] headers = { "序号", "位号", "结果", "贴装", "料号", "描述", "标准", "实测", "备注", "检测人", "检测时间" };
                //string bigImagePath = @"C:\Users\14802\Desktop\A.png";
                Man_Report _Reports = SQLDataControl.GetMan_Report(txtReportCode.Text);
                //_Reports.First().Picture
                PDFHepler.ExportPDF(saveFileDialog.FileName, "首件检测报告", headerList, null, _Reports.Picture, headers, inspectionItemList);
            }
        }

        public List<PDFHeader> GetPDFHeaders()
        {
            List<PDFHeader> headerList = new List<PDFHeader>();
            headerList.Add(new PDFHeader() { Title = "报告编号", Value = txtReportCode.Text });
            headerList.Add(new PDFHeader() { Title = "检测类型", Value = comcheckType.Text });
            headerList.Add(new PDFHeader() { Title = "检测状态", Value = comStatus.Text });
            headerList.Add(new PDFHeader() { Title = "检测结果", Value = lbCheckResult.Text });
            headerList.Add(new PDFHeader() { Title = "检测耗时", Value = txtAllTime.Text });
            headerList.Add(new PDFHeader() { Title = "板面", Value = comBoardSide.Text });
            headerList.Add(new PDFHeader() { Title = "元件总数", Value = txtTotalQty.Text });
            headerList.Add(new PDFHeader() { Title = "PASS数量", Value = txtPassQty.Text });
            headerList.Add(new PDFHeader() { Title = "FAIL数量", Value = txtFailQty.Text });
            headerList.Add(new PDFHeader() { Title = "漏检数量", Value = txtMissQty.Text });
            headerList.Add(new PDFHeader() { Title = "自动判定", Value = txtAutomationQty.Text });
            headerList.Add(new PDFHeader() { Title = "人工判定", Value = txtManualQty.Text });
            headerList.Add(new PDFHeader() { Title = "线体", Value = comPLCode.Text });
            headerList.Add(new PDFHeader() { Title = "班别", Value = comClass.Text });
            headerList.Add(new PDFHeader() { Title = "工单编号", Value = txtWoCode.Text });
            headerList.Add(new PDFHeader() { Title = "批次数量", Value = txtBatchQty.Text });
            headerList.Add(new PDFHeader() { Title = "批次号", Value = txtLotNumber.Text });
            headerList.Add(new PDFHeader() { Title = "产品编号", Value = comProductCode.Text });
            headerList.Add(new PDFHeader() { Title = "产品名称", Value = txtProductName.Text });
            headerList.Add(new PDFHeader() { Title = "条码", Value = txtBarcode.Text });
            headerList.Add(new PDFHeader() { Title = "贴装", Value = txtSMDQty.Text });
            headerList.Add(new PDFHeader() { Title = "空贴", Value = txtNoSMDQty.Text });
            headerList.Add(new PDFHeader() { Title = "电阻", Value = txt电阻QTY.Text });
            headerList.Add(new PDFHeader() { Title = "电容", Value = txt电容QTY.Text });
            headerList.Add(new PDFHeader() { Title = "电感", Value = txt电感QTY.Text });
            headerList.Add(new PDFHeader() { Title = "二极管", Value = txt二极管QTY.Text });
            headerList.Add(new PDFHeader() { Title = "三极管", Value = txt三级管QTY.Text });
            headerList.Add(new PDFHeader() { Title = "IC", Value = txtICQTY.Text });
            headerList.Add(new PDFHeader() { Title = "DCR", Value = txtDCRQTY.Text });
            headerList.Add(new PDFHeader() { Title = "其他", Value = txtOtherQty.Text });
            headerList.Add(new PDFHeader() { Title = "BOM版本", Value = txtBomVersion.Text });
            headerList.Add(new PDFHeader() { Title = "PCB版本", Value = txtPCBVersion.Text });
            return headerList;
        }

        public List<InspectionItem> GetInspectionItemList()
        {
            List<InspectionItem> inspectionItemList = new List<InspectionItem>();
            List<Man_ReportItem> _ReportItems = SQLDataControl.Search_Man_ReportItem(txtReportCode.Text);
            int seq = 1;
            foreach (var item in _ReportItems)
            {
                iTextSharp.text.Image standardImage = null;//iTextSharp.text.Image.GetInstance(@"C:\Users\14802\Desktop\B.png");
                if (item.StandardImage != null)
                {
                    standardImage = iTextSharp.text.Image.GetInstance(item.StandardImage);
                }

                iTextSharp.text.Image actualImage = null;//iTextSharp.text.Image.GetInstance(@"C:\Users\14802\Desktop\B.png");
                if (item.CurrentImage != null)
                {
                    actualImage = iTextSharp.text.Image.GetInstance(item.CurrentImage);
                }
                string remark = item.Remarks;
                if (item.LcrType == "R" || item.LcrType == "C")
                {
                    remark = item.LcrCheckValue + item.LcrUnitCode + "(" + item.LcrMinValue + "," + item.LcrMaxValue + ")";
                }

                inspectionItemList.Add(new InspectionItem()
                {
                    Seq = seq,
                    Position = item.Position,
                    Result = item.CheckResult,
                    Mount = item.IsSMD,
                    PartNo = item.MaterialCode,
                    Desc = item.MaterialName,
                    StandardImage = standardImage,
                    ActualImage = actualImage,
                    Remark = remark,
                    CheckUser = item.Creator,
                    CheckTime = item.CreationDate.ToString()
                });
                seq++;
            }
            return inspectionItemList;
        }

        /// <summary>
        /// 保存数据到Man_Report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            Save();
            MessageBox.Show("保存成功");
        }

        private void Save()
        {
            string checkType = "";
            switch (comcheckType.Text)
            {
                case "首检":
                    checkType = "10";
                    break;
                case "巡检":
                    checkType = "20";
                    break;
                case "尾检":
                    checkType = "30";
                    break;
                case "Faist inspection":
                    checkType = "40";
                    break;
            }

            string _class = "";
            switch (comClass.Text)
            {
                case "白班":
                    _class = "10";
                    break;
                default:
                    _class = "20";
                    break;
            }

            int batchQty = 0;
            if (!int.TryParse(txtBatchQty.Text, out batchQty))
            {
                batchQty = 0;
            }

            if (IsAddReport)
            {
                List<Eng_XYData> _XYDatas = SQLDataControl.GetEng_XYDatas(comProductCode.Text);
                //添加
                Man_Report report = new Man_Report();
                report.ReportCode = txtReportCode.Text;
                report.BoardSide = comBoardSide.Text;
                report.PLCode = comPLCode.Text;
                report.ProductCode = comProductCode.Text;
                report.ProductName = txtProductName.Text;
                report.WoCode = txtWoCode.Text;
                report.LotNumber = txtLotNumber.Text;
                report.BatchQty = batchQty;
                report.BoardQty = int.Parse(numtxtBoardQty.Value.ToString());
                report.IsCheckNoSMD = comIsCheckNoSMD.Text;
                report.Remarks = richRemarks.Text;
                report.Creator = DBEventAction.User.UserName;
                report.Modifier = DBEventAction.User.UserName;
                report.OptionCode = SQLDataControl.GetMeterOptionName(comOptionCode.Text);
                report.Barcode = txtBarcode.Text;
                report.@class = _class;
                report.CheckType = checkType;
                report.PCBVersion = txtPCBVersion.Text;
                report.BomVersion = txtBomVersion.Text;
                report.DeviceName = "";
                report.TotalQty = _XYDatas.Count;
                report.RQty = _XYDatas.Where(x => x.LcrType == "R").Count();
                report.CQty = _XYDatas.Where(x => x.LcrType == "C").Count();
                report.LQty = _XYDatas.Where(x => x.LcrType == "L").Count();
                report.DQty = _XYDatas.Where(x => x.LcrType == "D").Count();
                report.BQty = _XYDatas.Where(x => x.LcrType == "B").Count();
                report.OQty = _XYDatas.Where(x => x.LcrType == "O").Count();
                report.TQty = _XYDatas.Where(x => x.LcrType == "T").Count();
                report.IQty = _XYDatas.Where(x => x.LcrType == "I").Count();
                report.SmdQty = report.TotalQty - 2;
                report.NoSmdQty = report.TotalQty - report.SmdQty;
                //report.PositionImage = SQLDataControl.GetProgramOptionPicture(comProductCode.Text);
                SQLDataControl.AddMan_Report(report);
                IsAddReport = false;
                btnStart.Enabled = false;
                Add_ReportItem(_XYDatas);
            }
            else
            {
                //修改
                SQLDataControl.UpdateMan_Report
                    (
                    txtReportCode.Text,
                      txtBarcode.Text,
                     SQLDataControl.GetMeterOptionName(comOptionCode.Text),
                      txtLotNumber.Text,
                      comPLCode.Text,
                      checkType,
                      _class,
                      batchQty,
                      txtWoCode.Text,
                      txtPCBVersion.Text,
                      DBEventAction.User.UserName,
                      richRemarks.Text
                    );

            }

            comProductCode.Enabled = false;
        }

        /// <summary>
        /// 添加Man_ReportItem
        /// </summary>
        private void Add_ReportItem(List<Eng_XYData> _XYDatas)
        {
            _XYDatas = SQLDataControl.GetEng_XYDatas(comProductCode.Text);
            foreach (var item in _XYDatas)
            {
                List<Eng_Bom> eng_Boms = SQLDataControl.GetBomByProductCode(comProductCode.Text, item.MaterialCode);
                Man_ReportItem man_ReportItem = new Man_ReportItem();
                man_ReportItem.ReportCode = txtReportCode.Text; //报告编号
                man_ReportItem.Position = item.Position;     //元件位置
                man_ReportItem.BoardId = int.Parse(numtxtBoardQty.Text); //拼板数
                man_ReportItem.BomSequence = string.IsNullOrEmpty(item.MaterialCode) ? 0 : eng_Boms[0].Row;//bom序号
                man_ReportItem.MaterialCode = string.IsNullOrEmpty(item.MaterialCode) ? "" : eng_Boms[0].MaterialCode;//料号
                man_ReportItem.MaterialName = string.IsNullOrEmpty(item.MaterialCode) ? "" : eng_Boms[0].MaterialName;//物料描述
                man_ReportItem.LcrType = string.IsNullOrEmpty(item.MaterialCode) ? "" : eng_Boms[0].LcrType;     //LCR类型
                man_ReportItem.LcrStandardValue = string.IsNullOrEmpty(item.MaterialCode) ? 0 : eng_Boms[0].LcrStandardValue;//标准值
                man_ReportItem.LcrUnitCode = string.IsNullOrEmpty(item.MaterialCode) ? "" : eng_Boms[0].LcrUnitCode;//LCR单位
                man_ReportItem.LcrMaxValue = string.IsNullOrEmpty(item.MaterialCode) ? 0 : eng_Boms[0].LcrMaxValue;//最大值
                man_ReportItem.LcrMinValue = string.IsNullOrEmpty(item.MaterialCode) ? 0 : eng_Boms[0].LcrMinValue;//最小值
                man_ReportItem.MaxTolerance = string.IsNullOrEmpty(item.MaterialCode) ? 0 : eng_Boms[0].MaxTolerance;//上限公差
                man_ReportItem.MinTolerance = string.IsNullOrEmpty(item.MaterialCode) ? 0 : eng_Boms[0].MinTolerance;//下限公差
                man_ReportItem.Size = string.IsNullOrEmpty(item.MaterialCode) ? "" : eng_Boms[0].Size;//元件尺寸
                man_ReportItem.X = item.X;//X坐标
                man_ReportItem.Y = item.Y;//Y坐标
                man_ReportItem.Angle = item.Angle;//旋转角度
                man_ReportItem.IsSMD = man_ReportItem.Position.ToUpper().Contains("MARK") ? "NO" : "YES";//是否无SMD
                man_ReportItem.LX = item.LX;//拼图坐标
                man_ReportItem.LY = item.LY;//拼图坐标
                man_ReportItem.RX = item.RX;//XY位号图坐标
                man_ReportItem.RY = item.RY;//XY位号图坐标
                man_ReportItem.Status = "NEW";//状态
                man_ReportItem.IsDefined = "NO";//是否已定义
                man_ReportItem.OLcrStandardValue = man_ReportItem.LcrStandardValue;//OLCR标准值
                man_ReportItem.OLcrMaxValue = man_ReportItem.LcrMaxValue;//OLCR最大值
                man_ReportItem.OLcrMinValue = man_ReportItem.LcrMinValue;//OLCR最小值
                man_ReportItem.ToleranceType = string.IsNullOrEmpty(item.MaterialCode) ? "" : eng_Boms[0].ToleranceType;//公差类别
                man_ReportItem.OLcrUnitCode = man_ReportItem.LcrUnitCode;//OLCR单位
                man_ReportItem.XYGroups = 1;//XY分组数
                man_ReportItem.ErrorBoard = false;//错误拼板
                SQLDataControl.AddMan_ReportItem(man_ReportItem);
            }
            search(txtReportCode.Text);
        }

        private void btnEnd_Click(object sender, EventArgs e)
        {
            btnSave_Click(null, null);
            SQLDataControl.FinishMan_Report(txtReportCode.Text, DBEventAction.User.UserName);
            DBEventAction.RefreshReport?.Invoke(txtReportCode.Text);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            FormAction form3 = new FormAction(txtReportCode.Text, comProductCode.Text);
            form3.ShowDialog();
        }
    }
}
