using System;
using System.Reflection;

namespace OverhaulMod.Engine
{
    public class AdvancedPhotoModeProperty
    {
        public Type classType { get; }

        public string classMemberName { get; }

        private PropertyInfo m_propertyReference;
        public PropertyInfo propertyReference
        {
            get
            {
                if (m_propertyReference == null)
                {
                    m_propertyReference = classType.GetProperty(classMemberName, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
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
                    m_fieldReference = classType.GetField(classMemberName, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
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

        public Func<object> objectInstanceFunction { get; set; }

        public object objectInstance
        {
            get
            {
                Func<object> f = objectInstanceFunction;
                return f == null ? null : f();
            }
        }

        public object moddedValue
        {
            get;
            set;
        }

        public AdvancedPhotoModeProperty(Type classType, string classMemberName, Func<object> instanceFunction = null)
        {
            this.classType = classType;
            this.classMemberName = classMemberName;
            objectInstanceFunction = instanceFunction;
        }

        public void SetValue(object value)
        {
            moddedValue = value;
            FieldInfo fieldInfo = fieldReference;
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(objectInstance, value);
            }
            else
            {
                PropertyInfo propertyInfo = propertyReference;
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(objectInstance, value);
                }
            }
        }

        public object GetValue()
        {
            FieldInfo fieldInfo = fieldReference;
            if (fieldInfo != null)
            {
                return fieldInfo.GetValue(objectInstance);
            }
            else
            {
                PropertyInfo propertyInfo = propertyReference;
                if (propertyInfo != null)
                {
                    return propertyInfo.GetValue(objectInstance);
                }
            }
            return null;
        }

        public void SetModdedValue()
        {
            SetValue(moddedValue);
        }
    }
}
