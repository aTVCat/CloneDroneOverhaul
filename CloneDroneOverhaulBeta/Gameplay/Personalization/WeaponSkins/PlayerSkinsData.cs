using System.Collections.Generic;

namespace CDOverhaul.Gameplay
{
    public class PlayerSkinsData : ModDataContainerBase
    {
        public Dictionary<WeaponType, string> Skins;

        public override void RepairMissingFields()
        {
            if (Skins == null)
            {
                Skins = new Dictionary<WeaponType, string>();
            }
            if (Skins.Count != 5) // Implement target list count method later
            {
                Skins.Clear();
                Skins.Add(WeaponType.Sword, WeaponSkinsController.DefaultWeaponSkinName);
                Skins.Add(WeaponType.Bow, WeaponSkinsController.DefaultWeaponSkinName);
                Skins.Add(WeaponType.Hammer, WeaponSkinsController.DefaultWeaponSkinName);
                Skins.Add(WeaponType.Spear, WeaponSkinsController.DefaultWeaponSkinName);
                Skins.Add(WeaponType.Shield, WeaponSkinsController.DefaultWeaponSkinName);
            }
        }

        public bool IsSkinSelected(in WeaponType type, in string skin)
        {
            return Skins.ContainsKey(type) && Skins[type] == skin;
        }

        public void SelectSkin(in WeaponType type, in string skin)
        {
            if (Skins.ContainsKey(type))
            {
                Skins[type] = skin;
                SaveData();
            }
        }

        public void ResetSkinSelection(in WeaponType type)
        {
            if (Skins.ContainsKey(type))
            {
                Skins[type] = WeaponSkinsController.DefaultWeaponSkinName;
                SaveData();
            }
        }
    }
}
