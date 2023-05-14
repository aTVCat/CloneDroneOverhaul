using DiscordWebhook;
using Newtonsoft.Json;
using System;
using System.Net;
using UnityEngine;

namespace CDOverhaul
{
    public static class OverhaulWebhooksController
    {
        public const string CrashReportsWebhook = "https://discord.com/api/webhooks/1106574827806019665/n486TzxFbaF6sMmbqg2CUHKGN1o15UpR9AUJAmi5c7sdIwI1jeXpTReD4jtZ3U76PzWS";
        public static readonly Uri CrashReportsWebhookUri = new Uri(CrashReportsWebhook);

        [OverhaulSetting("Mod.Information.Send crash reports", true, false, "Once the game is crashed, a crash log will be sent to mod owner")]
        public static bool AllowSendingInformation;

        public static readonly string[] IgnoredStrings = new string[]
        {
            "DebugMenu.OnThrowErrorClicked"
        };

        private static bool m_HasExecutedCrashReportsWebhook = false;

        public static async void ExecuteCrashReportsWebhook(string content)
        {
            if (!AllowSendingInformation || CheckStringForExcludedErrors(content) || m_HasExecutedCrashReportsWebhook)
            {
                return;
            }
            m_HasExecutedCrashReportsWebhook = true;

            WebhookObject obj1 = new WebhookObject()
            {
                content = "<@779372500521320469>", // this pings me
                embeds = new Embed[]
                {
                    new Embed()
                    {
                        title = "__Game crashed. Client: " + OverhaulVersion.ModFullName + "__",
                        description = content,
                        color = int.Parse("E84A3F", System.Globalization.NumberStyles.HexNumber),
                    },
                },
            };

            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    await webClient.UploadStringTaskAsync(CrashReportsWebhookUri, "POST", JsonConvert.SerializeObject(obj1));
                }
            }
            catch
            {
                return;
            }
        }

        public static bool CheckStringForExcludedErrors(in string theString)
        {
            foreach(string str in IgnoredStrings)
            {
                if (theString.Contains(str))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
