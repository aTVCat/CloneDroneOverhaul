using CloneDroneOverhaul.V3Tests.Base;
using CloneDroneOverhaul.V3Tests.Gameplay;
using ModLibrary;

namespace CloneDroneOverhaul.LevelEditor
{
    public class V3_LevelEditorController : V3_ModControllerBase
    {
        private bool _isInLevelEditor;
        public bool IsInLevelEditor => _isInLevelEditor;

        private LevelEditorLevelData _currentLevelData;

        public override void OnEvent(in string eventName, in object[] args)
        {
            if (eventName == "onLevelEditorStarted")
            {
                _isInLevelEditor = true;
                RefreshLevelData();
            }
            if (eventName == "level.onSpawn")
            {
                if (IsInLevelEditor)
                {
                    RefreshLevelData();
                }
            }
            if (eventName == "level.data")
            {
                if (IsInLevelEditor)
                {
                    _currentLevelData = args[0] as LevelEditorLevelData;
                }
            }
        }

        /// <summary>
        /// Execute <b>level.data</b> event
        /// </summary>
        public void RefreshLevelData()
        {
            if (!IsInLevelEditor)
            {
                return;
            }
            V3_MainModController.CallEvent("level.data", new object[] { LevelEditorDataManager.Instance.GetCurrentLevelData() });
        }

        public static void Patch(in ELevelEditorBasicPatchType patchType)
        {
            switch (patchType)
            {
                case ELevelEditorBasicPatchType.DefaultToolsIsMove:
                    LevelEditorToolManager.Instance.SetActiveTool(LevelEditorToolType.Move);
                    break;
            }
        }

        public void Test_SetWeatherToRainy()
        {
            LevelEditorModdedMetadataManager.TrySetMetadata(ArenaWeatherController.METADATA_LEVELWEATHER_KEY, ArenaWeatherController.METADATA_LEVELWEATHER_RAINY_VALUE);
            RefreshLevelData();
        }
    }
}
