using HarmonyLib;
using OverhaulMod.UI;
using OverhaulMod.Utils;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(GameModeSelectScreen))]
    internal static class GameModeSelectScreen_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("SetMainScreenVisible")]
        private static bool SetMainScreenVisible_Prefix(GameModeSelectScreen __instance, bool visible)
        {
            bool isSinglePlayer = __instance.gameObject.name.StartsWith("Singleplayer");
            if (isSinglePlayer)
                return true;
            else
            {
                UIMultiplayerGameModeSelectScreen multiplayerGameModeSelectScreen = ModUIManager.Instance.Get<UIMultiplayerGameModeSelectScreen>(AssetBundleConstants.UI, ModUIConstants.UI_MULTIPLAYER_GAMEMODE_SELECT_SCREEN);
                if (multiplayerGameModeSelectScreen && multiplayerGameModeSelectScreen.visibleInHierarchy)
                {
                    multiplayerGameModeSelectScreen.SetMainScreenVisible(visible);
                    __instance.MainScreenBox.gameObject.SetActive(false);
                    return false;
                }
            }
            return true;
        }
    }
}
