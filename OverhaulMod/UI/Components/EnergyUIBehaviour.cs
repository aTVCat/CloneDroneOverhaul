using OverhaulMod.Engine;
using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.UI
{
    public class EnergyUIBehaviour : MonoBehaviour
    {
        [ModSetting(ModSettingsConstants.ENERGY_UI_FADE_OUT_IF_FULL, true)]
        public static bool EnableBehaviour;

        [ModSetting(ModSettingsConstants.ENERGY_UI_FADE_OUT_INTENSITY, 0.9f)]
        public static float FadeOutIntensity;

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
                float deltaTime = (EnableBehaviour && amount >= maxAmount && !energyUI.InsufficientEnergyText.gameObject.activeSelf ? -1f : 1f) * 2.5f * Time.unscaledDeltaTime;
                float targetAlpha = canvasGroup.alpha = Mathf.Clamp(canvasGroup.alpha + deltaTime, Mathf.Clamp01(1f - FadeOutIntensity), 1f);
                canvasGroup.alpha = targetAlpha;
            }
        }
    }
}
