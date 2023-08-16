using Bolt;
using CDOverhaul.BuiltIn.AdditionalContent;
using CDOverhaul.CustomMultiplayer;
using CDOverhaul.Device;
using CDOverhaul.DevTools;
using CDOverhaul.Gameplay;
using CDOverhaul.Gameplay.Editors.Personalization;
using CDOverhaul.Gameplay.Mindspace;
using CDOverhaul.Gameplay.Multiplayer;
using CDOverhaul.Gameplay.Overmodes;
using CDOverhaul.Gameplay.QualityOfLife;
using CDOverhaul.Graphics;
using CDOverhaul.Graphics.ArenaOverhaul;
using CDOverhaul.HUD;
using CDOverhaul.LevelEditor;
using CDOverhaul.Patches;
using ICSharpCode.SharpZipLib.Zip;
using Steamworks;
using System;
using System.Collections;
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

        private static bool s_HasUpdatedLangFont;
        public static bool IsSteamInitialized => SteamManager.Instance.Initialized;
        public static bool IsSteamOverlayOpened => SteamUtils.IsOverlayEnabled();

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
            if (moddedEvent == null || string.IsNullOrEmpty(moddedEvent.EventData) || !moddedEvent.EventData.StartsWith(OverhaulPlayerInfoController.PlayerInfoEventPrefix))
                return;

            string[] split = moddedEvent.EventData.Split('@');
            if (split[1] != OverhaulPlayerInfoController.PlayerInfoVersion)
                return;

            OverhaulPlayerInfoRefreshEventData eventData;
            try
            {
                eventData = moddedEvent.BinaryData.DeserializeObject<OverhaulPlayerInfoRefreshEventData>();
            }
            catch
            {
                OverhaulWebhooksController.ExecuteErrorsWebhook("Could not deserialize OverhaulPlayerInfoRefreshEventData! Version: " + split[1]);
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

            if (eventData.ReceiverPlayFabID == OverhaulPlayerIdentifier.GetLocalPlayFabID() || eventData.ReceiverPlayFabID == OverhaulPlayerInfoRefreshEventData.RECEIVER_EVERYONE)
                foreach (OverhaulPlayerInfo overhaulPlayerInfo in OverhaulPlayerInfo.AllOverhaulPlayerInfos)
                    if (overhaulPlayerInfo)
                        overhaulPlayerInfo.OnGenericStringEvent(eventData);
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

            GameObject controllers = new GameObject("Controllers");
            controllers.transform.SetParent(base.transform);

            DeviceSpecifics.Initialize();
            //OverhaulObjectStateModder.Reset();
            EnableCursorController.Reset();

            OverhaulEventsController.Initialize();
            OverhaulSettingsController.Initialize();
            OverhaulController.InitializeStatic(controllers);

            _ = OverhaulController.AddController<OverhaulGameplayCoreController>();
            _ = OverhaulController.AddController<OverhaulPlayerInfoController>();
            _ = OverhaulController.AddController<OverhaulVolumeController>();

            _ = OverhaulController.AddController<AutoBuildController>();
            _ = OverhaulController.AddController<LevelEditorFixes>();
            _ = OverhaulController.AddController<ModBotTagDisabler>();

            _ = OverhaulController.AddController<ViewModesController>();
            _ = OverhaulController.AddController<OverhaulDiscordController>();
            _ = OverhaulController.AddController<OverhaulMultiplayerController>();

            _ = OverhaulController.AddController<LevelEditorMoveObjectsByCoordsController>();

            OverhaulPlayerIdentifier.Initialize();
            if (OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IsBootScreenEnabled && !OverhaulBootUI.Show())
                _ = StaticCoroutineRunner.StartStaticCoroutine(OverhaulMod.Core.LoadSyncStuff(false));
        }

        private void Start()
        {
            if (!OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IsBootScreenEnabled)
                _ = StaticCoroutineRunner.StartStaticCoroutine(OverhaulMod.Core.LoadSyncStuff(false));
        }

        public IEnumerator LoadAsyncStuff()
        {
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
            yield break;
        }

        public IEnumerator LoadSyncStuff(bool waitForEndOfFrame = true)
        {
            PersonalizationEditor.Initialize();
            OverhaulAssetsContainer.Initialize();
            if (waitForEndOfFrame)
                yield return null;

            CanvasController = OverhaulController.AddController<OverhaulCanvasController>();
            if (waitForEndOfFrame)
                yield return null;

            _ = OverhaulController.AddController<HUD.Tooltips.OverhaulTooltipsController>();
            _ = OverhaulController.AddController<UpgradeModesController>();
            _ = OverhaulController.AddController<AdvancedPhotomodeController>();
            _ = OverhaulController.AddController<ArenaOverhaulController>();

            _ = OverhaulController.AddController<MindspaceOverhaulController>();
            _ = OverhaulController.AddController<OverhaulVFXController>();
            if (waitForEndOfFrame)
                yield return null;

            _ = OverhaulController.AddController<AdditionalContentController>();
            _ = OverhaulController.AddController<OverhaulAchievementsController>();
            _ = OverhaulController.AddController<OverhaulRepositoryController>();
            _ = OverhaulController.AddController<OvermodesController>();
            if (waitForEndOfFrame)
                yield return null;

            if (OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IsNewWeaponSkinsSystemEnabled)
                OverhaulController.AddController<Gameplay.WeaponSkins.WeaponSkinsController>();
            else
                OverhaulController.AddController<WeaponSkinsController>();

            if (OverhaulFeatureAvailabilitySystem.ImplementedInBuild.AreNewPersonalizationCategoriesEnabled)
            {
                _ = OverhaulController.AddController<Gameplay.Pets.PetsController>();
                _ = OverhaulController.AddController<Gameplay.Outfits.OutfitsController>();
            }

            OverhaulController.GetController<LevelEditorFixes>().AddUIs();

            _ = OverhaulController.AddController<MoreSkyboxesController>();
            if (waitForEndOfFrame)
                yield return null;

            OverhaulLocalizationController.Initialize();
            OverhaulTransitionController.Initialize();
            OverhaulAudioLibrary.Initialize();
            OverhaulPatchNotes.Initialize();
            OverhaulDebugActions.Initialize();
            OverhaulGraphicsController.Initialize();
            if (waitForEndOfFrame)
                yield return null;

            ReplacementBase.CreateReplacements();
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
            ReplacementBase.CancelEverything();
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
