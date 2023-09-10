namespace CDOverhaul.Gameplay.Pets
{
    public class PetSystem : PersonalizationItemsSystemBase
    {
        public const char SEPARATOR = ',';

        [OverhaulSettingAttribute_Old("Player.Pets.Equipped", "", !OverhaulVersion.IsDebugBuild)]
        public static string EquippedPets;

        [OverhaulSettingAttribute_Old("Player.Pets.EnemiesUsePets", false, !OverhaulVersion.IsDebugBuild)]
        public static bool AllowEnemiesUsePets;

        /*
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
        */

        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover, bool hasInitializedModel)
        {
            if (!hasInitializedModel)
                return;

            _ = firstPersonMover.gameObject.AddComponent<PetsWearer>();
        }

        public override string GetRepositoryFolder() => "Pets";

        public static void SetPetEquipped(PersonalizationItem item, bool equip, bool refreshPlayer = false)
        {
            if (item == null || (!item.IsUnlocked() && equip))
                return;

            string petSaveString = item.GetID() + SEPARATOR;
            bool isEquipped = EquippedPets.Contains(petSaveString);

            if (isEquipped && !equip)
                EquippedPets = EquippedPets.Replace(petSaveString, string.Empty);
            else if (!isEquipped && equip)
                EquippedPets += petSaveString;

            SavePreferences();
            if (refreshPlayer)
                CharacterTracker.Instance.GetPlayerRobot().RefreshPersonalizationItems();
        }

        public void SetPetEquipped(string item, bool equip) => SetPetEquipped(GetItem(item), equip);
    }
}
