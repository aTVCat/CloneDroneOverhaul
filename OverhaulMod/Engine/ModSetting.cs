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
            ModSettingsDataManager modSettingsDataManager = ModSettingsDataManager.Instance;

            FieldInfo fieldInfo = this.Field;
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(null, value);
            }

            string key = ID;
            switch (ValueType)
            {
                case ValueTypes.Bool:
                    modSettingsDataManager.SetInt(key, (bool)value ? 1 : 0);
                    break;
                case ValueTypes.Int:
                    modSettingsDataManager.SetInt(key, (int)value);
                    break;
                case ValueTypes.Float:
                    modSettingsDataManager.SetFloat(key, (float)value);
                    break;
                case ValueTypes.String:
                    modSettingsDataManager.SetString(key, (string)value);
                    break;
            }

            if (!_valueChangedListeners.IsNullOrEmpty())
            {
                foreach (Action<object> a in _valueChangedListeners)
                {
                    try
                    {
                        a?.Invoke(value);
                    }
                    catch (Exception e)
                    {
                        ModDebug.Exception(e);
                    }
                }
            }

            GlobalEventManager.Instance.Dispatch(ModSettingsManager.SETTING_CHANGED_EVENT);
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
