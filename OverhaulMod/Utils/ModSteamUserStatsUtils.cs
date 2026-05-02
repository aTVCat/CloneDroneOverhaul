using Steamworks;
using System;

namespace OverhaulMod.Utils
{
    public static class ModSteamUserStatsUtils
    {
        /// <summary>
        /// StartFading local player stats
        /// </summary>
        /// <param name="callback"></param><b>True</b> if data was successfully refreshed, otherwise <b>False</b></param>
        public static void RefreshLocalStats(Action<bool> callback)
        {
            if (!SteamManager.Instance || !SteamManager.Instance.Initialized)
            {
                callback?.Invoke(false);
                return;
            }

            bool isCompleted = false;

            CallResult<UserStatsReceived_t> callResult = null;
            ModActionUtils.DoInTime(delegate
            {
                if (callResult != null) callResult.Dispose();
                if (!isCompleted && callback != null) callback(false);
            }, 10f);

            callResult = new CallResult<UserStatsReceived_t>();
            callResult.Set(SteamUserStats.RequestUserStats(SteamUser.GetSteamID()), delegate (UserStatsReceived_t t, bool ioError)
            {
                isCompleted = true;
                if (callResult != null) callResult.Dispose();

                if (ioError || t.m_eResult != EResult.k_EResultOK)
                {
                    if (callback != null) callback(false);
                    return;
                }
                if (callback != null) callback(true);
            });
        }

        /// <summary>
        /// StartFading global player stats related to achievements
        /// </summary>
        /// <param name="callback"></param><b>True</b> if data was successfully refreshed, otherwise <b>False</b></param>
        public static void RefreshGlobalAchievementPercentages(Action<bool> callback)
        {
            if (!SteamManager.Instance || !SteamManager.Instance.Initialized)
            {
                if (callback != null) callback(false);
                return;
            }

            bool isCompleted = false;
            CallResult<GlobalAchievementPercentagesReady_t> cr = null;
            ModActionUtils.DoInTime(delegate
            {
                if (cr != null) cr.Dispose();
                if (!isCompleted && callback != null) callback(false);
            }, 10f);

            cr = CallResult<GlobalAchievementPercentagesReady_t>.Create(null);
            cr.Set(SteamUserStats.RequestGlobalAchievementPercentages(), delegate (GlobalAchievementPercentagesReady_t c, bool io)
            {
                isCompleted = true;
                if (c.m_eResult != EResult.k_EResultOK || io)
                {
                    if (callback != null) callback(false);
                    return;
                }
                if (callback != null) callback(true);
            });
        }

        /// <summary>
        /// Calls <see cref="RefreshLocalStats(Action{bool})"/> and <see cref="RefreshGlobalAchievementPercentages(Action{bool})"/>
        /// </summary>
        /// <param name="callback"></param>
        public static void RefreshAllStats(Action<bool> callback)
        {
            if (!SteamManager.Instance || !SteamManager.Instance.Initialized)
            {
                if (callback != null) callback(false);
                return;
            }

            RefreshLocalStats(delegate (bool result)
            {
                if (!result)
                {
                    if (callback != null) callback(false);
                    return;
                }

                RefreshGlobalAchievementPercentages(delegate (bool result2)
                {
                    if (callback != null) callback(result2);
                });
            });
        }

        public static bool GetAchievementAchievedPercent(string achievementId, out float percent)
        {
            return SteamUserStats.GetAchievementAchievedPercent(achievementId, out percent);
        }
    }
}