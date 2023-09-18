namespace CDOverhaul.RichPresence
{
    public class OverhaulRPCManager : OverhaulManager<OverhaulRPCManager>
    {
        [OverhaulSettingWithNotification(1)]
        [OverhaulSettingAttribute_Old("Mod.Information.Rich Presence", true)]
        public static bool EnableRPC = true;

        public OverhaulDiscordRPC discord
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
        }
    }
}
