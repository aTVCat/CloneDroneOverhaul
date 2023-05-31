namespace CDOverhaul
{
    internal class OverhaulCompatibilityChecker
    {
        public static void CheckGameVersion()
        {
            if (!OverhaulSessionController.GetKey<bool>("HasNotifiedPlayerAboutUnsupportedVersion"))
            {
                OverhaulSessionController.SetKey("HasNotifiedPlayerAboutUnsupportedVersion", true);

                string currentVersion = VersionNumberManager.Instance.GetVersionString();
                if (!Equals(currentVersion, OverhaulVersion.GameTargetVersion))
                    DelegateScheduler.Instance.Schedule(showUnsupportedGameVersionDialogue, 1f);
            }
        }

        private static void showUnsupportedGameVersionDialogue() => OverhaulDialogues.CreateDialogueFromPreset(OverhaulDialoguePresetType.UnsupportedGameVersion);
    }
}
