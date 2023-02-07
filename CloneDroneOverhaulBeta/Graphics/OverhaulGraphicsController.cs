using CDOverhaul.Gameplay;
using OverhaulAPI.SharedMonoBehaviours;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace CDOverhaul.Graphics
{
    public static class OverhaulGraphicsController
    {
        public static Camera UICamera { get; private set; }
        public static Camera MainCamera { get; private set; }

        [OverhaulSettingAttribute("Graphics.Rendering.Deffered rendering", false, false, "Improve lights renderer\nMedium performance impact!")]
        public static bool DefferedRenderer;

        [OverhaulSettingAttribute("Graphics.Shaders.Chromatic Aberration", true, false, "All things on the screen will get colored edges")]
        public static bool ChromaticAberrationEnabled;

        [OverhaulSettingAttribute("Graphics.Shaders.Vignette", true, false, "Shade screen edges")]
        public static bool VignetteEnabled;

        public static void Initialize()
        {
            OverhaulEventManager.AddEventListener<Camera>(MainGameplayController.MainCameraSwitchedEventString, patchCamera);
            OverhaulEventManager.AddEventListener(OverhaulMod.ModDeactivatedEventString, onModDisabled);
            OverhaulEventManager.AddEventListener(SettingsController.SettingChangedEventString, patchAllCameras);

            UICamera = GameUIRoot.Instance.GetComponent<Canvas>().worldCamera;
            addPostProcessingToCamera(UICamera);
        }

        private static void addPostProcessingToCamera(Camera camera)
        {
            patchAndSetCamera(camera, false);
            if (camera == null)
            {
                return;
            }

            OverhaulPostProcessBehaviour.AddPostProcessEffect(camera, AssetController.GetAsset<Material>("M_IE_ChromaticAb", Enumerators.EModAssetBundlePart.Part2), new System.Func<bool>(() => MainCamera != null && ChromaticAberrationEnabled));
            OverhaulPostProcessBehaviour.AddPostProcessEffect(camera, AssetController.GetAsset<Material>("M_IE_Spotlight", Enumerators.EModAssetBundlePart.Part2), new System.Func<bool>(() => MainCamera != null && VignetteEnabled));
        }

        private static void patchCamera(Camera camera)
        {
            patchAndSetCamera(camera, true);
        }

        private static void patchAndSetCamera(Camera camera, bool setCamera)
        {
            if (setCamera) MainCamera = camera;
            if (camera == null)
            {
                return;
            }
            camera.useOcclusionCulling = false;
            camera.renderingPath = !DefferedRenderer ? RenderingPath.UsePlayerSettings : RenderingPath.DeferredShading;

            Bloom bloom = camera.GetComponent<Bloom>();
            if (bloom != null)
            {
                bloom.bloomBlurIterations = 10;
                bloom.bloomIntensity = 0.7f;
                bloom.bloomThreshold = 1.25f;
                bloom.bloomThresholdColor = new Color(1, 1, 0.75f, 1);
            }
        }

        private static void patchAllCameras()
        {
            foreach (Camera cam in Camera.allCameras)
            {
                patchAndSetCamera(cam, false);
            }
        }

        private static void onModDisabled()
        {
            foreach (OverhaulPostProcessBehaviour b in UICamera.GetComponents<OverhaulPostProcessBehaviour>())
            {
                Object.Destroy(b);
            }
        }
    }
}
