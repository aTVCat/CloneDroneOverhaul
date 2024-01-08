using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    /// <summary>
    /// Implement the tab feature into UI
    /// </summary>
    public class TabManager
    {
        private ModdedObject m_prefab;
        private Transform m_container;
        private Type m_type;
        private Action<UIElementTab> m_onTabCreate;
        private Action<UIElementTab> m_onTabSelect;

        private Dictionary<string, UIElementTab> m_instantiatedTabs;

        public UIElementTab selectedTab
        {
            get;
            private set;
        }

        private bool m_interactable;
        public bool interactable
        {
            get
            {
                return m_interactable;
            }
            set
            {
                m_interactable = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="container"></param>
        /// <param name="onTabCreate">Called when tab was instantiated</param>
        /// <param name="onTabSelect">Called when user selects a tab</param>
        public void Config(ModdedObject prefab, Transform container, Type type, Action<UIElementTab> onTabCreate, Action<UIElementTab> onTabSelect)
        {
            if(prefab)
                prefab.gameObject.SetActive(false);

            m_prefab = prefab;
            m_container = container;
            m_onTabCreate = onTabCreate;
            m_onTabSelect = onTabSelect;
            m_type = type;
            m_interactable = true;

            m_instantiatedTabs = new Dictionary<string, UIElementTab>();
        }

        public void Clear()
        {
            if (!m_container)
                return;

            if (m_container.childCount != 0)
                TransformUtils.DestroyAllChildren(m_container);

            m_instantiatedTabs.Clear();
        }

        public void AddTab(string tabId)
        {
            if (!m_prefab || m_instantiatedTabs.ContainsKey(tabId))
                return;

            AddTab(UnityEngine.Object.Instantiate(m_prefab, m_container), tabId);
        }

        public void AddTab(ModdedObject moddedObject, string tabId)
        {
            if (m_instantiatedTabs.ContainsKey(tabId))
                return;

            moddedObject.gameObject.SetActive(true);
            UIElementTab tab = (UIElementTab)moddedObject.gameObject.AddComponent(m_type);
            tab.tabId = tabId;
            tab.InitializeElement();
            Button button = tab.GetButton();
            button.interactable = interactable;
            button.onClick.AddListener(delegate
            {
                UIElementTab oldTab = selectedTab;
                if (!interactable || oldTab == tab)
                    return;

                selectedTab = tab;
                if (oldTab)
                {
                    oldTab.OnTabDeselected();
                    oldTab.GetButton().interactable = interactable;
                }

                tab.OnTabSelected();
                tab.GetButton().interactable = false;
                callOnTabSelectMethod(tab);
            });
            callOnTabCreateMethod(tab);
            m_instantiatedTabs.Add(tabId, tab);
        }

        public void SelectTab(string tabId)
        {
            if (m_instantiatedTabs.TryGetValue(tabId, out UIElementTab tab))
                tab.GetButton().Press();
        }

        private void callOnTabCreateMethod(UIElementTab tab)
        {
            m_onTabCreate?.Invoke(tab);
        }

        private void callOnTabSelectMethod(UIElementTab tab)
        {
            m_onTabSelect?.Invoke(tab);
        }
    }
}
