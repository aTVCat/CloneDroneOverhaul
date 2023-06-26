using OverhaulAPI;
using System.Collections.Generic;

namespace CDOverhaul.Gameplay.Outfits
{
    public class OutfitsController : OverhaulGameplayController
    {
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

        #endregion

        public static List<AccessoryItem> AllAccessories { get; } = new List<AccessoryItem>();

        public static AccessoryItem EditingItem;
        public static string EditingCharacterModel;
        public static ModelOffset CopiedModelOffset;

        public override void Initialize()
        {
            base.Initialize();

            if (!OverhaulSessionController.GetKey<bool>("HasAddedAccessories"))
            {
                OverhaulSessionController.SetKey("HasAddedAccessories", true);
                addAccessories();
            }
        }

        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover, bool hasInitializedModel)
        {
            if (!hasInitializedModel)
                return;

            _ = firstPersonMover.gameObject.AddComponent<OutfitsWearer>();
        }

        private void addAccessories() // Todo: Make an editor like weapon skins one
        {
            AddAccessory<DefaultAccessoryItem>("Igrok's hat", "P_Acc_Head_Igrok's hat", MechBodyPartType.Head);
            SetAuthor(WeaponSkinsController.ATVCatDiscord);

            AddAccessory<DefaultAccessoryItem>("Halo", "P_Acc_Head_ImpostorHalo", MechBodyPartType.Head);
            SetAuthor(WeaponSkinsController.ATVCatDiscord);

            AddAccessory<DefaultAccessoryItem>("Puss Hat", "P_Acc_Head_PussHat", MechBodyPartType.Head);
            SetAuthor(WeaponSkinsController.TabiDiscord);

            AddAccessory<DefaultAccessoryItem>("Cone", "P_Acc_Head_ImpostorCone", MechBodyPartType.Head);
            SetAuthor(WeaponSkinsController.ATVCatDiscord);

            AddAccessory<DefaultAccessoryItem>("Deal with It", "P_Acc_DealWithIt", MechBodyPartType.Head);
            SetAuthor(WeaponSkinsController.CaptainMeowDiscord);

            AddAccessory<DefaultAccessoryItem>("Horns", "P_Horns", MechBodyPartType.Head);
            SetAuthor(WeaponSkinsController.ZoloRDiscord);
        }

        /// <summary>
        /// Register an accessory
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="accessoryName"></param>
        /// <param name="assetName"></param>
        /// <param name="accessoryType"></param>
        /// <param name="accessoryBodyPart"></param>
        /// <param name="descriptionFile"></param>
        public static void AddAccessory<T>(string accessoryName,
            string assetName,
            MechBodyPartType accessoryBodyPart) where T : AccessoryItem
        {
            string desc = null;

            AccessoryItem item = AccessoryItem.NewAccessory<T>(accessoryName, desc, accessoryBodyPart);
            if (!string.IsNullOrEmpty(assetName)) item.Prefab = OverhaulAssetsController.GetAsset(assetName, OverhaulAssetPart.Accessories);
            item.SetUpOffsets();
            AllAccessories.Add(item);
        }

        public void SetAuthor(string author)
        {
            if (AllAccessories.IsNullOrEmpty())
                return;

            AccessoryItem item = AllAccessories[AllAccessories.Count - 1];
            if (item != null)
                item.Author = author;
        }

        /// <summary>
        /// Get accessory by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="returnNullIfLocked"></param>
        /// <returns></returns>
        public static AccessoryItem GetAccessoryItem(string name, bool returnNullIfLocked = true)
        {
            bool canSearchThrough = !AllAccessories.IsNullOrEmpty() && !string.IsNullOrEmpty(name);
            if (!canSearchThrough)
                return null;

            AccessoryItem result = null;
            int i = 0;
            do
            {
                AccessoryItem item = AllAccessories[i];
                if (name.Equals(item.Name))
                {
                    if (!item.IsUnlocked() && returnNullIfLocked)
                        return null;

                    result = item;
                    break;
                }
                i++;
            } while (i < AllAccessories.Count);

            return result;
        }

        public static List<AccessoryItem> GetAccessories(string itemsString)
        {
            List<AccessoryItem> result = new List<AccessoryItem>();
            bool shouldSearchEquipped = !string.IsNullOrEmpty(itemsString) && itemsString.Contains(Separator.ToString()) && !AllAccessories.IsNullOrEmpty();
            if (!shouldSearchEquipped)
                return result;

            foreach (AccessoryItem item in AllAccessories)
            {
                if (itemsString.Contains(item.Name))
                {
                    AccessoryItem aItem = GetAccessoryItem(item.Name, false);
                    if (aItem == null)
                        continue;

                    result.Add(aItem);
                }
            }
            return result;
        }

        public static List<AccessoryItem> GetEquippedAccessories() => GetAccessories(EquippedAccessories);

        public static void SetAccessoryEquipped(AccessoryItem item, bool equip)
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

        public static void SetAccessoryEquipped(string item, bool equip) => SetAccessoryEquipped(GetAccessoryItem(item, equip), equip);
    }
}