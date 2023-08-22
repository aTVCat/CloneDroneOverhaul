using Bolt;
using CDOverhaul.Gameplay;
using CDOverhaul.Gameplay.Editors.Personalization;
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
        private static readonly List<string> s_ReportedPlayFabIDsThisSession = new List<string>();

        #region Open/Close menu

        public static OverhaulPauseMenu Instance;

        private float m_TargetFoV = 60f;
        private bool m_IsAnimatingCamera;

        public static void ToggleMenu()
        {
            if (!Instance || !Instance.AllowToggleMenu)
                return;

            if (Instance.gameObject.activeSelf)
            {
                _ = AudioManager.Instance.PlayClipGlobal(AudioLibrary.Instance.UISelectionBack, 0f, 1f, 0f);
                Instance.Hide();
                return;
            }
            _ = AudioManager.Instance.PlayClipGlobal(AudioLibrary.Instance.UISelectionPress, 0f, 1f, 0f);
            Instance.Show();
        }

        private Animator m_CameraAnimator;
        private Camera m_Camera;

        private float m_TimeMenuChangedItsState;
        public bool AllowToggleMenu => true;// Time.unscaledTime >= m_TimeMenuChangedItsState + 0.45f;

        #endregion

        private Button m_PersonalizationButton;
        private Transform m_PersonalizationPanel;
        private Button m_PersonalizationSkinsButton;
        private Button m_PersonalizationOutfitsButton;
        private Button m_PersonalizationPetsButton;
        private Button m_PersonalizationEditorButton;

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

        private GameObject m_PlayerActionsMenu;
        private GameObject m_PlayerActionsSelectPanel;
        private Text m_SelectedPlayerText;
        private Button m_OpenMutePanel;
        private Button m_OpenReportPanel;
        private Button m_CopyUserInfo;
        private Button m_PlayerActionsSelectClose;
        private GameObject m_PlayerActionsMutePanel;
        private Text m_MutePlayerText;
        private Button m_Mute;
        private Button m_CancelMute;
        private GameObject m_PlayerActionsReportPanel;
        private Text m_ReportPlayerText;
        private Dropdown m_ReportReason;
        private InputField m_ReportedPersonBehaviour;
        private Button m_CancelReport;
        private Button m_Report;

        private readonly Dictionary<string, GameObject> m_Logos = new Dictionary<string, GameObject>();

        private bool m_HasVotedUp;
        private bool m_HasVotedDown;

        public bool ScheduleHide;

        private ParametersMenu m_Parameters;
        private MultiplayerPlayerInfoState m_TargetPlayer;

        public override void Initialize()
        {
            base.Initialize();
            Instance = this;

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
            m_PersonalizationOutfitsButton.interactable = OverhaulFeatureAvailabilitySystem.ImplementedInBuild.AreNewPersonalizationCategoriesEnabled;
            m_PersonalizationPetsButton = MyModdedObject.GetObject<Button>(36);
            m_PersonalizationPetsButton.onClick.AddListener(OnPetsButtonClicked);
            m_PersonalizationPetsButton.interactable = OverhaulFeatureAvailabilitySystem.ImplementedInBuild.AreNewPersonalizationCategoriesEnabled;
            m_PersonalizationEditorButton = MyModdedObject.GetObject<Button>(58);
            m_PersonalizationEditorButton.onClick.AddListener(OnPersonalizationEditorButtonClicked);
            m_PersonalizationEditorButton.interactable = OverhaulFeatureAvailabilitySystem.ImplementedInBuild.AreNewPersonalizationCategoriesEnabled;

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
                    m_ExitSelectPanel.gameObject.SetActive(false);
                    m_PersonalizationPanel.gameObject.SetActive(false);
                    m_SettingsSelectPanel.gameObject.SetActive(false);

                    Button b = t.GetComponent<Button>();
                    if (b != null)
                        b.onClick.Invoke();
                }
            });
            _ = MyModdedObject.GetObject<Transform>(15).gameObject.AddComponent<OverhaulUIButtonScaler>();

            m_PlayerActionsMenu = MyModdedObject.GetObject<Transform>(37).gameObject;
            m_SelectedPlayerText = MyModdedObject.GetObject<Text>(50);
            m_PlayerActionsSelectPanel = MyModdedObject.GetObject<Transform>(38).gameObject;
            m_PlayerActionsSelectClose = MyModdedObject.GetObject<Button>(39);
            m_PlayerActionsSelectClose.onClick.AddListener(HidePlayerActions);
            m_OpenMutePanel = MyModdedObject.GetObject<Button>(40);
            m_OpenMutePanel.onClick.AddListener(ShowMutePanel);
            m_OpenReportPanel = MyModdedObject.GetObject<Button>(41);
            m_OpenReportPanel.onClick.AddListener(ShowReportPanel);
            m_CopyUserInfo = MyModdedObject.GetObject<Button>(42);
            m_CopyUserInfo.onClick.AddListener(CopyUserInfo);
            m_PlayerActionsMutePanel = MyModdedObject.GetObject<Transform>(43).gameObject;
            m_MutePlayerText = MyModdedObject.GetObject<Text>(51);
            m_Mute = MyModdedObject.GetObject<Button>(44);
            m_Mute.onClick.AddListener(MutePlayer);
            m_CancelMute = MyModdedObject.GetObject<Button>(45);
            m_CancelMute.onClick.AddListener(HidePlayerActions);
            m_PlayerActionsReportPanel = MyModdedObject.GetObject<Transform>(46).gameObject;
            m_ReportPlayerText = MyModdedObject.GetObject<Text>(52);
            m_CancelReport = MyModdedObject.GetObject<Button>(47);
            m_CancelReport.onClick.AddListener(HidePlayerActions);
            m_Report = MyModdedObject.GetObject<Button>(53);
            m_Report.onClick.AddListener(ReportPlayer);
            m_ReportReason = MyModdedObject.GetObject<Dropdown>(48);
            m_ReportedPersonBehaviour = MyModdedObject.GetObject<InputField>(49);
            m_ReportedPersonBehaviour.text = string.Empty;

            m_Logos.Add("ko", MyModdedObject.GetObject<Transform>(57).gameObject);
            m_Logos.Add("ja", MyModdedObject.GetObject<Transform>(56).gameObject);
            m_Logos.Add("zh-CN", MyModdedObject.GetObject<Transform>(55).gameObject);
            m_Logos.Add("zh-TW", MyModdedObject.GetObject<Transform>(55).gameObject);
            m_Logos.Add(string.Empty, MyModdedObject.GetObject<Transform>(54).gameObject);

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

            m_SettingsSelectPanel.gameObject.SetActive(false);
            m_ExitSelectPanel.gameObject.SetActive(false);
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
            AccessoriesPersonalizationPanel panel = GetController<AccessoriesPersonalizationPanel>();
            if (!panel)
                return;

            Hide();
            panel.Show();
        }

        public void OnPetsButtonClicked()
        {
            MountsPersonalizationPanel panel = GetController<MountsPersonalizationPanel>();
            if (!panel)
                return;

            Hide();
            panel.Show();
        }

        public void OnPersonalizationEditorButtonClicked()
        {
            PersonalizationEditorUI panel = GetController<PersonalizationEditorUI>();
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
            m_SettingsSelectPanel.gameObject.SetActive(false);
            m_PersonalizationPanel.gameObject.SetActive(false);
            SetPanelActive(m_ExitSelectPanel, m_ExitButton.transform, !m_ExitSelectPanel.gameObject.activeSelf);
        }

        public void OnMainMenuClicked()
        {
            SceneTransitionManager.Instance.DisconnectAndExitToMainMenu();
        }

        public void OnDesktopClicked()
        {
            OverhaulTransitionController.DoTransitionWithAction(delegate
            {
                Application.Quit();
            });
        }

        #endregion

        #region Settings

        public void OnSettingsClicked()
        {
            m_SettingsButton.OnDeselect(null);
            SetPanelActive(m_SettingsSelectPanel, m_SettingsButton.transform, !m_SettingsSelectPanel.gameObject.activeSelf);
            m_ExitSelectPanel.gameObject.SetActive(false);
            m_PersonalizationPanel.gameObject.SetActive(false);
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
            m_AdvCompletedText.text = OverhaulLocalizationManager.GetTranslation("Completed") + " " + completed + " " + OverhaulLocalizationManager.GetTranslation("Of") + " " + all;
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

        #region Player actions

        public void ShowPlayerActions(MultiplayerPlayerInfoState player)
        {
            if (!player || player.IsDetached() || string.IsNullOrEmpty(player.state.PlayFabID))
                return;

            List<string> options = GameUIRoot.Instance.EscMenu.ReportUserMenu.ReportTypeOptions;
            List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();
            for (int i = 0; i < options.Count; i++)
            {
                string translatedString = LocalizationManager.Instance.GetTranslatedString(options[i]);
                list.Add(new Dropdown.OptionData(translatedString, null));
            }

            m_TargetPlayer = player;
            m_PlayerActionsMenu.SetActive(true);
            m_PlayerActionsSelectPanel.SetActive(true);
            m_SelectedPlayerText.text = "More: " + player.state.DisplayName;
            m_PlayerActionsReportPanel.SetActive(false);
            m_ReportPlayerText.text = "Report " + player.state.DisplayName + "?";
            m_PlayerActionsMutePanel.SetActive(false);
            m_MutePlayerText.text = "Mute " + player.state.DisplayName + "?";
            m_CopyUserInfo.gameObject.SetActive(OverhaulFeatureAvailabilitySystem.IsFeatureUnlocked(OverhaulFeatureID.PermissionToCopyUserInfos));
            m_ReportedPersonBehaviour.text = string.Empty;
            m_ReportReason.options = list;
            m_ReportReason.value = 0;

            bool hasBlocked = false;
            BlockListData data = MultiplayerLoginManager.Instance.GetLocalPlayerBlockList();
            foreach (BlockListData.Entry entry in data.BlockList)
            {
                hasBlocked = entry.PlayfabId == m_TargetPlayer.state.PlayFabID;
                if (hasBlocked)
                    break;
            }
            m_OpenMutePanel.interactable = !player.IsLocalPlayer() && !hasBlocked;
            m_OpenReportPanel.interactable = !player.IsLocalPlayer() && !s_ReportedPlayFabIDsThisSession.Contains(player.state.PlayFabID);
        }

        public void ShowMutePanel()
        {
            m_PlayerActionsSelectPanel.SetActive(false);
            m_PlayerActionsReportPanel.SetActive(false);
            m_PlayerActionsMutePanel.SetActive(true);
        }

        public void ShowReportPanel()
        {
            m_PlayerActionsSelectPanel.SetActive(false);
            m_PlayerActionsReportPanel.SetActive(true);
            m_PlayerActionsMutePanel.SetActive(false);
        }

        public void HidePlayerActions()
        {
            m_TargetPlayer = null;
            m_PlayerActionsMenu.SetActive(false);
        }

        public void CopyUserInfo()
        {
            if (!m_TargetPlayer || m_TargetPlayer.IsDetached())
                return;

            string.Format("{0} - {1} ({2}, {3})", new object[]
            {
                m_TargetPlayer.state.DisplayName,
                m_TargetPlayer.state.LastBotStandingWins,
                m_TargetPlayer.state.PlayFabID,
                MultiplayerPlayerInfoStateDisplay.GetPlatformString((PlayFab.ClientModels.LoginIdentityProvider)m_TargetPlayer.state.PlatformID, false)
            }).CopyToClipboard();
            HidePlayerActions();
        }

        public void MutePlayer()
        {
            if (!m_TargetPlayer || m_TargetPlayer.IsDetached() || m_TargetPlayer.IsLocalPlayer())
                return;

            BlockListData blockListData = MultiplayerLoginManager.Instance.GetLocalPlayerBlockList();
            if (blockListData == null)
                return;

            BlockListData.Entry entry = new BlockListData.Entry
            {
                DisplayName = m_TargetPlayer.state.DisplayName,
                PlatformId = m_TargetPlayer.state.PlatformID,
                PlayfabId = m_TargetPlayer.state.PlayFabID
            };
            blockListData.BlockList.Add(entry);
            MultiplayerLoginManager.Instance.UpdateLocalPlayerBlockList(true, 1);
            HidePlayerActions();
        }

        public void ReportPlayer()
        {
            if (!m_TargetPlayer || m_TargetPlayer.IsDetached() || m_TargetPlayer.IsLocalPlayer() || s_ReportedPlayFabIDsThisSession.Contains(m_TargetPlayer.state.PlayFabID))
                return;

            MultiplayerPlayerInfoState localPlayerInfoState = MultiplayerPlayerInfoManager.Instance.GetLocalPlayerInfoState();
            string reportId = GameUIRoot.Instance.EscMenu.ReportUserMenu.ReportTypeOptions[m_ReportReason.value];
            string description = m_ReportedPersonBehaviour.text;
            string reportedPlayerPlayfabId = m_TargetPlayer.state.PlayFabID;
            string reportedPlayerDisplayName = m_TargetPlayer.state.DisplayName;
            string senderPlayfabId = localPlayerInfoState.state.PlayFabID;
            localPlayerInfoState.GetOrPrepareSafeDisplayName(delegate (string displayName)
            {
                ReportManager.SendUserReport(reportId, description, senderPlayfabId, displayName, reportedPlayerPlayfabId, reportedPlayerDisplayName);
            });
            HidePlayerActions();
            s_ReportedPlayFabIDsThisSession.Add(reportedPlayerPlayfabId);
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
            //animateCamera();

            TimeManager.Instance.OnGamePaused();

            RefreshAdvancements();
            RefreshRoomCodeField();
            RefreshRoomCodePanelActive();
            RefreshStartMatchButton();
            RefreshPlayersInMatch();
            RefreshCurrentWorkshopLevel();
            HidePlayerActions();

            string langCode = SettingsManager.Instance.GetCurrentLanguageID();
            foreach (GameObject value in m_Logos.Values)
                value.SetActive(false);

            foreach (string key in m_Logos.Keys)
                if (key == langCode)
                    m_Logos[key].SetActive(true);

            m_Logos[string.Empty].SetActive(!m_Logos.ContainsKey(langCode));

            m_SkipLevelButton.gameObject.SetActive(GameModeManager.CanSkipCurrentLevel());
            m_BackToLVLEditorButton.gameObject.SetActive(GameModeManager.IsLevelPlaytest());
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

            if (!m_IsAnimatingCamera && m_CameraAnimator)
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
            if (AllowToggleMenu && Input.GetKeyDown(KeyCode.Alpha0) && !m_PlayerActionsMenu.activeSelf)
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