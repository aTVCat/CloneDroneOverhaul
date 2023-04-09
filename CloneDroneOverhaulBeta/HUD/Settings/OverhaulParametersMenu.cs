using CDOverhaul.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class OverhaulParametersMenu : OverhaulUI
    {
        public const string CategoryTranslationPrefix = "PrefC_";
        public const string CategoryDescTranslationPrefix = "PrefCD_";
        public const string SectionTranslationPrefix = "PrefS_";
        public const string SettingTranslationPrefix = "PrefE_";
        public const string SettingDescTranslationPrefix = "PrefED_";
        public const string SettingButtonTranslationPrefix = "PrefEB_";

        private ModdedObject m_CategoryEntryPrefab;
        private Transform m_CategoryContainer;

        private Transform m_MainContainer;
        private ModdedObject m_PageDescPrefab;
        private ModdedObject m_SectionPrefab;
        private ModdedObject m_SettingPrefab;
        private ModdedObject m_ButtonsContainerPrefab;
        private ModdedObject m_ButtonForContainersPrefab;

        private Transform m_DescriptionTransform;

        private ScrollRect m_ScrollRect;
        private CanvasGroup m_MainCanvasGroup;
        private bool m_IsPopulatingSettings;

        private Dictionary<string, Transform> m_ButtonContainers = new Dictionary<string, Transform>();

        public static bool ShouldSelectShortcuts;

        public bool AllowSwitchingCategories => !m_IsPopulatingSettings;
        public string SelectedCategory { get; set; }

        public override void Initialize()
        {
            OverhaulParametersMenu.ShouldSelectShortcuts = false;
            m_CategoryEntryPrefab = MyModdedObject.GetObject<ModdedObject>(1);
            m_CategoryEntryPrefab.gameObject.SetActive(false);
            m_CategoryContainer = MyModdedObject.GetObject<Transform>(2);
            m_MainContainer = MyModdedObject.GetObject<Transform>(3);
            m_MainCanvasGroup = MyModdedObject.GetObject<CanvasGroup>(3);
            m_SectionPrefab = MyModdedObject.GetObject<ModdedObject>(4);
            m_SectionPrefab.gameObject.SetActive(false);
            m_SettingPrefab = MyModdedObject.GetObject<ModdedObject>(6);
            m_SettingPrefab.gameObject.SetActive(false);
            m_PageDescPrefab = MyModdedObject.GetObject<ModdedObject>(14);
            m_PageDescPrefab.gameObject.SetActive(false);
            m_ButtonsContainerPrefab = MyModdedObject.GetObject<ModdedObject>(15);
            m_ButtonsContainerPrefab.gameObject.SetActive(false);
            m_ButtonForContainersPrefab = MyModdedObject.GetObject<ModdedObject>(16);
            m_ButtonForContainersPrefab.gameObject.SetActive(false);
            m_DescriptionTransform = MyModdedObject.GetObject<Transform>(7);
            m_ScrollRect = MyModdedObject.GetObject<ScrollRect>(13);
            m_ScrollRect.movementType = ScrollRect.MovementType.Clamped;
            MyModdedObject.GetObject<UnityEngine.UI.Button>(5).onClick.AddListener(Hide);

            m_MainCanvasGroup.alpha = 1f;
            m_MainCanvasGroup.interactable = true;

            Hide();
        }

        protected override void OnDisposed()
        {
            m_CategoryContainer = null;
            m_CategoryEntryPrefab = null;
            m_MainContainer = null;
            m_SectionPrefab = null;
            m_SettingPrefab = null;
            m_DescriptionTransform = null;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Hide();
            }
        }

        public void Show()
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            if (GameUIRoot.Instance != null)
            {
                TitleScreenUI tUI = GameUIRoot.Instance.TitleScreenUI;
                if (tUI != null && tUI.gameObject.activeSelf)
                {
                    tUI.SetLogoAndRootButtonsVisible(false);
                }

                SettingsMenu mUI = GameUIRoot.Instance.SettingsMenu;
                if (mUI != null && mUI.gameObject.activeSelf)
                {
                    mUI.Hide();
                }
            }

            base.gameObject.SetActive(true);
            populateCategories();
            PopulateDescription(null, null);
            OverhaulCanvasController.SetCanvasPixelPerfect(false);

            if (ShouldSelectShortcuts)
            {
                ShouldSelectShortcuts = false;
                ParametersMenuCategoryButton.SetSelectedSpecific("Shortcuts");
                return;
            }

            ParametersMenuCategoryButton.SetSelectedSpecific("Graphics");
        }

        public void Hide()
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            TransformUtils.DestroyAllChildren(m_MainContainer);
            TransformUtils.DestroyAllChildren(m_CategoryContainer);
            if (GameUIRoot.Instance != null)
            {
                TitleScreenUI tUI = GameUIRoot.Instance.TitleScreenUI;
                if (tUI != null && tUI.gameObject.activeSelf)
                {
                    tUI.SetLogoAndRootButtonsVisible(true);
                }
            }

            OverhaulCanvasController.SetCanvasPixelPerfect(true);
            base.gameObject.SetActive(false);
        }

        private void populateCategories()
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            TransformUtils.DestroyAllChildren(m_CategoryContainer);
            List<string> categories = SettingsController.GetAllCategories();
            foreach (string category in categories)
            {
                ModdedObject categoryEntry = Instantiate(m_CategoryEntryPrefab, m_CategoryContainer);
                categoryEntry.gameObject.SetActive(true);
                categoryEntry.GetObject<Text>(0).text = OverhaulLocalizationController.GetTranslation(CategoryTranslationPrefix + category);
                categoryEntry.GetObject<UnityEngine.UI.Image>(1).sprite = SettingsController.GetSpriteForCategory(category);
                categoryEntry.gameObject.AddComponent<ParametersMenuCategoryButton>().Initialize(this, categoryEntry, category);
            }
        }

        public void PopulateCategory(in string categoryName, bool restorePosition = false)
        {
            if (IsDisposedOrDestroyed() || !base.gameObject.activeSelf || m_IsPopulatingSettings)
            {
                return;
            }

            _ = StaticCoroutineRunner.StartStaticCoroutine(populateCategoryCoroutine(categoryName, restorePosition));
        }

        private IEnumerator populateCategoryCoroutine(string categoryName, bool restorePosition)
        {
            m_IsPopulatingSettings = true;
            SelectedCategory = categoryName;
            float prevPos = m_ScrollRect.verticalScrollbar.value;

            m_MainCanvasGroup.alpha = 1f;
            for (int i = 0; i < 4; i++)
            {
                m_MainCanvasGroup.alpha -= 0.25f;
                yield return new WaitForSecondsRealtime(0.016f);
            }
            m_MainCanvasGroup.alpha = 0f;

            m_ScrollRect.verticalScrollbar.value = 1f;
            yield return null;
            m_ButtonContainers.Clear();
            TransformUtils.DestroyAllChildren(m_MainContainer);

            string desc = SettingsController.GetCategoryDescription(categoryName);
            if (!string.IsNullOrEmpty(desc))
            {
                ModdedObject categoryDesc = Instantiate(m_PageDescPrefab, m_MainContainer);
                categoryDesc.gameObject.SetActive(true);
                categoryDesc.GetObject<Text>(0).text = OverhaulLocalizationController.GetTranslation(CategoryDescTranslationPrefix + categoryName);
            }

            List<string> sections = SettingsController.GetAllSections(categoryName);
            foreach (string sectionName in sections)
            {
                yield return null;
                if (!base.gameObject.activeSelf)
                {
                    m_MainCanvasGroup.alpha = 1f;
                    m_IsPopulatingSettings = false;
                    yield break;
                }

                string[] array = sectionName.Split('.');
                ModdedObject categoryEntry = Instantiate(m_SectionPrefab, m_MainContainer);
                categoryEntry.gameObject.SetActive(true);
                categoryEntry.GetObject<Text>(0).text = OverhaulLocalizationController.GetTranslation(SectionTranslationPrefix + array[1]);

                List<string> settings = SettingsController.GetAllSettings(categoryName, array[1]);
                foreach (string settingName in settings)
                {
                    List<string> childrenSettings = SettingsController.GetChildrenSettings(settingName);
                    PopulateSetting(settingName, childrenSettings.Count == 0 ? ParametersMenuSettingPosition.Normal : ParametersMenuSettingPosition.Top);
                    int index = 0;
                    if (childrenSettings.Count != 0)
                    {
                        foreach (string cSettingName in childrenSettings)
                        {
                            ParametersMenuSettingPosition pos = ParametersMenuSettingPosition.Center;
                            if (childrenSettings.Count - 1 == index)
                            {
                                pos = ParametersMenuSettingPosition.Bottom;
                            }
                            PopulateSetting(cSettingName, pos);
                            index++;
                        }
                    }
                }
            }
            yield return null;
            yield return null;

            if (restorePosition)
            {
                m_ScrollRect.verticalScrollbar.value = prevPos;
            }

            for (int i = 0; i < 4; i++)
            {
                m_MainCanvasGroup.alpha += 0.25f;
                yield return new WaitForSecondsRealtime(0.016f);
            }
            m_MainCanvasGroup.alpha = 1f;
            m_IsPopulatingSettings = false;

            yield break;
        }

        public void PopulateSetting(in string path, in ParametersMenuSettingPosition position)
        {
            if (IsDisposedOrDestroyed() || !base.gameObject.activeSelf)
            {
                return;
            }

            SettingInfo info = SettingsController.GetSetting(path, false);
            if (info != null)
            {
                if (info.EventDispatcher != null)
                {
                    string bpath = info.Category + '.' + info.Section;
                    bool hasContainer = m_ButtonContainers.ContainsKey(bpath);
                    if (!hasContainer)
                    {
                        ModdedObject con = Instantiate(m_ButtonsContainerPrefab, m_MainContainer);
                        con.gameObject.SetActive(true);
                        m_ButtonContainers.Add(bpath, con.GetObject<Transform>(0));
                    }

                    ModdedObject bcButton = Instantiate(m_ButtonForContainersPrefab, m_ButtonContainers[bpath]);
                    bcButton.gameObject.SetActive(true);
                    bcButton.GetComponent<Button>().onClick.AddListener(info.EventDispatcher.DispatchEvent);
                    bcButton.GetComponent<Button>().interactable = info.EventDispatcher.CanBeShown == null || info.EventDispatcher.CanBeShown();
                    bcButton.GetObject<Text>(0).text = OverhaulLocalizationController.GetTranslation(SettingButtonTranslationPrefix + info.Name);

                    return;
                }

                ModdedObject setting = Instantiate(m_SettingPrefab, m_MainContainer);
                setting.gameObject.SetActive(true);
                setting.gameObject.AddComponent<ParametersMenuSetting>().Initialize(this, setting, path, position);
            }
        }

        public void PopulateDescription(in SettingInfo info, in SettingDescription description)
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            if (info == null || description == null)
            {
                m_DescriptionTransform.gameObject.SetActive(false);
                return;
            }
            m_DescriptionTransform.gameObject.SetActive(true);

            MyModdedObject.GetObject<Text>(8).text = OverhaulLocalizationController.GetTranslation(OverhaulParametersMenu.SettingTranslationPrefix + info.Name);
            MyModdedObject.GetObject<Text>(9).text = OverhaulLocalizationController.GetTranslation(OverhaulParametersMenu.SettingDescTranslationPrefix + info.Name);
            MyModdedObject.GetObject<UnityEngine.UI.Image>(10).gameObject.SetActive(description.Has43Image);
            MyModdedObject.GetObject<UnityEngine.UI.Image>(11).gameObject.SetActive(description.Has169Image);
            MyModdedObject.GetObject<UnityEngine.UI.Image>(10).sprite = description.Image_4_3;
            MyModdedObject.GetObject<UnityEngine.UI.Image>(11).sprite = description.Image_16_9;
        }
    }
}