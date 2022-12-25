using UnityEngine;

namespace CloneDroneOverhaul.V3Tests.Gameplay
{
    public class PooledPrefab_VFXEffect_ParticleSystemBase : PooledPrefabInstanceBase
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
