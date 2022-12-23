using CloneDroneOverhaul.V3Tests.Base;
using CloneDroneOverhaul.Utilities;

namespace CloneDroneOverhaul.V3Tests.Gameplay
{
    public class GameStatistic 
    {
        /// <summary>
        /// How many robots have you played as?
        /// </summary>
        public int ControlledCharacterCount { get; set; }

        /// <summary>
        /// A basic info about your robot body
        /// </summary>
        public RobotShortInformation PlayerRobotInformation { get; set; }
    }
}