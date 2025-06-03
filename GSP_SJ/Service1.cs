using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSP_SJ
{
    partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 服务代理
        /// </summary>
        private ServiceHost serviceHose = null;
        protected override void OnStart(string[] args)
        {
            // TODO: 在此处添加代码以启动服务。
            try
            {
                serviceHose = new ServiceHost(typeof(MyTestWcfServiceLibrary.MyFirstService));
                if (serviceHose.State != CommunicationState.Opened)
                {
                    serviceHose.Open();
                    MessageBox.Show("服务已打开");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        protected override void OnStop()
        {
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。
            if (serviceHose != null)
            {
                serviceHose.Close();
            }
        }
    }
}
