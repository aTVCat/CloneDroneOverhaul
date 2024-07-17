using OverhaulMod.Engine;
using OverhaulMod.UI;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Patches.Behaviours
{
    internal class EnergyUIPatchBehaviour : GamePatchBehaviour
    {
        [ModSetting(ModSettingsConstants.ENERGY_UI_REWORK, true)]
        public static bool EnablePatch;

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
        }

        public override void UnPatch()
        {
            PatchEnergyUI(true);
        }

        public void RefreshPatch()
        {
            PatchEnergyUI(!EnablePatch);
        }

        public void PatchEnergyUI(bool vanilla)
        {
            EnergyUI component = energyUI;
            if (!component)
                return;

            Transform transform1 = component.transform;
            if (!m_bg)
                m_bg = transform1.FindChildRecursive("FrameBG").gameObject;

            if (!m_barBGImage)
                m_barBGImage = transform1.FindChildRecursive("BarBG").GetComponent<Image>();

            if (m_barBGVanillaColor == default)
                m_barBGVanillaColor = m_barBGImage.color;

            if (!m_glowImage)
                m_glowImage = transform1.FindChildRecursive("GlowFill").GetComponent<Image>();

            if (m_barBGVanillaColor == default)
                m_glowVanillaColor = m_glowImage.color;

            if (!m_cantJumpBG)
                m_cantJumpBG = transform1.FindChildRecursive("CantJumpBG");

            if (!transform1.GetComponent<EnergyUIBehaviour>())
                _ = transform1.gameObject.AddComponent<EnergyUIBehaviour>();

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
