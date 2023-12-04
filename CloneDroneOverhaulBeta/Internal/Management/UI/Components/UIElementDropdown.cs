using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class UIElementDropdown : OverhaulBehaviour
    {
        [UIElementReference("SelectedOptionLabel")]
        private readonly Text m_SelectedOptionText;

        [UIElementDefaultVisibilityState(true)]
        [UIElementReference("CollapsedIcon")]
        private readonly GameObject m_CollapseIcon;
        [UIElementDefaultVisibilityState(false)]
        [UIElementReference("ExpandedIcon")]
        private readonly GameObject m_ExpandIcon;

        [UIElementDefaultVisibilityState(false)]
        [UIElementReference("TheDropdown")]
        private readonly GameObject m_DropdownObject;
        [UIElementReference("TheDropdown")]
        private readonly RectTransform m_DropdownTransform;
        [UIElementReference("TheDropdown")]
        private readonly CanvasGroup m_DropdownCanvasGroup;

        [UIElementDefaultVisibilityState(false)]
        [UIElementReference("DropdownEntry")]
        private readonly ModdedObject m_DropdownEntry;
        [UIElementReference("Content")]
        private readonly Transform m_Container;

        [UIElementReference("EmptyText")]
        private readonly GameObject m_EmptyText;

        private bool m_Expanded;
        public bool expanded
        {
            get
            {
                return m_Expanded;
            }
            set
            {
                m_Expanded = value;
                m_CollapseIcon.SetActive(!value);
                m_ExpandIcon.SetActive(value);
            }
        }

        public float targetYSize
        {
            get;
            set;
        } = 200f;

        public string selectedOptionText
        {
            get => m_SelectedOptionText.text;
            set => m_SelectedOptionText.text = value;
        }

        public List<Dropdown.OptionData> options
        {
            get;
            set;
        }

        private int m_Value;
        public int value
        {
            get => m_Value;
            set
            {
                m_Value = value;

                if (options.IsNullOrEmpty() || value >= options.Count)
                    return;
                selectedOptionText = options[value].text;
            }
        }

        public Action<int> onValueChanged
        {
            get;
            set;
        }

        public void Initialize()
        {
            UIController.AssignVariables(this);
            UIElementButton button = base.gameObject.AddComponent<UIElementButton>();
            button.AddOnClickListener(ExpandDropdown);
        }

        private void Update()
        {
            float deltaTime = Time.unscaledDeltaTime;
            float multipliedDeltaTime = deltaTime * 17.5f;
            float alpha = m_DropdownCanvasGroup.alpha;
            bool isExpanded = expanded;
            Vector2 sizeDelta = m_DropdownTransform.sizeDelta;

            if (isExpanded)
            {
                m_DropdownObject.SetActive(true);
                m_DropdownCanvasGroup.alpha = Mathf.Lerp(alpha, 1f, multipliedDeltaTime);
                sizeDelta.y = Mathf.Lerp(sizeDelta.y, targetYSize, multipliedDeltaTime);
            }
            else
            {
                m_DropdownCanvasGroup.alpha = Mathf.Lerp(alpha, 0f, multipliedDeltaTime);
                sizeDelta.y = Mathf.Lerp(sizeDelta.y, 30f, multipliedDeltaTime);

                if (alpha <= 0.05f)
                    m_DropdownObject.SetActive(false);
            }

            m_DropdownTransform.sizeDelta = sizeDelta;

            if (isExpanded && Input.GetMouseButtonDown(0) && !UIManager.Instance.IsMouseOverUIElement(m_DropdownObject))
            {
                CollapseDropdown();
            }
        }

        public void ExpandDropdown()
        {
            expanded = true;
            PopulateDropdown();
        }

        public void CollapseDropdown()
        {
            expanded = false;
        }

        public void PopulateDropdown()
        {
            TransformUtils.DestroyAllChildren(m_Container);
            if (options.IsNullOrEmpty())
            {
                m_EmptyText.SetActive(true);
                return;
            }
            m_EmptyText.SetActive(false);

            int index = 0;
            foreach (Dropdown.OptionData optionData in options)
            {
                ModdedObject moddedObject = Instantiate(m_DropdownEntry, m_Container);
                moddedObject.gameObject.SetActive(true);
                UIElementDropdownEntryDisplay display = moddedObject.gameObject.AddComponent<UIElementDropdownEntryDisplay>();
                display.Init(index, optionData, this);
                index++;
            }
        }

        private class UIElementDropdownEntryDisplay : OverhaulBehaviour
        {
            [UIElementReference("SelectedIndicator")]
            private readonly GameObject m_SelectedIndicator;
            [UIElementReference("Label")]
            private readonly Text m_Label;

            private Button m_Button;

            private UIElementDropdown m_Dropdown;
            private Dropdown.OptionData m_OptionData;
            private int m_Index;

            public void Init(int index, Dropdown.OptionData optionData, UIElementDropdown dropdown)
            {
                UIController.AssignVariables(this);
                m_Button = base.gameObject.GetComponent<Button>();
                m_Button.AddOnClickListener(Press);

                m_Dropdown = dropdown;
                m_OptionData = optionData;
                m_Index = index;

                m_SelectedIndicator.SetActive(dropdown.value == index);
                m_Label.text = optionData.text;
            }

            public void Press()
            {
                if (!m_Dropdown)
                    return;

                m_Dropdown.value = m_Index;
                m_Dropdown.onValueChanged?.Invoke(m_Index);
                m_Dropdown.CollapseDropdown();
            }
        }
    }
}
