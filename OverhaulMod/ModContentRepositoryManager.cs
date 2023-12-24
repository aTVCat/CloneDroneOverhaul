using OverhaulMod.Utils;
using System;
using System.Collections;
using UnityEngine.Networking;

namespace OverhaulMod
{
    public class ModContentRepositoryManager : Singleton<ModContentRepositoryManager>
    {
        public const bool USE_METHOD_FOR_PRIVATE = false;

        private const string TOKEN = "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";

        private const string LINK_PRIVATE = "https://api.github.com/repos/aTVCat/Overhaul-Mod-Content/contents/";

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
            });
        }
#endif

        public void GetTextFileContent(string path, Action<string> doneCallback, Action<string> errorCallback, int timeOut = 15)
        {
            if (!USE_METHOD_FOR_PRIVATE)
            {
                _ = ModActionUtils.RunCoroutine(getTextFileContentCoroutine(LINK + path, doneCallback, errorCallback, timeOut));
                return;
            }
            _ = ModActionUtils.RunCoroutine(getTextFileContentCoroutine(LINK_PRIVATE + path, doneCallback, errorCallback, timeOut));
        }

        private IEnumerator getTextFileContentCoroutine(string link, Action<string> doneCallback, Action<string> errorCallback, int timeOut = 15)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(link))
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
            }
            yield break;
        }
    }
}
