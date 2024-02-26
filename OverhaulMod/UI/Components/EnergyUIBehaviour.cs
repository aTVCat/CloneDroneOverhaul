using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.UI
{
    public class EnergyUIBehaviour : MonoBehaviour
    {
        private EnergyUI m_energyUI;
        private CanvasGroup m_canvasGroup;

        private void Start()
        {
            m_energyUI = ModCache.gameUIRoot.EnergyUI;
            m_canvasGroup = m_energyUI.gameObject.AddComponent<CanvasGroup>();
        }

        private void Update()
        {
            EnergyUI energyUI = m_energyUI;
            CanvasGroup canvasGroup = m_canvasGroup;
            if (energyUI && canvasGroup)
            {
                float amount = energyUI._lastAmount;
                float maxAmount = energyUI._lastRenderedMaxEnergy;
                float deltaTime = (amount >= maxAmount && !energyUI.InsufficientEnergyText.gameObject.activeSelf ? -1f : 1f) * 2.5f * Time.unscaledDeltaTime;
                float targetAlpha = canvasGroup.alpha = Mathf.Clamp(canvasGroup.alpha + deltaTime, 0.1f, 1f);
                canvasGroup.alpha = targetAlpha;
            }
        }
    }
}
