using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Visuals.ImageEffects
{
    public class OverhaulChromaticAberration : MonoBehaviour
    {
        private float _power;
        public float power
        {
            get
            {
                return _power;
            }
            set
            {
                _power = value;
                if (_supported)
                    _material.SetFloat("_ChromaticAberration", 0.01f * value);
            }
        }

        private float _center;
        public float center
        {
            get
            {
                return _center;
            }
            set
            {
                _center = value;
                if (_supported)
                    _material.SetFloat("_Center", value);
            }
        }

        private Shader _shader;

        private Material _material;

        private bool _supported;

        private void Start()
        {
            _shader = ModResources.Shader(AssetBundleConstants.IMAGE_EFFECTS, "ChromaticAberration");
            if (!_shader || !_shader.isSupported) return;

            _material = new Material(_shader);
            _supported = true;
        }

        private void OnDestroy()
        {
            if (_material)
            {
                Destroy(_material);
                _material = null;
                _shader = null;
            }
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (_supported)
            {
                Graphics.Blit(source, destination, _material);
            }
            else
            {
                Graphics.Blit(source, destination);
            }
        }
    }
}
