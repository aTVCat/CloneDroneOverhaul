using CDOverhaul.Gameplay;
using HarmonyLib;

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

            Character character = __instance.GetCharacter();
            if (!character)
            {
                return true;
            }

            int instanceId = character.GetInstanceID();
            if (!CharacterExpansionContainer.CachedContainers.ContainsKey(instanceId))
            {
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
                        return false;
                    }
                }
            }
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch("FixedUpdate")]
        private static void FixedUpdate_Postfix(AISwordsmanController __instance)
        {
            if (!OverhaulMod.IsModInitialized)
                return;

            Character character = __instance.GetCharacter();
            if (!character)
            {
                return;
            }

            int instanceId = character.GetInstanceID();
            if (!CharacterExpansionContainer.CachedContainers.ContainsKey(instanceId))
            {
                return;
            }

            CharacterExpansionContainer expansionContainer = CharacterExpansionContainer.CachedContainers[instanceId];
            foreach (OverhaulCharacterExpansion b in expansionContainer.Expansions)
            {
                if (b)
                    b.OnPostAIUpdate(__instance);
            }
        }
    }
}
