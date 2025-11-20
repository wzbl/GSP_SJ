using SqlHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSP_SJ
{
    public partial class Form3 : Form
    {

        ZoomablePictureBox pictureBoxZoom1;
        ZoomablePictureBox pictureBoxZoom2;
        public Form3(ComponentCanvas canva)
        {
            InitializeComponent();
            try
            {
                pictureBoxZoom1 = new ZoomablePictureBox();
                 Bitmap image = new Bitmap("C:\\Users\\14802\\Desktop\\1.bmp");
                 pictureBoxZoom1.Image = image;
                this.panel2.Controls.Add(pictureBoxZoom1);

                pictureBoxZoom2=new ZoomablePictureBox  ();
                Bitmap image2 = new Bitmap("C:\\Users\\14802\\Desktop\\2.jpg");
                pictureBoxZoom2.Image = image2;
                panel3.Controls.Add(pictureBoxZoom2);
            }
            catch (Exception ex)
            {

            }

            this.Load+=Form3_Load;
        }

        public Form3(List<P_Search_Eng_XYData_Result> eng_XYData_Results)
        {
            InitializeComponent();
            try
            {
                pictureBoxZoom1 = new ZoomablePictureBox();
                Bitmap image = PublicFunction.ByteToBitmap(SQLDataControl.GetProgramOptionPicture(eng_XYData_Results[0].产品编码));
                pictureBoxZoom1.Image = image;
                this.panel2.Controls.Add(pictureBoxZoom1);

                pictureBoxZoom2 = new ZoomablePictureBox();
                Bitmap image2 = new Bitmap("C:\\Users\\14802\\Desktop\\2.jpg");
                pictureBoxZoom2.Image = image2;
                panel3.Controls.Add(pictureBoxZoom2);

                dataGridView1.DataSource = eng_XYData_Results;
                foreach (var item in eng_XYData_Results)
                {
                    Size size = new Size(100, 100);
                    Color color = Color.HotPink;

                    string _x = item.屏幕坐标X.ToString();
                    string _y = item.屏幕坐标Y.ToString();

                    if (float.TryParse(_x.Trim().ToString(), out float x) && float.TryParse(_y.Trim().ToString(), out float y))
                    {
                        float angle = 0;
                        if (float.TryParse(item.角度.ToString(), out
                            angle))
                        {

                        }
                        Component component = new Component(item.元件位置, new PointF(x, y), size, angle, "null", color);
                        pictureBoxZoom1.components.Add(component);
                    }


                }
            }
            catch (Exception ex)
            {

            }

            this.Load += Form3_Load;
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            //DataTable dataTable = SqlHelper.SQL.Excute("select * from Eng_XYData where ProductCode ='1212'");
            //foreach (DataRow row in dataTable.Rows)
            //{
            //    try
            //    {
            //        if (row.ItemArray.Length > 10)
            //        {
            //            Size size = new Size(100, 100);
            //            Color color = Color.HotPink;
            //            string _x = row.ItemArray[11].ToString();
            //            string _y = row.ItemArray[12].ToString();

            //            if (float.TryParse(_x.Trim().ToString(), out float x) && float.TryParse(_y.Trim().ToString(), out float y))
            //            {
            //                float angle = 0;
            //                if (float.TryParse(row.ItemArray[10].ToString(), out angle))
            //                {

            //                }
            //                Component component = new Component(row.ItemArray[1].ToString(), new PointF(x , y), new SizeF(size.Width, size.Height), angle, "null", color);
            //                pictureBoxZoom1.components.Add(component);
            //            }
            //        }
            //    }
            //    catch (Exception)
            //    {



            //    }
            //}
            //dataGridView1.DataSource = dataTable;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string pos = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
             
                pictureBoxZoom1.setSelectCompont(pos,panel2.Width,panel2.Height);
            }
        }
    }

}
