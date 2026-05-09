using BrowApp;
using BrowApp.IO;
using BrowLib;
using BrowLib.FileClass;
using BrowLib.Motion;
using CKVisionAppNet;
using GSP.Laser;
using GSP.Light;
using GSP.SystemData;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GSP
{
    public class Global
    {

        // 格式：主版本.次版本.构建日期.构建时间
        // 例如：1.0.20231225.1430
        // 1.0 - 主次版本（手动维护）
        // 20231225 - 构建日期（自动生成）
        // 1430 - 构建时间（小时分钟，自动生成）

        /// <summary>
        /// 软件版本号  alpha（内测版） beta（公测版） rc（预发布版）Release：标准版
        ///（1）主版本号：当功能模块有较大的变动，比如增加模块或是整体架构发生变化。此版本 号由项目经理决定是否修改。
        ///（2）次版本号：相对于主版本号而言，次版本号的升级对应的只是局部的变动，但该局部 的变动造成程序和以前版本不能兼容，或者对该程序以前的协作关系产生了破坏，或者是功能上有大的改进或增强。此版本号由项目决定是否修改。
        ///（3）修订版本号：一般是Bug 的修复或是一些小的变动或是一些功能的扩充，要经常发布 修订版，修复一个严重 Bug 即可发布一个修订版。此版本号由项目经理决定是否修改。
        ///（4）日期版本号：用于记录修改项目的当前日期，每天对项目的修改都需要更改日期版本 号。此版本号由开发人员决定是否修改。
        /// </summary>
        public static string Vosion = "V2.0.1.20250821_beta";

        #region 初始化轴
        /// <summary>
        /// 初始化控制器
        /// </summary>
        public static BrowLib.Controller Mot = new Controller(GEnumEx.CardType.LTDMC, GEnumEx.IOType.DMC640, 2);
        /// <summary>
        /// 控制器添加轴对象
        /// </summary>
        public static MotAPI X轴 = Controller.AddAxis("X轴", GEnumEx.MotorType.Servo);
        public static MotAPI Y轴 = Controller.AddAxis("Y轴", GEnumEx.MotorType.Servo);
        public static MotAPI Z轴 = Controller.AddAxis("Z轴", GEnumEx.MotorType.Servo);
        public static MotAPI R轴 = Controller.AddAxis("R轴", GEnumEx.MotorType.Servo);
        public static MotAPI 左夹爪轴 = Controller.AddAxis("左夹爪轴", GEnumEx.MotorType.Step);
        public static MotAPI 右夹爪轴 = Controller.AddAxis("右夹爪轴", GEnumEx.MotorType.Step);
        public static MotAPI 调宽 = Controller.AddAxis("调宽", GEnumEx.MotorType.Step);
        public static MotAPI 皮带 = Controller.AddAxis("皮带", GEnumEx.MotorType.Step);
        public static MotAPI 顶升轴 = Controller.AddAxis("顶升轴", GEnumEx.MotorType.Step);
        #endregion

        #region 用户变量
        /// <summary>
        /// 用户名
        /// </summary>
        public static string UserName = null;
        /// <summary>
        /// 权限
        /// </summary>
        public static string Authority = null;
        /// <summary>
        /// 密码
        /// </summary>
        public static string Password = null;

        /// <summary>
        /// 用户权限刷新事件
        /// </summary>
        public static EventHandler UserHandler;
        #endregion

        /// <summary>
        /// 图像长度
        /// </summary>
        public static int ImageWidth = 2448;
        /// <summary>
        /// 图像宽度
        /// </summary>
        public static int ImageHight = 2048;

        //单张图像压缩比例
        public static double Ratio = 3;
        //图像整体压缩比例
        public static double ARatio = 1;
        /// <summary>
        /// 是否启用下相机
        /// </summary>
        public static bool Is_DownCam = false;
        /// <summary>
        /// 是否启用气压监测
        /// </summary>
        public static bool Is_Airpressure = false;
        /// <summary>
        /// 安全门是否在运行状态下监控
        /// </summary>
        public static bool IsRunState = false;

        /// <summary>
        /// 设备状态
        /// </summary>
        public static BrowLib.GEnumEx.MachineState MachineState = BrowLib.GEnumEx.MachineState.MachineStop;
        /// <summary>
        /// 速度百分比
        /// </summary>
        public static double XYVelocity = 0.5, RVelocity = 0.5, ZVelocity = 0.5;
        /// <summary>
        /// CCD模式是否启用
        /// </summary>
        public static bool IsCcdMode = true;
        /// <summary>
        /// 是否启用Mark
        /// </summary>
        public static bool Is_NoMark = false;

        /// <summary>
        /// 测试次数
        /// </summary>
        public static long TextNum;

        /// <summary>
        /// 蜂鸣器提示时间
        /// </summary>
        public static int BuzzeDely = 5000;

        /// <summary>
        /// 相机高度
        /// </summary>
        public static double CamHeight;
        /// <summary>
        /// 高度补偿
        /// </summary>
        public static double Hoffset;

        /// <summary>
        /// 运行模式 O单机模式 1联机模式
        /// </summary>
        public static int RunMode = 0;
        /// <summary>
        /// 轨道模式
        /// </summary>
        public static int OrbModel = 1;

        /// <summary>
        /// 机型变量 0离线  1在线  
        /// </summary>
        public static int Model = 1;
        public static string sModel;

        /// <summary>
        /// 蜂鸣器提示标记
        /// </summary>
        public static bool Buzzerflag = false;

        /// <summary>
        /// 激光型号
        /// </summary>
        public static int LaserType = 0;
        public static string sLaserType;

        /// <summary>
        /// 配方路径
        /// </summary>
        public static string RecipePatn = "def";
        public static string FBCCode;
        /// <summary>
        /// 是否加载配方变量
        /// </summary>
        public static bool IsRecipe = false;
        /// <summary>
        /// 实时显示
        /// </summary>
        public static bool CamGlab = false;
        /// <summary>
        /// 视觉模块全局对象
        /// </summary>
        public static VisionApp VisionApp = new VisionApp();
        /// <summary>
        /// 路径
        /// </summary>
        public static string GlabPath = Directory.GetCurrentDirectory().ToString();

        /// <summary>
        /// 配方数据刷新事件
        /// </summary>
        public static EventHandler RefFormHandler;

        /// <summary>
        /// 全局刷新事件
        /// </summary>
        public static Action GlobRefEvent;

        public static List<Spectype> Specificatio = new List<Spectype>();

        public static double MaxDx = 0.8;
        public static double MaxDy = 0.8;


        /// <summary>
        /// 系统运行标志
        /// </summary>
        public static bool SystemRun = false;
        /// <summary>
        /// 系统暂停标志
        /// </summary>
        public static bool PauseFlag = false;
        /// <summary>
        /// 系统停止标志
        /// </summary>
        public static bool StopFlag = false;
        /// <summary>
        /// 系统报警标志
        /// </summary>
        public static bool AlarmFlag = false;
        /// <summary>
        /// 系统初始化标志
        /// </summary>
        public static bool SystemInitialOk = false;
        /// <summary>
        /// 设备状态
        /// </summary>
        public static string strMachineState;

        /// <summary>
        /// 全局Bom刷新事件
        /// </summary>
        public static EventHandler GlobBomRefEvent;

        public static Queue<string> FAIapi = new Queue<string>();
        /// <summary>
        /// 视觉刷新事件
        /// </summary>
        public static EventHandler VisproEvent;

        public static LBase Light = new YC_Light();
        /// <summary>
        /// 激光全局对象
        /// </summary>
        public static LaserEx Laser = new LaserEx();

        /// <summary> 
        /// Tcp通讯对象
        /// </summary>
        public static TcpClass TcpClass =new TcpClass();
        public struct Spectype
        {
            public string name;
            public double L;
            public double K;
            public double G;
        }

        /// <summary>
        /// 系统参数
        /// </summary>
        public static SystemData.SystemData Systemdata = new SystemData.SystemData(Global.GlabPath + "/Systemfile/SysTem.sys");
        /// <summary>
        /// 标定参数
        /// </summary>
        public static CalibData CalibData = new CalibData(Global.GlabPath + "/Systemfile/CalibData.dat");

        /// <summary>
        /// MES配置参数
        /// </summary>
        public static MesConfig mesConfig=new MesConfig(Global.GlabPath + "/Systemfile/MesConfig.dat");
        /// <summary>
        /// 配方数据
        /// </summary>
        public static ParmData Parm = new ParmData();

        /// <summary>
        /// Bom原始数据
        /// </summary>
        public static DataTable BomData = new DataTable();
        /// <summary>
        /// 系统补偿器类
        /// </summary>
        public static CoordinateCompensator coordinateCompensator = new CoordinateCompensator();
        /// <summary>
        /// 像素比
        /// </summary>
        public static double Pix;


        /// <summary>
        /// 最大XY速度
        /// </summary>
        public static double MaxXYVel = 900;
        /// <summary>
        /// 最大XY加速度
        /// </summary>
        public static double MaxXYAcc = 9000;

        public static double MaxZVel = 400;

        public static double MaxZAcc = 5000;

        public static double MaxRVel = 700;

        public static double MaxRAcc = 6000;

        /// <summary>
        /// 运行速度
        /// </summary>
        public static double RunXYVel = MaxXYVel * XYVelocity;
        /// <summary>
        /// 运行加速度
        /// </summary>
        public static double RunXYAcc = MaxXYAcc * XYVelocity;
        public static double RunZVel = MaxZVel * ZVelocity;
        public static double RunZAcc = MaxZAcc * ZVelocity;
        public static double RunRVel = MaxRVel * RVelocity;
        public static double RunRAcc = MaxRAcc * RVelocity;





        #region 全局静态方法
        /// <summary>
        /// 刷新速度
        /// </summary>
        public static void RrfSpd()
        {
            RunXYVel = MaxXYVel * XYVelocity;
            RunXYAcc = MaxXYAcc * XYVelocity;

            RunZVel = MaxZVel * ZVelocity;
            RunZAcc = MaxZAcc * ZVelocity;

            RunRVel = MaxRVel * RVelocity;
            RunRAcc = MaxRAcc * RVelocity;
        }
        public static void ReadFAIConfig(out string Pcbcode, out string XYCode, out string BoardSide, out string ProductCode, out string ProductName)
        {
            BrowLib.FileClass.RwIni iniFile = new BrowLib.FileClass.RwIni(Global.Systemdata.CfgFile, "", Encoding.Default);
            Pcbcode = iniFile.Read("AutoFAI", "FBCCode", "AutoFAI", "UTF-8");
            XYCode = iniFile.Read("AutoFAI", "XYCode", "", "UTF-8");

            BoardSide = iniFile.Read("AutoFAI", "BoardSide", "AutoFAI", "UTF-8");
            ProductCode = iniFile.Read("AutoFAI", "ProductCode", "AutoFAI", "UTF-8");
            ProductName = iniFile.Read("AutoFAI", "ProductName", "AutoFAI", "UTF-8");
        }

        public static string ReadAuthority()
        {

            try
            {
                BrowLib.FileClass.RwIni iniFile = new BrowLib.FileClass.RwIni(Global.Systemdata.CfgFile, "", Encoding.Default);
                return iniFile.Read("AutoFAI", "AutoRoleType", "", "UTF-8");
            }
            catch { return null; }
           
        }


        /// <summary>
        /// 读取系统参数
        /// </summary>
        public static void GetSystem()
        {
            try
            {
                string raito, araito, Width, Hight, LightName, CameraMode, AirPressure, IsRunState_str;
                BrowLib.FileClass.RwIni iniFile = new BrowLib.FileClass.RwIni(Global.GlabPath + "\\System.ini", "", Encoding.Default);

                raito = iniFile.Read("压缩比例", "Ratio", "");
                araito = iniFile.Read("压缩比例","ARatio", "");

                Width = iniFile.Read("相机像素", "Width", "0");
                Hight = iniFile.Read("相机像素", "Hight", "0");
                LightName=iniFile.Read("光源名称", "LightName","");
                CameraMode= iniFile.Read("相机配置", "CameraMode", "");
                AirPressure= iniFile.Read("气压监测", "IsAirPressure", "0");
                IsRunState_str = iniFile.Read("安全门", "IsRunState", "0");
                switch (LightName)
                {
                    case "YC":
                        Light = new YC_Light();
                        break;
                        case "HG":
                        Light = new HG_Light();
                        break;
                    default:
                        Light = new YC_Light();
                        break;
                }
                Global.Ratio = Convert.ToDouble(raito);
                Global.ARatio = Convert.ToDouble(araito);
                Global.ImageWidth = Convert.ToInt32(Width);
                Global.ImageHight = Convert.ToInt32(Hight);

                Global.Is_DownCam = StringToBool(CameraMode);
                Global.Is_Airpressure= StringToBool(AirPressure);
                Global.IsRunState = StringToBool(IsRunState_str);
            }
            catch { }
        }
        private static bool StringToBool(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("值不能为空或空白");

            switch (value.Trim().ToLower())
            {
                case "1":
                case "true":
                case "yes":
                case "on":
                case "t":
                    return true;

                case "0":
                case "false":
                case "no":
                case "off":
                case "f":
                    return false;

                default:
                    throw new FormatException($"无法将字符串 '{value}' 转换为布尔值");
            }
        }
        public static string GetModel()
        {
            string Model;
            BrowLib.FileClass.RwIni iniFile = new BrowLib.FileClass.RwIni(Global.GlabPath + "\\System.ini", "", Encoding.Default);
            Model = iniFile.Read("机型", "Model", "");
            switch (Model)
            {
                case "FAI993"://离线机型
                    Global.Model = 0;
                    break;
                case "FAI991"://在线机型
                    Global.Model = 1;
                    break;
                case "FAI860"://在线机型
                    Global.Model = 2;
                    break;
                case "FAI860P":
                    Global.Model = 3;
                    break;
                default:
                    Global.Model = 0;
                    break;
            }
            return Model;
        }
        /// <summary>
        /// 初始化板卡
        /// </summary>
        /// <returns></returns>
        public static bool INICard()
        {
            bool result;
            try
            {
                RwIni iniFile = new RwIni(Global.GlabPath + "\\System.ini", "", Encoding.Default);
                string Model = iniFile.Read<string>("CARDModel", "Model", "GCS", null);
                string text = Model;
                string a = text;
                if (!(a == "DMC"))
                {
                    if (a == "GCS")
                    {
                        Global.Mot = new Controller(GEnumEx.CardType.GCS, GEnumEx.IOType.DMC640, 2);
                        bool RTN = Controller.CardAPI.InitCard(1, new int[] { 8, 8 }, 0, false, new string[] { "./GCS.cfg" });
                    }
                }
                else
                {
                    Global.Mot = new Controller(GEnumEx.CardType.LTDMC, GEnumEx.IOType.DMC640, 2);
                    bool RTN = Controller.CardAPI.InitCard(1, new int[] { 8, 8 }, 0, false, new string[] { "./DMC3800.ini" });
                }
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }
        public static string GetLaserType()
        {
            string LaserType;
            BrowLib.FileClass.RwIni iniFile = new BrowLib.FileClass.RwIni(Global.GlabPath + "\\System.ini", "", Encoding.Default);
            LaserType = iniFile.Read("激光型号", "LaserType", "");
            switch (LaserType)
            {
                case "HL-G103-S-J"://松下激
                    Global.LaserType = 0;
                    break;
                case "SD22-35-485-VA"://深视
                    Global.LaserType = 1;
                    break;
                case "SD33-35-485-VA"://深视
                    Global.LaserType = 2;
                    break;
                case "HG-C1050"://松下模拟量
                    Global.LaserType = 3;
                    break;
                case "PDL-050-485"://PH0SKEY
                    Global.LaserType = 4;
                    break;
                default:
                    Global.LaserType = 2;
                    break;
            }
            return LaserType;
        }

        public static void WriteTextNum()
        {
            BrowLib.FileClass.RwIni iniFile = new BrowLib.FileClass.RwIni(Global.GlabPath + "\\TextNum.ini", "", Encoding.Default);
            iniFile.WriteIni("测试针使用次数", "TextNum", Global.TextNum.ToString());
        }
        public static void ReadTextNum()
        {
            BrowLib.FileClass.RwIni iniFile = new BrowLib.FileClass.RwIni(Global.GlabPath + "\\TextNum.ini", "", Encoding.Default);
            string num = iniFile.Read("测试针使用次数", "TextNum", "1");
            Global.TextNum = Convert.ToInt64(num);
        }

        public static void GetSpecifications()
        {
            try
            {
                DataTable data = new BrowLib.FileClass.DataCsv().OpenCsv(Global.GlabPath + "\\Specifications.csv");
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    Specificatio.Add(new Spectype { name = data.Rows[i][0].ToString(), L = Convert.ToDouble(data.Rows[i][1]), K = Convert.ToDouble(data.Rows[i][2]), G = Convert.ToDouble(data.Rows[i][3]) });
                }
            }
            catch { }

        }
        /// <summary>
        /// 获取下针高度
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static double GetHight(string Type)
        {
            try
            {
                var Re= CalibData.Sizes.Where(H=>string.Equals(H.Name, Type, StringComparison.OrdinalIgnoreCase)).ToList();
                if (Re.Count>0)
                {
                  return  Convert.ToDouble(Re.First().Hight);
                }
                else
                {
                    return 0;
                }
            }
            catch { return 0; }
        }
        /// <summary>
        /// 获取开口大小
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static double GetSize(string Type)
        {
            try
            {
                var Re = CalibData.Sizes.Where(H => string.Equals(H.Name, Type, StringComparison.OrdinalIgnoreCase)).ToList();
                if (Re.Count > 0)
                {
                    return Convert.ToDouble(Re.First().Size);
                }
                else
                {
                    return 0;
                }
            }
            catch { return 0; }
        }
        /// <summary>
        /// 获取元件长度
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static double GetL(string Type)
        {
            try
            {
                var Re = CalibData.Sizes.Where(H => string.Equals(H.Name, Type, StringComparison.OrdinalIgnoreCase)).ToList();
                if (Re.Count > 0)
                {
                    return Convert.ToDouble(Re.First().Length);
                }
                else
                {
                    return 0;
                }
            }
            catch { return 0; }
        }
        ///<summary>
        /// 获取元件宽度
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static double GetW(string Type)
        {
            try
            {
                var Re = CalibData.Sizes.Where(H => string.Equals(H.Name, Type, StringComparison.OrdinalIgnoreCase)).ToList();
                if (Re.Count > 0)
                {
                    return Convert.ToDouble(Re.First().width);
                }
                else
                {
                    return 0;
                }
            }
            catch { return 0; }
        }
        /// <summary>
        /// 获取夹取尺寸
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static double JQSize(string Type)
        {
            try
            {
                var Re = CalibData.Sizes.Where(H => string.Equals(H.Name, Type, StringComparison.OrdinalIgnoreCase)).ToList();
                if (Re.Count > 0)
                {
                    return Convert.ToDouble(Re.First().Clipsize);
                }
                else
                {
                    return 0;
                }
            }
            catch { return 0; }
        }
        /// <summary>
        /// 获取相机针头偏差
        /// </summary>
        /// <param name="engle"></param>
        /// <param name="OffsetX"></param>
        /// <param name="OffsetY"></param>
        /// <returns></returns>
        public static bool GetAngleOffset(string engle, out double OffsetX, out double OffsetY)
        {

            if (CalibData.Offsets.Count <= 0)
            {
                OffsetX = 0;
                OffsetY = 0;
                return false;
            }
            try
            {
                var Re = CalibData.Offsets.Where(H => H.Angle == engle).ToList();
                if (Re.Count>0)
                {
                    OffsetX = Convert.ToDouble(Re.First().OffsetX);
                    OffsetY = Convert.ToDouble(Re.First().OffsetY);
                    return true;
                }
                else
                {
                    int Index = GetIndex(engle);
                    OffsetX = Convert.ToDouble(CalibData.Offsets[Index].OffsetX);
                    OffsetY = Convert.ToDouble(CalibData.Offsets[Index].OffsetY);
                    return true;
                }
            }
            catch
            {
                OffsetX = 0;
                OffsetY = 0;
                return false;
            }
        }
        public static bool GetAutoOffset(string engle, out double OffsetX, out double OffsetY)
        {
            double xOffset = 0, yOffset = 0, CenterdX, CenterdY, NCenterdX, NCenterdY;
            if (CalibData.Offsets.Count <= 0)
            {
                OffsetX = 0;
                OffsetY = 0;
                return false;
            }
            try
            {
                var Re = CalibData.Offsets.Where(H => H.Angle == engle).ToList();
                if (Re.Count > 0)
                {
                    OffsetX = Convert.ToDouble(Re.First().OffsetX);
                    OffsetY = Convert.ToDouble(Re.First().OffsetY);
                    return true;
                }
                else
                {
                    Global.GetAngleOffset("0", out xOffset, out yOffset);//0°偏差 作参考计算
                    new Algorithm().GetOffsetXY(Global.CalibData.PinPos.Xpos, Global.CalibData.PinPos.Ypos, 0, Global.CalibData.CenterData.CenterX, Global.CalibData.CenterData.CenterY, out CenterdX, out CenterdY);
                    new Algorithm().GetOffsetXY(Global.CalibData.PinPos.Xpos, Global.CalibData.PinPos.Ypos, Convert.ToDouble(engle), Global.CalibData.CenterData.CenterX, Global.CalibData.CenterData.CenterY, out NCenterdX, out NCenterdY);
                    OffsetX = xOffset+(NCenterdX - CenterdX);
                    OffsetY = yOffset+(NCenterdY - CenterdY);
                    return true;
                }
            }
            catch
            {
                OffsetX=0; OffsetY=0;
                return false;
            }
           
            
        }

        public static bool GetOffset(double engle, out double OffsetX, out double OffsetY,bool Enable=false)
        {
            try
            {
                if (Enable)
                {
                    GetAutoOffset(engle.ToString(), out OffsetX, out OffsetY);
                }
                else
                {
                    GetAngleOffset(engle.ToString(), out OffsetX, out OffsetY);
                }
                return true;
            }
            catch
            {
                OffsetX=0.0; OffsetY=0.0;
                return false;
            }
           
        }
        private static int GetIndex(string engle)
        {
            try
            {
                int index = 0;
                double rote = Convert.ToDouble(engle);
                double closestValue = Convert.ToDouble(CalibData.Offsets[0].Angle);
                double closestDifference = Math.Abs(rote - closestValue);

                for (int i = 0; i <CalibData.Offsets.Count; i++)
                {
                    double difference = Math.Abs(rote - Convert.ToDouble(CalibData.Offsets[i].Angle));
                    if (difference < closestDifference)
                    {
                        closestValue = Convert.ToDouble(CalibData.Offsets[i].Angle);
                        closestDifference = difference;
                        index = i;
                    }
                }
                return index;
            }
            catch { return 0; }

        }
        #endregion

    }
}
