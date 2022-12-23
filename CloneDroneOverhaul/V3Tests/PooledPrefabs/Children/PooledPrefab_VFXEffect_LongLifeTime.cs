using UnityEngine;
using System.Collections.Generic;
using CloneDroneOverhaul.V3Tests.Base;

namespace CloneDroneOverhaul.V3Tests.Gameplay
{
    public class PooledPrefab_VFXEffect_LongLifeTime : PooledPrefabInstanceBase
    {
        ParticleSystem _system;
        public override void PreparePrefab()
        {
            _system = GetComponent<ParticleSystem>();
        }

        protected override void OnPrefabUsed()
        {
            _system.Play();
        }

        protected override float LifeTime()
        {
            return 5f;
        }

    }
}
