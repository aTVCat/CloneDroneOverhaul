using System;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Gameplay.Pets
{
    public class PetsController : PersonalizationItemsController
    {
        public static readonly List<PetItem> AllPetItems = new List<PetItem>();

        public static bool ItemsNeedRefresh
        {
            get;
            set;
        }

        public override void Initialize()
        {
            ItemsNeedRefresh = true;
            base.Initialize();
        }

        public override void AddItems()
        {
            if (!ItemsNeedRefresh)
                return;
            ItemsNeedRefresh = false;
            AllPetItems.Clear();

            AllPetItems.Add(PetItem.CreateNew("Developer Pet", "television_cat", "blah blah", string.Empty, null));
            PetItem.AttachModelQuick(OverhaulAssetsController.GetAsset("P_DeveloperPet", OverhaulAssetPart.Pets));

            AllPetItems.Add(PetItem.CreateNew("Tick Bot", "Doborog", "imatickbot", string.Empty, null));
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

        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover, bool hasInitializedModel)
        {
            if (!hasInitializedModel)
                return;

            _ = firstPersonMover.gameObject.AddComponent<PetsWearer>();
        }
    }
}
