using System;
using System.Collections;
using UnityEngine;
using Steamworks;

namespace CDOverhaul.Workshop
{
    public class OverhaulWorkshopRequestResult : IDisposable
    {
        private bool m_IsDisposed;
        public bool IsDisposed() => m_IsDisposed;

        void IDisposable.Dispose()
        {
            GC.SuppressFinalize(this);
            m_IsDisposed = true;
        }
        ~OverhaulWorkshopRequestResult()
        {
            ((IDisposable)this).Dispose();
        }

        public bool Error;

        public bool UserIsBanned;

        public SteamUGCQueryCompleted_t QueryCompleted;
        public OverhaulWorkshopItem[] ItemsReceived;
    }
}