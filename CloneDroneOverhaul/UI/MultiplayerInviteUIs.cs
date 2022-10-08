using CloneDroneOverhaul.UI.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CloneDroneOverhaul.UI
{
    public class MultiplayerInviteUIs : ModGUIBase
    {
        public static MultiplayerInviteUIs Instance;
        private DuelUI duelUI;
        private CoopUI coopUI;
        private LBSUI lbsUI;

        public Sprite CoopEndlessImage;
        public Sprite CoopChallengeImage;


        public override void OnInstanceStart()
        {
            Instance = this;
            CoopEndlessImage = ModLibrary.AssetLoader.GetObjectFromFile<Sprite>("cdo_rw_stuff", "Background_EndlessCoop");
            CoopChallengeImage = ModLibrary.AssetLoader.GetObjectFromFile<Sprite>("cdo_rw_stuff", "Background_ChallengeCoop");
            base.MyModdedObject = base.GetComponent<ModdedObject>();
            duelUI = new DuelUI(MyModdedObject.GetObjectFromList<ModdedObject>(0));
            coopUI = new CoopUI(MyModdedObject.GetObjectFromList<ModdedObject>(1));
            lbsUI = new LBSUI(MyModdedObject.GetObjectFromList<ModdedObject>(3));
        }

        public override void RunFunction(string name, object[] arguments)
        {
        }

        public override void OnNewFrame()
        {
            lbsUI.Update();
        }

        public void ShowWithCode(string inviteCode, bool showSettings)
        {
            bool isLBS = false;
            bool isCoop = false;
            bool isCoopChallenge = false;
            bool isDuel = false;

            isLBS = GameModeManager.IsBattleRoyale();
            isCoop = GameModeManager.IsCoop();
            isCoopChallenge = GameModeManager.IsCoopChallenge();
            isDuel = GameModeManager.IsMultiplayerDuel();

            if (isDuel)
            {
                duelUI.SetActive(showSettings && MultiplayerMatchmakingManager.Instance.IsLocalPlayerHostOfCustomMatch());
            }
            if (isCoop)
            {
                coopUI.SetActive(showSettings, GameModeManager.IsCoopChallenge());
            }
            if (isLBS)
            {
                lbsUI.SetActive(showSettings);
            }
        }

        public override void OnManagedUpdate()
        {
            bool isLBS = false;
            bool isEndlessCoop = false;
            bool isCoopChallenge = false;
            bool isDuel = false;
            isLBS = GameModeManager.IsBattleRoyale();
            isEndlessCoop = GameModeManager.IsCoop() && !GameModeManager.IsCoopChallenge();
            isCoopChallenge = GameModeManager.IsCoopChallenge();
            isDuel = GameModeManager.IsMultiplayerDuel();
            duelUI.SetActive(isDuel && GameFlow1v1Manager.Instance != null && (GameFlow1v1Manager.Instance.IsProgress(MatchProgress1v1.WaitingForOpponent) || GameFlow1v1Manager.Instance.IsProgress(MatchProgress1v1.NotStarted)), true);
        }

        public bool ShallCursorBeActive()
        {
            return duelUI.gameObject.activeInHierarchy;
        }

        internal class LBSUI
        {
            public GameObject gameObject;
            private ModdedObject moddedObjectToTranslate;
            private ModdedObject moddedObjectMain;
            private RectTransform Container;
            private BetterToggleGroup SelectAllToggle;
            private Button RefreshEntriesButton;
            private Image Background;
            private Slider LevelLoadProgressSlider;
            private Text LevelLoadStateText;
            private RectTransform LevelLoadStateUI;
            private List<BattleRoyaleLevelDisplay> displays = new List<BattleRoyaleLevelDisplay>();

            private float timeToAllowNextRefresh = -1;

            public LBSUI(ModdedObject mObj)
            {
                gameObject = mObj.gameObject;
                SetActive(false);

                foreach (ModdedObject mObj2 in gameObject.GetComponents<ModdedObject>())
                {
                    if (mObj2.ID == "ToTranslate")
                    {
                        moddedObjectToTranslate = mObj2;
                    }
                    else if (mObj2.ID == "Main")
                    {
                        moddedObjectMain = mObj2;
                    }
                }

                Background = gameObject.GetComponent<Image>();

                Container = moddedObjectMain.GetObjectFromList<RectTransform>(9);
                SelectAllToggle = moddedObjectMain.GetObjectFromList<Toggle>(5).gameObject.AddAndConfigBetterToggleGroup(true);
                RefreshEntriesButton = moddedObjectMain.GetObjectFromList<Button>(2);
                RefreshEntriesButton.onClick.AddListener(RefreshLevels);
                moddedObjectMain.GetObjectFromList<RectTransform>(4).gameObject.SetActive(false);

                LevelLoadProgressSlider = moddedObjectMain.GetObjectFromList<Slider>(11);
                LevelLoadStateText = moddedObjectMain.GetObjectFromList<Text>(12);
                LevelLoadStateUI = moddedObjectMain.GetObjectFromList<RectTransform>(10);

                moddedObjectMain.GetObjectFromList<RectTransform>(13).gameObject.SetActive(false);

                moddedObjectMain.GetObjectFromList<Button>(8).onClick.AddListener(OnStartGameClicked);
                moddedObjectMain.GetObjectFromList<Button>(3).onClick.AddListener(OpenLBSWorkshop);
                moddedObjectMain.GetObjectFromList<Button>(6).onClick.AddListener(BaseUtils.CopyLobbyCode);

            }

            public void SetActive(bool value)
            {
                gameObject.SetActive(value);

                if (value)
                {
                    gameObject.GetComponent<Image>().sprite = OverhaulCacheManager.GetCached<Sprite>("LBSInviteScreenBG_" + Random.Range(1, 5).ToString());
                    moddedObjectMain.GetObjectFromList<RectTransform>(0).localScale = Vector3.zero;
                    iTween.ScaleTo(moddedObjectMain.GetObjectFromList<RectTransform>(0).gameObject, new Vector3(1.3f, 1.3f, 1.3f), 0.4f);

                    Background.color = Color.black;
                    Utilities.Coroutines.LerpImageColor(Background, new Color(0.75f, 0.75f, 0.75f, 1), 0.5f);
                    RefreshLevels();

                    LevelLoadStateUI.gameObject.SetActive(false);
                    LevelLoadProgressSlider.gameObject.SetActive(false);
                    moddedObjectMain.GetObjectFromList<RectTransform>(0).gameObject.SetActive(true);
                    moddedObjectMain.GetObjectFromList<RectTransform>(7).gameObject.SetActive(true);
                    moddedObjectMain.GetObjectFromList<Button>(7).GetComponent<Button>().onClick.AddListener(ShowCode);

                    moddedObjectMain.GetObjectFromList<Text>(6).text = MultiplayerMatchmakingManager.Instance.GetLastInviteCode();
                }
            }

            private void ShowCode()
            {
                moddedObjectMain.GetObjectFromList<RectTransform>(7).gameObject.SetActive(false);
            }

            public void Update()
            {
                if (!gameObject.activeInHierarchy)
                {
                    return;
                }
                MultiplayerConnectionState connectionState = BoltGlobalEventListenerSingleton<MultiplayerMatchmakingManager>.Instance.GetConnectionState();
                if (connectionState == MultiplayerConnectionState.Connected)
                {
                    if (BoltGlobalEventListenerSingleton<BattleRoyaleCustomGameManager>.Instance.IsUploadingLevel())
                    {
                        int uploadingLevelNum = BoltGlobalEventListenerSingleton<BattleRoyaleCustomGameManager>.Instance.GetUploadingLevelNum();
                        int uploadingLevelCount = BoltGlobalEventListenerSingleton<BattleRoyaleCustomGameManager>.Instance.GetUploadingLevelCount();
                        LevelLoadProgressSlider.maxValue = uploadingLevelCount;
                        LevelLoadProgressSlider.value = uploadingLevelNum + 1;
                        LevelLoadStateText.text = string.Concat(new object[]
                        {
                                 LocalizationManager.Instance.GetTranslatedString("Uploading Level", -1),
                                 " ",
                                 uploadingLevelNum,
                                 " / ",
                                 uploadingLevelCount
                        });
                    }
                    else
                    {
                        LevelLoadStateText.text = LocalizationManager.Instance.GetTranslatedString("Connected!", -1);
                    }
                }
            }

            public void OnStartGameClicked()
            {
                moddedObjectMain.GetObjectFromList<RectTransform>(13).gameObject.SetActive(true);

                StaticCoroutineRunner.StartStaticCoroutine(startLoadingLevelsCoroutine());
            }
            private IEnumerator startLoadingLevelsCoroutine()
            {
                yield return new WaitForSecondsRealtime(0.1f);

                moddedObjectMain.GetObjectFromList<RectTransform>(0).gameObject.SetActive(false);
                LevelLoadStateUI.gameObject.SetActive(true);
                LevelLoadProgressSlider.gameObject.SetActive(true);
                List<LevelDescription> list = new List<LevelDescription>();
                foreach (BattleRoyaleLevelDisplay disp in displays)
                {
                    if (disp.GetComponent<Toggle>().isOn)
                    {
                        list.Add(disp.LevelDescription);
                    }
                }
                BattleRoyaleCustomGameManager.Instance.StartPrivateGame(list, moddedObjectMain.GetObjectFromList<Toggle>(1).isOn);

                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();

                moddedObjectMain.GetObjectFromList<RectTransform>(13).gameObject.SetActive(false);
                Utilities.Coroutines.LerpImageColor(Background, new Color(0.5f, 0.5f, 0.5f, 1), 0.5f);

                yield return new WaitUntil(() => CharacterTracker.Instance.GetPlayer() != null);
                Utilities.Coroutines.LerpImageColor(Background, new Color(0.5f, 0.5f, 0.5f, 0), 0.5f);
                iTween.ScaleTo(moddedObjectMain.GetObjectFromList<RectTransform>(0).gameObject, Vector3.zero, 0.4f);
                yield return new WaitForSecondsRealtime(0.7f);
                SetActive(false);

                yield break;
            }

                public void RefreshLevels()
            {
                if (Time.unscaledTime < timeToAllowNextRefresh)
                {
                    return;
                }
                TransformUtils.DestroyAllChildren(Container);
                StaticCoroutineRunner.StartStaticCoroutine(refreshLevels());
                timeToAllowNextRefresh = Time.unscaledTime + 5;
            }

            private IEnumerator refreshLevels()
            {
                displays.Clear();
                List<LevelDescription> allWorkShopBattleRoyaleLevels = Singleton<WorkshopLevelManager>.Instance.GetAllWorkShopBattleRoyaleLevels();
                foreach (LevelDescription desc in allWorkShopBattleRoyaleLevels)
                {
                    yield return new WaitForEndOfFrame();
                    BattleRoyaleLevelDisplay disp = Instantiate<RectTransform>(moddedObjectMain.GetObjectFromList<RectTransform>(4), Container).gameObject.AddComponent<BattleRoyaleLevelDisplay>().SetUP(desc, SelectAllToggle);
                    disp.gameObject.SetActive(true);
                    displays.Add(disp);
                }
                yield break;
            }

            private void OpenLBSWorkshop()
            {
                BaseUtils.OpenURL("https://steamcommunity.com/workshop/browse/?appid=597170&requiredtags[]=Last+Bot+Standing+Level");
            }

            public class BattleRoyaleLevelDisplay : MonoBehaviour
            {
                public LevelDescription LevelDescription;
                public BattleRoyaleLevelDisplay SetUP(LevelDescription desc, BetterToggleGroup group)
                {
                    ModdedObject mObj = base.GetComponent<ModdedObject>();
                    LevelDescription = desc;
                    mObj.GetObjectFromList<Text>(1).text = desc.WorkshopItem.Title;
                    mObj.GetObjectFromList<Text>(2).text = "By: " + desc.WorkshopItem.CreatorName;
                    mObj.GetObjectFromList<Button>(3).onClick.AddListener(onDetailsClicked);
                    Utilities.Coroutines.LoadWorkshopImage(desc.WorkshopItem.PreviewURL, delegate (Sprite sp)
                    {
                        mObj.GetObjectFromList<Image>(0).sprite = sp;
                    });

                    Toggle toggle = base.GetComponent<Toggle>();
                    BetterToggleGroupEntry entry = toggle.gameObject.AddComponent<BetterToggleGroupEntry>().SetUp(group, toggle, true);
                    return this;
                }

                private void onDetailsClicked()
                {
                    BaseUtils.OpenURL("http://steamcommunity.com/sharedfiles/filedetails/?source=CloneDroneGame&id=" + LevelDescription.WorkshopItem.WorkshopItemID);
                }
            }
        }

        internal class DuelUI
        {
            public GameObject gameObject;
            private ModdedObject moddedObjectToTranslate;
            private ModdedObject moddedObjectMain;
            private BetterSlider StartSkillpoints;
            private BetterSlider SkillpointsPerDeath;
            private BetterSlider Clones;
            private Button StartButton;
            private Button ShowCodeButton;
            private Button CodeButton;
            private RectTransform Settings;
            private ModdedObject AdditParams;
            private Text ParamsText;


            public DuelUI(ModdedObject mObj)
            {
                gameObject = mObj.gameObject;
                gameObject.SetActive(false);

                foreach (ModdedObject mObj2 in gameObject.GetComponents<ModdedObject>())
                {
                    if (mObj2.ID == "ToTranslate")
                    {
                        moddedObjectToTranslate = mObj2;
                    }
                    else if (mObj2.ID == "Main")
                    {
                        moddedObjectMain = mObj2;
                    }
                }


                StartSkillpoints = moddedObjectMain.GetObjectFromList<ModdedObject>(0).AddAndConfigBetterSlider(new BetterSlider.Settings
                {
                    MinValue = 0,
                    MaxValue = 30,
                    UseInt = true
                });

                SkillpointsPerDeath = moddedObjectMain.GetObjectFromList<ModdedObject>(1).AddAndConfigBetterSlider(new BetterSlider.Settings
                {
                    MinValue = 0,
                    MaxValue = 30,
                    UseInt = true
                });

                Clones = moddedObjectMain.GetObjectFromList<ModdedObject>(2).AddAndConfigBetterSlider(new BetterSlider.Settings
                {
                    MinValue = 0,
                    MaxValue = 30, // A warning about 15+ clones
                    UseInt = true
                });

                AdditParams = moddedObjectMain.GetObjectFromList<ModdedObject>(8);
                ParamsText = AdditParams.GetObjectFromList<Text>(0);
                Settings = moddedObjectMain.GetObjectFromList<RectTransform>(7);

                StartButton = moddedObjectMain.GetObjectFromList<Button>(6);
                StartButton.onClick.AddListener(new UnityEngine.Events.UnityAction(OnStartGameClicked));

                ShowCodeButton = moddedObjectMain.GetObjectFromList<Button>(5);
                ShowCodeButton.onClick.AddListener(new UnityEngine.Events.UnityAction(OnShowCodeClicked));
                CodeButton = moddedObjectMain.GetObjectFromList<Button>(4);
                CodeButton.onClick.AddListener(new UnityEngine.Events.UnityAction(CopyCode));
            }

            public void SetActive(bool val, bool onlyChangeVisibility = false)
            {
                gameObject.SetActive(val);
                if (onlyChangeVisibility)
                {
                    return;
                }

                if (val)
                {
                    StartSkillpoints.SetValue(Multiplayer1v1CustomGameManager.Instance.GetSavedStartSkillPoints());
                    SkillpointsPerDeath.SetValue(Multiplayer1v1CustomGameManager.Instance.GetSavedSkillPointsPerDeath());
                    Clones.SetValue(Multiplayer1v1CustomGameManager.Instance.GetSavedStartClones());
                    Settings.gameObject.SetActive(true);
                    AdditParams.gameObject.SetActive(false);
                }
            }

            private void OnStartGameClicked()
            {
                ClientCustomize1v1GameEvent clientCustomize1v1GameEvent = ClientCustomize1v1GameEvent.Create(Bolt.GlobalTargets.OnlyServer, Bolt.ReliabilityModes.ReliableOrdered);
                clientCustomize1v1GameEvent.StartingSkillPoints = Mathf.RoundToInt(StartSkillpoints.SliderValue);
                clientCustomize1v1GameEvent.SkillPointsPerDeath = Mathf.RoundToInt(SkillpointsPerDeath.SliderValue);
                clientCustomize1v1GameEvent.StartClones = Mathf.RoundToInt(Clones.SliderValue);
                clientCustomize1v1GameEvent.Send();

                Multiplayer1v1CustomGameManager.Instance.SetSavedStartClones((int)Clones.SliderValue);
                Multiplayer1v1CustomGameManager.Instance.SetSavedStartSkillPoints((int)StartSkillpoints.SliderValue);
                Multiplayer1v1CustomGameManager.Instance.SetSkillPointsPerDeath((int)SkillpointsPerDeath.SliderValue);

                Settings.gameObject.SetActive(false);
                AdditParams.gameObject.SetActive(true);

                string codePart = string.Empty;
                if (OverhaulMain.GetSetting<bool>("Misc.Privacy.Show duel room code"))
                {
                    codePart = System.Environment.NewLine + "Code: " + MultiplayerMatchmakingManager.Instance.GetLastInviteCode();
                }
                else
                {
                    moddedObjectMain.GetObjectFromList<RectTransform>(9).gameObject.SetActive(false);
                }

                ParamsText.text = "Duel settings: " + System.Environment.NewLine + StartSkillpoints.SliderValue + "-" + SkillpointsPerDeath.SliderValue + "-" + Clones.SliderValue + codePart;
            }

            private void OnShowCodeClicked()
            {
                ShowCodeButton.gameObject.SetActive(false);
                CodeButton.GetComponent<Text>().text = MultiplayerMatchmakingManager.Instance.GetLastInviteCode();
            }

            private void CopyCode()
            {
                BaseUtils.CopyToClipboard(MultiplayerMatchmakingManager.Instance.GetLastInviteCode(), true, "Code ", " was copied to clipboard!");
            }

        }

        internal class CoopUI
        {
            private ModdedObject toTranslateMObj;
            private ModdedObject mainModdedObj;
            private BetterSlider StartSkillPointsSlider;
            private Dropdown TiersDropdown;
            private Toggle FriendlyFireToggle;
            private RectTransform StartTierRectT;
            private Button StartGameButton;
            private Image Background;

            private GameObject gameObject;

            public CoopUI(ModdedObject mObj)
            {
                Background = mObj.GetComponent<Image>();
                gameObject = mObj.gameObject;
                foreach (ModdedObject mObj2 in mObj.gameObject.GetComponents<ModdedObject>())
                {
                    if (mObj2.ID == "ToTranslate")
                    {
                        toTranslateMObj = mObj2;
                    }
                    else if (mObj2.ID == "Main")
                    {
                        mainModdedObj = mObj2;
                    }
                }

                StartTierRectT = mainModdedObj.GetObjectFromList<RectTransform>(7);

                StartSkillPointsSlider = mainModdedObj.GetObjectFromList<ModdedObject>(0).AddAndConfigBetterSlider(new BetterSlider.Settings
                {
                    MinValue = 0,
                    MaxValue = 30,
                    UseInt = true
                });
                TiersDropdown = mainModdedObj.GetObjectFromList<Dropdown>(1);
                FriendlyFireToggle = mainModdedObj.GetObjectFromList<Toggle>(2);
                FriendlyFireToggle.onValueChanged.AddListener(refreshToggleValues);

                StartGameButton = mainModdedObj.GetObjectFromList<Button>(6);
                StartGameButton.onClick.AddListener(OnStartGameClicked);

                mainModdedObj.GetObjectFromList<Button>(4).onClick.AddListener(CopyCode);
                mainModdedObj.GetObjectFromList<Button>(5).onClick.AddListener(OnShowCodeClicked);

                SetActive(false);
            }

            public void SetActive(bool val, bool isCoopChallenge = false)
            {
                Background.sprite = MultiplayerInviteUIs.Instance.CoopEndlessImage;
                if (isCoopChallenge)
                {
                    Background.sprite = MultiplayerInviteUIs.Instance.CoopChallengeImage;
                }
                Background.color = Color.black;
                Utilities.Coroutines.LerpImageColor(Background, new Color(1, 1, 1, 1), 0.5f);

                gameObject.SetActive(val);
                if (val)
                {
                    mainModdedObj.GetObjectFromList<RectTransform>(9).localScale = Vector3.zero;
                    iTween.ScaleTo(mainModdedObj.GetObjectFromList<RectTransform>(9).gameObject, Vector3.one, 0.4f);
                    TiersDropdown.options = EndlessModeManager.Instance.GetDifficultyTierDropdownOptions();

                    CoopCustomGameSettings savedHostSettings = CoopCustomGameManager.Instance.GetSavedHostSettings();
                    TiersDropdown.value = EndlessModeManager.Instance.GetDifficultyIndex(savedHostSettings.StartingTier);
                    FriendlyFireToggle.isOn = savedHostSettings.FriendlyFire;
                    StartSkillPointsSlider.SetValue(savedHostSettings.StartSkillPoints);
                    refreshToggleValues(savedHostSettings.FriendlyFire);

                    if (GameModeManager.IsCoopChallenge())
                    {
                        FriendlyFireToggle.isOn = SettingsManager.Instance.GetCustomCoopChallengesUseFriendlyFire();
                    }

                    StartTierRectT.gameObject.SetActive(!isCoopChallenge);
                    StartSkillPointsSlider.gameObject.SetActive(!isCoopChallenge);
                }
            }

            public void OnStartGameClicked()
            {
                CoopCustomGameSettings coopCustomGameSettings = new CoopCustomGameSettings();
                if (GameModeManager.IsCoopChallenge())
                {
                    coopCustomGameSettings.FriendlyFire = FriendlyFireToggle.isOn;
                    SettingsManager.Instance.SetCustomCoopChallengesUseFriendlyFire(coopCustomGameSettings.FriendlyFire);
                }
                else
                {
                    coopCustomGameSettings.FriendlyFire = FriendlyFireToggle.isOn;
                    coopCustomGameSettings.StartingTier = Singleton<EndlessModeManager>.Instance.GetDifficultyTier(TiersDropdown.value);
                    coopCustomGameSettings.StartSkillPoints = (int)StartSkillPointsSlider.SliderValue;
                    Singleton<CoopCustomGameManager>.Instance.SaveHostSettings(coopCustomGameSettings);
                }
                ClientCustomizeCoopGameEvent clientCustomizeCoopGameEvent = ClientCustomizeCoopGameEvent.Create(Bolt.GlobalTargets.OnlyServer, Bolt.ReliabilityModes.ReliableOrdered);
                coopCustomGameSettings.WriteValuesTo(clientCustomizeCoopGameEvent);
                clientCustomizeCoopGameEvent.Send();

                mainModdedObj.GetObjectFromList<RectTransform>(9).localScale = Vector3.one;
                iTween.ScaleTo(mainModdedObj.GetObjectFromList<RectTransform>(9).gameObject, Vector3.zero, 0.2f);
                Utilities.Coroutines.LerpImageColor(Background, Color.clear, 0.25f, Hide);
            }

            private void Hide()
            {
                SetActive(false);
            }

            private void refreshToggleValues(bool val)
            {
                mainModdedObj.GetObjectFromList<Text>(3).text = val ? "On" : "Off";
            }

            private void OnShowCodeClicked()
            {
                mainModdedObj.GetObjectFromList<Text>(4).text = MultiplayerMatchmakingManager.Instance.GetLastInviteCode();
                mainModdedObj.GetObjectFromList<Button>(5).gameObject.SetActive(false);
            }

            private void CopyCode()
            {
                BaseUtils.CopyToClipboard(MultiplayerMatchmakingManager.Instance.GetLastInviteCode(), true, "Code ", " was copied to clipboard!");
            }
        }
    }
}
