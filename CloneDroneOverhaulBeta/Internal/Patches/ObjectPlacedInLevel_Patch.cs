using CDOverhaul.Credits;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(ObjectPlacedInLevel))]
    internal static class ObjectPlacedInLevel_Patch
    {
        private static readonly Dictionary<ObjectPlacedInLevel, List<ThreeDOutline>> m_Objects = new Dictionary<ObjectPlacedInLevel, List<ThreeDOutline>>();

        [HarmonyPrefix]
        [HarmonyPatch("replaceMaterialWithSelected")]
        private static bool replaceMaterialWithSelected_Prefix(ObjectPlacedInLevel __instance, Renderer targetRenderer)
        {
            if (!OverhaulMod.IsModInitialized || !OverhaulFeatureAvailabilitySystem.BuildImplements.IsSelectionOutLineEnabled)
                return true;

            ThreeDOutline threeDOutline = targetRenderer.GetComponent<ThreeDOutline>();
            if (threeDOutline == null)
            {
                threeDOutline = targetRenderer.gameObject.AddComponent<ThreeDOutline>();

                if (m_Objects.ContainsKey(__instance))
                {
                    m_Objects[__instance].Add(threeDOutline);
                    return true;
                }
                m_Objects.Add(__instance, new List<ThreeDOutline>() { threeDOutline });
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("ChangeToNotSelectedVisuals")]
        private static bool ChangeToNotSelectedVisuals_Prefix(ObjectPlacedInLevel __instance)
        {
            if (!OverhaulMod.IsModInitialized)
                return true;

            if (OverhaulFeatureAvailabilitySystem.BuildImplements.IsSelectionOutLineEnabled && !m_Objects.IsNullOrEmpty() && m_Objects.ContainsKey(__instance))
            {
                List<ThreeDOutline> list = m_Objects[__instance];
                if (!list.IsNullOrEmpty())
                {
                    for (int i = list.Count - 1; i > -1; i--)
                    {
                        ThreeDOutline outl = list[i];
                        if (outl != null)
                        {
                            Object.Destroy(outl);
                        }
                    }
                }
            }
            return true;
        }
    }
}
