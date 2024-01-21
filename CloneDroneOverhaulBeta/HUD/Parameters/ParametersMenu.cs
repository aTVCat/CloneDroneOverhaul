using CDOverhaul.Gameplay.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class ParametersMenu : OverhaulUI
    {
        public const string CategoryTranslationPrefix = "PrefC_";
        public const string CategoryDescTranslationPrefix = "PrefCD_";
        public const string SectionTranslationPrefix = "PrefS_";
        public const string SettingTranslationPrefix = "PrefE_";
        public const string SettingDescTranslationPrefix = "PrefED_";
        public const string SettingButtonTranslationPrefix = "PrefEB_";

        public static readonly Color DefaultBarColor = new Color(0.25f, 0.4375f, 1f, 1f);

        public static ParametersMenu Instance;
        public static bool IsActive => Instance != null && Instance.gameObject.activeSelf;

        private static List<string> s_AllCategories;
        private static readonly List<string> s_SortCategories = new List<string>
        {
            "Mod",
            "Gameplay",
            "QoL",
            "Graphics",
            "Audio",
            "Game interface",
            "Shortcuts"
        };
        private static readonly Dictionary<string, List<string>> s_SortSections = new Dictionary<string, List<string>>
        {
            { "Gameplay", new List<string>()
            {
                "Gameplay.Control",
                "Gameplay.Camera",
                "Gameplay.Multiplayer",
                "Gameplay.Voxels",
                "Gameplay.Discord"
            } },

            { "Mod", new List<string>()
            {
                "Mod.Arena",
                "Mod.Information",
            } },

            { "Graphics", new List<string>()
            {
                "Graphics.Settings",
                "Graphics.Amplify Occlusion",
                "Graphics.Post effects",
                "Graphics.Shaders",
                "Graphics.Amplify color",
            } },

            { "Game interface", new List<string>()
            {
                "Game interface.Information",
            } },
        };

        private ModdedObject m_CategoryEntryPrefab;
        private Transform m_CategoryContainer;

        private Transform m_MainContainer;
        private ModdedObject m_PageDescPrefab;
        private ModdedObject m_SectionPrefab;
        private ModdedObject m_SettingPrefab;
        private ModdedObject m_ButtonsContainerPrefab;
        private ModdedObject m_ButtonForContainersPrefab;

        private Graphic[] m_ThemeGraphics;

        private ScrollRect m_ScrollRect;
        private CanvasGroup m_MainCanvasGroup;
        private bool m_IsPopulatingSettings;

        private readonly Dictionary<string, Transform> m_ButtonContainers = new Dictionary<string, Transform>();

        public static bool ShouldSelectShortcuts;

        public bool AllowSwitchingCategories => !m_IsPopulatingSettings;
        public string SelectedCategory { get; set; }

        public override void Initialize()
        {
            Instance = this;
            ShouldSelectShortcuts = false;

            if (s_AllCategories == null)
            {
                s_AllCategories = OverhaulSettingsController.GetAllCategories();
                sortCategories();
            }

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
            m_ScrollRect = MyModdedObject.GetObject<ScrollRect>(13);
            m_ScrollRect.movementType = ScrollRect.MovementType.Clamped;
            MyModdedObject.GetObject<UnityEngine.UI.Button>(5).onClick.AddListener(Hide);

            m_ThemeGraphics = new Graphic[1] { MyModdedObject.GetObject<Image>(22) };

            m_MainCanvasGroup.alpha = 1f;
            m_MainCanvasGroup.interactable = true;

            Hide();
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            Instance = null;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                Hide();
        }

        public void Show()
        {
            if (IsDisposedOrDestroyed())
                return;

            if (GameUIRoot.Instance != null)
            {
                GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(false);
                GameUIRoot.Instance.SettingsMenu.Hide();
            }

            base.gameObject.SetActive(true);
            refreshThemeOutlines();
            populateCategories();
            OverhaulCanvasController.SetCanvasPixelPerfect(false);

            if (ShouldSelectShortcuts)
            {
                ShouldSelectShortcuts = false;
                ParametersMenuCategoryButton.SetSelectedSpecific("Shortcuts");
            }
            else
            {
                ParametersMenuCategoryButton.SetSelectedSpecific("Graphics");
            }
        }

        public void Hide()
        {
            if (IsDisposedOrDestroyed())
                return;

            TransformUtils.DestroyAllChildren(m_MainContainer);
            TransformUtils.DestroyAllChildren(m_CategoryContainer);
            if (GameModeManager.Is(GameMode.None))
                GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(true);

            OverhaulCanvasController.SetCanvasPixelPerfect(true);
            base.gameObject.SetActive(false);
        }

        private void sortCategories()
        {
            foreach (string toSort in s_SortCategories)
            {
                int currentIndex = s_AllCategories.IndexOf(toSort);
                int newIndex = s_SortCategories.IndexOf(toSort);

                s_AllCategories.RemoveAt(currentIndex);
                s_AllCategories.Insert(newIndex, toSort);
            }
        }

        private void sortSections(List<string> sectionsToSort, string currentCategory)
        {
            if (!s_SortSections.ContainsKey(currentCategory) || sectionsToSort.IsNullOrEmpty())
                return;

            List<string> sort = s_SortSections[currentCategory];
            foreach (string toSort in sort)
            {
                int currentIndex = sectionsToSort.IndexOf(toSort);
                int newIndex = sort.IndexOf(toSort);

                sectionsToSort.RemoveAt(currentIndex);
                sectionsToSort.Insert(newIndex, toSort);
            }
        }

        private void refreshThemeOutlines()
        {
            if (m_ThemeGraphics.IsNullOrEmpty())
                return;

            foreach (Image outline in m_ThemeGraphics)
            {
                if (!outline)
                    continue;

                outline.color = OverhaulCombatState.GetUIThemeColor(DefaultBarColor);
            }
        }

        private void populateCategories()
        {
            if (IsDisposedOrDestroyed())
                return;

            TransformUtils.DestroyAllChildren(m_CategoryContainer);
            foreach (string category in s_AllCategories)
            {
                if (category == "Experimental" && !OverhaulVersion.IsDebugBuild)
                    continue;

                ModdedObject categoryEntry = Instantiate(m_CategoryEntryPrefab, m_CategoryContainer);
                categoryEntry.GetObject<Text>(0).text = OverhaulLocalizationController.GetTranslation(CategoryTranslationPrefix + category);
                categoryEntry.GetObject<Image>(1).sprite = OverhaulSettingsController.GetSpriteForCategory(category);
                categoryEntry.GetObject<Image>(2).color = category == "Experimental" ? Color.red : OverhaulCombatState.GetUIThemeColor(DefaultBarColor);
                categoryEntry.gameObject.AddComponent<ParametersMenuCategoryButton>().Initialize(this, categoryEntry, category);
                _ = categoryEntry.gameObject.AddComponent<OverhaulUIButtonScaler>();
                categoryEntry.gameObject.SetActive(true);
            }
        }

        public void PopulateCategory(in string categoryName, bool restorePosition = false)
        {
            if (IsDisposedOrDestroyed() || !base.gameObject.activeSelf || m_IsPopulatingSettings)
                return;

            MyModdedObject.GetObject<Text>(23).text = OverhaulLocalizationController.GetTranslation(CategoryTranslationPrefix + categoryName);
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

            string desc = OverhaulSettingsController.GetCategoryDescription(categoryName);
            if (!string.IsNullOrEmpty(desc))
            {
                ModdedObject categoryDesc = Instantiate(m_PageDescPrefab, m_MainContainer);
                categoryDesc.gameObject.SetActive(true);
                categoryDesc.GetObject<Text>(0).text = OverhaulLocalizationController.GetTranslation(CategoryDescTranslationPrefix + categoryName);
            }

            List<string> sections = OverhaulSettingsController.GetAllSections(categoryName);
            sortSections(sections, categoryName);
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
                List<string> settings = OverhaulSettingsController.GetAllSettings(categoryName, array[1]);
                if (settings.IsNullOrEmpty())
                {
                    continue;
                }

                ModdedObject categoryEntry = Instantiate(m_SectionPrefab, m_MainContainer);
                categoryEntry.gameObject.SetActive(true);
                categoryEntry.GetObject<Text>(0).text = OverhaulLocalizationController.GetTranslation(SectionTranslationPrefix + array[1]);

                foreach (string settingName in settings)
                {
                    List<string> childrenSettings = OverhaulSettingsController.GetChildrenSettings(settingName);
                    PopulateSetting(settingName, childrenSettings.Count == 0 ? ParametersMenuSettingPosition.Normal : ParametersMenuSettingPosition.Top);
                    int index = 0;
                    if (childrenSettings.Count != 0)
                    {
                        foreach (string cSettingName in childrenSettings)
                        {
                            ParametersMenuSettingPosition pos = ParametersMenuSettingPosition.Center;
                            if (childrenSettings.Count - 1 == index)
                                pos = ParametersMenuSettingPosition.Bottom;

                            PopulateSetting(cSettingName, pos);
                            index++;
                        }
                    }
                }
            }

            yield return null;

            if (restorePosition)
                m_ScrollRect.verticalScrollbar.value = prevPos;

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
                return;

            SettingInfo info = OverhaulSettingsController.GetSetting(path, false);
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
    }
}