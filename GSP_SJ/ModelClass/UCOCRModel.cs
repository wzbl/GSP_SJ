using Emgu.CV;
using Emgu.CV.Structure;
using HalconDotNet;
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
            RefreshData(image, rOI);
            rOI.ChangeROI += RefreshData;
        }

        HObject ho_image;

        private void RefreshData(Image image, DirectionalROI rOI)
        {
            Image<Bgr, byte> image1 = CropImage(ConvertToImageBgr(image), rOI.Bounds);
            if (image1 != null)
                picImg.Image = image1.ToBitmap();

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

                HObject ho_findCircleRoi;
                //执行ocr
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    HOperatorSet.GenRectangle1(out ho_findCircleRoi, rOI.Top, rOI.Left,
                    rOI.Bottom, rOI.Right);
                }

                HOperatorSet.ReduceDomain(gray, ho_findCircleRoi, out ho_image);
                //HOperatorSet.CropDomain(ho_image,  out ho_image);
                DeepOCRHelper.FindOCR(new HImage(ho_image), out string text);
                txtOCRText.Text = text;

                ho_findCircleRoi.Dispose();
                gray.Dispose();

            }
        }
        /// <summary>
        /// 获取轮廓
        /// </summary>
        private void GetImgshape()
        {
            try
            {
                using (HImage hImage = HalconImageConverter.BitmapToHImageRGB(new Bitmap(picImg.Image)))
                {
                    //提取轮廓
                    HOperatorSet.Rgb1ToGray(hImage, out HObject gray);
                    HOperatorSet.CreateShapeModel
                        (hImage,
                        "auto",
                        -0.39,
                        0.79,
                              "auto",
                              "auto",
                        "use_polarity",
                             "auto",
                            "auto",
                       out HTuple ModelID);

                    HOperatorSet.GetShapeModelContours(out HObject modelContours, ModelID, 1);

                    hWindowControl1.Visible = true;
                    DisplayXLDInCenter(hWindowControl1.HalconWindow, modelContours);
                }
            }
            catch (Exception ex)
            {

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

        public static Image<Bgr, byte> ConvertToImageBgr(Image image)
        {
            // 确保图像是Bitmap格式
            Bitmap bitmap = image as Bitmap ?? new Bitmap(image);

            // 直接转换为Image<Bgr, byte>
            return new Image<Bgr, byte>(bitmap);
        }

        public static Image<Bgr, byte> CropImage(Image<Bgr, byte> image, Rectangle roi)
        {
            // 验证ROI是否在图像范围内
            if (roi.X < 0 || roi.Y < 0 ||
                roi.Right > image.Width ||
                roi.Bottom > image.Height)
            {
                ////throw new ArgumentException("裁剪区域超出图像范围");
                //Rectangle rectangle = new Rectangle(roi.Left,roi.Top, image.Width-roi.Left,image.Height-roi.Top);
                //return image.Copy(rectangle);
                return null;
            }

            return image.Copy(roi);
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
                DeepOCRHelper.FindOCR(new HImage(ho_image), out string text);
                txtOCRText.Text = text;
            }
        }

    }
}
