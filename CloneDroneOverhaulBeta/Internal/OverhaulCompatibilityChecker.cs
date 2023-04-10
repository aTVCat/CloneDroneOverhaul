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
            if (!Equals(currentVersion, OverhaulVersion.GameTargetVersion))
            {
                DelegateScheduler.Instance.Schedule(showUnsupportedGameVersionDialogue, 1f);
            }
        }

        private static void showUnsupportedGameVersionDialogue()
        {
            OverhaulDialogues.CreateDialogueFromPreset(OverhaulDialoguePresetType.UnsupportedGameVersion);
        }
    }
}
