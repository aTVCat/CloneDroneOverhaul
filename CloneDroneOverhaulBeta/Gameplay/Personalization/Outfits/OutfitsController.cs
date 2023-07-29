using System.Collections.Generic;

namespace CDOverhaul.Gameplay.Outfits
{
    public class OutfitsController : PersonalizationItemsController
    {
        public const char SEPARATOR = ',';

        [OverhaulSetting("Player.Outfits.Equipped", "", !OverhaulVersion.IsDebugBuild)]
        public static string EquippedAccessories;

        [OverhaulSetting("Player.Outfits.EnemiesUseOutfits", false, !OverhaulVersion.IsDebugBuild)]
        public static bool AllowEnemiesWearAccesories;

        public static readonly List<OutfitItem> AllOutfitItems = new List<OutfitItem>();

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
            AllOutfitItems.Clear();

            AddOutfitItemQuick("Igrok's hat", "P_Acc_Head_Igrok's hat", "Head");
            SetOutfitItemAuthorQuick(WeaponSkinsController.ATVCatDiscord);

            AddOutfitItemQuick("Halo", "P_Acc_Head_ImpostorHalo", "Head");
            SetOutfitItemAuthorQuick(WeaponSkinsController.ATVCatDiscord);

            AddOutfitItemQuick("Puss Hat", "P_Acc_Head_PussHat", "Head");
            SetOutfitItemAuthorQuick(WeaponSkinsController.TabiDiscord);

            AddOutfitItemQuick("Cone", "P_Acc_Head_ImpostorCone", "Head");
            SetOutfitItemAuthorQuick(WeaponSkinsController.ATVCatDiscord);

            AddOutfitItemQuick("Deal with It", "P_Acc_DealWithIt", "Head");
            SetOutfitItemAuthorQuick(WeaponSkinsController.CaptainMeowDiscord);

            AddOutfitItemQuick("Horns", "P_Horns", "Head");
            SetOutfitItemAuthorQuick(WeaponSkinsController.ZoloRDiscord);
        }

        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover, bool hasInitializedModel)
        {
            if (!hasInitializedModel)
                return;

            _ = firstPersonMover.gameObject.AddComponent<OutfitsWearer>();
        }

        /// <summary>
        /// Add an outfit item
        /// </summary>
        /// <param name="accessoryName"></param>
        /// <param name="assetName"></param>
        /// <param name="accessoryBodyPart"></param>
        public static void AddOutfitItemQuick(string accessoryName, string assetName, string accessoryBodyPart)
        {
            OutfitItem item = OutfitItem.NewAccessory(accessoryName, accessoryBodyPart);
            item.Prefab = OverhaulAssetsController.GetAsset(assetName, OverhaulAssetPart.Accessories);
            item.SetUpOffsets();
            AllOutfitItems.Add(item);
        }

        public void SetOutfitItemAuthorQuick(string author)
        {
            if (AllOutfitItems.IsNullOrEmpty())
                return;

            OutfitItem item = AllOutfitItems[AllOutfitItems.Count - 1];
            if (item != null)
                item.Author = author;
        }

        public void SetOutfitItemDescriptionQuick(string description)
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
        /// <param name="bodyPart"></param>
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
                if (name == item.Name)
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

        public static List<OutfitItem> GetOutfitItemsBySaveString(string itemsString)
        {
            List<OutfitItem> result = new List<OutfitItem>();
            bool shouldSearchEquipped = !string.IsNullOrEmpty(itemsString) && itemsString.Contains(SEPARATOR.ToString()) && !AllOutfitItems.IsNullOrEmpty();
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

        public static List<OutfitItem> GetOutfitItemsOfBodyPart(string bodyPart)
        {
            List<OutfitItem> result = new List<OutfitItem>();
            foreach (OutfitItem item in AllOutfitItems)
            {
                if (item.BodyPart == bodyPart)
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

        public static List<OutfitItem> GetEquippedAccessories() => GetOutfitItemsBySaveString(EquippedAccessories);

        public static void SetAccessoryEquipped(OutfitItem item, bool equip, bool refreshPlayer = false)
        {
            if (item == null || (!item.IsUnlocked() && equip))
                return;

            string accessorySaveString = item.Name + SEPARATOR;
            bool isEquipped = EquippedAccessories.Contains(accessorySaveString);
            if (isEquipped && !equip)
                EquippedAccessories = EquippedAccessories.Replace(accessorySaveString, string.Empty);

            else if (!isEquipped && equip)
                EquippedAccessories += accessorySaveString;

            SavePreferences();
            if (refreshPlayer)
            {
                FirstPersonMover firstPersonMover = CharacterTracker.Instance.GetPlayerRobot();
                if (firstPersonMover)
                {
                    OutfitsWearer outfitsWearer = firstPersonMover.GetComponent<OutfitsWearer>();
                    if (outfitsWearer)
                    {
                        outfitsWearer.RefreshItems();
                    }
                }
            }
        }

        public static void SetAccessoryEquipped(string item, bool equip) => SetAccessoryEquipped(GetOutfitItem(item, equip), equip);

        #endregion
    }
}