using CDOverhaul.Gameplay;
using ModLibrary;
using System.Linq;
using UnityEngine;

namespace CDOverhaul.Graphics
{
    public class ViewModesExpansion : OverhaulCharacterExpansion
    {
        private static readonly Renderer[] s_EmptyRendererArray = new Renderer[0];
        public static readonly EnemyType[] UnsupportedCharacters = new EnemyType[]
        {
            EnemyType.EmperorNonCombat,
            EnemyType.EmperorCombat
        };

        public static bool IsFirstPersonMoverSupported(in FirstPersonMover firstPersonMover)
        {
            if (firstPersonMover == null || !firstPersonMover.HasCharacterModel())
                return false;

            return !UnsupportedCharacters.Contains(firstPersonMover.CharacterType);
        }
        public static Renderer[] GetRenderersOfBodyPart(in FirstPersonMover firstPersonMover, in MechBodyPartType bodyPartType)
        {
            if (!IsFirstPersonMoverSupported(firstPersonMover) || bodyPartType == MechBodyPartType.None)
                return s_EmptyRendererArray;

            MechBodyPart mechBodyPart = firstPersonMover.GetBodyPart(bodyPartType);
            if (mechBodyPart == null)
                return s_EmptyRendererArray;

            Renderer[] result = mechBodyPart.GetComponentsInChildren<Renderer>();
            return result;
        }
        public static Renderer[] GetRenderersOfBodyPart(in FirstPersonMover firstPersonMover, in string bodyPart)
        {
            if (!IsFirstPersonMoverSupported(firstPersonMover) || string.IsNullOrEmpty(bodyPart))
                return s_EmptyRendererArray;

            Transform transform = firstPersonMover.GetBodyPartParent(bodyPart);
            if (transform == null)
                return s_EmptyRendererArray;

            Renderer[] result = transform.GetComponentsInChildren<Renderer>();
            return result;
        }

        private Renderer[] m_HeadRenderers;
        private Renderer[] m_JawRenderers;

        private Transform m_DefaultCameraParent;
        private Transform m_DefaultCameraHolder;
        private Transform m_FPModeCameraParent;
        private Camera m_Camera;

        public override void Start()
        {
            base.Start();

            m_HeadRenderers = GetRenderersOfBodyPart(Owner, MechBodyPartType.Head);
            m_JawRenderers = GetRenderersOfBodyPart(Owner, "Jaw");
            m_Camera = Owner.GetPlayerCamera();
            m_FPModeCameraParent = Owner.GetBodyPartParent("Head");
            m_DefaultCameraHolder = Owner.GetPrivateField<Transform>("_cameraHolderTransform");
            m_DefaultCameraParent = m_Camera.transform.parent;

            RefreshView();

            OverhaulEventsController.AddEventListener(OverhaulGameplayCoreController.PlayerSetAsCharacter, RefreshView);
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            SetHeadRenderersActive(true);

            OverhaulEventsController.RemoveEventListener(OverhaulGameplayCoreController.PlayerSetAsCharacter, RefreshView);
        }

        protected override void OnDeath()
        {
            DestroyBehaviour();
        }

        private void LateUpdate()
        {
            if (ViewModesController.IsFirstPersonModeEnabled && m_Camera != null && !PhotoManager.Instance.IsInPhotoMode())
            {
                m_Camera.transform.localPosition = ViewModesController.DefaultCameraOffset;
                m_Camera.transform.localEulerAngles = Vector3.zero;
            }

            if(Time.frameCount % 5 == 0)
            {
                RefreshView();
            }
        }

        public void RefreshView()
        {
            m_Camera = Owner.GetPlayerCamera();

            RefreshHeadVisibility();
            RefreshCameraParent();
        }

        public void RefreshCameraParent()
        {
            if (!m_Camera || !m_DefaultCameraParent || !m_FPModeCameraParent)
                return;

            bool isFPModeEnabled = ViewModesController.IsFirstPersonModeEnabled;
            Transform toSet = isFPModeEnabled ? m_FPModeCameraParent : m_DefaultCameraParent;

            if (PhotoManager.Instance.IsInPhotoMode())
                return;

            m_Camera.transform.SetParent(toSet, false);

            Owner.SetPrivateField("_playerCameraParent", toSet);
            Owner.SetPrivateField("_cameraHolderTransform", isFPModeEnabled ? toSet : m_DefaultCameraHolder);
        }

        public void RefreshHeadVisibility()
        {
            SetHeadRenderersActive(!ViewModesController.IsFirstPersonModeEnabled || !Owner.IsMainPlayer() || !Owner.IsAlive());
        }

        public void SetHeadRenderersActive(bool value)
        {
            if (!m_HeadRenderers.IsNullOrEmpty())
                foreach (Renderer renderer in m_HeadRenderers)
                {
                    if (renderer)
                        renderer.enabled = value;
                }

            if (!m_JawRenderers.IsNullOrEmpty())
                foreach (Renderer renderer in m_JawRenderers)
                {
                    if (renderer)
                        renderer.enabled = value;
                }
        }
    }
}
