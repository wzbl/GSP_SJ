using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BorwinAnalyse.BaseClass
{
    public class SpecialLcrOffset
    {
        public SpecialLcrOffset()
        {

        }

        public void Init()
        {
            Key = "-";
            Use = false;
            Offset_mΩ = "-";
            Offset_Ω = "-";
            Offset_kΩ = "-";
            Offset_MΩ = "-";
            Offset_pF = "-";
            Offset_nF = "-";
            Offset_uF = "-";
            Offset_mF = "-";
            Offset_F = "-";
        }

        public bool Compare(SpecialLcrOffset _specialLcrOffset)
        {
            bool ret = true;
            if (this.Offset_mΩ != _specialLcrOffset.Offset_mΩ) ret = false;
            if (this.Use != _specialLcrOffset.Use) ret = false;
            if (this.Offset_Ω != _specialLcrOffset.Offset_Ω) ret = false;
            if (this.Offset_kΩ != _specialLcrOffset.Offset_kΩ) ret = false;
            if (this.Offset_MΩ != _specialLcrOffset.Offset_MΩ) ret = false;
            if (this.Offset_pF != _specialLcrOffset.Offset_pF) ret = false;
            if (this.Offset_nF != _specialLcrOffset.Offset_nF) ret = false;
            if (this.Offset_uF != _specialLcrOffset.Offset_uF) ret = false;
            if (this.Offset_mF != _specialLcrOffset.Offset_mF) ret = false;
            if (this.Offset_F != _specialLcrOffset.Offset_F) ret = false;

            return ret;
        }

        public string Key;
        public bool Use;
        public string Offset_mΩ;
        public string Offset_Ω;
        public string Offset_kΩ;
        public string Offset_MΩ;
        public string Offset_nF;
        public string Offset_pF;
        public string Offset_uF;
        public string Offset_mF;
        public string Offset_F;
    }
}
