using CloneDroneOverhaul.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CloneDroneOverhaul.V3Tests.Gameplay
{
    public class EndlessModeOverhaul : OverModeBase
    {
        /// <summary>
        /// It is always empty
        /// </summary>
        public static readonly List<LevelDescription> LevelDescriptions = new List<LevelDescription>()
        {
            new LevelDescription()
            {
                GeneratedUniqueID = OVERMODELEVELTAG
            }
        };
        public const string LegacyFileName = "OverModeEndlessData";

        /// <summary>
        /// Vanilla like save data
        /// </summary>
        public GameData Data_Legacy;

        public static EndlessModeOverhaul Instance;

        public override void Initialize()
        {
            Instance = this;
            bool succesfullyLoaded = false;
            succesfullyLoaded = Singleton<DataRepository>.Instance.TryLoad<GameData>(LegacyFileName, out Data_Legacy, false);
            if (succesfullyLoaded)
            {
                Data_Legacy.RepairAnyMissingFields(true);
            }
            else
            {
                ResetSaveData();
            }
        }

        private void test_Sword1()
        {
            SpawnEnemy(EnemyType.Swordsman1, new Vector3(0, 30, 0));
        }

        /// <summary>
        /// Resets the gamedata
        /// </summary>
        public void ResetSaveData()
        {
            GameData gameData = new GameData
            {
                NumClones = Singleton<CloneManager>.Instance.GetNumStartingClones(),
                NumLevelsWon = 0,
                AvailableSkillPoints = 0,
                PlayerUpgrades = Singleton<UpgradeManager>.Instance.CreateDefaultPlayerUpgrades(),
                LevelIDsBeatenThisPlaythrough = new List<string>(),
                LevelPrefabsBeatenThisPlaythrough = new List<string>(),
                LevelIDsExcludedThisGame = new List<string>()
            };
            gameData.SetDirty(true);
            gameData.PlayerArmorParts = new List<MechBodyPartType>();
            gameData.PlayerBodyPartDamages = new List<MechBodyPartDamage>();
            gameData.TransferredToEnemyType = EnemyType.None;
            gameData.AllyTransferredToEnemyType = EnemyType.None;
            gameData.NumConsciousnessTransfersLeft = 0;
            Data_Legacy = gameData; //GameDataManager.Instance.CallPrivateMethod<GameData>("createNewGameData");
        }
        /// <summary>
        /// Saves the data if IsDirty() returns true
        /// </summary>
        public void TrySaveLegacyData()
        {
            if (Data_Legacy.IsDirty())
            {
                Singleton<DataRepository>.Instance.Save(Data_Legacy, LegacyFileName, false, true);
            }
        }

        public override GameData GetLegacyGameData()
        {
            return Data_Legacy;
        }

        public override GameMode GetGamemode()
        {
            return (GameMode)29300;
        }

        public override List<LevelDescription> GetLevelDescriptions()
        {
            return LevelDescriptions;
        }

        public override void StartOvermode(Action onStartDone = null, bool spawnPlayer = false)
        {
            base.StartOvermode(delegate
            {
                ProcessEventAndReturn<Coroutine>(EventNames.SpawnLevel, new object[] { false, OVERMODELEVELTAG, null });
            }, spawnPlayer);
        }

        public override T ProcessEventAndReturn<T>(in EventNames eventName, in object[] args)
        {
            if (eventName == EventNames.SpawnLevel)
            {
                bool isAsync = (bool)args[0];
                string overrideLevelID = (string)args[1];
                Action completeCallback = args[2] as Action;
                return StartCoroutine(Coroutines.SpawnCurrentLevel_EndlessOverMode(isAsync, overrideLevelID, completeCallback)) as T;
            }
            return null;
        }
    }
}
