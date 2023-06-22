namespace CDOverhaul
{
    public class OverhaulSettingDescription
    {
        public string Description { get; set; }

        public OverhaulSettingDescription(in string desc)
        {
            Description = desc;
        }
    }
}
