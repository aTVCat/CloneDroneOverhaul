using OverhaulAPI;
using UnityEngine;

namespace CDOverhaul.Graphics
{
    public class OverhaulVFXController : OverhaulController
    {
        public const string LASER_CUT_VFX = "SwordLaserCut";
        public const string FIRE_CUT_VFX = "SwordFireCut";

        public const string FIRE_VFX = "FireParticles";

        private static Transform s_SwordBlockVFX;

        private static bool s_HasInitialized;

        public override void Initialize()
        {
            if (!s_HasInitialized)
            {
                s_SwordBlockVFX = OverhaulAssetsController.GetAsset("VFX_SwordBlock", OverhaulAssetPart.Part2).transform;
                s_SwordBlockVFX.gameObject.AddComponent<DestroyAfterWait>().SetWaitTime(1f);

                Transform newSwordBlockFireVFX = OverhaulAssetsController.GetAsset("VFX_FireSwordBlock", OverhaulAssetPart.Part2).transform;
                PooledPrefabController.CreateNewEntry<PooledPrefabInstanceBase>(newSwordBlockFireVFX, 10, FIRE_VFX);
                Transform newSwordCutVFX = OverhaulAssetsController.GetAsset("VFX_CutLaser", OverhaulAssetPart.Part2).transform;
                PooledPrefabController.CreateNewEntry<PooledPrefabInstanceBase>(newSwordCutVFX, 25, LASER_CUT_VFX);
                Transform newSwordCutFireVFX = OverhaulAssetsController.GetAsset("VFX_CutFire", OverhaulAssetPart.Part2).transform;
                PooledPrefabController.CreateNewEntry<PooledPrefabInstanceBase>(newSwordCutFireVFX, 25, FIRE_CUT_VFX);
                s_HasInitialized = true;
            }
            AttackManager.Instance.SwordBlockVFXPrefab = s_SwordBlockVFX;
        }
    }
}
