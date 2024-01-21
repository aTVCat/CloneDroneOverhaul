using System;
using System.Collections;

namespace CDOverhaul.Gameplay.Multiplayer
{
    [Serializable]
    public class OverhaulPlayerInfoRefreshEventData : OverhaulDisposable
    {
        public const string RECEIVER_EVERYONE = "everyone";

        public string SenderPlayFabID;
        public string ReceiverPlayFabID;

        public Hashtable Hashtable;

        public bool IsAnswer;
        public bool IsRequest;
    }
}
