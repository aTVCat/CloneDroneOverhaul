using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationPlayerInfoEditor : PersonalizationEditorUIElement
    {
        [ObjectDefaultVisibility(false)]
        [ObjectReference("Shading")]
        private readonly GameObject m_Shading;

        [ObjectReference("Dropdown2")]
        private Dropdown m_TypesDropdown;

        [ObjectReference("UserID")]
        private InputField m_IDInputField;

        private bool m_HasInitialized;

        public List<string> EditingList
        {
            get;
            set;
        }

        public int TargetIndex
        {
            get;
            set;
        }

        public bool AddNew
        {
            get;
            set;
        }

        public System.Action<string> CallBack
        {
            get;
            set;
        }

        protected override bool AssignVariablesAutomatically() => false;

        public void Show(List<string> list, int index, bool addNewEntry, System.Action<string> callback = null)
        {
            if (!m_HasInitialized)
            {
                OverhaulUIVer2.AssignValues(this);
                OverhaulUIVer2.AssignActionToButton(GetComponent<ModdedObject>(), "BackButton", Hide);
                OverhaulUIVer2.AssignActionToButton(GetComponent<ModdedObject>(), "Done", OnDoneClicked);
                m_HasInitialized = true;
            }

            base.gameObject.SetActive(true);
            m_Shading.SetActive(true);

            EditingList = list;
            TargetIndex = index;
            AddNew = addNewEntry;
            CallBack = callback;

            m_TypesDropdown.interactable = AllowUsingManyIDTypes();
            m_TypesDropdown.value = 0;

            if (list == null)
                return;

            string text = PersonalizationEditor.GetOnlyID(EditingList[index], out byte type);
            m_IDInputField.text = text;
            m_TypesDropdown.value = type - 1;
        }

        public void Hide()
        {
            base.gameObject.SetActive(false);
            m_Shading.SetActive(false);

            EditingList = null;
            TargetIndex = -1;
            AddNew = false;
            CallBack = null;
        }

        public void OnDoneClicked()
        {
            string text = string.Empty;
            switch (m_TypesDropdown.value)
            {
                case 0:
                    text = "steam ";
                    break;
                case 1:
                    text = "discord ";
                    break;
                case 2:
                    text = "playfab ";
                    break;
            }
            text += m_IDInputField.text;

            if (EditingList != null)
                EditingList[TargetIndex] = text;

            if (CallBack != null)
                CallBack(text);
            Hide();
        }

        public static bool AllowUsingManyIDTypes() => OverhaulVersion.IsDebugBuild;
    }
}
