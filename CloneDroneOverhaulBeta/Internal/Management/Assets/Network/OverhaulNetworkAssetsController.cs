using System;
using System.Collections;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace CDOverhaul.NetworkAssets
{
    public static class OverhaulNetworkAssetsController
    {
        public static string DownloadFolder => OverhaulMod.Core.ModDirectory + "Assets/Download/";

        public static float MultiplayerLocalPing => MultiplayerPlayerInfoManager.Instance == null ? 0f : MultiplayerPlayerInfoManager.Instance.GetLocalPing() / 1000f;

        public static void DownloadText(string address, Text text)
        {
            OverhaulDownloadInfo downloadInfo = new OverhaulDownloadInfo();
            downloadInfo.DoneAction = delegate
            {
                if (downloadInfo != null && text)
                {
                    text.text = downloadInfo.DownloadedText;
                }
            };
            DownloadTexture(address, downloadInfo);
        }

        public static OverhaulDownloadInfo DownloadAndSaveData(string address, string directoryPath, string fileName, Action onDoneDownloadingAction)
        {
            OverhaulDownloadInfo handler = new OverhaulDownloadInfo();
            Action onDone = delegate
            {
                if (!handler.Error)
                {
                    if (!Directory.Exists(directoryPath))
                        return;

                    if (!handler.DownloadedData.IsNullOrEmpty())
                        File.WriteAllBytes(directoryPath + fileName, handler.DownloadedData);
                    else
                        File.WriteAllText(directoryPath + fileName, handler.DownloadedText);
                }
            };

            handler.DoneAction = onDone.Combine(onDoneDownloadingAction);
            DownloadData(address, handler);
            return handler;
        }

        public static OverhaulDownloadInfo DownloadData(string address, Action onDoneDownloadingAction)
        {
            OverhaulDownloadInfo h = new OverhaulDownloadInfo
            {
                DoneAction = onDoneDownloadingAction
            };
            DownloadData(address, h);
            return h;
        }

        public static void DownloadData(string address, OverhaulDownloadInfo downloadHandler)
        {
            if (downloadHandler == null || downloadHandler.DoneAction == null)
                return;

            _ = StaticCoroutineRunner.StartStaticCoroutine(downloadDataCoroutine(address, downloadHandler));
        }

        private static IEnumerator downloadDataCoroutine(string address, OverhaulDownloadInfo downloadHandler)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(address))
            {
                downloadHandler.WebRequest = request;
                yield return request.SendWebRequest();

                downloadHandler.Error = request.isHttpError || request.isNetworkError;
                downloadHandler.ErrorString = request.error;
                if (!downloadHandler.Error)
                {
                    downloadHandler.DownloadedData = request.downloadHandler.data;
                    downloadHandler.DownloadedText = request.downloadHandler.text;
                }
            }

            downloadHandler.OnDownloadComplete();
            yield break;
        }

        public static void DownloadTexture(string address, OverhaulDownloadInfo downloadHandler)
        {
            if (downloadHandler == null || downloadHandler.DoneAction == null)
                return;

            _ = StaticCoroutineRunner.StartStaticCoroutine(downloadTextureCoroutine(address, downloadHandler));
        }

        public static void DownloadTexture(string address, RawImage rawImage)
        {
            OverhaulDownloadInfo downloadInfo = new OverhaulDownloadInfo();
            downloadInfo.DoneAction = delegate
            {
                if (downloadInfo != null && rawImage)
                {
                    rawImage.texture = downloadInfo.DownloadedTexture;
                }
            };
            DownloadTexture(address, downloadInfo);
        }

        private static IEnumerator downloadTextureCoroutine(string address, OverhaulDownloadInfo downloadHandler)
        {
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(address))
            {
                downloadHandler.WebRequest = request;
                yield return request.SendWebRequest();

                downloadHandler.Error = request.isHttpError || request.isNetworkError;
                downloadHandler.ErrorString = request.error;
                if (!downloadHandler.Error)
                {
                    downloadHandler.DownloadedData = request.downloadHandler.data;
                    downloadHandler.DownloadedTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                }
            }

            downloadHandler.OnDownloadComplete();
            yield break;
        }
    }
}