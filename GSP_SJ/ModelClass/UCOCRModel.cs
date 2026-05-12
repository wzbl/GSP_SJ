using Browocrlib;
using Emgu.CV;
using Emgu.CV.Structure;
using HalconDotNet;
using iTextSharp.text.pdf.codec;
using SqlHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Migrations.Model;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSP_SJ.ModelClass
{
    public partial class UCOCRModel : UserControl
    {
        public UCOCRModel(Image image, DirectionalROI rOI)
        {
            InitializeComponent();
            Dock = DockStyle.Top;
            rOI.ChangeROI += RefreshData;
        }

        HObject ho_image;

        private void RefreshData(Image image, DirectionalROI rOI)
        {
            using (HImage hImage = HalconImageConverter.BitmapToHImageRGB(new Bitmap(image)))
            {
                HOperatorSet.Rgb1ToGray(hImage, out HObject gray);
                //执行ocv

                HOperatorSet.GetImageSize(gray, out HTuple width, out HTuple height);

                // 设置窗口的显示部分为整个图像
                hWindowControl1.HalconWindow.SetPart(0, 0, height.D - 1, width.D - 1);

                PublicFunction.CreateHShapeModel(hWindowControl1, new HImage(gray), rOI.Bounds, out HXLDCont modelContour);
                if (modelContour == null)
                {
                    hWindowControl1.Visible = false;
                }
                else
                {
                    hWindowControl1.Visible = true;
                    hWindowControl1.HalconWindow.ClearWindow();
                    hWindowControl1.HalconWindow.DispObj(PublicFunction.GetCorrectedContour(modelContour));
                }
                txtOCRText.Text = "";
                HObject ho_findCircleRoi;
                //执行ocr
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    HOperatorSet.GenRectangle1(out ho_findCircleRoi, rOI.Top, rOI.Left,
                    rOI.Bottom, rOI.Right);
                }
                HOperatorSet.ReduceDomain(gray, ho_findCircleRoi, out ho_image);
                HOperatorSet.ChangeDomain(ho_image, ho_findCircleRoi, out HObject ho_ImageReduced2);
                HOperatorSet.CropDomain(ho_ImageReduced2, out HObject ho_ImageReduced3);
                hWindowControl2.HalconWindow.ClearWindow();
                FitImage(hWindowControl2.HalconWindow, ho_ImageReduced3);
                GetOCRText(ho_ImageReduced3);
                ho_ImageReduced2.Dispose();
                ho_ImageReduced3.Dispose();
                ho_findCircleRoi.Dispose();
                gray.Dispose();

            }
        }
        private void FitImage(HWindow hWin, HObject hImg)
        {
            //适应图像算法：
            //  该算法本质是让窗口跟随图像大小而改变，从而显示出完整的图像。但因为窗口大小是固定的，为了显示完整的图像，只能在保证图像宽高比的前提下根据窗口将图像进行缩放。
            //  已知：窗口的宽为L1,高为H1，图像的宽为L2，高为H2。假设从窗口左下角到右上角画一条斜线，则该线的斜率就表示窗口的高/宽比，记作K1,则K1=H1/L1，同理，图像的高宽比记作K2，K2=H2/L2。
            //  根据K1和K2的值相比较，明显可知，当K1>K2时，为了显示完整的图像(图像宽高按照1：1比例)，需要将图像的宽度根据窗口的宽度进行缩放，而图像的高度则根据图像的宽高比进行计算，以达到在窗口中显示完整的图像
            //  当K1>K2，使图像宽度=窗口宽度，则图像高度根据窗口高宽比进行计算：
            //      L2 = L1  
            //      H2 = K2*L2
            //      为了使图像显示在窗口的中心位置，则图像的起始坐标计算为：X1=0, Y1=(H1-H2)/2，终点坐标为：X2=L1-1, Y2=H1-Y1 => Y2=H1-((H1-H2)/2)
            //  当K1<K2，使图像高度=窗口高度，则图像宽度根据窗口高宽比进行计算：
            //      H2 = H1  
            //      L2 = H2/K2
            // 为了使图像显示在窗口的中心位置，则图像的起始坐标计算为：X1=(L1-L2)/2, Y1=0，终点坐标为：X2=L1-X1, Y2=H1-1

            HTuple imgWidth, imgHeight;//图像宽度和高度
            double ratio_win, ratio_img;//窗口比例，图像比例
            int beginRow, endRow, beginCol, endCol;//图像在窗口上显示的起始行，结束行，起始列，结束列位置
            try
            {
                if (hImg != null)
                {
                    HOperatorSet.GetImageSize(hImg, out imgWidth, out imgHeight);//获取图像的尺寸
                    ratio_win = (double)hWindowControl2.WindowSize.Width / (double)hWindowControl2.WindowSize.Height;//计算窗口宽高比例
                    ratio_img = (double)imgWidth / (double)imgHeight;//计算图像宽高比例
                    //窗口适应图像算法：保证按照图像宽/高比例显示图像
                    if (ratio_win >= ratio_img)
                    {
                        //如果窗口的宽高比大于图像的宽高比，则将图像按照【窗口高度】进行整体缩放，并调整图像的显示位置
                        //保证图像高度和窗口高度一致，图像宽度则根据图像的宽高比进行缩放
                        //图像的起始行位置位于窗口顶端(row=0),起始列位置按照窗口和缩放后的图像计算:(窗口宽度- 缩放后图像宽度)/2
                        beginRow = 0;
                        endRow = (int)imgHeight.D - 1;
                        beginCol = (int)(-imgWidth.D * (ratio_win / ratio_img - 1d) / 2d);
                        endCol = (int)(imgWidth.D + imgWidth.D * (ratio_win / ratio_img - 1d) / 2d);
                    }
                    else
                    {
                        beginCol = 0;
                        endCol = (int)imgWidth.D - 1;
                        beginRow = (int)(-imgHeight.D * (ratio_img / ratio_win - 1d) / 2d);
                        endRow = (int)(imgHeight.D + imgHeight.D * (ratio_img / ratio_win - 1d) / 2d);
                    }
                    hWin.SetPart(beginRow, beginCol, endRow, endCol);//使图像缩放
                    hWin.DispObj(hImg);//显示图像
                }
            }
            catch { }//屏蔽错误
        }

        private void GetOCRText(HObject ho_Imag)
        {
            List<string> ocrs = new List<string>();
            try
            {
                ocrs = Browocrlib.OCRHelper.GetOCR(HImageToEmguCVConverter.HImageToMat(new HImage(ho_Imag)), 6);
            }
            catch (Exception)
            {

            }

            foreach (var item in ocrs)
            {
                txtOCRText.Text = item;
            }
        }

        public void DisplayXLDInCenter(HWindow window, HObject xld)
        {
            try
            {
                // 获取XLD轮廓的边界信息
                HTuple row1, column1, row2, column2;
                HOperatorSet.SmallestRectangle1Xld(xld, out row1, out column1, out row2, out column2);

                // 计算XLD的中心坐标
                double centerRow = (row1.D + row2.D) / 2.0;
                double centerCol = (column1.D + column2.D) / 2.0;

                // 获取窗口尺寸
                HTuple windowWidth, windowHeight;
                HOperatorSet.GetWindowExtents(window, out _, out _, out windowWidth, out windowHeight);

                // 设置显示区域，使XLD中心对准窗口中心
                double zoom = 1.0; // 可以根据需要调整缩放比例
                double partRow = centerRow - (windowHeight.D / 2.0) / zoom;
                double partCol = centerCol - (windowWidth.D / 2.0) / zoom;

                HOperatorSet.SetPart(window,
                                    partRow,
                                    partCol,
                                    partRow + windowHeight.D / zoom,
                                    partCol + windowWidth.D / zoom);

                // 清除窗口并显示XLD
                HOperatorSet.ClearWindow(window);
                HOperatorSet.DispXld(xld, window);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"显示XLD时出错: {ex.Message}");
            }
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                //执行ocv
            }
            else
            {
                //执行ocr
                //DeepOCRHelper.FindOCR(new HImage(ho_image), out string text);
                //txtOCRText.Text = text;
            }
        }

    }
}
