namespace CDOverhaul
{
    public static class OverhaulGamemodeManager
    {
        private const string GunModID = "ee32ba1b-8c92-4f50-bdf4-400a14da829e";

        /// <summary>
        /// Check if we can see accessories on robots
        /// </summary>
        /// <returns></returns>
        public static bool SupportsPersonalization()
        {
            return true;
        }

        public static bool SupportsOutfits()
        {
            return !OverhaulVersion.Upd2Hotfix && SupportsPersonalization();
        }

        public static bool SupportsCombatOverhaul()
        {
            return !OverhaulVersion.Upd2Hotfix && !GameModeManager.IsMultiplayer();
        }

        public static bool SupportsBowSkins()
        {
            bool isEnabled = OverhaulMod.IsModEnabled(GunModID);
            return !isEnabled || (isEnabled && GameModeManager.IsMultiplayer());
        }
    }
}
