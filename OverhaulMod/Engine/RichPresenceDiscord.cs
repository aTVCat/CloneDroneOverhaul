using Discord;

namespace OverhaulMod.Engine
{
    public class RichPresenceDiscord : RichPresenceBase
    {
        /// <summary>
        /// Overhaul mod Discord App ID
        /// </summary>
        public const long APP_ID = 1091373211163308073;
        public const CreateFlags CREATE_FLAG = CreateFlags.NoRequireDiscord;

        private Discord.Discord m_client;
        private Activity? m_activity;
        private ActivityManager.UpdateActivityHandler m_activityHandler;

        public override void Start()
        {
            TryInitializeDiscord();
        }

        public override void OnDestroy()
        {
            DisposeDiscordClient();
        }

        private void OnApplicationQuit()
        {
            DisposeDiscordClient();
        }

        public override void Update()
        {
            base.Update();
            try
            {
                if (m_client != null)
                    m_client.RunCallbacks();
            }
            catch
            {
            }
        }

        public override void RefreshInformation()
        {
            base.RefreshInformation();
            if (m_client != null && m_activity != null)
            {
                ActivityManager manager = m_client.GetActivityManager();
                if (manager == null)
                    return;


                m_activity.value.Details = $"v{ModBuildInfo.version} · {gameModeString}";
                m_activity.value.State = !string.IsNullOrEmpty(gameModeDetailsString) ? gameModeDetailsString : string.Empty;
                manager.UpdateActivity(m_activity.value, m_activityHandler);
            }
        }

        public void TryInitializeDiscord()
        {
            if (m_client == null)
            {
                try
                {
                    m_client = new Discord.Discord(APP_ID, (ulong)CREATE_FLAG);
                }
                catch { }
            }

            if (m_activity == null)
                m_activity = new Activity
                {
                    Assets = new ActivityAssets
                    {
                        LargeImage = "defaultimage"
                    },
                    ApplicationId = 1091373211163308073,
                    Name = "Overhaul Mod",
                    Type = ActivityType.Playing
                };

            if (m_activityHandler == null)
                m_activityHandler = new ActivityManager.UpdateActivityHandler(handleActivityUpdate);
        }

        public void DisposeDiscordClient()
        {
            try
            {
                if (m_client != null)
                {
                    m_client.Dispose();
                    m_client = null;
                }
            }
            catch
            {

            }
        }

        private void handleActivityUpdate(Result res)
        {
            if (res != Result.Ok)
                Destroy(this);
        }
    }
}
