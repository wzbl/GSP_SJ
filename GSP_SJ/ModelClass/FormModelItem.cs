using ComponentFactory.Krypton.Toolkit;
using Emgu.CV.OCR;
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
    public partial class FormModelItem : KryptonForm
    {
        private string MaterialCode = "";

        private string MaterialName = "";

        private string ProducerCode = "";

        private string LCRType = "";

   

        public FormModelItem(string MaterialCode, string description, string producerCode, Image image, string LCRType)
        {
            InitializeComponent();
            this.MaterialCode = MaterialCode;
            Text = "[" + MaterialCode + "]描述[" + description + "]";
            this.Load += FormModelItem_Load;
            ProducerCode = producerCode;
            roiPictureBox1.Width = image.Width; roiPictureBox1.Height = image.Height;
            org = image;
            //org = ResizeImage(image, roiPictureBox1.Width, roiPictureBox1.Height); ;
    
            this.LCRType = LCRType;
            MaterialName = description;
        }

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
            IsPolarity.Checked = true;
            IsManual.Checked = true;
            NB_0.Checked = true;
            Search();
         
            if (dataGridView1.Rows.Count == 0 && org != null)
            {
                roiPictureBox1.Image = org;
                int w =  org.Width / 5;
                int h = org.Height / 5;
                Rectangle rectangle = new Rectangle(
                w,
                h,
                org.Width - (w*2),
                org.Height - (h*2));
                DirectionalROI rOI = new DirectionalROI(rectangle, "");
                roiPictureBox1.AddROI(rOI);


                //显示比对结果
                DispMatchResult();
            }

            dataGridView1.CellClick += dataGridView1_CellClick;
        }

        private void Search()
        {
            dataGridView1.Rows.Clear();
            if (string.IsNullOrEmpty(ProducerCode))
            {
                eng_PubModelItems = SQLDataControl.GETEng_PubModelItem(MaterialCode);
                if (eng_PubModelItems != null && eng_PubModelItems.Count > 0)
                {
                    foreach (var item in eng_PubModelItems)
                    {
                        dataGridView1.Rows.Add
                            (
                            item.ID,
                            false,
                            item.Creator,
                            item.CreationDate);
                    }
                }
            }
            else
            {
                eng_ModelItems = SQLDataControl.GetEng_ModelItem(ProducerCode, MaterialCode);
                if (eng_ModelItems != null && eng_ModelItems.Count > 0)
                {
                    foreach (var item in eng_ModelItems)
                    {
                        dataGridView1.Rows.Add
                            (
                            item.Id,
                            item.IsMain,
                            item.Creator,
                            item.CreationDate);
                    }

                    byte[] img = eng_ModelItems[eng_ModelItems.Count-1].mPicture;
                    MemoryStream memoryStream = new MemoryStream(img);
                    Image pic = Image.FromStream(memoryStream);

                    roiPictureBox1.Width = (int)(eng_ModelItems[eng_ModelItems.Count - 1].mPW * eng_ModelItems[eng_ModelItems.Count - 1].mZoomRatio);
                    roiPictureBox1.Height = (int)(eng_ModelItems[eng_ModelItems.Count - 1].mPH * eng_ModelItems[eng_ModelItems.Count - 1].mZoomRatio);
                    org = ResizeImage(pic, roiPictureBox1.Width, roiPictureBox1.Height);
                    roiPictureBox1.Image = org;
                    roiPictureBox1.ClearROIs();
                    int left = eng_ModelItems[eng_ModelItems.Count - 1].PLeft != null ? (int)eng_ModelItems[eng_ModelItems.Count - 1].PLeft : 30;
                    int top = eng_ModelItems[eng_ModelItems.Count - 1].PTop != null ? (int)eng_ModelItems[eng_ModelItems.Count - 1].PTop : 30;
                    int pw = eng_ModelItems[eng_ModelItems.Count - 1].Pw != null ? (int)eng_ModelItems[eng_ModelItems.Count - 1].Pw : 100;
                    int ph = eng_ModelItems[eng_ModelItems.Count - 1].Ph != null ? (int)eng_ModelItems[eng_ModelItems.Count - 1].Ph : 100;
                    Rectangle rectangle = new Rectangle(
                        left,
                        top,
                        pw,
                        ph);
                    DirectionalROI rOI = new DirectionalROI(rectangle, "0");
                    roiPictureBox1.AddROI(rOI);

                    DispPlayParam(eng_ModelItems[eng_ModelItems.Count - 1]);

                }
            }

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                dataGridView1.Rows[e.RowIndex].Selected = true;
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
            IsPolarity.Checked = (bool)pubModelItem.Polarity;
            IsManual.Checked = (bool)pubModelItem.IsManual;
            switch (pubModelItem.NA)
            {
                case 0:
                    NA_B.Checked = true;
                    NA_G.Checked = true;
                    NA_R.Checked = true;
                    break;
                case 1:
                    NA_B.Checked = true;
                    NA_G.Checked = false;
                    NA_R.Checked = false;
                    break;
                case 2:
                    NA_B.Checked = false;
                    NA_G.Checked = true;
                    NA_R.Checked = false;
                    break;
                case 3:
                    NA_B.Checked = false;
                    NA_G.Checked = false;
                    NA_R.Checked = true;
                    break;
                case 4:
                    NA_B.Checked = true;
                    NA_G.Checked = true;
                    NA_R.Checked = false;
                    break;
                case 5:
                    NA_B.Checked = true;
                    NA_G.Checked = false;
                    NA_R.Checked = true;
                    break;
                case 6:
                    NA_B.Checked = false;
                    NA_G.Checked = true;
                    NA_R.Checked = true;
                    break;
            }
            switch (pubModelItem.NB)
            {
                case 0:
                    NB_0.Checked = true;
                    break;
                case 1:
                    NB_1.Checked = true;
                    break;
                case 2:
                    NB_2.Checked = true;
                    break;
            }
            trackLight_XB.Value = (int)(pubModelItem.XB);
            txtXB.Text = pubModelItem.XB.ToString();
            trackcontrack_XA.Value = (int)(pubModelItem.XA);
            txtXA.Text = pubModelItem.XA.ToString();
            P1.Value = (int)(pubModelItem.P1);
            txtP1.Text = pubModelItem.P1.ToString();
            P2.Value = (int)(pubModelItem.P2);
            txtP2.Text = pubModelItem.P2.ToString();
            LCy.Value = (int)(pubModelItem.LCy);
            txtLCy.Text = pubModelItem.LCy.ToString();
            remarks.Text = pubModelItem.Remarks;
        }

        public void DispMatchResult()
        {
            ResultPanel.Controls.Clear();
            for (int i = 0; i < roiPictureBox1.ROIs.Count; i++)
            {
                UCOCRModel uCOCRModel1 = new UCOCRModel(roiPictureBox1.Image, roiPictureBox1.ROIs[i]);
                ResultPanel.Controls.Add(uCOCRModel1);
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
        private async void trackLight_Scroll(object sender, EventArgs e)
        {
            lock (lockObj)
            {
                ScaleImage();
            }
        }

        private void ScaleImage()
        {
            double factor = 3;

            if (trackLight_XB.Value < 0)
            {
                factor = 0.001;
            }

            using (HImage hImage = HalconImageConverter.BitmapToHImageRGB(new Bitmap(org)))
            {
                HalconDotNet.HOperatorSet.ScaleImage(hImage, out HObject imageScaled, factor, (int)Math.Abs(trackLight_XB.Value));

                roiPictureBox1.Image = HalconImageConverter.HImageToBitmapRGB(new HImage(imageScaled));
            }
            txtXB.Text = trackLight_XB.Value.ToString();
            RefreshOcvData();
        }

        /// <summary>
        /// 对比度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void trackcontrack_Scroll(object sender, EventArgs e)
        {
            lock (lockObj)
            {
                using (HImage hImage = HalconImageConverter.BitmapToHImageRGB(new Bitmap(org)))
                {
                    //HalconDotNet.HOperatorSet.Emphasize(hImage, out HObject ho_ImageScaled, org.Width, org.Height, (int)Math.Abs(trackcontrack.Value));
                    int val = (int)Math.Abs(trackcontrack_XA.Value);
                    PublicFunction.scale_image_range(hImage, out HObject ho_ImageScaled, 50, val);
                    if (trackcontrack_XA.Value < 0)
                    {
                        HalconDotNet.HOperatorSet.InvertImage(ho_ImageScaled, out ho_ImageScaled);
                    }
                    roiPictureBox1.Image = HalconImageConverter.HImageToBitmapRGB(new HImage(ho_ImageScaled));
                }
                txtXA.Text = trackcontrack_XA.Value.ToString();
                RefreshOcvData();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ProducerCode))
            {
                //公共模型
            }
            else
            {
                //Eng_Model ，Eng_ModelItem

                Eng_Model eng_Model = new Eng_Model();
                eng_Model.ProductCode = ProducerCode;
                eng_Model.MaterialCode = MaterialCode;
                eng_Model.MaterialName = MaterialName;
                eng_Model.LCRType = LCRType;
                eng_Model.Creator = Global.User.UserName;
                eng_Model.CreationDate = DateTime.Now;
                SQLDataControl.UpdateEng_Model(eng_Model);

                Eng_ModelItem eng_ModelItem = new Eng_ModelItem();
                eng_ModelItem.ProductCode = ProducerCode;
                eng_ModelItem.MaterialCode = MaterialCode;
                eng_ModelItem.Id = dataGridView1.SelectedRows.Count == 0 ? 1 : int.Parse(dataGridView1.SelectedRows[0].Cells[0].Value.ToString());
                eng_ModelItem.mPicture = PublicFunction.CompressImage(roiPictureBox1.Image, "", 100, roiPictureBox1.Image.Width, roiPictureBox1.Image.Height);
                eng_ModelItem.mPW = roiPictureBox1.Image.Width;
                eng_ModelItem.mPH = roiPictureBox1.Image.Height;
                eng_ModelItem.mZoomRatio = 1;
                eng_ModelItem.mDPI = 300;
                eng_ModelItem.Groups = 1;
                eng_ModelItem.IsMain = dataGridView1.SelectedRows.Count == 0 ? false : bool.Parse(dataGridView1.SelectedRows[0].Cells[1].Value.ToString());
                eng_ModelItem.Polarity = IsPolarity.Checked;
                eng_ModelItem.IsManual = IsManual.Checked;
                for (int i = 0; i < roiPictureBox1.ROIs.Count; i++)
                {
                    if (i == 0)
                    {
                        eng_ModelItem.PLeft = roiPictureBox1.ROIs[i].Bounds.X;
                        eng_ModelItem.PTop = roiPictureBox1.ROIs[i].Bounds.Y;
                        eng_ModelItem.Pw = roiPictureBox1.ROIs[i].Bounds.Width;
                        eng_ModelItem.Ph = roiPictureBox1.ROIs[i].Bounds.Height;
                    }
                    else if (i == 1)
                    {
                        eng_ModelItem.PLeft2 = roiPictureBox1.ROIs[i].Bounds.X;
                        eng_ModelItem.PTop2 = roiPictureBox1.ROIs[i].Bounds.Y;
                        eng_ModelItem.Pw2 = roiPictureBox1.ROIs[i].Bounds.Width;
                        eng_ModelItem.Ph2 = roiPictureBox1.ROIs[i].Bounds.Height;
                    }
                    else
                    {
                        eng_ModelItem.PLeft3 = roiPictureBox1.ROIs[i].Bounds.X;
                        eng_ModelItem.PTop3 = roiPictureBox1.ROIs[i].Bounds.Y;
                        eng_ModelItem.Pw3 = roiPictureBox1.ROIs[i].Bounds.Width;
                        eng_ModelItem.Ph3 = roiPictureBox1.ROIs[i].Bounds.Height;
                    }
                }
                if (NA_B.Checked && NA_G.Checked && NA_R.Checked)
                    eng_ModelItem.NA = 0;
                else if (NA_B.Checked && !NA_G.Checked && !NA_R.Checked)
                    eng_ModelItem.NA = 1;
                else if (!NA_B.Checked && NA_G.Checked && !NA_R.Checked)
                    eng_ModelItem.NA = 2;
                else if (!NA_B.Checked && !NA_G.Checked && NA_R.Checked)
                    eng_ModelItem.NA = 3;
                else if (NA_B.Checked && NA_G.Checked && !NA_R.Checked)
                    eng_ModelItem.NA = 4;
                else if (NA_B.Checked && !NA_G.Checked && NA_R.Checked)
                    eng_ModelItem.NA = 5;
                else if (!NA_B.Checked && NA_G.Checked && NA_R.Checked)
                    eng_ModelItem.NA = 6;

                if (NB_0.Checked)
                    eng_ModelItem.NB = 0;
                else if (NB_1.Checked)
                    eng_ModelItem.NB = 1;
                else
                    eng_ModelItem.NB = 2;
                eng_ModelItem.XA = trackcontrack_XA.Value;
                eng_ModelItem.XB = trackLight_XB.Value;
                eng_ModelItem.P1 = P1.Value;
                eng_ModelItem.P2 = P2.Value;
                eng_ModelItem.LCy = LCy.Value;
                eng_ModelItem.HCy = 100;
                eng_ModelItem.Remarks = remarks.Text;
                eng_ModelItem.Creator = Global.User.UserName;
                eng_ModelItem.CreationDate = DateTime.Now;
                SQLDataControl.UpdateEng_ModelItem(eng_ModelItem);
            }
            Search();
        }

        #region 图片旋转

        private void btnRotateImageLeft_Click(object sender, EventArgs e)
        {
            if (org != null)
            {
                org = PublicFunction.RotateImage(org, (int)-kryptonNumericUpDown1.Value);
                roiPictureBox1.Image = org;
            }
        }

        private void btnRotateImageRight_Click(object sender, EventArgs e)
        {
            if (org != null)
            {
                org = PublicFunction.RotateImage(org, (int)kryptonNumericUpDown1.Value);
                roiPictureBox1.Image = org;
            }
        }

        #endregion

        private void btnDeleteModel_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                SQLDataControl.DeleteEng_ModelItem(ProducerCode, MaterialCode, int.Parse(dataGridView1.SelectedRows[0].Cells[0].Value.ToString()));
                Search();
            }
        }
    }
}
