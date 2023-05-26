using Steamworks;
using System;

namespace CDOverhaul.Workshop
{
    public class OverhaulWorkshopRequestResult : IDisposable
    {
        private bool m_IsDisposed;
        public bool IsDisposed()
        {
            return m_IsDisposed;
        }

        void IDisposable.Dispose()
        {
            GC.SuppressFinalize(this);
            m_IsDisposed = true;
            ItemsReceived = null;
        }
        ~OverhaulWorkshopRequestResult()
        {
            ((IDisposable)this).Dispose();
        }

        public OverhaulWorkshopRequestResult()
        {
            PageCount = 1;
        }

        public bool Error;

        public bool UserIsBanned;

        public SteamUGCQueryCompleted_t QueryCompleted;
        public OverhaulWorkshopItem[] ItemsReceived;
        public int PageCount;
    }
}