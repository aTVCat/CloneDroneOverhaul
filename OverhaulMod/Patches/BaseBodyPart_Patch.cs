using HarmonyLib;
using OverhaulMod.Content.Personalization;
using OverhaulMod.Engine;
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
            if (!ModCore.IsActive()) return;

            PersonalizationAccessoryReferences personalizationAccessoryReferences = ComponentCacheManager.Instance.GetPersonalizationAccessoryReferences(__instance.transform);
            if (personalizationAccessoryReferences)
            {
                personalizationAccessoryReferences.RefreshVisibility();
            }
        }
    }
}
