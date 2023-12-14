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

        public void GetTextFileContent(string path, Action<string> doneCallback, Action<string> errorCallback)
        {
            _ = ModActionUtils.RunCoroutine(getTextFileContentCoroutine(LINK + path, doneCallback, errorCallback));
        }

        private IEnumerator getTextFileContentCoroutine(string link, Action<string> doneCallback, Action<string> errorCallback)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(link))
            {
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
