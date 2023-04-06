using ModLibrary;
using UnityEngine;

namespace CDOverhaul.Patches
{
    public class EnergyUIReplacementBehaviour : MonoBehaviour
    {
        [OverhaulSettingAttribute("Game interface.Gameplay.Hide energy bar when full", true, false, "Energy bar will become transparent when you're fully charged", null, null, "Game interface.Gameplay.New energy bar design")]
        public static bool HideEnergyUIWhenFull;

        private EnergyUI _energyUI;
        private CanvasGroup _canvasGroup;

        private bool _initialized;
        private float _targetAlpha;

        private void Start()
        {
            _energyUI = GameUIRoot.Instance.EnergyUI;
            _canvasGroup = _energyUI.gameObject.AddComponent<CanvasGroup>();
            _initialized = true;
        }

        private void Update()
        {
            if (_initialized && _energyUI && _canvasGroup)
            {
                float amount = _energyUI.GetPrivateField<float>("_lastAmount");
                float maxAmount = _energyUI.GetPrivateField<int>("_lastRenderedMaxEnergy");
                //_canvasGroup.alpha = amount >= maxAmount ? 0.2f : 1f; 

                if (HideEnergyUIWhenFull && EnergyUIReplacement.PatchHUD)
                {
                    _targetAlpha = amount >= maxAmount
                        ? Mathf.Clamp(_targetAlpha - (2f * Time.deltaTime), 0.1f, 1f)
                        : Mathf.Clamp(_targetAlpha + (3f * Time.deltaTime), 0.1f, 1f);
                    _canvasGroup.alpha = _targetAlpha;
                }
                else
                {
                    _canvasGroup.alpha = 1f;
                }
            }
        }
    }
}