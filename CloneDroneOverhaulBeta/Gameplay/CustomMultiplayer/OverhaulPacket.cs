using System;

namespace CDOverhaul.CustomMultiplayer
{
    [Serializable]
    public class OverhaulPacket
    {
        public ulong Target;

        /// <summary>
        /// Called when we receive the packet
        /// </summary>
        public virtual void Handle() { }

        public virtual int GetChannel() => 0;
    }
}
