using HarmonyLib;
using OverhaulMod.Content;
using static CharacterModel;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(CharacterModel))]
    internal static class CharacterModel_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(CharacterModel.OverridePatternColor), new System.Type[] { typeof(PatternColorSet), typeof(bool) })]
        private static void OverridePatternColor_Prefix(CharacterModel __instance, ref PatternColorSet newColor, bool forceMultiplayerHSBReplacement = false)
        {
            FirstPersonMover firstPersonMover = __instance.GetOwner();
            if (!firstPersonMover || (firstPersonMover.IsDetached() && firstPersonMover.CharacterType != EnemyType.None) || firstPersonMover.IsAIControlled()) return;

            ExclusivePerkManager exclusivePerkManager = ExclusivePerkManager.Instance;
            newColor.SwordColor = exclusivePerkManager.GetOverrideRobotColor(firstPersonMover, newColor.SwordColor);
            newColor.BowColor = exclusivePerkManager.GetOverrideRobotColor(firstPersonMover, newColor.BowColor);
            newColor.HammerColor = exclusivePerkManager.GetOverrideRobotColor(firstPersonMover, newColor.HammerColor);
            newColor.SpearColor = exclusivePerkManager.GetOverrideRobotColor(firstPersonMover, newColor.SpearColor);
            newColor.HeadColor = exclusivePerkManager.GetOverrideRobotColor(firstPersonMover, newColor.HeadColor);
            newColor.BodyColor = exclusivePerkManager.GetOverrideRobotColor(firstPersonMover, newColor.BodyColor);
            newColor.RootColor = exclusivePerkManager.GetOverrideRobotColor(firstPersonMover, newColor.RootColor);
        }
    }
}
