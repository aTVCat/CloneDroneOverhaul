using Steamworks;
using System;

namespace OverhaulMod.Utils
{
    public static class ModSteamUserStatsUtils
    {
        /// <summary>
        /// Refresh local player stats
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
            Callback<UserStatsReceived_t> cb = null;
            DelegateScheduler.Instance.Schedule(delegate
            {
                if (cb != null && !cb.m_bDisposed)
                    cb.Dispose();

                if (!isCompleted)
                    callback?.Invoke(false);
            }, 10f);

            cb = Callback<UserStatsReceived_t>.Create(delegate (UserStatsReceived_t userStatsReceived)
            {
                isCompleted = true;
                if (cb != null)
                    cb.Dispose();

                if (userStatsReceived.m_eResult != EResult.k_EResultOK && userStatsReceived.m_eResult != EResult.k_EResultFail)
                {
                    callback?.Invoke(false);
                    return;
                }
                callback?.Invoke(true);
            });

            if (!SteamUserStats.RequestCurrentStats())
            {
                callback?.Invoke(false);
            }
        }

        /// <summary>
        /// Refresh global player stats related to achievements
        /// </summary>
        /// <param name="callback"></param><b>True</b> if data was successfully refreshed, otherwise <b>False</b></param>
        public static void RefreshGlobalAchievementPercentages(Action<bool> callback)
        {
            if (!SteamManager.Instance || !SteamManager.Instance.Initialized)
            {
                callback?.Invoke(false);
                return;
            }

            bool isCompleted = false;
            CallResult<GlobalAchievementPercentagesReady_t> cr = null;
            DelegateScheduler.Instance.Schedule(delegate
            {
                if (cr != null && !cr.m_bDisposed)
                    cr.Dispose();

                if (!isCompleted)
                    callback?.Invoke(false);
            }, 10f);

            cr = CallResult<GlobalAchievementPercentagesReady_t>.Create(null);
            cr.Set(SteamUserStats.RequestGlobalAchievementPercentages(), delegate (GlobalAchievementPercentagesReady_t c, bool io)
            {
                isCompleted = true;
                if (c.m_eResult != EResult.k_EResultOK || io)
                {
                    callback?.Invoke(false);
                    return;
                }
                callback?.Invoke(true);
            });
        }

        /// <summary>
        /// Call <see cref="RefreshLocalStats(Action{bool})"/> and <see cref="RefreshGlobalAchievementPercentages(Action{bool})"/>
        /// </summary>
        /// <param name="callback"></param>
        public static void RefreshAllStats(Action<bool> callback)
        {
            if (!SteamManager.Instance || !SteamManager.Instance.Initialized)
            {
                callback?.Invoke(false);
                return;
            }

            RefreshLocalStats(delegate (bool result)
            {
                if (!result)
                {
                    callback?.Invoke(false);
                    return;
                }

                RefreshGlobalAchievementPercentages(delegate (bool result2)
                {
                    callback?.Invoke(result2);
                });
            });
        }

        public static bool GetAchievementAchievedPercent(string achievementId, out float percent)
        {
            return SteamUserStats.GetAchievementAchievedPercent(achievementId, out percent);
        }
    }
}
