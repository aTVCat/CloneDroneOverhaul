using CDOverhaul.Visuals.GPUInstancing;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul
{
    public class OverhaulGPUInstanceDrawer : OverhaulBehaviour
    {
        public Mesh MeshToDraw;

        public List<MatrixInfo> Matrices = new List<MatrixInfo>();

        private void Update()
        {
            RenderBatches();
        }

        public void RenderBatches()
        {
            if (!MeshToDraw)
                return;
        }

        public void AddObject(Transform transform)
        {
        }
    }
}
