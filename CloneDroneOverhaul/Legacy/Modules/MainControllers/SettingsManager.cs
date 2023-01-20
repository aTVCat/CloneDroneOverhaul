using AmplifyOcclusion;
using System;
using System.Collections.Generic;
using CloneDroneOverhaul.V3.Graphics;
using CloneDroneOverhaul.Modules;

namespace CloneDroneOverhaul.V3.Base
{
    public class OverhaulSettingsManager_Legacy : V3_ModControllerBase
    {
        private List<SettingEntry> Entries = new List<SettingEntry>();
        private List<SettingEntry.CategoryPath> PageDescriptions = new List<SettingEntry.CategoryPath>();
        private List<SettingEntry.OverridePrefs> PageOverrides = new List<SettingEntry.OverridePrefs>();
        public static OverhaulSettingsManager_Legacy Instance;
        public static CloneDroneOverhaulSettingsData Data => CloneDroneOverhaulDataContainer.SettingsData;

        void Awake()
        {
            AddSetting(SettingEntry.NewSetting<V3.HUD.EDitheringResolution>("DitherRes", "", "Graphics", "Additions", 1, null, new SettingEntry.UIValueSettings() { DropdownEnumType = typeof(V3.HUD.EDitheringResolution) }));
            AddSetting(SettingEntry.NewSetting<V3.HUD.EDitheringRefreshRate>("DitherRRate", "", "Graphics", "Additions", 1, null, new SettingEntry.UIValueSettings() { DropdownEnumType = typeof(V3.HUD.EDitheringRefreshRate) }));

            AddSetting(SettingEntry.NewSetting<float>("Roll Multipler", "", "Graphics", "Additions", 1.00f, null, new SettingEntry.UIValueSettings() { MinValue = 0.5f, MaxValue = 2 }));
            AddSetting(SettingEntry.NewSetting<float>("(Multiplayer) Roll Multipler", "", "Graphics", "Additions", 0.65f, null, new SettingEntry.UIValueSettings() { MinValue = 0.5f, MaxValue = 2 }));
            AddSetting(SettingEntry.NewSetting<bool>("Camera Rolling", "Camera will change its angle depending on your movement", "Graphics", "Additions", true, new SettingEntry.ChildrenSettings() { ChildrenSettingID = new string[] { "Graphics.Additions.Roll Multipler", "Graphics.Additions.(Multiplayer) Roll Multipler" } }));

            AddSetting(SettingEntry.NewSetting<SampleCountLevel>("Sample count", "", "Graphics", "Additions", 3, null, new SettingEntry.UIValueSettings() { DropdownEnumType = typeof(AmplifyOcclusion.SampleCountLevel) }));
            AddSetting(SettingEntry.NewSetting<float>("Noise Multipler", "", "Graphics", "Additions", 0.9f, null, new SettingEntry.UIValueSettings() { MinValue = 0.8f, MaxValue = 1.3f }));
            AddSetting(SettingEntry.NewSetting<float>("Occlusion intensity", "", "Graphics", "Additions", 0.95f, null, new SettingEntry.UIValueSettings() { MinValue = 0.5f, MaxValue = 1.3f }));
            AddSetting(SettingEntry.NewSetting<bool>("Amplify occlusion", "Makes the game more realistic\nNot recommended on low-end PCs", "Graphics", "Additions", true, new SettingEntry.ChildrenSettings() { ChildrenSettingID = new string[] { "Graphics.Additions.Sample count", "Graphics.Additions.Occlusion intensity" } }));
            AddSetting(SettingEntry.NewSetting<bool>("Noise effect", "", "Graphics", "Additions", true, new SettingEntry.ChildrenSettings() { ChildrenSettingID = new string[] { "Graphics.Additions.Noise Multipler", "Graphics.Additions.DitherRes", "Graphics.Additions.DitherRRate" } }));
            AddSetting(SettingEntry.NewSetting<EAntialiasingLevel>("Antialiasing", "", "Graphics", "Settings", 2, null, new SettingEntry.UIValueSettings() { DropdownEnumType = typeof(EAntialiasingLevel) }));

            AddSetting(SettingEntry.NewSetting<bool>("Floating dust", "", "Graphics", "World", true));

            AddSetting(SettingEntry.NewSetting<bool>("Show duel room code", "Show the duel room code after you start the game", "Misc", "Privacy", true));
            AddSetting(SettingEntry.NewSetting<bool>("Unlimited clone count", "Play with 999 clones!", "Misc", "Privacy", false));
            AddSetting(SettingEntry.NewSetting<bool>("Show version", "Buttom text Bottom text", "Misc", "Mod", true));

            AddSetting(SettingEntry.NewSetting<bool>("New Level Editor", "", "Levels", "Editor", false, null, null, null, null, !OverhaulDescription.TEST_FEATURES_ENABLED));

            AddSetting(SettingEntry.NewSetting<bool>("Last Bot Standing", "Camera will change its angle depending on your movement", "Patches", "GUI", false, null, null, null, null, !OverhaulDescription.TEST_FEATURES_ENABLED));
            AddSetting(SettingEntry.NewSetting<bool>("Subtitles", "", "Patches", "GUI", true));
            AddSetting(SettingEntry.NewSetting<bool>("Pause menu", "", "Patches", "GUI", true));

            AddSetting(SettingEntry.NewSetting<bool>("Fix sounds", "Fix the audio bugs with emotes, raptor kick and ect.", "Patches", "QoL", true));
            AddSetting(SettingEntry.NewSetting<bool>("Pixel perfect HUD", "Fixes blurry parts of the HUD", "Patches", "GUI", true));
            AddSetting(SettingEntry.NewSetting<bool>("Energy bar", "", "Patches", "GUI", true));
            AddSetting(SettingEntry.NewSetting<float>("HUD Scale", "", "Graphics", "Additions", 0f, null, new SettingEntry.UIValueSettings() { MinValue = -0.12f, MaxValue = 0.12f }));

            AddSetting(SettingEntry.NewSetting<float>("FPS Cap", "60 - Set VSync to On\n600 - Unlimited FPS", "Graphics", "Settings", 2f, null, new SettingEntry.UIValueSettings() { MinValue = 1, MaxValue = 20, Step = 30, OnlyInt = true }));
            AddSetting(SettingEntry.NewSetting<EShadowResolution>("Shadow resolution", "Default with \"Soft\" enabled is the most optimal variant", "Graphics", "Settings", 1, null, new SettingEntry.UIValueSettings() { DropdownEnumType = typeof(EShadowResolution) }));
            AddSetting(SettingEntry.NewSetting<EShadowBias>("Shadow bias", "With \"Minimum\" selected, you'll forget about weird shadows (NO)", "Graphics", "Settings", 2, null, new SettingEntry.UIValueSettings() { DropdownEnumType = typeof(EShadowBias) }));
            AddSetting(SettingEntry.NewSetting<EShadowDistance>("Shadow distance", "If you see this text... then you are using dnSpy or unity explorer", "Graphics", "Settings", 2, null, new SettingEntry.UIValueSettings() { DropdownEnumType = typeof(EShadowDistance) }));

            AddSetting(OverhaulSettingsManager_Legacy.SettingEntry.NewSetting<ELightLimit>("Light limit", "Sets the limit of visible point lights", "Graphics", "Settings", 1, null, new OverhaulSettingsManager_Legacy.SettingEntry.UIValueSettings
            {
                DropdownEnumType = typeof(ELightLimit)
            }, null, null, false));
            AddSetting(OverhaulSettingsManager_Legacy.SettingEntry.NewSetting<bool>("Vignette", "Shades screen edges", "Graphics", "Additions", true, null, null, null, null, false));
            AddSetting(SettingEntry.NewSetting<bool>("Soft shadows", "Make shadows less pixelated", "Graphics", "Settings", true));

            AddSetting(SettingEntry.NewSetting<bool>("Level of detail [LoD]", "", "Graphics", "Optimisation", true));
            AddSetting(SettingEntry.NewSetting<bool>("High LoD threshold", "", "Graphics", "Optimisation", true));
            AddSetting(SettingEntry.NewSetting<bool>("Shading", "", "Graphics", "Additions", true));

            AddSetting(SettingEntry.NewSetting<V3.Gameplay.EAdvancedCameraType>("Camera mode", "", "Misc", "Experience", 0, null, new SettingEntry.UIValueSettings() { DropdownEnumType = typeof(V3.Gameplay.EAdvancedCameraType) }));

            PageDescriptions.Add(new SettingEntry.CategoryPath().SetUp("Patches", "GUI", "Gameplay User Interface patches\nMake clone drone GUI more stylized.", "Patch.GUI"));
            PageDescriptions.Add(new SettingEntry.CategoryPath().SetUp("Graphics", "Additions", "Image effects that enchance entire game.\nEach enabled option may decrease performance!", "Graphics.Camera"));
            PageDescriptions.Add(new SettingEntry.CategoryPath().SetUp("Graphics", "World", "Visual improvements that don't really affect performance.", "Graphics.World"));
            PageDescriptions.Add(new SettingEntry.CategoryPath().SetUp("Graphics", "Settings", "Game renderer settings.", "Graphics.Settings"));
            PageDescriptions.Add(new SettingEntry.CategoryPath().SetUp("Misc", "Privacy", "Duel advanced options.", "Misc.Duels"));
            PageDescriptions.Add(new SettingEntry.CategoryPath().SetUp("Misc", "Mod", "Overhaul mod related settings.", "Misc.Overhaul"));
            PageDescriptions.Add(new SettingEntry.CategoryPath().SetUp("Levels", "Editor", "Enchance level building hobby!", "Levels.Editor"));
            PageDescriptions.Add(new SettingEntry.CategoryPath().SetUp("Patches", "QoL", "Fixes that are required by community.", "Patch.QoL"));
            PageDescriptions.Add(new SettingEntry.CategoryPath().SetUp("Graphics", "Optimisation", "Manage settings that might increase performance.", "Graphics.Optimisation"));

            PageOverrides.Add(new SettingEntry.OverridePrefs("Misc", "Privacy", "Gameplay", "Duels"));
            PageOverrides.Add(new SettingEntry.OverridePrefs("Graphics", "Additions", "Graphics", "Camera"));


            DelegateScheduler.Instance.Schedule(refreshSettings, 0.1f);
        }

        private void refreshSettings()
        {
            foreach (SettingEntry entry in Entries)
            {
                V3.Base.V3_MainModController.SendSettingWasRefreshed(entry.ID, OverhaulMain.GetSetting<object>(entry.ID));
            }
        }

        public string GetSectionName(string sectionInitName)
        {
            string name = sectionInitName;
            foreach (SettingEntry.OverridePrefs prefs in PageOverrides)
            {
                if (prefs.OldSectionName == sectionInitName)
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
            foreach (SettingEntry.CategoryPath path in PageDescriptions)
            {
                if (c == path.Category && s == path.Section)
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
                foreach (string str in idArray)
                {
                    if (str == entry.ID)
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
                    if (!string.IsNullOrEmpty(category) && entry.Path.Category == category)
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
        public void AddSetting(SettingEntry entry)
        {
            Entries.Add(entry);
            CloneDroneOverhaulDataContainer.SettingsData.SaveSetting(entry.ID, entry.DefaultValue, true);
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
                OverhaulSettingsManager_Legacy.SettingEntry entry = new OverhaulSettingsManager_Legacy.SettingEntry
                {
                    Name = name,
                    Description = description,
                    Type = typeof(T),
                    NameLocalizationID = nameID,
                    DescLocalizationID = descriptionID,
                    Path = new CategoryPath()
                    {
                        Category = category,
                        Section = categorySection
                    },
                    DefaultValue = defaultValue,
                    ChildSettings = childSettings,
                    ValueSettings = valueSettings,
                    ForceHide = forceHide
                };
                return entry;
            }
            public void SetUpLocalization(string nameID, string descriptionID)
            {
                NameLocalizationID = nameID;
                DescLocalizationID = descriptionID;
            }

            public string ID => Path.Category + "." + Path.Section + "." + Name;
            public string Name { get; private set; }
            public string NameLocalizationID { get; private set; }
            public string Description { get; private set; }
            public string DescLocalizationID { get; private set; }
            public object DefaultValue { get; private set; }
            public bool ForceHide { get; private set; }
            public ChildrenSettings ChildSettings
            {
                set
                {
                    if (value != null && value.ChildrenSettingID != null && value.ChildrenSettingID.Length > 0)
                    {
                        HasChildSettings = true;
                        foreach (SettingEntry entry in OverhaulSettingsManager_Legacy.GetInstance<OverhaulSettingsManager_Legacy>().GetAllSettings())
                        {
                            foreach (string str in value.ChildrenSettingID)
                            {
                                if (entry.ID == str)
                                {
                                    entry.IsHidden = true;
                                    if (entry.ChildSettings != null)
                                    {
                                        Modules.ModuleManagement.ShowError("Preferences warning!\nChildren settings cannot have children XD" + System.Environment.NewLine + "Details: " + entry.Name);
                                    }
                                }
                            }
                        }
                    }
                    children = value;
                }
                get => children;
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
