using BrowApp;
using BrowApp.Language;
using BrowApp.MessageTip;
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
        static FlowControl Flow = new FlowControl();
        static Algorithm Algorithm = new Algorithm();
        static MarkForm markFrm = null;

        public static void StartThread()
        {
            Flow.StartThread();
        }

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
                APP.Tip.ShowTip(0, "警告".tr(), "设备未归零,请先归零再执行操作".tr(), "确定".tr());
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
                    APP.Tip.ShowTip(0, "警告".tr(), "自动运行前请先复位".tr(), "确定".tr());

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
                    APP.Tip.ShowTip(0, "警告".tr(), "无配方数据".tr(), "确定".tr());

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

        /// <summary>
        /// 校正定位Mark
        /// </summary>
        /// <param name="position">位号</param>
        /// <param name="MarkNum">Mark号 1,2</param>
        /// <param name="MarkMode">mark方式</param>
        /// <param name="x">元件x坐标</param>
        /// <param name="y">元件y坐标</param>
        /// <param name="Mpix_X">拼图x像素</param>
        /// <param name="Mpix_Y">拼图y像素</param>
        /// <param name="iwidth">拼图宽</param>
        /// <param name="iHight">拼图高</param>
        /// <param name="pcbLong">板长</param>
        /// <param name="pcbHight">板宽</param>
        public static void C_Mark(string position, int MarkNum, int MarkMode, double x, double y, int Mpix_X, int Mpix_Y, int iwidth, int iHight,double pcbLong,double pcbHight)
        {
            switch (Global.Model)
            {
                case 0:
                    Global.Parm.PcbLong = pcbLong;
                    Global.Parm.PcbHight = pcbHight;
                    break;
                case 1:
                case 2:
                case 3:
                    Global.Parm.PcbLong = pcbHight;
                    Global.Parm.PcbHight = pcbLong;
                    break;
            }
            Cmark(Mpix_X, Mpix_Y, iwidth, iHight);
            if (markFrm == null || markFrm.IsDisposed)
            {
                markFrm = new MarkForm();
                markFrm.RefProc();
                markFrm.Show();
                markFrm.Activate();

            }
            else
            {
                markFrm.RefProc();
                markFrm.Activate();
                markFrm.FomActivated();
            }
            markFrm.Mode = MarkMode;
            markFrm.Code = position;
            switch (MarkNum)
            {
                case 1:
                    markFrm.Bmark1_X = x;
                    markFrm.Bmark1_Y = y;
                    markFrm.Num = 1;
                    break;
                case 2:
                    markFrm.Bmark2_X = x;
                    markFrm.Bmark2_Y = y;
                    markFrm.Num = 2;
                    break;
            }
            markFrm.RefMarkfrm();
        }

      
        /// <param name="Mpix_X">像素位置X</param>
        /// <param name="Mpix_Y">像素位置Y</param>
        /// <param name="iwidth">图像宽</param>
        /// <param name="iHight">图像高</param>
        private static void Cmark(int Mpix_X, int Mpix_Y, int iwidth, int iHight)
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

        /// <summary>
        ///启动拼图
        /// </summary>
        /// <param name="mode"></param>
        public static void PateSart(int mode)
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
                    APP.Tip.ShowTip(1, "警告".tr(), "请先解除报警/再启动设备".tr(), "确定".tr());

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
                    APP.Tip.ShowTip(0, "警告".tr(), "自动运行前请先复位".tr(), "确定".tr());

                    return;
                }
                if (Global.AlarmFlag)
                {
                    APP.Tip.ShowTip(0, "警告".tr(), "请先解除报警 / 再启动设备".tr(), "确定".tr());

                    return;
                }
                else
                {
                    Global.StopFlag = false;
                    Global.SystemRun = true;
                    Global.MachineState = GEnumEx.MachineState.MachineRuning;

                    if (Flow.IsManualRun()) { return; }
                    Flow.ExecuteManual(() =>
                    {
                        Flow.FlowPate(mode);
                    });
                }
            }
        }

        public static void SetWidth(double Width)
        {

            if (Width > Global.Systemdata.GdWight)
            {
                APP.Tip.ShowTip(0, "警告".tr(), "无法设置轨道宽度".tr(), "确定".tr());
                return;
            }

            Flow.ExecuteManual(() =>
            {
                int re = new HandFlow().SetWidth(20, Global.Systemdata.GdWight - Width);
                if (re == 0)
                {
                    FloatingTip.ShowOk("M:SetWidth_OK");

                }
                else
                {
                    FloatingTip.ShowError("M:SetWidth_NG");
                }
            });
        }

        public static void GoMark()
        {
            Global.StopFlag = false;
            if (Flow.IsManualRun()) { return; }
            Flow.ExecuteManual(() =>
            {
                Flow.GoToMark();
            });
        }

        public static void M_goTo(string StrCode, string Line, string IsCCD)
        {
            Global.StopFlag = false;
            if (Flow.IsManualRun()) { return; }
            Flow.ExecuteManual(() =>
            {
                Flow.M_goTo(StrCode, Line, IsCCD);
            });
        }
        public static void Mccd_Goto(string StrCode, string Line)
        {
            if (Global.BomData.Rows.Count <= 0)
            {
                APP.Tip.ShowTip(0, "警告".tr(), "未导入数据".tr(), "确定".tr());
                return;
            }
            if (!Global.IsRecipe)
            {
                APP.Tip.ShowTip(0, "警告".tr(), "无配方数据".tr(), "确定".tr());
                return;
            }
            if (Global.AlarmFlag)
            {
                APP.Tip.ShowTip(0, "警告".tr(), "请先清除报警再执行操作".tr(), "确定".tr());
                return;
            }
            Global.StopFlag = false;
            if (Flow.IsManualRun()) { return; }
            Flow.ExecuteManual(() =>
            {
                Flow.Mccd_Goto(StrCode, Line);
            });
        }
        public static void Laser_Go(string StrCode)
        {
            if (Global.BomData.Rows.Count <= 0)
            {
                APP.Tip.ShowTip(0, "警告".tr(), "未导入数据".tr(), "确定".tr());
                return;
            }
            if (!Global.IsRecipe)
            {
                APP.Tip.ShowTip(0, "警告".tr(), "无配方数据".tr(), "确定".tr());
                return;
            }
            Global.StopFlag = false;
            if (Flow.IsManualRun()) { return; }
            Flow.ExecuteManual(() =>
            {
                Flow.Laser_Go(StrCode);
            });
        }
        public static void GraspImage(string StrCode, double W, double H, double height, int R = 0, int G = 0, int B = 0)
        {
            if (Global.BomData.Rows.Count <= 0)
            {
                APP.Tip.ShowTip(0, "警告".tr(), "未导入数据".tr(), "确定".tr());
                return;
            }
            if (!Global.IsRecipe)
            {
                APP.Tip.ShowTip(0, "警告".tr(), "无配方数据".tr(), "确定".tr());
                return;
            }
            Global.StopFlag = false;
            if (Flow.IsManualRun()) { return; }
            Flow.ExecuteManual(() =>
            {
                Flow.GraspImage(StrCode, W, H, height, R, G, B);
            });
        }
        public static void AutoGraspimg()
        {
            if (Global.BomData.Rows.Count <= 0)
            {
                APP.Tip.ShowTip(0, "警告".tr(), "未导入数据".tr(), "确定".tr());
                return;
            }
            if (!Global.IsRecipe)
            {
                APP.Tip.ShowTip(0, "警告".tr(), "无配方数据".tr(), "确定".tr());
                return;
            }
            Flow.StartOCR();
        }

        /// <summary>
        /// 刷新配方
        /// </summary>
        /// <param name="mode"></param>
        public static void RefRecipe(int mode)
        {
            string FBCCode, XYCode, BoardSide, ProductCode, ProductName;
            double X, Y;
            Global.ReadFAIConfig(out FBCCode, out XYCode, out BoardSide, out ProductCode, out ProductName);
            Global.RecipePatn = XYCode + BoardSide;
            Global.Parm.SetCode = Global.RecipePatn;
            Global.VisionApp.SetCodePath = Global.RecipePatn;
            if (mode == 0)
            {
                Global.Is_NoMark = true;
                bool Btn1 = Global.Parm.Read(ref Global.Parm);
                if (Btn1) { Global.IsRecipe = true; }
                else
                    Global.IsRecipe = false;
            }
            else if (mode == 1)
            {
                Global.Is_NoMark = false;
                bool Btn1 = Global.Parm.Read(ref Global.Parm);
                bool Btn2 = Global.VisionApp.ReadObj("Mark.proc", "Task3");
                if (Btn1 && Btn2) { Global.IsRecipe = true; }
                else
                    Global.IsRecipe = false;
            }

            GSP.VisionGlobal.bMak1_X = Global.Parm.BMak1Pos.Xpos;
            GSP.VisionGlobal.bMak1_Y = Global.Parm.BMak1Pos.Ypos;

            GSP.VisionGlobal.bMak2_X = Global.Parm.BMak2Pos.Xpos;
            GSP.VisionGlobal.bMak2_Y = Global.Parm.BMak2Pos.Ypos;

        }
    }
}
