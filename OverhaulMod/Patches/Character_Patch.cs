/*using HarmonyLib;
using OverhaulMod.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(Character))]
    internal static class Character_Patch
    {
        [HarmonyTranspiler]
        [HarmonyPatch(nameof(Character.OnBodyPartsDamaged))]
        private static IEnumerable<CodeInstruction> OnBodyPartsDamaged_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count; i++)
            {
                CodeInstruction ci = codes[i];
                if (ci.opcode == OpCodes.Callvirt && ci.Calls(ModCache.SetTimeScaleForSeconds))
                {
                    codes[i - 2].operand = 0.2f;
                    codes[i - 1].operand = 0.3f;
                    break;
                }
            }

            return codes.AsEnumerable();
        }
    }
}
*/