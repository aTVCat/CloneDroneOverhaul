using CDOverhaul.Credits;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.QualityOfLife
{
    public class LevelEditorSelectionSettingsPanel : OverhaulBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
    {
        [OverhaulSetting("Player.LevelEditor.SelectionOutlineColor", 0.6f, true)]
        public static float OutlineColorRaw;
        [OverhaulSetting("Player.LevelEditor.SelectionOutlineColorBrightness", 0f, true)]
        public static float OutlineColorBrightnessRaw;

        [OverhaulSetting("Player.LevelEditor.SelectionOutlineWidth", 3.5f, true)]
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

            DelegateScheduler.Instance.Schedule(RefreshSliders, 1f);
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0) && !m_IsMouseIn)
            {
                Hide();
            }
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
            {
                return;
            }

            m_CannotExecuteSliderEvents = true;
            LevelEditorFixes.SelectionSettingsPanel.GetComponent<ModdedObject>().GetObject<Slider>(0).value = OutlineWidth;
            LevelEditorFixes.SelectionSettingsPanel.GetComponent<ModdedObject>().GetObject<Slider>(1).value = OutlineColorRaw;
            LevelEditorFixes.SelectionSettingsPanel.GetComponent<ModdedObject>().GetObject<Slider>(3).value = OutlineColorBrightnessRaw;
            m_CannotExecuteSliderEvents = false;
        }

        public static void ApplySettings()
        {
            if (LevelEditorFixes.SelectionSettingsPanel == null)
            {
                return;
            }

            SettingInfo.SavePref(SettingsController.GetSetting("Player.LevelEditor.SelectionOutlineColor", true), LevelEditorFixes.SelectionSettingsPanel.GetComponent<ModdedObject>().GetObject<Slider>(1).value);
            SettingInfo.SavePref(SettingsController.GetSetting("Player.LevelEditor.SelectionOutlineColorBrightness", true), LevelEditorFixes.SelectionSettingsPanel.GetComponent<ModdedObject>().GetObject<Slider>(3).value);
            SettingInfo.SavePref(SettingsController.GetSetting("Player.LevelEditor.SelectionOutlineWidth", true), LevelEditorFixes.SelectionSettingsPanel.GetComponent<ModdedObject>().GetObject<Slider>(0).value);

            OutlineColor = Color.HSVToRGB(OutlineColorRaw, OutlineColorBrightnessRaw, 1f);
            ThreeDOutline.UpdateMaterialProperties();
        }

        private void applySettings(float a)
        {
            if (m_CannotExecuteSliderEvents)
            {
                return;
            }
            ApplySettings();
        }

        public void ToggleVisibility()
        {
            if (IsShown)
            {
                Hide();
                return;
            }
            Show();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            m_IsMouseIn = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_IsMouseIn = false;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            m_IsMouseIn = false;
        }
    }
}
