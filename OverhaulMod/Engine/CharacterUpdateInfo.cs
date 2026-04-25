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

        public CharacterUpdateRequest Request;

        public bool UpdateWeaponSkins()
        {
            if (Request.UpdateWeaponSkins && PersonalizationController)
            {
                Request.UpdateWeaponSkins = false;
                PersonalizationController.RefreshWeaponSkins();
                return true;
            }
            return false;
        }

        public bool UpdateAccessories()
        {
            if (Request.UpdateAccessories && PersonalizationController)
            {
                Request.UpdateAccessories = false;
                PersonalizationController.RefreshAccessories();
                return true;
            }
            return false;
        }

        public bool UpdatePets()
        {
            return false;
        }

        public bool UpdateWeaponBag()
        {
            if (Request.UpdateWeaponBag && WeaponBag)
            {
                Request.UpdateWeaponBag = false;
                WeaponBag.RespawnRenderers();
                WeaponBag.RefreshVisibilityOfRenderers();
                return true;
            }
            return false;
        }
    }
}