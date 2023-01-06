using CloneDroneOverhaul.V3Tests.Base;

namespace CloneDroneOverhaul.V3Tests.Gameplay
{
    /// <summary>
    /// Better camera management
    /// </summary>
    public class AdvancedCameraController : V3_ModControllerBase
    {
        public EAdvancedCameraType CurrentCameraMode { get; private set; }

        private Character _playerInstance;
        private Character _player
        {
            get => _playerInstance;
            set
            {
                _playerInstance = value;

                if (_playerInstance == null)
                {
                    _playerCameraInfo = null;
                    return;
                }

                PatchCamera(_playerInstance, CurrentCameraMode);
            }
        }
        private RobotAdvancedCameraController _playerCameraInfo;

        private void Start()
        {
            GlobalEventManager.Instance.AddEventListener("UpgradeUIClosed", delegate 
            {
                if(GameModeManager.IsMultiplayer())
                DelegateScheduler.Instance.Schedule(delegate
                {
                    _playerInstance = CharacterTracker.Instance.GetPlayerRobot();
                    PatchCamera(_player, CurrentCameraMode);
                }, 1);
            });
        }

        public void SwitchMode()
        {
            if (!OverhaulDescription.TEST_FEATURES_ENABLED)
            {
                return;
            }
            CurrentCameraMode++;
            if ((int)CurrentCameraMode > 2)
            {
                CurrentCameraMode = 0;
            }
            PatchCamera(_player, CurrentCameraMode);
        }

        public bool AllowCameraAnimatorAndMoverEnabled()
        {
            return CurrentCameraMode == EAdvancedCameraType.ThirdPerson;
        }

        public void PatchCamera(in Character characterIn, in EAdvancedCameraType cameraPos)
        {
            if (!OverhaulDescription.TEST_FEATURES_ENABLED)
            {
                return;
            }

            Character character = characterIn != null ? characterIn : _player;

            DelegateScheduler.Instance.Schedule(delegate
            {

                if (character == null)
                {
                    return;
                }
                RobotAdvancedCameraController info = character.GetComponent<RobotAdvancedCameraController>();
                if (info == null)
                {
                    _playerCameraInfo = character.gameObject.AddComponent<RobotAdvancedCameraController>().Initialize(this, character);
                }
                _playerCameraInfo.PatchCharacter(character.GetCameraMover().gameObject, CurrentCameraMode);


            }, 0.1f);
        }

        public override void OnEvent(in string eventName, in object[] args)
        {
            if (eventName == "onPlayerSet")
            {
                CloneDroneOverhaul.Utilities.RobotShortInformation info = args[0] as CloneDroneOverhaul.Utilities.RobotShortInformation;
                _player = info.Instance;
            }

            if(_playerCameraInfo == null)
            {
                return;
            }

            if (eventName == "cinematicCamera.On")
            {
                _playerCameraInfo.ShowModels(null, true);
            }
            if (eventName == "cinematicCamera.Off")
            {
                _playerCameraInfo.HideModels(null);
            }
        }

        public override void OnSettingRefreshed(in string settingName, in object value)
        {
            if(settingName == "Misc.Experience.Camera mode")
            {
                CurrentCameraMode = (EAdvancedCameraType)(int)value;
                if(_player != null) PatchCamera(_player, CurrentCameraMode);
            }
        }

        public static void TryChangeCameraPosition()
        {
            AdvancedCameraController.GetInstance<AdvancedCameraController>().SwitchMode();
        }

    }
}