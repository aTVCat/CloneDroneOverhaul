using Bolt;

namespace OverhaulAPI.UnsortedClasses
{
    internal class MultiplayerEventListener : GlobalEventListener
    {
        public override void OnEvent(GenericStringForModdingEvent evnt)
        {
            MultiplayerAPI.OnEvent(evnt);
        }

        public override void Connected(BoltConnection connection)
        {
        }

        public override void Disconnected(BoltConnection connection)
        {
        }
    }
}
