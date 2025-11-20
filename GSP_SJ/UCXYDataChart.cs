using SqlHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSP_SJ
{
    public partial class UCXYDataChart : UserControl
    {
        public ComponentCanvas canvas;

        public UCXYDataChart()
        {
            InitializeComponent();
            this.Load += UCXYDataChart_Load;
            Dock = DockStyle.Fill;
        }
        List<P_Search_Eng_XYData_Result> eng_XYData_Results;
        public void RefreshData(List<P_Search_Eng_XYData_Result> eng_XYData_Results)
        {
            this.eng_XYData_Results = eng_XYData_Results;
            canvas.Components.Clear();
            foreach (var item in eng_XYData_Results)
            {


                Size size = new Size(86, 55);
                Color color = Color.HotPink;
                if (item.元件尺寸.Contains("01005"))
                {
                    size = new Size(16, 9);
                    color = Color.BlueViolet;
                }
                else if (item.元件尺寸.Contains("0201"))
                {
                    size = new Size(25, 13);
                    color = Color.DarkCyan;
                }
                else if (item.元件尺寸.Contains("0402"))
                {
                    size = new Size(43, 23);
                    color = Color.Blue;
                }
                else if (item.元件尺寸.Contains("0603"))
                {
                    size = new Size(68, 37);
                    color = Color.Aquamarine;
                }
                else if (item.元件尺寸.Contains("0805"))
                {
                    size = new Size(86, 55);
                    color = Color.Cyan;
                }
                else if (item.元件尺寸.Contains("1206"))
                {
                    size = new Size(133, 68);
                    color = Color.Brown;
                }
                else if (item.元件位置.Contains("MARK"))
                {
                    size = new Size(68, 37);
                    color = Color.DarkGreen;
                }
                string _x = item.X坐标.ToString();
                string _y = item.Y坐标.ToString();

                if (float.TryParse(_x.Trim().ToString(), out float x) && float.TryParse(_y.Trim().ToString(), out float y))
                {
                    float angle = 0;
                    if (float.TryParse(item.角度.ToString(), out
                        angle))
                    {

                    }
                    Component component = new Component(item.元件位置, new PointF(x * Component.radio, (-y) * Component.radio), new SizeF(size.Width * Component.radio / 25.0f, size.Height * Component.radio / 25.0f), angle, "null", color);
                    canvas.Components.Add(component);
                }


            }
        }

        private void UCXYDataChart_Load(object sender, EventArgs e)
        {
            canvas = new ComponentCanvas();
            canvas.Dock = DockStyle.Fill;
            this.Controls.Add(canvas);

            // 添加工具栏
            AddToolbar();
        }


        private void AddToolbar()
        {
            ToolStrip toolStrip = new ToolStrip();

            //    // 缩放控制
            ToolStripLabel scaleLabel = new ToolStripLabel("缩放:");
            ToolStripTextBox scaleBox = new ToolStripTextBox();
            scaleBox.Text = "100%";
            scaleBox.Width = 60;
            scaleBox.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    if (scaleBox.Text.EndsWith("%"))
                    {
                        string percentStr = scaleBox.Text.TrimEnd('%');
                        if (float.TryParse(percentStr, out float percent))
                        {
                            canvas.Scale = percent / 100.0f;
                        }
                    }
                }
            };
            ToolStripButton toolStrip1 = new ToolStripButton("重置视图");
            toolStrip1.Click += (s, e) => canvas.ResetView();
            ToolStripButton toolStrip2 = new ToolStripButton("适合视图");
            toolStrip2.Click += (s, e) => canvas.FitToView();
            ToolStripButton toolStrip3 = new ToolStripButton("水平X镜像");//原点X轴对称
            toolStrip3.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {
                    canvas.Components[i].Position = new PointF(-canvas.Components[i].Position.X, canvas.Components[i].Position.Y);
                }
                canvas.FitToView();
            };
            ToolStripButton toolStrip4 = new ToolStripButton("水平Y镜像");//原点Y轴对称
            toolStrip4.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {
                    canvas.Components[i].Position = new PointF(canvas.Components[i].Position.X, -canvas.Components[i].Position.Y);
                }
                canvas.FitToView();
            };

            ToolStripButton toolStrip5 = new ToolStripButton("逆时针90");
            toolStrip5.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {
                    canvas.RotateCounterClockwise90(canvas.Components[i].Position.X, canvas.Components[i].Position.Y, out double x1, out double y1);
                    canvas.Components[i].Position = new PointF((float)x1, (float)y1);
                }
                canvas.FitToView();
            };
            ToolStripButton toolStrip6 = new ToolStripButton("顺时针90");
            toolStrip6.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {
                    canvas.RotateClockwise90(canvas.Components[i].Position.X, canvas.Components[i].Position.Y, out double x1, out double y1);
                    canvas.Components[i].Position = new PointF((float)x1, (float)y1);
                }
                canvas.FitToView();
            };

            ToolStripButton toolStrip7 = new ToolStripButton("旋转180");
            toolStrip7.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {
                    canvas.Components[i].Position = new PointF(-canvas.Components[i].Position.X, -canvas.Components[i].Position.Y);
                }
                canvas.FitToView();
            };

            ToolStripButton toolStrip8 = new ToolStripButton("元件水平X镜像");
            toolStrip8.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {
                    if (Math.Abs(Math.Sin(canvas.Components[i].Angle)) != 1)
                    {
                        canvas.Components[i].Angle = 180 - canvas.Components[i].Angle;
                    }
                }
                canvas.FitToView();
            };
            ToolStripButton toolStrip9 = new ToolStripButton("元件水平Y镜像");
            toolStrip9.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {
                    if (Math.Abs(Math.Sin(canvas.Components[i].Angle)) != 0)
                    {
                        canvas.Components[i].Angle = 360 - canvas.Components[i].Angle;
                       
                    }
                }
                canvas.FitToView();
            };

            ToolStripButton toolStrip10 = new ToolStripButton("元件逆时针90");
            toolStrip10.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {
                    canvas.Components[i].Angle = canvas.Components[i].Angle - 90;
                }
                canvas.FitToView();
            };
            ToolStripButton toolStrip11 = new ToolStripButton("元件顺时针90");
            toolStrip11.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {
                    canvas.Components[i].Angle = 90 + canvas.Components[i].Angle;
                }
                canvas.FitToView();
            };

            ToolStripButton toolStrip12 = new ToolStripButton("元件旋转180");
            toolStrip12.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {
                    canvas.Components[i].Angle = 180 + canvas.Components[i].Angle;
                }
                canvas.FitToView();
            };


            ToolStripButton toolStrip13 = new ToolStripButton("图像界面");
            toolStrip13.Click += (s, e) =>
            {
                Form3 form3 = new Form3(eng_XYData_Results);
                form3.ShowDialog();
            };

            ToolStripButton toolStrip14 = new ToolStripButton("保存数据");
            toolStrip14.Click += (s, e) =>
            {
                canvas.SaveToFile("C:\\Users\\14802\\Desktop\\1.bmp");
                eng_XYData_Results = SQLDataControl.SearchXYData(canvas.ProductCode);
               
            };

            toolStrip.Items.AddRange(new ToolStripItem[]
            {
             toolStrip1,
             toolStrip2 ,
             toolStrip3,
             toolStrip4,
             toolStrip5,
             toolStrip6,
             toolStrip7,
             toolStrip8,
             toolStrip9,
             toolStrip10,
             toolStrip11,
             toolStrip12,
             toolStrip13,
             toolStrip14
            });

            this.Controls.Add(toolStrip);
        }
    }
}
