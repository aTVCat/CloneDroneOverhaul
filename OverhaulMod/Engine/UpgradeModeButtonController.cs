using OverhaulMod.UI;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Engine
{
    public class UpgradeModeButtonController : OverhaulUIBehaviour
    {
        [UIElement("RevertUpgradesText")]
        private readonly GameObject m_revertUpgradesTextObject;

        [UIElement("RevertUpgradesText")]
        private readonly Text m_revertUpgradesText;

        [UIElement("GetUpgradesText")]
        private readonly GameObject m_getUpgradesTextObject;

        [UIElement("GetUpgradesText")]
        private readonly Text m_getUpgradesText;

        private Button m_button;

        private Image m_graphic;

        protected override void OnInitialized()
        {
            Button button = base.GetComponent<Button>();
            m_button = button;

            Image image = base.GetComponent<Image>();
            m_graphic = image;
        }

        public override void OnEnable()
        {
            bool shouldBeActive = !GameModeManager.IsOnTitleScreen() && !GameModeManager.IsMultiplayer() && !ModIntegrationUtils.ModdedMultiplayer.IsInModdedMultiplayer();
            m_button.interactable = shouldBeActive;
            m_graphic.enabled = shouldBeActive;
            m_getUpgradesTextObject.SetActive(shouldBeActive);
            m_revertUpgradesTextObject.SetActive(shouldBeActive);
        }

        public void SetText(bool revert)
        {
            m_revertUpgradesText.enabled = !revert;
            m_getUpgradesText.enabled = revert;
        }

        public void SetSprite(Sprite sprite)
        {
            m_graphic.sprite = sprite;
        }
    }
}
