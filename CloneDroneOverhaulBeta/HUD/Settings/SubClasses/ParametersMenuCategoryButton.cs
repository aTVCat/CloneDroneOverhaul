using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class ParametersMenuCategoryButton : OverhaulBehaviour
    {
        private static readonly List<ParametersMenuCategoryButton> m_SpawnedBehaviours = new List<ParametersMenuCategoryButton>();

        private ModdedObject m_ModdedObject;
        private Button m_Button;
        private OverhaulParametersMenu m_UI;

        public string Category;
        private bool m_IsSelected;

        public static void SetSelectedSpecific(string category)
        {
            if (m_SpawnedBehaviours.IsNullOrEmpty())
            {
                return;
            }

            foreach(ParametersMenuCategoryButton b in m_SpawnedBehaviours)
            {
                if(b != null && !b.IsDisposedOrDestroyed() && !string.IsNullOrEmpty(b.Category) && b.Category.Equals(category))
                {
                    b.SetSelected(true, true);
                }
            }
        }

        public void Initialize(in OverhaulParametersMenu menu, in ModdedObject moddedObject, in string categoryName)
        {
            m_ModdedObject = moddedObject;
            m_UI = menu;
            m_Button = base.GetComponent<Button>();
            m_Button.onClick.AddListener(select);
            Category = categoryName;

            m_SpawnedBehaviours.Add(this);
            SetSelected(false, false);
        }

        protected override void OnDisposed()
        {
            _ = m_SpawnedBehaviours.Remove(this);
            m_ModdedObject = null;
            m_UI = null;
            Category = null;
        }

        /*
        private void Update()
        {
            if(IsDisposedOrDestroyed() || m_Button == null)
            {
                return;
            }

            m_Button.interactable = m_UI != null && !m_UI.IsDisposedOrDestroyed() && m_UI.AllowSwitchingCategories;
        }*/

        public void SetSelected(in bool value, in bool checkOthers = true)
        {
            m_IsSelected = value;
            if (checkOthers)
            {
                foreach (ParametersMenuCategoryButton b in m_SpawnedBehaviours)
                {
                    if (b != this)
                    {
                        b.SetSelected(false, false);
                    }
                }
            }
            m_ModdedObject.GetObject<Transform>(2).gameObject.SetActive(value);
            if (value)
            {
                m_UI.PopulateCategory(Category);
            }
        }

        private void select()
        {
            if (!m_UI.AllowSwitchingCategories || m_IsSelected)
            {
                return;
            }
            SetSelected(true);
        }
    }
}