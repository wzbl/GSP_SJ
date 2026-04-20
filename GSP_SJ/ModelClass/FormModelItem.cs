using HalconDotNet;
using SqlHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSP_SJ.ModelClass
{
    public partial class FormModelItem : Form
    {
        private string MaterialCode = "";

        private string ProducerCode = "";
        public FormModelItem(string MaterialCode, string description, string producerCode = "")
        {
            InitializeComponent();
            this.MaterialCode = MaterialCode;
            Text = "[" + MaterialCode + "]描述[" + description + "]";
            this.Load += FormModelItem_Load;
            ProducerCode = producerCode;
        }
        List<Eng_PubModelItem> eng_PubModelItems;
        List<Eng_ModelItem> eng_ModelItems;

        private Image org = null;
        private void FormModelItem_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ProducerCode))
            {
                eng_PubModelItems = SQLDataControl.GETEng_PubModelItem(MaterialCode);
                if (eng_PubModelItems != null && eng_PubModelItems.Count > 0)
                {
                    dataGridView1.DataSource = eng_PubModelItems;
                }
            }
            else
            {
                eng_ModelItems = SQLDataControl.GetEng_ModelItem(ProducerCode,MaterialCode);
                if (eng_ModelItems != null && eng_ModelItems.Count > 0)
                {
                    dataGridView1.DataSource = eng_ModelItems;
                }
            }

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (string.IsNullOrEmpty(ProducerCode))
                {
                    byte[] img = eng_PubModelItems[e.RowIndex].mPicture;
                    MemoryStream memoryStream = new MemoryStream(img);
                    Image pic = Image.FromStream(memoryStream);

                    roiPictureBox1.Width = (int)(eng_PubModelItems[e.RowIndex].mPW * eng_PubModelItems[e.RowIndex].mZoomRatio);
                    roiPictureBox1.Height = (int)(eng_PubModelItems[e.RowIndex].mPH * eng_PubModelItems[e.RowIndex].mZoomRatio);
                    org = ResizeImage(pic, roiPictureBox1.Width, roiPictureBox1.Height);
                    roiPictureBox1.Image = org;
                    roiPictureBox1.ClearROIs();
                    int left = eng_PubModelItems[e.RowIndex].PLeft != null ? (int)eng_PubModelItems[e.RowIndex].PLeft : 30;
                    int top = eng_PubModelItems[e.RowIndex].PTop != null ? (int)eng_PubModelItems[e.RowIndex].PTop : 30;
                    int pw = eng_PubModelItems[e.RowIndex].Pw != null ? (int)eng_PubModelItems[e.RowIndex].Pw : 100;
                    int ph = eng_PubModelItems[e.RowIndex].Ph != null ? (int)eng_PubModelItems[e.RowIndex].Ph : 100;
                    Rectangle rectangle = new Rectangle(
                        left,
                        top,
                        pw,
                        ph);


                    DirectionalROI rOI = new DirectionalROI(rectangle, e.RowIndex.ToString());
                    roiPictureBox1.AddROI(rOI);

                    //显示模板参数
                    DispPlayParamPub(eng_PubModelItems[e.RowIndex]);
                }
                else
                {
                    byte[] img = eng_ModelItems[e.RowIndex].mPicture;
                    MemoryStream memoryStream = new MemoryStream(img);
                    Image pic = Image.FromStream(memoryStream);

                    roiPictureBox1.Width = (int)(eng_ModelItems[e.RowIndex].mPW * eng_ModelItems[e.RowIndex].mZoomRatio);
                    roiPictureBox1.Height = (int)(eng_ModelItems[e.RowIndex].mPH * eng_ModelItems[e.RowIndex].mZoomRatio);
                    org = ResizeImage(pic, roiPictureBox1.Width, roiPictureBox1.Height);
                    roiPictureBox1.Image = org;
                    roiPictureBox1.ClearROIs();
                    int left = eng_ModelItems[e.RowIndex].PLeft != null ? (int)eng_ModelItems[e.RowIndex].PLeft : 30;
                    int top = eng_ModelItems[e.RowIndex].PTop != null ? (int)eng_ModelItems[e.RowIndex].PTop : 30;
                    int pw = eng_ModelItems[e.RowIndex].Pw != null ? (int)eng_ModelItems[e.RowIndex].Pw : 100;
                    int ph = eng_ModelItems[e.RowIndex].Ph != null ? (int)eng_ModelItems[e.RowIndex].Ph : 100;
                    Rectangle rectangle = new Rectangle(
                        left,
                        top,
                        pw,
                        ph);


                    DirectionalROI rOI = new DirectionalROI(rectangle, e.RowIndex.ToString());
                    roiPictureBox1.AddROI(rOI);

                    //显示模板参数
                    DispPlayParam(eng_ModelItems[e.RowIndex]);
                }
               

                //显示比对结果
                DispMatchResult();
            }
        }

        public void DispPlayParamPub(Eng_PubModelItem pubModelItem)
        {

        }

        public void DispPlayParam(Eng_ModelItem pubModelItem)
        {

        }

        public void DispMatchResult()
        {
            splitContainer2.Panel2.Controls.Clear();
            for (int i = 0; i < roiPictureBox1.ROIs.Count; i++)
            {
                UCOCRModel uCOCRModel1 = new UCOCRModel(roiPictureBox1.Image, roiPictureBox1.ROIs[i]);
                splitContainer2.Panel2.Controls.Add(uCOCRModel1);
            }
        }

        public void RefreshOcvData()
        {
            for (int i = 0; i < roiPictureBox1.ROIs.Count; i++)
            {
                roiPictureBox1.ROIs[i].ChangeROI?.Invoke(roiPictureBox1.Image, roiPictureBox1.ROIs[i]);
            }
        }

        public Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        /// <summary>
        /// 新增ROI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddRoi_Click(object sender, EventArgs e)
        {
            Rectangle rectangle = new Rectangle(
                 10,
                 10,
                 40,
                 40);

            DirectionalROI rOI = new DirectionalROI(rectangle);
            roiPictureBox1.AddROI(rOI);

            DispMatchResult();
        }

        /// <summary>
        /// 删除选择ROI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteRoi_Click(object sender, EventArgs e)
        {
            roiPictureBox1.DeleteSelectedROI();
            DispMatchResult();
        }

        object lockObj = new object();

        /// <summary>
        /// 亮度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackLight_Scroll(object sender, EventArgs e)
        {
            lock (lockObj)
            {
                ScaleImage();
            }
        }

        private void ScaleImage()
        {
            double factor = 3;
            if (trackLight.Value < 0)
            {
                factor = 0.001;
            }
            using (HImage hImage = HalconImageConverter.BitmapToHImageRGB(new Bitmap(org)))
            {
                HalconDotNet.HOperatorSet.ScaleImage(hImage, out HObject imageScaled, factor, (int)Math.Abs(trackLight.Value));

                roiPictureBox1.Image = HalconImageConverter.HImageToBitmapRGB(new HImage(imageScaled));
            }

            RefreshOcvData();
        }

        /// <summary>
        /// 对比度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackcontrack_Scroll(object sender, EventArgs e)
        {
            lock (lockObj)
            {
                using (HImage hImage = HalconImageConverter.BitmapToHImageRGB(new Bitmap(org)))
                {
                    //HalconDotNet.HOperatorSet.Emphasize(hImage, out HObject ho_ImageScaled, org.Width, org.Height, (int)Math.Abs(trackcontrack.Value));
                    int val = (int)Math.Abs(trackcontrack.Value);
                    PublicFunction.scale_image_range(hImage, out HObject ho_ImageScaled, 50, val);
                    if (trackcontrack.Value < 0)
                    {
                        HalconDotNet.HOperatorSet.InvertImage(ho_ImageScaled, out ho_ImageScaled);
                    }
                    roiPictureBox1.Image = HalconImageConverter.HImageToBitmapRGB(new HImage(ho_ImageScaled));
                }
                RefreshOcvData();
            }
        }

    }
}
