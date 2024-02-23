using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementTitleScreenBackgroundConfig : OverhaulUIBehaviour
    {
        [UIElement("Label")]
        private readonly Text m_label;

        [UIElementAction(nameof(OnEditButtonClicked))]
        [UIElement("EditStaticBGButton")]
        private readonly Button m_editButton;

        private TitleScreenBackgroundInfo m_backgroundInfo;
        public TitleScreenBackgroundInfo backgroundInfo
        {
            get
            {
                return m_backgroundInfo;
            }
            set
            {
                m_backgroundInfo = value;

                if (m_backgroundInfo != null && m_label)
                    m_label.text = m_backgroundInfo.Level == null ? "None" : m_backgroundInfo.Level.LevelID;
            }
        }

        public bool refreshWhenEdited
        {
            get;
            set;
        }

        public void OnEditButtonClicked()
        {
            List<LevelDescription> list = new List<LevelDescription>();
            list.AddRange(WorkshopLevelManager.Instance.GetAllWorkShopEndlessLevels());
            list.AddRange(LevelManager.Instance._endlessLevels);

            ModUIUtils.LevelDescriptionBrowser(list, delegate (LevelDescription level)
            {
                TitleScreenCustomizationManager titleScreenCustomizationManager = TitleScreenCustomizationManager.Instance;
                if (level == null)
                {
                    backgroundInfo.Level = null;
                    titleScreenCustomizationManager.SetStaticLevel(null, refreshWhenEdited);
                    titleScreenCustomizationManager.SaveCustomizationInfo();
                    m_label.text = "None";
                }
                else
                {
                    backgroundInfo.Level = level;
                    titleScreenCustomizationManager.SetStaticLevel(level, refreshWhenEdited);
                    titleScreenCustomizationManager.SaveCustomizationInfo();
                    m_label.text = level.LevelID;
                }
            });
        }
    }
}
