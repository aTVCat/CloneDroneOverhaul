using DiscordWebhook;
using Newtonsoft.Json;
using System;
using System.Net;

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
            "DebugMenu.OnThrowErrorClicked",
            "UnityExplorer",
            "Can't find asset",
            "Unsupported color:",
            "Failed to read input report",
            "Failed to start reading input report",
            "You are not a client",
            "DecompressOnLoad",
            "Failed to get input data",
            "Global_Server",
            "entity which is detached",
            "InterruptDiscord",
            "SummonAllyAbility.spawnConstructionCube",
            "CanApplyArmorPiece"
        };
        private static bool m_HasExecutedCrashReportsWebhook = false;

        public static void ExecuteCrashReportsWebhook(string content)
        {
            if (!AllowSendingInformation || m_HasExecutedCrashReportsWebhook)
                return;
            m_HasExecutedCrashReportsWebhook = true;

            WebhookObject obj1 = new WebhookObject()
            {
                content = OverhaulVersion.IsDebugBuild ? "<@779372500521320469>" : string.Empty,
                embeds = new Embed[]
                {
                    new Embed()
                    {
                        title = "__Game crashed. Client: " + OverhaulVersion.Watermark + "__",
                        description = content,
                        color = int.Parse("E84A3F", System.Globalization.NumberStyles.HexNumber),
                    },
                },
            };
            ExecuteWebhook(obj1);
        }

        public static async void ExecuteWebhook(WebhookObject webhookObject)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    _ = await webClient.UploadStringTaskAsync(CrashReportsWebhookUri, "POST", JsonConvert.SerializeObject(webhookObject));
                }
            }
            catch { }
        }

        public static bool HasExcludedError(in string theString)
        {
            foreach (string str in IgnoredStrings)
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
