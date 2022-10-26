using CloneDroneOverhaul.Modules;
using CloneDroneOverhaul.Utilities;
using ModLibrary;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace CloneDroneOverhaul
{
    public class BaseStaticReferences
    {
        public static Modules.ModuleManagement ModuleManager { get; internal set; }
        public static Modules.SettingsManager SettingsManager { get; internal set; }
        public static UI.GUIManagement GUIs { get; internal set; }
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
        public static void SpawnLevelFromPath(string path, bool clearLevels, System.Action callback = null)
        {
            if (clearLevels) LevelManager.Instance.CleanUpLevelThisFrame();
            if (System.IO.File.Exists(path))
            {
                LevelEditorLevelData levelEditorLevelData = LevelManager.Instance.LoadLevelEditorLevelData(path);
                Transform root = new GameObject("ExplorationModeLevel").transform;
                StaticCoroutineRunner.StartStaticCoroutine(SpawnLevel(root, levelEditorLevelData, callback));
            }
            else
            {
                ModuleManagement.ShowError("Level " + path + " was not found");
            }
        }
        private static IEnumerator SpawnLevel(Transform transform, LevelEditorLevelData data, System.Action callback = null)
        {
            yield return OverhaulMain.LocalMonoBehaviour.StartCoroutine(LevelEditorDataManager.Instance.DeserializeInto(transform, data, true));
            if(callback != null) callback();
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
            FileManagerStuff.OpenSkinsFolder();
        }
    }

    public static class VanillaPrefs
    {
        static bool[] EscMenuChildrenVisibility;
        public readonly static AudioConfiguration AudioConfig = AudioSettings.GetConfiguration().Clone();

        internal static void RememberStuff()
        {
            EscMenuChildrenVisibility = new bool[GameUIRoot.Instance.EscMenu.transform.childCount - 1];
            for(int i = 0; i < EscMenuChildrenVisibility.Length; i++)
            {
                EscMenuChildrenVisibility[i] = GameUIRoot.Instance.EscMenu.transform.GetChild(i).gameObject.activeSelf;
            }
        }
    }

    public static class FileManagerStuff
    {
        internal static Process ExplorerProcess { get; set; }

        public static void OpenSkinsFolder()
        {
            ExplorerProcess = Process.Start(new ProcessStartInfo()
            {
                FileName = "C:\\Program Files (x86)\\Steam\\steamapps\\common",
                UseShellExecute = true,
                Verb = "open"
            });
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
        }

        public Modules.CloneDroneOverhaulSettingsData SettingsData;
    }
}
