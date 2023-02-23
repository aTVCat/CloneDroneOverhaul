using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class RobotsController : ModController
    {
        public override void Initialize()
        {
            _ = OverhaulEventManager.AddEventListener<FirstPersonMover>(MainGameplayController.FirstPersonMoverSpawnedEventString, onFPMSpawned);
            _ = OverhaulEventManager.AddEventListener<FirstPersonMover>(MainGameplayController.PlayerSetAsFirstPersonMover, tryFixFPMForPlayer);

            HasAddedEventListeners = true;
            HasInitialized = true;
        }

        private void onFPMSpawned(FirstPersonMover mover)
        {
            if (mover == null)
            {
                return;
            }
            _ = mover.gameObject.AddComponent<FirstPersonMoverData>();
            _ = mover.gameObject.AddComponent<FirstPersonMoverOverhaul>();
        }

        private void tryFixFPMForPlayer(FirstPersonMover mover)
        {
            if (mover == null)
            {
                return;
            }
            mover.MaxSpeed = Mathf.Clamp(mover.MaxSpeed, 12f, float.PositiveInfinity);
            if (mover.IsUsingMagBoots())
            {
                mover.OverrideBaseMoveSpeed(8f);
            }

            /*
            AdvancedWeaponController weaponController = FirstPersonMoverExtention.GetExtention<AdvancedWeaponController>(mover);
            if (weaponController != null)
            {
                weaponController.SetDefaultSpeed(mover.MaxSpeed);
            }*/
        }
    }
}
