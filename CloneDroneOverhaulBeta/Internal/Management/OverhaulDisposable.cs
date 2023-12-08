using System;
using System.Reflection;

namespace CDOverhaul
{
    [Serializable]
    public class OverhaulDisposable : IDisposable
    {
        [NonSerialized]
        public bool IsDisposed;

        ~OverhaulDisposable() => Dispose();
        protected virtual void OnDisposed() { }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            OnDisposed();
            IsDisposed = true;
            AssignNullToAllVars(this);
            GC.SuppressFinalize(this);
        }

        public static void AssignNullToAllVars(object @object)
        {
            /*
            foreach (FieldInfo field in @object.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                Type fieldType = field.FieldType;
                if (fieldType != typeof(bool) && fieldType != typeof(int) && fieldType != typeof(float))
                {
                    field.SetValue(@object, null);
                }
            }*/
        }
    }
}