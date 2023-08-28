using Bolt;
using CDOverhaul.CustomMultiplayer;
using CDOverhaul.Gameplay;
using CDOverhaul.Gameplay.Editors.Personalization;
using CDOverhaul.Gameplay.Multiplayer;
using CDOverhaul.Gameplay.Overmodes;
using CDOverhaul.Gameplay.QualityOfLife;
using CDOverhaul.HUD;
using CDOverhaul.Visuals;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace CDOverhaul
{
    public class OverhaulCore : GlobalEventListener
    {
        [OverhaulSetting("Gameplay.Multiplayer.Relay Connection", false, false, "This one fixes connection issues, but also increases/decreases ping for some users")]
        public static bool IsRelayConnectionEnabled;

        /// <summary>
        /// The mod directory path.
        /// Ends with '/'
        /// </summary>
        public string ModDirectory => OverhaulMod.Base.ModInfo.FolderPath;
        public static string ModDirectoryStatic => OverhaulMod.Base.ModInfo.FolderPath;

        public static event Action OnAssetsLoadDone;

        private List<IGenericStringEventListener> m_GenericStringEventListeners = new List<IGenericStringEventListener>();

        private static bool s_HasUpdatedLangFont;

        /// <summary>
        /// The UI controller instance
        /// </summary>
        public OverhaulCanvasController CanvasController
        {
            get;
            private set;
        }

        public override void OnEvent(GenericStringForModdingEvent moddedEvent)
        {
            foreach (IGenericStringEventListener listener in m_GenericStringEventListeners)
                if (listener != null)
                    listener.OnGenericStringEvent(moddedEvent);

            if (moddedEvent == null || string.IsNullOrEmpty(moddedEvent.EventData) || !moddedEvent.EventData.StartsWith(OverhaulPlayerInfosSystem.EVENT_PREFIX))
                return;

            string[] split = moddedEvent.EventData.Split('@');
            if (split[1] != OverhaulPlayerInfosSystem.VERSION)
                return;

            OverhaulPlayerInfoRefreshEventData eventData;
            try
            {
                eventData = moddedEvent.BinaryData.DeserializeObject<OverhaulPlayerInfoRefreshEventData>();
            }
            catch
            {
                return;
            }

            // Exceptions
            if (eventData == default)
            {
                OverhaulWebhooksController.ExecuteErrorsWebhook("Event data is DEFAULT! Version: " + split[1]);
                return;
            }
            else if (eventData.IsRequest && eventData.IsAnswer)
            {
                OverhaulWebhooksController.ExecuteErrorsWebhook("The event is defined as Answer and Request at the same time! Version: " + split[1]);
                return;
            }

            if (eventData.ReceiverPlayFabID == OverhaulPlayerInfoRefreshEventData.RECEIVER_EVERYONE || eventData.ReceiverPlayFabID == OverhaulPlayerIdentifier.GetLocalPlayFabID())
                foreach (OverhaulPlayerInfo overhaulPlayerInfo in OverhaulPlayerInfo.AllOverhaulPlayerInfos)
                    if (overhaulPlayerInfo)
                        overhaulPlayerInfo.OnGenericStringEvent(eventData);
        }

        public void AddGenericStringEventListener(IGenericStringEventListener listener)
        {
            if (m_GenericStringEventListeners == null)
                m_GenericStringEventListeners = new List<IGenericStringEventListener>();

            if (!m_GenericStringEventListeners.Contains(listener))
                m_GenericStringEventListeners.Add(listener);
        }

        public void RemoveGenericStringEventListener(IGenericStringEventListener listener)
        {
            if (m_GenericStringEventListeners.IsNullOrEmpty())
                return;

            m_GenericStringEventListeners.Remove(listener);
        }

        internal bool TryInitialize(out string errorString)
        {
            errorString = null;
            try
            {
                initialize();
            }
            catch (Exception ex)
            {
                errorString = ex.ToString();
            }
            return errorString == null;
        }

        private void initialize()
        {
            if (OverhaulMod.Core != null)
                return;

            OverhaulMod.Core = this;
            _ = OverhaulAPI.OverhaulAPICore.LoadAPI();
            DontDestroyOnLoad(gameObject);

            GameObject controllers = new GameObject("Controllers");
            controllers.transform.SetParent(base.transform);
            OverhaulController.InitializeStatic(controllers);

            using (ModInitialize modInitialize = new ModInitialize())
                modInitialize.Load();

            _ = OverhaulController.Add<OverhaulDiscordController>();
            _ = OverhaulController.Add<OverhaulMultiplayerController>();

            OverhaulPlayerIdentifier.Initialize();
            if (!OverhaulBootUI.Show())
                _ = StaticCoroutineRunner.StartStaticCoroutine(OverhaulMod.Core.LoadSyncStuff(false));
        }

        private void Update()
        {
            if (!OverhaulDebugConsole.ConsoleInstance)
                return;

            if (Input.GetKeyDown(KeyCode.F6))
            {
                bool isVisible = OverhaulDebugConsole.ConsoleInstance.gameObject.activeSelf;
                if (isVisible)
                {
                    OverhaulDebugConsole.ConsoleInstance.Hide();
                }
                else
                {
                    OverhaulDebugConsole.ConsoleInstance.Show();
                }
            }
        }

        public IEnumerator LoadAsyncStuff()
        {
            QualitySettings.asyncUploadTimeSlice = 4;
            QualitySettings.asyncUploadBufferSize = 16;
            QualitySettings.asyncUploadPersistentBuffer = true;

            bool hasLoadedPart1Bundle = OverhaulAssetsController.HasLoadedAssetBundle(OverhaulAssetsController.ModAssetBundle_Part1);
            bool hasLoadedPart2Bundle = OverhaulAssetsController.HasLoadedAssetBundle(OverhaulAssetsController.ModAssetBundle_Part2);
            bool hasLoadedSkinsBundle = OverhaulAssetsController.HasLoadedAssetBundle(OverhaulAssetsController.ModAssetBundle_Skins);
            bool hasLoadedOutfitsBundle = OverhaulAssetsController.HasLoadedAssetBundle(OverhaulAssetsController.ModAssetBundle_Accessouries);
            bool hasLoadedPetsBundle = OverhaulAssetsController.HasLoadedAssetBundle(OverhaulAssetsController.ModAssetBundle_Pets);
            bool hasLoadedArenaUpdateBundle = OverhaulAssetsController.HasLoadedAssetBundle(OverhaulAssetsController.ModAssetBundle_ArenaOverhaul);

            if (!hasLoadedPart1Bundle) _ = OverhaulAssetsController.LoadAssetBundleAsync(OverhaulAssetsController.ModAssetBundle_Part1, delegate (OverhaulAssetsController.AssetBundleLoadHandler h)
            {
                hasLoadedPart1Bundle = true;
            });

            if (!hasLoadedPart2Bundle) _ = OverhaulAssetsController.LoadAssetBundleAsync(OverhaulAssetsController.ModAssetBundle_Part2, delegate (OverhaulAssetsController.AssetBundleLoadHandler h)
            {
                hasLoadedPart2Bundle = true;
            }, false);

            if (!hasLoadedSkinsBundle) _ = OverhaulAssetsController.LoadAssetBundleAsync(OverhaulAssetsController.ModAssetBundle_Skins, delegate (OverhaulAssetsController.AssetBundleLoadHandler h)
            {
                hasLoadedSkinsBundle = true;
            }, false);

            if (!hasLoadedOutfitsBundle) _ = OverhaulAssetsController.LoadAssetBundleAsync(OverhaulAssetsController.ModAssetBundle_Accessouries, delegate (OverhaulAssetsController.AssetBundleLoadHandler h)
            {
                hasLoadedOutfitsBundle = true;
            }, false);

            if (!hasLoadedPetsBundle) _ = OverhaulAssetsController.LoadAssetBundleAsync(OverhaulAssetsController.ModAssetBundle_Pets, delegate (OverhaulAssetsController.AssetBundleLoadHandler h)
            {
                hasLoadedPetsBundle = true;
            }, false);

            if (!hasLoadedArenaUpdateBundle) _ = OverhaulAssetsController.LoadAssetBundleAsync(OverhaulAssetsController.ModAssetBundle_ArenaOverhaul, delegate (OverhaulAssetsController.AssetBundleLoadHandler h)
            {
                hasLoadedArenaUpdateBundle = true;
            });

            yield return new WaitUntil(() => hasLoadedPart1Bundle && hasLoadedPart2Bundle && hasLoadedSkinsBundle && hasLoadedOutfitsBundle && hasLoadedPetsBundle && hasLoadedArenaUpdateBundle);
            OnAssetsLoadDone?.Invoke();
            yield break;
        }

        public IEnumerator LoadSyncStuff(bool waitForEndOfFrame = true)
        {
            PersonalizationEditor.Initialize();
            OverhaulAssetsContainer.Initialize();
            if (waitForEndOfFrame)
                yield return null;

            CanvasController = OverhaulController.Add<OverhaulCanvasController>();
            if (waitForEndOfFrame)
                yield return null;

            _ = OverhaulController.Add<HUD.Tooltips.OverhaulTooltipsController>();
            _ = OverhaulController.Add<UpgradeModesController>();
            _ = OverhaulController.Add<AdvancedPhotomodeController>();

            _ = OverhaulController.Add<OverhaulVFXController>();
            if (waitForEndOfFrame)
                yield return null;

            _ = OverhaulController.Add<OverhaulAchievementsController>();
            _ = OverhaulController.Add<OverhaulRepositoryController>();
            _ = OverhaulController.Add<OvermodesController>();
            if (waitForEndOfFrame)
                yield return null;

            _ = OverhaulController.Add<Gameplay.WeaponSkins.WeaponSkinsController>();

            if (OverhaulFeatureAvailabilitySystem.ImplementedInBuild.AreNewPersonalizationCategoriesEnabled)
            {
                _ = OverhaulController.Add<Gameplay.Pets.PetsController>();
                _ = OverhaulController.Add<Gameplay.Outfits.OutfitsController>();
            }

            if (waitForEndOfFrame)
                yield return null;

            OverhaulTransitionController.Initialize();
            OverhaulAudioLibrary.Initialize();
            OverhaulPatchNotes.Initialize();
            if (waitForEndOfFrame)
                yield return null;

            OverhaulUpdateChecker.CheckForUpdates();
            OverhaulCompatibilityChecker.CheckGameVersion();

            if (!s_HasUpdatedLangFont)
                _ = StaticCoroutineRunner.StartStaticCoroutine(updateLangFontCoroutine());

            OverhaulMod.HasBootProcessEnded = true;
            yield break;
        }

        private void OnDestroy()
        {
            CanvasController = null;
            OverhaulMod.Core = null;
        }

        private static IEnumerator updateLangFontCoroutine()
        {
            yield return new WaitUntil(() => SettingsManager.Instance.IsInitialized());
            LocalizationManager.Instance.SetCurrentLanguage(SettingsManager.Instance.GetCurrentLanguageID());
            s_HasUpdatedLangFont = true;
            yield break;
        }

        public static string ReadText(string filePath)
        {
            if (!File.Exists(filePath))
                return string.Empty;

            string result = string.Empty;
            using (StreamReader r = File.OpenText(filePath))
                result = r.ReadToEnd();

            return result;
        }

        public static bool TryReadText(string filePath, out string content, out Exception exception)
        {
            exception = null;
            content = string.Empty;
            try
            {
                content = ReadText(filePath);
            }
            catch (Exception exc)
            {
                exception = exc;
                return false;
            }
            return true;
        }

        public static async void WriteTextAsync(string filePath, string content, IOStateInfo iOStateInfo = null, bool clearContents = true)
        {
            if (string.IsNullOrEmpty(filePath))
                return;

            if (content == null)
                content = string.Empty;

            if (clearContents)
                File.WriteAllText(filePath, string.Empty);

            if (iOStateInfo != null)
                iOStateInfo.IsInProgress = true;

            byte[] toWrite = Encoding.UTF8.GetBytes(content);
            using (FileStream stream = File.OpenWrite(filePath))
            {
                await stream.WriteAsync(toWrite, 0, toWrite.Length);

                if (iOStateInfo != null)
                    iOStateInfo.IsInProgress = false;
            }
        }

        public static IEnumerator WriteTextCoroutine(string filePath, string content, IOStateInfo iOStateInfo = null, bool clearContents = true)
        {
            if (string.IsNullOrEmpty(filePath))
                yield break;

            if (content == null)
                content = string.Empty;

            if (clearContents)
                File.WriteAllText(filePath, string.Empty);

            if (iOStateInfo != null)
                iOStateInfo.IsInProgress = true;

            byte[] toWrite = Encoding.UTF8.GetBytes(content);
            using (FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write, 4096, true))
            {
                yield return stream.WriteAsync(toWrite, 0, toWrite.Length);

                if (iOStateInfo != null)
                    iOStateInfo.IsInProgress = false;
            }
            yield break;
        }

        public static void WriteText(string filePath, string content, bool clearContents = true)
        {
            if (string.IsNullOrEmpty(filePath))
                return;

            if (content == null)
                content = string.Empty;

            if (clearContents)
                File.WriteAllText(filePath, string.Empty);

            byte[] toWrite = Encoding.UTF8.GetBytes(content);
            using (FileStream stream = File.OpenWrite(filePath))
            {
                stream.Write(toWrite, 0, toWrite.Length);
            }
        }

        public static bool TryWriteText(string filePath, string content, out Exception exception, bool clearContents = true)
        {
            exception = null;
            try
            {
                WriteText(filePath, content, clearContents);
            }
            catch (Exception exc)
            {
                exception = exc;
                return false;
            }
            return true;
        }

        public static bool UnZipFile(string path, string extractDirectory)
        {
            if (!File.Exists(path))
                return false;

            new FastZip().ExtractZip(path, extractDirectory, null);
            return true;
        }

        public class IOStateInfo
        {
            public bool IsInProgress;
        }
    }
}
