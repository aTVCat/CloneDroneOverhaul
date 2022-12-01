using CloneDroneOverhaul.PooledPrefabs;
using CloneDroneOverhaul.Utilities;
using ModLibrary;
using UnityEngine;
using UnityEngine.Rendering;
using UnityStandardAssets.ImageEffects;
using System.Collections.Generic;
using System.Linq;
using static System.Collections.Specialized.BitVector32;
using PicaVoxel;
using System;
using AmplifyOcclusion;

namespace CloneDroneOverhaul.Modules
{
    public class OverhaulSettingsManager : ModuleBase
    {
        private List<SettingEntry> Entries = new List<SettingEntry>();
        private List<SettingEntry.CategoryPath> PageDescriptions = new List<SettingEntry.CategoryPath>();
        private List<SettingEntry.OverridePrefs> PageOverrides = new List<SettingEntry.OverridePrefs>();
        public static OverhaulSettingsManager Instance;
        public static CloneDroneOverhaulSettingsData Data
        {
            get
            {
                return CloneDroneOverhaulDataContainer.Instance.SettingsData;
            }
        }

        public override void Start()
        {
            BaseStaticReferences.SettingsManager = this;
            Instance = this;

            AddSetting(SettingEntry.NewSetting<float>("Roll Multipler", "", "Graphics", "Additions", 1.00f, null, new SettingEntry.UIValueSettings() { MinValue = 0.5f, MaxValue = 2 }));
            AddSetting(SettingEntry.NewSetting<float>("(Multiplayer) Roll Multipler", "", "Graphics", "Additions", 0.65f, null, new SettingEntry.UIValueSettings() { MinValue = 0.5f, MaxValue = 2 }));
            AddSetting(SettingEntry.NewSetting<bool>("Camera Rolling", "Camera will change its angle depending on your movement", "Graphics", "Additions", true, new SettingEntry.ChildrenSettings() { ChildrenSettingID = new string[] { "Graphics.Additions.Roll Multipler", "Graphics.Additions.(Multiplayer) Roll Multipler" } }));

            AddSetting(SettingEntry.NewSetting<SampleCountLevel>("Sample count", "", "Graphics", "Additions", 1, null, new SettingEntry.UIValueSettings() { DropdownEnumType = typeof(AmplifyOcclusion.SampleCountLevel)}));
            AddSetting(SettingEntry.NewSetting<float>("Noise Multipler", "", "Graphics", "Additions", 1.00f, null, new SettingEntry.UIValueSettings() { MinValue = 0.8f, MaxValue = 1.3f}));
            AddSetting(SettingEntry.NewSetting<float>("Occlusion intensity", "", "Graphics", "Additions", 0.85f, null, new SettingEntry.UIValueSettings() { MinValue = 0.5f, MaxValue = 1.3f }));
            AddSetting(SettingEntry.NewSetting<bool>("Amplify occlusion", "Makes the game more realistic\nNot recommended on low-end PCs", "Graphics", "Additions", true, new SettingEntry.ChildrenSettings() { ChildrenSettingID = new string[] { "Graphics.Additions.Sample count", "Graphics.Additions.Occlusion intensity" } }));
            AddSetting(SettingEntry.NewSetting<bool>("Noise effect", "", "Graphics", "Additions", true, new SettingEntry.ChildrenSettings() { ChildrenSettingID = new string[] { "Graphics.Additions.Noise Multipler" } }));

            AddSetting(SettingEntry.NewSetting<bool>("Floating dust", "", "Graphics", "World", true));

            AddSetting(SettingEntry.NewSetting<bool>("Show duel room code", "Show the duel room code after you start the game", "Misc", "Privacy", true));
            AddSetting(SettingEntry.NewSetting<bool>("Unlimited clone count", "Play with 999 clones!", "Misc", "Privacy", false));
            AddSetting(SettingEntry.NewSetting<bool>("Show version", "Buttom text Bottom text", "Misc", "Mod", true));

            AddSetting(SettingEntry.NewSetting<bool>("New Level Editor", "", "Levels", "Editor", false, null, null, null, null, false));

            AddSetting(SettingEntry.NewSetting<bool>("Last Bot Standing", "Camera will change its angle depending on your movement", "Patches", "GUI", false, null, null, null, null, true));
            AddSetting(SettingEntry.NewSetting<bool>("Fix sounds", "Fix the audio bugs with emotes, raptor kick and ect.", "Patches", "QoL", true));

            AddSetting(SettingEntry.NewSetting<float>("FPS Cap", "60 - Set VSync to On\n600 - Unlimited FPS", "Graphics", "Settings", 2f, null, new SettingEntry.UIValueSettings() { MinValue = 1, MaxValue = 20, Step = 30, OnlyInt = true }));
            AddSetting(SettingEntry.NewSetting<ShadowResolution>("Shadow resolution", "Default with \"Soft\" enabled is the most optimal variant", "Graphics", "Settings", 1, null, new SettingEntry.UIValueSettings() { DropdownEnumType = typeof(ShadowResolution) }));
            AddSetting(SettingEntry.NewSetting<ShadowBias>("Shadow bias", "With \"Minimum\" selected, you'll forget about weird shadows (NO)", "Graphics", "Settings", 2, null, new SettingEntry.UIValueSettings() { DropdownEnumType = typeof(ShadowBias) }));
            AddSetting(SettingEntry.NewSetting<ShadowDistance>("Shadow distance", "If you see this text... then you are using dnSpy or unity explorer", "Graphics", "Settings", 2, null, new SettingEntry.UIValueSettings() { DropdownEnumType = typeof(ShadowDistance) }));
            AddSetting(SettingEntry.NewSetting<bool>("Soft shadows", "Make shadows less pixelated", "Graphics", "Settings", true));

            PageDescriptions.Add(new SettingEntry.CategoryPath().SetUp("Patches", "GUI", "Gameplay User Interface patches\nMake clone drone GUI more stylized", string.Empty));
            PageOverrides.Add(new SettingEntry.OverridePrefs("Misc", "Privacy", "Gameplay", "Duels"));
            PageOverrides.Add(new SettingEntry.OverridePrefs("Graphics", "Additions", "Graphics", "Camera"));

            OverhaulMain.Timer.AddNoArgActionToCompleteNextFrame(delegate
            {
                this.refreshSettings();
            });
        }

        private void refreshSettings()
        {
            foreach (SettingEntry entry in Entries)
            {
                BaseStaticReferences.ModuleManager.OnSettingRefreshed(entry.ID, OverhaulMain.GetSetting<object>(entry.ID), true);
            }
        }

        public string GetSectionName(string sectionInitName)
        {
            string name = sectionInitName;
            foreach(SettingEntry.OverridePrefs prefs in PageOverrides)
            {
                if(prefs.OldSectionName == sectionInitName)
                {
                    name = prefs.NewSectionName;
                }
            }
            return name;
        }
        public string GetCategoryName(string categoryInitName)
        {
            string name = categoryInitName;
            foreach (SettingEntry.OverridePrefs prefs in PageOverrides)
            {
                if (prefs.OldCategoryName == categoryInitName)
                {
                    name = prefs.NewCategoryName;
                }
            }
            return name;
        }

        public SettingEntry.CategoryPath GetPageData(string c, string s)
        {
            bool found = false;
            SettingEntry.CategoryPath page = default(SettingEntry.CategoryPath);
            foreach(SettingEntry.CategoryPath path in PageDescriptions)
            {
                if(c == path.Category && s == path.Section)
                {
                    found = true;
                    page = path;
                    break;
                }
            }
            if (!found)
            {
                page.IsEmpty = true;
            }
            return page;
        }

        public List<SettingEntry.CategoryPath> GetPaths(string category)
        {
            List<SettingEntry.CategoryPath> list = new List<SettingEntry.CategoryPath>();

            foreach (SettingEntry entry in GetAllSettings())
            {
                if (!entry.ForceHide && entry.Path.Category == category)
                {
                    list.Add(entry.Path);
                }
            }
            return list;
        }
        public List<SettingEntry> GetSettings(string category, string section)
        {
            List<SettingEntry> list = new List<SettingEntry>();

            foreach (SettingEntry entry in GetAllSettings())
            {                
                if (entry.Path.Category == category && entry.Path.Section == section)
                {
                    list.Add(entry);
                }
            }
            return list;
        }
        public List<SettingEntry> GetSettings(string[] idArray)
        {
            List<SettingEntry> list = new List<SettingEntry>();
            foreach (SettingEntry entry in GetAllSettings())
            {
                foreach(string str in idArray)
                {
                    if(str == entry.ID)
                    {
                        list.Add(entry);
                    }
                }
            }
            return list;
        }
        public List<SettingEntry> SearchSettings(string text, string category = null, string section = null)
        {
            string name = text.ToLower();
            List<SettingEntry> list = new List<SettingEntry>();
            foreach (SettingEntry entry in GetAllSettings())
            {
                if (entry.Name.ToLower().Contains(name))
                {
                    if(!string.IsNullOrEmpty(category) && entry.Path.Category == category)
                    {
                        if (!string.IsNullOrEmpty(section) && entry.Path.Section == section)
                        {
                            list.Add(entry);
                            goto IL_0000;
                        }
                        list.Add(entry);
                        goto IL_0000;
                    }
                    list.Add(entry);
                }
                IL_0000:;
            }
            return list;
        }
        public List<SettingEntry> GetAllSettings()
        {
            return Entries;
        }
        public void AddSetting(Modules.OverhaulSettingsManager.SettingEntry entry)
        {
            Entries.Add(entry);
            CloneDroneOverhaulDataContainer.Instance.SettingsData.SaveSetting(entry.ID, entry.DefaultValue, true);
        }

        public class SettingEntry
        {
            public struct OverridePrefs
            {
                public string OldCategoryName;
                public string OldSectionName;
                public string NewCategoryName;
                public string NewSectionName;

                public OverridePrefs(string oldCategoryName, string oldSectionName, string categoryName, string sectionName)
                {
                    NewCategoryName = categoryName;
                    NewSectionName = sectionName;
                    OldCategoryName = oldCategoryName;
                    OldSectionName = oldSectionName;
                }
            }

            public struct CategoryPath
            {
                public string Category;
                public string Section;

                public string SectionPageDescription;
                public string SectionPageDescriptionLocalID;

                public bool IsEmpty;

                public CategoryPath SetUp(string category, string section)
                {
                    Category = category;
                    Section = section;
                    return this;
                }

                public CategoryPath SetUp(string category, string section, string sectionPageDescription, string sectionPageDescriptionLocalID)
                {
                    Category = category;
                    Section = section;
                    SectionPageDescription = sectionPageDescription;
                    SectionPageDescriptionLocalID = sectionPageDescriptionLocalID;
                    return this;
                }
            }

            public class ChildrenSettings
            {
                public string[] ChildrenSettingID;
            }

            public class UIValueSettings
            {
                //Float slider stuff
                public float MinValue;
                public float MaxValue;
                public int Step = -1;
                public bool OnlyInt;

                public Type DropdownEnumType;
            }

            public static SettingEntry NewSetting<T>(string name, string description, string category, string categorySection, object defaultValue, ChildrenSettings childSettings = null, UIValueSettings valueSettings = null, string nameID = null, string descriptionID = null, bool forceHide = false)
            {
                Modules.OverhaulSettingsManager.SettingEntry entry = new Modules.OverhaulSettingsManager.SettingEntry();
                entry.Name = name;
                entry.Description = description;
                entry.Type = typeof(T);
                entry.NameLocalizationID = nameID;
                entry.DescLocalizationID = descriptionID;
                entry.Path = new CategoryPath(){
                    Category = category,
                    Section = categorySection
                };
                entry.DefaultValue = defaultValue;
                entry.ChildSettings = childSettings;
                entry.ValueSettings = valueSettings;
                entry.ForceHide = forceHide;
                return entry;
            }
            public void SetUpLocalization(string nameID, string descriptionID)
            {
                NameLocalizationID = nameID;
                DescLocalizationID = descriptionID;
            }

            public string ID
            {
                get
                {
                    return Path.Category + "." + Path.Section + "." + Name;
                }
            }
            public string Name { get; private set; }
            public string NameLocalizationID{ get; private set; }
            public string Description { get; private set; }
            public string DescLocalizationID { get; private set; }
            public object DefaultValue { get; private set; }
            public bool ForceHide { get; private set; }
            public ChildrenSettings ChildSettings
            {
                set
                {
                    if(value != null && value.ChildrenSettingID != null && value.ChildrenSettingID.Length > 0)
                    {
                        HasChildSettings = true;
                        foreach (SettingEntry entry in BaseStaticReferences.SettingsManager.GetAllSettings())
                        {
                            foreach (string str in value.ChildrenSettingID)
                            {
                                if (entry.ID == str)
                                {
                                    entry.IsHidden = true;
                                    if(entry.ChildSettings != null)
                                    {
                                        Modules.ModuleManagement.ShowError_Type2("Preferences warning!", "Children settings cannot have children XD" + System.Environment.NewLine + "Details: " + entry.Name);
                                    }
                                }
                            }
                        }
                    }
                    children = value;
                }
                get
                {
                    return children;
                }
            }
            private ChildrenSettings children;
            public UIValueSettings ValueSettings { get; private set; }
            public bool HasChildSettings { get; private set; }
            public CategoryPath Path { get; private set; }
            public System.Type Type { get; private set; }
            public bool IsHidden { get; set; }
        }
    }
}
