using UnityEngine;

namespace CDOverhaul
{
    [RequireComponent(typeof(FirstPersonMover), typeof(UpgradeCollection), typeof(EnergySource))]
    public class OverhaulCharacterExpansion : OverhaulBehaviour
    {
        /// <summary>
        /// Owner of the expansion script
        /// </summary>
        public FirstPersonMover Owner
        {
            get;
            private set;
        }

        /// <summary>
        /// Owner's upgrade collection
        /// </summary>
        public UpgradeCollection UpgradeCollection
        {
            get;
            private set;
        }

        /// <summary>
        /// Owner's energy source
        /// </summary>
        public EnergySource EnergySource
        {
            get;
            private set;
        }

        public bool IsOwnerMainPlayer() => !IsDisposedOrDestroyed() && Owner != null && Owner.IsMainPlayer();
        public bool IsOwnerPlayer() => !IsDisposedOrDestroyed() && Owner != null && Owner.IsPlayer();

        public bool IsOwnerMultiplayerPlayer() => !IsDisposedOrDestroyed() && Owner != null && string.IsNullOrEmpty(Owner.GetPlayFabID());
        public bool IsOwnerMultiplayerNotMainPlayer() => !IsOwnerMainPlayer() && IsOwnerMultiplayerPlayer();

        public bool IsEnemy() => !IsOwnerPlayer() && !Owner.IsPlayerTeam;
        public bool IsAlly() => !IsEnemy();

        /// <summary>
        /// Check if user, if <paramref name="type"/> is 0 - pressed key this frame, 1 - holding the key, 2 - ended pressing key this frame
        /// </summary>
        /// <param name="code"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsPressed(KeyCode code, byte type)
        {
            bool result;
            switch (type)
            {
                case 0:
                    result = Input.GetKeyDown(code);
                    break;
                case 1:
                    result = Input.GetKey(code) && !IsPressed(code, 0);
                    break;
                case 2:
                    result = Input.GetKeyUp(code);
                    break;
                default:
                    return false;
            }
            return result;
        }

        protected override void OnDisposed()
        {
            Owner = null;
            UpgradeCollection = null;
            EnergySource = null;

            OverhaulEventsController.RemoveEventListener<Character>(GlobalEvents.CharacterKilled, onDeath, true);
            OverhaulEventsController.RemoveEventListener<FirstPersonMover>(GlobalEvents.UpgradesRefreshed, OnUpgradesRefresh, true);
        }

        public override void Start()
        {
            Owner = base.GetComponent<FirstPersonMover>();
            UpgradeCollection = base.GetComponent<UpgradeCollection>();
            EnergySource = base.GetComponent<EnergySource>();

            _ = OverhaulEventsController.AddEventListener<Character>(GlobalEvents.CharacterKilled, onDeath, true);
            _ = OverhaulEventsController.AddEventListener<FirstPersonMover>(GlobalEvents.UpgradesRefreshed, OnUpgradesRefresh, true);
        }

        /// <summary>
        /// May be the best way to control character movement
        /// </summary>
        /// <param name="command"></param>
        public virtual void OnPreCommandExecute(FPMoveCommand command) { }
        public virtual void OnPostCommandExecute(FPMoveCommand command) { }

        public virtual void OnPreAIUpdate(AISwordsmanController aiController, out bool continueExecution) { continueExecution = true; }
        public virtual void OnPostAIUpdate(AISwordsmanController aiController) { }

        public virtual void OnEvent(SendFallingEvent sendFallingEvent) { }

        private void onDeath(Character c)
        {
            if (IsDisposedOrDestroyed() || c.GetInstanceID() != Owner.GetInstanceID())
                return;

            OnDeath();
        }
        protected virtual void OnDeath() { }

        public void OnUpgradesRefresh(FirstPersonMover mover)
        {
            if (IsDisposedOrDestroyed() || mover.GetInstanceID() != Owner.GetInstanceID())
                return;

            OnRefresh();
        }
        protected virtual void OnRefresh() { }
    }
}