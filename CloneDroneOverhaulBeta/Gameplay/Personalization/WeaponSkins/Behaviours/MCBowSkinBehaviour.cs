using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class MCBowSkinBehaviour : WeaponSkinBehaviour
    {
        private static bool m_HasFrames;
        private static Mesh m_Frame1;
        private static Mesh m_Frame2;
        private static Mesh m_Frame3;
        private static Mesh m_Frame4;

        private MeshFilter m_Filter;
        private float m_TimeBeganDrawing;

        private int m_CurrentFrame;
        private bool m_OwnerDied;

        public override void OnBeginDraw()
        {
            if (m_TimeBeganDrawing != -1f)
            {
                return;
            }
            m_TimeBeganDrawing = Time.time;
        }

        public override void OnEndDraw()
        {
            m_TimeBeganDrawing = -1f;
        }

        public override void OnDeath()
        {
            m_OwnerDied = true;
            SetFrame(1);
        }

        public override void OnPreLoad()
        {
            m_HasFrames = true;
            m_Frame1 = OverhaulAssetsController.GetAsset<Mesh>("MCBowMesh1", OverhaulAssetPart.WeaponSkins);
            m_Frame2 = OverhaulAssetsController.GetAsset<Mesh>("MCBowMesh2", OverhaulAssetPart.WeaponSkins);
            m_Frame3 = OverhaulAssetsController.GetAsset<Mesh>("MCBowMesh3", OverhaulAssetPart.WeaponSkins);
            m_Frame4 = OverhaulAssetsController.GetAsset<Mesh>("MCBowMesh4", OverhaulAssetPart.WeaponSkins);
        }

        public override void Start()
        {
            m_TimeBeganDrawing = -1f;
            m_Filter = base.GetComponent<MeshFilter>();
        }

        public override void OnDisable()
        {
            OnEndDraw();
        }

        protected override void OnDisposed()
        {
            m_Filter = null;
        }

        private void Update()
        {
            if (IsDisposedOrDestroyed() || m_OwnerDied)
            {
                return;
            }

            float time = Time.time;
            if (m_TimeBeganDrawing != -1f && time >= m_TimeBeganDrawing)
            {
                if (time >= m_TimeBeganDrawing + 0.45f)
                {
                    SetFrame(4);
                }
                else if (time >= m_TimeBeganDrawing + 0.3f)
                {
                    SetFrame(3);
                }
                else if (time >= m_TimeBeganDrawing + 0.15f)
                {
                    SetFrame(2);
                }
            }
            else
            {
                SetFrame(1);
            }
        }

        public void SetFrame(byte frame)
        {
            if (IsDisposedOrDestroyed() || !m_HasFrames)
            {
                return;
            }

            if (m_OwnerDied)
            {
                frame = 1;
            }
            if (m_CurrentFrame == frame || m_Filter == null)
            {
                return;
            }

            m_CurrentFrame = frame;
            switch (frame)
            {
                case 1:
                    m_Filter.mesh = m_Frame1;
                    break;
                case 2:
                    m_Filter.mesh = m_Frame2;
                    break;
                case 3:
                    m_Filter.mesh = m_Frame3;
                    break;
                case 4:
                    m_Filter.mesh = m_Frame4;
                    break;
                default:
                    m_Filter.mesh = m_Frame1;
                    break;
            }
        }
    }
}