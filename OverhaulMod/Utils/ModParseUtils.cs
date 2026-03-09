using Steamworks;
using System.Globalization;
using UnityEngine;

namespace OverhaulMod.Utils
{
    public static class ModParseUtils
    {
        public static int TryParseInt(string @string, int defaultValue)
        {
            return !int.TryParse(@string, out int result) ? defaultValue : result;
        }

        public static float TryParseFloat(string @string, float defaultValue)
        {
            return @string.IsNullOrEmpty() || !float.TryParse(@string, NumberStyles.Number, CultureInfo.InvariantCulture, out float result) ? defaultValue : result;
        }

        public static bool TryParseBool(string @string, bool defaultValue)
        {
            return !bool.TryParse(@string, out bool result) ? defaultValue : result;
        }

        public static ulong TryParseULong(string @string, ulong defaultValue)
        {
            return !ulong.TryParse(@string, out ulong result) ? defaultValue : result;
        }

        public static Color TryParseColor(string @string)
        {
            return TryParseColor(@string, Color.white);
        }

        public static Color TryParseColor(string @string, Color defaultValue)
        {
            if (@string[0] != '#') @string = '#' + @string;
            return !ColorUtility.TryParseHtmlString(@string, out Color result) ? defaultValue : result;
        }

        public static CSteamID TryParseSteamID(string @string)
        {
            return !ulong.TryParse(@string, out ulong result) ? default : (CSteamID)result;
        }
    }
}
