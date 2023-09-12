using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class UIElementSettingsMenuToggle : OverhaulBehaviour
    {
        [UIElementReference("EnabledGraphic")]
        private GameObject m_EnabledStateObjects;
        [UIElementReference("HandleOn")]
        private Graphic m_EnabledStateHandle;

        [UIElementReference("DisabledGraphic")]
        private GameObject m_DisabledStateObjects;
        [UIElementReference("HandleOff")]
        private Graphic m_DisabledStateHandle;

        private Button m_Button;

        private bool m_IsOn;
        public bool isOn
        {
            get => m_IsOn;
            set
            {
                m_IsOn = value;
                m_DisabledStateObjects.SetActive(!value);
                m_EnabledStateObjects.SetActive(value);
                m_Button.targetGraphic = value ? m_EnabledStateHandle : m_DisabledStateHandle;
            }
        }

        private UnityAction<bool> m_OnValueChanged;
        public UnityAction<bool> onValueChanged
        {
            get => m_OnValueChanged;
            set => m_OnValueChanged = value;
        }

        public override void Awake()
        {
            m_Button = base.GetComponent<Button>();
            m_Button.AddOnClickListener(OnClicked);
            UIController.AssignValues(this);
        }

        public void OnClicked()
        {
            isOn = !isOn;
            if(onValueChanged != null)
            {
                onValueChanged(isOn);
            }
        }
    }
}
