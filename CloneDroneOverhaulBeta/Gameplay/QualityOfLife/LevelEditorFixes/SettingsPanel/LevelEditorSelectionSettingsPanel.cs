﻿using CDOverhaul.Credits;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.QualityOfLife
{
    public class LevelEditorSelectionSettingsPanel : OverhaulBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
    {
        [OverhaulSettingAttribute_Old("Player.LevelEditor.EnableOutline", true, true)]
        public static bool EnableOutline;

        [OverhaulSettingAttribute_Old("Player.LevelEditor.SelectionOutlineColor", 0.6f, true)]
        public static float OutlineColorRaw;
        [OverhaulSettingAttribute_Old("Player.LevelEditor.SelectionOutlineColorBrightness", 0f, true)]
        public static float OutlineColorBrightnessRaw;

        [OverhaulSettingAttribute_Old("Player.LevelEditor.SelectionOutlineWidth", 3.5f, true)]
        public static float OutlineWidth;

        public static Color OutlineColor { get; private set; }

        public bool IsShown { get; private set; }
        private bool m_IsMouseIn;
        private static bool m_CannotExecuteSliderEvents;

        public override void Start()
        {
            RefreshSliders();
            Hide();

            base.GetComponent<ModdedObject>().GetObject<Slider>(0).onValueChanged.AddListener(applySettings);
            base.GetComponent<ModdedObject>().GetObject<Slider>(1).onValueChanged.AddListener(applySettings);
            base.GetComponent<ModdedObject>().GetObject<Slider>(3).onValueChanged.AddListener(applySettings);
            base.GetComponent<ModdedObject>().GetObject<Toggle>(4).onValueChanged.AddListener(applySettings);
            base.GetComponent<ModdedObject>().GetObject<Button>(5).onClick.AddListener(ResetSettings);

            DelegateScheduler.Instance.Schedule(RefreshSliders, 1f);
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0) && !m_IsMouseIn)
                Hide();
        }

        public void Show()
        {
            base.gameObject.SetActive(true);
            IsShown = true;

            RefreshSliders();
        }

        public void Hide()
        {
            base.gameObject.SetActive(false);
            IsShown = false;
            m_IsMouseIn = false;
        }

        public static void RefreshSliders()
        {
            if (LevelEditorFixes.SelectionSettingsPanel == null)
                return;

            m_CannotExecuteSliderEvents = true;
            LevelEditorFixes.SelectionSettingsPanel.GetComponent<ModdedObject>().GetObject<Slider>(0).value = OutlineWidth;
            LevelEditorFixes.SelectionSettingsPanel.GetComponent<ModdedObject>().GetObject<Slider>(1).value = OutlineColorRaw;
            LevelEditorFixes.SelectionSettingsPanel.GetComponent<ModdedObject>().GetObject<Slider>(3).value = OutlineColorBrightnessRaw;
            LevelEditorFixes.SelectionSettingsPanel.GetComponent<ModdedObject>().GetObject<Toggle>(4).isOn = EnableOutline;
            m_CannotExecuteSliderEvents = false;
        }

        public static void ApplySettings()
        {
            if (LevelEditorFixes.SelectionSettingsPanel == null)
                return;

            OverhaulSettingInfo_Old.SavePref(OverhaulSettingsManager_Old.GetSetting("Player.LevelEditor.SelectionOutlineColor", true), LevelEditorFixes.SelectionSettingsPanel.GetComponent<ModdedObject>().GetObject<Slider>(1).value);
            OverhaulSettingInfo_Old.SavePref(OverhaulSettingsManager_Old.GetSetting("Player.LevelEditor.SelectionOutlineColorBrightness", true), LevelEditorFixes.SelectionSettingsPanel.GetComponent<ModdedObject>().GetObject<Slider>(3).value);
            OverhaulSettingInfo_Old.SavePref(OverhaulSettingsManager_Old.GetSetting("Player.LevelEditor.SelectionOutlineWidth", true), LevelEditorFixes.SelectionSettingsPanel.GetComponent<ModdedObject>().GetObject<Slider>(0).value);
            OverhaulSettingInfo_Old.SavePref(OverhaulSettingsManager_Old.GetSetting("Player.LevelEditor.EnableOutline", true), LevelEditorFixes.SelectionSettingsPanel.GetComponent<ModdedObject>().GetObject<Toggle>(4).isOn);

            OutlineColor = Color.HSVToRGB(OutlineColorRaw, OutlineColorBrightnessRaw, 1f);
            ThreeDOutline.UpdateMaterialProperties();
        }

        public static void ResetSettings()
        {
            OverhaulSettingInfo_Old info1 = OverhaulSettingsManager_Old.GetSetting("Player.LevelEditor.SelectionOutlineColor", true);
            OverhaulSettingInfo_Old.SavePref(info1, info1.DefaultValue);

            OverhaulSettingInfo_Old info2 = OverhaulSettingsManager_Old.GetSetting("Player.LevelEditor.SelectionOutlineColorBrightness", true);
            OverhaulSettingInfo_Old.SavePref(info2, info2.DefaultValue);

            OverhaulSettingInfo_Old info3 = OverhaulSettingsManager_Old.GetSetting("Player.LevelEditor.SelectionOutlineWidth", true);
            OverhaulSettingInfo_Old.SavePref(info3, info3.DefaultValue);

            OverhaulSettingInfo_Old info4 = OverhaulSettingsManager_Old.GetSetting("Player.LevelEditor.EnableOutline", true);
            OverhaulSettingInfo_Old.SavePref(info4, info4.DefaultValue);

            OutlineColor = Color.HSVToRGB(OutlineColorRaw, OutlineColorBrightnessRaw, 1f);
            ThreeDOutline.UpdateMaterialProperties();
            RefreshSliders();
        }

        private void applySettings(float a)
        {
            if (m_CannotExecuteSliderEvents)
                return;

            ApplySettings();
        }

        private void applySettings(bool a)
        {
            if (m_CannotExecuteSliderEvents)
                return;

            ApplySettings();
        }

        public void ToggleVisibility()
        {
            if (IsShown)
                Hide();
            else
                Show();
        }

        public void OnPointerEnter(PointerEventData eventData) => m_IsMouseIn = true;
        public void OnPointerExit(PointerEventData eventData) => m_IsMouseIn = false;
        public void OnPointerUp(PointerEventData eventData) => OnPointerExit(null);
    }
}
