using System.Collections.Generic;

namespace CDOverhaul.Gameplay.Outfits
{
    public class OutfitsSystem : PersonalizationItemsSystemBase
    {
        public const char SEPARATOR = ',';

        [OverhaulSetting("Player.Outfits.Equipped", "", !OverhaulVersion.IsDebugBuild)]
        public static string EquippedAccessories;

        [OverhaulSetting("Player.Outfits.EnemiesUseOutfits", false, !OverhaulVersion.IsDebugBuild)]
        public static bool AllowEnemiesWearAccesories;

        /*
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
        SetOutfitItemAuthorQuick(WeaponSkinsController.ZoloRDiscord);*/

        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover, bool hasInitializedModel)
        {
            if (!hasInitializedModel)
                return;

            _ = firstPersonMover.gameObject.AddComponent<OutfitsWearer>();
        }

        public override string GetRepositoryFolder() => "Outfits";

        public List<PersonalizationItem> GetOutfitItemsOfBodyPart(string bodyPart)
        {
            List<PersonalizationItem> result = new List<PersonalizationItem>();
            List<PersonalizationItem> items = Items;
            if (items.IsNullOrEmpty())
                return result;

            foreach (PersonalizationItem item in items)
            {
                if (item is OutfitItem outfitItem && outfitItem.BodyPart == bodyPart)
                    result.Add(item);
            }
            return result;
        }

        #region Save data

        public static void SetAccessoryEquipped(PersonalizationItem item, bool equip, bool refreshPlayer = false)
        {
            if (item == null || (!item.IsUnlocked() && equip))
                return;

            string accessorySaveString = item.GetID() + SEPARATOR;
            bool isEquipped = EquippedAccessories.Contains(accessorySaveString);

            if (isEquipped && !equip)
                EquippedAccessories = EquippedAccessories.Replace(accessorySaveString, string.Empty);
            else if (!isEquipped && equip)
                EquippedAccessories += accessorySaveString;

            SavePreferences();
            if (refreshPlayer)
                CharacterTracker.Instance.GetPlayerRobot().RefreshPersonalizationItems();

        }

        public void SetAccessoryEquipped(string item, bool equip) => SetAccessoryEquipped(GetItem(item)?.GetID(), equip);

        #endregion
    }
}