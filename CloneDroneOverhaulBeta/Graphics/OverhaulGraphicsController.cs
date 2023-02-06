using CDOverhaul.Gameplay;
using OverhaulAPI.SharedMonoBehaviours;
using UnityEngine;

namespace CDOverhaul.Graphics
{
    public static class OverhaulGraphicsController
    {
        public static Camera UICamera { get; private set; }
        public static Camera MainCamera { get; private set; }

        [OverhaulSettingAttribute("Graphics.Shaders.Chromatic Aberration", true, false, "All things on the screen will get colored edges")]
        public static bool ChromaticAberrationEnabled;

        [OverhaulSettingAttribute("Graphics.Shaders.Vignette", true, false, "Shade screen edges")]
        public static bool VignetteEnabled;

        public static void Initialize()
        {
            OverhaulEventManager.AddEventListener<Camera>(MainGameplayController.MainCameraSwitchedEventString, mainCameraUpdate);
            OverhaulEventManager.AddEventListener(OverhaulMod.ModDeactivatedEventString, onModDisabled);

            UICamera = GameUIRoot.Instance.GetComponent<Canvas>().worldCamera;
            addPostProcessingToCamera(UICamera);
        }

        private static void addPostProcessingToCamera(Camera camera)
        {
            if (camera == null)
            {
                return;
            }

            OverhaulPostProcessBehaviour.AddPostProcessEffect(camera, AssetController.GetAsset<Material>("M_IE_ChromaticAb", Enumerators.EModAssetBundlePart.Part2), new System.Func<bool>(() => MainCamera != null && ChromaticAberrationEnabled));
            OverhaulPostProcessBehaviour.AddPostProcessEffect(camera, AssetController.GetAsset<Material>("M_IE_Spotlight", Enumerators.EModAssetBundlePart.Part2), new System.Func<bool>(() => MainCamera != null && VignetteEnabled));
        }

        private static void mainCameraUpdate(Camera camera)
        {
            MainCamera = camera;
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
