namespace CDOverhaul
{
    public static class OverhaulGamemodeManager
    {
        /// <summary>
        /// Check if we can see accessories on robots
        /// </summary>
        /// <returns></returns>
        public static bool SupportsAccessories()
        {
            return !GameModeManager.Is(GameMode.Story);
        }
    }
}
