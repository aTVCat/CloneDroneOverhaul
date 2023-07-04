using CDOverhaul.Graphics.Robots;
using HarmonyLib;
using UnityEngine;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(SeveredVolumeGenerator))]
    internal static class SeveredVolumeGenerator_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("CreateVolumeCopy")]
        private static void CreateVolumeCopy_Postfix(SeveredBodyPart __result)
        {
            if(Random.Range(0, 10) < 7)
            {
                _ = __result.gameObject.AddComponent<SeveredBodyPartSparks>();
            }
            __result.gameObject.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
        }
    }
}
