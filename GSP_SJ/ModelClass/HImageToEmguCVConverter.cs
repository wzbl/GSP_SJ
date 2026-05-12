using Emgu.CV;
using Emgu.CV.CvEnum;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GSP_SJ.ModelClass
{
    public class HImageToEmguCVConverter
    {
        /// <summary>
        /// 将 Halcon HImage 转换为 Emgu.CV Mat 对象
        /// </summary>
        /// <param name="hImage">Halcon 图像对象</param>
        /// <returns>Emgu.CV Mat 对象</returns>
        public static Mat HImageToMat(HImage hImage)
        {
            if (hImage == null || !hImage.IsInitialized())
                throw new ArgumentException("HImage is not initialized");

            try
            {
                // 获取图像信息
                string type;
                int width, height;
                IntPtr ptr = hImage.GetImagePointer1(out type, out width, out height);

                if (ptr == IntPtr.Zero)
                    throw new Exception("Failed to get image pointer");

                Mat mat;

                // 根据图像类型创建相应的 Mat
                if (type == "byte") // 8位灰度图像
                {
                    mat = new Mat(height, width, DepthType.Cv8U, 1);

                    // 复制图像数据
                    CopyMemory(mat.DataPointer, ptr, width * height);
                }
                else if (type == "uint2") // 16位灰度图像
                {
                    mat = new Mat(height, width, DepthType.Cv16U, 1);
                    CopyMemory(mat.DataPointer, ptr, width * height * 2);
                }
                else if (type == "real") // 32位浮点图像
                {
                    mat = new Mat(height, width, DepthType.Cv32F, 1);
                    CopyMemory(mat.DataPointer, ptr, width * height * 4);
                }
                else if (type == "rgb") // 24位RGB图像
                {
                    mat = new Mat(height, width, DepthType.Cv8U, 3);
                    CopyMemory(mat.DataPointer, ptr, width * height * 3);
                }
                else if (type == "rgba") // 32位RGBA图像
                {
                    mat = new Mat(height, width, DepthType.Cv8U, 4);
                    CopyMemory(mat.DataPointer, ptr, width * height * 4);
                }
                else
                {
                    throw new Exception($"Unsupported image type: {type}");
                }
                return ConvertToThreeChannel(mat);
                //return mat;
            }
            catch (Exception ex)
            {
                throw new Exception("Error converting HImage to Mat: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 处理多通道图像（如RGB图像分开存储的情况）
        /// </summary>
        public static Mat HImageToMatMultiChannel(HImage hImage)
        {
            if (hImage == null || !hImage.IsInitialized())
                throw new ArgumentException("HImage is not initialized");

            try
            {
                // 获取图像通道数
                HTuple channels = hImage.CountChannels();

                if (channels.Length == 0)
                    throw new Exception("Failed to get channel count");

                int channelCount = channels.I;

                // 获取图像尺寸
                HTuple width, height;
                hImage.GetImageSize(out width, out height);

                // 创建多通道Mat
                Mat mat = new Mat(height.I, width.I, DepthType.Cv8U, channelCount);

                // 处理每个通道
                for (int i = 1; i <= channelCount; i++)
                {
                    // 获取单个通道
                    HImage channel = hImage.AccessChannel(i);

                    // 获取通道数据指针
                    IntPtr ptr = channel.GetImagePointer1(out string type, out int w, out int h);

                    if (ptr == IntPtr.Zero)
                        throw new Exception($"Failed to get pointer for channel {i}");

                    // 创建单通道Mat
                    Mat singleChannel = new Mat(h, w, DepthType.Cv8U, 1);
                    CopyMemory(singleChannel.DataPointer, ptr, w * h);

                    // 将单通道复制到多通道Mat的相应位置
                    CvInvoke.InsertChannel(singleChannel, mat, i - 1);

                    // 释放资源
                    singleChannel.Dispose();
                    channel.Dispose();
                }

                return mat;
            }
            catch (Exception ex)
            {
                throw new Exception("Error converting multi-channel HImage to Mat: " + ex.Message, ex);
            }
        }

        public static Mat ConvertToThreeChannel(Mat inputImage)
        {
            if (inputImage.NumberOfChannels == 1)
            {
                // 将单通道图像转换为三通道
                Mat threeChannel = new Mat();
                CvInvoke.CvtColor(inputImage, threeChannel, ColorConversion.Gray2Bgr);
                return threeChannel;
            }
            else if (inputImage.NumberOfChannels == 3)
            {
                // 已经是三通道，直接返回
                return inputImage.Clone();
            }
            else
            {
                throw new ArgumentException("Unsupported number of channels");
            }
        }

        public static Mat PrepareInputForModel(Mat image, System.Drawing.Size targetSize)
        {
            // 调整图像大小
            Mat resized = new Mat();
            CvInvoke.Resize(image, resized, targetSize);

            // 转换为三通道（如果需要）
            Mat threeChannel = ConvertToThreeChannel(resized);

            // 转换为模型期望的数据类型（通常是float32）
            Mat floatMat = new Mat();
            threeChannel.ConvertTo(floatMat, DepthType.Cv32F);

            // 归一化（如果需要）
            CvInvoke.Normalize(floatMat, floatMat, 0, 1, NormType.MinMax);

            return floatMat;
        }

        // 使用 P/Invoke 进行内存复制
        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
        private static extern void CopyMemory(IntPtr dest, IntPtr src, int size);
    }
}
