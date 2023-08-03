using CDOverhaul.Graphics;
using HarmonyLib;
namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(PlayerAllyManager))]
    internal static class PlayerAllyManager_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("onCharacterPlayedEmote")]
        private static bool onCharacterPlayedEmote_Prefix(PlayerAllyManager __instance, FirstPersonMover emotingPlayer)
        {
            if (!GameModeManager.IsStoryChapter3() && !GameModeManager.IsStoryChapter5())
                return false;

            if (!emotingPlayer || CharacterTracker.Instance.GetPlayerAlly() == emotingPlayer)
                return false;

            __instance.StartCoroutine(BaseFixes.WaitThenMirrorEmote_Patched(emotingPlayer.GetEmotePlaying()));
            return false;
        }
    }
}