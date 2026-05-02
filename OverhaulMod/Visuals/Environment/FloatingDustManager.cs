using OverhaulMod.Engine;
using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Visuals.Environment
{
    public class FloatingDustManager : Singleton<FloatingDustManager>
    {
        [ModSetting(ModSettingIDs.ENABLE_FLOATING_DUST, true)]
        public static bool EnableFloatingDust;

        private ParticleSystem[] _normalSpaceDust;
        private ParticleSystem[] _mindSpaceDust;

        private float _timeToUpdate;

        public Transform floatingDustVFXHolder
        {
            get;
            private set;
        }

        private void Start()
        {
            createHolderIfNull();
            populateVFX();
        }

        private void Update()
        {
            _timeToUpdate -= Time.deltaTime;
            if (_timeToUpdate <= 0f)
            {
                _timeToUpdate = 1f;
                RefreshVFX();
            }
        }

        private void createHolderIfNull()
        {
            if (floatingDustVFXHolder)
                return;

            GameObject newHolder = new GameObject("OverhaulFloatingDust");
            newHolder.transform.position = Vector3.zero;
            DontDestroyOnLoad(newHolder);
            floatingDustVFXHolder = newHolder.transform;
        }

        private void populateVFX()
        {
            if (_normalSpaceDust == null)
            {
                _normalSpaceDust = new ParticleSystem[]
                {
                    Instantiate(ModResources.Prefab(ModAssetBundles.VFX, "FloatingDust_Normal"), floatingDustVFXHolder).GetComponent<ParticleSystem>()
                };
            }

            if (_mindSpaceDust == null)
            {
                _mindSpaceDust = new ParticleSystem[]
                {
                    Instantiate(ModResources.Prefab(ModAssetBundles.VFX, "FloatingDust_Mindspace0"), floatingDustVFXHolder).GetComponent<ParticleSystem>(),
                    Instantiate(ModResources.Prefab(ModAssetBundles.VFX, "FloatingDust_Mindspace1"), floatingDustVFXHolder).GetComponent<ParticleSystem>()
                };
            }
        }

        public void RefreshVFX()
        {
            Transform transform = floatingDustVFXHolder;
            if (!transform)
                return;

            bool allowParticles = EnableFloatingDust;
            bool isMindspace = false;

            FirstPersonMover firstPersonMover = CharacterTracker.Instance?.GetPlayerRobot();
            if (firstPersonMover)
                isMindspace = firstPersonMover.IsMindSpaceCharacter;
            else
                allowParticles = false;

            SetParticlesActive(_normalSpaceDust, !isMindspace && allowParticles);
            SetParticlesActive(_mindSpaceDust, isMindspace && allowParticles);
            transform.position = allowParticles ? firstPersonMover.transform.position : Vector3.zero;
        }

        public void SetParticlesActive(ParticleSystem[] particleSystems, bool value)
        {
            if (particleSystems == null || particleSystems.Length == 0)
                return;

            foreach (ParticleSystem system in particleSystems)
            {
                system.SetEmissionEnabled(value);
                system.transform.localPosition = Vector3.zero;
            }
        }
    }
}
