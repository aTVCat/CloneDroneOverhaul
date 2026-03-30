using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace OverhaulMod.Content
{
    public class RepositoryManager : Singleton<RepositoryManager>
    {
        public const string REPOSITORY_URL = "https://raw.githubusercontent.com/aTVCat/Overhaul-Mod-Content/main/";

        public void GetTextFile(string path, Action<string> doneCallback, Action<string> errorCallback, out UnityWebRequest unityWebRequest, int timeOut = 20)
        {
            unityWebRequest = UnityWebRequest.Get(REPOSITORY_URL + path);
            _ = ModActionUtils.RunCoroutine(getFileCoroutine(unityWebRequest, true, delegate (object obj)
            {
                if (doneCallback != null) doneCallback((string)obj);
            }, errorCallback, timeOut));
        }

        public void GetFile(string path, Action<byte[]> doneCallback, Action<string> errorCallback, out UnityWebRequest unityWebRequest, int timeOut = 20)
        {
            unityWebRequest = UnityWebRequest.Get(REPOSITORY_URL + path);
            _ = ModActionUtils.RunCoroutine(getFileCoroutine(unityWebRequest, false, delegate (object obj)
            {
                if (doneCallback != null) doneCallback((byte[])obj);
            }, errorCallback, timeOut));
        }

        public void GetTexture(string link, Action<Texture2D> doneCallback, Action<string> errorCallback, out UnityWebRequest unityWebRequest, int timeOut = 20, bool cache = false)
        {
            string fullUrl = REPOSITORY_URL + link;
            ModDownloadCacheManager downloadCacheManager = ModDownloadCacheManager.Instance;
            if (cache && downloadCacheManager.HasCachedDownload(fullUrl))
            {
                GetLocalTexture(downloadCacheManager.GetPathOfDownload(fullUrl), doneCallback, errorCallback, out unityWebRequest);
                return;
            }

            unityWebRequest = UnityWebRequestTexture.GetTexture(fullUrl);
            _ = ModActionUtils.RunCoroutine(getTextureCoroutine(unityWebRequest, doneCallback, errorCallback, timeOut, cache));
        }

        public void GetCustomTextFile(string link, Action<string> doneCallback, Action<string> errorCallback, out UnityWebRequest unityWebRequest, int timeOut = 20)
        {
            unityWebRequest = UnityWebRequest.Get(link);
            _ = ModActionUtils.RunCoroutine(getFileCoroutine(unityWebRequest, true, delegate (object obj)
            {
                if (doneCallback != null) doneCallback((string)obj);
            }, errorCallback, timeOut));
        }

        public void GetCustomFile(string link, Action<byte[]> doneCallback, Action<string> errorCallback, out UnityWebRequest unityWebRequest, int timeOut = 20)
        {
            unityWebRequest = UnityWebRequest.Get(link);
            _ = ModActionUtils.RunCoroutine(getFileCoroutine(unityWebRequest, false, delegate (object obj)
            {
                if (doneCallback != null) doneCallback((byte[])obj);
            }, errorCallback, timeOut));
        }

        public void GetCustomTexture(string link, Action<Texture2D> doneCallback, Action<string> errorCallback, out UnityWebRequest unityWebRequest, int timeOut = 20, bool cache = false)
        {
            ModDownloadCacheManager downloadCacheManager = ModDownloadCacheManager.Instance;
            if (cache && downloadCacheManager.HasCachedDownload(link))
            {
                GetLocalTexture(downloadCacheManager.GetPathOfDownload(link), doneCallback, errorCallback, out unityWebRequest);
                return;
            }

            unityWebRequest = UnityWebRequestTexture.GetTexture(link);
            _ = ModActionUtils.RunCoroutine(getTextureCoroutine(unityWebRequest, doneCallback, errorCallback, timeOut, cache));
        }

        public void GetLocalTextFile(string path, Action<string> doneCallback, Action<string> errorCallback, out UnityWebRequest unityWebRequest)
        {
            unityWebRequest = UnityWebRequest.Get("file://" + path);
            _ = ModActionUtils.RunCoroutine(getFileCoroutine(unityWebRequest, true, delegate (object obj)
            {
                if (doneCallback != null) doneCallback((string)obj);
            }, errorCallback, -1));
        }

        public void GetLocalFile(string path, Action<byte[]> doneCallback, Action<string> errorCallback, out UnityWebRequest unityWebRequest)
        {
            unityWebRequest = UnityWebRequest.Get("file://" + path);
            _ = ModActionUtils.RunCoroutine(getFileCoroutine(unityWebRequest, false, delegate (object obj)
            {
                if (doneCallback != null) doneCallback((byte[])obj);
            }, errorCallback, -1));
        }

        public void GetLocalTexture(string path, Action<Texture2D> doneCallback, Action<string> errorCallback, out UnityWebRequest unityWebRequest)
        {
            unityWebRequest = UnityWebRequestTexture.GetTexture("file://" + path);
            _ = ModActionUtils.RunCoroutine(getTextureCoroutine(unityWebRequest, doneCallback, errorCallback, -1, false));
        }

        private IEnumerator getFileCoroutine(UnityWebRequest webRequest, bool returnText, Action<object> doneCallback, Action<string> errorCallback, int timeOut)
        {
            if (timeOut != -1) webRequest.timeout = timeOut;

            yield return webRequest.SendWebRequest();

            try
            {
                if (!webRequest.isNetworkError && !webRequest.isHttpError)
                {
                    if (doneCallback != null)
                    {
                        if (returnText)
                            doneCallback(webRequest.downloadHandler.text);
                        else
                            doneCallback(webRequest.downloadHandler.data);
                    }
                }
                else
                {
                    if (errorCallback != null) errorCallback(webRequest.error);
                }
            }
            finally
            {
                webRequest.Dispose();
            }
            yield break;
        }

        private IEnumerator getTextureCoroutine(UnityWebRequest webRequest, Action<Texture2D> doneCallback, Action<string> errorCallback, int timeOut, bool cache)
        {
            if (timeOut != -1) webRequest.timeout = timeOut;

            yield return webRequest.SendWebRequest();

            try
            {
                if (!webRequest.isNetworkError && !webRequest.isHttpError)
                {
                    if (cache)
                    {
                        string file = ModDownloadCacheManager.Instance.GetPathOfDownload(webRequest.url);
                        ModFileUtils.WriteBytes(webRequest.downloadHandler.data, file);
                    }

                    if (doneCallback != null) doneCallback((webRequest.downloadHandler as DownloadHandlerTexture).texture);
                }
                else
                {
                    if (errorCallback != null) errorCallback(webRequest.error);
                }
            }
            finally
            {
                webRequest.Dispose();
            }
            yield break;
        }
    }
}
