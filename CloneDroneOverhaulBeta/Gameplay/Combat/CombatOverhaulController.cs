using CDOverhaul.Gameplay.Combat.Fights;
using ModLibrary;

namespace CDOverhaul.Gameplay.Combat
{
    public class CombatOverhaulController : OverhaulGameplayController
    {
        private CombatOverhaulTutorialController m_TutorialController;
        private CombatOverhaulInventoryController m_InventoryController;
        private CombatOverhaulCombosController m_CombosController;

        public override void Initialize()
        {
            base.Initialize();

            m_TutorialController = AddController<CombatOverhaulTutorialController>();
            m_InventoryController = AddController<CombatOverhaulInventoryController>();
            m_CombosController = AddController<CombatOverhaulCombosController>();
        }

        public void StartTutorial()
        {
            MetagameProgressManager.Instance.SetProgress(MetagameProgress.P11_StartedSpaceCombat);
            GameDataManager.Instance.GetPrivateField<GameData>("_storyModeData").CurentLevelID = "CombatTutorial";
            GameDataManager.Instance.GetPrivateField<GameData>("_storyModeData").SetDirty(true);
            DelegateScheduler.Instance.Schedule(GameFlowManager.Instance.LoadGameplaySceneAndStartStoryMode, 0.2f);
        }

        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover, bool hasInitializedModel)
        {
            if (!hasInitializedModel || !OverhaulGamemodeManager.SupportsCombatOverhaul())
            {
                return;
            }

            CombatSprintAndStance stance = firstPersonMover.gameObject.AddComponent<CombatSprintAndStance>();
            CombatOverhaulUnbalancing unbalancing = firstPersonMover.gameObject.AddComponent<CombatOverhaulUnbalancing>();
            unbalancing.SprintAndStance = stance;
        }
    }
}
