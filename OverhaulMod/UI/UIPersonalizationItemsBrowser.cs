using OverhaulMod.Engine;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPersonalizationItemsBrowser : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElement("Panel")]
        private readonly RectTransform m_panel;

        private RectTransform m_rectTransform;

        private bool m_isOpen;

        public override bool enableCursor => true;

        protected override void OnInitialized()
        {
            m_rectTransform = base.GetComponent<RectTransform>();
        }

        public override void Show()
        {
            base.Show();
            m_isOpen = true;
            setCameraZoomedIn(true);
        }

        public override void Hide()
        {
            base.Hide();
            m_isOpen = false;
            setCameraZoomedIn(false);
        }

        private void LateUpdate()
        {
            refreshCameraRect();
        }

        private void refreshCameraRect()
        {
            CameraManager cameraManager = CameraManager.Instance;
            if (m_isOpen)
            {
                float proportionOfWidthTakenUpBySidebar = m_panel.rect.width / m_rectTransform.rect.width;
                cameraManager.SetCameraRect(new Rect(proportionOfWidthTakenUpBySidebar, 0, 1f - proportionOfWidthTakenUpBySidebar, 1f));
            }
            else
            {
                cameraManager.ResetCameraRect();
            }
        }

        private void setCameraZoomedIn(bool value)
        {
            FirstPersonMover firstPersonMover = CharacterTracker.Instance.GetPlayerRobot();
            CameraManager cameraManager = CameraManager.Instance;
            if (!value)
            {
                refreshCameraRect();
                cameraManager.ResetCameraHolderPosition(firstPersonMover);
                cameraManager.ResetCameraHolderEulerAngles(firstPersonMover);
                cameraManager.enableForceFOVOffset = false;
                return;
            }

            refreshCameraRect();
            cameraManager.SetCameraHolderPosition(Vector3.forward * 0.75f, firstPersonMover);
            cameraManager.SetCameraHolderEulerAngles(Vector3.up * -90f, firstPersonMover);
            cameraManager.enableForceFOVOffset = true;
            cameraManager.forceFOVOffset = -5f;

            ModGameUtils.WaitForPlayerInputUpdate(delegate (IFPMoveCommandInput commandInput)
            {
                commandInput.IsResetLookKeyDown = true;
            });
        }
    }
}
