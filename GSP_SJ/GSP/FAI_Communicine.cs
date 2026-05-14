using BrowApp;
using BrowApp.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSP
{
    public class FAI_Communicine
    {
        FlowControl H_Flow = new FlowControl();
        /// <summary>
        /// 进板事件
        /// </summary>
        private void In_Click()
        {
            if (!Global.SystemInitialOk)
            {
                APP.Tip.ShowTip(0, "警告".tr(), "设备未归零,请先归零再执行操作".tr(), "确定".tr());
                Global.TcpClass.Send("M:Flow_In_NG");
                return;
            }
            Global.StopFlag = false;
            if (H_Flow.IsManualRun()) { return; }
            H_Flow.ExecuteManual(() => { H_Flow.Flow_In(); });
        }
        /// <summary>
        /// 出板事件
        /// </summary>
        private void Out_Click()
        {
            if (!Global.SystemInitialOk)
            {
                APP.Tip.ShowTip(0, "警告".tr(), "设备未归零,请先归零再执行操作".tr(), "确定".tr());
                Global.TcpClass.Send("M:Flow_OUT_NG");
                return;
            }
            Global.StopFlag = false;
            if (H_Flow.IsManualRun()) { return; }
            H_Flow.ExecuteManual(() => { H_Flow.Flow_Out(); });
        }






























    }
}
