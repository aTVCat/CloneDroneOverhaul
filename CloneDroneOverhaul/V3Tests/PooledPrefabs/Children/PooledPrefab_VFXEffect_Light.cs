using UnityEngine;

namespace CloneDroneOverhaul.V3Tests.Gameplay
{
    public class PooledPrefab_VFXEffect_Light : PooledPrefabInstanceBase
    {
        private Light _light;
        private Animator _animator;

        public override void PreparePrefab()
        {
            _light = GetComponent<Light>();
            _animator = GetComponent<Animator>();
        }

        protected override void OnPrefabUsed()
        {
            _animator.Play("LightStarted");
        }

        protected override float LifeTime()
        {
            return 3f;
        }

        public void SetLightSettings(in float lightRange, in Color color, in float animationSpeed = 1f)
        {
            _light.color = color;
            _light.range = lightRange;
            _animator.speed = animationSpeed;
        }
    }
}
