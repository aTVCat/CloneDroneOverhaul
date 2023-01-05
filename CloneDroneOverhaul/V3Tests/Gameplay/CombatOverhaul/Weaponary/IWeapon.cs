namespace CloneDroneOverhaul.V3Tests.Gameplay
{
    public interface IWeapon
    {
        WeaponType WeaponType();
        void OnEquip();
        void OnUnquip();
    }
}
