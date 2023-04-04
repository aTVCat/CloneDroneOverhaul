using OverhaulAPI;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            SettingInfo info = SettingsController.GetSetting("Player.Outfits.Equipped", true);
            if(info == null)
            {
                return;
            }
            SettingInfo.SavePref(info, EquippedAccessories);
        }

        #endregion

        private static readonly List<AccessoryItem> m_Accessories = new List<AccessoryItem>();
        public static List<AccessoryItem> AllAccessories => m_Accessories;

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
            {
                return;
            }

            _ = firstPersonMover.gameObject.AddComponent<OutfitsWearer>();
        }

        private void addAccessories()
        {
            AddAccessory<DefaultAccessoryItem>("Igrok's hat", "P_Acc_Head_Igrok's hat", AccessoryType.Attached, MechBodyPartType.Head);
            SetAuthor(WeaponSkinsController.ATVCatDiscord);

            AddAccessory<DefaultAccessoryItem>("Halo", "P_Acc_Head_ImpostorHalo", AccessoryType.Attached, MechBodyPartType.Head);
            SetAuthor(WeaponSkinsController.ATVCatDiscord);

            AddAccessory<DefaultAccessoryItem>("Puss Hat", "P_Acc_Head_PussHat", AccessoryType.Attached, MechBodyPartType.Head);
            SetAuthor(WeaponSkinsController.TabiDiscord);

            AddAccessory<DefaultAccessoryItem>("Cone", "P_Acc_Head_ImpostorCone", AccessoryType.Attached, MechBodyPartType.Head);
            SetAuthor(WeaponSkinsController.ATVCatDiscord);

            AddAccessory<DefaultAccessoryItem>("Deal with It", "P_Acc_DealWithIt", AccessoryType.Attached, MechBodyPartType.Head);
            SetAuthor(WeaponSkinsController.CaptainMeowDiscord);
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
            AccessoryType accessoryType,
            MechBodyPartType accessoryBodyPart,
            string descriptionFile = null) where T : AccessoryItem
        {
            // Make a method for doing this
            string desc = null;
            if (!string.IsNullOrEmpty(descriptionFile))
            {
                string path = OverhaulMod.Core.ModDirectory + "Assets/OutfitDescriptions/" + descriptionFile + ".txt";
                bool fileExists = File.Exists(path);
                if (!fileExists)
                {
                    return;
                }

                StreamReader r = File.OpenText(path);
                desc = r.ReadToEnd();
                r.Close();
            }


            AccessoryItem item = AccessoryItem.NewAccessory<T>(accessoryName, desc, accessoryType, accessoryBodyPart);
            if(!string.IsNullOrEmpty(assetName)) item.Prefab = AssetsController.GetAsset(assetName, OverhaulAssetsPart.Accessories);
            item.SetUpOffsets();
            m_Accessories.Add(item);
        }

        public void SetAuthor(string author)
        {
            if (m_Accessories.IsNullOrEmpty())
            {
                return;
            }

            AccessoryItem item = m_Accessories[m_Accessories.Count - 1];
            if(item != null)
            {
                item.Author = author;
            }
        }

        /// <summary>
        /// Get accessory by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="returnNullIfLocked"></param>
        /// <returns></returns>
        public static AccessoryItem GetAccessoryItem(string name, bool returnNullIfLocked = true)
        {
            bool canSearchThrough = !m_Accessories.IsNullOrEmpty() && !string.IsNullOrEmpty(name);
            if (!canSearchThrough)
            {
                return null;
            }

            AccessoryItem result = null;
            int i = 0;
            do
            {
                AccessoryItem item = m_Accessories[i];
                if (name.Equals(item.Name))
                {
                    if (!item.IsUnlocked())
                    {
                        if(returnNullIfLocked) return null;
                    }

                    result = item;
                    break;
                }
                i++;
            } while (i < m_Accessories.Count);

            return result;
        }

        public static List<AccessoryItem> GetAccessories(string itemsString)
        {
            List<AccessoryItem> result = new List<AccessoryItem>();
            bool shouldSearchEquipped = !string.IsNullOrEmpty(itemsString) && itemsString.Contains(Separator.ToString()) && !m_Accessories.IsNullOrEmpty();
            if (!shouldSearchEquipped)
            {
                return result;
            }

            foreach(AccessoryItem item in m_Accessories)
            {
                if (itemsString.Contains(item.Name))
                {
                    AccessoryItem aItem = GetAccessoryItem(item.Name, false);
                    if (aItem == null)
                    {
                        continue;
                    }
                    result.Add(aItem);
                }
            }
            return result;
        }

        public static List<AccessoryItem> GetEquippedAccessories()
        {
            return GetAccessories(EquippedAccessories);
        }

        public static void SetAccessoryEquipped(AccessoryItem item, bool equip)
        {
            if (item == null || (!item.IsUnlocked() && equip))
            {
                return;
            }

            string accessorySaveString = item.Name + Separator;
            bool isEquipped = EquippedAccessories.Contains(accessorySaveString);
            if (isEquipped)
            {
                if (!equip)
                {
                    EquippedAccessories = EquippedAccessories.Replace(accessorySaveString, string.Empty);
                    SavePreferences();
                }
                return;
            }
            if (!equip)
            {
                return;
            }

            EquippedAccessories += accessorySaveString;
            SavePreferences();
        }

        public static void SetAccessoryEquipped(string item, bool equip)
        {
            SetAccessoryEquipped(GetAccessoryItem(item, returnNullIfLocked: equip), equip);
        }
    }
}