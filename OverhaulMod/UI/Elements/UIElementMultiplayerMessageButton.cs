using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementMultiplayerMessageButton : OverhaulUIBehaviour
    {
        [UIElement("ViewMultiplayerErrorButton")]
        private readonly Graphic _bgGraphic;

        [UIElement("FineIcon", false)]
        private readonly GameObject _fineIconObject;
        [UIElement("FineIcon")]
        private readonly Outline _fineIconOutline;

        [UIElement("WarnIcon", true)]
        private readonly GameObject _warnIconObject;
        [UIElement("WarnIcon")]
        private readonly Outline _warnIconOutline;

        private (Color, Color)[] _colors;

        private float _timeLeftForAnUpdate;
        private int _prevState;

        private PlayFabPlayerDataManager _playFabPlayerDataManager;
        public PlayFabPlayerDataManager playFabPlayerDataManager
        {
            get
            {
                if (!_playFabPlayerDataManager)
                    _playFabPlayerDataManager = PlayFabPlayerDataManager.Instance;

                return _playFabPlayerDataManager;
            }
        }

        private MultiplayerLoginManager _multiplayerLoginManager;
        public MultiplayerLoginManager multiplayerLoginManager
        {
            get
            {
                if (!_multiplayerLoginManager)
                    _multiplayerLoginManager = MultiplayerLoginManager.Instance;

                return _multiplayerLoginManager;
            }
        }

        protected override void OnInitialized()
        {
            _colors = new (Color, Color)[]
            {
                (ModParseUtils.TryParseColor("E62E2E", Color.white), ModParseUtils.TryParseColor("661919", Color.gray)),
                (ModParseUtils.TryParseColor("E6B92E", Color.white), ModParseUtils.TryParseColor("998126", Color.gray)),
                (ModParseUtils.TryParseColor("00B301", Color.white), ModParseUtils.TryParseColor("00610A", Color.gray))
            };

            ModUIManager.Instance.Get<UITitleScreenRework>(ModAssetBundles.UI, ModUIs.UI_TITLE_SCREEN_REWORK).SetMultiplayerButtonActive(false);
        }

        public override void Update()
        {
            _timeLeftForAnUpdate -= Time.deltaTime;
            if (_timeLeftForAnUpdate <= 0f)
            {
                Refresh();
            }
        }

        public void Refresh()
        {
            _timeLeftForAnUpdate = 0.5f;

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
                SetColor(_colors[0].Item1, _colors[0].Item2);
                shouldActivateMultiplayerButton = false;
            }
            else if (userWarned)
            {
                curState = 1;
                SetIcon(true);
                SetColor(_colors[1].Item1, _colors[1].Item2);
                shouldActivateMultiplayerButton = true;
            }
            else
            {
                curState = 2;
                SetIcon(false);
                SetColor(_colors[2].Item1, _colors[2].Item2);
                shouldActivateMultiplayerButton = true;
            }

            UITitleScreenRework titleScreenRework = ModUIManager.Instance?.Get<UITitleScreenRework>(ModAssetBundles.UI, ModUIs.UI_TITLE_SCREEN_REWORK);
            if (!titleScreenRework)
                return;

            if (curState != _prevState)
            {
                if (titleScreenRework.ErrorMessage)
                {
                    titleScreenRework.ErrorMessage.Refresh();
                    if (curState == 2)
                        titleScreenRework.ErrorMessage.Hide();
                }
                _prevState = curState;
            }

            titleScreenRework.SetMultiplayerButtonActive(shouldActivateMultiplayerButton);
        }

        public void SetColor(Color firstColor, Color secondColor)
        {
            _bgGraphic.color = firstColor;
            _fineIconOutline.effectColor = secondColor;
            _warnIconOutline.effectColor = secondColor;
        }

        public void SetIcon(bool warn)
        {
            _fineIconObject.SetActive(!warn);
            _warnIconObject.SetActive(warn);
        }
    }
}
