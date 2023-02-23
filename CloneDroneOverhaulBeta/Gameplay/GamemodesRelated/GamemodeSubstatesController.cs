namespace CDOverhaul
{
    /// <summary>
    /// A gamemode in gamemode?
    /// </summary>
    public class GamemodeSubstatesController : ModController
    {
        public const string SubstateChangedEventString = "GamemodeSubstateChanged";

        private GamemodeSubstate _gamemodeSubstate;
        public GamemodeSubstate GamemodeSubstate
        {
            get => _gamemodeSubstate;
            set
            {
                GamemodeSubstate _prevState = _gamemodeSubstate;
                _gamemodeSubstate = value;

                if (_gamemodeSubstate != _prevState)
                {
                    OverhaulEventManager.DispatchEvent(SubstateChangedEventString);
                }
            }
        }
    }
}
