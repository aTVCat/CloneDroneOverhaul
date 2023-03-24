using System;

namespace CDOverhaul.Gameplay.Multiplayer
{
    public class OverhaulModdedPlayerInfoController : OverhaulController
    {
        public override void Initialize()
        {
            GlobalEventManager.Instance.AddEventListener<IPlayerInfoState>(GlobalEvents.MultiplayerPlayerInfoStateAttached, new Action<IPlayerInfoState>(OnAttachedInfoState));
        }

        public void OnAttachedInfoState(IPlayerInfoState state)
        {
            DelegateScheduler.Instance.Schedule(delegate
            {
                MultiplayerPlayerInfoState mstate = MultiplayerPlayerInfoManager.Instance.GetPlayerInfoState(state.PlayFabID);
                if(mstate != null)
                {
                    _ = mstate.gameObject.AddComponent<OverhaulModdedPlayerInfo>();
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
