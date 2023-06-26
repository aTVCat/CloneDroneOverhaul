using CDOverhaul.Gameplay;
using OverhaulAPI;
using UnityEngine;

namespace CDOverhaul.Graphics.Robots
{
    public class SeveredBodyPartSparks : OverhaulBehaviour
    {


        private float m_TimeToSpark;

        private void Start()
        {
            setTime();
        }

        private void Update()
        {
            if (Time.time >= m_TimeToSpark)
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
            Vector3 vector3 = base.transform.position;
            vector3.x += UnityEngine.Random.Range(-1f, 1f);
            vector3.y += UnityEngine.Random.Range(-1f, 1f);
            vector3.z += UnityEngine.Random.Range(-1f, 1f);

            _ = PooledPrefabController.SpawnObject<WeaponSkinCustomVFXInstance>(RobotEffectsBehaviour.RobotDeathSparksVFXID, vector3, Vector3.zero);
        }
    }
}
