using ModLibrary;
using System;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class MainGameplayController : ModController
    {
        #region Events
        public const string GamemodeChangedEventString = "GamemodeChanged";
        public const string MainCameraSwitchedEventString = "CameraMainSwitched";
        public const string CurrentCameraSwitchedEventString = "CameraCurrentSwitched";

        public const string FirstPersonMoverSpawnedEventString = "FPMSpawned";
        public const string FirstPersonMoverSpawned_DelayEventString = "FPMSpawned_Delay";

        public const string PlayerSetAsCharacter = "PlayerSet_Base";
        public const string PlayerSetAsFirstPersonMover = "PlayerSet_FirstPersonMover";
        #endregion

        /// <summary>
        /// The start index of mod gamemodes
        /// </summary>
        public const int GamemodeStartIndex = 2000;

        public static MainGameplayController Instance
        {
            get;
            private set;
        }

        public GameFlowManager GameFlowManager
        {
            get;
            private set;
        }

        public WeaponSkinsController WeaponSkins
        {
            get;
            private set;
        }

        public RobotsController Robots
        {
            get;
            private set;
        }

        public LevelController Levels
        {
            get;
            private set;
        }

        public GamemodeSubstatesController GamemodeSubstates { get; private set; }

        private GameMode _gamemodeLastFrame;
        private Camera _cameraLastFrame;
        private Camera _cameraCurrentLastFrame;

        public override void Initialize()
        {
            Instance = this;

            GameFlowManager = GameFlowManager.Instance;
            WeaponSkins = ModControllerManager.InitializeController<WeaponSkinsController>();
            Robots = ModControllerManager.InitializeController<RobotsController>();
            GamemodeSubstates = ModControllerManager.InitializeController<GamemodeSubstatesController>();
            Levels = ModControllerManager.InitializeController<LevelController>();
            _ = ModControllerManager.InitializeController<WeaponSkinsControllerV2>();

            DelegateScheduler.Instance.Schedule(sendGamemodeWasUpdateEvent, 0.1f);

            HasAddedEventListeners = true;
            HasInitialized = true;
        }

        /// <summary>
        /// Set gamemode, spawn level and player
        /// </summary>
        /// <param name="gamemode"></param>
        /// <param name="levelData"></param>
        /// <param name="spawnLevel"></param>
        /// <param name="spawnPlayer"></param>
        public void StartGamemode(GameMode gamemode, LevelEditorLevelData levelData, Action onPlayerSpawned = null, bool spawnLevel = true, bool spawnPlayer = true)
        {
            SingleplayerServerStarter.Instance.StartServerThenCall(delegate
            {
                SetGamemode(gamemode);

                GameUIRoot.Instance.TitleScreenUI.Hide();
                GameUIRoot.Instance.SetPlayerHUDVisible(spawnPlayer);

                if (spawnLevel)
                {
                    LevelManager.Instance.CleanUpLevelThisFrame();
                }

                if (spawnLevel)
                {
                    _ = Levels.SpawnLevel(levelData, "Level" + UnityEngine.Random.Range(0, 100).ToString(), true, delegate { if (spawnPlayer) { SpawnPlayer(); if (onPlayerSpawned != null) { onPlayerSpawned(); } } });
                }
            });
        }

        /// <summary>
        /// Set gamemode specified in <paramref name="mode"/>
        /// </summary>
        /// <param name="mode"></param>
        public void SetGamemode(in GameMode mode)
        {
            GameFlowManager.SetPrivateField<GameMode>("_gameMode", mode);
        }

        /// <summary>
        /// Spawn player if <see cref="AdventureCheckPoint"/> would be found
        /// </summary>
        public void SpawnPlayer()
        {
            AdventureCheckPoint point = FindObjectOfType<AdventureCheckPoint>();
            if (point != null)
            {
                _ = GameFlowManager.SpawnPlayer(point.transform, true, true, null);
            }
            //GameFlow.CallPrivateMethod("createPlayerAndSetLift");
        }

        /// <summary>
        /// Send <see cref="GamemodeChangedEventString"/> event
        /// </summary>
        private void sendGamemodeWasUpdateEvent()
        {
            OverhaulEventManager.DispatchEvent(GamemodeChangedEventString);
        }

        private void Update()
        {
            GameMode currentGamemode = GameFlowManager.GetCurrentGameMode();
            if (currentGamemode != _gamemodeLastFrame)
            {
                sendGamemodeWasUpdateEvent();
            }
            _gamemodeLastFrame = currentGamemode;

            Camera mainCamera = Camera.main;
            if (mainCamera != _cameraLastFrame)
            {
                OverhaulEventManager.DispatchEvent(MainCameraSwitchedEventString, mainCamera);
            }
            _cameraLastFrame = mainCamera;

            Camera currentCamera = Camera.current;
            if (currentCamera != _cameraCurrentLastFrame)
            {
                OverhaulEventManager.DispatchEvent(CurrentCameraSwitchedEventString, currentCamera);
            }
            _cameraCurrentLastFrame = currentCamera;
        }
    }
}