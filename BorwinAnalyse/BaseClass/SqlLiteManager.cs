using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BorwinAnalyse.BaseClass
{
    public class SqlLiteManager
    {
        private static SqlLiteManager instance;

        public static SqlLiteManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new SqlLiteManager();
                return instance;
            }
            set => instance = value;
        }

        /// <summary>
        /// 初始化数据库
        /// </summary>
        public void Init()
        {
            InitData();
            InitTable();
            SpecialLcrOffsetManager.Instance.SearchALLSpectialOffset();
           
        }

        /// <summary>
        /// 数据库对象
        /// </summary>
        public SQLConnect DB;

        /// <summary>
        /// BOM数据库对象
        /// </summary>
        public SQLConnect BomDB;

        private void InitData()
        {
            string DicPath = @"SqlLiteData";
            if (!Directory.Exists(DicPath))
            {
                Directory.CreateDirectory(DicPath);
            }
            if (!File.Exists(DicPath + "\\MachineData.db"))
            {
                DB = new SQLConnect(DicPath + "\\MachineData.db");
                DB.NewDbFile(DicPath + "\\MachineData.db");
            }
            if (DB == null)
            {
                DB = new SQLConnect(DicPath + "\\MachineData.db");
            }
            DB.OpenDB();

            if (!Directory.Exists(DicPath))
            {
                Directory.CreateDirectory(DicPath);
            }
            if (!File.Exists(DicPath + "\\BomData.db"))
            {
                BomDB = new SQLConnect(DicPath + "\\BomData.db");
                BomDB.NewDbFile(DicPath + "\\BomData.db");
            }
            if (BomDB == null)
            {
                BomDB = new SQLConnect(DicPath + "\\BomData.db");
            }
            BomDB.OpenDB();
        }

        private void InitTable()
        {
            string tableName = "ALLBOM";
            string commandText = "CREATE TABLE IF NOT EXISTS " + tableName + "(id varchar PRIMARY KEY,modelName varchar,barCode varchar, replaceCode varchar, description varchar,result varchar,type varchar,size varchar,value varchar,unit varchar,grade varchar,tapeType varchar,tapeWidth varchar,pitch varchar,judgeOCV varchar,exp5 varchar)";
            BomDB.NewTable(commandText);

            string tableName4 = "CurrentBOM";
            string commandText4 = "CREATE TABLE IF NOT EXISTS " + tableName4 + "(id varchar PRIMARY KEY,modelName varchar,barCode varchar,replaceCode varchar, description varchar,result varchar,type varchar,size varchar,value varchar,unit varchar,grade varchar,tapeType varchar,tapeWidth varchar,pitch varchar,judgeOCV varchar,exp5 varchar)";
            BomDB.NewTable(commandText4);

            string MaterialInfo = "MaterialInformation";
            string MaterialInfoCom = "CREATE TABLE IF NOT EXISTS " + MaterialInfo + "(code varchar,updateTime varchar,tapeType varchar,tapeWidth varchar,pitch varchar,judgeOCV varchar)";
            BomDB.NewTable(MaterialInfoCom);

            string SpecialOffset = "SpecialOffset";
            string SpecialOffsetCom = "CREATE TABLE IF NOT EXISTS " + SpecialOffset + "(Key varchar, Use varchar, Offset_mΩ varchar,Offset_Ω varchar,Offset_kΩ varchar,Offset_MeΩ varchar,Offset_pF varchar,Offset_nF varchar,Offset_uF varchar,Offset_mF varchar,Offset_F varchar)";
            BomDB.NewTable(SpecialOffsetCom);


            string tableName2 = "Language";
            string commandText2 = "CREATE TABLE IF NOT EXISTS " + tableName2 + "(context varchar PRIMARY KEY,chinese varchar, english varchar,exp1 varchar,exp2 varchar,exp3 varchar,exp4 varchar,exp5 varchar)";
            DB.NewTable(commandText2);


            string tableName3 = "LanguageType";
            string commandText3 = "CREATE TABLE IF NOT EXISTS " + tableName3 + "(languageIndex varchar PRIMARY KEY,name varchar,currentLanguage varchar )";
            DB.NewTable(commandText3);

            string log = "Log";
            string logCom = "CREATE TABLE IF NOT EXISTS " + log + "(time datetime ,type varchar,level varchar,content varchar,operator varchar,exp1 varchar,exp2 varchar,exp3 varchar,exp4 varchar,exp5 varchar)";
            DB.NewTable(logCom);

            string Consumable = "Consumable";
            string ConsumableCom = "CREATE TABLE IF NOT EXISTS " + Consumable + "(Item varchar,DateUpdate varchar,UseCount varchar,UseLimit varchar,DateReplace varchar,Description varchar)";
            DB.NewTable(ConsumableCom);

            string Battery = "BatteryManager";
            string BatteryCom = "CREATE TABLE IF NOT EXISTS " + Battery + "(ID varchar,CycleCount varchar,LastCapacity varchar,UpdateTime varchar)";
            DB.NewTable(BatteryCom);
        }
    }
}
