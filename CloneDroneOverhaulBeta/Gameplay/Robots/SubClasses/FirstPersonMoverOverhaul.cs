using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class FirstPersonMoverOverhaul : FirstPersonMoverExtention
    {
        [OverhaulSetting("Robots.Events.Death Animation", true, false, "Robots won't instantly freeze, instead, they will slow down")]
        public static bool AllowDeathAnimation;

        public CharacterModel OwnerModel { get; set; }

        /// <summary>
        /// Check if legs and upper parts can be animated
        /// </summary>
        public bool HasAllAnimators => OwnerModel != null && OwnerModel.UpperAnimator != null && OwnerModel.LegsAnimator != null;

        /// <summary>
        /// Check if we are using any ability or the owner's size is like emperor one 
        /// </summary>
        public bool IsBigRobotOrUsingAnyAbility => (Owner != null && (Owner.CharacterType == EnemyType.EmperorCombat || Owner.CharacterType == EnemyType.EmperorNonCombat)) || Owner.IsUsingAnyAbility();

        private bool _hasAddedEventListeners;
        private bool _animatingDeath;

        private float _timeCharacterDied;
        private bool _diedWhileUsingAbilityOrAsBigRobot;

        protected override void Initialize(FirstPersonMover owner)
        {
            OwnerModel = owner.GetCharacterModel();
            _timeCharacterDied = -1;

            OverhaulEventManager.AddEventListener<Character>("CharacterKilled", onKill, true);
            _hasAddedEventListeners = true;
        }

        private void onKill(Character c)
        {
            if (c.GetInstanceID() == Owner.GetInstanceID())
            {
                if (!AllowDeathAnimation)
                {
                    SetAnimatorsSpeed(0f);
                    return;
                }
                _animatingDeath = true;
                _diedWhileUsingAbilityOrAsBigRobot = IsBigRobotOrUsingAnyAbility;
                _timeCharacterDied = Time.time;
                removeEventListeners();
            }
        }

        public void SetAnimatorsSpeed(in float speed)
        {
            if (!HasAllAnimators)
            {
                return;
            }
            OwnerModel.UpperAnimator.enabled = speed >= 0.1f;
            OwnerModel.LegsAnimator.enabled = speed >= 0.1f;
            OwnerModel.UpperAnimator.speed = speed;
            OwnerModel.LegsAnimator.speed = speed;
        }

        private void removeEventListeners()
        {
            if (!_hasAddedEventListeners)
            {
                return;
            }

            _hasAddedEventListeners = false;
            OverhaulEventManager.RemoveEventListener<Character>("CharacterKilled", onKill, true);
        }

        private void OnDestroy()
        {
            removeEventListeners();
        }

        private void FixedUpdate()
        {
            if (_timeCharacterDied != -1)
            {
                if (HasAllAnimators && Time.time > _timeCharacterDied + 2f)
                {
                    _timeCharacterDied = -1;
                    _animatingDeath = false;
                    SetAnimatorsSpeed(0f);
                }
            }

            if (_animatingDeath)
            {
                if (!HasAllAnimators)
                {
                    _animatingDeath = false;
                    return;
                }

                float speed = OwnerModel.UpperAnimator.speed;
                float d = 0f;
                if (_diedWhileUsingAbilityOrAsBigRobot)
                {
                    d = Time.fixedDeltaTime * 0.75f;
                }
                else
                {
                    if (speed < 0.3f)
                    {
                        d = Time.fixedDeltaTime * 0.1f;
                    }
                    else
                    {
                        d = Time.fixedDeltaTime * 0.65f;
                    }
                }
                float f = Mathf.Clamp(speed - d, 0f, 0.75f);

                SetAnimatorsSpeed(f);
            }
        }
    }
}