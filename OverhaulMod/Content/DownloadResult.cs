using OverhaulMod.Utils;

namespace OverhaulMod.Content
{
    public class DownloadResult
    {
        public string Error;

        public bool IsError()
        {
            return !Error.IsNullOrEmpty();
        }
    }
}
