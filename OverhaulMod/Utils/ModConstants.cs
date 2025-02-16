using OverhaulMod.Content;
using System.Collections.Generic;
using UnityEngine.UI;

namespace OverhaulMod.Utils
{
    public static class ModConstants
    {
        public static readonly string LoremIpsumText = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus ut viverra diam, nec rhoncus dui. Suspendisse quis rutrum dolor.";

        public static List<Dropdown.OptionData> CursorSkinOptions = new List<Dropdown.OptionData>()
        {
            new Dropdown.OptionData("Default"),
            new Dropdown.OptionData("Skin 1"),
            new Dropdown.OptionData("Skin 2"),
        };

        public static List<Dropdown.OptionData> FontOptionsNoAddon = new List<Dropdown.OptionData>()
        {
            new Dropdown.OptionData("Default"),
            new Dropdown.OptionData("VCR OSD Mono"),
            new Dropdown.OptionData("Piksieli Prosto"),
            new Dropdown.OptionData("Edit Undo BRK"),
            new Dropdown.OptionData("Open Sans (Regular)"),
            new Dropdown.OptionData("Open Sans (Extra Bold)"),
        };

        public static List<Dropdown.OptionData> FontOptions = new List<Dropdown.OptionData>()
        {
            new Dropdown.OptionData("Default"),
            new Dropdown.OptionData("VCR OSD Mono"),
            new Dropdown.OptionData("Piksieli Prosto"),
            new Dropdown.OptionData("Edit Undo BRK"),
            new Dropdown.OptionData("Open Sans (Regular)"),
            new Dropdown.OptionData("Open Sans (Extra Bold)"),
            new Dropdown.OptionData("Noto Sans CJK TC (Regular)"),
            new Dropdown.OptionData("Noto Sans CJK TC (Bold)"),
            new Dropdown.OptionData("Noto Sans CJK TC (Black)"),
        };

        public static List<Dropdown.OptionData> GetFontOptions(int dropdownValue = -1)
        {
            if (dropdownValue > 5 || AddonManager.Instance.HasInstalledAddon(AddonManager.EXTRAS_ADDON_ID, 0))
            {
                return FontOptions;
            }
            return FontOptionsNoAddon;
        }
    }
}
