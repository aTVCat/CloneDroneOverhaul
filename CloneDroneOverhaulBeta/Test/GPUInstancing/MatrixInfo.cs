using UnityEngine;

namespace CDOverhaul.Visuals.GPUInstancing
{
    public class MatrixInfo
    {
        public Transform TargetTransform;

        public MaterialInfo MaterialInfo;

        public Matrix4x4 Matrix => TargetTransform.GetMatrix();

        public bool IsDestroyed() => !TargetTransform;
        public bool IsDisabled() => !TargetTransform.gameObject.activeInHierarchy;
    }
}
