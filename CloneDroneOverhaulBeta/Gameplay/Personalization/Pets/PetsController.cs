using System;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Gameplay.Pets
{
    public class PetsController : OverhaulGameplayController
    {
        public static readonly List<PetItem> Pets = new List<PetItem>();

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover, bool hasInitializedModel)
        {
            if (!hasInitializedModel)
                return;

            _ = firstPersonMover.gameObject.AddComponent<PetsWearer>();
        }

        public void AddPets()
        {
            if (OverhaulSessionController.GetKey<bool>("hasInitialized"))
                return;

            OverhaulSessionController.SetKey("hasInitialized", true);

            Pets.Add(PetItem.CreateNew("Developer Pet", "television_cat", "blah blah", string.Empty, null));
            PetItem.AttachModelQuick(OverhaulAssetsController.GetAsset("P_DeveloperPet", OverhaulAssetPart.Pets));

            Pets.Add(PetItem.CreateNew("Tick Bot", "Doborog", "imatickbot", string.Empty, null));
            PetItem.AttachModelQuick(OverhaulAssetsController.GetAsset("P_TickBot", OverhaulAssetPart.Pets));
            PetItem.GetBehaviourSettingsQuick().OffsetTargetPositionNodes = new Tuple<string, UnityEngine.Vector3>[]
            {
                new Tuple<string, UnityEngine.Vector3>(TargetPositionNodes.Offset, new UnityEngine.Vector3(0, 2.7f, 0)),
                new Tuple<string, UnityEngine.Vector3>(TargetPositionNodes.OwnerTransformRight, new UnityEngine.Vector3(1.5f, 1.5f))
            };
            PetItem.GetBehaviourSettingsQuick().OffsetScale = Vector3.one * 0.7f;
            PetItem.GetBehaviourSettingsQuick().FollowHeadRotation = true;
            PetItem.GetBehaviourSettingsQuick().RangeToLookAtEnemy = 20f;
        }
    }
}
