using CloneDroneOverhaul;
using UnityEngine;

namespace CloneDroneOverhaul.Modules
{
    public class AdvancedPhotoModeManager : ModuleBase
    {
        private bool _usingSlowMo;
        public bool IsSlowMoEnabled => PhotoManager.Instance.IsInPhotoMode() && _usingSlowMo;

        public override void Start()
        {
        }

        public override void OnNewFrame()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                if (PhotoManager.Instance.IsInPhotoMode())
                {
                    if (IsSlowMoEnabled)
                    {
                        Singleton<TimeManager>.Instance.RestoreOverridePausedTimeScale();
                        _usingSlowMo = false;
                    }
                    else
                    {
                        Singleton<TimeManager>.Instance.SetOverridePausedTimeScale(0.1f);
                        _usingSlowMo = true;
                    }
                }
            }
        }

        private Camera GetCamera()
        {
            return PlayerCameraManager.Instance.GetMainCamera();
        }
    }
}



namespace AdvancedPhotoModeEffects
{
    public class AdvancedPhotoModeCameraEffect : MonoBehaviour
    {
    }

    public class BWEffect : AdvancedPhotoModeCameraEffect
    {
        public float intensity;
        private Material material;

        private void Awake()
        {
            material = new Material(OverhaulCacheAndGarbageController.GetCached<Shader>("Shader_BWDiffuse"));
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (intensity == 0)
            {
                Graphics.Blit(source, destination);
                return;
            }

            material.SetFloat("_bwBlend", intensity);
            Graphics.Blit(source, destination, material);
        }
    }

    public class ShaderEffect_BleedingColors : AdvancedPhotoModeCameraEffect
    {
        public float intensity = 3;
        public float shift = 0.5f;
        private Material material;

        private void Awake()
        {
            material = new Material(OverhaulCacheAndGarbageController.GetCached<Shader>("Shader_BleedingColors"));
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            material.SetFloat("_Intensity", intensity);
            material.SetFloat("_ValueX", shift);
            Graphics.Blit(source, destination, material);
        }
    }


    public class ShaderEffect_CRT : AdvancedPhotoModeCameraEffect
    {
        public float scanlineIntensity = 100;
        public int scanlineWidth = 1;
        private Material material_Displacement;
        private Material material_Scanlines;

        private void Awake()
        {
            material_Scanlines = new Material(OverhaulCacheAndGarbageController.GetCached<Shader>("Shader_Scanlines"));
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            material_Scanlines.SetFloat("_Intensity", scanlineIntensity * 0.01f);
            material_Scanlines.SetFloat("_ValueX", scanlineWidth);

            Graphics.Blit(source, destination, material_Scanlines);
        }
    }

    public class ShaderEffect_Tint : AdvancedPhotoModeCameraEffect
    {
        public float y = 1;
        public float u = 1;
        public float v = 1;
        private Material material;

        private void Awake()
        {
            material = new Material(OverhaulCacheAndGarbageController.GetCached<Shader>("Shader_Tint"));
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {

            material.SetFloat("_ValueX", y);
            material.SetFloat("_ValueY", u);
            material.SetFloat("_ValueZ", v);

            Graphics.Blit(source, destination, material);
        }
    }

}
