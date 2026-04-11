using ModLibrary;
using OverhaulMod.Combat;
using OverhaulMod.Content.Personalization;
using OverhaulMod.Engine;
using OverhaulMod.Patches.Behaviours;
using OverhaulMod.Utils;
using OverhaulMod.Visuals;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR;

namespace OverhaulMod
{
    [MainModClass]
    public class ModCore : Mod
    {
        public const string CUSTOMIZATION_FOLDER_NAME = "customization";

        public const string CUSTOMIZATION_PERSISTENT_FOLDER_NAME = "customizationPersistent";

        [ModSetting(ModSettingsConstants.ENABLE_TITLE_BAR_OVERHAUL, true)]
        public static bool EnableTitleBarOverhaul;

        [ModSetting(ModSettingsConstants.SHOW_SPEAKER_NAME, true)]
        public static bool ShowSpeakerName;

        [ModSetting(ModSettingsConstants.SWAP_SUBTITLES_COLOR, false)]
        public static bool SwapSubtitlesColor;

        [ModSetting(ModSettingsConstants.CURSOR_SKIN, 1)]
        public static int CursorSkin;

        [ModSetting(ModSettingsConstants.DISABLE_SCREEN_SHAKING, false)]
        public static bool DisableScreenShaking;

        public static bool ShouldStartCustomizationEditor;

        public static event Action<Camera, Camera> OnCameraSwitched;

        public static ModCore Instance { get; private set; }

        //public static StringBuilder TempStringBuilder = new StringBuilder();

        public static bool IsEnabled
        {
            get;
            private set;
        }

        private static string s_folder;
        public static string Folder
        {
            get
            {
                ModCore modCore = Instance;
                if (modCore == null)
                {
                    return null;
                }

                if (s_folder == null)
                {
                    s_folder = modCore.ModInfo.FolderPath;
                }
                return s_folder;
            }
        }

        private static string s_savesFolder;
        public static string SavesFolder
        {
            get
            {
                if (s_savesFolder == null)
                {
                    s_savesFolder = $"{Path.Combine(ModUserDataFolder, "saves")}/";
                }
                return s_savesFolder;
            }
        }

        private static string s_assetsFolder;
        public static string AssetsFolder
        {
            get
            {
                if (s_assetsFolder == null)
                {
                    s_assetsFolder = $"{Path.Combine(Folder, "assets")}/";
                }
                return s_assetsFolder;
            }
        }

        private static string s_dataFolder;
        public static string DataFolder
        {
            get
            {
                if (s_dataFolder == null)
                {
                    s_dataFolder = $"{Path.Combine(AssetsFolder, "data")}/";
                }
                return s_dataFolder;
            }
        }

        private static string s_modUserDataFolder;
        public static string ModUserDataFolder
        {
            get
            {
                if (s_modUserDataFolder == null)
                {
                    s_modUserDataFolder = $"{Path.Combine(Application.persistentDataPath, "OverhaulMod")}/";
                }
                return s_modUserDataFolder;
            }
        }

        private static string s_developerFolder;
        public static string DeveloperFolder
        {
            get
            {
                if (s_developerFolder == null)
                {
                    s_developerFolder = $"{Path.Combine(ModUserDataFolder, "devFolder")}/";
                }
                return s_developerFolder;
            }
        }

        private static string s_contentFolder;
        public static string ContentFolder
        {
            get
            {
                if (s_contentFolder == null)
                {
                    s_contentFolder = $"{Path.Combine(ModUserDataFolder, "content")}/";
                }
                return s_contentFolder;
            }
        }

        private static string s_addonsFolder;
        public static string AddonsFolder
        {
            get
            {
                if (s_addonsFolder == null)
                {
                    s_addonsFolder = $"{Path.Combine(ContentFolder, "addons")}/";
                }
                return s_addonsFolder;
            }
        }

        private static string s_customizationFolder;
        public static string CustomizationFolder
        {
            get
            {
                if (s_customizationFolder == null)
                {
                    s_customizationFolder = $"{Path.Combine(ContentFolder, CUSTOMIZATION_FOLDER_NAME)}/";
                }
                return s_customizationFolder;
            }
        }

        private static string s_persistentCustomizationFolder;
        public static string CustomizationPersistentFolder
        {
            get
            {
                if (s_persistentCustomizationFolder == null)
                {
                    s_persistentCustomizationFolder = $"{Path.Combine(ContentFolder, CUSTOMIZATION_PERSISTENT_FOLDER_NAME)}/";
                }
                return s_persistentCustomizationFolder;
            }
        }

        private static string s_textureFolder;
        public static string TexturesFolder
        {
            get
            {
                if (s_textureFolder == null)
                {
                    s_textureFolder = $"{Path.Combine(AssetsFolder, "textures")}/";
                }
                return s_textureFolder;
            }
        }

        private static string s_editorTexturesFolder;
        public static string EditorTexturesFolder
        {
            get
            {
                if (s_editorTexturesFolder == null)
                {
                    s_editorTexturesFolder = $"{Path.Combine(TexturesFolder, "editor")}/";
                }
                return s_editorTexturesFolder;
            }
        }

        private bool _hasAttemptedToLoadOnThisScene;

        public override void OnModLoaded()
        {
            Instance = this;
            IsEnabled = true;

            ModLoader.Load();
            _hasAttemptedToLoadOnThisScene = true;

            GlobalEventManager.Instance.AddEventListenerOnce(GlobalEvents.GameInitializtionCompleted, onGameInitialized);
        }

        public override void OnModEnabled()
        {
            Instance = this;
            IsEnabled = true;

            if (!_hasAttemptedToLoadOnThisScene)
            {
                ModLoader.Load();
                _hasAttemptedToLoadOnThisScene = true;
            }

            GamePatchBehaviour.Load();

            ModSpecialUtils.SetTitleBarStateDependingOnSettings();

            if (GameModeManager.IsOnTitleScreen()) _ = ModUIConstants.ShowTitleScreenReworkIfHaventBefore();
        }

        public override void OnModDeactivated()
        {
            Instance = null;
            IsEnabled = false;
            _hasAttemptedToLoadOnThisScene = false;

            GameUIRoot gameUIRoot = ModCache.gameUIRoot;
            if (gameUIRoot)
            {
                ModUIManager modUIManager = ModUIManager.Instance;
                if (modUIManager)
                {
                    if (modUIManager.IsUIVisible(AssetBundleConstants.UI, ModUIConstants.UI_PAUSE_MENU))
                    {
                        _ = modUIManager.Hide(AssetBundleConstants.UI, ModUIConstants.UI_PAUSE_MENU);
                        if (gameUIRoot.EscMenu)
                            gameUIRoot.EscMenu.Show();
                    }
                }
            }

            ModSpecialUtils.SetTitleBarStateDependingOnSettings();

            GamePatchBehaviour.Unload();

            ModLoader.Unload();
        }

        public override void OnClientConnectedToServer()
        {
            PersonalizationMultiplayerManager.Instance.SendPlayerCustomizationDataEvent(false);
            if (GameModeManager.Is(GameMode.EndlessCoop) || GameModeManager.Is(GameMode.CoopChallenge))
                _ = waitThenFixArenaLiftInCoop().Run();

            ArenaRemodelManager.Instance.PatchVanillaParts(false);
        }

        public override void OnLevelEditorStarted()
        {
            ArenaRemodelManager.Instance.SetUpperInteriorActive(false);
            PostEffectsManager.Instance.RefreshCameraPostEffects();
        }

        public override void OnMultiplayerEventReceived(GenericStringForModdingEvent moddedEvent)
        {
            PersonalizationMultiplayerManager.Instance.OnEvent(moddedEvent);
        }

        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover)
        {
            _ = ModActionUtils.RunCoroutine(waitUntilCharacterModelInitialization(firstPersonMover));
        }

        public override void OnUpgradesRefreshed(FirstPersonMover owner, UpgradeCollection upgrades)
        {
            owner.addWeaponToEquipppedIfHasUpgradeAndModelPresent(ModUpgradesManager.SCYTHE_UNLOCK_UPGRADE, ModWeaponsManager.SCYTHE_TYPE);
            owner.RefreshModWeaponModels();

            CharacterExtension robotInventory = ModComponentCache.GetRobotInventory(owner.transform);
            if (robotInventory)
            {
                robotInventory.OnUpgradesRefreshed(upgrades);
            }
        }

        public override void AfterUpgradesRefreshed(FirstPersonMover owner, UpgradeCollection upgrades)
        {
            PersonalizationController personalizationController = owner.GetComponent<PersonalizationController>();
            if (personalizationController)
            {
                personalizationController.OnUpgrade();
            }

            RobotWeaponBag robotWeaponBag = owner.GetComponent<RobotWeaponBag>();
            if (robotWeaponBag)
            {
                robotWeaponBag.OnUpgrade();
            }
        }

        public override void OnLanguageChanged(string newLanguageID, Dictionary<string, string> localizationDictionary)
        {
            if (newLanguageID.IsNullOrEmpty() || localizationDictionary.IsNullOrEmpty())
                return;

            ModLocalizationManager manager = ModLocalizationManager.Instance;
            if (manager)
            {
                manager.PopulateTranslationDictionary(ref localizationDictionary, newLanguageID);
            }

            FPSManager fpsManager = FPSManager.Instance;
            if (fpsManager)
            {
                fpsManager.RefreshDropdownOptionTranslation();
            }
        }

        public static void RefreshCursor(object obj)
        {
            RefreshCursor();
        }

        public static void RefreshCursor()
        {
            switch (CursorSkin)
            {
                case 1:
                    Cursor.SetCursor(ModResources.Texture2D(AssetBundleConstants.UI, "Cursor"), Vector2.zero, CursorMode.Auto);
                    break;
                case 2:
                    Cursor.SetCursor(ModResources.Texture2D(AssetBundleConstants.UI, "Cursor2"), Vector2.zero, CursorMode.Auto);
                    break;
                default:
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    break;
            }
        }

        private IEnumerator waitUntilCharacterModelInitialization(FirstPersonMover firstPersonMover)
        {
            while (firstPersonMover && (!firstPersonMover.gameObject.activeInHierarchy || !firstPersonMover.HasCharacterModel()))
                yield return null;

            yield return null;
            yield return null;
            yield return null;

            if (!firstPersonMover || !firstPersonMover.IsAttachedAndAlive())
                yield break;

            _ = firstPersonMover.gameObject.AddComponent<CharacterExtension>();
            _ = firstPersonMover.gameObject.AddComponent<PersonalizationController>();

            if (ModFeatures.IsEnabled(ModFeatures.FeatureType.WeaponBag))
            {
                bool isAllowedOnEnemies = GameModeManager.IsSinglePlayer() || GameModeManager.IsCoop();

                if (!firstPersonMover.IsMindSpaceCharacter && (isAllowedOnEnemies || firstPersonMover.IsMainPlayer()))
                {
                    _ = firstPersonMover.gameObject.AddComponent<RobotWeaponBag>();
                }
            }

            if (GameModeManager.IsCoop())
            {
                firstPersonMover.gameObject.AddComponent<WeaponInvisibilityFixer>().Initialize(firstPersonMover);
            }

            if (XRSettings.enabled)
                yield break;

            while (firstPersonMover && !firstPersonMover._playerCamera || ((GameModeManager.IsMultiplayerDuel() || GameModeManager.IsBattleRoyale()) && !firstPersonMover.HasConstructionFinished()))
                yield return null;

            if (firstPersonMover && firstPersonMover.HasCharacterModel() && firstPersonMover._playerCamera)
                CameraManager.Instance.AddControllers(firstPersonMover._playerCamera, firstPersonMover);

            yield break;
        }

        private IEnumerator waitThenFixArenaLiftInCoop()
        {
            yield return new WaitForSeconds(3f);
            if (!ArenaLiftManager.Instance)
                yield break;

            ArenaLift lift = ArenaLiftManager.Instance.Lift;
            if (lift && (lift._state == null || lift._stateHolder == null))
            {
                foreach (MovingPlatformStateHolder sh in Resources.FindObjectsOfTypeAll<MovingPlatformStateHolder>())
                {
                    BoltEntity boltEntity = sh.GetComponent<BoltEntity>();
                    if (!boltEntity || !boltEntity.IsAttached)
                        continue;

                    if (sh.state.UniqueIndex == lift.GetUniqueIndex())
                    {
                        lift._state = sh.state;
                        lift._stateHolder = sh;
                        break;
                    }
                }
            }
            yield break;
        }

        private void onGameInitialized()
        {
            ModManagers.Instance.TriggerGameLoadedEvent();
            if (ShouldStartCustomizationEditor)
            {
                ShouldStartCustomizationEditor = false;
                if (PersonalizationEditorManager.Instance)
                    PersonalizationEditorManager.Instance.StartEditorGameMode(true);
            }
        }

        public static void TriggerOnCameraSwitchedEvent(Camera oldCamera, Camera newCamera)
        {
            OnCameraSwitched?.Invoke(oldCamera, newCamera);
        }
    }
}