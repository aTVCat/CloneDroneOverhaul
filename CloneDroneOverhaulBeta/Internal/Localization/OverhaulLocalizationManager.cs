using CDOverhaul.HUD;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul
{
    public class OverhaulLocalizationManager : OverhaulManager<OverhaulLocalizationManager>
    {
        public const string LocalizationFileName = "Localization";

        private List<Text> m_ListOfTexts;

        private bool m_IsLocalizingHUD;
        private bool m_EventIsScheduled;
        private bool m_IsSavingFile;

        public static bool Error => LocalizationData == null;
        public static OverhaulLocalizationData LocalizationData
        {
            get;
            private set;
        }

        public override void Initialize()
        {
            m_ListOfTexts = new List<Text>();
            LoadData();
            OverhaulEventsController.AddEventListener(GlobalEvents.UILanguageChanged, LocalizeUI, true);
        }

        public override void OnSceneReloaded()
        {
            OverhaulEventsController.AddEventListener(GlobalEvents.UILanguageChanged, LocalizeUI, true);
            RefreshTexts();
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            OverhaulEventsController.RemoveEventListener(GlobalEvents.UILanguageChanged, LocalizeUI, true);
        }

        public void LoadData()
        {
            string path = OverhaulMod.Core.ModDirectory + "Assets/" + LocalizationFileName + ".json";
            if (!File.Exists(path))
            {
                OverhaulDebug.Error("Could not find localization file " + path, EDebugType.ModInit);
                return;
            }

            using (Stream stream = File.OpenRead(path))
            using (StreamReader streamReader = new StreamReader(stream))
            using (JsonReader jsonTextReader = new JsonTextReader(streamReader))
            {
                LocalizationData = new JsonSerializer().Deserialize<OverhaulLocalizationData>(jsonTextReader);
            }
        }

        public async void SaveData()
        {
            if (!Error && !m_IsSavingFile)
            {
                LocalizationData.SavedInVersion = OverhaulVersion.modVersion;
                m_IsSavingFile = true;

                byte[] byteArray = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(LocalizationData, Formatting.None, DataRepository.CreateSettings()));
                using (FileStream fileStream = File.OpenWrite(OverhaulMod.Core.ModDirectory + "Assets/" + OverhaulLocalizationManager.LocalizationFileName + ".json"))
                {
                    await fileStream.WriteAsync(byteArray, 0, byteArray.Length);
                }

                LocalizeUI();
                m_IsSavingFile = false;
            }
        }

        public void GetTexts()
        {
            m_ListOfTexts.Clear();
            m_ListOfTexts.AddRange(OverhaulCanvasController.GetAllComponentsWithModdedObjectRecursive<Text>("LID_", OverhaulCanvasController.reference.HUDModdedObject.transform));
        }

        public void RefreshTexts()
        {
            LocalizeUI();
            ScheduleEvent();
        }

        public static bool HasTranslation(string translationID) => !Error && LocalizationData.Translations["en"].ContainsKey(translationID);
        public static string GetTranslation(string translationID) => Error ? translationID : LocalizationData.GetTranslation(translationID);

        public void ScheduleEvent()
        {
            if (m_EventIsScheduled || DelegateScheduler.Instance == null)
                return;

            m_EventIsScheduled = true;
            DelegateScheduler.Instance.Schedule(delegate
            {
                GlobalEventManager.Instance.Dispatch(GlobalEvents.UILanguageChanged);
                m_EventIsScheduled = false;
            }, 0.5f);
        }

        public void LocalizeUI()
        {
            if (m_IsLocalizingHUD || !SettingsManager.Instance.IsInitialized())
                return;

            _ = StaticCoroutineRunner.StartStaticCoroutine(localizeHUDCoroutine());
        }

        private IEnumerator localizeHUDCoroutine()
        {
            m_IsLocalizingHUD = true;
            yield return new WaitForSecondsRealtime(0.1f);

            int i = 0;
            foreach (Text text in m_ListOfTexts)
            {
                if (text != null)
                {
                    if (i % 20 == 0)
                    {
                        yield return null;
                    }
                    LocalizationData.GetTranslation(text.GetComponent<ModdedObject>(), true);
                }
                i++;
            }
            m_IsLocalizingHUD = false;
            yield break;
        }
    }
}
