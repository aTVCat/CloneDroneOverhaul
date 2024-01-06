using System;
using System.Runtime.InteropServices;

namespace OverhaulMod.Utils
{
    public static class SpecialUtils
    {
        private static class InnerSpecialUtils
        {
            private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
            private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

            public const string INITIAL_TITLE_BAR_TEXT = "Clone Drone in the Danger Zone";

            private static string s_previousTitleBarText;

            [DllImport("user32.dll", EntryPoint = "FindWindow")]
            private static extern System.IntPtr FindWindow(string className, string windowName);

            [DllImport("dwmapi.dll")]
            private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

            [DllImport("user32.dll", EntryPoint = "SetWindowText")]
            private static extern bool SetWindowText(System.IntPtr hwnd, string lpString);

            private static bool useImmersiveDarkMode(IntPtr handle, bool enabled)
            {
                if (isWindows10OrGreater(17763))
                {
                    int attribute = DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1;
                    if (isWindows10OrGreater(18985))
                    {
                        attribute = DWMWA_USE_IMMERSIVE_DARK_MODE;
                    }

                    int useImmersiveDarkMode = enabled ? 1 : 0;
                    return DwmSetWindowAttribute(handle, attribute, ref useImmersiveDarkMode, sizeof(int)) == 0;
                }
                return false;
            }

            private static bool isWindows10OrGreater(int build = -1)
            {
                return Environment.OSVersion.Version.Major >= 10 && Environment.OSVersion.Version.Build >= build;
            }

            public static void SetTitleBarDarkModeEnabled(bool enabled)
            {
                IntPtr intPtr = FindWindow(null, string.IsNullOrEmpty(s_previousTitleBarText) ? INITIAL_TITLE_BAR_TEXT : s_previousTitleBarText);
                if (intPtr != null)
                {
                    _ = useImmersiveDarkMode(intPtr, enabled);
                }
            }

            public static void SetTitleBarText(string text)
            {
                if (string.IsNullOrEmpty(text))
                    return;

                string windowTitle = string.IsNullOrEmpty(s_previousTitleBarText) ? INITIAL_TITLE_BAR_TEXT : s_previousTitleBarText;

                IntPtr intPtr = FindWindow(null, windowTitle);
                if (intPtr != null)
                {
                    _ = SetWindowText(intPtr, text);
                    s_previousTitleBarText = text;
                }
            }
        }

        public const string INITIAL_TITLE_BAR_TEXT = "Clone Drone in the Danger Zone";

        public static void SetOverhauledTitleBarState()
        {
            SetTitleBarDarkModeEnabled(true);
            SetTitleBarText("Clone Drone (Overhaul Mod)");
        }

        public static void RestoreInitialTitleBarState()
        {
            SetTitleBarDarkModeEnabled(false);
            SetTitleBarText(INITIAL_TITLE_BAR_TEXT);
        }

        public static void SetTitleBarDarkModeEnabled(bool enabled)
        {
            try
            {
                InnerSpecialUtils.SetTitleBarDarkModeEnabled(enabled);
            }
            catch
            {

            }
        }

        public static void SetTitleBarText(string text)
        {
            try
            {
                InnerSpecialUtils.SetTitleBarText(text);
            }
            catch
            {

            }
        }
    }
}
