using CloneDroneOverhaul.V3Tests.Base;
using ModLibrary;

namespace CloneDroneOverhaul.LevelEditor
{
    public class MetaDataController : V3_ModControllerBase
    {
        public const string OVERHAUL_MOD_KEY_PREFIX = "OverhaulMod_";

        /// <summary>
        /// The event <b>"level.data"</b> event will be sent
        /// </summary>
        public void RefreshCurrentLevelMetaData()
        {
            LevelEditorLevelData levelData = GetCurrentLevelData();
            if(levelData == null)
            {
                V3_MainModController.CallEvent("level.data", new object[] { null });
                return;
            }
            V3_MainModController.CallEvent("level.data", new object[] { levelData.ModdedMetadata });
        }

        public static LevelEditorLevelData GetCurrentLevelData()
        {
            if (LevelEditorModdedMetadataManager.IsCurrentlyEditingLevel())
            {
                return LevelEditorDataManager.Instance.GetCurrentLevelData();
            }
            if (LevelManager.Instance.GetCurrentLevelID() != null)
            {
                LevelDescription currentLevelDescription = LevelManager.Instance.GetCurrentLevelDescription();
                if (currentLevelDescription != null)
                {
                    return currentLevelDescription.GetLevelEditorLevelData();
                }
            }
            return null;
        }
    }
}