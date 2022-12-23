using UnityEngine;
using System.Collections.Generic;
using CloneDroneOverhaul.V3Tests.Base;

namespace CloneDroneOverhaul.V3Tests.Gameplay
{
    public class PooledPrefab_VFXEffect_LongLifeTime : PooledPrefabInstanceBase
    {
        ParticleSystem _system;
      
        protected override void OnPrefabUsed()
        {
            if(_system == null)
            {
                _system = GetComponent<ParticleSystem>();
            }
            _system.Play();
        }

        protected override float LifeTime()
        {
            return 5f;
        }

    }
}
