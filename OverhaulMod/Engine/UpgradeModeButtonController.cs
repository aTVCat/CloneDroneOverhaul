using OverhaulMod.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Engine
{
    public class UpgradeModeButtonController : OverhaulUIBehaviour
    {
        [UIElement("RevertUpgradesText")]
        private GameObject m_revertUpgradesTextObject;

        [UIElement("RevertUpgradesText")]
        private Text m_revertUpgradesText;

        [UIElement("GetUpgradesText")]
        private GameObject m_getUpgradesTextObject;

        [UIElement("GetUpgradesText")]
        private Text m_getUpgradesText;

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
            bool multiplayer = GameModeManager.IsMultiplayer();
            m_button.interactable = !multiplayer;
            m_graphic.enabled = !multiplayer;
            m_getUpgradesTextObject.SetActive(!multiplayer);
            m_revertUpgradesTextObject.SetActive(!multiplayer);
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
