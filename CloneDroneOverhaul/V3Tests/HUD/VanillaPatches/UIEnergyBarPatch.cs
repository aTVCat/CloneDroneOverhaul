using UnityEngine;
using UnityEngine.UI;

namespace CloneDroneOverhaul.V3Tests.HUD
{
    public class UIEnergyBarPatch : V3_ModHUDBase
    {
        private EnergyUI _energyUI;

        private (Vector3, Vector3) _positions;

        private Transform _bg;

        private Image _barBG;
        private Color _barBGInitColor;
        private readonly Color _barBGPatchedColor = new Color(0, 0, 0.2f, 0.25f);
        private (Vector3, Vector3) _barBGPositions;
        private (Vector3, Vector3) _barBGScale = (new Vector3(1, 1, 1), new Vector3(1.04f, 1.3f, 1));

        private Image _glow;
        private Color _glowColor;
        private readonly Color _glowPatchedColor = new Color(0.1f, 0.4f, 1f, 0.9f);
        private (Vector3, Vector3) _glowScale = (new Vector3(1, 1, 1), new Vector3(1.05f, 1, 1));

        private Transform _cantJumpBG;
        private (Vector3, Vector3) _cantJumpBGScale = (Vector3.one, Vector3.zero);

        private bool _hasInitialized;

        private void Start()
        {
            _energyUI = GameUIRoot.Instance.EnergyUI;
            if (_energyUI == null)
            {
                return;
            }

            RectTransform transform = _energyUI.transform as RectTransform;
            _positions.Item1 = transform.anchoredPosition;
            _positions.Item2 = transform.anchoredPosition + new Vector2(0, 13);

            _bg = transform.Find("FrameBG");
            _barBG = transform.Find("BarBG").GetComponent<Image>();
            _barBGInitColor = _barBG.color;
            _barBGPositions.Item1 = _barBG.transform.localPosition;
            _barBGPositions.Item2 = new Vector3(0, 9, 0);
            _glow = transform.Find("GlowFill").GetComponent<Image>();
            _glowColor = _glow.color;
            _cantJumpBG = transform.Find("CantJumpBG");

            _hasInitialized = true;
        }

        public override void OnSettingRefreshed(in string settingName, in object value)
        {
            if (settingName == "Patches.GUI.Energy bar")
            {
                bool val = !(bool)value;
                if (!_hasInitialized)
                {
                    DelegateScheduler.Instance.Schedule(delegate
                    {
                        PatchEnergyUI(val);
                    }, 0.1f);
                    return;
                }
                PatchEnergyUI(val);
            }
        }

        public void PatchEnergyUI(in bool recover)
        {
            if (!_hasInitialized)
            {
                return;
            }

            (_energyUI.transform as RectTransform).anchoredPosition = recover ? _positions.Item1 : _positions.Item2;
            _bg.gameObject.SetActive(recover);
            _barBG.color = recover ? _barBGInitColor : _barBGPatchedColor;
            _barBG.transform.localPosition = recover ? _barBGPositions.Item1 : _barBGPositions.Item2;
            _barBG.transform.localScale = recover ? _barBGScale.Item1 : _barBGScale.Item2;
            _glow.color = recover ? _glowColor : _glowPatchedColor;
            _glow.transform.localScale = recover ? _glowScale.Item1 : _glowScale.Item2;
            _cantJumpBG.localScale = recover ? _cantJumpBGScale.Item1 : _cantJumpBGScale.Item2;
        }
    }
}
