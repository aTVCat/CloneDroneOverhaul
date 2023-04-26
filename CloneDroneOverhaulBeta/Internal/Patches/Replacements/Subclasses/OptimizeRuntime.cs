using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace CDOverhaul.Patches
{
    public class OptimizeRuntime : ReplacementBase
    {
        [OverhaulSetting("Optimization.Unloading.Enable automatic garbage collector", true, false, "Unity engine usually cleans up memory automatically")]
        public static bool GarbageCollectorEnabled;
        [OverhaulSetting("Optimization.Unloading.Clear cache on level spawn", false, false, "Free some memory when new level is loaded")]
        public static bool DoCleanupOnLevelSpawn;
        [OverhaulSetting("Optimization.Unloading.Clear cache fully", false, false, "Free as much memory as possible when new level is loaded", null, null, "Optimization.Unloading.Clear cache on level spawn")]
        public static bool FullClean;

        [SettingInforms(1)]
        [OverhaulSetting("Optimization.Unloading.Reduce light sources count on scene", false, false)]
        public static bool ReduceLightCount;

        public override void Replace()
        {
            base.Replace();

            _ = OverhaulEventsController.AddEventListener(GlobalEvents.LevelSpawned, CollectGarbage, true);
            _ = OverhaulEventsController.AddEventListener(SettingsController.SettingChangedEventString, RefreshGC);

            _ = GameUIRoot.Instance.gameObject.AddComponent<GameUIRootBehaviour>();

            SuccessfullyPatched = true;
        }

        /// <summary>
        /// Reduce memory usage as much as possible
        /// </summary>
        public static void CollectGarbage()
        {
            if (!DoCleanupOnLevelSpawn)
            {
                return;
            }

            GC.Collect();
            _ = Resources.UnloadUnusedAssets();

            if (FullClean)
            {
                _ = UnityEngine.Caching.ClearCache();
                CacheManager.Instance.CreateOrClearInstance();
            }
        }

        private void RefreshGC()
        {
            GarbageCollector.GCMode = GarbageCollectorEnabled ? GarbageCollector.Mode.Enabled : GarbageCollector.Mode.Disabled;
        }

        public override void Cancel()
        {
            base.Cancel();

            OverhaulEventsController.RemoveEventListener(GlobalEvents.LevelSpawned, CollectGarbage, true);
            OverhaulEventsController.RemoveEventListener(SettingsController.SettingChangedEventString, RefreshGC);
        }
    }
}
