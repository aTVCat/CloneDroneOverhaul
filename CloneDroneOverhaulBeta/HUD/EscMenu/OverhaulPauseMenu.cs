using Bolt;
using CDOverhaul.Gameplay;
using CDOverhaul.Workshop;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class OverhaulPauseMenu : OverhaulUI
    {
        [OverhaulSetting("Game interface.Gameplay.New pause menu design", true, false, "The full redesign with new features implemented")]
        public static bool UseThisMenu;

        [OverhaulSetting("Game interface.Gameplay.Zoom camera", true, false, "If camera zoom breaks gameplay, disable this setting", "Game interface.Gameplay.New pause menu design")]
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
        private Button m_PersonalizationPetsButton;

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

        private Button m_BackToLVLEditorButton;
        private Button m_SkipLevelButton;

        private Transform m_PersonalizationNotification;

        private Transform m_PlayersInMatchPanel;
        private PrefabAndContainer m_PlayersInMatch;

        private Transform m_CurrentWorkshopLevel;
        private Text m_WorkshopLevelTitleText;
        private Text m_WorkshopLevelCreatorText;
        private Text m_WorkshopLevelDescriptionText;
        private Button m_WorkshopLevelUpVote;
        private Button m_WorkshopLevelDownVote;
        private Button m_WorkshopLevelDetails;

        private bool m_HasVotedUp;
        private bool m_HasVotedDown;

        public bool ScheduleHide;

        private ParametersMenu m_Parameters;

        public override void Initialize()
        {
            base.Initialize();
            m_Instance = this;

            m_CurrentWorkshopLevel = MyModdedObject.GetObject<Transform>(29);
            m_WorkshopLevelTitleText = MyModdedObject.GetObject<Text>(30);
            m_WorkshopLevelCreatorText = MyModdedObject.GetObject<Text>(31);
            m_WorkshopLevelDescriptionText = MyModdedObject.GetObject<Text>(32);
            m_WorkshopLevelUpVote = MyModdedObject.GetObject<Button>(33);
            m_WorkshopLevelUpVote.onClick.AddListener(ToggleVoteUp);
            m_WorkshopLevelDownVote = MyModdedObject.GetObject<Button>(34);
            m_WorkshopLevelDownVote.onClick.AddListener(ToggleVoteDown);
            m_WorkshopLevelDetails = MyModdedObject.GetObject<Button>(35);

            m_PlayersInMatchPanel = MyModdedObject.GetObject<Transform>(26);
            m_PlayersInMatch = new PrefabAndContainer(MyModdedObject, 27, 28);

            m_PersonalizationButton = MyModdedObject.GetObject<Button>(0);
            m_PersonalizationButton.onClick.AddListener(OnPersonalizationButtonClicked);
            _ = m_PersonalizationButton.gameObject.AddComponent<OverhaulUIButtonScaler>();
            m_PersonalizationPanel = MyModdedObject.GetObject<Transform>(1);
            m_PersonalizationSkinsButton = MyModdedObject.GetObject<Button>(2);
            m_PersonalizationSkinsButton.onClick.AddListener(OnSkinsButtonClicked);
            m_PersonalizationOutfitsButton = MyModdedObject.GetObject<Button>(3);
            m_PersonalizationOutfitsButton.onClick.AddListener(OnOutfitsButtonClicked);
            m_PersonalizationPetsButton = MyModdedObject.GetObject<Button>(36);
            m_PersonalizationPetsButton.interactable = !OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IsPetsDemo;

            m_ExitButton = MyModdedObject.GetObject<Button>(4);
            m_ExitButton.onClick.AddListener(OnExitClicked);
            _ = m_ExitButton.gameObject.AddComponent<OverhaulUIButtonScaler>();
            m_ExitSelectPanel = MyModdedObject.GetObject<Transform>(5);
            m_ExitSelectToMainMenuButton = MyModdedObject.GetObject<Button>(6);
            m_ExitSelectToMainMenuButton.onClick.AddListener(OnMainMenuClicked);
            m_ExitSelectToDesktopButton = MyModdedObject.GetObject<Button>(7);
            m_ExitSelectToDesktopButton.onClick.AddListener(OnDesktopClicked);

            m_AdvancementsButton = MyModdedObject.GetObject<Button>(8);
            m_AdvancementsButton.onClick.AddListener(OnAdvClicked);
            _ = m_AdvancementsButton.gameObject.AddComponent<OverhaulUIButtonScaler>();
            m_AdvFillImage = MyModdedObject.GetObject<Image>(9);
            m_AdvCompletedText = MyModdedObject.GetObject<Text>(10);

            m_SettingsButton = MyModdedObject.GetObject<Button>(11);
            m_SettingsButton.onClick.AddListener(OnSettingsClicked);
            _ = m_SettingsButton.gameObject.AddComponent<OverhaulUIButtonScaler>();
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

            m_BackToLVLEditorButton = MyModdedObject.GetObject<Button>(24);
            m_BackToLVLEditorButton.onClick.AddListener(GameUIRoot.Instance.EscMenu.OnBackToLevelEditorButtonClicked);
            _ = m_BackToLVLEditorButton.gameObject.AddComponent<OverhaulUIButtonScaler>();
            m_SkipLevelButton = MyModdedObject.GetObject<Button>(25);
            m_SkipLevelButton.onClick.AddListener(delegate
            {
                GameUIRoot.Instance.EscMenu.OnSkipWorkshopLevelClicked();
                Hide();
            });
            _ = m_SkipLevelButton.gameObject.AddComponent<OverhaulUIButtonScaler>();

            m_PersonalizationNotification = MyModdedObject.GetObject<Transform>(23);

            MyModdedObject.GetObject<Button>(16).onClick.AddListener(OnContinueClicked);
            _ = MyModdedObject.GetObject<Transform>(16).gameObject.AddComponent<OverhaulUIButtonScaler>();
            MyModdedObject.GetObject<Button>(15).onClick.AddListener(delegate
            {
                Transform t = TransformUtils.FindChildRecursive(GameUIRoot.Instance.EscMenu.transform, "SettingsButton(Clone)");
                if (t != null)
                {
                    Button b = t.GetComponent<Button>();
                    if (b != null)
                        b.onClick.Invoke();
                }
            });
            _ = MyModdedObject.GetObject<Transform>(15).gameObject.AddComponent<OverhaulUIButtonScaler>();

            Hide();
        }

        public override void OnModDeactivated()
        {
            try
            {
                if (GameUIRoot.Instance != null && GameUIRoot.Instance.EscMenu != null && base.gameObject.activeSelf && UseThisMenu)
                {
                    Hide();
                    GameUIRoot.Instance.EscMenu.Show();
                    GameUIRoot.Instance.EscMenu.gameObject.SetActive(true);
                    GameUIRoot.Instance.RefreshCursorEnabled();
                    TimeManager.Instance.OnGamePaused();
                }
            }
            catch
            {
                return; // not really a cool way to resolve nullreferenceexc
            }
        }

        public void AlignTransformY(Transform targetTransform, Transform transformToUse)
        {
            targetTransform.position = new Vector3(targetTransform.position.x, transformToUse.position.y, targetTransform.position.z);
        }

        public void SetPanelActive(Transform t, Transform caller, bool value)
        {
            if (value)
                AlignTransformY(t, caller.transform);

            t.gameObject.SetActive(value);
        }

        #region Personalization

        public void OnPersonalizationButtonClicked()
        {
            if (!WeaponSkinsController.HasNoticedSkinsButton)
            {
                WeaponSkinsController.HasNoticedSkinsButton = true;
                SettingInfo.SavePref(OverhaulSettingsController.GetSetting("Player.WeaponSkins.NoticedSkinsButton", true), true);
                m_PersonalizationNotification.gameObject.SetActive(false);
            }

            m_PersonalizationButton.OnDeselect(null);
            m_PersonalizationSkinsButton.interactable = OverhaulGamemodeManager.SupportsPersonalization();
            m_PersonalizationOutfitsButton.interactable = OverhaulGamemodeManager.SupportsOutfits();
            SetPanelActive(m_PersonalizationPanel, m_PersonalizationButton.transform, !m_PersonalizationPanel.gameObject.activeSelf);
        }

        public void OnSkinsButtonClicked()
        {
            PersonalizationMenu menu = PersonalizationMenu.SkinsSelection;
            if (menu == null)
            {
                return;
            }

            Hide();
            menu.SetMenuActive(true);
        }

        public void OnOutfitsButtonClicked()
        {
            OverhaulPersonalizationPanel panel = OverhaulPersonalizationPanel.GetPanel(PersonalizationCategory.Outfits);
            if (!panel)
                return;

            Hide();
            panel.Show();
        }

        #endregion

        #region Exit

        public void OnExitClicked()
        {
            m_ExitButton.OnDeselect(null);
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
            m_SettingsButton.OnDeselect(null);
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
                m_Parameters = GetController<ParametersMenu>();
                if (m_Parameters == null || m_Parameters.IsDisposedOrDestroyed() || m_Parameters.Error)
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
            m_AdvCompletedText.text = OverhaulLocalizationController.GetTranslation("Completed") + " " + completed + " " + OverhaulLocalizationController.GetTranslation("Of") + " " + all;
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
            m_StartMatchButton.interactable = false;
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
                    m_StartMatchButton.interactable = true;
                }
                else if (BattleRoyaleManager.Instance.IsProgress(BattleRoyaleMatchProgress.FightingStarted))
                {
                    m_StartMatchButtonText.text = LocalizationManager.Instance.GetTranslatedString("Final Zone!", -1);
                    m_StartMatchButton.interactable = true;
                }
            }
            else if (ArenaCoopManager.Instance != null && !ArenaCoopManager.Instance.IsMatchStarted())
            {
                m_StartMatchButtonText.text = LocalizationManager.Instance.GetTranslatedString("Start Match!", -1);
                m_StartMatchButton.interactable = true;
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

        #region Players in match

        public void RefreshPlayersInMatch()
        {
            m_PlayersInMatchPanel.gameObject.SetActive(false);
            m_PlayersInMatch.ClearContainer();

            MultiplayerPlayerInfoManager manager = MultiplayerPlayerInfoManager.Instance;
            if (!GameModeManager.IsMultiplayer() || GameModeManager.IsMultiplayerDuel() || !manager)
                return;

            List<MultiplayerPlayerInfoState> allMultiplayerPlayerInfos = manager.GetAllPlayerInfoStates();
            if (allMultiplayerPlayerInfos.IsNullOrEmpty())
                return;

            m_PlayersInMatchPanel.gameObject.SetActive(true);
            int index = 0;
            do
            {
                MultiplayerPlayerInfoState multiplayerPlayerInfo = allMultiplayerPlayerInfos[index];
                if (!multiplayerPlayerInfo || multiplayerPlayerInfo.IsDetached())
                {
                    index++;
                    continue;
                }

                ModdedObject entry = m_PlayersInMatch.CreateNew();
                MultiplayerPlayerInfoStateDisplay display = entry.gameObject.AddComponent<MultiplayerPlayerInfoStateDisplay>();
                display.Initialize(multiplayerPlayerInfo, entry);

                index++;
            } while (index < allMultiplayerPlayerInfos.Count);
        }

        #endregion

        #region Workshop level

        public void RefreshCurrentWorkshopLevel()
        {
            m_CurrentWorkshopLevel.gameObject.SetActive(false);

            SteamWorkshopItem item = NowPlayingWorkshopManager.Instance.GetCurrentItem();
            if (item == null)
                return;

            m_CurrentWorkshopLevel.gameObject.SetActive(true);
            m_WorkshopLevelTitleText.text = item.Title;
            m_WorkshopLevelCreatorText.text = item.CreatorName;
            m_WorkshopLevelDescriptionText.text = item.Description;
            RefreshVote();
        }

        public void RefreshVote()
        {
            SteamWorkshopItem item = NowPlayingWorkshopManager.Instance.GetCurrentItem();
            if (item == null)
                return;

            m_WorkshopLevelUpVote.interactable = true;
            m_WorkshopLevelDownVote.interactable = true;
            m_HasVotedUp = true;
            m_HasVotedDown = true;

            OverhaulSteamBrowser.GetItemVoteInfo(item.WorkshopItemID, delegate (bool skip, bool up, bool down, bool fail)
            {
                if (fail)
                {
                    m_WorkshopLevelUpVote.interactable = false;
                    m_WorkshopLevelDownVote.interactable = false;
                    return;
                }

                m_WorkshopLevelUpVote.interactable = !up;
                m_WorkshopLevelDownVote.interactable = !down;

                m_HasVotedUp = up;
                m_HasVotedDown = down;
            });
        }

        public void ToggleVoteUp()
        {
            SteamWorkshopItem item = NowPlayingWorkshopManager.Instance.GetCurrentItem();
            if (item == null || m_HasVotedUp)
                return;

            OverhaulSteamBrowser.SetItemVote(item.WorkshopItemID, true, delegate (SetUserItemVoteResult_t res, bool up)
            {
                m_WorkshopLevelUpVote.interactable = false;
                m_WorkshopLevelDownVote.interactable = true;
                m_HasVotedUp = true;
                m_HasVotedDown = false;
            });
        }

        public void ToggleVoteDown()
        {
            SteamWorkshopItem item = NowPlayingWorkshopManager.Instance.GetCurrentItem();
            if (item == null || m_HasVotedDown)
                return;

            OverhaulSteamBrowser.SetItemVote(item.WorkshopItemID, false, delegate (SetUserItemVoteResult_t res, bool up)
            {
                m_WorkshopLevelUpVote.interactable = true;
                m_WorkshopLevelDownVote.interactable = false;
                m_HasVotedUp = false;
                m_HasVotedDown = true;
            });
        }

        #endregion

        public void OnContinueClicked()
        {
            if (!AllowToggleMenu)
                return;

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
            RefreshPlayersInMatch();
            RefreshCurrentWorkshopLevel();

            m_SkipLevelButton.gameObject.SetActive(GameModeManager.CanSkipCurrentLevel());
            m_BackToLVLEditorButton.gameObject.SetActive((WorkshopLevelManager.Instance != null && WorkshopLevelManager.Instance.IsPlaytestActive()) || GameModeManager.IsLevelPlaytest());
            m_PersonalizationNotification.gameObject.SetActive(!WeaponSkinsController.HasNoticedSkinsButton && !GameModeManager.IsInLevelEditor());
            m_PersonalizationButton.interactable = !GameModeManager.IsInLevelEditor();

            OverhaulCanvasController.SetCanvasPixelPerfect(false);
            ShowCursor = true;
        }

        public void HideMenu(bool dontUnpause = false)
        {
            if (!dontUnpause)
                TimeManager.Instance.OnGameUnPaused();

            m_TimeMenuChangedItsState = Time.unscaledTime;
            base.gameObject.SetActive(false);

            SetPanelActive(m_PersonalizationPanel, null, false);
            SetPanelActive(m_ExitSelectPanel, null, false);
            SetPanelActive(m_SettingsSelectPanel, null, false);

            if (!m_IsAnimatingCamera && m_CameraAnimator != null)
                _ = StaticCoroutineRunner.StartStaticCoroutine(animateCameraCoroutine(m_Camera, m_CameraAnimator, true));

            if (!dontUnpause)
            {
                ShowCursor = false;
                OverhaulCanvasController.SetCanvasPixelPerfect(true);
            }
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