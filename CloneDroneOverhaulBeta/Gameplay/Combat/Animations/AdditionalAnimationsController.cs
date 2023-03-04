using System;
using UnityEngine;

namespace CDOverhaul.Gameplay.Combat
{
    /// <summary>
    /// Allows managing animations, that are not in game
    /// </summary>
    public class AdditionalAnimationsController : OverhaulGameplayController
    {
        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
        }

        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover, bool hasInitializedModel)
        {
            if (!hasInitializedModel)
            {
                return;
            }
            CharacterModel model = firstPersonMover.GetCharacterModel();

            Animator upperAnimator = model.UpperAnimator;
            bool hasUpperAnimator = upperAnimator != null;
            if (hasUpperAnimator)
            {
                Animation animationComponent = upperAnimator.gameObject.AddComponent<Animation>();
                AnimationClip clip = AssetController.GetAsset<AnimationClip>("TestAnim", OverhaulAssetsPart.Combat_Update);
                animationComponent.AddClip(clip, clip.name);
            }

            Animator legsAnimator = model.LegsAnimator;
            bool hasLegsAnimator = legsAnimator != null;
            if (hasLegsAnimator)
            {

            }
        }

        public override string[] Commands()
        {
            throw new NotImplementedException();
        }
        public override string OnCommandRan(string[] command)
        {
            throw new NotImplementedException();
        }
    }
}
