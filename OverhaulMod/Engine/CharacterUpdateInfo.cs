using OverhaulMod.Content.Personalization;
using OverhaulMod.Visuals;

namespace OverhaulMod.Engine
{
    public class CharacterUpdateInfo
    {
        public Character ReferenceCharacter;

        public PersonalizationController PersonalizationController;

        public RobotWeaponBag WeaponBag;

        public CharacterUpdateImportance Importance;

        public bool UpdateWeaponSkins, UpdateWeaponBag;

        public bool UpdateSkinsIfRequired()
        {
            if (UpdateWeaponSkins && PersonalizationController && PersonalizationController.HasInitialized())
            {
                UpdateWeaponSkins = false;
                PersonalizationController.RefreshWeaponSkins();
                return true;
            }
            return false;
        }

        public bool UpdateWeaponBagIfRequired()
        {
            if (UpdateWeaponBag && WeaponBag && WeaponBag.HasInitialized())
            {
                UpdateWeaponBag = false;
                WeaponBag.RespawnRenderers();
                WeaponBag.RefreshVisibilityOfRenderers();
                return true;
            }
            return false;
        }
    }
}