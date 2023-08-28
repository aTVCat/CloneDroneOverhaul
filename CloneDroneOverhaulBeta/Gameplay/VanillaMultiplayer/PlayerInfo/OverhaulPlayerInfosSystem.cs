using System;
using System.Collections.Generic;
using System.Text;

namespace CDOverhaul.Gameplay.Multiplayer
{
    public class OverhaulPlayerInfosSystem : OverhaulGameplaySystem
    {
        public const string VERSION = "V3";
        public const string EVENT_PREFIX = "[OverhaulPlayerInfoEvent]@";

        public static readonly List<Tuple<Func<bool>, string>> UserFlags = new List<Tuple<Func<bool>, string>>();

        public override void Start()
        {
            base.Start();
            Tuple<Func<bool>, string> debugFlag = new Tuple<Func<bool>, string>(() => OverhaulVersion.IsDebugBuild, "debug ");
            Tuple<Func<bool>, string> discordFlag = new Tuple<Func<bool>, string>(() => OverhaulDiscordController.HasInitialized, "discord ");
            //Tuple<Func<bool>, string> testerFlag = new Tuple<Func<bool>, string>(() => OverhaulDiscordController.HasInitialized, "tester ");
        }

        public override void AddListeners()
        {
            base.AddListeners();
            OverhaulEventsController.AddEventListener<IPlayerInfoState>(GlobalEvents.MultiplayerPlayerInfoStateAttached, onAttachedInfoState, true);
            OverhaulDebug.Log("InfosSystem - AddedListeners", EDebugType.Initialize);
        }

        public override void RemoveListeners()
        {
            base.RemoveListeners();
            OverhaulEventsController.RemoveEventListener<IPlayerInfoState>(GlobalEvents.MultiplayerPlayerInfoStateAttached, onAttachedInfoState, true);
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
                    playerInfoState.gameObject.AddComponent<OverhaulPlayerInfo>().Initialize(playerInfoState);
                }
            }, 3f);
        }

        public static string GetUserFlags()
        {
            StringBuilder builder = new StringBuilder();
            foreach (Tuple<Func<bool>, string> flag in UserFlags)
                if (flag.Item1 != null && flag.Item1() && !string.IsNullOrEmpty(flag.Item2))
                    builder.Append(flag);

            return builder.ToString();
        }
    }
}
