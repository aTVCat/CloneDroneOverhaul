using CDOverhaul.Gameplay;
using CDOverhaul.Gameplay.Combat;
using OverhaulAPI;
using System.Collections;
using UnityEngine;

namespace CDOverhaul.Graphics.Robots
{
    public class RobotEffectsBehaviour : OverhaulCharacterExpansion
    {
        public const string RobotDeathSparksVFXID = "RobotDeathSparksVFX";

        private static bool m_HasInitializedVFX;

        private Transform m_HeadTransform;

        private static void initializeVFX()
        {
            PooledPrefabController.TurnObjectIntoPooledPrefab<WeaponSkinCustomVFXInstance>(AssetsController.GetAsset("VFX_Sparks", OverhaulAssetsPart.Part2).transform, 10, RobotDeathSparksVFXID);
            m_HasInitializedVFX = true;
        }

        public override void Start()
        {
            base.Start();
            FindHeadTransform();

            if (!m_HasInitializedVFX)
            {
                initializeVFX();
            }
        }

        protected override void OnDeath()
        {
            FindHeadTransform();

            if(m_HeadTransform != null)
            {
                PooledPrefabController.SpawnObject<WeaponSkinCustomVFXInstance>(RobotDeathSparksVFXID, m_HeadTransform.position, Vector3.zero);
            }
        }

        public void FindHeadTransform()
        {
            if(m_HeadTransform != null || Owner == null || !Owner.HasCharacterModel())
            {
                return;
            }

            m_HeadTransform = Owner.GetBodyPartParent("Head");
        }
    }
}