using CloneDroneOverhaul.Patching.VisualFixes;
using HarmonyLib;
using ModLibrary;
using PicaVoxel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UI;
using CloneDroneOverhaul.Utilities;

namespace CloneDroneOverhaul.Patching
{
    [HarmonyPatch(typeof(GameUIRoot))]
    public class OverhaulUIPatches
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

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SelectableUI), "Start")]
        private static void SelectableUI_Start_Postfix(SelectableUI __instance)
        {
            ObjectFixer.FixObject(__instance.transform, "FixSelectableUI", __instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerStatusPanel1v1), "populateValues")]
        public static void PlayerStatusPanel1v1_populateValues_Postfix(PlayerStatusPanel1v1 __instance)
        {
            List<MultiplayerPlayerInfoState> allPlayerInfoStates = Singleton<MultiplayerPlayerInfoManager>.Instance.GetAllPlayerInfoStates();
            bool flag = allPlayerInfoStates != null && allPlayerInfoStates.Count == 2;
            if (flag)
            {
                Sprite imageSprite = Singleton<MultiplayerCharacterCustomizationManager>.Instance.CharacterModels[allPlayerInfoStates[1].state.CharacterModelIndex].ImageSprite;
                Sprite imageSprite2 = Singleton<MultiplayerCharacterCustomizationManager>.Instance.CharacterModels[allPlayerInfoStates[0].state.CharacterModelIndex].ImageSprite;
                Image component = __instance.transform.GetChild(3).GetComponent<Image>();
                Image component2 = __instance.transform.GetChild(2).GetComponent<Image>();
                component.sprite = imageSprite;
                component2.sprite = imageSprite2;
            }
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(CloneUI), "recreateCloneIcons")]
        private static IEnumerable<CodeInstruction> CloneUI_recreateCloneIcons_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> list = new List<CodeInstruction>(instructions);
            for (int i = 3; i < 27; i++)
            {
                list[i].opcode = OpCodes.Nop;
            }
            return list.AsEnumerable<CodeInstruction>();
        }
    }

    [HarmonyPatch(typeof(ChapterLoadingScreen))]
    public class OverhaulPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ErrorManager), "sendExceptionDetailsToLoggly")]
        private static bool ErrorManager_sendExceptionDetailsToLoggly_Prefix()
        {
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CharacterTracker), "SetPlayer")]
        private static void CharacterTracker_SetPlayer_Prefix(CharacterTracker __instance, Character player)
        {
            BaseStaticReferences.ModuleManager.ExecuteFunction("onPlayerSet", new object[] { player.GetRobotInfo(), __instance.GetPrivateField<Character>("_player").GetRobotInfo() });
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ArmorPiece), "Initialize")]
        private static void ArmorPiece_Initialize_Postfix(ArmorPiece __instance)
        {
            ObjectFixer.FixObject(__instance.transform, "FixArmorPiece", __instance);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(AttackManager), "CreateSwordBlockVFX")]
        private static bool AttackManager_CreateSwordBlockVFX_Prefix(Vector3 position)
        {
            OverhaulMain.Visuals.EmitSwordBlockVFX(position);
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MultiplayerPlayerInfoManager), "AddPlayerInfoState")]
        private static void MultiplayerPlayerInfoManager_AddPlayerInfoState_Postfix(MultiplayerPlayerInfoState multiplayerPlayerInfoState)
        {
            BaseStaticReferences.ModuleManager.ExecuteFunction("onPlayerJoined", new object[] { multiplayerPlayerInfoState });
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
            if (__result)
                BodyPartPatcher.OnBodyPartDamaged(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MechBodyPart), "Start")]
        public static void MechBodyPart_Start_Postfix(MechBodyPart __instance)
        {
            BodyPartPatcher.OnBodyPartStart(__instance);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ExplodeWhenCut), "onBodyPartDamaged")]
        public static void ExplodeWhenCut_onBodyPartDamaged_Prefix(ExplodeWhenCut __instance)
        {
            if (!__instance.GetPrivateField<bool>("_hasExploded"))
            {
                OverhaulMain.Visuals.EmitExplosion(__instance.ExplosionSpawnPoint.position);
            }
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
            if (UnityEngine.Random.Range(1, 10) > 5) OverhaulMain.Visuals.EmitBurningVFX(currentFrame.GetVoxelWorldPosition(voxelPosition));
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

        [HarmonyPostfix]
        [HarmonyPatch(typeof(LevelEditorPointLight), "Start")]
        private static void LevelEditorPointLight_Start_Postfix(LevelEditorPointLight __instance)
        {
            CloneDroneOverhaul.Modules.PointLightDust dust = __instance.gameObject.AddComponent<CloneDroneOverhaul.Modules.PointLightDust>();
            dust.Target = __instance.transform;
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(AchievementProgressUI), "populateUI")]
        private static void AchievementProgressUI_populateUI_Postfix(AchievementProgressUI __instance)
        {
            float fractionOfAchievementsCompleted = GameplayAchievementManager.Instance.GetFractionOfAchievementsCompleted();
            GameplayAchievement[] achs = GameplayAchievementManager.Instance.GetAllAchievements();
            int completed = 0;
            foreach (GameplayAchievement ach in achs)
            {
                if (ach.IsComplete())
                {
                    completed++;
                }
            }
            (__instance.ProgressPercentageLabel.transform as RectTransform).sizeDelta = new Vector2(300, 50);
            __instance.ProgressPercentageLabel.text = Mathf.FloorToInt(fractionOfAchievementsCompleted * 100f) + "% [" + completed + "/" + achs.Length + "]";
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(LevelEditorPerformanceStatsPanel), "Initialize")]
        public static void LevelEditorPerformanceStatsPanel_Initialize_Postfix(LevelEditorPerformanceStatsPanel __instance)
        {
            ObjectFixer.FixObject(__instance.transform, "FixPerformanceStats", __instance);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LocalizationManager), "GetTranslatedString", new System.Type[] { typeof(string), typeof(int) })]
        public static bool LocalizationManager_GetTranslatedString_Prefix(ref string __result, string stringID, int maxCharactersBeforeJapaneseLineBreak = -1)
        {
            if(stringID == "Thanks for playing!")
            {
                string str = OverhaulMain.GetTranslatedString("Credits_ThankForUsingTheMod");
                __result = str;
                return false;
            }
            return true;
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
