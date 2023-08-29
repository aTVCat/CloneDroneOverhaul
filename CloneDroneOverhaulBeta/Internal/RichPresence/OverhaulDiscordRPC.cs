using Discord;
using ModLibrary;
using System;
using UnityEngine;
using static Discord.UserManager;

namespace CDOverhaul.RichPresence
{
    public class OverhaulDiscordRPC : OverhaulRPCBase
    {        
        /// <summary>
        /// Overhaul mod Discord App ID
        /// </summary>
        public const long APP_ID = 1091373211163308073;
        public const CreateFlags CREATE_FLAG = CreateFlags.NoRequireDiscord;

        private Discord.Discord m_Client;
        private Activity m_Activity;
        private ActivityManager.UpdateActivityHandler m_ActivityHandler;

        public User localUser
        {
            get
            {
                if (!initialized)
                    return default;

                User user;
                try
                {
                    user = m_Client.GetUserManager().GetCurrentUser();
                }
                catch
                {
                    return default;
                }
                return user;
            }
        }

        public override void Start()
        {
            TryInitializeDiscord();
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            DisposeDiscordClient();
        }

        protected override void Update()
        {
            base.Update();
            if (!initialized)
                return;

            try
            {
                m_Client.RunCallbacks();
            }
            catch
            {
                Dispose(true);
                return;
            }
        }

        public override void RefreshInformation()
        {
            base.RefreshInformation();
            GetGameModeDetailsString();

            m_Activity.State = !string.IsNullOrEmpty(GameModeDetailsString) ? GameModeString + " [" + GameModeDetailsString + "]" : GameModeString;
            m_Client.GetActivityManager().UpdateActivity(m_Activity, m_ActivityHandler);
        }

        public void TryInitializeDiscord()
        {
            if (initialized)
                return;

            try
            {
                m_Client = new Discord.Discord(APP_ID, (ulong)CREATE_FLAG);
            }
            catch
            {
                return;
            }

            m_Activity = new Activity
            {
                Assets = new ActivityAssets
                {
                    LargeImage = "defaultimage"
                },
                ApplicationId = 1091373211163308073,
                Name = "Overhaul Mod",
                Details = "v" + OverhaulVersion.modVersion.ToString(),
                Type = ActivityType.Playing
            };
            m_ActivityHandler = new ActivityManager.UpdateActivityHandler(handleActivityUpdate);
            initialized = true;
        }

        public void DisposeDiscordClient()
        {
            if (!initialized)
                return;

            m_Client.Dispose();
        }

        private void handleActivityUpdate(Result res)
        {
            if (res != Result.Ok)
                DisposeDiscordClient();
        }
    }
}
