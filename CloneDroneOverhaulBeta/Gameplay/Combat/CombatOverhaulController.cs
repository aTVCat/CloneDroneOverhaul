using ModLibrary;

namespace CDOverhaul.Gameplay.Combat
{
    public class CombatOverhaulController : OverhaulGameplayController
    {
        private CombatOverhaulTutorialController m_TutorialController;

        public override void Initialize()
        {
            base.Initialize();

            m_TutorialController = AddController<CombatOverhaulTutorialController>();
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

            _ = firstPersonMover.gameObject.AddComponent<CombatSprintAndStance>();
        }
    }
}
