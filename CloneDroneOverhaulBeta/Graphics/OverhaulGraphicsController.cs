using CDOverhaul.Gameplay;
using OverhaulAPI.SharedMonoBehaviours;
using UnityEngine;

namespace CDOverhaul.Graphics
{
    public static class OverhaulGraphicsController
    {
        public static bool IsWorking { get; private set; }
        public static Camera UICamera { get; private set; }

        public static void Initialize()
        {
            if (IsWorking)
            {
                return;
            }

            OverhaulEventManager.AddEventListener<Camera>(MainGameplayController.MainCameraSwitchedEventString, patchCameraSettings);
            OverhaulEventManager.AddEventListener(OverhaulBase.ModDeactivatedEventString, onModDisabled);

            IsWorking = true;
            UICamera = GameUIRoot.Instance.GetComponent<Canvas>().worldCamera;

            addPostProcessingToCamera(UICamera);
        }

        private static void addPostProcessingToCamera(Camera camera)
        {
            if (!IsWorking || camera == null)
            {
                return;
            }

            Material[] array = new Material[]
            {
                 AssetController.GetAsset<Material>("M_IE_ChromaticAb", Enumerators.EModAssetBundlePart.Part2),
                  AssetController.GetAsset<Material>("M_IE_Spotlight", Enumerators.EModAssetBundlePart.Part2)
            };
            OverhaulPostProcessBehaviour.AddPostProcessEffects(camera, array);
        }

        private static void patchCameraSettings(Camera camera)
        {
            if (camera == null)
            {
                return;
            }

            camera.renderingPath = RenderingPath.DeferredShading;
        }

        private static void onModDisabled()
        {
            IsWorking = false;
            foreach (OverhaulPostProcessBehaviour b in UICamera.GetComponents<OverhaulPostProcessBehaviour>())
            {
                Object.Destroy(b);
            }
        }
    }
}
