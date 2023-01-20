using CloneDroneOverhaul.LevelEditor;
using CloneDroneOverhaul.Modules;
using CloneDroneOverhaul.UI;
using CloneDroneOverhaul.V3;
using CloneDroneOverhaul.V3.Base;
using CloneDroneOverhaul.V3.HUD;
using CloneDroneOverhaul.V3.Notifications;
using ModBotWebsiteAPI;
using ModLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using CloneDroneOverhaul.V3.Gameplay;

namespace CloneDroneOverhaul
{
    /// <summary>
    /// Most of the stuff in this class is gonna move on <see cref="V3.Base.V3_MainModController"/>
    /// </summary>
    [MainModClass]
    public class OverhaulMain : Mod
    {
        public static OverhaulMain Instance { get; internal set; }
        public static DelegateTimer Timer { get; set; }
        public static ModuleManagement Modules { get; set; }

        private static bool _hasCheckedForUpdates;

        private static bool _hasCachedStuff;

        private Text _settingsButtonText;

        protected override void OnModLoaded()
        {
            if (OverhaulMain.Instance != null) return;
            OverhaulMain.Instance = this;
            InitializeOverhaul();
        }

        private void InitializeOverhaul()
        {
            if (!OverhaulMain._hasCachedStuff)
            {
                rememberVanillaPreferences();
                OverhaulCacheAndGarbageController.PrepareStuff();
                OverhaulMain._hasCachedStuff = true;
            }
            addModules();
            V3_MainModController.Initialize();
            fixVanillaStuff();
            finalPreparations();
            checkforUpdate();
        }

        protected override UnityEngine.Object OnResourcesLoad(string path)
        {
            return LevelEditorCustomObjectsManager.TryGetObject(path);
        }

        protected override void OnLanguageChanged(string newLanguageID, Dictionary<string, string> localizationDictionary)
        {
            Timer.CompleteNextFrame(delegate
            {
                OverhaulMain.Modules.ExecuteFunction("onLanguageChanged", null);
                _settingsButtonText.text = OverhaulMain.GetTranslatedString("OverhaulSettings");
            });
        }

        protected override void OnLevelEditorStarted()
        {
            LevelEditorCustomObjectsManager.OnLevelEditorStarted();
            OverhaulMain.Modules.ExecuteFunction("onLevelEditorStarted", null);
        }

        protected override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover)
        {
            OverhaulMain.Modules.ExecuteFunction("firstPersonMover.OnSpawn", new object[]
            {
                firstPersonMover.GetRobotInfo()
            });
        }

        protected override void OnUpgradesRefreshed(FirstPersonMover owner, UpgradeCollection upgrades)
        {
            string str = FirstPersonMoverAddititonBase.TEMPORAL_PREFIX + owner.GetInstanceID();
            if (OverhaulCacheAndGarbageController.ContainsTemporalObject(str))
            {
                FirstPersonMoverAddititonBase aBase = OverhaulCacheAndGarbageController.GetTemporalObject<FirstPersonMoverAddititonBase>(str);
                if (aBase != null)
                {
                    aBase.OnUpgradesRefreshed(upgrades);
                }
            }
        }

        private void rememberVanillaPreferences()
        {
            VanillaPrefs.RememberStuff();
        }

        private void checkforUpdate()
        {
            if (OverhaulMain._hasCheckedForUpdates)
            {
                return;
            }
            OverhaulMain._hasCheckedForUpdates = true;
            UpdateController.CheckForUpdates(new Action<Version>(OnUpdateReceivedGitHub));
            API.GetModData("rAnDomPaTcHeS1", new Action<JsonObject>(OnModDataGet));
        }

        private void OnModDataGet(JsonObject json)
        {
            int a = int.Parse(json["Version"].ToString());
            int b = int.Parse(base.ModInfo.Version.ToString());
            if (a > b)
            {
                SNotificationButton[] buttons = new SNotificationButton[] { new SNotificationButton("Mod site", null) };
                buttons[0].SetAction(UpdateController.OpenModBotPage);
                SNotification notif = new SNotification("New update is out on Mod-Bot!", "If you wish, you may visit Overhaul mod site by clicking \"Mod-Bot\" and download new version.", 20f, UINotifications.NotificationSize_Default, buttons);
                notif.Send();
            }
        }

        private void OnUpdateReceivedGitHub(Version newVersion)
        {
            if (newVersion <= UpdateController.ToVersion(OverhaulDescription.GetModVersion(false)))
            {
                return;
            }

            SNotificationButton[] buttons = new SNotificationButton[] { new SNotificationButton("GitHub", null) };
            buttons[0].SetAction(UpdateController.OpenGitHubWithReleases);
            SNotification notif = new SNotification("Version a" + newVersion + " out on GitHub!", "You may visit mod GitHub site by clicking \"GitHub\" to download new version.", 20f, UINotifications.NotificationSize_Default, buttons);
            notif.Send();
        }

        private void addModules()
        {
            ModuleManagement moduleManagement = new ModuleManagement();
            moduleManagement.AddModule<ModDataManager>(false);

            OverhaulMain.Modules = moduleManagement;

            CloneDroneOverhaulDataContainer.Initialize();
        }

        private void finalPreparations()
        {
            if (OverhaulDescription.TEST_FEATURES_ENABLED)
            {
                HotkeysModule.GetInstance<HotkeysModule>().AddHotkey(new Hotkey
                {
                    Key2 = KeyCode.C,
                    Key1 = KeyCode.LeftControl,
                    Method = new Action(BaseUtils.DebugFireSword)
                });
                HotkeysModule.GetInstance<HotkeysModule>().AddHotkey(new Hotkey
                {
                    Key2 = KeyCode.B,
                    Key1 = KeyCode.LeftControl,
                    Method = new Action(BaseUtils.ExplodePlayer)
                });
                HotkeysModule.GetInstance<HotkeysModule>().AddHotkey(new Hotkey
                {
                    Key2 = KeyCode.V,
                    Key1 = KeyCode.LeftControl,
                    Method = new Action(BaseUtils.AddSkillPoint)
                });
                HotkeysModule.GetInstance<HotkeysModule>().AddHotkey(new Hotkey
                {
                    Key2 = KeyCode.M,
                    Key1 = KeyCode.LeftControl,
                    Method = delegate ()
                    {
                        V3.Gameplay.LevelConstructor.BuildALevel(new V3.Gameplay.LevelConstructor.LevelSettings(), true);
                    }
                });
            }
            HotkeysModule.GetInstance<HotkeysModule>().AddHotkey(new Hotkey
            {
                Key1 = KeyCode.F2,
                Method = new Action(CloneDroneOverhaul.V3.Graphics.CameraRollController.SwitchHud)
            });

            Transform transform = TransformUtils.FindChildRecursive(Singleton<GameUIRoot>.Instance.TitleScreenUI.transform, "BottomButtons");
            transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
            transform.localPosition = new Vector3(0f, -180f, 0f);
            if (TransformUtils.FindChildRecursive(Singleton<GameUIRoot>.Instance.TitleScreenUI.transform, "OptionsButton"))
            {
                Transform transform2 = UnityEngine.Object.Instantiate<Transform>(TransformUtils.FindChildRecursive(Singleton<GameUIRoot>.Instance.TitleScreenUI.transform, "OptionsButton"), transform);
                transform2.SetSiblingIndex(1);
                UnityEngine.Object.Destroy(transform2.GetComponentInChildren<LocalizedTextField>());
                transform2.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
                transform2.GetComponent<Button>().onClick.AddListener(UIModSettings.GetInstance<UIModSettings>().Show);
                transform2.GetComponentInChildren<Text>().text = "Overhaul Settings";
                _settingsButtonText = transform2.GetComponentInChildren<Text>();
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
        }

        private void fixVanillaStuff()
        {
            // Make Jetpack1 skin available for everyone
            Singleton<MultiplayerCharacterCustomizationManager>.Instance.CharacterModels[17].UnlockedByAchievementID = string.Empty;

            // Changing pixels per unit of main canvas
            GameUIRoot.Instance.GetComponent<Canvas>().referencePixelsPerUnit = 75f;

            
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
            QualitySettings.softParticles = true;
            QualitySettings.streamingMipmapsMemoryBudget = 4096f;
            QualitySettings.streamingMipmapsMaxLevelReduction = 6;
            QualitySettings.streamingMipmapsMaxFileIORequests = 4096;
            QualitySettings.asyncUploadBufferSize = 16;
            QualitySettings.asyncUploadTimeSlice = 4;
            QualitySettings.asyncUploadPersistentBuffer = true;
            QualitySettings.shadowCascades = 0; // 2 Before

            Singleton<SkyBoxManager>.Instance.LevelConfigurableSkyboxes[8].SetColor("_Tint", new Color(0.6f, 0.73f, 2f, 1f));
            Singleton<AttackManager>.Instance.HitColor = new Color(4f, 0.65f, 0.35f, 0.2f);
            Singleton<AttackManager>.Instance.BodyOnFireColor = new Color(1f, 0.42f, 0.22f, 0.1f);
            Singleton<EmoteManager>.Instance.PitchLimits.Max = 5f;
            Singleton<EmoteManager>.Instance.PitchLimits.Min = 0f;
        }

        public static string GetTranslatedString(in string ID, in bool returnSameIfNull = false)
        {
            Localization.TranslationEntry translation = Localization.LocalizationController.GetInstance<Localization.LocalizationController>().GetTranslation(ID);
            if (translation == null)
            {
                if (returnSameIfNull)
                {
                    return ID;
                }
                return "NL: " + ID;
            }
            string text = translation.Translations[Singleton<LocalizationManager>.Instance.GetCurrentLanguageCode()];
            if (text == "nontranslated")
            {
                text = translation.Translations["en"];
            }
            return text;
        }

        public static T GetSetting<T>(in string ID)
        {
            return CloneDroneOverhaulDataContainer.SettingsData.GetSettingValue<T>(ID);
        }

        public static T GetSettingValue<T>(in string settingName)
        {
            return V3.Base.ModSettingsController.GetSettingValue<T>(settingName);
        }
    }

    public static class OverhaulDescription
    {
        public const string LEVEL_EDITOR_TOOLS_MODID = "286ea03e-b667-46ae-8c12-95eb08c412e4";

        public const bool TEST_FEATURES_ENABLED = true;
        public const bool OVERRIDE_VERSION = false;
        public const string OVERRIDE_VERSION_STRING = "a0.2.0.25";
        public static readonly string VersionString = "a" + Assembly.GetExecutingAssembly().GetName().Version.ToString() + "-PREVIEW 3";
        public static readonly string ModBotVersionString = OverhaulMain.Instance.ModInfo.Version.ToString();
        public const OverhaulDescription.Branch CURRENT_BRANCH = Branch.Github;

        public const string MOD_FULLNAME_SPACE = "Clone Drone Overhaul ";
        public const string MOD_FULLNAME = "Clone Drone Overhaul";
        public const string MOD_SHORTNAME_SPACE = "CDO ";
        public const string MOD_SHORTNAME = "CDO";

        public static string GetModFolder()
        {
            return OverhaulMain.Instance.ModInfo.FolderPath;
        }

        public static string GetModName(bool includeVersion, bool shortVariant = false)
        {
            string text = (!shortVariant) ? (MOD_FULLNAME_SPACE + CURRENT_BRANCH.ToString()) : MOD_SHORTNAME;
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
            return gameVersion + " (." + ModBotVersionString + ")";
        }

        private static string getGameVersion()
        {
            return OVERRIDE_VERSION ? OVERRIDE_VERSION_STRING : VersionString;
        }

        public static bool LevelEditorToolsEnabled()
        {
            return PlayerPrefs.HasKey(LEVEL_EDITOR_TOOLS_MODID) && PlayerPrefs.GetInt(LEVEL_EDITOR_TOOLS_MODID) == 1;
        }

        public enum Branch
        {
            Github,
            ModBot
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

    public static class CrossModManager
    {
        public static void DoAction(string name, object[] arguments)
        {
            if (OverhaulDescription.LevelEditorToolsEnabled())
            {
                doAction_LET(name, arguments);
            }
            else
            {

            }
        }
        private static void doAction_LET(string name, object[] arguments)
        {
            try
            {
                
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
