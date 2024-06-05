using OverhaulMod.Utils;

namespace OverhaulMod
{
    public static class ModDebug
    {
        public static bool forceDisableCursor
        {
            get;
            set;
        }

        public static void MessagePopupTest()
        {
            string desc = string.Empty;
            for (int i = 0; i < 75; i++)
            {
                desc += "very long string ";
            }

            ModUIUtils.MessagePopupOK("hmm", desc, 175f, true);
        }
    }
}
