using CKVisionAppNet;
using ComponentFactory.Krypton.Docking;
using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Toolkit;
using GSP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSP.UI
{
    public partial class UI_Vision : UserControl
    {
        public static UI_Vision Program;

        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        CKVisionAppApi.ProjectNotifyCallBack drawCallBack;// 视图显示自定义绘制图形回调

        CKVisionAppApi.ViewMessageCallBack msgCallBack;// 视图消息回调

        public void ViewMsgCallBack(uint msg, IntPtr wParam, IntPtr lParam, IntPtr pUserParam)
        {
            switch (msg)
            {
                case (int)CKVisionAppNet.AppVIEW_WM.MOUSEMOVE: //鼠标移动消息
                    {
                        double dx = (double)(lParam.ToInt32() & 0xffff);
                        double dy = (double)(((lParam.ToInt32() >> 16) & 0xffff));
                    }
                    break;
                case (int)AppVIEW_WM.LBUTTONDOWN:   //鼠标左键点击消息
                    {
                        double dx = (double)(lParam.ToInt32() & 0xffff);
                        double dy = (double)(((lParam.ToInt32() >> 16) & 0xffff));
                    }
                    break;
                case (int)AppVIEW_WM.RBUTTONDOWN:
                    {
                        ;/// MessageBox.Show("ViewMsgCallBack RBUTTONDOWN");

                    }
                    break;
                case (int)AppVIEW_WM.MOUSEWHEEL:
                    {
                        ///  MessageBox.Show("ViewMsgCallBack MOUSEWHEEL");

                    }
                    break;
            }
        }

        public UI_Vision()
        {
            InitializeComponent();
            Program = this;
            this.Load += UI_Vision_Load;
            this.Dock = DockStyle.Fill;
        }
        /// <summary>
        ///  避免窗体控件改变大小界面瞬间的叠影
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }
        private void UI_Vision_Load(object sender, EventArgs e)
        {
          RefVision();
          BrowApp.Language.Language.Instance.UpdateLanguage(this, this.components.Components);
        }
        void RefVision()
        {
            VPrcListBox.Items.Clear();
            VisionNavigator.Pages.Clear();
            foreach (string dt in Global.VisionApp.TaskName)
            {
                VPrcListBox.Items.Add(CreateNewItem(dt, 0));
                KryptonPage kryptonPage = new KryptonPage();
                kryptonPage.Width = VisionNavigator.Width;
                kryptonPage.Height = VisionNavigator.Height;
                kryptonPage.Text = dt;
                kryptonPage.SizeChanged += PageSizeChanged;
                VisionNavigator.Pages.Add(kryptonPage);
                Global.VisionApp.ShowEditor(kryptonPage.Handle,
               kryptonPage.Width,
               kryptonPage.Height, dt);
            }
        
        }

        private object CreateNewItem(string text,int index)
        {
            KryptonListItem item = new KryptonListItem();
            item.ShortText = text;
            item.LongText = text;
            item.Image = imageList2.Images[index];
            return item;
        }

        private void VPrcListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string s =  VPrcListBox.Items[VPrcListBox.SelectedIndex].ToString();
            for (int i = 0; i < VisionNavigator.Pages.Count; i++)
            {
                if (VisionNavigator.Pages[i].Text==s)
                {
                    VisionNavigator.SelectedPage = VisionNavigator.Pages[i];
                    Global.VisionApp.ShowEditor(VisionNavigator.Pages[i].Handle, VisionNavigator.Pages[i].Width, VisionNavigator.Pages[i].Height, s);
                    break;
                }
            }
       
        }
        private void PageSizeChanged(object sender, EventArgs e)
        {
            Global.VisionApp.ShowEditor(((System.Windows.Forms.Control)sender).Handle,
                ((System.Windows.Forms.Control)sender).Width, ((System.Windows.Forms.Control)sender).Height, 
                ((ComponentFactory.Krypton.Navigator.KryptonPage)sender).Text);
        }
        /// <summary>
        /// 打开项目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenObj_btn_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Title = "加载项目";
            fd.Filter = "项目文件(*.proj)|*.proj";
            fd.FilterIndex = 1;
            if (fd.ShowDialog() == DialogResult.OK)
            {
                if (!Global.VisionApp.LoadProject(fd.FileName))
                {
                    KryptonMessageBox.Show("项目文件加载失败？？？");
                }
            }
            else
            {
                return;
            }
            try
            {
                RefVision();
            }
            catch { }
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveObj_btn_Click(object sender, EventArgs e)
        {
            SaveFileDialog fd = new SaveFileDialog();
            fd.Title = "保存项目";
            fd.Filter = "项目文件(*.proj)|*.proj";
            fd.FilterIndex = 1;
            if (fd.ShowDialog() == DialogResult.OK)
            {
                if (!Global.VisionApp.SaveProject(fd.FileName))
                {
                    KryptonMessageBox.Show("项目文件保存失败？？？");
                }
            }
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddPrc_btn_Click(object sender, EventArgs e)
        {
            string prcName = Global.VisionApp.Add_Proc("Task");
            VPrcListBox.Items.Add(CreateNewItem(prcName, 0));
            KryptonPage kryptonPage = new KryptonPage();
            kryptonPage.Text = prcName;
            kryptonPage.SizeChanged += PageSizeChanged;
            VisionNavigator.Pages.Add(kryptonPage);
            int lastPasge = VPrcListBox.Items.Count - 1;
            VPrcListBox.SelectedItem = VPrcListBox.Items[lastPasge];
           
        }
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemovePrc_btn_Click(object sender, EventArgs e)
        {
            try
            {
                int Index = VPrcListBox.SelectedIndex;
                string PrcName = VPrcListBox.Items[VPrcListBox.SelectedIndex].ToString();
                VisionNavigator.Pages.RemoveAt(Index);
                Global.VisionApp.Del_Proc(PrcName);
                VPrcListBox.Items.RemoveAt(Index);
            }
            catch { }
        }
        /// <summary>
        /// 全局对象
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Globalobject_btn_Click(object sender, EventArgs e)
        {
            Global.VisionApp.ShowObjectdlg();
        }
        /// <summary>
        /// 全局变量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Globalvariable_btn_Click(object sender, EventArgs e)
        {
            Global.VisionApp.ShowGlobalVariableDlg();
        }
    }
}
