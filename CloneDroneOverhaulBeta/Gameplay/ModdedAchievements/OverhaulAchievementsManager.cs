using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class OverhaulAchievementsManager : OverhaulManager<OverhaulAchievementsManager>
    {
        private readonly List<string> m_Achievements = new List<string>();

        public T CreateAchievement<T>(string name, string description, Sprite previewImage) where T : OverhaulAchievement
        {
            if (m_Achievements.Contains(name))
                return null;

            T achievement = ScriptableObject.CreateInstance<T>();
            achievement.Name = name;
            achievement.Description = description;
            achievement.SteamAchievementID = "none";
            achievement.AchievementID = "Overhaul_" + name;
            achievement.Image = previewImage;

            GameplayAchievementManager manager = GameplayAchievementManager.Instance;
            List<GameplayAchievement> list = manager.Achievements.ToList();
            list.Add(achievement);
            manager.Achievements = list.ToArray();

            m_Achievements.Add(name);
            return achievement;
        }
    }
}
