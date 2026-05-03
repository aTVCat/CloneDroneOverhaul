using OverhaulMod.Content.Personalization;
using System.Collections.Generic;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementPEItemAdditionalConfigurationPanel : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(OnHideBowStringsToggled))]
        [UIElement("HideBowStringsToggle")]
        private readonly Toggle _hideBowStrings;

        [UIElementAction(nameof(OnEditedOverrideParentDropdown))]
        [UIElement("OverrideParentDropdown")]
        private readonly Dropdown _overrideParentDropdown;

        [UIElementAction(nameof(OnEditedBowStringsWidth))]
        [UIElement("BowStringsWidthSlider")]
        private readonly Slider _bowStringsWidth;

        [UIElement("ExclusiveForField", typeof(UIElementPEExclusivityField))]
        private readonly UIElementPEExclusivityField _exclusiveForField;

        private bool _disallowCallbacks;

        public PersonalizationItemInfo EditingItemInfo
        {
            get => PersonalizationEditorManager.Instance.EditingItemInfo;
        }

        protected override void OnInitialized()
        {
            List<Dropdown.OptionData> overrideParentList = _overrideParentDropdown.options;
            overrideParentList.Clear();
            overrideParentList.Add(new DropdownStringOptionData()
            {
                text = "Default",
                StringValue = null,
            });
            overrideParentList.Add(new DropdownStringOptionData()
            {
                text = "Left hand",
                StringValue = "HandL",
            });
            overrideParentList.Add(new DropdownStringOptionData()
            {
                text = "Right hand",
                StringValue = "HandR",
            });
            _overrideParentDropdown.RefreshShownValue();
        }

        public void Populate()
        {
            PersonalizationItemInfo itemInfo = EditingItemInfo;
            if (itemInfo == null) return;

            bool isBow = itemInfo.Category == PersonalizationCategory.WeaponSkins && itemInfo.Weapon == WeaponType.Bow;

            _disallowCallbacks = true;

            _hideBowStrings.isOn = itemInfo.HideBowStrings;
            _hideBowStrings.interactable = isBow;

            _overrideParentDropdown.interactable = isBow;

            _bowStringsWidth.value = itemInfo.BowStringsWidth;
            _bowStringsWidth.interactable = isBow;

            for (int i = 0; i < _overrideParentDropdown.options.Count; i++)
            {
                if ((_overrideParentDropdown.options[i] as DropdownStringOptionData).StringValue == itemInfo.OverrideParent)
                {
                    _overrideParentDropdown.value = i;
                    break;
                }
            }

            _exclusiveForField.referenceList = itemInfo.ExclusiveFor_V2;

            _disallowCallbacks = false;
        }

        public void ApplyValues()
        {
            PersonalizationItemInfo itemInfo = EditingItemInfo;
            if (itemInfo == null) return;
        }

        public void OnHideBowStringsToggled(bool value)
        {
            if (_disallowCallbacks)
                return;

            PersonalizationItemInfo personalizationItemInfo = EditingItemInfo;
            if (personalizationItemInfo == null)
                return;

            personalizationItemInfo.HideBowStrings = value;
        }

        public void OnEditedOverrideParentDropdown(int value)
        {
            if (_disallowCallbacks)
                return;

            PersonalizationItemInfo personalizationItemInfo = EditingItemInfo;
            if (personalizationItemInfo == null)
                return;

            personalizationItemInfo.OverrideParent = (_overrideParentDropdown.options[value] as DropdownStringOptionData).StringValue;

            PersonalizationEditorManager manager = PersonalizationEditorManager.Instance;
            manager.SerializeRoot();
            manager.SpawnRootObject();
            UIPE.Instance.Inspector.EditObjectAgain();
        }

        public void OnEditedBowStringsWidth(float value)
        {
            if (_disallowCallbacks)
                return;

            PersonalizationItemInfo personalizationItemInfo = EditingItemInfo;
            if (personalizationItemInfo == null)
                return;

            personalizationItemInfo.BowStringsWidth = value;

            PersonalizationController controller = PersonalizationEditorManager.Instance.PreviewingPersonalizationController;
            if (controller)
                controller.SetBowStringsWidth(value);
        }
    }
}