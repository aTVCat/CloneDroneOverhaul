using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace CDOverhaul.Patches
{
    public class OptimizeRuntime : ReplacementBase
    {
        [OverhaulSetting("Optimization.Unloading.Clear cache on level spawn", false, false, "Free some memore when new level is loaded")]
        public static bool DoCleanupOnLevelSpawn;

        [OverhaulSetting("Optimization.Unloading.Clear cache fully", false, false, "Free as much memory as possible when new level is loaded")]
        public static bool FullClean;

        [OverhaulSetting("Optimization.Unloading.Enable automatic garbage collector", true, false, "Unity engine usually cleans up memory automatically")]
        public static bool GarbageCollectorEnabled;

        public override void Replace()
        {
            base.Replace();

            OverhaulEventManager.AddEventListener(GlobalEvents.LevelSpawned, CollectGarbage, true);
            OverhaulEventManager.AddEventListener(SettingsController.SettingChangedEventString, RefreshGC);

            SuccessfullyPatched = true;
        }

        public void CollectGarbage()
        {
            if (!DoCleanupOnLevelSpawn)
            {
                return;
            }

            GC.Collect();
            Resources.UnloadUnusedAssets();

            if (FullClean)
            {
                UnityEngine.Caching.ClearCache();
                CacheManager.Instance.CreateOrClearInstance();
            }
        }

        public void RefreshGC()
        {
            GarbageCollector.GCMode = GarbageCollectorEnabled ? GarbageCollector.Mode.Enabled : GarbageCollector.Mode.Disabled;
        }

        public override void Cancel()
        {
            base.Cancel();

            OverhaulEventManager.RemoveEventListener(GlobalEvents.LevelSpawned, CollectGarbage, true);
        }
    }
}
