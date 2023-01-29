namespace CDOverhaul
{
    public static class OverhaulExceptions
    {
        public const string Prefix = "Overhaul mod: ";

        public const string Exc_SettingError = Prefix + "SettingError";
        public const string Exc_SettingOOBError = Prefix + "SettingOOBError"; // out of bounds
        public const string Exc_SettingAddError = Prefix + "SettingAddError";
        public const string Exc_SettingSaveError = Prefix + "SettingSaveError";
        public const string Exc_SettingGetError = Prefix + "SettingGetError";
    }
}
