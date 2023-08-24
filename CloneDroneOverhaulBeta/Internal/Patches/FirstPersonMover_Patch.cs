using Bolt;
using CDOverhaul.Gameplay;
using CDOverhaul.Visuals;
using HarmonyLib;
using UnityEngine;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(FirstPersonMover))]
    internal static class FirstPersonMover_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("ExecuteCommand")]
        private static void ExecuteCommand_Prefix(FirstPersonMover __instance, Command command, bool resetState)
        {
            if (!OverhaulMod.IsModInitialized)
                return;

            int instanceId = __instance.GetInstanceID();
            if (!CharacterExpansionContainer.CachedContainers.ContainsKey(instanceId))
                return;

            CharacterExpansionContainer expansionContainer = CharacterExpansionContainer.CachedContainers[instanceId];
            foreach (OverhaulCharacterExpansion b in expansionContainer.Expansions)
            {
                if (b)
                    b.OnPreCommandExecute((FPMoveCommand)command);
            }
        }

        // A fix of the crash
        [HarmonyPrefix]
        [HarmonyPatch("RollbackAsIfKicked")]
        private static bool RollbackAsIfKicked_Prefix(FirstPersonMover __instance)
        {
            return !OverhaulMod.IsModInitialized || __instance.HasCharacterModel();
        }

        // Another attempt to fix invis weapons in multiplayer
        [HarmonyPostfix]
        [HarmonyPatch("OnSwordSwingStarted")]
        private static void OnSwordSwingStarted_Postfix(FirstPersonMover __instance)
        {
            if (__instance._currentWeaponModel)
                __instance._currentWeaponModel.gameObject.SetActive(true);
        }

        [HarmonyPostfix]
        [HarmonyPatch("ExecuteCommand")]
        private static void ExecuteCommand_Postfix(FirstPersonMover __instance, Command command, bool resetState)
        {
            if (!OverhaulMod.IsModInitialized)
                return;

            int instanceId = __instance.GetInstanceID();
            if (!CharacterExpansionContainer.CachedContainers.ContainsKey(instanceId))
                return;

            CharacterExpansionContainer expansionContainer = CharacterExpansionContainer.CachedContainers[instanceId];
            foreach (OverhaulCharacterExpansion b in expansionContainer.Expansions)
            {
                if (b)
                    b.OnPostCommandExecute((FPMoveCommand)command);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("CreateArrowAndDrawBow")]
        private static void CreateArrowAndDrawBow_Postfix(FirstPersonMover __instance)
        {
            if (!OverhaulMod.IsModInitialized)
                return;

            WeaponSkinsWearer w = __instance.GetComponent<WeaponSkinsWearer>();
            if (w == null)
                return;

            WeaponSkinBehaviour s = w.GetSpecialBehaviourInEquippedWeapon<WeaponSkinBehaviour>();
            if (s == null)
                return;

            s.OnBeginDraw();
        }

        [HarmonyPostfix]
        [HarmonyPatch("ReleaseNockedArrow")]
        private static void ReleaseNockedArrow_Postfix(FirstPersonMover __instance)
        {
            if (!OverhaulMod.IsModInitialized)
                return;

            WeaponSkinsWearer w = __instance.GetComponent<WeaponSkinsWearer>();
            if (w == null)
                return;

            WeaponSkinBehaviour s = w.GetSpecialBehaviourInEquippedWeapon<WeaponSkinBehaviour>();
            if (s == null)
                return;

            s.OnEndDraw();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(FirstPersonMover), "OnEvent", new System.Type[] { typeof(SendFallingEvent) })]
        private static void OnEvent_Postfix(FirstPersonMover __instance, SendFallingEvent fallingEvent)
        {
            if (!OverhaulMod.IsModInitialized)
                return;

            int instanceId = __instance.GetInstanceID();
            if (!CharacterExpansionContainer.CachedContainers.ContainsKey(instanceId))
                return;

            CharacterExpansionContainer expansionContainer = CharacterExpansionContainer.CachedContainers[instanceId];
            foreach (OverhaulCharacterExpansion b in expansionContainer.Expansions)
            {
                if (b)
                    b.OnEvent(fallingEvent);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("RefreshUpgrades")]
        private static void RefreshUpgrades_Postfix(FirstPersonMover __instance)
        {
            if (!OverhaulMod.IsModInitialized)
                return;

            int instanceId = __instance.GetInstanceID();
            if (!CharacterExpansionContainer.CachedContainers.ContainsKey(instanceId))
                return;

            CharacterExpansionContainer expansionContainer = CharacterExpansionContainer.CachedContainers[instanceId];
            foreach (OverhaulCharacterExpansion b in expansionContainer.Expansions)
            {
                if (b)
                    b.OnUpgradesRefresh(__instance);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch("ReleaseNockedArrow")]
        private static void ReleaseNockedArrow_Prefix(FirstPersonMover __instance, int serverFrame, ref Vector3 startPosition, Vector3 startFlyDirection, float rotationZ)
        {
            if (!OverhaulMod.IsModInitialized || !ViewModesManager.IsFirstPersonModeEnabled || !__instance.IsMainPlayer())
                return;

            startPosition = __instance.GetCharacterModel().ArrowHolder.position;
        }
    }
}
