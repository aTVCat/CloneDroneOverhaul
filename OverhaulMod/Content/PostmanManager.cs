using DiscordWebhook;
using Newtonsoft.Json;
using OverhaulMod.Content;
using OverhaulMod.Content.Personalization;
using OverhaulMod.Utils;
using Steamworks;
using System;
using System.IO;
using System.Net;
using System.Security.Policy;
using System.Threading.Tasks;
using UnityEngine;

namespace OverhaulMod
{
    public class PostmanManager : Singleton<PostmanManager>
    {
        private const string START_ENABLED_EMOJI = "<:star_enabled:1196444610751373434>";
        private const string START_DISABLED_EMOJI = "<:star_disabled:1196444266772316182>";

        public static string ErrorReportText;

        private static Destinations s_destinations;

        public void SendCrashReport(string text, Action successCallback, Action<string> errorCallback)
        {
            int color = int.Parse("d63a51", System.Globalization.NumberStyles.HexNumber);
            string deviceInfo = $"- **OS:** {SystemInfo.operatingSystem}\n- **CPU:** {SystemInfo.processorType}\n * {SystemInfo.processorCount}/{SystemInfo.processorFrequency}\n- **GPU:** {SystemInfo.graphicsDeviceName}\n * {SystemInfo.graphicsMemorySize} MBs\n- **Memory:** {SystemInfo.systemMemorySize} MBs";
            string gameMode = "N/A";
            try
            {
                gameMode = GameFlowManager.Instance._gameMode.ToString();
            }
            catch { }

            string levelId = "N/A";
            try
            {
                levelId = LevelManager.Instance.GetCurrentLevelID();
            }
            catch { }

            string gameVer = "N/A";
            try
            {
                gameVer = VersionNumberManager.Instance.GetVersionString();
            }
            catch { }

            string lang = "N/A";
            try
            {
                lang = LocalizationManager.Instance.GetCurrentLanguageCode();
            }
            catch { }

            string time = "N/A";
            try
            {
                time = Time.unscaledTime.ToString();
            }
            catch { }

            if (text.Length > 1490)
                text = text.Remove(1490);

            text = $"```{text}```";

            WebhookObject obj1 = new WebhookObject()
            {
                content = $"## __Game crashed. v{ModBuildInfo.version}__",
                embeds = new Embed[]
                {
                    new Embed()
                    {
                        title = "**Stack trace**",
                        description = text,
                        color = color,
                    },

                    new Embed()
                    {
                        title = "**Details**",
                        description = $"{gameMode}, {levelId}, {gameVer}, {lang}, {time}\n{deviceInfo}",
                        color = color,
                    },
                },
            };

            DownloadDestinations(delegate (Destinations destinations)
            {
                if (destinations == null)
                {
                    errorCallback?.Invoke("Could not get the link");
                    return;
                }

                SendMessage(obj1, destinations.CrashReport, successCallback, errorCallback);
            });
        }

        public void SendFeedback(int rank, string improveText, string likedText, Action successCallback, Action<string> errorCallback)
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
                content = $"## __Feedback. v{ModBuildInfo.version}__ {rankText}",
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

            DownloadDestinations(delegate (Destinations destinations)
            {
                if (destinations == null)
                {
                    errorCallback?.Invoke("Could not get the link");
                    return;
                }

                SendMessage(obj1, destinations.Feedback, successCallback, errorCallback);
            });
        }

        public void SendSurveyAnswer(string selectedVariant, string newsTitle, Action successCallback, Action<string> errorCallback)
        {
            int color = int.Parse("32a852", System.Globalization.NumberStyles.HexNumber);
            string userInfo = $"- **User:** {SteamFriends.GetPersonaName()} [[Profile]](<https://steamcommunity.com/profiles/{SteamUser.GetSteamID()}>)";
            WebhookObject obj1 = new WebhookObject()
            {
                content = $"## __Survey answer. v{ModBuildInfo.version}__ {newsTitle}",
                embeds = new Embed[]
                {
                    new Embed()
                    {
                        title = "**Selected answer**",
                        description = selectedVariant,
                        color = color,
                    },

                    new Embed()
                    {
                        title = "**Details**",
                        description = $"{userInfo}",
                        color = color,
                    },
                },
            };

            DownloadDestinations(delegate (Destinations destinations)
            {
                if (destinations == null)
                {
                    errorCallback?.Invoke("Could not get the link");
                    return;
                }

                SendMessage(obj1, destinations.Survey, successCallback, errorCallback);
            });
        }

        public void SendVerificationRequest(string zipPath, PersonalizationItemInfo personalizationItem, Action successCallback, Action<string> errorCallback)
        {
            string weaponString;
            switch (personalizationItem.Weapon)
            {
                case Combat.ModWeaponsManager.SCYTHE_TYPE:
                    weaponString = "Scythe";
                    break;
                default:
                    weaponString = personalizationItem.Weapon.ToString();
                    break;
            }

            int color = int.Parse("32a852", System.Globalization.NumberStyles.HexNumber);
            string userInfo = $"- **User:** {SteamFriends.GetPersonaName()} [[Profile]](<https://steamcommunity.com/profiles/{ModUserInfo.localPlayerSteamID}>)\n- **PlayFab ID:** {ModUserInfo.localPlayerPlayFabID}";
            string itemInfo = $"- **Name:** {personalizationItem.Name}\n- **Author:** {personalizationItem.GetAuthorsString()}\n- **Category:** {personalizationItem.Category}\n- **Weapon:** {weaponString}\n- **Version:** {personalizationItem.Version}";

            WebhookObject obj1 = new WebhookObject()
            {
                content = $"## __{(personalizationItem.IsVerified ? "An item to update" : "New item to verify")}. v{ModBuildInfo.version}__{(personalizationItem.IsSentForVerification ? "\n# REUPLOAD" : string.Empty)}\nid: {personalizationItem.ItemID}",
                embeds = new Embed[]
                {
                    new Embed()
                    {
                        title = "**Information**",
                        description = itemInfo,
                        color = color,
                    },

                    new Embed()
                    {
                        title = "**Details**",
                        description = $"{userInfo}",
                        color = color,
                    },
                },
            };

            DownloadDestinations(delegate (Destinations destinations)
            {
                if(destinations == null)
                {
                    errorCallback?.Invoke("Could not get the link");
                    return;
                }

                SendMessage(obj1, destinations.VerificationRequest, delegate
                {
                    SendFile(zipPath, destinations.VerificationRequest, successCallback, errorCallback);
                }, errorCallback);
            });
        }

        public async void SendMessage(WebhookObject webhookObject, string url, Action successCallback, Action<string> errorCallback)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    System.Threading.Tasks.Task<string> task = webClient.UploadStringTaskAsync(new Uri(url), "POST", JsonConvert.SerializeObject(webhookObject));
                    _ = await task;
                    if (task.Exception != null)
                    {
                        ModDebug.LogWarning(task.Exception);
                        errorCallback?.Invoke(task.Exception.ToString());
                    }
                    else
                    {
                        successCallback?.Invoke();
                    }
                }
            }
            catch (Exception exc)
            {
                ModDebug.LogWarning(exc);
                errorCallback?.Invoke(exc.ToString());
            }
        }

        public async void SendFile(string filePath, string url, Action successCallback, Action<string> errorCallback)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    System.Threading.Tasks.Task<byte[]> task = webClient.UploadFileTaskAsync(new Uri(url), "POST", filePath);
                    _ = await task;
                    if (task.Exception != null)
                    {
                        ModDebug.LogWarning(task.Exception);
                        errorCallback?.Invoke(task.Exception.ToString());
                    }
                    else
                    {
                        successCallback?.Invoke();
                    }
                }
            }
            catch (Exception exc)
            {
                ModDebug.LogWarning(exc);
                errorCallback?.Invoke(exc.ToString());
            }
        }

        public void DownloadDestinations(Action<Destinations> result)
        {
            if(s_destinations != null)
            {
                if (result != null)
                    result(s_destinations);

                return;
            }

            string tempPath = Path.GetTempFileName();
            GoogleDriveManager.Instance.DownloadFile("https://drive.google.com/file/d/1H5L3Yt_xEuy1RvLFv0DN68bXwCav7x6P/view?usp=drive_link", tempPath, null,
            delegate (string r)
            {
                if (!r.IsNullOrEmpty())
                {
                    if (result != null)
                        result(null);

                    return;
                }

                try
                {
                    s_destinations = Destinations.Deserialize(tempPath);
                }
                catch
                {
                    if (result != null)
                        result(null);
                }

                try
                {
                    File.Delete(tempPath);
                }
                catch { }

                if (result != null)
                    result(s_destinations);
            });
        }

        public class Destinations
        {
            public string CrashReport, Feedback, Survey, VerificationRequest;

            public static void Serialize(Destinations destinations, string path = null)
            {
                if (path.IsNullOrEmpty())
                    path = Path.Combine(ModCore.savesFolder, "Destinations.json");

                ModJsonUtils.WriteStream(path, destinations);
            }

            public static Destinations Deserialize(string path)
            {
                return ModJsonUtils.DeserializeStream<Destinations>(path);
            }

            public static Destinations CreateNew() => new Destinations();
        }
    }
}