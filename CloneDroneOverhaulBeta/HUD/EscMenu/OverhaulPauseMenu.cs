using System.Collections;
using UnityEngine;

namespace CDOverhaul.HUD
{
    public class OverhaulPauseMenu : OverhaulUI
    {
        [OverhaulSetting("Game interface.Gameplay.New pause menu", true, false, "wip")]
        public static bool UseThisMenu;

        public static bool ForceUseOldMenu;

        #region Open/Close menu

        private static OverhaulPauseMenu m_Instance;

        private float m_TargetFoV = 60f;
        private bool m_IsAnimatingCamera;

        public static void ToggleMenu()
        {
            if (!m_Instance.AllowToggleMenu)
            {
                return;
            }

            if (m_Instance.gameObject.activeSelf)
            {
                m_Instance.Hide();
                return;
            }
            m_Instance.Show();
        }

        #endregion

        private Animator m_CameraAnimator;
        private Camera m_Camera;

        private float m_TimeMenuChangedItsState;
        public bool AllowToggleMenu => Time.unscaledTime >= m_TimeMenuChangedItsState + 0.45f;


        public override void Initialize()
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }
            m_Instance = this;

            Hide();
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
        }

        public void Show()
        {
            m_TimeMenuChangedItsState = Time.unscaledTime;
            base.gameObject.SetActive(true);
            animateCamera();

            TimeManager.Instance.OnGamePaused();

            ShowCursor = true;
        }

        public void Hide()
        {
            m_TimeMenuChangedItsState = Time.unscaledTime;
            base.gameObject.SetActive(false);

            TimeManager.Instance.OnGameUnPaused();

            if (!m_IsAnimatingCamera && m_CameraAnimator != null)
            {
                _ = StaticCoroutineRunner.StartStaticCoroutine(animateCameraCoroutine(m_Camera, m_CameraAnimator, true));
            }

            ShowCursor = false;
        }

        private void Update()
        {
            if (AllowToggleMenu && Input.GetKeyDown(KeyCode.Alpha0))
            {
                ForceUseOldMenu = true;
                Hide();
                GameUIRoot.Instance.EscMenu.Show();
                GameUIRoot.Instance.RefreshCursorEnabled();
                ForceUseOldMenu = false;
            }
        }

        private void animateCamera()
        {
            if (CharacterTracker.Instance == null)
            {
                return;
            }

            Character player = CharacterTracker.Instance.GetPlayer();
            if(player == null)
            {
                return;
            }

            m_Camera = player.GetPlayerCamera();
            if(m_Camera == null)
            {
                return;
            }

            m_CameraAnimator = m_Camera.GetComponentInParent<Animator>();
            if (m_CameraAnimator == null)
            {
                return;
            }

            if (m_IsAnimatingCamera)
            {
                return;
            }

            m_TargetFoV = m_Camera.fieldOfView;
            _ = StaticCoroutineRunner.StartStaticCoroutine(animateCameraCoroutine(m_Camera, m_CameraAnimator, false));
        }

        private IEnumerator animateCameraCoroutine(Camera camera, Animator animator, bool animatorState)
        {
            m_IsAnimatingCamera = true;
            int iterations = 20;
            if (!animatorState)
            {
                animator.enabled = false;
                while (iterations > -1)
                {
                    iterations--;
                    camera.fieldOfView += (40 - camera.fieldOfView) * 0.5f * (Time.unscaledDeltaTime * 22);
                    yield return null;
                }
            }
            else
            {
                while (iterations > -1)
                {
                    iterations--;
                    camera.fieldOfView += (m_TargetFoV - camera.fieldOfView) * 0.5f * (Time.unscaledDeltaTime * 22);
                    yield return null;
                }
                animator.enabled = true;
            }
            m_IsAnimatingCamera = false;
            yield break;
        }
    }
}