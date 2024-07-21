using OverhaulMod.Utils;
using System;
using UnityEngine;

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

        public static void MessagePopupTest()
        {
            string desc = string.Empty;
            for (int i = 0; i < 75; i++)
            {
                desc += "very long string ";
            }

            ModUIUtils.MessagePopupOK("hmm", desc, 175f, true);
        }

        public static void FileDownloadTest()
        {
            FileDownloader fileDownloader = new FileDownloader();
            fileDownloader.DownloadProgressChanged += onDownloadProgress;
            fileDownloader.DownloadFileCompleted += (sender, e) =>
            {
                if (e.Cancelled)
                    Debug.Log("Download cancelled");
                else if (e.Error != null)
                    Debug.Log("Download failed: " + e.Error);
                else
                    Debug.Log("Download completed");
            };
            fileDownloader.DownloadFileAsync("https://drive.google.com/file/d/1T_sWBJdpXe74dZrtN7pPZXjT4sVgq7-o/view?usp=drive_link", "D:\\CloneDroneBeta.zip");
        }

        private static void onDownloadProgress(object sender, FileDownloader.DownloadProgress e)
        {
            if (s_lastFrameDownloadProgressWasDisplayed == Time.frameCount)
                return;

            s_lastFrameDownloadProgressWasDisplayed = Time.frameCount;
            Debug.Log("Progress changed " + e.BytesReceived + " " + e.TotalBytesToReceive);
        }
    }
}
