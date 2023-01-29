using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class SettingsMenuCategoryEntryBehaviour : MonoBehaviour
    {
        private static readonly List<SettingsMenuCategoryEntryBehaviour> _spawnedBehaviours = new List<SettingsMenuCategoryEntryBehaviour>();

        private ModdedObject _moddedObject;
        private UISettingsMenu _ui;

        public string Category;

        public void Initialize(in UISettingsMenu menu, in ModdedObject moddedObject, in string categoryName)
        {
            _moddedObject = moddedObject;
            _ui = menu;
            Category = categoryName;
            base.GetComponent<Button>().onClick.AddListener(select);

            _spawnedBehaviours.Add(this);

            SetSelected(false, false);
        }

        public void SetSelected(in bool value, in bool checkOthers = true)
        {
            if (checkOthers)
            {
                foreach (SettingsMenuCategoryEntryBehaviour b in _spawnedBehaviours)
                {
                    if (b != this)
                    {
                        b.SetSelected(false, false);
                    }
                }
            }
            _moddedObject.GetObject<Transform>(2).gameObject.SetActive(value);
            if (value)
            {
                _ui.PopulateCategory(Category);
            }
        }

        private void select()
        {
            SetSelected(true);
        }

        private void OnDestroy()
        {
            if (!_spawnedBehaviours.Contains(this))
            {
                return;
            }
            _spawnedBehaviours.Remove(this);
        }
    }
}