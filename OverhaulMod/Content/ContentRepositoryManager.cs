using OverhaulMod.Utils;
using System;
using System.Collections;
using UnityEngine.Networking;

namespace OverhaulMod.Content
{
    public class ContentRepositoryManager : Singleton<ContentRepositoryManager>
    {
        public const bool USE_METHOD_FOR_PRIVATE = false;

        private const string TOKEN = "";

        private const string LINK_PRIVATE = "";

        private const string LINK = "https://raw.githubusercontent.com/aTVCat/Overhaul-Mod-Content/main/";

#if DEBUG
        public string debugFileContents
        {
            get;
            set;
        }

        public string debugError
        {
            get;
            set;
        }

        public void DebugGetTextFile()
        {
            GetTextFileContent("OverhaulContent.txt", delegate (string contents)
            {
                debugFileContents = contents;
            }, delegate (string error)
            {
                debugError = error;
            }, out _);
        }
#endif

        public void GetTextFileContent(string path, Action<string> doneCallback, Action<string> errorCallback, out UnityWebRequest unityWebRequest, int timeOut = 15)
        {
            unityWebRequest = UnityWebRequest.Get(LINK + path);
            _ = ModActionUtils.RunCoroutine(getTextFileContentCoroutine(unityWebRequest, doneCallback, errorCallback, timeOut));
        }

        public void GetFileContent(string path, Action<byte[]> doneCallback, Action<string> errorCallback, out UnityWebRequest unityWebRequest, int timeOut = 15)
        {
            unityWebRequest = UnityWebRequest.Get(LINK + path);
            _ = ModActionUtils.RunCoroutine(getFileContentCoroutine(unityWebRequest, doneCallback, errorCallback, timeOut));
        }

        private IEnumerator getTextFileContentCoroutine(UnityWebRequest webRequest, Action<string> doneCallback, Action<string> errorCallback, int timeOut = 15)
        {
            webRequest.timeout = timeOut;
            if (USE_METHOD_FOR_PRIVATE)
            {
                webRequest.SetRequestHeader("Content-Type", "application/json");
                webRequest.SetRequestHeader("Authorization", "token " + TOKEN);
                webRequest.SetRequestHeader("Accept", "application/vnd.github.v3.raw");
            }
            yield return webRequest.SendWebRequest();

            if (!webRequest.isNetworkError && !webRequest.isHttpError)
                doneCallback?.Invoke(webRequest.downloadHandler.text);
            else
                errorCallback?.Invoke(webRequest.error);

            webRequest.Dispose();
            yield break;
        }

        private IEnumerator getFileContentCoroutine(UnityWebRequest webRequest, Action<byte[]> doneCallback, Action<string> errorCallback, int timeOut = 15)
        {
            webRequest.timeout = timeOut;
            if (USE_METHOD_FOR_PRIVATE)
            {
                webRequest.SetRequestHeader("Content-Type", "application/json");
                webRequest.SetRequestHeader("Authorization", "token " + TOKEN);
                webRequest.SetRequestHeader("Accept", "application/vnd.github.v3.raw");
            }
            yield return webRequest.SendWebRequest();

            if (!webRequest.isNetworkError && !webRequest.isHttpError)
                doneCallback?.Invoke(webRequest.downloadHandler.data);
            else
                errorCallback?.Invoke(webRequest.error);

            webRequest.Dispose();
            yield break;
        }
    }
}
