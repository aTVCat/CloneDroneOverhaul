using OverhaulMod.Engine;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class EnergyUIBehaviour : MonoBehaviour
    {
        [ModSetting(ModSettingsConstants.ENERGY_UI_FADE_OUT_IF_FULL, true)]
        public static bool EnableBehaviour;

        [ModSetting(ModSettingsConstants.ENERGY_UI_FADE_OUT_INTENSITY, 0.9f)]
        public static float FadeOutIntensity;

        public Image GlowFill;

        private EnergyUI m_energyUI;
        private CanvasGroup m_canvasGroup;

        private CharacterTracker m_characterTracker;

        private void Start()
        {
            m_energyUI = ModCache.gameUIRoot.EnergyUI;
            m_canvasGroup = m_energyUI.gameObject.AddComponent<CanvasGroup>();
            m_characterTracker = CharacterTracker.Instance;
        }

        private void Update()
        {
            EnergyUI energyUI = m_energyUI;
            CanvasGroup canvasGroup = m_canvasGroup;
            if (energyUI && canvasGroup)
            {
                FirstPersonMover player = m_characterTracker.GetPlayerRobot();

                float amount = energyUI._lastAmount;
                float maxAmount = energyUI._lastRenderedMaxEnergy;
                bool shouldhighlightTheBar = !EnableBehaviour || amount < maxAmount || energyUI.InsufficientEnergyText.gameObject.activeSelf || (player && player.IsRidingOtherCharacter());

                float deltaTime = (shouldhighlightTheBar ? 1f : -1f) * 2.5f * Time.unscaledDeltaTime;
                float targetAlpha = canvasGroup.alpha = Mathf.Clamp(canvasGroup.alpha + deltaTime, Mathf.Clamp01(1f - FadeOutIntensity), 1f);
                canvasGroup.alpha = targetAlpha;

                if (GlowFill)
                {
                    Color color = GlowFill.color;
                    color.a = Mathf.Clamp01(amount / 4f);
                    GlowFill.color = color;
                }
            }
        }
    }
}
