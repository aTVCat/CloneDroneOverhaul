using Bolt;
using CDOverhaul.BuiltIn.AdditionalContent;
using CDOverhaul.CustomMultiplayer;
using CDOverhaul.Device;
using CDOverhaul.DevTools;
using CDOverhaul.Gameplay;
using CDOverhaul.Gameplay.Mindspace;
using CDOverhaul.Gameplay.Multiplayer;
using CDOverhaul.Gameplay.Outfits;
using CDOverhaul.Gameplay.Pets;
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

            if(eventData.ReceiverPlayFabID == OverhaulPlayerIdentifier.GetLocalPlayFabID() || eventData.ReceiverPlayFabID == OverhaulPlayerInfoRefreshEventData.RECEIVER_EVERYONE)
                foreach (OverhaulPlayerInfo overhaulPlayerInfo in OverhaulPlayerInfo.AllOverhaulPlayerInfos)
                    if (overhaulPlayerInfo)
                        overhaulPlayerInfo.OnGenericStringEvent(eventData);
        }

        internal bool Initialize(out string errorString)
        {
            try
            {
                initialize();
            }
            catch (Exception ex)
            {
                errorString = ex.ToString();
                return false;
            }
            errorString = null;
            return true;
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
            OverhaulObjectStateModder.Reset();
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
            _ = OverhaulController.AddController<CustomMultiplayerController>();

            _ = OverhaulController.AddController<LevelEditorMoveObjectsByCoordsController>();

            OverhaulGraphicsController.Initialize();
            OverhaulPlayerIdentifier.Initialize();

            OverhaulGPUInstancingController.Initialize();
            if (OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IsBootScreenEnabled && !OverhaulBootUI.Show())
                LoadSyncStuff();
        }

        private void Start()
        {
            if (!OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IsBootScreenEnabled)
                LoadSyncStuff();
        }

        public IEnumerator LoadAsyncStuff()
        {
            bool hasLoadedPart1Bundle = OverhaulAssetsController.HasLoadedAssetBundle(OverhaulAssetsController.ModAssetBundle_Part1);
            bool hasLoadedPart2Bundle = OverhaulAssetsController.HasLoadedAssetBundle(OverhaulAssetsController.ModAssetBundle_Part2);
            bool hasLoadedSkinsBundle = OverhaulAssetsController.HasLoadedAssetBundle(OverhaulAssetsController.ModAssetBundle_Skins);
            bool hasLoadedOutfitsBundle = OverhaulAssetsController.HasLoadedAssetBundle(OverhaulAssetsController.ModAssetBundle_Accessouries);
            bool hasLoadedPetsBundle = OverhaulAssetsController.HasLoadedAssetBundle(OverhaulAssetsController.ModAssetBundle_Pets);
            bool hasLoadedCombatUpdateBundle = OverhaulAssetsController.HasLoadedAssetBundle(OverhaulAssetsController.ModAssetBundle_CombatUpdate);
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
            });

            if (!hasLoadedOutfitsBundle) _ = OverhaulAssetsController.LoadAssetBundleAsync(OverhaulAssetsController.ModAssetBundle_Accessouries, delegate (OverhaulAssetsController.AssetBundleLoadHandler h)
            {
                hasLoadedOutfitsBundle = true;
            });

            if (!hasLoadedPetsBundle) _ = OverhaulAssetsController.LoadAssetBundleAsync(OverhaulAssetsController.ModAssetBundle_Pets, delegate (OverhaulAssetsController.AssetBundleLoadHandler h)
            {
                hasLoadedPetsBundle = true;
            });

            if (!hasLoadedCombatUpdateBundle) _ = OverhaulAssetsController.LoadAssetBundleAsync(OverhaulAssetsController.ModAssetBundle_CombatUpdate, delegate (OverhaulAssetsController.AssetBundleLoadHandler h)
            {
                hasLoadedCombatUpdateBundle = true;
            });

            if (!hasLoadedArenaUpdateBundle) _ = OverhaulAssetsController.LoadAssetBundleAsync(OverhaulAssetsController.ModAssetBundle_ArenaOverhaul, delegate (OverhaulAssetsController.AssetBundleLoadHandler h)
            {
                hasLoadedArenaUpdateBundle = true;
            });

            yield return new WaitUntil(() => hasLoadedPart1Bundle && hasLoadedPart2Bundle && hasLoadedSkinsBundle && hasLoadedOutfitsBundle && hasLoadedPetsBundle && hasLoadedArenaUpdateBundle);
            yield break;
        }

        public void LoadSyncStuff()
        {
            CanvasController = OverhaulController.AddController<OverhaulCanvasController>();

            _ = OverhaulController.AddController<HUD.Tooltips.OverhaulTooltipsController>();
            _ = OverhaulController.AddController<UpgradeModesController>();
            _ = OverhaulController.AddController<AdvancedPhotomodeController>();
            _ = OverhaulController.AddController<ArenaOverhaulController>();

            _ = OverhaulController.AddController<MindspaceOverhaulController>();
            _ = OverhaulController.AddController<OverhaulVFXController>();

            _ = OverhaulController.AddController<AdditionalContentController>();
            _ = OverhaulController.AddController<OverhaulAchievementsController>();
            _ = OverhaulController.AddController<OverhaulRepositoryController>();

            OverhaulController.GetController<Gameplay.WeaponSkins.WeaponSkinsController>();
            OverhaulController.GetController<OutfitsController>();
            OverhaulController.GetController<PetsController>();

            OverhaulController.GetController<LevelEditorFixes>().AddUIs();

            _ = OverhaulController.AddController<MoreSkyboxesController>();

            OverhaulSettingsController.CreateHUD();
            OverhaulLocalizationController.Initialize();
            OverhaulTransitionController.Initialize();
            OverhaulAudioLibrary.Initialize();
            OverhaulPatchNotes.Initialize();
            OverhaulDebugActions.Initialize();

            ReplacementBase.CreateReplacements();
            OverhaulUpdateChecker.CheckForUpdates();
            OverhaulCompatibilityChecker.CheckGameVersion();

            if (!s_HasUpdatedLangFont)
            {
                _ = StaticCoroutineRunner.StartStaticCoroutine(updateLangFontCoroutine());
            }

            OverhaulMod.HasBootProcessEnded = true;
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

        public static async void WriteTextAsync(string filePath, string content, IOStateInfo iOStateInfo = null)
        {
            if (string.IsNullOrEmpty(filePath)) return;
            if (content == null) content = string.Empty;

            if (iOStateInfo != null) iOStateInfo.IsInProgress = true;
            byte[] toWrite = Encoding.UTF8.GetBytes(content);
            using (FileStream stream = File.OpenWrite(filePath))
            {
                await stream.WriteAsync(toWrite, 0, toWrite.Length);
                if (iOStateInfo != null) iOStateInfo.IsInProgress = false;
            }
        }

        public static IEnumerator WriteTextCoroutine(string filePath, string content, IOStateInfo iOStateInfo = null)
        {
            if (string.IsNullOrEmpty(filePath)) yield break;
            if (content == null) content = string.Empty;

            if (iOStateInfo != null) iOStateInfo.IsInProgress = true;
            byte[] toWrite = Encoding.UTF8.GetBytes(content);
            using (FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write, 4096, true))
            {
                yield return stream.WriteAsync(toWrite, 0, toWrite.Length);
                if (iOStateInfo != null) iOStateInfo.IsInProgress = false;
            }
            yield break;
        }

        public static void WriteText(string filePath, string content)
        {
            if (string.IsNullOrEmpty(filePath)) return;
            if (content == null) content = string.Empty;

            byte[] toWrite = Encoding.UTF8.GetBytes(content);
            using (FileStream stream = File.OpenWrite(filePath))
                stream.Write(toWrite, 0, toWrite.Length);
        }

        public static bool TryWriteText(string filePath, string content, out Exception exception)
        {
            exception = null;
            try
            {
                WriteText(filePath, content);
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
