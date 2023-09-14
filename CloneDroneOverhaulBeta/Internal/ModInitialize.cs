using CDOverhaul.Device;
using CDOverhaul.DevTools;
using CDOverhaul.Gameplay;
using CDOverhaul.Gameplay.Editors.Personalization;
using CDOverhaul.Gameplay.Overmodes;
using CDOverhaul.HUD;
using CDOverhaul.LevelEditor;
using CDOverhaul.Patches;
using CDOverhaul.RichPresence;
using CDOverhaul.Visuals;
using CDOverhaul.Visuals.ArenaOverhaul;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CDOverhaul
{
    public class ModInitialize : OverhaulDisposable
    {
        public static event Action OnAssetsLoadDone;

        public static void TriggerAssetLoadDoneEvent()
        {
            OnAssetsLoadDone?.Invoke();
        }

        public void LoadMainFramework()
        {
            List<Action> toInit = new List<Action>()
            {
                InitMainControllers,
                InitDebugManagers,
                InitGraphicsManagers,
                InitLevelEditorManagers,
                InitContentManagers,
                InitEnhancementManagers,
                InitMiscManagers,
                InitGameplayManagers,
                InitUserManagers
            };
            foreach (Action action in toInit)
            {
                string name = action.Method.Name;
                OverhaulDebug.Log("Initializing: " + name, EDebugType.Initialize);
                action();
                OverhaulDebug.Log("Initialized: " + name, EDebugType.Initialize);
            }
        }

        public IEnumerator LoadAssetsFramework(bool async)
        {
            List<IEnumerator> toInit = new List<IEnumerator>()
            {
                LoadAssetsCoroutine(async)
            };

            QualitySettings.asyncUploadTimeSlice = 4;
            QualitySettings.asyncUploadBufferSize = 16;
            QualitySettings.asyncUploadPersistentBuffer = true;
            foreach (IEnumerator coroutine in toInit)
            {
                yield return StaticCoroutineRunner.StartStaticCoroutine(coroutine);
            }
            OverhaulAssetsContainer.Initialize();
            yield break;
        }

        public void InitMainControllers()
        {
            EnableCursorController.Reset();
            DeviceSpecifics.Initialize();
            OverhaulEvents.Initialize();
            OverhaulSettingsManager_Old.Initialize();

            GameObject mainObject = new GameObject("Main");
            mainObject.transform.SetParent(OverhaulController.mainGameObject.transform.parent);

            _ = OverhaulController.Add<OverhaulSettingsManager>(mainObject.transform);
            _ = OverhaulController.Add<OverhaulLocalizationManager>(mainObject.transform);
        }

        public void InitDebugManagers()
        {
            OverhaulDebugConsole.Initialize();
            OverhaulDebugActions.Initialize();
        }

        public void InitGraphicsManagers()
        {
            GameObject graphicsObject = new GameObject("Graphics");
            graphicsObject.transform.SetParent(OverhaulController.mainGameObject.transform.parent);

            _ = OverhaulController.Add<OverhaulRenderManager>(graphicsObject.transform);
            _ = OverhaulController.Add<OverhaulCameraManager>(graphicsObject.transform);
            _ = OverhaulController.Add<OverhaulGraphicsManager>(graphicsObject.transform);
            _ = OverhaulController.Add<OverhaulUIEffectsManager>(graphicsObject.transform);
            _ = OverhaulController.Add<OverhaulEffectsManager>(graphicsObject.transform);
        }

        public void InitGameplayManagers()
        {
            GameObject gameplayObject = new GameObject("Gameplay");
            gameplayObject.transform.SetParent(OverhaulController.mainGameObject.transform.parent);

            _ = OverhaulController.Add<OverhaulGameplayManager>(gameplayObject.transform);
            //_ = OverhaulController.Add<HUD.Tooltips.OverhaulTooltipsController>();
        }

        public void InitLevelEditorManagers()
        {
            GameObject levelEditorObject = new GameObject("LevelEditor");
            levelEditorObject.transform.SetParent(OverhaulController.mainGameObject.transform.parent);

            _ = OverhaulController.Add<OverhaulLevelEditorManager>(levelEditorObject.transform);
        }

        public void InitEnhancementManagers()
        {
            GameObject patchesObject = new GameObject("Enhancements");
            patchesObject.transform.SetParent(OverhaulController.mainGameObject.transform.parent);

            _ = OverhaulController.Add<ArenaOverhaulManager>(patchesObject.transform);
            _ = OverhaulController.Add<ReplacementsManager>(patchesObject.transform);
        }

        public void InitContentManagers()
        {
            GameObject contentObject = new GameObject("Content");
            contentObject.transform.SetParent(OverhaulController.mainGameObject.transform.parent);

            _ = OverhaulController.Add<OverhaulRepositoryManager>(contentObject.transform);
            _ = OverhaulController.Add<AdditionalContentManager>(contentObject.transform);
            _ = OverhaulController.Add<OvermodesManager>(contentObject.transform);
            _ = OverhaulController.Add<PersonalizationManager>(contentObject.transform);
        }

        public void InitUserManagers()
        {
            OverhaulPlayerIdentifier.Initialize();

            GameObject userObject = new GameObject("User");
            userObject.transform.SetParent(OverhaulController.mainGameObject.transform.parent);

            _ = OverhaulController.Add<OverhaulRPCManager>(userObject.transform);
        }

        public void InitMiscManagers()
        {
            PersonalizationEditor.Initialize();
            Changelogs.Initialize();
            OverhaulCompatibilityChecker.CheckGameVersion();
            OverhaulUpdateChecker.CheckForUpdates();

            GameObject miscObject = new GameObject("Misc");
            miscObject.transform.SetParent(OverhaulController.mainGameObject.transform.parent);

            _ = OverhaulController.Add<OverhaulAudioLibrary>(miscObject.transform);
            _ = OverhaulController.Add<OverhaulTransitionManager>(miscObject.transform);
            _ = OverhaulController.Add<OverhaulUIManager>(miscObject.transform);
        }

        public IEnumerator LoadAssetsCoroutine(bool async = true)
        {
            List<string> toLoad = new List<string>()
            {
                OverhaulAssetLoader.ModAssetBundle_Part1,
                OverhaulAssetLoader.ModAssetBundle_Part2,
                OverhaulAssetLoader.ModAssetBundle_ArenaOverhaul,
            };
            excludeLoadedAssetBundles(toLoad);

            bool[] boolArray = new bool[toLoad.Count];
            int i = toLoad.Count - 1;
            do
            {
                int index = i;
                string assetBundle = toLoad[index];
                if (!async)
                {
                    OverhaulAssetLoader.LoadAssetBundleIfNotLoaded(assetBundle, true);
                }
                else
                {
                    OverhaulAssetLoader.LoadAssetBundleAsync(assetBundle, delegate (OverhaulAssetLoader.AssetBundleLoadHandler handler)
                    {
                        boolArray[index] = true;
                    });
                }
                i--;
            } while (i > -1);

            yield return new WaitUntil(() => !async || !boolArray.Contains(false));
            OverhaulMod.HasBootProcessEnded = true;
            TriggerAssetLoadDoneEvent();
            yield break;
        }

        private static void excludeLoadedAssetBundles(List<string> list)
        {
            if (list.IsNullOrEmpty())
                return;

            int i = list.Count - 1;
            do
            {
                if (OverhaulAssetLoader.HasLoadedAssetBundle(list[i]))
                    list.RemoveAt(i);

                i--;
            } while (i > -1);
        }
    }
}
