using OverhaulMod.Content.Personalization;
using OverhaulMod.Visuals;

namespace OverhaulMod.Engine
{
    public class CharacterUpdateInfo
    {
        public Character ReferenceCharacter;

        public CharacterUpdateImportance Importance;

        public CharacterUpdateRequest Request;

        private PersonalizationController _personalizationController;

        private RobotWeaponBag _weaponBag;

        public void RefreshReferences()
        {
            _personalizationController = ComponentCacheManager.Instance.GetPersonalizationController(ReferenceCharacter.transform);
            _weaponBag = ReferenceCharacter.GetComponent<RobotWeaponBag>();
        }

        public bool UpdateWeaponSkins()
        {
            if (Request.UpdateWeaponSkins && _personalizationController)
            {
                Request.UpdateWeaponSkins = false;
                _personalizationController.RefreshWeaponSkins();
                return true;
            }
            return false;
        }

        public bool UpdateAccessories()
        {
            if (Request.UpdateAccessories && _personalizationController)
            {
                Request.UpdateAccessories = false;
                _personalizationController.RefreshAccessories();
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
            if (Request.UpdateWeaponBag && _weaponBag)
            {
                Request.UpdateWeaponBag = false;
                ModDebug.Log("Update weapon bag!");
                _weaponBag.RespawnRenderers();
                _weaponBag.RefreshVisibilityOfRenderers();
                return true;
            }
            return false;
        }
    }
}