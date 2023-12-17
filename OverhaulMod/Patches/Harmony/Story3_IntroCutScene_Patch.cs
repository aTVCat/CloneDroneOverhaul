using HarmonyLib;
using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverhaulMod.Patches.Harmony
{
    [HarmonyPatch(typeof(Story3_IntroCutScene))]
    internal static class Story3_IntroCutScene_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("startMission")]
        private static void startMission_Postfix()
        {
            MetagameProgressManager.Instance.SetProgress(MetagameProgress.P6_EnteredFleetBeacon);
        }
    }
}
