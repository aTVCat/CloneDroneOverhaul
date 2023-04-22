using Bolt;
using CDOverhaul.Gameplay;
using CDOverhaul.Gameplay.Multiplayer;
using CDOverhaul.Graphics;
using CDOverhaul.HUD;
using CDOverhaul.NetworkAssets.AdditionalContent;
using CDOverhaul.Patches;
using System;
using System.IO;
using UnityEngine;

namespace CDOverhaul
{
    /// <summary>
    /// The core of the mod. Contains important variables
    /// </summary>
    public class OverhaulCore : GlobalEventListener
    {
        /// <summary>
        /// The mod directory path.
        /// Ends with '/'
        /// </summary>
        public string ModDirectory => OverhaulMod.Base.ModInfo.FolderPath;

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
            {
                return;
            }
            OverhaulCompatibilityChecker.CheckGameVersion();
            OverhaulMod.Core = this;
            _ = OverhaulAPI.API.LoadAPI();

            if (ExperimentalBranchManager.Instance != null)
            {
                ExperimentalBranchManager.Instance.IsExperimentalBranch = false;
            }

            GameObject controllers = new GameObject("Controllers");
            controllers.transform.SetParent(base.transform);

            OverhaulAudioLibrary.Initialize();
            OverhaulLevelAdder.Initialize();
            OverhaulEventsController.Initialize();
            SettingsController.Initialize();
            OverhaulConsoleController.Initialize();
            EnableCursorController.Reset();
            OverhaulController.InitializeStatic(controllers);

            CanvasController = OverhaulController.AddController<OverhaulCanvasController>();
            _ = OverhaulController.AddController<VoxelsController>();
            _ = OverhaulController.AddController<OverhaulGameplayCoreController>();
            _ = OverhaulController.AddController<OverhaulModdedPlayerInfoController>();
            _ = OverhaulController.AddController<SkyboxOverhaulController>();
            _ = OverhaulController.AddController<OverhaulAdditionalContentController>();

            SettingsController.PostInitialize();
            OverhaulDebugger.Initialize();
            OverhaulGraphicsController.Initialize();
            ExclusivityController.Initialize();
            OverhaulTransitionController.Initialize();
            OverhaulLocalizationController.Initialize();

            if (OverhaulDiscordController.Instance == null)
            {
                _ = new GameObject("OverhaulDiscordRPCController").AddComponent<OverhaulDiscordController>();
            }

            ReplacementBase.CreateReplacements();
        }

        private void OnDestroy()
        {
            CanvasController = null;
            OverhaulMod.Core = null;
            ReplacementBase.CancelEverything();
        }

        public static string ReadTextFile(string filePath)
        {
            string path = filePath.Contains(OverhaulMod.Core.ModDirectory) ? filePath : OverhaulMod.Core.ModDirectory + filePath;
            bool fileExists = File.Exists(path);
            if (!fileExists)
            {
                return string.Empty;
            }

            StreamReader r = File.OpenText(path);
            string result = r.ReadToEnd();
            r.Close();

            return result;
        }
    }
}
