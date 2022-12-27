using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using CloneDroneOverhaul.V3Tests.HUD;

namespace CloneDroneOverhaul.V3Tests.Gameplay.Animations
{
    public class UIAnimationPanel : V3_ModHUDBase
    {
        public bool HasInitialized { get; private set; }

        private void Start()
        {
            if (!hasModdedObject())
            {
                throw new NullReferenceException("Overhaul animation panel: ModdedObject is null or empty");
            }
            HasInitialized = true;

            PopulateAnimationDropdown();

            AnimationsDropdown.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<int>(onSelectedAnimation));
        }


        /// <summary>
        /// Refresh all available animations
        /// </summary>
        public void PopulateAnimationDropdown()
        {
            if (!isreadyToWorkWithAnimations())
            {
                return;
            }

            AnimationsDropdown.ClearOptions();
            foreach (string str in GetAnimations())
            {
                AnimationsDropdown.options.Add(new Dropdown.OptionData(str));
            }
        }

        #region Callbacks

        private void onSelectedAnimation(int animationIndex)
        {
            if (!isreadyToWorkWithAnimations())
            {
                return;
            }

            OnSelectedAnimation(new SAnimation(AnimationsDropdown.options[animationIndex].text, AnimationType));
        }

        #endregion

        #region References

        public Func<List<string>> GetAnimations;
        public Action<SAnimation> OnSelectedAnimation;
        public EAnimationType AnimationType;
        public Dropdown AnimationsDropdown;

        #endregion

        #region Checks

        /// <summary>
        /// Check if modded object is null or empty
        /// </summary>
        /// <returns></returns>
        private bool hasModdedObject()
        {
            return ModdedObject != null && ModdedObject.objects.Count != 0;
        }

        private bool isreadyToWorkWithAnimations()
        {
            return hasModdedObject() && HasInitialized && AnimationsDropdown != null && OnSelectedAnimation != null;
        }

        #endregion
    }
}
