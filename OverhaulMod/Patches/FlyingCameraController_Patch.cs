using HarmonyLib;
using OverhaulMod.Utils;
using Rewired;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(FlyingCameraController))]
    internal static class FlyingCameraController_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Update")]
        private static bool Update_Prefix(FlyingCameraController __instance)
        {
            if (UIManager.Instance.IsMouseOverUIElement())
                return false;

            if (!PhotoManager.Instance.IsInPhotoMode() || __instance._isMovementDisabled)
                return true;

            Player player = ReInput.players.GetPlayer(0);
            if (player != null)
            {
                bool rmbHeld = player.GetButton(3);
                InputManager.Instance.SetCursorEnabled(!rmbHeld);
            }
            return true;
        }

        [HarmonyTranspiler]
        [HarmonyPatch("Update")]
        private static IEnumerable<CodeInstruction> Update_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count; i++)
            {
                CodeInstruction ci = codes[i];
                if (ci.opcode == OpCodes.Call && ci.Calls(ModCache.unityTimeFixedUnscaledDeltaTimePropertyGetter))
                {
                    codes[i] = new CodeInstruction(OpCodes.Call, ModCache.unityTimeUnscaledDeltaTimePropertyGetter);
                }
            }

            return codes.AsEnumerable();
        }
    }
}
