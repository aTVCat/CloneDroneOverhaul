using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace OverhaulMod.Engine
{
    public class ModSetting
    {
        public string ID;

        public object DefaultValue;

        public Tags Tag;

        public ValueTypes ValueType;

        public FieldInfo Field;

        public bool RequiresRestarting;

        private List<Action<object>> _valueChangedListeners;
        public event Action<object> ValueChangedEvent
        {
            add
            {
                if (_valueChangedListeners == null) _valueChangedListeners = new List<Action<object>>();
                _valueChangedListeners.Add(value);
            }
            remove
            {
                if (_valueChangedListeners == null) _valueChangedListeners = new List<Action<object>>();
                _ = _valueChangedListeners.Remove(value);
            }
        }

        public bool _hasNotifiedAboutRestarting;

        public object GetValue()
        {
            ModSettingsDataManager modSettingsDataManager = ModSettingsDataManager.Instance;

            string key = ID;
            object result;
            switch (ValueType)
            {
                case ValueTypes.Bool:
                    result = modSettingsDataManager.GetInt(key, (bool)DefaultValue ? 1 : 0) == 1;
                    break;
                case ValueTypes.Int:
                    result = modSettingsDataManager.GetInt(key, (int)DefaultValue);
                    break;
                case ValueTypes.Float:
                    result = modSettingsDataManager.GetFloat(key, (float)DefaultValue);
                    break;
                case ValueTypes.String:
                    result = modSettingsDataManager.GetString(key, (string)DefaultValue);
                    break;
                default:
                    result = default;
                    break;
            }
            return result;
        }

        public object GetFieldValue()
        {
            FieldInfo fieldInfo = this.Field;
            return fieldInfo == null ? null : fieldInfo.GetValue(null);
        }

        public void SetValue(object value)
        {
            Field.SetValue(null, value);

            switch (ValueType)
            {
                case ValueTypes.Bool:
                    ModSettingsDataManager.Instance.SetInt(ID, (bool)value ? 1 : 0);
                    break;
                case ValueTypes.Int:
                    ModSettingsDataManager.Instance.SetInt(ID, (int)value);
                    break;
                case ValueTypes.Float:
                    ModSettingsDataManager.Instance.SetFloat(ID, (float)value);
                    break;
                case ValueTypes.String:
                    ModSettingsDataManager.Instance.SetString(ID, (string)value);
                    break;
            }

            if (_valueChangedListeners.IsNullOrEmpty()) return;

            foreach (Action<object> action in _valueChangedListeners)
            {
                try
                {
                    if (action != null) action(value);
                }
                catch (Exception e)
                {
                    ModDebug.Exception(e);
                }
            }
        }

        public void SetValueFromUI(object value)
        {
            SetValue(value);
            if (RequiresRestarting && !_hasNotifiedAboutRestarting)
            {
                _ = ModUIs.ShowRestartRequiredScreen(true);
                _hasNotifiedAboutRestarting = true;
            }
        }

        public void SetBoolValue(bool value)
        {
            if (ValueType == ValueTypes.Bool)
                SetValue(value);
        }

        public void SetIntValue(int value)
        {
            if (ValueType == ValueTypes.Int)
                SetValue(value);
        }

        public void SetFloatValue(float value)
        {
            if (ValueType == ValueTypes.Float)
                SetValue(value);
        }

        public void SetStringValue(string value)
        {
            if (ValueType == ValueTypes.String)
                SetValue(value);
        }

        public enum Tags
        {
            None,
            UISetting,
            IgnoreExport,
        }

        public enum ValueTypes
        {
            None,
            Bool,
            Int,
            Float,
            String
        }
    }
}
