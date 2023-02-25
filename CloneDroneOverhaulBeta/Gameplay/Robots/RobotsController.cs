using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class RobotsController : OverhaulController
    {
        public override string[] Commands()
        {
            throw new System.NotImplementedException();
        }

        public override void Initialize()
        {
            _ = OverhaulEventManager.AddEventListener<FirstPersonMover>(OverhaulGameplayCoreController.FirstPersonMoverSpawnedEventString, onFPMSpawned);
            _ = OverhaulEventManager.AddEventListener<FirstPersonMover>(OverhaulGameplayCoreController.PlayerSetAsFirstPersonMover, tryFixFPMForPlayer);
        }

        public override string OnCommandRan(string[] command)
        {
            throw new System.NotImplementedException();
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
