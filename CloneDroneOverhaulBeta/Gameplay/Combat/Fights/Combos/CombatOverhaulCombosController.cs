using System.Collections.Generic;

namespace CDOverhaul.Gameplay.Combat.Fights
{
    public class CombatOverhaulCombosController : OverhaulGameplayController
    {
        private static List<ComboBase> m_AllCombos;

        public override void Initialize()
        {
            base.Initialize();
            if (!OverhaulSessionController.GetKey<bool>("HasInitialized"))
            {
                OverhaulSessionController.SetKey("HasInitialized", true);

                m_AllCombos = new List<ComboBase>()
                {
                    new DoubleStrike()
                };
            }
        }

        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover, bool hasInitializedModel)
        {
            return;
            if (!hasInitializedModel)
            {
                return;
            }

            CombatOverhaulCombosBehaviour b = firstPersonMover.gameObject.AddComponent<CombatOverhaulCombosBehaviour>();
            b.Initialize(m_AllCombos);
        }
    }
}
