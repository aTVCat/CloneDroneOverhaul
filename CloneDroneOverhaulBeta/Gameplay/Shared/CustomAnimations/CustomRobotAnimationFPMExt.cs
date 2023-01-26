using CDOverhaul.Gameplay;

namespace CDOverhaul.Shared
{
    public class CustomRobotAnimationFPMExtention : FirstPersonMoverExtention
    {
        public bool IsPlayingCustomAnimation { get; private set; }
        public CharacterModel OwnerModel { get; set; }
        public CustomRobotAnimationPlayInfo CurrentAnimation { get; set; }

        protected override void Initialize(FirstPersonMover owner)
        {
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
                OwnerModel.SetManualUpperAnimationEnabled(IsPlayingCustomAnimation);
            }
        }
    }
}
