using CDOverhaul.Gameplay.Multiplayer;

namespace CDOverhaul.Gameplay
{
    public abstract class PersonalizationItemsWearer : OverhaulCharacterExpansion
    {
        public OverhaulPlayerInfo PlayerInformation
        {
            get => OverhaulPlayerInfo.GetOverhaulPlayerInfo(Owner);
        }

        public override void Start()
        {
            base.Start();
            OverhaulEventsController.AddEventListener<string>(OverhaulPlayerInfo.PlayerDataUpdateEventString, RefreshItemsMultiplayer);

            // Repair upgrade fix
            if (GameModeManager.IsSinglePlayer() && (Owner.IsPlayer() || Owner.IsPlayerTeam))
            {
                DelegateScheduler.Instance.Schedule(delegate
                {
                    if (!IsDisposedOrDestroyed())
                    {
                        RefreshItems();
                    }
                }, 4.5f);
            }
        }

        protected override void OnDisposed()
        {
            OverhaulEventsController.RemoveEventListener<string>(OverhaulPlayerInfo.PlayerDataUpdateEventString, RefreshItemsMultiplayer);
        }

        public abstract void RefreshItems();
        public virtual void RefreshItemsMultiplayer(string playFabID)
        {
            if (Owner && Owner.IsAlive() && Owner.GetPlayFabID() == playFabID)
                RefreshItems();
        }
    }
}
