using OverhaulMod.UI;
using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod
{
    public class ModUIManager : Singleton<ModUIManager>
    {
        private Transform m_gameUIRootTransform;
        public Transform GameUIRootTransform
        {
            get
            {
                if (!m_gameUIRootTransform)
                {
                    m_gameUIRootTransform = ModCache.gameUIRoot.transform;
                }
                return m_gameUIRootTransform;
            }
        }

        private Dictionary<string, GameObject> m_InstantiatedUIs;

        public override void Awake()
        {
            base.Awake();

            m_InstantiatedUIs = new Dictionary<string, GameObject>();

            ModCore.GameInitialized += onGameInitialized;
        }

        private void OnDestroy()
        {
            ModCore.GameInitialized -= onGameInitialized;
        }

        private void onGameInitialized()
        {
            List<string> keysToRemove = new List<string>();
            foreach (KeyValuePair<string, GameObject> keyValue in m_InstantiatedUIs)
            {
                if (!keyValue.Value)
                    keysToRemove.Add(keyValue.Key);
            }
            foreach (string key in keysToRemove)
                _ = m_InstantiatedUIs.Remove(key);

            _ = Show<UIVersionLabel>(AssetBundleConstants.UI, "UI_VersionLabel", EUILayer.BeforeCrashScreen);
        }

        public bool HasInstantiatedUI(string assetKey)
        {
            return m_InstantiatedUIs.ContainsKey(assetKey);
        }

        public int GetSiblingIndex(EUILayer layer)
        {
            switch (layer)
            {
                default:
                    return 0;
                case EUILayer.Last:
                    return GameUIRootTransform.childCount;
                case EUILayer.BeforeTitleScreen:
                    return ModCache.titleScreenUI.transform.GetSiblingIndex();
                case EUILayer.AfterTitleScreen:
                    return ModCache.titleScreenUI.transform.GetSiblingIndex() + 1;
                case EUILayer.BeforeEscMenu:
                    return ModCache.gameUIRoot.EscMenu.transform.GetSiblingIndex();
                case EUILayer.AfterEscMenu:
                    return ModCache.gameUIRoot.EscMenu.transform.GetSiblingIndex() + 1;
                case EUILayer.BeforeCrashScreen:
                    return ModCache.gameUIRoot.ErrorWindow.transform.GetSiblingIndex();
                case EUILayer.AfterCrashScreen:
                    return ModCache.gameUIRoot.ErrorWindow.transform.GetSiblingIndex() + 1;
            }
        }

        public T Show<T>(string assetBundle, string assetKey, EUILayer layer = EUILayer.Last) where T : OverhaulUIBehaviour
        {
            string fullName = assetBundle + "." + assetKey;
            if (!HasInstantiatedUI(fullName))
            {
                GameObject prefab = ModResources.GetResource<GameObject>(assetBundle, assetKey);
                GameObject gameObject = Instantiate(prefab, GameUIRootTransform);
                gameObject.SetActive(true);
                m_InstantiatedUIs.Add(fullName, gameObject);
                T result1 = gameObject.AddComponent<T>();
                result1.name = fullName;
                result1.InitializeUI();
                result1.Show();
                RectTransform transform = gameObject.transform as RectTransform;
                transform.SetSiblingIndex(GetSiblingIndex(layer));
                transform.anchorMin = Vector2.zero;
                transform.anchorMax = Vector2.one;
                transform.sizeDelta = Vector2.zero;
                transform.anchoredPosition = Vector2.zero;
                transform.localScale = Vector3.one;

                return result1;
            }

            T result = m_InstantiatedUIs[fullName].GetComponent<T>();
            result.Show();
            ModCache.gameUIRoot.RefreshCursorEnabled();
            return result;
        }

        public T Show<T>(string assetBundle, string assetKey, Transform parent) where T : OverhaulUIBehaviour
        {
            T result = Show<T>(assetBundle, assetKey, EUILayer.Last);
            if (parent)
            {
                result.transform.SetParent(parent);
            }
            return result;
        }

        public T Get<T>(string assetBundle, string assetKey) where T : OverhaulUIBehaviour
        {
            string fullName = assetBundle + "." + assetKey;
            return m_InstantiatedUIs.TryGetValue(fullName, out GameObject gameObject) ? (gameObject?.GetComponent<T>()) : null;
        }

        public bool Hide(string assetBundle, string assetKey)
        {
            OverhaulUIBehaviour overhaulUIBehaviour = Get<OverhaulUIBehaviour>(assetBundle, assetKey);
            if (overhaulUIBehaviour && overhaulUIBehaviour.visible)
            {
                overhaulUIBehaviour.Hide();
                return true;
            }
            return false;
        }

        public bool ShouldEnableCursor()
        {
            foreach (GameObject gameObject in m_InstantiatedUIs.Values)
            {
                if (!gameObject || !gameObject.activeInHierarchy)
                    continue;

                OverhaulUIBehaviour behaviour = gameObject.GetComponent<OverhaulUIBehaviour>();
                if (behaviour && !behaviour.isElement && behaviour.enableCursorIfVisible)
                    return true;
            }
            return false;
        }

        internal void RemoveFromList(OverhaulUIBehaviour uIBehaviour)
        {
            _ = m_InstantiatedUIs.Remove(uIBehaviour.name);
        }

        public enum EUILayer
        {
            First,

            BeforeTitleScreen,
            AfterTitleScreen,

            BeforeEscMenu,
            AfterEscMenu,

            BeforeCrashScreen,
            AfterCrashScreen,

            Last
        }
    }
}
