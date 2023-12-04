using UnityEngine;

namespace OverhaulMod
{
    public class TransformInfo
    {
        public Vector3 Position;

        public Vector3 EulerAngles;

        public Vector3 LocalScale;

        public TransformInfo(Vector3 pos, Vector3 ea, Vector3 ls)
        {
            Position = pos;
            EulerAngles = ea;
            LocalScale = ls;
        }

        public TransformInfo(Vector3 pos, Vector3 ea)
        {
            Position = pos;
            EulerAngles = ea;
            LocalScale = Vector3.one;
        }

        public TransformInfo(Vector3 pos)
        {
            Position = pos;
            EulerAngles = Vector3.zero;
            LocalScale = Vector3.one;
        }

        public TransformInfo()
        {
        }
    }
}
