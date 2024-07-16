using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPatchNotes : OverhaulUIBehaviour
    {
        [ModSetting(ModSettingsConstants.HAS_SHOWED_PATCH_NOTES, false)]
        public static bool HasShowedPatchNotes;

        [UIElementAction(nameof(OnCloseButtonClicked))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElement("Description")]
        private readonly Text m_descriptionText;

        public override bool hideTitleScreen => true;

        private bool m_shouldRefreshText;

        protected override void OnInitialized()
        {
            string langCode = LocalizationManager.Instance.GetCurrentLanguageCode();
            if (langCode != "ru" && langCode != "en")
                langCode = "en";

            string file = Path.Combine(ModCore.dataFolder, $"changelog_{langCode}.txt");
            string text;
            if (File.Exists(file))
                text = ModIOUtils.ReadText(file);
            else
            {
                if(langCode != "en")
                {
                    file = Path.Combine(ModCore.dataFolder, $"changelog_en.txt");
                    if(File.Exists(file))
                        text = ModIOUtils.ReadText(file);
                    else
                        text = "Changelog file load error.";
                }
                else
                {
                    text = "Changelog file load error.";
                }
            }

            m_descriptionText.text = text;
            m_shouldRefreshText = true;
        }

        public override void Update()
        {
            base.Update();
            if (m_shouldRefreshText)
            {
                m_shouldRefreshText = false;

                Vector2 sizeDelta = m_descriptionText.rectTransform.sizeDelta;
                sizeDelta.y = m_descriptionText.preferredHeight + 30f;
                m_descriptionText.rectTransform.sizeDelta = sizeDelta;
            }
        }

        public void OnCloseButtonClicked()
        {
            Hide();
            ModSettingsManager.SetBoolValue(ModSettingsConstants.HAS_SHOWED_PATCH_NOTES, true);
            ModSettingsDataManager.Instance.Save();
        }
    }
}
