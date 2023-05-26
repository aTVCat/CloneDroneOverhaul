using CDOverhaul.Localization;
using CDOverhaul.Workshop;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class OverhaulCanvasController : OverhaulController
    {
        /// <summary>
        /// The prefab name of HUD
        /// </summary>
        private const string OverhaulHUDName = "BetaOverhaulUI";

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

        public override void Initialize()
        {
            m_CanvasFromPrefab = Instantiate(OverhaulAssetsController.GetAsset(OverhaulHUDName, OverhaulAssetPart.Part1));

            ModdedObject moddedObject = m_CanvasFromPrefab.GetComponent<ModdedObject>();
            HUDModdedObject = moddedObject.GetObject<ModdedObject>(0);
            ParentTransformToGameUIRoot(HUDModdedObject.transform);
            SetCanvasPixelPerfect(true);

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
            _ = AddHUD<OverhaulPauseMenu>(HUDModdedObject.GetObject<ModdedObject>(6));
            _ = AddHUD<Overlays.OverhaulOverlays>(HUDModdedObject.GetObject<ModdedObject>(7));
            _ = AddHUD<WeaponSkinsMenu>(HUDModdedObject.GetObject<ModdedObject>(8));
            _ = AddHUD<WeaponSkinsMenu>(HUDModdedObject.GetObject<ModdedObject>(5)).IsOutfitSelection = true;
            _ = AddHUD<OverhaulDialogues>(HUDModdedObject.GetObject<ModdedObject>(9));
            _ = AddHUD<OverhaulPatchNotesUI>(HUDModdedObject.GetObject<ModdedObject>(10));
            _ = AddHUD<OverhaulLocalizationEditor>(HUDModdedObject.GetObject<ModdedObject>(4));
            _ = AddHUD<OverhaulLoadingScreen>(HUDModdedObject.GetObject<ModdedObject>(12));
            _ = AddHUD<OverhaulWorkshopBrowserUI>(HUDModdedObject.GetObject<ModdedObject>(13));
            _ = AddHUD<OverhaulUIDescriptionTooltip>(HUDModdedObject.GetObject<ModdedObject>(15));
            _ = AddHUD<OverhaulUIImageViewer>(HUDModdedObject.GetObject<ModdedObject>(17));

            m_CanvasFromPrefab.GetComponent<Canvas>().enabled = false;
            m_CanvasFromPrefab.GetComponent<CanvasScaler>().enabled = false;
        }

        /// <summary>
        /// Add new HUD controller to <paramref name="moddedObject"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="moddedObject"></param>
        /// <returns></returns>
        public T AddHUD<T>(in ModdedObject moddedObject) where T : OverhaulUI
        {
            return moddedObject == null ? null : OverhaulController.AddController<T>(moddedObject.transform);
        }

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
                {
                    break;
                }
                else if (@object.name == name)
                {
                    return @object;
                }
            }
            return null;
        }

        public List<T> GetAllComponentsWithModdedObjectRecursive<T>(string targetModdedObjectId, Transform targetTransform) where T : Component
        {
            List<T> list = new List<T>();
            if (targetTransform == null || targetTransform.childCount == 0)
            {
                return list;
            }

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
            if(GameUIRoot.Instance == null) return;
            transform.SetParent(GameUIRoot.Instance.transform);
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
        }

        /// <summary>
        /// Set <see cref="Canvas.pixelPerfect"/> value in <see cref="GameUIRoot"/>
        /// </summary>
        /// <param name="value"></param>
        public static void SetCanvasPixelPerfect(in bool value)
        {
            GameUIRoot.Instance.GetComponent<Canvas>().pixelPerfect = value;
        }

        public override void OnModDeactivated()
        {
            destroyHUD();
        }

        private void destroyHUD()
        {
            if (!HasSpawnedHUD)
            {
                return;
            }

            Destroy(HUDModdedObject.gameObject);
            Destroy(m_CanvasFromPrefab);
        }

        public override string[] Commands()
        {
            throw new System.NotImplementedException();
        }

        public override string OnCommandRan(string[] command)
        {
            throw new System.NotImplementedException();
        }
    }
}
