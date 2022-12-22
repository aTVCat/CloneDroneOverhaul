using CloneDroneOverhaul.V3Tests.Base;
using UnityEngine;
using UnityEngine.UI;

namespace CloneDroneOverhaul.V3Tests.Gameplay
{
    /// <summary>
    /// Better camera management
    /// </summary>
    public class AdvancedCameraController : V3_ModControllerBase
    {
        public AdvancedCameraType GlobalCameraState { get; private set; }
        public AdvancedCameraType PlayerCameraState { get; private set; }

        private Character _playerInstance;
        private Character _player
        {
            get
            {
                return _playerInstance;
            }
            set
            {
                _playerInstance = value;

                if(value == null)
                {
                    _playerCameraInfo = null;
                    return;
                }
                PatchCamera(value, GlobalCameraState);
            }
        }
        private RobotAdvancedCameraController _playerCameraInfo;

        public void SwitchCameraPosition()
        {
            if (!OverhaulDescription.TEST_FEATURES_ENABLED)
            {
                return;
            }
            GlobalCameraState++;
            if((int)GlobalCameraState > 3)
            {
                GlobalCameraState = 0;
            }
            PatchCamera(_player, GlobalCameraState);
        }

        public bool AllowCameraAnimatorAndMoverEnabled()
        {
            return PlayerCameraState == AdvancedCameraType.ThirdPerson;
        }

        public void PatchCamera(in Character characterIn, in AdvancedCameraType cameraPos)
        {
            PlayerCameraState = cameraPos;
            Character character = null;
            if(characterIn != null)
            {
                character = characterIn;
            }
            else
            {
                character = _player;

                if (!character)
                {
                    return;
                }
            }

            DelegateScheduler.Instance.Schedule(delegate
            {
                RobotAdvancedCameraController info = character.GetComponent<RobotAdvancedCameraController>();
                if (!info)
                {
                    _playerCameraInfo = character.gameObject.AddComponent<RobotAdvancedCameraController>().Initialize(this, character);
                }
                _playerCameraInfo.PatchCharacter(character.GetCameraMover().gameObject, GlobalCameraState);
            }, 0.1f);
        }

        public static void TryChangeCameraPosition()
        {
            AdvancedCameraController.GetInstance<AdvancedCameraController>().SwitchCameraPosition();
        }

        public override void OnEvent(in string eventName, in object[] args)
        {
            if(eventName == "onPlayerSet")
            {
                CloneDroneOverhaul.Utilities.RobotShortInformation info = args[0] as CloneDroneOverhaul.Utilities.RobotShortInformation;
                _player = info.Instance;
            }
        }
    }
}