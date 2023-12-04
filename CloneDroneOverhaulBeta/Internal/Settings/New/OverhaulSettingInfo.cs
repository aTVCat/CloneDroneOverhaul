using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CDOverhaul
{
    public class OverhaulSettingInfo
    {
        public OverhaulSettingInfo()
        {
            attributes = new List<Attribute>();
        }

        public string name { get; private set; }
        public string section { get; private set; }
        public string category { get; private set; }

        private string m_Path;
        public string path
        {
            get => m_Path;
            set
            {
                m_Path = value;

                string[] split = value.Split('.');
                category = split[0];
                section = split[1];
                name = split[2];
            }
        }

        private FieldInfo m_FieldReference;
        public FieldInfo fieldReference
        {
            get => m_FieldReference;
            set
            {
                m_FieldReference = value;

                fieldType = value.FieldType;
                settingType = getSettingType(value.FieldType);
            }
        }

        public Type fieldType { get; private set; }
        public EOverhaulSettingType settingType { get; private set; }
        public List<Attribute> attributes { get; private set; }
        public object defaultValue { get; set; }
        public bool isHidden { get; set; }

        private List<Action> m_OnValueSavedCallbacks;
        public event Action onValueSavedCallback
        {
            add
            {
                if (m_OnValueSavedCallbacks == null)
                    m_OnValueSavedCallbacks = new List<Action>();

                if (m_OnValueSavedCallbacks.Contains(value))
                    return;

                m_OnValueSavedCallbacks.Add(value);
            }
            remove
            {
                _ = m_OnValueSavedCallbacks.Remove(value);
            }
        }

        private EOverhaulSettingType getSettingType(Type type)
        {
            EOverhaulSettingType result;
            if (type == typeof(bool))
                result = EOverhaulSettingType.Bool;
            else result = type == typeof(int)
                ? EOverhaulSettingType.Int
                : type == typeof(float)
                ? EOverhaulSettingType.Float
                : type == typeof(string)
                ? EOverhaulSettingType.String
                : type == typeof(KeyCode) ? EOverhaulSettingType.KeyCode : EOverhaulSettingType.Other;
            return result;
        }

        public T GetValue<T>()
        {
            string path = "Overhaul." + this.path;
            object result = default;
            switch (settingType)
            {
                case EOverhaulSettingType.Bool:
                    result = PlayerPrefs.GetInt(path, (bool)defaultValue ? 1 : 0) == 1;
                    break;
                case EOverhaulSettingType.Int:
                    result = PlayerPrefs.GetInt(path, (int)defaultValue);
                    break;
                case EOverhaulSettingType.Float:
                    result = PlayerPrefs.GetFloat(path, (float)defaultValue);
                    break;
                case EOverhaulSettingType.String:
                    result = PlayerPrefs.GetString(path, (string)defaultValue);
                    break;
                case EOverhaulSettingType.KeyCode:
                    result = (KeyCode)PlayerPrefs.GetInt(path, (int)(KeyCode)defaultValue);
                    break;
            }
            return (T)result;
        }

        public void SetValue(object value)
        {
            fieldReference.SetValue(null, value);
            OverhaulEvents.DispatchEvent(OverhaulSettingsManager_Old.SETTING_VALUE_UPDATED_EVENT);
        }

        public void SetValue(object value, bool save)
        {
            SetValue(value);
            if (save)
                SaveValue();
        }

        public void ResetValue()
        {
            SetValue(defaultValue, true);
        }

        public void SaveValue()
        {
            string path = "Overhaul." + this.path;
            object toSave = fieldReference.GetValue(null);
            Type type = toSave.GetType();

            if (type == typeof(bool))
                PlayerPrefs.SetInt(path, (bool)toSave ? 1 : 0);
            else if (type == typeof(int))
                PlayerPrefs.SetInt(path, (int)toSave);
            else if (type == typeof(float))
                PlayerPrefs.SetFloat(path, (float)toSave);
            else if (type == typeof(string))
                PlayerPrefs.SetString(path, (string)toSave);
            else if (type == typeof(KeyCode))
                PlayerPrefs.SetInt(path, (int)(KeyCode)toSave);

            if (m_OnValueSavedCallbacks.IsNullOrEmpty())
                return;

            List<int> toRemove = new List<int>();
            int index = 0;
            foreach (Action action in m_OnValueSavedCallbacks)
            {
                if (action.Target != null)
                {
                    action();
                }
                else
                {
                    toRemove.Add(index);
                }
                index++;
            }

            foreach (int toRemoveIndex in toRemove)
            {
                m_OnValueSavedCallbacks.RemoveAt(toRemoveIndex);
            }
        }

        public void AddAttribute(Attribute attribute) => attributes.Add(attribute);
        public void AddAttributeRange(List<Attribute> attributes) => this.attributes.AddRange(attributes);
        public T GetAttribute<T>() where T : Attribute
        {
            foreach (Attribute attribute in attributes)
            {
                if (attribute.GetType() == typeof(T))
                    return (T)attribute;
            }
            return null;
        }
        public bool HasAttribute<T>() where T : Attribute
        {
            return GetAttribute<T>() != null;
        }
    }
}
