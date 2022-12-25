using CloneDroneOverhaul.V3Tests.Base;

namespace CloneDroneOverhaul.V3Tests.Gameplay
{
    public class GameStatisticsController : V3_ModControllerBase
    {
        private static bool _hasinitialized;
        public static GameStatistic GameStatistics { get; set; }

        private void Awake()
        {
            if (!_hasinitialized)
            {
                GameStatistics = new GameStatistic();
                _hasinitialized = true;
            }
        }

        public override void OnEvent(in string eventName, in object[] args)
        {
            if (eventName == "onPlayerSet")
            {
                CloneDroneOverhaul.Utilities.RobotShortInformation info = args[0] as CloneDroneOverhaul.Utilities.RobotShortInformation;
                GameStatistics.PlayerRobotInformation = info;
                GameStatistics.ControlledCharacterCount++;

                Optimisation.OptimiseOnStartup.SetArenaCameraEnabled();
                ArenaController.ArenaInterior.ArenaTVs.gameObject.SetActive(true);
            }
        }
    }
}