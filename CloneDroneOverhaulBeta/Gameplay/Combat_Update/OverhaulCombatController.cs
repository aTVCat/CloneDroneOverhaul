namespace CDOverhaul.Gameplay.Combat_Update
{
    public class OverhaulCombatController : ModController
    {
        public static bool AllowNewCombat => !GameModeManager.IsMultiplayer();

        public override void Initialize()
        {
            HasAddedEventListeners = true;
            HasInitialized = true;
        }
    }
}
