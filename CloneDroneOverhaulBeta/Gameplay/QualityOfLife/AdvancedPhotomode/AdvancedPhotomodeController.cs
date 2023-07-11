using CDOverhaul.HUD;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using System;
using CDOverhaul.Graphics;

namespace CDOverhaul.Gameplay.QualityOfLife
{
    public class AdvancedPhotomodeController : OverhaulController
    {
        public const string PhotoModeSettingUpdateEvent = "AdvancedPhotomode.SettingUpdate";

        public static AdvancedPhotomodeController Instance;

        public static bool IsAdvancedModeEnabled => OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IsPhotoModeOverhaulEnabled;

        private static readonly List<AdvancedPhotomodeSettingAttribute> s_AllSettings = new List<AdvancedPhotomodeSettingAttribute>();
        public static List<string> GetAllCategories()
        {
            List<string> result = new List<string>();
            foreach(AdvancedPhotomodeSettingAttribute attribute in s_AllSettings)
            {
                if (!result.Contains(attribute.CategoryName))
                    result.Add(attribute.CategoryName);
            }
            return result;
        }
        public static List<AdvancedPhotomodeSettingAttribute> GetAllSettingsOfCategory(string name)
        {
            List<AdvancedPhotomodeSettingAttribute> result = new List<AdvancedPhotomodeSettingAttribute>();
            foreach (AdvancedPhotomodeSettingAttribute attribute in s_AllSettings)
            {
                if (attribute.CategoryName == name)
                    result.Add(attribute);
            }
            return result;
        }

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

            _ = OverhaulEventsController.AddEventListener(PhotoModeSettingUpdateEvent, updateSettings);
            _ = OverhaulEventsController.AddEventListener("EnteredPhotoMode", onEnteredPhotomode, true);
            _ = OverhaulEventsController.AddEventListener("ExitedPhotoMode", onExitedPhotomode, true);

            m_PhotoControlsImage = PhotoModeControls.GetComponent<Image>();
            m_PhotoControlsObjects = new GameObject[PhotoModeControls.transform.childCount];
            int index = 0;
            do
            {
                m_PhotoControlsObjects[index] = PhotoModeControls.transform.GetChild(index).gameObject;
                index++;
            } while (index < PhotoModeControls.transform.childCount);
            SetVanillaUIVisible(false);

            getAllSettings();
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

        private void getAllSettings()
        {
            if (!OverhaulSessionController.GetKey<bool>("HasInitializedSettings"))
            {
                OverhaulSessionController.SetKey("HasInitializedSettings", true);

                Type[] allTypes = OverhaulMod.GetAllTypes();
                int typeIndex = 0;
                do
                {
                    Type currentType = allTypes[typeIndex];
                    FieldInfo[] allFields = currentType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    if (allFields.IsNullOrEmpty())
                    {
                        typeIndex++;
                        continue;
                    }

                    int fieldIndex = 0;
                    do
                    {
                        FieldInfo currentField = allFields[fieldIndex];

                        AdvancedPhotomodeSettingAttribute mainAttribute = currentField.GetCustomAttribute<AdvancedPhotomodeSettingAttribute>();
                        if (mainAttribute == null)
                        {
                            fieldIndex++;
                            continue;
                        }

                        mainAttribute.Field = currentField;
                        mainAttribute.SliderParameters = currentField.GetCustomAttribute<AdvancedPhotomodeSliderParametersAttribute>();
                        mainAttribute.ContentParameters = currentField.GetCustomAttribute<AdvancedPhotomodeRequireContentAttribute>();
                        s_AllSettings.Add(mainAttribute);

                        fieldIndex++;
                    } while (fieldIndex < allFields.Length);

                    typeIndex++;
                } while (typeIndex < allTypes.Length);
            }
        }

        public void SetVanillaUIVisible(bool value)
        {
            m_PhotoControlsImage.enabled = value;
            foreach (GameObject gameObject in m_PhotoControlsObjects)
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

        private void updateSettings()
        {
            if (!PhotoManager.IsInPhotoMode())
                return;

            RenderSettings.fog = AdvancedPhotomodeSettings.OverrideSettings ? AdvancedPhotomodeSettings.Fog : AdvancedPhotomodeSettings.FogEnabled;
            RenderSettings.fogStartDistance = AdvancedPhotomodeSettings.OverrideSettings ? AdvancedPhotomodeSettings.FogStart : AdvancedPhotomodeSettings.FogStartDistance;
            RenderSettings.fogEndDistance = AdvancedPhotomodeSettings.OverrideSettings ? AdvancedPhotomodeSettings.FogEnd : AdvancedPhotomodeSettings.FogEndDistance;
            RenderSettings.fogColor = AdvancedPhotomodeSettings.OverrideSettings ? new HSBColor(AdvancedPhotomodeSettings.FogColH, AdvancedPhotomodeSettings.FogColS, AdvancedPhotomodeSettings.FogColB).ToColor() : AdvancedPhotomodeSettings.FogColor;

            OverhaulGraphicsController.PatchAllCameras();
        }
    }
}
