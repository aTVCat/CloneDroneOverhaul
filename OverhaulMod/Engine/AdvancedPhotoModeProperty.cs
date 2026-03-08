using System;
using System.Reflection;

namespace OverhaulMod.Engine
{
    public class AdvancedPhotoModeProperty
    {
        public Type classType { get; }

        public string classMemberName { get; }

        private PropertyInfo _propertyReference;
        public PropertyInfo propertyReference
        {
            get
            {
                if (_propertyReference == null)
                {
                    _propertyReference = classType.GetProperty(classMemberName, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                }
                return _propertyReference;
            }
        }

        private FieldInfo _fieldReference;
        public FieldInfo fieldReference
        {
            get
            {
                if (_fieldReference == null)
                {
                    _fieldReference = classType.GetField(classMemberName, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                }
                return _fieldReference;
            }
        }

        private Type _valueType;
        public Type valueType
        {
            get
            {
                if (_valueType == null)
                {
                    FieldInfo fieldInfo = fieldReference;
                    if (fieldInfo != null)
                    {
                        _valueType = fieldInfo.FieldType;
                    }
                    else
                    {
                        PropertyInfo propertyInfo = propertyReference;
                        if (propertyInfo != null)
                        {
                            _valueType = propertyInfo.PropertyType;
                        }
                    }
                }
                return _valueType;
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
