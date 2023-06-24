using Bolt;
using CDOverhaul.Gameplay;
using CDOverhaul.Gameplay.Multiplayer;
using CDOverhaul.Gameplay.QualityOfLife;
using CDOverhaul.Graphics;
using CDOverhaul.HUD;
using CDOverhaul.Patches;
using System;
using System.Collections;
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
                return;

            OverhaulCompatibilityChecker.CheckGameVersion();
            OverhaulMod.Core = this;
            _ = OverhaulAPI.API.LoadAPI();

            GameObject controllers = new GameObject("Controllers");
            controllers.transform.SetParent(base.transform);

            OverhaulObjectStateModder.ClearDestroyedObjects();
            OverhaulAudioLibrary.Initialize();
            OverhaulEventsController.Initialize();
            OverhaulSettingsController.Initialize();
            EnableCursorController.Reset();
            OverhaulController.InitializeStatic(controllers);

            CanvasController = OverhaulController.AddController<OverhaulCanvasController>();
            _ = OverhaulController.AddController<OverhaulVolumeController>();
            _ = OverhaulController.AddController<OverhaulGameplayCoreController>();
            _ = OverhaulController.AddController<OverhaulModdedPlayerInfoController>();

            _ = OverhaulController.AddController<AutoBuild>();
            _ = OverhaulController.AddController<LevelEditorFixes>();

            _ = OverhaulController.AddController<ViewModesController>();

            OverhaulSettingsController.CreateHUD();
            OverhaulGraphicsController.Initialize();
            PlayFabDataController.Initialize();
            OverhaulTransitionController.Initialize();
            OverhaulLocalizationController.Initialize();
            OverhaulPatchNotes.Initialize();
            OverhaulBootUI.Show();

            if (OverhaulDiscordController.Instance == null)
                _ = new GameObject("OverhaulDiscordRPCController").AddComponent<OverhaulDiscordController>();

            ReplacementBase.CreateReplacements();
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
