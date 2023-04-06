using UnityEngine;

namespace CDOverhaul
{
    internal class OverhaulCompatibilityChecker
    {
        private static bool m_HasNotifiedPlayerAboutUnsupportedVersion;

        public static void CheckGameVersion()
        {
            if (m_HasNotifiedPlayerAboutUnsupportedVersion)
            {
                return;
            }

            m_HasNotifiedPlayerAboutUnsupportedVersion = true;
            string currentVersion = VersionNumberManager.Instance.GetVersionString();
            if (currentVersion != OverhaulVersion.GameTargetVersion)
            {
                DelegateScheduler.Instance.Schedule(delegate
                {
                    OverhaulDialogues.Create2BDialogue("Unsupported Clone Drone version!", "Current Overhaul mod version is made for version " + OverhaulVersion.GameTargetVersion + " of the game.\nThis may result bugs and crashes.\nYou may continue using the mod or delete one. It is better to update the mod", 24f, "Ok", null, "Visit site", delegate
                    {
                        UnityEngine.Application.OpenURL("https://modbot.org/modPreview.html?modID=rAnDomPaTcHeS1");
                    }, new Vector2(330, 145));
                }, 1f);
            }
        }
    }
}
