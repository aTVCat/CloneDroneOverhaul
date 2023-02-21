using UnityEngine;

namespace CDOverhaul.Gameplay.Combat_Update
{
    public class FirstPersonMoverInteractionBehaviour : FirstPersonMoverExtention
    {
        protected override void Initialize(FirstPersonMover owner)
        {
        }

        public override void OnAIUpdate(in AISwordsmanController controller)
        {
        }

        public void On2SwordsCollided(MeleeImpactArea we, MeleeImpactArea other)
        {
            TryHoldWeaponHit(other);
        }

        public void TryHoldWeaponHit(MeleeImpactArea other)
        {
            //Todo: restric robots from moving
        }
    }
}