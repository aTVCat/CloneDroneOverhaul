namespace CDOverhaul.Gameplay
{
    public abstract class WeaponSkinBehaviour : OverhaulBehaviour
    {
        public abstract void OnBeginDraw();
        public abstract void OnEndDraw();
        public abstract void OnDeath();
        public virtual void OnPreLoad() { }
        public virtual void OnSetColor(UnityEngine.Color color) { }
    }
}