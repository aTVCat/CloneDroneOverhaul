using OverhaulMod.Engine;
using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Visuals
{
    public class SeveredBodyPartSparks : MonoBehaviour
    {
        [ModSetting(ModSettingIDs.ENABLE_GARBAGE_PARTICLES, true)]
        public static bool EnableGarbageParticles;

        private CameraManager _cameraManager;

        private float _timeLeftToSpark;

        private void Start()
        {
            _cameraManager = CameraManager.Instance;
            resetTimer();
        }

        private void Update()
        {
            _timeLeftToSpark = Mathf.Max(0f, _timeLeftToSpark - Time.deltaTime);
            if (_timeLeftToSpark == 0f)
            {
                resetTimer();
                spark();
            }
        }

        private void resetTimer()
        {
            _timeLeftToSpark = UnityEngine.Random.Range(4f, 25f);
        }

        private void spark()
        {
            if (!EnableGarbageParticles)
                return;

            Camera camera = _cameraManager?.MainCamera;
            if (!camera)
                return;

            if (Vector3.Distance(camera.transform.position, base.transform.position) > 40f)
                return;

            Vector3 vector3 = base.transform.position;
            vector3.x += UnityEngine.Random.value - 0.5f;
            vector3.y += UnityEngine.Random.value - 0.5f;
            vector3.z += UnityEngine.Random.value - 0.5f;

            ParticleManager.Instance.SpawnSparksParticles(vector3);
        }
    }
}
