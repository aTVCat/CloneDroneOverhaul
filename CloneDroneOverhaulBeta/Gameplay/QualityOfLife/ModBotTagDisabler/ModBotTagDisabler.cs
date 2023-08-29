namespace CDOverhaul.Gameplay.QualityOfLife
{
    public class ModBotTagDisabler : OverhaulGameplaySystem
    {
        [OverhaulSetting("QoL.Multiplayer.Disable Mod-Bot tags", false)]
        public static bool DisableTags;
    }
}
