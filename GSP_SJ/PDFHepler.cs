using iTextSharp.text;
using iTextSharp.text.pdf;
//using PdfSharp;
//using PdfSharp.Charting;
//using PdfSharp.Drawing;
//using PdfSharp.Fonts;
//using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;

namespace GSP_SJ
{
    public class PDFHepler
    {


        /// <summary>
        /// 生成PDF
        /// </summary>
        /// <param name="savePath">保存路径</param>
        /// <param name="title">标题</param>
        /// <param name="pDFHeaders">报告汇总</param>
        /// <param name="bigImage">大拼图路径</param>
        /// <param name="InspectionItem_headers">明细头</param>
        /// <param name="inspectionItems">明细列</param>
        public static void ExportPDF(string savePath, string Title, List<PDFHeader> pDFHeaders, string bigImagePath, string[] InspectionItem_headers, List<InspectionItem> inspectionItems)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                Document doc = new Document(PageSize.A4, 35, 35, 25, 25);

                using (var fs = new FileStream(savePath, FileMode.Create))
                {
                    PdfWriter.GetInstance(doc, fs);
                    doc.Open();

                    // 中文字体
                    BaseFont bfChinese = BaseFont.CreateFont("C:/Windows/Fonts/simsun.ttc,1", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                    Font fontTitle = new Font(bfChinese, 16, Font.BOLD);
                    Font fontHead = new Font(bfChinese, 11, Font.BOLD);
                    Font fontText = new Font(bfChinese, 9, Font.NORMAL);
                    Font fontDescription = new Font(bfChinese, 7, Font.NORMAL);

                    // 标题
                    Paragraph title = new Paragraph(Title, fontTitle);
                    title.Alignment = Element.ALIGN_CENTER;
                    title.SpacingAfter = 12;
                    doc.Add(title);

                    Paragraph description = new Paragraph("PASS类型:M:人工;R:OCR;V:OCV;B:批量(抽检)", fontDescription);
                    description.Alignment = Element.ALIGN_RIGHT;
                    description.SpacingAfter = 5;
                    doc.Add(description);

                    // 头部信息
                    PdfPTable tableHead = new PdfPTable(6);
                    tableHead.WidthPercentage = 100;
                    for (int i = 0; i < pDFHeaders.Count; i++)
                    {
                        AddHeadCell(tableHead, pDFHeaders[i].Title, pDFHeaders[i].Value, fontHead, fontText);
                    }

                    doc.Add(tableHead);

                    // 大图A
                    Paragraph pA = new Paragraph("一、PCB整板大图", fontHead);
                    pA.SpacingBefore = 10;
                    doc.Add(pA);

                    if (bigImagePath != null && File.Exists(bigImagePath))
                    {
                        Image imageA = Image.GetInstance(bigImagePath);
                        imageA.ScaleToFit(530, 220);
                        imageA.Alignment = Element.ALIGN_CENTER;
                        doc.Add(imageA);
                    }
                    else
                    {
                        // 没有图片
                    }

                    // 检测明细表格（核心：标准图、实测图嵌入列中）
                    Paragraph pTable = new Paragraph("二、元件检测明细", fontHead);
                    pTable.SpacingBefore = 10;
                    pTable.SpacingAfter = 8;
                    doc.Add(pTable);

                    // 列宽（适配图片）
                    float[] widths = { 30, 50, 50, 30, 80, 120, 100, 100, 100, 60, 80 };
                    PdfPTable tableData = new PdfPTable(widths);
                    tableData.WidthPercentage = 100;
                    tableData.HeaderRows = 0; //只在第一页显示表头，
                    //tableData.HeaderRows = 1; //（每页都显示表头）

                    // 表头

                    foreach (string h in InspectionItem_headers)
                    {
                        PdfPCell cell = new PdfPCell(new Paragraph(h, fontHead));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        tableData.AddCell(cell);
                    }

                    // 表格数据（每行都带标准图B + 实测图B）
                    foreach (var item in inspectionItems)
                    {
                        AddCell(tableData, item.Seq.ToString(), fontText);
                        AddCell(tableData, item.Position, fontText);
                        // ==========================================
                        // 🔥 结果列：自动变色（PASS绿，FAIL红）
                        // ==========================================
                        Font resultFont = fontText;
                        if (item.Result == null)
                        {
                            AddCell(tableData, "", resultFont); // 👈 用带颜色的字体
                        }
                        else
                        {
                            if (item.Result.Equals("PASS", StringComparison.OrdinalIgnoreCase))
                            {
                                resultFont = new Font(fontText.BaseFont, fontText.Size, Font.NORMAL, BaseColor.GREEN);
                            }
                            else if (item.Result.Equals("FAIL", StringComparison.OrdinalIgnoreCase))
                            {
                                resultFont = new Font(fontText.BaseFont, fontText.Size, Font.NORMAL, BaseColor.RED);
                            }
                            AddCell(tableData, item.Result, resultFont); // 👈 用带颜色的字体
                        }

                        AddCell(tableData, item.Mount, fontText);
                        AddCell(tableData, item.PartNo, fontText);
                        AddCell(tableData, item.Desc, fontText);

                        // ========== 核心：标准列放标准图B ==========
                     
                        AddImageCell(tableData, item.StandardImage, 60, 40);
                        // ========== 核心：实测列放实测图B ==========
                        AddImageCell(tableData, item.ActualImage, 60, 40);
                        AddCell(tableData, item.Remark, fontText);
                        AddCell(tableData, item.CheckUser, fontText);
                        AddCell(tableData, item.CheckTime, fontText);
                    }

                    doc.Add(tableData);
                    doc.Close();
                }
                ExportSuccessTips(savePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //导出成功提示
        private static void ExportSuccessTips(string filePathAndName)
        {
            if (string.IsNullOrEmpty(filePathAndName)) return;

            MessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (MessageBox.Show("保存成功，是否打开文件？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start(filePathAndName);
            }
        }

        #region 工具方法
        static void AddHeadCell(PdfPTable table, string k, string v, Font fk, Font fv)
        {
            PdfPCell ck = new PdfPCell(new Paragraph(k, fk));
            ck.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(ck);
            table.AddCell(new PdfPCell(new Paragraph(v, fv)));
        }

        static void AddCell(PdfPTable table, string txt, Font f)
        {
            PdfPCell cell = new PdfPCell(new Paragraph(txt ?? "", f));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(cell);
        }

        // 给表格添加图片（标准图B、实测图B）
        static void AddImageCell(PdfPTable table, Image img, float w, float h)
        {
            PdfPCell cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Padding = 2;

            if (img != null)
            {
                //Image img = Image.GetInstance(imgPath);
                img.ScaleToFit(w, h);
                img.Alignment = Element.ALIGN_CENTER;
                cell.AddElement(img);
            }
            else
            {
                cell.AddElement(new Paragraph("无图", new Font(BaseFont.CreateFont(), 8)));
            }

            table.AddCell(cell);
        }
        #endregion


    }
    // 检测项实体（对应你的报告数据）
    public class InspectionItem
    {
        public int Seq { get; set; }
        public string Position { get; set; }
        public string Result { get; set; }
        public string Mount { get; set; }
        public string PartNo { get; set; }
        public string Desc { get; set; }
        public Image StandardImage { get; set; } // 标准图
        public Image ActualImage { get; set; }   // 实测图
        /// <summary>
        /// 检查备注
        /// </summary>
        public string Remark { get; set; }
        public string CheckUser { get; set; }
        public string CheckTime { get; set; }
    }


    /// <summary>
    /// PDF报告头
    /// </summary>
    public class PDFHeader
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
    }

}




