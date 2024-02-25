using AmplifyOcclusion;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Visuals
{
    public class PostEffectsManager : Singleton<PostEffectsManager>
    {
        [ModSetting(ModSettingsConstants.ENABLE_SSAO, true)]
        public static bool EnableSSAO;

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
            amplifyOcclusionEffect.enabled = EnableSSAO;
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
        }
    }
}
