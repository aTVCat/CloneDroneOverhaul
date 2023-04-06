using Bolt;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class OverhaulPauseMenu : OverhaulUI
    {
        [OverhaulSetting("Game interface.Gameplay.New pause menu design", true, false, "The full redesign with new features implemented")]
        public static bool UseThisMenu;

        [OverhaulSetting("Game interface.Gameplay.Zoom camera", true, false, "If camera zoom breaks gameplay, disable this setting", null, null, "Game interface.Gameplay.New pause menu design")]
        public static bool UseZoom;

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
                _ = AudioManager.Instance.PlayClipGlobal(AudioLibrary.Instance.UISelectionBack, 0f, 1f, 0f);
                m_Instance.Hide();
                return;
            }
            _ = AudioManager.Instance.PlayClipGlobal(AudioLibrary.Instance.UISelectionPress, 0f, 1f, 0f);
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
        private Button m_PersonalizationOutfitsButton;

        private Button m_ExitButton;
        private Transform m_ExitSelectPanel;
        private Button m_ExitSelectToMainMenuButton;
        private Button m_ExitSelectToDesktopButton;

        private Button m_AdvancementsButton;
        private Text m_AdvCompletedText;
        private Image m_AdvFillImage;

        private Button m_SettingsButton;
        private Transform m_SettingsSelectPanel;
        private Button m_GameSettingsButton;
        private Button m_ModSettingsButton;

        private Transform m_RoomCodePanel;
        private InputField m_RoomCodeField;
        private Button m_RoomCodeCopyButton;
        private Button m_RoomCodeRevealButton;
        private Button m_StartMatchButton;
        private Text m_StartMatchButtonText;

        public bool ScheduleHide;

        private OverhaulParametersMenu m_Parameters;

        public override void Initialize()
        {
            m_Instance = this;

            m_PersonalizationButton = MyModdedObject.GetObject<Button>(0);
            m_PersonalizationButton.onClick.AddListener(OnPersonalizationButtonClicked);
            m_PersonalizationPanel = MyModdedObject.GetObject<Transform>(1);
            m_PersonalizationSkinsButton = MyModdedObject.GetObject<Button>(2);
            m_PersonalizationSkinsButton.onClick.AddListener(OnSkinsButtonClicked);
            m_PersonalizationOutfitsButton = MyModdedObject.GetObject<Button>(3);
            m_PersonalizationOutfitsButton.onClick.AddListener(OnOutfitsButtonClicked);

            m_ExitButton = MyModdedObject.GetObject<Button>(4);
            m_ExitButton.onClick.AddListener(OnExitClicked);
            m_ExitSelectPanel = MyModdedObject.GetObject<Transform>(5);
            m_ExitSelectToMainMenuButton = MyModdedObject.GetObject<Button>(6);
            m_ExitSelectToMainMenuButton.onClick.AddListener(OnMainMenuClicked);
            m_ExitSelectToDesktopButton = MyModdedObject.GetObject<Button>(7);
            m_ExitSelectToDesktopButton.onClick.AddListener(OnDesktopClicked);

            m_AdvancementsButton = MyModdedObject.GetObject<Button>(8);
            m_AdvancementsButton.onClick.AddListener(OnAdvClicked);
            m_AdvFillImage = MyModdedObject.GetObject<Image>(9);
            m_AdvCompletedText = MyModdedObject.GetObject<Text>(10);

            m_SettingsButton = MyModdedObject.GetObject<Button>(11);
            m_SettingsButton.onClick.AddListener(OnSettingsClicked);
            m_SettingsSelectPanel = MyModdedObject.GetObject<Transform>(12);
            m_GameSettingsButton = MyModdedObject.GetObject<Button>(13);
            m_GameSettingsButton.onClick.AddListener(OnGameSettingsClicked);
            m_ModSettingsButton = MyModdedObject.GetObject<Button>(14);
            m_ModSettingsButton.onClick.AddListener(OnModSettingsClicked);

            m_RoomCodePanel = MyModdedObject.GetObject<Transform>(17);
            m_RoomCodeField = MyModdedObject.GetObject<InputField>(20);
            m_RoomCodeCopyButton = MyModdedObject.GetObject<Button>(19);
            m_RoomCodeCopyButton.onClick.AddListener(OverhaulGamemodeManager.CopyPrivateRoomCode);
            m_RoomCodeRevealButton = MyModdedObject.GetObject<Button>(18);
            m_RoomCodeRevealButton.onClick.AddListener(OnRoomCodeRevealButtonClicked);

            m_StartMatchButton = MyModdedObject.GetObject<Button>(21);
            m_StartMatchButton.onClick.AddListener(OnStartMatchClicked);
            m_StartMatchButtonText = MyModdedObject.GetObject<Text>(22);

            MyModdedObject.GetObject<Button>(16).onClick.AddListener(OnContinueClicked);
            MyModdedObject.GetObject<Button>(15).onClick.AddListener(delegate
            {
                Transform t = TransformUtils.FindChildRecursive(GameUIRoot.Instance.EscMenu.transform, "SettingsButton(Clone)");
                if (t != null)
                {
                    Button b = t.GetComponent<Button>();
                    if (b != null)
                    {
                        b.onClick.Invoke();
                    }
                }
            });

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

        public void SetPanelActive(Transform t, Transform caller, bool value)
        {
            if (value)
            {
                AlignTransformY(t, caller.transform);
            }
            t.gameObject.SetActive(value);
        }

        #region Personalization

        public void OnPersonalizationButtonClicked()
        {
            m_PersonalizationSkinsButton.interactable = OverhaulGamemodeManager.SupportsPersonalization();
            m_PersonalizationOutfitsButton.interactable = OverhaulGamemodeManager.SupportsOutfits();
            SetPanelActive(m_PersonalizationPanel, m_PersonalizationButton.transform, !m_PersonalizationPanel.gameObject.activeSelf);
        }

        public void OnSkinsButtonClicked()
        {
            WeaponSkinsMenu menu = WeaponSkinsMenu.SkinsSelection;
            if (menu == null)
            {
                return;
            }

            Hide();
            menu.SetMenuActive(true);
        }

        public void OnOutfitsButtonClicked()
        {
            WeaponSkinsMenu menu = WeaponSkinsMenu.OutfitSelection;
            if (menu == null)
            {
                return;
            }

            Hide();
            menu.SetMenuActive(true);
        }

        #endregion

        #region Exit

        public void OnExitClicked()
        {
            SetPanelActive(m_ExitSelectPanel, m_ExitButton.transform, !m_ExitSelectPanel.gameObject.activeSelf);
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

        #region Settings

        public void OnSettingsClicked()
        {
            SetPanelActive(m_SettingsSelectPanel, m_SettingsButton.transform, !m_SettingsSelectPanel.gameObject.activeSelf);
        }

        public void OnGameSettingsClicked()
        {
            SetPanelActive(m_SettingsSelectPanel, null, false);
            GameUIRoot.Instance.SettingsMenu.Show();
            _ = StaticCoroutineRunner.StartStaticCoroutine(settingsCoroutine());
            HideMenu(true);
        }

        private IEnumerator settingsCoroutine()
        {
            yield return new WaitUntil(() => !GameUIRoot.Instance.SettingsMenu.gameObject.activeSelf);
            Show();
            yield break;
        }

        public void OnModSettingsClicked()
        {
            if (m_Parameters == null)
            {
                m_Parameters = GetController<OverhaulParametersMenu>();
                if (m_Parameters == null || m_Parameters.IsDisposedOrDestroyed() || m_Parameters.HadBadStart)
                {
                    return;
                }
            }

            SetPanelActive(m_SettingsSelectPanel, null, false);
            HideMenu(true);
            m_Parameters.Show();
            _ = StaticCoroutineRunner.StartStaticCoroutine(modSettingsCoroutine());
        }

        private IEnumerator modSettingsCoroutine()
        {
            yield return new WaitUntil(() => !m_Parameters.gameObject.activeSelf);
            if (ScheduleHide)
            {
                ScheduleHide = false;
                yield break;
            }
            Show();
            yield break;
        }

        #endregion

        #region Advancements

        public void RefreshAdvancements()
        {
            int completed = 0;
            GameplayAchievementManager manager = GameplayAchievementManager.Instance;
            int all = manager.Achievements.Length;
            int i = 0;
            do
            {
                completed += manager.Achievements[i].IsComplete() ? 1 : 0;
                i++;

            } while (i < all);

            m_AdvFillImage.fillAmount = completed / all;
            m_AdvCompletedText.text = "Completed:  " + completed + " of " + all;
        }

        public void OnAdvClicked()
        {
            HideMenu(true);
            GameUIRoot.Instance.AchievementProgressUI.Show();
            _ = StaticCoroutineRunner.StartStaticCoroutine(advCoroutine());
        }

        private IEnumerator advCoroutine()
        {
            yield return new WaitUntil(() => !GameUIRoot.Instance.AchievementProgressUI.gameObject.activeSelf);
            Show();
            yield break;
        }

        #endregion

        #region Room code

        public void RefreshRoomCodePanelActive()
        {
            m_RoomCodePanel.gameObject.SetActive(OverhaulGamemodeManager.ShouldShowRoomCodePanel());
        }

        public void RefreshRoomCodeField()
        {
            m_RoomCodeField.text = OverhaulGamemodeManager.GetPrivateRoomCode();
            m_RoomCodeRevealButton.gameObject.SetActive(true);
        }

        public void RefreshStartMatchButton()
        {
            m_StartMatchButton.gameObject.SetActive(false);
            bool shouldShow = OverhaulGamemodeManager.ShouldShowRoomCodePanel();
            if (!shouldShow)
            {
                return;
            }

            if (BattleRoyaleManager.Instance != null)
            {
                if (BattleRoyaleManager.Instance.IsProgress(BattleRoyaleMatchProgress.InWaitingArea))
                {
                    m_StartMatchButtonText.text = LocalizationManager.Instance.GetTranslatedString("Start Match!", -1);
                    m_StartMatchButton.gameObject.SetActive(true);
                }
                else if (BattleRoyaleManager.Instance.IsProgress(BattleRoyaleMatchProgress.FightingStarted))
                {
                    m_StartMatchButtonText.text = LocalizationManager.Instance.GetTranslatedString("Final Zone!", -1);
                    m_StartMatchButton.gameObject.SetActive(true);
                }
            }
            else if (ArenaCoopManager.Instance != null && !ArenaCoopManager.Instance.IsMatchStarted())
            {
                m_StartMatchButtonText.text = LocalizationManager.Instance.GetTranslatedString("Start Match!", -1);
                m_StartMatchButton.gameObject.SetActive(true);
            }

        }

        public void OnRoomCodeRevealButtonClicked()
        {
            m_RoomCodeRevealButton.gameObject.SetActive(false);
        }

        public void OnStartMatchClicked()
        {
            Hide();

            if (BattleRoyaleManager.Instance != null)
            {
                if (BattleRoyaleManager.Instance.IsProgress(BattleRoyaleMatchProgress.InWaitingArea))
                {
                    ClientRequestsStartingLevelNowEvent.Create(GlobalTargets.OnlyServer, ReliabilityModes.ReliableOrdered).Send();
                }
                else if (BattleRoyaleManager.Instance.IsProgress(BattleRoyaleMatchProgress.FightingStarted))
                {
                    ClientRequestsFinalZoneActivation.Create(GlobalTargets.OnlyServer, ReliabilityModes.ReliableOrdered).Send();
                }
            }
            else if (ArenaCoopManager.Instance != null && !ArenaCoopManager.Instance.IsMatchStarted())
            {
                ClientRequestsStartingLevelNowEvent.Create(GlobalTargets.OnlyServer, ReliabilityModes.ReliableOrdered).Send();
            }
        }

        #endregion

        public void OnContinueClicked()
        {
            if (!AllowToggleMenu)
            {
                return;
            }

            Hide();
        }

        public void Show()
        {
            m_TimeMenuChangedItsState = Time.unscaledTime;
            base.gameObject.SetActive(true);
            animateCamera();

            TimeManager.Instance.OnGamePaused();

            RefreshAdvancements();
            RefreshRoomCodeField();
            RefreshRoomCodePanelActive();
            RefreshStartMatchButton();

            m_PersonalizationButton.interactable = !GameModeManager.IsInLevelEditor();

            ShowCursor = true;
        }

        public void HideMenu(bool dontUnpause = false)
        {
            if (!dontUnpause) TimeManager.Instance.OnGameUnPaused();
            m_TimeMenuChangedItsState = Time.unscaledTime;
            base.gameObject.SetActive(false);

            SetPanelActive(m_PersonalizationPanel, null, false);
            SetPanelActive(m_ExitSelectPanel, null, false);
            SetPanelActive(m_SettingsSelectPanel, null, false);

            if (!m_IsAnimatingCamera && m_CameraAnimator != null)
            {
                _ = StaticCoroutineRunner.StartStaticCoroutine(animateCameraCoroutine(m_Camera, m_CameraAnimator, true));
            }

            if (!dontUnpause) ShowCursor = false;
        }

        public void Hide()
        {
            HideMenu(false);
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
            if (player == null)
            {
                return;
            }

            m_Camera = player.GetPlayerCamera();
            if (m_Camera == null)
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

            if (!UseZoom)
            {
                return;
            }
            _ = StaticCoroutineRunner.StartStaticCoroutine(animateCameraCoroutine(m_Camera, m_CameraAnimator, false));
        }

        private IEnumerator animateCameraCoroutine(Camera camera, Animator animator, bool animatorState)
        {
            if (!UseZoom)
            {
                if (camera != null) camera.fieldOfView = m_TargetFoV;
                if (animator != null) animator.enabled = true;
                yield break;
            }

            m_IsAnimatingCamera = true;
            int iterations = 20;
            if (!animatorState)
            {
                if (animator != null) animator.enabled = false;
                while (iterations > -1)
                {
                    iterations--;
                    if (camera != null) camera.fieldOfView += (40 - camera.fieldOfView) * 0.5f * (Time.unscaledDeltaTime * 22);
                    yield return null;
                }
            }
            else
            {
                while (iterations > -1)
                {
                    iterations--;
                    if (camera != null) camera.fieldOfView += (m_TargetFoV - camera.fieldOfView) * 0.5f * (Time.unscaledDeltaTime * 22);
                    yield return null;
                }
                if (animator != null) animator.enabled = true;
            }
            m_IsAnimatingCamera = false;
            yield break;
        }

        #endregion
    }
}