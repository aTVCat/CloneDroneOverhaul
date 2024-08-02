using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Visuals.ImageEffects
{
    public class OverhaulChromaticAberration : MonoBehaviour
    {
        public float Power = 0.25f;

        public bool OnTheScreenEdges = true;

        private Shader m_shader;

        private Material m_material;

        private bool m_supported;

        public void Start()
        {
            m_shader = ModResources.Shader(AssetBundleConstants.IMAGE_EFFECTS, "ChromaticAberration");
            m_material = new Material(m_shader);

            if (!SystemInfo.supportsImageEffects)
            {
                m_supported = false;
                return;
            }

            if (!m_shader && !m_shader.isSupported)
            {
                m_supported = false;
                return;
            }
            m_supported = true;
        }

        public void OnRenderImage(RenderTexture inTexture, RenderTexture outTexture)
        {
            if (m_supported && m_shader)
            {
                m_material.SetFloat("_ChromaticAberration", 0.01f * Power);

                if (OnTheScreenEdges)
                    m_material.SetFloat("_Center", 0.5f);

                else
                    m_material.SetFloat("_Center", 0);

                Graphics.Blit(inTexture, outTexture, m_material);
            }
            else
            {
                Graphics.Blit(inTexture, outTexture);
            }
        }
    }
}
