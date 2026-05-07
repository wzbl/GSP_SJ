using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Emgu.CV.Stitching.Stitcher;

namespace SqlHelper
{
    public class SQLDataControl
    {
        private static FAI_NewEntities db = new FAI_NewEntities();

        #region 程序

        /// <summary>
        /// 获取所有程序
        /// </summary>
        /// <returns></returns>
        public static List<View_Eng_Program> GetAllProgramm()
        {
            return db.View_Eng_Program.AsNoTracking().ToList();
        }

        public static View_Eng_Program GetProgramm(string productCode)
        {
            return db.View_Eng_Program.AsNoTracking().Where(x => x.产品编号 == productCode).First();
        }

        public static void AddProgram(string productCode, string programName, string customerCode, string boardSide, string creator, string ruleCode, string vision, string options)
        {
            db.P_Insert_Eng_Program(productCode, programName, customerCode, boardSide, ruleCode, "NORMAL", "", creator, creator, vision, options);
        }

        public static void DeleteProgram(string productCode)
        {
            int i = db.Database.ExecuteSqlCommand("DELETE FROM Eng_Program WHERE ProductCode = {0}", productCode);
            DeleteBom(productCode);
            DeleteXYData(productCode);
            db.SaveChanges();
        }

        #endregion

        #region 测试报告

        public static List<P_Man_Report_Search_Result> SearchMan_Report(DateTime start, DateTime end, string line, string result, string status)
        {
            return db.P_Man_Report_Search(start, end, status, result, line).ToList();
        }
        public static void DeleteMan_Report(string reportCode)
        {
            db.Database.ExecuteSqlCommand("delete from Man_Report where ReportCode={0}", reportCode);
            db.SaveChanges();
        }

        public static DbRawSqlQuery<Man_Report> GetMan_Report(string ReportCode)
        {
            DbRawSqlQuery<Man_Report> res = db.Database.SqlQuery<Man_Report>("select * from Man_Report where ReportCode = "+ ReportCode);
            return res;
        }

        public static void AddMan_Report(Man_Report report)
        {
            db.P_Man_ReportAdd
                (
                 report.ReportCode,
                 report.BoardSide,
                 report.PLCode,
                 report.ProductCode,
                 report.ProductName,
                 report.WoCode,
                 report.LotNumber,
                 report.BatchQty,
                 report.BoardQty,
                 report.IsCheckNoSMD,
                 report.Remarks,
                 report.Creator,
                 report.Modifier,
                 report.OptionCode,
                 report.Barcode,
                 report.@class,
                 report.CheckType,
                 report.PCBVersion,
                 report.BomVersion,
                 report.DeviceName);

        }

        /// <summary>
        /// 修改测试报告
        /// </summary>
        /// <param name="ReportCode"></param>
        /// <param name="Barcode"></param>
        /// <param name="OptionCode"></param>
        /// <param name="LotNumber"></param>
        /// <param name="PLCode"></param>
        /// <param name="CheckType"></param>
        /// <param name="_Class"></param>
        /// <param name="BatchQty"></param>
        /// <param name="WoCode"></param>
        /// <param name="PCBVersion"></param>
        public static void UpdateMan_Report(string ReportCode, string Barcode, string OptionCode, string LotNumber, string PLCode, string CheckType, string _Class, int BatchQty, string WoCode, string PCBVersion, string Modifier, string Remark)
        {
            try
            {
                int i = db.Database.ExecuteSqlCommand("update Man_Report set Barcode={0},OptionCode={1},LotNumber={2},PLCode={3},CheckType={4},class={5},BatchQty={6},WoCode={7},PCBVersion={8},Modifier={9} ,ModificationDate={10},Remarks={11} where ReportCode={12}", Barcode, OptionCode, LotNumber, PLCode, CheckType, _Class, BatchQty, WoCode, PCBVersion, Modifier, DateTime.Now, Remark, ReportCode);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

        }


        public static void FinishMan_Report(string ReportCode, string finishBy)
        {
            int i = db.Database.ExecuteSqlCommand("update Man_Report set Status={0},CheckResult={1}, FinishedBy={2},FinishedDate={3} where ReportCode={4}", "CLOSE", "PASS", finishBy, DateTime.Now, ReportCode);
            db.SaveChanges();
        }


        public static void SetQAMan_Report(string ReportCode, string QAChecker)
        {
            int i = db.Database.ExecuteSqlCommand("update Man_Report set IsQACheck={0},QAChecker={1}, QACheckTimer={2} where ReportCode={3}", "YES", QAChecker, DateTime.Now, ReportCode);
            db.SaveChanges();
        }


        public static List<Man_ReportItem> Search_Man_ReportItem(string reportCode)
        {
            return db.Man_ReportItem.AsNoTracking().Where(x => x.ReportCode == reportCode).ToList();
        }

        public static void AddMan_ReportItem(Man_ReportItem item)
        {
            try
            {
                db.Database.ExecuteSqlCommand("insert into " +
           "Man_ReportItem " +
           "(ReportCode," +
           "Position," +
           "BoardId," +
           //"Sequence," +
           "BomSequence," +
           "MaterialCode," +
           "MaterialName," +
           "LcrType," +
           "LcrStandardValue," +
           "LcrUnitCode," +
           "LcrMaxValue," +
           "LcrMinValue," +
           "MaxTolerance," +
           "MinTolerance," +
           "Size," +
           "X,Y,Angle,IsSMD,LX,LY,RX,RY,Status," +
           "IsDefined,OLcrStandardValue,OLcrMaxValue,OLcrMinValue,ToleranceType,OLcrUnitCode,XYGroups,ErrorBoard)" +
           "values({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30})",
           item.ReportCode,
           item.Position,
           item.BoardId,
           item.BomSequence,
           item.MaterialCode,
           item.MaterialName,
           item.LcrType,
           item.LcrStandardValue,
           item.LcrUnitCode,
           item.LcrMaxValue,
           item.LcrMinValue,
           item.MaxTolerance,
           item.MinTolerance,
           item.Size, item.X, item.Y, item.Angle, item.IsSMD, item.LX, item.LY, item.RX, item.RY, item.Status,
           item.IsDefined, item.OLcrStandardValue, item.OLcrMaxValue, item.OLcrMinValue, item.ToleranceType, item.OLcrUnitCode, item.XYGroups, item.ErrorBoard);
            }
            catch (Exception)
            {

            }

        }

        public static void UpdateMan_ReportItem_LXY(string ReportCode, string position, decimal px, decimal py)
        {
            int i = db.Database.ExecuteSqlCommand("update Man_ReportItem set LX={0} ,LY ={1} where ReportCode={2} and Position = {3}", px, py, ReportCode, position);
        }

        #endregion

        #region BOM

        /// <summary>
        /// 搜索BOM
        /// </summary>
        /// <param name="productCode"></param>
        /// <returns></returns>
        public static List<P_Search_Eng_Bom_Result> SearchBom(string productCode)
        {
            return db.P_Search_Eng_Bom(productCode).ToList();
        }

        public static List<Eng_Bom> GetBomByProductCode(string productCode, string MaterialCode)
        {
            return db.Eng_Bom.AsNoTracking().Where(x => x.ProductCode == productCode && x.MaterialCode == MaterialCode).ToList();
        }

        public static void AddBom(Eng_Bom bom)
        {
            db.Eng_Bom.Add(bom);
            db.SaveChanges();
        }

        public static void DeleteBom(string productCode)
        {
            int i = db.Database.ExecuteSqlCommand("DELETE FROM Eng_Bom WHERE ProductCode = {0}", productCode);
            db.Database.ExecuteSqlCommand("DELETE FROM Eng_Bom WHERE ProductCode = {0}", productCode);
            db.Database.ExecuteSqlCommand("DELETE FROM Eng_BomSub WHERE ProductCode = {0}", productCode);
            db.Database.ExecuteSqlCommand("DELETE FROM Eng_XYData WHERE ProductCode = {0}", productCode);
            db.SaveChanges();
        }

        public static void DeleteMaterialCode(string productCode, string MaterialCode)
        {
            int i = db.Database.ExecuteSqlCommand("DELETE FROM Eng_Bom WHERE ProductCode = {0} and MaterialCode = {1}", productCode, MaterialCode);
            DeleteEng_BomSub(productCode, MaterialCode);
            db.SaveChanges();
        }

        public static void InsertBom
            (string productCode,
            string position,
            string materialCode,
            string materialName,
            Nullable<int> quantity,
              Nullable<int> row,
            string lcrType,
            Nullable<decimal> lcrStandardValue,
            string lcrUnitCode,
            Nullable<decimal> lcrMaxValue,
            Nullable<decimal> lcrMinValue,
            string size,
            string remarks,
            string creator,
            Nullable<decimal> maxTolerance,
            Nullable<decimal> minTolerance,
            string toleranceType)
        {
            db.P_Insert_Eng_Bom(productCode, row, materialCode, materialName, quantity, position, null, lcrType, lcrStandardValue, lcrUnitCode, lcrMaxValue, lcrMinValue, size, remarks, creator, creator, maxTolerance, minTolerance, null, null, toleranceType, null);
        }


        public static void InsertEngBom(
            string productCode,
            string position,
            string materialCode,
            string materialName,
            int quantity,
            int row,
            string lcrType,
            decimal lcrStandardValue,
            string lcrUnitCode,
            decimal lcrMaxValue,
            decimal lcrMinValue,
            string size,
            string ReplaceCode,
            string creator,
            decimal maxTolerance,
            decimal minTolerance,
            string toleranceType)
        {
            if (db.Eng_Bom.Where(x => x.ProductCode == productCode && x.MaterialCode == materialCode).ToList().Count == 0)
            {
                db.Eng_Bom.Add(new Eng_Bom()
                {
                    ProductCode = productCode,
                    Position = position,
                    MaterialCode = materialCode,
                    MaterialName = materialName,
                    Quantity = quantity,
                    LcrType = lcrType,
                    LcrStandardValue = lcrStandardValue,
                    LcrUnitCode = lcrUnitCode,
                    LcrMaxValue = lcrMaxValue,
                    LcrMinValue = lcrMinValue,
                    Size = size,
                    Creator = creator,
                    Modifier = creator,
                    CreationDate = DateTime.Now,
                    ModificationDate = DateTime.Now,
                    MaxTolerance = maxTolerance,
                    MinTolerance = minTolerance,
                    ToleranceType = toleranceType,
                    Row = row,
                    AllowSubstitute = null,
                    ComponentPackaging = null,
                    ScreenPrinting = null,
                    Status = null
                });
            }
            else
            {
                db.Eng_Bom.Where(x => x.ProductCode == productCode && x.MaterialCode == materialCode).ToList().ForEach(x =>
                {
                    x.Position = position;
                    x.MaterialName = materialName;
                    x.Quantity = quantity;
                    x.LcrType = lcrType;
                    x.LcrStandardValue = lcrStandardValue;
                    x.LcrUnitCode = lcrUnitCode;
                    x.LcrMaxValue = lcrMaxValue;
                    x.LcrMinValue = lcrMinValue;
                    x.Size = size;
                    x.Modifier = creator;
                    x.ModificationDate = DateTime.Now;
                    x.MaxTolerance = maxTolerance;
                    x.MinTolerance = minTolerance;
                    x.ToleranceType = toleranceType;
                    //x.Row = row;
                });
            }
            try
            {
                //db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                var sqlEx = ex.GetBaseException() as SqlException;
                if (sqlEx != null)
                {
                    Console.WriteLine($"SQL Error Number: {sqlEx.Number}, Message: {sqlEx.Message}");
                    // 常见 Number: 2627 (主键冲突), 547 (外键约束), 515 (不能插入NULL)
                }
                throw;
            }

        }

        #endregion

        #region XYData
        public static List<P_Search_Eng_XYData_Result> SearchXYData(string productCode)
        {
            return db.P_Search_Eng_XYData(productCode).ToList();
        }

        public static List<Eng_XYData> GetEng_XYDatas(string productCode)
        {
            return db.Eng_XYData.Where(x => x.ProductCode == productCode).ToList();
        }

        public static void DeleteXYData(string productCode)
        {
            int i = db.Database.ExecuteSqlCommand("DELETE FROM Eng_XYData WHERE ProductCode = {0}", productCode);
            db.SaveChanges();
        }
        public static void InsertXYData(
            string productCode,
            string position,
            Nullable<int> boardId,
            string boardSide,
            string isSMD,
            string materialCode,
            string materialName,
            Nullable<decimal> x,
            Nullable<decimal> y,
            string unit,
            Nullable<decimal> angle,
            Nullable<decimal> rX,
            Nullable<decimal> rY,
            string remarks,
            string creator,
            string modifier,
            string standardCode,
            Nullable<decimal> standardRotation,
            string size,
            string lcrType,
            string isDefined,
            Nullable<decimal> oX,
            Nullable<decimal> oY,
            Nullable<decimal> oAngle,
            Nullable<int> row,
            Nullable<decimal> lX,
            Nullable<decimal> lY,
            string isMerge,
            byte[] standardImage,
            Nullable<int> sequence,
            Nullable<decimal> furnaceLcrStandardValue,
            string furnaceLcrUnitCode,
            Nullable<decimal> furnaceLcrMaxValue,
            Nullable<decimal> furnaceLcrMinValue,
            Nullable<decimal> lXCompensation,
            Nullable<decimal> lYCompensation,
            string componentPackaging,
            Nullable<decimal> sX,
            Nullable<decimal> sY,
            Nullable<decimal> adjustedAngle,
            Nullable<int> groups)
        {
            db.P_Insert_Eng_XYData(productCode, position, boardId, boardSide, isSMD, materialCode, materialName, x, y, unit, angle, rX, rY, remarks, creator, modifier, standardCode, standardRotation, size, lcrType, isDefined, oX, oY, oAngle, row, lX, lY, isMerge, standardImage, sequence, furnaceLcrStandardValue, furnaceLcrUnitCode, furnaceLcrMaxValue, furnaceLcrMinValue, lXCompensation, lYCompensation, componentPackaging, sX, sY, adjustedAngle, groups);
        }

        public static void UpdateXYData_RXY(string productCode, string position, decimal px, decimal py)
        {
            //db.Eng_XYData.AsNoTracking().Where(x => x.ProductCode == productCode && x.Position == position).ToList().ForEach(x =>
            //{
            //    x.RX = px; x.RY = py;
            //});

            int i = db.Database.ExecuteSqlCommand("update Eng_XYData set RX={0} ,RY ={1} where ProductCode={2} and Position = {3}", px, py, productCode, position);
            db.SaveChanges();
        }


        public static void UpdateXYData_LXY(string productCode, string position, decimal px, decimal py)
        {
            int i = db.Database.ExecuteSqlCommand("update Eng_XYData set LX={0} ,LY ={1} where ProductCode={2} and Position = {3}", px, py, productCode, position);

        }

        public static void UpdateProgramOptionPicture(string productCode, string BoardSide, byte[] img)
        {
            db.P_Update_Eng_ProgramOption(productCode, BoardSide, img);
            db.SaveChanges();
        }

        public static byte[] GetProgramOptionPicture(string productCode)
        {
            var pic = db.Eng_ProgramOption.AsNoTracking().Where(x => x.ProductCode == productCode).ToList();
            if (pic.Count > 0)
            {
                return pic[0].Picture;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 模板

        /// <summary>
        /// 获取所有公共模板库
        /// </summary>
        /// <returns></returns>
        public static List<Eng_PubModel> GetAllEng_PubModel()
        {
            return db.Eng_PubModel.AsNoTracking().ToList();
        }

        public static void DeleteEng_PubModel(string MaterialCode)
        {
            db.Database.ExecuteSqlCommand("delete from Eng_PubModel where MaterialCode={0}", MaterialCode);
            db.Database.ExecuteSqlCommand("delete from Eng_PubModelItem where MaterialCode={0}", MaterialCode);
            db.SaveChanges();
        }

        public static void DeleteAllEng_PubModel()
        {
            db.Database.ExecuteSqlCommand("delete from Eng_PubModel");
            db.Database.ExecuteSqlCommand("delete from Eng_PubModelItem");
            db.SaveChanges();
        }

        /// <summary>
        /// 获取模板数据
        /// </summary>
        /// <returns></returns>
        public static List<Eng_PubModelItem> GETEng_PubModelItem(string materialCode)
        {
            return db.Eng_PubModelItem.AsNoTracking().Where(x => x.MaterialCode == materialCode).ToList();
        }

        public static List<Eng_Model> GetEng_Model(string productCode)
        {
            return db.Eng_Model.AsNoTracking().Where(x => x.ProductCode == productCode).ToList();
        }

        public static List<Eng_ModelItem> GetEng_ModelItem(string productCode, string materialCode)
        {
            return db.Eng_ModelItem.AsNoTracking().Where(x => x.ProductCode == productCode && x.MaterialCode == materialCode).ToList();
        }

        public static void UpdateEng_Model(Eng_Model eng_Model)
        {
            if (db.Eng_ModelItem.AsNoTracking().Where(x => x.ProductCode == eng_Model.ProductCode && x.MaterialCode == eng_Model.MaterialCode).ToList().Count == 0)
            {
                int i = db.Database.ExecuteSqlCommand("insert into Eng_Model(ProductCode,MaterialCode,MaterialName,LCRType,Creator,CreationDate,Remarks) values ({0},{1},{2},{3},{4},{5},{6})", eng_Model.ProductCode, eng_Model.MaterialCode, eng_Model.MaterialName, eng_Model.LCRType, eng_Model.Creator, eng_Model.CreationDate, "");
                db.SaveChanges();
            }
        }

        public static void UpdateEng_ModelItem(Eng_ModelItem modelItem)
        {
            if (db.Eng_ModelItem.AsNoTracking().Where(x => x.ProductCode == modelItem.ProductCode && x.MaterialCode == modelItem.MaterialCode && x.Id == modelItem.Id).ToList().Count == 0)
            {
                int i = db.Database.ExecuteSqlCommand("insert into Eng_ModelItem(ProductCode,MaterialCode,mPicture,mPW,mPH,mZoomRatio,mDPI,Id,Groups,IsMain,Polarity,IsManual,LCy,HCy,NA,NB,XA,XB,P1,P2,P3,P4,P5,PLeft,PTop,Pw,Ph,PLeft2,PTop2,Pw2,Ph2,PLeft3,PTop3,Pw3,Ph3,Creator,CreationDate,Remarks) values ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37})",
                    modelItem.ProductCode, modelItem.MaterialCode, modelItem.mPicture,
                    modelItem.mPW, modelItem.mPH, modelItem.mZoomRatio, modelItem.mDPI, 
                    modelItem.Id, modelItem.Groups, modelItem.IsMain, modelItem.Polarity, modelItem.IsManual,
                    modelItem.LCy, modelItem.HCy, modelItem.NA, modelItem.NB, modelItem.XA, modelItem.XB, 
                    modelItem.P1, modelItem.P2, modelItem.P3, modelItem.P4, modelItem.P5,
                    modelItem.PLeft, modelItem.PTop, modelItem.Pw, modelItem.Ph,
                    modelItem.PLeft2, modelItem.PTop2, modelItem.Pw2, modelItem.Ph2, 
                    modelItem.PLeft3, modelItem.PTop3, modelItem.Pw3, modelItem.Ph3, 
                    modelItem.Creator, modelItem.CreationDate, modelItem.Remarks);
            }
            else
            {
                int i = db.Database.ExecuteSqlCommand("update Eng_ModelItem set mPicture={0},mPW={1},mPH={2},mZoomRatio={3},mDPI={4},Id={5},Groups={6},IsMain={7},Polarity={8},IsManual={9},LCy={10},HCy={11},NA={12},NB={13},XA={14},XB={15},P1={16},P2={17},P3={18},P4={19},P5={20},PLeft={21},PTop={22},Pw={23},Ph={24},PLeft2={25},PTop2={26},Pw2={27},Ph2={28},PLeft3={29},PTop3={30},Pw3={31},Ph3={32},Remarks={33} where ProductCode = {34} and MaterialCode ={35} and Id={36}",
                    modelItem.mPicture,
                    modelItem.mPW,  modelItem.mPH,
                    modelItem.mZoomRatio,  modelItem.mDPI,
                    modelItem.Id, modelItem.Groups, modelItem.IsMain, modelItem.Polarity, modelItem.IsManual, 
                    modelItem.LCy, modelItem.HCy, modelItem.NA, modelItem.NB, modelItem.XA, modelItem.XB, 
                    modelItem.P1, modelItem.P2, modelItem.P3, modelItem.P4, modelItem.P5,
                    modelItem.PLeft, modelItem.PTop, modelItem.Pw, modelItem.Ph, modelItem.PLeft2, modelItem.PTop2,
                    modelItem.Pw2, modelItem.Ph2, modelItem.PLeft3, modelItem.PTop3, modelItem.Pw3, modelItem.Ph3, modelItem.Remarks,
                    modelItem. ProductCode  , modelItem. MaterialCode , modelItem.Id);
            }
            db.SaveChanges();
        }

        public static void DeleteEng_ModelItem(string productCode, string materialCode, int id)
        {
            db.Database.ExecuteSqlCommand("delete from Eng_ModelItem where ProductCode={0} and MaterialCode={1} and id ={2}", productCode, materialCode,id);
            db.SaveChanges();
        }
        #endregion

        #region 电桥参数
        public static string GetMeterOptionCode(string Optioncode)
        {
            List<Eng_MeterOption> eng_MeterOptions = GetMeterOption().Where(x => x.OptionCode == Optioncode).ToList();
            if (eng_MeterOptions != null && eng_MeterOptions.Count > 0)
            {
                return eng_MeterOptions[0].OptionName;
            }
            return Optioncode;
        }

        public static string GetMeterOptionName(string OptionName)
        {
            List<Eng_MeterOption> eng_MeterOptions = GetMeterOption().Where(x => x.OptionName == OptionName).ToList();
            if (eng_MeterOptions != null && eng_MeterOptions.Count > 0)
            {
                return eng_MeterOptions[0].OptionCode;
            }
            return OptionName;
        }

        public static List<Eng_MeterOptionItem> GetMeterOptionItem(string OptionName)
        {
            string optionCode = GetMeterOption().Where(x => x.OptionName == OptionName).ToList()[0].OptionCode;
            return db.Eng_MeterOptionItem.AsNoTracking().Where(x => x.OptionCode == optionCode).ToList();
        }

        public static List<View_Eng_MeterOptionItem> View_Eng_MeterOptionItem()
        {
            return db.View_Eng_MeterOptionItem.AsNoTracking().ToList();
        }

        public static List<View_ComponentSize> View_ComponentSize()
        {
            return db.View_ComponentSize.AsNoTracking().ToList();
        }

        public static List<View_Bas_CompensationValue> View_Bas_CompensationValue()
        {
            return db.View_Bas_CompensationValue.AsNoTracking().ToList();
        }

        /// <summary>
        /// 获取电桥参数
        /// </summary>
        /// <returns></returns>
        public static List<Eng_MeterOption> GetMeterOption()
        {
            return db.Eng_MeterOption.AsNoTracking().Where(x => x.IsVisible == "YES").ToList();
        }

        /// <summary>
        /// 删除电桥参数
        /// </summary>
        /// <param name="OptionCode"></param>
        public static void DeleteMeterOption(string OptionCode)
        {
            ObjectParameter retVal = new ObjectParameter("retVal", 0);
            ObjectParameter retMsg = new ObjectParameter("retMsg", "");
            string optionCode = GetMeterOption().Where(x => x.OptionName == OptionCode).ToList()[0].OptionCode;
            db.P_Eng_MeterOptionDelete(optionCode, retVal, retMsg);
            db.SaveChanges();
        }

        /// <summary>
        /// 重命名电桥参数
        /// </summary>
        /// <param name="OptionCode"></param>
        public static void RenameMeterOption(string OptionCode, string OptionName)
        {
            ObjectParameter retVal = new ObjectParameter("retVal", 0);
            ObjectParameter retMsg = new ObjectParameter("retMsg", "");
            string optionCode = GetMeterOption().Where(x => x.OptionName == OptionName).ToList()[0].OptionCode;
            db.P_Eng_MeterOptionRename(optionCode, OptionCode, retVal, retMsg);
            db.SaveChanges();
        }


        public static void CopyMeterOption(string newName, string SourceName)
        {
            ObjectParameter retVal = new ObjectParameter("retVal", 0);
            ObjectParameter retMsg = new ObjectParameter("retMsg", "");
            string optionCode = GetMeterOption().Where(x => x.OptionName == SourceName).ToList()[0].OptionCode;
            db.P_Eng_MeterOptionCopy(newName, optionCode, retVal, retMsg);
        }

        /// <summary>
        /// 获取电桥数据
        /// </summary>
        /// <param name="Sort">
        /// LcrType:元件类型
        /// RUnit：电阻单位
        /// CUnit：电容单位
        /// LUnit：电感单位
        /// DCRUnit：DCR单位
        /// LEDUnit：LED单位
        /// RFunctionType：电阻测试类型
        /// CFunctionType：电容测试类型
        /// LFunctionType：电感测试类型
        /// DCRFunctionType：DCR测试类型
        /// LEDFunctionType：LED测试类型
        /// Frequency：测试频率
        /// Voltage：测试电压
        /// RangeType：量程类别
        /// Range：量程
        /// Speed：测试速度
        /// Resistance：测试内阻
        /// </param>
        /// <returns></returns>
        public static List<Eng_Meter> GetEng_MeterBySort(string Sort)
        {
            return db.Eng_Meter.AsNoTracking().Where(x => x.Sort == Sort).ToList();
        }


        public static string GetEng_MeterByShowValue(string Sort, string ShowValue)
        {
            List<Eng_Meter> _Meters = db.Eng_Meter.AsNoTracking().Where(x => x.Sort == Sort && x.ShowValue == ShowValue).ToList();
            if (_Meters != null && _Meters.Count > 0)
            {
                return _Meters[0].SaveValue;
            }
            else
            {
                return ShowValue;
            }
        }

        public static string GetEng_MeterBySaveValue(string Sort, string SaveValue)
        {
            List<Eng_Meter> _Meters = db.Eng_Meter.AsNoTracking().Where(x => x.Sort == Sort && x.SaveValue == SaveValue).ToList();
            if (_Meters != null && _Meters.Count > 0)
            {
                return _Meters[0].ShowValue;
            }
            else
            {
                return SaveValue;
            }
        }

        public static void DeleteEng_MeterOptionItem(string optionName, int row)
        {
            try
            {
                string optionCode = GetMeterOption().Where(x => x.OptionName == optionName).ToList()[0].OptionCode;
                int i = db.Database.ExecuteSqlCommand("delete from Eng_MeterOptionItem where OptionCode ={0} and Row = {1}", optionCode, row);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        #region 电桥配置
        public static void DeleteBas_CompensationValue(int row)
        {
            db.Database.ExecuteSqlCommand("delete from Bas_CompensationValue where Sort = {0}", row);
        }

        public static List<Bas_CompensationValue> SearchBas_CompensationValue()
        {
            return db.Bas_CompensationValue.AsNoTracking().ToList();
        }

        public static void AddBas_CompensationValue(Bas_CompensationValue compensationValue)
        {
            if (SearchBas_CompensationValue().Where(x => x.Sort == compensationValue.Sort).Count() == 0)
            {
                //添加
                int i = db.Database.ExecuteSqlCommand("insert into Bas_CompensationValue(Sort,LcrMinValue,LcrMaxValue,LcrUnitCode,LcrPrecision,LcrCompensationMaxValue,LcrCompensationMinValue,IsEnabled,Creator,CreationDate,Modifier,ModificationDate) values({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11})", compensationValue.Sort, compensationValue.LcrMinValue, compensationValue.LcrMaxValue, compensationValue.LcrUnitCode, compensationValue.LcrPrecision, compensationValue.LcrCompensationMaxValue, compensationValue.LcrCompensationMinValue, compensationValue.IsEnabled, compensationValue.Creator, compensationValue.CreationDate, compensationValue.Modifier, compensationValue.ModificationDate);
            }
            else
            {
                //修改
                int i = db.Database.ExecuteSqlCommand("update Bas_CompensationValue set LcrMinValue = {0},LcrMaxValue = {1},LcrUnitCode = {2},LcrPrecision = {3},LcrCompensationMaxValue = {4},LcrCompensationMinValue = {5},IsEnabled = {6},Modifier = {7},ModificationDate = {8} where Sort = {9}", compensationValue.LcrMinValue, compensationValue.LcrMaxValue, compensationValue.LcrUnitCode, compensationValue.LcrPrecision, compensationValue.LcrCompensationMaxValue, compensationValue.LcrCompensationMinValue, compensationValue.IsEnabled, compensationValue.Modifier, compensationValue.ModificationDate, compensationValue.Sort);
            }
        }
        #endregion

        #region 资料库

        public static List<View_Bas_Material> View_Bas_Material()
        {
            return db.View_Bas_Material.AsNoTracking().ToList();
        }

        /// <summary>
        /// 更新资料库
        /// type 0 添加，删除 1，2 修改
        /// </summary>
        public static void UpdateBas_Material(int type, string customerCode, string materialCode, string materialName, string lcrType, Nullable<decimal> lcrStandardValue, string lcrUnitCode, Nullable<decimal> lcrMaxValue, Nullable<decimal> lcrMinValue, string size, Nullable<decimal> compensateValue, byte[] picture, string remarks, string creator, Nullable<System.DateTime> creationDate, string modifier, Nullable<System.DateTime> modificationDate, string screenPrinting, Nullable<decimal> maxTolerance, Nullable<decimal> minTolerance, string toleranceType, string componentPackaging, out string msg)
        {
            ObjectParameter msgObj = new ObjectParameter("msg", "");
            db.P_Bas_Material
                (type,
                customerCode,
                materialCode,
                materialName,
                lcrType,
                lcrStandardValue,
                lcrUnitCode,
                lcrMaxValue,
                lcrMinValue, size, compensateValue, picture, remarks, creator, creationDate, modifier, modificationDate, screenPrinting, maxTolerance, minTolerance, toleranceType, componentPackaging, msgObj);
            msg = msgObj.Value.ToString();
        }

        #endregion

        #region 替代料
        /// <summary>
        /// 更新替代料
        /// </summary>
        /// <param name="productCode"></param>
        /// <param name="materialCode"></param>
        /// <param name="subMatCode"></param>
        /// <param name="subMatName"></param>
        /// <param name="creater"></param>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        public static void Eng_BomSubAdd(string productCode, string materialCode, string subMatCode, string subMatName, string creater, int type, out string msg)
        {
            msg = "";
            try
            {
                ObjectParameter retVal = new ObjectParameter("retVal", 0);
                ObjectParameter retMsg = new ObjectParameter("retMsg", "");
                db.P_Eng_BomItemSubAdd(productCode, materialCode, subMatCode, subMatName, creater, type, retVal, retMsg);
                if (type == 0 && retVal.Value.ToString() != "1")
                {
                    msg = retMsg.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
        }

        /// <summary>
        /// 搜索替代料
        /// </summary>
        /// <param name="productCode"></param>
        /// <param name="materialCode"></param>
        /// <returns></returns>
        public static List<Eng_BomSub> SearchEng_BomSub(string productCode, string materialCode)
        {
            return db.Eng_BomSub.AsNoTracking().Where(x => x.ProductCode == productCode && x.MaterialCode == materialCode).ToList();
        }

        public static void UpdatateEng_BomSub(string productCode, string materialCode, string subMatCode, string subMatName)
        {
            try
            {
                int i = db.Database.ExecuteSqlCommand("update Eng_BomSub set SubMatName={0}  where ProductCode={1} and MaterialCode = {2} and subMatCode ={3}", subMatName, productCode, materialCode, subMatName);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

        public static void DeleteEng_BomSub(string productCode, string materialCode)
        {
            try
            {
                int i = db.Database.ExecuteSqlCommand("delete from Eng_BomSub where ProductCode={0} and MaterialCode = {1}", productCode, materialCode);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        #region 客户管理
        /// <summary>
        /// 获取客户
        /// </summary>
        /// <returns></returns>
        public static List<Bas_Customer> GetBas_Custom()
        {
            return db.Bas_Customer.AsNoTracking().ToList();
        }

        /// <summary>
        /// 更新客户
        /// </summary>
        /// <param name="customerCode"></param>
        public static void UpdateBas_Custom(int row, string customerCode, string customerName)
        {
            db.Database.ExecuteSqlCommand("update Bas_Customer set CustomerName ={0} , CustomerCode={1} where Row={2}", customerName, customerCode, row);
            db.SaveChanges();
        }

        /// <summary>
        /// 添加客户
        /// </summary>
        /// <param name="customerCode"></param>
        /// <param name="customerName"></param>
        /// <param name="isDefault"></param>
        public static void AddBas_Custom(int row, string customerCode, string customerName)
        {
            db.Database.ExecuteSqlCommand("insert into Bas_Customer(Row,customerCode,customerName) values ({0},{1},{2})", row, customerCode, customerName);
            db.SaveChanges();
        }

        public static void DeleteBas_Custom(int row)
        {
            db.Database.ExecuteSqlCommand("delete from Bas_Customer where Row={0}", row);
            db.SaveChanges();
        }

        public static void AddEng_MeterOptionItem(string optionName, int row, string lcrType, decimal? minValue, string minValueUnit, decimal? maxValue, string maxValueUnit, string functionType, string frequency, string voltage, string rangeType, string range, int? holdTimes, string speed, string resistance, int? readCount, decimal? fRValue, decimal? minValidValue, string minValidValueUnit, decimal? maxValidValue, string maxValidValueUnit, string remarks)
        {
            string optionCode = GetMeterOption().Where(x => x.OptionName == optionName).ToList()[0].OptionCode;
            db.P_Eng_MeterOptionItem(optionCode, row, 1, lcrType, minValue, minValueUnit, maxValue, maxValueUnit, functionType, frequency, voltage, rangeType, range, holdTimes, speed, resistance, readCount, fRValue, minValidValue, minValidValueUnit, maxValidValue, maxValidValueUnit, remarks);
        }
        #endregion

        #region 用户管理

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <returns></returns>
        public static List<Sys_User> GetUser()
        {
            return db.Sys_User.AsNoTracking().ToList();
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="passWord"></param>
        /// <returns></returns>
        public static bool Login(string userCode, string passWord, out string Msg)
        {
            ObjectParameter retVal = new ObjectParameter("retVal", 0);
            ObjectParameter retMsg = new ObjectParameter("retMsg", "");
            db.P_Sys_CheckUser(userCode, passWord, retVal, retMsg);
            Msg = retMsg.Value.ToString();
            if (retVal.Value.ToString() == "1")
            {
                return true;
            }

            return false;
        }

        public static void AddUser(string userCode, string userName, string passWord, string roleCode, string status, string creator)
        {
            db.P_sys_UserAdd(userCode, userName, passWord, roleCode, status, creator);
        }

        public static void DeleteUser(string userCode)
        {
            if (userCode != "0000")
            {
                db.Database.ExecuteSqlCommand("delete from Sys_User where UserCode={0}", userCode);
                db.SaveChanges();
            }

        }

        public static bool ChangePassword(string userCode, string oldPassWord, string newPassWord, out string Msg)
        {
            ObjectParameter retVal = new ObjectParameter("retVal", 0);
            ObjectParameter retMsg = new ObjectParameter("retMsg", "");
            db.P_Sys_ChangePassword(userCode, oldPassWord, newPassWord, retVal, retMsg);
            Msg = retMsg.Value.ToString();
            return retVal.Value.ToString() == "1";
        }

        #endregion

        #region 尺寸管理

        public static List<Man_ComponentSize> GetMan_ComponentSize()
        {
            return db.Man_ComponentSize.AsNoTracking().ToList();
        }

        public static void AddMan_ComponentSize(int SizeId, string SizeCode, int PixelWidth, int PixelHeight, decimal PhysicalSizeLength, decimal PhysicalSizeWidth, bool IsOCR, string Remark)
        {
            if (db.Man_ComponentSize.AsNoTracking().Where(x => x.SizeId == SizeId).ToList().Count == 0)
            {
                int i = db.Database.ExecuteSqlCommand("insert into Man_ComponentSize(SizeCode,PixelWidth,PixelHeight,PhysicalSizeLength,PhysicalSizeWidth,IsOCR,Remark) values ({0},{1},{2},{3},{4},{5},{6})", SizeCode, PixelWidth, PixelHeight, PhysicalSizeLength, PhysicalSizeWidth, IsOCR, Remark);
            }

            else
                UpdateMan_ComponentSize(SizeId, SizeCode, PixelWidth, PixelHeight, PhysicalSizeLength, PhysicalSizeWidth, IsOCR, Remark);
            db.SaveChanges();
        }

        public static void DeleteMan_ComponentSize(int SizeId)
        {
            db.Database.ExecuteSqlCommand("delete from Man_ComponentSize where SizeId={0}", SizeId);
            db.SaveChanges();
        }


        public static void UpdateMan_ComponentSize(int SizeId, string SizeCode, int PixelWidth, int PixelHeight, decimal PhysicalSizeLength, decimal PhysicalSizeWidth, bool IsOCR, string Remark)
        {
            int i = db.Database.ExecuteSqlCommand("update Man_ComponentSize set SizeCode={0},PixelWidth={1},PixelHeight={2},PhysicalSizeLength={3},PhysicalSizeWidth={4},IsOCR={5},Remark={6} where SizeId={7}", SizeCode, PixelWidth, PixelHeight, PhysicalSizeLength, PhysicalSizeWidth, IsOCR, Remark, SizeId);

            db.SaveChanges();
        }
        #endregion
    }
}
