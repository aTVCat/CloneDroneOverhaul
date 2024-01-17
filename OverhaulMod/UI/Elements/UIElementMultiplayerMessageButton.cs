using OverhaulMod.Utils;
using RootMotion;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementMultiplayerMessageButton : OverhaulUIBehaviour
    {
        [UIElement("ViewMultiplayerErrorButton")]
        public Graphic m_bgGraphic;
        [UIElement("Icon")]
        public RawImage m_icon;
        [UIElement("Icon")]
        public Outline m_iconOutline;

        [UIElement("LoadingIndicator", false)]
        public GameObject m_iconLoadingIndicator;

        private (Color, Color)[] m_colors;

        private float m_timeLeftForAnUpdate;
        private int m_prevState;

        protected override void OnInitialized()
        {
            m_colors = new (Color, Color)[]
            {
                (ModParseUtils.TryParseToColor("E62E2E", Color.white), ModParseUtils.TryParseToColor("661919", Color.gray)),
                (ModParseUtils.TryParseToColor("E6B92E", Color.white), ModParseUtils.TryParseToColor("998126", Color.gray)),
                (ModParseUtils.TryParseToColor("00B301", Color.white), ModParseUtils.TryParseToColor("00610A", Color.gray))
            };
        }

        public override void Update()
        {
            m_timeLeftForAnUpdate -= Time.deltaTime;
            if(m_timeLeftForAnUpdate <= 0f)
            {
                Refresh();
            }
        }

        public void Refresh()
        {
            m_timeLeftForAnUpdate = 0.5f;

            int curState = m_prevState;
            bool userLoggedIn = MultiplayerLoginManager.Instance.IsLoggedIntoPlayfab();
            BanOrWarningMessage banOrWarningMessage = PlayFabPlayerDataManager.Instance.GetBanOrWarningMessage();
            if(!userLoggedIn || MultiplayerLoginManager.Instance.IsBanned())
            {
                if(banOrWarningMessage != null && banOrWarningMessage.IsWarning)
                {
                    m_prevState = 1;
                    SetColor(m_colors[1].Item1, m_colors[1].Item2);
                    return;
                }
                m_prevState = 2;
                SetColor(m_colors[0].Item1, m_colors[0].Item2);
            }
            else if (banOrWarningMessage != null)
            {
                m_prevState = 3;
                SetColor(banOrWarningMessage.IsWarning ? m_colors[1].Item1 : m_colors[0].Item1, banOrWarningMessage.IsWarning ? m_colors[1].Item2 : m_colors[0].Item2);
            }
            else
            {
                m_prevState = 0;
                SetColor(m_colors[2].Item1, m_colors[2].Item2);
            }

            if(curState != m_prevState)
            {
                UITitleScreenRework titleScreenRework = ModUIManager.Instance.Get<UITitleScreenRework>(AssetBundleConstants.UI, ModUIConstants.UI_TITLE_SCREEN);
                if (titleScreenRework && titleScreenRework.visibleInHierarchy)
                {
                    titleScreenRework.ErrorMessage.Refresh();
                }
            }
        }

        public void SetColor(Color firstColor, Color secondColor)
        {
            m_bgGraphic.color = firstColor;
            m_iconOutline.effectColor = secondColor;
        }
    }
}
