using OverhaulMod.Engine;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPressActionKeyDescription : OverhaulUIBehaviour
    {
        [ModSetting(ModSettingIDs.PAK_DESCRIPTION_BG, true)]
        public static bool EnableBG;

        [ModSetting(ModSettingIDs.PAK_DESCRIPTION_FONT, 1)]
        public static int FontType;

        [ModSetting(ModSettingIDs.PAK_DESCRIPTION_FONT_SIZE, 10)]
        public static int FontSize;

        [UIElement("BG")]
        private readonly RectTransform _bg;

        [UIElement("BG", false)]
        private readonly GameObject _bgObject;

        [UIElement("BG")]
        private readonly Image _bgImage;

        [UIElement("BG")]
        private readonly CanvasGroup _bgCanvasGroup;

        [UIElement("Text")]
        private readonly Text _text;

        private BetterOutline _textOutline;

        public override bool CloseOnEscapeButtonPress => false;

        private float _expandProgress;

        private bool _show;

        private int _siblingIndex;

        protected override void OnInitialized()
        {
            BetterOutline betterOutline = _text.gameObject.AddComponent<BetterOutline>();
            betterOutline.effectColor = Color.black;
            betterOutline.effectDistance = Vector2.one * 1.25f;
            _textOutline = betterOutline;

            GlobalEventManager.Instance.AddEventListener<string>(ModResources.ASSET_BUNDLE_LOADED_EVENT, onAssetBundleLoaded);

            ModSettingsManager.Instance.AddSettingValueChangedListener(refreshSettings, ModSettingIDs.PAK_DESCRIPTION_BG);
            ModSettingsManager.Instance.AddSettingValueChangedListener(refreshSettings, ModSettingIDs.PAK_DESCRIPTION_FONT);
            ModSettingsManager.Instance.AddSettingValueChangedListener(refreshSettings, ModSettingIDs.PAK_DESCRIPTION_FONT_SIZE);

            refreshSettings(null);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            GlobalEventManager.Instance.RemoveEventListener<string>(ModResources.ASSET_BUNDLE_LOADED_EVENT, onAssetBundleLoaded);

            ModSettingsManager.Instance.RemoveSettingValueChangedListener(refreshSettings, ModSettingIDs.PAK_DESCRIPTION_BG);
            ModSettingsManager.Instance.RemoveSettingValueChangedListener(refreshSettings, ModSettingIDs.PAK_DESCRIPTION_FONT);
            ModSettingsManager.Instance.RemoveSettingValueChangedListener(refreshSettings, ModSettingIDs.PAK_DESCRIPTION_FONT_SIZE);
        }

        public override void Update()
        {
            Text textComponent = _text;

            RectTransform rt = _bg;
            Vector2 sd = rt.sizeDelta;
            sd.x = Mathf.Lerp(0f, Mathf.Clamp(textComponent.preferredWidth + 15f, 100f, 200f), NumberUtils.EaseOutQuad(0f, 1f, _expandProgress));
            sd.y = Mathf.Lerp(0f, textComponent.preferredHeight + 12.5f, NumberUtils.EaseOutQuad(0f, 1f, _expandProgress));
            rt.sizeDelta = sd;

            _bgObject.SetActive(_expandProgress > 0f);
            if (!_show && _expandProgress == 0f)
            {
                if (!textComponent.text.IsNullOrEmpty())
                    textComponent.text = null;
            }

            _expandProgress = Mathf.Clamp01(_expandProgress + ((_show ? 1f : -1f) * Time.unscaledDeltaTime * 7.5f));
        }

        private void onAssetBundleLoaded(string assetBundle)
        {
            if (assetBundle == AssetBundleConstants.UI_EXTRA)
                refreshSettings(null);
        }

        public void ShowText(string text)
        {
            if (FontType == 0)
            {
                _text.font = LocalizationManager.Instance.GetCurrentSubtitlesFont();
            }

            _text.text = text;
            _show = true;
        }

        public void HideText()
        {
            _show = false;
        }

        public void SetSiblingIndex(bool last)
        {
            if (last)
            {
                _siblingIndex = base.transform.GetSiblingIndex();
                base.transform.SetAsLastSibling();
            }
            else if (_siblingIndex != 0)
            {
                base.transform.SetSiblingIndex(_siblingIndex);
            }
        }

        private void refreshSettings(object obj)
        {
            _textOutline.enabled = !EnableBG;
            _bgImage.enabled = EnableBG;

            _text.fontSize = FontSize;
            _text.font = ModResources.FontByIndex(FontType);
        }
    }
}
