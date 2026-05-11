using BrowApp;
using BrowApp.Language;
using BrowLib;
using CKVisionAppNet;
using GSP;
using GSP.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSP_SJ
{
    public class MotionCommand
    {
        static  FlowControl Flow = new FlowControl();
        static Algorithm Algorithm = new Algorithm();
        public static void Home()
        {
            if (Global.SystemRun) { return; }
            Global.MachineState = GEnumEx.MachineState.MachineInitialize;
            Global.SystemInitialOk = false;
            Global.StopFlag = false;
            HomeStart home = new HomeStart();
            home.ShowDialog();
            if (home.IsHomeflg)
            {
                Global.MachineState = GEnumEx.MachineState.MachineStop;
               
            }
            else
            {
                Global.MachineState = GEnumEx.MachineState.MachineError;
              
            }
        }

        public static void GoToStopPos()
        {
            new HandFlow().SafeMoveXYZR(200, 20, 0, Global.Systemdata.StopPos.Xpos, Global.Systemdata.StopPos.Ypos, Global.Systemdata.StopPos.Zpos, 0);
        }

        public static void In_Click()
        {
            if (!Global.SystemInitialOk)
            {
                APP.Tip.ShowTip(0, "警告".tr(), "设备未归零,请先归零再执行操作".tr(), "确定".tr());
                Global.TcpClass.Send("M:Flow_In_NG");
                return;
            }
            Global.StopFlag = false;
            if (Flow.IsManualRun()) { return; }
            Flow.ExecuteManual(() =>
            {
                Flow.Flow_In();
            });
        }
        public static void Out_Click()
        {
            if (!Global.SystemInitialOk)
            {
                APP.Tip.ShowTip(0, "警告".tr(), "设备未归零,请先归零再执行操作".tr(), "确定");
                Global.TcpClass.Send("M:Flow_OUT_NG");
                return;
            }
            Global.StopFlag = false;
            if (Flow.IsManualRun()) { return; }
            Flow.ExecuteManual(() =>
            {
                Flow.Flow_Out();
            });
        }

        public static void Action开路清零()
        {
            double size = Global.GetSize("0201");//获取开口大小
            Flow.ExecuteManual(() =>
            {
                new HandFlow().SafeMoveXYZR(200, 10, 0, Global.CalibData.ZeroPos.Xpos, Global.CalibData.ZeroPos.Ypos, Global.CalibData.ZeroPos.Zpos, 0, size);
           
            });
        }

        public static void Action短路清零()
        {
            double size2 = Global.GetSize("0201");//获取开口大小
            Flow.ExecuteManual(() =>
            {
                new HandFlow().SafeMoveXYZR(200, 10, 0, Global.CalibData.ZeroPos2.Xpos, Global.CalibData.ZeroPos2.Ypos, Global.CalibData.ZeroPos2.Zpos, 0, size2);
      
            });
        }

        public static void Action短路清零完成()
        {
            new HandFlow().SafeMoveXyz(Global.Systemdata.StopPos.Zpos, Global.Systemdata.StopPos.Xpos, Global.Systemdata.StopPos.Ypos, Global.Systemdata.StopPos.Zpos);
        }

        public static void Start()
        {
            if (Global.PauseFlag == true)
            {
                if (!Global.AlarmFlag)
                {
                    Global.PauseFlag = false;
                    Global.MachineState = GEnumEx.MachineState.MachineRuning;
                    APP.Log.I_Log("暂停恢复启动");
                }
                else
                {
                    APP.Tip.ShowTip(0, "警告".tr(), "请先解除报警/再启动设备".tr(), "确定".tr());
                  
                    return;
                }
            }
            else
            {
                if (Global.SystemRun)
                {
                    return;
                }
                if (!Global.SystemInitialOk)
                {
                    APP.Tip.ShowTip(0, "警告", "自动运行前请先复位".tr(), "确定");
                
                    return;
                }
                if (Global.AlarmFlag)
                {
                    APP.Tip.ShowTip(0, "警告".tr(), "请先解除报警 / 再启动设备".tr(), "确定".tr());
              
                    return;
                }
                if (Global.BomData.Rows.Count <= 0)
                {
                    APP.Tip.ShowTip(0, "警告".tr(), "未导入数据".tr(), "确定".tr());
           
                    return;
                }
                if (!Global.IsRecipe)
                {
                    APP.Tip.ShowTip(0, "警告", "无配方数据".tr(), "确定".tr());
               
                    return;
                }
                else
                {
                    Global.BomData = null;
                
                    Flow.START();
                    Global.States = 3;//运行
                }
            }
        }

        public static void Stop()
        {
            Global.StopFlag = true;
            Global.PauseFlag = false;
            Global.SystemRun = false;
            Global.OrbModel = 1;
            Global.MachineState = GEnumEx.MachineState.MachineStop;
            BrowLib.Controller.CardAPI.StopAxis();
            BrowLib.Controller.CardAPI.ClearSts();
            Global.VisionApp.StopRunProc("Task5");
        }


        public static void Pause()
        {
            if (Global.MachineState == GEnumEx.MachineState.MachineRuning)
            {
                Global.PauseFlag = true;
                Global.MachineState = GEnumEx.MachineState.MachinePause;
            }
        }


        public static void Cmark(int Mpix_X, int Mpix_Y, int iwidth, int iHight)
        {
            VisionApp.CalibType Calib = new VisionApp.CalibType();
            Calib = Global.VisionApp.GetCalib(GSP.VisionGlobal.UpCailb_Obj);
            double Cen_X = 0, Cen_Y = 0, X, Y, width, hight;
            Algorithm.GetStartCenter_Pix(Global.Parm.PcbLong, Global.Parm.PcbHight, Global.Systemdata.CameRaview.Row,
            Global.Systemdata.CameRaview.Col, iwidth, iHight, out width, out hight);
            switch (Global.Model)
            {
                case 0://993
                    Global.VisionApp.PixToPos(Calib, (iwidth - width / 2), (hight / 2), out Cen_X, out Cen_Y);
                    break;
                case 1://991
                    Global.VisionApp.PixToPos(Calib, (iwidth - width / 2), (iHight - hight / 2), out Cen_X, out Cen_Y);
                    break;
                case 2: //860
                case 3://860P
                    Global.VisionApp.PixToPos(Calib, (width / 2), (hight / 2), out Cen_X, out Cen_Y);
                    break;
            }
            Global.VisionApp.PixToPos(Calib, Mpix_X, Mpix_Y, out X, out Y);
            double Dx = (X - Cen_X) * Global.Ratio * Global.ARatio;
            double Dy = (Y - Cen_Y) * Global.Ratio * Global.ARatio;
            new HandFlow().SafeMoveXyz(0, Global.Systemdata.PatlePos.Xpos - Dx,
           Global.Systemdata.PatlePos.Ypos - Dy, Global.CamHeight);
        }
    }
}
