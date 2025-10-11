using MyTestWcfServiceLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace GSP_SJ
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        ServiceReference1.MyFirstServiceClient client = new ServiceReference1.MyFirstServiceClient();

        private void btnOpenServices_Click(object sender, EventArgs e)
        {
            SqlHelper.SQL.ConnectSqlSever(@"DESKTOP-9F1CI68\SQLEXPRESS", "FAI_new", "sa", "123456");
            SqlHelper.SQL.InsertIMG(textBox1.Text,textBox2.Text);
        }

        private void btnCloseServices_Click(object sender, EventArgs e)
        {
            SqlHelper.SQL.ConnectSqlSever(@"DESKTOP-9F1CI68\SQLEXPRESS", "FAI_new", "sa", "123456");
            MemoryStream mystream = new MemoryStream(SqlHelper.SQL.SelectIMG());
            //用指定的数据流来创建一个image图片
            System.Drawing.Image img = System.Drawing.Image.FromStream(mystream, true);

            pictureBox1.Image = ResizeImage(img, pictureBox1.Width,pictureBox1.Height);
        }

        public Bitmap ResizeImage(Image image, int maxWidth, int maxHeight)
        {
            try
            {
                double ratio = Math.Min((double)maxWidth / image.Width, (double)maxHeight / image.Height);
                int newWidth = (int)(image.Width * ratio);
                int newHeight = (int)(image.Height * ratio);

                var destImage = new Bitmap(newWidth, newHeight);
                using (var graphics = Graphics.FromImage(destImage))
                {
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.DrawImage(image, 0, 0, newWidth, newHeight);
                }
                return destImage;
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}
