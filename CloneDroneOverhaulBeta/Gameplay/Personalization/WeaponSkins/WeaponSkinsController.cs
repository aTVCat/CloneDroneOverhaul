using CDOverhaul.Gameplay.Pets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
