using CDOverhaul.HUD;
using CDOverhaul.Workshop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static MessageMenu;

namespace CDOverhaul
{
    public class OverhaulUIManager : OverhaulManager<OverhaulUIManager>
    {
        private Dictionary<string, GameObject> m_CachedPrefabs;
        private Dictionary<string, UIController> m_InstantiatedPrefabs;

        public GameObject canvasObjectPrefab
        {
            get;
            private set;
        }

        public Transform containerTransform
        {
            get;
            private set;
        }

        public OverhaulUIPrefabs uiPrefabs
        {
            get;
            private set;
        }

        public override void Initialize()
        {
            base.Initialize();
            m_CachedPrefabs = new Dictionary<string, GameObject>();
            m_InstantiatedPrefabs = new Dictionary<string, UIController>();
        }

        protected override void OnAssetsLoaded()
        {
            base.OnAssetsLoaded();
            instantiateCanvas();
            showMainUIs();
            if(!uiPrefabs)
                uiPrefabs = base.gameObject.AddComponent<OverhaulUIPrefabs>();
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            if (uiPrefabs)
                uiPrefabs.Dispose(true);
        }

        public override void OnSceneReloaded()
        {
            base.OnSceneReloaded();
            instantiateCanvas();
            showMainUIs();
        }

        private void instantiateCanvas()
        {
            if (containerTransform)
                return;

            m_InstantiatedPrefabs.Clear();
            if (!canvasObjectPrefab)
                canvasObjectPrefab = OverhaulAssetLoader.GetAsset("OverhaulUIBase", OverhaulAssetPart.Part1, false);

            GameObject canvasObject = Instantiate(canvasObjectPrefab, GameUIRoot.Instance.transform);
            canvasObject.transform.localScale = Vector3.one;

            ModdedObject moddedObject = canvasObject.GetComponent<ModdedObject>();
            containerTransform = moddedObject.GetObject<Transform>(0);
            containerTransform.SetParent(containerTransform.parent.parent, false);
            containerTransform.name = "OverhaulUIs";
            Destroy(canvasObject);
        }

        private void showMainUIs()
        {
            if (!containerTransform)
                return;

            UIConstants.ShowVersionLabel();
        }

        public T Show<T>(string assetKey, object[] args = null) where T : UIController
        {
            OverhaulDebug.Log(string.Format("Show UI: {0} <{1}>", new object[] { assetKey, typeof(T).ToString() }), EDebugType.UI);
            T toShow = null;
            if (!m_InstantiatedPrefabs.ContainsKey(assetKey))
            {
                GameObject toInstantiate = null;
                if (!m_CachedPrefabs.ContainsKey(assetKey))
                {
                    toInstantiate = OverhaulAssetLoader.GetAsset(assetKey, OverhaulAssetPart.Part1, false);
                    m_CachedPrefabs.Add(assetKey, toInstantiate);
                }
                else
                {
                    toInstantiate = m_CachedPrefabs[assetKey];
                }
                toShow = Instantiate(toInstantiate, containerTransform, false).AddComponent<T>();
                prepareUIObject(toShow.gameObject, args);
                toShow.Initialize();
                toShow.OnGetArguments(args);
                m_InstantiatedPrefabs.Add(assetKey, toShow);
            }
            else
            {
                toShow = (T)m_InstantiatedPrefabs[assetKey];
            }
            toShow.Show();
            OverhaulDebug.Log("Showed UI: " + assetKey, EDebugType.UI);
            return toShow;
        }

        public T GetUI<T>(string assetKey) where T : UIController
        {
            m_InstantiatedPrefabs.TryGetValue(assetKey, out UIController result);
            return (T)result;
        }

        private void prepareUIObject(GameObject gameObject, object[] args)
        {
            if (!gameObject || args.IsNullOrEmpty())
                return;

            if (!args.Contains(UIConstants.Arguments.DONT_UPDATE_EFFECTS))
            {
                replaceUIEffects(gameObject);
            }
        }

        private void replaceUIEffects(GameObject gameObject)
        {
            Outline[] outlines = gameObject.GetComponentsInChildren<Outline>();
            foreach(Outline outline in outlines)
            {
                if (outline.GetType() != typeof(Outline))
                    continue;

                BetterOutline betterOutline = outline.gameObject.AddComponent<BetterOutline>();
                betterOutline.effectDistance = outline.effectDistance;
                betterOutline.effectColor = outline.effectColor;
                Destroy(outline);
            }
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
    }
}
