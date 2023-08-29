namespace CDOverhaul.RichPresence
{
    public class OverhaulRPCManager : OverhaulManager<OverhaulRPCManager>
    {
        // Todo: make more settings
        [OverhaulSettingWithNotification(1)]
        [OverhaulSetting("Mod.Information.Rich Presence", true)]
        public static bool EnableRPC;

        public OverhaulDiscordRPC discord
        {
            get;
            private set;
        }

        public OverhaulSteamRPC steam
        {
            get;
            private set;
        }

        public override void Start()
        {
            base.Start();
            if (!EnableRPC)
                return;

            discord = base.gameObject.AddComponent<OverhaulDiscordRPC>();
            steam = base.gameObject.AddComponent<OverhaulSteamRPC>();
        }
    }
}
