using ModLibrary;

namespace CDOverhaul.Gameplay.Combat
{
    /// <summary>
    /// This one will be reworked later
    /// I've made many changes to my plans and scrapped many things...
    /// </summary>
    public class CombatOverhaulController : OverhaulGameplayController
    {
        public override void Initialize()
        {
            base.Initialize();
        }

        public void StartTutorial()
        {
            MetagameProgressManager.Instance.SetProgress(MetagameProgress.P11_StartedSpaceCombat);
            GameDataManager.Instance.GetPrivateField<GameData>("_storyModeData").CurentLevelID = "CombatTutorial";
            GameDataManager.Instance.GetPrivateField<GameData>("_storyModeData").SetDirty(true);
            DelegateScheduler.Instance.Schedule(GameFlowManager.Instance.LoadGameplaySceneAndStartStoryMode, 0.2f);
        }
    }
}
