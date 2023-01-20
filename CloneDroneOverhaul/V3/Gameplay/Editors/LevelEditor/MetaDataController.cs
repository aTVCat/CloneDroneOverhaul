using CloneDroneOverhaul.V3.Base;
using ModLibrary;

namespace CloneDroneOverhaul.LevelEditor
{
    public class MetaDataController : V3_ModControllerBase
    {
        public const string OVERHAUL_MOD_KEY_PREFIX = "OverhaulMod_";

        public static LevelEditorLevelData CurrentLevelData;

        /// <summary>
        /// The event <b>"level.data"</b> event will be sent
        /// </summary>
        public void RefreshCurrentLevelMetaData()
        {
            DelegateScheduler.Instance.Schedule(delegate
            {
                LevelEditorLevelData levelData = GetCurrentLevelData();
                CurrentLevelData = levelData;
                if (levelData == null)
                {
                    V3_MainModController.CallEvent("level.data", new object[] { null });
                    return;
                }
                V3_MainModController.CallEvent("level.data", new object[] { levelData });
            }, 0.1f);
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