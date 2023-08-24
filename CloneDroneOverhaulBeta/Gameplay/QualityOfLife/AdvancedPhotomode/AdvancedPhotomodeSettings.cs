using CDOverhaul.Examples.AdditionalContent;
using UnityEngine;
using UnityEngine.Rendering;

namespace CDOverhaul.Gameplay.QualityOfLife
{
    public static class AdvancedPhotomodeSettings
    {
        public static Material SkyboxMaterial;

        public static bool FogEnabledBefore;
        public static float FogStartDistanceBefore;
        public static float FogEndDistanceBefore;
        public static Color FogColorBefore;

        public static float AmbModeBefore;
        public static float AmbIntensityBefore;
        public static Color AmbColorBefore;

        public static bool DLEnabledBefore;
        public static float DLIntensityBefore;
        public static float DLXBefore;
        public static float DLYBefore;
        public static Color DLColorBefore;

        public static void RememberCurrentSettings()
        {
            SkyboxMaterial = RenderSettings.skybox;

            FogEnabledBefore = RenderSettings.fog;
            FogStartDistanceBefore = RenderSettings.fogStartDistance;
            FogEndDistanceBefore = RenderSettings.fogEndDistance;
            FogColorBefore = RenderSettings.fogColor;

            AmbModeBefore = (int)RenderSettings.ambientMode;
            AmbIntensityBefore = RenderSettings.ambientIntensity;
            AmbColorBefore = RenderSettings.ambientLight;

            Light light = DirectionalLightManager.Instance.DirectionalLight;
            Transform lightTransform = light.transform;
            DLEnabledBefore = light.gameObject.activeSelf;
            DLIntensityBefore = light.intensity;
            DLColorBefore = light.color;
            DLXBefore = lightTransform.localEulerAngles.x;
            DLYBefore = lightTransform.localEulerAngles.y;
        }

        public static void RestoreSettings()
        {
            if (SkyboxMaterial)
                RenderSettings.skybox = SkyboxMaterial;

            RenderSettings.fog = FogEnabledBefore;
            RenderSettings.fogStartDistance = FogStartDistanceBefore;
            RenderSettings.fogEndDistance = FogEndDistanceBefore;
            RenderSettings.fogColor = FogColorBefore;

            RenderSettings.ambientMode = (AmbientMode)AmbModeBefore;
            RenderSettings.ambientIntensity = AmbIntensityBefore;
            RenderSettings.ambientLight = AmbColorBefore;

            Light light = DirectionalLightManager.Instance.DirectionalLight;
            Transform lightTransform = light.transform;
            light.gameObject.SetActive(DLEnabledBefore);
            light.intensity = DLIntensityBefore;
            light.color = DLColorBefore;
            lightTransform.localEulerAngles = new Vector3(DLXBefore, DLYBefore);
        }

        [AdvancedPhotomodeSetting("Override parameters", "General")]
        public static bool OverrideSettings = false;
        public static bool IsOverridingSettings => OverrideSettings && AdvancedPhotomodeController.Instance && AdvancedPhotomodeController.Instance.PhotoManager.IsInPhotoMode();


        [AdvancedPhotomodeSetting("Enable", "Fog")]
        public static bool Fog = true;

        [AdvancedPhotomodeSliderParameters(1f, 2999.9f)]
        [AdvancedPhotomodeSetting("Start Distance", "Fog")]
        public static float FogStart = 100f;

        [AdvancedPhotomodeSliderParameters(1.1f, 3000f)]
        [AdvancedPhotomodeSetting("End Distance", "Fog")]
        public static float FogEnd = 400f;


        [AdvancedPhotomodeSliderParameters(0f, 1f)]
        [AdvancedPhotomodeSetting("Hue", "Fog Color")]
        public static float FogColH;

        [AdvancedPhotomodeSliderParameters(0f, 1f)]
        [AdvancedPhotomodeSetting("Saturation", "Fog Color")]
        public static float FogColS;

        [AdvancedPhotomodeSliderParameters(0f, 1f)]
        [AdvancedPhotomodeSetting("Brightness", "Fog Color")]
        public static float FogColB;


        [AdvancedPhotomodeSliderParameters(0, 2)]
        [AdvancedPhotomodeSetting("Mode", "Ambient")]
        public static int AmbMode = 0;

        [AdvancedPhotomodeSliderParameters(0f, 3f)]
        [AdvancedPhotomodeSetting("Intensity", "Ambient")]
        public static float AmbIntensity = 0.6f;


        [AdvancedPhotomodeSliderParameters(0f, 1f)]
        [AdvancedPhotomodeSetting("Hue", "Ambient Color")]
        public static float AmbColH;

        [AdvancedPhotomodeSliderParameters(0f, 1f)]
        [AdvancedPhotomodeSetting("Saturation", "Ambient Color")]
        public static float AmbColS;

        [AdvancedPhotomodeSliderParameters(0f, 1f)]
        [AdvancedPhotomodeSetting("Brightness", "Ambient Color")]
        public static float AmbColB = 0.5f;


        [AdvancedPhotomodeSetting("Enable", "Directional Light")]
        public static bool DLEnable = true;

        [AdvancedPhotomodeSliderParameters(0f, 3f)]
        [AdvancedPhotomodeSetting("Intensity", "Directional Light")]
        public static float DLIntensity;

        [AdvancedPhotomodeSliderParameters(0f, 360f)]
        [AdvancedPhotomodeSetting("Rotation X", "Directional Light")]
        public static float DLX;

        [AdvancedPhotomodeSliderParameters(0f, 360f)]
        [AdvancedPhotomodeSetting("Rotation Y", "Directional Light")]
        public static float DLY;


        [AdvancedPhotomodeSliderParameters(0f, 1f)]
        [AdvancedPhotomodeSetting("Hue", "Directional Light Color")]
        public static float DLColH;

        [AdvancedPhotomodeSliderParameters(0f, 1f)]
        [AdvancedPhotomodeSetting("Saturation", "Directional Light Color")]
        public static float DLColS;

        [AdvancedPhotomodeSliderParameters(0f, 1f)]
        [AdvancedPhotomodeSetting("Brightness", "Directional Light Color")]
        public static float DLColB;


        [AdvancedPhotomodeSetting("Enable", "Ambient Occlusion")]
        public static bool SSAOEnable = true;

        [AdvancedPhotomodeSliderParameters(0.1f, 1.2f)]
        [AdvancedPhotomodeSetting("Intensity", "Ambient Occlusion")]
        public static float SSAOIntensity = 0.75f;

        [AdvancedPhotomodeSliderParameters(0, 3)]
        [AdvancedPhotomodeSetting("Sample Count", "Ambient Occlusion")]
        public static int SSAOSampleCount = 1;


        [AdvancedPhotomodeRequireContent(MoreSkyboxesController.RequiredContent)]
        [AdvancedPhotomodeSliderParameters(-1, 15)]
        [AdvancedPhotomodeSetting("Variant", "More skyboxes")]
        public static int MoreSkyboxesIndex = -1;

        [AdvancedPhotomodeRequireContent(MoreSkyboxesController.RequiredContent)]
        [AdvancedPhotomodeSliderParameters(0f, 360f)]
        [AdvancedPhotomodeSetting("Rotation", "More skyboxes")]
        public static float SkyboxRotation = 0f;

        [AdvancedPhotomodeSetting("Copy settings", "More skyboxes", true)]
        public static void CopyLevelRenderSettings()
        {
            LevelDescription levelDescription = LevelManager.Instance.GetCurrentLevelDescription();
            if (levelDescription == null || string.IsNullOrEmpty(levelDescription.PrefabName))
                return;

            string levelId = levelDescription.PrefabName.Contains("/") ? levelDescription.PrefabName.Substring(levelDescription.PrefabName.LastIndexOf("/") + 1) : levelDescription.PrefabName;
            (string.Format("[ \"{0}\", new Hashtable() [ {1} ]", levelId, getLevelRenderSettingsString()).Replace('[', '{').Replace(']', '}') + " },").CopyToClipboard();
        }

        [AdvancedPhotomodeSetting("Copy skybox settings", "More skyboxes", true)]
        public static void CopyLevelSkyboxSettings()
        {
            LevelDescription levelDescription = LevelManager.Instance.GetCurrentLevelDescription();
            if (levelDescription == null || string.IsNullOrEmpty(levelDescription.PrefabName))
                return;

            string levelId = levelDescription.PrefabName.Contains("/") ? levelDescription.PrefabName.Substring(levelDescription.PrefabName.LastIndexOf("/") + 1) : levelDescription.PrefabName;
            ("{ \"" + levelId + "\", " + MoreSkyboxesIndex + " },").CopyToClipboard();
        }

        private static string getLevelRenderSettingsString()
        {
            string result = getHashtableEntryString_Color("FogColor", RenderSettings.fogColor, false) +
                getHashtableEntryString_Color("DLColor", DirectionalLightManager.Instance.DirectionalLight.color, false) +
                getHashtableEntryString_Color("AmbientColor", RenderSettings.ambientLight, false) +
                getHashtableEntryString_Float("DLX", DirectionalLightManager.Instance.DirectionalLight.transform.localEulerAngles.x, false) +
                getHashtableEntryString_Float("DLY", DirectionalLightManager.Instance.DirectionalLight.transform.localEulerAngles.y, false) +
                getHashtableEntryString_Float("FogEnd", RenderSettings.fogEndDistance, false) +
                getHashtableEntryString_Float("FogStart", RenderSettings.fogStartDistance, false);

            Material material = RenderSettings.skybox;
            if (material && material.HasProperty("_Rotation"))
                result += getHashtableEntryString_Float("SkyboxRotation", material.GetFloat("_Rotation"), false);

            result += getHashtableEntryString_Float("DLIntensity", DirectionalLightManager.Instance.DirectionalLight.intensity, true);
            return result;
        }

        private static string getFloatString(float value)
        {
            return value.ToString().Replace(',', '.') + "f";
        }

        private static string getHashtableEntryString_Float(string property, float value, bool isLast)
        {
            return string.Format("[ \"{0}\", {1} ]", property, getFloatString(value)) + (isLast ? " " : ", ");
        }

        private static string getHashtableEntryString_Color(string property, Color value, bool isLast)
        {
            float colR = value.r;
            float colG = value.b;
            float colB = value.g;
            return string.Format("[ \"{0}\", new Color({1}, {2}, {3}, 1f) ]", new object[] { property, getFloatString(colR), getFloatString(colG), getFloatString(colB) }) + (isLast ? " " : ",");
        }
    }
}
