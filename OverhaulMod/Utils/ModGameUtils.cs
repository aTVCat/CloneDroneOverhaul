using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OverhaulMod.Utils
{
    public static class ModGameUtils
    {
        public static bool SyncSteamAchievements()
        {
            GameplayAchievementManager gameplayAchievementManager = GameplayAchievementManager.Instance;
            if (!gameplayAchievementManager)
                return false;

            bool result = false;
            foreach(var achievement in gameplayAchievementManager.Achievements)
            {
                SteamUserStats.GetAchievement(achievement.SteamAchievementID, out bool isComplete);

                int currentProgress = gameplayAchievementManager.GetProgress(achievement.AchievementID);
                int targetProgress = isComplete ? achievement.TargetProgress : 0;
                if(targetProgress != currentProgress)
                    result = true;

                if (isComplete)
                    gameplayAchievementManager.SetAchievementProgress(achievement, targetProgress, true);
                else
                    gameplayAchievementManager.SetAchievementProgress(achievement, 0, true);
            }
            return result;
        }
    }
}
