using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Visuals.ImageEffects
{
    public class OverhaulColorBlindness : MonoBehaviour
    {
        // 0 - Normal
        // 1 - Protanopia
        // 2 - Deuteranopia
        // 3 - Tritanopia

        private int _type;
        public int type
        {
            get
            {
                return _type;
            }
            set
            {
                if (_material)
                    _material.SetInt("type", value);

                _type = value;
            }
        }

        public Shader _shader;

        private Material _material;

        private bool _supported;

        private void Start()
        {
            if (!SystemInfo.supportsImageEffects || SystemInfo.graphicsShaderLevel < 30)
                return;

            _shader = ModResources.Shader(ModAssetBundles.IMAGE_EFFECTS, "Colorblind");
            if (!_shader || !_shader.isSupported)
                return;

            _material = new Material(_shader);
            _material.SetInt("type", type);

            if (!_material || _material.passCount != 1)
                return;

            _supported = true;
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
