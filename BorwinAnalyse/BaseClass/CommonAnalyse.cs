using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using NPOI.Util;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Menu;
using NPOI.SS.Formula.Functions;
using NPOI.XSSF.Streaming.Values;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
using System.Globalization;
using BorwinAnalyse.Model;
using NPOI.SS.Formula.Eval;
using BrowApp.Language;

namespace BorwinAnalyse.BaseClass
{
    /// <summary>
    /// BOM解析类
    /// </summary>
    public class CommonAnalyse
    {
        private static CommonAnalyse instance;
        public static CommonAnalyse Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CommonAnalyse();
                }
                return instance;
            }
        }

        public CommonAnalyse()
        {


        }

        /// <summary>
        /// 是否启用字符分割
        /// </summary>
        public bool IsSeparator = false;
        public List<Separator> Separators = new List<Separator>();

        /// <summary>
        /// 是否启用字符替换
        /// </summary>
        public bool IsSubstitutionRules = false;
        public List<SubstitutionRules> SubstitutionRules = new List<SubstitutionRules>();

        public List<GradeChange> GradeChanges = new List<GradeChange>();

        /// <summary>
        /// 用户电容偏差等级
        /// </summary>
        public List<GradeChange> GradeChangesCustCap = new List<GradeChange>();

        /// <summary>
        /// 用户电阻偏差等级
        /// </summary>      
        public List<GradeChange> GradeChangesCustRes = new List<GradeChange>();

        /// <summary>
        /// 电阻,统一RES
        /// </summary>
        public string Resistance = "resistance,RES";
        /// <summary>
        /// 电容，统一CAP
        /// </summary>
        public string Capacitance = "capacitance,CAP";

        /// <summary>
        /// 电阻单位，大写
        /// </summary>
        public string ResistanceUnit = "MΩ,KΩ,Ω,M,K,R,HΩ";

        /// <summary>
        /// 电容单位，大写
        /// </summary>
        public string CapacitanceUnit = "PF,NF,UF,P,N,U";

        /// <summary>
        /// 元件规格
        /// </summary>
        //public string ComponentSpecifications = "01005,0201,0402,0603,0805,1010,1206,1210,2010";
        public string ComponentSpecifications = "01005,0201,0402,0603,0805,1010,1206,1210";

        /// <summary>
        /// 是否有标题行
        /// </summary>
        public bool IsTitleRow = false;

        /// <summary>
        /// 是否删除字符
        /// </summary>
        public bool IsDeleteString = false;

        /// <summary>
        /// 删除前缀数
        /// </summary>
        public int PrefixNumber = 0;

        /// <summary>
        /// 删除后缀数
        /// </summary>
        public int SuffixNumber = 0;

        /// <summary>
        /// 是否启用在中间的单位((如4R7=4.7Ω 4K7=4.7KΩ) 电容中间R代表.(如0R5=0.5PF))
        /// </summary>
        public bool IsIntermediateUnit = false;

        /// <summary>
        /// 启用偏差等级(未找到时使用)
        /// </summary>
        public bool IsGrade_ON_NO_Find = false;
        public string ResGrade_ON_NO_Find = "0%";
        public string CapGrade_ON_NO_Find = "0%";

        //string s_Grade = "CDFGJKMN";

        /// <summary>
        /// 是否启用值包含偏差等级
        /// </summary>
        public bool IsValueContainsGrade = false;

        /// <summary>
        /// 识别数码法
        /// </summary>
        public bool IsIdentifyingDigits = false;

        /// <summary>
        /// 启用默认电阻单位
        /// </summary>
        public bool IsResDefaultUnit = false;

        /// <summary>
        /// 默认电阻单位
        /// </summary>
        public string ResDefaultUnit = "Ω";

        /// <summary>
        /// 启用排除字符
        /// </summary>
        public bool IsExcludeContext = false;

        /// <summary>
        /// 使用用户电阻等级
        /// </summary>
        public bool IsUseCustomerResGrade = false;

        /// <summary>
        /// 使用用户电容等级
        /// </summary>
        public bool IsUseCustomerCapGrade = false;

        /// <summary>
        /// 使用特殊位置的偏差等级
        /// </summary>
        public bool IsSearchGradeByPos = false;

        /// <summary>
        /// 特殊位置偏差等级时，目标定位字符，开始的第几位
        /// </summary>
        public string txtGradePos = "电感";

        /// <summary>
        /// 特殊位置偏差等级时，目标定位字符
        /// </summary>
        public string txtStrAfterGrade = "电感";

        /// <summary>
        /// 排除字符
        /// </summary>
        public string ExcludeContext = "电感";

        /// <summary>
        /// 是否启用替代料号
        /// </summary>
        public bool IsHadReplaceCode = false;

        /// <summary>
        /// 存在替代料号列
        /// </summary>
        public bool IsHadReplaceCodeCol = false;

        /// <summary>
        /// 相同点位的料，互为替代料
        /// </summary>
        public bool IsSameLocationRule = false;

        /// <summary>
        /// 相同点位的料，互为替代料
        /// </summary>
        public string ReplaceCodeSeparator = ",";

        /// <summary>
        /// 启用值、尺寸、等级结合
        /// </summary>
        public bool IsMergeDescription = false;
        public bool IsUseNormalRule = true;

        public List<Model.ManufactureRuleModel> ManufactureRuleModels = new List<ManufactureRuleModel>();

        public void Load()
        {
            string savePath = @"Ini/CommonAnalyse.json";
            if (!File.Exists(savePath))
            {
                FileStream fs1 = new FileStream(savePath, FileMode.Create, FileAccess.ReadWrite);
                fs1.Close();
            }
            else
            {
                instance = JsonConvert.DeserializeObject<CommonAnalyse>(File.ReadAllText(savePath));
            }
        }
        public void Save()
        {
            string savePath = @"Ini/CommonAnalyse.json";
            if (!File.Exists(savePath))
            {
                FileStream fs1 = new FileStream(savePath, FileMode.Create, FileAccess.ReadWrite);
                fs1.Close();
            }
            File.WriteAllText(savePath, JsonConvert.SerializeObject(instance));
        }

        /// <summary>
        /// 解析入口
        /// </summary>
        /// <param name="description">解析字符</param>
        public AnalyseResult AnalyseMethod(string description)
        {
            AnalyseResult analyseResult = new AnalyseResult();
            List<string> specList = new List<string>(); //分隔符处理后
            string LCR_Type = "";
            if (IsDeleteString)
            {
                description = RemoveLeft(description, PrefixNumber);
                description = RemoveRight(description, SuffixNumber);
            }

            if (IsSubstitutionRules)
            {
                SubstitutionRule(ref description);
            }

            if (IsSeparator)
            {
                specList = Separator(description);
            }

            GetType(description, ref analyseResult, ref LCR_Type);
            if (analyseResult.Type == "Other") return analyseResult;

            GetSize(description, ref analyseResult);
            if (analyseResult.Size == "Error") return analyseResult;

            var gradeResult = specList.Where("CDFGJKMN".Contains).ToList();
            if (gradeResult?.Count > 0)
                analyseResult.Grade = gradeResult[0];

            if (string.IsNullOrEmpty(analyseResult.Grade))
            {
                string textValue = string.Empty;
                for (int i = 0; i < specList?.Count; i++)
                {
                    textValue = specList[i];
                    if (textValue.StartsWith("%"))
                    {
                        if (textValue.EndsWith("%"))
                        {
                            analyseResult.Grade = Regex.Replace(textValue, @"[^\d.\d]", "") + "%";
                        }
                        else
                        {
                            string tempGrade = Regex.Replace(textValue, @"[^0-9.]", "");
                            if (IsNumeric(tempGrade))
                            {
                                analyseResult.Grade = tempGrade + "%";
                            }
                            else
                            {
                                analyseResult.Grade = tempGrade;
                            }
                        }
                        break;
                    }
                    else if (textValue.Contains("%"))
                    {
                        analyseResult.Grade = TheGrade(textValue);
                        break;
                    }
                    if (string.IsNullOrEmpty(analyseResult.Grade) && IsValueGrade(textValue))
                    {
                        if (IsValueContainsGrade)
                        {
                            Regex r = new Regex(@"[a-zA-Z]+");
                            System.Text.RegularExpressions.Match m = r.Match(textValue);
                            string grade = m.Value;
                            if (!string.IsNullOrEmpty(grade))
                            {
                                analyseResult.Grade = grade;
                                textValue = textValue.Replace(grade, "");
                                if (textValue.Length == 3 && IsNumeric(textValue))
                                {
                                    int v1 = int.Parse(textValue.Substring(0, 2));
                                    int v2 = int.Parse(textValue.Substring(2, 1));
                                    double val = v1 * Math.Pow(10, v2);
                                    textValue = val.ToString();
                                }
                                analyseResult.Value = textValue;
                                //analyseResult.Unit = "P";
                                analyseResult.Check();
                                //如果值不为空
                                if (analyseResult.Result)
                                {
                                    return analyseResult;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                string textValue = string.Empty;
                for (int i = 0; i < specList?.Count; i++)
                {
                    textValue = specList[i];
                    if (textValue.StartsWith("%"))
                    {
                        if (textValue.EndsWith("%"))
                        {
                            analyseResult.Grade = Regex.Replace(textValue, @"[^\d.\d]", "") + "%";
                        }
                        else
                        {
                            string tempGrade = Regex.Replace(textValue, @"[^0-9.]", "");
                            //string tempGrade = textValue.TrimStart('%');
                            if (IsNumeric(tempGrade))
                            {
                                analyseResult.Grade = tempGrade + "%";
                            }
                            else
                            {
                                analyseResult.Grade = tempGrade;
                            }
                        }
                        break;
                    }
                    else if (textValue.Contains("%"))
                    {
                        analyseResult.Grade = TheGrade(textValue);
                        break;
                    }
                }
            }

            //未找到偏差等级是否启用自定义
            if (string.IsNullOrEmpty(analyseResult.Grade) && IsGrade_ON_NO_Find)
            {
                if (analyseResult.Type == "电阻".tr())
                {
                    analyseResult.Grade = ResGrade_ON_NO_Find;
                }
                else if (analyseResult.Type == "电容".tr())
                {
                    analyseResult.Grade = CapGrade_ON_NO_Find;
                }
            }

            //去掉物料类型
            var typeList = specList.Where(u => u.Contains(LCR_Type)).ToList();
            foreach (var item in typeList)
            {
                specList.Remove(item);
            }
            //去掉包含规格项
            var sizeIndexList = specList.Where(u => u.Contains(analyseResult.Size)).ToList();
            foreach (var item in sizeIndexList)
            {
                specList.Remove(item);
            }
            //去掉包含偏差等级项
            if (!string.IsNullOrEmpty(analyseResult.Grade))
            {
                specList.Remove(analyseResult.Grade);
            }

            //找到可能包含值得项
            List<string> listValue = new List<string>();
            foreach (var item in specList)
            {
                if (Regex.IsMatch(item.Trim(), "^[0-9].*[A-Za-z0-9.]*$"))//修改正则表达式，当为0的时候匹配失败
                {

                    listValue.Add(item);
                }
            }

            if (listValue.Count > 0)//找到多个待处理
            {
                if (analyseResult.Type == "电阻".tr())
                {
                    //通过单位分割
                    analyseResult.Unit = "";
                    for (int i = 0; i < listValue.Count; i++)
                    {
                        string textValue = listValue[i];
                        var unitList = ResistanceUnit.Split(',').Where(textValue.Contains).ToList();
                        if (unitList?.Count > 0)
                        {
                            analyseResult.Unit = TheLongestName(unitList.ToArray());
                            //提取值
                            string[] valueArray = textValue.Split(analyseResult.Unit.ToCharArray()[0]);

                            if (valueArray.Length == 2 && string.IsNullOrEmpty(valueArray[1]))
                            {
                                analyseResult.Value = valueArray[0];
                            }
                            else if (valueArray.Length == 2 && !string.IsNullOrEmpty(valueArray[1]))
                            {
                                if (IsInt(valueArray[1]))
                                {
                                    analyseResult.Value = textValue.Replace(analyseResult.Unit.ToCharArray()[0], '.');
                                }
                                else
                                {
                                    analyseResult.Value = valueArray[0];
                                }
                            }
                            break;
                        }
                    }
                }
                else if (analyseResult.Type == "电容".tr())
                {
                    //通过单位分割
                    analyseResult.Unit = "";
                    for (int i = 0; i < listValue.Count; i++)
                    {
                        string textValue = listValue[i];
                        var unitList = CapacitanceUnit.ToUpper().Split(',').Where(textValue.ToUpper().Contains).ToList();
                        if (unitList?.Count > 0)
                        {
                            //找出最长
                            analyseResult.Unit = TheLongestName(unitList.ToArray());
                            //提取值
                            string[] valueArray = textValue.ToUpper().Split(analyseResult.Unit.ToCharArray()[0]);
                            if (valueArray.Length == 2 && string.IsNullOrEmpty(valueArray[1]))
                            {
                                analyseResult.Value = valueArray[0];
                            }
                            else if (valueArray.Length == 2 && !string.IsNullOrEmpty(valueArray[1]))
                            {
                                if (IsInt(valueArray[1]))
                                {
                                    analyseResult.Value = textValue.ToUpper().Replace(analyseResult.Unit.ToCharArray()[0], '.');
                                }
                                else
                                {
                                    analyseResult.Value = valueArray[0];
                                }
                            }
                            break;
                        }
                    }
                }

                //找出数字字符串
                if (string.IsNullOrEmpty(analyseResult.Value))
                {
                    if (analyseResult.Type == "电阻".tr())
                    {
                        for (int i = 0; i < listValue.Count; i++)
                        {
                            string textValue = listValue[i];
                            if (IsNumeric(textValue))
                            {
                                if (textValue?.Length > 0)
                                {
                                    if (IsIdentifyingDigits)
                                    {
                                        int factor = (int)Math.Pow(10, Convert.ToInt32(textValue.Substring(textValue.Length - 1)));
                                        analyseResult.Value = (Convert.ToDouble(textValue.Remove(textValue.Length - 1, 1)) * factor).ToString();
                                    }
                                    else
                                    {
                                        analyseResult.Value = textValue;
                                    }

                                }
                                break;
                            }
                            else
                            {   //电阻分析加入特殊处理 如0R5 5R00...
                                string pattern = @"(?<=\d)\D+(?=\d)";
                                Regex regex = new Regex(pattern);
                                string result = regex.Replace(textValue, ".");
                                if (IsIntermediateUnit)
                                    analyseResult.Value = Regex.Replace(result, @"[^0-9.]", "");
                                break;
                            }
                        }
                    }
                    else if (analyseResult.Type == "电容".tr())
                    {
                        for (int i = 0; i < listValue.Count; i++)
                        {
                            string textValue = listValue[i];
                            if (IsNumeric(textValue))
                            {
                                if (IsIdentifyingDigits)
                                {
                                    int factor = (int)Math.Pow(10, Convert.ToInt32(textValue.Substring(textValue.Length - 1)));
                                    analyseResult.Value = (Convert.ToDouble(textValue.Remove(textValue.Length - 1, 1)) * factor).ToString();
                                }
                                else
                                {
                                    analyseResult.Value = textValue;
                                }
                                break;
                            }
                            else
                            {
                                //电阻分析加入特殊处理 如0R5 5R00...
                                string pattern = @"(?<=\d)\D+(?=\d)";
                                Regex regex = new Regex(pattern);
                                string result = regex.Replace(textValue, ".");
                                if (IsIntermediateUnit)
                                    analyseResult.Value = Regex.Replace(result, @"[^0-9.]", "");
                                break;
                            }
                        }
                    }
                }
            }

            analyseResult.Check();

            return analyseResult;
        }

        /// <summary>
        /// 解析物料宽度
        /// </summary>
        /// <param name="spec"></param>
        /// <param name="fromTable"></param>
        /// <param name="partNumber"></param>
        /// <returns></returns>
        public void AnalyWidth(string spec, ref AnalyseResult analyseResult)
        {
            try
            {
                if (!string.IsNullOrEmpty(spec))
                {
                    string Rstr = Regex.Replace(spec, @"[^\d.\d]", "");
                    string er = Rstr.Substring(2, 2);
                    analyseResult.Width = Convert.ToDouble(Rstr.Substring(0, 2)).ToString();
                    analyseResult.Space = Convert.ToDouble(Rstr.Substring(2, 2)).ToString();
                }
            }
            catch
            {
                analyseResult.Width = string.Empty;
                analyseResult.Space = string.Empty;
            }
        }

        /// <summary>
        /// 解析入口
        /// </summary>
        /// <param name="description">解析字符</param>
        public AnalyseResult AnalyseMethod_copy(string description)
        {
            AnalyseResult analyseResult = new AnalyseResult();
            List<string> specList = new List<string>(); //物料描述分隔符处理后
            List<string> specListBuf = new List<string>(); //物料描述分隔符处理后
            List<string> unitResList = ResistanceUnit.Split(',').ToList(); //电阻单位
            List<string> unitCapList = CapacitanceUnit.Split(',').ToList();//电容单位

            string LCR_Type = "";

            bool hadIdentifyingDigits = false;

            #region 产商规则
            if (!analyseResult.Result)
            {
                analyseResult = new AnalyseResult();
                bool chk = false;
                for (int i = 0; i < CommonAnalyse.Instance.ManufactureRuleModels.Count; i++)
                {
                    analyseResult = new AnalyseResult();
                    if (CommonAnalyse.Instance.ManufactureRuleModels[i].Enable)
                    {
                        int index = 0;
                        #region 类型
                        chk = GetTypeManufactureRule(CommonAnalyse.Instance.ManufactureRuleModels[i].ResCode, description, out index);
                        if (chk)
                        {
                            analyseResult.Type = "电阻";
                        }
                        else
                        {
                            chk = GetTypeManufactureRule(CommonAnalyse.Instance.ManufactureRuleModels[i].CapCode, description, out index);
                            if (chk)
                            {
                                analyseResult.Type = "电容";
                            }
                        }
                        #endregion

                        #region 尺寸
                        if (chk)
                        {
                            chk = false;
                            string size = String.Empty;

                            int sizeStartId = 0;

                            try
                            {
                                if (!CommonAnalyse.Instance.ManufactureRuleModels[i].IsUseStandardSize)
                                {
                                    if (CommonAnalyse.Instance.ManufactureRuleModels[i].IsUseSizeId)
                                    {
                                        int.TryParse(CommonAnalyse.Instance.ManufactureRuleModels[i].SizeStartId, out sizeStartId);
                                    }

                                    if (CommonAnalyse.Instance.ManufactureRuleModels[i].IsUseSizeIdAfterType)
                                    {
                                        sizeStartId = index;
                                    }

                                    chk = GetSizeManufactureRule(CommonAnalyse.Instance.ManufactureRuleModels[i], description.Substring(sizeStartId, CommonAnalyse.Instance.ManufactureRuleModels[i].SizeCodeLength), out size);
                                    if (chk)
                                    {
                                        analyseResult.Size = size;
                                    }
                                }
                                else
                                {
                                    if (CommonAnalyse.Instance.ManufactureRuleModels[i].IsUseSizeIdAfterType)
                                    {
                                        sizeStartId = index;
                                    }

                                    chk = GetSizeManufactureRule(description, sizeStartId, out size);
                                    if (chk)
                                    {
                                        analyseResult.Size = size;
                                    }
                                }
                            }
                            catch
                            {

                            }
                        }
                        #endregion

                        #region 标准值、等级
                        if (chk)
                        {
                            chk = false;
                            string valuestr = String.Empty;
                            int startid = 0;
                            if (CommonAnalyse.Instance.ManufactureRuleModels[i].IsUseValueStartId)
                            {
                                startid = CommonAnalyse.Instance.ManufactureRuleModels[i].ValueStartId;
                                if (description.IndexOf("RAT") == 0 && analyseResult.Size == "01005")
                                {
                                    startid += 1;
                                }
                            }

                            if (CommonAnalyse.Instance.ManufactureRuleModels[i].IsUseValueStartIdAfterChar)
                            {
                                startid = description.IndexOf(CommonAnalyse.Instance.ManufactureRuleModels[i].CharBeforeValue) + 1;
                            }

                            bool isNumStart = false;
                            for (int m = startid; m < description.Length; m++)
                            {
                                if (char.IsDigit(description[m]) 
                                    || (description[m] == 'R' && m == CommonAnalyse.Instance.ManufactureRuleModels[i].ValueStartId && CommonAnalyse.Instance.ManufactureRuleModels[i].Manufacture == "村田")
                                    || (description[m] == 'R'))
                                {
                                    if(description[m] == 'R')
                                    {
                                        if (String.IsNullOrEmpty(valuestr))
                                        {
                                            valuestr += "0.";
                                        }
                                        else
                                        {
                                            valuestr += ".";
                                        }
                                        
                                    }
                                    else
                                    {
                                        valuestr += description[m];
                                    }
                                    isNumStart = true;
                                }
                                else
                                {
                                    if (isNumStart)
                                    {
                                        if (CommonAnalyse.Instance.ManufactureRuleModels[i].IsUseRKM && description[m] == 'K')
                                        {
                                            valuestr += ".";
                                            analyseResult.Unit = "KΩ";
                                        }
                                        else if (CommonAnalyse.Instance.ManufactureRuleModels[i].IsUseRKM && description[m] == 'M')
                                        {
                                            valuestr += ".";
                                            analyseResult.Unit = "MΩ";
                                        }
                                        else
                                        {
                                            index = m;
                                            break; // 遇到非数字字符时停止
                                        }
                                    }
                                }
                            }

                            if (analyseResult.Unit == "KΩ" || analyseResult.Unit == "MΩ")
                            {
                                double val = 0;
                                double.TryParse(valuestr, out val);
                                analyseResult.Value = val.ToString();
                            }

                            if (valuestr == "0.")
                            {
                                analyseResult.Value = String.Empty;
                            }

                            if (valuestr.IndexOf(".") == (valuestr.Length - 1))
                            {
                                valuestr = valuestr.Replace(".", "");
                                double val = 0;
                                double.TryParse(valuestr, out val);
                                analyseResult.Value = val.ToString();
                            }

                            if (description.IndexOf("RK73Z") == 0)
                            {
                                analyseResult.Unit = "Ω";
                                analyseResult.Value = "0";
                                analyseResult.Grade = description.Substring(6, 1);
                                valuestr = "0";
                            }

                            if (valuestr == "0" || (valuestr == "0000" && analyseResult.Type == "电阻"))
                            {
                                analyseResult.Unit = "Ω";
                                analyseResult.Value = "0";
                                valuestr = "0";
                            }

                            //数码法分析
                            if (valuestr.Contains(".") && String.IsNullOrEmpty(analyseResult.Value))
                            {
                                analyseResult.Value = double.Parse(valuestr).ToString();
                                if (analyseResult.Type == "电阻")
                                {
                                    analyseResult.Unit = "Ω";
                                }
                                if (analyseResult.Type == "电容")
                                {
                                    analyseResult.Unit = "PF";
                                }
                            }
                            else if (!String.IsNullOrEmpty(valuestr))
                            {

                                if (valuestr.ToString().Contains(".") && analyseResult.Type == "电阻" && String.IsNullOrEmpty(analyseResult.Unit))
                                {
                                    analyseResult.Unit = "Ω";
                                    analyseResult.Value = valuestr;
                                }
                                else 
                                { 
                                    string textValue = valuestr;
                                    string value = String.Empty;
                                    if (valuestr.Length >= 2)
                                    {
                                        value = textValue.Substring(textValue.Length - 1);
                                    }
                                    double val = 0;
                                    if (textValue[0] == 'R')
                                    {
                                        if (double.TryParse("0." + textValue.Replace("R", ""), out val))
                                        {
                                            analyseResult.Value = val.ToString();
                                            analyseResult.Unit = "PF";
                                        }
                                        else
                                        {
                                            analyseResult.Value = String.Empty;
                                        }
                                    }
                                    else if (CommonAnalyse.Instance.ManufactureRuleModels[i].IsIdentifyingDigits && valuestr.Length > 2 && int.TryParse(valuestr, out int res0) && int.TryParse(value, out int res) && string.IsNullOrEmpty(analyseResult.Unit))
                                    {
                                        if (res >= 0 && res <= 7)
                                        {
                                            hadIdentifyingDigits = true;
                                            int factor = (int)Math.Pow(10, Convert.ToInt32(res));
                                            val = Convert.ToDouble(textValue.Remove(textValue.Length - 1, 1)) * factor;
                                            analyseResult.Value = val.ToString();
                                            if (analyseResult.Type == "电阻")
                                            {
                                                analyseResult.Unit = "Ω";
                                            }
                                            if (analyseResult.Type == "电容")
                                            {
                                                analyseResult.Unit = "PF";
                                            }

                                            if (analyseResult.Unit == "PF")
                                            {
                                                if (val >= 1000000)
                                                {
                                                    analyseResult.Value = (val / 1000000).ToString();
                                                    analyseResult.Unit = "UF";
                                                }
                                                else if (val >= 1000)
                                                {
                                                    analyseResult.Value = (val / 1000).ToString();
                                                    analyseResult.Unit = "NF";
                                                }
                                            }
                                            if (analyseResult.Unit == "Ω")
                                            {
                                                if (val >= 1000000)
                                                {
                                                    analyseResult.Value = (val / 1000000).ToString();
                                                    analyseResult.Unit = "MΩ";
                                                }
                                                else if (val >= 1000)
                                                {
                                                    analyseResult.Value = (val / 1000).ToString();
                                                    analyseResult.Unit = "KΩ";
                                                }
                                            }
                                        }
                                    }
                                    else if(valuestr.Length == 1)
                                    {
                                        if (analyseResult.Type == "电阻")
                                        {
                                            analyseResult.Unit = "Ω";
                                        }
                                        if (analyseResult.Type == "电容")
                                        {
                                            analyseResult.Unit = "PF";
                                        }
                                        analyseResult.Value = valuestr;
                                    }
                                }
                            }


                            try
                            {
                                string grade = String.Empty;
                                if (CommonAnalyse.Instance.ManufactureRuleModels[i].IsUseGradeStartId)
                                {
                                    int gradeId = 0;
                                    int.TryParse(CommonAnalyse.Instance.ManufactureRuleModels[i].GradeStartId, out gradeId);
                                    //if (description.IndexOf("TSR") == 0 && (analyseResult.Size == "0402" || analyseResult.Size == "0201"))
                                    //{
                                    //    gradeId += 1;
                                    //}
                                    grade = description.Substring(gradeId, 1);
                                }
                                else
                                {
                                    grade = description.Substring(index, 1);
                                }
                                chk = GetGradeManufactureRule(CommonAnalyse.Instance.ManufactureRuleModels[i], analyseResult.Type, grade);
                                if (chk)
                                {
                                    analyseResult.Grade = grade;
                                }
                            }
                            catch
                            {

                            }
                        }
                        #endregion
                    }

                    analyseResult.Check();

                    if (analyseResult.Result)
                    {
                        return analyseResult;
                    }
                }
            }

            if (!CommonAnalyse.Instance.IsUseNormalRule)
            {
                return analyseResult;
            }


            #endregion

            #region 删除首位
            if (IsDeleteString)
            {
                description = RemoveLeft(description, PrefixNumber);
                description = RemoveRight(description, SuffixNumber);
            }
            #endregion

            #region 替换
            if (IsSubstitutionRules)
            {
                SubstitutionRule(ref description);
            }
            #endregion

            #region 分割符
            if (IsSeparator)
            {
                specList = Separator(description);
            }

            if (specList.Count <= 1)
            {
                analyseResult.Type = "分割字符串失败".tr();
                return analyseResult;
            }
            #endregion

            #region 去除长数字字符
            for (int i = 0; i < specList.Count; i++) { 
                if (IsNumeric(specList[i]) && (specList[i].Length > 7))
                {
                    specList.RemoveAt(i);
                }
            }
            #endregion

            #region 包含特点字符时，排除
            if (IsExcludeContext)
            {
                if (!string.IsNullOrEmpty(ExcludeContext))
                {
                    string[] excludeText = ExcludeContext.Split(',');
                    List<string> listExclude = Arrays.AsList(excludeText);
                    List<string> excludes = listExclude.Where(x => description.Contains(x)).ToList();
                    if (excludes.Count > 0)
                    {
                        analyseResult.Type = "Other";
                        return analyseResult;
                    }
                }
            }
            #endregion

            #region 获取类型
            GetType(description, ref analyseResult, ref LCR_Type);
            if (analyseResult.Type == "Other") return analyseResult;
            //去掉物料类型
            var typeList = specList.Where(u => u.Contains(LCR_Type)).ToList();
            foreach (var item in typeList)
            {
                //specList.Remove(item);
            }
            #endregion

            #region 获取尺寸
            GetSize(description, ref analyseResult);
            if (analyseResult.Size == "Error") return analyseResult;
            //去掉尺寸
            var sizeIndexList = specList.Where(u => u.Contains(analyseResult.Size)).ToList();
            foreach (var item in sizeIndexList)
            {
                //判断是否有尺寸等级（例：J(0603)）
                if (string.IsNullOrEmpty(analyseResult.Grade) && !description.Contains("%"))
                {
                    var gradeResult = item.Where("CDFGJKMN".Contains).ToList();
                    if (gradeResult?.Count > 0)
                    {
                        char Grade = gradeResult[0];
                        if (GradeChanges.Where(x => x.Grade == Grade.ToString()).ToList().Count > 0)
                        {
                            analyseResult.Grade = GradeChanges.Where(x => x.Grade == Grade.ToString()).ToList()[0].Percent;
                        }
                    }
                }
                if (CommonAnalyse.Instance.IsMergeDescription)
                {
                    description.Replace(analyseResult.Size, "");
                }
                else
                {
                    specList.Remove(item);
                }
            }

            if (CommonAnalyse.Instance.IsMergeDescription)
            {
                description.Replace(analyseResult.Size, "");
            }

            #endregion

            specListBuf = specList;

            if (CommonAnalyse.Instance.IsSearchGradeByPos)
            {
                if(!String.IsNullOrEmpty(CommonAnalyse.Instance.txtStrAfterGrade) && !String.IsNullOrEmpty(CommonAnalyse.Instance.txtGradePos))
                {
                    int pos = 0;
                    int index = 0;
                    if (int.TryParse(CommonAnalyse.Instance.txtGradePos, out pos)) {
                        string[] strsAfterGrade = CommonAnalyse.Instance.txtStrAfterGrade.Split(',');
                        int lenstrsAfterGrade = strsAfterGrade.Length;
                        if (lenstrsAfterGrade > 0)
                        {
                            for (int i = 0; i < lenstrsAfterGrade; i++)
                            {
                                index = description.IndexOf(strsAfterGrade[i]);
                                if (index > 0)
                                {
                                    try
                                    {
                                        analyseResult.Grade = description.Substring(index + pos, 1);
                                        break;
                                    }
                                    catch
                                    {

                                    }
                                }
                            }
                        }
                    }
                }
            }

            #region 获取值，单位，等级
            for (int i = 0; i < specList.Count; i++)
            {
                #region 获取单位
                if (string.IsNullOrEmpty(analyseResult.Unit))
                {
                    List<string> units = new List<string>();
                    if (analyseResult.Type == "电阻")
                    {
                        units = unitResList.Where(x => specList[i].Contains(x) && !Regex.IsMatch(specList[i], @"^[A-Za-z]+$")).ToList();
                    }
                    else if (analyseResult.Type == "电容")
                    {
                        units = unitCapList.Where(x => specList[i].Contains(x) && !Regex.IsMatch(specList[i], @"^[A-Za-z]+$")).ToList();
                    }
                    if (units.Count > 0)
                    {
                        analyseResult.Unit = units[0];
                        if (analyseResult.Type == "电阻")
                        {
                            if (analyseResult.Unit == "R")
                            {
                                analyseResult.Unit = "Ω";
                            }
                            if (!analyseResult.Unit.Contains("Ω"))
                            {
                                analyseResult.Unit += "Ω";
                            }
                        }
                        else if (analyseResult.Type == "电容")
                        {
                            if (!analyseResult.Unit.Contains("F") && !analyseResult.Unit.Contains("f"))
                            {
                                analyseResult.Unit += "F";
                            }
                        }
                        if (string.IsNullOrEmpty(analyseResult.Value))
                        {
                            //提取值
                            string[] valueArray = specList[i].Split(analyseResult.Unit.ToCharArray()[0]);
                            if (valueArray.Length == 2 && string.IsNullOrEmpty(valueArray[1]))
                            {
                                analyseResult.Value = valueArray[0];
                            }
                            else if (valueArray.Length == 2 && !string.IsNullOrEmpty(valueArray[1]))
                            {
                                if (IsInt(valueArray[1]))
                                {
                                    analyseResult.Value = specList[i].Replace(analyseResult.Unit.ToCharArray()[0], '.');
                                }
                                else
                                {
                                    analyseResult.Value = valueArray[0];
                                }
                            }
                            if (!IsNumeric(analyseResult.Value))
                            {
                                analyseResult.Unit = String.Empty;
                                analyseResult.Value = String.Empty;
                            }
                            if (!string.IsNullOrEmpty(analyseResult.Value)) { 
                                string pow = analyseResult.Value.Substring(analyseResult.Value.Length - 1);
                                if (IsIdentifyingDigits && analyseResult.Value.Length > 2 && int.TryParse(analyseResult.Value, out int res0) && int.TryParse(pow, out int res))
                                {
                                    if (res >= 0 && res <= 6 && (analyseResult.Unit == "Ω" || analyseResult.Unit == "PF"))
                                    {
                                        int factor = (int)Math.Pow(10, Convert.ToInt32(res));
                                        analyseResult.Value = (Convert.ToDouble(analyseResult.Value.Remove(analyseResult.Value.Length - 1, 1)) * factor).ToString();
                                        hadIdentifyingDigits = true;
                                    }
                                }
                            }   
                            double dValue = 0.0;
                            if(double.TryParse(analyseResult.Value, out dValue))
                            {
                                if(analyseResult.Unit == "PF")
                                {
                                    if(dValue >= 1000000)
                                    {
                                        analyseResult.Value = (dValue / 1000000).ToString();
                                        analyseResult.Unit = "UF";
                                    }
                                    else if (dValue >= 1000)
                                    {
                                        analyseResult.Value = (dValue / 1000).ToString();
                                        analyseResult.Unit = "NF";
                                    }
                                }
                                if (analyseResult.Unit == "Ω")
                                {
                                    if (dValue >= 1000000)
                                    {
                                        analyseResult.Value = (dValue / 1000000).ToString();
                                        analyseResult.Unit = "MΩ";
                                    }
                                    else if (dValue >= 1000)
                                    {
                                        analyseResult.Value = (dValue / 1000).ToString();
                                        analyseResult.Unit = "KΩ";
                                    }
                                }
                            }
                        }
                        continue;
                    }

                }
                #endregion

                #region 获取等级
                string textValue = specList[i];
                if (string.IsNullOrEmpty(analyseResult.Grade))
                {
                    //值含偏差等级
                    if (textValue.Contains("%"))
                    {
                        if (textValue.StartsWith("%"))
                        {
                            if (textValue.EndsWith("%"))
                            {
                                analyseResult.Grade = Regex.Replace(textValue, @"[^\d.\d]", "") + "%";
                            }
                            else
                            {
                                string tempGrade = Regex.Replace(textValue, @"[^0-9.]", "");
                                if (IsNumeric(tempGrade))
                                {
                                    analyseResult.Grade = tempGrade + "%";
                                }
                                else
                                {
                                    analyseResult.Grade = tempGrade;
                                }
                            }
                        }
                        else
                        {
                            analyseResult.Grade = TheGrade(textValue);
                        }
                    }
                    else if (IsValueContainsGrade && IsValueGrade(textValue)&&string.IsNullOrEmpty(analyseResult.Value))
                    {
                        Regex r = new Regex(@"[a-zA-Z]+");
                        System.Text.RegularExpressions.Match m = r.Match(textValue);
                        string grade = m.Value;
                        if (!string.IsNullOrEmpty(grade) && "CDFGJKMN".Contains(grade) && !textValue.Contains("Ω"))
                        {
                            analyseResult.Grade = grade;
                            textValue = textValue.Replace(grade, "");
                            if (textValue.Length == 3 && IsNumeric(textValue))
                            {
                                hadIdentifyingDigits = true;
                                int v1 = int.Parse(textValue.Substring(0, 2));
                                int v2 = int.Parse(textValue.Substring(2, 1));
                                double val = v1 * Math.Pow(10, v2);
                                textValue = val.ToString();
                            }
                            analyseResult.Value = textValue;
                            continue;
                        }
                    }
                    else if (!string.IsNullOrEmpty(analyseResult.Value) && textValue.StartsWith("±"))
                    {
                        textValue = textValue.Replace("±", "");
                        Regex r = new Regex(@"[a-zA-Z]+");
                        System.Text.RegularExpressions.Match m = r.Match(textValue);
                        string grade = m.Value;
                        if (!string.IsNullOrEmpty(grade))
                        {
                            textValue = textValue.Replace(grade, "");
                        }

                        analyseResult.Grade = (double.Parse(textValue) / double.Parse(analyseResult.Value) * 100).ToString("0.000") + "%";
                    }
                }
                #endregion

            }

            #region 获取值
            if (string.IsNullOrEmpty(analyseResult.Value) || !IsNumeric(analyseResult.Value))
            {
                for (int i = 0; i < specListBuf.Count; i++)
                {
                    string textValue = specListBuf[i];
                    string value = textValue.Substring(textValue.Length - 1);
                    if (IsIdentifyingDigits && textValue.Length > 2 && int.TryParse(textValue, out int res0) && int.TryParse(value, out int res) && string.IsNullOrEmpty(analyseResult.Unit))
                    {
                        if (res >= 0 && res <= 6)
                        {
                            hadIdentifyingDigits = true;
                            int factor = (int)Math.Pow(10, Convert.ToInt32(res));
                            analyseResult.Value = (Convert.ToDouble(textValue.Remove(textValue.Length - 1, 1)) * factor).ToString();
                        }
                    }

                    if (string.IsNullOrEmpty(analyseResult.Value))
                    {
                        textValue = specList[i];
                        if (IsNumeric(textValue))
                        {
                            analyseResult.Value = textValue;
                        }
                        else
                        {
                            //电阻分析加入特殊处理 如0R5 5R00...
                            //string pattern = @"(?<=\d)\D+(?=\d)";
                            //Regex regex = new Regex(pattern);
                            //string result = regex.Replace(textValue, ".");
                            //if (IsIntermediateUnit)
                            //    analyseResult.Value = Regex.Replace(result, @"[^0-9.]", "");

                            int nonDigitCount = Regex.Matches(textValue, @"[a-zA-Z]").Count;      //判断字母个数
                            if(nonDigitCount == 1){
                                string letter = Regex.Replace(textValue, @"[^a-zA-Z]", "");     //提取字母
                                if (letter == "R" || letter == "P" || letter == "M" || letter == "K")
                                {
                                    if (textValue.IndexOf(letter) != (textValue.Length - 1))
                                    {
                                        analyseResult.Value = textValue.Replace(letter, ".");
                                    }
                                    else
                                    {
                                        analyseResult.Value = textValue.Replace(letter, "");
                                    }
                                    if (!IsNumeric(analyseResult.Value))
                                    {
                                        analyseResult.Value = String.Empty;
                                    }
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(analyseResult.Value))
                    {
                        break;
                    }
                }
            }
            #endregion

            #region 删除单位
            if (!string.IsNullOrEmpty(analyseResult.Unit))
            {
                var unitList = specList.Where(u => u.Contains(analyseResult.Unit)).ToList();
                foreach (var item in unitList)
                {
                    specList.Remove(item);
                }
            }
            #endregion

            #endregion

            #region 最后未找到的项处理
            //if (IsResDefaultUnit&&string.IsNullOrEmpty(analyseResult.Unit)&& analyseResult.Type == "电阻".tr())
            //{
            //    //如果电阻没找到，默认单位为Ω
            //    analyseResult.Unit = ResDefaultUnit;
            //}

            if (string.IsNullOrEmpty(analyseResult.Grade))
            {
                var gradeResult = specList.Where("CDFGJKMN".Contains).ToList();
                if (gradeResult?.Count > 0)
                {
                    string Grade = gradeResult[0];
                    analyseResult.Grade = Grade;   
                }
                    
                else if (IsGrade_ON_NO_Find)
                {
                    //自定义等级偏差
                    if (analyseResult.Type == "电阻".tr())
                    {
                        analyseResult.Grade = ResGrade_ON_NO_Find;
                    }
                    else if (analyseResult.Type == "电容".tr())
                    {
                        analyseResult.Grade = CapGrade_ON_NO_Find;
                    }
                }
            }

            if (analyseResult.Type == "电阻".tr() && CommonAnalyse.Instance.IsUseCustomerResGrade)
            {
                if (GradeChangesCustRes.Where(x => x.Grade == analyseResult.Grade).ToList().Count > 0)
                {
                    if (GradeChangesCustRes.Where(x => x.Grade == analyseResult.Grade).ToList()[0].Percent != "(null)" 
                        && GradeChangesCustRes.Where(x => x.Grade == analyseResult.Grade).ToList()[0].PercentLow == "(null)")
                    {
                        analyseResult.Grade = GradeChangesCustRes.Where(x => x.Grade == analyseResult.Grade).ToList()[0].Percent;
                    }
                }
            }
            else if (analyseResult.Type == "电容".tr() && CommonAnalyse.Instance.IsUseCustomerCapGrade)
            {
                if (GradeChangesCustCap.Where(x => x.Grade == analyseResult.Grade).ToList().Count > 0)
                {
                    if(GradeChangesCustCap.Where(x => x.Grade == analyseResult.Grade).ToList()[0].Percent != "(null)"
                        && GradeChangesCustCap.Where(x => x.Grade == analyseResult.Grade).ToList()[0].PercentLow == "(null)")
                    {
                        analyseResult.Grade = GradeChangesCustCap.Where(x => x.Grade == analyseResult.Grade).ToList()[0].Percent;
                    }
                }
            }
            else if (GradeChanges.Where(x => x.Grade == analyseResult.Grade).ToList().Count > 0)
            {
                analyseResult.Grade = GradeChanges.Where(x => x.Grade == analyseResult.Grade).ToList()[0].Percent;
            }
            else if (GradeChanges.Where(x => x.Percent == analyseResult.Grade).ToList().Count > 0)
            {
                analyseResult.Grade = GradeChanges.Where(x => x.Percent == analyseResult.Grade).ToList()[0].Percent;
            }
            else if (analyseResult.Grade.Contains("%"))
            {
                
            }
            else
            {
                analyseResult.Grade = String.Empty;
            }
            #endregion

            #region 已知类型没有单位时，设定默认单位
            bool IsDefaultUnit = false;
            if (string.IsNullOrEmpty(analyseResult.Unit))
            {
                if (analyseResult.Type == "电阻".tr())
                {
                    IsDefaultUnit = true;
                    analyseResult.Unit = "Ω";
                }
                else if (analyseResult.Type == "电容".tr())
                {
                    IsDefaultUnit = true;
                    analyseResult.Unit = "PF";
                }
            }
            #endregion

            #region 已知数值，对数值再次分析
            if (!string.IsNullOrEmpty(analyseResult.Value))
            {
                double dVal = 0.0;
                string textValue = analyseResult.Value;
                string value = textValue.Substring(textValue.Length - 1);
                if (IsIdentifyingDigits && textValue.Length > 2 && int.TryParse(textValue, out int res0) && int.TryParse(value, out int res) && !hadIdentifyingDigits)
                {
                    if (res >= 0 && res <= 6 && IsDefaultUnit)
                    {
                        int factor = (int)Math.Pow(10, Convert.ToInt32(res));
                        analyseResult.Value = (Convert.ToDouble(textValue.Remove(textValue.Length - 1, 1)) * factor).ToString();
                    }
                }
                if (double.TryParse(analyseResult.Value, out dVal))
                {
                    if (analyseResult.Unit == "PF")
                    {
                        if (dVal >= 1000000)
                        {
                            analyseResult.Value = (dVal / 1000000).ToString();
                            analyseResult.Unit = "UF";
                        }
                        else if (dVal >= 1000)
                        {
                            analyseResult.Value = (dVal / 1000).ToString();
                            analyseResult.Unit = "NF";
                        }
                    }
                    if (analyseResult.Unit == "Ω")
                    {
                        if (dVal >= 1000000)
                        {
                            analyseResult.Value = (dVal / 1000000).ToString();
                            analyseResult.Unit = "MΩ";
                        }
                        else if (dVal >= 1000)
                        {
                            analyseResult.Value = (dVal / 1000).ToString();
                            analyseResult.Unit = "KΩ";
                        }
                    }
                }
            }
            #endregion

            analyseResult.Check();

            return analyseResult;
        }

        private bool GetTypeManufactureRule(string rule, string description, out int id)
        {
            id = 0;

            if (String.IsNullOrEmpty(rule))
            {
                return false;
            }
            var result = rule.Split(',').Where(x => description.IndexOf(x) == 0).ToList();
            if (result.Count > 0)
            {
                id = result[0].Length;
                return true;
            }

            return false;
        }

        private bool GetSizeManufactureRule(Model.ManufactureRuleModel manufactureRuleModel, string Sizedescription, out string size)
        {
            size = String.Empty;

            if (manufactureRuleModel.substitutionRules.Count <= 0)
            {
                return false;
            }
            
            for(int i = 0; i < manufactureRuleModel.substitutionRules.Count; i++)
            {
                if (manufactureRuleModel.substitutionRules[i].Enable)
                {
                    if(Sizedescription.IndexOf(manufactureRuleModel.substitutionRules[i].FindContent) >= 0)
                    {
                        size = manufactureRuleModel.substitutionRules[i].Replace;
                        return true;
                    }
                }
            }

            return false;
        }

        private bool GetSizeManufactureRule(string description , int startIndex, out string size)
        {
            size = String.Empty;

            if(description.Length > (5 + startIndex))
            {
                if(description.Substring(startIndex, 5) == "01005")
                {
                    size = "01005";
                    return true;
                }
            }

            if (description.Length > (4 + startIndex))
            {
                if (description.Substring(startIndex, 4) == "0201")
                {
                    size = "0201";
                    return true;
                }
                if (description.Substring(startIndex, 4) == "0402")
                {
                    size = "0402";
                    return true;
                }
                if (description.Substring(startIndex, 4) == "0603")
                {
                    size = "0603";
                    return true;
                }
                if (description.Substring(startIndex, 4) == "0805")
                {
                    size = "0805";
                    return true;
                }
                if (description.Substring(startIndex, 4) == "1206")
                {
                    size = "1206";
                    return true;
                }
                if (description.Substring(startIndex, 4) == "1210")
                {
                    size = "1210";
                    return true;
                }
            }

            return false;
        }

        private bool GetGradeManufactureRule(Model.ManufactureRuleModel manufactureRuleModel, string type, string grade)
        {

            if (type == "电阻")
            {
                if (manufactureRuleModel.GradeRes.Count <= 0)
                {
                    return false;
                }

                for (int i = 0; i < manufactureRuleModel.GradeRes.Count; i++)
                {
                    if (manufactureRuleModel.GradeRes[i].Grade == grade)
                    {
                        return true;
                    }
                }
            }

            if (type == "电容")
            {
                if (manufactureRuleModel.GradeCap.Count <= 0)
                {
                    return false;
                }

                for (int i = 0; i < manufactureRuleModel.GradeCap.Count; i++)
                {
                    if (manufactureRuleModel.GradeCap[i].Grade == grade)
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        /// <summary>
        /// 获取类型
        /// </summary>
        /// <param name="description"></param>
        /// <param name="analyseResult"></param>
        /// <param name="LCR_Type"></param>
        private void GetType(string description, ref AnalyseResult analyseResult, ref string LCR_Type)
        {
            var resResult = Resistance.ToUpper().Split(',').Where(description.ToUpper().Contains).ToList();
            var capResult = Capacitance.ToUpper().Split(',').Where(description.ToUpper().Contains).ToList();

            if (resResult?.Count > 0)
            {
                analyseResult.Type = "电阻".tr();
                LCR_Type = resResult[0];
                return;
            }

            if (capResult?.Count > 0)
            {
                analyseResult.Type = "电容".tr();
                LCR_Type = capResult[0];
                return;
            }

            if (analyseResult.Type == "Other")
            {
                resResult = "Ω".ToUpper().Split(',').Where(description.ToUpper().Contains).ToList();
                if (resResult?.Count > 0)
                {
                    analyseResult.Type = "电阻".tr();
                    LCR_Type = resResult[0];
                    return;
                }
            }

            if (analyseResult.Type == "Other")
            {
                resResult = "PF,UF,NF".ToUpper().Split(',').Where(description.ToUpper().Contains).ToList();
                if (resResult?.Count > 0)
                {
                    analyseResult.Type = "电容".tr();
                    LCR_Type = resResult[0];
                    return;
                }
            }

            Regex regex = new Regex("^-?\\d+$|^(-?\\d+)(\\.\\d+)?$");                       //整型或浮点型
            Regex regex2= new Regex(@"[a-zA-Z]+");                                          //字母
            //Regex regex3= new Regex("[\u4e00-\u9fa5]");                                   //汉字

            if (analyseResult.Type == "Other")
            {
                resResult = "K,M,R".ToUpper().Split(',').Where(description.ToUpper().Contains).ToList();
                if (resResult?.Count > 0)
                {
                    for(int k = 0; k < resResult?.Count; k++)
                    {
                        int i = description.IndexOf(resResult[k]);

                        if (i < (description.Length - 1))
                        {
                            string back = description.Substring(i + 1, 1);

                            if (regex2.IsMatch(back) || (regex.IsMatch(back) && resResult[k] != "R"))
                            {
                                continue;
                            }
                        }

                        if (i == 1)
                        {
                            string st = description.Substring(i - 1, 1);
                            if (regex.IsMatch(st))
                            {
                                analyseResult.Type = "电阻".tr();
                                LCR_Type = resResult[0];
                                return;
                            }
                        }
                        else if(i > 1)
                        {
                            string st = description.Substring(i - 2, 2);
                            if (regex.IsMatch(st) || (regex.IsMatch(st.Substring(1,1)) && st.IndexOf(",") == 0))
                            {
                                analyseResult.Type = "电阻".tr();
                                LCR_Type = resResult[0];
                                return;
                            }
                            else
                            {
                                if (i > 2) st = description.Substring(i - 3, 3);
                                if (regex.IsMatch(st))
                                {
                                    analyseResult.Type = "电阻".tr();
                                    LCR_Type = resResult[0];
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            if (analyseResult.Type == "Other")
            {
                resResult = "F,N,U,P".ToUpper().Split(',').Where(description.ToUpper().Contains).ToList();
                if (resResult?.Count > 0)
                {
                    for (int k = 0; k < resResult?.Count; k++)
                    {
                        int i = description.IndexOf(resResult[k]);

                        if (i < (description.Length - 1)) { 
                            string back = description.Substring(i + 1, 1);

                            if (regex2.IsMatch(back) || regex.IsMatch(back))
                            {
                                if(back != "f" || back != "F")
                                continue;
                            }
                        }

                        if (i == 1 )
                        {
                            string st = description.Substring(i - 1, 1);
                            if (regex.IsMatch(st))
                            { 
                                analyseResult.Type = "电容".tr();
                                LCR_Type = resResult[0];
                                return;
                            }
                        }
                        else if (i > 1)
                        {
                            string st = description.Substring(i - 2, 2);
                            if (regex.IsMatch(st) || (regex.IsMatch(st.Substring(1, 1)) && st.IndexOf(",") == 0))
                            {
                                analyseResult.Type = "电容".tr();
                                LCR_Type = resResult[0];
                                return;
                            }
                            else
                            {
                                if(i > 2) st = description.Substring(i - 3, 3);
                                if (regex.IsMatch(st))
                                {
                                    analyseResult.Type = "电容".tr();
                                    LCR_Type = resResult[0];
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取尺寸
        /// </summary>
        private void GetSize(string description, ref AnalyseResult analyseResult)
        {
            var sizeResult = ComponentSpecifications.ToUpper().Split(',').Where(description.ToUpper().Contains).ToList();
            if (sizeResult?.Count > 0)
                analyseResult.Size = sizeResult[0];
        }

        /// <summary>
        /// 分割字符方法
        /// </summary>
        /// <param name="description"></param>
        private List<string> Separator(string description)
        {
            var splitCharList = Separators.Where(u => u.Enable == true);
            List<char> listSplitChar = new List<char>();
            foreach (var item in splitCharList)
            {
                //char singleSplitChar = MidStrEx(item.Acsii, "(", ")").ToCharArray()[0];
                if (!listSplitChar.Contains(item.Acsii.ToArray()[0]))
                    listSplitChar.Add(item.Acsii.ToArray()[0]);
            }
            List<string> specList = description.Split(listSplitChar.ToArray(), StringSplitOptions.RemoveEmptyEntries).Where(a => a.Length >= 1).Select(b => b).ToList();

            return specList;
        }

        private string MidStrEx(string sourse, string startstr, string endstr)
        {
            string result = string.Empty;
            int startindex, endindex;
            try
            {
                startindex = sourse.IndexOf(startstr);
                if (startindex == -1)
                    return result;
                string tmpstr = sourse.Substring(startindex + startstr.Length);
                endindex = tmpstr.IndexOf(endstr);
                if (endindex == -1)
                    return result;
                result = tmpstr.Remove(endindex);
            }
            catch (Exception)
            {
                return sourse;
            }
            return result;
        }

        /// <summary>
        /// 替换字符方法
        /// </summary>
        /// <returns></returns>
        private void SubstitutionRule(ref string description)
        {
            var replaceList = SubstitutionRules.Where(u => u.Enable == true);
            foreach (var item in replaceList.ToList())
            {
                if (description.Contains(item.FindContent))
                {
                    if (item.Is_Case_sensitive)//区分大小写
                    {
                        if (item.Is_Full_half_width)//区分全半角
                        {
                            description = description.Replace(item.FindContent, item.Replace);
                        }
                        else//不区分全半角
                        {
                            description = description.Replace(item.FindContent.ToSBC(), item.Replace);
                            description = description.Replace(item.FindContent.ToDBC(), item.Replace);
                        }
                    }
                    else//不区分大小写
                    {
                        if (item.Is_Full_half_width)//区分全半角
                        {
                            description = Regex.Replace(description, Regex.Escape(item.FindContent), item.Replace, RegexOptions.IgnoreCase);
                        }
                        else //不区分全半角
                        {
                            description = Regex.Replace(description, Regex.Escape(item.FindContent.ToSBC()), item.Replace, RegexOptions.IgnoreCase);
                            description = Regex.Replace(description, Regex.Escape(item.FindContent.ToDBC()), item.Replace, RegexOptions.IgnoreCase);
                        }
                    }
                }
            }
        }

        //从字符串前面删除指定字符个数
        private string RemoveLeft(string s, int len)
        {
            return s.PadLeft(len).Remove(0, len);
        }

        //从字符串后面删除指定字符个数
        private string RemoveRight(string s, int len)
        {
            s = s.PadRight(len);
            return s.Remove(s.Length - len, len);
        }

        private bool IsNumeric(string s)
        {
            if (double.TryParse(s, out double v))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private string TheGrade(string str)
        {
            string r = @"[0-9.]+%";
            Regex reg = new Regex(r, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            MatchCollection mc = reg.Matches(str);
            foreach (System.Text.RegularExpressions.Match m in mc)
            {
                return m.Groups[0].Value;
            }
            return str;
        }

        //数字开头字母结尾
        private bool IsValueGrade(string mobile)
        {
            return Regex.IsMatch(mobile, @"^\d[\d\w]+\w$");
        }

        private bool IsInt(string value)
        {
            return Regex.IsMatch(value, @"^[0-9]*[1-9][0-9]*$");
        }

        private string TheLongestName(string[] array)
        {
            string longest = array[0];
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Length > longest.Length)
                {
                    longest = array[i];
                }
            }
            return longest;
        }


        public bool Check(string analyType, string analySize, string analyValue, string analyUnit, string analyGrade)
        {
            if (analyType != "Other" && analySize != "Error" && !string.IsNullOrEmpty(analyValue) && !string.IsNullOrEmpty(analyUnit) && !string.IsNullOrEmpty(analyGrade))
            {
                bool chk = true;
                double value = 0;

                if (!double.TryParse(analyValue, out value))
                {
                    chk = false;
                }

                UnitType unit = UnitType.None;
                if (!Enum.TryParse<UnitType>(analyUnit.ToUpper(), true, out unit))
                {
                    chk = false;
                }

                SizeType size = SizeType.None;
                if (!Enum.TryParse<SizeType>("Size" + analySize, true, out size))
                {
                    chk = false;
                }

                if (chk)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        public void AddManufacture(Model.ManufactureRuleModel manufactureRuleModel)
        {
            ManufactureRuleModels.Add(manufactureRuleModel);
            Save();
        }
      
    }

    public static class TrHelp
    {
        /// <summary> 转半角的函数(DBC case) </summary>
        /// <param name="input">任意字符串</param>
        /// <returns>半角字符串</returns>
        ///<remarks>
        ///全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        ///</remarks>
        public static string ToDBC(this string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }

        /// <summary>
        /// 转全角的函数(SBC case)
        /// </summary>
        /// <param name="input">任意字符串</param>
        /// <returns>全角字符串</returns>
        ///<remarks>
        ///全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        ///</remarks>
        public static string ToSBC(this string input)
        {
            //半角转全角：
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                    c[i] = (char)(c[i] + 65248);
            }
            return new string(c);
        }

        public static int I(this string str)
        {
            if (int.TryParse(str, out int res))
            {
                return res;
            }
            return 0;
        }

        public static bool B(this string str)
        {
            if (bool.TryParse(str, out bool res))
            {
                return res;
            }
            return false;
        }

        public static double D(this string str)
        {
            if (double.TryParse(str, out double res))
            {
                return res;
            }
            return 0;
        }
    }

    [Serializable]
    public class GradeChange
    {
        /// <summary>
        /// 字符等级
        /// </summary>
        public string Grade;
        /// <summary>
        /// 百分比
        /// </summary>
        public string Percent;
        /// <summary>
        /// 百分比
        /// </summary>
        public string PercentLow;
        /// <summary>
        /// 加
        /// </summary>
        public string DiffUpper;
        /// 减
        /// </summary>
        public string DiffLower;
    }

    /// <summary>
    /// 分隔符定义
    /// </summary>
    [Serializable]
    public class Separator
    {
        /// <summary>
        /// 是否可用
        /// </summary>
        public bool Enable = false;

        /// <summary>
        /// Asc码
        /// </summary>
        public string Acsii = "";

        /// <summary>
        /// 说明
        /// </summary>
        public string Illustrate = "";
    }

    /// <summary>
    /// 替换规则
    /// </summary>
    [Serializable]
    public class SubstitutionRules
    {
        /// <summary>
        /// 是否可用
        /// </summary>
        public bool Enable = false;

        /// <summary>
        /// 查找内容
        /// </summary>
        public string FindContent = "";

        /// <summary>
        /// 替换
        /// </summary>
        public string Replace = "";

        /// <summary>
        /// 是否区分大小写
        /// </summary>
        public bool Is_Case_sensitive = false;

        /// <summary>
        /// 是否区分全半角
        /// </summary>
        public bool Is_Full_half_width = false;

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark = "";
    }

    public sealed class AnalyseResult
    {
        /// <summary>
        /// 条码
        /// </summary>
        public string BarCode = "";
        /// <summary>
        /// 物料描述
        /// </summary>
        public string Description;
        public bool Result = false;

        /// <summary>
        /// 宽度
        /// </summary>
        public string Width { get; set; } = string.Empty;

        /// <summary>
        /// 间距
        /// </summary>
        public string Space { get; set; } = string.Empty;

        public string Type { get; set; } = "Other";
        public string Size { get; set; } = "Error";
        public string Value { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string ReplaceCode { get; set; } = string.Empty;

        /// <summary>
        /// 元件位置
        /// </summary>
        public string Position = "";

        /// <summary>
        /// 用量
        /// </summary>
        public int Quarity = 0;

        public decimal MinValue = 0;
        public decimal MaxValue = 0;

        /// <summary>
        /// 相同点位的料，互为替代料的关键信息
        /// </summary>
        public string ReplaceCodeKey = String.Empty;

        public string DefaultFormat()
        {
            return $"{Type}-{Size}-{Value}-{Unit}-{Grade}";
        }

        public void Clear()
        {
            Type = "Other";
            Size = "Error";
            Value = string.Empty;
            Unit = string.Empty;
            Grade = string.Empty;
        }

        public override string ToString()
        {
            return "类型".tr() + ":" + Type + "规格".tr() + ":" + Size + " 标准值".tr() + ":" + Value + "单位".tr() + ":" + Unit + " 等级".tr() + ":" + Grade;
        }

        public void Check()
        {
            if (Type != "Other" && Size != "Error" && !string.IsNullOrEmpty(Value) && !string.IsNullOrEmpty(Unit) && !string.IsNullOrEmpty(Grade))
            {
                bool chk = true;
                double value = 0;

                if(!double.TryParse(Value, out value))
                {
                    chk = false;
                }

                UnitType unit = UnitType.None;
                if (!Enum.TryParse<UnitType>(Unit.ToUpper(), true, out unit))
                {
                    chk = false;
                }

                SizeType size = SizeType.None;
                if (!Enum.TryParse<SizeType>("Size" + Size, true, out size))
                {
                    chk = false;
                }

                if (chk)
                {
                    Result = true;
                }
                else
                {
                    Result = false;
                }
            }
        }
    }

    public enum SizeType
    {
        None = 0,
        Size01005,
        Size0201,
        Size0402,
        Size0603,
        Size0805,
        Size1206,
        Size1210
    }

    public enum UnitType
    {
        None = 0,
        mΩ,
        Ω,
        kΩ,
        MΩ,
        PF,
        NF,
        UF,
        F
    }

    public enum 产商规则
    {
        None,
        村田
    }

}
