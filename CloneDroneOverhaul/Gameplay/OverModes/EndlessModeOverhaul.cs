using UnityEngine;
using System.Collections.Generic;
using ModLibrary;

namespace CloneDroneOverhaul.Gameplay.OverModes
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
                GeneratedUniqueID = null
            }
        };
        public const string LegacyFileName = "OverModeEndlessData";

        /// <summary>
        /// Vanilla like save data
        /// </summary>
        public GameData Data_Legacy;

        public override void Initialize()
        {
            bool succesfullyLoaded = false;
            succesfullyLoaded = Singleton<DataRepository>.Instance.TryLoad<GameData>(LegacyFileName, out this.Data_Legacy, false);
            if (succesfullyLoaded)
            {
                this.Data_Legacy.RepairAnyMissingFields(true);
            }
            else
            {
                ResetSaveData();
            }
        }

        /// <summary>
        /// Spawn an enemy in level
        /// </summary>
        /// <param name="type"></param>
        /// <param name="position"></param>
        public static void SpawnEnemy(EnemyType type, Vector3 position)
        {
            EnemyFactory.Instance.SpawnEnemy(type, position, Vector3.zero);
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
            GameData gameData = new GameData();
            gameData.NumClones = Singleton<CloneManager>.Instance.GetNumStartingClones();
            gameData.NumLevelsWon = 0;
            gameData.AvailableSkillPoints = 0;
            gameData.PlayerUpgrades = Singleton<UpgradeManager>.Instance.CreateDefaultPlayerUpgrades();
            gameData.LevelIDsBeatenThisPlaythrough = new List<string>();
            gameData.LevelPrefabsBeatenThisPlaythrough = new List<string>();
            gameData.LevelIDsExcludedThisGame = new List<string>();
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

        public override GameMode GetOverModeGameMode()
        {
            return (GameMode)29300;
        }

        public override void StartGamemode()
        {
            base.StartGamemode();

        }
    }
}
