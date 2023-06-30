using CDOverhaul.Graphics.Robots;
using HarmonyLib;
using UnityEngine;

namespace CDOverhaul.Internal.Patches
{
    [HarmonyPatch(typeof(SeveredVolumeGenerator))]
    internal static class SeveredVolumeGenerator_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("CreateVolumeCopy")]
        private static void CreateVolumeCopy_Postfix(SeveredBodyPart __result)
        {
            _ = __result.gameObject.AddComponent<SeveredBodyPartSparks>();
            __result.gameObject.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
        }
    }
}
