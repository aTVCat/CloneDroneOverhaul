using OverhaulMod.Utils;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationItemImportResult : OperationResult
    {
        public ImportResult Result;

        public PersonalizationItemImportResult() { }

        public PersonalizationItemImportResult(ImportResult result)
        {
            Result = result;
        }

        public PersonalizationItemImportResult(string error)
        {
            Result = error.IsNullOrEmpty() ? ImportResult.Success : ImportResult.Error;
            Error = error;
        }

        public PersonalizationItemImportResult(ImportResult result, string error)
        {
            Result = result;
            Error = error;
        }

        public override bool HasFailed()
        {
            if (Result == ImportResult.Error)
            {
                return true;
            }
            return base.HasFailed();
        }

        public enum ImportResult
        {
            Success,
            Updated,
            Cancelled,
            Error
        }
    }
}