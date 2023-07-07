using CDOverhaul.HUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.QualityOfLife
{
    public class AdvancedPhotomodeController : OverhaulController 
    {
        public static AdvancedPhotomodeController Instance;

        public static bool IsAdvancedModeEnabled => OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IsPhotoModeOverhaulEnabled;

        public PhotoManager PhotoManager
        {
            get;
            private set;
        }

        public PhotoModeControlsDisplay PhotoModeControls
        {
            get;
            private set;
        }

        private AdvancedPhotomodeUI m_NewUI;
        public AdvancedPhotomodeUI NewUI
        {
            get
            {
                if (!m_NewUI)
                    m_NewUI = GetController<AdvancedPhotomodeUI>();

                return m_NewUI;
            }
        }

        private Image m_PhotoControlsImage;
        private GameObject[] m_PhotoControlsObjects;

        public override void Initialize()
        {
            Instance = this;
            PhotoManager = PhotoManager.Instance;
            PhotoModeControls = GameUIRoot.Instance.PhotoModeControlsDisplay;

            OverhaulEventsController.AddEventListener("EnteredPhotoMode", onEnteredPhotomode, true);
            OverhaulEventsController.AddEventListener("ExitedPhotoMode", onExitedPhotomode, true);

            m_PhotoControlsImage = PhotoModeControls.GetComponent<Image>();
            m_PhotoControlsObjects = new GameObject[PhotoModeControls.transform.childCount];
            int index = 0;
            do
            {
                m_PhotoControlsObjects[index] = PhotoModeControls.transform.GetChild(index).gameObject;
                index++;
            } while (index < PhotoModeControls.transform.childCount);
            SetVanillaUIVisible(false);
        }

        public override void OnModDeactivated()
        {
            SetVanillaUIVisible(true);
        }

        protected override void OnDisposed()
        {
            Instance = null;
            base.OnDisposed();

            OverhaulEventsController.RemoveEventListener("EnteredPhotoMode", onEnteredPhotomode, true);
            OverhaulEventsController.RemoveEventListener("ExitedPhotoMode", onExitedPhotomode, true);
        }

        public void SetVanillaUIVisible(bool value)
        {
            m_PhotoControlsImage.enabled = value;
            foreach(GameObject gameObject in m_PhotoControlsObjects)
            {
                if (gameObject)
                    gameObject.SetActive(value);
            }
        }

        private void onEnteredPhotomode()
        {
            SetVanillaUIVisible(!IsAdvancedModeEnabled);

            if (!IsAdvancedModeEnabled || !NewUI)
                return;

            NewUI.Show();
        }

        private void onExitedPhotomode()
        {
            if (!NewUI)
                return;

            NewUI.Hide();
        }
    }
}
