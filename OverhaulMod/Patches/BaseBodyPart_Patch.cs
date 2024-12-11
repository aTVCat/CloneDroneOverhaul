using HarmonyLib;
using OverhaulMod.Content.Personalization;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using OverhaulMod.Visuals;
using PicaVoxel;
using System;
using UnityEngine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(BaseBodyPart))]
    internal static class BaseBodyPart_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(BaseBodyPart.dispatchBodyPartDamaged))]
        private static void dispatchBodyPartDamaged_Postfix(BaseBodyPart __instance, int attackID, Vector3 impactDirection, Character damageOrigin, DamageSourceType damageSourceType)
        {
            PersonalizationAccessoryReferences personalizationAccessoryReferences = ModComponentCache.GetPersonalizationAccessoryReferences(__instance.transform);
            if (personalizationAccessoryReferences)
            {
                personalizationAccessoryReferences.RefreshVisibility();
            }
        }
    }
}
