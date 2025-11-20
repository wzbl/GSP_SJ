using BorwinAnalyse.UCControls;
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
    public partial class UCProgram : UserControl
    {
        private string ProductCode = "";
        UCBOM uCBOM;
        UCXYDataChart uCXYDataChart;
        public UCProgram()
        {
            InitializeComponent();
            uCBOM = new UCBOM();
            this.kryptonPage1.Controls.Add(uCBOM);
            Dock = DockStyle.Fill;
            this.Load += UCProgram_Load;
        }
        public UCProgram(View_Eng_Program view_Eng_Program) 
        {
            InitializeComponent();
            txtProductCode.Enabled = false;
            this.ProductCode = view_Eng_Program.产品编号;
            txtProductCode.Text = view_Eng_Program.产品编号;
            txtProductName.Text = view_Eng_Program.产品名称;
            txtCustom.Text = view_Eng_Program.客户;
            txtBoardSide.Text = view_Eng_Program.板面;
            uCBOM = new UCBOM(view_Eng_Program.产品编号);
            this.kryptonPage1.Controls.Add(uCBOM);
            Dock = DockStyle.Fill;
            this.Load += UCProgram_Load;
        }

        private void UCProgram_Load(object sender, EventArgs e)
        {
            uCXYDataChart=new UCXYDataChart();
            this.kryptonPage3.Controls.Add(uCXYDataChart);
            RefreshBomData();
            RefreshXYData();
        }

        private void RefreshBomData()
        {

        }

        private void RefreshXYData()
        {
            List<P_Search_Eng_XYData_Result> eng_XYData_Results = SQLDataControl.SearchXYData(ProductCode);
            if (eng_XYData_Results.Count > 0)
            {
                dgvXYData.DataSource = eng_XYData_Results;
                uCXYDataChart.canvas.ProductCode = ProductCode;
                uCXYDataChart.RefreshData(eng_XYData_Results);
            }
        }

        private void btnExportXYData_Click(object sender, EventArgs e)
        {
            uCBOM.ProductCode = txtProductCode.Text;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dgvXYData.Rows.Count; i++)
            {
                SQLDataControl.InsertXYData(
                    txtProductCode.Text,                                                                       //产品编码
                    dgvXYData.Rows[i].Cells[1].Value.ToString(),                                              //position
                   int.Parse(dgvXYData.Rows[i].Cells[2].Value.ToString()),                //boardId
                      dgvXYData.Rows[i].Cells[3].Value.ToString(),  //boardSide
                       dgvXYData.Rows[i].Cells[4].Value.ToString(),//isSMD
                       dgvXYData.Rows[i].Cells[5].Value.ToString(),//materialCode
                        dgvXYData.Rows[i].Cells[6].Value.ToString(),//materialName
                       decimal.Parse(dgvXYData.Rows[i].Cells[7].Value.ToString()),// x
                        decimal.Parse(dgvXYData.Rows[i].Cells[8].Value.ToString()),//y
                           dgvXYData.Rows[i].Cells[9].Value.ToString(),       //unit
                            decimal.Parse(dgvXYData.Rows[i].Cells[10].Value.ToString()),       //angle
                           decimal.Parse(dgvXYData.Rows[i].Cells[11].Value.ToString()),
                             decimal.Parse(dgvXYData.Rows[i].Cells[12].Value.ToString()),
                           dgvXYData.Rows[i].Cells[13].Value.ToString(),            //remarks
                               dgvXYData.Rows[i].Cells[14].Value.ToString(),
                               dgvXYData.Rows[i].Cells[16].Value.ToString(),
                             "",
                               0,   //standardRotation
                               dgvXYData.Rows[i].Cells[18].Value.ToString(),
                               dgvXYData.Rows[i].Cells[19].Value.ToString(),
                               dgvXYData.Rows[i].Cells[20].Value.ToString(),
                              decimal.Parse(dgvXYData.Rows[i].Cells[21].Value.ToString()),//ox
                              decimal.Parse(dgvXYData.Rows[i].Cells[22].Value.ToString()),//oy
                             decimal.Parse(dgvXYData.Rows[i].Cells[23].Value.ToString()),//oangle
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
            SQLDataControl.AddProgram(txtProductCode.Text,
                txtProductName.Text, txtCustom.Text, txtBoardSide.Text, "test");

            uCBOM.ProductCode = txtProductCode.Text;
        }

    }
}
