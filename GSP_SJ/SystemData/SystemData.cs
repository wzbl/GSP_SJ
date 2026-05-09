using BrowApp;
using BrowApp.Language;
using LightController;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GSP.SystemData
{
    public class SystemData:BaseData
    {
        public SystemData(string Path) { SavePath = Path;}


        private  string SavePath;
        public SystemData() { }
        
        public class PosType
        {
            public double Xpos;
            public double Ypos;
            public double Zpos;
        }

        public class LedType
        {
            public int LED_R { get; set; }
            public int LED_G { get; set; }
            public int LED_B { get; set; }
        }
        public class RcType
        {
            public double Row { get; set; } = 0;
            public double Col { get; set; } = 0;
        }

        /// <summary>
        ///  XY速度百分比
        /// </summary>
        public double XYVelocity;
        /// <summary>
        ///  XY速度百分比
        /// </summary>
        public double RVelocity;
        /// <summary>
        ///  XY速度百分比
        /// </summary>
        public double ZVelocity;
        /// <summary>
        /// CCD高度
        /// </summary>
        public double Ccdhight;
        /// <summary>
        /// 安全高度
        /// </summary>
        public double SafeHigh;
        /// <summary>
        /// 抬针高度
        /// </summary>
        public double SafeHigh2;
        /// <summary>
        /// 运行模式
        /// </summary>
        public int RunMode;
        /// <summary>
        /// 是否启动视觉监控模式
        /// </summary>
        public bool IsCcdMode;
        /// <summary>
        /// 配置文件路径
        /// </summary>
        public string CfgFile;
        /// <summary>
        /// 拼图路径
        /// </summary>
        public string PateFile;
        /// <summary>
        /// 是否启用Mark
        /// </summary>
        public bool IsNoMark;
        /// <summary>
        /// 缩放因子
        /// </summary>
        public double ZoomFactor;
        /// <summary>
        /// 轨道模式
        /// </summary>
        public int OrbModel;
        /// <summary>
        /// Z轴缓冲速度
        /// </summary>
        public double buf_Zspeed;
        /// <summary>
        /// 是否启用安全门
        /// </summary>
        public bool IsSafeDoor = true;
        /// <summary>
        /// 激光型号切换
        /// </summary>
        public int LaserType = 2;
        /// <summary>
        /// 机型选择
        /// </summary>
        public int Model = 0;
        /// <summary>
        /// 使用寿命
        /// </summary>
        public long Servicelife = 500000;
        /// <summary>
        /// 测试延时
        /// </summary>
        public int TestDlay = 10;
        /// <summary>
        /// 蜂鸣器提示时间
        /// </summary>
        public int BuzzeDely = 2000;

        /// <summary>
        /// Mark识别光亮度
        /// </summary>
        public LedType M_LED=new LedType();
        /// <summary>
        /// 拼图光亮度1
        /// </summary>
        public LedType P_LED=new LedType();
        /// <summary>
        /// 拼图光亮度
        /// </summary>
        public LedType P_LED2 = new LedType();
        /// <summary>
        /// 二次定位光亮度
        /// </summary>
        public LedType S_LED = new LedType();
        /// <summary>
        /// 停止位置
        /// </summary>
        public PosType StopPos=new PosType() ;
        /// <summary>
        /// 拼图起始位置
        /// </summary>
        public PosType PatlePos=new PosType();
        /// <summary>
        /// 相机拼图视野
        /// </summary>
        public RcType CameRaview=new RcType();
        /// <summary>
        /// 相机单拍视野
        /// </summary>
        public RcType sCameRaview=new RcType();
        /// <summary>
        /// 图片裁切尺寸
        /// </summary>
        public RcType Cutsize = new RcType();

        /// <summary>
        /// 轨道补偿
        /// </summary>
        public double Trackoffset;
        /// <summary>
        /// 进板延时
        /// </summary>
        public double InDaytime;

        /// <summary>
        /// 老化测试位置
        /// </summary>
        public DataTable AgingTable { get; set; } = new DataTable();

        public string LightCom { get; set; }="com3";

        public string LaserCom{ get; set; }= "com2";

        /// <summary>
        /// 轨道零点宽度
        /// </summary>
        public double GdWight;

        public void Save()
        {
            SavePath = Global.GlabPath + "/Systemfile/SysTem.sys";
            this.Save(SavePath, this);
        }
        public bool Read(ref SystemData calib)
        {
            try
            {
                SavePath = Global.GlabPath +"/Systemfile/SysTem.sys";
                calib = this.Read<SystemData>(SavePath);
                return true;
            }
            catch (Exception ex) { APP.Log.W_Log(ex.ToString()); return false; }
        } 
    }
}
