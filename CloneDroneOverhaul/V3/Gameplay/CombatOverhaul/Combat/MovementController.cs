using System;
using System.Collections.Generic;
using UnityEngine;
using ModLibrary;

namespace CloneDroneOverhaul.V3.Gameplay
{
    public class MovementController : FirstPersonMoverAddititonBase
    {
        bool _hasInitialized;
        bool _error;

        Animator _legsAnimator;

        float _speedMultipler;
        float _sprint;

        bool _hasSprint;
        float _restTime = 0f;

        protected override void OnReceiveCommand(in FPMoveCommand command)
        {
            if (!_hasInitialized)
            {
                _hasInitialized = true;
                CharacterModel model = Owner.GetCharacterModel();
                if (!model)
                {
                    _error = true;
                    return;
                }
                _legsAnimator = model.LegsAnimator;
                if (!_legsAnimator)
                {
                    _error = true;
                }
            }
        }

        public override void OnUpgradesRefreshed(in UpgradeCollection collection)
        {
            _hasSprint = collection.GetUpgradeLevel(UpgradeType.Dash) == 0;
        }

        void Update()
        {
            if(!_hasInitialized || _error || Owner == null)
            {
                return;
            }

            bool f = Owner.GetPrivateField<bool>("_isMovingForward");
            bool b = Owner.GetPrivateField<bool>("_isMovingBack");
            bool r = Owner.GetPrivateField<bool>("_isMovingRight");
            bool l = Owner.GetPrivateField<bool>("_isMovingLeft");

            bool oneLeg = Owner.HasLegDamage();

            /*
            bool fallen = Owner.HasFallenDown() || Owner.IsDownFromKickByOtherCharacter();
            bool j = Owner.IsJumping();
            bool k = Owner.IsKicking();
            bool gk = Owner.IsGettingUpFromKick();
            bool d = Owner.IsJetpackEngaged();*/

            bool isMoving = f || b || r || l; /*|| fallen || j || k || gk || d;*/
            bool additCondition = Owner.IsMainPlayer() && Owner.IsPlayerInputEnabled() && (!Owner.HasFallenDown() && !Owner.IsGettingUpFromKick() && !Owner.IsKicking());

            EnergySource source = Owner.GetEnergySource();
            bool canSprint = source != null && source.CanConsume(0.1f) && _hasSprint && additCondition && !oneLeg;

            float baseMultipler = oneLeg ? 0.5f : 1f;
            float accelerationMultipler = oneLeg ? 0.5f : 4f;

            if (_restTime > 0)
            {
                _restTime -= Time.deltaTime;
                canSprint = false;
            }

            if (!isMoving)
            {
                _speedMultipler = Mathf.Clamp(_speedMultipler - Time.deltaTime * 6f * Time.timeScale, 0f, baseMultipler + _sprint);
            }
            else
            {
                _speedMultipler = Mathf.Clamp(_speedMultipler + Time.deltaTime * accelerationMultipler * Time.timeScale, 0f, baseMultipler + _sprint);
            }

            if (canSprint && Owner.IsMainPlayer() && Input.GetKey(KeyCode.LeftShift))
            {
                _sprint = Mathf.Clamp(_sprint + Time.deltaTime * 0.5f * Time.timeScale, 0f, 0.5f);
                source.Consume(0.35f * Time.deltaTime * Time.timeScale);
                if (!source.CanConsume(0.1f))
                {
                    _restTime = 5f;
                }
            }
            else
            {
                _sprint = Mathf.Clamp(_sprint - Time.deltaTime * 6f, 0f, 0.5f);
            }
            if(_legsAnimator != null) _legsAnimator.speed = 1f + (_sprint * 0.75f);

            Owner.SetPrivateField<float>("_moveSpeedMultiplier", _speedMultipler);
        }
    }
}
