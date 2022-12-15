using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CloneDroneOverhaul.UI
{
    public class SettingsUI : ModGUIBase
    {
        public static SettingsUI Instance;

        private RectTransform CategoryContainer;
        private ModdedObject CategoryItemPrefab;
        private RectTransform SectionContainer;
        private ModdedObject SectionItemPrefab;

        private RectTransform ViewPort;

        private Button CloseButton;

        private ModdedObject BoolValuePrefab;
        private ModdedObject FloatValuePrefab;
        private ModdedObject EnumValuePrefab;

        private RectTransform ChildrenSettings;
        private RectTransform AdditSettingsContainer;
        private Button CloseChildSettingsButton;

        private InputField SearchField;

        private string selectedCategory;
        private string selectedSection;

        public override void OnInstanceStart()
        {
            base.MyModdedObject = base.GetComponent<ModdedObject>();
            Instance = this;

            CategoryContainer = MyModdedObject.GetObjectFromList<RectTransform>(1);
            CategoryItemPrefab = MyModdedObject.GetObjectFromList<ModdedObject>(0);
            SectionContainer = MyModdedObject.GetObjectFromList<RectTransform>(3);
            SectionItemPrefab = MyModdedObject.GetObjectFromList<ModdedObject>(2);
            ViewPort = MyModdedObject.GetObjectFromList<RectTransform>(4);
            CloseButton = MyModdedObject.GetObjectFromList<Button>(9);
            CloseButton.onClick.AddListener(new UnityEngine.Events.UnityAction(Hide));

            SearchField = MyModdedObject.GetObjectFromList<InputField>(5);
            SearchField.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<string>(searchSettings));
            SearchField.onEndEdit.AddListener(new UnityEngine.Events.UnityAction<string>(onEndSearching));

            ChildrenSettings = MyModdedObject.GetObjectFromList<RectTransform>(10);
            AdditSettingsContainer = MyModdedObject.GetObjectFromList<RectTransform>(13);
            CloseChildSettingsButton = MyModdedObject.GetObjectFromList<Button>(11);
            CloseChildSettingsButton.onClick.AddListener(new UnityEngine.Events.UnityAction(HideAdditSettings));

            BoolValuePrefab = MyModdedObject.GetObjectFromList<ModdedObject>(8);
            FloatValuePrefab = MyModdedObject.GetObjectFromList<ModdedObject>(14);
            EnumValuePrefab = MyModdedObject.GetObjectFromList<ModdedObject>(16);
            Hide();
        }

        public void Hide()
        {
            base.gameObject.SetActive(false);
            HideAdditSettings();
        }

        public void ShowWithOpenedPage(string category, string section)
        {
            Show();
            SelectCategory(category);
            SelectSection(section);
        }
        public void Show()
        {
            GameUIRoot.Instance.SettingsMenu.Hide();
            GetComponent<Animator>().Play("SettingsShow");
            base.gameObject.SetActive(true);
            populateSettings(true, Modules.OverhaulSettingsManager.Instance.GetAllSettings());

            base.MyModdedObject.GetObjectFromList<Text>(17).text = OverhaulMain.GetTranslatedString("FullCDOSettings");
            base.MyModdedObject.GetObjectFromList<Text>(20).text = OverhaulMain.GetTranslatedString("PressSpacebar");
            base.MyModdedObject.GetObjectFromList<Text>(19).text = OverhaulMain.GetTranslatedString("FindASetting");
            base.MyModdedObject.GetObjectFromList<Text>(18).text = OverhaulMain.GetTranslatedString("ToBeFilled");
        }

        public void SelectCategory(string str)
        {
            selectedSection = string.Empty;
            selectedCategory = str;
            populateSections();
        }
        public void SelectSection(string str)
        {
            selectedSection = str;
            populateSettings(false, Modules.OverhaulSettingsManager.Instance.GetSettings(selectedCategory, selectedSection));
        }
        public void ShowAdditionalSettings(Modules.OverhaulSettingsManager.SettingEntry entry)
        {
            ChildrenSettings.gameObject.SetActive(true);
            TransformUtils.DestroyAllChildren(AdditSettingsContainer);
            List<Modules.OverhaulSettingsManager.SettingEntry> list = Modules.OverhaulSettingsManager.Instance.GetSettings(entry.ChildSettings.ChildrenSettingID);
            foreach (Modules.OverhaulSettingsManager.SettingEntry entry2 in list)
            {
                populateSetting(entry2, AdditSettingsContainer, true);
            }
        }
        public void HideAdditSettings()
        {
            ChildrenSettings.gameObject.SetActive(false);
        }
        private void searchSettings(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                populateSettings(false, Modules.OverhaulSettingsManager.Instance.GetSettings(selectedCategory, selectedSection));
            }
            populateSettings(false, Modules.OverhaulSettingsManager.Instance.SearchSettings(str));
        }
        private void onEndSearching(string str)
        {
            //OverhaulMain.Timer.AddNoArgActionToCompleteNextFrame(onEndSearchingNextFrame);
        }
        private void onEndSearchingNextFrame()
        {
            SearchField.text = "";
            populateSettings(false, Modules.OverhaulSettingsManager.Instance.GetSettings(selectedCategory, selectedSection));
        }

        private void populateSections()
        {
            List<string> spawnedSections = new List<string>();
            TransformUtils.DestroyAllChildren(SectionContainer);
            if (string.IsNullOrEmpty(selectedSection) && !string.IsNullOrEmpty(selectedCategory))
            {
                List<Modules.OverhaulSettingsManager.SettingEntry.CategoryPath> paths = Modules.OverhaulSettingsManager.Instance.GetPaths(selectedCategory);
                foreach (Modules.OverhaulSettingsManager.SettingEntry.CategoryPath pathhh in paths)
                {
                    if (!spawnedSections.Contains(pathhh.Section))
                    {
                        ModdedObject mObj = Instantiate<ModdedObject>(SectionItemPrefab, SectionContainer);
                        mObj.GetObjectFromList<Text>(0).text = OverhaulMain.GetTranslatedString("Settings_Section_" + Modules.OverhaulSettingsManager.Instance.GetSectionName(pathhh.Section));
                        spawnedSections.Add(pathhh.Section);
                        mObj.gameObject.SetActive(true);
                        CategoryButton button = mObj.gameObject.AddComponent<CategoryButton>();
                        button.SectionName = pathhh.Section;
                    }
                }
            }
        }

        private void populateSettings(bool onlyCategories, List<Modules.OverhaulSettingsManager.SettingEntry> settingsList)
        {
            List<Modules.OverhaulSettingsManager.SettingEntry> settings = settingsList;

            List<string> spawnedCategories = new List<string>();

            if (onlyCategories)
            {
                TransformUtils.DestroyAllChildren(CategoryContainer);
            }

            base.MyModdedObject.GetObjectFromList<TMPro.TextMeshProUGUI>(15).text = string.Empty;
            Modules.OverhaulSettingsManager.SettingEntry.CategoryPath path = Modules.OverhaulSettingsManager.Instance.GetPageData(selectedCategory, selectedSection);
            if (!path.IsEmpty)
            {
                base.MyModdedObject.GetObjectFromList<TMPro.TextMeshProUGUI>(15).text = path.SectionPageDescription;
            }
            TransformUtils.DestroyAllChildren(ViewPort);

            foreach (Modules.OverhaulSettingsManager.SettingEntry entry in settings)
            {
                if (!entry.ForceHide && onlyCategories)
                {
                    if (!spawnedCategories.Contains(entry.Path.Category))
                    {
                        ModdedObject mObj = Instantiate<ModdedObject>(CategoryItemPrefab, CategoryContainer);
                        mObj.GetObjectFromList<Text>(0).text = OverhaulMain.GetTranslatedString("Settings_Category_" + Modules.OverhaulSettingsManager.Instance.GetCategoryName(entry.Path.Category));
                        spawnedCategories.Add(entry.Path.Category);
                        mObj.gameObject.SetActive(true);
                        CategoryButton button = mObj.gameObject.AddComponent<CategoryButton>();
                        button.CategoryName = entry.Path.Category;
                    }
                    goto IL_0000;
                }

                populateSetting(entry, ViewPort, false);

            IL_0000:;
            }
        }

        void populateSetting(Modules.OverhaulSettingsManager.SettingEntry entry, Transform parent, bool ignoreIsHidden)
        {
            if (entry.ForceHide)
            {
                return;
            }

            if (!ignoreIsHidden && entry.IsHidden)
            {
                return;
            }
            ModdedObject setting = null;
            if (entry.Type == typeof(bool))
            {
                setting = Instantiate<ModdedObject>(BoolValuePrefab, parent);
                setting.gameObject.AddComponent<UISettingEntry>().SetupValue(entry.Type, entry, CloneDroneOverhaulDataContainer.Instance.SettingsData.GetSettingValue(entry.ID));
            }
            else if (entry.Type == typeof(float))
            {
                setting = Instantiate<ModdedObject>(FloatValuePrefab, parent);
                setting.gameObject.AddComponent<UISettingEntry>().SetupValue(entry.Type, entry, CloneDroneOverhaulDataContainer.Instance.SettingsData.GetSettingValue(entry.ID));
            }
            else if (entry.Type == typeof(int))
            {
                setting = Instantiate<ModdedObject>(FloatValuePrefab, parent);
                setting.gameObject.AddComponent<UISettingEntry>().SetupValue(entry.Type, entry, CloneDroneOverhaulDataContainer.Instance.SettingsData.GetSettingValue(entry.ID));
            }
            else if (entry.Type.IsEnum)
            {
                setting = Instantiate<ModdedObject>(EnumValuePrefab, parent);
                setting.gameObject.AddComponent<UISettingEntry>().SetupValue(entry.Type, entry, CloneDroneOverhaulDataContainer.Instance.SettingsData.GetSettingValue(entry.ID), entry.ValueSettings.DropdownEnumType);
            }
            if (setting == null)
            {
                Modules.ModuleManagement.ShowError("Unsupported setting type! " + entry.Type.ToString());
            }
            else
            {
                setting.gameObject.SetActive(true);
            }
            setting.GetObjectFromList<Text>(0).text = OverhaulMain.GetTranslatedString("SName_" + entry.ID);
            setting.GetObjectFromList<Text>(3).text = OverhaulMain.GetTranslatedString("SDesc_" + entry.ID);
            setting.GetObjectFromList<InputField>(2).text = entry.ID;
            setting.GetObjectFromList<InputField>(2).gameObject.SetActive(OverhaulDescription.IsBetaBuild());
        }

        private class UISettingEntry : MonoBehaviour
        {
            private ModdedObject MyModdedObject;
            private Modules.OverhaulSettingsManager.SettingEntry Entry;
            private System.Type Type;
            private Modules.OverhaulSettingsManager.SettingEntry.UIValueSettings Settings;

            public void SetupValue(Type type, Modules.OverhaulSettingsManager.SettingEntry sEntry, object value, Type enumerator = null)
            {
                Entry = sEntry;
                Settings = sEntry.ValueSettings;
                MyModdedObject = base.GetComponent<ModdedObject>();
                Type = type;

                MyModdedObject.GetObjectFromList<RectTransform>(4).gameObject.SetActive(Entry.HasChildSettings);
                MyModdedObject.GetObjectFromList<Button>(4).onClick.AddListener(new UnityEngine.Events.UnityAction(ShowAdditSettings));

                if (type == typeof(bool))
                {
                    base.GetComponent<Toggle>().isOn = (bool)value;
                    base.GetComponent<Toggle>().onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>(ToggleBoolValue));
                }
                if (type.IsEnum)
                {
                    Dropdown dr = MyModdedObject.GetObjectFromList<Dropdown>(5);
                    List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();
                    foreach (string str in type.GetEnumNames())
                    {
                        list.Add(new Dropdown.OptionData(str));
                    }
                    dr.options = list;
                    dr.value = (int)value;
                    dr.onValueChanged.AddListener(SetEnumValue);

                }
                if (type == typeof(float))
                {
                    if (Entry.ValueSettings != null)
                    {
                        Slider slider = MyModdedObject.GetObjectFromList<Slider>(5);
                        slider.wholeNumbers = Entry.ValueSettings.OnlyInt;
                        slider.minValue = Entry.ValueSettings.MinValue;
                        slider.maxValue = Entry.ValueSettings.MaxValue;
                        slider.value = (float)value;
                        slider.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<float>(SetFloatValue));
                        string valtxt = ((float)value * (Settings.Step == -1 ? 1 : Settings.Step)).ToString();
                        if (valtxt.Length > 4)
                        {
                            valtxt = valtxt.Remove(4);
                        }
                        MyModdedObject.GetObjectFromList<Text>(6).text = "[" + valtxt + "]";
                    }
                    else
                    {
                        Modules.ModuleManagement.ShowError("Settings of type Float or Int require UIValueSettings to be filled " + Entry.Name + "(" + Entry.Type.ToString() + ")");
                    }
                }
            }

            void ShowAdditSettings()
            {
                GUIManagement.Instance.GetGUI<UI.SettingsUI>().ShowAdditionalSettings(Entry);
            }

            void ToggleBoolValue(bool val)
            {
                CloneDroneOverhaulDataContainer.Instance.SettingsData.SaveSetting(Entry.ID, val, false);
            }
            void SetFloatValue(float val)
            {
                CloneDroneOverhaulDataContainer.Instance.SettingsData.SaveSetting(Entry.ID, val, false);
                string valtxt = (val * (Settings.Step == -1 ? 1 : Settings.Step)).ToString();
                if (valtxt.Length > 4)
                {
                    valtxt = valtxt.Remove(4);
                }
                MyModdedObject.GetObjectFromList<Text>(6).text = "[" + valtxt + "]";
            }
            void SetEnumValue(int num)
            {
                CloneDroneOverhaulDataContainer.Instance.SettingsData.SaveSetting(Entry.ID, num, false);
            }
        }

        private class CategoryButton : MonoBehaviour
        {
            public string CategoryName;
            public string SectionName;

            private void Awake()
            {
                GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(OnButtonClicked));
            }

            private void OnButtonClicked()
            {
                string str = string.IsNullOrEmpty(CategoryName) ? SectionName : CategoryName;
                bool isCategory = !string.IsNullOrEmpty(CategoryName);

                if (isCategory)
                {
                    UI.SettingsUI.Instance.SelectCategory(str);
                }
                else
                {
                    UI.SettingsUI.Instance.SelectSection(str);
                }
            }
        }
    }
}
