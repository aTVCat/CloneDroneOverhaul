using System.Collections;
using UnityEngine;

namespace CDOverhaul.Gameplay.Combat
{
    public class CombatOverhaulInventoryController : OverhaulGameplayController
    {
        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover, bool hasInitializedModel)
        {
            if (!hasInitializedModel)
            {
                return;
            }

            firstPersonMover.gameObject.AddComponent<RobotInventory>();
        }
    }
}