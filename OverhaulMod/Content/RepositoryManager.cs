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
                doneCallback?.Invoke((string)obj);
            }, errorCallback, timeOut));
        }

        public void GetFile(string path, Action<byte[]> doneCallback, Action<string> errorCallback, out UnityWebRequest unityWebRequest, int timeOut = 20)
        {
            unityWebRequest = UnityWebRequest.Get(REPOSITORY_URL + path);
            _ = ModActionUtils.RunCoroutine(getFileCoroutine(unityWebRequest, false, delegate (object obj)
            {
                doneCallback?.Invoke((byte[])obj);
            }, errorCallback, timeOut));
        }

        public void GetTexture(string link, Action<Texture2D> doneCallback, Action<string> errorCallback, out UnityWebRequest unityWebRequest, int timeOut = 20)
        {
            unityWebRequest = UnityWebRequestTexture.GetTexture(REPOSITORY_URL + link);
            _ = ModActionUtils.RunCoroutine(getTextureCoroutine(unityWebRequest, doneCallback, errorCallback, timeOut));
        }

        public void GetCustomTextFile(string link, Action<string> doneCallback, Action<string> errorCallback, out UnityWebRequest unityWebRequest, int timeOut = 20)
        {
            unityWebRequest = UnityWebRequest.Get(link);
            _ = ModActionUtils.RunCoroutine(getFileCoroutine(unityWebRequest, true, delegate (object obj)
            {
                doneCallback?.Invoke((string)obj);
            }, errorCallback, timeOut));
        }

        public void GetCustomFile(string link, Action<byte[]> doneCallback, Action<string> errorCallback, out UnityWebRequest unityWebRequest, int timeOut = 20)
        {
            unityWebRequest = UnityWebRequest.Get(link);
            _ = ModActionUtils.RunCoroutine(getFileCoroutine(unityWebRequest, false, delegate (object obj)
            {
                doneCallback?.Invoke((byte[])obj);
            }, errorCallback, timeOut));
        }

        public void GetCustomTexture(string link, Action<Texture2D> doneCallback, Action<string> errorCallback, out UnityWebRequest unityWebRequest, int timeOut = 20)
        {
            unityWebRequest = UnityWebRequestTexture.GetTexture(link);
            _ = ModActionUtils.RunCoroutine(getTextureCoroutine(unityWebRequest, doneCallback, errorCallback, timeOut));
        }

        public void GetLocalTextFile(string path, Action<string> doneCallback, Action<string> errorCallback, out UnityWebRequest unityWebRequest)
        {
            unityWebRequest = UnityWebRequest.Get("file://" + path);
            _ = ModActionUtils.RunCoroutine(getFileCoroutine(unityWebRequest, true, delegate (object obj)
            {
                doneCallback?.Invoke((string)obj);
            }, errorCallback, -1));
        }

        public void GetLocalFile(string path, Action<byte[]> doneCallback, Action<string> errorCallback, out UnityWebRequest unityWebRequest)
        {
            unityWebRequest = UnityWebRequest.Get("file://" + path);
            _ = ModActionUtils.RunCoroutine(getFileCoroutine(unityWebRequest, false, delegate (object obj)
            {
                doneCallback?.Invoke((byte[])obj);
            }, errorCallback, -1));
        }

        public void GetLocalTexture(string path, Action<Texture2D> doneCallback, Action<string> errorCallback, out UnityWebRequest unityWebRequest)
        {
            unityWebRequest = UnityWebRequestTexture.GetTexture("file://" + path);
            _ = ModActionUtils.RunCoroutine(getTextureCoroutine(unityWebRequest, doneCallback, errorCallback, -1));
        }

        private IEnumerator getFileCoroutine(UnityWebRequest webRequest, bool returnText, Action<object> doneCallback, Action<string> errorCallback, int timeOut)
        {
            if (timeOut != -1)
                webRequest.timeout = timeOut;

            yield return webRequest.SendWebRequest();

            try
            {
                if (!webRequest.isNetworkError && !webRequest.isHttpError)
                {
                    if (returnText)
                        doneCallback?.Invoke(webRequest.downloadHandler.text);
                    else
                        doneCallback?.Invoke(webRequest.downloadHandler.data);
                }
                else
                {
                    errorCallback?.Invoke(webRequest.error);
                }
            }
            finally
            {
                webRequest.Dispose();
            }
            yield break;
        }

        private IEnumerator getTextureCoroutine(UnityWebRequest webRequest, Action<Texture2D> doneCallback, Action<string> errorCallback, int timeOut)
        {
            if (timeOut != -1)
                webRequest.timeout = timeOut;

            yield return webRequest.SendWebRequest();

            try
            {
                if (!webRequest.isNetworkError && !webRequest.isHttpError)
                {
                    doneCallback?.Invoke((webRequest.downloadHandler as DownloadHandlerTexture).texture);
                }
                else
                {
                    errorCallback?.Invoke(webRequest.error);
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
