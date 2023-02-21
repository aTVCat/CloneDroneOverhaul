using HarmonyLib;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(Story3_EndCutscene))]
    internal static class Story3_EndCutscene_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("startMainEndSequence")]
        private static void startMainEndSequence_Postfix(Story3_EndCutscene __instance)
        {
            /*
            TimeManager.Instance.RestoreTimeScale();
            BaseFixes f = ReplacementBase.GetReplacement<BaseFixes>();
            if(f != null && f.GetAllyFix() != null)
            {
                f.GetAllyFix().StartForcingAllyToStayAtPositionForever();
            }*/
        }

        [HarmonyPostfix]
        [HarmonyPatch("OnEmperorStartedFalling")]
        private static void OnEmperorStartedFalling_Postfix(Story3_EndCutscene __instance)
        {
            /*
            BaseFixes f = ReplacementBase.GetReplacement<BaseFixes>();
            if (f != null && f.GetAllyFix() != null)
            {
                f.GetAllyFix().StopFixingAlly();
            }*/
        }
    }
}
