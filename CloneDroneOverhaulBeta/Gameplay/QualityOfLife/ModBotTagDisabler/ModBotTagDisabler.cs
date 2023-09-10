namespace CDOverhaul.Gameplay.QualityOfLife
{
    public class ModBotTagDisabler : OverhaulGameplaySystem
    {
        [OverhaulSettingAttribute_Old("QoL.Multiplayer.Disable Mod-Bot tags", false)]
        public static bool DisableTags;
    }
}
