using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    /// <summary>
    /// Implement the tab feature into UI
    /// </summary>
    public class TabManager
    {
        private GameObject _prefab;
        private Transform _container;
        private Type _type;
        private Action<UIElementTab> _onTabCreate;
        private Action<UIElementTab> _onTabSelect;

        private Dictionary<string, UIElementTab> _instantiatedTabs;

        public string[] PreconfiguredTabs;

        public UIElementTab PreviousSelectedTab
        {
            get;
            private set;
        }

        public UIElementTab SelectedTab
        {
            get;
            private set;
        }

        public bool IsInteractable { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="container"></param>
        /// <param name="onTabCreate">Called when tab was instantiated</param>
        /// <param name="onTabSelect">Called when user selects a tab</param>
        public void Config(GameObject prefab, Transform container, Type type, Action<UIElementTab> onTabCreate, Action<UIElementTab> onTabSelect)
        {
            if (prefab)
                prefab.gameObject.SetActive(false);

            _prefab = prefab;
            _container = container;
            _onTabCreate = onTabCreate;
            _onTabSelect = onTabSelect;
            _type = type;
            IsInteractable = true;

            _instantiatedTabs = new Dictionary<string, UIElementTab>();
        }

        public void ReinstantiatePreconfiguredTabs()
        {
            Clear();
            string[] tabs = PreconfiguredTabs;
            if (!tabs.IsNullOrEmpty())
                foreach (string tab in tabs)
                    AddTab(tab);
        }

        public void Clear()
        {
            if (!_container)
                return;

            if (_container.childCount != 0)
                TransformUtils.DestroyAllChildren(_container);

            _instantiatedTabs.Clear();
        }

        public void AddTab(string tabId)
        {
            if (!_prefab || _instantiatedTabs.ContainsKey(tabId))
                return;

            AddTab(UnityEngine.Object.Instantiate(_prefab, _container), tabId);
        }

        public void AddTab(GameObject moddedObject, string tabId)
        {
            if (_instantiatedTabs.ContainsKey(tabId))
                return;

            moddedObject.SetActive(true);
            UIElementTab tab = (UIElementTab)moddedObject.AddComponent(_type);
            tab.tabId = tabId;
            callOnTabCreateMethod(tab);
            tab.InitializeElement();
            Button button = tab.GetButton();
            button.interactable = IsInteractable;
            button.onClick.AddListener(delegate
            {
                UIElementTab oldTab = SelectedTab;
                if (!IsInteractable || oldTab == tab)
                    return;

                PreviousSelectedTab = oldTab;
                SelectedTab = tab;
                if (oldTab)
                {
                    oldTab.OnTabDeselected();
                    oldTab.GetButton().interactable = IsInteractable;
                }

                tab.OnTabSelected();
                tab.GetButton().interactable = false;
                callOnTabSelectMethod(tab);
            });
            _instantiatedTabs.Add(tabId, tab);
        }

        public bool HasTab(string tabId)
        {
            return _instantiatedTabs.ContainsKey(tabId);
        }

        public void SelectTab(string tabId)
        {
            if (_instantiatedTabs.TryGetValue(tabId, out UIElementTab tab))
                tab.GetButton().OnPointerClick(new UnityEngine.EventSystems.PointerEventData(null));
        }

        public void DeselectAllTabs()
        {
            PreviousSelectedTab = null;
            SelectedTab = null;
            foreach (UIElementTab tab in _instantiatedTabs.Values)
                if (tab)
                    tab.GetButton().interactable = true;
        }

        private void callOnTabCreateMethod(UIElementTab tab)
        {
            _onTabCreate?.Invoke(tab);
        }

        private void callOnTabSelectMethod(UIElementTab tab)
        {
            _onTabSelect?.Invoke(tab);
        }
    }
}
