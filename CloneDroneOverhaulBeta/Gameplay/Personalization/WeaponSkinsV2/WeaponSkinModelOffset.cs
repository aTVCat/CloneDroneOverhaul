using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class WeaponSkinModelOffset
    {
        public Vector3 OffsetPosition
        {
            get;
            private set;
        }

        public Vector3 OffsetEulerAngles
        {
            get;
            private set;
        }

        public Vector3 OffsetLocalScale
        {
            get;
            private set;
        }

        public WeaponSkinModelOffset(Vector3 position)
        {
            OffsetPosition = position;
            OffsetEulerAngles = Vector3.zero;
            OffsetLocalScale = Vector3.one;
        }

        public WeaponSkinModelOffset(Vector3 position, Vector3 eulerAngles)
        {
            OffsetPosition = position;
            OffsetEulerAngles = eulerAngles;
            OffsetLocalScale = Vector3.one;
        }

        public WeaponSkinModelOffset(Vector3 position, Vector3 eulerAngles, Vector3 localScale)
        {
            OffsetPosition = position;
            OffsetEulerAngles = eulerAngles;
            OffsetLocalScale = localScale;
        }
    }
}
