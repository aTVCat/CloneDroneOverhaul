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

        public static bool IsFirstPersonMoverSupported(in FirstPersonMover firstPersonMover) => firstPersonMover != null && firstPersonMover.HasCharacterModel() && !UnsupportedCharacters.Contains(firstPersonMover.CharacterType);

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

        private LevelEditorCinematicCamera m_CinematicCamera;
        private bool m_CineCameraOn;
        private bool m_HasRefreshedCinematicCameraOnStart;

        private Transform m_FPModeCameraParent;
        private Camera m_Camera;

        public override void Start()
        {
            base.Start();

            m_ShieldRenderers = GetRenderersOfBodyPart(Owner, MechBodyPartType.Shield);
            m_HeadRenderers = GetRenderersOfBodyPart(Owner, "Head");
            m_JawRenderers = GetRenderersOfBodyPart(Owner, "Jaw");
            m_Camera = Owner.GetPlayerCamera();
            m_FPModeCameraParent = Owner.GetBodyPartParent("Head");

            RefreshView();

            _ = OverhaulEventsController.AddEventListener(OverhaulGameplayCoreController.PlayerSetAsCharacter, RefreshView);
            _ = OverhaulEventsController.AddEventListener<LevelEditorCinematicCamera>(GlobalEvents.CinematicCameraTurnedOn, OnCinematicCameraTurnedOn, true);
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            SetHeadRenderersActive(true);

            OverhaulEventsController.RemoveEventListener(OverhaulGameplayCoreController.PlayerSetAsCharacter, RefreshView);
            OverhaulEventsController.RemoveEventListener<LevelEditorCinematicCamera>(GlobalEvents.CinematicCameraTurnedOn, OnCinematicCameraTurnedOn, true);
        }

        protected override void OnDeath()
        {
            DestroyBehaviour();
        }

        private void LateUpdate()
        {
            if (!m_HasRefreshedCinematicCameraOnStart)
            {
                RefreshCinematicCameraOnStart();
                m_HasRefreshedCinematicCameraOnStart = true;
            }

            if (!m_CineCameraOn && m_FPModeCameraParent != null && ViewModesController.IsFirstPersonModeEnabled && m_Camera != null && !PhotoManager.Instance.IsInPhotoMode())
            {
                if (!TimeManager.Instance.IsGamePaused()) m_Camera.fieldOfView = 80;
                m_Camera.transform.position = m_FPModeCameraParent.transform.position + (m_FPModeCameraParent.transform.up * (ViewModesController.DefaultCameraUpTransformMultiplier + (ViewModesController.IsLargeBot(Owner) ? ViewModesController.AdditionalCameraUpTransformMultiplier : 0f)));
                if (ViewModesController.SyncCameraWithHeadRotation)
                    m_Camera.transform.eulerAngles = m_FPModeCameraParent.transform.eulerAngles;
            }

            if (Time.frameCount % 5 == 0)
                RefreshHeadVisibility();

            if (m_CineCameraOn)
            {
                if (!m_CinematicCamera || !m_CinematicCamera.HasTakenOverPlayerCamera())
                    OnCinematicCameraTurnedOff();
            }
        }

        public void RefreshCinematicCameraOnStart()
        {
            if (!m_Camera || !m_Camera.transform.parent || !m_Camera.transform.parent.parent)
                return;

            Transform cinematicCameraTransform = m_Camera.transform.parent.parent;
            if (cinematicCameraTransform.gameObject.name.Contains("CinematicCamera"))
            {
                m_CinematicCamera = cinematicCameraTransform.GetComponent<LevelEditorCinematicCamera>();
                m_CineCameraOn = m_CinematicCamera;
            }
        }

        // Todo: Add a settings that allows first person mode even in cutscenes?
        public void OnCinematicCameraTurnedOn(LevelEditorCinematicCamera cam)
        {
            m_CinematicCamera = cam;
            m_CineCameraOn = true;
        }

        public void OnCinematicCameraTurnedOff()
        {
            m_CinematicCamera = null;
            m_CineCameraOn = false;
        }

        public void RefreshView()
        {
            m_Camera = Owner.GetPlayerCamera();

            RefreshHeadVisibility();
        }

        public void RefreshHeadVisibility()
        {
            SetHeadRenderersActive(m_CineCameraOn || !ViewModesController.IsFirstPersonModeEnabled || !Owner.IsMainPlayer() || !Owner.IsAlive() || PhotoManager.Instance.IsInPhotoMode());
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
