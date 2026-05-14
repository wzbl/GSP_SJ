#region 所有步骤实现
using BrowApp;
using BrowApp.Language;
using BrowLib;
using GSP;
using GSP.Flow;
using GSP.UI;
using System;
using System.Data;
using System.Drawing;
using System.Runtime.Remoting.Contexts;
using System.Threading;



public class ProcessEntity
{
    #region 常量定义
    private const double POSITION_TOLERANCE = 0.02;
    private const int LOOP_SLEEP_MS = 2;
    private const int CAMERA_DELAY_MS = 10;
    private const int LIFT_DELAY_MS = 300;
    private const int FEEDING_TIMEOUT_MS = 5000;
    private const int LIFT_TIMEOUT_MS = 1000;
    private const double SAFETY_HEIGHT_DIFF = 0.1;
    #endregion

    /// <summary>
    /// 步骤0：初始检查
    /// </summary>
    public class Step0_InitialCheck : BaseStep
    {
        public override int StepId => 0;
        public override string StepName => "初始检查";

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            context.Log("开始初始检查");
            //机型模式切换分支
            switch (Global.Model)
            {
                case 0:
                    context.Log("离线机模式，直接进入步骤6");
                    return 6;
                case 1://在线机991
                case 2://860机型
                case 3://860
                    bool hasBoard = BrowLib.Controller.InPort["阻挡感应光电_IN"].IsOn(100);
                    if (hasBoard)
                    {
                        context.Log("检测到PCB板，进入步骤6");
                        return 6;
                    }
                    else
                    {
                        context.ShowWarning("初始检查", "请放入PCB板");
                        return 0;
                    }
                default:
                    context.StopProcess($"未知机型{Global.Model}");
                    return -1;
            }
        }

        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤1：入口传感器检测
    /// </summary>
    public class Step1_EntrySensorCheck : BaseStep
    {
        public override int StepId => 1;
        public override string StepName => "入口传感器检测";
        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;
            //口传感器检测
            bool isSensorOn = BrowLib.Controller.InPort["入口感应光电_IN"].IsOn(500);
            if (isSensorOn)
            {
                context.Log("入口感应到位");
                BrowLib.Controller.OutPort["上位机要板_OUT"].Off();//上位机要板信号关闭
                context.Log("关闭上位机要板信号");
                return 2;
            }
            return 1;
        }
    }
    /// <summary>
    /// 步骤2：开始进板
    /// </summary>
    public class Step2_StartBoardIn : BaseStep
    {
        public override int StepId => 2;
        public override string StepName => "开始进板";

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;
            context.Log("开始进板");
            Global.皮带.JOP(1, 50);
            context.GlobalTimer.Restart();
            return 3;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤3：进板到位检查
    /// </summary>
    public class Step3_BoardInPositionCheck : BaseStep
    {
        public override int StepId => 3;
        public override string StepName => "进板到位检查";

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;
            // 阻挡传感器检测（实际项目中替换为真实硬件调用）
            bool isBoardInPosition = BrowLib.Controller.InPort["阻挡感应光电_IN"].IsOn();
            if (isBoardInPosition)
            {
                context.Log("板到工作位");
                Global.皮带.ChangSpeed(5);
                Global.皮带.JOP(1, 5);//皮带减速
                Thread.Sleep((int)Global.Systemdata.InDaytime);
                Global.皮带.AxisStop();
                context.Log("皮带停止");
                return 4;
            }
            else if (context.GlobalTimer.GetTime() > 5000)
            {
                context.ShowWarning("进板检查", "进板超时");
                context.StopProcess("进板超时");
                return -1;
            }
            return 3;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// 步骤4：调宽轴移动
    /// </summary>
    public class Step4_AdjustWidth : BaseStep
    {
        public override int StepId => 4;
        public override string StepName => "调宽轴移动";

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            context.Log("调宽轴移动");
            Global.调宽.PMove(10, 1000, -1 * Global.Systemdata.Trackoffset, 0);
            context.Log($"调宽轴移动至位置: {-1 * Global.Systemdata.Trackoffset}");
            return 5;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤5：调宽轴到位检查
    /// </summary>
    public class Step5_AdjustWidthCheck : BaseStep
    {
        public override int StepId => 5;
        public override string StepName => "调宽轴到位检查";

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;
            //调宽轴到位检测（实际项目中替换为真实硬件调用）
            bool isWidthAdjusted = Global.调宽.Runing();
            if (isWidthAdjusted)
            {
                context.Log("调宽轴到位");
                return 6;
            }

            return 5;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤6：顶升气缸顶起
    /// </summary>
    public class Step6_LiftCylinderUp : BaseStep
    {
        public override int StepId => 6;
        public override string StepName => "顶升气缸顶起";
        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;
            // 模拟顶升气缸控制（实际项目中替换为真实硬件调用）
            BrowLib.Controller.OutPort["顶升气缸_OUT"].On();
            context.Log("顶升气缸打开");
            context.GlobalTimer.Restart();
            return 7;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤7：顶升到位检查
    /// </summary>
    public class Step7_LiftPositionCheck : BaseStep
    {
        public override int StepId => 7;
        public override string StepName => "顶升到位检查";
        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;
            // 模拟顶升到位检测（实际项目中替换为真实硬件调用）
            bool isLiftUp = BrowLib.Controller.InPort["顶升上位_IN"].IsOn(100);
            if (isLiftUp)
            {
                context.Log("顶升顶起到位");
                Thread.Sleep(300);
                return 8;
            }
            else if (context.GlobalTimer.GetTime() > 1000)
            {
                int result = context.ShowWarning("顶升检查", "顶升到位超时", "继续", "停止");
                if (result == 0)
                {
                    context.Log("用户选择继续");
                    return 8;
                }
                else
                {
                    context.StopProcess("顶升到位超时-用户选择停止");
                    return -1;
                }
            }
            return 7;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤8：设置光源+Z轴到拍照高度
    /// </summary>
    public class Step8_SetLightAndZPosition : BaseStep
    {
        public override int StepId => 8;
        public override string StepName => "设置光源+Z轴到拍照高度";

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;
            Global.Light.SetRgbLight(Global.Systemdata.M_LED.LED_R, Global.Systemdata.M_LED.LED_G, Global.Systemdata.M_LED.LED_B);
            context.Log($"设置光源亮度[R={Global.Systemdata.M_LED.LED_R},G={Global.Systemdata.M_LED.LED_G},B={Global.Systemdata.M_LED.LED_B}]");
            Global.VisionApp.StopRunProc("Task5");//停止实时刷新
                                                  //Z轴移动（实际项目中替换为真实硬件调用）
            Global.Z轴.PMove(Global.RunZVel, Global.RunZAcc, Global.CamHeight, 1);
            context.Log($"Z轴移动到拍照高度: {Global.CamHeight}");
            return 9;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 步骤9：Z轴到位检查+R轴回零
    /// </summary>
    public class Step9_ZAxisCheckAndRReset : BaseStep
    {
        public override int StepId => 9;
        public override string StepName => "Z轴到位检查+R轴回零";

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            if(CheckAxisPosition("Z轴", Global.CamHeight, POSITION_TOLERANCE))
            {
                //模拟R轴回零（实际项目中替换为真实硬件调用）
                context.Log("R轴回零");
                Global.R轴.PMove(Global.RunZVel, Global.RunZAcc, 0, 1);
                return 10;
            }
            return 9;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤10：R轴回零检查
    /// </summary>
    public class Step10_RAxisResetCheck : BaseStep
    {
        public override int StepId => 10;
        public override string StepName => "R轴回零检查";

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;
            if (CheckAxisPosition("Z轴", -1, POSITION_TOLERANCE))
            {
                context.Log("R轴回零位完成");
                return 11;
            }
            return 10;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤11：Mark点定位移动
    /// </summary>
    public class Step11_MarkPositionMove : BaseStep
    {
        public override int StepId => 11;
        public override string StepName => "Mark点定位移动";

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;
            // 模拟无Mark点判断（实际项目中替换为真实配置）
            if (Global.Is_NoMark)
            {
                context.Log("无Mark点，跳过定位");
                return 17;
            }
            // 模拟Mark点位置（实际项目中替换为真实配置）
            double makX, makY;
            if (context.MakNum == 0)
            {

                makX = Global.Parm.Mak1Pos.Xpos;
                makY = Global.Parm.Mak1Pos.Ypos;
               
                context.Log("移动到MARK1位置");
            }
            else
            {
                makX = Global.Parm.Mak2Pos.Xpos;
                makY = Global.Parm.Mak2Pos.Ypos;
               
                context.Log("移动到MARK2位置");
            }
            // 模拟XY轴移动（实际项目中替换为真实硬件调用）
            BrowLib.Controller.MotionAPI.LinXyMoveA(Global.RunXYVel, Global.RunXYAcc, makX, makY, 1, 2);
            context.Log($"移动到Mark位置{context.MakNum}: X={makX}, Y={makY}");
            return 12;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤12：Mark点到位检查
    /// </summary>
    public class Step12_MarkPositionCheck : BaseStep
    {
        public override int StepId => 12;
        public override string StepName => "Mark点到位检查";

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟XY轴到位检测（实际项目中替换为真实硬件调用）
            bool isPositionOk = BrowLib.Controller.MotionAPI.LinXYRuningA();
            if (isPositionOk)
            {
                context.Log($"到达Mark位置{context.MakNum}");
                return 13;
            }
            return 12;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤13：Z轴到Mark拍照高度
    /// </summary>
    public class Step13_ZAxisToMarkHeight : BaseStep
    {
        public override int StepId => 13;
        public override string StepName => "Z轴到Mark拍照高度";

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟Z轴移动（实际项目中替换为真实硬件调用）
            Global.Z轴.PMove(Global.RunZVel, Global.RunZAcc, Global.CamHeight, 1);
           
            context.Log($"Z轴移动到Mark{context.MakNum}拍照高度: {Global.CamHeight}");

            return 14;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 步骤14：Z轴Mark高度到位检查
    /// </summary>
    public class Step14_ZAxisMarkHeightCheck : BaseStep
    {
        public override int StepId => 14;
        public override string StepName =>"Z轴Mark高度到位检查";

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟Z轴位置检测（实际项目中替换为真实硬件调用）
            if (CheckAxisPosition("Z轴", Global.CamHeight, POSITION_TOLERANCE))
            {
                context.Log($"Z轴到达Mark{context.MakNum}拍照高度");
                Thread.Sleep(100);
                return 15;
            }
            return 14;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤15：Mark点视觉拍照
    /// </summary>
    public class Step15_MarkVisionCapture : BaseStep
    {
        public override int StepId => 15;
        public override string StepName => "Mark点视觉拍照";

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟视觉拍照（实际项目中替换为真实视觉库调用）
            if (context.MakNum == 0)
            {
                Global.VisionApp.SetToolData("Task3", "定义变量", 2102, 0, "false", 0);
                Global.VisionApp.SetToolData("Task3", "定义变量", 2110, 0, "1", 1);
                Global.VisionApp.ExecuteProc("Task3", 0);
                context.Log("触发Mark1拍照");
            }
            else if (context.MakNum == 1)
            {
                Global.VisionApp.SetToolData("Task3", "定义变量", 2102, 0, "false", 0);
                Global.VisionApp.SetToolData("Task3", "定义变量", 2110, 0, "2", 1);
                Global.VisionApp.ExecuteProc("Task3", 1);
                context.Log("触发Mark2拍照");
            }
            Thread.Sleep(10);
            return 16;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤16：Mark点视觉结果处理
    /// </summary>
    public class Step16_MarkVisionResult : BaseStep
    {
        public override int StepId => 16;
        public override string StepName =>"Mark点视觉结果处理";

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟视觉处理完成判断（实际项目中替换为真实视觉库调用）
            bool isVisionDone = Global.VisionApp.EndProc["Task3"];
            if (!isVisionDone)
            {
                if (context.MakNum == 0)
                {
                    // 模拟Mark1结果（实际项目中替换为真实视觉库调用）
                    bool isMark1Ok = Global.VisionApp.RunState("Task3", "执行结果");
                    if (isMark1Ok)
                    {
                        context.DMak1_X = Global.VisionApp.GetDblValue("Task3", "定义变量", 2103, 0);
                        context.DMak1_Y = Global.VisionApp.GetDblValue("Task3", "定义变量", 2104, 0);
                        context.Log($"Mark1结果: Dx={context.DMak1_X:F3}, Dy={context.DMak1_Y:F3}");
                        context.MakNum++;
                        return 11;
                    }
                    else
                    {
                        MarkRectify mark = new MarkRectify();
                        mark.ShowDialog();
                        if (mark.Isok)
                        {
                            context.DMak1_X = mark.Dx;
                            context.DMak1_Y = mark.Dy;
                            context.MakNum++;
                            return 11;
                        }
                        else
                        {
                            context.ShowWarning("Mark定位", "Mark1定位失败");
                            context.StopProcess("Mark1定位失败");
                            return -1;
                        }
                    }
                }
                else if (context.MakNum == 1)
                {
                    // 模拟Mark2结果（实际项目中替换为真实视觉库调用）
                    bool isMark2Ok = Global.VisionApp.RunState("Task3", "执行结果");
                    if (isMark2Ok)
                    {
                        context.DMak2_X = Global.VisionApp.GetDblValue("Task3", "定义变量", 2103, 0);
                        context.DMak2_Y = Global.VisionApp.GetDblValue("Task3", "定义变量", 2104, 0);
                        context.Log($"Mark2结果: Dx={context.DMak2_X:F3}, Dy={context.DMak2_Y:F3}");
                        context.MakNum = 0;
                        return 17;
                    }
                    else
                    {
                        MarkRectify mark = new MarkRectify();
                        mark.ShowDialog();
                        if (mark.Isok)
                        {
                            context.DMak2_X = mark.Dx;
                            context.DMak2_Y = mark.Dy;
                            context.MakNum=0;
                            return 17;
                        }
                        else
                        {
                            context.ShowWarning("Mark定位", "Mark2定位失败");
                            context.StopProcess("Mark2定位失败");
                            return -1;
                        }
                    }
                }
            }
            return 16;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤17：Mark点纠偏计算
    /// </summary>
    public class Step17_MarkRectifyCalculate : BaseStep
    {
        public override int StepId => 17;
        public override string StepName =>"Mark点纠偏计算";

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;
            Global.Light.SetRgbLight(Global.Systemdata.P_LED.LED_R, Global.Systemdata.P_LED.LED_G, Global.Systemdata.P_LED.LED_B);
            // 模拟设置光源（实际项目中替换为真实硬件调用）
            context.Log($"设置光源亮度[R={Global.Systemdata.P_LED.LED_R},G={Global.Systemdata.P_LED.LED_G},B={Global.Systemdata.P_LED.LED_B}]");

            // 模拟Mark纠偏计算（实际项目中替换为真实算法）

            VisionGlobal.Mak1_X = Global.Parm.Mak1Pos.Xpos - context.DMak1_X;//纠偏Mask1X位置
            VisionGlobal.Mak1_Y = Global.Parm.Mak1Pos.Ypos - context.DMak1_Y;//纠偏Mask1Y位置

            VisionGlobal.Mak2_X = Global.Parm.Mak2Pos.Xpos - context.DMak2_X;//纠偏Mask2X位置
            VisionGlobal.Mak2_Y = Global.Parm.Mak2Pos.Ypos - context.DMak2_Y;//纠偏Mask2Y位置

            context.Log($"Mark1纠偏后: X={VisionGlobal.Mak1_X:F4}, Y={VisionGlobal.Mak1_Y:F4}");
            context.Log($"Mark2纠偏后: X={VisionGlobal.Mak2_X:F4}, Y={VisionGlobal.Mak2_Y:F4}");

            context.CycleTimer.Restart();

            return 18;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤18：运行模式选择
    /// </summary>
    public class Step18_RunModeSelect : BaseStep
    {
        public override int StepId => 18;
        public override string StepName =>"运行模式选择";
        public override bool IgnorePause => true;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟运行模式（实际项目中替换为真实配置）
            int runMode = Global.RunMode; // 0-单机 1-在线
            switch (runMode)
            {
                case 0:
                    context.Log("单机模式，进入步骤200");
                    return 200;
                case 1:
                    context.Log("在线模式，进入步骤19");
                    return 19;
                default:
                    context.StopProcess($"未知运行模式{runMode}");
                    return -1;
            }
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤19：TCP发送开始指令
    /// </summary>
    public class Step19_TcpSendStart : BaseStep
    {
        public override int StepId => 19;
        public override string StepName => "TCP发送开始指令";

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟TCP发送（实际项目中替换为真实TCP库调用）
            Global.TcpClass.TCP_AStaart = false;
            Global.TcpClass.Send("A:GET\r\n", StepId.ToString());
            context.Log("发送TCP开始指令: A:GET");
            return 20;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤20：TCP接收结果处理
    /// </summary>
    public class Step20_TcpReceiveResult : BaseStep
    {
        public override int StepId => 20;
        public override string StepName => "TCP接收结果处理";
        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;
            // 模拟TCP接收完成判断（实际项目中替换为真实TCP库调用）
            bool isTcpReceived = Global.TcpClass.TCP_AStaart;
            if (isTcpReceived)
            {
                string str2 = Global.TcpClass.TCP_Result.Trim();
                string[] re1 = str2.Split(':');
                if (re1[0] == "A")
                {
                    if (re1[1] == "Finish")
                    {
                        context.CycleTimer.Stop();
                        context.CycleTime= context.GetTime(context.CycleTimer.ElapsedTicks);
                        context.Log($"周期时间: {context.CycleTime:F2}秒");
                        switch (Global.Model)//机型选择
                        {
                            case 0:
                                context.CurrentStep = 50;//离线
                                break;
                            case 1:
                            case 2:
                            case 3:
                                switch (Global.OrbModel)//轨道模式选择
                                {
                                    case 0:
                                        context.CurrentStep = 60;//在线
                                        break;
                                    case 1:
                                        context.CurrentStep = 50;//离线
                                        break;
                                }
                                break;
                        }
                    }
                    else
                    {
                        context.StrCode = re1[1];
                        switch (re1[2])
                        {
                            case "2":
                                BrowLib.Controller.OutPort["四线切换两线"].On();
                                break;
                            case "4":
                                BrowLib.Controller.OutPort["四线切换两线"].Off();
                                break;
                        }
                        if (re1.Length > 3)
                        {
                            if (re1[3] == "Rectify")
                            {
                                context.Mode = 1;
                                context.CurrentStep = 21;
                            }
                            else if (re1[3] == "Laser")
                            {
                                context.Mode = 2;
                                context.CurrentStep = 21;
                            }
                            else if (re1[3] == "RectifyFirst")
                            {
                                context.Mode = 3;
                                context.CurrentStep = 21;
                            }
                            else if (re1[3] == "RectifyNext")
                            {
                                context.Dx = 0; context.Dy = 0;
                                double translationX=0, translationY=0;
                                //Global.BomData = new JD.Fai.Data.FlyingProbe().GetDataTable(Global.FBCCode);
                                new Algorithm().CalculateTransferXY(Global.BomData, out translationX, out translationY);
                                VisionGlobal.TranslationX = translationX;
                                VisionGlobal.TranslationY = translationY;
                                context.Mode = 0;
                                context.CurrentStep = 21;
                            }
                            else if (re1[3] == "RectifyCheck")
                            {
                                context.Mode = 4;
                                context.CurrentStep = 21;
                            }
                            else
                            {
                                context.Mode = 0;
                                context.CurrentStep = 21;
                            }
                        }
                        else
                        {
                            context.Log($"指令回复错误: {context.StrCode}");
                            context.Mode = 0;
                            context.CurrentStep = 21;
                        }
                    }
                }
                return context.CurrentStep;
            }
           return 20;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤21：位号坐标计算
    /// </summary>
    public class Step21_PositionCalculate : BaseStep
    {
        public override int StepId => 21;
        public override string StepName => "位号坐标计算";

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟BOM数据读取（实际项目中替换为真实数据）
            //位号获取坐标位置
            DataRow[] dataRows = Global.BomData.Select("位置号='" + context.StrCode + "'");
            context.X = Convert.ToDouble(dataRows[0]["原始X坐标"].ToString());
            context.Y = Convert.ToDouble(dataRows[0]["原始Y坐标"].ToString());
            context.R = Convert.ToDouble(dataRows[0]["原始方向"].ToString());
            context.X_offset = Convert.ToDouble(dataRows[0]["X坐标调整"].ToString());
            context.Y_offset= Convert.ToDouble(dataRows[0]["Y坐标调整"].ToString());
            context.R = new Algorithm().GetAngle(context.R);

            //象限坐标系平移  
            context.X = context.X + VisionGlobal.TranslationX;
            context.Y = context.Y + VisionGlobal.TranslationY;
            //拼版偏移
            context.X = context.X + Global.Parm.PbXoffset;
            context.Y = context.Y + Global.Parm.PbYoffset;
            
            context.Type = dataRows[0]["尺寸"].ToString();

            context.Size = Global.GetSize(context.Type);//获取开口大小
            context.ZHight = Global.GetHight(context.Type);//获取下针高度
            context.Size = Global.JQSize(context.Type);//获取夹取宽度
            double Mx=0, My=0;
            //Mark计算
            int Ecode = new Algorithm().MakAlgorithm(0, context.X, context.Y, VisionGlobal.Mak1_X, VisionGlobal.Mak1_Y,
            VisionGlobal.Mak2_X, VisionGlobal.Mak2_Y, VisionGlobal.bMak1_X + VisionGlobal.TranslationX,
            VisionGlobal.bMak1_Y + VisionGlobal.TranslationY, VisionGlobal.bMak2_X + VisionGlobal.TranslationX,
            VisionGlobal.bMak2_Y + VisionGlobal.TranslationY, out Mx, out My);
            if (Ecode != 0)
            {
                APP.Tip.ShowTip(1, "警告".tr(), "Mark绑定数据出错".tr(), "确定".tr());
                Global.StopFlag = true;
                Global.MachineState = GEnumEx.MachineState.MachineStop;
                Global.SystemRun = false;
                return-1;
            }
            //整体补偿
            context.Wx = Mx + Global.Parm.Offset_X;
            context.Wy = My + Global.Parm.Offset_Y;

            context.Log($"计算目标位置: X={context.Wx}, Y={context.Wy}, R={context.R}");

            switch(context.Mode)
            {
                case 0:
                    context.CurrentStep= 22;
                    break;
                case 1:
                    Global.VisionApp.StopRunProc("Task5");
                     context.CurrentStep = 100;
                    break;
                case 2:
                    Global.VisionApp.StopRunProc("Task5");
                    context.CurrentStep = 300;
                    break;
                case 3:
                    Global.VisionApp.StopRunProc("Task5");
                    context.CurrentStep =400;
                    break;
                case 4:
                    Global.VisionApp.StopRunProc("Task5");
                    context.CurrentStep = 500;
                    break;
            }
            return context.CurrentStep;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤22：XY轴移动+R轴旋转+夹爪张开
    /// </summary>
    public class Step22_XYMoveAndRRotate : BaseStep
    {
        public override int StepId => 22;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟CCD模式判断（实际项目中替换为真实配置）
            bool isCcdMode = false;
            if (isCcdMode)
            {
                context.Log($"移动到目标位置: X={context.Wx}, Y={context.Wy}");
            }
            else
            {
                // 模拟偏移计算（实际项目中替换为真实算法）
                double offsetX = 0.1, offsetY = 0.1;
                context.Wx += offsetX;
                context.Wy += offsetY;

                // 模拟坐标补偿（实际项目中替换为真实算法）
                double compensatedX = context.Wx;
                double compensatedY = context.Wy;

                context.Log($"移动到补偿后位置: X={compensatedX}, Y={compensatedY}");
                context.Log($"旋转R轴至: {context.R}度");
                context.Log($"夹爪张开至: {context.Size}");
            }

            return 23;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤23：运动到位检查
    /// </summary>
    public class Step23_MotionPositionCheck : BaseStep
    {
        public override int StepId => 23;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟CCD模式判断（实际项目中替换为真实配置）
            bool isCcdMode = false;
            if (isCcdMode)
            {
                // 模拟XY轴到位检测（实际项目中替换为真实硬件调用）
                bool isXYOk = true;
                if (isXYOk)
                {
                    context.Log("XY轴到位");
                    context.ZHight = 50.0;
                    return 24;
                }
            }
            else
            {
                // 模拟多轴到位检测（实际项目中替换为真实硬件调用）
                bool isMotionOk = true;
                if (isMotionOk)
                {
                    context.Log("R轴和夹爪到位");
                    context.ZHight += 2.0; // 补偿高度
                    context.Log($"下针高度补偿后: {context.ZHight}");
                    return 24;
                }
            }

            return 23;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤24：Z轴下针
    /// </summary>
    public class Step24_ZAxisNeedleDown : BaseStep
    {
        public override int StepId => 24;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟Z轴下针（实际项目中替换为真实硬件调用）
            context.Log($"Z轴下针至高度: {context.ZHight}");

            return 26; // 跳过步骤25（减速）直接到26
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤25：Z轴减速
    /// </summary>
    public class Step25_ZAxisSlowDown : BaseStep
    {
        public override int StepId => 25;
        public override bool IgnorePause => true;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟Z轴位置检测（实际项目中替换为真实硬件调用）
            double zPos = context.ZHight - 1.0;
            if (context.ZHight - zPos <= 2)
            {
                context.Log("Z轴减速");
                return 26;
            }

            return 25;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤26：Z轴到位+夹爪夹紧
    /// </summary>
    public class Step26_ZAxisPositionAndClamp : BaseStep
    {
        public override int StepId => 26;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟Z轴到位检测（实际项目中替换为真实硬件调用）
            double zPos = context.ZHight;
            if (Math.Abs(zPos - context.ZHight) < 0.02)
            {
                context.Log($"夹爪夹紧至尺寸: {context.JqSize}");

                if (context.JqSize > 0 && context.JqSize < 1)
                {
                    context.Log("执行夹紧动作");
                    return 27;
                }
                else
                {
                    Thread.Sleep(50);
                    // 模拟测试计数（实际项目中替换为真实数据）
                    context.Log("测试完成，发送结果");
                    return 28;
                }
            }

            return 26;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤27：夹爪夹紧到位
    /// </summary>
    public class Step27_ClampPositionCheck : BaseStep
    {
        public override int StepId => 27;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟夹爪到位检测（实际项目中替换为真实硬件调用）
            bool isClampOk = true;
            if (isClampOk)
            {
                Thread.Sleep(50);
                context.Log("测试完成，发送结果");
                return 28;
            }

            return 27;
        }
        public override void Undo() { throw new NotImplementedException(); }
    }

    /// <summary>
    /// 步骤28：TCP结果接收
    /// </summary>
    public class Step28_TcpReceiveTestResult : BaseStep
    {
        public override int StepId => 28;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟TCP接收完成判断（实际项目中替换为真实TCP库调用）
            bool isTcpReceived = true;
            if (!isTcpReceived)
            {
                return 28;
            }

            // 模拟测试结果（实际项目中替换为真实数据）
            string tcpResult = "A:TestOk";
            string[] parts = tcpResult.Split(':');

            if (parts[0] == "A" && parts[1] == "TestOk")
            {
                // 模拟CCD模式判断（实际项目中替换为真实配置）
                bool isCcdMode = false;
                if (!isCcdMode)
                {
                    context.Log("夹爪张开");
                    return 29;
                }
                else
                {
                    return 19;
                }
            }

            return 28;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤29：Z轴回安全位
    /// </summary>
    public class Step29_ZAxisBackToSafe : BaseStep
    {
        public override int StepId => 29;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟Z轴回安全位（实际项目中替换为真实硬件调用）
            double safeHeight = 80.0;
            context.Log($"Z轴回安全位: {safeHeight}");

            return 30;
        }
        public override void Undo() { throw new NotImplementedException(); }
    }

    /// <summary>
    /// 步骤30：Z轴安全位检查
    /// </summary>
    public class Step30_ZAxisSafePositionCheck : BaseStep
    {
        public override int StepId => 30;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟Z轴位置检测（实际项目中替换为真实硬件调用）
            double zPos = 80.0;
            double safeHeight = 80.0;
            if (Math.Abs(zPos - safeHeight) < 0.1)
            {
                context.Log("Z轴到达安全位");
                return 19;
            }

            return 30;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤50：离线模式Z轴回停止位
    /// </summary>
    public class Step50_OfflineZAxisStopPos : BaseStep
    {
        public override int StepId => 50;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟Z轴回停止位（实际项目中替换为真实硬件调用）
            double stopZ = 100.0;
            context.Log($"Z轴回停止位: {stopZ}");

            return 51;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤51：Z轴停止位检查
    /// </summary>
    public class Step51_ZAxisStopPosCheck : BaseStep
    {
        public override int StepId => 51;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟Z轴位置检测（实际项目中替换为真实硬件调用）
            double zPos = 100.0;
            double stopZ = 100.0;
            if (Math.Abs(zPos - stopZ) < 0.02)
            {
                return 52;
            }

            return 51;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤52：顶升气缸下降
    /// </summary>
    public class Step52_LiftCylinderDown : BaseStep
    {
        public override int StepId => 52;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            Thread.Sleep(100);
            return 54;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤54：XY轴回停止位
    /// </summary>
    public class Step54_XYAxisBackToStop : BaseStep
    {
        public override int StepId => 54;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟XY轴回停止位（实际项目中替换为真实硬件调用）
            double stopX = 0.0, stopY = 0.0;
            context.Log($"XY轴回停止位: X={stopX}, Y={stopY}");

            return 55;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤55：R轴回零+夹爪复位
    /// </summary>
    public class Step55_RAxisResetAndClamp : BaseStep
    {
        public override int StepId => 55;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟XY轴到位检测（实际项目中替换为真实硬件调用）
            bool isXYOk = true;
            if (isXYOk)
            {
                context.Log("R轴回零，夹爪复位");
                return 56;
            }

            return 55;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤56：流程结束
    /// </summary>
    public class Step56_ProcessFinish : BaseStep
    {
        public override int StepId => 56;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟所有轴到位检测（实际项目中替换为真实硬件调用）
            bool isAllAxisOk = true;
            if (isAllAxisOk)
            {
                context.Log("加工完成，停止所有动作");
                context.StopProcess("加工完成");
                return 0; // 回到初始步骤
            }

            return 56;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤60：在线模式Z轴回停止位
    /// </summary>
    public class Step60_OnlineZAxisStopPos : BaseStep
    {
        public override int StepId => 60;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟Z轴回停止位（实际项目中替换为真实硬件调用）
            double stopZ = 100.0;
            context.Log($"Z轴回停止位: {stopZ}");

            return 61;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤61：Z轴停止位检查
    /// </summary>
    public class Step61_OnlineZAxisStopPosCheck : BaseStep
    {
        public override int StepId => 61;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟Z轴位置检测（实际项目中替换为真实硬件调用）
            double zPos = 100.0;
            double stopZ = 100.0;
            if (Math.Abs(zPos - stopZ) < 0.02)
            {
                return 62;
            }

            return 61;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤62：调宽复位+顶升下降+放板
    /// </summary>
    public class Step62_AdjustWidthReset : BaseStep
    {
        public override int StepId => 62;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟调宽复位（实际项目中替换为真实硬件调用）
            context.Log("调宽轴复位，顶升气缸下降");
            Thread.Sleep(500);
            context.Log("等待下位机要板信号");

            return 63;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤63：下位机要板信号检查
    /// </summary>
    public class Step63_DownMachineBoardCheck : BaseStep
    {
        public override int StepId => 63;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟下位机要板信号（实际项目中替换为真实硬件调用）
            bool isBoardRequested = true;
            if (isBoardRequested)
            {
                context.Log("收到下位机要板信号");
                return 64;
            }

            return 63;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤64：皮带出板
    /// </summary>
    public class Step64_BeltBoardOut : BaseStep
    {
        public override int StepId => 64;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟调宽轴到位检测（实际项目中替换为真实硬件调用）
            bool isWidthOk = true;
            if (isWidthOk)
            {
                context.Log("皮带启动，出板");
                return 65;
            }

            return 64;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤65：XY轴回停止位
    /// </summary>
    public class Step65_XYAxisStopPos : BaseStep
    {
        public override int StepId => 65;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟XY轴回停止位（实际项目中替换为真实硬件调用）
            double stopX = 0.0, stopY = 0.0;
            context.Log($"XY轴回停止位: X={stopX}, Y={stopY}");

            return 66;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤66：出口传感器检查
    /// </summary>
    public class Step66_ExitSensorCheck : BaseStep
    {
        public override int StepId => 66;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟出口传感器检测（实际项目中替换为真实硬件调用）
            bool isExitSensorOn = true;
            if (isExitSensorOn)
            {
                context.Log("检测到板到达出口，皮带加速");
                return 67;
            }

            return 66;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤67：出板完成检查
    /// </summary>
    public class Step67_BoardOutFinishCheck : BaseStep
    {
        public override int StepId => 67;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟出口传感器检测（实际项目中替换为真实硬件调用）
            bool isBoardOut = true;
            if (isBoardOut)
            {
                context.Log("板已出完，停止皮带");
                return 68;
            }

            return 67;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤68：在线流程结束
    /// </summary>
    public class Step68_OnlineProcessFinish : BaseStep
    {
        public override int StepId => 68;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            context.Log("在线加工完成，停止所有动作");
            context.StopProcess("在线加工完成");
            return 0; // 回到初始步骤
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤100：视觉二次定位Z轴到拍照高度
    /// </summary>
    public class Step100_VisionRectifyZAxis : BaseStep
    {
        public override int StepId => 100;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟Z轴移动（实际项目中替换为真实硬件调用）
            double camHeight = 50.0;
            context.Log($"Z轴到拍照高度: {camHeight}");

            return 101;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤101：Z轴拍照高度检查
    /// </summary>
    public class Step101_VisionRectifyZCheck : BaseStep
    {
        public override int StepId => 101;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟Z轴位置检测（实际项目中替换为真实硬件调用）
            double zPos = 50.0;
            double camHeight = 50.0;
            if (Math.Abs(zPos - camHeight) < 0.02)
            {
                return 103;
            }

            return 101;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤103：视觉二次定位XY移动
    /// </summary>
    public class Step103_VisionRectifyXYMove : BaseStep
    {
        public override int StepId => 103;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟XY轴移动（实际项目中替换为真实硬件调用）
            context.Log($"移动到目标位置: X={context.Wx}, Y={context.Wy}");

            return 104;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤104：XY到位检查
    /// </summary>
    public class Step104_VisionRectifyXYCheck : BaseStep
    {
        public override int StepId => 104;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟XY轴到位检测（实际项目中替换为真实硬件调用）
            bool isXYOk = true;
            if (isXYOk)
            {
                Thread.Sleep(100);
                return 105;
            }

            return 104;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤105：视觉二次定位参数设置
    /// </summary>
    public class Step105_VisionRectifyParam : BaseStep
    {
        public override int StepId => 105;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟视觉参数设置（实际项目中替换为真实视觉库调用）
            double L = 10.0, W = 5.0;
            context.Log($"设置视觉参数: L={L}, W={W}, R={context.R}");

            return 106;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤106：视觉二次定位结果处理
    /// </summary>
    public class Step106_VisionRectifyResult : BaseStep
    {
        public override int StepId => 106;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟视觉处理完成判断（实际项目中替换为真实视觉库调用）
            bool isVisionDone = true;
            if (!isVisionDone)
            {
                return 106;
            }

            // 模拟视觉结果（实际项目中替换为真实视觉库调用）
            bool isVisionOk = true;
            if (isVisionOk)
            {
                context.Dx = 0.05;
                context.Dy = 0.05;

                // 模拟偏差检查（实际项目中替换为真实配置）
                double maxDx = 0.1, maxDy = 0.1;
                if (Math.Abs(context.Dx) > maxDx || Math.Abs(context.Dy) > maxDy)
                {
                    context.ShowWarning("视觉定位", "偏差超出范围");
                    context.StopProcess("视觉定位偏差超限");
                    return -1;
                }

                context.Wx += context.Dx;
                context.Wy += context.Dy;
                context.Log($"视觉补偿: Dx={context.Dx:F3}, Dy={context.Dy:F3}");
                return 107;
            }
            else
            {
                context.ShowWarning("视觉定位", "二次定位失败");
                context.StopProcess("视觉二次定位失败");
                return -1;
            }
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤107：视觉二次定位角度调整
    /// </summary>
    public class Step107_VisionRectifyAngle : BaseStep
    {
        public override int StepId => 107;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟角度调整（实际项目中替换为真实算法）
            context.Log("角度调整完成");
            return 108;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤108：视觉二次定位Z轴减速
    /// </summary>
    public class Step108_VisionRectifyZSlowDown : BaseStep
    {
        public override int StepId => 108;
        public override bool IgnorePause => true;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟Z轴减速（实际项目中替换为真实硬件调用）
            context.Log("Z轴减速");
            return 109;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤109：视觉二次定位Z轴检查
    /// </summary>
    public class Step109_VisionRectifyZCheck : BaseStep
    {
        public override int StepId => 109;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟Z轴到位检测（实际项目中替换为真实硬件调用）
            bool isZOk = true;
            if (isZOk)
            {
                return 110;
            }

            return 109;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤110：视觉二次定位完成
    /// </summary>
    public class Step110_VisionRectifyFinish : BaseStep
    {
        public override int StepId => 110;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            context.Log("视觉二次定位完成");
            return 22; // 回到步骤22继续执行
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤200：离线模式开始
    /// </summary>
    public class Step200_OfflineModeStart : BaseStep
    {
        public override int StepId => 200;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            context.Log("离线模式开始执行");
            return 21; // 直接进入坐标计算步骤
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤300：激光检测Z轴移动
    /// </summary>
    public class Step300_LaserCheckZAxis : BaseStep
    {
        public override int StepId => 300;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟Z轴移动（实际项目中替换为真实硬件调用）
            context.Log("Z轴移动到激光检测高度");

            return 301;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤301：激光检测Z轴位置检查
    /// </summary>
    public class Step301_LaserCheckZPosition : BaseStep
    {
        public override int StepId => 301;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟Z轴到位检测（实际项目中替换为真实硬件调用）
            bool isZOk = true;
            if (isZOk)
            {
                return 302;
            }

            return 301;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤302：激光检测参数设置
    /// </summary>
    public class Step302_LaserCheckParam : BaseStep
    {
        public override int StepId => 302;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟激光参数设置（实际项目中替换为真实激光库调用）
            context.Log("设置激光检测参数");

            return 303;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤303：激光检测执行
    /// </summary>
    public class Step303_LaserCheckExecute : BaseStep
    {
        public override int StepId => 303;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟激光检测（实际项目中替换为真实激光库调用）
            context.Log("执行激光检测");

            return 304;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤304：激光检测结果处理
    /// </summary>
    public class Step304_LaserCheckResult : BaseStep
    {
        public override int StepId => 304;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟激光结果（实际项目中替换为真实激光库调用）
            bool isLaserOk = true;
            if (isLaserOk)
            {
                context.Log("激光检测通过");
                return 305;
            }
            else
            {
                context.ShowWarning("激光检测", "检测失败");
                context.StopProcess("激光检测失败");
                return -1;
            }
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤305：激光检测完成
    /// </summary>
    public class Step305_LaserCheckFinish : BaseStep
    {
        public override int StepId => 305;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            context.Log("激光检测完成");
            return 22; // 回到步骤22继续执行
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤400：首次纠偏Z轴移动
    /// </summary>
    public class Step400_RectifyFirstZAxis : BaseStep
    {
        public override int StepId => 400;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟Z轴移动（实际项目中替换为真实硬件调用）
            context.Log("Z轴移动到纠偏高度");

            return 401;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤401：首次纠偏Z轴检查
    /// </summary>
    public class Step401_RectifyFirstZCheck : BaseStep
    {
        public override int StepId => 401;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟Z轴到位检测（实际项目中替换为真实硬件调用）
            bool isZOk = true;
            if (isZOk)
            {
                return 402;
            }

            return 401;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤402：首次纠偏参数设置
    /// </summary>
    public class Step402_RectifyFirstParam : BaseStep
    {
        public override int StepId => 402;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟纠偏参数设置（实际项目中替换为真实算法）
            context.Log("设置首次纠偏参数");

            return 403;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤403：首次纠偏执行
    /// </summary>
    public class Step403_RectifyFirstExecute : BaseStep
    {
        public override int StepId => 403;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟首次纠偏（实际项目中替换为真实算法）
            context.Log("执行首次纠偏");

            return 404;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤404：首次纠偏结果处理
    /// </summary>
    public class Step404_RectifyFirstResult : BaseStep
    {
        public override int StepId => 404;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟纠偏结果（实际项目中替换为真实算法）
            bool isRectifyOk = true;
            if (isRectifyOk)
            {
                context.Log("首次纠偏通过");
                return 405;
            }
            else
            {
                context.ShowWarning("首次纠偏", "纠偏失败");
                context.StopProcess("首次纠偏失败");
                return -1;
            }
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤405：首次纠偏完成
    /// </summary>
    public class Step405_RectifyFirstFinish : BaseStep
    {
        public override int StepId => 405;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            context.Log("首次纠偏完成");
            return 22; // 回到步骤22继续执行
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤500：纠偏检查Z轴移动
    /// </summary>
    public class Step500_RectifyCheckZAxis : BaseStep
    {
        public override int StepId => 500;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟Z轴移动（实际项目中替换为真实硬件调用）
            context.Log("Z轴移动到纠偏检查高度");

            return 501;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤501：纠偏检查Z轴检查
    /// </summary>
    public class Step501_RectifyCheckZCheck : BaseStep
    {
        public override int StepId => 501;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟Z轴到位检测（实际项目中替换为真实硬件调用）
            bool isZOk = true;
            if (isZOk)
            {
                return 502;
            }

            return 501;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤502：纠偏检查参数设置
    /// </summary>
    public class Step502_RectifyCheckParam : BaseStep
    {
        public override int StepId => 502;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟检查参数设置（实际项目中替换为真实算法）
            context.Log("设置纠偏检查参数");

            return 503;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤503：纠偏检查执行
    /// </summary>
    public class Step503_RectifyCheckExecute : BaseStep
    {
        public override int StepId => 503;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟纠偏检查（实际项目中替换为真实算法）
            context.Log("执行纠偏检查");

            return 504;
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤504：纠偏检查结果处理
    /// </summary>
    public class Step504_RectifyCheckResult : BaseStep
    {
        public override int StepId => 504;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            // 模拟检查结果（实际项目中替换为真实算法）
            bool isCheckOk = true;
            if (isCheckOk)
            {
                context.Log("纠偏检查通过");
                return 505;
            }
            else
            {
                context.ShowWarning("纠偏检查", "检查失败");
                context.StopProcess("纠偏检查失败");
                return -1;
            }
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 步骤505：纠偏检查完成
    /// </summary>
    public class Step505_RectifyCheckFinish : BaseStep
    {
        public override int StepId => 505;

        public override int Execute(StepContext context)
        {
            if (CheckStop(context)) return -1;

            context.Log("纠偏检查完成");
            return 22; // 回到步骤22继续执行
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }
}
#endregion