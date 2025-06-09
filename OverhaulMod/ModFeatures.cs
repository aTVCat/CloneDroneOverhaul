using OverhaulMod.Content;
using System.Collections.Generic;

namespace OverhaulMod
{
    public static class ModFeatures
    {
        private static readonly Dictionary<FeatureType, bool> m_cachedValues = new Dictionary<FeatureType, bool>();

        public static void CacheValues()
        {
            Dictionary<FeatureType, bool> d = m_cachedValues;
            d.Clear();

            foreach (FeatureType feature in typeof(FeatureType).GetEnumValues())
                d.Add(feature, IsEnabled(feature, false));
        }

        public static bool IsEnabled(FeatureType feature, bool useCaching = true)
        {
            if (useCaching)
            {
                Dictionary<FeatureType, bool> d = m_cachedValues;
                if (d != null && d.ContainsKey(feature))
                    return d[feature];
            }

            bool result;
            switch (feature)
            {
                case FeatureType.EndlessModeMenu:
                    result = true;
                    break;
                case FeatureType.NewWeapons:
                    result = true;
                    break;
                case FeatureType.WeaponBag:
                    result = ModBuildInfo.VERSION_5_0;
                    break;
                case FeatureType.ChapterSelectMenuRework:
                    result = true;
                    break;
                case FeatureType.LoadingScreenRework:
                    result = true;
                    break;
                case FeatureType.PauseMenuRework:
                    result = true;
                    break;
                case FeatureType.TitleScreenRework:
                    result = true;
                    break;
                case FeatureType.ArrowModelRefresh:
                    result = true;
                    break;
                case FeatureType.WeatherSystem:
                    result = ModBuildInfo.VERSION_4_3;
                    break;
                case FeatureType.WorkshopBrowserRework:
                    result = true;
                    break;
                case FeatureType.NightmariumDifficultyTier:
                    result = ModBuildInfo.VERSION_5_0;
                    break;
                case FeatureType.GameModeSelectionScreensRework:
                    result = true;
                    break;
                case FeatureType.NewGameplayContent:
                    result = ModBuildInfo.VERSION_5_0;
                    break;
                case FeatureType.NewEnemies:
                    result = ModBuildInfo.VERSION_5_0;
                    break;
                case FeatureType.DuelInviteMenuRework:
                    result = true;
                    break;
                case FeatureType.Tooltips:
                    result = false;
                    break;
                case FeatureType.Pets:
                    result = false;
                    break;
                case FeatureType.AdvancedSettings:
                    result = true;
                    break;
                case FeatureType.WorkshopBrowserContextMenu:
                    result = false;
                    break;
                case FeatureType.StoryModeModifiers:
                    result = false;
                    break;
                case FeatureType.QuickReset:
                    result = false;
                    break;
                case FeatureType.WinLoseDialogRework:
                    result = false;
                    break;
                case FeatureType.ShieldSkins:
                    result = false;
                    break;
                case FeatureType.PersonalizationEditorTutorialVideo:
                    result = true;
                    break;
                case FeatureType.ScytheSkins:
                    result = true;
                    break;
                case FeatureType.MoreImageEffects:
                    result = ModBuildInfo.VERSION_4_2_PATCH;
                    break;
                case FeatureType.SubtitleTextFieldRework:
                    result = true;
                    break;
                case FeatureType.UpdatesMenuRework:
                    result = ModBuildInfo.VERSION_4_2_PATCH;
                    break;
                case FeatureType.RequireNormalAndFireVariantsForSwordAndSpearSkins:
                    result = false;
                    break;
                case FeatureType.DisplayDisconnectedPlayers:
                    result = true;
                    break;
                case FeatureType.Intro:
                    result = ModBuildInfo.VERSION_4_2_PATCH;
                    break;
                case FeatureType.VersionLabelUpdates:
                    result = ModBuildInfo.VERSION_4_2_PATCH;
                    break;
                case FeatureType.TitleScreenModdedSectionRework:
                    result = ModBuildInfo.VERSION_4_2_PATCH;
                    break;
                case FeatureType.SettingDescriptionBox:
                    result = ModBuildInfo.VERSION_4_2_PATCH;
                    break;
                case FeatureType.ColorBlindnessOptions:
                    result = ModBuildInfo.VERSION_4_2_PATCH;
                    break;
                case FeatureType.ShowUpgradeUIExitButtonInLBS:
                    result = false;
                    break;
                case FeatureType.MoreSettingsMod_ColorBlindnessOptions:
                    result = false;
                    break;
                case FeatureType.ReflectionProbe:
                    result = ModBuildInfo.VERSION_4_2_PATCH;
                    break;
                case FeatureType.Accessories:
                    result = ModBuildInfo.VERSION_4_3;
                    break;
                case FeatureType.AdditionalGraphicsSettings:
                    result = ModBuildInfo.VERSION_4_2_PATCH;
                    break;
                case FeatureType.CustomizationMenuUpdates:
                    result = ModBuildInfo.VERSION_4_2_PATCH;
                    break;
                case FeatureType.UpdatedTransitions:
                    result = ModBuildInfo.VERSION_4_2_PATCH;
                    break;
                case FeatureType.RevertUpgrades:
                    result = ModBuildInfo.VERSION_5_0;
                    break;
                case FeatureType.DisplayNewGraphicsOptionsInSettings:
                    result = false;
                    break;
                case FeatureType.WorkshopBrowserHistoryAndCheckpoints:
                    result = ModBuildInfo.VERSION_4_3;
                    break;
                case FeatureType.UISounds:
                    result = ModBuildInfo.VERSION_4_4;
                    break;
                case FeatureType.TransitionTips:
                    result = false;
                    break;
                default:
                    return false;
            }

            if (!result)
            {
                ExclusivePerkManager exclusivePerkManager = ExclusivePerkManager.Instance;
                if (exclusivePerkManager && exclusivePerkManager.IsFeatureUnlocked(feature))
                    result = true;
            }
            return result;
        }

        public enum FeatureType
        {
            EndlessModeMenu,

            ChapterSelectMenuRework,

            LoadingScreenRework,

            PauseMenuRework,

            TitleScreenRework,

            ArrowModelRefresh,

            NewWeapons,

            WeaponBag,

            WeatherSystem,

            WorkshopBrowserRework,

            NightmariumDifficultyTier,

            GameModeSelectionScreensRework,

            NewGameplayContent,

            NewEnemies,

            DuelInviteMenuRework,

            Tooltips,

            Pets,

            AdvancedSettings,

            WinLoseDialogRework,

            WorkshopBrowserContextMenu,

            StoryModeModifiers,

            QuickReset,

            ShieldSkins,

            PersonalizationEditorTutorialVideo,

            ScytheSkins,

            MoreImageEffects,

            SubtitleTextFieldRework,

            UpdatesMenuRework,

            RequireNormalAndFireVariantsForSwordAndSpearSkins,

            DisplayDisconnectedPlayers,

            ShowUpgradeUIExitButtonInLBS,

            Intro,

            VersionLabelUpdates,

            TitleScreenModdedSectionRework,

            SettingDescriptionBox,

            ColorBlindnessOptions,

            MoreSettingsMod_ColorBlindnessOptions,

            ReflectionProbe,

            Accessories,

            AdditionalGraphicsSettings,

            CustomizationMenuUpdates,

            UpdatedTransitions,

            RevertUpgrades,

            DisplayNewGraphicsOptionsInSettings,

            WorkshopBrowserHistoryAndCheckpoints,

            UISounds,

            TransitionTips,
        }
    }
}
