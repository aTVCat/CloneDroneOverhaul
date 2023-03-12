using ModLibrary;
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
            if(currentVersion != OverhaulVersion.GameTargetVersion)
            {/*
                try
                {
                    _ = new Generic2ButtonDialogue("Overhaul mod: Unsupported Clone Drone version!\n\nCurrent Overhaul mod version is made for version " + OverhaulVersion.GameTargetVersion + " of the game.\nThis may result bugs and crashes.\nYou may continue using the mod or delete one. Update the mod, if new mod version is out.", "Continue", null, "Mod site", delegate
                    {
                        Application.OpenURL("https://modbot.org/modPreview.html?modID=rAnDomPaTcHeS1");
                    });
                }
                catch
                {

                }*/
            }
        }
    }
}
