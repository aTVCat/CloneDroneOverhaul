using CloneDroneOverhaul.Modules;
using CloneDroneOverhaul.Utilities;
using ModLibrary;
using Newtonsoft.Json;
using Steamworks;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.Networking;

namespace CloneDroneOverhaul
{
    public class BaseStaticReferences
    {
        public static Modules.ModuleManagement ModuleManager { get; internal set; }
        public static Modules.OverhaulSettingsManager SettingsManager { get; internal set; }
        public static UI.NewEscMenu NewEscMenu { get; internal set; }
    }

    internal class BaseStaticValues
    {
        public static string ModDataFolder
        {
            get
            {
                return string.Empty;
            }
        }
        public static bool IsModEnabled { get; internal set; }
        public static bool IsEscMenuWaitingToShow { get; set; }
        public static string GetInviteCode { get; internal set; }
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
                ulong current = 0;
                ulong total = 0;
                bool isValid = SteamUGC.GetItemDownloadInfo(item.WorkshopItemID, out current, out total);
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

            static IEnumerator loadSpriteFromFile(string path, Action<Sprite> onLoaded)
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

            static IEnumerator loadTextureFromFile(string path, Action<Texture2D> onLoaded)
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

        public static Character SpawnRobotWithDisabledComponents(Transform spawnPos, EnemyType type)
        {
            Character result = EnemyFactory.Instance.SpawnEnemy(type, spawnPos.position, spawnPos.eulerAngles).GetComponent<Character>();
            Coroutines.WaitForCharacterInitAndCall(result, delegate
            {
            });
            return result;
        }

        public static SelectableUI AddSelectableUIToObject(GameObject obj)
        {
            SelectableUI result = null;
            result = obj.AddComponent<SelectableUI>();
            result.GameThemeData = Patching.VisualFixes.ObjectFixer.GameUIThemeData;
            return result;
        }

        public static Assembly CompileAsseblyFromFilesInPath(string path)
        {
            Assembly result = null;

            System.CodeDom.Compiler.CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = false;
            parameters.OutputAssembly = "AutoGen.dll";

            result = CodeDomProvider.CreateProvider("CSharp").CompileAssemblyFromSource(parameters, System.IO.File.ReadAllLines("C:\\Program Files (x86)\\Steam\\steamapps\\common\\Clone Drone in the Danger Zone\\mods\\CloneDroneOverhaulRW\\CSScripts\\BWEffect.cs")).CompiledAssembly;

            return result;
        }

        public static void SpawnLevelFromPath(string path, bool clearLevels, System.Action callback = null)
        {
            if (clearLevels)
            {
                LevelManager.Instance.CleanUpLevelThisFrame();

                List<Transform> list = GameInformationManager.Instance.OvermodeLevelTransforms;
                if (list.Count != 0)
                {
                    for (int i = list.Count - 1; i > -1; i--)
                    {
                        UnityEngine.GameObject.Destroy(list[i].gameObject);
                    }
                }
                GameInformationManager.Instance.OvermodeLevelTransforms.Clear();
            }
            if (System.IO.File.Exists(path))
            {
                LevelEditorLevelData levelEditorLevelData = LevelManager.Instance.LoadLevelEditorLevelData(path);
                Transform root = new GameObject("Level").transform;
                GameInformationManager.Instance.OvermodeLevelTransforms.Add(root);
                StaticCoroutineRunner.StartStaticCoroutine(SpawnLevel(root, levelEditorLevelData, callback));
            }
            else
            {
                ModuleManagement.ShowError("Level " + path + " was not found");
            }
        }
        private static IEnumerator SpawnLevel(Transform transform, LevelEditorLevelData data, System.Action callback = null)
        {
            yield return OverhaulMain.MainMonoBehaviour.StartCoroutine(LevelEditorDataManager.Instance.DeserializeInto(transform, data, true));
            if (callback != null) callback();
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

        public static void SetGameSpeed(float timeScale, float time)
        {
            StaticCoroutineRunner.StartStaticCoroutine(setGameSpeed(timeScale, time));
        }
        private static System.Collections.IEnumerator setGameSpeed(float timeScale, float time)
        {
            UnityEngine.Time.timeScale = timeScale;
            yield return new UnityEngine.WaitForSecondsRealtime(time);
            UnityEngine.Time.timeScale = 1;
            yield break;
        }
        public static void QuitGame()
        {
            UnityEngine.Application.Quit();
        }
        public static void CopyToClipboard(string text, bool showMessage = false, string messageP1 = "", string messageP2 = "")
        {
            UnityEngine.TextEditor textEditor = new UnityEngine.TextEditor();
            textEditor.text = text;
            textEditor.SelectAll();
            textEditor.Copy();
            if (showMessage)
            {
                CloneDroneOverhaul.UI.Notifications.Notification notif = new UI.Notifications.Notification();
                notif.SetUp(messageP1 + text + messageP2, "", 10, new UnityEngine.Vector2(450, 52), UnityEngine.Color.clear, new UI.Notifications.Notification.NotificationButton[] { });
            }
        }

        public static void CopyLobbyCode()
        {
            CopyToClipboard(MultiplayerMatchmakingManager.Instance.GetLastInviteCode(), true, "Code ", " was copied to clipboard!");
        }

        public static void SmoothChangeMaterialPropery(UnityEngine.Material material, string propertyName, float targetValue, float multipler = 0.04f)
        {
            if (material.HasProperty(propertyName))
            {
                material.SetFloat(propertyName, material.GetFloat(propertyName) + (targetValue * multipler));
            }
        }

        public static void SmoothChangeImageColor(UnityEngine.UI.Image image, Color initColor, Color targetColor, float time)
        {
            image.color = Color.Lerp(initColor, targetColor, time);
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

        public static void DebugSize()
        {
            FirstPersonMover mover = CharacterTracker.Instance.GetPlayerRobot();
            if (mover != null)
            {
                mover.AddUpgradeToRobot(UpgradeType.Size, 1);
                mover.GetEnergySource().HasInfiniteEnergy = true;
            }
        }

        public static void Console_ShowAppDataPath()
        {
            debug.Log(UnityEngine.Application.dataPath);
        }

        public static void Test_OpenSkinsFolder()
        {
            if (!GameModeManager.IsOnTitleScreen())
            {
                return;
            }
        }
    }

    public static class VanillaPrefs
    {
        static bool[] EscMenuChildrenVisibility;
        public readonly static AudioConfiguration AudioConfig = AudioSettings.GetConfiguration().Clone();

        internal static void RememberStuff()
        {
            EscMenuChildrenVisibility = new bool[GameUIRoot.Instance.EscMenu.transform.childCount - 1];
            for (int i = 0; i < EscMenuChildrenVisibility.Length; i++)
            {
                EscMenuChildrenVisibility[i] = GameUIRoot.Instance.EscMenu.transform.GetChild(i).gameObject.activeSelf;
            }
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


    public class CloneDroneOverhaulDataContainer
    {
        public static CloneDroneOverhaulDataContainer Instance;

        public CloneDroneOverhaulDataContainer()
        {
            Instance = this;
            ModDataManager dataManager = BaseStaticReferences.ModuleManager.GetModule<ModDataManager>();
            SettingsData = dataManager.CreateInstanceOfDataClass<CloneDroneOverhaulSettingsData>(true, false);
            LevelEditorData = dataManager.CreateInstanceOfDataClass<ModdedLevelEditorSaveData>(true, false);
        }

        public Modules.CloneDroneOverhaulSettingsData SettingsData;
        public Modules.ModdedLevelEditorSaveData LevelEditorData;
    }
}
