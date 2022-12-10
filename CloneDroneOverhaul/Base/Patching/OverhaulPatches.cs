using CloneDroneOverhaul.Gameplay.OverModes;
using CloneDroneOverhaul.LevelEditor;
using CloneDroneOverhaul.Modules;
using CloneDroneOverhaul.Patching.VisualFixes;
using CloneDroneOverhaul.UI;
using CloneDroneOverhaul.Utilities;
using HarmonyLib;
using ModLibrary;
using PicaVoxel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UI;
using CloneDroneOverhaul.UI.Components;

namespace CloneDroneOverhaul.Patching
{
    internal class OverhualSepratedPatchMethods
    {
        public static void PatchSettings(SettingsMenu __instance)
        {
            OverhaulMain.Timer.AddNoArgAction(delegate
            {
                __instance.VsyncOnToggle.transform.GetChild(1).GetComponent<Text>().text = OverhaulMain.GetTranslatedString("Settings_VSyncNotif");
                __instance.VsyncOnToggle.transform.GetChild(0).gameObject.SetActive(false);
                __instance.VsyncOnToggle.interactable = false;
                __instance.VsyncOnToggle.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
                __instance.VsyncOnToggle.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 20);
                if (__instance.GetComponent<SelectableUI>() != null) __instance.GetComponent<SelectableUI>().enabled = false;
                if (__instance.VsyncOnToggle.GetComponent<DoOnMouseActions>() == null) DoOnMouseActions.AddComponent(__instance.VsyncOnToggle.gameObject, delegate
                {
                    SettingsUI.Instance.ShowWithOpenedPage("Graphics", "Settings");
                });
            }, 0.05f, true);
        }
    }

    [HarmonyPatch(typeof(GameUIRoot))]
    public class OverhaulUIPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameUIRoot), "RefreshCursorEnabled")]
        private static bool GameUIRoot_RefreshCursorEnabled_Prefix()
        {
            try
            {
                if (GUIManagement.Instance.GetGUI<UI.NewErrorWindow>().gameObject.activeInHierarchy || GUIManagement.Instance.GetGUI<Localization.OverhaulLocalizationEditor>().gameObject.activeInHierarchy || UI.MultiplayerInviteUIs.Instance.ShallCursorBeActive() ||
                      GUIManagement.Instance.GetGUI<UI.SettingsUI>().gameObject.activeInHierarchy || MultiplayerUIs.Instance.BRMObj.GetObjectFromList<RectTransform>(6).gameObject.activeInHierarchy || NewPhotoModeUI.Instance.ShouldShowCursor())
                {
                    global::InputManager.Instance.SetCursorEnabled(true);
                    return false;
                }
            }
            catch
            {
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TitleScreenUI), "OnWorkshopBrowserButtonClicked")]
        private static bool TitleScreenUI_OnWorkshopBrowserButtonClicked_Prefix(TitleScreenUI __instance)
        {
            Modules.ArenaManager.SetRootAndLogoVisible(false);
            NewWorkshopBrowserUI.Instance.Show();
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TitleScreenUI), "setLogoAndRootButtonsVisible")]
        private static void TitleScreenUI_setLogoAndRootButtonsVisible_Postfix(TitleScreenUI __instance, bool visible)
        {
            __instance.transform.GetChild(1).gameObject.SetActive(visible);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TitleScreenUI), "Hide")]
        private static void TitleScreenUI_Hide_Postfix(TitleScreenUI __instance)
        {
            NewGameModeSelectionScreen.Instance.Hide();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PhotoModeControlsDisplay), "SetVisibility")]
        private static void PhotoModeControlsDisplay_SetVisibility_Postfix(PhotoModeControlsDisplay __instance, bool value)
        {
            if (!OverhaulDescription.IsBetaBuild())
            {
                return;
            }
            __instance.gameObject.SetActive(false);
            if (value)
            {
                UI.NewPhotoModeUI.Instance.Show();
            }
            else
            {
                UI.NewPhotoModeUI.Instance.Hide();
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Multiplayer1v1UI), "refreshUIVisibility")]
        private static void Multiplayer1v1UI_refreshUIVisibility_Postfix(Multiplayer1v1UI __instance)
        {
            __instance.WaitingOnOpponentLabel.gameObject.gameObject.SetActive(false);
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
        [HarmonyPatch(typeof(BattleRoyaleUI), "Show")]
        private static bool BattleRoyaleUI_Show_Postfix(BattleRoyaleUI __instance)
        {
            if (OverhaulMain.GetSetting<bool>("Patches.GUI.Last Bot Standing"))
            {
                MultiplayerUIs.Instance.Show(MultiplayerUIs.MultiplayerUI.BattleRoyale);
                return false;
            }
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BattleRoyaleUI), "Hide")]
        private static void BattleRoyaleUI_Hide_Postfix()
        {
            MultiplayerUIs.Instance.Hide(MultiplayerUIs.MultiplayerUI.BattleRoyale);
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(SelectableUI), "Start")]
        private static void SelectableUI_Start_Postfix(SelectableUI __instance)
        {
            ObjectFixer.FixObject(__instance.transform, "FixSelectableUI", __instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(EscMenu), "Show")]
        private static void EscMenu_Show_Postfix(EscMenu __instance)
        {
            ObjectFixer.FixObject(__instance.transform, "FixEscMenu", __instance);
            BaseStaticReferences.NewEscMenu.Show();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(EscMenu), "Hide")]
        private static void EscMenu_Hide_Postfix(EscMenu __instance)
        {
            GameUIRoot.Instance.AchievementProgressUI.Hide();
            BaseStaticReferences.NewEscMenu.Hide();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SettingsMenu), "Hide")]
        private static void SettingsMenu_Hide_Postfix(SettingsMenu __instance)
        {
            if (BaseStaticValues.IsEscMenuWaitingToShow)
            {
                BaseStaticReferences.NewEscMenu.Show();
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SettingsMenu), "Show")]
        private static void SettingsMenu_Show_Postfix(SettingsMenu __instance)
        {
            OverhualSepratedPatchMethods.PatchSettings(__instance);
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SettingsMenu), "ShowContentsForTab")]
        private static void SettingsMenu_ShowContentsForTab_Postfix(SettingsMenu __instance)
        {
            OverhualSepratedPatchMethods.PatchSettings(__instance);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MultiplayerInviteCodeUI), "ShowWithCode")]
        private static bool MultiplayerInviteCodeUI_ShowWithCode_Prefix(MultiplayerInviteCodeUI __instance, string inviteCode, bool showSettings)
        {
            GUIManagement.Instance.GetGUI<UI.MultiplayerInviteUIs>().ShowWithCode(inviteCode, showSettings);
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LevelEditorUI), "Show")]
        private static bool LevelEditorUI_Show_Prefix()
        {
            if (OverhaulMain.GetSetting<bool>("Levels.Editor.New Level Editor"))
            {
                GUIManagement.Instance.GetGUI<LevelEditor.ModdedLevelEditorUI>().Show();
                return false;
            }
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(AchievementProgressUI), "Hide")]
        private static void AchievementProgressUI_Hide_Postfix(EscMenu __instance)
        {
            if (BaseStaticValues.IsEscMenuWaitingToShow)
            {
                BaseStaticReferences.NewEscMenu.Show();
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameModeSelectScreen), "Show")]
        private static bool GameModeSelectScreen_Show_Prefix(GameModeSelectScreen __instance)
        {
            NewGameModeSelectionScreen.Instance.Show(__instance.GameModeData);
            return false;
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameModeSelectScreen), "Hide")]
        private static void GameModeSelectScreen_Show_Postfix(GameModeSelectScreen __instance)
        {
            NewGameModeSelectionScreen.Instance.Hide();
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameModeSelectScreen), "SetMainScreenVisible")]
        private static bool GameModeSelectScreen_SetMainScreenVisible_Prefix(GameModeSelectScreen __instance, bool visible)
        {
            NewGameModeSelectionScreen.Instance.SetMainScreenVisible(visible);
            return false;
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
    // Token: 0x02000044 RID: 68
    [HarmonyPatch(typeof(ChapterLoadingScreen))]
    public class OverhaulPatches
    {
        // Token: 0x060001FD RID: 509 RVA: 0x0000C790 File Offset: 0x0000A990
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ErrorManager), "sendExceptionDetailsToLoggly")]
        private static bool ErrorManager_sendExceptionDetailsToLoggly_Prefix()
        {
            return false;
        }

        // Token: 0x060001FE RID: 510 RVA: 0x0000C793 File Offset: 0x0000A993
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PhotoManager), "Update")]
        private static bool PhotoManager_Update_Prefix()
        {
            return true;
        }

        // Token: 0x060001FF RID: 511 RVA: 0x0000C796 File Offset: 0x0000A996
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CharacterTracker), "SetPlayer")]
        private static void CharacterTracker_SetPlayer_Prefix(CharacterTracker __instance, Character player)
        {
            BaseStaticReferences.ModuleManager.ExecuteFunction("onPlayerSet", new object[]
            {
                player.GetRobotInfo(),
                __instance.GetPrivateField<Character>("_player").GetRobotInfo()
            });
        }

        // Token: 0x06000200 RID: 512 RVA: 0x0000C7C9 File Offset: 0x0000A9C9
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FirstPersonMover), "OnDestroy")]
        private static void FirstPersonMover_OnDestroy_Postfix(FirstPersonMover __instance)
        {
            BaseStaticReferences.ModuleManager.ExecuteFunction("firstPersonMover.OnDestroy", new object[]
            {
                __instance.GetRobotInfo()
            });
        }

        // Token: 0x06000201 RID: 513 RVA: 0x0000C7E9 File Offset: 0x0000A9E9
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ArmorPiece), "Initialize")]
        private static void ArmorPiece_Initialize_Postfix(ArmorPiece __instance)
        {
            ObjectFixer.FixObject(__instance.transform, "FixArmorPiece", __instance);
        }

        // Token: 0x06000202 RID: 514 RVA: 0x0000C7FC File Offset: 0x0000A9FC
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SettingsManager), "ShouldHideGameUI")]
        private static void SettingsManager_ShouldHideGameUI_Postfix(ref bool __result)
        {
            if (MiscEffectsManager.IsUIHidden)
            {
                __result = true;
            }
        }

        // Token: 0x06000203 RID: 515 RVA: 0x0000C808 File Offset: 0x0000AA08
        [HarmonyPrefix]
        [HarmonyPatch(typeof(KillFeedUI), "onKillEventReceived")]
        private static bool KillFeedUI_onKillEventReceived_Prefix(MultiplayerKillEvent killEvent)
        {
            bool isUIHidden = MiscEffectsManager.IsUIHidden;
            return false;
        }

        // Token: 0x06000204 RID: 516 RVA: 0x0000C811 File Offset: 0x0000AA11
        [HarmonyPrefix]
        [HarmonyPatch(typeof(AttackManager), "CreateSwordBlockVFX")]
        private static bool AttackManager_CreateSwordBlockVFX_Prefix(Vector3 position)
        {
            OverhaulMain.Visuals.EmitSwordBlockVFX(position, false);
            return false;
        }

        // Token: 0x06000205 RID: 517 RVA: 0x0000C820 File Offset: 0x0000AA20
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ArrowProjectile), "PlayGroundImpactVFX")]
        private static bool ArrowProjectile_PlayGroundImpactVFX_Prefix(ArrowProjectile __instance)
        {
            if (__instance.GetPrivateField<bool>("_isFlaming"))
            {
                Singleton<GlobalFireParticleSystem>.Instance.CreateGroundImpactVFX(__instance.transform.position);
            }
            else
            {
                OverhaulMain.Visuals.EmitArrowCollision(__instance.transform.position);
            }
            return false;
        }

        // Token: 0x06000206 RID: 518 RVA: 0x0000C85D File Offset: 0x0000AA5D
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AttackManager), "CreateHammerHitEffectVFX")]
        private static void AttackManager_CreateHammerHitEffectVFX_Postfix(Vector3 position)
        {
            OverhaulMain.Visuals.EmitHammerHitVFX(position);
        }

        // Token: 0x06000207 RID: 519 RVA: 0x0000C86A File Offset: 0x0000AA6A
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AttackManager), "CreateKickHitVFX")]
        private static void AttackManager_CreateKickHitVFX_Postfix(Vector3 position)
        {
            OverhaulMain.Visuals.EmitKickVFX(position);
        }

        // Token: 0x06000208 RID: 520 RVA: 0x0000C877 File Offset: 0x0000AA77
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AttackManager), "CreateRocketJumpVFX")]
        private static void AttackManager_CreateRocketJumpVFX_Postfix(Vector3 position)
        {
            OverhaulMain.Visuals.EmitDashVFX(position, true, true);
        }

        // Token: 0x06000209 RID: 521 RVA: 0x0000C886 File Offset: 0x0000AA86
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MultiplayerPlayerInfoManager), "AddPlayerInfoState")]
        private static void MultiplayerPlayerInfoManager_AddPlayerInfoState_Postfix(MultiplayerPlayerInfoState multiplayerPlayerInfoState)
        {
            BaseStaticReferences.ModuleManager.ExecuteFunction("onPlayerJoined", new object[]
            {
                multiplayerPlayerInfoState
            });
        }

        // Token: 0x0600020A RID: 522 RVA: 0x0000C8A1 File Offset: 0x0000AAA1
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GlobalFireParticleSystem), "CreateGroundImpactVFX")]
        public static void GlobalFireParticleSystem_CreateGroundImpactVFX_Postfix(Vector3 positon)
        {
            OverhaulMain.Visuals.EmitSwordBlockVFX(positon, true);
        }

        // Token: 0x0600020B RID: 523 RVA: 0x0000C8AF File Offset: 0x0000AAAF
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MindSpaceBodyPart), "tryExplodeBodyPart")]
        public static void MindSpaceBodyPart_tryExplodeBodyPart_Postfix(MindSpaceBodyPart __instance, ref bool __result)
        {
            if (__result)
            {
                BodyPartPatcher.OnBodyPartDamaged(__instance);
            }
        }

        // Token: 0x0600020C RID: 524 RVA: 0x0000C8BC File Offset: 0x0000AABC
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ExplodeWhenCut), "onBodyPartDamaged")]
        public static void ExplodeWhenCut_onBodyPartDamaged_Prefix(ExplodeWhenCut __instance)
        {
            if (__instance != null && !__instance.GetPrivateField<bool>("_hasExploded") && __instance.ExplosionSpawnPoint != null)
            {
                OverhaulMain.Visuals.EmitExplosion(__instance.ExplosionSpawnPoint.position);
                Vector3 position = __instance.transform.position;
                Vector3 b = default(Vector3);
                RobotShortInformation robotInfo = Singleton<CharacterTracker>.Instance.GetPlayer().GetRobotInfo();
                if (!robotInfo.IsNull)
                {
                    b = robotInfo.Instance.transform.position;
                    if (Vector3.Distance(position, b) < 40f)
                    {
                        Singleton<PlayerCameraManager>.Instance.ShakeCamera(0.2f, 0.8f);
                    }
                }
            }
        }

        // Token: 0x0600020D RID: 525 RVA: 0x0000C967 File Offset: 0x0000AB67
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MechBodyPart), "destroyVoxelAtPositionFromCut")]
        public static void MechBodyPart_destroyVoxelAtPositionFromCut_Postfix(MechBodyPart __instance, PicaVoxelPoint picaVoxelPoint, Voxel? voxelAtPosition, Vector3 localPosition, Vector3 volumeWorldCenter, Vector3 impactDirectionWorld, FireSpreadDefinition fireSpreadDefinition, Frame currentFrame)
        {
            BodyPartPatcher.OnVoxelCut(__instance, picaVoxelPoint, voxelAtPosition, localPosition, volumeWorldCenter, impactDirectionWorld, fireSpreadDefinition, currentFrame);
        }

        // Token: 0x0600020E RID: 526 RVA: 0x0000C97C File Offset: 0x0000AB7C
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MechBodyPart), "tryBurnColorAt")]
        public static void MechBodyPart_tryBurnColorAt_Postfix(MechBodyPart __instance, Frame currentFrame, PicaVoxelPoint voxelPosition, int offsetX, int offsetY, int offsetZ, float colorMultiplier = -1f)
        {
            BodyPartPatcher.CanCalculateVoxelWorldPositionNextTime = !BodyPartPatcher.CanCalculateVoxelWorldPositionNextTime;
            if (!BodyPartPatcher.CanCalculateVoxelWorldPositionNextTime)
            {
                return;
            }
            if (UnityEngine.Random.Range(1, 10) > 5)
            {
                Vector3 voxelWorldPosition = currentFrame.GetVoxelWorldPosition(voxelPosition);
                OverhaulMain.Visuals.EmitBurningVFX(voxelWorldPosition);
            }
        }

        // Token: 0x0600020F RID: 527 RVA: 0x0000C9BC File Offset: 0x0000ABBC
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

        // Token: 0x06000210 RID: 528 RVA: 0x0000CA8E File Offset: 0x0000AC8E
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PS4BodyCubeMaterialFix), "Awake")]
        public static void PS4BodyCubeMaterialFix_Awake_Postfix(PS4BodyCubeMaterialFix __instance)
        {
            ObjectFixer.FixObject(__instance.transform, "PS4Cube", __instance);
        }

        // Token: 0x06000211 RID: 529 RVA: 0x0000CAA1 File Offset: 0x0000ACA1
        [HarmonyPostfix]
        [HarmonyPatch(typeof(DirectionalLightManager), "RefreshDirectionalLight")]
        private static void DirectionalLightManager_RefreshDirectionalLight_Postfix(DirectionalLightManager __instance)
        {
            __instance.DirectionalLight.shadowNormalBias = 1.1f;
            __instance.DirectionalLight.shadowBias = 1f;
        }

        // Token: 0x06000212 RID: 530 RVA: 0x0000CAC3 File Offset: 0x0000ACC3
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ArenaCustomizationManager), "LerpUpgradeRoomMaterialTo")]
        private static void ArenaCustomizationManager_LerpUpgradeRoomMaterialTo_Prefix(ref Color targetColor)
        {
            targetColor *= 1.5f;
        }

        // Token: 0x06000213 RID: 531 RVA: 0x0000CADB File Offset: 0x0000ACDB
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LevelEditorObjectPlacementManager), "Select")]
        private static bool LevelEditorObjectPlacementManager_Select_Prefix(ObjectPlacedInLevel objectToSelect, bool deselectAllAnimationTracks = true)
        {
            return !GUIManagement.Instance.GetGUI<NewEscMenu>().gameObject.activeInHierarchy;
        }

        // Token: 0x06000214 RID: 532 RVA: 0x0000CAF6 File Offset: 0x0000ACF6
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LevelEditorObjectPlacementManager), "Select")]
        private static void LevelEditorObjectPlacementManager_Select_Postfix(LevelEditorObjectPlacementManager __instance, ObjectPlacedInLevel objectToSelect, bool deselectAllAnimationTracks = true)
        {
            CrossModManager.DoAction("ModdedLevelEditor.RefreshSelected", new object[]
            {
                __instance
            });
        }

        // Token: 0x06000215 RID: 533 RVA: 0x0000CB0C File Offset: 0x0000AD0C
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LevelEditorObjectPlacementManager), "Deselect")]
        private static void LevelEditorObjectPlacementManager_Deselect_Postfix(LevelEditorObjectPlacementManager __instance, ObjectPlacedInLevel objectToDeselect)
        {
            CrossModManager.DoAction("ModdedLevelEditor.RefreshSelected", new object[]
            {
                __instance
            });
        }

        // Token: 0x06000216 RID: 534 RVA: 0x0000CB22 File Offset: 0x0000AD22
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LevelEditorObjectPlacementManager), "DeselectEverything")]
        private static void LevelEditorObjectPlacementManager_DeselectEverything_Postfix(LevelEditorObjectPlacementManager __instance)
        {
            CrossModManager.DoAction("ModdedLevelEditor.RefreshSelected", new object[]
            {
                __instance
            });
        }

        // Token: 0x06000217 RID: 535 RVA: 0x0000CB38 File Offset: 0x0000AD38
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LevelEditorToolManager), "SetActiveTool")]
        private static void LevelEditorToolManager_SetActiveTool_Postfix(LevelEditorObjectPlacementManager __instance)
        {
            GUIManagement.Instance.GetGUI<ModdedLevelEditorUI>().ToolBar.RefreshSelected();
        }

        // Token: 0x06000218 RID: 536 RVA: 0x0000CB50 File Offset: 0x0000AD50
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LevelEditorPointLight), "Start")]
        private static void LevelEditorPointLight_Start_Postfix(LevelEditorPointLight __instance)
        {
            if (!OverhaulMain.Visuals.IsDustEnabled())
            {
                return;
            }
            PointLightDust pointLightDust = __instance.gameObject.AddComponent<PointLightDust>();
            pointLightDust.Target = __instance.transform;
        }

        // Token: 0x06000219 RID: 537 RVA: 0x0000CB82 File Offset: 0x0000AD82
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LevelEditorPerformanceStatsPanel), "Initialize")]
        public static void LevelEditorPerformanceStatsPanel_Initialize_Postfix(LevelEditorPerformanceStatsPanel __instance)
        {
            ObjectFixer.FixObject(__instance.transform, "FixPerformanceStats", __instance);
        }

        // Token: 0x0600021A RID: 538 RVA: 0x0000CB95 File Offset: 0x0000AD95
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SceneTransitionManager), "DisconnectAndExitToMainMenu")]
        public static void SceneTransitionManager_DisconnectAndExitToMainMenu_Postfix(SceneTransitionManager __instance)
        {
            OverhaulMain.Modules.ExecuteFunction("onBoltShutdown", null);
        }

        // Token: 0x0600021B RID: 539 RVA: 0x0000CBA8 File Offset: 0x0000ADA8
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LocalizationManager), "GetTranslatedString", new Type[]
        {
            typeof(string),
            typeof(int)
        })]
        public static bool LocalizationManager_GetTranslatedString_Prefix(ref string __result, string stringID, int maxCharactersBeforeJapaneseLineBreak = -1)
        {
            if (stringID == "Thanks for playing!")
            {
                string translatedString = OverhaulMain.GetTranslatedString("Credits_ThankForUsingTheMod");
                __result = translatedString;
                return false;
            }
            return true;
        }

        // Token: 0x0600021C RID: 540 RVA: 0x0000CBD4 File Offset: 0x0000ADD4
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GarbageTarget), "Start")]
        public static void GarbageTarget_Start_Postfix(GarbageTarget __instance)
        {
            Rigidbody component = __instance.GetComponent<Rigidbody>();
            if (component != null)
            {
                component.interpolation = RigidbodyInterpolation.Interpolate;
            }
        }

        // Token: 0x0600021D RID: 541 RVA: 0x0000CBF8 File Offset: 0x0000ADF8
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LevelEditorObjectPlacementManager), "PlaceObjectInLevelRoot")]
        private static bool LevelEditorObjectPlacementManager_PlaceObjectInLevelRoot_Prefix(LevelObjectEntry objectPlacedLevelObjectEntry, Transform levelRoot, ref ObjectPlacedInLevel __result)
        {
            __result = ModdedLevelEditorManager.Instance.PlaceObject(objectPlacedLevelObjectEntry, levelRoot);
            return false;
        }

        // Token: 0x0600021E RID: 542 RVA: 0x0000CC09 File Offset: 0x0000AE09
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetCollider), "OnEnable")]
        private static void PlanetCollider_OnEnable_Prefix(PlanetCollider __instance)
        {
            ObjectFixer.FixObject(__instance.transform, "Planet_Earth", __instance);
        }

        // Token: 0x0600021F RID: 543 RVA: 0x0000CC1C File Offset: 0x0000AE1C
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ArenaLiftManager), "Update")]
        private static bool ArenaLiftManager_Update_Prefix(ArenaLiftManager __instance)
        {
            GameMode currentGameMode = Singleton<GameFlowManager>.Instance.GetCurrentGameMode();
            if (currentGameMode == EndlessModeOverhaul.Instance.GetGamemode())
            {
                if (!__instance.Lift.IsMoving() && __instance.AreAllPlayersInTheLift() && (__instance.GetLiftTarget() == ArenaLiftPosition.StartArea || __instance.GetLiftTarget() == ArenaLiftPosition.UpgradeRoom))
                {
                    Singleton<GameFlowManager>.Instance.OnAllPlayersInLiftThatsNotMoving();
                }
                if (!__instance.Lift.IsMoving() && __instance.GetLiftTarget() == ArenaLiftPosition.Arena && __instance.AreAllPlayersInTheLift() && Singleton<GameFlowManager>.Instance.HasWonRound())
                {
                    __instance.Lift.GoToUpgradeRoom();
                }
                return false;
            }
            return true;
        }

        // Token: 0x06000220 RID: 544 RVA: 0x0000CCA9 File Offset: 0x0000AEA9
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LevelManager), "ShouldCelebrateArenaVictoryForCurrentLevel")]
        private static void LevelManager_ShouldCelebrateArenaVictoryForCurrentLevel_Postfix(LevelManager __instance, ref bool __result)
        {
            if (OverModesController.CurrentGamemodeIsOvermode())
            {
                __result = false;
            }
        }

        // Token: 0x06000221 RID: 545 RVA: 0x0000CCB5 File Offset: 0x0000AEB5
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LevelManager), "getLevelDescriptions")]
        private static bool LevelManager_getLevelDescriptions_Prefix(LevelManager __instance, ref List<LevelDescription> __result)
        {
            if (OverModesController.CurrentGamemodeIsOvermode())
            {
                __result = OverModesController.GetCurrentOvermode().GetLevelDescriptions();
                return false;
            }
            return true;
        }

        // Token: 0x06000222 RID: 546 RVA: 0x0000CCCD File Offset: 0x0000AECD
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameFlowManager), "RefreshIsLevelWon")]
        private static bool GameFlowManager_RefreshIsLevelWon_Prefix()
        {
            return !OverModesController.CurrentGamemodeIsOvermode();
        }

        // Token: 0x06000223 RID: 547 RVA: 0x0000CCD9 File Offset: 0x0000AED9
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameFlowManager), "ResetGameAndSpawnHuman")]
        private static bool GameFlowManager_ResetGameAndSpawnHuman_Prefix()
        {
            if (OverModesController.CurrentGamemodeIsOvermode())
            {
                OverModeBase.EventNames eventName = OverModeBase.EventNames.SpawnLevel;
                object[] array = new object[3];
                array[0] = true;
                OverModesController.ProcessEventAndReturn<IEnumerator>(eventName, array);
                return false;
            }
            return true;
        }

        // Token: 0x06000224 RID: 548 RVA: 0x0000CCFB File Offset: 0x0000AEFB
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameDataManager), "getCurrentGameData")]
        private static bool GameDataManager_getCurrentGameData_Prefix(GameDataManager __instance, ref GameData __result)
        {
            if (OverModesController.CurrentGamemodeIsOvermode())
            {
                __result = OverModesController.GetCurrentOvermode().GetLegacyGameData();
                return false;
            }
            return true;
        }

        // Token: 0x06000225 RID: 549 RVA: 0x0000CD13 File Offset: 0x0000AF13
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ObjectPlacedInLevel), "Initialize")]
        private static void ObjectPlacedInLevel_Initialize_Postfix(ObjectPlacedInLevel __instance)
        {
            ObjectFixer.FixObject(__instance.transform, "objectPlacedInLevel", __instance);
        }

        // Token: 0x06000226 RID: 550 RVA: 0x0000CD26 File Offset: 0x0000AF26
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LevelManager), "SpawnCurrentLevel")]
        private static bool LevelManager_SpawnCurrentLevel_Prefix(LevelManager __instance, ref IEnumerator __result, bool isAsync = false, string overrideLevelID = null, Action completeCallback = null)
        {
            if (OverModesController.CurrentGamemodeIsOvermode())
            {
                __result = OverModesController.ProcessEventAndReturn<IEnumerator>(OverModeBase.EventNames.SpawnLevel, new object[]
                {
                    isAsync,
                    overrideLevelID,
                    completeCallback
                });
                return false;
            }
            return true;
        }

        // Token: 0x06000227 RID: 551 RVA: 0x0000CD54 File Offset: 0x0000AF54
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(CameraShaker), "Update")]
        private static IEnumerable<CodeInstruction> CameraShaker_Update_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> list = new List<CodeInstruction>(instructions);
            for (int i = 56; i < 72; i++)
            {
                list[i].opcode = OpCodes.Nop;
            }
            for (int j = 77; j < 81; j++)
            {
                list[j].opcode = OpCodes.Nop;
            }
            return list.AsEnumerable<CodeInstruction>();
        }

        // Token: 0x06000228 RID: 552 RVA: 0x0000CDAC File Offset: 0x0000AFAC
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(PhotoManager), "Update")]
        private static IEnumerable<CodeInstruction> PhotoManager_Update_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> list = new List<CodeInstruction>(instructions);
            if (!OverhaulDescription.IsBetaBuild())
            {
                return list.AsEnumerable<CodeInstruction>();
            }
            for (int i = 66; i < 95; i++)
            {
                list[i].opcode = OpCodes.Nop;
            }
            return list.AsEnumerable<CodeInstruction>();
        }
    }



    public class BodyPartPatcher
    {
        public static bool CanCalculateVoxelWorldPositionNextTime = true;
        public static void OnBodyPartStart(Frame frame)
        {
            Voxel[] voxels = frame.Voxels;
            ReplaceVoxelColor replaceVoxels = frame.GetComponentInParent<ReplaceVoxelColor>();
            for (int i = 0; i < voxels.Length; i++)
            {
                Color normalCol = voxels[i].Color;

                float num = UnityEngine.Random.Range(0.95f, 1.00f);
                bool shouldUpdateColor = false;
                if (replaceVoxels == null)
                {
                    shouldUpdateColor = voxels[i].Color.a > 253;
                }

                if (replaceVoxels != null && replaceVoxels.Old != normalCol)
                {
                    shouldUpdateColor = true;
                }
                shouldUpdateColor = true;

                if (shouldUpdateColor)
                {
                    Color32 color = new Color32((byte)Mathf.RoundToInt(voxels[i].Color.r * num), (byte)Mathf.RoundToInt(voxels[i].Color.g * num),
                       (byte)Mathf.RoundToInt(voxels[i].Color.b * num), voxels[i].Color.a);
                    voxels[i].Color = color;
                }
            }
            frame.UpdateAllChunks();
        }

        public static void OnVoxelCut(MechBodyPart __instance, PicaVoxelPoint picaVoxelPoint, Voxel? voxelAtPosition, Vector3 localPosition, Vector3 volumeWorldCenter, Vector3 impactDirectionWorld, FireSpreadDefinition fireSpreadDefinition, Frame currentFrame)
        {
            try
            {
                CanCalculateVoxelWorldPositionNextTime = !CanCalculateVoxelWorldPositionNextTime;
                if (!CanCalculateVoxelWorldPositionNextTime)
                {
                    return;
                }
                Vector3 voxelWorldPosition = currentFrame.GetVoxelWorldPosition(picaVoxelPoint);
                if (fireSpreadDefinition == null)
                {
                    Vector3 a = (voxelWorldPosition - volumeWorldCenter).normalized + impactDirectionWorld;
                }

                OverhaulMain.Visuals.EmitBodyPartCutVFX(voxelWorldPosition, fireSpreadDefinition != null);
            }
            catch (System.Exception ex)
            {
                CloneDroneOverhaul.UI.Notifications.Notification notif = new UI.Notifications.Notification();
                notif.SetUp("Nullpoint : BodyPartPatcher - OnVoxelCut", ex.Message, 5, new UnityEngine.Vector2(400, 700), new UnityEngine.Color(1f, 0.1559941f, 0.1792453f, 0.6f), new UI.Notifications.Notification.NotificationButton[] { });
            }
        }

        public static void OnBodyPartDamaged(BaseBodyPart part)
        {
            if (part is MindSpaceBodyPart)
            {
                OverhaulMain.Visuals.EmitMSBodyPartDamage(part.transform.position);
            }
        }

        public static void AddMechBodyPartComponent(GameObject obj)
        {
            MechBodyPart part = obj.AddComponent<MechBodyPart>();
            throw new System.NotImplementedException();
        }
    }
}
