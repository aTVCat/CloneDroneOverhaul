using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul
{
    public static class OverhaulGPUInstancingController
    {
        public static OverhaulGPUInstanceDrawer CubeBasicInstancer;
        public static OverhaulGPUInstanceDrawer CubeGlowingInstancer;
        public static OverhaulGPUInstanceDrawer PlatformInstancer;

        public static void Initialize()
        {
            if (!OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IsGpuInstancingEnabled)
                return;

            CubeBasicInstancer = new GameObject("CubeBasicInstancer").AddComponent<OverhaulGPUInstanceDrawer>();
            CubeGlowingInstancer = new GameObject("CubeGlowingInstancer").AddComponent<OverhaulGPUInstanceDrawer>();
            PlatformInstancer = new GameObject("PlatformInstancer").AddComponent<OverhaulGPUInstanceDrawer>();
        }

        public static Matrix4x4 GetMatrix(this Transform transform) => Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
    }
}
