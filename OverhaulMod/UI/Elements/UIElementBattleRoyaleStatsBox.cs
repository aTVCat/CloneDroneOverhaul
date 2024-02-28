using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementBattleRoyaleStatsBox : OverhaulUIBehaviour
    {
        [UIElement("WinCountText")]
        private Text m_winCountText;

        [UIElement("NextGarbageBotWinsText")]
        private Text m_nextGarbageBotWinsText;

        [UIElement("NextGarbageBotImage", false)]
        private Image m_nextGarbageBotImage;        

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
