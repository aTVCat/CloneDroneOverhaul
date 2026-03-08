using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace OverhaulMod
{
    public static class ModDebug
    {
        private static int s_lastFrameDownloadProgressWasDisplayed;

        public static bool forceDisableCursor
        {
            get;
            set;
        }

        public static void Log(object obj, bool logInReleaseBuilds = false)
        {
            if (ModBuildInfo.debug || logInReleaseBuilds)
                Debug.Log(obj);
        }

        public static void LogWarning(object obj, bool logInReleaseBuilds = false)
        {
            if (ModBuildInfo.debug || logInReleaseBuilds)
                Debug.LogWarning(obj);
        }

        public static void LogError(object obj, bool logInReleaseBuilds = false)
        {
            if (ModBuildInfo.debug || logInReleaseBuilds)
                Debug.LogError(obj);
        }

        public static void LogException(Exception obj, bool logInReleaseBuilds = false)
        {
            if (ModBuildInfo.debug || logInReleaseBuilds)
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