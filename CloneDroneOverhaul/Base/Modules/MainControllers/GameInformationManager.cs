using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CloneDroneOverhaul.Modules
{
    public class GameInformationManager : ModuleBase
    {
        public override void Start()
        {
            GameInformationManager.Instance = this;
        }

        public GameInformationManager.LevelInformation GetCurrentLevelInfo()
        {
            return GameInformationManager.LevelInformation.GetCurrentLevelInfo(true);
        }

        public GameInformationManager.QualityInformation GetQualityInfo()
        {
            return GameInformationManager.QualityInformation.GetQualityInformation();
        }

        public GameInformationManager.UnoptimizedThings GetUnoptimizedThingsInfo()
        {
            return GameInformationManager.UnoptimizedThings.GetFPSLoweringStuff();
        }

        public static GameInformationManager Instance;

        public List<Transform> OvermodeLevelTransforms = new List<Transform>();

        public struct UnoptimizedThings
        {
            public static GameInformationManager.UnoptimizedThings GetFPSLoweringStuff()
            {
                GameInformationManager.UnoptimizedThings result = default(GameInformationManager.UnoptimizedThings);
                result.getValues();
                return result;
            }

            private void getValues()
            {
                this._allCameras = UnityEngine.Object.FindObjectsOfType<Camera>();
            }

            public Camera[] AllCameras
            {
                get
                {
                    return this._allCameras;
                }
            }

            private Camera[] _allCameras;
        }

        public struct QualityInformation
        {
            public static GameInformationManager.QualityInformation GetQualityInformation()
            {
                return default(GameInformationManager.QualityInformation);
            }

            public float ShadowDistance
            {
                get
                {
                    return QualitySettings.shadowDistance;
                }
            }

            public float PixelLightCount
            {
                get
                {
                    return (float)QualitySettings.pixelLightCount;
                }
            }

            public LightmapSettings LightmapSettings
            {
                get
                {
                    return UnityEngine.Object.FindObjectOfType<LightmapSettings>();
                }
            }
        }

        public struct LevelInformation
        {
            public static GameInformationManager.LevelInformation GetCurrentLevelInfo(bool includeExtraInfo)
            {
                Transform currentLevelTransform = Singleton<LevelManager>.Instance.GetCurrentLevelTransform();
                GameInformationManager.LevelInformation result = default(GameInformationManager.LevelInformation);
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
                if (this.LevelData == null)
                {
                    if (!createLevelDataIfNull)
                    {
                        return;
                    }
                    this.LevelData = new LevelEditorLevelData();
                    LevelEditorLevelObject levelEditorLevelObject = new LevelEditorLevelObject();
                    levelEditorLevelObject.SerializeFrom(this.LevelTransform.gameObject);
                    this.LevelData.RootLevelObject = levelEditorLevelObject;
                }
                string contents = JsonConvert.SerializeObject(this.LevelData, Formatting.None, Singleton<DataRepository>.Instance.GetSettings());
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
