using ModLibrary;
using UnityEngine;

namespace CDOverhaul.Patches
{
    public class EnergyUIReplacementBehaviour : MonoBehaviour
    {
        [OverhaulSetting("Game interface.Gameplay.Hide energy bar when full", true, false, "Energy bar will become transparent when you're fully charged", "Game interface.Gameplay.New energy bar design")]
        public static bool HideEnergyUIWhenFull;

        private EnergyUI m_EnergyUI;
        private CanvasGroup m_CanvasGroup;

        private void Start()
        {
            m_EnergyUI = GameUIRoot.Instance.EnergyUI;
            m_CanvasGroup = m_EnergyUI.gameObject.AddComponent<CanvasGroup>();
        }

        private void Update()
        {
            if (m_EnergyUI && m_CanvasGroup)
            {
                float amount = m_EnergyUI._lastAmount;
                float maxAmount = m_EnergyUI._lastRenderedMaxEnergy;
                float deltaTime = (amount >= maxAmount ? -1f : 1f) * 2.5f * Time.deltaTime;
                float targetAlpha = HideEnergyUIWhenFull && EnergyUIReplacement.PatchHUD ? m_CanvasGroup.alpha = Mathf.Clamp(m_CanvasGroup.alpha + (deltaTime), 0.1f, 1f) : 1f;
                m_CanvasGroup.alpha = targetAlpha;
            }
        }
    }
}