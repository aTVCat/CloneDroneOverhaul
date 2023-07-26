using UnityEngine;

namespace CDOverhaul.Gameplay.Combat
{
    /// <summary>
    /// Allows managing animations, that are not in game
    /// </summary>
    public class FirstPersonMoverModdedAnimationsController : OverhaulGameplayController
    {
        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover, bool hasInitializedModel)
        {
            if (!OverhaulMod.HasBootProcessEnded || OverhaulVersion.IsVersion2 || !hasInitializedModel || firstPersonMover == null)
                return;

            CharacterModel model = firstPersonMover.GetCharacterModel();
            if (model == null)
                return;

            Animator upperAnimator = model.UpperAnimator;
            Animation animationComponent = null;
            Animation animationComponent2 = null;
            bool hasUpperAnimator = upperAnimator != null;
            if (hasUpperAnimator)
            {
                animationComponent = upperAnimator.gameObject.GetComponent<Animation>();
                if (!animationComponent)
                    animationComponent = upperAnimator.gameObject.AddComponent<Animation>();

                AnimationClip clip = OverhaulAssetsController.GetAsset<AnimationClip>("TestAnim", OverhaulAssetPart.Combat_Update);
                animationComponent.AddClip(clip, clip.name);
                AnimationClip clip2 = OverhaulAssetsController.GetAsset<AnimationClip>("WeaponUse_PrepareBoomerang", OverhaulAssetPart.Combat_Update);
                animationComponent.AddClip(clip2, clip2.name);
                AnimationClip clip3 = OverhaulAssetsController.GetAsset<AnimationClip>("WeaponUse_ThrowBoomerang", OverhaulAssetPart.Combat_Update);
                animationComponent.AddClip(clip3, clip3.name);
                AnimationClip clip4 = OverhaulAssetsController.GetAsset<AnimationClip>("WeaponUse_PickUpBoomerang", OverhaulAssetPart.Combat_Update);
                animationComponent.AddClip(clip4, clip4.name);
                AnimationClip clip5 = OverhaulAssetsController.GetAsset<AnimationClip>("Combo_DoubleStrike", OverhaulAssetPart.Combat_Update);
                animationComponent.AddClip(clip5, clip5.name);
            }

            Animator legsAnimator = model.LegsAnimator;
            bool hasLegsAnimator = legsAnimator != null;
            if (hasLegsAnimator)
            {
                animationComponent2 = upperAnimator.gameObject.GetComponent<Animation>();
                if (!animationComponent2)
                    animationComponent2 = upperAnimator.gameObject.AddComponent<Animation>();
            }

            firstPersonMover.gameObject.AddComponent<FirstPersonMoverModdedAnimationsExpansion>().SetAnimationReferences(animationComponent, animationComponent2);
        }
    }
}
