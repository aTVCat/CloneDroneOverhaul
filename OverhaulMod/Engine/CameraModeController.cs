using UnityEngine;

namespace OverhaulMod.Engine
{
    public class CameraModeController : MonoBehaviour
    {
        private PlayerCameraMover m_cameraMover;
        private Animator m_cameraAnimator;

        private Transform m_transformToFollow;

        private FirstPersonMover m_owner;

        private Vector3 m_offset;

        public bool hasToRefreshFields
        {
            get;
            set;
        }

        public void SetOwner(FirstPersonMover firstPersonMover)
        {
            m_owner = firstPersonMover;
            hasToRefreshFields = true;
        }

        public void RefreshFields()
        {
            m_cameraMover = base.GetComponent<PlayerCameraMover>();

            FirstPersonMover firstPersonMover = m_owner;
            if (!firstPersonMover || !firstPersonMover.HasCharacterModel())
                return;

            m_cameraAnimator = firstPersonMover._cameraHolderAnimator;

            MechBodyPart headPart = m_owner.GetBodyPart(MechBodyPartType.Head);
            if (!headPart)
            {
                return;
            }

            Transform transform = headPart.transform.parent;
            m_transformToFollow = transform;

            RefreshOffset();
        }

        public void RefreshOffset()
        {
            if (!m_transformToFollow)
            {
                m_offset = Vector3.zero;
                return;
            }

            Bounds bounds = new Bounds(m_transformToFollow.localPosition, Vector3.one);
            int index = 0;
            foreach (MeshFilter meshFilter in m_transformToFollow.GetComponentsInChildren<MeshFilter>())
            {
                Mesh mesh = meshFilter.mesh;
                if (mesh)
                {
                    if (index == 0)
                        bounds = mesh.bounds;
                    else
                        bounds.Encapsulate(mesh.bounds);
                }
                index++;
            }

            m_offset = new Vector3(0f, bounds.center.y + (bounds.extents.y * 0.15f), 0f);
        }

        public bool IsMindTransferInProgress()
        {
            PlayerCameraMover mover = m_cameraMover;
            return mover && mover._isConsciousnessTransferInProgress;
        }

        private void Update()
        {
            PlayerCameraMover mover = m_cameraMover;
            if (mover)
            {
                CameraManager manager = CameraManager.Instance;
                FirstPersonMover firstPersonMover = m_owner;

                if (CameraManager.EnableFirstPersonMode)
                {
                    mover._hasCollidedWithEnvironment = false;
                }
                else if (manager && manager.refreshCameraMoverNextFrame && firstPersonMover && firstPersonMover.IsMainPlayer())
                {
                    mover._hasCollidedWithEnvironment = true;
                    manager.refreshCameraMoverNextFrame = false;
                }
            }
        }

        private void LateUpdate()
        {
            if (hasToRefreshFields)
            {
                RefreshFields();
                hasToRefreshFields = false;
            }

            if (!CameraManager.EnableFirstPersonMode || IsMindTransferInProgress() || !m_owner || !m_owner.IsAlive() || !m_owner.HasCharacterModel())
                return;

            if (!m_transformToFollow || (m_cameraAnimator && !m_cameraAnimator.enabled))
                return;

            base.transform.position = m_transformToFollow.position + m_offset;

            CharacterModel characterModel = m_owner.GetCharacterModel();
            characterModel.transform.localPosition = Vector3.zero;
        }
    }
}
