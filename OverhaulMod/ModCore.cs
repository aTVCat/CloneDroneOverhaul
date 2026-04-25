using ModLibrary;
using OverhaulMod.Content.Personalization;
using OverhaulMod.Engine;
using OverhaulMod.Gameplay;
using OverhaulMod.Utils;
using OverhaulMod.Visuals;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod
{
    [MainModClass]
    public class ModCore : Mod
    {
        [ModSetting(ModSettingsConstants.ENABLE_TITLE_BAR_OVERHAUL, true)]
        public static bool EnableTitleBarOverhaul;

        [ModSetting(ModSettingsConstants.SHOW_SPEAKER_NAME, true)]
        public static bool ShowSpeakerName;

        [ModSetting(ModSettingsConstants.SWAP_SUBTITLES_COLOR, false)]
        public static bool SwapSubtitlesColor;

        public static ModCore Instance { get; private set; }

        public static bool IsEnabled { get; private set; }

        public override void OnModLoaded()
        {
            Instance = this;
            IsEnabled = true;
            ModLoader.Load(true);
        }

        public override void OnModEnabled()
        {
            Instance = this;
            IsEnabled = true;
            ModLoader.Load(false);
        }

        public override void OnModDeactivated()
        {
            Instance = null;
            IsEnabled = false;
            ModLoader.Unload();
        }

        public override void OnClientConnectedToServer()
        {
            PersonalizationMultiplayerManager.Instance.SendPlayerCustomizationDataEvent(false);
            ArenaRemodelManager.Instance.PatchVanillaParts(false);
            ArenaRemodelManager.Instance.FixLiftInCoop();
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
            ModCharacterManager.Instance.OnFirstPersonMoverSpawned(firstPersonMover);
        }

        public override void OnUpgradesRefreshed(FirstPersonMover owner, UpgradeCollection upgrades)
        {
            ModCharacterManager.Instance.OnUpgradesStartedRefreshing(owner, upgrades);
        }

        public override void AfterUpgradesRefreshed(FirstPersonMover owner, UpgradeCollection upgrades)
        {
            ModCharacterManager.Instance.OnUpgradesRefreshed(owner, upgrades);
        }

        public override void OnLanguageChanged(string newLanguageID, Dictionary<string, string> localizationDictionary)
        {
            ModLocalizationManager manager = ModLocalizationManager.Instance;
            if (manager)
            {
                manager.PopulateTranslationDictionary(ref localizationDictionary, newLanguageID);
                manager.RefreshMiscTranslations();
            }
        }
    }
}