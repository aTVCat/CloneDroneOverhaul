using OverhaulMod.Engine;
using OverhaulMod.UI;
using OverhaulMod.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace OverhaulMod
{
    public class ModUIManager : Singleton<ModUIManager>, IGameLoadListener
    {
        [ModSetting(ModSettingsConstants.SHOW_CHAPTER_SELECTION_MENU_REWORK, true, ModSetting.Tag.UISetting)]
        public static bool ShowChapterSelectionMenuRework;

        [ModSetting(ModSettingsConstants.SHOW_ENDLESS_MODE_MENU, true, ModSetting.Tag.UISetting)]
        public static bool ShowEndlessModeMenu;

        [ModSetting(ModSettingsConstants.SHOW_CHALLENGES_MENU_REWORK, true, ModSetting.Tag.UISetting)]
        public static bool ShowChallengesMenuRework;

        [ModSetting(ModSettingsConstants.SHOW_WORKSHOP_BROWSER_REWORK, true, ModSetting.Tag.UISetting)]
        public static bool ShowWorkshopBrowserRework;

        [ModSetting(ModSettingsConstants.SHOW_ADVANCEMENTS_MENU_REWORK, true, ModSetting.Tag.UISetting)]
        public static bool ShowAdvancementsMenuRework;

        [ModSetting(ModSettingsConstants.SHOW_SETTINGS_MENU_REWORK, true, ModSetting.Tag.UISetting)]
        public static bool ShowSettingsMenuRework;

        private Dictionary<string, GameObject> m_instantiatedUIs;

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

        public bool skipHidingCustomUIs
        {
            get;
            private set;
        }

        public Action actionToInvoke
        {
            get;
            private set;
        }

        public override void Awake()
        {
            base.Awake();

            m_instantiatedUIs = new Dictionary<string, GameObject>();
        }

        private void OnDestroy()
        {
            Dictionary<string, GameObject> dictionary = m_instantiatedUIs;
            if (!dictionary.IsNullOrEmpty())
            {
                foreach (GameObject panel in dictionary.Values)
                    if (panel)
                        Destroy(panel);
            }
            dictionary.Clear();
        }

        public void OnGameLoaded()
        {
            onGameInitialized();
        }

        private void onGameInitialized()
        {
            List<string> keysToRemove = new List<string>();
            foreach (KeyValuePair<string, GameObject> keyValue in m_instantiatedUIs)
            {
                if (!keyValue.Value)
                    keysToRemove.Add(keyValue.Key);
            }
            foreach (string key in keysToRemove)
                _ = m_instantiatedUIs.Remove(key);

            ModUIConstants.ShowVersionLabel();
            ModUIConstants.ShowImageEffects();
            if (ModFeatures.IsEnabled(ModFeatures.FeatureType.Tooltips))
                ModUIConstants.ShowTooltips();
        }

        public bool HasInstantiatedUI(string assetKey)
        {
            return m_instantiatedUIs.ContainsKey(assetKey);
        }

        public bool IsUIVisible(string assetBundle, string assetKey)
        {
            string fullName = assetBundle + "." + assetKey;
            return HasInstantiatedUI(fullName) && m_instantiatedUIs[fullName].activeInHierarchy;
        }

        public int GetSiblingIndex(UILayer layer)
        {
            switch (layer)
            {
                case UILayer.Last:
                    return GameUIRootTransform.childCount;
                case UILayer.BeforeTitleScreen:
                    return ModCache.titleScreenUI.transform.GetSiblingIndex();
                case UILayer.AfterTitleScreen:
                    return ModCache.titleScreenUI.transform.GetSiblingIndex() + 1;
                case UILayer.BeforeEnergyUI:
                    return ModCache.gameUIRoot.EnergyUI.transform.GetSiblingIndex();
                case UILayer.AfterEnergyUI:
                    return ModCache.gameUIRoot.EnergyUI.transform.GetSiblingIndex() + 1;
                case UILayer.BeforeUpgradeUI:
                    return ModCache.gameUIRoot.UpgradeUI.transform.GetSiblingIndex();
                case UILayer.AfterUpgradeUI:
                    return ModCache.gameUIRoot.UpgradeUI.transform.GetSiblingIndex() + 1;
                case UILayer.BeforeEscMenu:
                    return ModCache.gameUIRoot.EscMenu.transform.GetSiblingIndex();
                case UILayer.AfterEscMenu:
                    return ModCache.gameUIRoot.EscMenu.transform.GetSiblingIndex() + 1;
                case UILayer.BeforeCrashScreen:
                    return ModCache.gameUIRoot.ErrorWindow.transform.GetSiblingIndex();
                case UILayer.AfterCrashScreen:
                    return ModCache.gameUIRoot.ErrorWindow.transform.GetSiblingIndex() + 1;
                case UILayer.BeforeMultiplayerConnectScreen:
                    return ModCache.gameUIRoot.MultiplayerConnectingScreen.transform.GetSiblingIndex();
                case UILayer.AfterMultiplayerConnectScreen:
                    return ModCache.gameUIRoot.MultiplayerConnectingScreen.transform.GetSiblingIndex() + 1;
            }
            return 0;
        }

        public T Show<T>(string assetBundle, string assetKey, UILayer layer = UILayer.Last, int siblingIndexOffset = 0) where T : OverhaulUIBehaviour
        {
            string fullName = assetBundle + "." + assetKey;
            if (!HasInstantiatedUI(fullName))
            {
                GameObject prefab = ModResources.Load<GameObject>(assetBundle, assetKey);
                GameObject gameObject = Instantiate(prefab, GameUIRootTransform);
                gameObject.SetActive(true);
                m_instantiatedUIs.Add(fullName, gameObject);
                RectTransform transform = gameObject.transform as RectTransform;
                transform.SetSiblingIndex(GetSiblingIndex(layer) + siblingIndexOffset);
                transform.anchorMin = Vector2.zero;
                transform.anchorMax = Vector2.one;
                transform.sizeDelta = Vector2.zero;
                transform.anchoredPosition = Vector2.zero;
                transform.localScale = Vector3.one;

                T result1 = gameObject.AddComponent<T>();
                result1.uiName = fullName;

#if DEBUG
                Stopwatch stopwatch = Stopwatch.StartNew();
                result1.InitializeUI();
                stopwatch.Stop();
                UnityEngine.Debug.Log($"Initialized an UI in {stopwatch.ElapsedMilliseconds} ms, {stopwatch.ElapsedTicks} ticks");
#else
          result1.InitializeUI();
#endif
                result1.Show();

                return result1;
            }

            T result = m_instantiatedUIs[fullName].GetComponent<T>();
            result.Show();
            return result;
        }

        public T Show<T>(string assetBundle, string assetKey, Transform parent) where T : OverhaulUIBehaviour
        {
            T result = Show<T>(assetBundle, assetKey, UILayer.Last);
            if (parent)
            {
                result.transform.SetParent(parent);
            }
            result.transform.SetAsLastSibling();
            return result;
        }

        public T Get<T>(string assetBundle, string assetKey) where T : OverhaulUIBehaviour
        {
            string fullName = assetBundle + "." + assetKey;
            return m_instantiatedUIs.TryGetValue(fullName, out GameObject gameObject) ? (gameObject?.GetComponent<T>()) : null;
        }

        public bool Hide(string assetBundle, string assetKey)
        {
            OverhaulUIBehaviour overhaulUIBehaviour = Get<OverhaulUIBehaviour>(assetBundle, assetKey);
            if (overhaulUIBehaviour && overhaulUIBehaviour.visible && !overhaulUIBehaviour.forceCancelHide)
            {
                overhaulUIBehaviour.Hide();
                return true;
            }
            return false;
        }

        public void RefreshUI(bool refreshOnlyCursor)
        {
            ModCache.gameUIRoot.RefreshCursorEnabled();
            if (!refreshOnlyCursor)
            {
                ModCache.gameUIRoot.SetUIOverLogoModeEnabled(ShouldEnableUIOverLogoMode());
                ModCache.titleScreenUI.setLogoAndRootButtonsVisible(GameModeManager.IsOnTitleScreen() && !ShouldHideTitleScreen());
            }
        }

        public void HideLegacyMenuInsteadOfCustom(GameObject objectToTrack)
        {
            skipHidingCustomUIs = true;
            _ = ModActionUtils.RunCoroutine(letOriginalUIHideNextTime(objectToTrack));
        }

        public void InvokeActionInsteadOfHidingCustomUI(Action action)
        {
            actionToInvoke = action;
        }

        public bool TryInvokeAction()
        {
            if (actionToInvoke != null)
            {
                actionToInvoke.Invoke();
                actionToInvoke = null;
                return true;
            }
            return false;
        }

        private IEnumerator letOriginalUIHideNextTime(GameObject objectToTrack)
        {
            while (objectToTrack && objectToTrack.activeInHierarchy)
                yield return null;

            skipHidingCustomUIs = false;
            yield break;
        }

        public bool ShouldEnableCursor()
        {
            foreach (GameObject gameObject in m_instantiatedUIs.Values)
            {
                if (!gameObject || !gameObject.activeInHierarchy)
                    continue;

                OverhaulUIBehaviour behaviour = gameObject.GetComponent<OverhaulUIBehaviour>();
                if (behaviour && !behaviour.isElement && behaviour.enableCursor)
                    return true;
            }
            return false;
        }

        public bool ShouldEnableUIOverLogoMode()
        {
            foreach (GameObject gameObject in m_instantiatedUIs.Values)
            {
                if (!gameObject || !gameObject.activeInHierarchy)
                    continue;

                OverhaulUIBehaviour behaviour = gameObject.GetComponent<OverhaulUIBehaviour>();
                if (behaviour && !behaviour.isElement && behaviour.enableUIOverLogoMode)
                    return true;
            }
            return false;
        }

        public bool ShouldHideTitleScreen()
        {
            foreach (GameObject gameObject in m_instantiatedUIs.Values)
            {
                if (!gameObject || !gameObject.activeInHierarchy)
                    continue;

                OverhaulUIBehaviour behaviour = gameObject.GetComponent<OverhaulUIBehaviour>();
                if (behaviour && !behaviour.isElement && behaviour.hideTitleScreen)
                    return true;
            }
            return false;
        }

        internal void RemoveFromList(OverhaulUIBehaviour uIBehaviour)
        {
            _ = m_instantiatedUIs.Remove(uIBehaviour.uiName);
        }

        public enum UILayer
        {
            First,

            BeforeTitleScreen,
            AfterTitleScreen,

            BeforeEnergyUI,
            AfterEnergyUI,

            BeforeUpgradeUI,
            AfterUpgradeUI,

            BeforeMultiplayerConnectScreen,
            AfterMultiplayerConnectScreen,

            BeforeEscMenu,
            AfterEscMenu,

            BeforeCrashScreen,
            AfterCrashScreen,

            Last
        }
    }
}
