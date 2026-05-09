using InternalModBot;
using ModLibrary;
using OverhaulMod.Content.Personalization;
using OverhaulMod.Engine;
using OverhaulMod.Gameplay;
using OverhaulMod.Visuals;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod
{
    [MainModClass]
    public class ModCore : Mod
    {
        public static ModCore Instance { get; private set; }

        private static bool s_enabled;

        private static bool s_restrictLaunching;

        public static bool IsActive() => !s_restrictLaunching && s_enabled;

        public override void OnModLoaded()
        {
            if (s_restrictLaunching) return;

            Instance = this;

            checkGameVersion();

            s_enabled = true;

            ModLoader.Load(true);
        }

        public override void OnModEnabled()
        {
            if (s_restrictLaunching) return;

            Instance = this;
            s_enabled = true;
            ModLoader.Load(false);
        }

        public override void OnModDeactivated()
        {
            if (s_restrictLaunching) return;

            Instance = null;
            s_enabled = false;
            ModLoader.Unload();
        }

        public override void OnClientConnectedToServer()
        {
            if (s_restrictLaunching) return;

            PersonalizationMultiplayerManager.Instance.SendPlayerCustomizationDataEvent(false);
            ArenaRemodelManager.Instance.PatchVanillaParts(false);
            ArenaRemodelManager.Instance.FixLiftInCoop();
        }

        public override void OnLevelEditorStarted()
        {
            if (s_restrictLaunching) return;

            ArenaRemodelManager.Instance.SetUpperInteriorActive(false);
            PostEffectsManager.Instance.RefreshCameraPostEffects();
        }

        public override void OnObjectPlacedInLevelInitialized(ObjectPlacedInLevel objectPlacedInLevel, Transform levelRoot)
        {
            if (s_restrictLaunching) return;

            LevelObjectEntry levelObjectEntry = objectPlacedInLevel.LevelObjectEntry;
            if (levelObjectEntry != null && (levelObjectEntry.PathUnderResources == RealisticLightingManager.LightSettingsObjectResourcePath || levelObjectEntry.PathUnderResources == RealisticLightingManager.LightSettingsOverrideObjectResourcePath) && !objectPlacedInLevel.GetComponent<AdditionalSkyboxSettings>())
            {
                objectPlacedInLevel.gameObject.AddComponent<AdditionalSkyboxSettings>();
            }
        }

        public override void OnMultiplayerEventReceived(GenericStringForModdingEvent moddedEvent)
        {
            if (s_restrictLaunching) return;

            PersonalizationMultiplayerManager.Instance.OnEvent(moddedEvent);
        }

        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover)
        {
            if (s_restrictLaunching) return;

            ModCharacterManager.Instance.OnFirstPersonMoverSpawned(firstPersonMover);
        }

        public override void OnUpgradesRefreshed(FirstPersonMover owner, UpgradeCollection upgrades)
        {
            if (s_restrictLaunching) return;

            ModCharacterManager.Instance.OnUpgradesStartedRefreshing(owner, upgrades);
        }

        public override void AfterUpgradesRefreshed(FirstPersonMover owner, UpgradeCollection upgrades)
        {
            if (s_restrictLaunching) return;

            ModCharacterManager.Instance.OnUpgradesRefreshed(owner, upgrades);
        }

        public override void OnLanguageChanged(string newLanguageID, Dictionary<string, string> localizationDictionary)
        {
            if (s_restrictLaunching) return;

            ModLocalizationManager manager = ModLocalizationManager.Instance;
            if (manager)
            {
                manager.PopulateTranslationDictionary(ref localizationDictionary, newLanguageID);
                manager.RefreshMiscTranslations();
            }
        }

        private void checkGameVersion()
        {
            VersionNumberManager versionNumberManager = VersionNumberManager.Instance;
            if (!versionNumberManager)
            {
                s_restrictLaunching = true;
                throw new Exception("VersionNumberManager not found!\n");
            }

            string versionString = versionNumberManager.GetVersionString();
            if (!Version.TryParse(versionString, out Version gameVersion))
            {
                s_restrictLaunching = true;
                throw new Exception("Could not parse the game version string!\n");
            }

            if (!ModBuild.IsGameVersionSupported(gameVersion))
            {
                s_restrictLaunching = true;
                throw new Exception($"{"Clone Drone".AddColor(Color.orange)} must be on version {ModBuild.MinimumGameVersion.ToString().AddColor(Color.cyan)} or higher (You're on {versionString.AddColor(Color.yellow)}). Update the game.\n");
            }

            checkModBotVersion();
        }

        private void checkModBotVersion()
        {
            Version modBotVersion = typeof(StartupManager).Assembly.GetName().Version;
            if (!ModBuild.IsModBotVersionSupported(modBotVersion))
            {
                s_restrictLaunching = true;
                throw new Exception($"{"Mod-Bot".AddColor(Color.orange)} must be on version {ModBuild.MinimumModBotVersion.ToString().AddColor(Color.cyan)} or higher (You're on {modBotVersion.ToString().AddColor(Color.yellow)}). Update Mod-Bot.\n");
            }
        }
    }
}