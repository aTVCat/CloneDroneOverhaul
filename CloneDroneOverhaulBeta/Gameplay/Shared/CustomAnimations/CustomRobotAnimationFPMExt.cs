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

        private bool _hasUpperAnimator;
        public bool HasUpperAnimator => OwnerModel != null && _hasUpperAnimator;

        public CharacterModel OwnerModel { get; set; }
        public CustomRobotAnimationPlayBehaviour CurrentAnimation { get; set; }

        protected override void Initialize(FirstPersonMover owner)
        {
            ManualAnimationEnabled = false;
            IsPlayingCustomAnimation = false;
            OwnerModel = owner.GetCharacterModel();
            CurrentAnimation = new CustomRobotAnimationPlayBehaviour();

            _hasUpperAnimator = OwnerModel.UpperAnimator != null;
        }

        /// <summary>
        /// Start playing an animation on robot
        /// </summary>
        /// <param name="name"></param>
        public void PlayAnimation(in string name)
        {
            if (OwnerModel == null)
            {
                return;
            }
            CustomAnimationsController c = OverhaulBase.Core.Shared.CustomAnimations;
            CurrentAnimation.StartPlaying(c.AnimationsContainer.GetAnimation(name));
            IsPlayingCustomAnimation = true;
        }

        private void Update()
        {
            if (HasUpperAnimator)
            {
                OwnerModel.UpperAnimator.enabled = !IsPlayingCustomAnimation;
                CurrentAnimation.OnUpdate(this);
            }
        }
    }
}
