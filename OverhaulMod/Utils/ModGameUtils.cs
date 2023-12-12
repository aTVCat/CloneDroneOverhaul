using Steamworks;

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
            foreach (GameplayAchievement achievement in gameplayAchievementManager.Achievements)
            {
                _ = SteamUserStats.GetAchievement(achievement.SteamAchievementID, out bool isComplete);

                int currentProgress = gameplayAchievementManager.GetProgress(achievement.AchievementID);
                int targetProgress = isComplete ? achievement.TargetProgress : 0;
                if (targetProgress != currentProgress)
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
