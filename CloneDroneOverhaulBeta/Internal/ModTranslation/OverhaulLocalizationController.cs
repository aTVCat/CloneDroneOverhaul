using CDOverhaul.HUD;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace CDOverhaul.Localization
{
    public static class OverhaulLocalizationController
    {
        private static bool? m_Error;
        public static bool Error => !m_Error.HasValue || m_Error.Value || m_Data == null;

        public const string LocalizationFileName = "Localization";
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

        private static readonly List<Text> m_ListOfTexts = new List<Text>();
        private static bool m_TryingToLocalizeHUD;

        public static void Initialize()
        {
            OverhaulCanvasController controller = OverhaulController.GetController<OverhaulCanvasController>();
            m_ListOfTexts.Clear();
            m_ListOfTexts.AddRange(controller.GetAllComponentsWithModdedObjectRecursive<Text>("LID_", controller.HUDModdedObject.transform));

            if (OverhaulSessionController.GetKey<bool>("LoadedTranslations"))
            {
                return;
            }
            OverhaulSessionController.SetKey("LoadedTranslations", true);
            _ = OverhaulEventManager.AddEventListener(GlobalEvents.UILanguageChanged, TryLocalizeHUD, true);
            loadData();
        }
        
        private static async void loadData()
        {
            string path = OverhaulMod.Core.ModDirectory + "Assets/" + LocalizationFileName + ".json";
            if (!File.Exists(path))
            {
                _ = File.Create(path);
                m_Error = false;
                m_Data = new OverhaulLocalizationData();
                m_Data.RepairFields();
                m_Data.SavedInVersion = OverhaulVersion.ModVersion;
                SaveData();
                return;
            }
            StreamReader reader = File.OpenText(path);

            Task<string> task = reader.ReadToEndAsync();
            _ = await task;
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

            TryLocalizeHUD();
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

        public static void TryLocalizeHUD()
        {
            if (m_TryingToLocalizeHUD)
            {
                return;
            }
            _ = StaticCoroutineRunner.StartStaticCoroutine(localizeHUDCoroutine());
        }

        private static IEnumerator localizeHUDCoroutine()
        {
            m_TryingToLocalizeHUD = true;
            yield return null;
            yield return null;

            int iteration = 0;
            foreach(Text text in m_ListOfTexts)
            {
                if(text != null)
                {
                    if(iteration % 10 == 0)
                    {
                        yield return null;
                    }

                    m_Data.GetTranslation(text.GetComponent<ModdedObject>(), true);
                }
                iteration++;
            }

            m_TryingToLocalizeHUD = false;
            yield break;
        }
    }
}
