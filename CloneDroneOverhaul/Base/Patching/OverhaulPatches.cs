using CloneDroneOverhaul.LevelEditor;
using CloneDroneOverhaul.Modules;
using CloneDroneOverhaul.Patching.VisualFixes;
using CloneDroneOverhaul.UI;
using CloneDroneOverhaul.UI.Components;
using CloneDroneOverhaul.Utilities;
using CloneDroneOverhaul.V3Tests.Gameplay;
using CloneDroneOverhaul.V3Tests.HUD;
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

namespace CloneDroneOverhaul.Patching
{
    internal class OverhaulSepratedPatchMethods
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
                if (__instance.GetComponent<SelectableUI>() != null)
                {
                    __instance.GetComponent<SelectableUI>().enabled = false;
                }

                if (__instance.VsyncOnToggle.GetComponent<DoOnMouseActions>() == null)
                {
                    DoOnMouseActions.AddComponent(__instance.VsyncOnToggle.gameObject, delegate
                {
                    UISettings.GetInstance<UISettings>().ShowWithOpenedPage("Graphics", "Settings");
                });
                }
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
                if (UICrashScreen.GetInstance<UICrashScreen>().gameObject.activeInHierarchy || GUIManagement.Instance.GetGUI<Localization.OverhaulLocalizationEditor>().gameObject.activeInHierarchy || UIInviteToLobby.GetInstance<UIInviteToLobby>().ShallCursorBeActive() ||
                       UISettings.GetInstance<UISettings>().gameObject.activeInHierarchy || UIMultiplayer.GetInstance<UIMultiplayer>().BRMObj.GetObjectFromList<RectTransform>(6).gameObject.activeInHierarchy)
                {
                    InputManager.Instance.SetCursorEnabled(true);
                    return false;
                }
            }
            catch
            {
            }
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UpgradeUITooltip), "Show", new Type[] { typeof(RectTransform), typeof(UpgradeDescription) })]
        private static void UpgradeUITooltip_Show_Postfix(UpgradeUITooltip __instance, RectTransform iconTransform, UpgradeDescription upgradeDescription)
        {
            __instance.TitleLabel.text = OverhaulMain.GetTranslatedString(__instance.TitleLabel.text, true);
            __instance.DescriptionLabel.text = OverhaulMain.GetTranslatedString(__instance.DescriptionLabel.text, true);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameUIRoot), "InitializeUI")]
        private static void GameUIRoot_InitializeUI_Postfix()
        {
            //V3Tests.Base.SceneTransitionController.EndTripToMainMenu();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TitleScreenUI), "OnWorkshopBrowserButtonClicked")]
        private static bool TitleScreenUI_OnWorkshopBrowserButtonClicked_Prefix(TitleScreenUI __instance)
        {
            if (!OverhaulDescription.TEST_FEATURES_ENABLED)
            {
                return true;
            }
            V3Tests.Gameplay.ArenaController.SetRootAndLogoVisible(false);
            UIWorkshopBrowser.GetInstance<UIWorkshopBrowser>().Show();
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
            UIGamemodeSelection.GetInstance<UIGamemodeSelection>().Hide();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PhotoModeControlsDisplay), "SetVisibility")]
        private static void PhotoModeControlsDisplay_SetVisibility_Postfix(PhotoModeControlsDisplay __instance, bool value)
        {
            if (!OverhaulDescription.TEST_FEATURES_ENABLED)
            {
                return;
            }
            __instance.gameObject.SetActive(false);
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
            UICrashScreen.GetInstance<UICrashScreen>().Show(errorMessage);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ErrorWindow), "Hide")]
        private static void ErrorWindow_Hide_Postfix()
        {
            UICrashScreen.GetInstance<UICrashScreen>().Hide();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BattleRoyaleUI), "Show")]
        private static bool BattleRoyaleUI_Show_Postfix(BattleRoyaleUI __instance)
        {
            if (OverhaulMain.GetSetting<bool>("Patches.GUI.Last Bot Standing"))
            {
                UIMultiplayer.GetInstance<UIMultiplayer>().Show(UIMultiplayer.EMultiplayerUI.BattleRoyale);
                return false;
            }
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BattleRoyaleUI), "Hide")]
        private static void BattleRoyaleUI_Hide_Postfix()
        {
            UIMultiplayer.GetInstance<UIMultiplayer>().Hide(UIMultiplayer.EMultiplayerUI.BattleRoyale);
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
            if (!OverhaulMain.GetSetting<bool>("Patches.GUI.Pause menu"))
            {
                __instance.GetComponent<UnityEngine.UI.Image>().enabled = true;
                return;
            }
            ObjectFixer.FixObject(__instance.transform, "FixEscMenu", __instance);
            UIPauseMenu.GetInstance<UIPauseMenu>().Show();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(EscMenu), "Hide")]
        private static void EscMenu_Hide_Postfix(EscMenu __instance)
        {
            GameUIRoot.Instance.AchievementProgressUI.Hide();
            UIPauseMenu.GetInstance<UIPauseMenu>().Hide();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SettingsMenu), "Hide")]
        private static void SettingsMenu_Hide_Postfix(SettingsMenu __instance)
        {
            if (BaseStaticValues.IsEscMenuWaitingToShow)
            {
                UIPauseMenu.GetInstance<UIPauseMenu>().Show();
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SettingsMenu), "Show")]
        private static void SettingsMenu_Show_Postfix(SettingsMenu __instance)
        {
            OverhaulSepratedPatchMethods.PatchSettings(__instance);
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SettingsMenu), "ShowContentsForTab")]
        private static void SettingsMenu_ShowContentsForTab_Postfix(SettingsMenu __instance)
        {
            OverhaulSepratedPatchMethods.PatchSettings(__instance);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MultiplayerInviteCodeUI), "ShowWithCode")]
        private static bool MultiplayerInviteCodeUI_ShowWithCode_Prefix(MultiplayerInviteCodeUI __instance, string inviteCode, bool showSettings)
        {
            UIMultiplayer.GetInstance<UIInviteToLobby>().ShowWithCode(inviteCode, showSettings);
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LevelEditorUI), "Show")]
        private static bool LevelEditorUI_Show_Prefix()
        {
            if (OverhaulMain.GetSetting<bool>("Levels.Editor.New Level Editor") && OverhaulDescription.TEST_FEATURES_ENABLED)
            {
                UILevelEditorV2.GetInstance<UILevelEditorV2>().Initialize();
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
                UIPauseMenu.GetInstance<UIPauseMenu>().Show();
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameModeSelectScreen), "Show")]
        private static bool GameModeSelectScreen_Show_Prefix(GameModeSelectScreen __instance)
        {
            UIGamemodeSelection.GetInstance<UIGamemodeSelection>().Show(__instance.GameModeData);
            return false;
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameModeSelectScreen), "Hide")]
        private static void GameModeSelectScreen_Show_Postfix(GameModeSelectScreen __instance)
        {
            UIGamemodeSelection.GetInstance<UIGamemodeSelection>().Hide();
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameModeSelectScreen), "SetMainScreenVisible")]
        private static bool GameModeSelectScreen_SetMainScreenVisible_Prefix(GameModeSelectScreen __instance, bool visible)
        {
            UIGamemodeSelection.GetInstance<UIGamemodeSelection>().SetMainScreenVisible(visible);
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

        /*
        [HarmonyPrefix]
        [HarmonyPatch(typeof(EnergyUI), "showGlow")]
        public static bool EnergyUI_showGlow_Postfix(EnergyUI __instance, float position, float energyAmount, ref string animationName)
        {
            if(animationName == "InsufficientAmount")
            {
                return !GameModeManager.IsMultiplayer();
            }
            return true;
        }*/

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

    [HarmonyPatch(typeof(ChapterLoadingScreen))]
    public class OverhaulPatches
    {
        /*
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UpgradeManager), "AddStartUpgradesIfMissing")]
        private static bool UpgradeManager_AddStartUpgradesIfMissing_Postfix(ref Dictionary<UpgradeType, int> playerUpgrades)
        {
            return false;
        }*/

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Character), "SetCameraAnimatorEnabled")]
        private static bool Character_SetCameraAnimatorEnabled_Prefix()
        {
            return V3Tests.Gameplay.AdvancedCameraController.GetInstance<AdvancedCameraController>().AllowCameraAnimatorAndMoverEnabled();
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(FirstPersonMover), "Update")]
        private static void Character_Update_Postfix(FirstPersonMover __instance)
        {
            if (__instance.IsRidingOtherCharacter() && __instance.transform.parent != null && __instance.transform.parent.gameObject.name == "RiderContainer")
            {
                __instance.transform.localPosition = Vector3.zero;
                __instance.transform.localEulerAngles = Vector3.zero;
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(LevelEditorCinematicCamera), "TurnOff")]
        private static void LevelEditorCinematicCamera_TurnOff_Postfix()
        {
            V3Tests.Base.V3_MainModController.CallEvent("cinematicCamera.Off", null);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(LevelEditorCinematicCamera), "TurnOn")]
        private static void LevelEditorCinematicCamera_TurnOn_Postfix()
        {
            V3Tests.Base.V3_MainModController.CallEvent("cinematicCamera.On", null);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerCameraMover), "LateUpdate")]
        private static bool PlayerCameraMover_LateUpdate_Prefix()
        {
            return V3Tests.Gameplay.AdvancedCameraController.GetInstance<AdvancedCameraController>().AllowCameraAnimatorAndMoverEnabled();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SceneTransitionManager), "DisconnectAndExitToMainMenu")]
        private static bool SceneTransitionManager_DisconnectAndExitToMainMenu_Prefix()
        {
            V3Tests.HUD.SceneTransitionController.GoToMainMenu();
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SceneTransitionManager), "InstantiateSceneTransitionOverlay")]
        private static bool SceneTransitionManager_InstantiateSceneTransitionOverlay_Prefix()
        {
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SceneTransitionManager), "onShutDownComplete")]
        private static bool SceneTransitionManager_onShutDownComplete_Prefix()
        {
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ErrorManager), "sendExceptionDetailsToLoggly")]
        private static bool ErrorManager_sendExceptionDetailsToLoggly_Prefix()
        {
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PhotoManager), "Update")]
        private static bool PhotoManager_Update_Prefix()
        {
            return true;
        }

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

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FirstPersonMover), "OnDestroy")]
        private static void FirstPersonMover_OnDestroy_Postfix(FirstPersonMover __instance)
        {
            BaseStaticReferences.ModuleManager.ExecuteFunction("firstPersonMover.OnDestroy", new object[]
            {
                __instance.GetRobotInfo()
            });
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FirstPersonMover), "ExecuteCommand")]
        private static void FirstPersonMover_ExecuteCommand_Postfix(FirstPersonMover __instance, Bolt.Command command, bool resetState)
        {
            string str = FirstPersonMoverAddititonBase.TEMPORAL_PREFIX + __instance.GetInstanceID();
            if (OverhaulCacheAndGarbageController.ContainsTemporalObject(str))
            {
                FirstPersonMoverAddititonBase aBase = OverhaulCacheAndGarbageController.GetTemporalObject<FirstPersonMoverAddititonBase>(str);
                if(aBase != null)
                {
                    aBase.ReceiveCommand((FPMoveCommand)command);
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ControlsHintPanel), "areHintsDisabled")]
        private static void ControlsHintPanel_areHintsDisabled_Postfix(ref bool __result)
        {
            __result = true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ArmorPiece), "Initialize")]
        private static void ArmorPiece_Initialize_Postfix(ArmorPiece __instance)
        {
            ObjectFixer.FixObject(__instance.transform, "FixArmorPiece", __instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SettingsManager), "ShouldHideGameUI")]
        private static void SettingsManager_ShouldHideGameUI_Postfix(ref bool __result)
        {
            if (MiscEffectsManager.IsUIHidden)
            {
                __result = true;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(AttackManager), "CreateSwordBlockVFX")]
        private static bool AttackManager_CreateSwordBlockVFX_Prefix(Vector3 position)
        {
            OverhaulGraphicsController.Simulate_SwordBlock(position, false);
            return false;
        }

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
                OverhaulGraphicsController.Simulate_ArrowCollision(__instance.transform.position);
            }
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(AttackManager), "CreateHammerHitEffectVFX")]
        private static void AttackManager_CreateHammerHitEffectVFX_Postfix(Vector3 position)
        {
            OverhaulGraphicsController.Simulate_HammerHit(position);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(AttackManager), "CreateKickHitVFX")]
        private static void AttackManager_CreateKickHitVFX_Postfix(Vector3 position)
        {
            OverhaulGraphicsController.Simulate_Kick(position);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MultiplayerPlayerInfoManager), "AddPlayerInfoState")]
        private static void MultiplayerPlayerInfoManager_AddPlayerInfoState_Postfix(MultiplayerPlayerInfoState multiplayerPlayerInfoState)
        {
            BaseStaticReferences.ModuleManager.ExecuteFunction("onPlayerJoined", new object[]
            {
                multiplayerPlayerInfoState
            });
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GlobalFireParticleSystem), "CreateGroundImpactVFX")]
        public static void GlobalFireParticleSystem_CreateGroundImpactVFX_Postfix(Vector3 positon)
        {
            OverhaulGraphicsController.Simulate_SwordBlock(positon, true);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MindSpaceBodyPart), "tryExplodeBodyPart")]
        public static void MindSpaceBodyPart_tryExplodeBodyPart_Postfix(MindSpaceBodyPart __instance, ref bool __result)
        {
            if (__result)
            {
                BodyPartPatcher.OnBodyPartDamaged(__instance);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ExplodeWhenCut), "onBodyPartDamaged")]
        public static void ExplodeWhenCut_onBodyPartDamaged_Prefix(ExplodeWhenCut __instance)
        {
            if (__instance != null && !__instance.GetPrivateField<bool>("_hasExploded") && __instance.ExplosionSpawnPoint != null)
            {
                OverhaulGraphicsController.Simulate_Explosion(__instance.ExplosionSpawnPoint.position);

                RobotShortInformation robotInfo = V3Tests.Gameplay.GameStatisticsController.GameStatistics.PlayerRobotInformation;
                if (!robotInfo.IsNull)
                {
                    Vector3 position = __instance.transform.position;
                    Vector3 b = robotInfo.Instance.transform.position;
                    if (Vector3.Distance(position, b) < 40f)
                    {
                        Singleton<PlayerCameraManager>.Instance.ShakeCamera(0.2f, 0.8f);
                    }
                }
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
            BodyPartPatcher.CanCalculateVoxelWorldPositionNextTime = !BodyPartPatcher.CanCalculateVoxelWorldPositionNextTime;
            if (!BodyPartPatcher.CanCalculateVoxelWorldPositionNextTime)
            {
                return;
            }
            if (UnityEngine.Random.Range(1, 10) > 5)
            {
                Vector3 voxelWorldPosition = currentFrame.GetVoxelWorldPosition(voxelPosition);
                OverhaulGraphicsController.Simulate_Burning(voxelWorldPosition);
            }
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
            ObjectFixer.FixObject(__instance.transform, "PS4Cube", __instance);
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

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LevelEditorObjectPlacementManager), "Select")]
        private static bool LevelEditorObjectPlacementManager_Select_Prefix(ObjectPlacedInLevel objectToSelect, bool deselectAllAnimationTracks = true)
        {
            return !UIPauseMenu.GetInstance<UIPauseMenu>().gameObject.activeInHierarchy;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(LevelEditorObjectPlacementManager), "Select")]
        private static void LevelEditorObjectPlacementManager_Select_Postfix(LevelEditorObjectPlacementManager __instance, ObjectPlacedInLevel objectToSelect, bool deselectAllAnimationTracks = true)
        {
            CrossModManager.DoAction("ModdedLevelEditor.RefreshSelected", new object[]
            {
                __instance
            });
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(LevelEditorObjectPlacementManager), "Deselect")]
        private static void LevelEditorObjectPlacementManager_Deselect_Postfix(LevelEditorObjectPlacementManager __instance, ObjectPlacedInLevel objectToDeselect)
        {
            CrossModManager.DoAction("ModdedLevelEditor.RefreshSelected", new object[]
            {
                __instance
            });
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(LevelEditorObjectPlacementManager), "DeselectEverything")]
        private static void LevelEditorObjectPlacementManager_DeselectEverything_Postfix(LevelEditorObjectPlacementManager __instance)
        {
            CrossModManager.DoAction("ModdedLevelEditor.RefreshSelected", new object[]
            {
                __instance
            });
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(LevelEditorToolManager), "SetActiveTool")]
        private static void LevelEditorToolManager_SetActiveTool_Postfix(LevelEditorToolType toolType)
        {
            if (!UILevelEditorV2.IsEnabled)
            {
                return;
            }
            UILevelEditorV2.GetInstance<UILevelEditorV2>().GetHUD<UIToolbar>().SetActiveTool(toolType);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(LevelEditorPointLight), "Start")]
        private static void LevelEditorPointLight_Start_Postfix(LevelEditorPointLight __instance)
        {
            OverhaulGraphicsController.PatchLight(__instance.PointLight, __instance.UseShadows);
            if (!ArenaWeatherController.IsDustEnabled)
            {
                return;
            }
            PointLightDust pointLightDust = __instance.gameObject.AddComponent<PointLightDust>();
            pointLightDust.Target = __instance.transform;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SceneTransitionManager), "DisconnectAndExitToMainMenu")]
        public static void SceneTransitionManager_DisconnectAndExitToMainMenu_Postfix(SceneTransitionManager __instance)
        {
            OverhaulMain.Modules.ExecuteFunction("onBoltShutdown", null);
        }

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

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LevelEditorObjectPlacementManager), "PlaceObjectInLevelRoot")]
        private static bool LevelEditorObjectPlacementManager_PlaceObjectInLevelRoot_Prefix(LevelObjectEntry objectPlacedLevelObjectEntry, Transform levelRoot, ref ObjectPlacedInLevel __result)
        {
            __result = ModdedLevelEditorManager.Instance.PlaceObject(objectPlacedLevelObjectEntry, levelRoot);
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetCollider), "OnEnable")]
        private static void PlanetCollider_OnEnable_Prefix(PlanetCollider __instance)
        {
            ObjectFixer.FixObject(__instance.transform, "Planet_Earth", __instance);
        }

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

        [HarmonyPostfix]
        [HarmonyPatch(typeof(LevelManager), "ShouldCelebrateArenaVictoryForCurrentLevel")]
        private static void LevelManager_ShouldCelebrateArenaVictoryForCurrentLevel_Postfix(LevelManager __instance, ref bool __result)
        {
            if (OverModesController.CurrentGamemodeIsOvermode())
            {
                __result = false;
            }
        }

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

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameFlowManager), "RefreshIsLevelWon")]
        private static bool GameFlowManager_RefreshIsLevelWon_Prefix()
        {
            return !OverModesController.CurrentGamemodeIsOvermode();
        }

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

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ObjectPlacedInLevel), "Initialize")]
        private static void ObjectPlacedInLevel_Initialize_Postfix(ObjectPlacedInLevel __instance)
        {
            ObjectFixer.FixObject(__instance.transform, "objectPlacedInLevel", __instance);
        }

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

        [HarmonyPostfix]
        [HarmonyPatch(typeof(LevelEditorDataManager), "DeserializeInto")]
        private static void LevelEditorDataManager_DeserializeInto_Postfix()
        {
            MetaDataController.GetInstance<MetaDataController>().RefreshCurrentLevelMetaData();
        }

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

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(PhotoManager), "Update")]
        private static IEnumerable<CodeInstruction> PhotoManager_Update_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> list = new List<CodeInstruction>(instructions);
            if (!OverhaulDescription.TEST_FEATURES_ENABLED)
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

                OverhaulGraphicsController.Simulate_Cut(voxelWorldPosition, fireSpreadDefinition != null);
            }
            catch (System.Exception ex)
            {
            }
        }

        public static void OnBodyPartDamaged(BaseBodyPart part)
        {
            if (part is MindSpaceBodyPart)
            {
                OverhaulGraphicsController.Simulate_Mindspace_Hit(part.transform.position);
            }
        }

        public static void AddMechBodyPartComponent(GameObject obj)
        {
            MechBodyPart part = obj.AddComponent<MechBodyPart>();
            throw new System.NotImplementedException();
        }
    }
}
