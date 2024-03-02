﻿using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(SettingsMenu))]
    internal static class SettingsMenu_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("refreshResolutionOptions")]
        private static void refreshResolutionOptions_Postfix(SettingsMenu __instance)
        {
            Resolution resolution = new Resolution
            {
                width = Screen.width,
                height = Screen.height,
                refreshRate = -1
            };

            List<SettingsResolution> list = __instance._uniqueResolutionOptions;
            foreach (SettingsResolution r in list)
                if (r.Resolution.width == resolution.width && r.Resolution.height == resolution.height)
                    return;

            resolution.refreshRate = list[list.Count - 1].Resolution.refreshRate;
            list.Add(new SettingsResolution(resolution));
            __instance._uniqueResolutionOptions = list.OrderBy(s => s.Resolution.width).ThenBy(s => s.Resolution.height).ToList();
        }
    }
}