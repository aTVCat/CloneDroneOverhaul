using UnityEngine;

namespace OverhaulAPI
{
    public class SerializeTransform
    {
        public Vector3 Position;

        public Vector3 EulerAngles;

        public Vector3 LocalScale;

        public string ParentName;

        public static SerializeTransform SerializeTheTransform(Transform transform)
        {
            SerializeTransform s = new SerializeTransform()
            {
                Position = transform.localPosition,
                EulerAngles = transform.localEulerAngles,
                LocalScale = transform.localScale,
                ParentName = transform.parent != null ? transform.parent.name : string.Empty
            };
            return s;
        }

        public static void ApplyOnTransform(SerializeTransform sTransform, Transform transform)
        {
            if (sTransform == null || transform == null) return;
            transform.localPosition = sTransform.Position;
            transform.localEulerAngles = sTransform.EulerAngles;
            transform.localScale = sTransform.LocalScale;
        }
    }
}
