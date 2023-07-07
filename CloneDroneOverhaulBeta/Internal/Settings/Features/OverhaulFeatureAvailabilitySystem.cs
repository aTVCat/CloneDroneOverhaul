//#define AllowLevelDataPatches

using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace CDOverhaul
{
    public static class OverhaulFeatureAvailabilitySystem
    {
        private static readonly ReadOnlyCollection<OverhaulFeatureDefinition> m_Features = new ReadOnlyCollection<OverhaulFeatureDefinition>(new List<OverhaulFeatureDefinition>()
        {
            CreateNew(OverhaulFeatureID.PermissionToManageSkins)
        });

        private static OverhaulFeatureDefinition CreateNew(OverhaulFeatureID id)
        {
            OverhaulFeatureDefinition def;
            switch (id)
            {
                case OverhaulFeatureID.PermissionToManageSkins:
                    def = new OverhaulFeatureDefinition.AbilityToManageSkins();
                    break;
                default:
                    def = new OverhaulFeatureDefinition();
                    break;
            }
            def.FeatureID = id;

            return def;
        }

        public static bool IsFeatureUnlocked(in OverhaulFeatureID featureID)
        {
            if (OverhaulVersion.IsDebugBuild)
                return true;

            OverhaulFeatureDefinition def = GetFeatureDefinition(featureID);
            return def != null && def.IsAvailable();
        }

        public static OverhaulFeatureDefinition GetFeatureDefinition(in OverhaulFeatureID featureID)
        {
            if (m_Features.IsNullOrEmpty())
                return null;

            int i = 0;
            do
            {
                OverhaulFeatureDefinition def = m_Features[i];
                if (def != null && def.FeatureID == featureID)
                    return def;
                i++;
            } while (i < m_Features.Count);
            return null;
        }

        public static string GetStringOfFeature(in OverhaulFeatureID featureID)
        {
            switch (featureID)
            {
                case OverhaulFeatureID.PermissionToManageSkins:
                    return "Permission to manage skins"; // Todo: Translate all
                case OverhaulFeatureID.PermissionToEditLocalization:
                    return "Permission to translate the mod";
            }
            return "Unknown feature (" + featureID.ToString() + ")";
        }

        public static string GetColoredStringOfFeature(in OverhaulFeatureID featureID)
        {
            return GetStringOfFeature(featureID).AddColor(IsFeatureUnlocked(featureID) ? Color.white : Color.gray);
        }

        /// <summary>
        /// Features that doesn't require player IDs to work. This class defines what things should be included in the build
        /// </summary>
        public static class ImplementedInBuild
        {
            /// <summary>
            /// Level editor selection outline
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.Level Editor Selection Outline", true, false, "Selected objects have outline")]
            private static readonly bool IS_SELECTION_OUTLINE_ENABLED = true;
            public static bool IsSelectionOutLineEnabled => !OverhaulVersion.IsUpdate2 && IS_SELECTION_OUTLINE_ENABLED;

            /// <summary>
            /// VFX that plays when switching skins
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.Skin Switch VFX", false, false, "")]
            private static readonly bool IS_SKIN_SWITCHING_VFX_ENABLED = false;
            public static bool IsSkinSwitchingVFXEnabled => !OverhaulVersion.IsUpdate2 && IS_SKIN_SWITCHING_VFX_ENABLED;

            /// <summary>
            /// Allow creating private matches with additional things allowed
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.Custom Multiplayer", false, false, "")]
            private static readonly bool IS_CUSTOM_MULTIPLAYER_TEST_ENABLED = false;
            public static bool IsCustomMultiplayerTestEnabled => !OverhaulVersion.IsUpdate2 && IS_CUSTOM_MULTIPLAYER_TEST_ENABLED;

            /// <summary>
            /// Better way of saving/loading levels
            /// </summary>
#if AllowLevelDataPatches
            private static readonly bool IS_NEW_SAVE_AND_LOAD_SYSTEM_ENABLED = true;
#else
            private static readonly bool IS_NEW_SAVE_AND_LOAD_SYSTEM_ENABLED = false;
#endif
            public static bool IsNewSaveAndLoadSystemEnabled => !OverhaulVersion.IsUpdate2 && IS_NEW_SAVE_AND_LOAD_SYSTEM_ENABLED;

            /// <summary>
            /// Make tickboxes use player favourite color
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.More Theme Color", false, false, "")]
            private static readonly bool APPLY_THEME_COLOR_ON_SETTINGS = false;
            public static bool ApplyThemeColorOnSettings => !OverhaulVersion.IsUpdate2 && APPLY_THEME_COLOR_ON_SETTINGS;

            /// <summary>
            /// First person mode
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.View Modes Feature", true, false, "Enable first person mode")]
            private static readonly bool IS_VIEW_MODES_SETTING_ENABLED = true;
            public static bool IsViewModesSettingsEnabled => !OverhaulVersion.IsUpdate2 && IS_VIEW_MODES_SETTING_ENABLED;

            /// <summary>
            /// Startup screen
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.Boot Screen", true, false, "Overhaul logo on game start")]
            private static readonly bool IS_BOOT_SCREEN_ENABLED = true;
            public static bool IsBootScreenEnabled => !OverhaulVersion.IsUpdate2 && IS_BOOT_SCREEN_ENABLED;

            /// <summary>
            /// 
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.Discord Servers Panel", true, false, "")]
            private static readonly bool IS_DISCORD_PANEL_ENABLED = true;
            public static bool IsDiscordPanelEnabled => !OverhaulVersion.IsUpdate2 && IS_DISCORD_PANEL_ENABLED;

            /// <summary>
            /// 
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.Enable Tooltips", true, false, "")]
            private static readonly bool ARE_TOOLTIPS_ENABLED = true;
            public static bool AreTooltipsEnabled => !OverhaulVersion.IsUpdate2 && ARE_TOOLTIPS_ENABLED;

            /// <summary>
            /// 
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.Gamemodes UI Overhaul", true, false, "")]
            private static readonly bool IS_OVERHAULED_GAMEMODES_UI_ENABLED = true;
            public static bool IsOverhaulGamemodesUIEnabled => !OverhaulVersion.IsUpdate2 && IS_OVERHAULED_GAMEMODES_UI_ENABLED;

            /// <summary>
            /// 
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.Audio Reverb Filter", true, false, "")]
            private static readonly bool IS_AUDIO_REVERB_FILTER_ENABLED = true;
            public static bool IsAudioReverbFilterEnabled => !OverhaulVersion.IsUpdate2 && IS_AUDIO_REVERB_FILTER_ENABLED;

            /// <summary>
            /// 
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.First Use Setup", true, false, "")]
            private static readonly bool IS_FIRST_USE_SETUP_UI_ENABLED = true;
            public static bool IsFirstUseSetupUIEnabled => !OverhaulVersion.IsUpdate2 && IS_FIRST_USE_SETUP_UI_ENABLED;

            /// <summary>
            /// 
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.Combat Overhaul", true, false, "Faster arrows, hammers collide with each other")]
            private static readonly bool IS_COMBAT_OVERHAUL_ENABLED = true;
            public static bool IsCombatOverhaulEnabled => !OverhaulVersion.IsUpdate2 && IS_COMBAT_OVERHAUL_ENABLED;

            /// <summary>
            /// 
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.Robot Effects", true, false, "Sparks on death")]
            private static readonly bool ARE_ROBOT_EFFECTS_ENABLED = true;
            public static bool AreRobotEffectsEnabled => !OverhaulVersion.IsUpdate2 && ARE_ROBOT_EFFECTS_ENABLED;

            /// <summary>
            /// Enable the transition initially used in prototype builds
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.New Transition Screen", true, false, "With animation!")]
            private static readonly bool IS_NEW_TRANSITION_SCREEN_ENABLED = true;
            public static bool IsNewTransitionScreenEnabled => !OverhaulVersion.IsUpdate2 && IS_NEW_TRANSITION_SCREEN_ENABLED;

            /// <summary>
            /// 
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.Energy UI Updates", true, false, "")]
            private static readonly bool ARE_ENERGY_UI_IMPROVEMENTS_ENABLED = true;
            public static bool AreEnergyUIImprovementsEnabled => !OverhaulVersion.IsUpdate2 && ARE_ENERGY_UI_IMPROVEMENTS_ENABLED;

            /// <summary>
            /// 
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.Workshop Browser Overhaul2", true, false, "")]
            private static readonly bool IS_NEW_WORKSHOP_BROWSER_ENABLED = true;
            public static bool IsNewWorkshopBrowserEnabled => !OverhaulVersion.IsUpdate2 && IS_NEW_WORKSHOP_BROWSER_ENABLED;

            /// <summary>
            /// 
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.Pets DEMO", true, false, "")]
            private static readonly bool IS_PETS_DEMO = true;
            public static bool IsPetsDemo => OverhaulVersion.IsUpdate2 || IS_PETS_DEMO;

            /// <summary>
            /// Revert upgrade by right click
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.Allow reverting upgrades", true, false, "")]
            private static readonly bool ALLOW_REVERTING_UPGRADES = true;
            public static bool AllowReveringUpgrades => !OverhaulVersion.IsUpdate2 && ALLOW_REVERTING_UPGRADES;

            /// <summary>
            /// Do some changes to usual level editor UI
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.Vanilla level editor overhaul", false, false, "")]
            private static readonly bool IS_VANILLA_LVL_EDITOR_UI_OVERHAUL_ENABLED = false;
            public static bool IsVanillaLevelEditorUIOverhaulEnabled => OverhaulVersion.IsUpdate4 && IS_VANILLA_LVL_EDITOR_UI_OVERHAUL_ENABLED;

            /// <summary>
            /// Overhaul level editor UI completely
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.Full level editor overhaul", false, false, "")]
            private static readonly bool IS_FULL_LVL_EDITOR_UI_OVERHAUL_ENABLED = false;
            public static bool IsFullLevelEditorUIOverhaulEnabled => OverhaulVersion.IsUpdate4 && IS_FULL_LVL_EDITOR_UI_OVERHAUL_ENABLED;

            /// <summary>
            /// More functional photo mode
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.Photo mode overhaul", true, false, "")]
            private static readonly bool IS_PHOTO_MODE_OVERHAUL_ENABLED = true;
            public static bool IsPhotoModeOverhaulEnabled => !OverhaulVersion.IsUpdate2 && IS_PHOTO_MODE_OVERHAUL_ENABLED;


            public const bool IS_DEVELOPER_ALLOWED_TO_USE_LOCKED_STUFF = true;
        }
    }
}
