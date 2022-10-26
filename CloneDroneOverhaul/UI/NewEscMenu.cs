using CloneDroneOverhaul.Utilities;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace CloneDroneOverhaul.UI
{
    public class NewEscMenu : ModGUIBase
    {
        private bool _wasMainMenuButtonClicked;
        private bool _wasExitToDesktopButtonClicked;
        private bool _userDismissedStream;

        private bool ShowPlayerList { get { return GameModeManager.IsMultiplayer(); } }
        private bool ShowPhotoModeButton { get { return !GameModeManager.IsMultiplayer() && !CharacterTracker.Instance.GetPlayer().GetRobotInfo().IsNull; } }
        private bool ShowWorkshopLevelInfo { get { return GameModeManager.IsPlayingWorkshopLevelInEndlessOrTwitch(); } }

        // 93285630 - Erik
        // 103096704 - Brian
        // 138733010 - Vastlite
        private int DevLiveID
        {
            get
            {
                bool somebodyLive = TwitchWhoIsLiveManager.Instance.HasDismissedDevLiveNotificationsThisSession() || !SettingsManager.Instance.AreDevLiveNotificationsEnabled();
                if (somebodyLive)
                {
                    for (int i = 0; i < TwitchWhoIsLiveManager.Instance.DeveloperLivePrefabs.Length; i++)
                    {
                        if (TwitchWhoIsLiveManager.Instance.IsDeveloperUserIDLive(TwitchWhoIsLiveManager.Instance.DeveloperLivePrefabs[i].DeveloperUserID))
                        {
                            return TwitchWhoIsLiveManager.Instance.DeveloperLivePrefabs[i].DeveloperUserID;
                        }
                    }
                }
                return -1;
            }
        }

        private string _currentDevStreamLink;

        private string GetAchievementsCompletedText
        {
            get
            {
                float fractionOfAchievementsCompleted = GameplayAchievementManager.Instance.GetFractionOfAchievementsCompleted();
                GameplayAchievement[] achs = GameplayAchievementManager.Instance.GetAllAchievements();
                int completed = 0;
                foreach (GameplayAchievement ach in achs)
                {
                    if (ach.IsComplete())
                    {
                        completed++;
                    }
                }
                return "[" + completed + "/" + achs.Length + "] " + OverhaulMain.GetTranslatedString("Ach_Completed");
            }
        }

        private Text Tips;
        private Text GameStatus;

        private ModdedObject SpecialButtonsMObj;
        private Button PhotoMode;
        private Button CDOSettings;
        private Button Addons;
        private Button StatisticsButton;

        private ModdedObject RightSideMObj;
        private Button ContinueButton;
        private Button SettingsButton;
        private Button AchButton;
        private Text AchProgressText;
        private Button MainMenuButton;
        private Button DesktopButton;
        //
        private ModdedObject WorkshopLevelInfoMObj;
        private Text WLI_Title;
        private Text WLI_Desc;
        private Text WLI_Author;
        private Image WLI_Preview;
        private Button WLI_SkipLevelButton;
        private Button WLI_Page;
        //
        private ModdedObject LevelProgressMObj;
        private Text LP_Title;

        private ModdedObject DevLiveMObj;
        private RectTransform DL_Erik;
        private RectTransform DL_Brian;
        private RectTransform DL_Vast;
        private Button SayHelloButton;
        private Button CloseButton;
        private Text DL_DevLiveText;
        private Text DL_DevBuildsGameText;

        private ModdedObject PlayersInMatchMObj;
        private Text PIM_Label;
        private RectTransform PIM_Prefab;
        private RectTransform PIM_Container;

        private ModdedObject AdditThingsMObj;
        //
        private RectTransform CodeUI;
        private Text CUI_InviteFriendsText;
        private Button CUI_ShowCodeButton;
        private Text CUI_Code;
        private Button CUI_CodeButton;
        //
        private RectTransform StartMatchButtonParent;
        private Button StartMatchButton;
        //
        private RectTransform BackToLvLEditorButtonParent;
        private Button BackToLvLEditorButton;

        public override void OnInstanceStart()
        {
            base.MyModdedObject = GetComponent<ModdedObject>();
            BaseStaticReferences.NewEscMenu = this;
            Hide();

            SpecialButtonsMObj = MyModdedObject.GetObjectFromList<ModdedObject>(0);
            PhotoMode = SpecialButtonsMObj.GetObjectFromList<Button>(0);
            CDOSettings = SpecialButtonsMObj.GetObjectFromList<Button>(1);
            CDOSettings.onClick.AddListener(UI.SettingsUI.Instance.Show);
            Addons = SpecialButtonsMObj.GetObjectFromList<Button>(2);
            StatisticsButton = SpecialButtonsMObj.GetObjectFromList<Button>(3);

            RightSideMObj = MyModdedObject.GetObjectFromList<ModdedObject>(1);
            ContinueButton = RightSideMObj.GetObjectFromList<Button>(0);
            SettingsButton = RightSideMObj.GetObjectFromList<Button>(1);
            AchButton = RightSideMObj.GetObjectFromList<Button>(2);
            AchProgressText = RightSideMObj.GetObjectFromList<Text>(3);
            MainMenuButton = RightSideMObj.GetObjectFromList<Button>(4);
            DesktopButton = RightSideMObj.GetObjectFromList<Button>(5);
            //
            WorkshopLevelInfoMObj = RightSideMObj.GetObjectFromList<ModdedObject>(6);
            WLI_Title = WorkshopLevelInfoMObj.GetObjectFromList<Text>(0);
            WLI_Desc = WorkshopLevelInfoMObj.GetObjectFromList<Text>(1);
            WLI_Author = WorkshopLevelInfoMObj.GetObjectFromList<Text>(2);
            WLI_SkipLevelButton = WorkshopLevelInfoMObj.GetObjectFromList<Button>(4);
            WLI_Page = WorkshopLevelInfoMObj.GetObjectFromList<Button>(5);
            WLI_Preview = WorkshopLevelInfoMObj.GetObjectFromList<Image>(3);
            //
            LevelProgressMObj = RightSideMObj.GetObjectFromList<ModdedObject>(7);
            LP_Title = LevelProgressMObj.GetObjectFromList<Text>(0);

            DevLiveMObj = MyModdedObject.GetObjectFromList<ModdedObject>(2);
            DL_Erik = DevLiveMObj.GetObjectFromList<RectTransform>(0);
            DL_Brian = DevLiveMObj.GetObjectFromList<RectTransform>(7);
            DL_Vast = DevLiveMObj.GetObjectFromList<RectTransform>(8);
            SayHelloButton = DevLiveMObj.GetObjectFromList<Button>(4);
            SayHelloButton.onClick.AddListener(onSayHelloClicked);
            CloseButton = DevLiveMObj.GetObjectFromList<Button>(6);
            CloseButton.onClick.AddListener(onDismissDevStreamClicked);
            DL_DevLiveText = DevLiveMObj.GetObjectFromList<Text>(2);
            DL_DevBuildsGameText = DevLiveMObj.GetObjectFromList<Text>(3);

            PlayersInMatchMObj = MyModdedObject.GetObjectFromList<ModdedObject>(4);
            PIM_Label = PlayersInMatchMObj.GetObjectFromList<Text>(0);
            PIM_Prefab = PlayersInMatchMObj.GetObjectFromList<RectTransform>(2);
            PIM_Container = PlayersInMatchMObj.GetObjectFromList<RectTransform>(1);

            AdditThingsMObj = MyModdedObject.GetObjectFromList<ModdedObject>(7);
            CodeUI = AdditThingsMObj.GetObjectFromList<RectTransform>(5);
            CUI_Code = AdditThingsMObj.GetObjectFromList<Text>(0);
            CUI_CodeButton = AdditThingsMObj.GetObjectFromList<Button>(0);
            CUI_InviteFriendsText = AdditThingsMObj.GetObjectFromList<Text>(2);
            CUI_ShowCodeButton = AdditThingsMObj.GetObjectFromList<Button>(1);
            StartMatchButtonParent = AdditThingsMObj.GetObjectFromList<RectTransform>(3);
            StartMatchButton = AdditThingsMObj.GetObjectFromList<Button>(4);
            BackToLvLEditorButtonParent = AdditThingsMObj.GetObjectFromList<RectTransform>(6);
            BackToLvLEditorButton = AdditThingsMObj.GetObjectFromList<Button>(7);

            Tips = MyModdedObject.GetObjectFromList<Text>(3);
            GameStatus = MyModdedObject.GetObjectFromList<Text>(5);

            WLI_SkipLevelButton.onClick.AddListener(new UnityEngine.Events.UnityAction(GameUIRoot.Instance.EscMenu.OnSkipWorkshopLevelClicked));

            StartMatchButton.onClick.AddListener(new UnityEngine.Events.UnityAction(GameUIRoot.Instance.EscMenu.OnStartBattleRoyaleLevelClicked));
            BackToLvLEditorButton.onClick.AddListener(new UnityEngine.Events.UnityAction(GameUIRoot.Instance.EscMenu.OnBackToLevelEditorButtonClicked));
            CUI_ShowCodeButton.onClick.AddListener(new UnityEngine.Events.UnityAction(onShowCodeButtonClicked));

            ContinueButton.onClick.AddListener(new UnityEngine.Events.UnityAction(onContinueClicked));
            SettingsButton.onClick.AddListener(new UnityEngine.Events.UnityAction(onSettingsClicked));
            AchButton.onClick.AddListener(new UnityEngine.Events.UnityAction(onAchClicked));
            MainMenuButton.onClick.AddListener(new UnityEngine.Events.UnityAction(onMainMenuClicked));
            DesktopButton.onClick.AddListener(new UnityEngine.Events.UnityAction(onExitGameClicked));

            CUI_CodeButton.onClick.AddListener(new UnityEngine.Events.UnityAction(onCopyCodeClicked));

            WLI_Page.onClick.AddListener(new UnityEngine.Events.UnityAction(onWorkshopItemPageClicked));

            PhotoMode.onClick.AddListener(new UnityEngine.Events.UnityAction(onPhotoModeButtonClicked)); // Make "Legacy photo mode activation method" setting
        }

        public override void RunFunction(string name, object[] arguments)
        {
            if (name == "onBoltShutdown")
            {
                Hide();
            }
        }

        public void Show()
        {
            base.gameObject.SetActive(true);
            GetComponent<Animator>().Play("EscMenuShow");
            AchProgressText.text = GetAchievementsCompletedText;
            ContinueButton.GetComponent<Text>().text = OverhaulMain.GetTranslatedString("EscMenu_Continue");
            SettingsButton.GetComponent<Text>().text = OverhaulMain.GetTranslatedString("EscMenu_Settings");
            AchButton.GetComponent<Text>().text = OverhaulMain.GetTranslatedString("EscMenu_Achievements");
            MainMenuButton.GetComponent<Text>().text = OverhaulMain.GetTranslatedString("EscMenu_MainMenu");
            DesktopButton.GetComponent<Text>().text = OverhaulMain.GetTranslatedString("EscMenu_ExitToDesktop");
            PlayersInMatchMObj.GetObjectFromList<Text>(0).text = OverhaulMain.GetTranslatedString("EscMenu_PlayersInMatch");

            MyModdedObject.GetObjectFromList<Text>(8).text = OverhaulMain.GetTranslatedString("PhotoMode");
            MyModdedObject.GetObjectFromList<Text>(9).text = OverhaulMain.GetTranslatedString("DebugInfo");
            MyModdedObject.GetObjectFromList<Text>(10).text = OverhaulMain.GetTranslatedString("CDOSettings");
            MyModdedObject.GetObjectFromList<Text>(11).text = OverhaulMain.GetTranslatedString("ShowCode");
            MyModdedObject.GetObjectFromList<Text>(12).text = OverhaulMain.GetTranslatedString("StartDaMatch");
            MyModdedObject.GetObjectFromList<Text>(13).text = OverhaulMain.GetTranslatedString("BackToLVLEditor");

            if (BaseStaticValues.IsEscMenuWaitingToShow)
            {
                BaseStaticValues.IsEscMenuWaitingToShow = false;
                return;
            }

            refreshTip();
            refreshLevelProgress();
            refreshHosting();
            refreshSpecialButtons();
            refreshWorkshop();
            refreshPlayers();
            refreshDevs();
        }
        public void Hide()
        {
            base.gameObject.SetActive(false);
            BaseStaticReferences.GUIs.GetGUI<UI.SettingsUI>().Hide();
        }

        private void refreshTip()
        {
            Tips.text = "";
        }

        private void refreshDevs()
        {
            int devID = DevLiveID;
            bool showNotification = devID != -1;
            DevLiveMObj.gameObject.SetActive(showNotification && !_userDismissedStream);
            if (showNotification)
            {
                // 93285630 - Erik
                // 103096704 - Brian
                // 138733010 - Vastlite

                DL_Erik.gameObject.SetActive(false);
                DL_Vast.gameObject.SetActive(false);
                DL_Brian.gameObject.SetActive(false);

                string devName = string.Empty;

                if (devID == 93285630)
                {
                    DL_Erik.gameObject.SetActive(true);
                    devName = "Erik";
                    _currentDevStreamLink = "https://www.twitch.tv/doborog";
                }
                if (devID == 103096704)
                {
                    DL_Brian.gameObject.SetActive(true);
                    devName = "Brian";
                    _currentDevStreamLink = "https://www.twitch.tv/bribrobot";
                }
                if (devID == 138733010)
                {
                    DL_Vast.gameObject.SetActive(true);
                    devName = "VastLite";
                    _currentDevStreamLink = "https://www.twitch.tv/vastlite";
                }

                DL_DevBuildsGameText.text = "Watch <size=16>" + devName + "</size>" + System.Environment.NewLine + "Build the game!";

                if (GameplayAchievementManager.Instance.HasUnlockedAchievement("SayHelloToDoborog"))
                {
                    Tips.text = "Tip: Press that \"Say hello\" button to get \"Hello doborog\" Achievement";
                }
            }
        }

        private void refreshPlayers()
        {
            PIM_Prefab.gameObject.SetActive(false);
            PlayersInMatchMObj.gameObject.SetActive(GameModeManager.IsMultiplayer());
            if (!GameModeManager.IsMultiplayer())
            {
                return;
            }
            TransformUtils.DestroyAllChildren(PIM_Container);
            List<MultiplayerPlayerInfoState> allPlayerInfoStates = Singleton<MultiplayerPlayerInfoManager>.Instance.GetAllPlayerInfoStates();
            for (int i = 0; i < allPlayerInfoStates.Count; i++)
            {
                MultiplayerPlayerInfoState infoState = allPlayerInfoStates[i];
                if (!infoState.IsDetached() && infoState.ShouldBeVisibleToPlayers() && !(infoState == Singleton<MultiplayerPlayerInfoManager>.Instance.GetLocalPlayerInfoState()))
                {
                    ModdedObject mObj = Instantiate<ModdedObject>(PIM_Prefab.GetComponent<ModdedObject>(), PIM_Container);
                    mObj.gameObject.SetActive(true);
                    mObj.GetObjectFromList<InputField>(0).text = infoState.state.DisplayName;
                    mObj.GetObjectFromList<Text>(2).text = GameModeManager.IsBattleRoyale() ? infoState.state.LastBotStandingWins.ToString() : "--";
                    mObj.GetObjectFromList<Text>(1).text = OverhaulMain.GetTranslatedString("EscMenu_Wins");
                    mObj.GetObjectFromList<Text>(4).text = MultiplayerPlayerInfoManager.Instance.GetPlayerPlatform(infoState.state.PlayFabID).ToString();
                    mObj.GetObjectFromList<Text>(3).text = OverhaulMain.GetTranslatedString("EscMenu_Platform");
                }
            }
        }
        private void refreshWorkshop()
        {
            WLI_SkipLevelButton.interactable = GameModeManager.CanSkipCurrentLevel();
            if (!GameModeManager.Is(GameMode.Endless))
            {
                this.WorkshopLevelInfoMObj.gameObject.SetActive(false);
                return;
            }

            SteamWorkshopItem item = Singleton<WorkshopLevelManager>.Instance.GetCurrentLevelWorkshopItem();
            if(item != null)
            {
                WorkshopLevelInfoMObj.gameObject.SetActive(true);
                Coroutines.LoadWorkshopImage(item.PreviewURL, delegate (Sprite spr)
                {
                    WLI_Preview.sprite = spr;
                });
                WLI_Author.text = "By " + item.CreatorName;
                WLI_Desc.text = item.Description;
                WLI_Title.text = item.Title;
            }
            else
            {
                this.WorkshopLevelInfoMObj.gameObject.SetActive(false);
            }
        }
        private void refreshSpecialButtons()
        {
            PhotoMode.gameObject.SetActive(ShowPhotoModeButton);
            SettingsButton.interactable = CharacterTracker.Instance.GetPlayer() != null;
            AchButton.interactable = CharacterTracker.Instance.GetPlayer() != null;
        }
        private void refreshHosting()
        {
            BackToLvLEditorButtonParent.gameObject.SetActive(WorkshopLevelManager.Instance.IsPlaytestActive());
            string details = string.Empty;
            bool isHost = GameModeManager.IsMultiplayer() && MultiplayerMatchmakingManager.Instance.IsLocalPlayerHostOfCustomMatch();
            CodeUI.gameObject.SetActive(isHost);
            CUI_Code.text = MultiplayerMatchmakingManager.Instance.GetLastInviteCode();
            CUI_ShowCodeButton.gameObject.SetActive(true);
            CUI_InviteFriendsText.text = OverhaulMain.GetTranslatedString("EscMenu_InviteFriends");

            if (GameModeManager.IsMultiplayer())
            {
                details = OverhaulMain.GetTranslatedString("Multiplayer");
            }

            if (GameModeManager.IsMultiplayer() && GameModeManager.Instance.IsPrivateMatch())
            {
                details = OverhaulMain.GetTranslatedString("Multiplayer") + "," + System.Environment.NewLine + OverhaulMain.GetTranslatedString("PrivateMatch");
            }

            StartMatchButtonParent.gameObject.SetActive(false);

            if (isHost)
            {
                details = OverhaulMain.GetTranslatedString("Multiplayer") + "," + System.Environment.NewLine + OverhaulMain.GetTranslatedString("HostOfPrivateRoom");
                if (ArenaCoopManager.Instance != null && !ArenaCoopManager.Instance.IsMatchStarted())
                {
                    StartMatchButtonParent.gameObject.SetActive(true);
                }
                if (BattleRoyaleManager.Instance != null)
                {
                    if (BattleRoyaleManager.Instance.IsProgress(BattleRoyaleMatchProgress.InWaitingArea))
                    {
                        StartMatchButtonParent.gameObject.SetActive(true);
                    }
                    else if (BattleRoyaleManager.Instance.IsProgress(BattleRoyaleMatchProgress.FightingStarted))
                    {
                        StartMatchButtonParent.gameObject.SetActive(true);
                    }
                }
            }

            this.GameStatus.text = details;
        }

        private void refreshLevelProgress()
        {
            GameMode gameMode = GameFlowManager.Instance.GetCurrentGameMode();
            bool canShow = gameMode == GameMode.BattleRoyale || gameMode == GameMode.Endless ||
                gameMode == GameMode.EndlessCoop;
            LevelProgressMObj.gameObject.SetActive(canShow);
            if (!canShow)
            {
                return;
            }

            if (gameMode == GameMode.Endless || gameMode == GameMode.EndlessCoop)
            {
                string final = OverhaulMain.GetTranslatedString("EscMenu_EndlessMode") + ", " + OverhaulMain.GetTranslatedString("EscMenu_Level") + " " + (LevelManager.Instance.GetNumberOfLevelsWon() + 1) + ", ";
                EndlessTierDescription desc = EndlessModeManager.Instance.GetNextLevelDifficultyTierDescription(LevelManager.Instance.GetNumberOfLevelsWon());
                string hexColor = desc.TextColor.ToHex();
                if (hexColor == "#A3732C")
                {
                    hexColor = "#DA7F3C";
                }
                string tier = "<color=" + hexColor + ">" + LocalizationManager.Instance.GetTranslatedString(desc.TierDescription, -1) + "</color>";
                final = final + tier;

                LP_Title.text = final;
            }
            if(gameMode == GameMode.BattleRoyale)
            {
                if(BattleRoyaleManager.Instance == null)
                {
                    return;
                }
                BattleRoyaleMatchProgress progress = (BattleRoyaleMatchProgress)BattleRoyaleManager.Instance.state.MatchProgress;
                string final = OverhaulMain.GetTranslatedString("EscMenu_LBS") + ", ";
                if (progress == BattleRoyaleMatchProgress.InWaitingArea)
                {
                    final += OverhaulMain.GetTranslatedString("LBS_WaitingArea");
                }
                else if (progress == BattleRoyaleMatchProgress.GarbageBotsGrabbingPlayers || progress == BattleRoyaleMatchProgress.GatheringGarbageBotsInRing || progress == BattleRoyaleMatchProgress.NotStarted)
                {
                    final += OverhaulMain.GetTranslatedString("LBS_MatchStart");
                }
                else if (progress == BattleRoyaleMatchProgress.CombatAreaShrinking)
                {
                    final += OverhaulMain.GetTranslatedString("LBS_CombatAreaShrinking");
                }
                else if (progress == BattleRoyaleMatchProgress.MatchSettled)
                {
                    final += OverhaulMain.GetTranslatedString("LBS_End");
                }
                LP_Title.text = final;
            }
        }

        private void onContinueClicked()
        {
            GameUIRoot.Instance.EscMenu.Hide();
        }
        private void onSettingsClicked()
        {
            GameUIRoot.Instance.SettingsMenu.Show();
            BaseStaticReferences.NewEscMenu.Hide();
            BaseStaticValues.IsEscMenuWaitingToShow = true;
        }
        private void onAchClicked()
        {
            GameUIRoot.Instance.AchievementProgressUI.Show();
            BaseStaticReferences.NewEscMenu.Hide();
            BaseStaticValues.IsEscMenuWaitingToShow = true;
        }
        private void onMainMenuClicked() // Implement Generic2Button
        {
            if (_wasMainMenuButtonClicked)
            {
                Hide();
                GameUIRoot.Instance.EscMenu.OnMainMenuConfirmClicked();
                return;
            }
            _wasMainMenuButtonClicked = true;
            MainMenuButton.GetComponent<Text>().text = OverhaulMain.GetTranslatedString("EscMenu_AreYouSure");
            OverhaulMain.Timer.AddNoArgAction(resetMainMenuButton, 3, true);
        }
        private void onExitGameClicked()
        {
            if (_wasExitToDesktopButtonClicked)
            {
                GameUIRoot.Instance.EscMenu.OnExitGameConfirmClicked();
                return;
            }
            _wasExitToDesktopButtonClicked = true;
            DesktopButton.GetComponent<Text>().text = OverhaulMain.GetTranslatedString("EscMenu_AreYouSure");
            OverhaulMain.Timer.AddNoArgAction(resetExitGameButton, 3, true);
        }
        private void resetMainMenuButton()
        {
            _wasMainMenuButtonClicked = false;
            MainMenuButton.GetComponent<Text>().text = OverhaulMain.GetTranslatedString("EscMenu_MainMenu");
        }
        private void resetExitGameButton()
        {
            _wasExitToDesktopButtonClicked = false;
            DesktopButton.GetComponent<Text>().text = OverhaulMain.GetTranslatedString("EscMenu_ExitDesktop");
        }

        private void onShowCodeButtonClicked()
        {
            CUI_ShowCodeButton.gameObject.SetActive(false);
        }
        private void onPhotoModeButtonClicked()
        {
            GameUIRoot.Instance.EscMenu.Hide();
            PhotoManager.Instance.TriggerPhotoModeOnOff(true);
        }

        private void onCopyCodeClicked()
        {
            BaseUtils.CopyToClipboard(this.CUI_Code.text, true, "Code ", " was copied to clipboard!");
        }

        private void onWorkshopItemPageClicked()
        {
            SteamWorkshopItem item = Singleton<WorkshopLevelManager>.Instance.GetCurrentLevelWorkshopItem();
            if(item != null)
            {
                BaseUtils.OpenURL("http://steamcommunity.com/sharedfiles/filedetails/?source=CloneDroneGame&id=" + item.WorkshopItemID.ToString());
            }
        }

        private void onSayHelloClicked()
        {
            Singleton<GlobalEventManager>.Instance.Dispatch<SimpleAchievementEvent>("SimpleAchivementEvent", SimpleAchievementEvent.ClickedDoborogButton);
            BaseUtils.OpenURL(_currentDevStreamLink);
        }

        private void onDismissDevStreamClicked()
        {
            _userDismissedStream = true;
            DevLiveMObj.gameObject.SetActive(false);
        }
    }
}
