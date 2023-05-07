using System;
using System.Collections;
using System.IO;
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

        public static float MultiplayerLocalPing => MultiplayerPlayerInfoManager.Instance == null ? 0f : MultiplayerPlayerInfoManager.Instance.GetLocalPing() / 1000f;

        public static OverhaulNetworkDownloadHandler DownloadAndSaveFile(string address, string directoryPath, string fileName, Action onDoneDownloadingAction)
        {
            OverhaulNetworkDownloadHandler handler = new OverhaulNetworkDownloadHandler();
            Action onDone = delegate
            {
                if (!handler.Error)
                {
                    if (!Directory.Exists(directoryPath))
                    {
                        return;
                    }

                    if (!handler.DownloadedData.IsNullOrEmpty())
                    {
                        File.WriteAllBytes(directoryPath + fileName, handler.DownloadedData);
                    }
                    else
                    {
                        File.WriteAllText(directoryPath + fileName, handler.DownloadedText);
                    }
                }
            };
            Action act2 = onDone.Combine(onDoneDownloadingAction);
            handler.DoneAction = act2;
            DownloadFile(address, handler);
            return handler;
        }

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

        public static void DownloadTexture(string address, OverhaulNetworkDownloadHandler downloadHandler)
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

            _ = StaticCoroutineRunner.StartStaticCoroutine(downloadTextureCoroutine(address, downloadHandler));
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
            }
            downloadHandler.DoneAction.Invoke();
            downloadHandler.WebRequest = null;

            yield break;
        }

        private static IEnumerator downloadTextureCoroutine(string address, OverhaulNetworkDownloadHandler downloadHandler)
        {
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(address))
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
                    downloadHandler.DownloadedTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                }
            }
            downloadHandler.DoneAction.Invoke();
            downloadHandler.WebRequest = null;
            downloadHandler.DownloadedTexture = null;

            yield break;
        }

        private static bool checkAddress(string address)
        {
            return !string.IsNullOrEmpty(address);
        }

        private static void emptyVoid() { }
    }
}