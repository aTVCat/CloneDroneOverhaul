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
        private readonly InputField _xField;

        [UIElementAction(nameof(OnYFieldChanged))]
        [UIElement("YField")]
        private readonly InputField _yField;

        [UIElementAction(nameof(OnZFieldChanged))]
        [UIElement("ZField")]
        private readonly InputField _zField;

        private bool _disableCallbacks;

        private Vector3 _vector;
        public Vector3 vector
        {
            get
            {
                return _vector;
            }
            set
            {
                _vector = value;
                _disableCallbacks = true;
                _xField.text = value.x.ToString(CultureInfo.InvariantCulture);
                _yField.text = value.y.ToString(CultureInfo.InvariantCulture);
                _zField.text = value.z.ToString(CultureInfo.InvariantCulture);
                _disableCallbacks = false;

                onValueChanged.Invoke(value);
            }
        }

        public Vector3ChangedEvent onValueChanged { get; set; } = new Vector3ChangedEvent();

        public void OnXFieldChanged(string val)
        {
            if (_disableCallbacks)
                return;

            onXChanged(ModParseUtils.TryParseFloat(val, 0f));
        }

        public void OnYFieldChanged(string val)
        {
            if (_disableCallbacks)
                return;

            onYChanged(ModParseUtils.TryParseFloat(val, 0f));
        }

        public void OnZFieldChanged(string val)
        {
            if (_disableCallbacks)
                return;

            onZChanged(ModParseUtils.TryParseFloat(val, 0f));
        }

        private void onXChanged(float val)
        {
            Vector3 vector1 = vector;
            vector1.x = val;
            _vector = vector1;
            onValueChanged.Invoke(vector1);
        }

        private void onYChanged(float val)
        {
            Vector3 vector1 = vector;
            vector1.y = val;
            _vector = vector1;
            onValueChanged.Invoke(vector1);
        }

        private void onZChanged(float val)
        {
            Vector3 vector1 = vector;
            vector1.z = val;
            _vector = vector1;
            onValueChanged.Invoke(vector1);
        }

        [Serializable]
        public class Vector3ChangedEvent : UnityEvent<Vector3>
        {
        }
    }
}
