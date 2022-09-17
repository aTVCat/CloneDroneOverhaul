using ModLibrary;
using System.Collections.Generic;
using System.Diagnostics;
using CloneDroneOverhaul.Utilities;

namespace CloneDroneOverhaul
{
    public class BaseStaticReferences
    {
        public static Modules.ModuleManagement ModuleManager { get; internal set; }
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
    }

    public class BaseUtils
    {
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
                            baseBodyPart.CrackVolumeAround(baseBodyPart.transform.position, UnityEngine.Vector3.down, nextAttackID, null, 2, null, -1, DamageSourceType.SpeedHackBanFire, null);
                        }
                    }
                }
            }
        }

        public static void DebugFireSword()
        {
            FirstPersonMover mover = CharacterTracker.Instance.GetPlayerRobot();
            if(mover != null)
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
}
