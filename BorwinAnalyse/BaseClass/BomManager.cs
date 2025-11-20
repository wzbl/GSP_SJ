
using BorwinAnalyse.Model;
using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibSDK;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace BorwinAnalyse.BaseClass
{
    /// <summary>
    /// bom管理类
    /// </summary>
    public class BomManager
    {
        private static BomManager instance;
        public static BomManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BomManager();
                }
                return instance;
            }
        }

        public string CurrentBomName = "";
        public List<string> BomNames = new List<string>();

        public void Init()
        {
            GetAllBomData();
            GetCurrentBomData();
        }

        /// <summary>
        /// 获取所有Bom数据
        /// </summary>
        /// <returns></returns>
        public void GetAllBomData()
        {
            string comm = "select *  from ALLBOM";
            DataTable dataTable = SqlLiteManager.Instance.BomDB.Search(comm, "ALLBOM");
            AllBomData.Clear();
            BomNames.Clear();
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                BomDataModel bomDataModel = new BomDataModel();
                bomDataModel.id = dataTable.Rows[i].ItemArray[0].ToString();
                bomDataModel.modelName = dataTable.Rows[i].ItemArray[1].ToString();
                bomDataModel.barCode = dataTable.Rows[i].ItemArray[2].ToString();
                bomDataModel.replaceCode = dataTable.Rows[i].ItemArray[3].ToString();
                bomDataModel.description = dataTable.Rows[i].ItemArray[4].ToString();
                bomDataModel.result = dataTable.Rows[i].ItemArray[5].ToString();
                bomDataModel.type = dataTable.Rows[i].ItemArray[6].ToString();
                bomDataModel.size = dataTable.Rows[i].ItemArray[7].ToString();
                bomDataModel.value = dataTable.Rows[i].ItemArray[8].ToString();
                bomDataModel.unit = dataTable.Rows[i].ItemArray[9].ToString();
                bomDataModel.grade = dataTable.Rows[i].ItemArray[10].ToString();
                bomDataModel.tapeType = dataTable.Rows[i].ItemArray[11].ToString();
                bomDataModel.tapeWidth = dataTable.Rows[i].ItemArray[12].ToString();
                bomDataModel.pitch = dataTable.Rows[i].ItemArray[13].ToString();
                bomDataModel.judgeOCV = dataTable.Rows[i].ItemArray[14].ToString();
                bomDataModel.exp5 = dataTable.Rows[i].ItemArray[15].ToString();
                if (!BomNames.Contains(bomDataModel.modelName))
                {
                    BomNames.Add(bomDataModel.modelName);
                }
                AllBomData.Add(bomDataModel);
            }
        }

        /// <summary>
        /// 获取所有Bom数据表
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllBomDataTable(string modelName)
        {
            string comm;
            if (modelName == "ALL")
            {
                comm = "select *  from ALLBOM";
            }
            else
            {
                comm = string.Format("select *  from ALLBOM where modelName = '{0}'", modelName);
            }
            DataTable dataTable = SqlLiteManager.Instance.BomDB.Search(comm, "ALLBOM");

            return dataTable;
        }

        /// <summary>
        /// 获取当前的Bom数据
        /// </summary>
        /// <returns></returns>
        private void GetCurrentBomData()
        {
            string comm = "select *  from CurrentBOM";
            DataTable dataTable = SqlLiteManager.Instance.BomDB.Search(comm, "CurrentBOM");
            CurrentBomData.Clear();
            CurrentBomName = "";
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                BomDataModel bomDataModel = new BomDataModel();
                bomDataModel.id = dataTable.Rows[i].ItemArray[0].ToString();
                bomDataModel.modelName = dataTable.Rows[i].ItemArray[1].ToString();
                bomDataModel.barCode = dataTable.Rows[i].ItemArray[2].ToString();
                bomDataModel.replaceCode = dataTable.Rows[i].ItemArray[3].ToString();
                bomDataModel.description = dataTable.Rows[i].ItemArray[4].ToString();
                bomDataModel.result = dataTable.Rows[i].ItemArray[5].ToString();
                bomDataModel.type = dataTable.Rows[i].ItemArray[6].ToString();
                bomDataModel.size = dataTable.Rows[i].ItemArray[7].ToString();
                bomDataModel.value = dataTable.Rows[i].ItemArray[8].ToString();
                bomDataModel.unit = dataTable.Rows[i].ItemArray[9].ToString();
                bomDataModel.grade = dataTable.Rows[i].ItemArray[10].ToString();
                bomDataModel.tapeType = dataTable.Rows[i].ItemArray[11].ToString();
                bomDataModel.tapeWidth = dataTable.Rows[i].ItemArray[12].ToString();
                bomDataModel.pitch = dataTable.Rows[i].ItemArray[13].ToString();
                bomDataModel.judgeOCV = dataTable.Rows[i].ItemArray[14].ToString();
                bomDataModel.exp5 = dataTable.Rows[i].ItemArray[15].ToString();
                CurrentBomName = bomDataModel.modelName;
                CurrentBomData.Add(bomDataModel);
            }
        }

        /// <summary>
        /// BOM解析完成后保存
        /// </summary>
        /// <param name="dataGrid"></param>
        public void SaveInAllBomData(string modelName, KryptonDataGridView dataGrid)
        {
            for (int i = 0; i < dataGrid.RowCount; i++)
            {
                if (dataGrid.Rows[i].IsNewRow || string.IsNullOrEmpty(dataGrid.Rows[i].Cells[0].FormattedValue.ToString()))
                {
                    continue;
                }
                string id = Guid.NewGuid().ToString();
                BomDataModel bomDataModel = new BomDataModel();
                bomDataModel.id = id;
                bomDataModel.modelName = modelName;
                bomDataModel.barCode = dataGrid.Rows[i].Cells["barCode"].FormattedValue.ToString();
                
                bomDataModel.description = dataGrid.Rows[i].Cells["description"].FormattedValue.ToString();
                //bomDataModel.result = dataGrid.Rows[i].Cells["result"].FormattedValue.ToString();
                bomDataModel.type = dataGrid.Rows[i].Cells["type"].FormattedValue.ToString();
                bomDataModel.size = dataGrid.Rows[i].Cells["size"].FormattedValue.ToString();
                bomDataModel.value = dataGrid.Rows[i].Cells["value"].FormattedValue.ToString();
                bomDataModel.unit = dataGrid.Rows[i].Cells["unit"].FormattedValue.ToString();
                bomDataModel.grade = dataGrid.Rows[i].Cells["grade"].FormattedValue.ToString();
                bomDataModel.tapeType = dataGrid.Rows[i].Cells["tapeType"].FormattedValue.ToString();
                bomDataModel.tapeWidth = dataGrid.Rows[i].Cells["tapeWidth"].FormattedValue.ToString();
                bomDataModel.pitch = dataGrid.Rows[i].Cells["pitch"].FormattedValue.ToString();
                bomDataModel.judgeOCV = dataGrid.Rows[i].Cells["judgeOCV"].FormattedValue.ToString();
                //bomDataModel.exp5 = dataGrid.Rows[i].Cells["exp5"].FormattedValue.ToString();
                bomDataModel.replaceCode = dataGrid.Rows[i].Cells["ReplaceCode"].FormattedValue.ToString();

                bomDataModel.result = CommonAnalyse.Instance.Check(bomDataModel.type, bomDataModel.size, bomDataModel.value, bomDataModel.unit, bomDataModel.grade).ToString();

                if(bomDataModel.result != dataGrid.Rows[i].Cells["result"].FormattedValue.ToString())
                {
                    int inddex = i;
                    dataGrid.BeginInvoke(new Action(() =>
                    {
                        dataGrid.Rows[inddex].Cells["result"].Value = bomDataModel.result;
                        DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
                        if (dataGrid.Rows[inddex].Cells["result"].FormattedValue.ToString() == "True")
                        {
                            dataGridViewCellStyle.ForeColor = System.Drawing.Color.RoyalBlue;
                        }
                        else
                        {
                            dataGridViewCellStyle.ForeColor = System.Drawing.Color.Red;
                        }
                        dataGrid.Rows[inddex].Cells["result"].Style = dataGridViewCellStyle;
                    }));
                }

                string cmd;
                List<BomDataModel> bomDataModels = BomManager.Instance.AllBomData.Where(x => x.barCode == bomDataModel.barCode).ToList();
                if(bomDataModels.Count > 0)
                {
                    //cmd = string.Format("insert into ALLBOM values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}')", bomDataModel.id, bomDataModel.modelName, bomDataModel.barCode, bomDataModel.replaceCode, bomDataModel.description, bomDataModel.result, bomDataModel.type, bomDataModel.size, bomDataModel.value, bomDataModel.unit, bomDataModel.grade, bomDataModel.tapeType, bomDataModel.tapeWidth, bomDataModel.pitch, bomDataModel.judgeOCV, bomDataModel.exp5);
                    cmd = string.Format("UPDATE ALLBOM SET modelName = '{1}' ,barCode = '{2}',replaceCode = '{3}',description = '{4}',result = '{5}',type = '{6}',size = '{7}',value = '{8}',unit = '{9}',grade = '{10}',tapeType = '{11}',tapeWidth = '{12}',pitch = '{13}',judgeOCV = '{14}',exp5 = '{15}' where  id = '{0}'",
                    bomDataModels[0].id, bomDataModel.modelName, bomDataModel.barCode, bomDataModel.replaceCode, bomDataModel.description, bomDataModel.result, bomDataModel.type, bomDataModel.size, bomDataModel.value, bomDataModel.unit, bomDataModel.grade, bomDataModel.tapeType, bomDataModel.tapeWidth, bomDataModel.pitch, bomDataModel.judgeOCV, bomDataModel.exp5);
                }
                else
                {
                    cmd = string.Format("insert into ALLBOM values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}')", bomDataModel.id, bomDataModel.modelName, bomDataModel.barCode, bomDataModel.replaceCode, bomDataModel.description, bomDataModel.result, bomDataModel.type, bomDataModel.size, bomDataModel.value, bomDataModel.unit, bomDataModel.grade, bomDataModel.tapeType, bomDataModel.tapeWidth, bomDataModel.pitch, bomDataModel.judgeOCV, bomDataModel.exp5);
                }
                
                SqlLiteManager.Instance.BomDB.Insert(cmd);
                bomDataModels = null;
            }
            dataGrid.Refresh();
            GetAllBomData();
        }

        /// <summary>
        /// BOM解析完成后作为当前模板
        /// </summary>
        /// <param name="modelName"></param>
        public void SaveInCurrentBomData(string modelName, KryptonDataGridView dataGrid)
        {
            string comms = "DELETE FROM CurrentBOM";
            SqlLiteManager.Instance.BomDB.Insert(comms);
            for (int i = 0; i < dataGrid.RowCount; i++)
            {
                if (dataGrid.Rows[i].IsNewRow || string.IsNullOrEmpty(dataGrid.Rows[i].Cells[0].FormattedValue.ToString()))
                {
                    continue;
                }
                string id = Guid.NewGuid().ToString();
                BomDataModel bomDataModel = new BomDataModel();
                bomDataModel.id = id;
                bomDataModel.modelName = modelName;
                bomDataModel.barCode = dataGrid.Rows[i].Cells[0].FormattedValue.ToString();
                //bomDataModel.replaceCode = "";
                bomDataModel.description = dataGrid.Rows[i].Cells[1].FormattedValue.ToString();
                bomDataModel.result = dataGrid.Rows[i].Cells[2].FormattedValue.ToString();
                bomDataModel.type = dataGrid.Rows[i].Cells[3].FormattedValue.ToString();
                bomDataModel.size = dataGrid.Rows[i].Cells[4].FormattedValue.ToString();
                bomDataModel.value = dataGrid.Rows[i].Cells[5].FormattedValue.ToString();
                bomDataModel.unit = dataGrid.Rows[i].Cells[6].FormattedValue.ToString();
                bomDataModel.grade = dataGrid.Rows[i].Cells[7].FormattedValue.ToString();
                bomDataModel.tapeType = dataGrid.Rows[i].Cells[8].FormattedValue.ToString();
                bomDataModel.tapeWidth = dataGrid.Rows[i].Cells[9].FormattedValue.ToString();
                bomDataModel.pitch = dataGrid.Rows[i].Cells[10].FormattedValue.ToString();
                bomDataModel.judgeOCV = dataGrid.Rows[i].Cells[11].FormattedValue.ToString();
                //bomDataModel.exp5 = dataGrid.Rows[i].Cells[12].FormattedValue.ToString();
                bomDataModel.replaceCode = dataGrid.Rows[i].Cells[12].FormattedValue.ToString();

                string cmd;
                List<BomDataModel> bomDataModels = BomManager.Instance.CurrentBomData.Where(x => x.barCode == bomDataModel.barCode).ToList();
                if (bomDataModels.Count > 0)
                {
                    //cmd = string.Format("insert into ALLBOM values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}')", bomDataModel.id, bomDataModel.modelName, bomDataModel.barCode, bomDataModel.replaceCode, bomDataModel.description, bomDataModel.result, bomDataModel.type, bomDataModel.size, bomDataModel.value, bomDataModel.unit, bomDataModel.grade, bomDataModel.tapeType, bomDataModel.tapeWidth, bomDataModel.pitch, bomDataModel.judgeOCV, bomDataModel.exp5);
                    cmd = string.Format("UPDATE CurrentBOM SET modelName = '{1}' ,barCode = '{2}',replaceCode = '{3}',description = '{4}',result = '{5}',type = '{6}',size = '{7}',value = '{8}',unit = '{9}',grade = '{10}',tapeType = '{11}',tapeWidth = '{12}',pitch = '{13}',judgeOCV = '{14}',exp5 = '{15}' where  id = '{0}'",
                    bomDataModels[0].id, bomDataModel.modelName, bomDataModel.barCode, bomDataModel.replaceCode, bomDataModel.description, bomDataModel.result, bomDataModel.type, bomDataModel.size, bomDataModel.value, bomDataModel.unit, bomDataModel.grade, bomDataModel.tapeType, bomDataModel.tapeWidth, bomDataModel.pitch, bomDataModel.judgeOCV, bomDataModel.exp5);
                }
                else
                {
                    cmd = string.Format("insert into CurrentBOM values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}')", bomDataModel.id, bomDataModel.modelName, bomDataModel.barCode, bomDataModel.replaceCode, bomDataModel.description, bomDataModel.result, bomDataModel.type, bomDataModel.size, bomDataModel.value, bomDataModel.unit, bomDataModel.grade, bomDataModel.tapeType, bomDataModel.tapeWidth, bomDataModel.pitch, bomDataModel.judgeOCV, bomDataModel.exp5);
                }
                
                SqlLiteManager.Instance.BomDB.Insert(cmd);
                bomDataModels = null;
            }
            GetCurrentBomData();
        }

        public void DeleteModel(string modelName)
        {
            string comms1 = modelName == "ALL" ? "DELETE FROM CurrentBOM" : string.Format("DELETE FROM CurrentBOM where modelName = '{0}' ", modelName);
            SqlLiteManager.Instance.BomDB.Insert(comms1);

            string comms2 = modelName == "ALL" ? "DELETE FROM ALLBOM" : string.Format("DELETE FROM ALLBOM where modelName = '{0}' ", modelName);
            SqlLiteManager.Instance.BomDB.Insert(comms2);
            GetAllBomData();
            GetCurrentBomData();
        }

        public void SetCurrentModel(string modelName)
        {
            string comms = "DELETE FROM CurrentBOM";
            SqlLiteManager.Instance.BomDB.Insert(comms);
            string comm = string.Format("insert into  CurrentBom  select * from allBom where modelname ='{0}'", modelName);
            SqlLiteManager.Instance.BomDB.Insert(comm);
            CurrentBomName = modelName;
            GetAllBomData();
            GetCurrentBomData();
        }

        public void UpdataBomData(BomDataModel bomDataModel)
        {
            string cmd = string.Format("UPDATE ALLBOM SET replaceCode = '{0}', tapeType = '{1}', tapeWidth ='{2}',pitch='{3}',judgeOCV='{4}',exp5='{5}' where  modelName = '{6}' and barCode = '{7}'", bomDataModel.replaceCode, bomDataModel.tapeType, bomDataModel.tapeWidth, bomDataModel.pitch, bomDataModel.judgeOCV, bomDataModel.exp5, bomDataModel.modelName, bomDataModel.barCode);
            SqlLiteManager.Instance.BomDB.Insert(cmd);
            if (CurrentBomName == bomDataModel.modelName)
            {
                SetCurrentModel(CurrentBomName);
            }
        }

        public void InserIntoBomData(BomDataModel bomDataModel)
        {
            string cmd = string.Format("insert into ALLBOM values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}')", bomDataModel.id, bomDataModel.modelName, bomDataModel.barCode, bomDataModel.replaceCode, bomDataModel.description, bomDataModel.result, bomDataModel.type, bomDataModel.size, bomDataModel.value, bomDataModel.unit, bomDataModel.grade, bomDataModel.tapeType, bomDataModel.tapeWidth, bomDataModel.pitch, bomDataModel.judgeOCV, bomDataModel.exp5);
            SqlLiteManager.Instance.BomDB.Insert(cmd);
            GetAllBomData();
            if (CurrentBomName == bomDataModel.modelName)
            {
                SetCurrentModel(CurrentBomName);
                GetCurrentBomData();
            }
        }

        /// <summary>
        /// 所有Bom数据
        /// </summary>
        public List<BomDataModel> AllBomData = new List<BomDataModel>();

        /// <summary>
        /// 当前的BomData
        /// </summary>
        public List<BomDataModel> CurrentBomData = new List<BomDataModel>();

        /// <summary>
        /// 在Bom中查询条码信息
        /// </summary>
        /// <param name="barCode">条码</param>
        /// <param name="modelName">模板名称</param>
        /// <returns></returns>
        public BomDataModel SearchByBarCode(string barCode)
        {
            List<BomDataModel> bomDataModels = new List<BomDataModel>();
            bomDataModels = CurrentBomData.Where(x => x.barCode == barCode).ToList();
            if (bomDataModels.Count == 0)
            {
                bomDataModels = AllBomData.Where(x => x.barCode == barCode).ToList();
            }
            return bomDataModels.Count > 0 ? bomDataModels[0] : null;
        }

        /// <summary>
        /// 在Bom中查询条码信息
        /// </summary>
        /// <param name="barCode">条码</param>
        /// <param name="modelName">模板名称</param>
        /// <returns></returns>
        public List<BomDataModel> SearchByBarCode(string barCode, string modelName)
        {
            if (modelName == CurrentBomName)
            {
                return CurrentBomData.Where(x => x.barCode == barCode && x.modelName == modelName).ToList();
            }
            else
            {
                return AllBomData.Where(x => x.barCode == barCode && x.modelName == modelName).ToList();
            }

        }

        /// <summary>
        /// 补全BOM中的信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="type"></param>
        /// <param name="width"></param>
        /// <param name="pitch"></param>
        /// <param name="ocv"></param>
        public void BOMInfoComplement(string code, string type, string width, string pitch, string ocv)
        {
            List<BomDataModel> bomDatas = AllBomData.Where(x => x.barCode == code && CurrentBomName == x.modelName).ToList();

            if (bomDatas.Count <= 0) {
                return;
            }

            bomDatas[0].tapeType = type;
            bomDatas[0].tapeWidth = width;
            bomDatas[0].pitch = pitch;
            bomDatas[0].judgeOCV = ocv;

            UpdataBomData(bomDatas[0]);
        }
    }
}
