using System;
using System.Collections;
using System.Net;

namespace CloneDroneOverhaul
{
    public class UpdateController
    {
        public static void OpenGitHubWithReleases()
        {
            BaseUtils.OpenURL("https://github.com/aTVCat/CloneDroneOverhaul/releases");
        }
        public static void OpenModBotPage()
        {
            BaseUtils.OpenURL("https://modbot.org/modPreview.html?modID=rAnDomPaTcHeS1");
        }

        public static void CheckForUpdates(Action<Version> onReceivedVersion) //https://raw.githubusercontent.com/aTVCat/CloneDroneOverhaul/update-preview/Version.txt
        {
            StaticCoroutineRunner.StartStaticCoroutine(checkForUpdate(onReceivedVersion));
        }

        private static IEnumerator checkForUpdate(Action<Version> onReceivedVersion)
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
                    onReceivedVersion(ToVersion(version));
                }
                yield break;
            }
            yield break;
        }

        public static Version ToVersion(string raw)
        {
            raw = raw.Replace("a", string.Empty);
            int milestone = 0, major = 0, minor = 0, build = 0;

            string[] tokens = raw.Split('.');

            if (tokens.Length > 0)
            {
                int.TryParse(tokens[0], out milestone);

                if (tokens.Length > 1)
                {
                    int.TryParse(tokens[1], out major);

                    if (tokens.Length > 2)
                    {
                        int.TryParse(tokens[2], out minor);

                        if (tokens.Length > 3)
                        {
                            int.TryParse(tokens[3], out build);
                        }
                    }
                }
            }
            return new Version(milestone, major, minor, build);
        }
    }
}
