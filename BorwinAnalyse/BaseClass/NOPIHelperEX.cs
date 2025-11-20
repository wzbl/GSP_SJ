using BrowApp.Language;
using NPOI;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BorwinAnalyse.BaseClass
{
    public class NOPIHelperEX
    {

        public class ExcelImageModel
        {
            public ExcelImageModel(int row, int col, System.Drawing.Image image)
            {
                Row = row;
                Col = col;
                Img = image;
            }
            public int Row { get; set; } = 0;
            public int Col { get; set; } = 0;

            public System.Drawing.Image Img;

        }


        /// <summary>
        /// 将Excel导入到Datatable
        /// </summary>
        /// <param name="filePath">excel路径</param>
        /// <param name="isColumnName">第一行是否是列名</param>
        /// <returns>返回datatable</returns>
        public static DataTable ExcelToDataTable(string filePath, bool isColumnName)
        {
            try
            {
                DataTable dataTable = null;
                IWorkbook workbook = null;
                ISheet sheet = null;
                int startRow = 0;
                using (FileStream fs = File.OpenRead(filePath))
                {
                    // 2007版本
                    if (filePath.IndexOf(".xlsx") > 0 || filePath.IndexOf(".XLSX") > 0)
                        workbook = new XSSFWorkbook(fs);
                    // 2003版本
                    else if (filePath.IndexOf(".xls") > 0 || filePath.IndexOf(".XLS") > 0)
                        workbook = new HSSFWorkbook(fs);
                    if (workbook != null)
                    {
                        dataTable = new DataTable();
                        for (int index = 0; index < workbook.NumberOfSheets; index++)
                        {
                            sheet = workbook.GetSheetAt(index);//遍历sheet
                            if (sheet != null)
                            {
                                int rowCount = sheet.LastRowNum;//总行数
                                if (rowCount > 0)
                                {
                                    IRow firstRow = null;
                                    int cellCount = 0;
                                    for (int i = 0; i < sheet.LastRowNum; i++)
                                    {
                                        if (sheet.GetRow(i) != null)//第一行
                                        {
                                            firstRow = sheet.GetRow(i);
                                            cellCount = firstRow.LastCellNum;//列数
                                            break;
                                        }
                                    }
                                    if (cellCount <= 2)
                                    {
                                        cellCount = rowORcolAllCount(filePath, 0, false);
                                    }
                                    cellCount = cellCount <= 2 ? 20 : cellCount;
                                    //构建datatable的列
                                    if (isColumnName)
                                    {
                                        startRow = 1;//如果第一行是列名，则从第二行开始读取
                                        for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                        {
                                            ICell cell = firstRow.GetCell(i);
                                            if (cell != null)
                                            {
                                                cell.SetCellType(CellType.String);
                                                if (cell.StringCellValue != null)
                                                {
                                                    DataColumn column = new DataColumn(cell.StringCellValue);

                                                    if (!dataTable.Columns.Contains("A" + i.ToString()))
                                                    {
                                                        dataTable.Columns.Add("A" + i.ToString());
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (!dataTable.Columns.Contains("A" + i.ToString()))
                                                {
                                                    dataTable.Columns.Add("A" + i.ToString());
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        startRow = sheet.FirstRowNum;
                                        for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                        {
                                            DataColumn column = new DataColumn("A" + i);
                                            if (!dataTable.Columns.Contains("A" + i.ToString()))
                                            {
                                                dataTable.Columns.Add(column);
                                            }
                                        }
                                    }

                                    for (int i = startRow; i <= rowCount; ++i)
                                    {
                                        IRow row = sheet.GetRow(i);
                                        if (row == null) continue; //没有数据的行默认是null　　
                                        if (!IsNullRow(row))
                                        {
                                            continue;
                                        }
                                        DataRow dataRow = dataTable.NewRow();
                                        int CellNum = row.FirstCellNum;
                                        if (CellNum < 0) { CellNum = 0; }
                                        for (int j = CellNum; j < cellCount; j++)
                                        {
                                            if (row.GetCell(j) != null) //同理，没有数据的单元格都默认是null
                                                dataRow[j] = row.GetCell(j).ToString();
                                        }
                                        dataTable.Rows.Add(dataRow);
                                    }
                                }
                            }
                        }
                    }
                }
                return dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.tr());
                return null;
            }
        }


        private static bool IsNullRow(IRow row)
        {
            int fileCount = 0;
            for (int i = row.FirstCellNum; i < row.LastCellNum; i++)
            {
                if (row.GetCell(i) == null)
                {
                    fileCount++;
                }
                else
                {
                    if (string.IsNullOrEmpty(row.GetCell(i).ToString()))
                    {
                        fileCount++;
                    }
                }

            }
            if (fileCount == row.LastCellNum - row.FirstCellNum)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 导入Excel
        /// </summary>
        /// <returns></returns>
        public static DataTable ImportExcel()
        {
            DataTable daNpoi = new DataTable();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "xlsx|*.xlsx|xls表格|*.xls|XLS|*.XLS";
            openFileDialog.RestoreDirectory = true;
            string fileExt = "";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileExt = openFileDialog.FileName;
                if (string.IsNullOrEmpty(fileExt)) return null;
            }
            else return null;
            bool isColumnName = true;
            IWorkbook workbook;

            using (FileStream fs = new FileStream(fileExt, FileMode.Open, FileAccess.Read))
            {
                if (fileExt.IndexOf(".xlsx") > 0 || fileExt.IndexOf(".XLSX") > 0)
                {
                    workbook = new XSSFWorkbook(fs);
                }
                else if (fileExt.IndexOf(".xls") > 0 || fileExt.IndexOf(".XLS") > 0)
                {
                    workbook = new HSSFWorkbook(fs);
                }
                else
                {

                    workbook = null;
                }

                ISheet sheet = null;
                sheet = workbook.GetSheetAt(0);

                IRow header = sheet.GetRow(sheet.FirstRowNum);
                int startRow = 0;
                if (isColumnName)
                {
                    startRow = sheet.FirstRowNum + 1;
                    for (int i = header.FirstCellNum; i < header.LastCellNum; i++)
                    {
                        ICell cell = header.GetCell(i);
                        if (cell != null)
                        {
                            string cellValue = cell.ToString();
                            if (cellValue != null)
                            {
                                DataColumn col = new DataColumn(cellValue);
                                daNpoi.Columns.Add(col);
                            }
                            else
                            {
                                DataColumn col = new DataColumn();
                                daNpoi.Columns.Add(col);
                            }
                        }
                    }
                }

                for (int i = startRow; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null)
                    {
                        continue;
                    }
                    DataRow dr = daNpoi.NewRow();
                    for (int j = row.FirstCellNum; j < row.LastCellNum; j++)
                    {

                        if (row.GetCell(j) != null)
                        {
                            dr[j] = row.GetCell(j).ToString();
                        }
                    }
                    daNpoi.Rows.Add(dr);
                }
            }
            ImportSuccessTips(fileExt);
            return daNpoi;
        }

        /// <summary>
        /// 导入Excel
        /// </summary>
        /// <returns></returns>
        public static DataTable ImportExcel(string filePath)
        {
            DataTable daNpoi = new DataTable();
            //OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.Multiselect = false;
            //openFileDialog.Filter = "xlsx|*.xlsx|xls表格|*.xls|XLS|*.XLS";
            //openFileDialog.RestoreDirectory = true;
            //string fileExt = "";
            //if (openFileDialog.ShowDialog() == DialogResult.OK)
            //{
            //    fileExt = openFileDialog.FileName;
            //    if (string.IsNullOrEmpty(fileExt)) return null;
            //}
            //else return null;
            string fileExt = filePath;
            bool isColumnName = true;
            IWorkbook workbook;

            using (FileStream fs = new FileStream(fileExt, FileMode.Open, FileAccess.Read))
            {
                if (fileExt.IndexOf(".xlsx") > 0 || fileExt.IndexOf(".XLSX") > 0)
                {
                    workbook = new XSSFWorkbook(fs);
                }
                else if (fileExt.IndexOf(".xls") > 0 || fileExt.IndexOf(".XLS") > 0)
                {
                    workbook = new HSSFWorkbook(fs);
                }
                else
                {

                    workbook = null;
                }

                ISheet sheet = null;
                sheet = workbook.GetSheetAt(0);

                IRow header = sheet.GetRow(sheet.FirstRowNum);
                int startRow = 0;
                if (isColumnName)
                {
                    startRow = sheet.FirstRowNum + 1;
                    for (int i = header.FirstCellNum; i < header.LastCellNum; i++)
                    {
                        ICell cell = header.GetCell(i);
                        if (cell != null)
                        {
                            string cellValue = cell.ToString();
                            if (cellValue != null)
                            {
                                DataColumn col = new DataColumn(cellValue);
                                daNpoi.Columns.Add(col);
                            }
                            else
                            {
                                DataColumn col = new DataColumn();
                                daNpoi.Columns.Add(col);
                            }
                        }
                    }
                }

                for (int i = startRow; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null)
                    {
                        continue;
                    }
                    DataRow dr = daNpoi.NewRow();
                    for (int j = row.FirstCellNum; j < row.LastCellNum; j++)
                    {

                        if (row.GetCell(j) != null)
                        {
                            dr[j] = row.GetCell(j).ToString();
                        }
                    }
                    daNpoi.Rows.Add(dr);
                }
            }
            //ImportSuccessTips(fileExt);
            return daNpoi;
        }

        public static List<ExcelImageModel> GetExcelPicture(string path)
        {
            List<ExcelImageModel> ListExcelImage = new List<ExcelImageModel>();

            string fileExt = path;

            IWorkbook workbook;
            using (FileStream fs = new FileStream(fileExt, FileMode.Open, FileAccess.Read))
            {
                if (fileExt.IndexOf(".xlsx") > 0 || fileExt.IndexOf(".XLSX") > 0)
                {
                    workbook = new XSSFWorkbook(fs);
                }
                else if (fileExt.IndexOf(".xls") > 0 || fileExt.IndexOf(".XLS") > 0)
                {
                    workbook = new HSSFWorkbook(fs);
                }
                else
                {
                    workbook = null;
                }
                // 处理图片
                if (workbook is XSSFWorkbook xssfWorkbook)
                {

                    XSSFSheet sheet = (XSSFSheet)workbook.GetSheetAt(0); // 获取第一个工作表
                    List<XSSFPicture> listPic = new List<XSSFPicture>();
                    // 获取工作表中的所有图片
                    foreach (POIXMLDocumentPart part in sheet.GetRelations())
                    {
                        if (part is XSSFDrawing drawing)
                        {
                            foreach (XSSFShape shape in drawing.GetShapes())
                            {
                                if (shape is XSSFPicture picture)
                                {
                                    listPic.Add(picture);
                                }
                            }
                        }
                    }

                    //var pictures = xssfWorkbook.GetAllPictures();
                    foreach (var picture in listPic)
                    {
                        //XSSFPictureData xSSFPictureData = picture as XSSFPictureData;
                        // 获取图片的位置信息
                        var shape = (XSSFShape)picture;
                        XSSFPictureData xSSFPictureData;
                        if (shape.GetAnchor() is XSSFClientAnchor anchor)
                        {
                            int rowIndex = anchor.Row1; // 图片所在行
                            int colIndex = anchor.Col1; // 图片所在列

                            // 将图片转换为 Bitmap
                            xSSFPictureData = (XSSFPictureData)picture.PictureData;
                            using (MemoryStream ms = new MemoryStream(xSSFPictureData.Data))
                            {
                                System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                                ExcelImageModel excelImageModel = new ExcelImageModel(rowIndex, colIndex, img);
                                ListExcelImage.Add(excelImageModel);
                            }
                        }
                        xSSFPictureData = null;
                    }
                }
                else if (workbook is HSSFWorkbook hssfWorkbook)
                {
                    //HSSFSheet sheet = (HSSFSheet)workbook.GetSheetAt(0); // 获取第一个工作表
                    //List<HSSFPicture> listPic = new List<HSSFPicture>();
                    //// 获取工作表中的所有图片
                    //foreach (POIXMLDocumentPart part in sheet.GetRelations())
                    //{
                    //    if (part is XSSFDrawing drawing)
                    //    {
                    //        foreach (HSSFShape shape in drawing.GetShapes())
                    //        {
                    //            if (shape is HSSFPicture picture)
                    //            {
                    //                listPic.Add(picture);
                    //            }
                    //        }
                    //    }
                    //}

                    //foreach (var picture in listPic)
                    //{
                    //    // 获取图片的位置信息
                    //    var shape = (HSSFShape)picture;
                    //    if (shape.Anchor is HSSFClientAnchor anchor)
                    //    {
                    //        int rowIndex = anchor.Row1; // 图片所在行
                    //        int colIndex = anchor.Col1; // 图片所在列

                    //        // 将图片转换为 Bitmap
                    //        using (MemoryStream ms = new MemoryStream(hSSFPictureData.Data))
                    //        {
                    //            System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                    //            ExcelImageModel excelImageModel = new ExcelImageModel(rowIndex, colIndex, img);
                    //            ListExcelImage.Add(excelImageModel);
                    //        }
                    //    }
                    //    hSSFPictureData = null;
                    //}
                }

                return ListExcelImage;
            }
        }



        /// <summary>
        /// 读取excel某个工作表的有效行数或者最大有效列数
        /// </summary>
        /// <param name="filePath">代表excel表格保存的地址</param>
        /// <param name="sheetNumber">代表将要读取的sheet表的索引位置</param>
        /// <param name="readFlag">为true代表读取的为：有效行数，为：false代表读取的为：最大有效列数</param>
        /// <returns>返回值 “不为-1” 代表读取成功，否则为读取失败</returns>
        private static int rowORcolAllCount(string filePath, int sheetNumber, bool readFlag = false)
        {
            try
            {
                int rowORcolCnt = -1;//初始化为-1
                FileStream fs = null;
                IWorkbook workbook = null;
                ISheet sheet = null;
                using (fs = File.OpenRead(filePath))
                {
                    // 2007版本
                    if (filePath.IndexOf(".xlsx") > 0)
                        workbook = new XSSFWorkbook(fs);
                    // 2003版本
                    else if (filePath.IndexOf(".xls") > 0)
                        workbook = new HSSFWorkbook(fs);
                    sheet = workbook.GetSheetAt(sheetNumber);
                    if (sheet != null)
                    {
                        if (readFlag)//如果需要读取‘有效行数’
                        {
                            rowORcolCnt = sheet.LastRowNum + 1;//有效行数(NPOI读取的有效行数不包括列头，所以需要加1)
                        }
                        else  //如果需要读取‘最大有效列数’
                        {
                            for (int rowCnt = sheet.FirstRowNum; rowCnt <= sheet.LastRowNum; rowCnt++)//迭代所有行
                            {
                                IRow row = sheet.GetRow(rowCnt);
                                if (row != null && row.LastCellNum > rowORcolCnt)
                                {
                                    rowORcolCnt = row.LastCellNum;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return -1;
        }

        public static DataTable ToDataTable(DataGridView myDGV)
        {
            DataTable dt = new DataTable();
            for (int i = 0; i < myDGV.ColumnCount; i++)
            {
                dt.Columns.Add(myDGV.Columns[i].HeaderText);
            }
            //写入数值
            for (int r = 0; r < myDGV.Rows.Count; r++)
            {
                List<object> values = new List<object>();
                for (int i = 0; i < myDGV.ColumnCount; i++)
                {
                    values.Add(myDGV.Rows[r].Cells[i].Value);
                }
                dt.Rows.Add(values.ToArray());
            }
            return dt;
        }


        /// <summary>
        /// 导出dataTable
        /// </summary>
        /// <param name="TableName"></param>
        public static void ExportDataToExcel(DataTable TableName)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            //设置文件标题
            saveFileDialog.Title = "导出Excel文件".tr();
            //设置文件类型
            saveFileDialog.Filter = "Excel (*.xlsx)|*.xlsx|Excel 97-2003 (*.xls)|*.xls";
            //设置默认文件类型显示顺序  
            saveFileDialog.FilterIndex = 1;
            //是否自动在文件名中添加扩展名
            saveFileDialog.AddExtension = true;
            //是否记忆上次打开的目录
            saveFileDialog.RestoreDirectory = true;

            //按下确定选择的按钮  
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                //获得文件路径 
                string localFilePath = saveFileDialog.FileName.ToString();

                //数据初始化
                int TotalCount;     //总行数
                int RowRead = 0;    //已读行数
                int Percent = 0;    //百分比

                TotalCount = TableName.Rows.Count;

                //NPOI
                IWorkbook workbook;
                string FileExt = Path.GetExtension(localFilePath).ToLower();
                if (FileExt == ".xlsx")
                {
                    workbook = new XSSFWorkbook();
                }
                else if (FileExt == ".xls")
                {
                    workbook = new HSSFWorkbook();
                }
                else
                {
                    workbook = null;
                }
                if (workbook == null)
                {
                    return;
                }
                ISheet sheet = workbook.CreateSheet("Sheet1");


                //秒钟
                Stopwatch timer = new Stopwatch();
                timer.Start();

                try
                {
                    //读取标题  
                    IRow rowHeader = sheet.CreateRow(0);
                    for (int i = 0; i < TableName.Columns.Count; i++)
                    {
                        ICell cell = rowHeader.CreateCell(i);
                        cell.SetCellValue(TableName.Columns[i].ColumnName);
                    }

                    //读取数据  
                    for (int i = 0; i < TableName.Rows.Count; i++)
                    {
                        IRow rowData = sheet.CreateRow(i + 1);
                        for (int j = 0; j < TableName.Columns.Count; j++)
                        {
                            ICell cell = rowData.CreateCell(j);
                            cell.SetCellValue(TableName.Rows[i][j].ToString());
                        }
                        //状态栏显示
                        RowRead++;
                        Percent = (int)(100 * RowRead / TotalCount);

                        System.Windows.Forms.Application.DoEvents();
                    }

                    System.Windows.Forms.Application.DoEvents();

                    //转为字节数组  
                    MemoryStream stream = new MemoryStream();
                    workbook.Write(stream);
                    var buf = stream.ToArray();

                    //保存为Excel文件  
                    using (FileStream fs = new FileStream(localFilePath, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(buf, 0, buf.Length);
                        fs.Flush();
                        fs.Close();
                    }

                    System.Windows.Forms.Application.DoEvents();

                    //关闭秒钟
                    timer.Reset();
                    timer.Stop();

                    //成功提示
                    ExportSuccessTips(localFilePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "提示".tr(), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {
                    //关闭秒钟
                    timer.Reset();
                    timer.Stop();
                }
            }
        }

        /// <summary>
        /// 判断内容是否是数字
        /// </summary>
        /// <param name="message"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool isNumeric(string message, out double result)
        {
            Regex rex = new Regex(@"^[-]?\d+[.]?\d*$");
            result = -1;
            if (rex.IsMatch(message))
            {
                result = double.Parse(message);
                return true;
            }
            else
                return false;
        }


        //导出成功提示
        private static void ExportSuccessTips(string filePathAndName)
        {
            if (string.IsNullOrEmpty(filePathAndName)) return;

            MessageBox.Show("导出成功！".tr(), "提示".tr(), MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (MessageBox.Show("保存成功，是否打开文件？".tr(), "提示".tr(), MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start(filePathAndName);
            }
        }

        //导入成功提示
        private static void ImportSuccessTips(string filePathAndName)
        {
            if (string.IsNullOrEmpty(filePathAndName)) return;

            MessageBox.Show("导入成功！".tr(), "提示".tr(), MessageBoxButtons.OK, MessageBoxIcon.Information);

        }


        /// <summary>
        /// 创建Excel
        /// </summary>
        /// <param name="excelPath"></param>
        /// <param name="cellTitle"></param>
        public static void CreateExcelData(string excelName, List<string> cellTitle)
        {
            XSSFWorkbook workbook;
            ISheet sheet;
            int cellId = 0;
            FileStream fout;

            IRow row;

            ICell cell = null;
            ICellStyle style = null;

            //string excelPath = "D:\\HistoryData\\" + DateTime.Now.ToString("yyyy-MM");

            //if (!Directory.Exists(excelPath)) {
            //    Directory.CreateDirectory(excelPath);
            //}

            //string excelName = excelPath + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";

            if (!File.Exists(excelName))
            {
                workbook = new XSSFWorkbook();
                sheet = workbook.CreateSheet("sheet0");

                row = sheet.CreateRow(0);
                //设置行高
                row.Height = 2 * 256;

                cellId = 0;
                foreach (string data in cellTitle)
                {
                    cell = row.CreateCell(cellId);
                    cell.SetCellValue(data);
                    //设置居中
                    style = workbook.CreateCellStyle();
                    style.VerticalAlignment = VerticalAlignment.Center;
                    style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    cell.CellStyle = style;
                    cellId++;
                }

                fout = new FileStream(excelName, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite);//写入fout.Flush();
                workbook.Write(fout);//写入文件
                workbook = null;
                fout.Close();
            }
        }

        /// <summary>
        /// 插入数据到Excel
        /// </summary>
        /// <param name="excelName"></param>
        /// <param name="cellData"></param>
        public static void AddExcelData(string excelName, List<string> cellData, List<string> pictureList)
        {
            FileStream fs;
            XSSFWorkbook workbook;
            ISheet sheet;
            int cellId = 0;
            FileStream fout;

            IRow row;

            ICell cell = null;
            ICellStyle style = null;

            fs = new FileStream(excelName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);                        //读取流POIFSFileSystem ps = new POIFSFileSystem(fs),//需using NPOI.POIFS.FileSystem;
            workbook = new XSSFWorkbook(fs);
            sheet = workbook.GetSheetAt(0);                                                                             //获取工作表

            int rowIndex = sheet.LastRowNum + 1;                                                                        // 最后一行
            row = sheet.CreateRow(rowIndex);
            //设置行高
            row.Height = 2 * 256;

            style = workbook.CreateCellStyle();
            style.VerticalAlignment = VerticalAlignment.Center;
            style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;

            cellId = 0;
            foreach (string data in cellData)
            {
                cell = row.CreateCell(cellId);
                cell.SetCellValue(data);
                //设置居中
                cell.CellStyle = style;
                cellId++;
            }

            if (pictureList.Count > 0)
            {
                for (int k = 0; k < pictureList.Count; k++)
                {
                    if (File.Exists(pictureList[k]))
                    {
                        Bitmap bmp = new Bitmap(pictureList[k]);
                        using (MemoryStream ms = new MemoryStream())
                        {
                            cell = row.CreateCell(cellId);
                            //设置居中
                            cell.CellStyle = style;
                            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                            byte[] bytes = ms.ToArray();
                            int pictureIdx = 0;
                            sheet.SetColumnWidth(cellId, 5000);
                            sheet.GetRow(rowIndex).Height = (short)(3000);
                            pictureIdx = workbook.AddPicture(bytes, NPOI.SS.UserModel.PictureType.JPEG); //添加图片
                            XSSFClientAnchor anchor = new XSSFClientAnchor(100, 100, 0, 0, cellId, rowIndex, cellId + 1, rowIndex + 1);
                            anchor.AnchorType = AnchorType.MoveDontResize;
                            XSSFPicture pict = (XSSFPicture)sheet.CreateDrawingPatriarch().CreatePicture(anchor, pictureIdx);
                            pict.Resize(0.9, 0.9);

                            //ICreationHelper helper = workbook.GetCreationHelper();
                            //IDrawing drawing = sheet.CreateDrawingPatriarch();
                            //IClientAnchor anchor_ = helper.CreateClientAnchor();
                            //anchor_.Dx1 = 100;
                            //anchor_.Dy1 = 100;
                            //anchor_.Col1 = cellId;
                            //anchor_.Row1 = rowIndex;
                            //anchor_.Col2 = cellId + 1;
                            //anchor_.Row2 = rowIndex + 1;
                            //IPicture picture = drawing.CreatePicture(anchor_, pictureIdx);
                            //picture.Resize(0.9, 0.9);
                        }


                        bmp.Dispose();
                    }
                    cellId++;
                }
            }

            fout = new FileStream(excelName, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);                     //写入fout.Flush();
            workbook.Write(fout);                                                                                       //写入文件
            workbook = null;
            fout.Close();

        }
    }
}
