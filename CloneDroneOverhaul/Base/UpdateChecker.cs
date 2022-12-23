using System;
using System.Collections;
using System.Net;

namespace CloneDroneOverhaul
{
    public class UpdateChecker
    {
        public static void OpenGitHubWithReleases()
        {
            BaseUtils.OpenURL("https://github.com/aTVCat/CloneDroneOverhaul/releases");
        }
        public static void OpenModBotPage()
        {
            BaseUtils.OpenURL("https://modbot.org/modPreview.html?modID=rAnDomPaTcHeS1");
        }

        public static void CheckForUpdates(Action<string> onReceivedVersion) //https://raw.githubusercontent.com/aTVCat/CloneDroneOverhaul/update-preview/Version.txt
        {
            StaticCoroutineRunner.StartStaticCoroutine(checkForUpdate(onReceivedVersion));
        }

        private static IEnumerator checkForUpdate(Action<string> onReceivedVersion)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add("a", "a");
                wc.DownloadFileAsync(new Uri("https://raw.githubusercontent.com/aTVCat/CloneDroneOverhaul/update-preview/Version.txt"), Modules.ModDataManager.Mod_TempFolder + "Version.txt");
                while (wc.IsBusy)
                {
                    yield return new UnityEngine.WaitForEndOfFrame();
                }

                string version = System.IO.File.ReadAllText(Modules.ModDataManager.Mod_TempFolder + "Version.txt").TrimEnd(new char[] { char.Parse("\n"), char.Parse(" ") });
                if (onReceivedVersion != null)
                {
                    onReceivedVersion(version);
                }
                yield break;
            }
            yield break;
        }
    }
}
