using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GSP_SJ.ModelClass
{
    public static class HalconImageConverter
    {
        [DllImport("kernel32.dll")]
        private static extern void CopyMemory(IntPtr dest, IntPtr src, int length);
        /// <summary>
        /// 将 HImage 转换为 Bitmap（支持灰度图和彩色图）
        /// </summary>
        /// <param name="hImage">Halcon 图像对象</param>
        /// <returns>.NET Bitmap 对象</returns>
        public static Bitmap ConvertToBitmap(HImage hImage)
        {
            if (hImage == null || !hImage.IsInitialized())
                throw new ArgumentException("HImage 未初始化或为空");

            try
            {
                // 获取图像类型和尺寸
                HTuple type, width, height;
                HOperatorSet.GetImageType(hImage, out type);
                HOperatorSet.GetImageSize(hImage, out width, out height);

                //// 根据图像类型处理
                //if (type.S == "byte")
                //{
                //    return ConvertByteImage(hImage, width, height);
                //}
                //else if (type.S == "real")
                //{
                //    return ConvertRealImage(hImage, width, height);
                //}
                //else if (type.S == "uint2")
                //{
                //    return ConvertUint2Image(hImage, width, height);
                //}
                //else if (type.S.StartsWith("rgb"))
                {
                    return ConvertRgbImage(hImage, width, height);
                }
                //else
                //{
                //    throw new NotSupportedException($"不支持的图像类型: {type.S}");
                //}
            }
            catch (Exception ex)
            {
                throw new Exception("转换 HImage 到 Bitmap 失败", ex);
            }
        }

        /// <summary>
        /// 转换字节类型图像（8位灰度图）
        /// </summary>
        private static Bitmap ConvertByteImage(HImage hImage, HTuple width, HTuple height)
        {
            // 获取图像指针
            HTuple pointer, type;
            HOperatorSet.GetImagePointer1(hImage, out pointer, out type, out width, out height);

            // 创建 Bitmap
            Bitmap bitmap = new Bitmap(width.I, height.I, PixelFormat.Format8bppIndexed);

            // 设置灰度调色板
            ColorPalette palette = bitmap.Palette;
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            bitmap.Palette = palette;

            // 锁定 Bitmap 数据
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, width.I, height.I),
                ImageLockMode.WriteOnly,
                PixelFormat.Format8bppIndexed
            );

            try
            {
                // 复制数据
                int size = width.I * height.I;
                byte[] byteData = new byte[size];
                Marshal.Copy(pointer.IP, byteData, 0, size);

                Marshal.Copy(byteData, 0, bitmapData.Scan0, size);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            return bitmap;
        }

        /// <summary>
        /// 转换 RGB 图像
        /// </summary>
        private static Bitmap ConvertRgbImage(HImage hImage, HTuple width, HTuple height)
        {
            // 获取 RGB 通道指针
            HTuple pointerR, pointerG, pointerB, type;
            HOperatorSet.GetImagePointer3(hImage, out pointerR, out pointerG, out pointerB, out type, out width, out height);

            // 创建 Bitmap
            Bitmap bitmap = new Bitmap(width.I, height.I, PixelFormat.Format24bppRgb);

            // 锁定 Bitmap 数据
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, width.I, height.I),
                ImageLockMode.WriteOnly,
                PixelFormat.Format24bppRgb
            );

            try
            {
                int stride = bitmapData.Stride;
                int bytesPerPixel = 3; // 24bpp = 3 bytes per pixel
                int size = width.I * height.I * bytesPerPixel;

                // 创建临时缓冲区
                byte[] buffer = new byte[size];

                // 交错复制 R、G、B 通道数据
                unsafe
                {
                    byte* rPtr = (byte*)pointerR.IP;
                    byte* gPtr = (byte*)pointerG.IP;
                    byte* bPtr = (byte*)pointerB.IP;

                    fixed (byte* destPtr = buffer)
                    {
                        for (int y = 0; y < height.I; y++)
                        {
                            for (int x = 0; x < width.I; x++)
                            {
                                int srcIndex = y * width.I + x;
                                int destIndex = y * stride + x * bytesPerPixel;

                                destPtr[destIndex + 2] = rPtr[srcIndex];     // R
                                destPtr[destIndex + 1] = gPtr[srcIndex];     // G
                                destPtr[destIndex] = bPtr[srcIndex];         // B
                            }
                        }
                    }
                }

                // 复制到 Bitmap
                Marshal.Copy(buffer, 0, bitmapData.Scan0, size);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            return bitmap;
        }

        /// <summary>
        /// 转换实数类型图像（32位浮点）
        /// </summary>
        private static Bitmap ConvertRealImage(HImage hImage, HTuple width, HTuple height)
        {
            // 获取图像数据
            HTuple pointer, type;
            HOperatorSet.GetImagePointer1(hImage, out pointer, out type, out width, out height);

            // 创建 Bitmap
            Bitmap bitmap = new Bitmap(width.I, height.I, PixelFormat.Format8bppIndexed);

            // 设置灰度调色板
            ColorPalette palette = bitmap.Palette;
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            bitmap.Palette = palette;

            // 获取浮点数据并转换为字节
            int size = width.I * height.I;
            float[] floatData = new float[size];
            Marshal.Copy(pointer.IP, floatData, 0, size);

            // 找到最小和最大值以进行归一化
            float min = float.MaxValue;
            float max = float.MinValue;
            foreach (float value in floatData)
            {
                if (value < min) min = value;
                if (value > max) max = value;
            }

            float range = max - min;
            if (range == 0) range = 1; // 避免除以零

            // 转换为字节数据
            byte[] byteData = new byte[size];
            for (int i = 0; i < size; i++)
            {
                byteData[i] = (byte)(255 * (floatData[i] - min) / range);
            }

            // 锁定 Bitmap 数据
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, width.I, height.I),
                ImageLockMode.WriteOnly,
                PixelFormat.Format8bppIndexed
            );

            try
            {
                Marshal.Copy(byteData, 0, bitmapData.Scan0, size);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            return bitmap;
        }

        /// <summary>
        /// 转换 16 位无符号整数图像
        /// </summary>
        private static Bitmap ConvertUint2Image(HImage hImage, HTuple width, HTuple height)
        {
            // 获取图像数据
            HTuple pointer, type;
            HOperatorSet.GetImagePointer1(hImage, out pointer, out type, out width, out height);

            // 创建 Bitmap
            Bitmap bitmap = new Bitmap(width.I, height.I, PixelFormat.Format8bppIndexed);

            // 设置灰度调色板
            ColorPalette palette = bitmap.Palette;
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            bitmap.Palette = palette;

            // 获取 16 位数据并转换为字节
            int size = width.I * height.I;
            ushort[] ushortData = new ushort[size];
            //Marshal.Copy(pointer.IP, null, 0, size);

            // 找到最小和最大值以进行归一化
            ushort min = ushort.MaxValue;
            ushort max = ushort.MinValue;
            foreach (ushort value in ushortData)
            {
                if (value < min) min = value;
                if (value > max) max = value;
            }

            ushort range = (ushort)(max - min);
            if (range == 0) range = 1; // 避免除以零

            // 转换为字节数据
            byte[] byteData = new byte[size];
            for (int i = 0; i < size; i++)
            {
                byteData[i] = (byte)(255 * (ushortData[i] - min) / (float)range);
            }

            // 锁定 Bitmap 数据
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, width.I, height.I),
                ImageLockMode.WriteOnly,
                PixelFormat.Format8bppIndexed
            );

            try
            {
                Marshal.Copy(byteData, 0, bitmapData.Scan0, size);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            return bitmap;
        }


        public static HObject BitmapToHImage(Bitmap bitmap)
        {
            HObject ho_Image = null;
            try
            {
                // 将Bitmap锁定到内存
                BitmapData bmpData = bitmap.LockBits(
                    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    ImageLockMode.ReadOnly,
                    bitmap.PixelFormat);

                // 根据像素格式创建Halcon图像
                if (bitmap.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    HOperatorSet.GenImage1(out ho_Image, "byte",
                                         bitmap.Width, bitmap.Height,
                                         bmpData.Scan0);
                }
                else if (bitmap.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    HOperatorSet.GenImageInterleaved(out ho_Image, bmpData.Scan0,
                                                   "bgr", bitmap.Width, bitmap.Height,
                                                   -1, "byte", 0, 0, 0, 0, -1, 0);
                }
                else if (bitmap.PixelFormat == PixelFormat.Format32bppArgb)
                {
                    HOperatorSet.GenImageInterleaved(out ho_Image, bmpData.Scan0,
                                                   "bgra", bitmap.Width, bitmap.Height,
                                                   -1, "byte", 0, 0, 0, 0, -1, 0);
                }

                // 解锁Bitmap
                bitmap.UnlockBits(bmpData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"转换失败: {ex.Message}");
                ho_Image?.Dispose();
                ho_Image = null;
            }
            return ho_Image;
        }

        public static Bitmap HImageToBitmap(HImage hImage)
        {
            try
            {
                // 获取图像信息
                HTuple width, height;
                hImage.GetImageSize(out width, out height);

                // 获取图像指针和数据
                HTuple pointer, type, widthVal;
                hImage.GetImagePointer1(out pointer, out type, out widthVal);

                // 创建Bitmap
                Bitmap bitmap = new Bitmap(width.I, height.I, PixelFormat.Format8bppIndexed);

                // 设置灰度调色板
                ColorPalette palette = bitmap.Palette;
                for (int i = 0; i < 256; i++)
                {
                    palette.Entries[i] = Color.FromArgb(i, i, i);
                }
                bitmap.Palette = palette;

                // 锁定Bitmap数据
                BitmapData bitmapData = bitmap.LockBits(
                    new Rectangle(0, 0, width.I, height.I),
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format8bppIndexed);

                // 复制数据
                byte[] imageData = new byte[width.I * height.I];
                Marshal.Copy(pointer, imageData, 0, imageData.Length);
                Marshal.Copy(imageData, 0, bitmapData.Scan0, imageData.Length);

                bitmap.UnlockBits(bitmapData);
                return bitmap;
            }
            catch (Exception ex)
            {
                throw new Exception("HImage转Bitmap失败: " + ex.Message);
            }
        }

        // RGB图像版本
        public static Bitmap HImageToBitmapRGB(HImage hImage)
        {
            try
            {
                HTuple width, height;
                hImage.GetImageSize(out width, out height);

                HTuple pointerR, pointerG, pointerB, type, widthVal, hightVal;
                hImage.GetImagePointer3(out pointerR, out pointerG, out pointerB,
                                       out type, out widthVal, out hightVal);

                Bitmap bitmap = new Bitmap(width.I, height.I, PixelFormat.Format24bppRgb);

                BitmapData bitmapData = bitmap.LockBits(
                    new Rectangle(0, 0, width.I, height.I),
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format24bppRgb);

                int stride = bitmapData.Stride;
                byte[] rgbData = new byte[stride * height.I];

                // 合并RGB通道
                byte[] rData = new byte[width.I * height.I];
                byte[] gData = new byte[width.I * height.I];
                byte[] bData = new byte[width.I * height.I];

                Marshal.Copy(pointerR, rData, 0, rData.Length);
                Marshal.Copy(pointerG, gData, 0, gData.Length);
                Marshal.Copy(pointerB, bData, 0, bData.Length);

                for (int y = 0; y < height.I; y++)
                {
                    for (int x = 0; x < width.I; x++)
                    {
                        int index = y * width.I + x;
                        int rgbIndex = y * stride + x * 3;

                        rgbData[rgbIndex + 2] = rData[index];     // R
                        rgbData[rgbIndex + 1] = gData[index];     // G  
                        rgbData[rgbIndex + 0] = bData[index];     // B
                    }
                }

                Marshal.Copy(rgbData, 0, bitmapData.Scan0, rgbData.Length);
                bitmap.UnlockBits(bitmapData);

                return bitmap;
            }
            catch (Exception ex)
            {
                throw new Exception("HImage转RGB Bitmap失败: " + ex.Message);
            }
        }

        public static HImage BitmapToHImage2(Bitmap bitmap)
        {
            try
            {
                if (bitmap.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    return BitmapToHImageGray(bitmap);
                }
                else
                {
                    return BitmapToHImageRGB(bitmap);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Bitmap转HImage失败: " + ex.Message);
            }
        }

        public static HObject BitmapToHObject(Bitmap bitmap)
        {
            HObject ho_Image = null;
            try
            {
                // 将Bitmap锁定到内存
                BitmapData bmpData = bitmap.LockBits(
                    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    ImageLockMode.ReadOnly,
                    bitmap.PixelFormat);

                // 根据像素格式创建Halcon图像
                if (bitmap.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    HOperatorSet.GenImage1(out ho_Image, "byte",
                                         bitmap.Width, bitmap.Height,
                                         bmpData.Scan0);
                }
                else if (bitmap.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    HOperatorSet.GenImageInterleaved(out ho_Image, bmpData.Scan0,
                                                   "bgr", bitmap.Width, bitmap.Height,
                                                   -1, "byte", 0, 0, 0, 0, -1, 0);
                }
                else if (bitmap.PixelFormat == PixelFormat.Format32bppArgb)
                {
                    HOperatorSet.GenImageInterleaved(out ho_Image, bmpData.Scan0,
                                                   "bgra", bitmap.Width, bitmap.Height,
                                                   -1, "byte", 0, 0, 0, 0, -1, 0);
                }

                // 解锁Bitmap
                bitmap.UnlockBits(bmpData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"转换失败: {ex.Message}");
                ho_Image?.Dispose();
                ho_Image = null;
            }
            return ho_Image;
        }

        // 灰度图像转换
        private static HImage BitmapToHImageGray(Bitmap bitmap)
        {
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format8bppIndexed);

            try
            {
                byte[] imageData = new byte[bitmap.Width * bitmap.Height];
                Marshal.Copy(bitmapData.Scan0, imageData, 0, imageData.Length);

                HImage hImage = new HImage();
                hImage.GenImage1("byte", bitmap.Width, bitmap.Height, bitmapData.Scan0);

                return hImage;
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }

        // RGB图像转换 - 修正版本
        public static HImage BitmapToHImageRGB(Bitmap bitmap)
        {
            // 确保是24位或32位RGB格式
            Bitmap rgbBitmap;
            if (bitmap.PixelFormat != PixelFormat.Format24bppRgb &&
                bitmap.PixelFormat != PixelFormat.Format32bppRgb)
            {
                rgbBitmap = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format24bppRgb);
                using (Graphics g = Graphics.FromImage(rgbBitmap))
                {
                    g.DrawImage(bitmap, 0, 0);
                }
            }
            else
            {
                rgbBitmap = bitmap;
            }

            BitmapData bitmapData = rgbBitmap.LockBits(
                new Rectangle(0, 0, rgbBitmap.Width, rgbBitmap.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);

            try
            {
                int stride = bitmapData.Stride;
                int width = rgbBitmap.Width;
                int height = rgbBitmap.Height;

                // 分配非托管内存来存储分离的通道数据
                IntPtr rPtr = Marshal.AllocHGlobal(width * height);
                IntPtr gPtr = Marshal.AllocHGlobal(width * height);
                IntPtr bPtr = Marshal.AllocHGlobal(width * height);

                try
                {
                    byte[] rgbData = new byte[stride * height];
                    Marshal.Copy(bitmapData.Scan0, rgbData, 0, rgbData.Length);

                    // 分离RGB通道到非托管内存
                    byte[] rData = new byte[width * height];
                    byte[] gData = new byte[width * height];
                    byte[] bData = new byte[width * height];

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int index = y * width + x;
                            int rgbIndex = y * stride + x * 3;

                            rData[index] = rgbData[rgbIndex + 2];     // R
                            gData[index] = rgbData[rgbIndex + 1];     // G
                            bData[index] = rgbData[rgbIndex + 0];     // B
                        }
                    }

                    // 复制到非托管内存
                    Marshal.Copy(rData, 0, rPtr, rData.Length);
                    Marshal.Copy(gData, 0, gPtr, gData.Length);
                    Marshal.Copy(bData, 0, bPtr, bData.Length);

                    // 使用IntPtr创建HImage
                    HImage hImage = new HImage();
                    hImage.GenImage3("byte", width, height, rPtr, gPtr, bPtr);

                    return hImage;
                }
                finally
                {
                    // 释放非托管内存
                    Marshal.FreeHGlobal(rPtr);
                    Marshal.FreeHGlobal(gPtr);
                    Marshal.FreeHGlobal(bPtr);
                }
            }
            finally
            {
                rgbBitmap.UnlockBits(bitmapData);
                if (rgbBitmap != bitmap)
                {
                    rgbBitmap.Dispose();
                }
            }
        }


        // 更高效的RGB转换版本，减少内存复制
        //public static HImage BitmapToHImageRGB_Fast(Bitmap bitmap)
        //{
        //    // 转换为24位RGB格式
        //    Bitmap rgbBitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
        //                                   PixelFormat.Format24bppRgb);

        //    BitmapData bitmapData = rgbBitmap.LockBits(
        //        new Rectangle(0, 0, rgbBitmap.Width, rgbBitmap.Height),
        //        ImageLockMode.ReadOnly,
        //        PixelFormat.Format24bppRgb);

        //    try
        //    {
        //        int width = rgbBitmap.Width;
        //        int height = rgbBitmap.Height;
        //        int stride = bitmapData.Stride;

        //        // 分配非托管内存
        //        IntPtr rPtr = Marshal.AllocHGlobal(width * height);
        //        IntPtr gPtr = Marshal.AllocHGlobal(width * height);
        //        IntPtr bPtr = Marshal.AllocHGlobal(width * height);

        //        //unsafe
        //        {
        //            byte* src = (byte*)bitmapData.Scan0;
        //            byte* r = (byte*)rPtr;
        //            byte* g = (byte*)gPtr;
        //            byte* b = (byte*)bPtr;

        //            for (int y = 0; y < height; y++)
        //            {
        //                byte* row = src + y * stride;
        //                for (int x = 0; x < width; x++)
        //                {
        //                    int destIndex = y * width + x;
        //                    int srcIndex = x * 3;

        //                    b[destIndex] = row[srcIndex];     // B
        //                    g[destIndex] = row[srcIndex + 1]; // G
        //                    r[destIndex] = row[srcIndex + 2]; // R
        //                }
        //            }
        //        }

        //        HImage hImage = new HImage();
        //        hImage.GenImage3("byte", width, height, rPtr, gPtr, bPtr);

        //        // 释放非托管内存
        //        Marshal.FreeHGlobal(rPtr);
        //        Marshal.FreeHGlobal(gPtr);
        //        Marshal.FreeHGlobal(bPtr);

        //        return hImage;
        //    }
        //    finally
        //    {
        //        rgbBitmap.UnlockBits(bitmapData);
        //        rgbBitmap.Dispose();
        //    }
        //}
    }
}
