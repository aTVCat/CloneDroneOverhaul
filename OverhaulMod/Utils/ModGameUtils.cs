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

                int progress = isComplete ? achievement.TargetProgress : 0;
                if(progress != gameplayAchievementManager.GetProgress(achievement.AchievementID))
                    result = true;

                gameplayAchievementManager.SetAchievementProgress(achievement, progress, true);
            }
            return result;
        }
    }
}
