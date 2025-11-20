using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
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
            return db.View_Eng_Program.ToList();
        }

        public static void AddProgram(string productCode, string programName, string customerCode, string boardSide, string creator)
        {
            if (db.Eng_Program.Where(x => x.ProductCode == productCode).ToList().Count == 0)
            {
                db.Eng_Program.Add(new Eng_Program()
                {
                    ProductCode = productCode,
                    ProductName = programName,
                    CustomerCode = customerCode,
                    BoardSide = boardSide,
                    Creator = creator,
                    CreationDate = DateTime.Now
                });
            }
            else
            {
                db.Eng_Program.Where(x => x.ProductCode == productCode).ToList().ForEach(x =>
                {
                    x.ProductName = programName;
                    x.CustomerCode = customerCode;
                    x.BoardSide = boardSide;
                    x.Modifier = creator;
                    x.ModificationDate = DateTime.Now;
                });
            }
            db.SaveChanges();
        }

        public static void DeleteProgram(string productCode)
        {
            db.Eng_Program.Where(x => x.ProductCode == productCode).ToList().ForEach(x =>
            {
                db.Eng_Bom.Where(y => y.ProductCode == x.ProductCode).ToList().ForEach(y =>
                {
                    db.Eng_Bom.Remove(y);
                });
                db.Eng_Program.Remove(x);

                db.Eng_XYData.Where(y => y.ProductCode == x.ProductCode).ToList().ForEach(y =>
                {
                    db.Eng_XYData.Remove(y);
                });
            });
            db.SaveChanges();
        }

        public static List<Man_Report> GetAllReport()
        {
             return db.Man_Report.ToList();
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

        public static void InsertBom(string productCode, Nullable<int> row, string materialCode, string materialName, Nullable<int> quantity, string position, string allowSubstitute, string lcrType, Nullable<decimal> lcrStandardValue, string lcrUnitCode, Nullable<decimal> lcrMaxValue, Nullable<decimal> lcrMinValue, string size, string remarks, string creator, string modifier, Nullable<decimal> maxTolerance, Nullable<decimal> minTolerance, string status, string screenPrinting, string toleranceType, string componentPackaging)
        {
            db.P_Insert_Eng_Bom(productCode, row, materialCode, materialName, quantity, position, allowSubstitute, lcrType, lcrStandardValue, lcrUnitCode, lcrMaxValue, lcrMinValue, size, remarks, creator, modifier, maxTolerance, minTolerance, status, screenPrinting, toleranceType, componentPackaging);
        }


        public static void InsertEngBom(string productCode,
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
                    CreationDate = DateTime.Now,
                    MaxTolerance = maxTolerance,
                    MinTolerance = minTolerance,
                    ToleranceType = toleranceType,
                    Row = row
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
                    x.Row = row;
                });
            }
            db.SaveChanges();
        }

        public static List<P_Search_Eng_XYData_Result> SearchXYData(string productCode)
        {
            return db.P_Search_Eng_XYData(productCode).ToList();
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
            db.Eng_XYData.Where(x => x.ProductCode == productCode && x.Position == position).ToList().ForEach(x =>
            {
                x.RX = px; x.RY = py;
            });
            db.SaveChanges();
        }

        public static void UpdateProgramOptionPicture(string productCode, string BoardSide, byte[] img)
        {
            if (db.Eng_ProgramOption.Where(x => x.ProductCode == productCode).ToList().Count == 0)
            {
                db.Eng_ProgramOption.Add(new Eng_ProgramOption()
                {
                    ProductCode = productCode,
                    BoardSide = BoardSide,
                    Picture = img
                });
            }
            else
            {
                db.Eng_ProgramOption.Where(x => x.ProductCode == productCode).ToList().ForEach(x =>
                {
                    x.Picture = img;
                });
            }
            db.SaveChanges();
        }

        public static byte[] GetProgramOptionPicture(string productCode)
        {
            var pic = db.Eng_ProgramOption.Where(x => x.ProductCode == productCode).ToList();
            if (pic.Count > 0)
            {
                return pic[0].Picture;
            }
            else
            {
                return null;
            }
        }
    }
}
