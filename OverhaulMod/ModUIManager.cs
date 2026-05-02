using OverhaulMod.Content;
using OverhaulMod.Engine;
using OverhaulMod.UI;
using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod
{
    public class ModUIManager : Singleton<ModUIManager>, IGameLoadListener
    {
        [ModSetting(ModSettingIDs.SHOW_CHAPTER_SELECTION_MENU_REWORK, true, ModSetting.Tags.UISetting)]
        public static bool ShowChapterSelectionMenuRework;

        [ModSetting(ModSettingIDs.SHOW_ENDLESS_MODE_MENU, true, ModSetting.Tags.UISetting)]
        public static bool ShowEndlessModeMenu;

        [ModSetting(ModSettingIDs.SHOW_CHALLENGES_MENU_REWORK, true, ModSetting.Tags.UISetting)]
        public static bool ShowChallengesMenuRework;

        [ModSetting(ModSettingIDs.SHOW_WORKSHOP_BROWSER_REWORK, true, ModSetting.Tags.UISetting)]
        public static bool ShowWorkshopBrowserRework;

        [ModSetting(ModSettingIDs.SHOW_ADVANCEMENTS_MENU_REWORK, true, ModSetting.Tags.UISetting)]
        public static bool ShowAdvancementsMenuRework;

        [ModSetting(ModSettingIDs.SHOW_SETTINGS_MENU_REWORK, true, ModSetting.Tags.UISetting)]
        public static bool ShowSettingsMenuRework;

        [ModSetting(ModSettingIDs.SHOW_TITLE_SCREEN_REWORK, true, ModSetting.Tags.UISetting)]
        public static bool ShowTitleScreenRework;

        [ModSetting(ModSettingIDs.SHOW_DUEL_INVITE_MENU_REWORK, true, ModSetting.Tags.UISetting)]
        public static bool ShowDuelInviteMenuRework;

        [ModSetting(ModSettingIDs.CURSOR_SKIN, 1)]
        public static int CursorSkin;

        private Dictionary<string, GameObject> _instantiatedUIs;

        private List<OverhaulUIBehaviour> _shownUIs;

        public bool SkipHidingCustomUIs
        {
            get;
            private set;
        }

        public Action ActionToInvoke
        {
            get;
            private set;
        }

        public override void Awake()
        {
            base.Awake();
            _instantiatedUIs = new Dictionary<string, GameObject>();
            _shownUIs = new List<OverhaulUIBehaviour>();

            if (Time.timeSinceLevelLoad < 3f)
                ModUIs.ShowIntro();
        }

        private void Start()
        {
            RefreshCursor();

            if (AddonManager.Instance.HasInstalledAddon(AddonManager.EXTRAS_ADDON_ID, out string path))
            {
                ModResources.LoadBundleAsync(ModAssetBundles.UI_EXTRA, null, path);
            }
        }

        private void OnDestroy()
        {
            Dictionary<string, GameObject> dictionary = _instantiatedUIs;
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
            foreach (KeyValuePair<string, GameObject> keyValue in _instantiatedUIs)
            {
                if (!keyValue.Value)
                    keysToRemove.Add(keyValue.Key);
            }
            foreach (string key in keysToRemove)
                _ = _instantiatedUIs.Remove(key);

            _ = ModUIs.ShowVersionLabel();
            _ = ModUIs.ShowCinematicEffects();
            _ = ModUIs.ShowSubtitleTextFieldRework();
        }

        public bool HasInstantiatedUI(string assetKey)
        {
            return _instantiatedUIs.ContainsKey(assetKey);
        }

        public bool IsVisible(string assetBundle, string assetKey)
        {
            string fullName = $"{assetBundle}.{assetKey}";
            return HasInstantiatedUI(fullName) && _instantiatedUIs[fullName].activeInHierarchy;
        }

        public int GetSiblingIndex(UILayer layer)
        {
            switch (layer)
            {
                case UILayer.Last:
                    return ModCache.UIRootTransform.childCount;
                case UILayer.BeforeTitleScreen:
                    return ModCache.TitleScreenUI.transform.GetSiblingIndex();
                case UILayer.AfterTitleScreen:
                    return ModCache.TitleScreenUI.transform.GetSiblingIndex() + 1;
                case UILayer.BeforeEnergyUI:
                    return ModCache.UIRoot.EnergyUI.transform.GetSiblingIndex();
                case UILayer.AfterEnergyUI:
                    return ModCache.UIRoot.EnergyUI.transform.GetSiblingIndex() + 1;
                case UILayer.BeforeUpgradeUI:
                    return ModCache.UIRoot.UpgradeUI.transform.GetSiblingIndex();
                case UILayer.AfterUpgradeUI:
                    return ModCache.UIRoot.UpgradeUI.transform.GetSiblingIndex() + 1;
                case UILayer.BeforeEscMenu:
                    return ModCache.UIRoot.EscMenu.transform.GetSiblingIndex();
                case UILayer.AfterEscMenu:
                    return ModCache.UIRoot.EscMenu.transform.GetSiblingIndex() + 1;
                case UILayer.BeforeCrashScreen:
                    return ModCache.UIRoot.ErrorWindow.transform.GetSiblingIndex();
                case UILayer.AfterCrashScreen:
                    return ModCache.UIRoot.ErrorWindow.transform.GetSiblingIndex() + 1;
                case UILayer.BeforeMultiplayerConnectScreen:
                    return ModCache.UIRoot.MultiplayerConnectingScreen.transform.GetSiblingIndex();
                case UILayer.AfterMultiplayerConnectScreen:
                    return ModCache.UIRoot.MultiplayerConnectingScreen.transform.GetSiblingIndex() + 1;
            }
            return 0;
        }

        public T Show<T>(string assetBundle, string assetKey, UILayer layer = UILayer.Last, int siblingIndexOffset = 0) where T : OverhaulUIBehaviour
        {
            string fullName = $"{assetBundle}.{assetKey}";
            if (!HasInstantiatedUI(fullName))
            {
                GameObject prefab = ModResources.Prefab(assetBundle, assetKey);
                GameObject gameObject = Instantiate(prefab, ModCache.UIRootTransform);
                gameObject.SetActive(true);
                _instantiatedUIs.Add(fullName, gameObject);

                adjustUITransform(gameObject.transform as RectTransform);
                gameObject.transform.SetSiblingIndex(GetSiblingIndex(layer) + siblingIndexOffset);

                T result1 = gameObject.AddComponent<T>();
                result1.Name = fullName;

                Stopwatch stopwatch = Stopwatch.StartNew();
                result1.InitializeAsUI();
                stopwatch.Stop();
                ModDebug.Log($"Initialized {assetBundle}.{assetKey} in {stopwatch.ElapsedMilliseconds} ms, {stopwatch.ElapsedTicks} ticks");

                result1.Show();

                if (result1.CloseOnEscapeButtonPress)
                    _shownUIs.Add(result1);

                return result1;
            }
            else
            {
                RectTransform uiTransform = _instantiatedUIs[fullName].transform as RectTransform;
                if (uiTransform.parent != ModCache.UIRootTransform) uiTransform.SetParent(ModCache.UIRootTransform, false);

                adjustUITransform(uiTransform);
                uiTransform.SetSiblingIndex(GetSiblingIndex(layer) + siblingIndexOffset);
            }

            T result = _instantiatedUIs[fullName].GetComponent<T>();
            result.Show();

            if (result.CloseOnEscapeButtonPress)
                _shownUIs.Add(result);

            return result;
        }

        private void adjustUITransform(RectTransform transform)
        {
            transform.anchorMin = Vector2.zero;
            transform.anchorMax = Vector2.one;
            transform.sizeDelta = Vector2.zero;
            transform.anchoredPosition = Vector2.zero;
            transform.localScale = Vector3.one;
        }

        public T Show<T>(string assetBundle, string assetKey, Transform parent) where T : OverhaulUIBehaviour
        {
            T result = Show<T>(assetBundle, assetKey, UILayer.Last);
            if (parent && result.transform.parent != parent) result.transform.SetParent(parent);
            result.transform.SetAsLastSibling();
            return result;
        }

        public T Get<T>(string assetBundle, string assetKey) where T : OverhaulUIBehaviour
        {
            string fullName = $"{assetBundle}.{assetKey}";
            return _instantiatedUIs.TryGetValue(fullName, out GameObject gameObject) ? (gameObject?.GetComponent<T>()) : null;
        }

        public bool Hide(string assetBundle, string assetKey)
        {
            OverhaulUIBehaviour overhaulUIBehaviour = Get<OverhaulUIBehaviour>(assetBundle, assetKey);
            if (overhaulUIBehaviour && overhaulUIBehaviour.IsVisible && !overhaulUIBehaviour.ForceCancelHide)
            {
                overhaulUIBehaviour.Hide();
                return true;
            }
            return false;
        }

        public OverhaulUIBehaviour GetLastShownUI()
        {
            if (_shownUIs.Count == 0)
                return null;

            return _shownUIs[_shownUIs.Count - 1];
        }

        public void RemoveUIFromLastShown(OverhaulUIBehaviour behaviour)
        {
            _ = _shownUIs.Remove(behaviour);
        }

        public void RefreshUI(bool refreshOnlyCursor)
        {
            ModCache.UIRoot.RefreshCursorEnabled();
            if (!refreshOnlyCursor)
            {
                ModCache.UIRoot.SetUIOverLogoModeEnabled(ShouldEnableUIOverLogoMode());
                ModCache.TitleScreenUI.setLogoAndRootButtonsVisible(GameModeManager.IsOnTitleScreen() && !ShouldHideTitleScreen());
            }
        }

        public void InvokeActionInsteadOfHidingCustomUI(Action action)
        {
            ActionToInvoke = action;
        }

        public bool TryInvokeAction()
        {
            if (ActionToInvoke != null)
            {
                ActionToInvoke.Invoke();
                ActionToInvoke = null;
                return true;
            }
            return false;
        }

        public bool ShouldEnableCursor()
        {
            foreach (GameObject gameObject in _instantiatedUIs.Values)
            {
                if (!gameObject || !gameObject.activeInHierarchy)
                    continue;

                OverhaulUIBehaviour behaviour = gameObject.GetComponent<OverhaulUIBehaviour>();
                if (behaviour && !behaviour.IsElement && behaviour.EnableCursor)
                    return true;
            }
            return false;
        }

        public bool ShouldEnableUIOverLogoMode()
        {
            foreach (GameObject gameObject in _instantiatedUIs.Values)
            {
                if (!gameObject || !gameObject.activeInHierarchy)
                    continue;

                OverhaulUIBehaviour behaviour = gameObject.GetComponent<OverhaulUIBehaviour>();
                if (behaviour && !behaviour.IsElement && behaviour.EnableUIOverLogoMode)
                    return true;
            }
            return false;
        }

        public bool ShouldHideTitleScreen()
        {
            foreach (GameObject gameObject in _instantiatedUIs.Values)
            {
                if (!gameObject || !gameObject.activeInHierarchy)
                    continue;

                OverhaulUIBehaviour behaviour = gameObject.GetComponent<OverhaulUIBehaviour>();
                if (behaviour && !behaviour.IsElement && behaviour.HideTitleScreen)
                    return true;
            }
            return false;
        }

        internal void RemoveFromList(OverhaulUIBehaviour uIBehaviour)
        {
            _ = _instantiatedUIs.Remove(uIBehaviour.Name);
        }

        public void RefreshUIVisibility()
        {
            if (!BoltNetwork.IsRunning)
                return;

            bool shouldDisplay = !SettingsManager.Instance.ShouldHideGameUI();
            GameUIRoot gameUIRoot = ModCache.UIRoot;

            if (GameModeManager.IsBattleRoyale())
            {
                gameUIRoot.BattleRoyaleUI.refreshVisibility();
                gameUIRoot.BattleRoyaleUI.HumansLeftUIRoot.SetActive(shouldDisplay);

                if (shouldDisplay)
                    gameUIRoot.KillFeedUI.Show();
                else
                    gameUIRoot.KillFeedUI.Hide();
            }

            Character character = CharacterTracker.Instance.GetPlayer();
            gameUIRoot.SetPlayerHUDVisible(shouldDisplay && character && character.IsAttachedAndAlive() && !CutSceneManager.Instance.IsInCutscene());
        }

        public static void RefreshCursor()
        {
            switch (CursorSkin)
            {
                case 1:
                    Cursor.SetCursor(ModResources.Texture2D(ModAssetBundles.UI, "Cursor"), Vector2.zero, CursorMode.Auto);
                    break;
                case 2:
                    Cursor.SetCursor(ModResources.Texture2D(ModAssetBundles.UI, "Cursor2"), Vector2.zero, CursorMode.Auto);
                    break;
                default:
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    break;
            }
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
