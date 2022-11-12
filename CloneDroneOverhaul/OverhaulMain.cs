using CloneDroneOverhaul.LevelEditor;
using CloneDroneOverhaul.Modules;
using CloneDroneOverhaul.Utilities;
using ModBotWebsiteAPI;
using ModLibrary;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CloneDroneOverhaul
{
    [MainModClass]
    public class OverhaulMain : Mod
    {
        private static bool hasCheckForUpdates;
        private static bool hasCachedStuff;
        public bool IsModInitialized { get; private set; }
        public static OverhaulMain Instance { get; internal set; }
        private static Localization.OverhaulLocalizationManager Localization { get; set; }
        public static DelegateTimer Timer { get; set; }
        public static VisualsModule Visuals { get; set; }
        public static UI.GUIManagement GUI { get; set; }
        public static WeaponSkins.WeaponSkinManager Skins { get; set; }
        public static ModuleManagement Modules { get; set; }
        public static LevelEditor.ModdedLevelEditorManager ModdedEditor { get; set; }
        public static OverhaulMainMonoBehaviour LocalMonoBehaviour { get; set; }

        private Text _settingsButtonText;

        public string GetModFolder() //C:/Program Files (x86)/Steam/steamapps/common/Clone Drone in the Danger Zone/mods/CloneDroneOverhaulRW/
        {
            return ModInfo.FolderPath;
        }

        protected override void OnModLoaded()
        {
            OverhaulCacheManager.ClearTemporal();
            if (OverhaulMain.Instance != null)
            {
                return;
            }

            ThreadLaunch();
        }
        void ThreadLaunch()
        {
            AppDomain.CurrentDomain.Load(File.ReadAllBytes(ModInfo.FolderPath + "netstandard.dll"));
            OverhaulMain.Instance = this;
            BaseStaticValues.IsModEnabled = true;
            //LAN.LANMultiplayerManager.CreateManager();

            if (!hasCachedStuff)
            {
                rememberVanillaPreferences();
                OverhaulCacheManager.CacheStuff();
                hasCachedStuff = true;
            }
            checkDlls();
            addReferences();
            addModules();
            addListeners();
            spawnGUI();
            fixVanillaStuff();

            IsModInitialized = true;

            finalPreparations();
            checkforUpdate();
        }

        protected override void OnModDeactivated()
        {
            BaseStaticValues.IsModEnabled = false;
        }

        /*
        protected override UnityEngine.Object OnResourcesLoad(string path)
        {
            if(path == "Data/LevelEditorLevels/Story5/C5_5_PrisonCellFlashback")
            {
                return new TextAsset(File.ReadAllText(GetModFolder() + "C5_5_PrisonCellFlashback.json"));
            }
            return LevelEditor.LevelEditorCustomObjectsManager.TryGetObject(path);
        }
       */

        protected override void OnLanguageChanged(string newLanguageID, System.Collections.Generic.Dictionary<string, string> localizationDictionary)
        {
            Modules.ExecuteFunction("onLanguageChanged", null);
            _settingsButtonText.text = GetTranslatedString("OverhaulSettings");
        }


        protected override void OnLevelEditorStarted()
        {
            LevelEditorCustomObjectsManager.OnLevelEditorStarted();
            Modules.ExecuteFunction("onLevelEditorStarted", null);
        }
        protected override void OnModRefreshed()
        {
            checkDlls();
        }
        protected override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover)
        {
            OverhaulMain.Modules.ExecuteFunction("firstPersonMover.OnSpawn", new object[] { firstPersonMover.GetRobotInfo() });
        }

        private void rememberVanillaPreferences()
        {
            VanillaPrefs.RememberStuff();
        }

        private void checkforUpdate()
        {
            if (hasCheckForUpdates)
            {
                return;
            }
            hasCheckForUpdates = true;

            if(OverhaulDescription.GetModVersionBranch() != OverhaulDescription.Branch.ModBot)
            {
                UpdateChecker.CheckForUpdates(OnUpdateReceivedGitHub);
            }
            else
            {
                API.GetModData("rAnDomPaTcHeS1", new Action<JsonObject>(OnModDataGet));
            }
        }
        private void OnModDataGet(JsonObject json)
        {
            string ver = json["Version"].ToString();
            if(ver != this.ModInfo.Version.ToString())
            {
                CloneDroneOverhaul.UI.Notifications.Notification notif = new UI.Notifications.Notification();
                notif.SetUp("New update available!", "Version " + ver + " is available to download", 20, Vector2.zero, Color.clear, new UI.Notifications.Notification.NotificationButton[] { new UI.Notifications.Notification.NotificationButton { Action = new UnityEngine.Events.UnityAction(notif.HideThis), Text = "OK" }, new UI.Notifications.Notification.NotificationButton { Action = new UnityEngine.Events.UnityAction(UpdateChecker.OpenModBotPage), Text = "ModBot" } });
            }
        }
        private void OnUpdateReceivedGitHub(string newVersion)
        {
            if (newVersion == OverhaulDescription.GetModVersion(false))
            {
                return;
            }

            CloneDroneOverhaul.UI.Notifications.Notification notif = new UI.Notifications.Notification();
            notif.SetUp("New update available!", "See mod Mod-Bot page", 20, Vector2.zero, Color.clear, new UI.Notifications.Notification.NotificationButton[] { new UI.Notifications.Notification.NotificationButton { Action = new UnityEngine.Events.UnityAction(notif.HideThis), Text = "OK" }, new UI.Notifications.Notification.NotificationButton { Action = new UnityEngine.Events.UnityAction(UpdateChecker.OpenGitHubWithReleases), Text = "GitHub" } });
        }

        private void addReferences()
        {
            BaseStaticReferences.ModuleManager = new ModuleManagement();
        }

        private void addModules()
        {
            ModuleManagement manager = BaseStaticReferences.ModuleManager;
            Timer = manager.AddModule<DelegateTimer>();
            manager.AddModule<ModDataManager>();
            new CloneDroneOverhaulDataContainer();
            Modules = manager;
            manager.AddModule<CloneDroneOverhaul.Modules.OverhaulSettingsManager>();
            Localization = manager.AddModule<Localization.OverhaulLocalizationManager>();
            Visuals = manager.AddModule<VisualsModule>();
            manager.AddModule<HotkeysModule>();
            GUI = manager.AddModule<UI.GUIManagement>();
            BaseStaticReferences.GUIs = GUI;
            Skins = manager.AddModule<WeaponSkins.WeaponSkinManager>();
            manager.AddModule<WorldGUIs>();
            manager.AddModule<RobotsOverhaulModule>();
            manager.AddModule<Modules.MultiplayerManager>();
            manager.AddModule<ArenaManager>();
            manager.AddModule<MiscEffectsManager>();
            ModdedEditor = manager.AddModule<LevelEditor.ModdedLevelEditorManager>();
            manager.AddModule<ExplorationGameModeManager>();
            manager.AddModule<PatchesManager>();
            manager.AddModule<AdvancedPhotoModeManager>();
            manager.AddModule<GarbagePositionerManager>();
            manager.AddModule<GameInformationManager>();
        }

        private void checkDlls()
        {
            return; // Early tests of cross-modding
            foreach (System.Reflection.Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName.Contains("LevelEditorTools,"))
                {
                    OverhaulDescription.IsLETInstalled = true;
                    break;
                }
            }
        }

        private void addListeners()
        {
            UnityEngine.GameObject obj = new UnityEngine.GameObject("CDOListeners");
            LocalMonoBehaviour = obj.AddComponent<OverhaulMainMonoBehaviour>();
        }

        private void finalPreparations()
        {
            if (OverhaulDescription.IsBetaBuild())
            {
                BaseStaticReferences.ModuleManager.GetModule<HotkeysModule>().AddHotkey(new Hotkey
                {
                    Key2 = UnityEngine.KeyCode.C,
                    Key1 = UnityEngine.KeyCode.LeftControl,
                    Method = BaseUtils.DebugFireSword
                });
                BaseStaticReferences.ModuleManager.GetModule<HotkeysModule>().AddHotkey(new Hotkey
                {
                    Key2 = UnityEngine.KeyCode.B,
                    Key1 = UnityEngine.KeyCode.LeftControl,
                    Method = BaseUtils.ExplodePlayer
                });
                BaseStaticReferences.ModuleManager.GetModule<HotkeysModule>().AddHotkey(new Hotkey
                {
                    Key2 = UnityEngine.KeyCode.V,
                    Key1 = UnityEngine.KeyCode.LeftControl,
                    Method = BaseUtils.AddSkillPoint
                });
                BaseStaticReferences.ModuleManager.GetModule<HotkeysModule>().AddHotkey(new Hotkey
                {
                    Key2 = UnityEngine.KeyCode.M,
                    Key1 = UnityEngine.KeyCode.LeftControl,
                    Method = BaseUtils.Console_ShowAppDataPath
                });
                BaseStaticReferences.ModuleManager.GetModule<HotkeysModule>().AddHotkey(new Hotkey
                {
                    Key2 = UnityEngine.KeyCode.X,
                    Key1 = UnityEngine.KeyCode.LeftControl,
                    Method = BaseUtils.DebugSize
                });
            }
            BaseStaticReferences.ModuleManager.GetModule<HotkeysModule>().AddHotkey(new Hotkey
            {
                Key1 = UnityEngine.KeyCode.F2,
                Method = MiscEffectsManager.SwitchHud
            });

            //Setting up some graphics stuff
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

            //Mindspace skybox color
            SkyBoxManager.Instance.LevelConfigurableSkyboxes[8].SetColor("_Tint", new Color(0.6f, 0.73f, 2f, 1f));

            //hit colors
            AttackManager.Instance.HitColor = new Color(4, 0.65f, 0.35f, 0.2f);
            AttackManager.Instance.BodyOnFireColor = new Color(1, 0.42f, 0.22f, 0.1f);

            //Emote pitch limit
            EmoteManager.Instance.PitchLimits.Max = 5f;
            EmoteManager.Instance.PitchLimits.Min = 0f;

            //Timer.AddNoArgActionToCompleteNextFrame(DisableVSync);

            //New cursor texture (bad idea actually)
            if (-1 == 0)
            {
                Texture2D tex = AssetLoader.GetObjectFromFile<Texture2D>("cdo_rw_stuff", "CursorV6");
                Cursor.SetCursor(tex, Vector2.zero, CursorMode.ForceSoftware);
            }

            Transform trans = TransformUtils.FindChildRecursive(GameUIRoot.Instance.TitleScreenUI.transform, "BottomButtons");
            trans.localScale = new Vector3(0.85f, 0.85f, 0.85f); //OptionsButton
            trans.localPosition = new Vector3(0, -180, 0); //CanvasRoundedUnityDarkEdge
            //TransformUtils.FindChildRecursive(GameUIRoot.Instance.TitleScreenUI.transform, "LeftFadeBG").GetComponent<UnityEngine.UI.Image>().color = new Color(0, 0, 0, 0.65f);
            //TransformUtils.FindChildRecursive(GameUIRoot.Instance.TitleScreenUI.transform, "PlaySingleplayer").GetComponent<UnityEngine.UI.Image>().sprite = AssetLoader.GetObjectFromFile<Sprite>("cdo_rw_stuff", "CanvasRoundedUnityDarkEdge");
            //TransformUtils.FindChildRecursive(GameUIRoot.Instance.TitleScreenUI.transform, "MultiplayerButton_NEW").GetComponent<UnityEngine.UI.Image>().sprite = AssetLoader.GetObjectFromFile<Sprite>("cdo_rw_stuff", "CanvasRoundedUnityDarkEdge");

            for (int i = 0; i < trans.childCount; i++)
            {
                //trans.GetChild(i).GetComponent<UnityEngine.UI.Image>().sprite = AssetLoader.GetObjectFromFile<Sprite>("cdo_rw_stuff", "CanvasRoundedUnityDarkEdge");
                if (!trans.GetChild(i).gameObject.name.Contains("HR"))
                {
                    trans.GetChild(i).GetComponent<RectTransform>().sizeDelta = new Vector2(115, 29);
                }
            }

            if (TransformUtils.FindChildRecursive(GameUIRoot.Instance.TitleScreenUI.transform, "OptionsButton"))
            {
                Transform t2 = UnityEngine.GameObject.Instantiate<Transform>(TransformUtils.FindChildRecursive(GameUIRoot.Instance.TitleScreenUI.transform, "OptionsButton"), trans);
                t2.SetSiblingIndex(1);
                UnityEngine.GameObject.Destroy(t2.GetComponentInChildren<LocalizedTextField>());
                t2.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
                t2.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(UI.SettingsUI.Instance.Show));
                t2.GetComponentInChildren<Text>().text = "Overhaul Settings";
                _settingsButtonText = t2.GetComponentInChildren<Text>();
            }

            foreach (UnityEngine.UI.Image img in GameUIRoot.Instance.GetComponentsInChildren<UnityEngine.UI.Image>(true))
            {
                if (img != null && img.sprite != null)
                {
                    if (img.sprite.name == "UISprite" || img.sprite.name == "Knob")
                    {
                        img.sprite = AssetLoader.GetObjectFromFile<Sprite>("cdo_rw_stuff", "CanvasRoundedUnityDarkEdge");
                    }
                    if (img.sprite.name == "Checkmark")
                    {
                        img.sprite = AssetLoader.GetObjectFromFile<Sprite>("cdo_rw_stuff", "CheckmarkSmall");
                        img.color = Color.black;
                    }
                    if (img.sprite.name == "Background")
                    {
                        img.sprite = AssetLoader.GetObjectFromFile<Sprite>("cdo_rw_stuff", "CanvasRoundedUnity");
                    }
                    Outline outline = img.GetComponent<Outline>();
                    if (outline != null)
                    {
                        outline.enabled = false;
                    }
                }
            }

            foreach (EnemyConfiguration character in EnemyFactory.Instance.Enemies)
            {
                if (character.EnemyPrefab.GetComponent<FirstPersonMover>() != null)
                {
                    foreach (PicaVoxel.Frame part in character.EnemyPrefab.GetComponent<FirstPersonMover>().CharacterModelPrefab.transform.GetChild(0).GetChild(0).GetComponentsInChildren<PicaVoxel.Frame>(true))
                    {
                        Patching.BodyPartPatcher.OnBodyPartStart(part);
                    }
                }
            }

            new GameObject("CDO_RW_BoltEventListener").AddComponent<BoltEventListener>();
        }

        private void DisableVSync()
        {
            SettingsManager.Instance.SetVsyncOn(false);
            Application.targetFrameRate = 119;
        }

        private void fixVanillaStuff()
        {
            MultiplayerCharacterCustomizationManager.Instance.CharacterModels[17].UnlockedByAchievementID = string.Empty;
            PatchesManager.Instance.UpdateAudioSettings(GetSetting<bool>("Patches.QoL.Fix sounds"));
        }

        private void spawnGUI()
        {
            UI.GUIManagement mngr = BaseStaticReferences.ModuleManager.GetModule<UI.GUIManagement>();

            GameObject obj = GameObject.Instantiate(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "CDO_RW_UI"));
            mngr.AddGUI(obj.GetComponent<ModdedObject>().GetObjectFromList<Transform>(0).gameObject.AddComponent<UI.Watermark>());
            mngr.AddGUI(obj.GetComponent<ModdedObject>().GetObjectFromList<Transform>(9).gameObject.AddComponent<UI.SettingsUI>());
            mngr.AddGUI(obj.GetComponent<ModdedObject>().GetObjectFromList<Transform>(2).gameObject.AddComponent<Localization.OverhaulLocalizationEditor>());
            mngr.AddGUI(obj.GetComponent<ModdedObject>().GetObjectFromList<Transform>(1).gameObject.AddComponent<UI.NewErrorWindow>());
            mngr.AddGUI(obj.GetComponent<ModdedObject>().GetObjectFromList<Transform>(4).gameObject.AddComponent<UI.BackupMindTransfersUI>());
            mngr.AddGUI(obj.GetComponent<ModdedObject>().GetObjectFromList<Transform>(3).gameObject.AddComponent<UI.Notifications.NotificationsUI>());
            mngr.AddGUI(obj.GetComponent<ModdedObject>().GetObjectFromList<Transform>(5).gameObject.AddComponent<UI.NewEscMenu>());
            mngr.AddGUI(obj.GetComponent<ModdedObject>().GetObjectFromList<Transform>(8).gameObject.AddComponent<UI.MultiplayerInviteUIs>());
            mngr.AddGUI(obj.GetComponent<ModdedObject>().GetObjectFromList<Transform>(10).gameObject.AddComponent<LevelEditor.ModdedLevelEditorUI>());
            mngr.AddGUI(obj.GetComponent<ModdedObject>().GetObjectFromList<Transform>(11).gameObject.AddComponent<UI.MultiplayerUIs>());
            mngr.AddGUI(obj.GetComponent<ModdedObject>().GetObjectFromList<Transform>(12).gameObject.AddComponent<UI.NewKillFeedUI>());
            mngr.AddGUI(obj.GetComponent<ModdedObject>().GetObjectFromList<Transform>(13).gameObject.AddComponent<UI.NewGameModeSelectionScreen>());
            mngr.AddGUI(obj.GetComponent<ModdedObject>().GetObjectFromList<Transform>(14).gameObject.AddComponent<UI.NewPhotoModeUI>());
        }

        public static string GetTranslatedString(string ID)
        {
            CloneDroneOverhaul.Localization.TranslationEntry entry = Localization.GetTranslation(ID);
            if (entry == null)
            {
                return "NL: " + ID;
            }
            string text = entry.Translations[LocalizationManager.Instance.GetCurrentLanguageCode()];
            if (text == "nontranslated")
            {
                text = entry.Translations["en"];
            }
            return text;
        }

        public static T GetSetting<T>(string ID)
        {
            return CloneDroneOverhaulDataContainer.Instance.SettingsData.GetSettingValue<T>(ID);
        }
    }

    public static class OverhaulDescription
    {
        public enum Branch
        {
            Github,

            ModBot
        }

        public static string GetModName(bool includeVersion, bool shortVariant = false)
        {
            string name = shortVariant == false ? "Clone Drone Overhaul " + GetModVersionBranch() : "CDO";
            if (includeVersion)
            {
                return name + " " + GetModVersion();
            }
            return name;
        }

        public static string GetModVersion(bool withModBotVersion = true)
        {
            string version = "a0.2.0.13";
            if (!withModBotVersion)
            {
                return version;
            }
            return version + " (." + OverhaulMain.Instance.ModInfo.Version + ")";
        }

        public static Branch GetModVersionBranch()
        {
            return Branch.ModBot;
        }

        public static bool IsBetaBuild()
        {
            return false;
        }


        internal static bool IsLETInstalled;
        public static bool LevelEditorToolsInstalled()
        {
            return BaseUtils.GetModInfoByID("286ea03e-b667-46ae-8c12-95eb08c412e4") != null;
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

    public static class CrossModManager //probably a mistake
    {
        public static void DoAction(string name, object[] arguments)
        {
            try
            {
                if (name == "ModdedLevelEditor.RefreshSelected")
                {
                    OverhaulMain.GUI.GetGUI<LevelEditor.ModdedLevelEditorUI>().RefreshSelected((arguments[0] as LevelEditorObjectPlacementManager).GetSelectedSceneObjects());
                }
                if (name == "ModdedLevelEditor.RefreshSelectedLETMod")
                {
                    LevelEditor.ModdedLevelEditorUI.ObjectsSelectedPanel ObjectsSelected = arguments[0] as LevelEditor.ModdedLevelEditorUI.ObjectsSelectedPanel;
                    if (OverhaulDescription.LevelEditorToolsInstalled())
                    {
                        float? rotAngle = LevelEditorTools.AccurateRotationTool.RotationAngle;
                        ObjectsSelected.AdditionalText.text = string.Concat(new string[]
                        {
                    "Level Editor Tools Mod:",
                    System.Environment.NewLine,
                    "R + " + LevelEditorTools.PositionerTool.SetXKey + " to rotate objects " + rotAngle + " degrees along X axis",
                                 System.Environment.NewLine,
                    "R + " + LevelEditorTools.PositionerTool.SetYKey + " to rotate objects " + rotAngle + " degrees along Y axis",
                                 System.Environment.NewLine,
                    "R + " + LevelEditorTools.PositionerTool.SetZKey + " to rotate objects " + rotAngle + " degrees along Z axis",
                        });
                    }
                    else
                    {
                        ObjectsSelected.AdditionalText.text = "Install/Enable Level editor tools mod for advanced controls";
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
