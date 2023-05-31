using Steamworks;
using System;

namespace CDOverhaul.Workshop
{
    public class OverhaulWorkshopRequestResult : OverhaulDisposable
    {
        protected override void OnDisposed()
        {
            OverhaulDisposable.AssignNullToAllVars(this);
        }

        public OverhaulWorkshopRequestResult() => PageCount = 1;

        public bool Error;

        public bool UserIsBanned;

        public SteamUGCQueryCompleted_t QueryCompleted;
        public OverhaulWorkshopItem[] ItemsReceived;
        public int PageCount;
    }
}