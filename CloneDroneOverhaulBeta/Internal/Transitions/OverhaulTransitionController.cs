using UnityEngine;

namespace CDOverhaul.Internal.Transitions
{
    /// <summary>
    /// Todo: Add better transitions without bugs and with cool loading animation
    /// </summary>
    public static class OverhaulTransitionController
    {
        internal static void Initialize()
        {
            RectTransform rectT = AssetController.GetAsset("LoadingScreen", OverhaulAssetsPart.Main).GetComponent<RectTransform>();
            SceneTransitionManager.Instance.SceneTransitionCoverScreen = rectT;
        }
    }
}
