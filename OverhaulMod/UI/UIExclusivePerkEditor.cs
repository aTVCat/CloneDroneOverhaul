using OverhaulMod.Content;
using OverhaulMod.Utils;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIExclusivePerkEditor : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _exitButton;

        [UIElement("PerksPanel", false)]
        private readonly GameObject _perksPanel;

        [UIElement("EditorBG", false)]
        private readonly GameObject _editorBg;

        [UIElement("NonEditorBG", true)]
        private readonly GameObject _nonEditorBg;

        [UIElementAction(nameof(OnPerksButtonClicked))]
        [UIElement("PerksButton")]
        private readonly Button _perksButton;

        [UIElementAction(nameof(OnSaveButtonClicked))]
        [UIElement("SaveButton")]
        private readonly Button _saveButton;

        [UIElementAction(nameof(OnDeleteButtonClicked))]
        [UIElement("DeleteButton")]
        private readonly Button _deleteButton;

        [UIElementAction(nameof(OnSavesFolderButtonClicked))]
        [UIElement("SavesFolderButton")]
        private readonly Button _savesFolderButton;

        [UIElementAction(nameof(OnSetSelfButtonClicked))]
        [UIElement("SetSelfButton")]
        private readonly Button _setSelfButton;

        [UIElement("NeedsSaveIcon", false)]
        private readonly GameObject _needsSaveIcon;

        [UIElementAction(nameof(OnClosePerksPanelButtonClicked))]
        [UIElement("ClosePerksPanelButton")]
        private readonly Button _closePerksPanelButton;

        [UIElementAction(nameof(OnNewPerkButtonClicked))]
        [UIElement("NewPerkButton")]
        private readonly Button _newPerkButton;

        [UIElement("PerkDisplay", false)]
        private readonly ModdedObject _perkDisplay;

        [UIElement("Content")]
        private readonly Transform _perkPanelContent;

        [UIElementAction(nameof(OnPerkNameEdited))]
        [UIElement("PerkNameField")]
        private readonly InputField _perkNameField;

        [UIElementAction(nameof(OnPerkTypeDropdownEdited))]
        [UIElement("PerkTypeDropdown")]
        private readonly Dropdown _perkTypeDropdown;

        [UIElementAction(nameof(OnOwnerPlayfabIDEdited))]
        [UIElement("OwnerPlayfabID")]
        private readonly InputField _ownerPlayfabIDField;

        [UIElementAction(nameof(OnRevealOwnerPlayfabIDButtonClicked))]
        [UIElement("RevealOnwerPlayfabIDButton", true)]
        private readonly Button _revealOwnerPlayfabIDButton;

        [UIElementAction(nameof(OnOwnerSteamIDEdited))]
        [UIElement("OwnerSteamID")]
        private readonly InputField _ownerSteamIDField;

        [UIElementAction(nameof(OnRevealOwnerSteamIDButtonClicked))]
        [UIElement("RevealOnwerSteamIDButton", true)]
        private readonly Button _revealOwnerSteamIDButton;

        [UIElementAction(nameof(OnEditIconButtonClicked))]
        [UIElement("EditPerkIconButton")]
        private readonly Button _editIconButton;

        [UIElementAction(nameof(OnClosePerkIconsPanelButtonClicked))]
        [UIElement("CloseIconsPanelButton")]
        private readonly Button _closePerkIconsButton;

        [UIElement("IconsPanel", false)]
        private readonly GameObject _perkIconsPanel;

        [UIElement("PerkIconDisplay", false)]
        private readonly ModdedObject _perkIconDisplay;

        [UIElement("PerkIconsContent")]
        private readonly Transform _perkIconsPanelContent;

        [UIElement("PerkIcon")]
        private readonly Image _perkIcon;

        [UIElement("ExclusiveColorPerkSettings", false)]
        private readonly GameObject _exclusiveColorPerkSettingsObject;

        [UIElement("ECColorToReplaceDropdown")]
        private readonly Dropdown _ecColorToReplaceDropdown;

        [ColorPicker(true)]
        [UIElement("ECNewColorButton")]
        private readonly UIElementColorPickerButton _ecNewColorButton;

        private ExclusivePerkInfo _editingPerk;

        private bool _disableUICallbacks;

        protected override void OnInitialized()
        {
            _saveButton.interactable = false;

            _ecColorToReplaceDropdown.options = HumanFactsManager.Instance.GetColorDropdownOptions();
            _ecNewColorButton.color = Color.white;

            System.Collections.Generic.List<Dropdown.OptionData> options = _perkTypeDropdown.options;
            options.Clear();
            foreach (object enumValue in typeof(ExclusivePerkType).GetEnumValues())
            {
                ExclusivePerkType exclusivePerkType = (ExclusivePerkType)enumValue;
                options.Add(new DropdownIntOptionData() { text = StringUtils.AddSpacesToCamelCasedString(exclusivePerkType.ToString()), IntValue = (int)enumValue });
            }
            _perkTypeDropdown.RefreshShownValue();
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }

        private void editPerk(ExclusivePerkInfo exclusivePerkInfo)
        {
            _editingPerk = exclusivePerkInfo;

            setFieldsValue(exclusivePerkInfo);

            _revealOwnerPlayfabIDButton.gameObject.SetActive(true);
            _revealOwnerSteamIDButton.gameObject.SetActive(true);

            if (_perksPanel.activeSelf)
                togglePerksPanel();

            _editorBg.SetActive(true);
            _nonEditorBg.SetActive(false);
            _saveButton.interactable = true;

            refreshSettings();
        }

        private void saveEditingPerk()
        {
            _needsSaveIcon.SetActive(false);

            ExclusivePerkInfoList infoList = ExclusivePerkManager.Instance.GetPerkInfoList();
            if (infoList != null)
            {
                ExclusivePerkInfo perk = _editingPerk;
                if (perk != null)
                {
                    updatePerkInfo(perk);
                    perk.SerializeData();

                    writeData();
                }
                else
                {
                    ModUIUtils.MessagePopupOK("Error", "You're not editing any perk.", true);
                }
            }
            else
            {
                ModUIUtils.MessagePopupOK("Error", "Perk list is not available.\nThis is a bug.", true);
            }
        }

        private void writeData()
        {
            ExclusivePerkInfoList infoList = ExclusivePerkManager.Instance.GetPerkInfoList();
            ModJsonUtils.WriteStream(Path.Combine(ModCore.modUserDataFolder, ExclusivePerkManager.FILE_NAME), infoList);
            ModJsonUtils.WriteStream(Path.Combine(ModCore.savesFolder, ExclusivePerkManager.FILE_NAME), infoList);
        }

        private void refreshSettings()
        {
            _exclusiveColorPerkSettingsObject.SetActive(_editingPerk.PerkType == ExclusivePerkType.Color);
        }

        private void setFieldsValue(ExclusivePerkInfo perk)
        {
            _disableUICallbacks = true;

            _perkNameField.text = perk.DisplayName;
            for (int i = 0; i < _perkTypeDropdown.options.Count; i++)
            {
                DropdownIntOptionData dropdownIntOptionData = _perkTypeDropdown.options[i] as DropdownIntOptionData;
                if (dropdownIntOptionData.IntValue == (int)perk.PerkType)
                {
                    _perkTypeDropdown.value = i;
                    break;
                }
            }
            _ownerPlayfabIDField.text = perk.PlayFabID;
            _ownerSteamIDField.text = perk.SteamID.ToString();

            _perkIcon.sprite = perk.Icon.IsNullOrEmpty() ? null : ModResources.Sprite(AssetBundleConstants.PERK_ICONS, perk.Icon);

            object data = perk.DeserializeData();
            if (data == null)
            {
                perk.SetDefaultData();
                data = perk.DeserializeData();
            }

            switch (perk.PerkType)
            {
                case ExclusivePerkType.Color:
                    ExclusivePerkColor ec = (ExclusivePerkColor)perk.DeserializeData();
                    _ecNewColorButton.color = ec.NewColor;
                    _ecColorToReplaceDropdown.value = ec.Index + 1;
                    break;
            }

            _disableUICallbacks = false;
        }

        private void updatePerkInfo(ExclusivePerkInfo perk)
        {
            perk.DisplayName = _perkNameField.text;
            perk.PerkType = (ExclusivePerkType)(_perkTypeDropdown.options[_perkTypeDropdown.value] as DropdownIntOptionData).IntValue;
            perk.PlayFabID = _ownerPlayfabIDField.text;

            object data = perk.DeserializeData();
            if (data == null)
            {
                perk.SetDefaultData();
                data = perk.DeserializeData();
            }

            switch (perk.PerkType)
            {
                case ExclusivePerkType.Color:
                    ExclusivePerkColor ec = (ExclusivePerkColor)data;
                    ec.NewColor = _ecNewColorButton.color;
                    ec.Index = _ecColorToReplaceDropdown.value - 1;
                    break;
            }

            if (_ownerSteamIDField.text.IsNullOrEmpty())
                perk.SteamID = 0;
            else if (!ulong.TryParse(_ownerSteamIDField.text, out perk.SteamID))
                ModUIUtils.MessagePopupOK("Warning", "Could not parse the Steam ID.\nMake sure it only has numbers", true);
        }

        private void togglePerksPanel()
        {
            if (!_perksPanel.activeSelf)
                populatePerksPanel();

            _perksPanel.SetActive(!_perksPanel.activeSelf);
        }

        private void populatePerksPanel()
        {
            if (_perkPanelContent.childCount != 0)
                TransformUtils.DestroyAllChildren(_perkPanelContent);

            ExclusivePerkInfoList infoList = ExclusivePerkManager.Instance.GetPerkInfoList();
            if (infoList != null)
            {
                for (int i = 0; i < infoList.List.Count; i++)
                {
                    ExclusivePerkInfo perk = infoList.List[i];
                    ModdedObject moddedObject = Instantiate(_perkDisplay, _perkPanelContent);
                    moddedObject.gameObject.SetActive(true);
                    moddedObject.GetObject<Text>(0).text = perk.DisplayName;
                    moddedObject.GetObject<Image>(1).sprite = perk.Icon.IsNullOrEmpty() ? null : ModResources.Sprite(AssetBundleConstants.PERK_ICONS, perk.Icon);

                    Button button = moddedObject.GetComponent<Button>();
                    button.onClick.AddListener(delegate
                    {
                        editPerk(perk);
                    });
                }
            }
            else
            {
                ModUIUtils.MessagePopupOK("Error", "Perk list is not available.\nThis is a bug.", true);
            }
        }

        public void OnPerksButtonClicked()
        {
            togglePerksPanel();
        }

        public void OnSaveButtonClicked()
        {
            saveEditingPerk();
        }

        public void OnClosePerksPanelButtonClicked()
        {
            togglePerksPanel();
        }

        public void OnClosePerkIconsPanelButtonClicked()
        {
            _perkIconsPanel.SetActive(false);
        }

        public void OnNewPerkButtonClicked()
        {
            ExclusivePerkInfoList infoList = ExclusivePerkManager.Instance.GetPerkInfoList();
            if (infoList != null)
            {
                ExclusivePerkInfo exclusivePerkInfo = new ExclusivePerkInfo();
                exclusivePerkInfo.SetDefaultData();
                infoList.List.Add(exclusivePerkInfo);
                editPerk(exclusivePerkInfo);
            }
            else
            {
                ModUIUtils.MessagePopupOK("Error", "Perk list is not available.\nThis is a bug.", true);
            }
        }

        public void OnEditIconButtonClicked()
        {
            _perkIconsPanel.SetActive(true);

            if (_perkIconsPanelContent.childCount != 0)
                return;

            AssetBundle assetBundle = ModResources.LoadAndGetAssetBundle(AssetBundleConstants.PERK_ICONS);
            foreach (Sprite sprite in assetBundle.LoadAllAssets<Sprite>())
            {
                ModdedObject moddedObject = Instantiate(_perkIconDisplay, _perkIconsPanelContent);
                moddedObject.gameObject.SetActive(true);
                moddedObject.GetObject<Image>(0).sprite = sprite;

                Button button = moddedObject.GetComponent<Button>();
                button.onClick.AddListener(delegate
                {
                    _editingPerk.Icon = sprite.name;
                    _perkIcon.sprite = sprite;
                    _needsSaveIcon.SetActive(true);
                    OnClosePerkIconsPanelButtonClicked();
                });
            }
        }

        public void OnRevealOwnerPlayfabIDButtonClicked()
        {
            _revealOwnerPlayfabIDButton.gameObject.SetActive(false);
        }

        public void OnRevealOwnerSteamIDButtonClicked()
        {
            _revealOwnerSteamIDButton.gameObject.SetActive(false);
        }

        public void OnPerkNameEdited(string text)
        {
            if (_disableUICallbacks)
                return;

            _needsSaveIcon.SetActive(true);
        }

        public void OnPerkTypeDropdownEdited(int value)
        {
            if (_disableUICallbacks)
                return;

            _needsSaveIcon.SetActive(true);
            _editingPerk.PerkType = (ExclusivePerkType)(_perkTypeDropdown.options[_perkTypeDropdown.value] as DropdownIntOptionData).IntValue;
            _editingPerk.SetDefaultData();
            refreshSettings();
        }

        public void OnOwnerPlayfabIDEdited(string text)
        {
            if (_disableUICallbacks)
                return;

            _needsSaveIcon.SetActive(true);
        }

        public void OnOwnerSteamIDEdited(string text)
        {
            if (_disableUICallbacks)
                return;

            _needsSaveIcon.SetActive(true);
        }

        public void OnDeleteButtonClicked()
        {
            if (_editingPerk == null)
                return;

            ModUIUtils.MessagePopup(true, $"Delete \"{_editingPerk.DisplayName}\"?", "yo", 125f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Yes", "No", null, delegate
            {
                ExclusivePerkInfoList infoList = ExclusivePerkManager.Instance.GetPerkInfoList();
                if (infoList != null)
                {
                    infoList.List.Remove(_editingPerk);
                    writeData();

                    OnPerksButtonClicked();
                }
                else
                {
                    ModUIUtils.MessagePopupOK("Error", "Perk list is not available.\nThis is a bug.", true);
                }
            });
        }

        public void OnSavesFolderButtonClicked()
        {
            ModFileUtils.OpenFileExplorer(ModCore.savesFolder);
        }

        public void OnSetSelfButtonClicked()
        {
            _ownerPlayfabIDField.text = ModUserInfo.localPlayerPlayFabID;
            _ownerSteamIDField.text = ModUserInfo.localPlayerSteamID.ToString();
        }
    }
}
