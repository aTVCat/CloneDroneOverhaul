using CloneDroneOverhaul.Utilities;
using CloneDroneOverhaul.V3Tests.Gameplay;
using UnityEngine;

namespace CloneDroneOverhaul.V3Tests.HUD
{
    public class UIControlInstructions : V3_ModHUDBase
    {
        private ControlInstructionEntry _raptorDismount;
        private ControlInstructionEntry _raptorKick;
        private ControlInstructionEntry _flameBreath;
        private ControlInstructionEntry _laser;
        private ControlInstructionEntry _mindTransfers;

        private float _timeToNextCheck;

        private void Start()
        {
            _raptorDismount = ControlInstructionEntry.AddComponent(ModdedObject.GetObjectFromList<ModdedObject>(0));
            _raptorDismount.SetUseTrigger(KeyCode.E);
            _flameBreath = ControlInstructionEntry.AddComponent(ModdedObject.GetObjectFromList<ModdedObject>(1));
            _flameBreath.SetUseTrigger(KeyCode.F);
            _raptorKick = ControlInstructionEntry.AddComponent(ModdedObject.GetObjectFromList<ModdedObject>(2));
            _raptorKick.SetUseTrigger(KeyCode.None, true, 1);
            _laser = ControlInstructionEntry.AddComponent(ModdedObject.GetObjectFromList<ModdedObject>(3));
            _laser.SetUseTrigger(KeyCode.F);
            _mindTransfers = ControlInstructionEntry.AddComponent(ModdedObject.GetObjectFromList<ModdedObject>(4));
            _mindTransfers.SetUseTrigger(KeyCode.Tab);
        }

        public void RefreshControls()
        {
            RobotShortInformation info = GameStatisticsController.GameStatistics.PlayerRobotInformation;
            if (info != null && !info.IsNull && !SettingsManager.Instance.ShouldHideGameUI())
            {
                Character character = info.Instance;
                if (info.IsFPM)
                {
                    bool isInPhotoMode = PhotoManager.Instance.IsInPhotoMode();
                    FirstPersonMover mover = character as FirstPersonMover;
                    _raptorDismount.SetVisible(mover.IsRidingOtherCharacter());
                    _raptorKick.SetVisible(mover.CharacterType == EnemyType.FireRaptor || mover.CharacterType == EnemyType.LaserRaptor);
                    _flameBreath.SetVisible(mover.IsRidingOtherCharacter() || mover.HasAbility(UpgradeType.FlameBreath));
                    _laser.SetVisible(mover.HasAbility(UpgradeType.EnergyLaser));
                    _laser.SetIsAvailable(!mover.IsRidingOtherCharacter());
                    _mindTransfers.SetVisible(GameModeManager.ConsciousnessTransferToKillerEnabled());
                }
            }
        }

        public void HideAll()
        {
            _raptorDismount.SetVisible(false);
            _flameBreath.SetVisible(false);
            _raptorKick.SetVisible(false);
            _laser.SetVisible(false);
            _mindTransfers.SetVisible(false);
        }

        private void Update()
        {
            if (SettingsManager.Instance.ShouldHideGameUI())
            {
                HideAll();
                return;
            }

            bool isInPhotoMode = PhotoManager.Instance.IsInPhotoMode() || CutSceneManager.Instance.IsInCutscene();
            _timeToNextCheck -= Time.deltaTime;
            if (_timeToNextCheck <= 0f && !isInPhotoMode)
            {
                _timeToNextCheck = 0.5f;
                RefreshControls();
            }
            if (isInPhotoMode)
            {
                HideAll();
            }
        }
    }
}
