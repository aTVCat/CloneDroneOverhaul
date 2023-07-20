using OverhaulAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul.Graphics 
{
    public class OverhaulVFXController : OverhaulController
    {
        public const string LASER_CUT_VFX = "SwordLaserCut";
        public const string FIRE_CUT_VFX = "SwordFireCut";

        public const string FIRE_VFX = "FireParticles";

        public override void Initialize()
        {
            AttackManager manager = AttackManager.Instance;

            Transform newSwordBlockVFX = OverhaulAssetsController.GetAsset("VFX_SwordBlock", OverhaulAssetPart.Part2).transform;
            newSwordBlockVFX.gameObject.AddComponent<DestroyAfterWait>().SetWaitTime(1f);
            manager.SwordBlockVFXPrefab = newSwordBlockVFX;

            Transform newSwordBlockFireVFX = OverhaulAssetsController.GetAsset("VFX_FireSwordBlock", OverhaulAssetPart.Part2).transform;
            PooledPrefabController.CreateNewEntry<PooledPrefabInstanceBase>(newSwordBlockFireVFX, 10, FIRE_VFX);

            Transform newSwordCutVFX = OverhaulAssetsController.GetAsset("VFX_CutLaser", OverhaulAssetPart.Part2).transform;
            PooledPrefabController.CreateNewEntry<PooledPrefabInstanceBase>(newSwordCutVFX, 25, LASER_CUT_VFX);

            Transform newSwordCutFireVFX = OverhaulAssetsController.GetAsset("VFX_CutFire", OverhaulAssetPart.Part2).transform;
            PooledPrefabController.CreateNewEntry<PooledPrefabInstanceBase>(newSwordCutFireVFX, 25, FIRE_CUT_VFX);
        }
    }
}
