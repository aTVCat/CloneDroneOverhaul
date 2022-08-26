using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using UnityEngine;
using PicaVoxel;
using HarmonyLib;
using ModLibrary;
using CloneDroneOverhaul.Patching.VisualFixes;

namespace CloneDroneOverhaul.Patching
{
    [HarmonyPatch(typeof(ChapterLoadingScreen))]
    public class OverhaulPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameUIRoot), "RefreshCursorEnabled")]
        private static void GameUIRoot_RefreshCursorEnabled_Postfix()
        {
            try
            {
                if (OverhaulMain.GUI.GetGUI<UI.NewErrorWindow>().gameObject.activeInHierarchy || OverhaulMain.GUI.GetGUI<Localization.OverhaulLocalizationEditor>().gameObject.activeInHierarchy)
                    global::InputManager.Instance.SetCursorEnabled(true);
            }
            catch
            {
                Debug.Log("we have nullpoint");
            }
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ErrorWindow), "Show")]
        private static void ErrorWindow_Show_Postfix(ErrorWindow __instance, string errorMessage)
        {
            __instance.gameObject.SetActive(false);
            BaseStaticReferences.ModuleManager.GetModule<UI.GUIManagement>().GetGUI<UI.NewErrorWindow>().Show(errorMessage);
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ErrorWindow), "Hide")]
        private static void ErrorWindow_Hide_Postfix()
        {
            BaseStaticReferences.ModuleManager.GetModule<UI.GUIManagement>().GetGUI<UI.NewErrorWindow>().Hide();
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ErrorManager), "sendExceptionDetailsToLoggly")]
        private static bool ErrorManager_sendExceptionDetailsToLoggly_Prefix()
        {
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ArmorPiece), "Initialize")]
        private static void ArmorPiece_Initialize_Postfix(ArmorPiece __instance)
        {
            ObjectFixer.FixObject(__instance.transform, "FixArmorPiece", __instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SelectableUI), "Start")]
        private static void SelectableUI_Start_Postfix(SelectableUI __instance)
        {
            ObjectFixer.FixObject(__instance.transform, "FixSelectableUI", __instance);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(AttackManager), "CreateSwordBlockVFX")]
        private static bool AttackManager_CreateSwordBlockVFX_Prefix(Vector3 position)
        {
            OverhaulMain.Visuals.EmitSwordBlockVFX(position);
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GlobalFireParticleSystem), "CreateGroundImpactVFX")]
        public static void GlobalFireParticleSystem_CreateGroundImpactVFX_Postfix(Vector3 positon)
        {
            OverhaulMain.Visuals.EmitSwordBlockVFX(positon, true);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MindSpaceBodyPart), "tryExplodeBodyPart")]
        public static void MindSpaceBodyPart_tryExplodeBodyPart_Postfix(MindSpaceBodyPart __instance, ref bool __result)
        {
            if(__result)
            BodyPartPatcher.OnBodyPartDamaged(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MechBodyPart), "Start")]
        public static void MechBodyPart_Start_Postfix(MechBodyPart __instance)
        {
            BodyPartPatcher.OnBodyPartStart(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MechBodyPart), "destroyVoxelAtPositionFromCut")]
        public static void MechBodyPart_destroyVoxelAtPositionFromCut_Postfix(MechBodyPart __instance, PicaVoxelPoint picaVoxelPoint, Voxel? voxelAtPosition, Vector3 localPosition, Vector3 volumeWorldCenter, Vector3 impactDirectionWorld, FireSpreadDefinition fireSpreadDefinition, Frame currentFrame)
        {
            BodyPartPatcher.OnVoxelCut(__instance, picaVoxelPoint, voxelAtPosition, localPosition, volumeWorldCenter, impactDirectionWorld, fireSpreadDefinition, currentFrame);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MechBodyPart), "tryBurnColorAt")]
        public static void MechBodyPart_tryBurnColorAt_Postfix(MechBodyPart __instance, Frame currentFrame, PicaVoxelPoint voxelPosition, int offsetX, int offsetY, int offsetZ, float colorMultiplier = -1f)
        {
            if(UnityEngine.Random.Range(1, 5) > 3) OverhaulMain.Visuals.EmitBurningVFX(currentFrame.GetVoxelWorldPosition(voxelPosition));
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MindSpaceBodyPart), "Awake")]
        public static void MindSpaceBodyPart_Awake_Postfix(MindSpaceBodyPart __instance)
        {
            Material privateField = __instance.GetPrivateField<Material>("_originalMaterial");
            bool flag = privateField.name == "MindSpaceSwordHandle";
            if (flag)
            {
                privateField.color = new Color(7f, 3f, 1f, 2.2f);
            }
            else
            {
                bool flag2 = privateField.name == "MindSpaceSwordBlade";
                if (flag2)
                {
                    privateField.color = new Color(3f, 0.21f, 0f, 1f);
                }
                else
                {
                    privateField.color = new Color(0.34f, 0.5f, 1.1f, 0.85f);
                }
            }
            __instance.TakingDamageMaterial.color = new Color(0.9f, 0f, 0f, 0.8f);
            __instance.SetPrivateField("_originalMaterial", privateField);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PS4BodyCubeMaterialFix), "Awake")]
        public static void PS4BodyCubeMaterialFix_Awake_Postfix(PS4BodyCubeMaterialFix __instance)
        {
            MeshRenderer component = __instance.GetComponent<MeshRenderer>();
            bool flag = component == null;
            if (!flag)
            {
                Material material = component.material;
                bool flag2 = material == null;
                if (!flag2)
                {
                    bool flag3 = material.name == "MindSpaceMaterial_EmperorCrown (Instance)";
                    if (flag3)
                    {
                        material.color = new Color(2f, 1.5f, 0.35f, 0.98f);
                    }
                    else
                    {
                        bool flag4 = material.name == "MindSpaceMaterial_EmperorFace (Instance)";
                        if (flag4)
                        {
                            material.color = new Color(0.5f, 0.76f, 1.8f, 0.89f);
                        }
                    }
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(DirectionalLightManager), "RefreshDirectionalLight")]
        private static void DirectionalLightManager_RefreshDirectionalLight_Postfix(DirectionalLightManager __instance)
        {
            __instance.DirectionalLight.shadowNormalBias = 1.1f;
            __instance.DirectionalLight.shadowBias = 1f;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ArenaCustomizationManager), "LerpUpgradeRoomMaterialTo")]
        private static void ArenaCustomizationManager_LerpUpgradeRoomMaterialTo_Prefix(ref Color targetColor)
        {
            targetColor *= 1.5f;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(MechBodyPart), "destroyVoxelAtPositionFromCut")]
        private static IEnumerable<CodeInstruction> MechBodyPart_destroyVoxelAtPositionFromCut_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> list = new List<CodeInstruction>(instructions);
            for (int i = 36; i < 66; i++)
            {
                list[i].opcode = OpCodes.Nop;
            }
            return list.AsEnumerable<CodeInstruction>();
        }
    }
}
