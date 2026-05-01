using OverhaulMod.Engine;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class EnergyBarBehaviour : MonoBehaviour
    {
        [ModSetting(ModSettingIDs.ENERGY_UI_FADE_OUT_IF_FULL, true)]
        public static bool EnableBehaviour;

        [ModSetting(ModSettingIDs.ENERGY_UI_FADE_OUT_INTENSITY, 0.9f)]
        public static float FadeOutIntensity;

        public EnergyUI EnergyUI;

        public Image GlowFill;

        public RectTransform GlowFillTransform;

        public bool IsMountEnergyBar;

        private CanvasGroup _canvasGroup;

        private CharacterTracker _characterTracker;

        private void Start()
        {
            _canvasGroup = EnergyUI.gameObject.AddComponent<CanvasGroup>();
            _characterTracker = CharacterTracker.Instance;
        }

        private void Update()
        {
            EnergyUI energyUI = EnergyUI;
            CanvasGroup canvasGroup = _canvasGroup;
            if (energyUI && canvasGroup)
            {
                FirstPersonMover target = _characterTracker.GetPlayerRobot();
                if (IsMountEnergyBar)
                {
                    FirstPersonMover mount = target.GetCharacterWeAreRiding();
                    target = mount;
                }

                float amount = energyUI._lastAmount;
                float maxAmount = energyUI._lastRenderedMaxEnergy;
                bool shouldhighlightTheBar = IsMountEnergyBar || !EnableBehaviour || amount < maxAmount || energyUI.InsufficientEnergyText.gameObject.activeSelf || (target && target.IsRidingOtherCharacter());

                float deltaTime = (shouldhighlightTheBar ? 1f : -1f) * 2.5f * Time.unscaledDeltaTime;
                float targetAlpha = canvasGroup.alpha = Mathf.Clamp(canvasGroup.alpha + deltaTime, Mathf.Clamp01(1f - FadeOutIntensity), 1f);
                canvasGroup.alpha = targetAlpha;

                if (GlowFill)
                {
                    Color color = GlowFill.color;
                    color.a = Mathf.Clamp01(amount / 2f);
                    GlowFill.color = color;
                }

                if (GlowFillTransform)
                {
                    Vector2 offsetMin = GlowFillTransform.offsetMin;
                    offsetMin.x = Mathf.Lerp(-25f, -50f, amount / 4f);
                    GlowFillTransform.offsetMin = offsetMin;
                }
            }
        }
    }
}
