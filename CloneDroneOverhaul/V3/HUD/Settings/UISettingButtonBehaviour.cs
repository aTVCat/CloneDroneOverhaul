using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace CloneDroneOverhaul.V3.HUD
{
    public class UISettingCategoryButtonBehaviour : MonoBehaviour
    {
        private HUD.UIModSettings _ui;

        private string _category;

        private Text _header;
        private Transform _selectedBG;

        public UISettingCategoryButtonBehaviour Initialize(in string categoryName, in HUD.UIModSettings ui)
        {
            ModdedObject component = base.GetComponent<ModdedObject>();
            _header = component.GetObjectFromList<Text>(1);
            _selectedBG = component.GetObjectFromList<Transform>(0);
            _ui = ui;
            _category = categoryName;

            GetComponent<Button>().onClick.AddListener(Select);
            _header.text = _category;

            SetSelected(false);

            return this;
        }

        public void Select()
        {
            _ui.SelectCategory(_category);
            SetSelected(true);
        }

        public void SetSelected(in bool value)
        {
            _selectedBG.gameObject.SetActive(value);
        }
    }
}
