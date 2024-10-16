﻿using HarmonyLib;
using ModLibrary;
using OverhaulMod.Combat;
using OverhaulMod.Combat.Weapons;
using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(FirstPersonMover))]
    internal static class FirstPersonMover_Patch
    {
        /*
        [HarmonyPrefix]
        [HarmonyPatch(nameof(FirstPersonMover.tryKick))]
        private static bool tryKick_Prefix(FirstPersonMover __instance, FPMoveCommand moveCommand, bool isFirstExecution, bool isOwner)
        {
            CharacterModel characterModel = __instance._characterModel;
            return !characterModel || !characterModel.IsWeaponModelVisibleAndNotDropped((WeaponType)52);
        }*/

        /*
        [HarmonyPrefix]
        [HarmonyPatch(nameof(FirstPersonMover.PushDownIfAboveGround))]
        private static bool PushDownIfAboveGround_Prefix(FirstPersonMover __instance)
        {
            _ = ModActionUtils.RunCoroutine(ModCore.PushDownIfAboveGroundCoroutine_Patch(__instance));
            return false;
        }*/

        [HarmonyPostfix]
        [HarmonyPatch(nameof(FirstPersonMover.UnSetGrabbedByGarbageBot))]
        private static void UnSetGrabbedByGarbageBot_Postfix(FirstPersonMover __instance)
        {
            if (GameModeManager.IsBattleRoyale())
            {
                if (__instance.GetEquippedWeaponType() == WeaponType.None)
                {
                    ModGameUtils.WaitForPlayerInputUpdate(delegate (IFPMoveCommandInput commandInput)
                    {
                        commandInput.NextWeapon = true;
                    });
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(FirstPersonMover.tryRenderAttack))]
        private static void tryRenderAttack_Prefix(FirstPersonMover __instance, int attackServerFrame, ref AttackDirection attackDirection)
        {
            if (__instance.GetEquippedWeaponModel() is ModWeaponModel modWeaponModel)
            {
                if (!modWeaponModel.attackDirections.HasFlag(attackDirection))
                    attackDirection = modWeaponModel.defaultAttackDirection;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(FirstPersonMover.tryEnableJump))]
        private static void tryEnableJump_Prefix(FirstPersonMover __instance, FPMoveCommand moveCommand, Vector3 platformVelocity, float boltFrameDeltaTime, bool isImmobile, bool isFirstExecution)
        {
            if (GameModeManager.IsMultiplayer() || !__instance.IsMainPlayer())
                return;

            RobotInventory robotInventory = ModComponentCache.GetRobotInventory(__instance.transform);
            if (robotInventory)
            {
                if (__instance._isOnGroundServer)
                {
                    robotInventory.IsNotAbleToDoubleJump = false;
                }

                if (!robotInventory.IsNotAbleToDoubleJump && robotInventory.hasDoubleJumpAbility && __instance._isJumping && moveCommand.Input.Jump)
                {
                    EnergySource energySource = __instance._energySource;
                    if (!energySource || !energySource.CanConsume(0.5f))
                    {
                        ModCache.gameUIRoot.EnergyUI.onInsufficientEnergyAttempt(0.5f);
                        return;
                    }
                    energySource.Consume(0.5f);

                    __instance.AddVelocity(__instance.JumpVelocity);
                    robotInventory.IsNotAbleToDoubleJump = true;
                    AttackManager.Instance.CreateBattleCruiserGatlingImpactVFX(__instance.transform.position + Vector3.up);
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(FirstPersonMover.SimulateController))]
        private static void SimulateController_Postfix(FirstPersonMover __instance)
        {
            if (!__instance.IsMainPlayer())
                return;

            ModGameUtils.InvokePlayerInputUpdateAction(__instance._moveCommandInput);
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(FirstPersonMover.executeAttackCommands))]
        private static void executeAttackCommands_Postfix(FirstPersonMover __instance, FPMoveCommand moveCommand, bool isImmobile, bool isFirstExecution, bool isOwner)
        {
            if (__instance.GetEquippedWeaponModel() is ModWeaponModel modWeaponModel)
            {
                modWeaponModel.OnExecuteAttackCommands(__instance, moveCommand.Input);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(FirstPersonMover.HasMeleeWeaponEquipped))]
        private static void HasMeleeWeaponEquipped_Postfix(FirstPersonMover __instance, ref bool __result)
        {
            if (!__result)
                __result = ModWeaponsManager.Instance.IsMeleeWeapon(__instance._currentWeapon);
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(FirstPersonMover.getWeaponDisabledTimeAfterCut))]
        private static void getWeaponDisabledTimeAfterCut_Postfix(FirstPersonMover __instance, ref float __result)
        {
            if (__instance.GetEquippedWeaponModel() is ModWeaponModel modWeaponModel)
            {
                __result = modWeaponModel.disableAttacksForSeconds;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(FirstPersonMover.RefreshWeaponAnimatorProperties))]
        private static void RefreshWeaponAnimatorProperties_Postfix(FirstPersonMover __instance)
        {
            if (__instance.GetEquippedWeaponModel() is ModWeaponModel modWeaponModel)
            {
                if (modWeaponModel.animatorControllerOverride)
                {
                    CharacterModel characterModel = __instance._characterModel;
                    if (characterModel)
                    {
                        characterModel.SetUpperAnimator(modWeaponModel.animatorControllerOverride);
                    }
                }
                modWeaponModel.OnRefreshWeaponAnimatorProperties(__instance);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(FirstPersonMover.GetAttackSpeed))]
        private static void GetAttackSpeed_Postfix(FirstPersonMover __instance, ref float __result)
        {
            if (GameModeManager.UsesMultiplayerSpeedMultiplier())
                return;

            if (__instance.GetEquippedWeaponModel() is ModWeaponModel modWeaponModel)
            {
                __result = modWeaponModel.attackSpeed;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(FirstPersonMover.CreateCharacterModel))]
        private static void CreateCharacterModel_Postfix(FirstPersonMover __instance)
        {
            ModWeaponsManager.Instance.AddWeaponsToRobot(__instance);
        }
    }
}
