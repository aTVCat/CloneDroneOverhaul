using CDOverhaul.Gameplay;
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
            return firstPersonMover != null && firstPersonMover.HasCharacterModel()
&& !UnsupportedCharacters.Contains(firstPersonMover.CharacterType);
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
        private Renderer[] m_ShieldRenderers;

        private Transform m_FPModeCameraParent;
        private Camera m_Camera;

        public override void Start()
        {
            base.Start();

            m_ShieldRenderers = GetRenderersOfBodyPart(Owner, MechBodyPartType.Shield);
            m_HeadRenderers = GetRenderersOfBodyPart(Owner, MechBodyPartType.Head);
            m_JawRenderers = GetRenderersOfBodyPart(Owner, "Jaw");
            m_Camera = Owner.GetPlayerCamera();
            m_FPModeCameraParent = Owner.GetBodyPartParent("Head");

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
            if (m_FPModeCameraParent != null && ViewModesController.IsFirstPersonModeEnabled && m_Camera != null && !PhotoManager.Instance.IsInPhotoMode())
            {
                m_Camera.transform.position = m_FPModeCameraParent.transform.position + (m_FPModeCameraParent.transform.up * 0.45f);
                m_Camera.transform.eulerAngles = m_FPModeCameraParent.transform.eulerAngles;
            }

            if (Time.frameCount % 5 == 0)
            {
                RefreshHeadVisibility();
            }
        }

        public void RefreshView()
        {
            m_Camera = Owner.GetPlayerCamera();

            RefreshHeadVisibility();
        }

        public void RefreshHeadVisibility()
        {
            SetHeadRenderersActive(!ViewModesController.IsFirstPersonModeEnabled || !Owner.IsMainPlayer() || !Owner.IsAlive() || PhotoManager.Instance.IsInPhotoMode());
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

            if (!m_ShieldRenderers.IsNullOrEmpty())
                foreach (Renderer renderer in m_ShieldRenderers)
                {
                    if (renderer)
                        renderer.enabled = value;
                }
        }
    }
}
