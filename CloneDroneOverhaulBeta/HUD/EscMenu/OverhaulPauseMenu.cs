using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class OverhaulPauseMenu : OverhaulUI
    {
        [OverhaulSetting("Game interface.Gameplay.New pause menu", true, !OverhaulVersion.TechDemo2Enabled, "wip")]
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

        private Animator m_CameraAnimator;
        private Camera m_Camera;

        private float m_TimeMenuChangedItsState;
        public bool AllowToggleMenu => Time.unscaledTime >= m_TimeMenuChangedItsState + 0.45f;

        #endregion

        private Button m_PersonalizationButton;
        private Transform m_PersonalizationPanel;
        private Button m_PersonalizationSkinsButton;

        private Button m_ExitButton;
        private Transform m_ExitSelectPanel;
        private Button m_ExitSelectToMainMenuButton;
        private Button m_ExitSelectToDesktopButton;

        public override void Initialize()
        {
            m_Instance = this;

            m_PersonalizationButton = MyModdedObject.GetObject<Button>(0);
            m_PersonalizationButton.onClick.AddListener(OnPersonalizationButtonClicked);
            m_PersonalizationPanel = MyModdedObject.GetObject<Transform>(1);
            m_PersonalizationSkinsButton = MyModdedObject.GetObject<Button>(2);
            m_PersonalizationSkinsButton.onClick.AddListener(OnSkinsButtonClicked);

            m_ExitButton = MyModdedObject.GetObject<Button>(4);
            m_ExitButton.onClick.AddListener(OnExitClicked);
            m_ExitSelectPanel = MyModdedObject.GetObject<Transform>(5);
            m_ExitSelectToMainMenuButton = MyModdedObject.GetObject<Button>(6);
            m_ExitSelectToMainMenuButton.onClick.AddListener(OnMainMenuClicked);
            m_ExitSelectToDesktopButton = MyModdedObject.GetObject<Button>(7);
            m_ExitSelectToDesktopButton.onClick.AddListener(OnDesktopClicked);

            Hide();
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
        }

        public void AlignTransformY(Transform targetTransform, Transform transformToUse)
        {
            targetTransform.position = new Vector3(targetTransform.position.x, transformToUse.position.y, targetTransform.position.z);
        }

        #region Personalization

        public void OnPersonalizationButtonClicked()
        {
            SetPersonalizationPanelActive(!m_PersonalizationPanel.gameObject.activeSelf);
        }
        public void SetPersonalizationPanelActive(bool value)
        {
            if (value)
            {
                SetExitPanelActive(false);
                AlignTransformY(m_PersonalizationPanel, m_PersonalizationButton.transform);
            }
            m_PersonalizationPanel.gameObject.SetActive(value);
        }

        public void OnSkinsButtonClicked()
        {
            WeaponSkinsMenu menu = GetController<WeaponSkinsMenu>();
            if (menu == null)
            {
                return;
            }

            // Todo: Notify player about unpausing the game OR make it possible to change skins while under arena
            Hide();

            menu.SetMenuActive(true);
        }

        #endregion


        #region Exit

        public void OnExitClicked()
        {
            SetExitPanelActive(!m_ExitSelectPanel.gameObject.activeSelf);
        }
        public void SetExitPanelActive(bool value)
        {
            if (value)
            {
                SetPersonalizationPanelActive(false);
                AlignTransformY(m_ExitSelectPanel, m_ExitButton.transform);
            }
            m_ExitSelectPanel.gameObject.SetActive(value);
        }

        public void OnMainMenuClicked()
        {
            SceneTransitionManager.Instance.DisconnectAndExitToMainMenu();
        }

        public void OnDesktopClicked()
        {
            Application.Quit();
        }

        #endregion

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
            TimeManager.Instance.OnGameUnPaused();
            m_TimeMenuChangedItsState = Time.unscaledTime;
            base.gameObject.SetActive(false);

            SetPersonalizationPanelActive(false);
            SetExitPanelActive(false);

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

        #region Camera Animation

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

        #endregion
    }
}