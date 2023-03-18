using System.Collections;
using UnityEngine;

namespace CDOverhaul.Gameplay.Combat
{
    /// <summary>
    /// I might scrap it for future updates
    /// Todo: If I'll have some free time, implement this
    /// </summary>
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