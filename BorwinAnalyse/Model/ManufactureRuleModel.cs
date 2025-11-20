using BorwinAnalyse.BaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BorwinAnalyse.Model
{
    public class ManufactureRuleModel
    {
        public string Manufacture = String.Empty;
        public bool Enable = false;
        public bool IsIdentifyingDigits = false;
        public bool IsUseValueStartId = false;
        public bool IsUseValueStartIdAfterChar = false;
        public bool IsUseSizeId = false;
        public bool IsUseSizeIdAfterType = false;
        public bool IsUseStandardSize = false;
        public bool IsUseGradeStartId = false;
        public bool IsUseRKM = false;
        public List<SubstitutionRules> substitutionRules = new List<SubstitutionRules>();
        public List<GradeChange> GradeRes = new List<GradeChange>();
        public List<GradeChange> GradeCap = new List<GradeChange>();

        public string ResCode = String.Empty;
        public string CapCode = String.Empty;

        public string CharBeforeValue = String.Empty;
        public string SizeStartId = String.Empty;
        public string GradeStartId = String.Empty;

        public int ValueStartId = 0;
        public int SizeCodeLength = 0;

        public ManufactureRuleModel(string name) {
            Manufacture = name;
            Enable = false;
            IsIdentifyingDigits = false;
            IsUseValueStartId = false;
            IsUseValueStartId = false;
            IsUseValueStartIdAfterChar = false;
            IsUseSizeIdAfterType = false;
            IsUseStandardSize = false;
            IsUseGradeStartId = false;
            IsUseRKM = false;
            ValueStartId = 0;
            substitutionRules = new List<SubstitutionRules>();
            GradeRes = new List<GradeChange>();
            GradeCap = new List<GradeChange>();

            ResCode = String.Empty;
            CapCode = String.Empty;

            CharBeforeValue = String.Empty;
            SizeStartId = String.Empty;
            GradeStartId = String.Empty;

            ValueStartId = 0;
            SizeCodeLength = 0;
        }
    }
}
