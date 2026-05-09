using BrowApp.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GSP
{
    public class HandFlow
    {
        
            public bool HandStop { get; set; } = true;

            public double XYspd { get; set; } = 300;

            public double XYacc { get; set; } = 3000;

            public double Zspd { get; set; } = 50;
            public double Zacc { get; set; } = 2000;

            public double Rspd { get; set; } = 200;
            public double Racc { get; set; } = 2000;

            public int SafeMoveXyz(double Sefhight, double Xpos, double Ypos, double Zpos)
            {
                Global.StopFlag = false;
                HandStop = true;
                int Step = 0;
                int Rte = 0;
                while (HandStop && !Global.StopFlag)
                {
                    switch (Step)
                    {
                        case 0:
                            Global.Z轴.PMove(50, 2000, Sefhight, 1);
                            Step++;
                            break;
                        case 1:
                            if (!Global.Z轴.GetSevOn()) { Rte = 1; HandStop = false; }
                            if (Global.Z轴.ALM()) { Rte = 2; HandStop = false; }
                            if (Global.Z轴.Runing()) { Rte = 0; Step++; }
                            break;
                        case 2:
                            BrowLib.Controller.MotionAPI.LinXyMoveA(300, 3000, Xpos, Ypos, 1);
                            Step++;
                            break;
                        case 3:
                            if (!Global.X轴.GetSevOn() || !Global.Y轴.GetSevOn()) { Rte = 1; HandStop = false; }
                            if (Global.X轴.ALM() || Global.Y轴.ALM()) { Rte = 2; HandStop = false; }
                            if (BrowLib.Controller.MotionAPI.LinXYRuningA()) { Rte = 0; Step++; }
                            break;
                        case 4:
                        Global.Z轴.PMove(50, 3000, Zpos, 1);
                            Step++;
                            break;
                        case 5:
                            if (!Global.Z轴.GetSevOn()) { Rte = 1; HandStop = false; }
                            if (Global.Z轴.ALM()) { Rte = 2; HandStop = false; }
                            if (Global.Z轴.Runing()) { Rte = 0; Step = 0; HandStop = false; }
                            break;
                    }
                    Thread.Sleep(10);
                }
                return Rte;
            }
            /// <summary>
            /// 手动调宽
            /// </summary>
            /// <param name="spd"></param>
            /// <param name="Width"></param>
            /// <returns></returns>
            public int SetWidth(double spd, double Width)
            {
                Global.StopFlag = false;
                HandStop = true;
                int Step = 0;
                int Rte = 0;
                while (HandStop && !Global.StopFlag)
                {
                    switch (Step)
                    {
                        case 0:
                        Global.调宽.PMove(spd, 2000, Width, 1);
                            Step++;
                            break;
                        case 1:
                            if (!Global.调宽.GetSevOn()) { Rte = 1; HandStop = false; }
                            if (Global.调宽.ALM()) { Rte = 2; HandStop = false; }
                            if (Global.调宽.Runing()) { Rte = 0; Step = 0; HandStop = false; }
                            break;
                    }

                    Thread.Sleep(10);
                }
                return Rte;
            }

            public int SafeMoveXYZR(double XYspd, double Zspd, double Sefhight, double Xpos, double Ypos, double Zpos, double Rpos, double dis, double SLspd)
            {
            Global.StopFlag = false;
                HandStop = true;
                int Step = 0;
                int Rte = 0;
                while (HandStop && !Global.StopFlag)
                {
                    switch (Step)
                    {
                        case 0:
                        Global.Z轴.PMove(50, 2000, Sefhight, 1);
                            Step++;
                            break;
                        case 1:
                            if (!Global.Z轴.GetSevOn()) { Rte = 1; HandStop = false; }
                            if (Global.Z轴.ALM()) { Rte = 2; HandStop = false; }
                            if (Global.Z轴.Runing()) { Rte = 0; Step++; }
                            break;
                        case 2:
                        BrowLib.Controller.MotionAPI.LinXyMoveA(XYspd, 3000, Xpos, Ypos, 1);
                        Global.R轴.PMove(XYspd, 3000, Rpos, 1);
                            Step++;
                            break;
                        case 3:
                            if (!Global.X轴.GetSevOn() || !Global.Y轴.GetSevOn() || !Global.R轴.GetSevOn()) { Rte = 1; HandStop = false; }
                            if (Global.X轴.ALM() || Global.Y轴.ALM() || Global.R轴.ALM()) { Rte = 2; HandStop = false; }
                            if (BrowLib.Controller.MotionAPI.LinXYRuningA() && Global.R轴.Runing()) { Rte = 0; Step++; }
                            break;
                        case 4:
                        Global.Z轴.PMove(Zspd, 2000, Zpos - dis, 1);
                            Step++;
                            break;
                        case 5:
                            if (!Global.Z轴.GetSevOn()) { Rte = 1; HandStop = false; }
                            if (Global.Z轴.ALM()) { Rte = 2; HandStop = false; }
                            if (Global.Z轴.Runing()) { Step++; }
                            break;
                        case 6:
                            double spd = SLspd;
                            if (SLspd == 0) { spd = Zspd; }
                            Global.Z轴.PMove(spd, 2000, Zpos, 1);
                            Step++;
                            break;
                        case 7:
                            if (!Global.Z轴.GetSevOn()) { Rte = 1; HandStop = false; }
                            if (Global.Z轴.ALM()) { Rte = 2; HandStop = false; }
                            if (Global.Z轴.Runing()) { Step = 0; HandStop = false; }
                            break;
                    }
                    Thread.Sleep(10);
                }
                return Rte;
            }
            public int SafeMoveXYZR(double XYspd, double Zspd, double Sefhight, double Xpos, double Ypos, double Zpos, double Rpos)
            {
            Global.StopFlag = false;
                HandStop = true;
                int Step = 0;
                int Rte = 0;
                while (HandStop && !Global.StopFlag)
                {
                    switch (Step)
                    {
                        case 0:
                            Global.Z轴.PMove(50, 2000, Sefhight, 1);
                            Step++;
                            break;
                        case 1:
                            if (!Global.Z轴.GetSevOn()) { Rte = 1; HandStop = false; }
                            if (Global.Z轴.ALM()) { Rte = 2; HandStop = false; }
                            if (Global.Z轴.Runing()) { Rte = 0; Step++; }
                            break;
                        case 2:
                            BrowLib.Controller.MotionAPI.LinXyMoveA(XYspd, 3000, Xpos, Ypos, 1);
                            Global.R轴.PMove(XYspd, 3000, Rpos, 1);
                            Step++;
                            break;
                        case 3:
                            if (!Global.X轴.GetSevOn() || !Global.Y轴.GetSevOn() || !Global.R轴.GetSevOn()) { Rte = 1; HandStop = false; }
                            if (Global.X轴.ALM() || Global.Y轴.ALM() || Global.R轴.ALM()) { Rte = 2; HandStop = false; }
                            if (BrowLib.Controller.MotionAPI.LinXYRuningA() && Global.R轴.Runing()) { Rte = 0; Step++; }
                            break;
                        case 4:
                            Global.Z轴.PMove(Zspd, 2000, Zpos, 1);
                            Step++;
                            break;
                        case 5:
                            if (!Global.Z轴.GetSevOn()) { Rte = 1; HandStop = false; }
                            if (Global.Z轴.ALM()) { Rte = 2; HandStop = false; }
                            if (Global.Z轴.Runing()) { Step = 0; HandStop = false; }
                            break;
                    }

                    Thread.Sleep(10);
                }
                return Rte;
            }
            public int SafeMoveXYZR(double XYspd, double Zspd, double Sefhight, double Xpos, double Ypos, double Zpos, double Rpos, double Size)
            {
            Global.StopFlag = false;
                HandStop = true;
                int Step = 0;
                int Rte = 0;
                while (HandStop && !Global.StopFlag)
                {
                    switch (Step)
                    {
                        case 0:
                            Global.Z轴.PMove(50, 2000, Sefhight, 1);
                            Step++;
                            break;
                        case 1:
                            if (!Global.Z轴.GetSevOn()) { Rte = 1; HandStop = false; }
                            if (Global.Z轴.ALM()) { Rte = 2; HandStop = false; }
                            if (Global.Z轴.Runing()) { Rte = 0; Step++; }
                            break;
                        case 2:
                            BrowLib.Controller.MotionAPI.LinXyMoveA(XYspd, 3000, Xpos, Ypos, 1);
                            Global.R轴.PMove(XYspd, 3000, Rpos, 1);
                            Global.左夹爪轴.PMove(20, 2000, Size, 1);
                            Global.右夹爪轴.PMove(20, 2000, Size, 1);
                            Step++;
                            break;
                        case 3:
                            if (!Global.X轴.GetSevOn() || !Global.Y轴.GetSevOn() || !Global.R轴.GetSevOn() || !Global.左夹爪轴.GetSevOn() || !Global.右夹爪轴.GetSevOn())
                            { Rte = 1; HandStop = false; }
                            if (Global.X轴.ALM() || Global.Y轴.ALM() || Global.R轴.ALM() || Global.左夹爪轴.ALM() || Global.右夹爪轴.ALM())
                            { Rte = 2; HandStop = false; }
                            if (BrowLib.Controller.MotionAPI.LinXYRuningA() && Global.R轴.Runing() && Global.左夹爪轴.Runing() && Global.右夹爪轴.Runing())
                            { Rte = 0; Step++; }
                            break;
                        case 4:
                            Global.Z轴.PMove(Zspd, 2000, Zpos, 1);
                            Step++;
                            break;
                        case 5:
                            if (!Global.Z轴.GetSevOn()) { Rte = 1; HandStop = false; }
                            if (Global.Z轴.ALM()) { Rte = 2; HandStop = false; }
                            if (Global.Z轴.Runing()) { Step = 0; HandStop = false; }
                            break;
                    }

                    Thread.Sleep(10);
                }
                return Rte;
            }
            public int SafeMoveXYZR(double XYspd, double Zspd, double Sefhight, double Xpos, double Ypos, double Zpos, double Rpos, double Size, double dis, double Slspd)
            {
                Global.StopFlag = false;
                HandStop = true;    
                int Step = 0;
                int Rte = 0;
                while (HandStop && !Global.StopFlag)
                {
                    switch (Step)
                    {
                        case 0:
                            Global.Z轴.PMove(50, 2000, Sefhight, 1);
                            Step++;
                            break;
                        case 1:
                            if (!Global.Z轴.GetSevOn()) { Rte = 1; HandStop = false; }
                            if (Global.Z轴.ALM()) { Rte = 2; HandStop = false; }
                            if (Global.Z轴.Runing()) { Rte = 0; Step++; }
                            break;
                        case 2:
                            BrowLib.Controller.MotionAPI.LinXyMoveA(XYspd, 3000, Xpos, Ypos, 1);
                            Global.R轴.PMove(XYspd, 3000, Rpos, 1);
                            Global.左夹爪轴.PMove(20, 1000, Size, 1);
                            Global.右夹爪轴.PMove(20, 1000, Size, 1);
                            Step++;
                            break;
                        case 3:
                            if (!Global.X轴.GetSevOn() || !Global.Y轴.GetSevOn() || !Global.R轴.GetSevOn() || !Global.左夹爪轴.GetSevOn() || !Global.右夹爪轴.GetSevOn())
                            { Rte = 1; HandStop = false; }
                            if (Global.X轴.ALM() || Global.Y轴.ALM() || Global.R轴.ALM() || Global.左夹爪轴.ALM() || Global.右夹爪轴.ALM())
                            { Rte = 2; HandStop = false; }
                            if (BrowLib.Controller.MotionAPI.LinXYRuningA() && Global.R轴.Runing() && Global.左夹爪轴.Runing() && Global.右夹爪轴.Runing())
                            { Rte = 0; Step++; }
                            break;
                        case 4:
                            Global.Z轴.PMove(Zspd, 2000, Zpos - dis, 1);
                            Step++;
                            break;
                        case 5:
                            if (!Global.Z轴.GetSevOn()) { Rte = 1; HandStop = false; }
                            if (Global.Z轴.ALM()) { Rte = 2; HandStop = false; }
                            if (Global.Z轴.Runing()) { Rte = 0; Step++; }
                            break;
                        case 6:
                            Global.Z轴.PMove(Slspd, 1000, Zpos, 1);
                            Step++;
                            break;
                        case 7:
                            if (!Global.Z轴.GetSevOn()) { Rte = 1; HandStop = false; }
                            if (Global.Z轴.ALM()) { Rte = 2; HandStop = false; }
                            if (Global.Z轴.Runing()) { Step = 0; HandStop = false; }
                            break;
                    }
                    Thread.Sleep(10);
                }
                return Rte;
            }
        }
}
