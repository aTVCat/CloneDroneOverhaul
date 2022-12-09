namespace CloneDroneOverhaul.UI.Components
{
    public class ValueBase
    {
        public bool Override;
        protected object _value;
        protected object _overrideVal;

        public void SetValue(object value, bool overrideVal)
        {
            if (overrideVal)
            {
                _overrideVal = value;
                return;
            }
            _value = value;
        }

        protected T getValue<T>()
        {
            T result = default(T);
            if ((Override ? _overrideVal : _value) != null)
            {
                result = (T)(Override ? _overrideVal : _value);
            }
            return result;
        }
    }

    public class BoolValue : ValueBase
    {
        public bool GetValue()
        {
            return (bool)getValue<bool>();
        }
    }

    public class FloatValue : ValueBase
    {
        public float GetValue()
        {
            return getValue<float>();
        }
    }

    public class IntValue : ValueBase
    {
        public int GetValue()
        {
            return getValue<int>();
        }
    }
}