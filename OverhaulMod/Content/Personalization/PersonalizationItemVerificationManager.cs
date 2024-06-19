using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationItemVerificationManager : Singleton<PersonalizationItemVerificationManager>
    {
        public void SendItemToVerification(PersonalizationItemInfo personalizationItemInfo, Action successCallback, Action<string> errorCallback)
        {
            PersonalizationEditorManager.Instance.ExportItem(personalizationItemInfo, out string dest, Path.GetTempPath());
            ModWebhookManager.Instance.ExecuteVerificationRequestWebhook(dest, personalizationItemInfo, successCallback, errorCallback);
        }
    }
}
