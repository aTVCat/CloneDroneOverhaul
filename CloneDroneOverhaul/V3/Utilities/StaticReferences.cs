using CloneDroneOverhaul.Modules;
using CloneDroneOverhaul.V3;
using CloneDroneOverhaul.V3.Utilities;
using ModLibrary;
using Newtonsoft.Json;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.Networking;
using CloneDroneOverhaul.V3.Notifications;

namespace CloneDroneOverhaul
{
    internal class BaseStaticValues
    {
        public static bool IsEscMenuWaitingToShow { get; set; }
    }

    public static class BaseUtils
    {
        public static class SteamWorkshopUtils
        {
            public enum VoteInfo
            {
                None,
                VoteUp,
                VoteDown
            }
            /// <summary>
            /// Gets the vote of item
            /// </summary>
            /// <param name="item"></param>
            /// <param name="onGetInfo"></param>
            public static void GetUserVote(SteamWorkshopItem item, Action<VoteInfo> onGetInfo)
            {
                VoteInfo vote = VoteInfo.None;
                SteamAPICall_t userItemVote = SteamUGC.GetUserItemVote(item.WorkshopItemID);
                CallResult<GetUserItemVoteResult_t> callRes = CallResult<GetUserItemVoteResult_t>.Create(delegate (GetUserItemVoteResult_t t, bool failure)
                {
                    if (failure || t.m_bVoteSkipped)
                    {
                        onGetInfo(vote);
                    }
                    else
                    {
                        if (t.m_bVotedUp)
                        {
                            vote = VoteInfo.VoteUp;
                        }
                        if (t.m_bVotedDown)
                        {
                            vote = VoteInfo.VoteDown;
                        }
                        onGetInfo(vote);
                    }
                });
            }
            /// <summary>
            /// Sets vote for item
            /// </summary>
            /// <param name="item"></param>
            /// <param name="vote"></param>
            public static void SetUserVote(SteamWorkshopItem item, VoteInfo vote)
            {
                if (vote == VoteInfo.None)
                {
                    return;
                }

                SteamAPICall_t hAPICall = SteamUGC.SetUserItemVote(item.WorkshopItemID, vote == VoteInfo.VoteUp ? true : false);
                CallResult<SetUserItemVoteResult_t> callResult = CallResult<SetUserItemVoteResult_t>.Create(delegate (SetUserItemVoteResult_t t, bool failure)
                {
                    if (failure)
                    {
                        return;
                    }
                });
                callResult.Set(hAPICall, null);
            }
            /// <summary>
            /// Checks if the user is subscribed to item
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            public static bool HasUserSubscribedToItem(string tag, int page)
            {
                return false;
            }
            /// <summary>
            /// Gets the number of all subscribed items
            /// </summary>
            /// <returns></returns>
            public static uint GetNumSubscribedItems()
            {
                uint result = SteamUGC.GetNumSubscribedItems();
                return result;
            }
            /// <summary>
            /// Gets all subscribed items
            /// </summary>
            /// <returns></returns>
            public static PublishedFileId_t[] GetSubscribedItems()
            {
                uint itemsCount = GetNumSubscribedItems();
                PublishedFileId_t[] array = new PublishedFileId_t[itemsCount];
                SteamUGC.GetSubscribedItems(array, itemsCount);
                return array;
            }
            public static float GetItemLoadProgress(SteamWorkshopItem item)
            {
                bool isValid = SteamUGC.GetItemDownloadInfo(item.WorkshopItemID, out ulong current, out ulong total);
                if (!isValid || total == 0)
                {
                    return -1f;
                }
                return (float)current / (float)total;
            }
        }
        public static class ImageUtils
        {
            public static void LoadSpriteFromFile(string path, Action<Sprite> onLoaded)
            {
                StaticCoroutineRunner.StartStaticCoroutine(loadSpriteFromFile(path, onLoaded));
            }

            private static IEnumerator loadSpriteFromFile(string path, Action<Sprite> onLoaded)
            {
                Sprite result = null;
                LoadTextureFromFile(path, delegate (Texture2D tex)
                {
                    result = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                });
                onLoaded(result);
                yield break;
            }

            public static void LoadTextureFromFile(string path, Action<Texture2D> onLoaded)
            {
                StaticCoroutineRunner.StartStaticCoroutine(loadTextureFromFile(path, onLoaded));
            }

            private static IEnumerator loadTextureFromFile(string path, Action<Texture2D> onLoaded)
            {
                Texture2D tex = new Texture2D(0, 0);
                tex.LoadImage(File.ReadAllBytes(path), false);
                onLoaded(tex);
                yield break;
                using (UnityWebRequest request = UnityWebRequestTexture.GetTexture("file://" + path))
                {
                    yield return request.SendWebRequest();
                    tex = DownloadHandlerTexture.GetContent(request);
                    onLoaded(tex);
                }
            }
        }

        public static T TryLoad<T>(string filepath)
        {
            T loadedData = default(T);
            if (File.Exists(filepath))
            {
                string text = File.ReadAllText(filepath);
                loadedData = JsonConvert.DeserializeObject<T>(text, DataRepository.Instance.GetSettings());
            }
            return loadedData;
        }
        public static Color ColorFromHex(string hex)
        {
            Color result = new Color();

            ColorUtility.TryParseHtmlString(hex, out result);

            return result;
        }

        public static SelectableUI AddSelectableUIToObject(GameObject obj)
        {
            SelectableUI result = null;
            result = obj.AddComponent<SelectableUI>();
            result.GameThemeData = ObjectFixer.GameUIThemeData;
            return result;
        }

        public static void SpawnLevelFromPath(string path, bool clearLevels, System.Action callback = null)
        {
            if (clearLevels)
            {
                LevelManager.Instance.CleanUpLevelThisFrame();

                List<Transform> list = V3.Base.GameInformationController.OvermodeLevelTransforms;
                if (list.Count != 0)
                {
                    for (int i = list.Count - 1; i > -1; i--)
                    {
                        UnityEngine.GameObject.Destroy(list[i].gameObject);
                    }
                }
                V3.Base.GameInformationController.OvermodeLevelTransforms.Clear();
            }
            if (System.IO.File.Exists(path))
            {
                LevelEditorLevelData levelEditorLevelData = LevelManager.Instance.LoadLevelEditorLevelData(path);
                Transform root = new GameObject("Level").transform;
                V3.Base.GameInformationController.OvermodeLevelTransforms.Add(root);
                StaticCoroutineRunner.StartStaticCoroutine(SpawnLevel(root, levelEditorLevelData, callback));
            }
            else
            {
                ModuleManagement.ShowError("Level " + path + " was not found");
            }
        }
        private static IEnumerator SpawnLevel(Transform transform, LevelEditorLevelData data, System.Action callback = null)
        {
            yield return StaticCoroutineRunner.StartStaticCoroutine(LevelEditorDataManager.Instance.DeserializeInto(transform, data, true));
            if (callback != null)
            {
                callback();
            }

            yield break;
        }

        public static ModLibrary.ModInfo GetModInfoByID(string id)
        {
            foreach (ModLibrary.ModInfo info in InternalModBot.ModsManager.Instance.GetActiveModInfos())
            {
                if (info.UniqueID == id)
                {
                    return info;
                }
            }
            return null;
        }

        public static void QuitGame()
        {
            UnityEngine.Application.Quit();
        }
        public static void CopyToClipboard(string text, bool showMessage = false)
        {
            UnityEngine.TextEditor textEditor = new UnityEngine.TextEditor
            {
                text = text
            };
            textEditor.SelectAll();
            textEditor.Copy();
            if (showMessage)
            {
                SNotification notif = new SNotification("Text or code copied to clipboard!", text, 20f, UINotifications.NotificationSize_Small, null);
                notif.Send();
            }
        }

        public static void CopyLobbyCode()
        {
            CopyToClipboard(MultiplayerMatchmakingManager.Instance.GetLastInviteCode(), true);
        }


        public static float SmoothChangeFloat(float initVal, float endVal, float time)
        {
            return Mathf.Lerp(initVal, endVal, time);
        }

        public static void OpenURL(string url)
        {
            bool flag = url.Contains("steamcommunity.com");
            bool flag2 = url.Contains("steampowered.com");
            if ((flag || flag2) && Singleton<SteamManager>.Instance.Initialized && Steamworks.SteamUtils.IsOverlayEnabled())
            {
                Steamworks.SteamFriends.ActivateGameOverlayToWebPage(url);
                return;
            }
            UnityEngine.Application.OpenURL(url);
        }

        public static void IgnoreLastCrash()
        {
            Singleton<ErrorManager>.Instance.SetPrivateField("_hasCrashed", false);
            Singleton<GameUIRoot>.Instance.ErrorWindow.Hide();
            Singleton<TimeManager>.Instance.OnGameUnPaused();
        }

        public static void AddSkillPoint()
        {
            if (GameModeManager.IsMultiplayer())
            {
                return;
            }
            UpgradeManager.Instance.SetAvailableSkillPoints(UpgradeManager.Instance.GetAvailableSkillPoints() + 1);
        }

        public static void ExplodePlayer()
        {
            bool flag = Singleton<CharacterTracker>.Instance.GetPlayerRobot() == null;
            if (!flag)
            {
                FirstPersonMover playerRobot = Singleton<CharacterTracker>.Instance.GetPlayerRobot();
                bool flag2 = !playerRobot.IsAttachedAndAlive();
                if (!flag2)
                {
                    List<BaseBodyPart> allBaseBodyParts = playerRobot.GetAllBaseBodyParts();
                    int nextAttackID = Singleton<AttackManager>.Instance.GetNextAttackID();
                    foreach (BaseBodyPart baseBodyPart in allBaseBodyParts)
                    {
                        bool activeInHierarchy = baseBodyPart.gameObject.activeInHierarchy;
                        if (activeInHierarchy)
                        {
                            baseBodyPart.CrackVolumeAround(baseBodyPart.transform.position, new UnityEngine.Vector3(0, 100, 0), nextAttackID, null, 2, null, -1, (DamageSourceType)31, null);
                        }
                    }
                }
            }
        }

        public static void DebugFireSword()
        {
            FirstPersonMover mover = CharacterTracker.Instance.GetPlayerRobot();
            if (mover != null)
            {
                mover.AddUpgradeToRobot(UpgradeType.SwordUnlock, 1);
                mover.AddUpgradeToRobot(UpgradeType.FireSword, 2);
                mover.GetEnergySource().HasInfiniteEnergy = true;
            }
        }
    }

    public static class VanillaPrefs
    {
        public static AudioConfiguration AudioConfig = AudioSettings.GetConfiguration().Clone();

        internal static void RememberStuff()
        {
            AudioConfig = AudioSettings.GetConfiguration().Clone();
        }
    }

    public static class FileManagerStuff
    {
        internal static Process ExplorerProcess { get; set; }

        public static void OpenFolder(string path)
        {
            ExplorerProcess = Process.Start(new ProcessStartInfo()
            {
                FileName = path,
                UseShellExecute = true,
                Verb = "open"
            });
        }

        public static string OpenFileSelect(string filter)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;
            OpenFileDialog openFileDialog = null;

            using (openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = filter;
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        fileContent = reader.ReadToEnd();
                    }
                }
            }

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return string.Empty;
            }

            return filePath;
        }
    }


    public static class CloneDroneOverhaulDataContainer
    {
        public static Modules.CloneDroneOverhaulSettingsData SettingsData;

        public static Modules.ModdedLevelEditorSaveData LevelEditorData;

        public static void Initialize()
        {
            ModDataManager dataManager = OverhaulMain.Modules.GetModule<ModDataManager>();
            SettingsData = dataManager.CreateInstanceOfDataClass<CloneDroneOverhaulSettingsData>(true, false);
            LevelEditorData = dataManager.CreateInstanceOfDataClass<ModdedLevelEditorSaveData>(true, false);
        }
    }
}
