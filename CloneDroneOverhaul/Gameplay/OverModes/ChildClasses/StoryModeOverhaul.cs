using CloneDroneOverhaul.Controllers;
using CloneDroneOverhaul.Modules;
using System;
using System.Collections.Generic;

namespace CloneDroneOverhaul.Gameplay.OverModes
{
    public class StoryModeOverhaul : OverModeBase
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
        public const string LegacyFileName = "StoryOvermodeData";
        public const string Chapter4Level1FilePath = "Levels\\Overmodes\\Story\\Chapter4\\OvermodeChapter4.json";

        /// <summary>
        /// Vanilla like save data
        /// </summary>
        public GameData Data_Legacy;

        public static StoryModeOverhaul Instance;
        public override void Initialize()
        {
            Instance = this;
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
        public override GameData GetLegacyGameData()
        {
            return Data_Legacy;
        }

        public override GameMode GetGamemode()
        {
            return (GameMode)29301;
        }
        public override List<LevelDescription> GetLevelDescriptions()
        {
            return LevelDescriptions;
        }

        public override T ProcessEventAndReturn<T>(EventNames eventName, object[] args)
        {
            if (eventName == EventNames.SpawnLevel)
            {
                BaseUtils.SpawnLevelFromPath(OverhaulDescription.GetModFolder() + Chapter4Level1FilePath, true, delegate
                {
                    GameCameraController.SetRendererEnabled(true, true);
                    AdventureCheckPoint checkpoint = UnityEngine.Object.FindObjectOfType<AdventureCheckPoint>();
                    checkpoint.OnPlayerAboutToSpawn();
                    GameFlowManager.Instance.SpawnPlayer(checkpoint.transform, true, true, null);
                    checkpoint.OnPlayerSpawned();
                    Data_Legacy.CurentLevelID = LevelDescriptions[0].GeneratedUniqueID;
                });
            }
            return null;
        }

        public override void StartOvermode(Action onStartDone = null, bool spawnPlayer = false)
        {
            GameCameraController.SetRendererEnabled(false, true);
            base.StartOvermode(delegate
            {
                ArenaManager.SetArenaVisible(false);
                ProcessEventAndReturn<Test123>(EventNames.SpawnLevel, null);
            }, false);
        }
    }
}
