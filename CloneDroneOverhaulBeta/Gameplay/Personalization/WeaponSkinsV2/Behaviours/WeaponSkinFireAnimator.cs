using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class WeaponSkinFireAnimator : WeaponSkinBehaviour
    {
        private Renderer m_Renderer;
        private Material m_Material;

        public Color TargetColor = Color.white;
        private const string Emission = "_EmissionColor";

        public override void Awake()
        {
            refresh();
        }

        public override void OnEnable()
        {
            refresh();
        }

        protected override void OnDisposed()
        {
            m_Renderer = null;
            m_Material = null;
        }

        private void refresh()
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }
            m_Renderer = base.GetComponent<Renderer>();
            if (m_Renderer == null)
            {
                return;
            }

            m_Material = m_Renderer.material;
            updateColor();
        }

        private void updateColor()
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }
            m_Material.SetColor(Emission, TargetColor * (Mathf.PingPong(Time.time, 2f) + 3f));
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

            updateColor();
        }

        public override void OnBeginDraw()
        {
        }

        public override void OnEndDraw()
        {
        }

        public override void OnDeath()
        {
        }

        public override void OnSetColor(Color color)
        {
            if(color == Color.clear)
            {
                TargetColor = Color.white;
                return;
            }
            TargetColor = color;
        }
    }
}