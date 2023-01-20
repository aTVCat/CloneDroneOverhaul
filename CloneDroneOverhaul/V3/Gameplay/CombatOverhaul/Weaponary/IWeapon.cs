namespace CloneDroneOverhaul.V3.Gameplay
{
    public interface IWeapon
    {
        WeaponType WeaponType();
        void OnEquip();
        void OnUnquip();
    }
}
