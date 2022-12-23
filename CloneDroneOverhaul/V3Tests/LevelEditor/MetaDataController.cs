using UnityEngine;
using ModLibrary;
using CloneDroneOverhaul.V3Tests.Base;

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
            V3_MainModController.CallEvent("level.data", new object[] { levelData.ModdedMetadata });
        }

        public static LevelEditorLevelData GetCurrentLevelData()
        {
            if (LevelEditorModdedMetadataManager.IsCurrentlyEditingLevel())
            {
                return Singleton<LevelEditorDataManager>.Instance.GetCurrentLevelData();
            }
            if(Singleton<LevelManager>.Instance.GetCurrentLevelID() != null)
            {
                LevelDescription currentLevelDescription = Singleton<LevelManager>.Instance.GetCurrentLevelDescription();
                if (currentLevelDescription != null)
                {
                    return currentLevelDescription.GetLevelEditorLevelData();
                }
            }
            return null;
        }
    }
}