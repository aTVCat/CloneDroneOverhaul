using OverhaulAPI;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class RobotAccessoryDestroy_VFX : PooledPrefabInstanceBase
    {
        private ParticleSystem _system;

        public override void PreparePrefab()
        {
            _system = GetComponent<ParticleSystem>();
        }

        protected override void OnPrefabUsed()
        {
            _system.Play();
        }
    }
}
