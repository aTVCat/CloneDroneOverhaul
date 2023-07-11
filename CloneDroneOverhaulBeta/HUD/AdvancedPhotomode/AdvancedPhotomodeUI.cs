using CDOverhaul.Gameplay.QualityOfLife;
using CDOverhaul.Graphics;
using ModLibrary;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class AdvancedPhotomodeUI : OverhaulUI
    {
        private GameObject m_PanelGameObject;

        private Button m_ExitButton;

        private PrefabAndContainer m_EntriesContainer;
        private PrefabAndContainer m_CategoryLabelsContainer;

        private bool m_HasPopulatedSettings;

        public override void Initialize()
        {
            m_PanelGameObject = MyModdedObject.GetObject<Transform>(0).gameObject;
            m_ExitButton = MyModdedObject.GetObject<Button>(4);
            m_ExitButton.onClick.AddListener(exitPhotomode);
            m_EntriesContainer = new PrefabAndContainer(MyModdedObject, 2, 3);
            m_CategoryLabelsContainer = new PrefabAndContainer(MyModdedObject, 1, 3);
            Hide();
        }

        private void LateUpdate()
        {
            if (Input.GetMouseButton(1))
                ShowCursor = false;
            else
                ShowCursor = m_PanelGameObject.gameObject.activeSelf;

            if (Input.GetKeyDown(KeyCode.C))
                SetPanelActive(!m_PanelGameObject.gameObject.activeSelf);

            if (Time.realtimeSinceStartup - PhotoManager.Instance.GetPrivateField<float>("_lastTimePhotoModeToggled") < 0.2f)
                return;

            if (ShowCursor && Input.GetKeyDown(KeyCode.BackQuote))
                exitPhotomode();

            if (ShowCursor)
                TimeManager.Instance.RestoreOverridePausedTimeScale();
        }

        public void Show()
        {
            AdvancedPhotomodeSettings.RememberCurrentSettings();

            base.gameObject.SetActive(true);
            SetPanelActive(true);

            if (!m_HasPopulatedSettings)
            {
                populate();
                m_HasPopulatedSettings = true;
            }
            OverhaulGraphicsController.PatchAllCameras();
            OverhaulEventsController.DispatchEvent(AdvancedPhotomodeController.PhotoModeSettingUpdateEvent);
        }

        public void Hide()
        {
            base.gameObject.SetActive(false);
            ShowCursor = false;
            OverhaulGraphicsController.PatchAllCameras();
            OverhaulEventsController.DispatchEvent(AdvancedPhotomodeController.PhotoModeSettingUpdateEvent);

            AdvancedPhotomodeSettings.RestoreSettings();
        }

        public void SetPanelActive(bool value)
        {
            m_PanelGameObject.gameObject.SetActive(value);
        }

        private void exitPhotomode()
        {
            Character player = Singleton<CharacterTracker>.Instance.GetPlayer();
            if (!player)
                return;

            PhotoManager.Instance.SetPrivateField("_isInPhotoMode", false);
            PhotoManager.Instance.SetPrivateField("_lastTimePhotoModeToggled", Time.realtimeSinceStartup);

            TimeManager.Instance.OnGameUnPaused();
            PhotoManager.Instance.GetPrivateField<FlyingCameraController>("_cameraController").gameObject.SetActive(false);
            player.MovePlayerCameraBack(0.1f);
            player.SetCameraAnimatorEnabled(true);
            AudioManager.Instance.RestoreMusic();
            GlobalEventManager.Instance.Dispatch("ExitedPhotoMode");
            GameUIRoot.Instance.PhotoModeControlsDisplay.SetVisibility(false);

            PlayerInputController component = player.GetComponent<PlayerInputController>();
            if (component)
            {
                component.enabled = PhotoManager.Instance.GetPrivateField<bool>("_wasPlayerInputEnabled");
            }
            GameUIRoot.Instance.RefreshCursorEnabled();
        }

        private void populate()
        {
            m_EntriesContainer.ClearContainer();
            List<string> categories = AdvancedPhotomodeController.GetAllCategories();
            foreach (string category in categories)
            {
                ModdedObject moddedObject = m_CategoryLabelsContainer.CreateNew();
                moddedObject.GetObject<Text>(0).text = category;

                List<AdvancedPhotomodeSettingAttribute> settings = AdvancedPhotomodeController.GetAllSettingsOfCategory(category);
                foreach(AdvancedPhotomodeSettingAttribute advancedPhotomodeSetting in settings)
                {
                    ModdedObject moddedObject1 = m_EntriesContainer.CreateNew();
                    moddedObject1.GetObject<Text>(0).text = advancedPhotomodeSetting.DisplayName;
                    AdvancedPhotomodeUIEntryDisplay entryDisplay = moddedObject1.gameObject.AddComponent<AdvancedPhotomodeUIEntryDisplay>();
                    entryDisplay.Initialize(advancedPhotomodeSetting);
                }
            }
        }
    }
}
