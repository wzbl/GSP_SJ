using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSP_SJ
{
    // 在窗体或控件中使用
    public partial class Form4 : Form
    {
        private DrawingCommandRecorder _recorder = new DrawingCommandRecorder();
        private Rectangle _drawingBounds = new Rectangle(0, 0, 10000, 10000); // 大尺寸

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // 记录绘图操作
            _recorder.RecordLine(new Point(0, 0), new Point(1000, 1000), Color.Red, 2);
            _recorder.RecordRectangle(new Rectangle(100, 100, 500, 300), Color.Blue, 3);
            _recorder.RecordString("Hello World", new PointF(200, 200), "Arial", 24, Color.Black);

            // 立即重放（在实际绘图中）
            _recorder.Replay(e.Graphics);
        }

        // 保存为二进制数据
        private void SaveDrawingData()
        {
            byte[] binaryData = _recorder.SerializeToBinary();
            File.WriteAllBytes("drawing_data.bin", binaryData);

            // 或者压缩保存
            byte[] compressed = CompressData(binaryData);
            File.WriteAllBytes("drawing_data.compressed", compressed);
        }

        // 加载并重绘
        private void LoadAndRedraw()
        {
            byte[] binaryData = File.ReadAllBytes("drawing_data.bin");
            var loadedRecorder = DrawingCommandRecorder.DeserializeFromBinary(binaryData);

            using (var graphics = this.CreateGraphics())
            {
                loadedRecorder.Replay(graphics);
            }
        }

        private byte[] CompressData(byte[] data)
        {
            using (var output = new MemoryStream())
            {
                using (var gzip = new GZipStream(output, CompressionMode.Compress))
                {
                    gzip.Write(data, 0, data.Length);
                }
                return output.ToArray();
            }
        }
    }
}
