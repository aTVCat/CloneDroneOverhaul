using System;
using System.Collections;
using UnityEngine;

namespace CDOverhaul
{
    public static class OverhaulTransitionController
    {
        private static GameObject s_OldTransitionPrefab;
        private static GameObject s_TransitionPrefab;

        private static bool s_FadeOutOnSceneLoad;
        private static OverhaulTransitionBehaviour s_CurrentTransition;

        internal static void Initialize()
        {
            if (!s_OldTransitionPrefab)
                s_OldTransitionPrefab = OverhaulAssetsController.GetAsset("LoadingScreen", OverhaulAssetPart.Main);
            if (!s_TransitionPrefab)
                s_TransitionPrefab = OverhaulAssetsController.GetAsset("OverhaulTransition", OverhaulAssetPart.Main);

            if (s_FadeOutOnSceneLoad)
                CreateTransition(false, true);

            SceneTransitionManager.Instance.SceneTransitionCoverScreen = s_OldTransitionPrefab.transform as RectTransform;
        }

        public static void CreateTransition(bool isSceneSwitching = false, bool fadeOut = false)
        {
            if (HasSpawnedTransitionHUD())
                return;

            s_CurrentTransition = UnityEngine.Object.Instantiate(s_TransitionPrefab).AddComponent<OverhaulTransitionBehaviour>();
            s_CurrentTransition.Initialize(fadeOut, isSceneSwitching);
            s_FadeOutOnSceneLoad = isSceneSwitching;
        }

        public static void EndTransition(bool fadeOut = true)
        {
            if (!HasSpawnedTransitionHUD())
                return;

            if (fadeOut)
            {
                s_CurrentTransition.Initialize(true);
                return;
            }
            UnityEngine.Object.Destroy(s_CurrentTransition.gameObject);
        }

        public static bool HasSpawnedTransitionHUD() => s_CurrentTransition;

        public static void GoToMainMenu()
        {
            CreateTransition(true, false);
        }

        public static void DoTransitionWithAction(Action action, Func<bool> waitUntilFunc = null, float waitUntilEndDelay = 0.35f)
        {
            _ = StaticCoroutineRunner.StartStaticCoroutine(doTransitionWithActionCoroutine(action, waitUntilFunc, waitUntilEndDelay));
        }

        private static IEnumerator doTransitionWithActionCoroutine(Action action, Func<bool> waitUntilFunc = null, float waitUntilEndDelay = 0.35f)
        {
            if (!s_TransitionPrefab)
            {
                action?.Invoke();
                yield break;
            }

            CreateTransition();

            yield return new WaitForSecondsRealtime(0.5f);

            action?.Invoke();
            if (waitUntilFunc != null) yield return new WaitUntil(waitUntilFunc);

            yield return new WaitForSecondsRealtime(waitUntilEndDelay);

            EndTransition(true);
            yield break;
        }
    }
}
