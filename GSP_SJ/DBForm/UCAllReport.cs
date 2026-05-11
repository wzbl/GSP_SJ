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
    public partial class UCAllReport : UserControl
    {
        public UCAllReport()
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
            this.Load += UCAllReport_Load;
        }

        /// <summary>
        ///  避免窗体控件改变大小界面瞬间的叠影
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }
        private void UCAllReport_Load(object sender, EventArgs e)
        {
            dgvProgram.CellClick += dgvProgram_CellClick;
            dgvProgram.CellDoubleClick += dgvProgram_CellDoubleClick;
            DBEventAction.RefreshReport += RefreshReport;
            startTime.Value = DateTime.Now.AddMonths(-1);
            UpdateLanguage();
        }
        private void UpdateLanguage()
        {
            BrowApp.Language.Language.Instance.UpdateLanguage(this, null);
        }
        private void RefreshReport(string ReportCode)
        {
            Search();

            List<P_Man_Report_Search_Result> p_Man_Report_s= Man_Reports.Where(x => x.ReportCode == ReportCode).ToList();
            if (p_Man_Report_s.Count>0)
            {
                DBEventAction.ReportItem?.Invoke(p_Man_Report_s[0]);
            }
        }

        List<P_Man_Report_Search_Result> Man_Reports = new List<P_Man_Report_Search_Result>();
        private void dgvProgram_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //打开报告
            if (e.RowIndex >= 0)
            {
                try
                {
                    DBEventAction.ReportItem?.Invoke(Man_Reports[e.RowIndex]);
                }
                catch (Exception ex)
                {

                }

            }
        }

        private void dgvProgram_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                dgvProgram.Rows[e.RowIndex].Selected = true;
        }

        private void Search()
        {
            string line = null;
            string result = null;
            string status = null;
            if (!string.IsNullOrEmpty(comLine.Text))
            {
                line = comLine.Text;
            }

            if (!string.IsNullOrEmpty(comResult.Text))
            {
                result = comResult.Text;
            }

            if (!string.IsNullOrEmpty(comStatus.Text))
            {
                status = comStatus.Text;
                switch (comStatus.Text)
                {
                    case "检测中":
                        status = "NEW";
                        break;
                    case "已结束":
                        status = "CLOSE";
                        break;
                    case "不可测":
                        status = "UNABLE";
                        break;
                }
            }

            dgvProgram.Rows.Clear();
            try
            {
                Man_Reports = SQLDataControl.SearchMan_Report(startTime.Value, endTime.Value, line, result, status);
                for (int i = 0; i < Man_Reports.Count; i++)
                {
                    switch (Man_Reports[i].Status)
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
                    string _class = Man_Reports[i].@class;
                    switch (Man_Reports[i].@class)
                    {
                        case "10":
                            _class = "白班";
                            break;
                        case "20":
                            _class = "夜班";
                            break;
                    }
                    string checkType = Man_Reports[i].CheckType;
                    switch (Man_Reports[i].CheckType)
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

                    dgvProgram.Rows.Add
                        (
                        Man_Reports[i].ReportCode,  //报告编号0
                        Man_Reports[i].BoardSide,   //面1
                        Man_Reports[i].ProductCode,  //产品编号2
                        Man_Reports[i].ProductName,  //产品名称3
                        SQLDataControl.GetMeterOptionCode(Man_Reports[i].OptionCode),                          //电桥参数4
                        Man_Reports[i].Barcode,                         //样品条码5
                        Man_Reports[i].PLCode,     //线体编号6
                        Man_Reports[i].WoCode,    //工单编号7
                        Man_Reports[i].LotNumber, //批次编号8
                        _class,                      //班别9
                        Man_Reports[i].BomVersion,//Bom版10
                        Man_Reports[i].PCBVersion,//PCB版11
                        checkType,//检测类型12
                        Man_Reports[i].BatchQty,//批次数量13
                        status,//状态14
                        Man_Reports[i].CheckResult,//检测结果15
                        Man_Reports[i].IsQACheck,//QA检测16
                        Man_Reports[i].TotalQty,//元件总数量
                        Man_Reports[i].AutomationQty,//自动判断数量
                        Man_Reports[i].ManualQty,//手动判断数量
                        Man_Reports[i].PassQty,//通过数量
                        Man_Reports[i].FailQty,//失败数量
                        Man_Reports[i].MissQty,//漏检数量
                        Man_Reports[i].IsForceFinish,//强制结束
                        Man_Reports[i].FinishedBy,//结束人
                        Man_Reports[i].FinishedDate,//结束日期
                        Man_Reports[i].DeviceName,//设备名称
                        Man_Reports[i].Remarks,
                        Man_Reports[i].Creator,
                        Man_Reports[i].CreationDate
                        );

                    switch (Man_Reports[i].Status)
                    {
                        case "NEW":
                            dgvProgram.Rows[i].Cells[14].Style.ForeColor = Color.Blue;
                            break;
                        case "CLOSE":
                            status = "已结束";
                            dgvProgram.Rows[i].Cells[14].Style.ForeColor = Color.Black;
                            break;
                        case "UNABLE":
                            status = "不可测";
                            dgvProgram.Rows[i].Cells[14].Style.ForeColor = Color.Red;
                            break;
                    }
                    if (Man_Reports[i].CheckResult == "PASS")
                    {
                        dgvProgram.Rows[i].Cells[15].Style.ForeColor = Color.Green;
                    }
                    else if (Man_Reports[i].CheckResult == "FAIL")
                    {
                        dgvProgram.Rows[i].Cells[15].Style.ForeColor = Color.Red;
                    }

                    if (Man_Reports[i].IsQACheck == "YES")
                    {
                        dgvProgram.Rows[i].Cells[16].Style.ForeColor = Color.Green;
                    }
                    else
                    {
                        dgvProgram.Rows[i].Cells[16].Style.ForeColor = Color.Black;
                    }
                }
            }
            catch (Exception)
            {

            }

        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (dgvProgram.SelectedRows.Count > 0)
            {
                int row = dgvProgram.SelectedRows[0].Index;
                string reportCode = dgvProgram.Rows[row].Cells[0].Value.ToString();
                SQLDataControl.DeleteMan_Report(reportCode);
                dgvProgram.Rows.RemoveAt(row);
            }

        }

        private void btnClearSearch_Click(object sender, EventArgs e)
        {
            comLine.SelectedIndex = -1;
            comResult.SelectedIndex = -1;
            comStatus.SelectedIndex = -1;
            Search();
        }

        private void comLine_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comLine.Text != null)
                Search();
        }

        private void comResult_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comResult.Text != null)
                Search();
        }

        private void comStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comStatus.Text != null)
                Search();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            Search();
        }
    }
}
