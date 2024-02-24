using OverhaulMod.Engine;
using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Visuals.Environment
{
    public class SeveredBodyPartSparks : MonoBehaviour
    {
        [ModSetting(ModSettingsConstants.ENABLE_GARBAGE_PARTICLES, true)]
        public static bool EnableGarbageParticles;

        private CameraManager m_cameraManager;

        private float m_timeLeftToSpark;

        private void Start()
        {
            m_cameraManager = CameraManager.Instance;
            setTime();
        }

        private void Update()
        {
            if (Time.frameCount % 20 == 0 && Time.time >= m_timeLeftToSpark)
            {
                setTime();
                spark();
            }
        }

        private void setTime()
        {
            m_timeLeftToSpark = Time.time + UnityEngine.Random.Range(4f, 25f);
        }

        private void spark()
        {
            if (!EnableGarbageParticles)
                return;

            Camera camera = m_cameraManager?.mainCamera;
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
