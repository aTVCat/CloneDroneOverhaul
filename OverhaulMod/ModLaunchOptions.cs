using System.Collections.Generic;
using System.Linq;

namespace OverhaulMod
{
    public static class ModLaunchOptions
    {
        private static bool s_hasInitialized;

        private static List<LaunchOption> s_launchOptions;

        internal static void Initialize()
        {
            if (s_hasInitialized)
                return;

            s_hasInitialized = true;
            refreshLaunchOptions();
        }

        private static void refreshLaunchOptions()
        {
            List<LaunchOption> list = new List<LaunchOption>();
            foreach (object option in typeof(LaunchOption).GetEnumValues())
            {
                LaunchOption launchOption = (LaunchOption)option;
                if (HasLaunchOption(launchOption))
                    list.Add(launchOption);
            }
            s_launchOptions = list;
        }

        public static bool HasLaunchOption(LaunchOption option)
        {
            if (s_launchOptions == null)
            {
                return System.Environment.GetCommandLineArgs().Contains(GetLaunchOptionString(option));
            }
            return s_launchOptions.Contains(option);
        }

        public static string GetLaunchOptionString(LaunchOption option)
        {
            switch (option)
            {
                case LaunchOption.LoadUnsupportedAddons:
                    return "-LoadUnsupportedAddons";
            }
            return string.Empty;
        }

        public enum LaunchOption
        {
            None,

            LoadUnsupportedAddons
        }
    }
}
