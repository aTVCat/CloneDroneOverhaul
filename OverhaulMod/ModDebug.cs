using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace OverhaulMod
{
    public static class ModDebug
    {
        private const string PREFIX = "[Overhaul]";

        private static int s_lastFrameDownloadProgressWasDisplayed;

        private static string s_environmentInfoString;

        public static bool ForceDisableCursor;

        public static void RefreshEnvironmentInfoString()
        {
            // runtime info
            string overhaulVersion = $"Overhaul {ModBuild.Version}";
            string modBotVersion = $"Mod-Bot {ModLibrary.Properties.Resources.ModBotVersion}";
            string gameVersion = $"Clone Drone {VersionNumberManager.Instance.GetVersionString()}";
            string unityVersion = $"Unity {Application.unityVersion}";
            string platform = $"{(GameVersionManager.IsSteamBuild() ? "Steam" : "Non-Steam")}";
            string language = $"{LocalizationManager.Instance.GetCurrentLanguageCode()}";

            // game environment info
            GameFlowManager gameFlowManager = GameFlowManager.Instance;
            string gameMode = gameFlowManager ? gameFlowManager.GetCurrentGameMode().ToString() : "N/A";

            LevelManager levelManager = LevelManager.Instance;
            string levelId = levelManager ? levelManager.GetCurrentLevelID() : "N/A";

            ArenaLiftManager arenaLiftManager = ArenaLiftManager.Instance;
            string liftTarget = arenaLiftManager && arenaLiftManager.Lift ? arenaLiftManager.GetLiftTarget().ToString() : "N/A";

            string detailsString = $"{overhaulVersion} · {modBotVersion} · {gameVersion} · {unityVersion} · {platform} · {language} | {gameMode} · {levelId} · {liftTarget}";
            s_environmentInfoString = detailsString;
        }

        public static string GetEnvironemntInfoString() => s_environmentInfoString;

        public static void Log(object obj)
        {
            Debug.Log($"{PREFIX} {obj}");
        }

        public static void Warn(object obj)
        {
            Debug.LogWarning($"{PREFIX} {obj}");
        }

        public static void Error(object obj)
        {
            Debug.LogError($"{PREFIX} {obj}");
        }

        public static void Exception(Exception obj)
        {
            Debug.LogException(obj);
        }

        public static void AddAntialiasingEffect(Camera camera)
        {
            Antialiasing antialiasing = camera.gameObject.AddComponent<Antialiasing>();
            antialiasing.dlaaShader = Shader.Find("Hidden/DLAA");
            antialiasing.nfaaShader = Shader.Find("Hidden/NFAA");
            antialiasing.ssaaShader = Shader.Find("Hidden/SSAA");
            antialiasing.shaderFXAAII = Shader.Find("Hidden/FXAA II");
            antialiasing.shaderFXAAIII = Shader.Find("Hidden/FXAA III (Console)");
            antialiasing.shaderFXAAPreset2 = Shader.Find("Hidden/FXAA Preset 2");
            antialiasing.shaderFXAAPreset3 = Shader.Find("Hidden/FXAA Preset 3");
            antialiasing.mode = AAMode.FXAA2;
        }

        public static void AddDepthOfFieldEffect(Camera camera)
        {
            _ = camera.gameObject.AddComponent<DepthOfField>();
        }

        public static void FileDownloadTest()
        {
            GoogleDriveFileDownloader fileDownloader = new GoogleDriveFileDownloader();
            fileDownloader.DownloadProgressChanged += onDownloadProgress;
            fileDownloader.DownloadFileCompleted += (sender, e) =>
            {
                if (e.Cancelled)
                    Log("Download cancelled");
                else if (e.Error != null)
                    Log("Download failed: " + e.Error);
                else
                    Log("Download completed");
            };
            fileDownloader.DownloadFileAsync("123", "D:\\123.zip");
        }

        private static void onDownloadProgress(object sender, GoogleDriveFileDownloader.DownloadProgress e)
        {
            if (s_lastFrameDownloadProgressWasDisplayed == Time.frameCount)
                return;

            s_lastFrameDownloadProgressWasDisplayed = Time.frameCount;
            Log("Progress changed " + e.BytesReceived + " " + e.TotalBytesToReceive);
        }
    }
}