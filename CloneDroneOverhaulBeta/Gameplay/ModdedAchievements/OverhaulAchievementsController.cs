using ModLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class OverhaulAchievementsController : OverhaulGameplayController
    {
        public override void Initialize()
        {
            base.Initialize();
        }

        public static T CreateAchievement<T>(string name, string description, Sprite previewImage) where T : OverhaulAchievement
        {
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

            Dictionary<string, GameplayAchievement> dictionary = manager.GetPrivateField<Dictionary<string, GameplayAchievement>>("_idToAchievementDictionary");
            dictionary.Add(achievement.AchievementID, achievement);

            return achievement;
        }
    }
}
