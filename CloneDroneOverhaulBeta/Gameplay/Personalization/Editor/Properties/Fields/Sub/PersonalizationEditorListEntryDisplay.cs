using System.Collections;
using UnityEngine;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorListEntryDisplay : PersonalizationEditorUIElement
    {
        [UIElementReferenceAttribute("BG")]
        private readonly Transform m_Container;

        public PersonalizationEditorListFieldDisplay FieldDisplay
        {
            get;
            set;
        }

        public object List
        {
            get;
            set;
        }

        public int Index
        {
            get;
            set;
        }

        protected override bool AssignVariablesAutomatically() => false;

        public void Initialize(PersonalizationEditorListFieldDisplay fieldDisplay, object targetList, int index)
        {
            UIController.AssignValues(this);
            List = targetList;
            Index = index;
            FieldDisplay = fieldDisplay;
            if (FieldDisplay.IsPositionNodesList)
            {
                PersonalizationEditorTupleFieldDisplay fieldDisplay1 = Instantiate(fieldDisplay.Category.TupleFieldDisplay, m_Container);
                fieldDisplay1.gameObject.SetActive(true);
                fieldDisplay1.InitializeAsCollectionObject(this, "Node", (List as IList)[Index]);
                RectTransform rectTransform = fieldDisplay1.transform as RectTransform;
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.sizeDelta = Vector2.zero;
            }
        }

    }
}
