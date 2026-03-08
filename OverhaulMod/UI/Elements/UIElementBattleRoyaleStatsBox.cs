using OverhaulMod.Utils;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementBattleRoyaleStatsBox : OverhaulUIBehaviour
    {
        [UIElement("WinCountText")]
        private readonly Text _winCountText;

        [UIElement("NextGarbageBotWinsText")]
        private readonly Text _nextGarbageBotWinsText;

        [UIElement("NextGarbageBotImage", false)]
        private readonly Image _nextGarbageBotImage;

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

            _winCountText.text = lastBotStandingWins.ToString();
            _nextGarbageBotImage.sprite = garbageBotInfo == null || !garbageBotInfo.PreviewImage ? null : garbageBotInfo.PreviewImage;
            _nextGarbageBotImage.gameObject.SetActive(garbageBotInfo != null);
            _nextGarbageBotWinsText.text = upcomingGarbageBotInfo == null ? "-" : upcomingGarbageBotInfo.MinWins.ToString();
        }
    }
}
