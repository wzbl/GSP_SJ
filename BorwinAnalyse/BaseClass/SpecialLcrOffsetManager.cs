using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BorwinAnalyse.BaseClass
{
    public class SpecialLcrOffsetManager
    {
        private static SpecialLcrOffsetManager instance;
        public static SpecialLcrOffsetManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SpecialLcrOffsetManager();
                }
                return instance;
            }
        }

        public List<SpecialLcrOffset> SpecialLcrOffsets = new List<SpecialLcrOffset>();

        public void SearchALLSpectialOffset()
        {
            string comms = "select * from SpecialOffset";
            DataTable dataTable = SqlLiteManager.Instance.BomDB.Search(comms, "SpecialOffset");
            SpecialLcrOffsets.Clear();
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                SpecialLcrOffset SpecialLcrOffset = new SpecialLcrOffset();
                SpecialLcrOffset.Key = dataTable.Rows[i].ItemArray[0].ToString();
                SpecialLcrOffset.Use = bool.Parse(dataTable.Rows[i].ItemArray[1].ToString());
                SpecialLcrOffset.Offset_mΩ = dataTable.Rows[i].ItemArray[2].ToString();
                SpecialLcrOffset.Offset_Ω = dataTable.Rows[i].ItemArray[3].ToString();
                SpecialLcrOffset.Offset_kΩ = dataTable.Rows[i].ItemArray[4].ToString();
                SpecialLcrOffset.Offset_MΩ = dataTable.Rows[i].ItemArray[5].ToString();
                SpecialLcrOffset.Offset_pF = dataTable.Rows[i].ItemArray[6].ToString();
                SpecialLcrOffset.Offset_nF = dataTable.Rows[i].ItemArray[7].ToString();
                SpecialLcrOffset.Offset_uF = dataTable.Rows[i].ItemArray[8].ToString();
                SpecialLcrOffset.Offset_mF = dataTable.Rows[i].ItemArray[9].ToString();
                SpecialLcrOffset.Offset_F = dataTable.Rows[i].ItemArray[10].ToString();
                SpecialLcrOffsets.Add(SpecialLcrOffset);
            }
        }

        public SpecialLcrOffset SearchSpecialOffset(string key)
        {
            string res = key;
            if (String.IsNullOrEmpty(res))
            {
                return null;
            }
            List<SpecialLcrOffset> SpeccialLcrOffset = SpecialLcrOffsets.Where(x => x.Key == res).ToList<SpecialLcrOffset>();
            if (SpeccialLcrOffset == null || SpeccialLcrOffset.Count == 0)
            {
                return null;
            }

            if (SpeccialLcrOffset != null && !SpeccialLcrOffset[0].Use)
            {
                return null;
            }

            return SpeccialLcrOffset[0];
        }

        public void Save(List<SpecialLcrOffset> list)
        {
            foreach (var specialOffset in list)
            {
                int id = SpecialLcrOffsets.FindIndex(x => x.Key == specialOffset.Key);
                if (id >= 0)
                {
                    if (!specialOffset.Compare(SpecialLcrOffsets[id]))
                    {
                        string comm = string.Format("update SpecialOffset set Use = '{1}',Offset_mΩ = '{2}',Offset_Ω = '{3}',Offset_kΩ = '{4}',Offset_MeΩ = '{5}'," +
                        "Offset_pF = '{6}',Offset_nF = '{7}',Offset_uF = '{8}',Offset_mF = '{9}'," +
                        "Offset_F = '{10}' where Key = '{0}'",
                        specialOffset.Key, specialOffset.Use, specialOffset.Offset_mΩ, specialOffset.Offset_Ω, specialOffset.Offset_kΩ, specialOffset.Offset_MΩ,
                        specialOffset.Offset_pF, specialOffset.Offset_nF, specialOffset.Offset_uF, specialOffset.Offset_mF, specialOffset.Offset_F);
                        SqlLiteManager.Instance.BomDB.Insert(comm);
                    }
                }
                else
                {
                    string comm = string.Format("insert into SpecialOffset values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')",
                        specialOffset.Key, specialOffset.Use, specialOffset.Offset_mΩ, specialOffset.Offset_Ω, specialOffset.Offset_kΩ, specialOffset.Offset_MΩ,
                        specialOffset.Offset_pF, specialOffset.Offset_nF, specialOffset.Offset_uF, specialOffset.Offset_mF, specialOffset.Offset_F);
                    SqlLiteManager.Instance.BomDB.Insert(comm);
                }
            }
            SearchALLSpectialOffset();
        }
    }
}
