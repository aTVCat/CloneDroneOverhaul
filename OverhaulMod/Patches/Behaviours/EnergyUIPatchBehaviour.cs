using OverhaulMod.UI;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Patches.Addons
{
    internal class EnergyUIPatchBehaviour : GamePatchBehaviour
    {
        private GameObject m_bg;
        private Transform m_cantJumpBG;

        private Image m_barBGImage;
        private Color m_barBGVanillaColor;

        private Image m_glowImage;
        private Color m_glowVanillaColor;

        private EnergyUI m_energyUI;
        public EnergyUI energyUI
        {
            get
            {
                if (!m_energyUI)
                {
                    m_energyUI = ModCache.gameUIRoot?.EnergyUI;
                }
                return m_energyUI;
            }
        }

        public override void Patch()
        {
            EnergyUI eui = energyUI;
            Transform transform = eui.transform;
            m_bg = transform.FindChildRecursive("FrameBG").gameObject;
            m_barBGImage = transform.FindChildRecursive("BarBG").GetComponent<Image>();
            m_barBGVanillaColor = m_barBGImage.color;
            m_glowImage = transform.FindChildRecursive("GlowFill").GetComponent<Image>();
            m_glowVanillaColor = m_glowImage.color;
            m_cantJumpBG = transform.FindChildRecursive("CantJumpBG");

            if (!transform.GetComponent<EnergyUIBehaviour>())
                _ = transform.gameObject.AddComponent<EnergyUIBehaviour>();

            PatchEnergyUI(false);
        }

        public override void UnPatch()
        {
            PatchEnergyUI(true);
        }

        public void PatchEnergyUI(bool vanilla)
        {
            EnergyUI component = energyUI;
            if (!component)
                return;

            GameObject errorText = component.InsufficientEnergyText?.gameObject;
            if (errorText)
            {
                SoftShadow softShadow = errorText.GetComponent<SoftShadow>();
                if (softShadow && vanilla)
                    Destroy(softShadow);
                else if (!softShadow && !vanilla)
                {
                    softShadow = errorText.AddComponent<SoftShadow>();
                    softShadow.effectDistance = Vector2.one * -2f;
                }
            }

            if (component.transform is RectTransform rt)
                rt.anchoredPosition = vanilla ? Vector3.zero : Vector3.up * 10f;

            if (m_bg)
                m_bg.SetActive(vanilla);

            Image image = m_barBGImage;
            if (image)
            {
                image.color = vanilla ? m_barBGVanillaColor : new Color(0, 0, 0.2f, 0.25f);

                Transform transform = image.transform;
                if (transform)
                    transform.localScale = vanilla ? Vector3.one : new Vector3(1.04f, 1.3f, 1f);
            }

            if (m_glowImage)
                m_glowImage.color = vanilla ? m_glowVanillaColor : new Color(0.1f, 0.4f, 1f, 0.9f);

            if (m_cantJumpBG)
                m_cantJumpBG.localScale = vanilla ? Vector3.one : Vector3.zero;
        }
    }
}
