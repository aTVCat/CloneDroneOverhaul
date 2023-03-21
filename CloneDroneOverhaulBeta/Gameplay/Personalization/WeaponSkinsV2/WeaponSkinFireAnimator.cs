using System.Collections;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class WeaponSkinFireAnimator : OverhaulBehaviour
    {
        private Renderer m_Renderer;
        private Material m_Material;

        public Color TargetColor = "#BFBFBF".ConvertHexToColor();

        private const string Emission = "_EmissionColor";

        public override void OnEnable()
        {
            m_Renderer = base.GetComponent<Renderer>();
            if(m_Renderer == null)
            {
                return;
            }

            m_Material = m_Renderer.material;
        }

        protected override void OnDisposed()
        {
            m_Renderer = null;
            m_Material = null;
        }

        private void Update()
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            if (m_Material == null)
            {
                return;
            }

            m_Material.SetColor(Emission, TargetColor * (Mathf.PingPong(Time.time, 3f) + 3f));
        }
    }
}