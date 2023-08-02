using UnityEngine.UI;

namespace CDOverhaul.HUD.Tooltips
{
    public class OverhaulClosestPlayerTooltip : OverhaulTooltip
    {
        private Text m_DisplayName;
        private Text m_WinsCount;
        private Text m_KillsCount;

        protected override float GetShowDuration() => 1f;
        public override void Initialize()
        {
            m_DisplayName = MyModdedObject.GetObject<Text>(0);
            m_WinsCount = MyModdedObject.GetObject<Text>(1);
            m_KillsCount = MyModdedObject.GetObject<Text>(2);
        }

        public void ShowTooltip(Character character)
        {
            if (!character)
                return;

            string playFabID = character.GetPlayFabID();
            if (string.IsNullOrEmpty(playFabID))
                return;

            MultiplayerPlayerInfoState playerInfoState = MultiplayerPlayerInfoManager.Instance.GetPlayerInfoState(playFabID);
            if (!playerInfoState || playerInfoState.IsDetached())
                return;

            m_DisplayName.text = playerInfoState.state.DisplayName;
            m_WinsCount.text = playerInfoState.state.LastBotStandingWins + " " + OverhaulLocalizationController.GetTranslation("Wins2");
            m_KillsCount.text = playerInfoState.state.Kills + " " + OverhaulLocalizationController.GetTranslation("Kills");
            Show();
        }
    }
}
