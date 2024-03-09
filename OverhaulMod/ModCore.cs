using ModLibrary;
using OverhaulMod.Combat;
using OverhaulMod.Content.Personalization;
using OverhaulMod.Engine;
using OverhaulMod.Patches.Addons;
using OverhaulMod.Utils;
using OverhaulMod.Visuals;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod
{
    [MainModClass]
    public class ModCore : Mod
    {
        [ModSetting(ModSettingsConstants.ENABLE_TITLE_BAR_OVERHAUL, true)]
        public static bool EnableTitleBarOverhaul;

        public static event Action GameInitialized;
        public static event Action<bool> ModStateChanged;
        public static event Action<Camera, Camera> OnCameraSwitched;

        public static ModCore instance { get; private set; }

        public static bool isEnabled
        {
            get;
            private set;
        }

        private static string s_folder;
        public static string folder
        {
            get
            {
                ModCore modCore = instance;
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
        public static string savesFolder
        {
            get
            {
                if (s_savesFolder == null)
                {
                    s_savesFolder = modDataFolder + "saves/";
                }
                return s_savesFolder;
            }
        }

        private static string s_assetsFolder;
        public static string assetsFolder
        {
            get
            {
                if (s_assetsFolder == null)
                {
                    s_assetsFolder = folder + "assets/";
                }
                return s_assetsFolder;
            }
        }

        private static string s_dataFolder;
        public static string dataFolder
        {
            get
            {
                if (s_dataFolder == null)
                {
                    s_dataFolder = assetsFolder + "data/";
                }
                return s_dataFolder;
            }
        }

        private static string s_modFolder;
        public static string modDataFolder
        {
            get
            {
                if (s_modFolder == null)
                {
                    s_modFolder = Application.persistentDataPath + "/OverhaulMod/";
                }
                return s_modFolder;
            }
        }

        private static string s_contentFolder;
        public static string contentFolder
        {
            get
            {
                if (s_contentFolder == null)
                {
                    s_contentFolder = modDataFolder + "content/";
                }
                return s_contentFolder;
            }
        }

        private static string s_addonsFolder;
        public static string addonsFolder
        {
            get
            {
                if (s_addonsFolder == null)
                {
                    s_addonsFolder = contentFolder + "addons/";
                }
                return s_addonsFolder;
            }
        }

        private static string s_customizationFolder;
        public static string customizationFolder
        {
            get
            {
                if (s_customizationFolder == null)
                {
                    s_customizationFolder = contentFolder + "customization/";
                }
                return s_customizationFolder;
            }
        }

        private static string s_textureFolder;
        public static string texturesFolder
        {
            get
            {
                if (s_textureFolder == null)
                {
                    s_textureFolder = assetsFolder + "textures/";
                }
                return s_textureFolder;
            }
        }

        private static string s_editorTexturesFolder;
        public static string editorTexturesFolder
        {
            get
            {
                if (s_editorTexturesFolder == null)
                {
                    s_editorTexturesFolder = texturesFolder + "editor/";
                }
                return s_editorTexturesFolder;
            }
        }

        public override void OnModEnabled()
        {
            instance = this;
            isEnabled = true;

            ModLoader.Load();
            GamePatchBehaviour.Load();

            TriggerGameInitializedEvent();
            TriggerModStateChangedEvent(true);

            ModSpecialUtils.SetTitleBarStateDependingOnSettings();
        }

        public override void OnModLoaded()
        {
            instance = this;

            ModLoader.Load();
        }

        public override void OnModDeactivated()
        {
            instance = null;
            isEnabled = false;

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

            TriggerModStateChangedEvent(false);

            ModSpecialUtils.SetTitleBarStateDependingOnSettings();

            GamePatchBehaviour.Unload();
            ModLoader.Unload();
        }

        public override UnityEngine.Object OnResourcesLoad(string path)
        {
            return LevelEditorPatch.Patch.GetResourceObject(path);
        }

        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover)
        {
            _ = ModActionUtils.RunCoroutine(waitUntilCharacterModelInitialization(firstPersonMover));
        }

        public override void OnUpgradesRefreshed(FirstPersonMover owner, UpgradeCollection upgrades)
        {
            owner.addWeaponToEquipppedIfHasUpgradeAndModelPresent(ModUpgradesManager.SCYTHE_UNLOCK_UPGRADE, ModWeaponsManager.SCYTHE_TYPE);
            owner.addWeaponToEquipppedIfHasUpgradeAndModelPresent(ModUpgradesManager.AXE_UNLOCK_UPGRADE, ModWeaponsManager.BATTLE_AXE_TYPE);
            owner.addWeaponToEquipppedIfHasUpgradeAndModelPresent(ModUpgradesManager.HALBERD_UNLOCK_UPGRADE, ModWeaponsManager.HALBERD_TYPE);
            owner.addWeaponToEquipppedIfHasUpgradeAndModelPresent(ModUpgradesManager.DUAL_KNIVES_UNLOCK_UPGRADE, ModWeaponsManager.DUAL_KNIVES_TYPE);
            owner.addWeaponToEquipppedIfHasUpgradeAndModelPresent(ModUpgradesManager.HANDS_UNLOCK_UPGRADE, ModWeaponsManager.HANDS_TYPE);
            owner.addWeaponToEquipppedIfHasUpgradeAndModelPresent(ModUpgradesManager.CLAWS_UNLOCK_UPGRADE, ModWeaponsManager.CLAWS_TYPE);
            owner.addWeaponToEquipppedIfHasUpgradeAndModelPresent(ModUpgradesManager.LASER_BLASTER_UPGRADE, ModWeaponsManager.PRIM_LASER_BLASTER_TYPE);
            owner.addWeaponToEquipppedIfHasUpgradeAndModelPresent(ModUpgradesManager.BOOMERANG_FIRE_UPGRADE, ModWeaponsManager.BOOMERANG_TYPE);
            owner.RefreshModWeaponModels();

            RobotInventory robotInventory = owner.GetComponent<RobotInventory>();
            if (robotInventory)
            {
                robotInventory.OnUpgradesRefreshed(upgrades);
            }
        }

        public override void OnLanguageChanged(string newLanguageID, Dictionary<string, string> localizationDictionary)
        {
            if (string.IsNullOrEmpty(newLanguageID) || localizationDictionary.IsNullOrEmpty())
                return;

            ModLocalizationManager manager = ModLocalizationManager.Instance;
            if (manager)
            {
                manager.PopulateTranslationDictionary(ref localizationDictionary, newLanguageID);
            }
        }

        private IEnumerator waitUntilCharacterModelInitialization(FirstPersonMover firstPersonMover)
        {
            yield return new WaitUntil(() => (!firstPersonMover || firstPersonMover.gameObject.activeInHierarchy) && firstPersonMover.HasCharacterModel());
            for (int i = 0; i < 3; i++)
                yield return null;

            if (!firstPersonMover || !firstPersonMover.IsAlive())
                yield break;

            PersonalizationManager.Instance.ConfigureFirstPersonMover(firstPersonMover);
            if (firstPersonMover.IsAttachedAndAlive())
            {
                if (ModFeatures.IsEnabled(ModFeatures.FeatureType.WeaponBag))
                    _ = firstPersonMover.gameObject.AddComponent<RobotWeaponBag>();

                RobotInventory robotInventory = firstPersonMover.gameObject.AddComponent<RobotInventory>();
            }

            yield return new WaitUntil(() => !firstPersonMover || firstPersonMover._playerCamera);
            if (firstPersonMover)
                CameraManager.Instance.AddControllers(firstPersonMover._playerCamera, firstPersonMover);

            yield break;
        }

        public static void TriggerGameInitializedEvent()
        {
            GameInitialized?.Invoke();
        }

        public static void TriggerModStateChangedEvent(bool enabled)
        {
            ModStateChanged?.Invoke(enabled);
        }

        public static void TriggerOnCameraSwitchedEvent(Camera oldCamera, Camera newCamera)
        {
            OnCameraSwitched?.Invoke(oldCamera, newCamera);
        }

        public static IEnumerator PushDownIfAboveGroundCoroutine_Patch(FirstPersonMover firstPersonMover)
        {
            while (firstPersonMover && !firstPersonMover.gameObject.activeInHierarchy)
                yield return null;

            yield return new WaitForSeconds(0.05f);
            if (!firstPersonMover || !firstPersonMover._characterController || firstPersonMover.IsRidingOtherCharacter() || GameFlowManager.Instance.IsInEditorMode())
                yield break;

            if (Physics.SphereCast(new Ray(firstPersonMover.transform.position + firstPersonMover.CenterOfCharacterOffset, -Vector3.up), firstPersonMover.CollisionRadius, out RaycastHit raycastHit, 20f, PhysicsManager.GetEnvironmentLayerMask(), QueryTriggerInteraction.Ignore))
            {
                float num = firstPersonMover.transform.position.y - raycastHit.point.y;
                if (num > 0f)
                {
                    firstPersonMover.transform.position += new Vector3(0f, -num, 0f);
                }
            }
            yield break;
        }
    }
}
