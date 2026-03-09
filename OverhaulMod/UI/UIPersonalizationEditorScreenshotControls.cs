using OverhaulMod.Content.Personalization;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPersonalizationEditorScreenshotControls : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(OnTakeScreenshotButtonClicked))]
        [UIElement("TakeScreenshotButton")]
        private readonly Button _takeScreenshotButton;

        [UIElementAction(nameof(OnResetCameraButtonClicked))]
        [UIElement("ResetCameraButton")]
        private readonly Button _resetCameraButton;

        [UIElementAction(nameof(OnScreenshotWeaponSkinsButtonClicked))]
        [UIElement("ScreenshotWeaponSkinsButton")]
        private readonly Button _screenshotWeaponSkinsButton;

        [UIElementAction(nameof(OnScreenshotAccessoriesButtonClicked))]
        [UIElement("ScreenshotAccessoriesButton")]
        private readonly Button _screenshotAccessoriesButton;

        [UIElementAction(nameof(OnScreenshotPetsButtonClicked))]
        [UIElement("ScreenshotPetsButton")]
        private readonly Button _screenshotPetsButton;

        [UIElement("ScreenshotOnlyNewToggle")]
        private readonly Toggle _screenshotOnlyNewToggle;

        [UIElementAction(nameof(OnSaveAngleButtonClicked))]
        [UIElement("SaveAngleButton")]
        private readonly Button _saveAngleButton;

        [UIElement("ModeratorControls", false)]
        private readonly GameObject _moderatorControls;

        [UIElement("ProgressLabel")]
        private readonly Text _progressLabel;

        public override bool closeOnEscapeButtonPress => false;

        protected override void OnInitialized()
        {
            _moderatorControls.SetActive(PersonalizationEditorManager.Instance.canVerifyItems);
            _progressLabel.enabled = false;
        }

        private void onTakenScreenshot(int itemIndex, int itemCount)
        {
            if(itemIndex == itemCount)
            {
                _progressLabel.enabled = false;
                _screenshotWeaponSkinsButton.interactable = true;
                return;
            }
            _progressLabel.text = $"{itemIndex} of {itemCount} processed...";
        }

        public void OnScreenshotWeaponSkinsButtonClicked()
        {
            if (PersonalizationEditorScreenshotManager.Instance.IsTakingScreenshots()) return;

            PersonalizationEditorScreenshotManager.Instance.TakeScreenshotsOfWeaponSkins(_screenshotOnlyNewToggle.isOn, onTakenScreenshot);
            _progressLabel.enabled = true;
            _progressLabel.text = "...";

            _screenshotWeaponSkinsButton.interactable = false;
        }

        public void OnScreenshotAccessoriesButtonClicked()
        {
            if (PersonalizationEditorScreenshotManager.Instance.IsTakingScreenshots()) return;
        }

        public void OnScreenshotPetsButtonClicked()
        {
            if (PersonalizationEditorScreenshotManager.Instance.IsTakingScreenshots()) return;
        }

        public void OnSaveAngleButtonClicked()
        {
            PersonalizationEditorScreenshotManager.Instance.SaveAngle();
        }

        public void OnTakeScreenshotButtonClicked()
        {
            PersonalizationEditorScreenshotManager.Instance.TakeAndSaveScreenshot();
        }

        public void OnResetCameraButtonClicked()
        {
            PersonalizationEditorScreenshotManager.Instance.AdjustCameraPositionForCurrentItem();
        }
    }
}