using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;
/*
using DiscordRPC;
using DiscordRPC.Logging;
*/

namespace CDOverhaul
{
    internal class OverhaulDiscordRPCController : OverhaulBehaviour
    {
        public static OverhaulDiscordRPCController Instance;

        private Discord.Discord m_Client;
        private Discord.Activity m_ClientActivity;

        private static bool m_HasInitialized;

        private void Start()
        {
            if (!m_HasInitialized)
            {
                ActivityAssets actAssets = new ActivityAssets();
                actAssets.LargeImage = "clonedroneoverhaulimage03placeholder";

                m_ClientActivity = new Activity();
                m_ClientActivity.Assets = actAssets;
                m_ClientActivity.ApplicationId = 1091373211163308073;
                m_ClientActivity.Name = "Overhaul mod";
                m_ClientActivity.Details = "Version " + OverhaulVersion.ModVersion.ToString() + OverhaulVersion.DebugString;
                m_ClientActivity.Type = ActivityType.Playing;

                m_Client = new Discord.Discord(1091373211163308073, (UInt64)global::Discord.CreateFlags.Default);
                m_HasInitialized = true;
            }
            m_Client.GetActivityManager().UpdateActivity(m_ClientActivity, null);

            OverhaulEventManager.AddEventListener(Gameplay.OverhaulGameplayCoreController.GamemodeChangedEventString, onGamemodeChanged);
            OverhaulEventManager.AddEventListener(OverhaulMod.ModDeactivatedEventString, DestroyDiscord);

            Instance = this;
        }

        private void Update()
        {
            if(m_Client != null)
            {
                m_Client.RunCallbacks();
            }
        }

        private void OnApplicationQuit()
        {
            DestroyDiscord();
        }

        private void onGamemodeChanged()
        {
            if (m_Client != null)
            {
                GameMode gm = GameFlowManager.Instance.GetCurrentGameMode();
                string gamemodeString = "In Menus";

                switch (gm)
                {
                    case GameMode.Adventure:
                        gamemodeString = "Playing an adventure level";
                        break;
                    case GameMode.BattleRoyale:
                        gamemodeString = "Playing LBS" + ((BattleRoyaleManager.Instance != null && BattleRoyaleManager.Instance.IsPrivateMatch()) ? " (Private)" : string.Empty);
                        break;
                    case GameMode.Challenge:
                        gamemodeString = "Playing a challenge";
                        break;
                    case GameMode.CoopChallenge:
                        gamemodeString = "Playing a coop challenge";
                        break;
                    case GameMode.Endless:
                        gamemodeString = "Playing endless mode";
                        break;
                    case GameMode.EndlessCoop:
                        gamemodeString = "Playing coop endless mode";
                        break;
                    case GameMode.LevelEditor:
                        gamemodeString = "In level editor";
                        break;
                    case GameMode.MultiplayerDuel:
                        gamemodeString = "Playing duel";
                        break;
                    case GameMode.SingleLevelPlaytest:
                        gamemodeString = "In level editor (playtest)";
                        break;
                    case GameMode.Story:
                        gamemodeString = "Playing story mode";
                        break;
                    case GameMode.Twitch:
                        gamemodeString = "Twich mode";
                        break;
                }

                m_ClientActivity.State = gamemodeString;
                m_Client.GetActivityManager().UpdateActivity(m_ClientActivity, null);
            }
        }

        public void DestroyDiscord()
        {
            m_HasInitialized = false;
            if (m_Client != null)
            {
                m_Client.Dispose();
            }
        }
    }
}
