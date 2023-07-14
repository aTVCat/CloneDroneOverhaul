using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul.Graphics.GPUInstancing
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
