﻿using CloneDroneOverhaul.Modules;
using ModLibrary;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ModBotWebsiteAPI;
using System;

namespace CloneDroneOverhaul
{
    [MainModClass]
    public class OverhaulMain : Mod
    {
        private static bool hasCheckForUpdates;
        public bool IsModInitialized { get; private set; }
        public static OverhaulMain Instance { get; internal set; }
        private static Localization.OverhaulLocalizationManager Localization { get; set; }
        public static DelegateTimer Timer { get; set; }
        public static VisualsModule Visuals { get; set; }
        public static UI.GUIManagement GUI { get; set; }
        public static WeaponSkins.WeaponSkinManager Skins { get; set; }
        public static ModuleManagement Modules { get; set; }

        public string GetModFolder() //C:/Program Files (x86)/Steam/steamapps/common/Clone Drone in the Danger Zone/mods/CloneDroneOverhaulRW/
        {
            return ModInfo.FolderPath;
        }

        protected override void OnModLoaded()
        {
            if (OverhaulMain.Instance != null)
            {
                return;
            }
            AppDomain.CurrentDomain.Load(File.ReadAllBytes(this.ModInfo.FolderPath + "netstandard.dll"));
            OverhaulMain.Instance = this;
            BaseStaticValues.IsModEnabled = true;

            addReferences();
            addModules();
            addListeners();
            spawnGUI();

            IsModInitialized = true;

            finalPreparations();
            checkforUpdate();
        }

        protected override void OnModDeactivated()
        {
            BaseStaticValues.IsModEnabled = false;
        }

        private void checkforUpdate()
        {
            if (hasCheckForUpdates)
            {
                return;
            }
            API.GetModData("rAnDomPaTcHeS1", new Action<JsonObject>(this.OnModDataGet));
        }
        private void OnModDataGet(JsonObject json)
        {
            hasCheckForUpdates = true;
            CloneDroneOverhaul.UI.Notifications.Notification notif = new UI.Notifications.Notification();
            notif.SetUp("New update available!", "It includes fixes, stability improvements and other.", 20, Vector2.zero, Color.clear, new UI.Notifications.Notification.NotificationButton[] { new UI.Notifications.Notification.NotificationButton { Action = new UnityEngine.Events.UnityAction(notif.HideThis), Text = "OK" } });
            return;
            if ((Int32)OverhaulMain.Instance.ModInfo.Version < (Int32)json["Version"])
            {
            }
        }

        private void addReferences()
        {
            BaseStaticReferences.ModuleManager = new ModuleManagement();
        }

        private void addModules()
        {
            ModuleManagement manager = BaseStaticReferences.ModuleManager;
            Modules = manager;
            Timer = manager.AddModule<DelegateTimer>();
            Localization = manager.AddModule<Localization.OverhaulLocalizationManager>();
            Visuals = manager.AddModule<VisualsModule>();
            manager.AddModule<HotkeysModule>();
            GUI = manager.AddModule<UI.GUIManagement>();
            Skins = manager.AddModule<WeaponSkins.WeaponSkinManager>();
            manager.AddModule<WorldGUIs>();
            manager.AddModule<RobotEventsModule>();
            manager.AddModule<ModDataManager>();
            manager.AddModule<Addons.AddonsManager>();
            manager.AddModule<Modules.MultiplayerManager>();
            manager.AddModule<ArenaAppearenceManager>();
        }

        private void addListeners()
        {
            UnityEngine.GameObject obj = new UnityEngine.GameObject("CDOListeners");
            obj.AddComponent<OverhaulMonoBehaviourListener>();
        }

        private void finalPreparations()
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
                Key1 = UnityEngine.KeyCode.F2,
                Method = CinematicGameManager.SwitchHud
            });
            BaseStaticReferences.ModuleManager.GetModule<HotkeysModule>().AddHotkey(new Hotkey
            {
                Key2 = UnityEngine.KeyCode.X,
                Key1 = UnityEngine.KeyCode.LeftControl,
                Method = BaseUtils.DebugSize
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

            SkyBoxManager.Instance.LevelConfigurableSkyboxes[8].SetColor("_Tint", new Color(0.6f, 0.73f, 2f, 1f));

            AttackManager.Instance.HitColor = new Color(4, 0.65f, 0.35f, 0.2f);
            AttackManager.Instance.BodyOnFireColor = new Color(1, 0.42f, 0.22f, 0.1f);

            EmoteManager.Instance.PitchLimits.Max = 5f;
            EmoteManager.Instance.PitchLimits.Min = 0f;

            Application.targetFrameRate = 119;
            Timer.AddNoArgActionToCompleteNextFrame(DisableVSync);

            if (-1 == 0)
            {
                Texture2D tex = AssetLoader.GetObjectFromFile<Texture2D>("cdo_rw_stuff", "CursorImg");
                Cursor.SetCursor(tex, Vector2.zero, CursorMode.ForceSoftware);
            }

            unlockSecretFeatures();
        }

        private void DisableVSync()
        {
            SettingsManager.Instance.SetVsyncOn(false);
        }

        private void unlockSecretFeatures()
        {
            MultiplayerCharacterCustomizationManager.Instance.CharacterModels[17].UnlockedByAchievementID = string.Empty;
        }

        private void spawnGUI()
        {
            UI.GUIManagement mngr = BaseStaticReferences.ModuleManager.GetModule<UI.GUIManagement>();

            GameObject obj = GameObject.Instantiate(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "CDO_RW_UI"));
            mngr.AddGUI(obj.GetComponent<ModdedObject>().GetObjectFromList<Transform>(0).gameObject.AddComponent<UI.Watermark>());
            mngr.AddGUI(obj.GetComponent<ModdedObject>().GetObjectFromList<Transform>(2).gameObject.AddComponent<Localization.OverhaulLocalizationEditor>());
            mngr.AddGUI(obj.GetComponent<ModdedObject>().GetObjectFromList<Transform>(1).gameObject.AddComponent<UI.NewErrorWindow>());
            mngr.AddGUI(obj.GetComponent<ModdedObject>().GetObjectFromList<Transform>(4).gameObject.AddComponent<UI.BackupMindTransfersUI>());
            mngr.AddGUI(obj.GetComponent<ModdedObject>().GetObjectFromList<Transform>(3).gameObject.AddComponent<UI.Notifications.NotificationsUI>());
            mngr.AddGUI(obj.GetComponent<ModdedObject>().GetObjectFromList<Transform>(5).gameObject.AddComponent<UI.NewEscMenu>());
        }

        public static string GetTranslatedString(string ID)
        {
            CloneDroneOverhaul.Localization.TranslationEntry entry = Localization.GetTranslation(ID);
            if (entry == null)
            {
                return "NT: " + ID;
            }
            return entry.Translations[LocalizationManager.Instance.GetCurrentLanguageCode()];
        }
    }

    public static class OverhaulDescription
    {
        public static string GetModName(bool includeVersion, bool shortVariant = false)
        {
            string name = shortVariant == false ? "Clone Drone Overhaul" : "CDO";
            if (includeVersion)
            {
                return name + " " + GetModVersion();
            }
            return name;
        }

        public static string GetModVersion()
        {
            return "a0.2.0.4 (." + OverhaulMain.Instance.ModInfo.Version + ")";
        }
    }

    public class OverhaulMonoBehaviourListener : ManagedBehaviour
    {
        private float timeWhenOneSecondWillPast;
        public static bool IsApplicationFocused { get; private set; }

        private bool IsReadyToWork()
        {
            return OverhaulMain.Instance.IsModInitialized;
        }

        public override void UpdateMe()
        {
            if (IsReadyToWork())
            {
                BaseStaticReferences.ModuleManager.OnManagedUpdate();
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
            if (GameModeManager.IsMultiplayer() && UnityEngine.Time.timeScale > 1.2f)
            {
                UnityEngine.Time.timeScale = 1f;
            }
            //AttackManager.Instance.MultiplayerAttackSpeedMultiplier = 0.8f * (1f / UnityEngine.Time.timeScale);
            if (IsReadyToWork())
            {
                BaseStaticReferences.ModuleManager.OnFrame();
            }
        }

        void OnApplicationFocus(bool hasFocus)
        {
            OverhaulMonoBehaviourListener.IsApplicationFocused = hasFocus;
            if (!hasFocus)
            {
                //Shader.Find("Standard").maximumLOD = 1;
                Application.targetFrameRate = 30;
            }
            else
            {
                //Shader.Find("Standard").maximumLOD = -1;
                Application.targetFrameRate = 119;
            }
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
            }
        }

        private void Start()
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }
        private void OnSceneUnloaded(Scene current)
        {
            OverhaulMain.Instance = null;
        }
    }

    public static class CodeExtensions
    {
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
                result = (GameModeManager.IsMultiplayer() && type != GameRequestType.RandomAnyQuickMatch && type != GameRequestType.RandomBattleRoyale && type != GameRequestType.RandomCoopChallenge && type != GameRequestType.RandomDuel && type != GameRequestType.RandomEndlessCoop);
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
                    result = (T)((object)obj);
                }
            }
            return result;
        }
    }

    // Changelog 0.2.0.2
    // Mind transfers panel shows above your head
    // Version watermark no longer overlaps PerformanceStatsPanel
}
