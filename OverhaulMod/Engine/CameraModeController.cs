using OverhaulMod.Utils;
using OverhaulMod.Visuals;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class CameraModeController : MonoBehaviour
    {
        private CameraManager m_cameraManager;

        private FirstPersonMover m_owner;

        private PlayerCameraMover m_cameraMover;

        private Animator m_cameraAnimator;

        private Transform m_targetTransform;

        private Vector3 m_offset;

        private float m_lerp;

        public float ForwardVectorMultiplier = 0.25f;

        public Vector3 ShakePositionOffset, ShakeRotationOffset;

        private List<Renderer> m_headRenderers;
        public List<Renderer> headRenderers
        {
            get
            {
                if (m_headRenderers == null)
                    m_headRenderers = m_owner.GetRenderersOfBodyPart("Head");

                return m_headRenderers;
            }
        }

        private List<Renderer> m_jawRenderers;
        public List<Renderer> jawRenderers
        {
            get
            {
                if (m_jawRenderers == null)
                    m_jawRenderers = m_owner.GetRenderersOfBodyPart("Jaw");

                return m_jawRenderers;
            }
        }

        private List<Renderer> m_shieldRenderers;
        public List<Renderer> shieldRenderers
        {
            get
            {
                if (m_shieldRenderers == null)
                    m_shieldRenderers = m_owner.GetRenderersOfBodyPart("Shield");

                return m_shieldRenderers;
            }
        }

        private List<Renderer> m_torsoRenderers;
        public List<Renderer> torsoRenderers
        {
            get
            {
                if (m_torsoRenderers == null)
                    m_torsoRenderers = m_owner.GetRenderersOfBodyPart("Torso");

                return m_torsoRenderers;
            }
        }

        private void Start()
        {
            m_lerp = CameraManager.EnableFirstPersonMode ? 0f : 1f;
            m_cameraManager = CameraManager.Instance;

            GlobalEventManager.Instance.AddEventListener(GlobalEvents.ConsciousnessTransferStarted, ForceEnableHeadRenderers);
            GlobalEventManager.Instance.AddEventListener(GlobalEvents.ConsciousnessTransferComplete, ForceEnableHeadRenderers);

            if (CameraManager.EnableFirstPersonMode)
                PostEffectsManager.Instance.RefreshCameraPostEffects();
        }

        private void OnDestroy()
        {
            GlobalEventManager.Instance.RemoveEventListener(GlobalEvents.ConsciousnessTransferStarted, ForceEnableHeadRenderers);
            GlobalEventManager.Instance.RemoveEventListener(GlobalEvents.ConsciousnessTransferComplete, ForceEnableHeadRenderers);
        }

        private void LateUpdate()
        {
            int frameCount = Time.frameCount;
            bool refresh = frameCount % 5 == 0;

            FirstPersonMover firstPersonMover = m_owner;
            if (IsMindTransferInProgress() || !firstPersonMover || !firstPersonMover.IsAlive())
                return;

            m_lerp = Mathf.Clamp01(m_lerp + ((CameraManager.EnableFirstPersonMode && !m_cameraManager.enableThirdPerson && !MultiplayerSpectateManager.Instance.IsInMultiplayerSpectatorMode() && (!GameModeManager.UsesMultiplayerSpawnPoints() || firstPersonMover.HasConstructionFinished()) && !firstPersonMover._isGrabbedForUpgrade ? -Time.unscaledDeltaTime : Time.unscaledDeltaTime) * 3f));

            if (refresh)
                RefreshHeadVisibility(m_lerp);

            Animator animator = m_cameraAnimator;
            if (!m_targetTransform || !animator || !animator.enabled)
                return;

            PlayerCameraMover playerCameraMover = m_cameraMover;
            if (playerCameraMover)
            {
                if (m_lerp > 0f && m_lerp < 1f)
                    playerCameraMover._hasCollidedWithEnvironment = true;
                else if (m_lerp == 0f)
                    playerCameraMover._hasCollidedWithEnvironment = false;

                playerCameraMover.LateUpdate();
            }

            Vector3 forwardVector = m_targetTransform.forward * (firstPersonMover._isOnFloorFromKick ? 0f : ForwardVectorMultiplier);
            Vector3 upVector = m_targetTransform.forward * (firstPersonMover._isOnFloorFromKick ? 0.35f : 0f);

            Transform transform = base.transform;
            Vector3 difference = transform.position;
            transform.position = ModUnityUtils.LerpVector3(m_targetTransform.position + m_offset + forwardVector + upVector, difference, NumberUtils.EaseInOutCubic(0f, 1f, m_lerp)) + ShakePositionOffset;

            if (m_lerp <= 0.99f)
            {
                Vector3 localPosition = transform.localPosition;
                localPosition.x = ShakePositionOffset.x;
                transform.localPosition = localPosition;
            }
        }

        public void Initialize(CameraManager cameraManager, FirstPersonMover owner, PlayerCameraMover playerCameraMover, Animator animator, Transform transformToFollow)
        {
            m_owner = owner;
            m_cameraMover = playerCameraMover;
            m_cameraAnimator = animator;
            m_targetTransform = transformToFollow;
            m_cameraManager = cameraManager;
            RefreshOffset();
        }

        public void SetHeadRenderersActive(bool value)
        {
            setRenderersActive(headRenderers, value);
            setRenderersActive(jawRenderers, value);
            setRenderersActive(shieldRenderers, value);
            setRenderersActive(torsoRenderers, (m_owner ? m_owner.IsMindSpaceCharacter : false) || value || m_owner._isOnFloorFromKick);
        }

        public bool IsMindTransferInProgress()
        {
            PlayerCameraMover mover = m_cameraMover;
            return mover && mover._isConsciousnessTransferInProgress;
        }

        public void ForceEnableHeadRenderers()
        {
            SetHeadRenderersActive(true);
        }

        public void RefreshOffset()
        {
            if (m_owner.IsMindSpaceCharacter)
            {
                m_offset = Vector3.up * 0.575f;
                return;
            }

            Bounds bounds = new Bounds(m_targetTransform.localPosition, Vector3.zero);
            int index = 0;
            foreach (MeshFilter meshFilter in m_targetTransform.GetComponentsInChildren<MeshFilter>())
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

            m_offset = new Vector3(0f, bounds.center.y + (bounds.extents.y * 0.2f), 0f);
        }

        public void RefreshHeadVisibility(float lerpValue)
        {
            Animator animator = m_cameraAnimator;
            SetHeadRenderersActive(lerpValue > 0.1f || !animator || !animator.enabled || !m_owner.IsAlive() || PhotoManager.Instance.IsInPhotoMode());
        }

        private void setRenderersActive(List<Renderer> list, bool value)
        {
            if (list.Count == 0)
                return;

            for (int i = 0; i < list.Count; i++)
            {
                Renderer renderer = list[i];
                if (renderer)
                {
                    renderer.enabled = value;
                }
            }
        }
    }
}
