using System;
using System.Reflection;

namespace CDOverhaul
{
    public class OverhaulDisposable : IDisposable
    {
        public bool IsDisposed
        {
            get;
            private set;
        }

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
            foreach (FieldInfo field in @object.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                Type fieldType = field.FieldType;
                if (fieldType != typeof(bool) && fieldType != typeof(int) && fieldType != typeof(float))
                {
                    field.SetValue(@object, null);
                }
            }

            OverhaulDisposable overhaulDisposable = @object as OverhaulDisposable;
            if(overhaulDisposable != null && !overhaulDisposable.IsDisposed)
            {
                overhaulDisposable.Dispose();
            }
        }
    }
}