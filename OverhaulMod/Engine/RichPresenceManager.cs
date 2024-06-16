using OverhaulMod.Utils;

namespace OverhaulMod.Engine
{
    public class RichPresenceManager : Singleton<RichPresenceManager>
    {
        [ModSettingRequireRestart]
        [ModSetting(ModSettingsConstants.ENABLE_RPC, true)]
        public static bool EnableRichPresence;

        [ModSetting(ModSettingsConstants.RPC_DETAILS, true)]
        public static bool RichPresenceDetails;

        [ModSetting(ModSettingsConstants.RPC_DISPLAY_LEVEL_FILE_NAME, false)]
        public static bool RichPresenceDisplayLevelFileName;

        public RichPresenceDiscord discord
        {
            get;
            private set;
        }

        private void Start()
        {
            if (EnableRichPresence)
                discord = base.gameObject.AddComponent<RichPresenceDiscord>();
        }
    }
}
