using CDOverhaul.HUD;
using CDOverhaul.Workshop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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

        public GameObject canvasObject
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
        }

        public override void OnSceneReloaded()
        {
            base.OnSceneReloaded();
            instantiateCanvas();
            showMainUIs();
        }

        private void instantiateCanvas()
        {
            if (canvasObject)
                return;

            m_InstantiatedPrefabs.Clear();
            if (!canvasObjectPrefab)
                canvasObjectPrefab = OverhaulAssetsController.GetAsset("OverhaulUIBase", OverhaulAssetPart.Part1, false);

            canvasObject = Instantiate(canvasObjectPrefab, GameUIRoot.Instance.transform);
            canvasObject.name = "OverhaulUIs";
            canvasObject.transform.localScale = Vector3.one;
        }

        private void showMainUIs()
        {
            if (!canvasObject)
                return;

            Show<UIOverhaulVersionLabel>("UI_VersionLabel");
        }

        public T Show<T>(string assetKey) where T : UIController
        {
            OverhaulDebug.Log(string.Format("Show UI: {0} <{1}>", new object[] { assetKey, typeof(T).ToString() }), EDebugType.UI);
            T toShow = null;
            if (!m_InstantiatedPrefabs.ContainsKey(assetKey))
            {
                GameObject toInstantiate = null;
                if (!m_CachedPrefabs.ContainsKey(assetKey))
                {
                    toInstantiate = OverhaulAssetsController.GetAsset(assetKey, OverhaulAssetPart.Part1, false);
                    m_CachedPrefabs.Add(assetKey, toInstantiate);
                }
                else
                {
                    toInstantiate = m_CachedPrefabs[assetKey];
                }
                toShow = Instantiate(toInstantiate, canvasObject.transform).AddComponent<T>();
                toShow.Initialize();
                m_InstantiatedPrefabs.Add(assetKey, toShow);
                OverhaulDebug.Log("Initialized UI: " + assetKey, EDebugType.UI);
            }
            else
            {
                toShow = (T)m_InstantiatedPrefabs[assetKey];
            }
            toShow.Show();
            OverhaulDebug.Log("Showed UI: " + assetKey, EDebugType.UI);
            return null;
        }
    }
}
