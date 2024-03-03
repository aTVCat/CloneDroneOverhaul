using OverhaulMod.UI;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Patches.Addons
{
    internal class EnergyUIPatchBehaviour : GamePatchBehaviour
    {
        private (Vector3, Vector3) m_positions;

        private Transform m_bg;

        private Image m_barBG;
        private Color m_barBGInitColor;
        private readonly Color m_barBGPatchedColor = new Color(0, 0, 0.2f, 0.25f);
        private (Vector3, Vector3) m_barBGPositions;
        private (Vector3, Vector3) m_barBGScale = (new Vector3(1, 1, 1), new Vector3(1.04f, 1.3f, 1));

        private Image m_glow;
        private Color m_glowColor;
        private readonly Color m_glowPatchedColor = new Color(0.1f, 0.4f, 1f, 0.9f);
        private (Vector3, Vector3) m_glowScale = (new Vector3(1, 1, 1), new Vector3(1.05f, 1, 1));

        private Transform m_cantJumpBG;
        private (Vector3, Vector3) m_cantJumpBGScale = (Vector3.one, Vector3.zero);

        private EnergyUI m_energyUI;
        public EnergyUI energyUI
        {
            get
            {
                if (!m_energyUI)
                {
                    m_energyUI = ModCache.gameUIRoot.EnergyUI;
                }
                return m_energyUI;
            }
        }

        public override void Patch()
        {
            RectTransform transform = energyUI.transform as RectTransform;
            m_positions.Item1 = transform.anchoredPosition;
            m_positions.Item2 = Vector2.up * 10f;

            m_bg = transform.Find("FrameBG");
            m_barBG = transform.Find("BarBG").GetComponent<Image>();
            m_barBGInitColor = m_barBG.color;
            m_barBGPositions.Item1 = m_barBG.transform.localPosition;
            m_barBGPositions.Item2 = new Vector3(0, 9, 0);
            m_glow = transform.Find("GlowFill").GetComponent<Image>();
            m_glowColor = m_glow.color;
            m_cantJumpBG = transform.Find("CantJumpBG");

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

            (component.transform as RectTransform).anchoredPosition = vanilla ? m_positions.Item1 : m_positions.Item2;
            m_bg.gameObject.SetActive(vanilla);
            m_barBG.color = vanilla ? m_barBGInitColor : m_barBGPatchedColor;
            m_barBG.transform.localPosition = vanilla ? m_barBGPositions.Item1 : m_barBGPositions.Item2;
            m_barBG.transform.localScale = vanilla ? m_barBGScale.Item1 : m_barBGScale.Item2;
            m_glow.color = vanilla ? m_glowColor : m_glowPatchedColor;
            m_glow.transform.localScale = vanilla ? m_glowScale.Item1 : m_glowScale.Item2;
            m_cantJumpBG.localScale = vanilla ? m_cantJumpBGScale.Item1 : m_cantJumpBGScale.Item2;
        }
    }
}
