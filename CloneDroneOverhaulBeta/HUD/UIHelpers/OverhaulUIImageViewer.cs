using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class OverhaulUIImageViewer : OverhaulUI  
    {
        private static OverhaulUIImageViewer m_Viewer;
        public static bool IsNull => m_Viewer == null || m_Viewer.IsDisposedOrDestroyed();

        public RawImage Image;

        public override void Initialize()
        {
            m_Viewer = this;
            Image = MyModdedObject.GetObject<RawImage>(0);
            SetActive(false, null);
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            m_Viewer = null;
            Image = null;
        }

        public static void SetActive(bool value, Texture texture)
        {
            if (IsNull)
            {
                return;
            }

            m_Viewer.gameObject.SetActive(value);
            m_Viewer.Image.texture = texture;
        }

        private void Update()
        {
            if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
            {
                SetActive(false, null);
            }
        }
    }
}