using OverhaulMod.Utils;
using System;
using System.Collections;
using UnityEngine.Networking;

namespace OverhaulMod
{
    public class ModContentRepositoryManager : Singleton<ModContentRepositoryManager>
    {
        private const string TOKEN = "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";

        private const string LINK = "https://api.github.com/repos/aTVCat/Overhaul-Mod-Content/contents/";

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
            _ = ModActionUtils.RunCoroutine(getTextFileContentCoroutine(LINK + path, doneCallback, errorCallback, timeOut));
        }

        private IEnumerator getTextFileContentCoroutine(string link, Action<string> doneCallback, Action<string> errorCallback, int timeOut = 15)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(link))
            {
                webRequest.timeout = timeOut;
                webRequest.SetRequestHeader("Authorization", "token " + TOKEN);
                webRequest.SetRequestHeader("Accept", "application/vnd.github.v3.raw");
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
