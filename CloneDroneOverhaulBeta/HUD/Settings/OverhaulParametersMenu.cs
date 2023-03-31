using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class OverhaulParametersMenu : OverhaulUI
    {
        private ModdedObject m_CategoryEntryPrefab;
        private Transform m_CategoryContainer;

        private Transform m_MainContainer;
        private ModdedObject m_SectionPrefab;
        private ModdedObject m_SettingPrefab;

        private Transform m_DescriptionTransform;

        private ScrollRect m_ScrollRect;
        private CanvasGroup m_MainCanvasGroup;
        private bool m_IsPopulatingSettings;

        public override void Initialize()
        {
            m_CategoryEntryPrefab = MyModdedObject.GetObject<ModdedObject>(1);
            m_CategoryEntryPrefab.gameObject.SetActive(false);
            m_CategoryContainer = MyModdedObject.GetObject<Transform>(2);
            m_MainContainer = MyModdedObject.GetObject<Transform>(3);
            m_MainCanvasGroup = MyModdedObject.GetObject<CanvasGroup>(3);
            m_SectionPrefab = MyModdedObject.GetObject<ModdedObject>(4);
            m_SectionPrefab.gameObject.SetActive(false);
            m_SettingPrefab = MyModdedObject.GetObject<ModdedObject>(6);
            m_SettingPrefab.gameObject.SetActive(false);
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

            TitleScreenUI tUI = GameUIRoot.Instance.TitleScreenUI;
            if (tUI.gameObject.activeSelf)
            {
                tUI.SetLogoAndRootButtonsVisible(false);
            }

            base.gameObject.SetActive(true);
            populateCategories();
            PopulateDescription(null, null);
        }

        public void Hide()
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            TransformUtils.DestroyAllChildren(m_MainContainer);
            TransformUtils.DestroyAllChildren(m_CategoryContainer);
            TitleScreenUI tUI = GameUIRoot.Instance.TitleScreenUI;
            if (tUI.gameObject.activeSelf)
            {
                tUI.SetLogoAndRootButtonsVisible(true);
            }
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
                categoryEntry.GetObject<Text>(0).text = category;
                categoryEntry.gameObject.AddComponent<ParametersMenuCategoryButton>().Initialize(this, categoryEntry, category);
            }
        }

        public void PopulateCategory(in string categoryName)
        {
            if (IsDisposedOrDestroyed() || !base.gameObject.activeSelf || m_IsPopulatingSettings)
            {
                return;
            }

            StaticCoroutineRunner.StartStaticCoroutine(populateCategoryCoroutine(categoryName));
        }

        private IEnumerator populateCategoryCoroutine(string categoryName)
        {
            m_IsPopulatingSettings = true;

            m_MainCanvasGroup.alpha = 1f;
            for (int i = 0; i < 4; i++)
            {
                m_MainCanvasGroup.alpha -= 0.25f;
                yield return new WaitForSecondsRealtime(0.016f);
            }
            m_MainCanvasGroup.alpha = 0f;

            m_ScrollRect.verticalScrollbar.value = 1f;
            m_ScrollRect.normalizedPosition = new Vector2(0f, 1f);
            yield return null;
            TransformUtils.DestroyAllChildren(m_MainContainer);
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
                categoryEntry.GetObject<Text>(0).text = array[1];

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

            ModdedObject setting = Instantiate(m_SettingPrefab, m_MainContainer);
            setting.gameObject.SetActive(true);
            setting.gameObject.AddComponent<ParametersMenuSetting>().Initialize(this, setting, path, position);
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

            MyModdedObject.GetObject<Text>(8).text = info.Name;
            MyModdedObject.GetObject<Text>(9).text = description.Description;
            MyModdedObject.GetObject<UnityEngine.UI.Image>(10).gameObject.SetActive(description.Has43Image);
            MyModdedObject.GetObject<UnityEngine.UI.Image>(11).gameObject.SetActive(description.Has169Image);
            MyModdedObject.GetObject<UnityEngine.UI.Image>(10).sprite = description.Image_4_3;
            MyModdedObject.GetObject<UnityEngine.UI.Image>(11).sprite = description.Image_16_9;
        }
    }
}