using OverhaulMod.UI;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Engine
{
    public class UpgradeModeButtonController : OverhaulUIBehaviour
    {
        [UIElement("RevertUpgradesText")]
        private readonly GameObject _revertUpgradesTextObject;

        [UIElement("RevertUpgradesText")]
        private readonly Text _revertUpgradesText;

        [UIElement("GetUpgradesText")]
        private readonly GameObject _getUpgradesTextObject;

        [UIElement("GetUpgradesText")]
        private readonly Text _getUpgradesText;

        private Button _button;

        private Image _graphic;

        protected override void OnInitialized()
        {
            Button button = base.GetComponent<Button>();
            _button = button;

            Image image = base.GetComponent<Image>();
            _graphic = image;
        }

        public override void OnEnable()
        {
            bool shouldBeActive = !GameModeManager.IsOnTitleScreen() && !GameModeManager.IsMultiplayer() && !ModIntegrationUtils.ModdedMultiplayer.IsInModdedMultiplayer();
            _button.interactable = shouldBeActive;
            _graphic.enabled = shouldBeActive;
            _getUpgradesTextObject.SetActive(shouldBeActive);
            _revertUpgradesTextObject.SetActive(shouldBeActive);
        }

        public void SetText(bool revert)
        {
            _revertUpgradesText.enabled = !revert;
            _getUpgradesText.enabled = revert;
        }

        public void SetSprite(Sprite sprite)
        {
            _graphic.sprite = sprite;
        }
    }
}
