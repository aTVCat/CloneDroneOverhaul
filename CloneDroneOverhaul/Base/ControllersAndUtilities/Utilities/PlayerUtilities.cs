using UnityEngine;

namespace CloneDroneOverhaul.Utilities
{
    public static class PlayerUtilities
    {
        public static RobotShortInformation GetPlayerRobotInfo()
        {
            return GetRobotInfo(CharacterTracker.Instance.GetPlayer());
        }

        public static RobotShortInformation GetRobotInfo(this Character character)
        {
            RobotShortInformation info = new RobotShortInformation
            {
                Instance = character
            };
            if (info.IsNull)
            {
                return info;
            }
            info.Instance = character;
            info.RobotCamera = character.GetPlayerCamera();
            info.UpgradeCollection = character.GetComponent<UpgradeCollection>();
            info.CharacterType = character.CharacterType;
            if (character is BattleCruiserController)
            {
                info.IsBattleCruiser = true;
            }
            if (character is FirstPersonMover)
            {
                info.IsFPM = true;

                FirstPersonMover mover = character as FirstPersonMover;
                info.IsFPMMindspace = mover.IsMindSpaceCharacter;
            }
            info.PlayfabID = character.GetPlayFabID();
            return info;
        }

        public static void AddUpgradeToRobot(this FirstPersonMover mover, UpgradeType type, int level = 1)
        {
            RobotShortInformation info = mover.GetRobotInfo();
            if (!info.IsNull && info.IsFPM)
            {
                UpgradeCollection coll = info.UpgradeCollection;
                if (coll != null)
                {
                    coll.AddUpgradeIfMissing(type, level);
                    (info.Instance as FirstPersonMover).RefreshUpgrades();
                    (info.Instance as FirstPersonMover).RefreshSizeUpgrades();
                }
                else
                {
                    Modules.ModuleManagement.ShowError("Upgrade adding failed. Reason: No upgrade collection found");
                }
            }
            else
            {
                Modules.ModuleManagement.ShowError("Upgrade adding failed. Reason: FirstPersonMover is NULL");
            }
        }
    }

    public class RobotShortInformation
    {
        public bool IsNull
        {
            get
            {
                if (!Instance || this is null)
                {
                    return true;
                }
                return false;
            }
        }

        public bool IsFPMMindspace;

        public bool IsBattleCruiser;

        public bool IsFPM;

        public string PlayfabID;

        public EnemyType CharacterType;

        public Character Instance;

        public UpgradeCollection UpgradeCollection;

        public Camera RobotCamera;
    }

    public struct RobotSpawnInfo
    {
        public Vector3 Position;
        public Vector3 Rotation;

        public Character SpawnPlayer()
        {
            GameObject obj = new GameObject("TempSpawnpoint");
            obj.transform.position = Position;
            obj.transform.eulerAngles = Rotation;
            Character charr = GameFlowManager.Instance.SpawnPlayer(obj.transform, true, true, null);
            GameObject.Destroy(obj);
            return charr;
        }
    }
}
