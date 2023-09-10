using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul
{
    public class OverhaulSettingInfo
    {
        public OverhaulSettingInfo()
        {
            m_Attributes = new List<Attribute>();
        }

        private string m_Name;
        public string name
        {
            get => m_Name;
            private set => m_Name = value;
        }

        private string m_Section;
        public string section
        {
            get => m_Section;
            private set => m_Section = value;
        }

        private string m_Category;
        public string category
        {
            get => m_Category;
            private set => m_Category = value;
        }

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

        private Type m_FieldType;
        public Type fieldType
        {
            get => m_FieldType;
            private set => m_FieldType = value;
        }

        private EOverhaulSettingType m_SettingType;
        public EOverhaulSettingType settingType
        {
            get => m_SettingType;
            private set => m_SettingType = value;
        }

        private List<Attribute> m_Attributes;
        public List<Attribute> attributes
        {
            get => m_Attributes;
            private set => m_Attributes = value;
        }

        private object m_DefaultValue;
        public object defaultValue
        {
            get => m_DefaultValue;
            set => m_DefaultValue = value;
        }

        private bool m_IsHidden;
        public bool isHidden
        {
            get => m_IsHidden;
            set => m_IsHidden = value;
        }

        private EOverhaulSettingType getSettingType(Type type)
        {
            EOverhaulSettingType result;
            if (type == typeof(bool))
                result = EOverhaulSettingType.Bool;
            else if (type == typeof(int))
                result = EOverhaulSettingType.Int;
            else if (type == typeof(float))
                result = EOverhaulSettingType.Float;
            else if (type == typeof(string))
                result = EOverhaulSettingType.String;
            else if (type == typeof(KeyCode))
                result = EOverhaulSettingType.KeyCode;
            else
                result = EOverhaulSettingType.Other;
            return result;
        }

        public void AddAttribute(Attribute attribute) => m_Attributes.Add(attribute);
        public void AddAttributeRange(List<Attribute> attributes) => m_Attributes.AddRange(attributes);
        public T GetAttribute<T>() where T : Attribute
        {
            foreach (Attribute attribute in m_Attributes)
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
