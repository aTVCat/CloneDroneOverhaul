using DiscordWebhook;
using Newtonsoft.Json;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace CDOverhaul
{
    public static class OverhaulWebhooksController
    {
        private const string START_ENABLED_EMOJI = "<:star_enabled:1196444610751373434>";
        private const string START_DISABLED_EMOJI = "<:star_disabled:1196444266772316182>";

        public const string CrashReportsWebhook = "https://discord.com/api/webhooks/1106574827806019665/n486TzxFbaF6sMmbqg2CUHKGN1o15UpR9AUJAmi5c7sdIwI1jeXpTReD4jtZ3U76PzWS";
        public static readonly Uri CrashReportsWebhookUri = new Uri(CrashReportsWebhook);

        public const string SurveysWebhook = "https://discord.com/api/webhooks/1124285317768290454/QuXjaAywp5eRXT2a5BfOtYGFS9h2eHb8giuze3yxLkZ1Y7m7m2AOTfxf9hB4IeCIkTk5";
        public static readonly Uri SurveysWebhookUri = new Uri(SurveysWebhook);

        public const string ErrorsWebhook = "https://discord.com/api/webhooks/1129035917324189745/FGpPRyvgI9YxyrCutXPoWrIGjJ0Z0KueFs4_pqU2wSLUsmYfVYm_qR9yTt-XST40ntSp";
        public static readonly Uri ErrorsWebhookUri = new Uri(ErrorsWebhook);

        [OverhaulSettingAttribute("Mod.Information.Send crash reports", true, false, "Once the game is crashed, a crash log will be sent to mod owner")]
        public static bool AllowSendingInformation;

        public static readonly string[] IgnoredStrings = new string[]
        {
            "DebugMenu.OnThrowErrorClicked",
            "UnityExplorer",
            "Unsupported color:",
            "input",
            "Input",
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
            "ThrowIfDisposedAndClosed",
            "GetCurrentAnimatorStateInfo",
            "MoreSkinsMod",
            "GunMod",
            "ChangeToNotSelectedVisuals",
            "HammerImpactMeleeArea",
            "StartConstructionSequence"
        };

        private static bool s_HasExecutedCrashReportsWebhook = false;
        private static readonly List<string> s_CaughtErrors = new List<string>();

        public static void ExecuteCrashReportsWebhook(string content, bool forceSend = false)
        {
            if (s_HasExecutedCrashReportsWebhook && !forceSend)
                return;
            s_HasExecutedCrashReportsWebhook = true;

            WebhookObject obj1 = new WebhookObject()
            {
                content = (OverhaulVersion.IsDebugBuild ? "<@779372500521320469> " : string.Empty) +
                "__New crash report! Version: " + OverhaulVersion.ModVersion + "__",
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

        public static void ExecuteSurveysWebhook(int rank, string improveText, string likedText)
        {
            rank = Mathf.Clamp(rank, 1, 5);
            string userInfo = $"- **User:** {SteamFriends.GetPersonaName()} [[Profile]](<https://steamcommunity.com/profiles/{SteamUser.GetSteamID()}>)";
            string deviceInfo = $"- **OS:** {SystemInfo.operatingSystem}\n- **CPU:** {SystemInfo.processorType}\n * {SystemInfo.processorCount}/{SystemInfo.processorFrequency}\n- **GPU:** {SystemInfo.graphicsDeviceName}\n * {SystemInfo.graphicsMemorySize} MBs\n- **Memory:** {SystemInfo.systemMemorySize} MBs";
            string rankText = string.Empty;
            for (int i = 0; i < rank; i++)
            {
                rankText += START_ENABLED_EMOJI;
                rankText += " ";
            }
            for (int i = 0; i < (5 - rank); i++)
            {
                rankText += START_DISABLED_EMOJI;
                rankText += " ";
            }

            int color = int.Parse("32a852", System.Globalization.NumberStyles.HexNumber);
            WebhookObject obj1 = new WebhookObject()
            {
                content = $"## __Feedback. v{OverhaulVersion.ModVersion}__ {rankText}",
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