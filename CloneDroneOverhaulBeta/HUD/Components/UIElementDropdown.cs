using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class UIElementDropdown : OverhaulBehaviour
    {
        [UIElementReference("SelectedOptionLabel")]
        private Text m_SelectedOptionText;

        [UIElementDefaultVisibilityState(true)]
        [UIElementReference("CollapsedIcon")]
        private GameObject m_CollapseIcon;
        [UIElementDefaultVisibilityState(false)]
        [UIElementReference("ExpandedIcon")]
        private GameObject m_ExpandIcon;

        [UIElementDefaultVisibilityState(false)]
        [UIElementReference("TheDropdown")]
        private GameObject m_DropdownObject;
        [UIElementReference("TheDropdown")]
        private RectTransform m_DropdownTransform;
        [UIElementReference("TheDropdown")]
        private CanvasGroup m_DropdownCanvasGroup;

        [UIElementDefaultVisibilityState(false)]
        [UIElementReference("DropdownEntry")]
        private ModdedObject m_DropdownEntry;
        [UIElementReference("Content")]
        private Transform m_Container;

        [UIElementReference("EmptyText")]
        private GameObject m_EmptyText;

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

        public override void Start()
        {
            UIController.AssignValues(this);
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

                if(alpha <= 0.05f)
                    m_DropdownObject.SetActive(false);
            }

            m_DropdownTransform.sizeDelta = sizeDelta;

            if(isExpanded && Input.GetMouseButtonDown(0) && !UIManager.Instance.IsMouseOverUIElement(m_DropdownObject))
            {
                CollapseDropdown();
            }
        }

        public void TestOptions()
        {
            options = new List<Dropdown.OptionData>()
            {
                new Dropdown.OptionData() { text = "Test 1"},
                new Dropdown.OptionData() { text = "Test 2"},
                new Dropdown.OptionData() { text = "Test 3"},
                new Dropdown.OptionData() { text = "Test 4"},
            };
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
            foreach(Dropdown.OptionData optionData in options)
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
            private GameObject m_SelectedIndicator;
            [UIElementReference("Label")]
            private Text m_Label;

            private Button m_Button;

            private UIElementDropdown m_Dropdown;
            private Dropdown.OptionData m_OptionData;
            private int m_Index;

            public void Init(int index, Dropdown.OptionData optionData, UIElementDropdown dropdown)
            {
                UIController.AssignValues(this);
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
