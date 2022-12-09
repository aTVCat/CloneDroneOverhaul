using CloneDroneOverhaul.Modules;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CloneDroneOverhaul.Gameplay.OverModes
{
    public class OverModeBase : Singleton<OverModeBase>
    {
        public virtual void Initialize()
        {
            throw new NotImplementedException("OverModeBase.Initialize() method needs override method");
        }

        public virtual GameData GetLegacyGameData()
        {
            return null;
        }

        public virtual GameMode GetGamemode()
        {
            return GameMode.None;
        }

        public virtual List<LevelDescription> GetLevelDescriptions()
        {
            return null;
        }

        public virtual void ProcessEvent(OverModeBase.EventNames eventName, object[] args)
        {
        }

        public virtual T ProcessEventAndReturn<T>(OverModeBase.EventNames eventName, object[] args) where T : class
        {
            return default(T);
        }

        public static void SpawnEnemy(EnemyType type, Vector3 position)
        {
            Singleton<EnemyFactory>.Instance.SpawnEnemy(type, position, Vector3.zero);
        }

        public virtual void StartOvermode(Action onStartDone = null, bool spawnPlayer = false)
        {
            Singleton<GameFlowManager>.Instance.HideTitleScreen(true);
            Singleton<LevelManager>.Instance.CleanUpLevelThisFrame();
            BoltGlobalEventListenerSingleton<SingleplayerServerStarter>.Instance.StartServerThenCall(delegate
            {
                Singleton<GameFlowManager>.Instance.SetGameMode(this.GetGamemode());
                ArenaManager.SetLogoVisible(false);
                if (spawnPlayer)
                {
                    GameplayOverhaulModule.Instance.CreateLiftAndSpawnPlayer();
                }
                if (onStartDone != null)
                {
                    onStartDone();
                }
            });
        }

        public const string OVERMODELEVELTAG = "Overmode_DefaultLevelTag";

        public enum EventNames
        {
            None,
            SpawnLevel
        }
    }
}