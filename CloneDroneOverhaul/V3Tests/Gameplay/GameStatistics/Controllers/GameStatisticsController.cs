using CloneDroneOverhaul.V3Tests.Base;
using CloneDroneOverhaul.Utilities;

namespace CloneDroneOverhaul.V3Tests.Gameplay
{
    public class GameStatisticsController : V3_ModControllerBase
    {
        private static bool _hasinitialized;
        public static GameStatistic GameStatistics { get; set; }

        internal static void Initialize()
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
                TrySetPlayer(args[0] as CloneDroneOverhaul.Utilities.RobotShortInformation);
            }
        }

        public void TrySetPlayer(in CloneDroneOverhaul.Utilities.RobotShortInformation info)
        {
            if(info == null || info.IsNull)
            {
                return;
            }

            GameStatistics.PlayerRobotInformation = info;
            GameStatistics.ControlledCharacterCount++;

            Optimisation.OptimiseOnStartup.SetArenaCameraEnabled();
            ArenaController.ArenaInterior.ArenaTVs.gameObject.SetActive(true);
        }

        public void TrySetPlayer()
        {
            TrySetPlayer(CharacterTracker.Instance.GetPlayer().GetRobotInfo());
        }
    }
}