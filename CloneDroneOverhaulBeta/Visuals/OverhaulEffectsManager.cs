﻿using CDOverhaul.Gameplay;
using OverhaulAPI;
using UnityEngine;

namespace CDOverhaul.Visuals
{
    public class OverhaulEffectsManager : OverhaulManager<OverhaulEffectsManager>
    {
        public const string GenericSparksVFX = "GenericSparks";

        public const string LASER_CUT_VFX = "SwordLaserCut";
        public const string FIRE_CUT_VFX = "SwordFireCut";

        public const string FIRE_VFX = "FireParticles";

        private static Transform s_SwordBlockVFX;

        private static bool s_HasInitialized;

        protected override void OnAssetsLoaded()
        {
            base.OnAssetsLoaded();
            if (!s_HasInitialized)
            {
                s_SwordBlockVFX = OverhaulAssetLoader.GetAsset("VFX_SwordBlock", OverhaulAssetPart.Part2).transform;
                s_SwordBlockVFX.gameObject.AddComponent<DestroyAfterWait>().SetWaitTime(1f);

                PooledPrefabController.CreateNewEntry<WeaponSkinCustomVFXInstance>(OverhaulAssetLoader.GetAsset("VFX_Sparks", OverhaulAssetPart.Part2).transform, 10, GenericSparksVFX);
                PooledPrefabController.CreateNewEntry<PooledPrefabInstanceBase>(OverhaulAssetLoader.GetAsset("VFX_FireSwordBlock", OverhaulAssetPart.Part2).transform, 10, FIRE_VFX);
                PooledPrefabController.CreateNewEntry<PooledPrefabInstanceBase>(OverhaulAssetLoader.GetAsset("VFX_CutLaser", OverhaulAssetPart.Part2).transform, 25, LASER_CUT_VFX);
                PooledPrefabController.CreateNewEntry<PooledPrefabInstanceBase>(OverhaulAssetLoader.GetAsset("VFX_CutFire", OverhaulAssetPart.Part2).transform, 25, FIRE_CUT_VFX);
                s_HasInitialized = true;
            }
            refreshAttackManager();
        }

        public override void OnSceneReloaded()
        {
            base.OnSceneReloaded();
            refreshAttackManager();
        }

        private void refreshAttackManager()
        {
            if (s_SwordBlockVFX)
            {
                AttackManager.Instance.SwordBlockVFXPrefab = s_SwordBlockVFX;
            }
        }
    }
}
