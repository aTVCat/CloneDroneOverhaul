using UnityEngine;

namespace OverhaulMod.Engine
{
    public class ModPhysicsManager : Singleton<ModPhysicsManager>
    {
        private static readonly RaycastHit[] _rayCastHitArray = new RaycastHit[10];

        private static readonly Ray _ray = new Ray();

        public static RaycastHit[] GetRayCastHitArray(bool clear = true)
        {
            RaycastHit[] result = _rayCastHitArray;
            if (clear)
                for (int i = 0; i < result.Length; i++)
                    result[i] = default;

            return result;
        }

        public static Ray GetRay(Vector3 position, Vector3 direction)
        {
            Ray ray = _ray;
            ray.origin = position;
            ray.direction = direction;
            return ray;
        }
    }
}
