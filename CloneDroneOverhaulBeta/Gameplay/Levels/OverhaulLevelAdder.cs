using ModLibrary;
using System;
using System.Collections.Generic;

namespace CDOverhaul.Gameplay
{
    public static class OverhaulLevelAdder
    {
        private static readonly List<string> m_Levels = new List<string>();

        internal static void Initialize()
        {
            m_Levels.Clear();
        }

        public static void AddLevel(string pathUnderModAssetsFolder, string levelID, GameMode gamemode, out string guid)
        {
            if (m_Levels.Contains(pathUnderModAssetsFolder))
            {
                guid = null;
                return;
            }

            guid = Guid.NewGuid().ToString();
            if (gamemode == GameMode.Story)
            {
                List<LevelDescription> levels = LevelManager.Instance.GetPrivateField<List<LevelDescription>>("_storyModeLevels");
                LevelDescription newLevelDescription = new LevelDescription()
                {
                    LevelID = levelID,
                    PrefabName = pathUnderModAssetsFolder,
                    LevelTags = new List<LevelTags>() { LevelTags.LevelEditor },
                    GeneratedUniqueID = guid
                };
                levels.Add(newLevelDescription);

                m_Levels.Add("Data/LevelEditorLevels/" + pathUnderModAssetsFolder);
            }
        }

        public static string GetLevel(ref string path)
        {
            if (!m_Levels.Contains(path))
            {
                return null;
            }

            path = path.Replace("Data/LevelEditorLevels/", string.Empty);
            return OverhaulCore.ReadTextFile("Assets/Levels/" + path + ".json");
        }

        public static bool HasLevel(in string path)
        {
            return m_Levels.Contains(path);
        }
    }
}
