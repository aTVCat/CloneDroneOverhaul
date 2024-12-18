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
        private readonly Button m_exitButton;

        [UIElement("PerksPanel", false)]
        private readonly GameObject m_perksPanel;

        [UIElement("EditorBG", false)]
        private readonly GameObject m_editorBg;

        [UIElement("NonEditorBG", true)]
        private readonly GameObject m_nonEditorBg;

        [UIElementAction(nameof(OnPerksButtonClicked))]
        [UIElement("PerksButton")]
        private readonly Button m_perksButton;

        [UIElementAction(nameof(OnSaveButtonClicked))]
        [UIElement("SaveButton")]
        private readonly Button m_saveButton;

        [UIElement("NeedsSaveIcon", false)]
        private readonly GameObject m_needsSaveIcon;

        [UIElementAction(nameof(OnClosePerksPanelButtonClicked))]
        [UIElement("ClosePerksPanelButton")]
        private readonly Button m_closePerksPanelButton;

        [UIElementAction(nameof(OnNewPerkButtonClicked))]
        [UIElement("NewPerkButton")]
        private readonly Button m_newPerkButton;

        [UIElement("PerkDisplay", false)]
        private readonly ModdedObject m_perkDisplay;

        [UIElement("Content")]
        private readonly Transform m_perkPanelContent;

        [UIElementAction(nameof(OnPerkNameEdited))]
        [UIElement("PerkNameField")]
        private readonly InputField m_perkNameField;

        [UIElementAction(nameof(OnPerkTypeDropdownEdited))]
        [UIElement("PerkTypeDropdown")]
        private readonly Dropdown m_perkTypeDropdown;

        [UIElementAction(nameof(OnOwnerPlayfabIDEdited))]
        [UIElement("OwnerPlayfabID")]
        private readonly InputField m_ownerPlayfabIDField;

        [UIElementAction(nameof(OnRevealOwnerPlayfabIDButtonClicked))]
        [UIElement("RevealOnwerPlayfabIDButton", true)]
        private readonly Button m_revealOwnerPlayfabIDButton;

        [UIElementAction(nameof(OnOwnerSteamIDEdited))]
        [UIElement("OwnerSteamID")]
        private readonly InputField m_ownerSteamIDField;

        [UIElementAction(nameof(OnRevealOwnerSteamIDButtonClicked))]
        [UIElement("RevealOnwerSteamIDButton", true)]
        private readonly Button m_revealOwnerSteamIDButton;

        [UIElementAction(nameof(OnEditIconButtonClicked))]
        [UIElement("EditPerkIconButton")]
        private readonly Button m_editIconButton;

        [UIElementAction(nameof(OnClosePerkIconsPanelButtonClicked))]
        [UIElement("CloseIconsPanelButton")]
        private readonly Button m_closePerkIconsButton;

        [UIElement("IconsPanel", false)]
        private readonly GameObject m_perkIconsPanel;

        [UIElement("PerkIconDisplay", false)]
        private readonly ModdedObject m_perkIconDisplay;

        [UIElement("PerkIconsContent")]
        private readonly Transform m_perkIconsPanelContent;

        [UIElement("PerkIcon")]
        private readonly Image m_perkIcon;

        private ExclusivePerkInfo m_editingPerk;

        private bool m_disableUICallbacks;

        protected override void OnInitialized()
        {
            m_saveButton.interactable = false;

            System.Collections.Generic.List<Dropdown.OptionData> options = m_perkTypeDropdown.options;
            options.Clear();
            foreach (object enumValue in typeof(ExclusivePerkType).GetEnumValues())
            {
                ExclusivePerkType exclusivePerkType = (ExclusivePerkType)enumValue;
                options.Add(new DropdownIntOptionData() { text = StringUtils.AddSpacesToCamelCasedString(exclusivePerkType.ToString()), IntValue = (int)enumValue });
            }
            m_perkTypeDropdown.RefreshShownValue();
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
            m_editingPerk = exclusivePerkInfo;

            if (m_perksPanel.activeSelf)
                togglePerksPanel();

            setFieldsValue(exclusivePerkInfo);

            m_revealOwnerPlayfabIDButton.gameObject.SetActive(true);
            m_revealOwnerSteamIDButton.gameObject.SetActive(true);

            m_editorBg.SetActive(true);
            m_nonEditorBg.SetActive(false);
            m_saveButton.interactable = true;
        }

        private void saveEditingPerk()
        {
            m_needsSaveIcon.SetActive(false);

            ExclusivePerkInfoList infoList = ExclusivePerkManager.Instance.GetPerkInfoList();
            if (infoList != null)
            {
                ExclusivePerkInfo perk = m_editingPerk;
                if (perk != null)
                {
                    updatePerkInfo(perk);

                    ModJsonUtils.WriteStream(Path.Combine(ModCore.modUserDataFolder, ExclusivePerkManager.FILE_NAME), infoList);
                    ModJsonUtils.WriteStream(Path.Combine(ModCore.savesFolder, ExclusivePerkManager.FILE_NAME), infoList);
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

        private void setFieldsValue(ExclusivePerkInfo perk)
        {
            m_disableUICallbacks = true;

            m_perkNameField.text = perk.DisplayName;
            for (int i = 0; i < m_perkTypeDropdown.options.Count; i++)
            {
                DropdownIntOptionData dropdownIntOptionData = m_perkTypeDropdown.options[i] as DropdownIntOptionData;
                if (dropdownIntOptionData.IntValue == (int)perk.PerkType)
                {
                    m_perkTypeDropdown.value = i;
                    break;
                }
            }
            m_ownerPlayfabIDField.text = perk.PlayFabID;
            m_ownerSteamIDField.text = perk.SteamID.ToString();

            m_perkIcon.sprite = perk.Icon.IsNullOrEmpty() ? null : ModResources.Sprite(AssetBundleConstants.PERK_ICONS, perk.Icon);

            m_disableUICallbacks = false;
        }

        private void updatePerkInfo(ExclusivePerkInfo perk)
        {
            perk.DisplayName = m_perkNameField.text;
            perk.PerkType = (ExclusivePerkType)(m_perkTypeDropdown.options[m_perkTypeDropdown.value] as DropdownIntOptionData).IntValue;
            perk.PlayFabID = m_ownerPlayfabIDField.text;

            if (m_ownerSteamIDField.text.IsNullOrEmpty())
                perk.SteamID = 0;
            else if (!ulong.TryParse(m_ownerSteamIDField.text, out perk.SteamID))
                ModUIUtils.MessagePopupOK("Warning", "Could not parse the Steam ID.\nMake sure it only has numbers", true);
        }

        private void togglePerksPanel()
        {
            if (!m_perksPanel.activeSelf)
                populatePerksPanel();

            m_perksPanel.SetActive(!m_perksPanel.activeSelf);
        }

        private void populatePerksPanel()
        {
            if (m_perkPanelContent.childCount != 0)
                TransformUtils.DestroyAllChildren(m_perkPanelContent);

            ExclusivePerkInfoList infoList = ExclusivePerkManager.Instance.GetPerkInfoList();
            if (infoList != null)
            {
                for (int i = 0; i < infoList.List.Count; i++)
                {
                    ExclusivePerkInfo perk = infoList.List[i];
                    ModdedObject moddedObject = Instantiate(m_perkDisplay, m_perkPanelContent);
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
            m_perkIconsPanel.SetActive(false);
        }

        public void OnNewPerkButtonClicked()
        {
            ExclusivePerkInfoList infoList = ExclusivePerkManager.Instance.GetPerkInfoList();
            if (infoList != null)
            {
                ExclusivePerkInfo exclusivePerkInfo = new ExclusivePerkInfo();
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
            m_perkIconsPanel.SetActive(true);

            if (m_perkIconsPanelContent.childCount != 0)
                return;

            AssetBundle assetBundle = ModResources.LoadAndGetAssetBundle(AssetBundleConstants.PERK_ICONS);
            foreach (Sprite sprite in assetBundle.LoadAllAssets<Sprite>())
            {
                ModdedObject moddedObject = Instantiate(m_perkIconDisplay, m_perkIconsPanelContent);
                moddedObject.gameObject.SetActive(true);
                moddedObject.GetObject<Image>(0).sprite = sprite;

                Button button = moddedObject.GetComponent<Button>();
                button.onClick.AddListener(delegate
                {
                    m_editingPerk.Icon = sprite.name;
                    m_perkIcon.sprite = sprite;
                    m_needsSaveIcon.SetActive(true);
                    OnClosePerkIconsPanelButtonClicked();
                });
            }
        }

        public void OnRevealOwnerPlayfabIDButtonClicked()
        {
            m_revealOwnerPlayfabIDButton.gameObject.SetActive(false);
        }

        public void OnRevealOwnerSteamIDButtonClicked()
        {
            m_revealOwnerSteamIDButton.gameObject.SetActive(false);
        }

        public void OnPerkNameEdited(string text)
        {
            if (m_disableUICallbacks)
                return;

            m_needsSaveIcon.SetActive(true);
        }

        public void OnPerkTypeDropdownEdited(int value)
        {
            if (m_disableUICallbacks)
                return;

            m_needsSaveIcon.SetActive(true);
        }

        public void OnOwnerPlayfabIDEdited(string text)
        {
            if (m_disableUICallbacks)
                return;

            m_needsSaveIcon.SetActive(true);
        }

        public void OnOwnerSteamIDEdited(string text)
        {
            if (m_disableUICallbacks)
                return;

            m_needsSaveIcon.SetActive(true);
        }
    }
}
