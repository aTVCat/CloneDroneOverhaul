using CDOverhaul.NetworkAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul.Gameplay
{
    public static class WeaponSkinsUpdater
    {
        public static bool HasUpdates
        {
            get;
            private set;
        }

        public static bool UnableToCheck
        {
            get;
            private set;
        }

        public static string GetFullStateString()
        {
            return GetAssetBundleFileStateString() + '\n' + GetUpdateStateString();
        }

        public static string GetAssetBundleFileStateString()
        {
            return OverhaulLocalizationController.GetTranslation("FileVersion:") + WeaponSkinsController.GetSkinsFileVersion();
        }

        public static string GetUpdateStateString()
        {
            return !HasUpdates ? OverhaulLocalizationController.GetTranslation("NoUpdatesAvailable") : OverhaulLocalizationController.GetTranslation("UpdatesAvailable").AddColor(UnityEngine.Color.red);
        }

        public static void RefreshUpdates(Action onComplete)
        {
            StaticCoroutineRunner.StartStaticCoroutine(checkForUpdatesCoroutine(onComplete));
        }

        private static IEnumerator checkForUpdatesCoroutine(Action onComplete)
        {
            yield return null;
            onComplete.Invoke();
            yield break;
        }
    }
}
