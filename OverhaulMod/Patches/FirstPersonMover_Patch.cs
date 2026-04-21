using HarmonyLib;
using OverhaulMod.Combat;
using OverhaulMod.Combat.Weapons;
using OverhaulMod.Content.Personalization;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using OverhaulMod.Visuals;
using UnityEngine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(FirstPersonMover))]
    internal static class FirstPersonMover_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(FirstPersonMover.OnMindTransferFinished))]
        private static void OnMindTransferFinished_Postfix(FirstPersonMover __instance)
        {
            if (__instance.HasCharacterModel() && __instance._playerCamera)
                CameraManager.Instance.AddControllers(__instance._playerCamera, __instance); // fix camera controllers not adding to enemies in story mode
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(FirstPersonMover.onDeath))]
        private static void onDeath_Prefix(FirstPersonMover __instance, out CharacterModel __state)
        {
            __state = __instance.GetCharacterModel();
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(FirstPersonMover.onDeath))]
        private static void onDeath_Postfix(FirstPersonMover __instance, CharacterModel __state)
        {
            if (__state)
            {
                Rigidbody rigidbody = __state.GetComponent<Rigidbody>();
                if (rigidbody)
                {
                    rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
                    DisableRigidBodyInterpolation disableInterpolation = __state.gameObject.AddComponent<DisableRigidBodyInterpolation>();
                    disableInterpolation.Initialize(rigidbody, 1f);
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(FirstPersonMover.tryRenderAttack))]
        private static void tryRenderAttack_Prefix(FirstPersonMover __instance, int attackServerFrame, ref AttackDirection attackDirection)
        {
            WeaponModel wm = __instance._currentWeaponModel;
            if (wm && wm.WeaponType == ModWeaponsManager.SCYTHE_TYPE && wm is ModWeaponModel modWeaponModel) // temporary made it work for scythe only
            {
                if (!modWeaponModel.attackDirections.HasFlag(attackDirection))
                    attackDirection = modWeaponModel.defaultAttackDirection;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(FirstPersonMover.tryEnableJump))]
        private static void tryEnableJump_Prefix(FirstPersonMover __instance, FPMoveCommand moveCommand, Vector3 platformVelocity, float boltFrameDeltaTime, bool isImmobile, bool isFirstExecution)
        {
            if (GameModeManager.IsMultiplayer() || !__instance.IsMainPlayer() || __instance.isImmobilized() || !__instance._isJumping || !moveCommand.Input.Jump)
                return;

            CharacterExtension characterExtension = ModComponentCache.GetRobotInventory(__instance.transform);
            if (characterExtension && characterExtension.CanPerformDoubleJump())
            {
                bool isMainPlayer = __instance.IsMainPlayer();
                if (__instance.IsDamaged(MechBodyPartType.LeftLeg) || __instance.IsDamaged(MechBodyPartType.RightLeg))
                {
                    if (isMainPlayer) ModCache.UIRoot.EnergyUI.SetErrorLabelVisible(LocalizationManager.Instance.GetTranslatedString("cant_double_jump_without_leg"));
                    return;
                }

                float energyToConsume = 1f;
                EnergySource energySource = __instance._energySource;
                if (!energySource || !energySource.CanConsume(energyToConsume))
                {
                    if (isMainPlayer) ModCache.UIRoot.EnergyUI.onInsufficientEnergyAttempt(energyToConsume);
                    return;
                }
                energySource.Consume(energyToConsume);

                Vector3 position = __instance.transform.position + Vector3.up;
                Vector3 velocityToAdd = (__instance.JumpVelocity * 1.4f) + (__instance.transform.forward * 4f);
                Vector3 velocity = __instance.GetVelocity();
                velocity.x += velocityToAdd.x;
                velocity.y = Mathf.Max(0f, velocity.y * 0.5f);
                velocity.y += velocityToAdd.y;
                velocity.z += velocityToAdd.z;
                __instance.SetVelocity(velocity);

                if (isMainPlayer) PlayerCameraManager.Instance.ShakeCamera(0.06f, 0.3f);

                AttackManager.Instance.CreateBattleCruiserGatlingImpactVFX(position);

                WorldAudioSource worldAudioSource = AudioManager.Instance.PlayClipAtPosition(ModAudioLibrary.Instance.DoubleJump, position);
                worldAudioSource._audioSource.spatialBlend = 0.6f;
                worldAudioSource.gameObject.AddComponent<RestoreSpatalizeBlendOnDisable>().Initialize(worldAudioSource._audioSource);

                characterExtension.OnPerformedDoubleJump();
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(FirstPersonMover.SimulateController))]
        private static void SimulateController_Postfix(FirstPersonMover __instance)
        {
            if (!__instance.IsMainPlayer()) return;

            ModGameUtils.InvokePlayerInputUpdateAction(__instance._moveCommandInput);
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
            WeaponModel wm = __instance._currentWeaponModel;
            if (wm && wm.WeaponType == ModWeaponsManager.SCYTHE_TYPE && wm is ModWeaponModel modWeaponModel)
            {
                __result = modWeaponModel.disableAttacksForSeconds;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(FirstPersonMover.RefreshWeaponAnimatorProperties))]
        private static void RefreshWeaponAnimatorProperties_Postfix(FirstPersonMover __instance)
        {
            WeaponModel wm = __instance._currentWeaponModel;
            if (wm && wm.WeaponType == ModWeaponsManager.SCYTHE_TYPE && wm is ModWeaponModel modWeaponModel)
            {
                modWeaponModel.OnRefreshWeaponAnimatorProperties(__instance);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(FirstPersonMover.GetAttackSpeed))]
        private static bool GetAttackSpeed_Prefix(FirstPersonMover __instance, ref float __result)
        {
            WeaponModel wm = __instance._currentWeaponModel;
            if (wm && wm.WeaponType == ModWeaponsManager.SCYTHE_TYPE && wm is ModWeaponModel modWeaponModel)
            {
                __result = modWeaponModel.attackSpeed;
                if (GameModeManager.UsesMultiplayerSpeedMultiplier()) __result *= AttackManager.Instance.MultiplayerAttackSpeedMultiplier;

                return false;
            }
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(FirstPersonMover.CreateCharacterModel))]
        private static void CreateCharacterModel_Postfix(FirstPersonMover __instance)
        {
            ModWeaponsManager.Instance.AddWeaponsToRobot(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(FirstPersonMover.SetEquippedWeaponType))]
        private static void SetEquippedWeaponType_Postfix(FirstPersonMover __instance)
        {
            PersonalizationController personalizationController = __instance.GetComponent<PersonalizationController>();
            if (personalizationController) personalizationController.RefreshBowSkinVisibility();
        }
    }
}
