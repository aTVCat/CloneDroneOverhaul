using UnityEngine;
using CloneDroneOverhaul.Utilities;
using CloneDroneOverhaul.Modules;

namespace CloneDroneOverhaul.Gameplay.OverModes
{
    public class OverModeBase : Singleton<OverModeBase>
    {
        /// <summary>
        /// Called when instance of the class is created
        /// </summary>
        public virtual void Initialize()
        {
            throw new System.NotImplementedException("OverModeBase.Initialize() method needs override method");
        }

        /// <summary>
        /// Get a gamemode value to set when we start playing
        /// </summary>
        /// <returns></returns>
        public virtual GameMode GetOverModeGameMode()
        {
            return GameMode.None;
        }

        /// <summary>
        /// Start gameplay
        /// </summary>
        public virtual void StartGamemode()
        {
            GameFlowManager.Instance.HideTitleScreen(true);
            LevelManager.Instance.CleanUpLevelThisFrame();
            SingleplayerServerStarter.Instance.StartServerThenCall(delegate
            {
                GameFlowManager.Instance.SetGameMode(GetOverModeGameMode());
                ArenaManager.SetLogoVisible(false);
                GameplayOverhaulModule.Instance.CreateLiftAndSpawnPlayer();
            });
        }
    }
}
