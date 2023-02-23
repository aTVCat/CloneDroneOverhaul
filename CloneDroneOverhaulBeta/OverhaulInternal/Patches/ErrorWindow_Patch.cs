using HarmonyLib;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(ErrorWindow))]
    internal static class ErrorWindow_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Show")]
        private static bool Show_Prefix(string errorMessage)
        {
            /*
            if (Time.timeSinceLevelLoad < 3f)
            {
                OverhaulExceptions.OnModCrashedWhileLoading(errorMessage);

                ErrorManager.Instance.SetPrivateField<bool>("_hasCrashed", false);

                return false;
            }*/
            return true;
        }
    }
}
