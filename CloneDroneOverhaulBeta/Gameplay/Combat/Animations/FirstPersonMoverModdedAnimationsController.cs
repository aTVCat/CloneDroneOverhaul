using System;
using UnityEngine;

namespace CDOverhaul.Gameplay.Combat
{
    /// <summary>
    /// Allows managing animations, that are not in game
    /// </summary>
    public class FirstPersonMoverModdedAnimationsController : OverhaulGameplayController
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
            if (OverhaulVersion.IsUpdate2Hotfix || !hasInitializedModel || firstPersonMover == null)
            {
                return;
            }

            CharacterModel model = firstPersonMover.GetCharacterModel();
            if (model == null)
            {
                return;
            }

            Animator upperAnimator = model.UpperAnimator;
            Animation animationComponent = null;
            Animation animationComponent2 = null;
            bool hasUpperAnimator = upperAnimator != null;
            if (hasUpperAnimator)
            {
                animationComponent = upperAnimator.gameObject.GetComponent<Animation>();
                if (!animationComponent)
                {
                    animationComponent = upperAnimator.gameObject.AddComponent<Animation>();
                }
                AnimationClip clip = AssetsController.GetAsset<AnimationClip>("TestAnim", OverhaulAssetsPart.Combat_Update);
                animationComponent.AddClip(clip, clip.name);
                AnimationClip clip2 = AssetsController.GetAsset<AnimationClip>("WeaponUse_PrepareBoomerang", OverhaulAssetsPart.Combat_Update);
                animationComponent.AddClip(clip2, clip2.name);
                AnimationClip clip3 = AssetsController.GetAsset<AnimationClip>("WeaponUse_ThrowBoomerang", OverhaulAssetsPart.Combat_Update);
                animationComponent.AddClip(clip3, clip3.name);
                AnimationClip clip4 = AssetsController.GetAsset<AnimationClip>("WeaponUse_PickUpBoomerang", OverhaulAssetsPart.Combat_Update);
                animationComponent.AddClip(clip4, clip4.name);
                AnimationClip clip5 = AssetsController.GetAsset<AnimationClip>("Combo_DoubleStrike", OverhaulAssetsPart.Combat_Update);
                animationComponent.AddClip(clip5, clip5.name);
            }

            Animator legsAnimator = model.LegsAnimator;
            bool hasLegsAnimator = legsAnimator != null;
            if (hasLegsAnimator)
            {
                animationComponent2 = upperAnimator.gameObject.GetComponent<Animation>();
                if (!animationComponent2)
                {
                    animationComponent2 = upperAnimator.gameObject.AddComponent<Animation>();
                }
            }

            firstPersonMover.gameObject.AddComponent<FirstPersonMoverModdedAnimationsExpansion>().SetAnimationReferences(animationComponent, animationComponent2);
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
