namespace CDOverhaul
{
    public static class OverhaulGamemodeManager
    {
        /// <summary>
        /// Check if we can see accessories on robots
        /// </summary>
        /// <returns></returns>
        public static bool SupportsOutfits()
        {
            return !GameModeManager.Is(GameMode.Story) && !GameModeManager.ConsciousnessTransferToAlliesEnabled() && !GameModeManager.ConsciousnessTransferToKillerEnabled();
        }

        public static bool SupportsCombatOverhaul()
        {
            return !GameModeManager.IsMultiplayer();
        }
    }
}
