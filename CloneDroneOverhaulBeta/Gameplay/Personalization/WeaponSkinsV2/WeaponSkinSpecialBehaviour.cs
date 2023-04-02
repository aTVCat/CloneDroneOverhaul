namespace CDOverhaul.Gameplay
{
    public abstract class WeaponSkinSpecialBehaviour : OverhaulBehaviour
    {
        public abstract void OnBeginDraw();
        public abstract void OnEndDraw();
        public abstract void OnDeath();
        public virtual void OnPreLoad() { }
    }
}