using CDOverhaul.Credits;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(ObjectPlacedInLevel))]
    internal static class ObjectPlacedInLevel_Patch
    {
        private static Dictionary<ObjectPlacedInLevel, List<ThreeDOutline>> m_Objects = new Dictionary<ObjectPlacedInLevel, List<ThreeDOutline>>();

        [HarmonyPrefix]
        [HarmonyPatch("replaceMaterialWithSelected")]
        private static bool replaceMaterialWithSelected_Prefix(ObjectPlacedInLevel __instance, Renderer targetRenderer)
        {
            if (!OverhaulMod.IsModInitialized)
            {
                return true;
            }

            ThreeDOutline o = targetRenderer.GetComponent<ThreeDOutline>();
            if (o == null)
            {
                o = targetRenderer.gameObject.AddComponent<ThreeDOutline>();

                if (m_Objects.ContainsKey(__instance))
                {
                    m_Objects[__instance].Add(o);
                    return false;
                }
                m_Objects.Add(__instance, new List<ThreeDOutline>() { o });
            }
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("ChangeToNotSelectedVisuals")]
        private static bool ChangeToNotSelectedVisuals_Prefix(ObjectPlacedInLevel __instance)
        {
            if (!OverhaulMod.IsModInitialized)
            {
                return true;
            }

            if (m_Objects.ContainsKey(__instance))
            {
                List<ThreeDOutline> list = m_Objects[__instance];
                for (int i = list.Count - 1; i > -1; i--)
                {
                    if (list[i] != null)
                    {
                        Object.Destroy(list[i]);
                    }
                }
                return false;
            }
            return false;
        }
    }
}
