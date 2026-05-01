using OverhaulMod.Utils;

namespace OverhaulMod.Engine
{
    public class RichPresenceManager : Singleton<RichPresenceManager>
    {
        [ModSettingRequireRestart]
        [ModSetting(ModSettingIDs.ENABLE_RPC, true)]
        public static bool EnableRichPresence;

        [ModSetting(ModSettingIDs.RPC_DETAILS, true)]
        public static bool RichPresenceDetails;

        [ModSetting(ModSettingIDs.RPC_DISPLAY_LEVEL_FILE_NAME, false)]
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
