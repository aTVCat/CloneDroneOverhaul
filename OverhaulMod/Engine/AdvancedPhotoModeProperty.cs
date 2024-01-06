using System;
using System.Reflection;

namespace OverhaulMod.Engine
{
    public class AdvancedPhotoModeProperty
    {
        private object m_ogValue;
        private bool m_hasEverSavedValue;

        private readonly Type m_classType;
        private readonly string m_classMemberName;

        public string displayName
        {
            get;
            private set;
        }

        public string categoryName
        {
            get;
            private set;
        }

        public bool disallowSavingValue
        {
            get;
            private set;
        }

        private PropertyInfo m_propertyReference;
        public PropertyInfo propertyReference
        {
            get
            {
                if (m_propertyReference == null)
                {
                    m_propertyReference = m_classType.GetProperty(m_classMemberName, BindingFlags.Public | BindingFlags.Static);
                }
                return m_propertyReference;
            }
        }

        private FieldInfo m_fieldReference;
        public FieldInfo fieldReference
        {
            get
            {
                if (m_fieldReference == null)
                {
                    m_fieldReference = m_classType.GetField(m_classMemberName, BindingFlags.Public | BindingFlags.Static);
                }
                return m_fieldReference;
            }
        }

        private Type m_valueType;
        public Type valueType
        {
            get
            {
                if (m_valueType == null)
                {
                    FieldInfo fieldInfo = fieldReference;
                    if (fieldInfo != null)
                    {
                        m_valueType = fieldInfo.FieldType;
                    }
                    else
                    {
                        PropertyInfo propertyInfo = propertyReference;
                        if (propertyInfo != null)
                        {
                            m_valueType = propertyInfo.PropertyType;
                        }
                    }
                }
                return m_valueType;
            }
        }

        public AdvancedPhotoModeProperty(string displayName, string categoryName, bool disallowSavingValue, Type classType, string classMemberName)
        {
            m_classType = classType;
            m_classMemberName = classMemberName;
            this.displayName = displayName;
            this.categoryName = categoryName;
            this.disallowSavingValue = disallowSavingValue;
        }

        public void SetValue(object value)
        {
            FieldInfo fieldInfo = fieldReference;
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(null, value);
            }
            else
            {
                PropertyInfo propertyInfo = propertyReference;
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(null, value);
                }
            }
        }

        public object GetValue()
        {
            FieldInfo fieldInfo = fieldReference;
            if (fieldInfo != null)
            {
                return fieldInfo.GetValue(null);
            }
            else
            {
                PropertyInfo propertyInfo = propertyReference;
                if (propertyInfo != null)
                {
                    return propertyInfo.GetValue(null);
                }
            }
            return null;
        }

        public void SaveValue()
        {
            if (disallowSavingValue)
                return;

            m_ogValue = GetValue();
            m_hasEverSavedValue = true;
        }

        public void RestoreValue()
        {
            if (disallowSavingValue || !m_hasEverSavedValue)
                return;

            SetValue(m_ogValue);
        }
    }
}
