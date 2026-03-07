using OverhaulMod.Combat;
using OverhaulMod.Content.Personalization;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPersonalizationItemBrowser : OverhaulUIBehaviour
    {
        [ModSetting(ModSettingsConstants.HAS_EVER_ROTATED_THE_CAMERA, false)]
        public static bool HasEverRotatedTheCamera;

        public static bool IsPreviewing;

        public static bool HasShownAssetsUpdateMenu;

        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_closeButton;

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

        [UIElement("MessageDisplay", false)]
        private readonly ModdedObject m_messageDisplay;
        [UIElement("UtilsPanel", false)]
        private readonly ModdedObject m_utilsPanel;
        [UIElement("BottomPanel", false)]
        private readonly ModdedObject m_bottomPanel;
        [UIElement("Content")]
        private readonly Transform m_container;
        [UIElement("Content")]
        private readonly CanvasGroup m_containerCanvasGroup;

        [UIElement("NotImplementedText", false)]
        private readonly GameObject m_notImplementedTextObject;

        [UIElementAction(nameof(OnAllowEnemiesUseWeaponSkinsToggled))]
        [UIElement("EnemiesUseSkinsToggle")]
        private readonly Toggle m_allowEnemiesUseWeaponSkinsToggle;

        [UIElementAction(nameof(OnClearButtonClicked))]
        [UIElement("ClearButton")]
        private readonly Button m_clearButton;

        [UIElement("ScrollRect")]
        private readonly RectTransform m_scrollRectTransform;
        [UIElement("ScrollRect")]
        private readonly Image m_scrollRectImage;
        [UIElement("Viewport")]
        private readonly RectTransform m_viewportTransform;
        [UIElement("ScrollbarVertical")]
        private readonly CanvasGroup m_scrollbarVerticalCanvasGroup;

        [UIElement("DescriptionBox", typeof(UIElementPersonalizationItemDescriptionBox), false)]
        private readonly UIElementPersonalizationItemDescriptionBox m_descriptionBox;

        [UIElementAction(nameof(OnSearchBoxChanged))]
        [UIElement("SearchBox")]
        private readonly InputField m_searchBox;

        [UIElement("LoadingIndicator")]
        private readonly CanvasGroup m_loadingIndicator;

        [UIElement("CameraRotationTutorial")]
        private readonly GameObject m_cameraRotationTutorial;

        [UIElement("ItemsLine", false)]
        private readonly Transform m_cardsLine;

        [UIElement("ItemCardDisplay", false)]
        private readonly ModdedObject m_itemCardDisplay;

        private RectTransform m_rectTransform;

        private bool m_allowUICallbacks;

        private PersonalizationCategory m_selectedCategory;

        private string m_selectedSubcategory;

        private Dictionary<string, UIElementPersonalizationItemDisplay> m_cachedDisplays;

        private bool m_isOpen, m_isPopulating, m_showContents, m_hasEverShown;

        private float m_transitionProgress, m_prevTransitionProgress;

        private string m_prevTab;

        private Button m_defaultSkinButton;

        private Transform m_cameraHolderTransform;

        private float m_cameraHolderRotationY;

        public override bool enableCursor => true;

        protected override void OnInitialized()
        {
            m_loadingIndicator.gameObject.SetActive(true);

            m_cachedDisplays = new Dictionary<string, UIElementPersonalizationItemDisplay>();
            m_rectTransform = base.GetComponent<RectTransform>();

            m_categoryTabs.AddTab(m_weaponSkinsTab.gameObject, "weapon skins");
            m_categoryTabs.AddTab(m_accessoriesTab.gameObject, "accessories");
            m_categoryTabs.AddTab(m_petsTab.gameObject, "pets");
            m_categoryTabs.SelectTab("weapon skins");
            m_prevTab = "weapon skins";

            m_descriptionBox.SetBrowserUI(this);
            m_allowEnemiesUseWeaponSkinsToggle.isOn = PersonalizationUserInfo.AllowEnemiesUseSkins;

            GlobalEventManager.Instance.AddEventListener(PersonalizationManager.CUSTOMIZATION_ASSETS_FILE_DOWNLOADED_EVENT, onCustomizationAssetsFileDownloaded);
            GlobalEventManager.Instance.AddEventListener(GlobalEvents.PlayerDied, tryHide);
            m_allowUICallbacks = true;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            IsPreviewing = false;
            GlobalEventManager.Instance.RemoveEventListener(PersonalizationManager.CUSTOMIZATION_ASSETS_FILE_DOWNLOADED_EVENT, onCustomizationAssetsFileDownloaded);
            GlobalEventManager.Instance.RemoveEventListener(GlobalEvents.PlayerDied, tryHide);
        }

        public override void Show()
        {
            base.Show();
            IsPreviewing = true;
            m_isOpen = true;
            m_showContents = !m_hasEverShown;
            m_transitionProgress = 0f;
            m_categoryTabs.interactable = true;

            if (m_categoryTabs.selectedTab && m_prevTab != m_categoryTabs.selectedTab.tabId)
            {
                if (m_container.childCount != 0)
                    TransformUtils.DestroyAllChildren(m_container);

                m_categoryTabs.SelectTab(m_prevTab);
            }

            _ = base.StartCoroutine(waitThenRefreshCameraCoroutine());
            ShowDownloadCustomizationAssetsDownloadMenuIfRequired();

            if (UIVersionLabel.instance)
                UIVersionLabel.instance.offsetX = 325f;

            m_cachedDisplays.Clear();
            if (m_container.childCount != 0)
                TransformUtils.DestroyAllChildren(m_container);

            if (m_selectedCategory == PersonalizationCategory.WeaponSkins)
                selectSubcategoryOfCurrentWeapon();

            m_hasEverShown = true;
            Populate();
        }

        public override void Hide()
        {
            base.Hide();
            m_isOpen = false;
            setCameraZoomedIn(false);
            UIVersionLabel.instance.offsetX = 0f;

            PersonalizationManager.Instance.userInfo.SaveIfDirty();
            ModSettingsDataManager.Instance.Save();

            PersonalizationMultiplayerManager.Instance.SendPlayerCustomizationDataEvent(false);
        }

        public override void Update()
        {
            base.Update();

            m_transitionProgress = Mathf.Clamp01(m_transitionProgress + (Time.unscaledDeltaTime * 5f * (m_showContents ? 1f : -1f)));
            if (m_transitionProgress != m_prevTransitionProgress)
            {
                m_prevTransitionProgress = m_transitionProgress;
                float progress = NumberUtils.EaseInOutQuad(0f, 1f, m_transitionProgress);

                float a = m_containerCanvasGroup.alpha;
                a = progress;
                m_containerCanvasGroup.alpha = a;

                m_scrollbarVerticalCanvasGroup.alpha = progress;
                m_loadingIndicator.alpha = 1f - progress;

                Color color2 = m_scrollRectImage.color;
                color2.r = Mathf.Lerp(0.05f, 0.15f, progress);
                color2.g = Mathf.Lerp(0.05f, 0.15f, progress);
                color2.b = Mathf.Lerp(0.05f, 0.15f, progress);
                m_scrollRectImage.color = color2;

                Vector2 offsetMax = m_viewportTransform.offsetMax;
                offsetMax.y = -50f * (1f - progress);
                m_viewportTransform.offsetMax = offsetMax;
            }

            Transform holder = m_cameraHolderTransform;
            if (holder)
            {
                bool mouseButtonDown = Input.GetMouseButton(1);
                if (mouseButtonDown && !HasEverRotatedTheCamera)
                {
                    ModSettingsManager.SetBoolValue(ModSettingsConstants.HAS_EVER_ROTATED_THE_CAMERA, true);
                }

                float d2 = Time.deltaTime * 15f;
                m_cameraHolderRotationY = Mathf.Lerp(m_cameraHolderRotationY, mouseButtonDown ? Input.GetAxis("Mouse X") * 1.25f : 0f, d2);

                Vector3 currentEulerAngles = holder.localEulerAngles;
                currentEulerAngles.y += m_cameraHolderRotationY;
                holder.localEulerAngles = currentEulerAngles;
            }

            m_cameraRotationTutorial.SetActive(!HasEverRotatedTheCamera);
        }

        private void LateUpdate()
        {
            refreshCameraRect();
        }

        public override void OnDisable()
        {
            base.OnDisable();

            IsPreviewing = false;
            m_isPopulating = false;
        }

        public bool IsMouseOverPanel() => m_panelMouseEvents.isMouseOverElement;

        public void ShowDescriptionBox(PersonalizationItemInfo itemInfo, RectTransform rectTransform)
        {
            m_descriptionBox.ShowForItem(itemInfo, rectTransform);
        }

        public void MakeDefaultSkinButtonInteractable()
        {
            Button button = m_defaultSkinButton;
            if (button) button.interactable = true;
        }

        public void ShowDownloadCustomizationAssetsDownloadMenuIfRequired()
        {
            if (HasShownAssetsUpdateMenu) return;

            if (PersonalizationManager.Instance.GetPersonalizationAssetsState() != PersonalizationAssetsState.Installed)
            {
                _ = ModUIConstants.ShowDownloadPersonalizationAssetsMenu(base.transform);
                HasShownAssetsUpdateMenu = true;
            }
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
                vector.y = 28f;
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

                if (ModFeatures.IsEnabled(ModFeatures.FeatureType.ShieldSkins))
                    m_subcategoryTabs.AddTab("Shield");

                if (ModFeatures.IsEnabled(ModFeatures.FeatureType.ScytheSkins) && BoltNetwork.IsServer)
                    m_subcategoryTabs.AddTab(ModWeaponsManager.SCYTHE_TYPE.ToString());

                selectSubcategoryOfCurrentWeapon();
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
            m_notImplementedTextObject.SetActive(false);

            refreshScrollRectSize();

            float timeToWait = Time.unscaledTime + 0.25f;
            while (timeToWait > Time.unscaledTime) yield return null;

            m_cachedDisplays.Clear();
            if (m_container.childCount != 0) TransformUtils.DestroyAllChildren(m_container);

            List<PersonalizationItemInfo> items = null;
            bool populatePage = false;
            bool isDeveloper = ModUserInfo.isDeveloper;

            WeaponType weaponType = getWeaponOfSubcategory();

            switch (m_selectedCategory)
            {
                case PersonalizationCategory.WeaponSkins:
                    equipWeapon(weaponType);
                    if (weaponType == WeaponType.Bow && ModSpecialUtils.IsModEnabled("ee32ba1b-8c92-4f50-bdf4-400a14da829e"))
                    {
                        ModdedObject messageDisplay = Instantiate(m_messageDisplay, m_container);
                        messageDisplay.gameObject.SetActive(true);
                        messageDisplay.GetObject<Text>(0).text = LocalizationManager.Instance.GetTranslatedString("bow_skins_not_supported_glock18");
                        break;
                    }
                    populatePage = true;
                    items = PersonalizationManager.Instance.itemList.GetWeaponSkins(weaponType, PersonalizationItemsSortType.Alphabet);
                    break;
                case PersonalizationCategory.Accessories:
                    populatePage = ModFeatures.IsEnabled(ModFeatures.FeatureType.Accessories);
                    m_notImplementedTextObject.SetActive(!populatePage);
                    if(populatePage) items = PersonalizationManager.Instance.itemList.GetItems(PersonalizationCategory.Accessories, PersonalizationItemsSortType.Alphabet);
                    break;
                case PersonalizationCategory.Pets:
                    populatePage = ModFeatures.IsEnabled(ModFeatures.FeatureType.Pets);
                    m_notImplementedTextObject.SetActive(!populatePage);
                    if (populatePage) items = PersonalizationManager.Instance.itemList.GetItems(PersonalizationCategory.Pets, PersonalizationItemsSortType.Alphabet);
                    break;
            }

            if (populatePage)
            {
                // spawn utils panel
                ModdedObject utilsPanel = Instantiate(m_utilsPanel, m_container);
                utilsPanel.gameObject.SetActive(true);
                Button defaultSkinButton = utilsPanel.GetObject<Button>(0);
                defaultSkinButton.onClick.AddListener(delegate
                {
                    defaultSkinButton.interactable = false;
                    PersonalizationUserInfo.SetWeaponSkin(weaponType, null);
                    PersonalizationController.DestroyWeaponSkinOnMainPlayer(weaponType);
                    GlobalEventManager.Instance.Dispatch(PersonalizationManager.ITEM_EQUIPPED_OR_UNEQUIPPED_EVENT);
                });
                defaultSkinButton.interactable = !PersonalizationUserInfo.GetWeaponSkin(weaponType).IsNullOrEmpty();
                m_defaultSkinButton = defaultSkinButton;

                utilsPanel.GetObject<Button>(1).onClick.AddListener(OnUpdateButtonClicked);

                PersonalizationManager personalizationManager = PersonalizationManager.Instance;
                if (personalizationManager.GetPersonalizationAssetsState() == PersonalizationAssetsState.NotInstalled)
                    utilsPanel.GetObject<Text>(2).text = LocalizationManager.Instance.GetTranslatedString("Download");
                else
                    utilsPanel.GetObject<Text>(2).text = LocalizationManager.Instance.GetTranslatedString("customization_button_update");

                // populate items
                int spawnedCards = 0;
                Transform lastCardsLine = null;
                for (int i = 0; i < items.Count; i++)
                {
                    if (i % 20 == 0) yield return null;

                    PersonalizationItemInfo item = items[i];
                    if (item.HideInBrowser && !isDeveloper) continue;

                    if (spawnedCards % 3 == 0)
                    {
                        lastCardsLine = Instantiate(m_cardsLine, m_container);
                        lastCardsLine.gameObject.SetActive(true);
                    }
                    instantiateItemEntryDisplay(item, lastCardsLine);
                    spawnedCards++;
                }

                // additional panels
                ModdedObject bottomPanel = Instantiate(m_bottomPanel, m_container);
                bottomPanel.gameObject.SetActive(true);
                Button editorButton = bottomPanel.GetObject<Button>(0);
                editorButton.onClick.AddListener(delegate
                {
                    ModUIUtils.MessagePopup(true, LocalizationManager.Instance.GetTranslatedString("enter_ceditor_dialog_header"), LocalizationManager.Instance.GetTranslatedString("enter_ceditor_dialog_text"), 150f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Yes", "No", null, delegate
                    {
                        ModCore.ShouldStartCustomizationEditor = true;
                        SceneTransitionManager.Instance.DisconnectAndExitToMainMenu();
                    });
                });

                ModdedObject messageDisplay1 = Instantiate(m_messageDisplay, m_container);
                messageDisplay1.gameObject.SetActive(true);
                messageDisplay1.GetObject<Text>(0).text = LocalizationManager.Instance.GetTranslatedString("authors_reminder");

                // refresh search
                OnSearchBoxChanged(m_searchBox.text);
            }

            float waitTime = Time.unscaledTime + 0.1f;
            while (Time.unscaledTime < waitTime)
                yield return null;

            m_prevTab = m_categoryTabs.selectedTab?.tabId;
            m_categoryTabs.interactable = true;
            m_subcategoryTabs.interactable = true;
            m_showContents = true;
            m_isPopulating = false;
            yield break;
        }

        private void instantiateItemEntryDisplay(PersonalizationItemInfo item, Transform parent = null)
        {
            ModdedObject moddedObject = Instantiate(m_itemCardDisplay, parent ? parent : m_container);
            moddedObject.gameObject.SetActive(true);

            UIElementPersonalizationItemDisplay personalizationItemDisplay = moddedObject.gameObject.AddComponent<UIElementPersonalizationItemDisplay>();
            personalizationItemDisplay.ItemInfo = item;
            personalizationItemDisplay.SetBrowserUI(this);
            personalizationItemDisplay.InitializeElement();

            string text = item.Name.ToLower();
            while (m_cachedDisplays.ContainsKey(text))
                text += "_1";

            m_cachedDisplays.Add(text, personalizationItemDisplay);
        }

        private IEnumerator waitThenRefreshCameraCoroutine()
        {
            FirstPersonMover firstPersonMover = CharacterTracker.Instance.GetPlayerRobot();
            if (firstPersonMover)
            {
                while (firstPersonMover.IsSwingingMeleeWeapon())
                    yield return null;
            }
            setCameraZoomedIn(true);
            yield break;
        }

        private WeaponType getEquippedSupportedWeapon(FirstPersonMover robot = null)
        {
            FirstPersonMover target = robot ? robot : CharacterTracker.Instance.GetPlayerRobot();
            WeaponType weaponType;
            if (target)
            {
                weaponType = target.GetEquippedWeaponType();
                if (!PersonalizationManager.SupportedWeapons.Contains(weaponType)) weaponType = WeaponType.Sword;
            }
            else
            {
                weaponType = WeaponType.Sword;
            }
            return weaponType;
        }

        private WeaponType getWeaponOfSubcategory()
        {
            if (!Enum.TryParse(m_selectedSubcategory, out WeaponType weaponType))
                weaponType = WeaponType.Sword;

            return weaponType;
        }

        private void selectSubcategoryOfCurrentWeapon()
        {
            selectSubcategoryOfWeapon(getEquippedSupportedWeapon());
        }

        private void selectSubcategoryOfWeapon(WeaponType weaponType)
        {
            string weaponTypeString = weaponType.ToString();
            if (m_subcategoryTabs.HasTab(weaponTypeString))
                m_subcategoryTabs.SelectTab(weaponTypeString);
            else
                m_subcategoryTabs.SelectTab("Sword");
        }

        private void refreshScrollRectSize()
        {
            RectTransform scrollRectTransform = m_scrollRectTransform;
            Vector2 offsetMax = scrollRectTransform.offsetMax;
            if (m_selectedCategory != PersonalizationCategory.WeaponSkins)
                offsetMax.y = -125f;
            else
                offsetMax.y = -155f;
            scrollRectTransform.offsetMax = offsetMax;
        }

        private void equipWeapon(WeaponType weaponType)
        {
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
                    case ModWeaponsManager.SCYTHE_TYPE:
                        FirstPersonMover firstPersonMover = CharacterTracker.Instance.GetPlayerRobot();
                        if (firstPersonMover)
                        {
                            firstPersonMover.SetEquippedWeaponType(ModWeaponsManager.SCYTHE_TYPE);
                        }
                        break;
                }
            });
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

        private void tryHide()
        {
            if (isActiveAndEnabled) Hide();
        }

        private void onCustomizationAssetsFileDownloaded()
        {
            Populate();
        }

        private void setCameraZoomedIn(bool value)
        {
            FirstPersonMover firstPersonMover = CharacterTracker.Instance.GetPlayerRobot();
            CameraManager cameraManager = CameraManager.Instance;
            if (!value)
            {
                refreshCameraRect();
                if (!firstPersonMover.IsRidingOtherCharacter()) cameraManager.ResetCameraHolderPosition(firstPersonMover);
                cameraManager.ResetCameraHolderEulerAngles(firstPersonMover);
                cameraManager.enableForceFOVOffset = false;
                cameraManager.enableThirdPerson = false;
                m_cameraHolderTransform = null;
                return;
            }

            refreshCameraRect();
            if (!firstPersonMover.IsRidingOtherCharacter()) cameraManager.SetCameraHolderPosition(new Vector3(0f, 0f, 0.75f), firstPersonMover);
            cameraManager.SetCameraHolderEulerAngles(Vector3.up * 220f, firstPersonMover);
            cameraManager.enableForceFOVOffset = true;
            cameraManager.enableThirdPerson = true;
            cameraManager.forceFOVOffset = -5f;

            FirstPersonMover robot = CharacterTracker.Instance.GetPlayerRobot();
            if (robot)
                m_cameraHolderTransform = robot._cameraHolderTransform;

            ModGameUtils.WaitForPlayerInputUpdate(delegate (IFPMoveCommandInput commandInput)
            {
                commandInput.IsResetLookKeyDown = true;
            });
        }

        public void OnSettingsButtonClicked()
        {
            _ = ModUIConstants.ShowPersonalizationSettingsMenu(base.transform);
        }

        public void OnUpdateButtonClicked()
        {
            _ = ModUIConstants.ShowDownloadPersonalizationAssetsMenu(base.transform);
        }

        public void OnSearchBoxChanged(string text)
        {
            m_clearButton.gameObject.SetActive(!text.IsNullOrEmpty());

            _ = text.ToLower();
            bool forceEnableAll = text.IsNullOrEmpty();
            foreach (KeyValuePair<string, UIElementPersonalizationItemDisplay> keyValue in m_cachedDisplays)
            {
                if (forceEnableAll)
                    keyValue.Value.gameObject.SetActive(true);
                else
                    keyValue.Value.gameObject.SetActive(keyValue.Key.Contains(text));
            }
        }

        public void OnClearButtonClicked()
        {
            m_searchBox.text = string.Empty;
        }

        public void OnAllowEnemiesUseWeaponSkinsToggled(bool value)
        {
            if (!m_allowUICallbacks)
                return;

            ModSettingsManager.SetBoolValue(ModSettingsConstants.ALLOW_ENEMIES_USE_WEAPON_SKINS, value, true);
        }
    }
}