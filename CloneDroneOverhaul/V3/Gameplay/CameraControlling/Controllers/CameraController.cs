using CloneDroneOverhaul.V3.Base;

namespace CloneDroneOverhaul.V3.Gameplay
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

                PatchCamera(value, CurrentCameraMode);
            }
        }
        private RobotAdvancedCameraController _playerCameraInfo;

        void LateUpdate()
        {
            /*
            if (_playerCameraInfo == null && _playerInstance != null)
            {
                _player = _playerInstance;
            }
            else if (_playerCameraInfo != null && !_playerCameraInfo.FoundHead())
            {
                _playerCameraInfo.Initialize(this, _player);
            }
            else if (_playerCameraInfo != null && _playerCameraInfo.FoundHead())
            {
                _playerCameraInfo.TryHideModels();
            }*/
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

            if (character == null)
            {
                return;
            }
            RobotAdvancedCameraController info = character.GetComponent<RobotAdvancedCameraController>();
            if (info == null)
            {
                _playerCameraInfo = character.gameObject.AddComponent<RobotAdvancedCameraController>().Initialize(this, character);
            }
            else
            {
                return;
            }

            if (character.GetCameraMover() == null)
            {
                if (OverhaulGraphicsController.CachedMainCamera != null)
                {
                    _playerCameraInfo.PatchCharacter(OverhaulGraphicsController.CachedMainCamera.gameObject, CurrentCameraMode);
                }
                return;
            }
            _playerCameraInfo.PatchCharacter(character.GetCameraMover().gameObject, CurrentCameraMode);
        }

        public override void OnEvent(in string eventName, in object[] args)
        {
            if (eventName == "onPlayerSet")
            {
                V3.RobotShortInformation info = args[0] as V3.RobotShortInformation;
                DelegateScheduler.Instance.Schedule(delegate
                {
                    _player = info.Instance;
                }, 0.1f);
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
            }
        }
    }
}