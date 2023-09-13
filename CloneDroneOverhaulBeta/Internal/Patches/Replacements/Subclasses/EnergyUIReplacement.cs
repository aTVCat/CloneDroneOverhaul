using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Patches
{
    public class EnergyUIReplacement : ReplacementBase
    {
        [OverhaulSetting(OverhaulSettingConstants.Categories.USER_INTERFACE, OverhaulSettingConstants.Sections.ENHANCEMENTS, "New energy bar design")]
        public static bool PatchHUD = true;

        private (Vector3, Vector3) m_Positions;

        private Transform m_BG;

        private Image m_BarBG;
        private Color m_BarBGInitColor;
        private readonly Color m_BarBGPatchedColor = new Color(0, 0, 0.2f, 0.25f);
        private (Vector3, Vector3) m_BarBGPositions;
        private (Vector3, Vector3) m_BarBGScale = (new Vector3(1, 1, 1), new Vector3(1.04f, 1.3f, 1));

        private Image m_Glow;
        private Color m_GlowColor;
        private readonly Color m_GlowPatchedColor = new Color(0.1f, 0.4f, 1f, 0.9f);
        private (Vector3, Vector3) m_GlowScale = (new Vector3(1, 1, 1), new Vector3(1.05f, 1, 1));

        private Transform m_CantJumpBG;
        private (Vector3, Vector3) m_CantJumpBGScale = (Vector3.one, Vector3.zero);

        private bool m_HasAddedListeners;

        private EnergyUI m_EnergyUI;
        public EnergyUI EnergyUI
        {
            get
            {
                if (!m_EnergyUI)
                {
                    m_EnergyUI = GameUIRoot.Instance.EnergyUI;
                }
                return m_EnergyUI;
            }
        }

        public override void Replace()
        {
            base.Replace();

            if (!m_HasAddedListeners)
            {
                OverhaulEvents.AddEventListener(OverhaulSettingsManager_Old.SETTING_VALUE_UPDATED_EVENT, RefreshPatch);
                m_HasAddedListeners = true;
            }

            RectTransform transform = EnergyUI.transform as RectTransform;
            m_Positions.Item1 = transform.anchoredPosition;
            m_Positions.Item2 = transform.anchoredPosition + new Vector2(0, 13);

            m_BG = transform.Find("FrameBG");
            m_BarBG = transform.Find("BarBG").GetComponent<Image>();
            m_BarBGInitColor = m_BarBG.color;
            m_BarBGPositions.Item1 = m_BarBG.transform.localPosition;
            m_BarBGPositions.Item2 = new Vector3(0, 9, 0);
            m_Glow = transform.Find("GlowFill").GetComponent<Image>();
            m_GlowColor = m_Glow.color;
            m_CantJumpBG = transform.Find("CantJumpBG");

            _ = EnergyUI.gameObject.AddComponent<EnergyUIReplacementBehaviour>();
            SuccessfullyPatched = true;

            PatchEnergyUI(!PatchHUD);
        }

        public override void Cancel()
        {
            base.Cancel();
            PatchEnergyUI(true);
        }

        public void RefreshPatch()
        {
            PatchEnergyUI(!PatchHUD);
        }

        public void PatchEnergyUI(bool recover)
        {
            if (!SuccessfullyPatched)
                return;

            (EnergyUI.transform as RectTransform).anchoredPosition = recover ? m_Positions.Item1 : m_Positions.Item2;
            m_BG.gameObject.SetActive(recover);
            m_BarBG.color = recover ? m_BarBGInitColor : m_BarBGPatchedColor;
            m_BarBG.transform.localPosition = recover ? m_BarBGPositions.Item1 : m_BarBGPositions.Item2;
            m_BarBG.transform.localScale = recover ? m_BarBGScale.Item1 : m_BarBGScale.Item2;
            m_Glow.color = recover ? m_GlowColor : m_GlowPatchedColor;
            m_Glow.transform.localScale = recover ? m_GlowScale.Item1 : m_GlowScale.Item2;
            m_CantJumpBG.localScale = recover ? m_CantJumpBGScale.Item1 : m_CantJumpBGScale.Item2;
        }

        public static void RefreshPatchStatic() => GetReplacement<EnergyUIReplacement>()?.RefreshPatch();
    }
}
