using CDOverhaul.HUD;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul
{
    public class OverhaulUIManager : OverhaulManager<OverhaulUIManager>
    {
        private Dictionary<string, GameObject> m_CachedPrefabs;
        private Dictionary<string, GameObject> m_InstantiatedPrefabs;

        private GameObject m_CanvasPrefab;

        public OverhaulUIPrefabs uiPrefabs
        {
            get;
            private set;
        }

        public Transform containerTransform
        {
            get;
            private set;
        }

        public bool hasLoadedAssets
        {
            get;
            private set;
        }

        public override void Initialize()
        {
            base.Initialize();
            m_CachedPrefabs = new Dictionary<string, GameObject>();
            m_InstantiatedPrefabs = new Dictionary<string, GameObject>();
        }

        protected override void OnAssetsLoaded()
        {
            base.OnAssetsLoaded();

            if (!uiPrefabs)
                uiPrefabs = base.gameObject.AddComponent<OverhaulUIPrefabs>();

            instantiateCanvas();
            showMainUIs();

            hasLoadedAssets = true;
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
            if (!containerTransform)
            {
                if (!m_CanvasPrefab)
                    m_CanvasPrefab = OverhaulAssetLoader.GetAsset("OverhaulUIBase", OverhaulAssetPart.Part1, false);

                GameObject canvasObject = Instantiate(m_CanvasPrefab, GameUIRoot.Instance.transform);
                canvasObject.transform.localScale = Vector3.one;

                ModdedObject moddedObject = canvasObject.GetComponent<ModdedObject>();
                containerTransform = moddedObject.GetObject<Transform>(0);
                containerTransform.SetParent(containerTransform.parent.parent, false);
                containerTransform.name = "OverhaulUIs";
                Destroy(canvasObject);
            }
        }

        private void showMainUIs()
        {
            UIConstants.ShowVersionLabel();
        }

        public T Show<T>(string assetKey, object[] args = null) where T : UIController
        {
            if (OverhaulCore.isShuttingDownBolt || !containerTransform)
                return null;

            if (args == null)
                args = Array.Empty<object>();

            OverhaulDebug.Log(string.Format("Show UI: {0} <{1}>", new object[] { assetKey, typeof(T).ToString() }), EDebugType.UI);

            T toShow;
            if (!HasInstantiated(assetKey))
            {
                GameObject prefab;
                if (!HasCached(assetKey))
                {
                    prefab = OverhaulAssetLoader.GetAsset(assetKey, OverhaulAssetPart.Part1, false);
                    m_CachedPrefabs.Add(assetKey, prefab);
                }
                else
                {
                    prefab = m_CachedPrefabs[assetKey];
                }

                GameObject gameObject = Instantiate(prefab, containerTransform, false);
                m_InstantiatedPrefabs.Add(assetKey, gameObject);
                prepareUIObject(gameObject, args);

                OverhaulDebug.Log(string.Format("Instantiated UI Prefab: {0} <{1}>", new object[] { assetKey, typeof(T).ToString() }), EDebugType.UI);

                toShow = gameObject.AddComponent<T>();
                toShow.Initialize();
            }
            else
            {
                toShow = m_InstantiatedPrefabs[assetKey]?.GetComponent<T>();
            }

            toShow.Show();
            toShow.OnGetArguments(args);

            OverhaulDebug.Log("Showed UI: " + assetKey, EDebugType.UI);

            return toShow;
        }

        public T GetUI<T>(string assetKey) where T : UIController
        {
            _ = m_InstantiatedPrefabs.TryGetValue(assetKey, out GameObject result);
            return result.GetComponent<T>();
        }

        public bool HasCached(string assetKey)
        {
            return m_CachedPrefabs.ContainsKey(assetKey);
        }

        public bool HasInstantiated(string assetKey)
        {
            return m_InstantiatedPrefabs.ContainsKey(assetKey);
        }

        public bool IsVisible(string assetKey)
        {
            UIController uiController = GetUI<UIController>(assetKey);
            return uiController && uiController.gameObject.activeSelf;
        }

        private void prepareUIObject(GameObject gameObject, object[] args)
        {
            if (!args.Contains(UIConstants.Arguments.DONT_UPDATE_EFFECTS))
            {
                replaceUIEffects(gameObject);
            }
        }

        private void replaceUIEffects(GameObject gameObject)
        {
            if (!gameObject)
                return;

            Outline[] outlines = gameObject.GetComponentsInChildren<Outline>();
            if (outlines.IsNullOrEmpty())
                return;

            foreach (Outline outline in outlines)
            {
                if (!outline || outline.GetType() != typeof(Outline))
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
        public static void ParentTransformToGameUIRoot(Transform transform)
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
        public static void SetCanvasPixelPerfect(bool value) => GameUIRoot.Instance.GetComponent<Canvas>().pixelPerfect = !GameModeManager.IsInLevelEditor() && value;
    }
}
