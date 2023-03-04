using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul.Gameplay.Combat
{
    public class CombatOverhaulController : OverhaulGameplayController
    {
        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover, bool hasInitializedModel)
        {
            if (!hasInitializedModel || !OverhaulGamemodeManager.SupportsCombatOverhaul())
            {
                return;
            }

            firstPersonMover.gameObject.AddComponent<CombatSprintAndStance>();
        }
    }
}
