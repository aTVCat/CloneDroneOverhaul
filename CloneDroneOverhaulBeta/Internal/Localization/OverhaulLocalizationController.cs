using CDOverhaul.HUD;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine.UI;

namespace CDOverhaul
{
    public static class OverhaulLocalizationController
    {
        public const string LocalizationFileName = "Localization";

        private static bool? m_Error;
        public static bool Error => !m_Error.HasValue || m_Error.Value || m_Data == null;

        private static OverhaulLocalizationData m_Data;
        public static OverhaulLocalizationData Localization => !m_Error.HasValue || m_Error.Value ? null : m_Data;

        public static bool HasTranslation(string translationID) => !Error && Localization.Translations["en"].ContainsKey(translationID);
        public static string GetTranslation(string translationID) => Error ? translationID : Localization.GetTranslation(translationID);

        private static readonly List<Text> m_ListOfTexts = new List<Text>();
        private static bool m_TryingToLocalizeHUD;

        private static bool m_EventIsScheduled;
        private static bool m_IsSavingFile;

        private static FileStream m_FileStream;
        private static long m_FileStreamEndPosition;

        public static void Initialize()
        {
            m_EventIsScheduled = false;
            m_ListOfTexts.Clear();

            OverhaulCanvasController controller = OverhaulController.GetController<OverhaulCanvasController>();
            if (controller != null)
                m_ListOfTexts.AddRange(controller.GetAllComponentsWithModdedObjectRecursive<Text>("LID_", controller.HUDModdedObject.transform));

            _ = OverhaulEventsController.AddEventListener(GlobalEvents.UILanguageChanged, TryLocalizeHUD, true);

            if (OverhaulSessionController.GetKey<bool>("LoadedTranslations"))
            {
                ScheduleEvent();
                return;
            }
            OverhaulSessionController.SetKey("LoadedTranslations", true);
            loadData();

            if (Error)
            {
                OverhaulWebhooksController.ExecuteErrorsWebhook("Localization PostInitialize - Error");
                return;
            }

            TryLocalizeHUD();
            ScheduleEvent();
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
            /*
            StreamReader reader = File.OpenText(path);
            Task<string> task = reader.ReadToEndAsync();
            _ = await task;
            if (task.IsCanceled || task.IsFaulted)
            {
                m_Error = true;
                return;
            }*/

            using (Stream s = File.OpenRead(path))
            using (StreamReader sr = new StreamReader(s))
            using (JsonReader areader = new JsonTextReader(sr))
            {
                JsonSerializer serializer = new JsonSerializer();
                //serializer.CheckAdditionalContent = true;

                OverhaulLocalizationData p = serializer.Deserialize<OverhaulLocalizationData>(areader);
                if (p != null)
                    p.RepairFields();

                m_Error = false;
                m_Data = p;
            }

            /*
            m_Error = false;
            m_Data = JsonConvert.DeserializeObject<OverhaulLocalizationData>(task.Result);
            if (m_Data != null) m_Data.RepairFields();
            task.Dispose();*/
        }

        public static async void SaveData()
        {
            if (m_Data != null && m_Error.HasValue && !m_Error.Value && !m_IsSavingFile)
            {
                m_Data.SavedInVersion = OverhaulVersion.ModVersion;
                m_IsSavingFile = true;

                if (OverhaulLoadingScreen.Instance != null)
                {
                    OverhaulLoadingScreen.Instance.SetScreenActive(true);
                    OverhaulLoadingScreen.Instance.SetScreenFill(0f);
                    OverhaulLoadingScreen.Instance.SetScreenText("Saving localization file...");
                }

                string content = Newtonsoft.Json.JsonConvert.SerializeObject(m_Data, Newtonsoft.Json.Formatting.None, DataRepository.CreateSettings());
                byte[] byteArray = Encoding.UTF8.GetBytes(content);

                m_FileStreamEndPosition = byteArray.LongLength;
                m_FileStream = File.OpenWrite(OverhaulMod.Core.ModDirectory + "Assets/" + OverhaulLocalizationController.LocalizationFileName + ".json");
                await m_FileStream.WriteAsync(byteArray, 0, byteArray.Length);
                m_FileStream.Close();
                m_FileStream = null;
                m_FileStreamEndPosition = 0L;

                if (OverhaulLoadingScreen.Instance != null)
                    OverhaulLoadingScreen.Instance.SetScreenActive(false);

                OverhaulLocalizationController.TryLocalizeHUD();
                m_IsSavingFile = false;
            }
        }

        public static void ScheduleEvent()
        {
            if (m_EventIsScheduled || DelegateScheduler.Instance == null)
                return;

            m_EventIsScheduled = true;
            DelegateScheduler.Instance.Schedule(delegate
            {
                m_EventIsScheduled = false;
                if (GlobalEventManager.Instance != null)
                    GlobalEventManager.Instance.Dispatch(GlobalEvents.UILanguageChanged);
            }, 0.5f);
        }

        public static void TryLocalizeHUD()
        {
            if (m_TryingToLocalizeHUD || !SettingsManager.Instance.IsInitialized())
                return;

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

        public static void UpdateLoadingScreen()
        {
            if (OverhaulLoadingScreen.Instance != null && OverhaulLoadingScreen.Instance.gameObject.activeSelf && m_FileStream != null && m_FileStreamEndPosition != 0L)
            {
                OverhaulLoadingScreen.Instance.SetScreenFill(m_FileStream.Position / (float)m_FileStreamEndPosition);
            }
        }
    }
}
