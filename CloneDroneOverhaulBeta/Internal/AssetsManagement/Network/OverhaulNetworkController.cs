using System;
using System.Collections;
using UnityEngine.Networking;

namespace CDOverhaul.NetworkAssets
{
    /// <summary>
    /// Controller that can download stuff from GitHub
    /// </summary>
    public static class OverhaulNetworkController
    {
        public static string DownloadFolder => OverhaulMod.Core.ModDirectory + "Assets/Download/";
        public static string PermanentDownloadFolder => OverhaulMod.Core.ModDirectory + "Assets/Download/Permanent/";

        public static OverhaulNetworkDownloadHandler DownloadFile(string address)
        {
            return DownloadFile(address, emptyVoid);
        }

        public static OverhaulNetworkDownloadHandler DownloadFile(string address, Action onDoneDownloadingAction)
        {
            OverhaulNetworkDownloadHandler h = new OverhaulNetworkDownloadHandler
            {
                DoneAction = onDoneDownloadingAction
            };
            DownloadFile(address, h);
            return h;
        }

        public static void DownloadFile(string address, OverhaulNetworkDownloadHandler downloadHandler)
        {
            if (downloadHandler == null || downloadHandler.DoneAction == null)
            {
                return;
            }

            if (!checkAddress(address))
            {
                downloadHandler.Error = true;
                downloadHandler.ErrorString = address + " is incorrect address";
                downloadHandler.DoneAction.Invoke();
                return;
            }

            _ = StaticCoroutineRunner.StartStaticCoroutine(downloadFileCoroutine(address, downloadHandler));
        }

        private static IEnumerator downloadFileCoroutine(string address, OverhaulNetworkDownloadHandler downloadHandler)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(address))
            {
                downloadHandler.WebRequest = request;

                yield return request.SendWebRequest();

                bool error = request.isHttpError || request.isNetworkError;
                if (error)
                {
                    downloadHandler.Error = true;
                    downloadHandler.ErrorString = request.error;
                }
                else
                {
                    downloadHandler.DownloadedData = request.downloadHandler.data;
                    downloadHandler.DownloadedText = request.downloadHandler.text;
                }
                downloadHandler.WebRequest = null;
            }
            downloadHandler.DoneAction.Invoke();

            yield break;
        }

        private static bool checkAddress(string address)
        {
            return !string.IsNullOrEmpty(address);
        }

        private static void emptyVoid() { }
    }
}