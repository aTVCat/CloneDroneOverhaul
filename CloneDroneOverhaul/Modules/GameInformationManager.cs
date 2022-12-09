using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CloneDroneOverhaul.Modules
{
    public class GameInformationManager : ModuleBase
    {
        // Token: 0x06000234 RID: 564 RVA: 0x0000D84E File Offset: 0x0000BA4E
        public override void Start()
        {
            GameInformationManager.Instance = this;
        }

        // Token: 0x06000235 RID: 565 RVA: 0x0000D856 File Offset: 0x0000BA56
        public GameInformationManager.LevelInformation GetCurrentLevelInfo()
        {
            return GameInformationManager.LevelInformation.GetCurrentLevelInfo(true);
        }

        // Token: 0x06000236 RID: 566 RVA: 0x0000D85E File Offset: 0x0000BA5E
        public GameInformationManager.QualityInformation GetQualityInfo()
        {
            return GameInformationManager.QualityInformation.GetQualityInformation();
        }

        // Token: 0x06000237 RID: 567 RVA: 0x0000D865 File Offset: 0x0000BA65
        public GameInformationManager.UnoptimizedThings GetUnoptimizedThingsInfo()
        {
            return GameInformationManager.UnoptimizedThings.GetFPSLoweringStuff();
        }

        // Token: 0x0400014D RID: 333
        public static GameInformationManager Instance;

        // Token: 0x0400014E RID: 334
        public List<Transform> OvermodeLevelTransforms = new List<Transform>();

        // Token: 0x020000A6 RID: 166
        public struct UnoptimizedThings
        {
            // Token: 0x06000400 RID: 1024 RVA: 0x0001658C File Offset: 0x0001478C
            public static GameInformationManager.UnoptimizedThings GetFPSLoweringStuff()
            {
                GameInformationManager.UnoptimizedThings result = default(GameInformationManager.UnoptimizedThings);
                result.getValues();
                return result;
            }

            // Token: 0x06000401 RID: 1025 RVA: 0x000165A9 File Offset: 0x000147A9
            private void getValues()
            {
                this._allCameras = UnityEngine.Object.FindObjectsOfType<Camera>();
            }

            // Token: 0x17000050 RID: 80
            // (get) Token: 0x06000402 RID: 1026 RVA: 0x000165B6 File Offset: 0x000147B6
            public Camera[] AllCameras
            {
                get
                {
                    return this._allCameras;
                }
            }

            // Token: 0x0400033E RID: 830
            private Camera[] _allCameras;
        }

        // Token: 0x020000A7 RID: 167
        public struct QualityInformation
        {
            // Token: 0x06000403 RID: 1027 RVA: 0x000165C0 File Offset: 0x000147C0
            public static GameInformationManager.QualityInformation GetQualityInformation()
            {
                return default(GameInformationManager.QualityInformation);
            }

            // Token: 0x17000051 RID: 81
            // (get) Token: 0x06000404 RID: 1028 RVA: 0x000165D6 File Offset: 0x000147D6
            public float ShadowDistance
            {
                get
                {
                    return QualitySettings.shadowDistance;
                }
            }

            // Token: 0x17000052 RID: 82
            // (get) Token: 0x06000405 RID: 1029 RVA: 0x000165DD File Offset: 0x000147DD
            public float PixelLightCount
            {
                get
                {
                    return (float)QualitySettings.pixelLightCount;
                }
            }

            // Token: 0x17000053 RID: 83
            // (get) Token: 0x06000406 RID: 1030 RVA: 0x000165E5 File Offset: 0x000147E5
            public LightmapSettings LightmapSettings
            {
                get
                {
                    return UnityEngine.Object.FindObjectOfType<LightmapSettings>();
                }
            }
        }

        // Token: 0x020000A8 RID: 168
        public struct LevelInformation
        {
            // Token: 0x06000407 RID: 1031 RVA: 0x000165EC File Offset: 0x000147EC
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

            // Token: 0x06000408 RID: 1032 RVA: 0x00016670 File Offset: 0x00014870
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

            // Token: 0x17000054 RID: 84
            // (get) Token: 0x06000409 RID: 1033 RVA: 0x000166F1 File Offset: 0x000148F1
            // (set) Token: 0x0600040A RID: 1034 RVA: 0x000166F9 File Offset: 0x000148F9
            public bool IsNull { get; private set; }

            // Token: 0x17000055 RID: 85
            // (get) Token: 0x0600040B RID: 1035 RVA: 0x00016702 File Offset: 0x00014902
            // (set) Token: 0x0600040C RID: 1036 RVA: 0x0001670A File Offset: 0x0001490A
            public string ID { get; private set; }

            // Token: 0x17000056 RID: 86
            // (get) Token: 0x0600040D RID: 1037 RVA: 0x00016713 File Offset: 0x00014913
            // (set) Token: 0x0600040E RID: 1038 RVA: 0x0001671B File Offset: 0x0001491B
            public LevelDescription LevelDescription { get; private set; }

            // Token: 0x17000057 RID: 87
            // (get) Token: 0x0600040F RID: 1039 RVA: 0x00016724 File Offset: 0x00014924
            // (set) Token: 0x06000410 RID: 1040 RVA: 0x0001672C File Offset: 0x0001492C
            public LevelEditorLevelData LevelData { get; private set; }

            // Token: 0x17000058 RID: 88
            // (get) Token: 0x06000411 RID: 1041 RVA: 0x00016735 File Offset: 0x00014935
            // (set) Token: 0x06000412 RID: 1042 RVA: 0x0001673D File Offset: 0x0001493D
            public Transform LevelTransform { get; private set; }

            // Token: 0x17000059 RID: 89
            // (get) Token: 0x06000413 RID: 1043 RVA: 0x00016746 File Offset: 0x00014946
            // (set) Token: 0x06000414 RID: 1044 RVA: 0x0001674E File Offset: 0x0001494E
            public LevelSection[] Extra_Sections { get; private set; }

            // Token: 0x1700005A RID: 90
            // (get) Token: 0x06000415 RID: 1045 RVA: 0x00016757 File Offset: 0x00014957
            // (set) Token: 0x06000416 RID: 1046 RVA: 0x0001675F File Offset: 0x0001495F
            public LevelEditorAnimation[] Extra_Animations { get; private set; }
        }
    }
}
