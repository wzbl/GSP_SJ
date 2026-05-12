using ComponentFactory.Krypton.Toolkit;
using HalconDotNet;
using SqlHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSP_SJ.ModelClass
{
    public partial class FormResCheck : KryptonForm
    {
        private string reportCode = "";
        private string productCode = "";
        private Image originalImage = null;
        private List<Component> Components;
        List<Man_ReportItem> R_Item;
        HImage hImage;

        public FormResCheck()
        {
            InitializeComponent();
            R_Item = DBEventAction.man_ReportItems.Where(x => x.LcrType == "R").ToList();
        }

        public FormResCheck(string reportCode, string productCode, Image bigImage, List<Component> components) : this()
        {
            this.reportCode = reportCode;
            this.productCode = productCode;
            this.originalImage = bigImage;
            Components = components;
            this.Load += Loaded;
        }

        private void Loaded(object sender, EventArgs e)
        {
            Run();
        }

        private async void Run()
        {
            await Task.Run(() =>
            {
                foreach (var item in R_Item)
                {
                    RoiImg(item);
                    if (hImage != null)
                    {
                        item.CheckType = "OCR";
                        item.CheckResult = "FAIL";
                        item.CreationDate = DateTime.Now;
                        item.Creator = DBEventAction.User.UserName;
                        try
                        {
                            this.Invoke(new Action(() =>
                            {
                                Bitmap img = HalconImageConverter.HImageToBitmapRGB(hImage);
                                pictureBox1.Image = img;
                                txtPos.Text = item.Position;
                                txtAngle.Text = item.Angle.ToString();
                                txtSize.Text = item.Size;
                                txtStandOCR.Text = item.StandardOcrStr;
                                GetOCRText(hImage);
                            }));
                        }
                        catch (Exception ex)
                        {


                        }

                    }
                }
            });
            this.Close();
        }


        private void GetOCRText(HObject ho_Imag)
        {
            List<string> ocrs = new List<string>();
            try
            {
                ocrs = Browocrlib.OCRHelper.GetOCR(HImageToEmguCVConverter.HImageToMat(new HImage(ho_Imag)), 6);
            }
            catch (Exception)
            {

            }

            foreach (var item in ocrs)
            {
                txtResult.Text = item;
            }
        }

        private void RoiImg(Man_ReportItem item)
        {
            hImage = null;
            HObject hObject, ho_ImageReduced, ho_findCircleRoi = null;
            HOperatorSet.GenEmptyObj(out hObject);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            try
            {
                Point point = new Point((int)item.LX, (int)item.LY);

                Component clickedComponent = Components.Where(x => x.Designator == item.Position).FirstOrDefault();

                Rectangle rOI = new Rectangle((int)(point.X - clickedComponent.Size.Width / 2), (int)(point.Y - clickedComponent.Size.Height / 2), (int)clickedComponent.Size.Width, (int)clickedComponent.Size.Height);

                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    //HOperatorSet.GenRectangle2(out ho_findCircleRoi, rOI.X, rOI.Y, clickedComponent.Angle,
                    //                                                        rOI.Height/2, rOI.Width/2);
                    HOperatorSet.GenRectangle1(out ho_findCircleRoi, rOI.Top - 50, rOI.Left - 50,
                                                                          rOI.Bottom + 50, rOI.Right + 50);
                    HOperatorSet.ReduceDomain(HalconImageConverter.BitmapToHImage2(new Bitmap(originalImage)), ho_findCircleRoi, out hObject);
                    HOperatorSet.ChangeDomain(hObject, ho_findCircleRoi, out ho_ImageReduced);
                    HOperatorSet.CropDomain(ho_ImageReduced, out ho_ImageReduced);
                    ho_ImageReduced = PublicFunction.RotateImage(ho_ImageReduced, double.Parse(item.Angle.ToString()));
                    hImage = new HImage(ho_ImageReduced);
                    HOperatorSet.GetImageSize(hImage, out HTuple width, out HTuple height);
                    if (width > 60 && height > 60)
                    {
                        HOperatorSet.GenRectangle1(out ho_findCircleRoi, 40, 40,
                                                                      height - 40, width - 40);
                        HOperatorSet.ChangeDomain(hImage, ho_findCircleRoi, out ho_ImageReduced);
                        HOperatorSet.CropDomain(ho_ImageReduced, out ho_ImageReduced);
                        hImage = new HImage(ho_ImageReduced);
                    }

                    //hImage.WriteImage("bmp", 0, "C:\\Users\\14802\\Desktop\\" + clickedComponent.Designator + ".bmp");
                }
            }
            catch (Exception)
            {

            }

            ho_findCircleRoi.Dispose();
            hObject.Dispose();
            ho_ImageReduced.Dispose();
        }
    }
}
