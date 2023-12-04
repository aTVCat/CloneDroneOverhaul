using UnityEngine;

namespace CDOverhaul
{
    public class OverhaulSettingsTest
    {
        [OverhaulSetting(OverhaulSettingConstants.Categories.TEST_CATEGORY, OverhaulSettingConstants.Sections.TEST_SECTION, "Test bool - On")]
        public static bool TestBoolTrue = true;

        [OverhaulSetting(OverhaulSettingConstants.Categories.TEST_CATEGORY, OverhaulSettingConstants.Sections.TEST_SECTION2, "Test bool - Off")]
        public static bool TestBoolFalse = false;

        [OverhaulSettingDropdownParameters("test 1@test 2@test3")]
        [OverhaulSetting(OverhaulSettingConstants.Categories.TEST_CATEGORY2, OverhaulSettingConstants.Sections.TEST_SECTION, "Test int")]
        public static int TestInt = 787;

        [OverhaulSettingSliderParameters(0f, 100f)]
        [OverhaulSetting(OverhaulSettingConstants.Categories.TEST_CATEGORY2, OverhaulSettingConstants.Sections.TEST_SECTION3, "Test float")]
        public static float TestFloat = 123f;

        [OverhaulSetting(OverhaulSettingConstants.Categories.TEST_CATEGORY3, OverhaulSettingConstants.Sections.TEST_SECTION2, "Test string")]
        public static string TestString = "The string";

        [OverhaulSetting(OverhaulSettingConstants.Categories.TEST_CATEGORY3, OverhaulSettingConstants.Sections.TEST_SECTION2, "Test key code")]
        public static KeyCode TestKeyCode = KeyCode.G;
    }
}
