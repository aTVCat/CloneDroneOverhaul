using CDOverhaul.Graphics;
using CDOverhaul.HUD;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.QualityOfLife
{
    public class AdvancedPhotomodeController : OverhaulController
    {
        public const string PhotoModeSettingUpdateEvent = "AdvancedPhotomode.SettingUpdate";

        public static AdvancedPhotomodeController Instance;

        public static bool IsAdvancedModeEnabled => OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IsPhotoModeOverhaulEnabled;

        private static readonly List<AdvancedPhotomodeSettingAttribute> s_AllSettings = new List<AdvancedPhotomodeSettingAttribute>();

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

        public static bool HasEverEnteredPhotoMode;

        public override void Initialize()
        {
            Instance = this;
            HasEverEnteredPhotoMode = false;
            PhotoManager = PhotoManager.Instance;
            PhotoModeControls = GameUIRoot.Instance.PhotoModeControlsDisplay;

            OverhaulEvents.AddEventListener(PhotoModeSettingUpdateEvent, updateSettings);
            OverhaulEvents.AddEventListener("EnteredPhotoMode", onEnteredPhotomode, true);
            OverhaulEvents.AddEventListener("ExitedPhotoMode", onExitedPhotomode, true);

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

            OverhaulEvents.RemoveEventListener("EnteredPhotoMode", onEnteredPhotomode, true);
            OverhaulEvents.RemoveEventListener("ExitedPhotoMode", onExitedPhotomode, true);
        }

        public static List<string> GetAllCategories()
        {
            List<string> result = new List<string>();
            foreach (AdvancedPhotomodeSettingAttribute attribute in s_AllSettings)
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
                    MethodInfo[] allMethods = currentType.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    if (!allMethods.IsNullOrEmpty())
                    {
                        int methodIndex = 0;
                        do
                        {
                            MethodInfo currentMethod = allMethods[methodIndex];

                            AdvancedPhotomodeSettingAttribute mainAttribute = currentMethod.GetCustomAttribute<AdvancedPhotomodeSettingAttribute>();
                            if (mainAttribute == null)
                            {
                                methodIndex++;
                                continue;
                            }

                            mainAttribute.Method = currentMethod;
                            s_AllSettings.Add(mainAttribute);

                            methodIndex++;
                        } while (methodIndex < allMethods.Length);
                    }

                    FieldInfo[] allFields = currentType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    if (!allFields.IsNullOrEmpty())
                    {
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
                    }
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
            HasEverEnteredPhotoMode = true;
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
            if (!HasEverEnteredPhotoMode || !PhotoManager.IsInPhotoMode())
                return;

            RenderSettings.fog = AdvancedPhotomodeSettings.OverrideSettings ? AdvancedPhotomodeSettings.Fog : AdvancedPhotomodeSettings.FogEnabledBefore;
            RenderSettings.fogStartDistance = AdvancedPhotomodeSettings.OverrideSettings ? AdvancedPhotomodeSettings.FogStart : AdvancedPhotomodeSettings.FogStartDistanceBefore;
            RenderSettings.fogEndDistance = AdvancedPhotomodeSettings.OverrideSettings ? AdvancedPhotomodeSettings.FogEnd : AdvancedPhotomodeSettings.FogEndDistanceBefore;
            RenderSettings.fogColor = AdvancedPhotomodeSettings.OverrideSettings ? new HSBColor(AdvancedPhotomodeSettings.FogColH, AdvancedPhotomodeSettings.FogColS, AdvancedPhotomodeSettings.FogColB).ToColor() : AdvancedPhotomodeSettings.FogColorBefore;

            RenderSettings.ambientMode = (AmbientMode)(AdvancedPhotomodeSettings.OverrideSettings ? AdvancedPhotomodeSettings.AmbMode : AdvancedPhotomodeSettings.AmbModeBefore);
            RenderSettings.ambientIntensity = AdvancedPhotomodeSettings.OverrideSettings ? AdvancedPhotomodeSettings.AmbIntensity : AdvancedPhotomodeSettings.AmbIntensityBefore;
            RenderSettings.ambientLight = AdvancedPhotomodeSettings.OverrideSettings ? new HSBColor(AdvancedPhotomodeSettings.AmbColS, AdvancedPhotomodeSettings.AmbColS, AdvancedPhotomodeSettings.AmbColB).ToColor() : AdvancedPhotomodeSettings.AmbColorBefore;

            Light light = DirectionalLightManager.Instance.DirectionalLight;
            Transform lightTransform = light.transform;
            lightTransform.gameObject.SetActive(AdvancedPhotomodeSettings.OverrideSettings ? AdvancedPhotomodeSettings.DLEnable : AdvancedPhotomodeSettings.DLEnabledBefore);
            lightTransform.localEulerAngles = new Vector3(AdvancedPhotomodeSettings.OverrideSettings ? AdvancedPhotomodeSettings.DLX : AdvancedPhotomodeSettings.DLXBefore,
                AdvancedPhotomodeSettings.OverrideSettings ? AdvancedPhotomodeSettings.DLY : AdvancedPhotomodeSettings.DLYBefore,
                0f);
            light.color = AdvancedPhotomodeSettings.OverrideSettings ? new HSBColor(AdvancedPhotomodeSettings.DLColH, AdvancedPhotomodeSettings.DLColS, AdvancedPhotomodeSettings.DLColB).ToColor() : AdvancedPhotomodeSettings.DLColorBefore;
            light.intensity = AdvancedPhotomodeSettings.OverrideSettings ? AdvancedPhotomodeSettings.DLIntensity : AdvancedPhotomodeSettings.DLIntensityBefore;

            OverhaulGraphicsController.PatchAllCameras();
        }
    }
}
