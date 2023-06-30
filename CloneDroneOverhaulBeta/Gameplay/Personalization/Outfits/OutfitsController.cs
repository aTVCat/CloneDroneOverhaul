using System.Collections.Generic;

namespace CDOverhaul.Gameplay.Outfits
{
    public class OutfitsController : OverhaulGameplayController
    {
        public static readonly List<OutfitItem> AllOutfitItems = new List<OutfitItem>();

        public override void Initialize()
        {
            base.Initialize();

            if (OverhaulSessionController.GetKey<bool>("HasAddedAccessories"))
                return;

            OverhaulSessionController.SetKey("HasAddedAccessories", true);
            addOutfitItems();
        }

        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover, bool hasInitializedModel)
        {
            if (!hasInitializedModel)
                return;

            _ = firstPersonMover.gameObject.AddComponent<OutfitsWearer>();
        }

        private void addOutfitItems()
        {
            AddOutfitItemQuick("Igrok's hat", "P_Acc_Head_Igrok's hat", MechBodyPartType.Head);
            SetAuthorQuick(WeaponSkinsController.ATVCatDiscord);

            AddOutfitItemQuick("Halo", "P_Acc_Head_ImpostorHalo", MechBodyPartType.Head);
            SetAuthorQuick(WeaponSkinsController.ATVCatDiscord);

            AddOutfitItemQuick("Puss Hat", "P_Acc_Head_PussHat", MechBodyPartType.Head);
            SetAuthorQuick(WeaponSkinsController.TabiDiscord);

            AddOutfitItemQuick("Cone", "P_Acc_Head_ImpostorCone", MechBodyPartType.Head);
            SetAuthorQuick(WeaponSkinsController.ATVCatDiscord);

            AddOutfitItemQuick("Deal with It", "P_Acc_DealWithIt", MechBodyPartType.Head);
            SetAuthorQuick(WeaponSkinsController.CaptainMeowDiscord);

            AddOutfitItemQuick("Horns", "P_Horns", MechBodyPartType.Head);
            SetAuthorQuick(WeaponSkinsController.ZoloRDiscord);
        }

        /// <summary>
        /// Add an outfit item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="accessoryName"></param>
        /// <param name="assetName"></param>
        /// <param name="accessoryType"></param>
        /// <param name="accessoryBodyPart"></param>
        /// <param name="descriptionFile"></param>
        public static void AddOutfitItemQuick(string accessoryName, string assetName, MechBodyPartType accessoryBodyPart)
        {
            OutfitItem item = OutfitItem.NewAccessory(accessoryName, accessoryBodyPart);
            item.Prefab = OverhaulAssetsController.GetAsset(assetName, OverhaulAssetPart.Accessories);
            item.SetUpOffsets();
            AllOutfitItems.Add(item);
        }

        public void SetAuthorQuick(string author)
        {
            if (AllOutfitItems.IsNullOrEmpty())
                return;

            OutfitItem item = AllOutfitItems[AllOutfitItems.Count - 1];
            if (item != null)
                item.Author = author;
        }

        public void SetDescriptionQuick(string description)
        {
            if (AllOutfitItems.IsNullOrEmpty())
                return;

            OutfitItem item = AllOutfitItems[AllOutfitItems.Count - 1];
            if (item != null)
                item.Description = description;
        }

        /// <summary>
        /// Get outfit item by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="returnNullIfLocked"></param>
        /// <returns></returns>
        public static OutfitItem GetOutfitItem(string name, bool returnNullIfLocked = true)
        {
            bool canSearchThrough = !AllOutfitItems.IsNullOrEmpty() && !string.IsNullOrEmpty(name);
            if (!canSearchThrough)
                return null;

            OutfitItem result = null;
            int i = 0;
            do
            {
                OutfitItem item = AllOutfitItems[i];
                if (name.Equals(item.Name))
                {
                    if (!item.IsUnlocked() && returnNullIfLocked)
                        return null;

                    result = item;
                    break;
                }
                i++;
            } while (i < AllOutfitItems.Count);

            return result;
        }

        public static List<OutfitItem> GetOutfitItems(string itemsString)
        {
            List<OutfitItem> result = new List<OutfitItem>();
            bool shouldSearchEquipped = !string.IsNullOrEmpty(itemsString) && itemsString.Contains(Separator.ToString()) && !AllOutfitItems.IsNullOrEmpty();
            if (!shouldSearchEquipped)
                return result;

            foreach (OutfitItem item in AllOutfitItems)
            {
                if (itemsString.Contains(item.Name))
                {
                    OutfitItem aItem = GetOutfitItem(item.Name, false);
                    if (aItem == null)
                        continue;

                    result.Add(aItem);
                }
            }
            return result;
        }

        #region Save data

        [OverhaulSetting("Player.Outfits.Equipped", "", !OverhaulVersion.IsDebugBuild)]
        public static string EquippedAccessories;

        public const char Separator = ',';

        public static void SavePreferences()
        {
            SettingInfo info = OverhaulSettingsController.GetSetting("Player.Outfits.Equipped", true);
            if (info == null)
                return;

            SettingInfo.SavePref(info, EquippedAccessories);
        }

        public static List<OutfitItem> GetEquippedAccessories() => GetOutfitItems(EquippedAccessories);

        public static void SetAccessoryEquipped(OutfitItem item, bool equip)
        {
            if (item == null || (!item.IsUnlocked() && equip))
                return;

            string accessorySaveString = item.Name + Separator;
            bool isEquipped = EquippedAccessories.Contains(accessorySaveString);
            if (isEquipped && !equip)
                EquippedAccessories = EquippedAccessories.Replace(accessorySaveString, string.Empty);

            else if (!isEquipped && equip)
                EquippedAccessories += accessorySaveString;

            SavePreferences();
        }

        public static void SetAccessoryEquipped(string item, bool equip) => SetAccessoryEquipped(GetOutfitItem(item, equip), equip);

        #endregion
    }
}