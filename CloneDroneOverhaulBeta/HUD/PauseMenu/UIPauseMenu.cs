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
    public class UIPauseMenu : UIController
    {
        [OverhaulSettingAttribute_Old("Game interface.Gameplay.New pause menu design", true, false, "The full redesign with new features implemented")]
        public static bool UseThisMenu;

        [OverhaulSettingAttribute_Old("Game interface.Gameplay.Zoom camera", true, false, "If camera zoom breaks gameplay, disable this setting", "Game interface.Gameplay.New pause menu design")]
        public static bool UseZoom;

        public static bool ForceUseOldMenu;
        private static readonly List<string> s_ReportedPlayFabIDsThisSession = new List<string>();

        [UIElementComponents(new System.Type[] { typeof(UIComponentButtonScaler) })]
        [UIElementActionReference(nameof(OnPersonalizationButtonClicked))]
        [UIElementReference(0)]
        private readonly Button m_PersonalizationButton;
        [UIElementDefaultVisibilityState(false)]
        [UIElementReference(1)]
        private readonly Transform m_PersonalizationPanel;
        [UIElementActionReference(nameof(OnSkinsButtonClicked))]
        [UIElementReference(2)]
        private readonly Button m_PersonalizationSkinsButton;
        [UIElementActionReference(nameof(OnOutfitsButtonClicked))]
        [UIElementReference(3)]
        private readonly Button m_PersonalizationOutfitsButton;
        [UIElementActionReference(nameof(OnPetsButtonClicked))]
        [UIElementReference(36)]
        private readonly Button m_PersonalizationPetsButton;
        [UIElementActionReference(nameof(OnPersonalizationEditorButtonClicked))]
        [UIElementReference(58)]
        private readonly Button m_PersonalizationEditorButton;
        [UIElementReference(24)]
        private readonly GameObject m_PersonalizationNotification;

        [UIElementComponents(new System.Type[] { typeof(UIComponentButtonScaler) })]
        [UIElementActionReference(nameof(OnExitButtonClicked))]
        [UIElementReference(4)]
        private readonly Button m_ExitButton;
        [UIElementDefaultVisibilityState(false)]
        [UIElementReference(5)]
        private readonly Transform m_ExitSelectPanel;
        [UIElementActionReference(nameof(OnMainMenuButtonClicked))]
        [UIElementReference(6)]
        private readonly Button m_ExitSelectToMainMenuButton;
        [UIElementActionReference(nameof(OnDesktopButtonClicked))]
        [UIElementReference(7)]
        private readonly Button m_ExitSelectToDesktopButton;

        [UIElementComponents(new System.Type[] { typeof(UIComponentButtonScaler) })]
        [UIElementActionReference(nameof(OnAdvancementsButtonClicked))]
        [UIElementReference(8)]
        private readonly Button m_AdvancementsButton;
        [UIElementReference(10)]
        private readonly Text m_AdvCompletedText;
        [UIElementReference(9)]
        private readonly Image m_AdvFillImage;

        [UIElementComponents(new System.Type[] { typeof(UIComponentButtonScaler) })]
        [UIElementActionReference(nameof(OnSettingsButtonClicked))]
        [UIElementReference(11)]
        private readonly Button m_SettingsButton;
        [UIElementDefaultVisibilityState(false)]
        [UIElementReference(12)]
        private readonly Transform m_SettingsSelectPanel;
        [UIElementActionReference(nameof(OnGameSettingsButtonClicked))]
        [UIElementReference(13)]
        private readonly Button m_GameSettingsButton;
        [UIElementActionReference(nameof(OnModSettingsButtonClicked))]
        [UIElementReference(14)]
        private readonly Button m_ModSettingsButton;

        [UIElementReference(17)]
        private readonly GameObject m_RoomCodePanel;
        [UIElementReference(20)]
        private readonly InputField m_RoomCodeField;
        [UIElementActionReference(nameof(OnCopyRoomCodeButtonClicked))]
        [UIElementReference(19)]
        private readonly Button m_RoomCodeCopyButton;
        [UIElementActionReference(nameof(OnRoomCodeRevealButtonClicked))]
        [UIElementReference(18)]
        private readonly Button m_RoomCodeRevealButton;
        [UIElementActionReference(nameof(OnStartMatchButtonClicked))]
        [UIElementReference(21)]
        private readonly Button m_StartMatchButton;
        [UIElementReference(22)]
        private readonly Text m_StartMatchButtonText;

        [UIElementComponents(new System.Type[] { typeof(UIComponentButtonScaler) })]
        [UIElementActionReference(nameof(OnReturnToLevelEditorButtonClicked))]
        [UIElementReference(24)]
        private readonly Button m_BackToLVLEditorButton;
        [UIElementComponents(new System.Type[] { typeof(UIComponentButtonScaler) })]
        [UIElementActionReference(nameof(OnSkipLevelButtonClicked))]
        [UIElementReference(25)]
        private readonly Button m_SkipLevelButton;

        [UIElementReference(26)]
        private readonly GameObject m_PlayersInMatchPanel;
        [PrefabContainer(27, 28)]
        private readonly PrefabContainer m_PlayersInMatch;

        [UIElementReference(29)]
        private readonly GameObject m_CurrentWorkshopLevel;
        [UIElementReference(30)]
        private readonly Text m_WorkshopLevelTitleText;
        [UIElementReference(31)]
        private readonly Text m_WorkshopLevelCreatorText;
        [UIElementReference(32)]
        private readonly Text m_WorkshopLevelDescriptionText;
        [UIElementActionReference(nameof(OnVoteUpButtonClicked))]
        [UIElementReference(33)]
        private readonly Button m_WorkshopLevelUpVote;
        [UIElementActionReference(nameof(OnVoteDownButtonClicked))]
        [UIElementReference(34)]
        private readonly Button m_WorkshopLevelDownVote;

        [UIElementReference(37)]
        private readonly GameObject m_PlayerActionsMenu;
        [UIElementReference(38)]
        private readonly GameObject m_PlayerActionsSelectPanel;
        [UIElementReference(50)]
        private readonly Text m_SelectedPlayerText;
        [UIElementActionReference(nameof(ShowMutePanel))]
        [UIElementReference(40)]
        private readonly Button m_OpenMutePanel;
        [UIElementActionReference(nameof(ShowReportPanel))]
        [UIElementReference(41)]
        private readonly Button m_OpenReportPanel;
        [UIElementActionReference(nameof(CopyUserInfo))]
        [UIElementReference(42)]
        private readonly Button m_CopyUserInfo;
        [UIElementActionReference(nameof(HidePlayerActions))]
        [UIElementReference(39)]
        private readonly Button m_PlayerActionsSelectClose;
        [UIElementReference(43)]
        private readonly GameObject m_PlayerActionsMutePanel;
        [UIElementReference(51)]
        private readonly Text m_MutePlayerText;
        [UIElementActionReference(nameof(MutePlayer))]
        [UIElementReference(44)]
        private readonly Button m_Mute;
        [UIElementActionReference(nameof(HidePlayerActions))]
        [UIElementReference(45)]
        private readonly Button m_CancelMute;
        [UIElementReference(46)]
        private readonly GameObject m_PlayerActionsReportPanel;
        [UIElementReference(52)]
        private readonly Text m_ReportPlayerText;
        [UIElementReference(48)]
        private readonly Dropdown m_ReportReason;
        [UIElementReference(49)]
        private readonly InputField m_ReportedPersonBehaviour;
        [UIElementActionReference(nameof(HidePlayerActions))]
        [UIElementReference(47)]
        private readonly Button m_CancelReport;
        [UIElementActionReference(nameof(ReportPlayer))]
        [UIElementReference(53)]
        private readonly Button m_Report;

        [UIElementReference(59)]
        private readonly CanvasGroup m_CanvasGroup;
        [UIElementReference(60)]
        private readonly RawImage m_BlurImage;

        private Material m_BlurMaterial;

        private readonly Dictionary<string, GameObject> m_Logos = new Dictionary<string, GameObject>();

        private bool m_HasVotedUp;
        private bool m_HasVotedDown;

        public bool ScheduleHide;

        private readonly ParametersMenu m_Parameters;
        private MultiplayerPlayerInfoState m_TargetPlayer;

        private float m_TargetCanvasGroupAlpha;
        private float m_TargetBlurSize;

        private bool m_IsHidding;

        private bool m_IsTemporaryHidden;
        public bool isTemporaryHidden
        {
            get => m_IsTemporaryHidden;
            set
            {
                m_IsTemporaryHidden = value;
                m_TargetCanvasGroupAlpha = value ? 0f : 1f;
                m_TargetBlurSize = value ? 0f : 2.2f;
                m_CanvasGroup.interactable = !value;
                m_CanvasGroup.blocksRaycasts = !value;
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            m_BlurMaterial = m_BlurImage.material;
            isTemporaryHidden = false;

            AssignActionToButton(MyModdedObject, 16, OnContinueClicked);
            _ = MyModdedObject.GetObject<Transform>(16).gameObject.AddComponent<UIComponentButtonScaler>();
            AssignActionToButton(MyModdedObject, 15, delegate
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
            _ = MyModdedObject.GetObject<Transform>(15).gameObject.AddComponent<UIComponentButtonScaler>();

            m_ReportedPersonBehaviour.text = string.Empty;

            m_Logos.Add("ko", MyModdedObject.GetObject<Transform>(57).gameObject);
            m_Logos.Add("ja", MyModdedObject.GetObject<Transform>(56).gameObject);
            m_Logos.Add("zh-CN", MyModdedObject.GetObject<Transform>(55).gameObject);
            m_Logos.Add("zh-TW", MyModdedObject.GetObject<Transform>(55).gameObject);
            m_Logos.Add(string.Empty, MyModdedObject.GetObject<Transform>(54).gameObject);
            Hide();
        }

        public override void Show()
        {
            base.Show();
            TimeManager.Instance.OnGamePaused();

            RefreshAdvancements();
            RefreshRoomCodeField();
            RefreshRoomCodePanelActive();
            RefreshStartMatchButton();
            RefreshPlayersInMatch();
            RefreshCurrentWorkshopLevel();
            HidePlayerActions();

            string langCode = SettingsManager.Instance.GetCurrentLanguageID();
            foreach (string key in m_Logos.Keys)
                m_Logos[key].SetActive(key == langCode);

            m_Logos[string.Empty].SetActive(!m_Logos.ContainsKey(langCode));

            m_IsHidding = false;
            m_SkipLevelButton.gameObject.SetActive(GameModeManager.CanSkipCurrentLevel());
            m_BackToLVLEditorButton.gameObject.SetActive(GameModeManager.IsLevelPlaytest());
            m_PersonalizationNotification.gameObject.SetActive(!WeaponSkinsController.HasNoticedSkinsButton && !GameModeManager.IsInLevelEditor());
            m_PersonalizationButton.interactable = !GameModeManager.IsInLevelEditor();
            OverhaulUIManager.SetCanvasPixelPerfect(false);
        }

        public override void Hide()
        {
            m_IsHidding = true;
            ShowCursor = false;
            SetPanelActive(m_PersonalizationPanel, null, false);
            SetPanelActive(m_ExitSelectPanel, null, false);
            SetPanelActive(m_SettingsSelectPanel, null, false);
            TimeManager.Instance.OnGameUnPaused();
        }

        public override void Update()
        {
            base.Update();
            if (!m_IsHidding && Input.GetKeyDown(KeyCode.Alpha0) && !m_PlayerActionsMenu.activeSelf)
            {
                ForceUseOldMenu = true;
                Hide();
                GameUIRoot.Instance.EscMenu.Show();
                GameUIRoot.Instance.RefreshCursorEnabled();
                ForceUseOldMenu = false;
            }

            float deltaTime = Time.unscaledDeltaTime * 15f;
            float alpha = m_CanvasGroup.alpha;
            alpha = Mathf.Lerp(alpha, m_IsHidding ? 0f : m_TargetCanvasGroupAlpha, deltaTime);
            m_CanvasGroup.alpha = alpha;

            if (m_IsHidding && alpha <= 0.1f)
            {
                m_IsHidding = false;
                base.gameObject.SetActive(false);
                return;
            }

            if (!m_BlurMaterial)
                return;
            _ = m_BlurMaterial.GetFloat("_Size");
            float blurSize = Mathf.Lerp(alpha, m_IsHidding ? 0f : m_TargetBlurSize, deltaTime);
            m_BlurMaterial.SetFloat("_Size", blurSize);
        }

        public override void OnEnable()
        {
            _ = AudioManager.Instance.PlayClipGlobal(AudioLibrary.Instance.UISelectionPress, 0f, 1f, 0f);
        }

        public override void OnDisable()
        {
            _ = AudioManager.Instance.PlayClipGlobal(AudioLibrary.Instance.UISelectionBack, 0f, 1f, 0f);
            m_CanvasGroup.alpha = 0f;
            isTemporaryHidden = false;
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
                OverhaulSettingInfo_Old.SavePref(OverhaulSettingsManager_Old.GetSetting("Player.WeaponSkins.NoticedSkinsButton", true), true);
                m_PersonalizationNotification.gameObject.SetActive(false);
            }

            m_PersonalizationButton.OnDeselect(null);
            m_SettingsSelectPanel.gameObject.SetActive(false);
            m_ExitSelectPanel.gameObject.SetActive(false);
            SetPanelActive(m_PersonalizationPanel, m_PersonalizationButton.transform, !m_PersonalizationPanel.gameObject.activeSelf);
        }

        public void OnSkinsButtonClicked()
        {
        }

        public void OnOutfitsButtonClicked()
        {
        }

        public void OnPetsButtonClicked()
        {
        }

        public void OnPersonalizationEditorButtonClicked()
        {
        }

        #endregion

        #region Exit

        public void OnExitButtonClicked()
        {
            m_ExitButton.OnDeselect(null);
            m_SettingsSelectPanel.gameObject.SetActive(false);
            m_PersonalizationPanel.gameObject.SetActive(false);
            SetPanelActive(m_ExitSelectPanel, m_ExitButton.transform, !m_ExitSelectPanel.gameObject.activeSelf);
        }

        public void OnMainMenuButtonClicked()
        {
            SceneTransitionManager.Instance.DisconnectAndExitToMainMenu();
        }

        public void OnDesktopButtonClicked()
        {
            OverhaulTransitionManager.reference.DoTransition(delegate
            {
                Application.Quit();
            });
        }

        #endregion

        #region Settings

        public void OnSettingsButtonClicked()
        {
            m_SettingsButton.OnDeselect(null);
            SetPanelActive(m_SettingsSelectPanel, m_SettingsButton.transform, !m_SettingsSelectPanel.gameObject.activeSelf);
            m_ExitSelectPanel.gameObject.SetActive(false);
            m_PersonalizationPanel.gameObject.SetActive(false);
        }

        public void OnGameSettingsButtonClicked()
        {
            if (!base.gameObject.activeSelf)
                return;

            SetPanelActive(m_SettingsSelectPanel, null, false);
            isTemporaryHidden = true;
            GameUIRoot.Instance.SettingsMenu.Show();
            _ = base.StartCoroutine(waitUntilGameSettingsMenuIsClosed());
        }

        public void OnModSettingsButtonClicked()
        {
            if (!base.gameObject.activeSelf)
                return;

            SetPanelActive(m_SettingsSelectPanel, null, false);
            isTemporaryHidden = true;
            UIConstants.ShowSettingsMenu();
            _ = base.StartCoroutine(waitUntilModSettingsMenuIsClosed());
        }

        private IEnumerator waitUntilModSettingsMenuIsClosed()
        {
            UISettingsMenu settingsMenu = OverhaulUIManager.reference?.GetUI<UISettingsMenu>(UIConstants.UI_SETTINGS_MENU);
            if (!settingsMenu)
                yield break;

            yield return new WaitUntil(() => !settingsMenu.gameObject.activeSelf);
            isTemporaryHidden = false;
            yield break;
        }

        private IEnumerator waitUntilGameSettingsMenuIsClosed()
        {
            GameObject settingsMenu = GameUIRoot.Instance?.SettingsMenu?.gameObject;
            if (!settingsMenu)
                yield break;

            yield return new WaitUntil(() => !settingsMenu.activeSelf);
            isTemporaryHidden = false;
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

            m_AdvFillImage.fillAmount = completed / (float)all;
            m_AdvCompletedText.text = string.Format("{0}/{1}", new object[] { completed, all });
        }

        public void OnAdvancementsButtonClicked()
        {
            if (!base.gameObject.activeSelf)
                return;

            isTemporaryHidden = true;
            GameUIRoot.Instance.AchievementProgressUI.Show();
            _ = base.StartCoroutine(waitUntilAdvancementsMenuIsClosed());
        }

        private IEnumerator waitUntilAdvancementsMenuIsClosed()
        {
            GameObject advancementsMenu = GameUIRoot.Instance?.AchievementProgressUI?.gameObject;
            if (!advancementsMenu)
                yield break;

            yield return new WaitUntil(() => !advancementsMenu.activeSelf);
            isTemporaryHidden = false;
            yield break;
        }

        #endregion

        #region Room code

        public void RefreshRoomCodePanelActive()
        {
            m_RoomCodePanel.SetActive(OverhaulGamemodeManager.ShouldShowRoomCodePanel());
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

        public void OnCopyRoomCodeButtonClicked()
        {
            OverhaulGamemodeManager.CopyPrivateRoomCode();
        }

        public void OnStartMatchButtonClicked()
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
            m_PlayersInMatchPanel.SetActive(false);
            m_PlayersInMatch.Clear();

            MultiplayerPlayerInfoManager manager = MultiplayerPlayerInfoManager.Instance;
            if (!GameModeManager.IsMultiplayer() || GameModeManager.IsMultiplayerDuel() || !manager)
                return;

            List<MultiplayerPlayerInfoState> allMultiplayerPlayerInfos = manager.GetAllPlayerInfoStates();
            if (allMultiplayerPlayerInfos.IsNullOrEmpty())
                return;

            m_PlayersInMatchPanel.SetActive(true);
            int index = 0;
            do
            {
                MultiplayerPlayerInfoState multiplayerPlayerInfo = allMultiplayerPlayerInfos[index];
                if (!multiplayerPlayerInfo || multiplayerPlayerInfo.IsDetached())
                {
                    index++;
                    continue;
                }

                ModdedObject entry = m_PlayersInMatch.InstantiateEntry();
                MultiplayerPlayerInfoStateDisplay display = entry.gameObject.AddComponent<MultiplayerPlayerInfoStateDisplay>();
                display.Initialize(multiplayerPlayerInfo, entry);

                index++;
            } while (index < allMultiplayerPlayerInfos.Count);
        }

        #endregion

        #region Workshop level

        public void RefreshCurrentWorkshopLevel()
        {
            m_CurrentWorkshopLevel.SetActive(false);

            SteamWorkshopItem item = NowPlayingWorkshopManager.Instance.GetCurrentItem();
            if (item == null)
                return;

            m_CurrentWorkshopLevel.SetActive(true);
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

        public void OnVoteUpButtonClicked()
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

        public void OnVoteDownButtonClicked()
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
            m_CopyUserInfo.gameObject.SetActive(OverhaulFeaturesSystem.IsFeatureUnlocked(EUnlockableFeatures.PermissionToCopyUserInfos));
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
            Hide();
        }

        public void OnReturnToLevelEditorButtonClicked()
        {
            GameUIRoot.Instance.EscMenu.OnBackToLevelEditorButtonClicked();
        }

        public void OnSkipLevelButtonClicked()
        {
            GameUIRoot.Instance.EscMenu.OnSkipWorkshopLevelClicked();
            Hide();
        }
    }
}