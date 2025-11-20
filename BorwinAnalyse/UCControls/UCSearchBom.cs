using BorwinAnalyse.BaseClass;
using BorwinAnalyse.Model;
using BrowApp.Language;
using NPOI.OpenXmlFormats.Dml;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BorwinAnalyse.UCControls
{
    public partial class UCSearchBom : UserControl
    {
        string seletedType = string.Empty;
        public UCSearchBom()
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
            this.Load += UCSearchBom_Load;
            this.components = new System.ComponentModel.Container();
        }

        private void UCSearchBom_Load(object sender, EventArgs e)
        {
            ComModelUpdata();
            UpdataLanguage();

            MicroKeyManager.Instance.AddEventToControl(this);
        }
        public void UpdataLanguage()
        {
           
        }
        /// <summary>
        /// 更新模板下拉框
        /// </summary>
        public void ComModelUpdata()
        {
            comModelName.Items.Clear();
            comModelName.Items.Add("ALL");
            for (int i = 0; i < BomManager.Instance.BomNames.Count; i++)
            {
                comModelName.Items.Add(BomManager.Instance.BomNames[i]);
                if (BomManager.Instance.CurrentBomName == BomManager.Instance.BomNames[i])
                {
                    comModelName.SelectedIndex = i;
                    comModelName.Text = BomManager.Instance.CurrentBomName;
                }
            }
        }


        private void btnSearchBom_Click(object sender, EventArgs e)
        {
            //btnSearchBom.Enabled = false;
            SearchByFilter();
        }


        public async void Search()
        {
            List<BomDataModel> bomDataModels;
            if (comModelName.Text == "ALL")
            {
                bomDataModels = BomManager.Instance.AllBomData;
            }
            else
            {
                bomDataModels = BomManager.Instance.AllBomData.Where(x => x.modelName == comModelName.Text).ToList<BomDataModel>();
            }
            gridBomData.Rows.Clear();
            if (bomDataModels != null)
            {
                await Task.Run(() =>
                {
                    foreach (var item in bomDataModels)
                    {
                        this.Invoke(new Action(() =>
                        {
                            gridBomData.Rows.Add(
                            gridBomData.RowCount + "_" + item.id,
                                item.modelName,
                                item.barCode,
                                item.replaceCode,
                                item.description,
                                item.result,
                                item.type,
                                item.size,
                                item.value,
                                item.unit,
                                item.grade,
                                item.tapeType,
                                item.tapeWidth,
                                item.pitch,
                                item.judgeOCV,
                                item.exp5
                          );
                           
                            if (gridBomData.RowCount % 5000 == 0)
                            {
                                gridBomData.Refresh();
                            }
                        }));
                    }
                });
                gridBomData.Refresh();
            }
            btnSearchBom.Enabled = true;
        }

        public void SearchByFilter()
        {
            List<BomDataModel> bomDataModels;
            //bomDataModels = BomManager.Instance.AllBomData.Where(x => x.barCode == txtbarCode.Text).ToList<BomDataModel>();
            bomDataModels = BomManager.Instance.AllBomData.Where(x => (x.barCode == txtbarCode.Text && txtbarCode.Text != String.Empty) &&
            //(x.type == comboType.Text && comboType.Text != String.Empty) &&
            //(x.type == seletedType && seletedType != String.Empty) &&
            (comboUnit.Text == String.Empty? true : x.unit == comboUnit.Text) &&
            (txtGrade.Text == String.Empty? true: x.grade == txtGrade.Text) &&
            (comboSize.Text == String.Empty? true :x.size == comboSize.Text) &&
            (txtValue.Text == String.Empty? true: x.value == txtValue.Text) &&
            (txtDescription.Text == String.Empty? true: x.description == txtDescription.Text) &&
            (comboResult.Text == String.Empty? true: x.result == comboResult.Text)
            ).ToList<BomDataModel>();
            gridBomData.Rows.Clear();

            if (bomDataModels != null)
            {
                foreach (var item in bomDataModels)
                {
                    gridBomData.Rows.Add(
                        item.id,
                        item.modelName,
                        item.barCode,
                        item.replaceCode,
                        item.description,
                        item.result,
                        item.type,
                        item.size,
                        item.value,
                        item.unit,
                        item.grade,
                        item.tapeType,
                        item.tapeWidth,
                        item.pitch,
                        item.judgeOCV,
                        item.exp5
                  );
                }
                gridBomData.Refresh();
            }
        }

        public void SaveSelectedData()
        {
            if (gridBomData.CurrentCell == null)
                return;
            int row = gridBomData.CurrentCell.RowIndex;
            if (row < 0) {
                return;
            }

            string code = gridBomData.Rows[row].Cells[2].FormattedValue.ToString();

            List<BomDataModel> bomDataModels;
            bomDataModels = BomManager.Instance.AllBomData.Where(x => (x.barCode == code && code != String.Empty)).ToList<BomDataModel>();

            if( bomDataModels != null && bomDataModels.Count > 0)
            {
                BomDataModel bomDataModel = bomDataModels[0];
                bomDataModel.id = gridBomData.Rows[row].Cells[0].FormattedValue.ToString();
                bomDataModel.modelName = gridBomData.Rows[row].Cells[1].FormattedValue.ToString();
                bomDataModel.barCode = gridBomData.Rows[row].Cells[2].FormattedValue.ToString();
                bomDataModel.replaceCode = gridBomData.Rows[row].Cells[3].FormattedValue.ToString();
                bomDataModel.description = gridBomData.Rows[row].Cells[4].FormattedValue.ToString();
                bomDataModel.result = gridBomData.Rows[row].Cells[5].FormattedValue.ToString();
                bomDataModel.type = gridBomData.Rows[row].Cells[6].FormattedValue.ToString();
                bomDataModel.size = gridBomData.Rows[row].Cells[7].FormattedValue.ToString();
                bomDataModel.value = gridBomData.Rows[row].Cells[8].FormattedValue.ToString();
                bomDataModel.unit = gridBomData.Rows[row].Cells[9].FormattedValue.ToString();
                bomDataModel.grade = gridBomData.Rows[row].Cells[10].FormattedValue.ToString();
                bomDataModel.tapeType = gridBomData.Rows[row].Cells[11].FormattedValue.ToString();
                bomDataModel.tapeWidth = gridBomData.Rows[row].Cells[12].FormattedValue.ToString();
                bomDataModel.pitch = gridBomData.Rows[row].Cells[13].FormattedValue.ToString();
                bomDataModel.judgeOCV = gridBomData.Rows[row].Cells[14].FormattedValue.ToString();
                bomDataModel.exp5 = gridBomData.Rows[row].Cells[15].FormattedValue.ToString();
                string cmd = string.Format("UPDATE ALLBOM SET modelName = '{1}' ,barCode = '{2}',replaceCode = '{3}',description = '{4}',result = '{5}',type = '{6}',size = '{7}',value = '{8}',unit = '{9}',grade = '{10}',tapeType = '{11}',tapeWidth = '{12}',pitch = '{13}',judgeOCV = '{14}',exp5 = '{15}' where  id = '{0}'", 
                    bomDataModel.id, bomDataModel.modelName, bomDataModel.barCode, bomDataModel.replaceCode, bomDataModel.description, bomDataModel.result, bomDataModel.type, bomDataModel.size, bomDataModel.value, bomDataModel.unit, bomDataModel.grade, bomDataModel.tapeType, bomDataModel.tapeWidth, bomDataModel.pitch, bomDataModel.judgeOCV, bomDataModel.exp5);
                SqlLiteManager.Instance.BomDB.Insert(cmd);
                MessageBox.Show("保存成功".tr());
            }
            else
            {
                if(String.IsNullOrEmpty(gridBomData.Rows[row].Cells[1].FormattedValue.ToString()) && String.IsNullOrEmpty(gridBomData.Rows[row].Cells[2].FormattedValue.ToString()) &&
                    //String.IsNullOrEmpty(gridBomData.Rows[row].Cells[3].FormattedValue.ToString()) && String.IsNullOrEmpty(gridBomData.Rows[row].Cells[4].FormattedValue.ToString()) &&
                    String.IsNullOrEmpty(gridBomData.Rows[row].Cells[4].FormattedValue.ToString()) &&
                    String.IsNullOrEmpty(gridBomData.Rows[row].Cells[5].FormattedValue.ToString()) && String.IsNullOrEmpty(gridBomData.Rows[row].Cells[6].FormattedValue.ToString()) &&
                    String.IsNullOrEmpty(gridBomData.Rows[row].Cells[7].FormattedValue.ToString()) && String.IsNullOrEmpty(gridBomData.Rows[row].Cells[8].FormattedValue.ToString()) &&
                    String.IsNullOrEmpty(gridBomData.Rows[row].Cells[9].FormattedValue.ToString()) && String.IsNullOrEmpty(gridBomData.Rows[row].Cells[10].FormattedValue.ToString()))
                {
                    string id = Guid.NewGuid().ToString();
                    BomDataModel bomDataModel = new BomDataModel();
                    bomDataModel.id = id;
                    bomDataModel.modelName = gridBomData.Rows[row].Cells[1].FormattedValue.ToString();
                    bomDataModel.barCode = gridBomData.Rows[row].Cells[2].FormattedValue.ToString();
                    bomDataModel.replaceCode = gridBomData.Rows[row].Cells[3].FormattedValue.ToString();
                    bomDataModel.description = gridBomData.Rows[row].Cells[4].FormattedValue.ToString();
                    bomDataModel.result = gridBomData.Rows[row].Cells[5].FormattedValue.ToString();
                    bomDataModel.type = gridBomData.Rows[row].Cells[6].FormattedValue.ToString();
                    bomDataModel.size = gridBomData.Rows[row].Cells[7].FormattedValue.ToString();
                    bomDataModel.value = gridBomData.Rows[row].Cells[8].FormattedValue.ToString();
                    bomDataModel.unit = gridBomData.Rows[row].Cells[9].FormattedValue.ToString();
                    bomDataModel.grade = gridBomData.Rows[row].Cells[10].FormattedValue.ToString();
                    bomDataModel.tapeType = gridBomData.Rows[row].Cells[11].FormattedValue.ToString();
                    bomDataModel.tapeWidth = gridBomData.Rows[row].Cells[12].FormattedValue.ToString();
                    bomDataModel.pitch = gridBomData.Rows[row].Cells[13].FormattedValue.ToString();
                    bomDataModel.judgeOCV = gridBomData.Rows[row].Cells[14].FormattedValue.ToString();
                    bomDataModel.exp5 = gridBomData.Rows[row].Cells[15].FormattedValue.ToString();
                    string cmd = string.Format("insert into ALLBOM values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}')", bomDataModel.id, bomDataModel.modelName, bomDataModel.barCode, bomDataModel.replaceCode, bomDataModel.description, bomDataModel.result, bomDataModel.type, bomDataModel.size, bomDataModel.value, bomDataModel.unit, bomDataModel.grade, bomDataModel.tapeType, bomDataModel.tapeWidth, bomDataModel.pitch, bomDataModel.judgeOCV, bomDataModel.exp5);
                    SqlLiteManager.Instance.BomDB.Insert(cmd);
                    MessageBox.Show("保存成功".tr());
                }
            }

            //List<BomDataModel> bomDataModels2;
            //bomDataModels2 = BomManager.Instance.CurrentBomData.Where(x => (x.barCode == code && code != String.Empty)).ToList<BomDataModel>();

            //if (bomDataModels2 != null && bomDataModels2.Count > 0)
            //{
            //    BomDataModel bomDataModel = bomDataModels2[0];
            //    bomDataModel.id = gridBomData.Rows[row].Cells[0].FormattedValue.ToString();
            //    bomDataModel.modelName = gridBomData.Rows[row].Cells[1].FormattedValue.ToString();
            //    bomDataModel.barCode = gridBomData.Rows[row].Cells[2].FormattedValue.ToString();
            //    bomDataModel.replaceCode = gridBomData.Rows[row].Cells[3].FormattedValue.ToString();
            //    bomDataModel.description = gridBomData.Rows[row].Cells[4].FormattedValue.ToString();
            //    bomDataModel.result = gridBomData.Rows[row].Cells[5].FormattedValue.ToString();
            //    bomDataModel.type = gridBomData.Rows[row].Cells[6].FormattedValue.ToString();
            //    bomDataModel.size = gridBomData.Rows[row].Cells[7].FormattedValue.ToString();
            //    bomDataModel.value = gridBomData.Rows[row].Cells[8].FormattedValue.ToString();
            //    bomDataModel.unit = gridBomData.Rows[row].Cells[9].FormattedValue.ToString();
            //    bomDataModel.grade = gridBomData.Rows[row].Cells[10].FormattedValue.ToString();
            //    bomDataModel.tapeType = gridBomData.Rows[row].Cells[11].FormattedValue.ToString();
            //    bomDataModel.tapeWidth = gridBomData.Rows[row].Cells[12].FormattedValue.ToString();
            //    bomDataModel.pitch = gridBomData.Rows[row].Cells[13].FormattedValue.ToString();
            //    bomDataModel.judgeOCV = gridBomData.Rows[row].Cells[14].FormattedValue.ToString();
            //    bomDataModel.exp5 = gridBomData.Rows[row].Cells[15].FormattedValue.ToString();
            //    string cmd = string.Format("UPDATE CurrentBOM SET modelName = '{1}' ,barCode = '{2}',replaceCode = '{3}',description = '{4}',result = '{5}',type = '{6}',size = '{7}',value = '{8}',unit = '{9}',grade = '{10}',tapeType = '{11}',tapeWidth = '{12}',pitch = '{13}',judgeOCV = '{14}',exp5 = '{15}' where  id = '{0}'",
            //        bomDataModel.id, bomDataModel.modelName, bomDataModel.barCode, bomDataModel.replaceCode, bomDataModel.description, bomDataModel.result, bomDataModel.type, bomDataModel.size, bomDataModel.value, bomDataModel.unit, bomDataModel.grade, bomDataModel.tapeType, bomDataModel.tapeWidth, bomDataModel.pitch, bomDataModel.judgeOCV, bomDataModel.exp5);
            //    SqlLiteManager.Instance.DB.Insert(cmd);
            //}

            BomManager.Instance.GetAllBomData();
            SearchByFilter();
        }

        public void DeleteData()
        {
            int row = gridBomData.CurrentCell.RowIndex;
            if (row < 0)
            {
                return;
            }

            string code = gridBomData.Rows[row].Cells[2].FormattedValue.ToString();

            List<BomDataModel> bomDataModels;
            bomDataModels = BomManager.Instance.AllBomData.Where(x => (x.barCode == code && code != String.Empty)).ToList<BomDataModel>();

            if( bomDataModels.Count > 0)
            {
                string comms1 = string.Format("DELETE FROM CurrentBOM where barCode = '{0}' ", code);
                SqlLiteManager.Instance.BomDB.Insert(comms1);

                string comms2 = string.Format("DELETE FROM ALLBOM where barCode = '{0}' ", code);
                SqlLiteManager.Instance.BomDB.Insert(comms2);
            }

            BomManager.Instance.GetAllBomData();
            SearchByFilter();
        }

        private void btnUpdataBom_Click(object sender, EventArgs e)
        {
            if (gridBomData.SelectedRows.Count > 0)
            {
                for (int i = 0; i < gridBomData.SelectedRows.Count; i++)
                {
                    DataGridViewCellCollection row = gridBomData.SelectedRows[i].Cells;
                    UpdataBomData(row);
                }
                BomManager.Instance.Init();
                Search();
            }

        }

        private void UpdataBomData(DataGridViewCellCollection row)
        {
            if (string.IsNullOrEmpty(row[0].FormattedValue.ToString()))
            {
                return;
            }
            BomDataModel bomDataModel = new BomDataModel();
            bomDataModel.id = row[0].FormattedValue.ToString();
            bomDataModel.modelName = row[1].FormattedValue.ToString();
            bomDataModel.barCode = row[2].FormattedValue.ToString();
            bomDataModel.replaceCode = row[3].FormattedValue.ToString();
            bomDataModel.description = row[4].FormattedValue.ToString();
            bomDataModel.result = row[5].FormattedValue.ToString();
            bomDataModel.type = row[6].FormattedValue.ToString();
            bomDataModel.size = row[7].FormattedValue.ToString();
            bomDataModel.value = row[8].FormattedValue.ToString();
            bomDataModel.unit = row[9].FormattedValue.ToString();
            bomDataModel.grade = row[10].FormattedValue.ToString();
            bomDataModel.tapeType = row[11].FormattedValue.ToString();
            bomDataModel.tapeWidth = row[12].FormattedValue.ToString();
            bomDataModel.pitch = row[13].FormattedValue.ToString();
            bomDataModel.judgeOCV = row[14].FormattedValue.ToString();
            bomDataModel.exp5 = row[15].FormattedValue.ToString();
            BomManager.Instance.UpdataBomData(bomDataModel);
        }

        private void btnDeleteModel_Click(object sender, EventArgs e)
        {
            MessageBox.Show("是否从本地BOM数据库中，删除数据：".tr() + "模板名称为".tr() + comModelName.Text);
            BomManager.Instance.DeleteModel(comModelName.Text);
            ComModelUpdata();
            MessageBox.Show("删除成功".tr());
        }

        private void btnSetModel_Click(object sender, EventArgs e)
        {
            if (BomManager.Instance.BomNames.Contains(comModelName.Text))
            {
                BomManager.Instance.SetCurrentModel(comModelName.Text);
                MessageBox.Show("设为模板成功".tr());
            }

        }

        private void btnAddBom_Click(object sender, EventArgs e)
        {
            if (!BomManager.Instance.BomNames.Contains(comModelName.Text))
            {
                MessageBox.Show("未选择模板".tr());
                return;
            }
            if (string.IsNullOrEmpty(txtbarCode.Text))
            {
                MessageBox.Show("未输入条码".tr());
                return;
            }
            if (BomManager.Instance.AllBomData.Where(x => x.barCode == txtbarCode.Text).ToList<BomDataModel>().ToList().Count > 0)
            {
                MessageBox.Show("当前条码已存在".tr());
                return;
            }

            BomDataModel bomDataModel = new BomDataModel();
            bomDataModel.id = Guid.NewGuid().ToString();
            bomDataModel.modelName = comModelName.Text;
            bomDataModel.barCode = txtbarCode.Text;
            bomDataModel.description = txtDescription.Text;
            bomDataModel.type = comboType.Text;
            bomDataModel.result = comboResult.Text;
            bomDataModel.size = comboSize.Text;
            bomDataModel.value = txtValue.Text;
            bomDataModel.unit = comboUnit.Text;
            bomDataModel.grade = txtGrade.Text;
            bomDataModel.tapeWidth = comboWidth.Text;
            bomDataModel.pitch = comboPitch.Text;
            bomDataModel.exp5 = "0";//string.Format("{0}-{1}-{2}-{3}-{4}", bomDataModel.type, bomDataModel.size, bomDataModel.value, bomDataModel.unit, bomDataModel.grade);
            BomManager.Instance.InserIntoBomData(bomDataModel);
            Search();
        }

        /// <summary>
        /// 双击时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridBomData_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //int index = e.RowIndex;
            //if (index < 0) return;
            //txtbarCode.Text = gridBomData.Rows[index].Cells[2].FormattedValue.ToString();
            //txtDescription.Text = gridBomData.Rows[index].Cells[4].FormattedValue.ToString();
            //comboResult.Text = gridBomData.Rows[index].Cells[5].FormattedValue.ToString();
            //comboType.Text = gridBomData.Rows[index].Cells[6].FormattedValue.ToString();
            //comboSize.Text = gridBomData.Rows[index].Cells[7].FormattedValue.ToString();
            //txtValue.Text = gridBomData.Rows[index].Cells[8].FormattedValue.ToString();
            //comboUnit.Text = gridBomData.Rows[index].Cells[9].FormattedValue.ToString();
            //txtGrade.Text = gridBomData.Rows[index].Cells[10].FormattedValue.ToString();
            //comboWidth.Text = gridBomData.Rows[index].Cells[12].FormattedValue.ToString();
            //comboPitch.Text = gridBomData.Rows[index].Cells[13].FormattedValue.ToString();
        }

        private void btnSearchByCode_Click(object sender, EventArgs e)
        {
            List<BomDataModel> bomDataModels;
            //bomDataModels = BomManager.Instance.AllBomData.Where(x => x.barCode == txtbarCode.Text).ToList<BomDataModel>();
            bomDataModels = BomManager.Instance.AllBomData.Where(x => (x.barCode == txtbarCode.Text && txtbarCode.Text!=String.Empty) 
                //(x.type == comboType.Text && comboType.Text != String.Empty) &&
                //(x.type == seletedType && seletedType != String.Empty) &&
                //(x.unit == comboUnit.Text && comboUnit.Text != String.Empty) &&
                //(x.grade == txtGrade.Text && txtGrade.Text != String.Empty) &&
                //(x.size == comboSize.Text && comboSize.Text != String.Empty) &&
                //(x.value == txtValue.Text && txtValue.Text != String.Empty) &&
                //(x.description == txtDescription.Text && txtDescription.Text != String.Empty) &&
                //(x.result == comboResult.Text && comboResult.Text != String.Empty)
            ).ToList<BomDataModel>();
            gridBomData.Rows.Clear();

            if (bomDataModels != null)
            {
                foreach (var item in bomDataModels)
                {
                    gridBomData.Rows.Add(
                        item.id,
                        item.modelName,
                        item.barCode,
                        item.replaceCode,
                        item.description,
                        item.result,
                        item.type,
                        item.size,
                        item.value,
                        item.unit,
                        item.grade,
                        item.tapeType,
                        item.tapeWidth,
                        item.pitch,
                        item.judgeOCV,
                        item.exp5
                  );
                }
                gridBomData.Refresh();
            }
        }

        private void UCSearchBom_Load_1(object sender, EventArgs e)
        {
            comboType.Items.Clear();
            comboType.BeginUpdate();
            comboType.Items.Add("电阻".tr());
            comboType.Items.Add("电容".tr());
            comboType.EndUpdate();
            comboType.SelectedIndex = -1;

            comboUnit.BeginUpdate();
            comboUnit.Items.Add("mΩ");
            comboUnit.Items.Add("Ω");
            comboUnit.Items.Add("kΩ");
            comboUnit.Items.Add("MΩ");
            comboUnit.Items.Add("PF");
            comboUnit.Items.Add("NF");
            comboUnit.Items.Add("UF");
            comboUnit.Items.Add("F");
            comboUnit.EndUpdate();
            comboUnit.SelectedIndex = -1;

            comboSize.BeginUpdate();
            comboSize.Items.Add("01005".tr());
            comboSize.Items.Add("0201".tr());
            comboSize.Items.Add("0402".tr());
            comboSize.Items.Add("0603".tr());
            comboSize.Items.Add("0805".tr());
            comboSize.Items.Add("1206".tr());
            comboSize.Items.Add("1210".tr());
            comboSize.EndUpdate();
            comboSize.SelectedIndex = -1;

            comboResult.BeginUpdate();
            comboResult.Items.Add("True".tr());
            comboResult.Items.Add("False".tr());
            comboResult.EndUpdate();
            comboResult.SelectedIndex = -1;

            comboWidth.BeginUpdate();
            comboWidth.Items.Add("8".tr());
            comboWidth.Items.Add("12".tr());
            comboWidth.Items.Add("16".tr());
            comboWidth.Items.Add("24".tr());
            comboWidth.EndUpdate();
            comboWidth.SelectedIndex = -1;

            comboPitch.BeginUpdate();
            comboPitch.Items.Add("2".tr());
            comboPitch.Items.Add("4".tr());
            comboPitch.Items.Add("8".tr());
            comboPitch.Items.Add("12".tr());
            comboPitch.Items.Add("16".tr());
            comboPitch.Items.Add("20".tr());
            comboPitch.EndUpdate();
            comboPitch.SelectedIndex = -1;
        }

        private void comboType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboType.SelectedIndex == 0) seletedType = "电阻";
            if (comboType.SelectedIndex == 1) seletedType = "电容";
        }

        private void btnBomAllData_Click(object sender, EventArgs e)
        {
            //btnBomAllData.Enabled = false;
            Search();
        }

        private void gridBomData_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveSelectedData();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteData();
        }

        private void btnExportFile_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(comModelName.Text))
            {
                MessageBox.Show("请先选择模板".tr());
                return;
            }

            DataTable dt = BomManager.Instance.GetAllBomDataTable(comModelName.Text);

            if (dt != null)
            {
                NOPIHelperEX.ExportDataToExcel(dt);
            }
            else
            {
                MessageBox.Show("此模板数据为空".tr());
                return;
            }

            dt.Dispose();
        }
    }
}
