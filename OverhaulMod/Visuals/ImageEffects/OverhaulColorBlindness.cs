using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OverhaulMod.Visuals.ImageEffects
{
    public class OverhaulColorBlindness : MonoBehaviour
    {
        // 0 - Normal
        // 1 - Protanopia
        // 2 - Deuteranopia
        // 3 - Tritanopia

        private int m_type;
        public int type
        {
            get
            {
                return m_type;
            }
            set
            {
                if(m_colorblindMaterial)
                    m_colorblindMaterial.SetInt("type", value);

                m_type = value;
            }
        }

        public Shader m_colorblindShader;

        private Material m_colorblindMaterial;

        private bool m_supported;

        private void Start()
        {
            if (!m_colorblindShader)
                m_colorblindShader = ModResources.Shader(AssetBundleConstants.IMAGE_EFFECTS, "Colorblind");

            if (!m_colorblindShader)
            {
                m_supported = false;
                enabled = false;
                return;
            }

            if (!SystemInfo.supportsImageEffects || SystemInfo.graphicsShaderLevel < 30)
            {
                m_supported = false;
                enabled = false;
                return;
            }

            EnsureMaterials();

            if (!m_colorblindMaterial || m_colorblindMaterial.passCount != 1)
            {
                m_supported = false;
                enabled = false;
                return;
            }

            m_supported = true;
        }

        private static Material CreateMaterial(Shader shader)
        {
            if (!shader)
                return null;

            Material m = new Material(shader);
            m.hideFlags = HideFlags.HideAndDontSave;
            return m;
        }

        private static void DestroyMaterial(Material mat)
        {
            if (mat)
            {
                DestroyImmediate(mat);
                mat = null;
            }
        }

        private void EnsureMaterials()
        {
            if (!m_colorblindMaterial && m_colorblindShader.isSupported)
            {
                m_colorblindMaterial = CreateMaterial(m_colorblindShader);
                m_colorblindMaterial.SetInt("type", type);
            }
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (!m_supported || !m_colorblindShader.isSupported)
            {
                enabled = false;
                return;
            }

            EnsureMaterials();
            Graphics.Blit(source, destination, m_colorblindMaterial, 0);
        }
    }
}
