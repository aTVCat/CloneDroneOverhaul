using UnityEngine;

namespace CloneDroneOverhaul.Utilities
{
    public static class PlayerUtilities
    {
        public static RobotShortInformation GetPlayerRobotInfo()
        {
            return GetRobotInfo(CharacterTracker.Instance.GetPlayer());
        }

        public static RobotShortInformation GetRobotInfo(Character character)
        {
            RobotShortInformation info = new RobotShortInformation();
            if (character == null)
            {
                info.IsNull = true;
                return info;
            }
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
        public bool IsNull;

        public bool IsFPMMindspace;

        public bool IsBattleCruiser;

        public bool IsFPM;

        public string PlayfabID;

        public EnemyType CharacterType;
    }
}
