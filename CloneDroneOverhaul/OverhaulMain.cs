using CloneDroneOverhaul.Gameplay.Levels;
using CloneDroneOverhaul.LevelEditor;
using CloneDroneOverhaul.Localization;
using CloneDroneOverhaul.Modules;
using CloneDroneOverhaul.UI;
using CloneDroneOverhaul.UI.Notifications;
using CloneDroneOverhaul.Utilities;
using CloneDroneOverhaul.WeaponSkins;
using LevelEditorTools;
using ModBotWebsiteAPI;
using ModLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using CloneDroneOverhaul.Gameplay.OverModes;

namespace CloneDroneOverhaul
{
    [MainModClass]
    public class OverhaulMain : Mod
    {
        // Token: 0x17000004 RID: 4
        // (get) Token: 0x0600002E RID: 46 RVA: 0x00004390 File Offset: 0x00002590
        // (set) Token: 0x0600002F RID: 47 RVA: 0x00004398 File Offset: 0x00002598
        public bool IsModInitialized { get; private set; }

        // Token: 0x17000005 RID: 5
        // (get) Token: 0x06000030 RID: 48 RVA: 0x000043A1 File Offset: 0x000025A1
        // (set) Token: 0x06000031 RID: 49 RVA: 0x000043A8 File Offset: 0x000025A8
        public static OverhaulMain Instance { get; internal set; }

        // Token: 0x17000006 RID: 6
        // (get) Token: 0x06000032 RID: 50 RVA: 0x000043B0 File Offset: 0x000025B0
        // (set) Token: 0x06000033 RID: 51 RVA: 0x000043B7 File Offset: 0x000025B7
        private static OverhaulLocalizationManager Localization { get; set; }

        // Token: 0x17000007 RID: 7
        // (get) Token: 0x06000034 RID: 52 RVA: 0x000043BF File Offset: 0x000025BF
        // (set) Token: 0x06000035 RID: 53 RVA: 0x000043C6 File Offset: 0x000025C6
        public static DelegateTimer Timer { get; set; }

        // Token: 0x17000008 RID: 8
        // (get) Token: 0x06000036 RID: 54 RVA: 0x000043CE File Offset: 0x000025CE
        // (set) Token: 0x06000037 RID: 55 RVA: 0x000043D5 File Offset: 0x000025D5
        public static VisualsModule Visuals { get; set; }

        // Token: 0x17000009 RID: 9
        // (get) Token: 0x06000038 RID: 56 RVA: 0x000043DD File Offset: 0x000025DD
        // (set) Token: 0x06000039 RID: 57 RVA: 0x000043E4 File Offset: 0x000025E4
        public static ModuleManagement Modules { get; set; }

        // Token: 0x1700000A RID: 10
        // (get) Token: 0x0600003A RID: 58 RVA: 0x000043EC File Offset: 0x000025EC
        // (set) Token: 0x0600003B RID: 59 RVA: 0x000043F3 File Offset: 0x000025F3
        public static OverhaulMainMonoBehaviour MainMonoBehaviour { get; set; }

        // Token: 0x0600003C RID: 60 RVA: 0x000043FB File Offset: 0x000025FB
        protected override void OnModLoaded()
        {
            OverModesController.InitializeForCurrentScene();
            OverhaulCacheManager.ClearTemporal();
            if (OverhaulMain.Instance != null)
            {
                return;
            }
            this.InitializeOverhaul();
        }

        // Token: 0x0600003D RID: 61 RVA: 0x00004418 File Offset: 0x00002618
        private void InitializeOverhaul()
        {
            OverhaulMain.Instance = this;
            BaseStaticValues.IsModEnabled = true;
            if (!OverhaulMain.hasCachedStuff)
            {
                this.rememberVanillaPreferences();
                OverhaulCacheManager.CacheStuff();
                OverhaulMain.hasCachedStuff = true;
            }
            this.addReferences();
            this.addModules();
            this.addListeners();
            this.spawnGUI();
            this.fixVanillaStuff();
            this.IsModInitialized = true;
            this.finalPreparations();
            this.checkforUpdate();
        }

        // Token: 0x0600003E RID: 62 RVA: 0x0000447A File Offset: 0x0000267A
        protected override void OnModDeactivated()
        {
            BaseStaticValues.IsModEnabled = false;
        }

        // Token: 0x0600003F RID: 63 RVA: 0x00004482 File Offset: 0x00002682
        protected override UnityEngine.Object OnResourcesLoad(string path)
        {
            return LevelEditorCustomObjectsManager.TryGetObject(path);
        }

        // Token: 0x06000040 RID: 64 RVA: 0x00004496 File Offset: 0x00002696
        protected override void OnLanguageChanged(string newLanguageID, Dictionary<string, string> localizationDictionary)
        {
            OverhaulMain.Modules.ExecuteFunction("onLanguageChanged", null);
            this._settingsButtonText.text = OverhaulMain.GetTranslatedString("OverhaulSettings");
        }

        // Token: 0x06000041 RID: 65 RVA: 0x000044BD File Offset: 0x000026BD
        protected override void OnLevelEditorStarted()
        {
            LevelEditorCustomObjectsManager.OnLevelEditorStarted();
            OverhaulMain.Modules.ExecuteFunction("onLevelEditorStarted", null);
        }

        // Token: 0x06000042 RID: 66 RVA: 0x000044D4 File Offset: 0x000026D4
        protected override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover)
        {
            OverhaulMain.Modules.ExecuteFunction("firstPersonMover.OnSpawn", new object[]
            {
                firstPersonMover.GetRobotInfo()
            });
        }

        // Token: 0x06000043 RID: 67 RVA: 0x000044F4 File Offset: 0x000026F4
        private void rememberVanillaPreferences()
        {
            VanillaPrefs.RememberStuff();
        }

        // Token: 0x06000044 RID: 68 RVA: 0x000044FB File Offset: 0x000026FB
        private void checkforUpdate()
        {
            if (OverhaulMain.hasCheckForUpdates)
            {
                return;
            }
            OverhaulMain.hasCheckForUpdates = true;
            UpdateChecker.CheckForUpdates(new Action<string>(this.OnUpdateReceivedGitHub));
            API.GetModData("rAnDomPaTcHeS1", new Action<JsonObject>(this.OnModDataGet));
        }

        // Token: 0x06000045 RID: 69 RVA: 0x00004534 File Offset: 0x00002734
        private void OnModDataGet(JsonObject json)
        {
            string a = json["Version"].ToString();
            if (a != base.ModInfo.Version.ToString())
            {
                Notification notification = new Notification();
                notification.SetUp("New update available!", "See CDO mod page", 20f, Vector2.zero, Color.clear, new Notification.NotificationButton[]
                {
                    new Notification.NotificationButton
                    {
                        Action = new UnityAction(notification.HideThis),
                        Text = "OK"
                    },
                    new Notification.NotificationButton
                    {
                        Action = new UnityAction(UpdateChecker.OpenModBotPage),
                        Text = "ModBot"
                    }
                }, false);
            }
        }

        // Token: 0x06000046 RID: 70 RVA: 0x000045E8 File Offset: 0x000027E8
        private void OnUpdateReceivedGitHub(string newVersion)
        {
            if (newVersion == OverhaulDescription.GetModVersion(false))
            {
                return;
            }
            Notification notification = new Notification();
            notification.SetUp("New update available!", "New version (" + newVersion + ") is available on GitHub!", 20f, Vector2.zero, Color.clear, new Notification.NotificationButton[]
            {
                new Notification.NotificationButton
                {
                    Action = new UnityAction(notification.HideThis),
                    Text = "OK"
                },
                new Notification.NotificationButton
                {
                    Action = new UnityAction(UpdateChecker.OpenGitHubWithReleases),
                    Text = "GitHub"
                }
            }, false);
        }

        // Token: 0x06000047 RID: 71 RVA: 0x00004688 File Offset: 0x00002888
        private void addReferences()
        {
            BaseStaticReferences.ModuleManager = new ModuleManagement();
        }

        // Token: 0x06000048 RID: 72 RVA: 0x00004694 File Offset: 0x00002894
        private void addModules()
        {
            ModuleManagement moduleManagement = new ModuleManagement();
            BaseStaticReferences.ModuleManager = moduleManagement;
            OverhaulMain.Timer = moduleManagement.AddModule<DelegateTimer>(false);
            moduleManagement.AddModule<ModDataManager>(false);
            new CloneDroneOverhaulDataContainer();
            OverhaulMain.Modules = moduleManagement;
            moduleManagement.AddModule<OverhaulSettingsManager>(false);
            OverhaulMain.Localization = moduleManagement.AddModule<OverhaulLocalizationManager>(false);
            OverhaulMain.Visuals = moduleManagement.AddModule<VisualsModule>(false);
            moduleManagement.AddModule<HotkeysModule>(false);
            moduleManagement.AddModule<GUIManagement>(false);
            moduleManagement.AddModule<WeaponSkinManager>(false);
            moduleManagement.AddModule<WorldGUIs>(false);
            moduleManagement.AddModule<GameplayOverhaulModule>(false);
            moduleManagement.AddModule<MultiplayerManager>(false);
            moduleManagement.AddModule<ArenaManager>(false);
            moduleManagement.AddModule<MiscEffectsManager>(false);
            moduleManagement.AddModule<ModdedLevelEditorManager>(false);
            moduleManagement.AddModule<ExplorationGameModeManager>(false);
            moduleManagement.AddModule<PatchesManager>(false);
            moduleManagement.AddModule<AdvancedPhotoModeManager>(false);
            moduleManagement.AddModule<GarbagePositionerManager>(false);
            moduleManagement.AddModule<GameInformationManager>(false);
            moduleManagement.AddModule<GameStateChangeController>(false);
        }

        // Token: 0x06000049 RID: 73 RVA: 0x00004768 File Offset: 0x00002968
        private void addListeners()
        {
            GameObject gameObject = new GameObject("CDOListeners");
            OverhaulMain.MainMonoBehaviour = gameObject.AddComponent<OverhaulMainMonoBehaviour>();
        }

        // Token: 0x0600004A RID: 74 RVA: 0x0000478C File Offset: 0x0000298C
        private void finalPreparations()
        {
            if (OverhaulDescription.IsBetaBuild())
            {
                BaseStaticReferences.ModuleManager.GetModule<HotkeysModule>().AddHotkey(new Hotkey
                {
                    Key2 = KeyCode.C,
                    Key1 = KeyCode.LeftControl,
                    Method = new Action(BaseUtils.DebugFireSword)
                });
                BaseStaticReferences.ModuleManager.GetModule<HotkeysModule>().AddHotkey(new Hotkey
                {
                    Key2 = KeyCode.B,
                    Key1 = KeyCode.LeftControl,
                    Method = new Action(BaseUtils.ExplodePlayer)
                });
                BaseStaticReferences.ModuleManager.GetModule<HotkeysModule>().AddHotkey(new Hotkey
                {
                    Key2 = KeyCode.V,
                    Key1 = KeyCode.LeftControl,
                    Method = new Action(BaseUtils.AddSkillPoint)
                });
                HotkeysModule module = BaseStaticReferences.ModuleManager.GetModule<HotkeysModule>();
                Hotkey hotkey = new Hotkey();
                hotkey.Key2 = KeyCode.M;
                hotkey.Key1 = KeyCode.LeftControl;
                hotkey.Method = delegate ()
                {
                    LevelConstructor.BuildALevel(new LevelConstructor.LevelSettings(), true);
                };
                module.AddHotkey(hotkey);
                BaseStaticReferences.ModuleManager.GetModule<HotkeysModule>().AddHotkey(new Hotkey
                {
                    Key2 = KeyCode.X,
                    Key1 = KeyCode.LeftControl,
                    Method = new Action(BaseUtils.DebugSize)
                });
            }
            BaseStaticReferences.ModuleManager.GetModule<HotkeysModule>().AddHotkey(new Hotkey
            {
                Key1 = KeyCode.F2,
                Method = new Action(MiscEffectsManager.SwitchHud)
            });
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
            QualitySettings.softParticles = true;
            QualitySettings.streamingMipmapsMemoryBudget = 4096f;
            QualitySettings.streamingMipmapsMaxLevelReduction = 6;
            QualitySettings.streamingMipmapsMaxFileIORequests = 4096;
            QualitySettings.asyncUploadBufferSize = 16;
            QualitySettings.asyncUploadTimeSlice = 4;
            QualitySettings.asyncUploadPersistentBuffer = true;
            QualitySettings.antiAliasing = 8;
            QualitySettings.shadowCascades = 2;
            Singleton<SkyBoxManager>.Instance.LevelConfigurableSkyboxes[8].SetColor("_Tint", new Color(0.6f, 0.73f, 2f, 1f));
            Singleton<AttackManager>.Instance.HitColor = new Color(4f, 0.65f, 0.35f, 0.2f);
            Singleton<AttackManager>.Instance.BodyOnFireColor = new Color(1f, 0.42f, 0.22f, 0.1f);
            Singleton<EmoteManager>.Instance.PitchLimits.Max = 5f;
            Singleton<EmoteManager>.Instance.PitchLimits.Min = 0f;
            Transform transform = TransformUtils.FindChildRecursive(Singleton<GameUIRoot>.Instance.TitleScreenUI.transform, "BottomButtons");
            transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
            transform.localPosition = new Vector3(0f, -180f, 0f);
            if (TransformUtils.FindChildRecursive(Singleton<GameUIRoot>.Instance.TitleScreenUI.transform, "OptionsButton"))
            {
                Transform transform2 = UnityEngine.Object.Instantiate<Transform>(TransformUtils.FindChildRecursive(Singleton<GameUIRoot>.Instance.TitleScreenUI.transform, "OptionsButton"), transform);
                transform2.SetSiblingIndex(1);
                UnityEngine.Object.Destroy(transform2.GetComponentInChildren<LocalizedTextField>());
                transform2.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
                transform2.GetComponent<Button>().onClick.AddListener(new UnityAction(SettingsUI.Instance.Show));
                transform2.GetComponentInChildren<Text>().text = "Overhaul Settings";
                this._settingsButtonText = transform2.GetComponentInChildren<Text>();
            }
            foreach (Image image in Singleton<GameUIRoot>.Instance.GetComponentsInChildren<Image>(true))
            {
                if (image != null && image.sprite != null)
                {
                    if (image.sprite.name == "UISprite" || image.sprite.name == "Knob")
                    {
                        image.sprite = AssetLoader.GetObjectFromFile<Sprite>("cdo_rw_stuff", "CanvasRoundedUnityDarkEdge");
                    }
                    if (image.sprite.name == "Checkmark")
                    {
                        image.sprite = AssetLoader.GetObjectFromFile<Sprite>("cdo_rw_stuff", "CheckmarkSmall");
                        image.color = Color.black;
                    }
                    if (image.sprite.name == "Background")
                    {
                        image.sprite = AssetLoader.GetObjectFromFile<Sprite>("cdo_rw_stuff", "CanvasRoundedUnity");
                    }
                    Outline component = image.GetComponent<Outline>();
                    if (component != null)
                    {
                        component.enabled = false;
                    }
                }
            }
            new GameObject("CDO_RW_BoltEventListener").AddComponent<BoltEventListener>();
        }

        // Token: 0x0600004B RID: 75 RVA: 0x00004C04 File Offset: 0x00002E04
        private void fixVanillaStuff()
        {
            Singleton<MultiplayerCharacterCustomizationManager>.Instance.CharacterModels[17].UnlockedByAchievementID = string.Empty;
            PatchesManager.Instance.UpdateAudioSettings(OverhaulMain.GetSetting<bool>("Patches.QoL.Fix sounds"));
            foreach (Camera camera in GameInformationManager.UnoptimizedThings.GetFPSLoweringStuff().AllCameras)
            {
                if (camera.name == "ArenaCamera")
                {
                    camera.pixelRect = new Rect(new Vector2(0f, 0f), new Vector2(640f, 360f));
                }
            }
        }

        // Token: 0x0600004C RID: 76 RVA: 0x00004C9C File Offset: 0x00002E9C
        private void spawnGUI()
        {
            GUIManagement module = BaseStaticReferences.ModuleManager.GetModule<GUIManagement>();
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "CDO_RW_UI"));
            gameObject.name = "CloneDroneOverhaulUI";
            module.AddGUI(gameObject.GetComponent<ModdedObject>().GetObjectFromList<Transform>(0).gameObject.AddComponent<Watermark>());
            module.AddGUI(gameObject.GetComponent<ModdedObject>().GetObjectFromList<Transform>(9).gameObject.AddComponent<SettingsUI>());
            module.AddGUI(gameObject.GetComponent<ModdedObject>().GetObjectFromList<Transform>(2).gameObject.AddComponent<OverhaulLocalizationEditor>());
            module.AddGUI(gameObject.GetComponent<ModdedObject>().GetObjectFromList<Transform>(1).gameObject.AddComponent<NewErrorWindow>());
            module.AddGUI(gameObject.GetComponent<ModdedObject>().GetObjectFromList<Transform>(4).gameObject.AddComponent<BackupMindTransfersUI>());
            module.AddGUI(gameObject.GetComponent<ModdedObject>().GetObjectFromList<Transform>(3).gameObject.AddComponent<NotificationsUI>());
            module.AddGUI(gameObject.GetComponent<ModdedObject>().GetObjectFromList<Transform>(5).gameObject.AddComponent<NewEscMenu>());
            module.AddGUI(gameObject.GetComponent<ModdedObject>().GetObjectFromList<Transform>(8).gameObject.AddComponent<MultiplayerInviteUIs>());
            module.AddGUI(gameObject.GetComponent<ModdedObject>().GetObjectFromList<Transform>(10).gameObject.AddComponent<ModdedLevelEditorUI>());
            module.AddGUI(gameObject.GetComponent<ModdedObject>().GetObjectFromList<Transform>(11).gameObject.AddComponent<MultiplayerUIs>());
            module.AddGUI(gameObject.GetComponent<ModdedObject>().GetObjectFromList<Transform>(12).gameObject.AddComponent<NewKillFeedUI>());
            module.AddGUI(gameObject.GetComponent<ModdedObject>().GetObjectFromList<Transform>(13).gameObject.AddComponent<NewGameModeSelectionScreen>());
            module.AddGUI(gameObject.GetComponent<ModdedObject>().GetObjectFromList<Transform>(14).gameObject.AddComponent<NewPhotoModeUI>());
            module.AddGUI(gameObject.GetComponent<ModdedObject>().GetObjectFromList<Transform>(15).gameObject.AddComponent<NewWorkshopBrowserUI>());
            module.AddGUI(gameObject.GetComponent<ModdedObject>().GetObjectFromList<Transform>(16).gameObject.AddComponent<VisualEffectsUI>());
        }

        // Token: 0x0600004D RID: 77 RVA: 0x00004E80 File Offset: 0x00003080
        public static string GetTranslatedString(string ID)
        {
            TranslationEntry translation = OverhaulMain.Localization.GetTranslation(ID);
            if (translation == null)
            {
                return "NL: " + ID;
            }
            string text = translation.Translations[Singleton<LocalizationManager>.Instance.GetCurrentLanguageCode()];
            if (text == "nontranslated")
            {
                text = translation.Translations["en"];
            }
            return text;
        }

        // Token: 0x0600004E RID: 78 RVA: 0x00004EDD File Offset: 0x000030DD
        public static T GetSetting<T>(string ID)
        {
            return CloneDroneOverhaulDataContainer.Instance.SettingsData.GetSettingValue<T>(ID);
        }

        // Token: 0x0400005D RID: 93
        private static bool hasCheckForUpdates;

        // Token: 0x0400005E RID: 94
        private static bool hasCachedStuff;

        // Token: 0x04000066 RID: 102
        public static OverhaulCacheManager CacheManagerReference = new OverhaulCacheManager();

        // Token: 0x04000067 RID: 103
        private Text _settingsButtonText;
    }

    public static class OverhaulDescription
    {
        public static string GetModFolder()
        {
            return OverhaulMain.Instance.ModInfo.FolderPath;
        }

        public static string GetModName(bool includeVersion, bool shortVariant = false)
        {
            string text = (!shortVariant) ? ("Clone Drone Overhaul " + OverhaulDescription.GetModVersionBranch().ToString()) : "CDO";
            if (includeVersion)
            {
                return text + " " + OverhaulDescription.GetModVersion(true);
            }
            return text;
        }

        public static string GetModVersion(bool withModBotVersion = true)
        {
            string gameVersion = OverhaulDescription.getGameVersion();
            if (!withModBotVersion)
            {
                return gameVersion;
            }
            return gameVersion + " (." + OverhaulMain.Instance.ModInfo.Version.ToString() + ")";
        }

        public static OverhaulDescription.Branch GetModVersionBranch()
        {
            return OverhaulDescription.Branch.Github;
        }

        private static string getGameVersion()
        {
            string result = "a0.2.0.17";
            string result2 = "a0.2.1.1";
            if (!OverhaulDescription.IsBetaBuild())
            {
                return result;
            }
            return result2;
        }

        public static bool IsBetaBuild()
        {
            return true;
        }

        public static bool LevelEditorToolsEnabled()
        {
            return PlayerPrefs.HasKey("286ea03e-b667-46ae-8c12-95eb08c412e4") && PlayerPrefs.GetInt("286ea03e-b667-46ae-8c12-95eb08c412e4") == 1;
        }

        public const string LEVEL_EDITOR_TOOLS_MODID = "286ea03e-b667-46ae-8c12-95eb08c412e4";

        public enum Branch
        {
            Github,
            ModBot
        }
    }

    public class OverhaulMainMonoBehaviour : ManagedBehaviour
    {
        private float timeWhenOneSecondWillPast;
        public static bool IsApplicationFocused { get; private set; }
        public LAN.LANMultiplayerManager LANManager;

        private GameMode _gameModeSetLastUpdate;

        private bool IsReadyToWork()
        {
            return OverhaulMain.Instance.IsModInitialized;
        }

        public override void UpdateMe()
        {
            if (IsReadyToWork())
            {
                BaseStaticReferences.ModuleManager.OnManagedUpdate();

                GameMode newGameMode = GameFlowManager.Instance.GetCurrentGameMode();
                if (newGameMode != _gameModeSetLastUpdate)
                {
                    OverhaulMain.Modules.ExecuteFunction<GameMode>("onGameModeUpdated", newGameMode);
                }
                _gameModeSetLastUpdate = newGameMode;
            }
        }

        private void Update()
        {
            if (Time.time >= timeWhenOneSecondWillPast)
            {
                timeWhenOneSecondWillPast = Time.time + 1;
                OnTimePast(1f);
            }
            if (Time.time >= timeWhenOneSecondWillPast - 0.5f)
            {
                OnTimePast(0.5f);
            }
            if (IsReadyToWork())
            {
                BaseStaticReferences.ModuleManager.OnFrame();
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                Profiler.EndThreadProfiling();
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            OverhaulMainMonoBehaviour.IsApplicationFocused = hasFocus;
        }

        private void OnTimePast(float time)
        {
            if (IsReadyToWork())
            {
                BaseStaticReferences.ModuleManager.OnTime(time);
            }
        }

        private void FixedUpdate()
        {
            if (IsReadyToWork())
            {
                BaseStaticReferences.ModuleManager.OnFixedUpdate();
                EffectsAndAbilitiesV4.RobotsExpansionManager.FixedUpdate();
            }
        }

        private void Start()
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            LANManager = LAN.LANMultiplayerManager.Instance;
        }
        private void OnSceneUnloaded(Scene current)
        {
            OverhaulMain.Instance = null;
        }

        void Awake()
        {
            Singleton<GlobalEventManager>.Instance.AddEventListener(GlobalEvents.BattleRoyaleMatchProgressChanged, delegate
            {
                executeFunction<object>("battleRoyale.MatchProgressUpdated", null);
            });
            Singleton<GlobalEventManager>.Instance.AddEventListener(GlobalEvents.BattleRoyaleTimeToGameStartUpdated, delegate
            {
                executeFunction<object>("battleRoyale.TimeToGameStartUpdated", null);
            });
            Singleton<GlobalEventManager>.Instance.AddEventListener(GlobalEvents.NumMultiplayerPlayersChanged, delegate
            {
                executeFunction<object>("battleRoyale.NumMultiplayerPlayersChanged", null);
            });

            Singleton<GlobalEventManager>.Instance.AddEventListener<Character>("CharacterKilled", delegate (Character charr)
            {
                executeFunction<Character>("battleRoyale.CharacterKilled", charr);
            });
        }

        void executeFunction<T>(string name, T obj)
        {
            if (name == "battleRoyale.MatchProgressUpdated")
            {
                OverhaulMain.Modules.ExecuteFunction(name, new object[]
                {
                    BattleRoyaleManager.Instance != null ? (BattleRoyaleMatchProgress)BattleRoyaleManager.Instance.state.MatchProgress : BattleRoyaleMatchProgress.NotStarted
                });
            }
            else if (name == "battleRoyale.TimeToGameStartUpdated")
            {
                OverhaulMain.Modules.ExecuteFunction(name, new object[]
                {
                    BattleRoyaleManager.Instance != null ? BattleRoyaleManager.Instance.GetSecondsToGameStart() : -1
                });
            }
            else if (name == "battleRoyale.NumMultiplayerPlayersChanged")
            {
                OverhaulMain.Modules.ExecuteFunction(name, new object[]
                {
                    MultiplayerPlayerInfoManager.Instance.GetPlayerCount()
                });
            }
            else if (name == "battleRoyale.CharacterKilled")
            {
                OverhaulMain.Modules.ExecuteFunction(name, new object[]
                {
                    (obj as Character).GetRobotInfo()
                });
            }
        }
    }

    public class BoltEventListener : Bolt.GlobalEventListener
    {
        public override void OnEvent(MatchInstance evnt)
        {
            BaseStaticReferences.ModuleManager.ExecuteFunction<MatchInstance>("Bolt.OnEvent", evnt);
        }
        public override void OnEvent(MultiplayerKillEvent evnt)
        {
            BaseStaticReferences.ModuleManager.ExecuteFunction<MultiplayerKillEvent>("Bolt.OnEvent", evnt);
        }
    }

    public static class CodeExtensions
    {
        public static FlyingCameraController GetCurrentFlyingCameraController(this PhotoManager mgr)
        {
            return mgr.GetPrivateField<FlyingCameraController>("_cameraController");
        }
        public static T Clone<T>(this T obj)
        {
            MethodInfo method = obj.GetType().GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);
            return (T)(object)((method != null) ? method.Invoke(obj, null) : null);
        }

        public static void SetGameMode(this GameFlowManager manager, GameMode gm)
        {
            manager.SetPrivateField<GameMode>("_gameMode", gm);
        }

        public static Color hexToColor(this string hex)
        {
            hex = hex.Replace("0x", "");
            hex = hex.Replace("#", "");
            byte a = 255;
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return new Color32(r, g, b, a);
        }

        public static T GetObjectFromList<T>(this ModdedObject obj, int index) where T : UnityEngine.Object
        {
            GameObject gameObject = obj.objects[index] as GameObject;
            T component = gameObject.GetComponent<T>();
            if (component == null)
            {
                throw new System.NullReferenceException();
            }
            return component;
        }

        public static float GetVoxelSize(this MechBodyPart part)
        {
            bool flag = part.GetPrivateField<float>("_voxelParticleSize") < 0f;
            if (flag)
            {
                bool flag2 = part.GetPrivateField<PicaVoxel.Volume>("_volume") == null;
                if (flag2)
                {
                    return 0.1f;
                }
                bool flag3 = part.ParticleSizeOverride > 0f;
                if (flag3)
                {
                    part.SetPrivateField("_voxelParticleSize", part.ParticleSizeOverride);
                }
                else
                {
                    part.SetPrivateField("_voxelParticleSize", part.GetPrivateField<PicaVoxel.Volume>("_volume").VoxelSize * part.transform.lossyScale.x);
                }
            }
            return part.GetPrivateField<float>("_voxelParticleSize");
        }

        public static bool IsPrivateMatch(this GameModeManager manager)
        {
            bool flag = MultiplayerMatchmakingManager.LastDuelRequest == null;
            bool result;
            if (flag)
            {
                result = false;
            }
            else
            {
                GameRequestType type = MultiplayerMatchmakingManager.LastDuelRequest.GameType;
                result = GameModeManager.IsMultiplayer() && type != GameRequestType.RandomAnyQuickMatch && type != GameRequestType.RandomBattleRoyale && type != GameRequestType.RandomCoopChallenge && type != GameRequestType.RandomDuel && type != GameRequestType.RandomEndlessCoop;
            }
            return result;
        }

    }

    public static class ByteSaver
    {
        public static byte[] ObjectToByteArray(object obj)
        {
            bool flag = obj == null;
            byte[] result;
            if (flag)
            {
                result = null;
            }
            else
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    binaryFormatter.Serialize(memoryStream, obj);
                    result = memoryStream.ToArray();
                }
            }
            return result;
        }

        public static T FromByteArray<T>(byte[] data)
        {
            bool flag = data == null;
            T result;
            if (flag)
            {
                result = default(T);
            }
            else
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream memoryStream = new MemoryStream(data))
                {
                    object obj = binaryFormatter.Deserialize(memoryStream);
                    result = (T)obj;
                }
            }
            return result;
        }
    }

    public static class CrossModManager
    {
        // Token: 0x06000075 RID: 117 RVA: 0x000055D0 File Offset: 0x000037D0
        public static void DoAction(string name, object[] arguments)
        {
            try
            {
                if (name == "ModdedLevelEditor.RefreshSelected")
                {
                    GUIManagement.Instance.GetGUI<ModdedLevelEditorUI>().RefreshSelected((arguments[0] as LevelEditorObjectPlacementManager).GetSelectedSceneObjects());
                }
                if (name == "ModdedLevelEditor.RefreshSelectedLETMod")
                {
                    ModdedLevelEditorUI.ObjectsSelectedPanel objectsSelectedPanel = arguments[0] as ModdedLevelEditorUI.ObjectsSelectedPanel;
                    if (OverhaulDescription.LevelEditorToolsEnabled())
                    {
                        float? rotationAngle = AccurateRotationTool.RotationAngle;
                        Text additionalText = objectsSelectedPanel.AdditionalText;
                        string[] array = new string[7];
                        array[0] = "Level Editor Tools Mod:";
                        array[1] = Environment.NewLine;
                        int num = 2;
                        string[] array2 = new string[5];
                        array2[0] = "R + ";
                        array2[1] = PositionerTool.SetXKey.ToString();
                        array2[2] = " to rotate objects ";
                        int num2 = 3;
                        float? num3 = rotationAngle;
                        array2[num2] = num3.ToString();
                        array2[4] = " degrees along X axis";
                        array[num] = string.Concat(array2);
                        array[3] = Environment.NewLine;
                        int num4 = 4;
                        string[] array3 = new string[5];
                        array3[0] = "R + ";
                        array3[1] = PositionerTool.SetYKey.ToString();
                        array3[2] = " to rotate objects ";
                        int num5 = 3;
                        num3 = rotationAngle;
                        array3[num5] = num3.ToString();
                        array3[4] = " degrees along Y axis";
                        array[num4] = string.Concat(array3);
                        array[5] = Environment.NewLine;
                        int num6 = 6;
                        string[] array4 = new string[5];
                        array4[0] = "R + ";
                        array4[1] = PositionerTool.SetZKey.ToString();
                        array4[2] = " to rotate objects ";
                        int num7 = 3;
                        num3 = rotationAngle;
                        array4[num7] = num3.ToString();
                        array4[4] = " degrees along Z axis";
                        array[num6] = string.Concat(array4);
                        additionalText.text = string.Concat(array);
                    }
                    else
                    {
                        objectsSelectedPanel.AdditionalText.text = "Install/Enable Level editor tools mod for advanced controls";
                    }
                }
            }
            catch
            {
            }
        }
    }

    // Changelog 0.2.0.2
    // Mind transfers panel shows above your head
    // Version watermark no longer overlaps PerformanceStatsPanel
}
