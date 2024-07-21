using OverhaulMod.Visuals;

namespace OverhaulMod.Patches.Behaviours
{
    internal class ProjectilePatchBehaviour : GamePatchBehaviour
    {
        public override void Patch()
        {
            ProjectileManager projectileManager = ProjectileManager.Instance;
            if (projectileManager)
            {
                UnityEngine.Transform prefab = projectileManager.ArrowPool?.Prefab;
                if (!prefab.GetComponent<ArrowModelRefresher>())
                {
                    _ = prefab.gameObject.AddComponent<ArrowModelRefresher>();
                }
            }
        }

        public override void UnPatch()
        {
            ProjectileManager projectileManager = ProjectileManager.Instance;
            if (projectileManager)
            {
                UnityEngine.Transform prefab = projectileManager.ArrowPool?.Prefab;
                ArrowModelRefresher arrowModelRefresher = prefab.GetComponent<ArrowModelRefresher>();
                if (arrowModelRefresher)
                {
                    Destroy(arrowModelRefresher);
                }
            }
        }
    }
}
