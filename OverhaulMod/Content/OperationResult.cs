using OverhaulMod.Utils;

namespace OverhaulMod.Content
{
    public class OperationResult
    {
        public string Error;

        public virtual bool HasFailed()
        {
            return !Error.IsNullOrEmpty();
        }
    }
}
