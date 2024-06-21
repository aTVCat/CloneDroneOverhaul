using OverhaulMod.Content.Personalization;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPersonalizationItemsBrowser : OverhaulUIBehaviour
    {
        public const string ITEM_DISPLAY_DEFAULT_TEXT_COLOR = "#FFFFFF";
        public const string ITEM_DISPLAY_DEFAULT_TEXT_OUTLINE_COLOR = "#000000";
        public const string ITEM_DISPLAY_DEFAULT_TEXT_GLOW_COLOR = "#FFFFFF";

        public const string ITEM_DISPLAY_EXCLUSIVE_TEXT_COLOR = "#FFD058";
        public const string ITEM_DISPLAY_EXCLUSIVE_TEXT_OUTLINE_COLOR = "#A46300";
        public const string ITEM_DISPLAY_EXCLUSIVE_TEXT_GLOW_COLOR = "#FFC44D";

        public const string ITEM_DISPLAY_NONVERIFIED_TEXT_COLOR = "#FFFFFF";
        public const string ITEM_DISPLAY_NONVERIFIED_TEXT_OUTLINE_COLOR = "#00285B";
        public const string ITEM_DISPLAY_NONVERIFIED_TEXT_GLOW_COLOR = "#3E96FF";

        public const string ITEM_DISPLAY_NONVERIFIED_EXCLUSIVE_TEXT_COLOR = "#FFFFFF";
        public const string ITEM_DISPLAY_NONVERIFIED_EXCLUSIVE_TEXT_OUTLINE_COLOR = "#006A0D";
        public const string ITEM_DISPLAY_NONVERIFIED_EXCLUSIVE_TEXT_GLOW_COLOR = "#00F81F";

        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElement("Panel")]
        private readonly RectTransform m_panel;

        [UIElement("Panel", typeof(UIElementMouseEventsComponent))]
        private readonly UIElementMouseEventsComponent m_panelMouseEvents;

        [TabManager(typeof(UIElementTab), null, null, null, nameof(OnCategoryTabSelected))]
        private readonly TabManager m_categoryTabs;
        [UIElement("WeaponSkinsTab")]
        private readonly ModdedObject m_weaponSkinsTab;
        [UIElement("AccessoriesTab")]
        private readonly ModdedObject m_accessoriesTab;
        [UIElement("PetsTab")]
        private readonly ModdedObject m_petsTab;

        [TabManager(typeof(UIElementTabWithText), nameof(m_subCategoryTabPrefab), nameof(m_subCategoryTabsContainer), nameof(OnSubcategoryTabCreated), nameof(OnSubcategoryTabSelected))]
        private readonly TabManager m_subcategoryTabs;
        [UIElement("SubcategoryTabPrefab", false)]
        private readonly ModdedObject m_subCategoryTabPrefab;
        [UIElement("SubcategoryTabs")]
        private readonly Transform m_subCategoryTabsContainer;

        [UIElement("ItemDisplay", false)]
        private readonly ModdedObject m_itemDisplay;
        [UIElement("TextDisplay", false)]
        private readonly ModdedObject m_textDisplay;
        [UIElement("UtilsPanel", false)]
        private readonly ModdedObject m_utilsPanel;
        [UIElement("Content")]
        private readonly Transform m_container;
        [UIElement("Content")]
        private readonly CanvasGroup m_containerCanvasGroup;

        [UIElement("NotImplementedText", false)]
        private readonly GameObject m_notImplementedTextObject;

        [UIElementAction(nameof(OnSettingsButtonClicked))]
        [UIElement("SettingsButton")]
        private readonly Button m_settingsButton;

        [UIElementAction(nameof(OnUpdateButtonClicked))]
        [UIElement("UpdateButton")]
        private readonly Button m_updateButton;

        [UIElement("UpdateButtonText")]
        private readonly Text m_updateButtonText;

        [UIElement("ScrollRect")]
        private readonly RectTransform m_scrollRectTransform;

        [UIElement("DescriptionBox", typeof(UIElementPersonalizationItemDescriptionBox), false)]
        private readonly UIElementPersonalizationItemDescriptionBox m_descriptionBox;

        [UIElementAction(nameof(OnSearchBoxChanged))]
        [UIElement("SearchBox")]
        private readonly InputField m_searchBox;

        [UIElementAction(nameof(OnSortDropdownChanged))]
        [UIElement("SortDropdown")]
        private readonly Dropdown m_sortDropdown;

        private Dictionary<string, GameObject> m_cachedDisplays;

        private RectTransform m_rectTransform;

        private bool m_isOpen, m_isPopulating, m_showContents;

        private string m_prevTab;

        private PersonalizationCategory m_selectedCategory;

        private int m_sortType;

        private string m_selectedSubcategory;

        private readonly UnityWebRequest m_webRequest;

        private Button m_defaultSkinButton;

        private bool m_allowUICallbacks;

        public override bool enableCursor => true;

        protected override void OnInitialized()
        {
            m_cachedDisplays = new Dictionary<string, GameObject>();
            m_rectTransform = base.GetComponent<RectTransform>();

            m_categoryTabs.AddTab(m_weaponSkinsTab.gameObject, "weapon skins");
            m_categoryTabs.AddTab(m_accessoriesTab.gameObject, "accessories");
            m_categoryTabs.AddTab(m_petsTab.gameObject, "pets");
            m_categoryTabs.SelectTab("weapon skins");
            m_prevTab = "weapon skins";

            m_descriptionBox.SetBrowserUI(this);
            m_sortDropdown.value = 1;

            GlobalEventManager.Instance.AddEventListener(PersonalizationManager.CUSTOMIZATION_ASSETS_FILE_DOWNLOADED_EVENT, onCustomizationAssetsFileDownloaded);
            m_allowUICallbacks = true;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            GlobalEventManager.Instance.RemoveEventListener(PersonalizationManager.CUSTOMIZATION_ASSETS_FILE_DOWNLOADED_EVENT, onCustomizationAssetsFileDownloaded);
        }

        public override void Show()
        {
            base.Show();
            m_isOpen = true;
            m_showContents = true;
            m_categoryTabs.interactable = true;

            if (m_categoryTabs.selectedTab && m_prevTab != m_categoryTabs.selectedTab.tabId)
            {
                if (m_container.childCount != 0)
                    TransformUtils.DestroyAllChildren(m_container);

                m_categoryTabs.SelectTab(m_prevTab);
            }

            setCameraZoomedIn(true);
            ShowDownloadCustomizationAssetsDownloadMenuIfRequired();
            refreshUpdateButton();
            Populate();
        }

        public override void Hide()
        {
            base.Hide();
            m_isOpen = false;
            setCameraZoomedIn(false);
        }

        public override void Update()
        {
            base.Update();

            float a = m_containerCanvasGroup.alpha;
            a = Mathf.Clamp(a + (Time.unscaledDeltaTime * 12.5f * (m_showContents ? 1f : -1f)), 0f, 1f);
            m_containerCanvasGroup.alpha = a;
        }

        private void LateUpdate()
        {
            refreshCameraRect();
        }

        public override void OnEnable()
        {
            base.OnEnable();
            UIVersionLabel.instance.forceHide = true;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            m_isPopulating = false;
            UIVersionLabel.instance.forceHide = false;

            PersonalizationManager.Instance.userInfo.SaveIfDirty();
            ModSettingsDataManager.Instance.Save();
        }

        public bool IsMouseOverPanel()
        {
            return m_panelMouseEvents.isMouseOverElement;
        }

        public void ShowDescriptionBox(PersonalizationItemInfo itemInfo, RectTransform rectTransform)
        {
            m_descriptionBox.ShowForItem(itemInfo, rectTransform);
        }

        public void MakeDefaultSkinButtonInteractable()
        {
            Button button = m_defaultSkinButton;
            if (button)
            {
                button.interactable = true;
            }
        }

        public void ShowDownloadCustomizationAssetsDownloadMenuIfRequired()
        {
            if (PersonalizationManager.Instance.GetPersonalizationAssetsState() != PersonalizationAssetsState.Installed)
                ModUIConstants.ShowDownloadPersonalizationAssetsMenu(base.transform);
        }

        public void OnCategoryTabSelected(UIElementTab elementTab)
        {
            UIElementTab oldTab = m_categoryTabs.prevSelectedTab;
            UIElementTab newTab = m_categoryTabs.selectedTab;
            if (oldTab)
            {
                m_prevTab = oldTab.tabId;

                RectTransform rt = oldTab.transform as RectTransform;
                Vector2 vector = rt.sizeDelta;
                vector.y = 25f;
                rt.sizeDelta = vector;
            }
            if (newTab)
            {
                RectTransform rt = newTab.transform as RectTransform;
                Vector2 vector = rt.sizeDelta;
                vector.y = 30f;
                rt.sizeDelta = vector;
            }

            m_subcategoryTabs.Clear();
            if (newTab.tabId == "weapon skins")
            {
                m_selectedCategory = PersonalizationCategory.WeaponSkins;
                m_subcategoryTabs.AddTab("Sword");
                m_subcategoryTabs.AddTab("Bow");
                m_subcategoryTabs.AddTab("Hammer");
                m_subcategoryTabs.AddTab("Spear");
                m_subcategoryTabs.AddTab("Shield");

                m_subcategoryTabs.SelectTab("Sword");
            }
            else if (newTab.tabId == "accessories")
            {
                m_selectedCategory = PersonalizationCategory.Accessories;
            }
            else if (newTab.tabId == "pets")
            {
                m_selectedCategory = PersonalizationCategory.Pets;
            }
            else
            {
                m_selectedCategory = PersonalizationCategory.None;
            }

            Populate();
        }

        public void OnSubcategoryTabSelected(UIElementTab elementTab)
        {
            m_selectedSubcategory = elementTab.tabId;
            Populate();
        }

        public void OnSubcategoryTabCreated(UIElementTab elementTab)
        {
            UIElementTabWithText elementTabWithText = elementTab as UIElementTabWithText;
            elementTabWithText.LocalizationID = $"customization_subtab_{elementTab.tabId.ToLower()}";
        }

        public void Populate()
        {
            if (m_isPopulating || !base.enabled || !base.gameObject.activeInHierarchy)
                return;

            m_isPopulating = true;
            _ = base.StartCoroutine(populateCoroutine());
        }

        private IEnumerator populateCoroutine()
        {
            m_showContents = false;
            m_categoryTabs.interactable = false;
            m_subcategoryTabs.interactable = false;
            m_sortDropdown.interactable = false;

            RectTransform scrollRectTransform = m_scrollRectTransform;
            Vector2 sizeDelta = scrollRectTransform.sizeDelta;
            if (m_selectedCategory != PersonalizationCategory.WeaponSkins)
                sizeDelta.y = -190f;
            else
                sizeDelta.y = -225f;
            scrollRectTransform.sizeDelta = sizeDelta;

            float timeToWait = Time.unscaledTime + 0.25f;
            while (timeToWait > Time.unscaledTime)
                yield return null;

            m_cachedDisplays.Clear();
            if (m_container.childCount != 0)
                TransformUtils.DestroyAllChildren(m_container);


            if (m_selectedCategory != PersonalizationCategory.WeaponSkins && !ModFeatures.IsEnabled(ModFeatures.FeatureType.AccessoriesAndPets))
            {
                m_notImplementedTextObject.SetActive(true);
            }
            else
            {
                m_notImplementedTextObject.SetActive(false);

                if (!Enum.TryParse(m_selectedSubcategory, out WeaponType weaponType))
                    weaponType = WeaponType.Sword;

                ModdedObject utilsPanel = Instantiate(m_utilsPanel, m_container);
                utilsPanel.gameObject.SetActive(true);
                Button defaultSkinButton = utilsPanel.GetObject<Button>(0);
                defaultSkinButton.onClick.AddListener(delegate
                {
                    defaultSkinButton.interactable = false;
                    PersonalizationController.SetWeaponSkin(weaponType, null);
                    PersonalizationController.DestroyWeaponSkin(weaponType);
                    GlobalEventManager.Instance.Dispatch(PersonalizationManager.ITEM_EQUIPPED_OR_UNEQUIPPED);
                });
                defaultSkinButton.interactable = !PersonalizationController.GetWeaponSkin(weaponType).IsNullOrEmpty();
                m_defaultSkinButton = defaultSkinButton;

                ModGameUtils.WaitForPlayerInputUpdate(delegate (IFPMoveCommandInput input)
                {
                    switch (weaponType)
                    {
                        case WeaponType.Sword:
                            input.Weapon1 = true;
                            break;
                        case WeaponType.Bow:
                            input.Weapon2 = true;
                            break;
                        case WeaponType.Hammer:
                            input.Weapon3 = true;
                            break;
                        case WeaponType.Spear:
                            input.Weapon4 = true;
                            break;
                        case WeaponType.Shield:
                            input.Weapon4 = true;
                            break;
                    }
                });

                System.Collections.Generic.List<PersonalizationItemInfo> list = PersonalizationManager.Instance.itemList.GetItems(m_selectedCategory, (PersonalizationItemsSortType)m_sortType);
                for (int i = 0; i < list.Count; i++)
                {
                    PersonalizationItemInfo item = list[i];
                    if (item.Weapon != weaponType)
                        continue;

                    bool isUnlocked = item.IsUnlocked();
                    bool isExclusive = item.IsExclusive();
                    bool isVerified = item.IsVerified;

                    string authorsString = item.GetAuthorsString();
                    string prefix;
                    if (authorsString == "vanilla")
                        prefix = LocalizationManager.Instance.GetTranslatedString("customization_vanilla");
                    else
                        prefix = $"{LocalizationManager.Instance.GetTranslatedString("customization_author")} ";

                    Color textColor;
                    Color textOutlineColor;
                    Color glowColor;

                    if (isExclusive && !isVerified)
                    {
                        textColor = ModParseUtils.TryParseToColor(ITEM_DISPLAY_NONVERIFIED_EXCLUSIVE_TEXT_COLOR, Color.white);
                        textOutlineColor = ModParseUtils.TryParseToColor(ITEM_DISPLAY_NONVERIFIED_EXCLUSIVE_TEXT_OUTLINE_COLOR, Color.black);
                        glowColor = ModParseUtils.TryParseToColor(ITEM_DISPLAY_NONVERIFIED_EXCLUSIVE_TEXT_GLOW_COLOR, Color.white);
                    }
                    else if (isExclusive)
                    {
                        textColor = ModParseUtils.TryParseToColor(ITEM_DISPLAY_EXCLUSIVE_TEXT_COLOR, Color.white);
                        textOutlineColor = ModParseUtils.TryParseToColor(ITEM_DISPLAY_EXCLUSIVE_TEXT_OUTLINE_COLOR, Color.black);
                        glowColor = ModParseUtils.TryParseToColor(ITEM_DISPLAY_EXCLUSIVE_TEXT_GLOW_COLOR, Color.white);
                    }
                    else if (!isVerified)
                    {
                        textColor = ModParseUtils.TryParseToColor(ITEM_DISPLAY_NONVERIFIED_TEXT_COLOR, Color.white);
                        textOutlineColor = ModParseUtils.TryParseToColor(ITEM_DISPLAY_NONVERIFIED_TEXT_OUTLINE_COLOR, Color.black);
                        glowColor = ModParseUtils.TryParseToColor(ITEM_DISPLAY_NONVERIFIED_TEXT_GLOW_COLOR, Color.white);
                    }
                    else
                    {
                        textColor = ModParseUtils.TryParseToColor(ITEM_DISPLAY_DEFAULT_TEXT_COLOR, Color.white);
                        textOutlineColor = ModParseUtils.TryParseToColor(ITEM_DISPLAY_DEFAULT_TEXT_OUTLINE_COLOR, Color.black);
                        glowColor = ModParseUtils.TryParseToColor(ITEM_DISPLAY_DEFAULT_TEXT_GLOW_COLOR, Color.white);
                    }
                    glowColor.a = !isExclusive && isVerified ? 0.25f : 0.4f;

                    ModdedObject moddedObject = Instantiate(m_itemDisplay, m_container);
                    moddedObject.gameObject.SetActive(true);

                    Text itemNameText = moddedObject.GetObject<Text>(0);
                    itemNameText.text = item.Name.ToUpper();
                    itemNameText.color = textColor;
                    Outline itemNameTextOutline = itemNameText.GetComponent<Outline>();
                    itemNameTextOutline.effectColor = textOutlineColor;

                    Image glowImage = moddedObject.GetObject<Image>(2);
                    glowImage.color = glowColor;

                    moddedObject.GetObject<Text>(1).text = $"{prefix}{authorsString}";

                    UIElementPersonalizationItemDisplay personalizationItemDisplay = moddedObject.gameObject.AddComponent<UIElementPersonalizationItemDisplay>();
                    personalizationItemDisplay.ItemInfo = item;
                    personalizationItemDisplay.SetBrowserUI(this);
                    personalizationItemDisplay.InitializeElement();

                    m_cachedDisplays.Add(item.Name.ToLower(), moddedObject.gameObject);

                    if (i % 15 == 0)
                        yield return null;
                }
            }

            OnSearchBoxChanged(m_searchBox.text);

            m_prevTab = m_categoryTabs.selectedTab?.tabId;
            m_categoryTabs.interactable = true;
            m_subcategoryTabs.interactable = true;
            m_sortDropdown.interactable = true;
            m_showContents = true;
            m_isPopulating = false;
            yield break;
        }

        private void onCustomizationAssetsFileDownloaded()
        {
            Populate();
            refreshUpdateButton();
        }

        private void refreshUpdateButton()
        {
            PersonalizationManager personalizationManager = PersonalizationManager.Instance;
            if (personalizationManager.GetPersonalizationAssetsState() == PersonalizationAssetsState.NotInstalled)
            {
                m_updateButtonText.text = LocalizationManager.Instance.GetTranslatedString("Download");
            }
            else
            {
                m_updateButtonText.text = LocalizationManager.Instance.GetTranslatedString("customization_button_update");
            }
        }

        private void refreshCameraRect()
        {
            CameraManager cameraManager = CameraManager.Instance;
            if (m_isOpen)
            {
                float proportionOfWidthTakenUpBySidebar = m_panel.rect.width / m_rectTransform.rect.width;
                cameraManager.SetCameraRect(new Rect(proportionOfWidthTakenUpBySidebar, 0, 1f - proportionOfWidthTakenUpBySidebar, 1f));
            }
            else
            {
                cameraManager.ResetCameraRect();
            }
        }

        private void setCameraZoomedIn(bool value)
        {
            FirstPersonMover firstPersonMover = CharacterTracker.Instance.GetPlayerRobot();
            CameraManager cameraManager = CameraManager.Instance;
            if (!value)
            {
                refreshCameraRect();
                cameraManager.ResetCameraHolderPosition(firstPersonMover);
                cameraManager.ResetCameraHolderEulerAngles(firstPersonMover);
                cameraManager.enableForceFOVOffset = false;
                cameraManager.enableThirdPerson = false;
                return;
            }

            refreshCameraRect();
            cameraManager.SetCameraHolderPosition(new Vector3(0f, 0f, 0.75f), firstPersonMover);
            cameraManager.SetCameraHolderEulerAngles(Vector3.up * 220f, firstPersonMover);
            cameraManager.enableForceFOVOffset = true;
            cameraManager.enableThirdPerson = true;
            cameraManager.forceFOVOffset = -5f;

            ModGameUtils.WaitForPlayerInputUpdate(delegate (IFPMoveCommandInput commandInput)
            {
                commandInput.IsResetLookKeyDown = true;
            });
        }

        public void OnSettingsButtonClicked()
        {
            ModUIConstants.ShowPersonalizationSettingsMenu(base.transform);
        }

        public void OnUpdateButtonClicked()
        {
            ModUIConstants.ShowDownloadPersonalizationAssetsMenu(base.transform);
        }

        public void OnSearchBoxChanged(string text)
        {
            _ = text.ToLower();
            bool forceEnableAll = text.IsNullOrEmpty();
            foreach (KeyValuePair<string, GameObject> keyValue in m_cachedDisplays)
            {
                if (forceEnableAll)
                    keyValue.Value.SetActive(true);
                else
                    keyValue.Value.SetActive(keyValue.Key.Contains(text));
            }
        }

        public void OnSortDropdownChanged(int value)
        {
            if (!m_allowUICallbacks)
                return;

            m_sortType = value;
            Populate();
        }
    }
}
