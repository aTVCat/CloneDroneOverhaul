namespace OverhaulMod.Engine
{
    public struct CharacterUpdateRequest
    {
        public bool UpdateWeaponSkins, UpdateAccessories, UpdatePets;

        public bool UpdateWeaponBag;

        public void Append(CharacterUpdateRequest request)
        {
            UpdateWeaponSkins |= request.UpdateWeaponSkins;
            UpdateAccessories |= request.UpdateAccessories;
            UpdatePets |= request.UpdatePets;
            UpdateWeaponBag |= request.UpdateWeaponBag;
        }
    }
}