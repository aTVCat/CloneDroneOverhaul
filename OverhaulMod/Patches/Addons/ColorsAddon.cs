using UnityEngine;

namespace OverhaulMod.Patches.Addons
{
    internal class ColorsAddon : GameAddon
    {
        public override void Patch()
        {
            AttackManager attackManager = AttackManager.Instance;
            if (attackManager)
            {
                attackManager.HitColor = new Color(4f, 0.65f, 0.35f, 0.2f);
                attackManager.BodyOnFireColor = new Color(1f, 0.42f, 0.22f, 0.1f);
            }
        }
    }
}
