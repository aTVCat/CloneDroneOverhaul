using CDOverhaul.DevTools;
using CDOverhaul.Gameplay;
using HarmonyLib;
using System.Diagnostics;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(AISwordsmanController))]
    internal static class AISwordsmanController_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("FixedUpdate")]
        private static bool FixedUpdate_Prefix(AISwordsmanController __instance)
        {
            if (!OverhaulMod.IsModInitialized)
                return true;

            Stopwatch stopwatch = OverhaulProfiler.StartTimer();

            Character character = __instance.GetCharacter();
            if (!character)
            {
                stopwatch.StopTimer("AI_SC_FixedUpdate_Pre");
                return true;
            }

            int instanceId = character.GetInstanceID();
            if (!CharacterExpansionContainer.CachedContainers.ContainsKey(instanceId))
            {
                stopwatch.StopTimer("AI_SC_FixedUpdate_Pre");
                return true;
            }

            CharacterExpansionContainer expansionContainer = CharacterExpansionContainer.CachedContainers[instanceId];
            foreach (OverhaulCharacterExpansion b in expansionContainer.Expansions)
            {
                if (b)
                {
                    b.OnPreAIUpdate(__instance, out bool continueEx);
                    if (!continueEx)
                    {
                        stopwatch.StopTimer("AI_SC_FixedUpdate_Pre");
                        return false;
                    }
                }
            }
            stopwatch.StopTimer("AI_SC_FixedUpdate_Pre");
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch("FixedUpdate")]
        private static void FixedUpdate_Postfix(AISwordsmanController __instance)
        {
            if (!OverhaulMod.IsModInitialized)
                return;

            Stopwatch stopwatch = OverhaulProfiler.StartTimer();

            Character character = __instance.GetCharacter();
            if (!character)
            {
                stopwatch.StopTimer("AI_SC_FixedUpdate_Post");
                return;
            }

            int instanceId = character.GetInstanceID();
            if (!CharacterExpansionContainer.CachedContainers.ContainsKey(instanceId))
            {
                stopwatch.StopTimer("AI_SC_FixedUpdate_Post");
                return;
            }

            CharacterExpansionContainer expansionContainer = CharacterExpansionContainer.CachedContainers[instanceId];
            foreach (OverhaulCharacterExpansion b in expansionContainer.Expansions)
            {
                if (b)
                    b.OnPostAIUpdate(__instance);
            }
            stopwatch.StopTimer("AI_SC_FixedUpdate_Post");
        }
    }
}
