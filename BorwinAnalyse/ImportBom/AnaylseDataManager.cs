using BorwinAnalyse.BaseClass;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BorwinAnalyse.ImportBom
{
    public class AnaylseDataManager
    {
        private static AnaylseDataManager instance;
        public static AnaylseDataManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AnaylseDataManager();
                }
                return instance;
            }
        }

        public void Load()
        {
            string savePath = @"Ini/AnaylseDataManager.json";
            if (!File.Exists(savePath))
            {
                FileStream fs1 = new FileStream(savePath, FileMode.Create, FileAccess.ReadWrite);
                fs1.Close();
            }
            else
            {
                instance = JsonConvert.DeserializeObject<AnaylseDataManager>(File.ReadAllText(savePath));
            }
        }

        public void Save()
        {
            string savePath = @"Ini/AnaylseDataManager.json";
            if (!File.Exists(savePath))
            {
                FileStream fs1 = new FileStream(savePath, FileMode.Create, FileAccess.ReadWrite);
                fs1.Close();
            }
            File.WriteAllText(savePath, JsonConvert.SerializeObject(instance));
        }


        public BomDataControl Bom = new BomDataControl();

        public XYDataControl XYData = new XYDataControl();


        /// <summary>
        /// 规则列表
        /// </summary>
        public List<string> Rules = new List<string>();
    }
}
