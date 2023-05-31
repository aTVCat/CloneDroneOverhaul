using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class FirstUseSetupUI : OverhaulUI
    {
        [OverhaulSetting("Player.Mod.HasConfiguredMod", false, true)]
        public static bool HasSetTheModUp;

        private Button m_DoneButton;

        private Button m_UseVanillaGraphicsSettings;
        private Outline m_UseVanillaGraphicsSettingsOutline;
        private Button m_UseOverhaulGraphicsSettings;
        private Outline m_UseOverhaulGraphicsSettingsOutline;

        public override void Initialize()
        {
            m_DoneButton = MyModdedObject.GetObject<Button>(2);
            m_DoneButton.onClick.AddListener(EndSetup);
            
            m_UseVanillaGraphicsSettings = MyModdedObject.GetObject<Button>(0);
            m_UseVanillaGraphicsSettings.onClick.AddListener(OnUseVanillaGraphicsSettingsClicked);
            m_UseVanillaGraphicsSettingsOutline = MyModdedObject.GetObject<Outline>(0);
            m_UseOverhaulGraphicsSettings = MyModdedObject.GetObject<Button>(1);
            m_UseOverhaulGraphicsSettings.onClick.AddListener(OnUseOverhaulGraphicsSettingsClicked);
            m_UseOverhaulGraphicsSettingsOutline = MyModdedObject.GetObject<Outline>(1);
            base.gameObject.SetActive(false);

            if (!HasSetTheModUp && GameModeManager.IsOnTitleScreen())
                DelegateScheduler.Instance.Schedule(Show, 1f);
        }

        public void Show()
        {
            base.gameObject.SetActive(true);
            ResetSelection();

            GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(false);
        }

        public void Hide()
        {
            base.gameObject.SetActive(false);

            GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(true);
        }

        public void EndSetup()
        {
            SettingInfo.SavePref(SettingsController.GetSetting("Player.Mod.HasConfiguredMod", true), true);
            Hide();
        }

        public void AllowEndingTheSetup()
        {
            m_DoneButton.interactable = true;
        }

        public void ResetSelection()
        {
            m_DoneButton.interactable = false;


            m_UseOverhaulGraphicsSettingsOutline.enabled = false;
            m_UseVanillaGraphicsSettingsOutline.enabled = false;
        }

        public void OnUseVanillaGraphicsSettingsClicked()
        {
            m_UseOverhaulGraphicsSettingsOutline.enabled = false;
            m_UseVanillaGraphicsSettingsOutline.enabled = true;
            AllowEndingTheSetup();
        }
        public void OnUseOverhaulGraphicsSettingsClicked()
        {
            m_UseOverhaulGraphicsSettingsOutline.enabled = true;
            m_UseVanillaGraphicsSettingsOutline.enabled = false;
            AllowEndingTheSetup();
        }
    }
}
