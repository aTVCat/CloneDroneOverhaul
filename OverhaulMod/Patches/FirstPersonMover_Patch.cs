using HarmonyLib;
using ModLibrary;
using Org.BouncyCastle.Crypto;
using OverhaulMod.Combat.Weapons;
using static BoltAssets;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(FirstPersonMover))]
    internal static class FirstPersonMover_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("tryKick")]
        private static bool tryKick_Prefix(FirstPersonMover __instance, FPMoveCommand moveCommand, bool isFirstExecution, bool isOwner)
        {
            if (__instance._characterModel.IsWeaponModelVisibleAndNotDropped((WeaponType)52))
            {
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("tryRenderAttack")]
        private static void tryRenderAttack_Prefix(FirstPersonMover __instance, int attackServerFrame, ref AttackDirection attackDirection)
        {
            WeaponModel weaponModel = __instance.GetEquippedWeaponModel();
            if (!(weaponModel is ModWeaponModel))
                return;

            ModWeaponModel modWeaponModel = (ModWeaponModel)weaponModel;
            if (modWeaponModel)
            {
                if (!modWeaponModel.attackDirections.HasFlag(attackDirection))
                    attackDirection = modWeaponModel.defaultAttackDirection;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("HasMeleeWeaponEquipped")]
        private static void HasMeleeWeaponEquipped_Postfix(FirstPersonMover __instance, ref bool __result)
        {
            if (!__result)
            {
                __result = ModWeaponsManager.Instance.IsMeleeWeapon(__instance._currentWeapon);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("getWeaponDisabledTimeAfterCut")]
        private static void getWeaponDisabledTimeAfterCut_Postfix(FirstPersonMover __instance, ref float __result)
        {
            WeaponModel weaponModel = __instance.GetEquippedWeaponModel();
            if (!(weaponModel is ModWeaponModel))
                return;

            ModWeaponModel modWeaponModel = (ModWeaponModel)weaponModel;
            if (modWeaponModel)
            {
                __result = modWeaponModel.disableAttacksForSeconds;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("RefreshWeaponAnimatorProperties")]
        private static void RefreshWeaponAnimatorProperties_Postfix(FirstPersonMover __instance)
        {
            WeaponModel weaponModel = __instance.GetEquippedWeaponModel();
            if (!(weaponModel is ModWeaponModel))
                return;

            ModWeaponModel modWeaponModel = (ModWeaponModel)weaponModel;
            if (modWeaponModel)
            {
                if (modWeaponModel.animatorControllerOverride)
                {
                    __instance._characterModel.SetUpperAnimator(modWeaponModel.animatorControllerOverride);
                }
                modWeaponModel.OnRefreshWeaponAnimatorProperties(__instance);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("GetAttackSpeed")]
        private static void GetAttackSpeed_Postfix(FirstPersonMover __instance, ref float __result)
        {
            if (GameModeManager.UsesMultiplayerSpeedMultiplier())
            {
                return;
            }

            WeaponModel weaponModel = __instance.GetEquippedWeaponModel();
            if (!(weaponModel is ModWeaponModel))
                return;

            ModWeaponModel modWeaponModel = (ModWeaponModel)weaponModel;
            if (modWeaponModel)
            {
                __result = modWeaponModel.attackSpeed;
            }
        }
    }
}
