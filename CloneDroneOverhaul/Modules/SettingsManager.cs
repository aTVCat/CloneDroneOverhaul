using CloneDroneOverhaul.PooledPrefabs;
using CloneDroneOverhaul.Utilities;
using ModLibrary;
using UnityEngine;
using UnityEngine.Rendering;
using UnityStandardAssets.ImageEffects;
using System.Collections.Generic;

namespace CloneDroneOverhaul.Modules
{
    public class SettingsManager : ModuleBase
    {
        private List<SettingEntry> Entries = new List<SettingEntry>();
        public static SettingsManager Instance;

        public override bool ShouldWork()
        {
            return true;
        }
        public override void OnActivated()
        {
            BaseStaticReferences.SettingsManager = this;
            Instance = this;

            AddSetting(SettingEntry.NewSetting<float>("Roll Multipler", "", "Graphics", "Additions", 1.00f, null, new SettingEntry.UIValueSettings() { MinValue = 0.5f, MaxValue = 2 }));
            AddSetting(SettingEntry.NewSetting<float>("(Multiplayer) Roll Multipler", "", "Graphics", "Additions", 0.65f, null, new SettingEntry.UIValueSettings() { MinValue = 0.5f, MaxValue = 2 }));
            AddSetting(SettingEntry.NewSetting<bool>("Camera Rolling", "Camera will change its angle depending on your movement", "Graphics", "Additions", true, new SettingEntry.ChildrenSettings() { ChildrenSettingID = new string[] { "Graphics.Additions.Roll Multipler", "Graphics.Additions.(Multiplayer) Roll Multipler" } }));

            AddSetting(SettingEntry.NewSetting<bool>("Show duel room code", "Show the duel room code after you start the game", "Misc", "Privacy", true));
        }

        public List<SettingEntry> GetSettings(string category, string section)
        {
            List<SettingEntry> list = new List<SettingEntry>();
            if (string.IsNullOrEmpty(section) && string.IsNullOrEmpty(category))
            {
                return list;
            }

            foreach (SettingEntry entry in GetAllSettings())
            {                
                if (string.IsNullOrEmpty(section))
                {
                    if (entry.Path.Category == category)
                    {
                        list.Add(entry);
                        goto IL_0000;
                    }
                }

                if (entry.Path.Category == category && entry.Path.Section == section)
                {
                    list.Add(entry);
                }
                IL_0000:;
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
        public void AddSetting(Modules.SettingsManager.SettingEntry entry)
        {
            Entries.Add(entry);
            CloneDroneOverhaulDataContainer.Instance.SettingsData.SaveSetting(entry.ID, entry.DefaultValue, true);
        }

        public class SettingEntry
        {
            public struct CategoryPath
            {
                public string Category;
                public string Section;
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
            }

            public static SettingEntry NewSetting<T>(string name, string description, string category, string categorySection, T defaultValue, ChildrenSettings childSettings = null, UIValueSettings valueSettings = null, string nameID = null, string descriptionID = null)
            {
                Modules.SettingsManager.SettingEntry entry = new Modules.SettingsManager.SettingEntry();
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
