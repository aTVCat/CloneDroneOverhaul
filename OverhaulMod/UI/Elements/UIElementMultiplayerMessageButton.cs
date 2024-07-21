using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementMultiplayerMessageButton : OverhaulUIBehaviour
    {
        [UIElement("ViewMultiplayerErrorButton")]
        private readonly Graphic m_bgGraphic;

        [UIElement("FineIcon", false)]
        private readonly GameObject m_fineIconObject;
        [UIElement("FineIcon")]
        private readonly Outline m_fineIconOutline;

        [UIElement("WarnIcon", true)]
        private readonly GameObject m_warnIconObject;
        [UIElement("WarnIcon")]
        private readonly Outline m_warnIconOutline;

        private (Color, Color)[] m_colors;

        private float m_timeLeftForAnUpdate;
        private int m_prevState;

        private PlayFabPlayerDataManager m_playFabPlayerDataManager;
        public PlayFabPlayerDataManager playFabPlayerDataManager
        {
            get
            {
                if (!m_playFabPlayerDataManager)
                    m_playFabPlayerDataManager = PlayFabPlayerDataManager.Instance;

                return m_playFabPlayerDataManager;
            }
        }

        private MultiplayerLoginManager m_multiplayerLoginManager;
        public MultiplayerLoginManager multiplayerLoginManager
        {
            get
            {
                if (!m_multiplayerLoginManager)
                    m_multiplayerLoginManager = MultiplayerLoginManager.Instance;

                return m_multiplayerLoginManager;
            }
        }

        protected override void OnInitialized()
        {
            m_colors = new (Color, Color)[]
            {
                (ModParseUtils.TryParseToColor("E62E2E", Color.white), ModParseUtils.TryParseToColor("661919", Color.gray)),
                (ModParseUtils.TryParseToColor("E6B92E", Color.white), ModParseUtils.TryParseToColor("998126", Color.gray)),
                (ModParseUtils.TryParseToColor("00B301", Color.white), ModParseUtils.TryParseToColor("00610A", Color.gray))
            };

            ModUIManager.Instance.Get<UITitleScreenRework>(AssetBundleConstants.UI, ModUIConstants.UI_TITLE_SCREEN).SetMultiplayerButtonActive(false);
        }

        public override void Update()
        {
            m_timeLeftForAnUpdate -= Time.deltaTime;
            if (m_timeLeftForAnUpdate <= 0f)
            {
                Refresh();
            }
        }

        public void Refresh()
        {
            m_timeLeftForAnUpdate = 0.5f;

            MultiplayerLoginManager loginManager = multiplayerLoginManager;
            PlayFabPlayerDataManager playerDataManager = playFabPlayerDataManager;
            if (!playerDataManager || !loginManager)
                return;

            BanOrWarningMessage banOrWarningMessage = playerDataManager.GetBanOrWarningMessage();
            bool userLoggedIn = loginManager.IsLoggedIntoPlayfab();
            bool userBanned = loginManager.IsBanned();
            bool userWarned = banOrWarningMessage != null && banOrWarningMessage.IsWarning;

            int curState;
            bool shouldActivateMultiplayerButton;
            if (!userLoggedIn || userBanned)
            {
                curState = 0;
                SetIcon(true);
                SetColor(m_colors[0].Item1, m_colors[0].Item2);
                shouldActivateMultiplayerButton = false;
            }
            else if (userWarned)
            {
                curState = 1;
                SetIcon(true);
                SetColor(m_colors[1].Item1, m_colors[1].Item2);
                shouldActivateMultiplayerButton = true;
            }
            else
            {
                curState = 2;
                SetIcon(false);
                SetColor(m_colors[2].Item1, m_colors[2].Item2);
                shouldActivateMultiplayerButton = true;
            }

            UITitleScreenRework titleScreenRework = ModUIManager.Instance?.Get<UITitleScreenRework>(AssetBundleConstants.UI, ModUIConstants.UI_TITLE_SCREEN);
            if (!titleScreenRework)
                return;

            if (curState != m_prevState)
            {
                if (titleScreenRework.ErrorMessage)
                {
                    titleScreenRework.ErrorMessage.Refresh();
                    if (curState == 2)
                        titleScreenRework.ErrorMessage.Hide();
                }
                m_prevState = curState;
            }

            titleScreenRework.SetMultiplayerButtonActive(shouldActivateMultiplayerButton);
        }

        public void SetColor(Color firstColor, Color secondColor)
        {
            m_bgGraphic.color = firstColor;
            m_fineIconOutline.effectColor = secondColor;
            m_warnIconOutline.effectColor = secondColor;
        }

        public void SetIcon(bool warn)
        {
            m_fineIconObject.SetActive(!warn);
            m_warnIconObject.SetActive(warn);
        }
    }
}
