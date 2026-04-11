using UnityEngine;

namespace OverhaulMod.Visuals
{
    public class ReplacedParticles : MonoBehaviour
    {
        public bool EmitIfOriginalSystemIsActive;

        private ParticleSystem _originalSystem;

        private ParticleSystem.EmissionModule _origialSystemEmission;

        private ParticleSystem[] _systems;

        private ParticleSystem.EmissionModule[] _systemsEmission;

        private void Start()
        {
            _systems = GetComponentsInChildren<ParticleSystem>();
            _systemsEmission = new ParticleSystem.EmissionModule[_systems.Length];
            for (int i = 0; i < _systems.Length; i++)
            {
                _systemsEmission[i] = _systems[i].emission;
            }
        }

        private void Update()
        {
            if (EmitIfOriginalSystemIsActive) return;
            setEmissionEnabled(_origialSystemEmission.enabled);
        }

        private void LateUpdate()
        {
            if (!EmitIfOriginalSystemIsActive) return;
            setEmissionEnabled(_originalSystem.gameObject.activeSelf);
            _originalSystem.gameObject.SetActive(false);
        }

        public void SetOriginalSystem(ParticleSystem particleSystem)
        {
            _originalSystem = particleSystem;
            _origialSystemEmission = _originalSystem.emission;
        }

        private void setEmissionEnabled(bool emit)
        {
            for (int i = 0; i < _systemsEmission.Length; i++)
            {
                _systemsEmission[i].enabled = emit;
            }
        }
    }
}