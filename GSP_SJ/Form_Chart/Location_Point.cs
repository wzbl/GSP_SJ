using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSP_SJ
{
    public class Location_Point
    {
        public static double[] handPositionX = new double[4];
        public static double[] handPositionY = new double[4];
        public static double[] eyePixelPositionX = new double[4];
        public static double[] eyePixelPositionY = new double[4];

        /// <summary>
        /// 保存标定数据
        /// </summary>
        public void Save()
        {

        }

        /// <summary>
        /// 加载标定数据
        /// </summary>
        public void Load()
        {

        }


        /// <summary>
        /// 获取手眼矩阵
        /// </summary>
        /// <param name="EyePixelPosition_x">图像图标X</param>
        /// <param name="EyePixelPosition_y">图像图标Y</param>
        /// <param name="HandPosition_x">世界坐标X</param>
        /// <param name="HandPosition_y">世界坐标Y</param>
        /// <param name="hv_HomMat2D"></param>
        private static void HandEyeMatrix(out HTuple hv_HomMat2D)
        {
            hv_HomMat2D = -1;
            try
            {
                //根据两组点之间的对应关系，计算一个2D仿射变换矩阵
                HOperatorSet.VectorToHomMat2d(eyePixelPositionX, eyePixelPositionY, handPositionX, handPositionY, out hv_HomMat2D);
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// <summary>
        /// 图像坐标转换-世界坐标
        /// </summary>
        /// </summary>
        /// <param name="PixelPositionX">图像坐标X</param>
        /// <param name="PixelPositionY">图像坐标Y</param>
        /// <param name="qx">世界坐标X</param>
        /// <param name="qy">世界坐标Y</param>
        public static void ImageToWorld(double PixelPositionX, double PixelPositionY, out HTuple qx, out HTuple qy)
        {
            try
            {
                HandEyeMatrix(out HTuple hv_HomMat2D);
                HOperatorSet.AffineTransPoint2d(hv_HomMat2D, PixelPositionX, PixelPositionY, out qx, out qy);
            }
            catch (Exception)
            {
                qx = 0;
                qy = 0;
            }
        }

        /// <summary>
        /// 世界坐标转换-图像坐标
        /// </summary>
        public static void WorldToImage(double qx, double qy, out HTuple PixelPositionX, out HTuple PixelPositionY)
        {
            try
            {
                HandEyeMatrix(out HTuple hv_HomMat2D);
                HOperatorSet.HomMat2dInvert(hv_HomMat2D, out HTuple hv_HomMat2D_Inverse);
                HOperatorSet.AffineTransPoint2d(hv_HomMat2D_Inverse, qx, qy, out PixelPositionX, out PixelPositionY);
            }
            catch (Exception)
            {
                PixelPositionX = 0;
                PixelPositionY = 0;
            }
        }

        /// <summary>
        /// 纠正坐标
        /// 验证标定的准确性
        /// </summary>
        /// <param name="centerRow">图像中心像素坐标</param>
        /// <param name="centerCol"></param>
        /// <param name="clickRow">点击图像的像素坐标</param>
        /// <param name="clickCol"></param>
        /// <param name="dx">轴移动位置</param>
        /// <param name="dy"></param>
        public static void CorrectPos(double centerRow, double centerCol, double clickRow, double clickCol, out double dx, out double dy)
        {
            ImageToWorld(centerRow, centerCol, out HTuple centerQx, out HTuple centerQy);
            ImageToWorld(clickRow, clickCol, out HTuple clickQx, out HTuple clickQy);
            dx = clickQx.D - centerQx.D;
            dy = clickQy.D - centerQy.D;
        }
    }

}
