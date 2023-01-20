using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CloneDroneOverhaul.V3.Base
{
    public class GameInformationController
    {
        public static GameInformationController.LevelInformation GetCurrentLevelInfo()
        {
            return GameInformationController.LevelInformation.GetCurrentLevelInfo(true);
        }

        public static GameInformationController.QualityInformation GetQualityInfo()
        {
            return GameInformationController.QualityInformation.GetQualityInformation();
        }

        public static GameInformationController.UnoptimizedThings GetUnoptimizedThingsInfo()
        {
            return GameInformationController.UnoptimizedThings.GetFPSLoweringStuff();
        }

        public static List<Transform> OvermodeLevelTransforms = new List<Transform>();

        public struct UnoptimizedThings
        {
            public static GameInformationController.UnoptimizedThings GetFPSLoweringStuff()
            {
                GameInformationController.UnoptimizedThings result = default(GameInformationController.UnoptimizedThings);
                result.getValues();
                return result;
            }

            private void getValues()
            {
                _allCameras = UnityEngine.Object.FindObjectsOfType<Camera>();
            }

            public Camera[] AllCameras => _allCameras;

            private Camera[] _allCameras;
        }

        public struct QualityInformation
        {
            public static GameInformationController.QualityInformation GetQualityInformation()
            {
                return default(GameInformationController.QualityInformation);
            }

            public float ShadowDistance => QualitySettings.shadowDistance;

            public float PixelLightCount => (float)QualitySettings.pixelLightCount;

            public LightmapSettings LightmapSettings => UnityEngine.Object.FindObjectOfType<LightmapSettings>();
        }

        public struct LevelInformation
        {
            public static GameInformationController.LevelInformation GetCurrentLevelInfo(bool includeExtraInfo)
            {
                Transform currentLevelTransform = Singleton<LevelManager>.Instance.GetCurrentLevelTransform();
                GameInformationController.LevelInformation result = default(GameInformationController.LevelInformation);
                result.IsNull = (currentLevelTransform == null);
                if (!result.IsNull)
                {
                    result.ID = Singleton<LevelManager>.Instance.GetCurrentLevelID();
                    result.LevelDescription = Singleton<LevelManager>.Instance.GetCurrentLevelDescription();
                    result.LevelTransform = currentLevelTransform;
                    if (includeExtraInfo)
                    {
                        result.Extra_Sections = currentLevelTransform.GetComponentsInChildren<LevelSection>(true);
                        result.Extra_Animations = currentLevelTransform.GetComponentsInChildren<LevelEditorAnimation>(true);
                    }
                }
                return result;
            }

            public void TrySaveLevel(bool openExplorer, string fileName, bool createLevelDataIfNull)
            {
                if (LevelData == null)
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
                string contents = JsonConvert.SerializeObject(LevelData, Formatting.None, Singleton<DataRepository>.Instance.GetSettings());/*
                File.WriteAllText(ModDataManager.DublicatedLevelsFolder + fileName + ".json", contents);
                if (openExplorer)
                {
                    FileManagerStuff.OpenFolder(ModDataManager.DublicatedLevelsFolder);
                }*/
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
