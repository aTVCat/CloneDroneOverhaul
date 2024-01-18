using OverhaulMod.Content;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementMultiplayerMessageButton : OverhaulUIBehaviour
    {
        public const string STATE_FINE_CACHE_KEY = "TitleScreenMButtonStateIcon_Fine";
        public const string STATE_BAD_CACHE_KEY = "TitleScreenMButtonStateIcon_Bad";

        [UIElement("ViewMultiplayerErrorButton")]
        public Graphic m_bgGraphic;
        [UIElement("Icon")]
        public RawImage m_icon;
        [UIElement("Icon")]
        public Outline m_iconOutline;

        [UIElement("LoadingIndicator", false)]
        public GameObject m_iconLoadingIndicator;

        private readonly Button m_playMultiplayerButton;

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

            bool shouldActivateMultiplayerButton = false;
            int curState = m_prevState;
            bool userLoggedIn = MultiplayerLoginManager.Instance.IsLoggedIntoPlayfab();
            BanOrWarningMessage banOrWarningMessage = PlayFabPlayerDataManager.Instance.GetBanOrWarningMessage();
            if (!userLoggedIn || MultiplayerLoginManager.Instance.IsBanned())
            {
                if (banOrWarningMessage != null && banOrWarningMessage.IsWarning)
                {
                    m_prevState = 1;
                    SetIcon(true);
                    SetColor(m_colors[1].Item1, m_colors[1].Item2);
                    return;
                }
                m_prevState = 2;
                SetIcon(true);
                SetColor(m_colors[0].Item1, m_colors[0].Item2);
            }
            else if (banOrWarningMessage != null)
            {
                m_prevState = 3;
                SetIcon(true);
                SetColor(banOrWarningMessage.IsWarning ? m_colors[1].Item1 : m_colors[0].Item1, banOrWarningMessage.IsWarning ? m_colors[1].Item2 : m_colors[0].Item2);
                shouldActivateMultiplayerButton = banOrWarningMessage.IsWarning;
            }
            else
            {
                m_prevState = 0;
                SetIcon(false);
                SetColor(m_colors[2].Item1, m_colors[2].Item2);
                shouldActivateMultiplayerButton = true;
            }

            if (curState != m_prevState)
            {
                UITitleScreenRework titleScreenRework = ModUIManager.Instance.Get<UITitleScreenRework>(AssetBundleConstants.UI, ModUIConstants.UI_TITLE_SCREEN);
                if (titleScreenRework && titleScreenRework.visibleInHierarchy)
                {
                    titleScreenRework.ErrorMessage.Refresh();
                    if (m_prevState == 0)
                    {
                        titleScreenRework.ErrorMessage.Hide();
                    }
                }
            }

            ModUIManager.Instance.Get<UITitleScreenRework>(AssetBundleConstants.UI, ModUIConstants.UI_TITLE_SCREEN).SetMultiplayerButtonActive(shouldActivateMultiplayerButton);
        }

        public void SetColor(Color firstColor, Color secondColor)
        {
            m_bgGraphic.color = firstColor;
            m_iconOutline.effectColor = secondColor;
        }

        public void SetIcon(bool warn)
        {
            Texture2D texture = null;

            if (warn)
            {
                if (!ModAdvancedCache.TryGet(STATE_BAD_CACHE_KEY, out texture))
                {
                    ContentRepositoryManager.Instance.GetLocalTexture(ModCore.texturesFolder + "ui/M_Warn.png", delegate (Texture2D textureFine)
                    {
                        textureFine.filterMode = FilterMode.Point;
                        if (ModAdvancedCache.Has(STATE_BAD_CACHE_KEY))
                        {
                            Destroy(ModAdvancedCache.Get<Texture2D>(STATE_BAD_CACHE_KEY));
                            return;
                        }
                        ModAdvancedCache.Add(STATE_BAD_CACHE_KEY, textureFine);
                        m_icon.texture = textureFine;
                    }, null, out _);
                    return;
                }
            }
            else
            {
                if (!ModAdvancedCache.TryGet(STATE_FINE_CACHE_KEY, out texture))
                {
                    ContentRepositoryManager.Instance.GetLocalTexture(ModCore.texturesFolder + "ui/M_Fine.png", delegate (Texture2D textureFine)
                    {
                        textureFine.filterMode = FilterMode.Point;
                        if (ModAdvancedCache.Has(STATE_FINE_CACHE_KEY))
                        {
                            Destroy(ModAdvancedCache.Get<Texture2D>(STATE_FINE_CACHE_KEY));
                            return;
                        }
                        ModAdvancedCache.Add(STATE_FINE_CACHE_KEY, textureFine);
                        m_icon.texture = textureFine;
                    }, null, out _);
                    return;
                }
            }

            m_icon.texture = texture;
        }
    }
}
