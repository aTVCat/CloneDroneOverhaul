﻿using OverhaulMod.Utils;
using System;

namespace OverhaulMod.Engine
{
    public class ScheduledActionsManager : Singleton<ScheduledActionsManager>
    {
        [ModSetting(ModSettingsConstants.REFRESH_MOD_UPDATES_DATE_TIME, null, ModSetting.Tag.IgnoreExport)]
        public static string RefreshModUpdatesDateTime;

        [ModSetting(ModSettingsConstants.REFRESH_EXCLUSIVE_PERKS_DATE_TIME, null, ModSetting.Tag.IgnoreExport)]
        public static string RefreshExclusivePerksDateTime;

        [ModSetting(ModSettingsConstants.REFRESH_CUSTOMIZATION_ASSETS_REMOTE_VERSION_DATE_TIME, null, ModSetting.Tag.IgnoreExport)]
        public static string RefreshCustomizationAssetsRemoteVersionDateTime;

        [ModSetting(ModSettingsConstants.REFRESH_NEWS_DATE_TIME, null, ModSetting.Tag.IgnoreExport)]
        public static string RefreshNewsDateTime;

        [ModSetting(ModSettingsConstants.REFRESH_ADDON_UPDATES_DATE_TIME, null, ModSetting.Tag.IgnoreExport)]
        public static string RefreshAddonUpdatesDateTime;

        [ModSetting(ModSettingsConstants.DISABLE_SCHEDULES, false)]
        public static bool DisableSchedules;

        public void SetActionExecuted(ScheduledActionType scheduledAction)
        {
            DateTime dateTime = DateTime.Now;
            switch (scheduledAction)
            {
                case ScheduledActionType.RefreshModUpdates:
                    ModSettingsManager.SetStringValue(ModSettingsConstants.REFRESH_MOD_UPDATES_DATE_TIME, dateTime.AddDays(2).ToString());
                    break;
                case ScheduledActionType.RefreshExclusivePerks:
                    ModSettingsManager.SetStringValue(ModSettingsConstants.REFRESH_EXCLUSIVE_PERKS_DATE_TIME, dateTime.AddDays(5).ToString());
                    break;
                case ScheduledActionType.RefreshCustomizationAssetsRemoteVersion:
                    ModSettingsManager.SetStringValue(ModSettingsConstants.REFRESH_CUSTOMIZATION_ASSETS_REMOTE_VERSION_DATE_TIME, dateTime.AddDays(2).ToString());
                    break;
                case ScheduledActionType.RefreshNews:
                    ModSettingsManager.SetStringValue(ModSettingsConstants.REFRESH_NEWS_DATE_TIME, dateTime.AddDays(3).ToString());
                    break;
                case ScheduledActionType.RefreshAddonUpdates:
                    ModSettingsManager.SetStringValue(ModSettingsConstants.REFRESH_ADDON_UPDATES_DATE_TIME, dateTime.AddDays(5).ToString());
                    break;
            }
            ModSettingsDataManager.Instance.Save();
        }

        public bool ShouldExecuteAction(ScheduledActionType scheduledAction)
        {
            return !DisableSchedules && DateTime.Now > GetActionExecutionDateTime(scheduledAction);
        }

        public DateTime GetActionExecutionDateTime(ScheduledActionType scheduledAction)
        {
            string dateTimeString;
            switch (scheduledAction)
            {
                case ScheduledActionType.RefreshModUpdates:
                    dateTimeString = RefreshModUpdatesDateTime;
                    break;
                case ScheduledActionType.RefreshExclusivePerks:
                    dateTimeString = RefreshExclusivePerksDateTime;
                    break;
                case ScheduledActionType.RefreshCustomizationAssetsRemoteVersion:
                    dateTimeString = RefreshCustomizationAssetsRemoteVersionDateTime;
                    break;
                case ScheduledActionType.RefreshNews:
                    dateTimeString = RefreshNewsDateTime;
                    break;
                case ScheduledActionType.RefreshAddonUpdates:
                    dateTimeString = RefreshAddonUpdatesDateTime;
                    break;
                default:
                    dateTimeString = null;
                    break;
            }

            if (!dateTimeString.IsNullOrEmpty() && DateTime.TryParse(dateTimeString, out DateTime dateTime))
            {
                return dateTime;
            }
            return DateTime.MinValue;
        }
    }
}
