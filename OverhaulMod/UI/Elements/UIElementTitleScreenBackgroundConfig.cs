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

            ModUIUtils.LevelDescriptionBrowser(list, delegate (List<LevelDescription> list1)
            {
                TitleScreenCustomizationManager titleScreenCustomizationManager = TitleScreenCustomizationManager.Instance;
                if (list1 == null)
                {
                    backgroundInfo.Level = null;
                    titleScreenCustomizationManager.SetStaticLevel(null, DifficultyTier.None, refreshWhenEdited);
                    titleScreenCustomizationManager.SaveCustomizationInfo();
                    m_label.text = "None";
                }
                else
                {
                    LevelDescription levelDescription = list1[0];

                    backgroundInfo.Level = levelDescription;
                    titleScreenCustomizationManager.SetStaticLevel(levelDescription, DifficultyTier.None, refreshWhenEdited);
                    titleScreenCustomizationManager.SaveCustomizationInfo();
                    m_label.text = levelDescription.LevelID;
                }
            }, false);
        }
    }
}
