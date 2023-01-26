using CDOverhaul.Gameplay;

namespace CDOverhaul.Shared
{
    public class CustomRobotAnimationFPMExtention : FirstPersonMoverExtention
    {
        private bool _isPlayingCustomAnimation;
        public bool IsPlayingCustomAnimation
        {
            get
            {
                if (ForcePlay)
                {
                    return true;
                }
                return ManualAnimationEnabled || _isPlayingCustomAnimation;
            }
            set
            {
                _isPlayingCustomAnimation = value;
            }
        }
        public bool ManualAnimationEnabled { get; set; }
        public bool ForcePlay { get; set; }

        public CharacterModel OwnerModel { get; set; }
        public CustomRobotAnimationPlayInfo CurrentAnimation { get; set; }

        protected override void Initialize(FirstPersonMover owner)
        {
            ManualAnimationEnabled = false;
            IsPlayingCustomAnimation = false;
            OwnerModel = owner.GetCharacterModel();
            CurrentAnimation = new CustomRobotAnimationPlayInfo();
        }

        public void PlayAnimation(in string name)
        {
            if (IsPlayingCustomAnimation || OwnerModel == null)
            {
                return;
            }
        }

        private void FixedUpdate()
        {
            if (OwnerModel != null)
            {
                OwnerModel.UpperAnimator.enabled = !IsPlayingCustomAnimation;
            }
        }
    }
}
