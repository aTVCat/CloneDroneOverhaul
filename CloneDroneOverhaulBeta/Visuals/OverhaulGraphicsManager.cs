using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CDOverhaul.Gameplay;
using CDOverhaul.HUD;
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

        public override void Initialize()
        {
            base.Initialize();
            addListeners();
            OverhaulDebug.Log("GraphicsManager initialized", EDebugType.ModInit);
        }

        protected override void OnDisposed()
        {
            amplifyOclusion?.Dispose();
            bloomOverhaul?.Dispose();
            amplifyColorOverhaul?.Dispose();
            chromaticAberration?.Dispose();
            vignette?.Dispose();
            blurEdges?.Dispose();
            m_HasInstantiatedEffects = false;

            removeListeners();
            base.OnDisposed();
            DestroyGameObject();
        }

        public override void OnSceneReloaded()
        {
            DelegateScheduler.Instance.Schedule(PatchMainCamera, 0.1f);
            addListeners();
        }

        protected override void OnAssetsLoaded()
        {
            instantiateEffects();
        }

        private void addListeners()
        {
            OverhaulEventsController.AddEventListener<Camera>(OverhaulGameplayCoreController.MainCameraSwitchedEventString, PatchCamera);
            OverhaulEventsController.AddEventListener<Camera>(OverhaulGameplayCoreController.CurrentCameraSwitchedEventString, PatchCamera);
            OverhaulEventsController.AddEventListener(OverhaulSettingsController.SettingChangedEventString, PatchMainCamera);
        }

        private void removeListeners()
        {
            OverhaulEventsController.RemoveEventListener<Camera>(OverhaulGameplayCoreController.MainCameraSwitchedEventString, PatchCamera);
            OverhaulEventsController.RemoveEventListener<Camera>(OverhaulGameplayCoreController.CurrentCameraSwitchedEventString, PatchCamera);
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
            if (!m_HasInstantiatedEffects || !camera || camera.orthographic || OverhaulImageEffectBehaviour.IgnoredCameras.Contains(camera.gameObject.name))
                return;

            amplifyOclusion.PatchCamera(camera);
            bloomOverhaul.PatchCamera(camera);
            amplifyColorOverhaul.PatchCamera(camera);
            chromaticAberration.PatchCamera(camera);
            vignette.PatchCamera(camera);
            blurEdges.PatchCamera(camera);
        }
    }
}
