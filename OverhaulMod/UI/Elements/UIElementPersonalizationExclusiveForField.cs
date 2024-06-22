using OverhaulMod.Content.Personalization;
using OverhaulMod.Utils;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementPersonalizationExclusiveForField : OverhaulUIBehaviour
    {
        [UIElement("Text")]
        private readonly Text m_text;

        [UIElementAction(nameof(OnEditButtonClicked))]
        [UIElement("EditButton")]
        private readonly Button m_editButton;

        private List<PersonalizationItemLockInfo> m_referenceList;
        public List<PersonalizationItemLockInfo> referenceList
        {
            get
            {
                return m_referenceList;
            }
            set
            {
                m_referenceList = value;
                refreshText();
            }
        }

        private void refreshText()
        {
            List<PersonalizationItemLockInfo> list = PersonalizationEditorManager.Instance?.currentEditingItemInfo?.ExclusiveFor_V2;
            if (list.IsNullOrEmpty())
            {
                m_text.text = "None";
                return;
            }

            int index = 0;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (PersonalizationItemLockInfo item in list)
            {
                object toAppend;
                if (item.PlayerNickname.IsNullOrEmpty())
                {
                    if (!item.PlayerPlayFabID.IsNullOrEmpty())
                    {
                        toAppend = item.PlayerPlayFabID;
                    }
                    else if (item.PlayerSteamID != 0UL)
                    {
                        toAppend = item.PlayerSteamID;
                    }
                    else if (item.PlayerDiscordUserID != 0L)
                    {
                        toAppend = item.PlayerDiscordUserID;
                    }
                    else
                    {
                        toAppend = "N/A";
                    }
                }
                else
                {
                    toAppend = item.PlayerNickname;
                }

                _ = stringBuilder.Append(toAppend);
                if(index < list.Count - 1)
                    stringBuilder.Append(", ");

                index++;
            }
            m_text.text = stringBuilder.ToString();
        }

        public void OnEditButtonClicked()
        {
            UIPersonalizationEditorExclusivityEditMenu menu = ModUIConstants.ShowPersonalizationEditorExclusivityEditMenu(UIPersonalizationEditor.instance.transform);
            menu.Populate(referenceList, refreshText);
        }
    }
}
