using DiscordWebhook;
using Newtonsoft.Json;
using OverhaulMod.Utils;
using Pathfinding;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using UnityEngine;

namespace OverhaulMod
{
    public class WebhookManager : Singleton<WebhookManager>
    {
        public const string CrashReportsWebhook = "https://discord.com/api/webhooks/1106574827806019665/n486TzxFbaF6sMmbqg2CUHKGN1o15UpR9AUJAmi5c7sdIwI1jeXpTReD4jtZ3U76PzWS";
        public static readonly Uri CrashReportsWebhookUri = new Uri(CrashReportsWebhook);

        public const string SurveysWebhook = "https://discord.com/api/webhooks/1124285317768290454/QuXjaAywp5eRXT2a5BfOtYGFS9h2eHb8giuze3yxLkZ1Y7m7m2AOTfxf9hB4IeCIkTk5";
        public static readonly Uri SurveysWebhookUri = new Uri(SurveysWebhook);

        public const string ErrorsWebhook = "https://discord.com/api/webhooks/1129035917324189745/FGpPRyvgI9YxyrCutXPoWrIGjJ0Z0KueFs4_pqU2wSLUsmYfVYm_qR9yTt-XST40ntSp";
        public static readonly Uri ErrorsWebhookUri = new Uri(ErrorsWebhook);

        private bool s_HasExecutedCrashReportsWebhook = false;

        private List<string> m_caughtErrors;

        private void Start()
        {
            m_caughtErrors = new List<string>();
        }

        public void ExecuteCrashReportsWebhook(string content, bool forceSend = false)
        {
            if (s_HasExecutedCrashReportsWebhook && !forceSend)
                return;
            s_HasExecutedCrashReportsWebhook = true;

            WebhookObject obj1 = new WebhookObject()
            {
                content = (ModBuildInfo.debug ? "<@779372500521320469>\n" : string.Empty) + $"## __Game crashed. v{ModBuildInfo.version}__",
                embeds = new Embed[]
                {
                    new Embed()
                    {
                        title = "**Stack Trace**",
                        description = $"```{content}```",
                        color = int.Parse("E84A3F", System.Globalization.NumberStyles.HexNumber),
                    },
                },
            };
            ExecuteWebhook(obj1, CrashReportsWebhookUri);
        }

        public void ExecuteSurveysWebhook(int rank, string improveText, string likedText)
        {
            string userInfo = $"- **User:** {SteamFriends.GetPersonaName()} [Profile](<https://steamcommunity.com/profiles/{SteamUser.GetSteamID()}>)";
            string deviceInfo = $"- **OS:** {SystemInfo.operatingSystem}\n- **CPU:** {SystemInfo.processorType}\n * {SystemInfo.processorCount}/{SystemInfo.processorFrequency}\n- **GPU:** {SystemInfo.graphicsDeviceName}\n * {SystemInfo.graphicsMemorySize} MBs\n- **Memory:** {SystemInfo.systemMemorySize} MBs";

            int color = int.Parse("f5ec42", System.Globalization.NumberStyles.HexNumber);
            WebhookObject obj1 = new WebhookObject()
            {
                content = $"## __Feedback. v{ModBuildInfo.version} (Rank: {rank})__",
                embeds = new Embed[]
                {
                    new Embed()
                    {
                        title = "**Improve**",
                        description = improveText,
                        color = color,
                    },

                    new Embed()
                    {
                        title = "**Favorite**",
                        description = likedText,
                        color = color,
                    },

                    new Embed()
                    {
                        title = "**Details**",
                        description = $"{userInfo}\n{deviceInfo}",
                        color = color,
                    },
                },
            };
            ExecuteWebhook(obj1, SurveysWebhookUri);
        }

        public void ExecuteErrorsWebhook(string errorString, bool forceSend = false)
        {
            if (m_caughtErrors.Contains(errorString) && !forceSend)
                return;
            m_caughtErrors.Add(errorString);

            WebhookObject obj1 = new WebhookObject()
            {
                content = $"## __Error. v{ModBuildInfo.version}__",
                embeds = new Embed[]
                {
                    new Embed()
                    {
                        title = "**Contents**",
                        description = $"```{errorString}```",
                        color = int.Parse("eb7d34", System.Globalization.NumberStyles.HexNumber),
                    },
                },
            };
            _ = new WebhookObject()
            {
                embeds = new Embed[]
                {
                    new Embed()
                    {

                    },
                },
                payload_json = "{\"embeds\":[{\"image\":{\"url\":\"attachment://test.jpg\"}}]}",
            };
            ExecuteWebhook(obj1, ErrorsWebhookUri);
        }

        public void ExecuteTestUploadWebhook(string filePath)
        {
            ExecuteFileWebhook(filePath, SurveysWebhookUri);
        }

        public void ExecuteCustomWebhook(string content, string url)
        {
            WebhookObject obj1 = new WebhookObject()
            {
                content = content
            };
            ExecuteWebhook(obj1, new Uri(url));
        }

        public async void ExecuteWebhook(WebhookObject webhookObject, Uri uri)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    _ = await webClient.UploadStringTaskAsync(uri, "POST", JsonConvert.SerializeObject(webhookObject));
                }
            }
            catch { }
        }

        public async void ExecuteFileWebhook(string filePath, Uri uri)
        {
            using (WebClient webClient = new WebClient())
            {
                webClient.UploadFile(uri, "POST", filePath);
            }
        }
    }
}