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
            Hide();
        }

        public void Hide()
        {
            base.gameObject.SetActive(false);
            HideAdditSettings();
        }

        public void Show()
        {
            GameUIRoot.Instance.SettingsMenu.Hide();
            GetComponent<Animator>().Play("SettingsShow");
            base.gameObject.SetActive(true);
            populateSettings(true, Modules.SettingsManager.Instance.GetAllSettings());
        }

        public void SelectCategory(string str)
        {
            selectedSection = string.Empty;
            selectedCategory = str;
            populateSettings(false, Modules.SettingsManager.Instance.GetSettings(selectedCategory, selectedSection));
        }
        public void SelectSection(string str)
        {
            selectedSection = str;
            populateSettings(false, Modules.SettingsManager.Instance.GetSettings(selectedCategory, selectedSection));
        }
        public void ShowAdditionalSettings(Modules.SettingsManager.SettingEntry entry)
        {
            ChildrenSettings.gameObject.SetActive(true);
            TransformUtils.DestroyAllChildren(AdditSettingsContainer);
            List<Modules.SettingsManager.SettingEntry> list = Modules.SettingsManager.Instance.GetSettings(entry.ChildSettings.ChildrenSettingID);
            foreach(Modules.SettingsManager.SettingEntry entry2 in list)
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
                populateSettings(false, Modules.SettingsManager.Instance.GetSettings(selectedCategory, selectedSection));
            }
            populateSettings(false, Modules.SettingsManager.Instance.SearchSettings(str));
        }
        private void onEndSearching(string str)
        {
            OverhaulMain.Timer.AddNoArgActionToCompleteNextFrame(onEndSearchingNextFrame);
        }
        private void onEndSearchingNextFrame()
        {
            SearchField.text = "";
            populateSettings(false, Modules.SettingsManager.Instance.GetSettings(selectedCategory, selectedSection));
        }

        private void populateSettings(bool onlyCategories, List<Modules.SettingsManager.SettingEntry> settingsList)
        {
            List<Modules.SettingsManager.SettingEntry> settings = settingsList;

            List<string> spawnedCategories = new List<string>();
            List<string> spawnedSections = new List<string>();

            if (onlyCategories)
            {
                TransformUtils.DestroyAllChildren(CategoryContainer);
            }

            TransformUtils.DestroyAllChildren(SectionContainer);
            TransformUtils.DestroyAllChildren(ViewPort);

            foreach (Modules.SettingsManager.SettingEntry entry in settings)
            {
                if (onlyCategories)
                {
                    if (!spawnedCategories.Contains(entry.Path.Category))
                    {
                        ModdedObject mObj = Instantiate<ModdedObject>(CategoryItemPrefab, CategoryContainer);
                        mObj.GetObjectFromList<Text>(0).text = entry.Path.Category;
                        spawnedCategories.Add(entry.Path.Category);
                        mObj.gameObject.SetActive(true);
                        CategoryButton button = mObj.gameObject.AddComponent<CategoryButton>();
                        button.CategoryName = entry.Path.Category;
                    }
                    goto IL_0000;
                }

                if (!spawnedSections.Contains(entry.Path.Section))
                {
                    ModdedObject mObj = Instantiate<ModdedObject>(SectionItemPrefab, SectionContainer);
                    mObj.GetObjectFromList<Text>(0).text = entry.Path.Section;
                    spawnedSections.Add(entry.Path.Section);
                    mObj.gameObject.SetActive(true);
                    CategoryButton button = mObj.gameObject.AddComponent<CategoryButton>();
                    button.SectionName = entry.Path.Section;
                }

                populateSetting(entry, ViewPort, false);

                IL_0000:;
            }
        }

        void populateSetting(Modules.SettingsManager.SettingEntry entry, Transform parent, bool ignoreIsHidden)
        {
            if (entry.Path.Section == selectedSection && entry.Path.Category == selectedCategory)
            {
            }

            if (!ignoreIsHidden && entry.IsHidden)
            {
                return;
            }
            ModdedObject setting = null;
            if (entry.Type == typeof(bool))
            {
                setting = Instantiate<ModdedObject>(BoolValuePrefab, parent);
                setting.GetObjectFromList<Text>(0).text = entry.Name;
                setting.GetObjectFromList<Text>(3).text = entry.Description;
                setting.GetObjectFromList<InputField>(2).text = entry.ID;
                setting.gameObject.AddComponent<UISettingEntry>().SetupValue<bool>(entry, CloneDroneOverhaulDataContainer.Instance.SettingsData.GetSettingValue(entry.ID));
            }
            else if (entry.Type == typeof(float))
            {
                setting = Instantiate<ModdedObject>(FloatValuePrefab, parent);
                setting.GetObjectFromList<Text>(0).text = entry.Name;
                setting.GetObjectFromList<Text>(3).text = entry.Description;
                setting.GetObjectFromList<InputField>(2).text = entry.ID;
                setting.gameObject.AddComponent<UISettingEntry>().SetupValue<float>(entry, CloneDroneOverhaulDataContainer.Instance.SettingsData.GetSettingValue(entry.ID));
            }
            if (setting == null)
            {
                Modules.ModuleManagement.ShowError("Unsupported setting type! " + entry.Type.ToString());
            }
            else
            {
                setting.gameObject.SetActive(true);
            }
        }

        private class UISettingEntry : MonoBehaviour
        {
            private ModdedObject MyModdedObject;
            private Modules.SettingsManager.SettingEntry Entry;
            private System.Type Type;

            public void SetupValue<T>(Modules.SettingsManager.SettingEntry sEntry, object value)
            {
                Entry = sEntry;
                MyModdedObject = base.GetComponent<ModdedObject>();
                Type = typeof(T);
                if (typeof(T) == typeof(bool))
                {
                    base.GetComponent<Toggle>().isOn = (bool)value;
                    base.GetComponent<Toggle>().onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>(ToggleBoolValue));
                    MyModdedObject.GetObjectFromList<RectTransform>(4).gameObject.SetActive(Entry.HasChildSettings);
                    MyModdedObject.GetObjectFromList<Button>(4).onClick.AddListener(new UnityEngine.Events.UnityAction(ShowAdditSettings));
                }
                if (typeof(T) == typeof(float))
                {
                    //base.GetComponent<Toggle>().isOn = (bool)value;
                    //base.GetComponent<Toggle>().onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>(ToggleBoolValue));
                    MyModdedObject.GetObjectFromList<RectTransform>(4).gameObject.SetActive(Entry.HasChildSettings);
                    MyModdedObject.GetObjectFromList<Button>(4).onClick.AddListener(new UnityEngine.Events.UnityAction(ShowAdditSettings));
                    if(Entry.ValueSettings != null)
                    {
                        Slider slider = MyModdedObject.GetObjectFromList<Slider>(5);
                        slider.wholeNumbers = false;
                        slider.minValue = Entry.ValueSettings.MinValue;
                        slider.maxValue = Entry.ValueSettings.MaxValue;
                        slider.value = (float)value;
                        slider.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<float>(SetFloatValue));
                        string valtxt = ((float)value).ToString();
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
                BaseStaticReferences.GUIs.GetGUI<UI.SettingsUI>().ShowAdditionalSettings(Entry);
            }

            void ToggleBoolValue(bool val)
            {
                CloneDroneOverhaulDataContainer.Instance.SettingsData.SaveSetting(Entry.ID, val, false);
            }
            void SetFloatValue(float val)
            {
                CloneDroneOverhaulDataContainer.Instance.SettingsData.SaveSetting(Entry.ID, val, false);
                string valtxt = val.ToString();
                if(valtxt.Length > 4)
                {
                    valtxt = valtxt.Remove(4);
                }
                MyModdedObject.GetObjectFromList<Text>(6).text = "[" + valtxt + "]";
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
