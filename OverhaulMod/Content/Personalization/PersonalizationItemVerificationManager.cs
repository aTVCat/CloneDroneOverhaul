using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            WebhookManager.Instance.ExecuteVerificationRequestWebhook(file, personalizationItemInfo, successCallback, errorCallback);
        }
    }
}
