using CDOverhaul.DevTools;
using CDOverhaul.Gameplay;
using OverhaulAPI;
using System.Diagnostics;
using UnityEngine;

namespace CDOverhaul.Visuals.Robots
{
    public class SeveredBodyPartSparks : OverhaulBehaviour
    {
        private OverhaulCameraManager m_CameraController;
        private float m_TimeToSpark;

        private void Start()
        {
            m_CameraController = OverhaulController.Get<OverhaulCameraManager>();
            setTime();
        }

        private void Update()
        {
            if (Time.frameCount % 20 == 0 && Time.time >= m_TimeToSpark)
            {
                setTime();
                spark();
            }
        }

        private void setTime()
        {
            m_TimeToSpark = Time.time + UnityEngine.Random.Range(4.00f, 25.00f);
        }

        private void spark()
        {
            if (!m_CameraController || !m_CameraController.mainCamera)
                return;

            if (Vector3.Distance(m_CameraController.mainCamera.transform.position, base.transform.position) > 60f)
                return;

            Stopwatch stopwatch = OverhaulProfiler.StartTimer();
            Vector3 vector3 = base.transform.position;
            vector3.x += UnityEngine.Random.Range(-1f, 1f);
            vector3.y += UnityEngine.Random.Range(-1f, 1f);
            vector3.z += UnityEngine.Random.Range(-1f, 1f);

            _ = PooledPrefabController.SpawnEntry<WeaponSkinCustomVFXInstance>(OverhaulVFXController.GenericSparksVFX, vector3, Vector3.zero);
            stopwatch.StopTimer("SeveredBodyPartSparks.spark");
        }
    }
}
