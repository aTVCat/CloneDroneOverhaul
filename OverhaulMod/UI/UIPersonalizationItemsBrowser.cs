using OverhaulMod.Content.Personalization;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPersonalizationItemsBrowser : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElement("Panel")]
        private readonly RectTransform m_panel;

        [TabManager(typeof(UIElementTab), null, null, null, nameof(OnTabSelected))]
        private readonly TabManager m_tabs;
        [UIElement("WeaponSkinsTab")]
        private readonly ModdedObject m_weaponSkinsTab;
        [UIElement("AccessoriesTab")]
        private readonly ModdedObject m_accessoriesTab;
        [UIElement("PetsTab")]
        private readonly ModdedObject m_petsTab;

        [UIElement("ItemDisplay", false)]
        private readonly ModdedObject m_itemDisplay;
        [UIElement("TextDisplay", false)]
        private readonly ModdedObject m_textDisplay;
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

        private RectTransform m_rectTransform;

        private bool m_isOpen, m_isPopulating, m_showContents;

        private string m_prevTab;

        private PersonalizationCategory m_selectedCategory;

        private readonly UnityWebRequest m_webRequest;

        public override bool enableCursor => true;

        protected override void OnInitialized()
        {
            m_rectTransform = base.GetComponent<RectTransform>();

            m_tabs.AddTab(m_weaponSkinsTab.gameObject, "weapon skins");
            m_tabs.AddTab(m_accessoriesTab.gameObject, "accessories");
            m_tabs.AddTab(m_petsTab.gameObject, "pets");
            m_tabs.SelectTab("weapon skins");
            m_prevTab = "weapon skins";

            GlobalEventManager.Instance.AddEventListener(PersonalizationManager.CUSTOMIZATION_ASSETS_FILE_DOWNLOADED_EVENT, onCustomizationAssetsFileDownloaded);
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
            m_tabs.interactable = true;

            if (m_tabs.selectedTab && m_prevTab != m_tabs.selectedTab.tabId)
            {
                if (m_container.childCount != 0)
                    TransformUtils.DestroyAllChildren(m_container);

                m_tabs.SelectTab(m_prevTab);
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
        }

        public void ShowDownloadCustomizationAssetsDownloadMenuIfRequired()
        {
            if (PersonalizationManager.Instance.GetPersonalizationAssetsState() != PersonalizationAssetsState.Installed)
                ModUIConstants.ShowDownloadCustomizationAssetsMenu(base.transform);
        }

        public void OnTabSelected(UIElementTab elementTab)
        {
            UIElementTab oldTab = m_tabs.prevSelectedTab;
            UIElementTab newTab = m_tabs.selectedTab;
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

            if (newTab.tabId == "weapon skins")
            {
                m_selectedCategory = PersonalizationCategory.WeaponSkins;
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
            m_tabs.interactable = false;

            float timeToWait = Time.unscaledTime + 0.25f;
            while (timeToWait > Time.unscaledTime)
                yield return null;

            if (m_container.childCount != 0)
                TransformUtils.DestroyAllChildren(m_container);

            if (m_selectedCategory != PersonalizationCategory.WeaponSkins && !ModFeatures.IsEnabled(ModFeatures.FeatureType.AccessoriesAndPets))
            {
                m_notImplementedTextObject.SetActive(true);
            }
            else
            {
                m_notImplementedTextObject.SetActive(false);

                FirstPersonMover firstPersonMover = CharacterTracker.Instance.GetPlayerRobot();
                WeaponType weaponType = WeaponType.None;
                System.Collections.Generic.List<PersonalizationItemInfo> list = PersonalizationManager.Instance.itemList.GetItems(m_selectedCategory, true);
                for (int i = 0; i < list.Count; i++)
                {
                    PersonalizationItemInfo item = list[i];
                    bool isUnlocked = item.IsUnlocked();

                    if (item.Weapon != weaponType)
                    {
                        WeaponType currentWeaponType = item.Weapon;

                        ModdedObject moddedObject1 = Instantiate(m_textDisplay, m_container);
                        moddedObject1.gameObject.SetActive(true);
                        moddedObject1.GetObject<Button>(0).onClick.AddListener(delegate
                        {
                            if (firstPersonMover)
                            {
                                ModGameUtils.WaitForPlayerInputUpdate(delegate (IFPMoveCommandInput commandInput)
                                {
                                    switch (currentWeaponType)
                                    {
                                        case WeaponType.Sword:
                                            commandInput.Weapon1 = true;
                                            break;
                                        case WeaponType.Bow:
                                            commandInput.Weapon2 = true;
                                            break;
                                        case WeaponType.Hammer:
                                            commandInput.Weapon3 = true;
                                            break;
                                        case WeaponType.Shield:
                                        case WeaponType.Spear:
                                            commandInput.Weapon4 = true;
                                            break;
                                    }
                                });
                            }
                        });
                        moddedObject1.GetObject<Button>(0).interactable = firstPersonMover && firstPersonMover._equippedWeapons.Contains(currentWeaponType);

                        Text text = moddedObject1.GetComponent<Text>();
                        text.text = item.Weapon.ToString();

                        weaponType = currentWeaponType;
                    }

                    string authorsString = item.GetAuthorsString();
                    string prefix;
                    if (authorsString.StartsWith("From ") || authorsString.StartsWith("from "))
                        prefix = string.Empty;
                    else
                        prefix = "By ";

                    ModdedObject moddedObject = Instantiate(m_itemDisplay, m_container);
                    moddedObject.gameObject.SetActive(true);
                    moddedObject.GetObject<Text>(0).text = item.Name;
                    moddedObject.GetObject<Text>(1).text = $"{prefix}{authorsString}";
                    moddedObject.GetObject<GameObject>(2).SetActive(!isUnlocked);
                    moddedObject.GetObject<GameObject>(3).SetActive(!item.IsVerified);
                    Button button = moddedObject.GetComponent<Button>();
                    button.onClick.AddListener(delegate
                    {
                        Character character = CharacterTracker.Instance.GetPlayer();
                        if (character)
                        {
                            PersonalizationController personalizationController = character.GetComponent<PersonalizationController>();
                            if (personalizationController)
                            {
                                personalizationController.EquipSkin(item);
                            }
                        }
                    });

                    if (i % 15 == 0)
                        yield return null;
                }
            }

            m_prevTab = m_tabs.selectedTab?.tabId;
            m_tabs.interactable = true;
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
                m_updateButtonText.text = "Download";
            }
            else
            {
                m_updateButtonText.text = "Update";
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
            cameraManager.SetCameraHolderPosition(Vector3.forward * 0.75f, firstPersonMover);
            cameraManager.SetCameraHolderEulerAngles(Vector3.up * -90f, firstPersonMover);
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

        }

        public void OnUpdateButtonClicked()
        {
            ModUIConstants.ShowDownloadCustomizationAssetsMenu(base.transform);
        }
    }
}
