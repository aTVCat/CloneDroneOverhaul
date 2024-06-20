using OverhaulMod.Utils;
using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementVector3Field : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(OnXFieldChanged))]
        [UIElement("XField")]
        private readonly InputField m_xField;

        [UIElementAction(nameof(OnYFieldChanged))]
        [UIElement("YField")]
        private readonly InputField m_yField;

        [UIElementAction(nameof(OnZFieldChanged))]
        [UIElement("ZField")]
        private readonly InputField m_zField;

        private bool m_disableCallbacks;

        private Vector3 m_vector;
        public Vector3 vector
        {
            get
            {
                return m_vector;
            }
            set
            {
                m_vector = value;
                m_disableCallbacks = true;
                m_xField.text = value.x.ToString(CultureInfo.InvariantCulture);
                m_yField.text = value.y.ToString(CultureInfo.InvariantCulture);
                m_zField.text = value.z.ToString(CultureInfo.InvariantCulture);
                m_disableCallbacks = false;

                onValueChanged.Invoke(value);
            }
        }

        public Vector3ChangedEvent onValueChanged { get; set; } = new Vector3ChangedEvent();

        public void OnXFieldChanged(string val)
        {
            if (m_disableCallbacks)
                return;

            onXChanged(ModParseUtils.TryParseToFloat(val, 0f));
        }

        public void OnYFieldChanged(string val)
        {
            if (m_disableCallbacks)
                return;

            onYChanged(ModParseUtils.TryParseToFloat(val, 0f));
        }

        public void OnZFieldChanged(string val)
        {
            if (m_disableCallbacks)
                return;

            onZChanged(ModParseUtils.TryParseToFloat(val, 0f));
        }

        private void onXChanged(float val)
        {
            Vector3 vector1 = vector;
            vector1.x = val;
            m_vector = vector1;
            onValueChanged.Invoke(vector1);
        }

        private void onYChanged(float val)
        {
            Vector3 vector1 = vector;
            vector1.y = val;
            m_vector = vector1;
            onValueChanged.Invoke(vector1);
        }

        private void onZChanged(float val)
        {
            Vector3 vector1 = vector;
            vector1.z = val;
            m_vector = vector1;
            onValueChanged.Invoke(vector1);
        }

        [Serializable]
        public class Vector3ChangedEvent : UnityEvent<Vector3>
        {
        }
    }
}
