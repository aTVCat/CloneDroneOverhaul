using CDOverhaul.Gameplay;
using System;
using System.Linq;
using UnityEngine;

namespace CDOverhaul.Graphics
{
    public class ViewModesExpansion : OverhaulCharacterExpansion
    {
        public static readonly EnemyType[] UnsupportedCharacters = new EnemyType[]
        {
            EnemyType.EmperorNonCombat,
            EnemyType.EmperorCombat
        };

        public static bool IsFirstPersonMoverSupported(in FirstPersonMover firstPersonMover) => firstPersonMover != null && firstPersonMover.HasCharacterModel() && !UnsupportedCharacters.Contains(firstPersonMover.CharacterType);

        public static Renderer[] GetRenderersOfBodyPart(in FirstPersonMover firstPersonMover, in MechBodyPartType bodyPartType)
        {
            if (!IsFirstPersonMoverSupported(firstPersonMover) || bodyPartType == MechBodyPartType.None)
                return Array.Empty<Renderer>();

            MechBodyPart mechBodyPart = firstPersonMover.GetBodyPart(bodyPartType);
            if (mechBodyPart == null)
                return Array.Empty<Renderer>();

            Renderer[] result = mechBodyPart.GetComponentsInChildren<Renderer>();
            return result;
        }
        public static Renderer[] GetRenderersOfBodyPart(in FirstPersonMover firstPersonMover, in string bodyPart)
        {
            if (!IsFirstPersonMoverSupported(firstPersonMover) || string.IsNullOrEmpty(bodyPart))
                return Array.Empty<Renderer>();

            Transform transform = firstPersonMover.GetBodyPartParent(bodyPart);
            if (transform == null)
                return Array.Empty<Renderer>();

            Renderer[] result = transform.GetComponentsInChildren<Renderer>();
            return result;
        }

        private Renderer[] m_HeadRenderers;
        public Renderer[] HeadRenderers
        {
            get
            {
                if (m_HeadRenderers.IsNullOrEmpty())
                {
                    m_HeadRenderers = GetRenderersOfBodyPart(Owner, "Head");
                }
                return m_HeadRenderers;
            }
        }

        private Renderer[] m_JawRenderers;
        public Renderer[] JawRenderers
        {
            get
            {
                if (m_JawRenderers.IsNullOrEmpty())
                {
                    m_JawRenderers = GetRenderersOfBodyPart(Owner, "Jaw");
                }
                return m_JawRenderers;
            }
        }

        private Renderer[] m_ShieldRenderers;
        public Renderer[] ShieldRenderers
        {
            get
            {
                if (m_ShieldRenderers.IsNullOrEmpty())
                {
                    m_ShieldRenderers = GetRenderersOfBodyPart(Owner, MechBodyPartType.Shield);
                }
                return m_ShieldRenderers;
            }
        }

        private LevelEditorCinematicCamera m_CinematicCamera;
        private bool m_CineCameraOn;
        private bool m_HasRefreshedCinematicCameraOnStart;

        private Transform m_FPModeCameraParent;
        private Camera m_Camera;

        public override void Start()
        {
            base.Start();

            m_Camera = Owner.GetPlayerCamera();
            m_FPModeCameraParent = Owner.GetBodyPartParent("Head");

            RefreshView();

            OverhaulEvents.AddEventListener(OverhaulGameplayCoreController.PlayerSetAsCharacter, RefreshView);
            OverhaulEvents.AddEventListener<LevelEditorCinematicCamera>(GlobalEvents.CinematicCameraTurnedOn, OnCinematicCameraTurnedOn, true);
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            SetHeadRenderersActive(true);

            OverhaulEvents.RemoveEventListener(OverhaulGameplayCoreController.PlayerSetAsCharacter, RefreshView);
            OverhaulEvents.RemoveEventListener<LevelEditorCinematicCamera>(GlobalEvents.CinematicCameraTurnedOn, OnCinematicCameraTurnedOn, true);
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
                //Vector3 bowAimOffset = Owner.IsAimingBow() ? ViewModesController.AimBowCameraOffset : Vector3.zero;
                m_Camera.transform.parent.localPosition = new Vector3(0f, 1.3f, -4.8f);

                m_Camera.transform.position = m_FPModeCameraParent.transform.position +
                    (m_FPModeCameraParent.transform.up *
                    (ViewModesController.DefaultCameraUpTransformMultiplier +
                    (ViewModesController.IsLargeBot(Owner) ? ViewModesController.AdditionalCameraUpTransformMultiplier : 0f) +
                    (Owner.IsRidingOtherCharacter() ? ViewModesController.AdditionalCameraUpTransformMultiplier * 2f : 0f)));

                Vector3 vector3 = m_Camera.transform.localPosition;
                vector3.x = 0f;
                m_Camera.transform.localPosition = vector3;

                if (ViewModesController.SyncCameraWithHeadRotation)
                    m_Camera.transform.eulerAngles = m_FPModeCameraParent.transform.eulerAngles;
            }

            if (Time.frameCount % 10 == 0)
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
            SetHeadRenderersActive(m_CineCameraOn || !ViewModesController.IsFirstPersonModeEnabled || !Owner.IsAlive() || PhotoManager.Instance.IsInPhotoMode());
        }

        public void SetHeadRenderersActive(bool value)
        {
            if (!Owner || !Owner.IsMainPlayer())
                return;

            Renderer[] array1 = HeadRenderers;
            if (!array1.IsNullOrEmpty())
                foreach (Renderer renderer in array1)
                {
                    if (renderer)
                        renderer.enabled = value;
                }

            Renderer[] array2 = JawRenderers;
            if (!array2.IsNullOrEmpty())
                foreach (Renderer renderer in array2)
                {
                    if (renderer)
                        renderer.enabled = value;
                }

            Renderer[] array3 = ShieldRenderers;
            if (!array3.IsNullOrEmpty())
                foreach (Renderer renderer in array3)
                {
                    if (renderer)
                        renderer.enabled = value;
                }
        }
    }
}
