using UnityEngine;

namespace CDOverhaul
{
    [RequireComponent(typeof(FirstPersonMover), typeof(UpgradeCollection), typeof(EnergySource))]
    public class OverhaulCharacterExpansion : OverhaulBehaviour
    {
        /// <summary>
        /// Owner of the expansion script
        /// </summary>
        public FirstPersonMover FirstPersonMover
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

        public bool IsOwnerMainPlayer() => !IsDisposedOrDestroyed() && FirstPersonMover.IsMainPlayer();
        public bool IsOwnerPlayer() => !IsDisposedOrDestroyed() && FirstPersonMover.IsPlayer();
        public bool IsOwnerMultiplayerPlayer() => !IsDisposedOrDestroyed() && string.IsNullOrEmpty(FirstPersonMover.GetPlayFabID());
        public bool IsOwnerMultiplayerNotMainPlayer() => !IsOwnerMainPlayer() && IsOwnerMultiplayerPlayer();
        public bool IsEnemy() => !IsOwnerPlayer() && !FirstPersonMover.IsPlayerTeam;
        public bool IsAlly() => !IsOwnerPlayer() && FirstPersonMover.IsPlayerTeam;

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
            FirstPersonMover = null;
            UpgradeCollection = null;
            EnergySource = null;

            OverhaulEventManager.RemoveEventListener<Character>(GlobalEvents.CharacterKilled, onDeath, true);
            OverhaulEventManager.RemoveEventListener<FirstPersonMover>(GlobalEvents.UpgradesRefreshed, onUpgradesRefresh, true);
        }

        public override void Start()
        {
            FirstPersonMover = base.GetComponent<FirstPersonMover>();
            UpgradeCollection = base.GetComponent<UpgradeCollection>();
            EnergySource = base.GetComponent<EnergySource>();

            _ = OverhaulEventManager.AddEventListener<Character>(GlobalEvents.CharacterKilled, onDeath, true);
            _ = OverhaulEventManager.AddEventListener<FirstPersonMover>(GlobalEvents.UpgradesRefreshed, onUpgradesRefresh, true);
        }

        /// <summary>
        /// May be the best way to control character movement
        /// </summary>
        /// <param name="command"></param>
        public virtual void OnPreCommandExecute(FPMoveCommand command)
        {

        }

        public virtual void OnPostCommandExecute(FPMoveCommand command)
        {

        }

        public virtual void OnPreAIUpdate(AISwordsmanController aiController, out bool continueExecution)
        {
            continueExecution = true;
        }

        public virtual void OnPostAIUpdate(AISwordsmanController aiController)
        {

        }

        public virtual void OnEvent(SendFallingEvent sendFallingEvent)
        {

        }

        private void onDeath(Character c)
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }
            if(c.GetInstanceID() != FirstPersonMover.GetInstanceID())
            {
                return;
            }
            OnDeath();
        }
        protected virtual void OnDeath()
        {

        }

        private void onUpgradesRefresh(FirstPersonMover mover)
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }
            if(mover.GetInstanceID() != FirstPersonMover.GetInstanceID())
            {
                return;
            }
            OnRefresh();
        }
        protected virtual void OnRefresh()
        {

        }
    }
}