using System;
using System.Collections.Generic;

namespace CDOverhaul.Gameplay.Multiplayer
{
    public class OverhaulPlayerInfoController : OverhaulGameplayController
    {
        public const string PlayerInfoVersion = "V2";
        public const string PlayerInfoEventPrefix = "[OverhaulPlayerInfoEvent]@";

        public static readonly List<Tuple<Func<bool>, string>> UserFlags = new List<Tuple<Func<bool>, string>>();

        private static bool s_HasAddedUserFlags;

        public override void Initialize()
        {
            base.Initialize();
            GlobalEventManager.Instance.AddEventListener<IPlayerInfoState>(GlobalEvents.MultiplayerPlayerInfoStateAttached, onAttachedInfoState);

            if (!s_HasAddedUserFlags)
            {
                Tuple<Func<bool>, string> debugFlag = new Tuple<Func<bool>, string>(() => OverhaulVersion.IsDebugBuild, "debug");
                Tuple<Func<bool>, string> discordFlag = new Tuple<Func<bool>, string>(() => OverhaulDiscordController.HasInitialized, "discord");
                s_HasAddedUserFlags = true;
            }
        }

        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover, bool hasInitializedModel)
        {
            if (!hasInitializedModel)
                return;

            _ = firstPersonMover.gameObject.AddComponent<PlayerStatusBehaviour>();
        }

        private void onAttachedInfoState(IPlayerInfoState state)
        {
            DelegateScheduler.Instance.Schedule(delegate
            {
                MultiplayerPlayerInfoState playerInfoState = MultiplayerPlayerInfoManager.Instance.GetPlayerInfoState(state.PlayFabID);
                if (playerInfoState && !playerInfoState.IsDetached() && !playerInfoState.state.IsDisconnected)
                {
                    _ = playerInfoState.gameObject.AddComponent<OverhaulPlayerInfo>();
                }
            }, 3f);
        }

        public static string GetUserFlags()
        {
            string result = string.Empty;
            foreach(Tuple<Func<bool>, string> flag in UserFlags)
                if (flag.Item1 != null && flag.Item1() && !string.IsNullOrEmpty(flag.Item2))
                    result += flag.Item2 + " ";
            return result;
        }
    }
}
