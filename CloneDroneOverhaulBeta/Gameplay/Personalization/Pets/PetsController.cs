using CDOverhaul.Gameplay.Outfits;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Gameplay.Pets
{
    public class PetsController : PersonalizationItemsController
    {
        public const char SEPARATOR = ',';

        [OverhaulSetting("Player.Pets.Equipped", "", !OverhaulVersion.IsDebugBuild)]
        public static string EquippedPets;

        [OverhaulSetting("Player.Pets.EnemiesUsePets", false, !OverhaulVersion.IsDebugBuild)]
        public static bool AllowEnemiesUsePets;

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

        public static PetItem GetPetItem(string name, bool returnNullIfLocked = true)
        {
            bool canSearchThrough = !AllPetItems.IsNullOrEmpty() && !string.IsNullOrEmpty(name);
            if (!canSearchThrough)
                return null;

            PetItem result = null;
            int i = 0;
            do
            {
                PetItem item = AllPetItems[i];
                if (name == item.Name)
                {
                    if (!item.IsUnlocked() && returnNullIfLocked)
                        return null;

                    result = item;
                    break;
                }
                i++;
            } while (i < AllPetItems.Count);
            return result;
        }

        public static List<PetItem> GetPetItemsBySaveString(string itemsString)
        {
            List<PetItem> result = new List<PetItem>();
            bool shouldSearchEquipped = !string.IsNullOrEmpty(itemsString) && itemsString.Contains(SEPARATOR.ToString()) && !AllPetItems.IsNullOrEmpty();
            if (!shouldSearchEquipped)
                return result;

            foreach (PetItem item in AllPetItems)
            {
                if (itemsString.Contains(item.Name))
                {
                    PetItem aItem = GetPetItem(item.Name, false);
                    if (aItem == null)
                        continue;

                    result.Add(aItem);
                }
            }
            return result;
        }

        public static void SetPetEquipped(PetItem item, bool equip, bool refreshPlayer = false)
        {
            if (item == null || (!item.IsUnlocked() && equip))
                return;

            string petSaveString = item.Name + SEPARATOR;
            bool isEquipped = EquippedPets.Contains(petSaveString);
            if (isEquipped && !equip)
                EquippedPets = EquippedPets.Replace(petSaveString, string.Empty);

            else if (!isEquipped && equip)
                EquippedPets += petSaveString;

            SavePreferences();
            if (refreshPlayer)
            {
                FirstPersonMover firstPersonMover = CharacterTracker.Instance.GetPlayerRobot();
                if (firstPersonMover)
                {
                    PetsWearer petsWearer = firstPersonMover.GetComponent<PetsWearer>();
                    if (petsWearer)
                    {
                        petsWearer.RefreshItems();
                    }
                }
            }
        }

        public static void SetPetEquipped(string item, bool equip) => SetPetEquipped(GetPetItem(item, equip), equip);
    }
}
