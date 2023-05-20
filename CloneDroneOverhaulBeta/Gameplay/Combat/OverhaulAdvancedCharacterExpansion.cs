using UnityEngine;

namespace CDOverhaul.Gameplay.Combat
{
    public class OverhaulAdvancedCharacterExpansion : OverhaulCharacterExpansion
    {
        /// <summary>
        /// The speed of every player in the game
        /// </summary>
        public const float Default_Player_Speed = 12f;

        /// <summary>
        /// The speed of any robot in the game
        /// </summary>
        public const float Default_Enemy_Speed = 7f;

        public float GetDefaultSpeed()
        {
            return IsOwnerPlayer() ? Default_Player_Speed : Default_Enemy_Speed;
        }

        /// <summary>
        /// Get cached character model instance
        /// </summary>
        public CharacterModel CharacterModel { get; private set; }
        /// <summary>
        /// Check if we have cached character model
        /// </summary>
        public bool HasCharacterModel => CharacterModel != null;

        /// <summary>
        /// Get animator script instance if we have one
        /// </summary>
        /// <param name="animatorType"></param>
        /// <returns></returns>
        public Animator GetAnimator(FirstPersonMoverAnimatorType animatorType)
        {
            if (!IsDisposedOrDestroyed() && HasCharacterModel)
            {
                switch (animatorType)
                {
                    case FirstPersonMoverAnimatorType.Legs:
                        return CharacterModel.LegsAnimator;
                    case FirstPersonMoverAnimatorType.Upper:
                        return CharacterModel.UpperAnimator;
                }
            }
            return null;
        }
        /// <summary>
        /// Check if we have animator of specified type
        /// </summary>
        /// <param name="animatorType"></param>
        /// <returns></returns>
        public bool HasAnimator(FirstPersonMoverAnimatorType animatorType)
        {
            if (!IsDisposedOrDestroyed() && HasCharacterModel)
            {
                switch (animatorType)
                {
                    case FirstPersonMoverAnimatorType.Legs:
                        return CharacterModel.LegsAnimator != null;
                    case FirstPersonMoverAnimatorType.Upper:
                        return CharacterModel.UpperAnimator != null;
                }
            }
            return false;
        }
        /// <summary>
        /// Get speed of specific character model animator
        /// </summary>
        /// <param name="animatorType"></param>
        /// <returns></returns>
        public float GetAnimatorSpeed(FirstPersonMoverAnimatorType animatorType)
        {
            if (!IsDisposedOrDestroyed() && HasAnimator(animatorType))
            {
                switch (animatorType)
                {
                    case FirstPersonMoverAnimatorType.Legs:
                        return CharacterModel.LegsAnimator.speed;
                    case FirstPersonMoverAnimatorType.Upper:
                        return CharacterModel.UpperAnimator.speed;
                }
            }
            return -1f;
        }
        /// <summary>
        /// Set speed of specific character model animator
        /// </summary>
        /// <param name="animatorType"></param>
        /// <param name="value"></param>
        public void SetAnimatorSpeed(FirstPersonMoverAnimatorType animatorType, float value)
        {
            if (IsDisposedOrDestroyed() || !HasAnimator(animatorType))
            {
                return;
            }

            GetAnimator(animatorType).speed = value;
        }

        public override void Start()
        {
            if (!OverhaulMod.IsModInitialized)
            {
                DestroyBehaviour();
                return;
            }

            base.Start();
            getRequiredVariables();
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            CharacterModel = null;
        }

        private void getRequiredVariables()
        {
            CharacterModel = Owner.GetCharacterModel();
            if (CharacterModel == null)
            {
                base.enabled = false;
            }
        }

        public void SetRobotSpeed(float speed, bool updateAnimators, bool reset = false)
        {
            Owner.MaxSpeed = speed;
            if (updateAnimators) SetAnimatorSpeed(FirstPersonMoverAnimatorType.Legs, reset ? 1f : speed / GetDefaultSpeed());
        }
    }
}