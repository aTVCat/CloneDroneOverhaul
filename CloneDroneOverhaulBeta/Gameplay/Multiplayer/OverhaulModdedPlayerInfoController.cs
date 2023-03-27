using System;

namespace CDOverhaul.Gameplay.Multiplayer
{
    public class OverhaulModdedPlayerInfoController : OverhaulGameplayController
    {
        public override void Initialize()
        {
            base.Initialize();
            GlobalEventManager.Instance.AddEventListener<IPlayerInfoState>(GlobalEvents.MultiplayerPlayerInfoStateAttached, new Action<IPlayerInfoState>(OnAttachedInfoState));
        }

        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover, bool hasInitializedModel)
        {
            if (!hasInitializedModel)
            {
                return;
            }

            DelegateScheduler.Instance.Schedule(delegate
            {
                if(firstPersonMover != null)
                {
                    _ = firstPersonMover.gameObject.AddComponent<PlayerStatusBehaviour>();
                }
            }, 0.2f);
        }

        public void OnAttachedInfoState(IPlayerInfoState state)
        {
            DelegateScheduler.Instance.Schedule(delegate
            {
                if(state != null)
                {
                    MultiplayerPlayerInfoState mstate = MultiplayerPlayerInfoManager.Instance.GetPlayerInfoState(state.PlayFabID);
                    if (mstate != null)
                    {
                        _ = mstate.gameObject.AddComponent<OverhaulModdedPlayerInfo>();
                    }
                }
            }, 0.1f);
        }

        public override string[] Commands()
        {
            throw new NotImplementedException();
        }

        public override string OnCommandRan(string[] command)
        {
            throw new NotImplementedException();
        }
    }
}
