using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SqlHelper
{
    public class PublicFunction
    {
        public static void scale_image_range(HObject ho_Image, out HObject ho_ImageScaled, HTuple hv_Min,
    HTuple hv_Max)
        {
            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_ImageSelected = null, ho_SelectedChannel = null;
            HObject ho_LowerRegion = null, ho_UpperRegion = null, ho_ImageSelectedScaled = null;

            // Local copy input parameter variables 
            HObject ho_Image_COPY_INP_TMP;
            ho_Image_COPY_INP_TMP = new HObject(ho_Image);



            // Local control variables 

            HTuple hv_LowerLimit = new HTuple(), hv_UpperLimit = new HTuple();
            HTuple hv_Mult = new HTuple(), hv_Add = new HTuple(), hv_NumImages = new HTuple();
            HTuple hv_ImageIndex = new HTuple(), hv_Channels = new HTuple();
            HTuple hv_ChannelIndex = new HTuple(), hv_MinGray = new HTuple();
            HTuple hv_MaxGray = new HTuple(), hv_Range = new HTuple();
            HTuple hv_Max_COPY_INP_TMP = new HTuple(hv_Max);
            HTuple hv_Min_COPY_INP_TMP = new HTuple(hv_Min);

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImageScaled);
            HOperatorSet.GenEmptyObj(out ho_ImageSelected);
            HOperatorSet.GenEmptyObj(out ho_SelectedChannel);
            HOperatorSet.GenEmptyObj(out ho_LowerRegion);
            HOperatorSet.GenEmptyObj(out ho_UpperRegion);
            HOperatorSet.GenEmptyObj(out ho_ImageSelectedScaled);
            //Convenience procedure to scale the gray values of the
            //input image Image from the interval [Min,Max]
            //to the interval [0,255] (default).
            //Gray values < 0 or > 255 (after scaling) are clipped.
            //
            //If the image shall be scaled to an interval different from [0,255],
            //this can be achieved by passing tuples with 2 values [From, To]
            //as Min and Max.
            //Example:
            //scale_image_range(Image:ImageScaled:[100,50],[200,250])
            //maps the gray values of Image from the interval [100,200] to [50,250].
            //All other gray values will be clipped.
            //
            //input parameters:
            //Image: the input image
            //Min: the minimum gray value which will be mapped to 0
            //     If a tuple with two values is given, the first value will
            //     be mapped to the second value.
            //Max: The maximum gray value which will be mapped to 255
            //     If a tuple with two values is given, the first value will
            //     be mapped to the second value.
            //
            //Output parameter:
            //ImageScale: the resulting scaled image.
            //
            if ((int)(new HTuple((new HTuple(hv_Min_COPY_INP_TMP.TupleLength())).TupleEqual(
                2))) != 0)
            {
                hv_LowerLimit.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_LowerLimit = hv_Min_COPY_INP_TMP.TupleSelect(
                        1);
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_Min = hv_Min_COPY_INP_TMP.TupleSelect(
                            0);
                        hv_Min_COPY_INP_TMP.Dispose();
                        hv_Min_COPY_INP_TMP = ExpTmpLocalVar_Min;
                    }
                }
            }
            else
            {
                hv_LowerLimit.Dispose();
                hv_LowerLimit = 0.0;
            }
            if ((int)(new HTuple((new HTuple(hv_Max_COPY_INP_TMP.TupleLength())).TupleEqual(
                2))) != 0)
            {
                hv_UpperLimit.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_UpperLimit = hv_Max_COPY_INP_TMP.TupleSelect(
                        1);
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_Max = hv_Max_COPY_INP_TMP.TupleSelect(
                            0);
                        hv_Max_COPY_INP_TMP.Dispose();
                        hv_Max_COPY_INP_TMP = ExpTmpLocalVar_Max;
                    }
                }
            }
            else
            {
                hv_UpperLimit.Dispose();
                hv_UpperLimit = 255.0;
            }
            //
            //Calculate scaling parameters.
            //Only scale if the scaling range is not zero.
            if ((int)((new HTuple(((((hv_Max_COPY_INP_TMP - hv_Min_COPY_INP_TMP)).TupleAbs()
                )).TupleLess(1.0E-6))).TupleNot()) != 0)
            {
                hv_Mult.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Mult = (((hv_UpperLimit - hv_LowerLimit)).TupleReal()
                        ) / (hv_Max_COPY_INP_TMP - hv_Min_COPY_INP_TMP);
                }
                hv_Add.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Add = ((-hv_Mult) * hv_Min_COPY_INP_TMP) + hv_LowerLimit;
                }
                //Scale image.
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ScaleImage(ho_Image_COPY_INP_TMP, out ExpTmpOutVar_0, hv_Mult,
                        hv_Add);
                    ho_Image_COPY_INP_TMP.Dispose();
                    ho_Image_COPY_INP_TMP = ExpTmpOutVar_0;
                }
            }
            //
            //Clip gray values if necessary.
            //This must be done for each image and channel separately.
            ho_ImageScaled.Dispose();
            HOperatorSet.GenEmptyObj(out ho_ImageScaled);
            hv_NumImages.Dispose();
            HOperatorSet.CountObj(ho_Image_COPY_INP_TMP, out hv_NumImages);
            HTuple end_val51 = hv_NumImages;
            HTuple step_val51 = 1;
            for (hv_ImageIndex = 1; hv_ImageIndex.Continue(end_val51, step_val51); hv_ImageIndex = hv_ImageIndex.TupleAdd(step_val51))
            {
                ho_ImageSelected.Dispose();
                HOperatorSet.SelectObj(ho_Image_COPY_INP_TMP, out ho_ImageSelected, hv_ImageIndex);
                hv_Channels.Dispose();
                HOperatorSet.CountChannels(ho_ImageSelected, out hv_Channels);
                HTuple end_val54 = hv_Channels;
                HTuple step_val54 = 1;
                for (hv_ChannelIndex = 1; hv_ChannelIndex.Continue(end_val54, step_val54); hv_ChannelIndex = hv_ChannelIndex.TupleAdd(step_val54))
                {
                    ho_SelectedChannel.Dispose();
                    HOperatorSet.AccessChannel(ho_ImageSelected, out ho_SelectedChannel, hv_ChannelIndex);
                    hv_MinGray.Dispose(); hv_MaxGray.Dispose(); hv_Range.Dispose();
                    HOperatorSet.MinMaxGray(ho_SelectedChannel, ho_SelectedChannel, 0, out hv_MinGray,
                        out hv_MaxGray, out hv_Range);
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_LowerRegion.Dispose();
                        HOperatorSet.Threshold(ho_SelectedChannel, out ho_LowerRegion, ((hv_MinGray.TupleConcat(
                            hv_LowerLimit))).TupleMin(), hv_LowerLimit);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_UpperRegion.Dispose();
                        HOperatorSet.Threshold(ho_SelectedChannel, out ho_UpperRegion, hv_UpperLimit,
                            ((hv_UpperLimit.TupleConcat(hv_MaxGray))).TupleMax());
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.PaintRegion(ho_LowerRegion, ho_SelectedChannel, out ExpTmpOutVar_0,
                            hv_LowerLimit, "fill");
                        ho_SelectedChannel.Dispose();
                        ho_SelectedChannel = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.PaintRegion(ho_UpperRegion, ho_SelectedChannel, out ExpTmpOutVar_0,
                            hv_UpperLimit, "fill");
                        ho_SelectedChannel.Dispose();
                        ho_SelectedChannel = ExpTmpOutVar_0;
                    }
                    if ((int)(new HTuple(hv_ChannelIndex.TupleEqual(1))) != 0)
                    {
                        ho_ImageSelectedScaled.Dispose();
                        HOperatorSet.CopyObj(ho_SelectedChannel, out ho_ImageSelectedScaled, 1,
                            1);
                    }
                    else
                    {
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.AppendChannel(ho_ImageSelectedScaled, ho_SelectedChannel,
                                out ExpTmpOutVar_0);
                            ho_ImageSelectedScaled.Dispose();
                            ho_ImageSelectedScaled = ExpTmpOutVar_0;
                        }
                    }
                }
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ConcatObj(ho_ImageScaled, ho_ImageSelectedScaled, out ExpTmpOutVar_0
                        );
                    ho_ImageScaled.Dispose();
                    ho_ImageScaled = ExpTmpOutVar_0;
                }
            }
            ho_Image_COPY_INP_TMP.Dispose();
            ho_ImageSelected.Dispose();
            ho_SelectedChannel.Dispose();
            ho_LowerRegion.Dispose();
            ho_UpperRegion.Dispose();
            ho_ImageSelectedScaled.Dispose();

            hv_Max_COPY_INP_TMP.Dispose();
            hv_Min_COPY_INP_TMP.Dispose();
            hv_LowerLimit.Dispose();
            hv_UpperLimit.Dispose();
            hv_Mult.Dispose();
            hv_Add.Dispose();
            hv_NumImages.Dispose();
            hv_ImageIndex.Dispose();
            hv_Channels.Dispose();
            hv_ChannelIndex.Dispose();
            hv_MinGray.Dispose();
            hv_MaxGray.Dispose();
            hv_Range.Dispose();

            return;
        }

        public static HShapeModel ShapeModel = new HShapeModel();
        public static void CreateHShapeModel(HWindowControl hWindow, HImage hImage, Rectangle rOI, out HXLDCont ModelContour)
        {
            HObject ho_ImageReduced = null;
            HObject ho_ModelContours = null;
            HObject ho_findCircleRoi = null;
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_ModelContours);
            HOperatorSet.GenEmptyObj(out ho_findCircleRoi);

            //提示信息
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                HOperatorSet.GenRectangle1(out ho_findCircleRoi, rOI.Top, rOI.Left,
                                                                        rOI.Bottom, rOI.Right);
            }
            HOperatorSet.ReduceDomain(hImage, ho_findCircleRoi, out ho_ImageReduced);
            ModelContour = null;

            try
            {
                ShapeModel.CreateShapeModel(
           new HImage(ho_ImageReduced),
           0,
             -0.39,
                        0.79,
           "auto",
           "auto",
           "ignore_local_polarity",
             "auto",
            "auto");

                ModelContour = ShapeModel.GetShapeModelContours(1);

                // 2. 计算 ROI 中心点
                HOperatorSet.SmallestRectangle1(ho_findCircleRoi, out HTuple row1, out HTuple column1, out HTuple row2, out HTuple column2);
                double originRow = (row1 + row2) / 2.0;
                double originCol = (column1 + column2) / 2.0;
                //在创建模板时，手动设置原点为 ROI 的中心：
                ShapeModel.SetShapeModelOrigin(originRow, originCol);

                ho_ImageReduced.Dispose();
                ho_ModelContours.Dispose();
                ho_findCircleRoi.Dispose();
            }
            catch (Exception)
            {
                //creatImg.Dispose();
                ho_ImageReduced.Dispose();
                ho_ModelContours.Dispose();
                ho_findCircleRoi.Dispose();
            }

        }

        public static bool FindHShapeModel(HWindowControl hWindow, HImage hImage, Rectangle rOI, HShapeModel shapeModel, out double[] pixelRow, out double[] pixelCol, bool IsOCR = false)
        {

            HObject ho_ImageReduced = null;
            HObject ho_ModelContours = null;
            HObject ho_findCircleRoi = null;
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_ModelContours);
            HOperatorSet.GenEmptyObj(out ho_findCircleRoi);
            HObject Modelcontours, contours;
            HTuple row, column, angle, scale, score, HomMat2D;
            HOperatorSet.GenEmptyObj(out Modelcontours);
            HOperatorSet.GenEmptyObj(out contours);

            try
            {
                if (shapeModel == null || !shapeModel.IsInitialized())
                {

                    pixelRow = null;
                    pixelCol = null;
                    return false;
                }


                //提示信息
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    HOperatorSet.GenRectangle1(out ho_findCircleRoi, rOI.Top, rOI.Left,
                                                                            rOI.Bottom, rOI.Right);
                }

                HOperatorSet.ReduceDomain(hImage, ho_findCircleRoi, out ho_ImageReduced);





                shapeModel.FindShapeModel(
               new HImage(ho_ImageReduced),
                -0.39,
                        0.79,
              0.6,
              1,
               0,
               "ignore_local_polarity",
              0,
              0.9,
               out row,
               out column,
               out angle,
               out score);
                if (row.Length > 0)
                {

                }
                else
                {
                    ho_ImageReduced.Dispose();
                    ho_findCircleRoi.Dispose();
                    ho_ModelContours.Dispose();
                    Modelcontours.Dispose();


                    pixelRow = null;
                    pixelCol = null;
                    return false;
                }

                for (int i = 0; i < row.Length; i++)
                {
                    HOperatorSet.HomMat2dIdentity(out HomMat2D);

                    // 应用缩放
                    //HOperatorSet.HomMat2dScale(HomMat2D, scale[i], scale[i], 0, 0, out HomMat2D);

                    // 应用旋转
                    HOperatorSet.HomMat2dRotate(HomMat2D, angle[i], 0, 0, out HomMat2D);

                    // 应用平移
                    HOperatorSet.HomMat2dTranslate(HomMat2D, row[i], column[i], out HomMat2D);
                    //获取模板轮廓
                    Modelcontours.Dispose();

                    HOperatorSet.GetShapeModelContours(out Modelcontours, shapeModel, 1);

                    //对模板轮廓进行仿射变换
                    HOperatorSet.AffineTransContourXld(Modelcontours, out contours, HomMat2D);
                    //HOperatorSet.Connection(contours, out HObject singleContour);
                    //HOperatorSet.SelectObj(contours, out HObject singleContour, 1);
                    //HOperatorSet.GetContourXld(singleContour, out HTuple r, out HTuple c);


                    HObject showcontour = new HObject(contours);

                    var center = GetVerticalDigitsCenter(contours);
                    double px = center.centerRow;
                    double py = center.centerCol;
                    row[i] = px;
                    column[i] = py;
                    double scor = score[i].D;

                    contours.Dispose();
                }
                ho_ImageReduced.Dispose();
                ho_findCircleRoi.Dispose();
                ho_ModelContours.Dispose();
                Modelcontours.Dispose();
                //creatImg.Dispose();
                pixelRow = row;
                pixelCol = column;
                return true;
            }
            catch (Exception ex)
            {

                ho_ImageReduced.Dispose();
                ho_findCircleRoi.Dispose();
                ho_ModelContours.Dispose();
                Modelcontours.Dispose();
                pixelRow = null;
                pixelCol = null;
                return false;
            }
        }

        #region 轮廓中心    
        /// <summary>
        /// 获取竖排数字整体轮廓的中心坐标
        /// </summary>
        /// <param name="digitsContour">竖排数字轮廓组</param>
        /// <param name="homMat2D">变换矩阵（可选）</param>
        /// <returns>整体中心坐标 (row, column)</returns>
        public static CenterXY GetVerticalDigitsCenter(
            HObject digitsContour,
            HTuple homMat2D = null)
        {
            try
            {
                HObject processedContour = digitsContour;

                // 1. 应用变换（如果提供了变换矩阵）
                if (homMat2D != null && homMat2D.Length > 0)
                {
                    HOperatorSet.AffineTransContourXld(digitsContour, out processedContour, homMat2D);
                }

                // 2. 方法一：直接计算所有轮廓点的中心（简单快速）
                return CalculateCenterFromAllPoints(processedContour);

                // 3. 方法二：先转换为区域再计算中心（更精确但稍慢）
                // return CalculateCenterFromRegion(processedContour);
            }
            catch (Exception ex)
            {
                throw new Exception($"计算竖排数字中心时出错: {ex.Message}");
            }
        }

        /// <summary>
        /// 通过所有轮廓点计算中心
        /// </summary>
        private static CenterXY CalculateCenterFromAllPoints(HObject contour)
        {
            // 获取所有轮廓点的坐标
            HTuple allRows = new HTuple();
            HTuple allCols = new HTuple();

            HTuple numContours;
            HOperatorSet.CountObj(contour, out numContours);

            // 遍历所有轮廓部分
            for (int i = 1; i <= numContours.I; i++)
            {
                HObject part;
                HOperatorSet.SelectObj(contour, out part, i);

                HTuple rows, cols;
                HOperatorSet.GetContourXld(part, out rows, out cols);

                allRows = allRows.TupleConcat(rows);
                allCols = allCols.TupleConcat(cols);
            }

            // 计算所有点的平均值作为中心
            double centerRow = allRows.TupleMean().D;
            double centerCol = allCols.TupleMean().D;

            CenterXY centerXY = new CenterXY();
            centerXY.centerRow = centerRow;
            centerXY.centerCol = centerCol;
            return centerXY;
        }

        /// <summary>
        /// 通过转换为区域计算中心（更精确）
        /// </summary>
        private static CenterXY CalculateCenterFromRegion(HObject contour)
        {
            // 将轮廓转换为区域
            HObject region;
            HOperatorSet.GenRegionContourXld(contour, out region, "filled");

            // 计算区域中心
            HTuple area, rowCenter, colCenter;
            HOperatorSet.AreaCenter(region, out area, out rowCenter, out colCenter);
            CenterXY centerXY = new CenterXY();
            centerXY.centerRow = rowCenter.D;
            centerXY.centerCol = colCenter.D;
            return centerXY;
        }

        /// <summary>
        /// 获取竖排数字的边界框中心
        /// </summary>
        public static CenterXY GetVerticalDigitsBoundingBoxCenter(
            HObject digitsContour,
            HTuple homMat2D = null)
        {
            try
            {
                HObject processedContour = digitsContour;

                // 应用变换（如果提供了变换矩阵）
                if (homMat2D != null && homMat2D.Length > 0)
                {
                    HOperatorSet.AffineTransContourXld(digitsContour, out processedContour, homMat2D);
                }

                // 获取最小外接矩形
                HTuple row1, column1, row2, column2;
                HOperatorSet.SmallestRectangle1Xld(processedContour, out row1, out column1, out row2, out column2);

                // 计算矩形中心
                double centerRow = (row1.D + row2.D) / 2.0;
                double centerCol = (column1.D + column2.D) / 2.0;
                CenterXY centerXY = new CenterXY();
                centerXY.centerRow = centerRow;
                centerXY.centerCol = centerCol;
                return centerXY;
            }
            catch (Exception ex)
            {
                throw new Exception($"计算边界框中心时出错: {ex.Message}");
            }
        }


        #endregion

        public static HXLDCont GetCorrectedContour(HXLDCont ModelContour)
        {
            if (ShapeModel == null || ModelContour == null)
                return null;

            double row, col;
            ShapeModel.GetShapeModelOrigin(out row, out col);

            HHomMat2D homMat = new HHomMat2D();
            homMat = homMat.HomMat2dTranslate(row, col);

            return ModelContour.AffineTransContourXld(homMat);
        }

        public static byte[] BitmapToByte(Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            ms.Seek(0, System.IO.SeekOrigin.Begin);
            byte[] bytes = new byte[ms.Length];
            ms.Read(bytes, 0, bytes.Length);
            ms.Dispose();
            return bytes;
        }


        public static Bitmap ByteToBitmap(byte[] ImageByte)
        {
            Bitmap bitmap = null; 
            using (MemoryStream stream = new MemoryStream(ImageByte))
            {
                bitmap = new Bitmap((Image)new Bitmap(stream));
            }
            return bitmap;
        }

        public static byte[] GetByte(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            byte[] btye2 = new byte[fs.Length];
            fs.Read(btye2, 0, Convert.ToInt32(fs.Length));
            fs.Close();
            return btye2;
        }

    }

    public class CenterXY
    {
        public double centerRow = 0;
        public double centerCol = 0;
    }
}
