using OverhaulMod.Content.Personalization;
using OverhaulMod.Engine;
using OverhaulMod.Gameplay;
using OverhaulMod.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPersonalizationItemBrowser : OverhaulUIBehaviour
    {
        [ModSetting(ModSettingIDs.HAS_EVER_ROTATED_THE_CAMERA, false)]
        public static bool HasEverRotatedTheCamera;

        public static bool IsPreviewing;

        public static bool HasShownAssetsUpdateMenu;

        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _closeButton;

        [UIElement("Panel")]
        private readonly RectTransform _panel;

        [UIElement("Panel", typeof(UIElementMouseEventsComponent))]
        private readonly UIElementMouseEventsComponent _panelMouseEvents;

        [TabManager(typeof(UIElementTab), null, null, null, nameof(OnCategoryTabSelected))]
        private readonly TabManager _categoryTabs;
        [UIElement("WeaponSkinsTab")]
        private readonly ModdedObject _weaponSkinsTab;
        [UIElement("AccessoriesTab")]
        private readonly ModdedObject _accessoriesTab;
        [UIElement("PetsTab")]
        private readonly ModdedObject _petsTab;

        [TabManager(typeof(UIElementTabWithText), nameof(_subCategoryTabPrefab), nameof(_subCategoryTabsContainer), nameof(OnSubcategoryTabCreated), nameof(OnSubcategoryTabSelected))]
        private readonly TabManager _subcategoryTabs;
        [UIElement("SubcategoryTabPrefab", false)]
        private readonly ModdedObject _subCategoryTabPrefab;
        [UIElement("SubcategoryTabs")]
        private readonly Transform _subCategoryTabsContainer;

        [UIElement("MessageDisplay", false)]
        private readonly ModdedObject _messageDisplay;
        [UIElement("UtilsPanel", false)]
        private readonly ModdedObject _utilsPanel;
        [UIElement("BottomPanel", false)]
        private readonly ModdedObject _bottomPanel;
        [UIElement("Content")]
        private readonly Transform _container;
        [UIElement("Content")]
        private readonly CanvasGroup _containerCanvasGroup;

        [UIElement("NotImplementedText", false)]
        private readonly GameObject _notImplementedTextObject;

        [UIElementAction(nameof(OnAllowEnemiesUseWeaponSkinsToggled))]
        [UIElement("EnemiesUseSkinsToggle")]
        private readonly Toggle _allowEnemiesUseWeaponSkinsToggle;

        [UIElementAction(nameof(OnClearButtonClicked))]
        [UIElement("ClearButton")]
        private readonly Button _clearButton;

        [UIElement("ScrollRect")]
        private readonly RectTransform _scrollRectTransform;
        [UIElement("ScrollRect")]
        private readonly Image _scrollRectImage;
        [UIElement("Viewport")]
        private readonly RectTransform _viewportTransform;
        [UIElement("ScrollbarVertical")]
        private readonly CanvasGroup _scrollbarVerticalCanvasGroup;

        [UIElement("DescriptionBox", typeof(UIElementPersonalizationItemDescriptionBox), false)]
        private readonly UIElementPersonalizationItemDescriptionBox _descriptionBox;

        [UIElementAction(nameof(OnSearchBoxChanged))]
        [UIElement("SearchBox")]
        private readonly InputField _searchBox;

        [UIElement("LoadingIndicator", true)]
        private readonly CanvasGroup _loadingIndicator;

        [UIElement("CameraRotationTutorial")]
        private readonly GameObject _cameraRotationTutorial;

        [UIElement("ItemsLine", false)]
        private readonly Transform _cardsLine;

        [UIElement("ItemCardDisplay", false)]
        private readonly ModdedObject _itemCardDisplay;

        private RectTransform _rectTransform;

        private bool _allowUICallbacks;

        private PersonalizationCategory _selectedCategory;

        private string _selectedSubcategory;

        private Dictionary<string, UIElementPersonalizationItemDisplay> _cachedDisplays;

        private bool _isOpen, _isPopulating, _showContents, _hasEverShown;

        private float _transitionProgress, _prevTransitionProgress;

        private string _prevTab;

        private Button _defaultSkinButton;

        private Transform _cameraHolderTransform;

        private float _cameraHolderRotationY;

        public override bool EnableCursor => true;

        public bool HasSearchBar = false;

        protected override void OnInitialized()
        {
            _cachedDisplays = new Dictionary<string, UIElementPersonalizationItemDisplay>();
            _rectTransform = base.GetComponent<RectTransform>();

            _categoryTabs.AddTab(_weaponSkinsTab.gameObject, "weapon skins");
            _categoryTabs.AddTab(_accessoriesTab.gameObject, "accessories");
            _categoryTabs.AddTab(_petsTab.gameObject, "pets");

            _descriptionBox.SetBrowserUI(this);
            _allowEnemiesUseWeaponSkinsToggle.isOn = PersonalizationUserInfo.AllowEnemiesUseSkins;

            GlobalEventManager.Instance.AddEventListener(PersonalizationManager.CUSTOMIZATION_ASSETS_FILE_DOWNLOADED_EVENT, onCustomizationAssetsFileDownloaded);
            GlobalEventManager.Instance.AddEventListener(GlobalEvents.PlayerDied, tryHide);
            _allowUICallbacks = true;
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
            _isOpen = true;
            _showContents = !_hasEverShown;
            _transitionProgress = 0f;
            _categoryTabs.IsInteractable = true;

            if (!_categoryTabs.SelectedTab)
            {
                _categoryTabs.SelectTab("weapon skins");
                _prevTransitionProgress = 1f;
                _transitionProgress = 0f;
            }
            else if (_prevTab != _categoryTabs.SelectedTab.tabId)
            {
                if (_container.childCount != 0)
                    TransformUtils.DestroyAllChildren(_container);

                _categoryTabs.SelectTab(_prevTab);
            }

            _ = base.StartCoroutine(waitThenRefreshCameraCoroutine());
            ShowDownloadCustomizationAssetsDownloadMenuIfRequired();

            if (UIVersionLabel.instance)
                UIVersionLabel.instance.offsetX = 325f;

            _cachedDisplays.Clear();
            if (_container.childCount != 0)
                TransformUtils.DestroyAllChildren(_container);

            if (_selectedCategory == PersonalizationCategory.WeaponSkins)
                selectSubcategoryOfCurrentWeapon();

            _hasEverShown = true;
            Populate();

            PersonalizationManager.Instance.SetIsSelectingItems(true);
        }

        public override void Hide()
        {
            base.Hide();
            _isOpen = false;
            setCameraZoomedIn(false);
            UIVersionLabel.instance.offsetX = 0f;

            PersonalizationManager.Instance.UserInfo.SaveIfDirty();
            ModSettingsDataManager.Instance.Save();

            PersonalizationMultiplayerManager.Instance.SendPlayerCustomizationDataEvent(false);
            PersonalizationManager.Instance.SetIsSelectingItems(false);
        }

        public override void Update()
        {
            base.Update();

            _transitionProgress = Mathf.Clamp01(_transitionProgress + (Time.unscaledDeltaTime * 5f * (_showContents ? 1f : -1f)));
            if (_transitionProgress != _prevTransitionProgress)
            {
                _prevTransitionProgress = _transitionProgress;
                float progress = NumberUtils.EaseInOutQuad(0f, 1f, _transitionProgress);

                float a = _containerCanvasGroup.alpha;
                a = progress;
                _containerCanvasGroup.alpha = a;

                _scrollbarVerticalCanvasGroup.alpha = progress;
                _loadingIndicator.alpha = 1f - progress;

                Color color2 = _scrollRectImage.color;
                color2.r = Mathf.Lerp(0.05f, 0.15f, progress);
                color2.g = Mathf.Lerp(0.05f, 0.15f, progress);
                color2.b = Mathf.Lerp(0.05f, 0.15f, progress);
                _scrollRectImage.color = color2;

                Vector2 offsetMax = _viewportTransform.offsetMax;
                offsetMax.y = -50f * (1f - progress);
                _viewportTransform.offsetMax = offsetMax;
            }

            Transform holder = _cameraHolderTransform;
            if (holder)
            {
                bool mouseButtonDown = Input.GetMouseButton(1);
                if (mouseButtonDown && !HasEverRotatedTheCamera)
                {
                    ModSettingsManager.SetBoolValue(ModSettingIDs.HAS_EVER_ROTATED_THE_CAMERA, true);
                }

                float d2 = Time.deltaTime * 15f;
                _cameraHolderRotationY = Mathf.Lerp(_cameraHolderRotationY, mouseButtonDown ? Input.GetAxis("Mouse X") * 1.25f : 0f, d2);

                Vector3 currentEulerAngles = holder.localEulerAngles;
                currentEulerAngles.y += _cameraHolderRotationY;
                holder.localEulerAngles = currentEulerAngles;
            }

            _cameraRotationTutorial.SetActive(!HasEverRotatedTheCamera);
        }

        private void LateUpdate()
        {
            refreshCameraRect();
        }

        public override void OnDisable()
        {
            base.OnDisable();

            IsPreviewing = false;
            _isPopulating = false;
        }

        public bool IsMouseOverPanel() => _panelMouseEvents.isMouseOverElement;

        public void ShowDescriptionBox(PersonalizationItemInfo itemInfo, RectTransform rectTransform)
        {
            _descriptionBox.ShowForItem(itemInfo, rectTransform);
        }

        public void MakeDefaultSkinButtonInteractable()
        {
            Button button = _defaultSkinButton;
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
            UIElementTab oldTab = _categoryTabs.PreviousSelectedTab;
            UIElementTab newTab = _categoryTabs.SelectedTab;
            if (oldTab)
            {
                _prevTab = oldTab.tabId;

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

            _subcategoryTabs.Clear();
            if (newTab.tabId == "weapon skins")
            {
                _selectedCategory = PersonalizationCategory.WeaponSkins;
                _subcategoryTabs.AddTab("Sword");
                _subcategoryTabs.AddTab("Bow");
                _subcategoryTabs.AddTab("Hammer");
                _subcategoryTabs.AddTab("Spear");

                if (ModFeatures.IsEnabled(ModFeatures.FeatureType.ShieldSkins))
                    _subcategoryTabs.AddTab("Shield");

                if (BoltNetwork.IsServer)
                    _subcategoryTabs.AddTab(ModWeaponsManager.SCYTHE_TYPE.ToString());

                selectSubcategoryOfCurrentWeapon();
            }
            else if (newTab.tabId == "accessories")
            {
                _selectedCategory = PersonalizationCategory.Accessories;
            }
            else if (newTab.tabId == "pets")
            {
                _selectedCategory = PersonalizationCategory.Pets;
            }
            else
            {
                _selectedCategory = PersonalizationCategory.None;
            }

            Populate();
        }

        public void OnSubcategoryTabSelected(UIElementTab elementTab)
        {
            _selectedSubcategory = elementTab.tabId;
            Populate();
        }

        public void OnSubcategoryTabCreated(UIElementTab elementTab)
        {
            UIElementTabWithText elementTabWithText = elementTab as UIElementTabWithText;
            elementTabWithText.LocalizationID = $"customization_subtab_{elementTab.tabId.ToLower()}";
        }

        public void Populate()
        {
            if (_isPopulating || !base.enabled || !base.gameObject.activeInHierarchy)
                return;

            _isPopulating = true;
            _ = base.StartCoroutine(populateCoroutine());
        }

        private IEnumerator populateCoroutine()
        {
            _showContents = false;
            _categoryTabs.IsInteractable = false;
            _subcategoryTabs.IsInteractable = false;
            _notImplementedTextObject.SetActive(false);

            refreshScrollRectSize();

            float timeToWait = Time.unscaledTime + 0.25f;
            while (timeToWait > Time.unscaledTime) yield return null;

            _cachedDisplays.Clear();
            if (_container.childCount != 0) TransformUtils.DestroyAllChildren(_container);

            List<PersonalizationItemInfo> items = null;
            bool populatePage = false;
            bool isDeveloper = ModUserInfo.IsDeveloper;

            WeaponType weaponType = getWeaponOfSubcategory();

            switch (_selectedCategory)
            {
                case PersonalizationCategory.WeaponSkins:
                    equipWeapon(weaponType);
                    if (weaponType == WeaponType.Bow && ModSpecialUtils.IsModEnabled("ee32ba1b-8c92-4f50-bdf4-400a14da829e"))
                    {
                        ModdedObject messageDisplay = Instantiate(_messageDisplay, _container);
                        messageDisplay.gameObject.SetActive(true);
                        messageDisplay.GetObject<Text>(0).text = LocalizationManager.Instance.GetTranslatedString("bow_skins_not_supported_glock18");
                        break;
                    }
                    populatePage = true;
                    items = PersonalizationManager.Instance.ItemList.GetWeaponSkins(weaponType, PersonalizationItemsSortType.Alphabet);
                    break;
                case PersonalizationCategory.Accessories:
                    populatePage = ModFeatures.IsEnabled(ModFeatures.FeatureType.Accessories);
                    _notImplementedTextObject.SetActive(!populatePage);
                    if (populatePage) items = PersonalizationManager.Instance.ItemList.GetItems(PersonalizationCategory.Accessories, PersonalizationItemsSortType.Alphabet);
                    break;
                case PersonalizationCategory.Pets:
                    populatePage = ModFeatures.IsEnabled(ModFeatures.FeatureType.Pets);
                    _notImplementedTextObject.SetActive(!populatePage);
                    if (populatePage) items = PersonalizationManager.Instance.ItemList.GetItems(PersonalizationCategory.Pets, PersonalizationItemsSortType.Alphabet);
                    break;
            }

            if (populatePage)
            {
                // spawn utils panel
                ModdedObject utilsPanel = Instantiate(_utilsPanel, _container);
                utilsPanel.gameObject.SetActive(true);
                Button defaultSkinButton = utilsPanel.GetObject<Button>(0);
                defaultSkinButton.onClick.AddListener(delegate
                {
                    defaultSkinButton.interactable = false;
                    PersonalizationUserInfo.SetWeaponSkin(weaponType, null);
                    GlobalEventManager.Instance.Dispatch(PersonalizationManager.ITEM_EQUIPPED_OR_UNEQUIPPED_EVENT);

                    PersonalizationManager.Instance.RefreshCustomizationOnAllRobots(false, false, PersonalizationCategory.WeaponSkins);
                });
                defaultSkinButton.interactable = _selectedCategory == PersonalizationCategory.WeaponSkins && !PersonalizationUserInfo.GetWeaponSkin(weaponType).IsNullOrEmpty();
                _defaultSkinButton = defaultSkinButton;

                utilsPanel.GetObject<Button>(1).onClick.AddListener(OnUpdateButtonClicked);

                PersonalizationManager personalizationManager = PersonalizationManager.Instance;
                if (personalizationManager.GetPersonalizationAssetsState() == PersonalizationAssetsState.NotInstalled)
                    utilsPanel.GetObject<Text>(2).text = LocalizationManager.Instance.GetTranslatedString("Download");
                else
                    utilsPanel.GetObject<Text>(2).text = LocalizationManager.Instance.GetTranslatedString("customization_button_update");

                // onGotDownloadList items
                int spawnedCards = 0;
                Transform lastCardsLine = null;
                for (int i = 0; i < items.Count; i++)
                {
                    if (i % 20 == 0) yield return null;

                    PersonalizationItemInfo item = items[i];
                    if (item.HideInBrowser && !isDeveloper) continue;

                    if (spawnedCards % 3 == 0)
                    {
                        lastCardsLine = Instantiate(_cardsLine, _container);
                        lastCardsLine.gameObject.SetActive(true);
                    }
                    instantiateItemEntryDisplay(item, lastCardsLine);
                    spawnedCards++;
                }

                // additional panels
                ModdedObject bottomPanel = Instantiate(_bottomPanel, _container);
                bottomPanel.gameObject.SetActive(true);
                Button editorButton = bottomPanel.GetObject<Button>(0);
                editorButton.onClick.AddListener(delegate
                {
                    ModUIUtils.MessagePopup(true, LocalizationManager.Instance.GetTranslatedString("enter_ceditor_dialog_header"), LocalizationManager.Instance.GetTranslatedString("enter_ceditor_dialog_text"), 150f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Yes", "No", null, delegate
                    {
                        ModLoader.ShouldStartCustomizationEditor = true;
                        SceneTransitionManager.Instance.DisconnectAndExitToMainMenu();
                    });
                });

                ModdedObject messageDisplay1 = Instantiate(_messageDisplay, _container);
                messageDisplay1.gameObject.SetActive(true);
                messageDisplay1.GetObject<Text>(0).text = LocalizationManager.Instance.GetTranslatedString("authors_reminder");

                // refresh search
                OnSearchBoxChanged(_searchBox.text);
            }

            float waitTime = Time.unscaledTime + 0.1f;
            while (Time.unscaledTime < waitTime)
                yield return null;

            _prevTab = _categoryTabs.SelectedTab?.tabId;
            _categoryTabs.IsInteractable = true;
            _subcategoryTabs.IsInteractable = true;
            _showContents = true;
            _isPopulating = false;
            yield break;
        }

        private void instantiateItemEntryDisplay(PersonalizationItemInfo item, Transform parent = null)
        {
            ModdedObject moddedObject = Instantiate(_itemCardDisplay, parent ? parent : _container);
            moddedObject.gameObject.SetActive(true);

            UIElementPersonalizationItemDisplay personalizationItemDisplay = moddedObject.gameObject.AddComponent<UIElementPersonalizationItemDisplay>();
            personalizationItemDisplay.ItemInfo = item;
            personalizationItemDisplay.SetBrowserUI(this);
            personalizationItemDisplay.InitializeElement();

            string text = item.Name.ToLower();
            while (_cachedDisplays.ContainsKey(text))
                text += "_1";

            _cachedDisplays.Add(text, personalizationItemDisplay);
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
                if (!PersonalizationManager.IsWeaponCustomizationSupported(weaponType)) weaponType = WeaponType.Sword;
            }
            else
            {
                weaponType = WeaponType.Sword;
            }
            return weaponType;
        }

        private WeaponType getWeaponOfSubcategory()
        {
            if (!Enum.TryParse(_selectedSubcategory, out WeaponType weaponType))
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
            if (_subcategoryTabs.HasTab(weaponTypeString))
                _subcategoryTabs.SelectTab(weaponTypeString);
            else
                _subcategoryTabs.SelectTab("Sword");
        }

        private void refreshScrollRectSize()
        {
            RectTransform scrollRectTransform = _scrollRectTransform;
            Vector2 offsetMax = scrollRectTransform.offsetMax;
            if (_selectedCategory != PersonalizationCategory.WeaponSkins)
                offsetMax.y = -125f;
            else
                offsetMax.y = -155f;

            if (!HasSearchBar) offsetMax.y += 40f;
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
            if (_isOpen)
            {
                float proportionOfWidthTakenUpBySidebar = _panel.rect.width / _rectTransform.rect.width;
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
            CameraManager cameraManager = CameraManager.Instance;
            FirstPersonMover firstPersonMover = CharacterTracker.Instance.GetPlayerRobot();
            if (!firstPersonMover)
            {
                cameraManager.EnableForceFOVOffset = false;
                cameraManager.EnableThirdPerson = false;
                _cameraHolderTransform = null;
                return;
            }

            if (!value)
            {
                refreshCameraRect();
                if (!firstPersonMover.IsRidingOtherCharacter()) cameraManager.ResetCameraHolderPosition(firstPersonMover);
                cameraManager.ResetCameraHolderEulerAngles(firstPersonMover);
                cameraManager.EnableForceFOVOffset = false;
                cameraManager.EnableThirdPerson = false;
                _cameraHolderTransform = null;
                return;
            }

            refreshCameraRect();
            if (!firstPersonMover.IsRidingOtherCharacter()) cameraManager.SetCameraHolderPosition(new Vector3(0f, 0f, 0.75f), firstPersonMover);
            cameraManager.SetCameraHolderEulerAngles(Vector3.up * 220f, firstPersonMover);
            cameraManager.EnableForceFOVOffset = true;
            cameraManager.EnableThirdPerson = true;
            cameraManager.ForceFOVOffset = -5f;
            _cameraHolderTransform = firstPersonMover._cameraHolderTransform;

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
            _clearButton.gameObject.SetActive(!text.IsNullOrEmpty());

            _ = text.ToLower();
            bool forceEnableAll = text.IsNullOrEmpty();
            foreach (KeyValuePair<string, UIElementPersonalizationItemDisplay> keyValue in _cachedDisplays)
            {
                if (forceEnableAll)
                    keyValue.Value.gameObject.SetActive(true);
                else
                    keyValue.Value.gameObject.SetActive(keyValue.Key.Contains(text));
            }
        }

        public void OnClearButtonClicked()
        {
            _searchBox.text = string.Empty;
        }

        public void OnAllowEnemiesUseWeaponSkinsToggled(bool value)
        {
            if (!_allowUICallbacks)
                return;

            ModSettingsManager.SetBoolValue(ModSettingIDs.ALLOW_ENEMIES_USE_WEAPON_SKINS, value, true);

            PersonalizationManager.Instance.RefreshCustomizationOnAllRobots(false, true);
        }
    }
}