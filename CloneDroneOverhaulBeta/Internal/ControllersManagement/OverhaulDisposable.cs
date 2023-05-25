using System;

namespace CDOverhaul
{
    public class OverhaulDisposable : IDisposable
    {
        public bool IsDisposed
        {
            get;
            private set;
        }

        protected virtual void OnDisposed()
        {
        }

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }
            OnDisposed();
            IsDisposed = true;
            GC.SuppressFinalize(this);
        }

        ~OverhaulDisposable()
        {
            Dispose();
        }
    }
}