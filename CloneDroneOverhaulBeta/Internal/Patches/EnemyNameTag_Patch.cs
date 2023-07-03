using CDOverhaul.Gameplay.QualityOfLife;
using HarmonyLib;
using PicaVoxel;
using UnityEngine;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(EnemyNameTag))]
    internal static class EnemyNameTag_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Initialize")]
        private static void Initialize_Postfix(EnemyNameTag __instance)
        {
            if (!OverhaulMod.IsModInitialized)
                return;

            if (ModBotTagDisabler.DisableTags)
            {
                __instance.NameText.gameObject.AddComponent<ModBotTagRemoverBehaviour>().NormalUsername = __instance.NameText.text;
            }
        }
    }
}
