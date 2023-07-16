using BestHTTP.SocketIO;
using DiscordWebhook;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace CDOverhaul
{
    public static class OverhaulWebhooksController
    {
        public const string CrashReportsWebhook = "https://discord.com/api/webhooks/1106574827806019665/n486TzxFbaF6sMmbqg2CUHKGN1o15UpR9AUJAmi5c7sdIwI1jeXpTReD4jtZ3U76PzWS";
        public static readonly Uri CrashReportsWebhookUri = new Uri(CrashReportsWebhook);

        public const string SurveysWebhook = "https://discord.com/api/webhooks/1124285317768290454/QuXjaAywp5eRXT2a5BfOtYGFS9h2eHb8giuze3yxLkZ1Y7m7m2AOTfxf9hB4IeCIkTk5";
        public static readonly Uri SurveysWebhookUri = new Uri(SurveysWebhook);

        public const string ErrorsWebhook = "https://discord.com/api/webhooks/1129035917324189745/FGpPRyvgI9YxyrCutXPoWrIGjJ0Z0KueFs4_pqU2wSLUsmYfVYm_qR9yTt-XST40ntSp";
        public static readonly Uri ErrorsWebhookUri = new Uri(ErrorsWebhook);

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
            "CanApplyArmorPiece",
            "MissingMethodException",
            "PlayUpperWithTimeSkipped",
            "_data is null",
            "Could not allocate memory",
            "MechBodyPart.UpdateMe",
            "SwordBlockArea.TryProcessBlockCollision",
            "Infinity or NaN floating point",
            "AudioManager",
            "ReleaseControlInternal",
        };


        private static bool s_HasExecutedCrashReportsWebhook = false;
        private static readonly List<string> s_CaughtErrors = new List<string>();

        public static void ExecuteCrashReportsWebhook(string content, bool forceSend = false)
        {
            if ((!AllowSendingInformation || s_HasExecutedCrashReportsWebhook) && !forceSend)
                return;
            s_HasExecutedCrashReportsWebhook = true;

            WebhookObject obj1 = new WebhookObject()
            {
                content = (OverhaulVersion.IsDebugBuild ? "<@779372500521320469> " : string.Empty) +
                "__Game crashed! Version: " + OverhaulVersion.ModVersion + "__",
                embeds = new Embed[]
                {
                    new Embed()
                    {
                        title = "**Stack Trace**",
                        description = content,
                        color = int.Parse("E84A3F", System.Globalization.NumberStyles.HexNumber),
                    },
                },
            };
            ExecuteWebhook(obj1, CrashReportsWebhookUri);
        }

        public static void ExecuteSurveysWebhook(int rank, string improveText, string likedText, bool includeGameLogs, bool includeDeviceInfo)
        {
            string rankContent = rank + "/5";
            string improveContent = improveText;
            string likedContent = likedText;
            string gameLogs = includeGameLogs ? "Work in progress..." : "**NOT INCLUDED**";
            string deviceInfo = includeDeviceInfo ?
                string.Format("- **OS:** {0}\n- **CPU:** {1}\n Processors: {2}\n Frequency: {3}\n- **GPU:** {4}\n- **Memory:** {5} MBs\n- **GPU Memory:** {6}", new object[] { SystemInfo.operatingSystem, SystemInfo.processorType, SystemInfo.processorCount, SystemInfo.processorFrequency, SystemInfo.graphicsDeviceName, SystemInfo.systemMemorySize, SystemInfo.graphicsMemorySize }) : "**NOT INCLUDED**";

            WebhookObject obj1 = new WebhookObject()
            {
                content = "__Feedback sent! Version: " + OverhaulVersion.ModVersion + "__",
                embeds = new Embed[]
                {
                    new Embed()
                    {
                        title = "**Rank**",
                        description = rankContent,
                        color = int.Parse("f5ec42", System.Globalization.NumberStyles.HexNumber),
                    },

                    new Embed()
                    {
                        title = "**Improve**",
                        description = improveContent,
                        color = int.Parse("f5ec42", System.Globalization.NumberStyles.HexNumber),
                    },

                    new Embed()
                    {
                        title = "**User's favorite**",
                        description = likedContent,
                        color = int.Parse("f5ec42", System.Globalization.NumberStyles.HexNumber),
                    },

                    new Embed()
                    {
                        title = "**Game logs**",
                        description = gameLogs,
                        color = int.Parse("f5ec42", System.Globalization.NumberStyles.HexNumber),
                    },

                    new Embed()
                    {
                        title = "**Device info**",
                        description = deviceInfo,
                        color = int.Parse("f5ec42", System.Globalization.NumberStyles.HexNumber),
                    },
                },
            };
            ExecuteWebhook(obj1, SurveysWebhookUri);
        }

        public static void ExecuteErrorsWebhook(string errorString, bool forceSend = false)
        {
            if ((!AllowSendingInformation || s_CaughtErrors.Contains(errorString)) && !forceSend)
                return;
            s_CaughtErrors.Add(errorString);

            WebhookObject obj1 = new WebhookObject()
            {
                content = "__Non-Crashing Error! Version: " + OverhaulVersion.ModVersion + "__",
                embeds = new Embed[]
                {
                    new Embed()
                    {
                        title = "**Contents**",
                        description = "```" + errorString + "```",
                        color = int.Parse("eb7d34", System.Globalization.NumberStyles.HexNumber),
                    },
                },
            };
            ExecuteWebhook(obj1, ErrorsWebhookUri);
        }

        public static async void ExecuteWebhook(WebhookObject webhookObject, Uri uri)
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