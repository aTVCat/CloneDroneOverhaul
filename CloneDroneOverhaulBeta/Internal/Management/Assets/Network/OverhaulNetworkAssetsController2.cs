using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace CDOverhaul.NetworkAssets
{
    public class OverhaulNetworkAssetsController
    {
        public static OverhaulDownloadInfo DownloadData(string uri, Action<OverhaulDownloadInfo> doneCallback)
        {
            OverhaulDownloadInfo overhaulDownloadInfo = new OverhaulDownloadInfo(doneCallback);
            StaticCoroutineRunner.StartStaticCoroutine(downloadFileCoroutine(uri, overhaulDownloadInfo));
            return overhaulDownloadInfo;
        }

        public static OverhaulDownloadInfo DownloadTexture(string uri, Action<OverhaulDownloadInfo> doneCallback)
        {
            OverhaulDownloadInfo overhaulDownloadInfo = new OverhaulDownloadInfo(doneCallback);
            StaticCoroutineRunner.StartStaticCoroutine(downloadTextureCoroutine(uri, overhaulDownloadInfo));
            return overhaulDownloadInfo;
        }

        public static void DownloadTexture(string uri, RawImage rawImageComponent)
        {
            OverhaulDownloadInfo overhaulDownloadInfo = DownloadTexture(uri, delegate(OverhaulDownloadInfo info)
            {
                if (!rawImageComponent || info.DownloadError)
                    return;

                if (rawImageComponent.mainTexture)
                    UnityEngine.Object.Destroy(rawImageComponent.texture);

                rawImageComponent.texture = info.DownloadResult.Texture;
            });
        }

        public static void DownloadText(string uri, Text textComponent)
        {
            OverhaulDownloadInfo overhaulDownloadInfo = DownloadData(uri, delegate (OverhaulDownloadInfo info)
            {
                if (!textComponent || info.DownloadError)
                    return;

                textComponent.text = info.DownloadResult.Text;
            });
        }

        private static IEnumerator downloadFileCoroutine(string uri, OverhaulDownloadInfo downloadInfo)
        {
            yield return null;

            using (UnityWebRequest request = UnityWebRequest.Get(uri))
            {
                downloadInfo.WebRequest = request;
                yield return request.SendWebRequest();

                downloadInfo.Error = request.isHttpError || request.isNetworkError;
                downloadInfo.ErrorString = request.error;
                downloadInfo.DownloadResult.ByteArray = request.downloadHandler.data;
                downloadInfo.DownloadResult.Text = request.downloadHandler.text;
            }
            downloadInfo.OnDownloadComplete();
            yield break;
        }

        private static IEnumerator downloadTextureCoroutine(string uri, OverhaulDownloadInfo downloadInfo)
        {
            yield return null;

            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(uri))
            {
                downloadInfo.WebRequest = request;
                yield return request.SendWebRequest();

                downloadInfo.Error = request.isHttpError || request.isNetworkError;
                downloadInfo.ErrorString = request.error;
                downloadInfo.DownloadResult.ByteArray = request.downloadHandler.data;
                downloadInfo.DownloadResult.Texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            }
            downloadInfo.OnDownloadComplete();
            yield break;
        }
    }
}
