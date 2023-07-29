//#define AllowLevelDataPatches

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CDOverhaul
{
    public static class OverhaulFeatureAvailabilitySystem
    {
        private static readonly ReadOnlyCollection<OverhaulFeatureDefinition> s_Features = new ReadOnlyCollection<OverhaulFeatureDefinition>(new List<OverhaulFeatureDefinition>()
        {
            createNew(OverhaulFeatureID.PermissionToManageSkins),
            createNew(OverhaulFeatureID.PermissionToCopyUserInfos)
        });

        private static OverhaulFeatureDefinition createNew(OverhaulFeatureID id)
        {
            OverhaulFeatureDefinition def;
            switch (id)
            {
                case OverhaulFeatureID.PermissionToManageSkins:
                    def = new OverhaulFeatureDefinition.AbilityToManageSkins();
                    break;
                case OverhaulFeatureID.PermissionToCopyUserInfos:
                    def = new OverhaulFeatureDefinition.AbilityToCopyUserInfos();
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

            OverhaulFeatureDefinition def = getFeatureDefinition(featureID);
            return def != null && def.IsAvailable();
        }

        private static OverhaulFeatureDefinition getFeatureDefinition(in OverhaulFeatureID featureID)
        {
            if (s_Features.IsNullOrEmpty())
                return null;

            int i = 0;
            do
            {
                OverhaulFeatureDefinition def = s_Features[i];
                if (def != null && def.FeatureID == featureID)
                    return def;
                i++;
            } while (i < s_Features.Count);
            return null;
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
            public static bool IsSelectionOutLineEnabled => !OverhaulVersion.IsVersion2 && IS_SELECTION_OUTLINE_ENABLED;

            /// <summary>
            /// VFX that plays when switching skins
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.Skin Switch VFX", false, false, "")]
            private static readonly bool IS_SKIN_SWITCHING_VFX_ENABLED = false;
            public static bool IsSkinSwitchingVFXEnabled => !OverhaulVersion.IsVersion2 && IS_SKIN_SWITCHING_VFX_ENABLED;

            /// <summary>
            /// Allow creating private matches with additional things allowed
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.Custom Multiplayer", false, false, "")]
            private static readonly bool IS_CUSTOM_MULTIPLAYER_TEST_ENABLED = false;
            public static bool IsCustomMultiplayerTestEnabled => !OverhaulVersion.IsVersion2 && IS_CUSTOM_MULTIPLAYER_TEST_ENABLED;

            /// <summary>
            /// Better way of saving/loading levels
            /// </summary>
#if AllowLevelDataPatches
            private static readonly bool IS_NEW_SAVE_AND_LOAD_SYSTEM_ENABLED = true;
#else
            private static readonly bool IS_NEW_SAVE_AND_LOAD_SYSTEM_ENABLED = false;
#endif
            public static bool IsNewSaveAndLoadSystemEnabled => !OverhaulVersion.IsVersion2 && IS_NEW_SAVE_AND_LOAD_SYSTEM_ENABLED;

            /// <summary>
            /// Make tickboxes use player favourite color
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.More Theme Color", false, false, "")]
            private static readonly bool APPLY_THEME_COLOR_ON_SETTINGS = false;
            public static bool ApplyThemeColorOnSettings => !OverhaulVersion.IsVersion2 && APPLY_THEME_COLOR_ON_SETTINGS;

            /// <summary>
            /// First person mode
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.View Modes Feature", true, false, "Enable first person mode")]
            private static readonly bool IS_VIEW_MODES_SETTING_ENABLED = true;
            public static bool IsViewModesSettingsEnabled => !OverhaulVersion.IsVersion2 && IS_VIEW_MODES_SETTING_ENABLED;

            /// <summary>
            /// Startup screen
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.Boot Screen", true, false, "Overhaul logo on game start")]
            private static readonly bool IS_BOOT_SCREEN_ENABLED = true;
            public static bool IsBootScreenEnabled => !OverhaulVersion.IsVersion2 && IS_BOOT_SCREEN_ENABLED;

            /// <summary>
            /// 
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.Discord Servers Panel", true, false, "")]
            private static readonly bool IS_DISCORD_PANEL_ENABLED = true;
            public static bool IsDiscordPanelEnabled => !OverhaulVersion.IsVersion2 && IS_DISCORD_PANEL_ENABLED;

            /// <summary>
            /// 
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.Enable Tooltips", true, false, "")]
            private static readonly bool ARE_TOOLTIPS_ENABLED = true;
            public static bool AreTooltipsEnabled => !OverhaulVersion.IsVersion2 && ARE_TOOLTIPS_ENABLED;

            /// <summary>
            /// 
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.Gamemodes UI Overhaul", true, false, "")]
            private static readonly bool IS_OVERHAULED_GAMEMODES_UI_ENABLED = true;
            public static bool IsOverhaulGamemodesUIEnabled => !OverhaulVersion.IsVersion2 && IS_OVERHAULED_GAMEMODES_UI_ENABLED;

            /// <summary>
            /// 
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.Audio Reverb Filter", true, false, "")]
            private static readonly bool IS_AUDIO_REVERB_FILTER_ENABLED = true;
            public static bool IsAudioReverbFilterEnabled => !OverhaulVersion.IsVersion2 && IS_AUDIO_REVERB_FILTER_ENABLED;

            /// <summary>
            /// 
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.First Use Setup", true, false, "")]
            private static readonly bool IS_FIRST_USE_SETUP_UI_ENABLED = true;
            public static bool IsFirstUseSetupUIEnabled => !OverhaulVersion.IsVersion2 && IS_FIRST_USE_SETUP_UI_ENABLED;

            /// <summary>
            /// 
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.Combat Overhaul", true, false, "Faster arrows, hammers collide with each other")]
            private static readonly bool IS_COMBAT_OVERHAUL_ENABLED = true;
            public static bool IsCombatOverhaulEnabled => !OverhaulVersion.IsVersion2 && IS_COMBAT_OVERHAUL_ENABLED;

            /// <summary>
            /// 
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.Robot Effects", true, false, "Sparks on death")]
            private static readonly bool ARE_ROBOT_EFFECTS_ENABLED = true;
            public static bool AreRobotEffectsEnabled => !OverhaulVersion.IsVersion2 && ARE_ROBOT_EFFECTS_ENABLED;

            /// <summary>
            /// Enable the transition initially used in prototype builds
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.New Transition Screen", true, false, "With animation!")]
            private static readonly bool IS_NEW_TRANSITION_SCREEN_ENABLED = true;
            public static bool IsNewTransitionScreenEnabled => !OverhaulVersion.IsVersion2 && IS_NEW_TRANSITION_SCREEN_ENABLED;

            /// <summary>
            /// 
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.Energy UI Updates", true, false, "")]
            private static readonly bool ARE_ENERGY_UI_IMPROVEMENTS_ENABLED = true;
            public static bool AreEnergyUIImprovementsEnabled => !OverhaulVersion.IsVersion2 && ARE_ENERGY_UI_IMPROVEMENTS_ENABLED;

            /// <summary>
            /// 
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.Workshop Browser Overhaul2", true, false, "")]
            private static readonly bool IS_NEW_WORKSHOP_BROWSER_ENABLED = true;
            public static bool IsNewWorkshopBrowserEnabled => !OverhaulVersion.IsVersion2 && IS_NEW_WORKSHOP_BROWSER_ENABLED;

            /// <summary>
            /// 
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.Pets DEMOv2", false, false, "")]
            private static readonly bool IS_PETS_DEMO = false;
            public static bool IsPetsDemo => OverhaulVersion.IsVersion2 || IS_PETS_DEMO;

            /// <summary>
            /// Revert upgrade by right click
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.Allow reverting upgrades", true, false, "")]
            private static readonly bool ALLOW_REVERTING_UPGRADES = true;
            public static bool AllowReveringUpgrades => !OverhaulVersion.IsVersion2 && ALLOW_REVERTING_UPGRADES;

            /// <summary>
            /// Do some changes to usual level editor UI
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.Vanilla level editor overhaul", false, false, "")]
            private static readonly bool IS_VANILLA_LVL_EDITOR_UI_OVERHAUL_ENABLED = false;
            public static bool IsVanillaLevelEditorUIOverhaulEnabled => OverhaulVersion.IsVersion4 && IS_VANILLA_LVL_EDITOR_UI_OVERHAUL_ENABLED;

            /// <summary>
            /// Overhaul level editor UI completely
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.Full level editor overhaul", false, false, "")]
            private static readonly bool IS_FULL_LVL_EDITOR_UI_OVERHAUL_ENABLED = false;
            public static bool IsFullLevelEditorUIOverhaulEnabled => OverhaulVersion.IsVersion4 && IS_FULL_LVL_EDITOR_UI_OVERHAUL_ENABLED;

            /// <summary>
            /// More functional photo mode
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.Photo mode overhaul", true, false, "")]
            private static readonly bool IS_PHOTO_MODE_OVERHAUL_ENABLED = true;
            public static bool IsPhotoModeOverhaulEnabled => OverhaulVersion.IsVersion3Update && IS_PHOTO_MODE_OVERHAUL_ENABLED;

            /// <summary>
            /// A way to optimize huge levels
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.GPU Instancing", false, false, "")]
            private static readonly bool IS_GPU_INSTANCING_ENABLED = false;
            public static bool IsGpuInstancingEnabled => OverhaulVersion.IsVersion4 && IS_GPU_INSTANCING_ENABLED;

            /// <summary>
            /// The redesign of game mode select screen
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.GameModeSelectScreen Redesign", true, false, "")]
            private static readonly bool IS_GAMEMODESELECTSCREEN_REDESIGN_ENABLED = true;
            public static bool IsGameModeSelectScreenRedesignEnabled => !OverhaulVersion.IsVersion2 && IS_GAMEMODESELECTSCREEN_REDESIGN_ENABLED;

            /// <summary>
            /// Arena overhaul
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.Arena overhaul", true, false, "")]
            private static readonly bool IS_ARENA_OVERHAUL_ENABLED = true;
            public static bool IsArenaOverhaulEnabled => !OverhaulVersion.IsVersion2 && IS_ARENA_OVERHAUL_ENABLED;

            /// <summary>
            /// 
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.Additional content support", true, false, "")]
            private static readonly bool IS_ADDITIONAL_CONTENT_SUPPORT_ENABLED = true;
            public static bool IsAdditionalContentSupportEnabled => !OverhaulVersion.IsVersion2 && IS_ADDITIONAL_CONTENT_SUPPORT_ENABLED;

            /// <summary>
            /// 
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.Achievements Menu Redesign", true, false, "")]
            private static readonly bool IS_ACHIEVEMENTS_MENU_REDESIGN_ENABLED = true;
            public static bool IsAchievementsMenuRedesignEnabled => OverhaulVersion.IsVersion4 && IS_ACHIEVEMENTS_MENU_REDESIGN_ENABLED;

            /// <summary>
            /// 
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.Connection screens redesign", true, false, "")]
            private static readonly bool IS_CONNECTION_SCREENS_REDESIGN_ENABLED = true;
            public static bool IsConnectionScreensRedesignEnabled => OverhaulVersion.IsVersion4 && IS_CONNECTION_SCREENS_REDESIGN_ENABLED;

            /// <summary>
            /// 
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.New play info sync way", true, false, "")]
            private static readonly bool IS_NEW_PLAYER_INFO_SYNC_MECHANISM_ENABLED = true;
            public static bool IsNewPlayerInfoSyncMechanismEnabled => IS_NEW_PLAYER_INFO_SYNC_MECHANISM_ENABLED;

            /// <summary>
            /// 
            /// </summary>
            [OverhaulSettingWithNotification(1)]
            [OverhaulSetting("Experimental.Features.New personalization system", true, false, "")]
            private static readonly bool IS_NEW_PERSONALIZATION_SYSTEM_ENABLED = true;
            public static bool IsNewPersonalizationSystemEnabled => OverhaulVersion.IsVersion3Update && IS_NEW_PERSONALIZATION_SYSTEM_ENABLED;


            public const bool IS_DEVELOPER_ALLOWED_TO_USE_LOCKED_STUFF = true;
        }
    }
}
