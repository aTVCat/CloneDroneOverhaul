namespace CDOverhaul
{
    /// <summary>
    /// A gamemode in gamemode?
    /// </summary>
    public class GamemodeSubstatesController : ModController
    {
        public const string SubstateChangedEventString = "GamemodeSubstateChanged";

        private EGamemodeSubstate _gamemodeSubstate;
        public EGamemodeSubstate GamemodeSubstate
        {
            get => _gamemodeSubstate;
            set
            {
                EGamemodeSubstate _prevState = _gamemodeSubstate;
                _gamemodeSubstate = value;
                if (value != _prevState)
                {
                    OverhaulEventManager.DispatchEvent(SubstateChangedEventString);
                }
            }
        }

        public override void Initialize()
        {
            OverhaulEventManager.AddEvent(SubstateChangedEventString);

            HasAddedEventListeners = true;
            IsInitialized = true;
        }
    }
}
