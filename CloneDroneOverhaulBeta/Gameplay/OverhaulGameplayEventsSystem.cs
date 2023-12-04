namespace CDOverhaul.Gameplay
{
    public class OverhaulGameplayEventsSystem : OverhaulGameplaySystem
    {
        private GameMode m_GameModePrevFrame;

        private void Update()
        {
            GameMode newGameMode = GameFlowManager.Instance._gameMode;
            if (!newGameMode.Equals(m_GameModePrevFrame))
            {
                OverhaulEvents.DispatchEvent(OverhaulGameplayManager.GAMEMODE_CHANGED_EVENT);
                m_GameModePrevFrame = newGameMode;
            }
        }
    }
}
