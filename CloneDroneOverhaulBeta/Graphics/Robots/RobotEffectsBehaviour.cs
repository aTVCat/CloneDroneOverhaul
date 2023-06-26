using CDOverhaul.Gameplay;
using OverhaulAPI;
using UnityEngine;

namespace CDOverhaul.Graphics.Robots
{
    public class RobotEffectsBehaviour : OverhaulCharacterExpansion
    {
        public const string RobotDeathSparksVFXID = "RobotDeathSparksVFX";

        private Transform m_HeadTransform;

        public override void Start()
        {
            base.Start();
            FindHeadTransform();
        }

        protected override void OnDeath()
        {
            FindHeadTransform();

            if (m_HeadTransform != null)
                _ = PooledPrefabController.SpawnObject<WeaponSkinCustomVFXInstance>(RobotDeathSparksVFXID, m_HeadTransform.position, Vector3.zero);
        }

        public void FindHeadTransform()
        {
            if (m_HeadTransform != null || Owner == null || !Owner.HasCharacterModel())
                return;

            m_HeadTransform = Owner.GetBodyPartParent("Head");
        }
    }
}