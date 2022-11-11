using CloneDroneOverhaul;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

namespace CloneDroneOverhaul.Modules
{
    public class GameInformationManager : ModuleBase
    {
        public override void Start()
        {
        }

        public LevelInformation GetCurrentLevelInfo()
        {
            return LevelInformation.GetCurrentLevelInfo(true);
        }


        public struct LevelInformation
        {
            public static LevelInformation GetCurrentLevelInfo(bool includeExtraInfo)
            {
                Transform lvlTransform = LevelManager.Instance.GetCurrentLevelTransform();

                LevelInformation levelInformation = default(LevelInformation);
                levelInformation.IsNull = lvlTransform == null;
                if (!levelInformation.IsNull)
                {
                    levelInformation.ID = LevelManager.Instance.GetCurrentLevelID();
                    levelInformation.LevelDescription = LevelManager.Instance.GetCurrentLevelDescription();
                    levelInformation.LevelTransform = lvlTransform;

                    if (includeExtraInfo)
                    {
                        levelInformation.Extra_Sections = lvlTransform.GetComponentsInChildren<LevelSection>(true);
                        levelInformation.Extra_Animations = lvlTransform.GetComponentsInChildren<LevelEditorAnimation>(true);
                    }
                }
                return levelInformation;
            }

            public void TrySaveLevel(bool openExplorer, string fileName, bool createLevelDataIfNull)
            {
                if(LevelData == null)
                {
                    if (!createLevelDataIfNull)
                    {
                        return;
                    }
                    LevelData = new LevelEditorLevelData();
                    LevelEditorLevelObject levelEditorLevelObject = new LevelEditorLevelObject();
                    levelEditorLevelObject.SerializeFrom(LevelTransform.gameObject);
                    LevelData.RootLevelObject = levelEditorLevelObject;
                }
                string contents = JsonConvert.SerializeObject(LevelData, Newtonsoft.Json.Formatting.None, DataRepository.Instance.GetSettings());
                File.WriteAllText(ModDataManager.DublicatedLevelsFolder + fileName + ".json", contents);

                if (openExplorer)
                {
                    FileManagerStuff.OpenFolder(ModDataManager.DublicatedLevelsFolder);
                }
            }

            public bool IsNull { get; private set; }
            public string ID { get; private set; }
            public LevelDescription LevelDescription { get; private set; }
            public LevelEditorLevelData LevelData { get; private set; }
            public Transform LevelTransform { get; private set; }
            public LevelSection[] Extra_Sections { get; private set; }
            public LevelEditorAnimation[] Extra_Animations { get; private set; }
        }
    }
}
