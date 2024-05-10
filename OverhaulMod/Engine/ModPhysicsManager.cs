using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class ModPhysicsManager : Singleton<ModPhysicsManager>
    {
        private static readonly RaycastHit[] m_rayCastHitArray = new RaycastHit[10];

        private static readonly Ray m_ray = new Ray();

        public static RaycastHit[] GetRayCastHitArray(bool clear = true)
        {
            RaycastHit[] result = m_rayCastHitArray;
            if (clear)
                for (int i = 0; i < result.Length; i++)
                    result[i] = default;

            return result;
        }

        public static Ray GetRay(Vector3 position, Vector3 direction)
        {
            Ray ray = m_ray;
            ray.origin = position;
            ray.direction = direction;
            return ray;
        }
    }
}
