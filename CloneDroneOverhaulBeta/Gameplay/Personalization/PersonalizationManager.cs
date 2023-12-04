using CDOverhaul.Gameplay.Outfits;
using CDOverhaul.Gameplay.Pets;
using CDOverhaul.Gameplay.WeaponSkins;

namespace CDOverhaul.Gameplay
{
    public class PersonalizationManager : OverhaulManager<PersonalizationManager>
    {
        public WeaponSkinsSystem weaponSkins
        {
            get;
            private set;
        }

        public OutfitsSystem outfits
        {
            get;
            private set;
        }

        public PetSystem pets
        {
            get;
            private set;
        }

        public override void Initialize()
        {
            base.Initialize();
            weaponSkins = base.gameObject.AddComponent<WeaponSkinsSystem>();
            outfits = base.gameObject.AddComponent<OutfitsSystem>();
            pets = base.gameObject.AddComponent<PetSystem>();
        }

        protected override void OnAssetsLoaded()
        {
            base.OnAssetsLoaded();
            weaponSkins.ReloadItems();
            outfits.ReloadItems();
            pets.ReloadItems();
        }
    }
}
