using CDOverhaul.DevTools;
using CDOverhaul.Gameplay.Editors.Personalization;
using CDOverhaul.HUD.Gamemodes;
using CDOverhaul.HUD.Overlays;
using CDOverhaul.HUD.Tutorial;
using CDOverhaul.HUD.Vanilla;
using CDOverhaul.Workshop;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class OverhaulCanvasManager : OverhaulManager<OverhaulCanvasManager>
    {
        private static GameObject s_UIPrefab;

        /// <summary>
        /// The modded object with all UI references
        /// </summary>
        public ModdedObject HUDModdedObject
        {
            get;
            private set;
        }
        public GameObject[] HUDPrefabsArray
        {
            get;
            private set;
        }

        private GameObject m_CanvasFromPrefab;

        public bool HasSpawnedHUD => HUDModdedObject != null;

        protected override void OnAssetsLoaded()
        {
            base.OnAssetsLoaded();
            InstantiateUI();
        }

        public override void OnSceneReloaded()
        {
            base.OnSceneReloaded();
            InstantiateUI();
        }

        public void InstantiateUI()
        {
            if (!s_UIPrefab)
                s_UIPrefab = OverhaulAssetsController.GetAsset("BetaOverhaulUI", OverhaulAssetPart.Part1);

            m_CanvasFromPrefab = Instantiate(s_UIPrefab);

            ModdedObject moddedObject = m_CanvasFromPrefab.GetComponent<ModdedObject>();
            HUDModdedObject = moddedObject.GetObject<ModdedObject>(0);
            ParentTransformToGameUIRoot(HUDModdedObject.transform);
            SetCanvasPixelPerfect(true);

            GameUIRoot.Instance.ErrorWindow.transform.SetAsLastSibling();

            ModdedObject prefabsModdedObject = moddedObject.GetObject<ModdedObject>(1);
            HUDPrefabsArray = new GameObject[prefabsModdedObject.objects.Count];
            int index = 0;
            foreach (Object @object in prefabsModdedObject.objects)
            {
                HUDPrefabsArray[index] = @object as GameObject;
                index++;
            }
            prefabsModdedObject.gameObject.SetActive(false);

            _ = AddHUD<OverhaulVersionLabel>(HUDModdedObject.GetObject<ModdedObject>(0));
            _ = AddHUD<ParametersMenu>(HUDModdedObject.GetObject<ModdedObject>(3));
            _ = AddHUD<OverhaulPauseMenu>(HUDModdedObject.GetObject<ModdedObject>(6));
            _ = AddHUD<OverhaulOverlays>(HUDModdedObject.GetObject<ModdedObject>(7));
            //_ = AddHUD<PersonalizationMenu>(HUDModdedObject.GetObject<ModdedObject>(8)).Category = Gameplay.PersonalizationCategory.WeaponSkins;
            _ = AddHUD<AccessoriesPersonalizationPanel>(HUDModdedObject.GetObject<ModdedObject>(5));
            _ = AddHUD<OverhaulDialogues>(HUDModdedObject.GetObject<ModdedObject>(9));
            _ = AddHUD<OverhaulPatchNotesUI>(HUDModdedObject.GetObject<ModdedObject>(10));
            _ = AddHUD<OverhaulLocalizationEditor>(HUDModdedObject.GetObject<ModdedObject>(4));
            _ = AddHUD<OverhaulLoadingScreen>(HUDModdedObject.GetObject<ModdedObject>(12));
            _ = AddHUD<OverhaulWorkshopBrowserUI>(HUDModdedObject.GetObject<ModdedObject>(13));
            _ = AddHUD<OverhaulUIDescriptionTooltip>(HUDModdedObject.GetObject<ModdedObject>(15));
            _ = AddHUD<OverhaulUIImageViewer>(HUDModdedObject.GetObject<ModdedObject>(17));
            _ = AddHUD<ModSetupWindow>(HUDModdedObject.GetObject<ModdedObject>(16));
            _ = AddHUD<OverhaulGamemodesUI>(HUDModdedObject.GetObject<ModdedObject>(19));
            _ = AddHUD<VanillaUIImprovements>(HUDModdedObject.GetObject<ModdedObject>(1));
            _ = AddHUD<AboutOverhaulMenu>(HUDModdedObject.GetObject<ModdedObject>(2));
            _ = AddHUD<OverhaulSurveyUI>(HUDModdedObject.GetObject<ModdedObject>(14));
            _ = AddHUD<OverhaulDevToolsUI>(HUDModdedObject.GetObject<ModdedObject>(21));
            _ = AddHUD<OverhaulFullscreenDialogueWindow>(HUDModdedObject.GetObject<ModdedObject>(22));
            _ = AddHUD<OverhaulTutorialUI>(HUDModdedObject.GetObject<ModdedObject>(23));
            _ = AddHUD<OverhaulCrashScreen>(HUDModdedObject.GetObject<ModdedObject>(25));
            _ = AddHUD<OverhaulAchievementsMenu>(HUDModdedObject.GetObject<ModdedObject>(26));
            _ = AddHUD<OverhaulConnectScreen>(HUDModdedObject.GetObject<ModdedObject>(27));
            _ = AddHUD<OverhaulDisconnectScreen>(HUDModdedObject.GetObject<ModdedObject>(28));
            _ = AddHUD<PersonalizationEditorUI>(HUDModdedObject.GetObject<ModdedObject>(29));
            _ = AddHUD<MountsPersonalizationPanel>(HUDModdedObject.GetObject<ModdedObject>(30));

            m_CanvasFromPrefab.GetComponent<Canvas>().enabled = false;
            m_CanvasFromPrefab.GetComponent<CanvasScaler>().enabled = false;
        }

        /// <summary>
        /// Add new HUD controller to <paramref name="moddedObject"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="moddedObject"></param>
        /// <returns></returns>
        public T AddHUD<T>(ModdedObject moddedObject) where T : OverhaulUI => moddedObject == null ? null : OverhaulController.Add<T>(moddedObject.transform);

        /// <summary>
        /// TBA
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameObject GetHUDPrefab(in string name)
        {
            foreach (GameObject @object in HUDPrefabsArray)
            {
                if (@object == null)
                    break;

                else if (@object.name == name)
                    return @object;
            }
            return null;
        }

        public static List<T> GetAllComponentsWithModdedObjectRecursive<T>(string targetModdedObjectId, Transform targetTransform) where T : Component
        {
            List<T> list = new List<T>();
            if (targetTransform == null || targetTransform.childCount == 0)
                return list;

            for (int i = 0; i < targetTransform.childCount; i++)
            {
                Transform t = targetTransform.GetChild(i);
                T component = t.GetComponent<T>();
                ModdedObject m = t.GetComponent<ModdedObject>();
                if (component != null && m != null && !string.IsNullOrEmpty(m.ID) && m.ID.Contains(targetModdedObjectId))
                {
                    list.Add(component);
                }
                list.AddRange(GetAllComponentsWithModdedObjectRecursive<T>(targetModdedObjectId, t));
            }
            return list;
        }

        /// <summary>
        /// Parent transform to <see cref="GameUIRoot"/>.Instance.transform and instantly fix position and scale
        /// </summary>
        /// <param name="transform"></param>
        public static void ParentTransformToGameUIRoot(in Transform transform)
        {
            if (GameUIRoot.Instance == null)
                return;

            transform.SetParent(GameUIRoot.Instance.transform);
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
        }

        /// <summary>
        /// Set <see cref="Canvas.pixelPerfect"/> value in <see cref="GameUIRoot"/>
        /// </summary>
        /// <param name="value"></param>
        public static void SetCanvasPixelPerfect(in bool value) => GameUIRoot.Instance.GetComponent<Canvas>().pixelPerfect = !GameModeManager.IsInLevelEditor() && value;

        public override void OnModDeactivated() => destroyHUD();
        private void destroyHUD()
        {
            if (!HasSpawnedHUD)
                return;

            Destroy(HUDModdedObject.gameObject);
            Destroy(m_CanvasFromPrefab);
        }
    }
}
