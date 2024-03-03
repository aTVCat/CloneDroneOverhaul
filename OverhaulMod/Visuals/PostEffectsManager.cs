using AmplifyOcclusion;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace OverhaulMod.Visuals
{
    public class PostEffectsManager : Singleton<PostEffectsManager>
    {
        [ModSetting(ModSettingsConstants.ENABLE_SSAO, true)]
        public static bool EnableSSAO;

        [ModSetting(ModSettingsConstants.ENABLE_BLOOM, true)]
        public static bool EnableBloom;

        public override void Awake()
        {
            base.Awake();
            ModCore.OnCameraSwitched += onCameraSwitched;
        }

        private void OnDestroy()
        {
            ModCore.OnCameraSwitched -= onCameraSwitched;
            RemovePostEffectsFromCamera(Camera.main);
        }

        private void onCameraSwitched(Camera a, Camera b)
        {
            RemovePostEffectsFromCamera(a);
            AddPostEffectsToCamera(b);
        }

        public void RefreshCameraPostEffects()
        {
            AddPostEffectsToCamera(CameraManager.Instance.mainCamera);
        }

        public void AddPostEffectsToCamera(Camera camera)
        {
            if (!camera || camera.orthographic)
                return;

            GameObject cameraGameObject = camera.gameObject;

            AmplifyOcclusionEffect amplifyOcclusionEffect = camera.GetComponent<AmplifyOcclusionEffect>();
            if (!amplifyOcclusionEffect)
                amplifyOcclusionEffect = cameraGameObject.AddComponent<AmplifyOcclusionEffect>();

            amplifyOcclusionEffect.PerPixelNormals = AmplifyOcclusionEffect.PerPixelNormalSource.None;
            amplifyOcclusionEffect.Bias = 0f;
            amplifyOcclusionEffect.BlurSharpness = 4f;
            amplifyOcclusionEffect.FilterResponse = 0.7f;
            amplifyOcclusionEffect.Bias = 0.2f;
            amplifyOcclusionEffect.SampleCount = SampleCountLevel.Medium;
            amplifyOcclusionEffect.Intensity = 0.75f;
            amplifyOcclusionEffect.ApplyMethod = AmplifyOcclusionEffect.ApplicationMethod.PostEffect;
            amplifyOcclusionEffect.FadeEnabled = true;
            amplifyOcclusionEffect.FadeStart = 0f;
            amplifyOcclusionEffect.FadeLength = Mathf.Min(500f, RenderSettings.fogEndDistance);
            amplifyOcclusionEffect.enabled = EnableSSAO;

            Bloom bloom = camera.GetComponent<Bloom>();
            if (bloom)
            {
                bloom.bloomBlurIterations = 4;
                bloom.bloomIntensity = 0.5f;
                bloom.bloomThreshold = 1f;
                bloom.enabled = EnableBloom;
            }
        }

        public void RemovePostEffectsFromCamera(Camera camera)
        {
            if (!camera)
                return;

            AmplifyOcclusionEffect amplifyOcclusionEffect = camera.GetComponent<AmplifyOcclusionEffect>();
            if (amplifyOcclusionEffect)
            {
                Destroy(amplifyOcclusionEffect);
            }

            Bloom bloom = camera.GetComponent<Bloom>();
            if (bloom)
            {
                bloom.bloomBlurIterations = 2;
                bloom.bloomIntensity = 0.5f;
                bloom.bloomThreshold = 0.9f;
                bloom.enabled = true;
            }
        }
    }
}
