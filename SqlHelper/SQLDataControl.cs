using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace SqlHelper
{
    public class SQLDataControl
    {
        private static FAI_NewEntities db = new FAI_NewEntities();

        /// <summary>
        /// 获取所有程序
        /// </summary>
        /// <returns></returns>
        public static List<View_Eng_Program> GetAllProgramm()
        {
            return db.View_Eng_Program.AsNoTracking().ToList();
        }

        public static void AddProgram(string productCode, string programName, string customerCode, string boardSide, string creator, string ruleCode)
        {
            db.P_Insert_Eng_Program(productCode, programName, customerCode, boardSide, ruleCode, "NORMAL", "", creator, creator);
        }

        public static void DeleteProgram(string productCode)
        {
            int i = db.Database.ExecuteSqlCommand("DELETE FROM Eng_Program WHERE ProductCode = {0}", productCode);
            DeleteBom(productCode);
            DeleteXYData(productCode);
            db.SaveChanges();
        }

        public static List<Man_Report> GetAllReport()
        {
            return db.Man_Report.AsNoTracking().ToList();
        }

        /// <summary>
        /// 搜索BOM
        /// </summary>
        /// <param name="productCode"></param>
        /// <returns></returns>
        public static List<P_Search_Eng_Bom_Result> SearchBom(string productCode)
        {
            return db.P_Search_Eng_Bom(productCode).ToList();
        }

        public static void AddBom(Eng_Bom bom)
        {
            db.Eng_Bom.Add(bom);
            db.SaveChanges();
        }

        public static void DeleteBom(string productCode)
        {
            int i = db.Database.ExecuteSqlCommand("DELETE FROM Eng_Bom WHERE ProductCode = {0}", productCode);
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

        public static List<P_Search_Eng_XYData_Result> SearchXYData(string productCode)
        {
            return db.P_Search_Eng_XYData(productCode).ToList();
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


        /// <summary>
        /// 获取所有公共模板库
        /// </summary>
        /// <returns></returns>
        public static List<Eng_PubModel> GetAllEng_PubModel()
        {
            return db.Eng_PubModel.AsNoTracking().ToList();
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
    }
}
