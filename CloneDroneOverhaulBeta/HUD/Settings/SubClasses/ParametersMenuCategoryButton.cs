using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class ParametersMenuCategoryButton : OverhaulMonoBehaviour
    {
        private static readonly List<ParametersMenuCategoryButton> _spawnedBehaviours = new List<ParametersMenuCategoryButton>();

        private ModdedObject m_ModdedObject;
        private OverhaulParametersMenu m_UI;
        public string Category;

        public void Initialize(in OverhaulParametersMenu menu, in ModdedObject moddedObject, in string categoryName)
        {
            m_ModdedObject = moddedObject;
            m_UI = menu;
            Category = categoryName;
            base.GetComponent<Button>().onClick.AddListener(select);

            _spawnedBehaviours.Add(this);
            SetSelected(false, false);
        }

        protected override void OnDisposed()
        {
            _ = _spawnedBehaviours.Remove(this);
            m_ModdedObject = null;
            m_UI = null;
            Category = null;
        }

        public void SetSelected(in bool value, in bool checkOthers = true)
        {
            if (checkOthers)
            {
                foreach (ParametersMenuCategoryButton b in _spawnedBehaviours)
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
            SetSelected(true);
        }

        public static void SetSelectedFirst()
        {
            if (_spawnedBehaviours.IsNullOrEmpty())
            {
                return;
            }
            _spawnedBehaviours[0].SetSelected(true, false);
        }
    }
}