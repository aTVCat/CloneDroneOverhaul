using CDOverhaul.HUD;
using CDOverhaul.Localization;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace CDOverhaul
{
    public static class OverhaulLocalizationController
    {
        private static bool? m_Error;
        public static bool Error => !m_Error.HasValue || m_Error.Value || m_Data == null;

        public const string LocalizationFileName = "Localization";
        private static OverhaulLocalizationData m_Data;
        public static OverhaulLocalizationData Localization => !m_Error.HasValue || m_Error.Value ? null : m_Data;

        public static bool HasTranslation(string translationID)
        {
            return !Error && Localization.Translations["en"].ContainsKey(translationID);
        }

        public static string GetTranslation(string translationID)
        {
            return Error ? translationID : Localization.GetTranslation(translationID);
        }

        private static readonly List<Text> m_ListOfTexts = new List<Text>();
        private static bool m_TryingToLocalizeHUD;

        private static bool m_EventIsScheduled;

        public static void Initialize()
        {
            m_EventIsScheduled = false;
            OverhaulCanvasController controller = OverhaulController.GetController<OverhaulCanvasController>();
            m_ListOfTexts.Clear();
            m_ListOfTexts.AddRange(controller.GetAllComponentsWithModdedObjectRecursive<Text>("LID_", controller.HUDModdedObject.transform));
            _ = OverhaulEventsController.AddEventListener(GlobalEvents.UILanguageChanged, TryLocalizeHUD, true);

            if (OverhaulSessionController.GetKey<bool>("LoadedTranslations"))
            {
                ScheduleEvent();
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
            if (task.IsCanceled || task.IsFaulted)
            {
                m_Error = true;
                return;
            }

            m_Error = false;
            m_Data = JsonConvert.DeserializeObject<OverhaulLocalizationData>(task.Result);
            if (m_Data != null)
            {
                m_Data.RepairFields();
            }

            TryLocalizeHUD();
            ScheduleEvent();
        }

        public static void SaveData()
        {
            if (m_Data != null && m_Error.HasValue && !m_Error.Value)
            {
                m_Data.SavedInVersion = OverhaulVersion.ModVersion;
                File.WriteAllText(OverhaulMod.Core.ModDirectory + "Assets/" + OverhaulLocalizationController.LocalizationFileName + ".json",
                 Newtonsoft.Json.JsonConvert.SerializeObject(m_Data, Newtonsoft.Json.Formatting.None, DataRepository.CreateSettings()));
            }
        }

        public static void ScheduleEvent()
        {
            if (m_EventIsScheduled)
            {
                return;
            }

            m_EventIsScheduled = true;
            DelegateScheduler.Instance.Schedule(delegate
            {
                m_EventIsScheduled = false;
                if (GlobalEventManager.Instance != null)
                {
                    GlobalEventManager.Instance.Dispatch(GlobalEvents.UILanguageChanged);
                }
            }, 0.5f);
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
            foreach (Text text in m_ListOfTexts)
            {
                if (text != null)
                {
                    if (iteration % 10 == 0)
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
