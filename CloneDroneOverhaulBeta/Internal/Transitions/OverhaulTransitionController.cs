using UnityEngine;

namespace CDOverhaul.Internal.Transitions
{
    /// <summary>
    /// Todo: Add better transitions without bugs and with cool loading animation
    /// </summary>
    public static class OverhaulTransitionController
    {
        private static RectTransform m_CurrentTransition;

        internal static void Initialize()
        {
            RectTransform rectT = AssetsController.GetAsset("LoadingScreen", OverhaulAssetsPart.Main).GetComponent<RectTransform>();
            SceneTransitionManager.Instance.SceneTransitionCoverScreen = rectT;
        }

        public static void CreateTransition()
        {
            if (TransitionHUDSpawned())
            {
                return;
            }
            m_CurrentTransition = SceneTransitionManager.Instance.InstantiateSceneTransitionOverlay();
        }

        public static void EndTransition()
        {
            if (!TransitionHUDSpawned())
            {
                return;
            }
            Object.Destroy(m_CurrentTransition.gameObject);
        }

        public static bool TransitionHUDSpawned()
        {
            return m_CurrentTransition != null;
        }
    }
}
