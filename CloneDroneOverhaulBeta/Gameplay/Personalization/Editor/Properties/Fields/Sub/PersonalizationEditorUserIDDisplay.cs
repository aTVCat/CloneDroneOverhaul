using Steamworks;
using System.Collections.Generic;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorUserIDDisplay : PersonalizationEditorUIElement
    {
        private static readonly string[] s_Labels = new string[]
        {
            "Steam",
            "Discord",
            "PlayFab",
        };

        [UIElementReferenceAttribute("DisplayName")]
        private readonly Text m_DisplayName;

        [UIElementReferenceAttribute("ID")]
        private readonly Text m_DisplayID;

        [UIElementReferenceAttribute("TypeLabel")]
        private readonly Text m_TypeLabel;

        private bool m_HasAlreadyInitialized;

        public string UserID
        {
            get;
            set;
        }

        public byte IDType
        {
            get;
            set;
        }

        public bool Error
        {
            get;
            set;
        }

        public int Index
        {
            get;
            set;
        }

        public PersonalizationEditorStringSpecViewFieldDisplay FieldDisplay
        {
            get;
            set;
        }

        protected override bool AssignVariablesAutomatically() => false;

        public void Initialize(string id, byte type, int index)
        {
            if (!m_HasAlreadyInitialized)
            {
                UIController.AssignVariables(this);

                base.GetComponent<Button>().AddOnClickListener(EditInfo);

                m_HasAlreadyInitialized = true;
            }

            UserID = id;
            IDType = type;
            Index = index;
            Error = false;

            switch (type)
            {
                case 1:
                    if (!ulong.TryParse(id, out ulong result))
                    {
                        Error = true;
                        m_DisplayName.text = "ERROR";
                        return;
                    }

                    m_DisplayName.text = SteamFriends.GetFriendPersonaName((CSteamID)result);
                    break;
                case 2:
                    if (!long.TryParse(id, out long result2))
                    {
                        Error = true;
                        m_DisplayName.text = "ERROR";
                        return;
                    }
                    /*
                    if (!OverhaulDiscordRPC.HasInitialized)
                    {
                        m_DisplayName.text = "[No Client]";
                        break;
                    }
                    m_DisplayName.text = "[Loading...]";
                    OverhaulDiscordRPC.GetUserInfo(result2, delegate (Discord.User user)
                    {
                        if (m_DisplayName)
                            m_DisplayName.text = user.Username;
                    }, delegate (Result r)
                    {
                        if (m_DisplayName)
                            m_DisplayName.text = string.Format("[ERROR: {0}]", r);
                    });*/
                    break;
                case 3:
                    m_DisplayName.text = "[Nicknames are unsupported]";
                    break;
            }
            m_DisplayID.text = id;
            m_TypeLabel.text = s_Labels[type - 1];
        }

        public void EditInfo()
        {
            if (!FieldDisplay)
                return;

            EditorUI.PlayerInfoConfigMenu.Show(FieldDisplay.FieldValue as List<string>, Index, false, delegate (string newValue)
            {
                if (newValue == "delete")
                {
                    FieldDisplay.Populate();
                    EditorUI.SavePanel.NeedsToSave = true;
                    return;
                }

                string text = PersonalizationEditor.GetOnlyID(newValue, out byte newType);
                Initialize(text, newType, Index);
                EditorUI.SavePanel.NeedsToSave = true;
            });
        }
    }
}
