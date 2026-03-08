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

        public override bool closeOnEscapeButtonPress => false;

        protected override void OnInitialized()
        {
            _moderatorControls.SetActive(PersonalizationEditorManager.Instance.canVerifyItems);
        }

        public void OnScreenshotWeaponSkinsButtonClicked()
        {
            PersonalizationEditorScreenshotManager.Instance.TakeScreenshotsOfWeaponSkins(_screenshotOnlyNewToggle.isOn);
        }

        public void OnScreenshotAccessoriesButtonClicked()
        {

        }

        public void OnScreenshotPetsButtonClicked()
        {

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