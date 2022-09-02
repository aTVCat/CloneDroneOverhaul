using ModLibrary;
using System.Collections.Generic;
using System.Diagnostics;

namespace CloneDroneOverhaul
{
    public class BaseStaticReferences
    {
        public static Modules.ModuleManagement ModuleManager { get; internal set; }
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
    }

    public class BaseUtils
    {
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
                            baseBodyPart.CrackVolumeAround(baseBodyPart.transform.position, UnityEngine.Vector3.down, nextAttackID, null, 2, null, -1, (DamageSourceType)993, null);
                        }
                    }
                }
            }
        }

        public static void TrySpawnEnemy()
        {
            ServerToClientsAdminCommand serverToClientsAdminCommand = ServerToClientsAdminCommand.Create(Bolt.GlobalTargets.AllClients, Bolt.ReliabilityModes.ReliableOrdered);
            serverToClientsAdminCommand.CommandType = (int)ServerToClientAdminCommandType.ActivateDebugSwordCutVisualization;
            serverToClientsAdminCommand.Send();
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
