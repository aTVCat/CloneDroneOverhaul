using Bolt;
using CDOverhaul.Gameplay;
using CDOverhaul.Gameplay.Multiplayer;
using CDOverhaul.Gameplay.Outfits;
using CDOverhaul.Gameplay.Pets;
using CDOverhaul.Gameplay.QualityOfLife;
using CDOverhaul.Graphics;
using CDOverhaul.HUD;
using CDOverhaul.MultiplayerSandbox;
using CDOverhaul.Patches;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;

namespace CDOverhaul
{
    public class OverhaulCore : GlobalEventListener
    {
        /// <summary>
        /// The mod directory path.
        /// Ends with '/'
        /// </summary>
        public string ModDirectory => OverhaulMod.Base.ModInfo.FolderPath;

        public static string ModDirectoryStatic => OverhaulMod.Base.ModInfo.FolderPath;

        [OverhaulSetting("Gameplay.Multiplayer.Relay Connection", false, false, "This one fixes connection issues, but also increases/decreases ping for some users")]
        public static bool IsRelayConnectionEnabled;

        private static bool s_HasUpdatedLangFont;

        /// <summary>
        /// The UI controller instance
        /// </summary>
        public OverhaulCanvasController CanvasController
        {
            get;
            private set;
        }

        internal bool Initialize(out string errorString)
        {
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                initialize();
                stopwatch.Stop();

                string contents = stopwatch.ElapsedMilliseconds + " Milliseconds";
                OverhaulCore.WriteText(ModDirectory + "Logs/StartupTime.log", contents);
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
            _ = OverhaulAPI.API.LoadAPI();

            GameObject controllers = new GameObject("Controllers");
            controllers.transform.SetParent(base.transform);

            OverhaulObjectStateModder.Reset();
            EnableCursorController.Reset();

            OverhaulEventsController.Initialize();
            OverhaulSettingsController.Initialize();
            OverhaulController.InitializeStatic(controllers);

            //CanvasController = OverhaulController.AddController<OverhaulCanvasController>();
            _ = OverhaulController.AddController<OverhaulGameplayCoreController>();
            _ = OverhaulController.AddController<OverhaulModdedPlayerInfoController>();
            _ = OverhaulController.AddController<OverhaulVolumeController>();

            _ = OverhaulController.AddController<AutoBuild>();
            _ = OverhaulController.AddController<LevelEditorFixes>();
            _ = OverhaulController.AddController<ModBotTagDisabler>();

            _ = OverhaulController.AddController<ViewModesController>();
            _ = OverhaulController.AddController<OverhaulDiscordController>();
            _ = OverhaulController.AddController<MultiplayerSandboxController>();

            OverhaulGraphicsController.Initialize();
            PlayFabDataController.Initialize();

            OverhaulCompatibilityChecker.CheckGameVersion();
            if (!OverhaulBootUI.Show())
            {
                LoadSyncStuff();
            }
        }

        public IEnumerator LoadAsyncStuff()
        {
            OverhaulCore.WriteText(ModDirectory + "Logs/StartupAssetBundles.log", OverhaulAssetsController.GetLoadedAssetBundlesString());

            bool hasLoadedPart1Bundle = OverhaulAssetsController.HasLoadedAssetBundle(OverhaulAssetsController.ModAssetBundle_Part1);
            bool hasLoadedPart2Bundle = OverhaulAssetsController.HasLoadedAssetBundle(OverhaulAssetsController.ModAssetBundle_Part2);
            bool hasLoadedSkinsBundle = OverhaulAssetsController.HasLoadedAssetBundle(OverhaulAssetsController.ModAssetBundle_Skins);
            bool hasLoadedOutfitsBundle = OverhaulAssetsController.HasLoadedAssetBundle(OverhaulAssetsController.ModAssetBundle_Accessouries);
            bool hasLoadedPetsBundle = OverhaulAssetsController.HasLoadedAssetBundle(OverhaulAssetsController.ModAssetBundle_Pets);
            bool hasLoadedCombatUpdateBundle = OverhaulAssetsController.HasLoadedAssetBundle(OverhaulAssetsController.ModAssetBundle_CombatUpdate);

            if (!hasLoadedPart1Bundle) OverhaulAssetsController.LoadAssetBundleAsync(OverhaulAssetsController.ModAssetBundle_Part1, delegate (OverhaulAssetsController.AssetBundleLoadHandler h)
            {
                hasLoadedPart1Bundle = true;
            });

            if (!hasLoadedPart2Bundle) OverhaulAssetsController.LoadAssetBundleAsync(OverhaulAssetsController.ModAssetBundle_Part2, delegate (OverhaulAssetsController.AssetBundleLoadHandler h)
            {
                hasLoadedPart2Bundle = true;
            });

            if (!hasLoadedSkinsBundle) OverhaulAssetsController.LoadAssetBundleAsync(OverhaulAssetsController.ModAssetBundle_Skins, delegate (OverhaulAssetsController.AssetBundleLoadHandler h)
            {
                hasLoadedSkinsBundle = true;
            });

            if (!hasLoadedOutfitsBundle) OverhaulAssetsController.LoadAssetBundleAsync(OverhaulAssetsController.ModAssetBundle_Accessouries, delegate (OverhaulAssetsController.AssetBundleLoadHandler h)
            {
                hasLoadedOutfitsBundle = true;
            });

            if (!hasLoadedPetsBundle) OverhaulAssetsController.LoadAssetBundleAsync(OverhaulAssetsController.ModAssetBundle_Pets, delegate (OverhaulAssetsController.AssetBundleLoadHandler h)
            {
                hasLoadedPetsBundle = true;
            });

            if (!hasLoadedCombatUpdateBundle) OverhaulAssetsController.LoadAssetBundleAsync(OverhaulAssetsController.ModAssetBundle_CombatUpdate, delegate (OverhaulAssetsController.AssetBundleLoadHandler h)
            {
                hasLoadedCombatUpdateBundle = true;
            });

            yield return new WaitUntil(() => hasLoadedPart1Bundle && hasLoadedPart2Bundle && hasLoadedSkinsBundle && hasLoadedOutfitsBundle && hasLoadedPetsBundle);
            yield break;
        }

        public void LoadSyncStuff()
        {
            CanvasController = OverhaulController.AddController<OverhaulCanvasController>();

            _ = OverhaulController.AddController<HUD.Tooltips.OverhaulTooltipsController>();

            OverhaulController.GetController<WeaponSkinsController>().AddSkins();
            OverhaulController.GetController<OutfitsController>().AddOutfitItems();
            OverhaulController.GetController<PetsController>().AddPets();

            OverhaulSettingsController.CreateHUD();
            OverhaulLocalizationController.Initialize();
            OverhaulTransitionController.Initialize();
            OverhaulAudioLibrary.Initialize();
            OverhaulPatchNotes.Initialize();

            ReplacementBase.CreateReplacements();

            if (!s_HasUpdatedLangFont)
            {
                s_HasUpdatedLangFont = true;
                LocalizationManager.Instance.SetCurrentLanguage(SettingsManager.Instance.GetCurrentLanguageID());
            }

            OverhaulMod.HasBootProcessEnded = true;
        }

        private void OnDestroy()
        {
            CanvasController = null;
            OverhaulMod.Core = null;
            ReplacementBase.CancelEverything();
        }

        private void Update()
        {
            OverhaulLocalizationController.UpdateLoadingScreen();
            CameraRollingBehaviour.UpdateViewBobbing();
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

        public class IOStateInfo
        {
            public bool IsInProgress;
        }
    }
}
