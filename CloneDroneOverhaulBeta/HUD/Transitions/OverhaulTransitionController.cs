using CDOverhaul.HUD.Transitions;
using UnityEngine;

namespace CDOverhaul
{
    /// <summary>
    /// Todo: Add better transitions with cool loading animation
    /// </summary>
    public static class OverhaulTransitionController
    {
        public static bool IsNewTransitionEnabled => OverhaulFeatureAvailabilitySystem.BuildImplements.IsNewTransitionScreenEnabled;

        private static GameObject s_TransitionPrefab;

        private static bool s_FadeOutOnSceneLoad;
        private static OverhaulTransitionBehaviour s_CurrentTransition;

        internal static void Initialize()
        {
            RectTransform rectT = OverhaulAssetsController.GetAsset("LoadingScreen", OverhaulAssetPart.Main).GetComponent<RectTransform>();
            SceneTransitionManager.Instance.SceneTransitionCoverScreen = rectT;

            if (IsNewTransitionEnabled)
            {
                if(!s_TransitionPrefab)
                    s_TransitionPrefab = OverhaulAssetsController.GetAsset("OverhaulTransition", OverhaulAssetPart.Main);

                if (s_FadeOutOnSceneLoad)
                    CreateTransition(false, true);
            }
        }

        public static void CreateTransition(bool isSceneSwitching = false, bool fadeOut = false)
        {
            if (HasSpawnedTransitionHUD())
                return;

            s_CurrentTransition = Object.Instantiate(s_TransitionPrefab).AddComponent<OverhaulTransitionBehaviour>();
            s_CurrentTransition.Initialize(fadeOut, isSceneSwitching);
            s_FadeOutOnSceneLoad = isSceneSwitching;
        }

        public static void EndTransition(bool fadeOut = true)
        {
            if (!HasSpawnedTransitionHUD())
                return;

            if(fadeOut)
            {
                s_CurrentTransition.Initialize(true);
                return;
            }
            Object.Destroy(s_CurrentTransition.gameObject);
        }

        public static bool HasSpawnedTransitionHUD() => s_CurrentTransition;

        public static void GoToMainMenu()
        {
            CreateTransition(true, false);
        }
    }
}
