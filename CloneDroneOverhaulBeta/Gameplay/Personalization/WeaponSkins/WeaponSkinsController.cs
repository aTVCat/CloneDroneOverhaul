using System.Collections.Generic;

namespace CDOverhaul.Gameplay.WeaponSkins
{
    /// <summary>
    /// Another new skins controller that should work better this time
    /// </summary>
    public class WeaponSkinsController : OverhaulGameplayController
    {
        public static readonly List<WeaponSkinItem> Skins = new List<WeaponSkinItem>();

        public override void Initialize()
        {
            base.Initialize();

            if (OverhaulSessionController.GetKey<bool>("hasInitialized"))
                return;

            OverhaulSessionController.SetKey("hasInitialized", true);
        }

        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover, bool hasInitializedModel)
        {
            if (!hasInitializedModel)
                return;
        }
    }
}
