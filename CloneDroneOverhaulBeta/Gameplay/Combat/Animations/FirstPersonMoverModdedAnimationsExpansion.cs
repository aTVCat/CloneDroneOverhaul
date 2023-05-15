using UnityEngine;

namespace CDOverhaul.Gameplay.Combat
{
    public class FirstPersonMoverModdedAnimationsExpansion : CombatOverhaulMechanic
    {
        private Animation m_UpperAnimation;
        private bool m_HasUpperAnimator;
        private Animation m_LowerAnimation;
        private bool m_HasLowerAnimator;
        public void SetAnimationReferences(Animation upper, Animation lower)
        {
            m_UpperAnimation = upper;
            m_LowerAnimation = lower;
        }

        public bool IsPlayingCustomAnimation => IsPlayingCustomLowerAnimation || IsPlayingCustomUpperAnimation;
        public bool IsPlayingCustomUpperAnimation => !IsDisposedOrDestroyed() && (ForceSetIsPlayingUpperAnimation || (m_HasUpperAnimator && m_UpperAnimation != null && m_UpperAnimation.isPlaying));
        public bool IsPlayingCustomLowerAnimation => !IsDisposedOrDestroyed() && (ForceSetIsPlayingLowerAnimation || (m_HasLowerAnimator && m_LowerAnimation != null && m_LowerAnimation.isPlaying));

        public bool ForceSetIsPlayingUpperAnimation;
        public bool ForceSetIsPlayingLowerAnimation;

        public override void Start()
        {
            base.Start();
            m_HasUpperAnimator = HasAnimator(FirstPersonMoverAnimatorType.Upper);
            m_HasLowerAnimator = HasAnimator(FirstPersonMoverAnimatorType.Legs);
        }

        protected override void OnDisposed()
        {
            m_UpperAnimation = null;
            m_LowerAnimation = null;
        }

        public override void OnEvent(SendFallingEvent sendFallingEvent)
        {
            if (!Owner.IsDetached() && Owner.entity.IsOwner && Owner.HasLocalControl())
            {
                StopPlayingCustomAnimations();
            }
        }

        public void PlayCustomAnimaton(string animationName, bool dontPlayIfFell = true)
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            Owner.SetActiveEmoteIndex(-1);
            if (dontPlayIfFell && Owner.GetState().IsFalling)
            {
                return;
            }

            if (m_HasUpperAnimator && m_UpperAnimation != null)
            {
                AnimationClip clip = m_UpperAnimation.GetClip(animationName);
                if (clip != null)
                {
                    m_UpperAnimation.CrossFade(animationName, 0.35f);
                    m_UpperAnimation.clip = clip;
                    DelegateScheduler.Instance.Schedule(delegate
                    {
                        if (!Owner.GetState().IsFalling) CharacterModel.RenderUpperAnimationFrame("Idle_Sword", 1000f);
                    }, Time.deltaTime);
                }
            }
            if (m_HasLowerAnimator && m_LowerAnimation != null)
            {
                AnimationClip clip = m_LowerAnimation.GetClip(animationName);
                if (clip != null)
                {
                    m_LowerAnimation.CrossFade(animationName, 0.35f);
                    m_LowerAnimation.clip = clip;
                }
            }
        }

        public void StopPlayingCustomAnimations()
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            if (m_HasUpperAnimator && m_UpperAnimation != null)
            {
                m_UpperAnimation.Stop();
            }
            if (m_HasLowerAnimator && m_LowerAnimation != null)
            {
                m_LowerAnimation.Stop();
            }
        }

        public string GetPlayingCustomAnimationName(FirstPersonMoverAnimatorType animationType)
        {
            if (!HasAnimator(animationType))
            {
                return string.Empty;
            }

            switch (animationType)
            {
                case FirstPersonMoverAnimatorType.Upper:
                    return m_UpperAnimation != null ? m_UpperAnimation.clip.name : string.Empty;
                    break;
                case FirstPersonMoverAnimatorType.Legs:
                    return m_LowerAnimation != null ? m_LowerAnimation.clip.name : string.Empty;
                    break;
            }
            return string.Empty;
        }

        private void LateUpdate()
        {
            if (IsDisposedOrDestroyed() || CharacterModel == null || !Owner.IsAlive())
            {
                return;
            }

            if (m_HasUpperAnimator) CharacterModel.UpperAnimator.enabled = !IsPlayingCustomUpperAnimation && !CharacterModel.IsManualUpperAnimationEnabled();
            if (m_HasLowerAnimator) CharacterModel.LegsAnimator.enabled = !IsPlayingCustomLowerAnimation && !CharacterModel.IsManualLegsAnimationEnabled();
        }
    }
}