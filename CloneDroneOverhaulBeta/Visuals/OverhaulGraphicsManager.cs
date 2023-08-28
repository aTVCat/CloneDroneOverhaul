using CDOverhaul.Gameplay;
using System.Linq;
using UnityEngine;

namespace CDOverhaul.Visuals
{
    public class OverhaulGraphicsManager : OverhaulManager<OverhaulGraphicsManager>
    {
        private bool m_HasInstantiatedEffects;

        public AmplifyOclusionImageEffect amplifyOclusion
        {
            get;
            private set;
        }

        public BloomOverhaulImageEffect bloomOverhaul
        {
            get;
            private set;
        }

        public AmplifyColorOverhaulImageEffect amplifyColorOverhaul
        {
            get;
            private set;
        }

        public ChromaticAberrationImageEffect chromaticAberration
        {
            get;
            private set;
        }

        public BlurEdgesImageEffect blurEdges
        {
            get;
            private set;
        }

        public VignetteImageEffect vignette
        {
            get;
            private set;
        }

        public CameraTiltEffect tilt
        {
            get;
            private set;
        }

        public CameraFOVEffect fov
        {
            get;
            private set;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void OnDisposed()
        {
            amplifyOclusion?.Dispose();
            bloomOverhaul?.Dispose();
            amplifyColorOverhaul?.Dispose();
            chromaticAberration?.Dispose();
            vignette?.Dispose();
            blurEdges?.Dispose();
            tilt?.Dispose();
            fov?.Dispose();
            m_HasInstantiatedEffects = false;

            base.OnDisposed();
            DestroyGameObject();
        }

        public override void OnSceneReloaded()
        {
            base.OnSceneReloaded();
            DelegateScheduler.Instance.Schedule(PatchMainCamera, 0.1f);
        }

        protected override void OnAssetsLoaded()
        {
            instantiateEffects();
        }

        public override void AddListeners()
        {
            base.AddListeners();
            OverhaulEventsController.AddEventListener<Camera>(OverhaulCameraManager.CAMERA_CHANGED_EVENT, PatchCamera);
            OverhaulEventsController.AddEventListener(OverhaulSettingsController.SettingChangedEventString, PatchMainCamera);
        }

        public override void RemoveListeners()
        {
            base.RemoveListeners();
            OverhaulEventsController.RemoveEventListener<Camera>(OverhaulCameraManager.CAMERA_CHANGED_EVENT, PatchCamera);
            OverhaulEventsController.RemoveEventListener(OverhaulSettingsController.SettingChangedEventString, PatchMainCamera);
        }

        private void instantiateEffects()
        {
            if (IsDisposedOrDestroyed())
                return;

            amplifyOclusion = base.gameObject.AddComponent<AmplifyOclusionImageEffect>();
            bloomOverhaul = base.gameObject.AddComponent<BloomOverhaulImageEffect>();
            amplifyColorOverhaul = base.gameObject.AddComponent<AmplifyColorOverhaulImageEffect>();
            chromaticAberration = base.gameObject.AddComponent<ChromaticAberrationImageEffect>();
            vignette = base.gameObject.AddComponent<VignetteImageEffect>();
            blurEdges = base.gameObject.AddComponent<BlurEdgesImageEffect>();
            tilt = base.gameObject.AddComponent<CameraTiltEffect>();
            fov = base.gameObject.AddComponent<CameraFOVEffect>();
            m_HasInstantiatedEffects = true;
            PatchMainCamera();
        }

        public void PatchMainCamera()
        {
            if (IsDisposedOrDestroyed())
                return;

            PatchCamera(OverhaulCameraManager.reference?.mainCamera);
        }

        public void PatchCamera(Camera camera)
        {
            if (!m_HasInstantiatedEffects || !camera || camera.orthographic || OverhaulCameraEffectBehaviour.IgnoredCameras.Contains(camera.gameObject.name))
                return;

            amplifyOclusion.PatchCamera(camera);
            bloomOverhaul.PatchCamera(camera);
            amplifyColorOverhaul.PatchCamera(camera);
            chromaticAberration.PatchCamera(camera);
            vignette.PatchCamera(camera);
            blurEdges.PatchCamera(camera);
            tilt.PatchCamera(camera);
            fov.PatchCamera(camera);
        }
    }
}
