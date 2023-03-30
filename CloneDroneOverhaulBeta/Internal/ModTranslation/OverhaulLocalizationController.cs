using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CDOverhaul.Localization
{
    public static class OverhaulLocalizationController
    {
        public const string LocalizationFileName = "Localization";
        private static bool? m_Error;

        private static OverhaulLocalizationData m_Data;
        public static OverhaulLocalizationData Localization
        {
            get
            {
                if (!m_Error.HasValue || m_Error.Value)
                {
                    return null;
                }
                return m_Data;
            }
        }

        public static void Initialize()
        {
            if (OverhaulSessionController.GetKey<bool>("LoadedTranslations"))
            {
                return;
            }
            OverhaulSessionController.SetKey("LoadedTranslations", true);

            loadData();
        }
        
        private static async void loadData()
        {
            string path = OverhaulMod.Core.ModDirectory + "Assets/" + LocalizationFileName + ".json";
            if (!File.Exists(path))
            {
                File.CreateText(path);
                m_Error = false;
                m_Data = new OverhaulLocalizationData();
                m_Data.RepairFields();
                m_Data.SavedInVersion = OverhaulVersion.ModVersion;
                return;
            }
            StreamReader reader = File.OpenText(path);

            Task<string> task = reader.ReadToEndAsync();
            await task;
            if(task.IsCanceled || task.IsFaulted)
            {
                m_Error = true;
                return;
            }

            m_Error = false;
            m_Data = JsonConvert.DeserializeObject<OverhaulLocalizationData>(task.Result);
            if(m_Data != null)
            {
                m_Data.RepairFields();
            }
        }

        public static void SaveData()
        {
            if(m_Data != null && m_Error.HasValue && !m_Error.Value)
            {
                m_Data.SavedInVersion = OverhaulVersion.ModVersion;
                File.WriteAllText(OverhaulMod.Core.ModDirectory + "Assets/" + OverhaulLocalizationController.LocalizationFileName + ".json",
                 Newtonsoft.Json.JsonConvert.SerializeObject(m_Data, Newtonsoft.Json.Formatting.None, DataRepository.CreateSettings()));
            }
        }
    }
}
