using CloneDroneOverhaul.Modules;
using CloneDroneOverhaul.Utilities;
using UnityEngine;

namespace CloneDroneOverhaul.V3Tests.Gameplay
{
    public class ExplorationGameModeManager_OLD : ModuleBase
    {
        public const GameMode EXPLORATION_GAMEMODE = (GameMode)8259;

        public override void Start()
        {
        }

        public void StartGameWithLevel(string levelJSONPath)
        {
            GameFlowManager.Instance.HideTitleScreen(true);
            SingleplayerServerStarter.Instance.StartServerThenCall(delegate
            {
                GameFlowManager.Instance.SetGameMode(EXPLORATION_GAMEMODE);
                ArenaManager.SetArenaVisible(false);
                ArenaManager.SetLogoVisible(false);
                BaseUtils.SpawnLevelFromPath(levelJSONPath, true, SpawnPlayer);
            });
        }

        private void SpawnPlayer()
        {
            RobotSpawnInfo info = new RobotSpawnInfo
            {
                Rotation = Vector3.zero,
                Position = new Vector3(0, 100, 0)
            };
            info.SpawnPlayer();
        }
    }
}
