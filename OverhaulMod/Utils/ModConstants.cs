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
    }
}
