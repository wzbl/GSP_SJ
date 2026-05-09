using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.MemoryMappedFiles;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing.Imaging;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Runtime.InteropServices;

namespace GSP
{
    /// <summary>
    /// 内存共享帮助类
    /// </summary>
    public class MemoryHelper
    {
        private static Mutex _mutex = new Mutex(false, "Global\\SharedBitmapMutex");
        /// <summary>
        /// 写入图像到共享内存
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="mmfName"></param>

        public static bool Write_SharedMemory_Bitmap(Bitmap bitmap, string mmfName)
        {
            bool result = false;
            try
            {
                _mutex.WaitOne(); // 同步锁

                byte[] bytes;
                using (MemoryStream ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Bmp); // 强制保存为 BMP 格式
                    bytes = ms.ToArray();
                }

                int sizeOfLen = sizeof(int);
                int totalLength = sizeOfLen + bytes.Length;

                // 尝试释放已存在的共享内存
                try
                {
                    using (var existingMmf = MemoryMappedFile.OpenExisting(mmfName, MemoryMappedFileRights.ReadWrite))
                    {
                        existingMmf.Dispose();
                    }
                }
                catch (FileNotFoundException) { }

                // 创建或打开共享内存
                using (var mmf = MemoryMappedFile.CreateOrOpen(
                    mmfName,
                    totalLength,
                    MemoryMappedFileAccess.ReadWrite,
                    MemoryMappedFileOptions.None,
                    HandleInheritability.None))
                {
                    using (var viewAccessor = mmf.CreateViewAccessor(0, totalLength))
                    {
                        viewAccessor.Write(0, bytes.Length);
                        viewAccessor.WriteArray(sizeOfLen, bytes, 0, bytes.Length);
                    }
                }

                result = true;
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"权限拒绝: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"IO错误: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"未知错误: {ex.ToString()}");
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
            return result;
        }

        /// <summary>
        /// 写入共享内存方法
        /// </summary>
        /// <param name="srcImgPtr"></param>
        /// <param name="Format"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <param name="mmfName"></param>
        /// <returns></returns>
        public static bool Write_SharedMemory(IntPtr srcImgPtr, int Format,  int Width,  int Height, string mmfName)
        {
            try
            {
                int byteSize =Width*Height*3;
                // 创建内存映射文件‌:ml-citation{ref="1" data="citationList"}
                using (var mmf = MemoryMappedFile.CreateOrOpen(mmfName, byteSize + 16))
                {
                    using (var writeEvent = new EventWaitHandle(false, EventResetMode.AutoReset, "BmpWriteEvent"))
                    {
                        // 写入元数据：宽度(4B)、高度(4B)、格式(4B)、数据长度(4B)
                        using (var accessor = mmf.CreateViewAccessor())
                        {
                            using (var mutex = new Mutex(false, $"Global\\{mmfName}_Mutex"))
                            {
                                if (mutex.WaitOne(TimeSpan.FromSeconds(5)))
                                {
                                    try
                                    {
                                        accessor.Write(0, Width);
                                        accessor.Write(4, Height);
                                        accessor.Write(8, Format);
                                        accessor.Write(12, byteSize);

                                        // 写入像素数据‌:ml-citation{ref="1" data="citationList"}
                                        using (var stream = mmf.CreateViewStream(16, byteSize))
                                        {
                                            //unsafe
                                            //{
                                            //    byte* ptr = (byte*)srcImgPtr.ToPointer();
                                            //    for (int i = 0; i < byteSize; i++)
                                            //    {
                                            //        stream.WriteByte(ptr[i]);
                                            //    }
                                            //}

                                            byte[] buffer = new byte[byteSize];
                                            Marshal.Copy(srcImgPtr, buffer, 0, byteSize);
                                            stream.Write(buffer, 0, byteSize);

                                        }
                                    }
                                    finally
                                    {
                                        mutex.ReleaseMutex();
                                        // 通知读取进程数据已就绪
                                        writeEvent.Set();
                                        Thread.Sleep(100);
                                    }

                                }
                            }

                        }
                    }
                }
            }
            catch { return false; }
            return true;
        }
        /// <summary>
        /// 从共享内存中读取图像
        /// </summary>
        /// <param name="mmfName"></param>
        /// <returns></returns>
        public static Bitmap Read_SharedMemory_Bitmap(string mmfName)
        {
            Bitmap bitmap = null;
            try
            {
                // 打开共享内存
                using (var mmf = MemoryMappedFile.OpenExisting(mmfName, MemoryMappedFileRights.Read))
                {
                    using (var viewAccessor = mmf.CreateViewAccessor())
                    {
                        using (var mutex = new Mutex(false, mmfName))
                        {
                            if (mutex.WaitOne(TimeSpan.FromSeconds(5)))
                            {
                                try
                                {
                                    // 读取图像长度
                                    int length = viewAccessor.ReadInt32(0);
                                    // 读取图像内容
                                    byte[] bytes = new byte[length];
                                    viewAccessor.ReadArray(sizeof(int), bytes, 0, length);

                                    // 将字节数组转换为 Bitmap
                                    using (MemoryStream ms = new MemoryStream(bytes))
                                    {
                                        bitmap = new Bitmap(ms);
                                    }
                                }
                                finally
                                {
                                    mutex.ReleaseMutex();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 记录错误日志
               
            }
            return bitmap;
        }

        public static Bitmap Read_SharedMemory_Bitmap1(string mmfName)
        {
            Bitmap bitmap = null;
            try
            {
                using (var writeEvent = new EventWaitHandle(false, EventResetMode.AutoReset, "BmpWriteEvent"))
                {
                    writeEvent.WaitOne(); //阻塞直到数据就绪
                    using (var mmf = MemoryMappedFile.OpenExisting(mmfName))
                    {
                        // 读取元数据‌:ml-citation{ref="1" data="citationList"}
                        int width, height, format, byteSize;
                        using (var accessor = mmf.CreateViewAccessor())
                        {
                            using (var mutex = new Mutex(false, $"Global\\{mmfName}_Mutex"))
                            {
                                if (mutex.WaitOne(TimeSpan.FromSeconds(5)))
                                {
                                    width = accessor.ReadInt32(0);
                                    height = accessor.ReadInt32(4);
                                    format = accessor.ReadInt32(8);
                                    byteSize = accessor.ReadInt32(12);

                                    // 创建Bitmap对象
                                    bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);

                                    // 读取像素数据‌:ml-citation{ref="1,6" data="citationList"}
                                    Rectangle rect = new Rectangle(0, 0, width, height);
                                    BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, bitmap.PixelFormat);
                                    try
                                    {
                                        using (var stream = mmf.CreateViewStream(16, byteSize))
                                        {
                                            //unsafe
                                            //{
                                            //    byte* dstPtr = (byte*)bmpData.Scan0.ToPointer();
                                            //    for (int i = 0; i < byteSize; i++)
                                            //    {
                                            //        dstPtr[i] = (byte)stream.ReadByte();
                                            //    }
                                            //}

                                            byte[] buffer = new byte[byteSize];
                                            stream.Read(buffer, 0, byteSize);
                                            Marshal.Copy(buffer, 0, bmpData.Scan0, byteSize); // 块复制优化‌
                                        }
                                    }
                                    finally
                                    {
                                        bitmap.UnlockBits(bmpData);
                                        mutex.ReleaseMutex();
                                    }
                                }

                            }
                        }
                        return bitmap;
                    }
                }
            }
            catch { return null; }

        }
    }
}
