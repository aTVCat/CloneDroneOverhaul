using OverhaulMod.Utils;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementBattleRoyaleStatsBox : OverhaulUIBehaviour
    {
        [UIElement("WinCountText")]
        private readonly Text m_winCountText;

        [UIElement("NextGarbageBotWinsText")]
        private readonly Text m_nextGarbageBotWinsText;

        [UIElement("NextGarbageBotImage", false)]
        private readonly Image m_nextGarbageBotImage;

        public override void OnEnable()
        {
            ModActionUtils.DoInFrame(refresh);
        }

        private void refresh()
        {
            MultiplayerLoginManager multiplayerLoginManager = MultiplayerLoginManager.Instance;
            PlayFabPlayerStatsManager playFabPlayerStatsManager = PlayFabPlayerStatsManager.Instance;
            BattleRoyaleGarbageBotCustomizationManager garbageBotCustomizationManager = BattleRoyaleGarbageBotCustomizationManager.Instance;
            if (!playFabPlayerStatsManager || !multiplayerLoginManager || !garbageBotCustomizationManager)
                return;

            PlayFabPlayerStats localPlayerStats = playFabPlayerStatsManager.GetLocalPlayerStats();
            if (localPlayerStats == null)
                return;

            int lastBotStandingWins = localPlayerStats.LastBotStandingWins;
            BattleRoyaleGarbageBotPerWin upcomingGarbageBotInfo = garbageBotCustomizationManager.GetUpcomingGarbageBotInfo(lastBotStandingWins);
            BattleRoyaleGarbageBotPerWin garbageBotInfo = garbageBotCustomizationManager.GetGarbageBotInfo(lastBotStandingWins);

            m_winCountText.text = lastBotStandingWins.ToString();
            m_nextGarbageBotImage.sprite = garbageBotInfo == null || !garbageBotInfo.PreviewImage ? null : garbageBotInfo.PreviewImage;
            m_nextGarbageBotImage.gameObject.SetActive(garbageBotInfo != null);
            m_nextGarbageBotWinsText.text = upcomingGarbageBotInfo == null ? "-" : upcomingGarbageBotInfo.MinWins.ToString();
        }
    }
}
