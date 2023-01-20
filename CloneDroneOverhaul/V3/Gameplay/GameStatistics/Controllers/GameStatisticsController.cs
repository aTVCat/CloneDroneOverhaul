using CloneDroneOverhaul.V3.Base;
using CloneDroneOverhaul.V3;
using UnityEngine;

namespace CloneDroneOverhaul.V3.Gameplay
{
    public class GameStatisticsController : V3_ModControllerBase
    {
        private static bool _hasinitialized;
        public static GameStatistic GameStatistics { get; set; }

        /// <summary>
        /// Used to track if gamemode was changed
        /// </summary>
        private GameMode _gameModeSetLastUpdate;

        private float _soundVolume;
        private bool _lerpingVolume;
        private float _interpolator;

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
                TrySetPlayer(args[0] as CloneDroneOverhaul.V3.RobotShortInformation);
            }
            if (eventName == "onBoltShutdown")
            {
                _soundVolume = AudioListener.volume;
                _lerpingVolume = true;
                _interpolator = 0f;
            }
        }

        private void Update()
        {
            if (_lerpingVolume)
            {
                _interpolator += 0.75f * Time.deltaTime;
                AudioListener.volume = BaseUtils.SmoothChangeFloat(_soundVolume, 0f, _interpolator);
                if (_interpolator > 0.99f)
                {
                    _lerpingVolume = false;
                }
            }

            GameMode newGameMode = GameFlowManager.Instance.GetCurrentGameMode();
            if (newGameMode != _gameModeSetLastUpdate)
            {
                OverhaulMain.Modules.ExecuteFunction<GameMode>("onGameModeUpdated", newGameMode);
            }
            _gameModeSetLastUpdate = newGameMode;
        }
    
        public void TrySetPlayer(in CloneDroneOverhaul.V3.RobotShortInformation info)
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