using System;
using System.Collections;
using UnityEngine;

namespace CDOverhaul
{
    public class OverhaulTransitionManager : OverhaulManager<OverhaulTransitionManager>
    {
        private static bool FadeOutOnSceneReload;

        private GameObject s_OldTransitionPrefab;
        private GameObject s_TransitionPrefab;

        public OverhaulTransitionBehaviour transition
        {
            get;
            private set;
        }

        public bool doingTransition
        {
            get => transition;
        }

        protected override void OnAssetsLoaded()
        {
            s_OldTransitionPrefab = OverhaulAssetsController.GetAsset("LoadingScreen", OverhaulAssetPart.Main);
            s_TransitionPrefab = OverhaulAssetsController.GetAsset("OverhaulTransition", OverhaulAssetPart.Main);
            refreshCoverScreen();
        }

        public override void OnSceneReloaded()
        {
            if (FadeOutOnSceneReload)
                DoTransition(false, true);

            refreshCoverScreen();
        }

        private void refreshCoverScreen()
        {
            if (!s_OldTransitionPrefab)
                return;

            SceneTransitionManager.Instance.SceneTransitionCoverScreen = s_OldTransitionPrefab.transform as RectTransform;
        }

        public void DoTransition(bool isSceneSwitching = false, bool fadeOut = false)
        {
            if (doingTransition)
                return;

            transition = Instantiate(s_TransitionPrefab).AddComponent<OverhaulTransitionBehaviour>();
            transition.Initialize(fadeOut, isSceneSwitching);
            FadeOutOnSceneReload = isSceneSwitching;
        }

        public void DoTransition(Action action, Func<bool> waitUntilFunc = null, float waitUntilEndDelay = 0.35f)
        {
            _ = StaticCoroutineRunner.StartStaticCoroutine(doTransitionWithActionCoroutine(action, waitUntilFunc, waitUntilEndDelay));
        }

        public void EndTransition(bool fadeOut = true)
        {
            if (!doingTransition)
                return;

            if (fadeOut)
            {
                transition.Initialize(true);
                return;
            }
            Destroy(transition.gameObject);
            transition = null;
        }

        public void GoToMainMenu()
        {
            DoTransition(true, false);
        }

        private IEnumerator doTransitionWithActionCoroutine(Action action, Func<bool> waitUntilFunc = null, float waitUntilEndDelay = 0.35f)
        {
            if (!s_TransitionPrefab)
            {
                action?.Invoke();
                yield break;
            }

            DoTransition();
            yield return new WaitForSecondsRealtime(0.5f);

            action?.Invoke();
            if (waitUntilFunc != null) yield return new WaitUntil(waitUntilFunc);

            yield return new WaitForSecondsRealtime(waitUntilEndDelay);

            EndTransition(true);
            yield break;
        }
    }
}
