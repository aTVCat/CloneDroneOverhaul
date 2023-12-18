using OverhaulMod.Visuals;

namespace OverhaulMod.Patches.Addons
{
    internal class ProjectileAddon : GameAddon
    {
        public override void Patch()
        {
            ProjectileManager projectileManager = ProjectileManager.Instance;
            if (projectileManager)
            {
                UnityEngine.Transform prefab = projectileManager.ArrowPool.Prefab;
                if (!prefab.GetComponent<ArrowModelRefresher>())
                {
                    _ = prefab.gameObject.AddComponent<ArrowModelRefresher>();
                }
            }
        }
    }
}
