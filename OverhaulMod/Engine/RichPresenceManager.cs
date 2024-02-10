namespace OverhaulMod.Engine
{
    public class RichPresenceManager : Singleton<RichPresenceManager>
    {
        public RichPresenceDiscord discord
        {
            get;
            private set;
        }

        private void Start()
        {
            discord = base.gameObject.AddComponent<RichPresenceDiscord>();
        }
    }
}
