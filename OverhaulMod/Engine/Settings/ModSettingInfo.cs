using UnityEngine;

namespace OverhaulMod.Engine.Settings
{
    /// <summary>
    /// Summary of setting to be used by UIs (mainly by <see cref="UI.UISettingsMenuReworkV2"/>)
    /// </summary>
    public class ModSettingInfo
    {
        /// <summary>
        /// The ID of setting
        /// </summary>
        public string ID;

        /// <summary>
        /// Name translation ID
        /// </summary>
        public string NameLocalizationID;

        /// <summary>
        /// Description translation ID
        /// </summary>
        public string DescriptionLocalizationID;

        /// <summary>
        /// Display element type
        /// </summary>
        public ModSettingElementType ElementType;

        /// <summary>
        /// Should it ask user to restart the game when changed
        /// </summary>
        public bool RequiresRestarting;

        /// <summary>
        /// Checkbox settings
        /// </summary>
        public CheckboxPreferences CheckboxPrefs;

        /// <summary>
        /// Slider settings
        /// </summary>
        public SliderPreferences SliderPrefs;

        /// <summary>
        /// Dropdown settings
        /// </summary>
        public DropdownPreferences DropdownPrefs;

        /// <summary>
        /// Key bind setter settings
        /// </summary>
        public KeyBindPreferences KeyBindPrefs;

        /// <summary>
        /// Button settings
        /// </summary>
        public ButtonPreferences ButtonPrefs;

        public struct CheckboxPreferences
        {
            public string SubPageID;
        }

        public struct SliderPreferences
        {
            public int WholeNumbers;

            public float Min, Max;

            public string TextFunctionID;
        }

        public struct DropdownPreferences
        {
            public string ListID;

            public bool HasImages;
        }

        public struct KeyBindPreferences
        {
            public KeyCode DefaultKey;
        }

        public struct ButtonPreferences
        {
            public string ActionID;
        }
    }
}