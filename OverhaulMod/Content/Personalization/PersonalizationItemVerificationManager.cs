using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationItemVerificationManager : Singleton<PersonalizationItemVerificationManager>
    {
        public void SendItemToVerification(string folder, PersonalizationItemInfo personalizationItemInfo, Action successCallback, Action<string> errorCallback)
        {
            string file = Path.GetTempPath() + "PersonalizationItem.zip";
            if (File.Exists(file))
                File.Delete(file);

            FastZip fastZip = new FastZip();
            fastZip.CreateZip(file, folder, true, null);

            ModWebhookManager.Instance.ExecuteVerificationRequestWebhook(file, personalizationItemInfo, successCallback, errorCallback);
        }
    }
}
