using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul
{
    public class OverhaulGPUInstanceObjectBehaviour : OverhaulBehaviour
    {
        public static string[] SupportedPaths = new string[]
        {
            "Prefabs/LevelObjects/Primitives/CubeGlowing",
             "Prefabs/LevelObjects/Primitives/CubeFlat",
             "Prefabs/LevelObjects/Primitives/Platform"
        };

        public OverhaulGPUInstanceDrawer Drawer;

        private Matrix4x4 m_OldMatrix;
        private Vector3 m_OldPos, m_OldEuler, m_OldScale;
        private bool m_Disabled;

        public override void Start()
        {
            ObjectPlacedInLevel objectPlacedInLevel = base.GetComponent<ObjectPlacedInLevel>();
            if (!objectPlacedInLevel)
            {
                base.enabled = false;
                return;
            }

            string path = objectPlacedInLevel.LevelObjectEntry.PathUnderResources;
            if (!SupportedPaths.Contains(path))
                return;

            MeshFilter filter = base.GetComponent<MeshFilter>();
            MeshRenderer renderer = base.GetComponent<MeshRenderer>();
            renderer.material.enableInstancing = true;
            renderer.enabled = false;

            Drawer = null;
            if (path == "Prefabs/LevelObjects/Primitives/CubeGlowing")
            {
                Drawer = OverhaulGPUInstancingController.CubeGlowingInstancer;
            }
            else if (path == "Prefabs/LevelObjects/Primitives/CubeFlat")
            {
                Drawer = OverhaulGPUInstancingController.CubeBasicInstancer;
            }
            else if (path == "Prefabs/LevelObjects/Primitives/Platform")
            {
                Drawer = OverhaulGPUInstancingController.PlatformInstancer;
            }

            if (!Drawer)
            {
                base.enabled = false;
                return;
            }

            if (!Drawer.MeshToDraw)
            {
                Drawer.MeshToDraw = filter.sharedMesh;
                //instancerBehaviour.Materials = new Material[] {renderer.material};
            }
            //Drawer.AddObject(base.transform, renderer.material, out m_OldMatrix);

            m_OldPos = base.transform.position;
            m_OldEuler = base.transform.eulerAngles;
            m_OldScale = base.transform.localScale;
        }

        public override void OnDisable()
        {
            if (!m_Disabled)
            {
                UpdateDraw();
                m_Disabled = true;
            }
        }

        
        public override void OnEnable()
        {
            if (m_Disabled)
            {
                UpdateDraw();
                m_Disabled = false;
            }
        }

        protected override void OnDisposed()
        {
            UpdateDraw();
        }

        private void Update()
        {
            Vector3 newPos = base.transform.position;
            Vector3 newEuler = base.transform.eulerAngles;
            Vector3 newLocalScale = base.transform.localScale;

            if((newPos, newEuler, newLocalScale) != (m_OldPos, m_OldEuler, m_OldScale))
            {
                UpdateDraw();
            }

            m_OldPos = newPos;
            m_OldEuler = newEuler;
            m_OldScale = newLocalScale;
        }

        public void UpdateDraw()
        {
            if (Drawer)
            {
                //Drawer.UpdateObject(base.transform, m_OldMatrix);
                m_OldMatrix = base.transform.GetMatrix();
            }
        }
    }
}
