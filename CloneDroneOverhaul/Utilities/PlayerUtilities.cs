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
            RobotShortInformation info = new RobotShortInformation();
            info.Instance = character;
            if (info.IsNull)
            {
                return info;
            }
            info.Instance = character;
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
    }

    public class RobotShortInformation
    {
        public bool IsNull
        {
            get
            {
                if (Instance == null || !Instance.gameObject.activeSelf)
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
    }
}
