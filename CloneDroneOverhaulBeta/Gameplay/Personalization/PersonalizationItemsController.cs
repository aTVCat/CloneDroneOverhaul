using CDOverhaul.Gameplay.Outfits;
using CDOverhaul.Gameplay.Pets;

namespace CDOverhaul.Gameplay
{
    public abstract class PersonalizationItemsController : OverhaulGameplayController
    {
        public override void Initialize()
        {
            base.Initialize();
            AddItems();
        }

        public abstract void AddItems();

        public static void SavePreferences()
        {
            SettingInfo info = OverhaulSettingsController.GetSetting("Player.Outfits.Equipped", true);
            if (info != null)
                SettingInfo.SavePref(info, OutfitsController.EquippedAccessories);

            SettingInfo info2 = OverhaulSettingsController.GetSetting("Player.Pets.Equipped", true);
            if (info2 != null)
                SettingInfo.SavePref(info2, PetsController.EquippedPets);
        }
    }
}
